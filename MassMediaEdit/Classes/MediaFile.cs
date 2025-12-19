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

  #endregion

}