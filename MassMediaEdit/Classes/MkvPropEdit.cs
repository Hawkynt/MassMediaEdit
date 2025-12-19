using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace Classes;

/// <summary>
/// Provides methods to edit MKV file properties using the mkvpropedit command-line tool.
/// </summary>
/// <remarks>
/// This class wraps the mkvpropedit executable from MKVToolNix to modify metadata
/// in Matroska (MKV) container files without remuxing.
/// </remarks>
internal static class MkvPropEdit {

  private enum ReturnCode {
    Success = 0,
    Warning = 1,
    Error = 2
  }

  /// <summary>
  /// Gets or sets the path to the mkvpropedit executable.
  /// </summary>
  /// <remarks>
  /// This property must be set to a valid, existing file path before calling any other methods.
  /// </remarks>
  /// <example>
  /// <code>
  /// MkvPropEdit.MkvPropEditExecutable = new FileInfo(@"C:\Tools\MKVToolNix\mkvpropedit.exe");
  /// </code>
  /// </example>
  public static FileInfo MkvPropEditExecutable { get; set; }

  /// <summary>
  /// Sets or removes the title metadata of an MKV file.
  /// </summary>
  /// <param name="file">The MKV file to modify.</param>
  /// <param name="title">The title to set, or <see langword="null"/>/whitespace to remove the existing title.</param>
  /// <exception cref="NotSupportedException">Thrown when <see cref="MkvPropEditExecutable"/> is not set or does not exist.</exception>
  /// <exception cref="Exception">Thrown when mkvpropedit returns an error.</exception>
  public static void SetTitle(FileInfo file, string title) {
    if (title.IsNullOrWhiteSpace())
      _Execute(file, "info", "--delete", "title");
    else
      _Execute(file, "info", "--set", $"title={title}");
  }

  /// <summary>
  /// Sets or removes the name of a video track in an MKV file.
  /// </summary>
  /// <param name="file">The MKV file to modify.</param>
  /// <param name="name">The name to set, or <see langword="null"/>/whitespace to remove the existing name.</param>
  /// <param name="videoStreamIndex">The zero-based index of the video stream to modify. Defaults to 0 (first video stream).</param>
  /// <exception cref="NotSupportedException">Thrown when <see cref="MkvPropEditExecutable"/> is not set or does not exist.</exception>
  /// <exception cref="Exception">Thrown when mkvpropedit returns an error.</exception>
  public static void SetVideoName(FileInfo file, string name, byte videoStreamIndex = 0) {
    var trackSelector = $"track:v{videoStreamIndex + 1}";
    if (name.IsNullOrWhiteSpace())
      _Execute(file, trackSelector, "--delete", "name");
    else
      _Execute(file, trackSelector, "--set", $"name={name}");
  }

  /// <summary>
  /// Sets or removes the stereoscopic 3D mode of a video track in an MKV file.
  /// </summary>
  /// <param name="file">The MKV file to modify.</param>
  /// <param name="value">
  /// The stereoscopic mode value to set. Common values include:
  /// <list type="bullet">
  ///   <item><description>0 - Mono (removes the stereoscopic flag)</description></item>
  ///   <item><description>1 - Side by side (left eye first)</description></item>
  ///   <item><description>2 - Top-bottom (right eye first)</description></item>
  ///   <item><description>3 - Top-bottom (left eye first)</description></item>
  ///   <item><description>11 - Side by side (right eye first)</description></item>
  /// </list>
  /// </param>
  /// <param name="videoStreamIndex">The zero-based index of the video stream to modify. Defaults to 0 (first video stream).</param>
  /// <exception cref="NotSupportedException">Thrown when <see cref="MkvPropEditExecutable"/> is not set or does not exist.</exception>
  /// <exception cref="Exception">Thrown when mkvpropedit returns an error.</exception>
  public static void SetVideoStereoscopicMode(FileInfo file, int value, byte videoStreamIndex = 0) {
    var trackSelector = $"track:v{videoStreamIndex + 1}";
    if (value == 0)
      _Execute(file, trackSelector, "--delete", "stereo-mode");
    else
      _Execute(file, trackSelector, "--set", $"stereo-mode={value}");
  }

