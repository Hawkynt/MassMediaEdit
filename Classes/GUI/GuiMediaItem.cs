using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Libraries;

namespace Classes.GUI {
  internal partial class GuiMediaItem : INotifyPropertyChanged {


    #region messing with languages

    public enum LanguageType {
      [FieldDisplayName("")]
      None,
      [FieldDisplayName("Other")]
      Other,
      [FieldDisplayName("German")]
      German,
      [FieldDisplayName("English")]
      English,
      [FieldDisplayName("Spanish")]
      Spanish,
      [FieldDisplayName("Japanese")]
      Japanese
    }

    private static LanguageType _FromCulture(CultureInfo culture) {
      if (culture == null)
        return LanguageType.None;

      switch (culture.ThreeLetterISOLanguageName) {
        case "deu":
        return LanguageType.German;
        case "eng":
        return LanguageType.English;
        case "jpn":
        return LanguageType.Japanese;
        case "spa":
        return LanguageType.Spanish;
        default:
        return LanguageType.Other;
      }
    }

    private static CultureInfo _ToCulture(LanguageType language) {
      switch (language) {
        case LanguageType.None:
        return null;
        case LanguageType.Other:
        return null;
        case LanguageType.German:
        return new CultureInfo("de");
        case LanguageType.English:
        return new CultureInfo("en");
        case LanguageType.Spanish:
        return new CultureInfo("es");
        case LanguageType.Japanese:
        return new CultureInfo("ja");
        default:
        throw new ArgumentOutOfRangeException(nameof(language), language, null);
      }
    }

    #endregion

    [Browsable(false)]
    public MediaFile MediaFile {
      get { return this._mediaFile; }
      private set {
        this._mediaFile = value;
        this._RefreshAllProperties();
      }
    }

    protected readonly Dictionary<string, object> commitData = new Dictionary<string, object>();
    private MediaFile _mediaFile;

    protected GuiMediaItem(MediaFile mediaFile) {
      this.MediaFile = mediaFile;
    }

    [Browsable(false)]
    public bool IsReadOnly => this is ReadOnlyGuiMediaItem;

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
      get { return (string)this.commitData.GetValueOrDefault(nameof(this.Title), () => this._OriginalTitle); }
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
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    protected void OnNeedsCommitChanged() => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.NeedsCommit)));

    private void _RefreshAllProperties() {
      this.OnPropertyChanged(nameof(this.Title));
      this.OnPropertyChanged(nameof(this.Video0StereoscopicMode));
      this.OnPropertyChanged(nameof(this.FileName));
      this.OnPropertyChanged(nameof(this.Container));
      this.OnPropertyChanged(nameof(this.Size));
      this.OnPropertyChanged(nameof(this.Video0Name));
      this.OnPropertyChanged(nameof(this.Audio0Language));
      this.OnPropertyChanged(nameof(this.Audio1Language));
      this.OnNeedsCommitChanged();
    }

    public void RevertChanges() {
      this.commitData.Clear();
      this._RefreshAllProperties();
    }

    public void RenameFileToMask(string mask) {
      var sourceFile = this.MediaFile.File;
      var directory = sourceFile.Directory;

      mask = mask.MultipleReplace(new Dictionary<string, string> {
        { "{filename}", sourceFile.Name },
        { "{extension}", sourceFile.Extension.Substring(1)},
        { "{title}", this.Title},
        { "{video:name}", this.Video0Name},
      });

      var targetFile = directory.File(mask);
      sourceFile.MoveTo(targetFile.FullName);
      this.MediaFile = MediaFile.FromFile(targetFile);
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
        MkvPropEdit.SetVideoName(file, (string)data[(nameof(this.Video0Name))]);

      if (data.ContainsKey(nameof(this.Video0StereoscopicMode)))
        MkvPropEdit.SetVideoStereoscopicMode(file, (int)data[(nameof(this.Video0StereoscopicMode))]);

      if (data.ContainsKey(nameof(this.Audio0Language)) && (LanguageType)data[nameof(this.Audio0Language)] != LanguageType.Other)
        MkvPropEdit.SetAudioLanguage(file, _ToCulture((LanguageType)data[nameof(this.Audio0Language)]));

      if (data.ContainsKey(nameof(this.Audio1Language)) && (LanguageType)data[nameof(this.Audio1Language)] != LanguageType.Other)
        MkvPropEdit.SetAudioLanguage(file, _ToCulture((LanguageType)data[nameof(this.Audio1Language)]), 1);

      data.Clear();
      this.MediaFile = MediaFile.FromFile(file);
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

    [ReadOnly(true)]
    public new LanguageType Audio0Language {
      get { return base.Audio0Language; }
      set { throw new NotSupportedException("Read-only instance"); }
    }

    [ReadOnly(true)]
    public new LanguageType Audio1Language {
      get { return base.Audio1Language; }
      set { throw new NotSupportedException("Read-only instance"); }
    }

  }
}
