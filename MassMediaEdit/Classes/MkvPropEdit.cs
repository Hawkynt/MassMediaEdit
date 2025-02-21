using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace Classes;

internal static class MkvPropEdit {
  private enum ReturnCode {
    Success = 0,
    Warning = 1,
    Error = 2
  }

  public static FileInfo MkvPropEditExecutable { get; set; }

  public static void SetTitle(FileInfo file, string title)
    => _Execute(
      title == null
        ? $"\"{file.FullName}\" --edit info --delete \"title\""
        : $"\"{file.FullName}\" --edit info --set \"title={title.Replace("\"", "\"\"")}\""
    );

  public static void SetVideoName(FileInfo file, string name, byte videoStreamIndex = 0)
    => _Execute(
      name == null
        ? $"\"{file.FullName}\" --edit track:v{videoStreamIndex + 1} --delete \"name\""
        : $"\"{file.FullName}\" --edit track:v{videoStreamIndex + 1} --set \"name={name.Replace("\"","\"\"")}\""
    );

  public static void SetVideoStereoscopicMode(FileInfo file, int value, byte videoStreamIndex = 0)
    => _Execute(
      value == 0
        ? $"\"{file.FullName}\" --edit track:v{videoStreamIndex + 1} --delete \"stereo-mode\""
        : $"\"{file.FullName}\" --edit track:v{videoStreamIndex + 1} --set \"stereo-mode={value}\""
    );

  public static void SetAudioLanguage(FileInfo file, CultureInfo value, byte audioStreamIndex = 0)
    => _Execute(
      value == null
        ? $"\"{file.FullName}\" --edit track:a{audioStreamIndex + 1} --delete \"language\""
        : $"\"{file.FullName}\" --edit track:a{audioStreamIndex + 1} --set \"language={value.ThreeLetterISOLanguageName}\""
    );

  public static void SetAudioDefault(FileInfo file, byte audioStreamIndex, bool set)
    => _Execute(
      $"\"{file.FullName}\" --edit track:a{audioStreamIndex + 1} --set \"flag-default={(set ? "1" : "0")}\""
    );

  private static void _Execute(string arguments) {

    var executable = MkvPropEditExecutable;
    if (executable is not { Exists: true })
      throw new NotSupportedException($"Please set path to MKVPropEdit first using {nameof(MkvPropEditExecutable)} property.");


    using var process = new Process { StartInfo = new ProcessStartInfo(executable.FullName, arguments) { CreateNoWindow = true, WindowStyle = ProcessWindowStyle.Hidden, RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false } };
    process.Start();
    process.WaitForExit();
    
    var result = (ReturnCode)process.ExitCode;
    if (result is ReturnCode.Success or ReturnCode.Warning)
      return;

    var output = process.StandardOutput.ReadToEnd();
    var error = process.StandardError.ReadToEnd();

    var regex = new Regex(@"Error\s*:\s*(?<error>.*?)\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
    var errorMessage = $"Something went wrong during MKVPropEdit. Arguments: {arguments}";
    var match = regex.Match(output);
    if (match.Success)
      errorMessage = match.Groups["error"].Value;
    else {
      match = regex.Match(error);
      if (match.Success)
        errorMessage = match.Groups["error"].Value;
    }

    throw new Exception(errorMessage) {
      Data = {
        { nameof(arguments), arguments },
        { nameof(output),output },
        { nameof(error),error }
      }
    };
  }


}