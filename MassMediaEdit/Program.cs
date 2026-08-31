using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Classes;
using MassMediaEdit.Documentation;
using MassMediaEdit.Presenters;
using MassMediaEdit.Properties;
using MassMediaEdit.Services;

namespace MassMediaEdit;

/// <summary>
/// Application entry point with dependency injection setup.
/// </summary>
internal static class Program {
  [System.STAThread]
  private static void Main(string[] args) {
    // Configure external tools
    MediaFile.MediaInfoExecutable = new FileInfo(Settings.Default.MediaInfoCLIPath);
    MkvPropEdit.MkvPropEditExecutable = new FileInfo(Settings.Default.MKVPropEditPath);
    MkvMerge.MkvMergeExecutable = new FileInfo(Settings.Default.MKVMergePath);

    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);

    var screenshotArgument = args?.FirstOrDefault(arg =>
      string.Equals(arg, ScreenshotDemoData.Argument, StringComparison.OrdinalIgnoreCase)
      || arg.StartsWith(ScreenshotDemoData.Argument + "=", StringComparison.OrdinalIgnoreCase)
    );
    using var screenshotDemo = screenshotArgument is not null ? ScreenshotDemoData.Create() : null;

    // Create the main form (View in MVP)
    using var mainForm = new MainForm();

    // Create services
    var uiSynchronizer = new WinFormsUiSynchronizer(mainForm);
    var backgroundTaskRunner = new BackgroundTaskRunner(uiSynchronizer);

    // Create and initialize the presenter
    var presenter = new MainPresenter(backgroundTaskRunner, uiSynchronizer);
    presenter.Initialize(mainForm);

    // Documentation screenshots use deterministic, pre-parsed media rows and never invoke the external tools.
    if (screenshotDemo is not null) {
      FileInfo? screenshotOutput = null;
      if (screenshotArgument!.StartsWith(ScreenshotDemoData.Argument + "=", StringComparison.OrdinalIgnoreCase)) {
        var outputPath = screenshotArgument[(ScreenshotDemoData.Argument.Length + 1)..];
        if (!string.IsNullOrWhiteSpace(outputPath))
          screenshotOutput = new FileInfo(Path.GetFullPath(outputPath));
      }
      screenshotDemo.ApplyTo(mainForm, screenshotOutput);
    } else if (args is { Length: > 0 } && !string.IsNullOrWhiteSpace(args[0]))
      presenter.AddFile(new FileInfo(args[0]));

    Application.Run(mainForm);
  }
}