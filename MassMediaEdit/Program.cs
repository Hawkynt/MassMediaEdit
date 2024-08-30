using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Classes;
using Hawkynt.NfoFileFormat;
using MassMediaEdit.Properties;

namespace MassMediaEdit;

class Program {

  [STAThread]
  static void Main(string[] args) {

    //_FixLibraryInconsistencies();
    
    MediaFile.MediaInfoExecutable = new FileInfo(Settings.Default.MediaInfoCLIPath);
    MkvPropEdit.MkvPropEditExecutable = new FileInfo(Settings.Default.MKVPropEditPath);
    MkvMerge.MkvMergeExecutable = new FileInfo(Settings.Default.MKVMergePath);

    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);
    using var window = new MainForm();
    var givenFile = args?.FirstOrDefault().DefaultIfNullOrWhiteSpace();
    if (givenFile != null)
      window.AddFile(new FileInfo(givenFile));

    Application.Run(window);
  }

  private static void _FixLibraryInconsistencies() {
    var regex = new Regex(@"\d+", RegexOptions.Compiled);
    var indir = new DirectoryInfo(@"O:\Shows");
    foreach (var show in indir.EnumerateDirectories()) {
      var nfo = show.File("tvshow.nfo");
      if (nfo.NotExists())
        continue;

      var meta = NfoFile.LoadShowOrNull(nfo);
      if (meta == null)
        continue;

      FixCertification();
      FixArts();

      nfo.CopyTo(nfo.FullName + ".bak", true);
      NfoFile.Update(nfo, meta);

      void FixArts() {
        var art = meta.Art ??= new Art();
        
        MakeLocal(art.Poster, v => art.Poster = v);
        if (art.Poster.IsNullOrWhiteSpace() && show.File("folder.jpg").Exists)
          art.Poster = "folder.jpg";

        MakeLocal(art.Fanart, v => art.Fanart = v);
        if (art.Fanart.IsNullOrWhiteSpace() && show.File("fanart.jpg").Exists)
          art.Fanart = "fanart.jpg";

      }

      void MakeLocal(string filePath, Action<string> setter) {
        if (filePath == null || !filePath.Contains("\\"))
          return;

        var file = show.File(filePath);
        if (file.NotExists())
          setter(string.Empty);

        var local = show.File(file.Name);
        if (local.FullName == file.FullName)
          setter(file.Name);
      }

      void FixCertification() {
        var mpaa = meta.MPAA ?? string.Empty;
        var cert = meta.Certification ?? string.Empty;

        var match = regex.Match(mpaa);
        if (!match.Success) {
          match = regex.Match(cert);
          if (!match.Success)
            return;
        }

        if (!match.Value.TryParseInt(out var age))
          return;

        meta.Certification = meta.MPAA = $"FSK {age}";
      }
    }
  }
}