using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("MassMediaEdit.Tests")]

namespace Classes;

public class MediaFile {

  public static FileInfo MediaInfoExecutable { get; set; }

  #region fields/props

  public FileInfo File { get; }

  public GeneralStream GeneralStream => this._Streams.OfType<GeneralStream>().FirstOrDefault();
  public IEnumerable<AudioStream> AudioStreams => this._Streams.OfType<AudioStream>();
  public IEnumerable<VideoStream> VideoStreams => this._Streams.OfType<VideoStream>();

  private MediaStream[] _streams;
  private MediaStream[] _Streams => this._streams ??= _GetStreams(_GetSections(this._GatherCommandlineInfo())).ToArray();

  #endregion

  private MediaFile(FileInfo file) {
    this.File = file;
  }
  
  /// <summary>
  /// Internal constructor for testing purposes that accepts pre-parsed streams.
  /// </summary>
  internal MediaFile(FileInfo file, MediaStream[] streams) {
    this.File = file;
    this._streams = streams;
  }

  private string[] _GatherCommandlineInfo() {
    var mediaInfoExecutable = MediaInfoExecutable;
    if (mediaInfoExecutable is not { Exists: true })
      throw new NotSupportedException($"Please set path to Media Info CLI first using {nameof(MediaInfoExecutable)} property.");

    var procStart = new ProcessStartInfo(mediaInfoExecutable.FullName, $"-full \"{this.File.FullName}\"") {
      RedirectStandardOutput = true,
      CreateNoWindow = true,
      UseShellExecute = false,
      StandardOutputEncoding = Encoding.UTF8,
    };
    using var process = Process.Start(procStart);
    return process.StandardOutput.ReadToEnd().Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries);
  }

  #region statics

  internal static IEnumerable<MediaStream> _GetStreams(IEnumerable<(string Name, SectionDictionary Values)> sections) {
    foreach (var (name, values) in sections) {
      switch ((name + " ").Split([' '], 2)[0]) {
        case "General":
          yield return new GeneralStream(values);
          break;
        case "Audio":
          yield return new AudioStream(values);
          break;
        case "Video":
          yield return new VideoStream(values);
          break;
      }
    }
  }

  internal static IEnumerable<(string Name, SectionDictionary Values)> _GetSections(IEnumerable<string> lines) {
    foreach (var (name, sectionLines) in _ParseSections(lines)) {
      var values = new SectionDictionary();
      foreach (var kvp in sectionLines) {
        var parts = kvp.Split([':'], 2);
        values.Add(parts[0].TrimEnd(), parts[1].TrimStart());
      }
      yield return (name, values);
    }

    yield break;

    static IEnumerable<(string Name, string[] Lines)> _ParseSections(IEnumerable<string> lines) {
      string currentSection = null;
      List<string> currentLines = [];
      foreach (var line in lines) {
        var parts = line.Split([':'], 2);
        if (parts.Length < 2) {
          if (currentSection != null)
            yield return (currentSection, currentLines.ToArray());

          currentSection = parts[0];
          currentLines.Clear();
        } else
          currentLines.Add(line);
      }

      if (currentSection != null)
        yield return (currentSection, currentLines.ToArray());
    }
  }
  
  public static MediaFile FromFile(FileInfo file) => new(file);

  /// <summary>
  /// Process multiple files in a single MediaInfo call for efficiency.
  /// Falls back to one-by-one processing for files that MediaInfo skipped or failed.
  /// </summary>
  public static IEnumerable<MediaFile> FromFiles(IEnumerable<FileInfo> files) {
    var fileList = files.ToList();
    if (fileList.Count == 0)
      yield break;

    // Try batch processing
    Dictionary<string, MediaFile>? batchResults = null;
    try {
      batchResults = _FromFilesBatch(fileList);
    } catch {
      // Batch completely failed - fall through to one-by-one for all files
    }

    // Return results, falling back to individual processing for missing files
    foreach (var file in fileList) {
      var key = file.FullName;

      // Check if batch processing found this file
      if (batchResults is not null && batchResults.TryGetValue(key, out var batchResult)) {
        yield return batchResult;
        continue;
      }

      // File not in batch output - process individually
      MediaFile? result = null;
      try {
        result = FromFile(file);
        // Force stream evaluation to catch errors
        _ = result._Streams;
      } catch {
        // Skip this file - it failed
        continue;
      }
      yield return result;
    }
  }

  private static Dictionary<string, MediaFile> _FromFilesBatch(IReadOnlyList<FileInfo> files) {
    var mediaInfoExecutable = MediaInfoExecutable;
    if (mediaInfoExecutable is not { Exists: true })
      throw new NotSupportedException($"Please set path to Media Info CLI first using {nameof(MediaInfoExecutable)} property.");

    const int maxCommandLineLength = 8000; // Windows limit is ~32k, stay safe
    var batches = _SplitIntoBatches(files, maxCommandLineLength);
    var results = new Dictionary<string, MediaFile>(StringComparer.OrdinalIgnoreCase);

    foreach (var batch in batches) {
      var args = "-full " + string.Join(" ", batch.Select(static f => $"\"{f.FullName}\""));
      var procStart = new ProcessStartInfo(mediaInfoExecutable.FullName, args) {
        RedirectStandardOutput = true,
        CreateNoWindow = true,
        UseShellExecute = false,
        StandardOutputEncoding = Encoding.UTF8,
      };

      using var process = Process.Start(procStart);
      var output = process!.StandardOutput.ReadToEnd();
      var lines = output.Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries);

      // Split output by file - each file starts with a "General" section
      var fileSections = _SplitByFile(lines);

      // Match each section to its file via "Complete name" in General section
      foreach (var sectionLines in fileSections) {
        var sections = _GetSections(sectionLines).ToList();
        var generalSection = sections.FirstOrDefault(s => s.Name == "General");

        if (generalSection.Values is null)
          continue;

        // Get the file path from "Complete name" field
        var completeName = generalSection.Values.GetValueOrDefault("Complete name");
        if (string.IsNullOrEmpty(completeName))
          continue;

        var streams = _GetStreams(sections).ToArray();
        var fileInfo = new FileInfo(completeName);
        results[completeName] = new MediaFile(fileInfo, streams);
      }
    }

    return results;
  }

  private static IEnumerable<IReadOnlyList<FileInfo>> _SplitIntoBatches(IReadOnlyList<FileInfo> files, int maxLength) {
    const int baseLength = 6; // "-full "
    var batch = new List<FileInfo>();
    var currentLength = baseLength;

    foreach (var file in files) {
      var pathLength = file.FullName.Length + 3; // quotes and space
      if (currentLength + pathLength > maxLength && batch.Count > 0) {
        yield return batch;
        batch = [];
        currentLength = baseLength;
      }
      batch.Add(file);
      currentLength += pathLength;
    }

    if (batch.Count > 0)
      yield return batch;
  }

  private static List<string[]> _SplitByFile(string[] lines) {
    var result = new List<string[]>();
    var currentFile = new List<string>();

    foreach (var line in lines) {
      // A line without ':' that equals "General" marks the start of a new file
      if (line == "General") {
        if (currentFile.Count > 0)
          result.Add(currentFile.ToArray());
        currentFile = [line];
      } else
        currentFile.Add(line);
    }

    if (currentFile.Count > 0)
      result.Add(currentFile.ToArray());

    return result;
  }

  #endregion

}