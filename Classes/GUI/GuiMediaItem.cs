using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
      Japanese,
      [FieldDisplayName("French")]
      French,
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
        case "fra":
        return LanguageType.French;
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
        case LanguageType.French:
        return new CultureInfo("fr");
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
    private float? _progress;
    private bool _isActionPending;

    protected GuiMediaItem(MediaFile mediaFile) {
      this.MediaFile = mediaFile;
    }

    [Browsable(false)]
    public bool IsReadOnly => !this.IsMkvContainer;

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

    [DataGridViewConditionalReadOnly(nameof(IsReadOnly))]
    public string Title {
      get { return (string)this.commitData.GetValueOrDefault(nameof(this.Title), () => this._OriginalTitle); }
      set {
        value = value.DefaultIfNullOrWhiteSpace()?.Trim();

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

    [DisplayName("Convert to MKV")]
    [DataGridViewButtonColumn(isEnabledWhen: nameof(IsMkvConversionEnabled), onClickMethodName: nameof(ConvertToMkv))]
    public string ConvertTo => this.IsMkvConversionEnabled ? "Convert" : "Unavailable";

    [Browsable(false)]
    private bool _IsActionPending {
      get { return this._isActionPending; }
      set {
        if (this._isActionPending == value)
          return;

        this._isActionPending = value;
        this.OnPropertyChanged();
        this.OnPropertyChanged(nameof(this.IsMkvConversionEnabled));
        this.OnPropertyChanged(nameof(this.ConvertTo));
      }
    }

    [Browsable(false)]
    public bool IsMkvConversionEnabled => !(this._IsActionPending || this.IsMkvContainer);

    [Browsable(false)]
    public bool IsMkvContainer => ".mkv".Equals(this.MediaFile.File.Extension, StringComparison.OrdinalIgnoreCase);

    [DataGridViewProgressBarColumn]
    public float? Progress {
      get { return this._progress; }
      private set {
        if (value == this._progress)
          return;

        this._progress = value;
        this.OnPropertyChanged();
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
      var subscribers = this.PropertyChanged;
      if (subscribers == null)
        return;

      var args = new PropertyChangedEventArgs(propertyName);
      foreach (var subscriber in subscribers.GetInvocationList()) {
        var target = subscriber.Target as ISynchronizeInvoke;
        if (target != null)
          if (target.InvokeRequired) {
            var asyncResult = target.BeginInvoke(subscriber, new object[] { this, args });
            asyncResult.AsyncWaitHandle.WaitOne();
            target.EndInvoke(asyncResult);
          } else
            subscriber.DynamicInvoke(this, args);
        else
          subscriber.DynamicInvoke(this, args);

      }

      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

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
      this.OnPropertyChanged(nameof(this.ConvertTo));
      this.OnPropertyChanged(nameof(this.IsMkvConversionEnabled));
      this.OnPropertyChanged(nameof(this.IsMkvContainer));
      this.OnNeedsCommitChanged();
    }

    public void RevertChanges() {
      this.commitData.Clear();
      this._RefreshAllProperties();
    }

    public void ConvertToMkv() {
      var sourceFile = this.MediaFile.File;
      var targetFile = sourceFile.WithNewExtension("mkv");
      Action action = () => {
        try {
          this._IsActionPending = true;
          MkvMerge.ConvertToMkv(sourceFile, targetFile, f => this.Progress = f);

          this.MediaFile = MediaFile.FromFile(targetFile);
          this.Progress = null;
        } finally {
          this._IsActionPending = true;
        }
      };

      action.BeginInvoke(action.EndInvoke, null);
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

      try {

        var data = this.commitData;

        if (data.ContainsKey(nameof(this.Title)))
          MkvPropEdit.SetTitle(file, (string)data[(nameof(this.Title))]);

        if (data.ContainsKey(nameof(this.Video0Name)))
          MkvPropEdit.SetVideoName(file, (string)data[(nameof(this.Video0Name))]);

        if (data.ContainsKey(nameof(this.Video0StereoscopicMode)))
          MkvPropEdit.SetVideoStereoscopicMode(file, (int)data[(nameof(this.Video0StereoscopicMode))]);

        if (data.ContainsKey(nameof(this.Audio0Language)) &&
            (LanguageType)data[nameof(this.Audio0Language)] != LanguageType.Other)
          MkvPropEdit.SetAudioLanguage(file, _ToCulture((LanguageType)data[nameof(this.Audio0Language)]));

        if (data.ContainsKey(nameof(this.Audio1Language)) &&
            (LanguageType)data[nameof(this.Audio1Language)] != LanguageType.Other)
          MkvPropEdit.SetAudioLanguage(file, _ToCulture((LanguageType)data[nameof(this.Audio1Language)]), 1);

        data.Clear();

      } catch (Exception e) {

#if DEBUG
        if (!Debugger.IsAttached)
          Debugger.Launch();

        Debugger.Break();
#endif

      }

      this.MediaFile = MediaFile.FromFile(file);
    }

    public static bool IsWriteableMediaType(FileInfo file) {
      return file.Extension == ".mkv";
    }

    public static GuiMediaItem FromMediaFile(MediaFile mediaFile) => new GuiMediaItem(mediaFile);
  }

}
