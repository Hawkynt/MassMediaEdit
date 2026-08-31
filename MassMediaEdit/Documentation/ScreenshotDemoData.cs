using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Classes;
using Models;

namespace MassMediaEdit.Documentation;

/// <summary>
/// Creates representative, deterministic rows for generated documentation screenshots.
/// This path deliberately bypasses MediaInfo and the other external tools so screenshot generation
/// cannot depend on binaries, codecs, or media files installed on the runner.
/// </summary>
internal sealed class ScreenshotDemoData : IDisposable {
  public const string Argument = "--screenshot-demo";

  private readonly DirectoryInfo _directory;

  private ScreenshotDemoData(DirectoryInfo directory, IReadOnlyList<GuiMediaItem> items) {
    this._directory = directory;
    this.Items = items;
  }

  public IReadOnlyList<GuiMediaItem> Items { get; }

  /// <summary>
  /// Builds a small data set that exercises the grid's important states: MKV editing, conversion
  /// from another container, NFO presence, two audio tracks, pending changes, and stereoscopic video.
  /// </summary>
  public static ScreenshotDemoData Create() {
    var directory = new DirectoryInfo(Path.Combine(Path.GetTempPath(), "MassMediaEdit", "ScreenshotDemo"));
    if (directory.Exists)
      directory.Delete(recursive: true);
    directory.Create();

    var aurora = _CreateItem(
      directory,
      "Aurora.Station.S01E01.2160p.mkv",
      24L * 1024 * 1024,
      "Matroska",
      3_125_000,
      "Aurora Station — Arrival",
      "Main feature",
      "HEVC",
      3840,
      2160,
      18_000_000,
      false,
      new AudioDefinition("de", "DTS", 48_000, 6, 1_509_000, true),
      new AudioDefinition("en", "AAC", 48_000, 2, 192_000, false)
    );
    File.WriteAllText(Path.ChangeExtension(aurora.MediaFile.File.FullName, ".nfo"), "<movie><title>Aurora Station — Arrival</title></movie>");

    var signalLost = _CreateItem(
      directory,
      "Signal.Lost.1080p.mp4",
      12L * 1024 * 1024,
      "MPEG-4",
      2_742_000,
      "Signal Lost",
      "Feature",
      "AVC",
      1920,
      1080,
      7_500_000,
      false,
      new AudioDefinition("en", "AAC", 48_000, 2, 256_000, true)
    );

    var parallax = _CreateItem(
      directory,
      "Parallax.3D.1080p.mkv",
      18L * 1024 * 1024,
      "Matroska",
      5_483_000,
      "Parallax",
      "Stereoscopic feature",
      "AVC",
      1920,
      1080,
      12_000_000,
      true,
      new AudioDefinition("ja", "FLAC", 48_000, 2, 980_000, true),
      new AudioDefinition("en", "AC-3", 48_000, 6, 640_000, false)
    );
    parallax.Title = "Parallax — remastered";

    return new ScreenshotDemoData(directory, [aurora, signalLost, parallax]);
  }

  /// <summary>
  /// Applies the documentation-only presentation state to the real form and optionally captures it
  /// after WinForms has shown and laid out the window.
  /// </summary>
  public void ApplyTo(MainForm form, FileInfo? screenshotOutput = null) {
    ArgumentNullException.ThrowIfNull(form);

    form.StartPosition = FormStartPosition.CenterScreen;
    form.ClientSize = new Size(1180, 500);
    form.AddItems(this.Items);

    if (screenshotOutput is null)
      return;

    form.Shown += (_, _) => form.BeginInvoke(new Action(() => {
      screenshotOutput.Directory?.Create();
      form.Refresh();

      using var bitmap = new Bitmap(form.ClientSize.Width, form.ClientSize.Height);
      form.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
      bitmap.Save(screenshotOutput.FullName, ImageFormat.Png);
      form.Close();
    }));
  }

  public void Dispose() {
    try {
      if (this._directory.Exists)
        this._directory.Delete(recursive: true);
    } catch (IOException) {
      // A killed screenshot process may still have a handle in flight. The runner is ephemeral.
    } catch (UnauthorizedAccessException) {
      // Cleanup is best-effort and must not turn a successful application close into an error.
    }
  }

  private static GuiMediaItem _CreateItem(
    DirectoryInfo directory,
    string fileName,
    long fileSize,
    string container,
    long durationInMilliseconds,
    string title,
    string videoName,
    string videoFormat,
    int width,
    int height,
    int videoBitRate,
    bool isStereoscopic,
    params AudioDefinition[] audioDefinitions
  ) {
    var file = new FileInfo(Path.Combine(directory.FullName, fileName));
    using (var stream = file.Create())
      stream.SetLength(fileSize);
    file.Refresh();

    List<MediaStream> streams = [
      new GeneralStream(_Section(
        ("codec", "Container"),
        ("codec", container),
        ("duration", durationInMilliseconds.ToString()),
        ("title", title)
      )),
      new VideoStream(_Section(
        ("title", videoName),
        ("format", videoFormat),
        ("width", width.ToString()),
        ("height", height.ToString()),
        ("bit rate", videoBitRate.ToString()),
        ("MultiView_Count", isStereoscopic ? "2" : "1"),
        ("MultiView_Layout", isStereoscopic ? "Side by side (left eye is first)" : string.Empty)
      ))
    ];

    foreach (var audio in audioDefinitions)
      streams.Add(new AudioStream(_Section(
        ("default", audio.IsDefault ? "Yes" : "No"),
        ("language", audio.Language),
        ("format", audio.Format),
        ("sampling rate", audio.SamplingRate.ToString()),
        ("channel(s)", audio.Channels.ToString()),
        ("bit rate", audio.BitRate.ToString())
      )));

    return GuiMediaItem.FromMediaFile(new MediaFile(file, streams.ToArray()));
  }

  private static SectionDictionary _Section(params (string Key, string Value)[] values) {
    var result = new SectionDictionary();
    foreach (var (key, value) in values)
      result.Add(key, value);
    return result;
  }

  private readonly record struct AudioDefinition(
    string Language,
    string Format,
    int SamplingRate,
    int Channels,
    int BitRate,
    bool IsDefault
  );
}
