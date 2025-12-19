using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Classes;

internal static class MkvMerge {

  private enum ReturnCode {
    Success = 0,
    Warning = 1,
    Error = 2
  }

  public static FileInfo MkvMergeExecutable { get; set; }

  public static void ConvertToMkv(FileInfo sourceFile, FileInfo targetFile, Action<float> progressReporter = null) {
    // Refresh file info and validate source exists
    sourceFile.Refresh();
    if (!sourceFile.Exists)
      throw new FileNotFoundException($"Source file not found: {sourceFile.FullName}", sourceFile.FullName);

    using var tempFile = PathExtensions.GetTempFileToken($"$$$Temp$$$.{targetFile.Name}", targetFile.Directory?.FullName ?? ".");
    _Execute(sourceFile, progressReporter, "--output", tempFile.File.FullName, sourceFile.FullName);
    if (tempFile.File.Exists)
      tempFile.File.MoveTo(targetFile, true);
  }

  private static readonly Regex _PROGRESS_DETECTOR = new(@"(?<value>\d+(?:\.\d+)?)\s*%", RegexOptions.Compiled);
  private static readonly Regex _ERROR_DETECTOR = new(@"^\s*Error:(.*?)$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

  private static void _Execute(FileInfo sourceFile, Action<float> progressReporter, params string[] arguments) {

    var executable = MkvMergeExecutable;
    if (executable is not { Exists: true })
      throw new NotSupportedException($"Please set path to MKVMerge first using {nameof(MkvMergeExecutable)} property.");

    using var process = new Process {
      StartInfo = new ProcessStartInfo(executable.FullName) {
        CreateNoWindow = true,
        WindowStyle = ProcessWindowStyle.Hidden,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        StandardOutputEncoding = Encoding.UTF8,
        StandardErrorEncoding = Encoding.UTF8
      }
    };

    foreach (var arg in arguments)
      process.StartInfo.ArgumentList.Add(arg);

    var outputBuilder = new StringBuilder();
    var errorBuilder = new StringBuilder();

    process.OutputDataReceived += (_, e) => {
      if (e.Data is null)
        return;

      outputBuilder.AppendLine(e.Data);

      if (progressReporter is null)
        return;

      var match = _PROGRESS_DETECTOR.Match(e.Data);
      if (!match.Success)
        return;

      var valueText = match.Groups["value"].Value;
      if (float.TryParse(valueText, NumberStyles.Float, CultureInfo.InvariantCulture, out var progress))
        progressReporter(progress);
    };

    process.ErrorDataReceived += (_, e) => {
      if (e.Data is not null)
        errorBuilder.AppendLine(e.Data);
    };

    process.Start();
    process.BeginOutputReadLine();
    process.BeginErrorReadLine();
    process.WaitForExit();

    var result = (ReturnCode)process.ExitCode;
    if (result is ReturnCode.Success or ReturnCode.Warning)
      return;

    var output = outputBuilder.ToString();
    var error = errorBuilder.ToString();

    var message = _ERROR_DETECTOR.Match(error);
    if (!message.Success)
      message = _ERROR_DETECTOR.Match(output);

    // Check if file still exists (might have been moved/deleted during processing)
    sourceFile.Refresh();
    var fileExistsInfo = sourceFile.Exists 
      ? $"File exists, size: {sourceFile.Length} bytes" 
      : "File does NOT exist on disk";

    var exceptionMessage = message.Success
      ? $"MKVMerge-Error: {message.Groups[1].Value}"
      : $"Something went wrong during MKVMerge. Arguments: {string.Join(" ", arguments)}"
    ;

    throw new Exception(exceptionMessage) {
      Data = {
        { nameof(arguments), arguments },
        { "StandardOutput", output },
        { "StandardError", error },
        { "ExitCode", result },
        { "SourceFile", sourceFile.FullName },
        { "FileStatus", fileExistsInfo },
      }
    };

  }
}