  /// <summary>
  /// Sets or removes the language of an audio track in an MKV file.
  /// </summary>
  /// <param name="file">The MKV file to modify.</param>
  /// <param name="value">The culture representing the language to set, or <see langword="null"/> to remove the existing language.</param>
  /// <param name="audioStreamIndex">The zero-based index of the audio stream to modify. Defaults to 0 (first audio stream).</param>
  /// <remarks>
  /// The language is stored using the ISO 639-2 three-letter language code (e.g., "eng" for English, "deu" for German).
  /// </remarks>
  /// <exception cref="NotSupportedException">Thrown when <see cref="MkvPropEditExecutable"/> is not set or does not exist.</exception>
  /// <exception cref="Exception">Thrown when mkvpropedit returns an error.</exception>
  public static void SetAudioLanguage(FileInfo file, CultureInfo value, byte audioStreamIndex = 0) {
    var trackSelector = $"track:a{audioStreamIndex + 1}";
    if (value == null)
      _Execute(file, trackSelector, "--delete", "language");
    else
      _Execute(file, trackSelector, "--set", $"language={value.ThreeLetterISOLanguageName}");
  }

  /// <summary>
  /// Sets whether an audio track is marked as the default track in an MKV file.
  /// </summary>
  /// <param name="file">The MKV file to modify.</param>
  /// <param name="audioStreamIndex">The zero-based index of the audio stream to modify.</param>
  /// <param name="isDefault"><see langword="true"/> to mark the track as default; <see langword="false"/> to unmark it.</param>
  /// <remarks>
  /// The default flag indicates which track should be selected by default during playback
  /// when no explicit track selection is made by the user.
  /// </remarks>
  /// <exception cref="NotSupportedException">Thrown when <see cref="MkvPropEditExecutable"/> is not set or does not exist.</exception>
  /// <exception cref="Exception">Thrown when mkvpropedit returns an error.</exception>
  public static void SetAudioDefault(FileInfo file, byte audioStreamIndex, bool isDefault)
    => _Execute(file, $"track:a{audioStreamIndex + 1}", "--set", $"flag-default={(isDefault ? "1" : "0")}");

  private static void _Execute(FileInfo file, params string[] arguments) {
    var executable = MkvPropEditExecutable;
    if (executable is not { Exists: true })
      throw new NotSupportedException($"Please set path to MKVPropEdit first using {nameof(MkvPropEditExecutable)} property.");

    using var process = new Process {
      StartInfo = new ProcessStartInfo(executable.FullName) {
        CreateNoWindow = true,
        WindowStyle = ProcessWindowStyle.Hidden,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false
      }
    };

    _BuildArgumentList(process.StartInfo.ArgumentList, file, arguments);

    process.Start();
    process.WaitForExit();

    var result = (ReturnCode)process.ExitCode;
    if (result is ReturnCode.Success or ReturnCode.Warning)
      return;

    var output = process.StandardOutput.ReadToEnd();
    var error = process.StandardError.ReadToEnd();
    var errorMessage = _ExtractErrorMessage(output, error, arguments);

    throw new Exception(errorMessage) {
      Data = {
        { nameof(ProcessStartInfo.ArgumentList), process.StartInfo.ArgumentList },
        { nameof(arguments), arguments },
        { nameof(output), output },
        { nameof(error), error }
      }
    };
  }

  private static void _BuildArgumentList(ICollection<string> argumentList, FileInfo file, string[] arguments) {
    argumentList.Add(file.FullName);
    argumentList.Add("--edit");
    foreach (var arg in arguments)
      argumentList.Add(arg);
  }

  private static string _ExtractErrorMessage(string output, string error, string[] arguments) {
    var regex = new Regex(@"Error\s*:\s*(?<error>.*?)\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

    var match = regex.Match(output);
    if (match.Success)
      return match.Groups["error"].Value;

    match = regex.Match(error);
    if (match.Success)
      return match.Groups["error"].Value;

    return $"Something went wrong during MKVPropEdit. Arguments: {string.Join(" ", arguments)}";
  }

}