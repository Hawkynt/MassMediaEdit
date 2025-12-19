using System.IO;
using System.Windows.Forms;
using Classes;
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

    // Create the main form (View in MVP)
    using var mainForm = new MainForm();

    // Create services
    var uiSynchronizer = new WinFormsUiSynchronizer(mainForm);
    var backgroundTaskRunner = new BackgroundTaskRunner(uiSynchronizer);

    // Create and initialize the presenter
    var presenter = new MainPresenter(backgroundTaskRunner, uiSynchronizer);
    presenter.Initialize(mainForm);

    // Handle command-line arguments
    if (args is { Length: > 0 } && !string.IsNullOrWhiteSpace(args[0]))
      presenter.AddFile(new FileInfo(args[0]));

    Application.Run(mainForm);
  }
}