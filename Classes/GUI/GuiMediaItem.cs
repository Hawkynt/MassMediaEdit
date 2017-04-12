using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Libraries;

namespace Classes.GUI {
  internal partial class GuiMediaItem : INotifyPropertyChanged {
    protected MediaFile MediaFile { get; private set; }
    protected readonly Dictionary<string, object> commitData = new Dictionary<string, object>();

    protected GuiMediaItem(MediaFile mediaFile) {
      this.MediaFile = mediaFile;
    }

    [DisplayName("Changed")]
    [DataGridViewColumnWidth(56)]
    public bool NeedsCommit => this.commitData.Count > 0;

    [DisplayName("File Name")]
    public string FileName => this.MediaFile.File.FullName;

    [DataGridViewColumnWidth(DataGridViewAutoSizeColumnMode.ColumnHeader)]
    public string Size => FilesizeFormatter.FormatUnit(this.MediaFile.File.Length) + "B";

    [DataGridViewColumnWidth(DataGridViewAutoSizeColumnMode.ColumnHeader)]
    public string Container => this.MediaFile.GeneralStream?.Codec;

    private string _OriginalTitle => this.MediaFile.GeneralStream?.Title;

    public string Title {
      get {
        return (string)this.commitData.GetValueOrDefault(nameof(this.Title), () => this._OriginalTitle);
      }
      set {
        value = value.DefaultIfNullOrWhiteSpace();

        if (value == this.Title)
          return;

        if (value == this._OriginalTitle)
          this.commitData.Remove(nameof(this.Title));
        else
          this.commitData.AddOrUpdate(nameof(this.Title), value);

        this.OnPropertyChanged();
        this.OnNeedsCommitChanged();
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
      => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName))
      ;
    protected void OnNeedsCommitChanged()
      => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.NeedsCommit)))
      ;

    private void _RefreshAllProperties() {
      this.OnPropertyChanged(nameof(this.Title));
      this.OnPropertyChanged(nameof(this.Video0StereoscopicMode));
      this.OnNeedsCommitChanged();
    }

    public void RevertChanges() {
      this.commitData.Clear();
      this._RefreshAllProperties();
    }

    public void CommitChanges() {
      if (!this.NeedsCommit)
        return;

      var file = this.MediaFile.File;
      if (!IsWriteableMediaType(file))
        throw new NotSupportedException("Can not write this media type.");

      var data = this.commitData;

      if (data.ContainsKey(nameof(this.Title)))
        MkvPropEdit.SetTitle(file, (string)data[(nameof(this.Title))]);

      if (data.ContainsKey(nameof(this.Video0Name)))
        MkvPropEdit.SetVideo0Name(file, (string)data[(nameof(this.Video0Name))]);

      if (data.ContainsKey(nameof(this.Video0StereoscopicMode)))
        MkvPropEdit.SetVideo0StereoscopicMode(file, (int)data[(nameof(this.Video0StereoscopicMode))]);

      this.MediaFile = MediaFile.FromFile(file);
      data.Clear();
      this._RefreshAllProperties();
    }

    public static bool IsWriteableMediaType(FileInfo file) {
      return file.Extension == ".mkv";
    }

    public static GuiMediaItem FromMediaFile(MediaFile mediaFile) {
      if (IsWriteableMediaType(mediaFile.File))
        return new GuiMediaItem(mediaFile);

      return new ReadOnlyGuiMediaItem(mediaFile);
    }
  }

  internal class ReadOnlyGuiMediaItem : GuiMediaItem {
    public ReadOnlyGuiMediaItem(MediaFile mediaFile) : base(mediaFile) { }

    [ReadOnly(true)]
    public new string Title {
      get { return base.Title; }
      set { throw new NotSupportedException("Read-only instance"); }
    }

    [ReadOnly(true)]
    public new string Video0Name {
      get { return base.Video0Name; }
      set { throw new NotSupportedException("Read-only instance"); }
    }

    [ReadOnly(true)]
    public new StereoscopicMode Video0StereoscopicMode {
      get { return base.Video0StereoscopicMode; }
      set { throw new NotSupportedException("Read-only instance"); }
    }

  }

}
