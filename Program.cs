using System;
using System.IO;
using System.Windows.Forms;
using Classes;
using MassMediaEdit.Properties;

namespace MassMediaEdit;

class Program {

  [STAThread]
  static void Main(string[] args) {

    MediaFile.MediaInfoExecutable = new FileInfo(Settings.Default.MediaInfoCLIPath);
    MkvPropEdit.MkvPropEditExecutable = new FileInfo(Settings.Default.MKVPropEditPath);
    MkvMerge.MkvMergeExecutable=new FileInfo(Settings.Default.MKVMergePath);

    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);
    using var window = new MainForm();
    var givenFile = args?.FirstOrDefault().DefaultIfNullOrWhiteSpace();
    if (givenFile != null)
      window.AddFile(new FileInfo(givenFile));

    Application.Run(window);
  }
}