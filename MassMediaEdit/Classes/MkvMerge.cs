using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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
    using var tempFile = PathExtensions.GetTempFileToken($"$$$Temp$$$.{targetFile.Name}", targetFile.Directory?.FullName ?? ".");
    _Execute($"--output \"{tempFile.File.FullName}\" \"{sourceFile.FullName}\"", progressReporter);
    if (tempFile.File.Exists)
      tempFile.File.MoveTo(targetFile, true);
  }

  private static readonly Regex _PROGRESS_DETECTOR = new(@"(?<value>\d+(?:\.\d+)?)\s*%", RegexOptions.Compiled);
  private static void _Execute(string arguments, Action<float> progressReporter) {

    var executable = MkvMergeExecutable;
    if (executable is not { Exists: true })
      throw new NotSupportedException($"Please set path to MKVMerge first using {nameof(MkvMergeExecutable)} property.");

    var startInfo = new ProcessStartInfo(executable.FullName, arguments);
    var asyncResult =
        progressReporter == null
          ? startInfo.BeginRedirectedRun()
          : startInfo.BeginRedirectedRun(
            stdout => {
              var line = stdout.CurrentLine;
              var match = _PROGRESS_DETECTOR.Match(line);
              if (!match.Success)
                return;

              var valueText = match.Groups["value"].Value;
              if (float.TryParse(valueText, NumberStyles.Float, CultureInfo.InvariantCulture, out var progress))
                progressReporter(progress);
            }
          )
      ;

    var tuple = startInfo.EndRedirectedRun(asyncResult);
    var result = (ReturnCode)tuple.ExitCode;
    if (result is ReturnCode.Success or ReturnCode.Warning)
      return;

    var output = tuple.StandardOutput;
    var error = tuple.StandardError;

    throw new Exception($"Something went wrong during MKVMerge. Arguments: {arguments}") {
      Data = {
        { nameof(arguments), arguments },
        { "StandardOutput",output },
        { "StandardError",error },
        { "ExitCode",result },
      }
    };

  }
}