using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Libraries;
using MassMediaEdit;

namespace Classes.GUI;

internal sealed partial class GuiMediaItem : INotifyPropertyChanged {
  
  #region messing with languages

  public enum LanguageType {
    [FieldDisplayName("")] None,
    [FieldDisplayName("Other")] Other,
    [FieldDisplayName("German")] German,
    [FieldDisplayName("English")] English,
    [FieldDisplayName("Spanish")] Spanish,
    [FieldDisplayName("Japanese")] Japanese,
    [FieldDisplayName("French")] French,
    [FieldDisplayName("Russian")] Russian,
  }

  private static LanguageType _FromCulture(CultureInfo culture) {
    if (culture == null)
      return LanguageType.None;

    return culture.ThreeLetterISOLanguageName switch {
      "deu" => LanguageType.German,
      "eng" => LanguageType.English,
      "jpn" => LanguageType.Japanese,
      "spa" => LanguageType.Spanish,
      "fra" => LanguageType.French,
      "rus" => LanguageType.Russian,
      _ => LanguageType.Other
    };
  }

  private static CultureInfo _ToCulture(LanguageType language) 
    => language switch {
      LanguageType.None => null,
      LanguageType.Other => null,
      LanguageType.German => new CultureInfo("de"),
      LanguageType.English => new CultureInfo("en"),
      LanguageType.Spanish => new CultureInfo("es"),
      LanguageType.Japanese => new CultureInfo("ja"),
      LanguageType.French => new CultureInfo("fr"),
      LanguageType.Russian => new CultureInfo("ru"),
      _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
    };

  #endregion

  [Browsable(false)]
  public MediaFile MediaFile {
    get => this._mediaFile;
    private set {
      this._mediaFile = value;
      this._RefreshAllProperties();
    }
  }

  private readonly Dictionary<string, object> commitData = new();
  private MediaFile _mediaFile;
  private float? _progress;
  private bool _isActionPending;

  private GuiMediaItem(MediaFile mediaFile) => this.MediaFile = mediaFile;

  [Browsable(false)]
  public bool IsReadOnly => !this.IsMkvContainer;

  [DisplayName("Changed")]
  [DataGridViewColumnWidth(56)]
  [DataGridViewCellStyle(backColorPropertyName:nameof(_CommitColor))]
  public bool NeedsCommit => this.commitData.Count > 0;
  private Color _CommitColor=> this.NeedsCommit ? Color.Salmon : Color.White;

  [DisplayName("File Name")]
  [DataGridViewColumnWidth((char) 10)]
  [DataGridViewClickable(onDoubleClickMethodName: nameof(Run))]
  public string FileName => this.MediaFile.File.FullName;

  [DataGridViewColumnWidth(DataGridViewAutoSizeColumnMode.ColumnHeader)]
  public string Size => FilesizeFormatter.FormatUnit(this.MediaFile.File.Length) + "B";

  [DataGridViewColumnWidth(DataGridViewAutoSizeColumnMode.ColumnHeader)]
  public string Duration {
    get {
      var duration = this.MediaFile.GeneralStream?.Duration ?? TimeSpan.Zero;
      return duration.TotalHours >= 1 
        ? $"{(int)duration.TotalHours}:{duration.Minutes:D2}:{duration.Seconds:D2}" 
        : $"{duration.Minutes}:{duration.Seconds:D2}";
    }
  }

  [DataGridViewColumnWidth(DataGridViewAutoSizeColumnMode.ColumnHeader)]
  public string Container => this.MediaFile.GeneralStream?.Codec;

  private string _OriginalTitle => this.MediaFile.GeneralStream?.Title;

  [DataGridViewColumnWidth((char) 20)]
  [DataGridViewConditionalReadOnly(nameof(IsReadOnly))]
  public string Title {
    get { return (string) this.commitData.GetValueOrDefault(nameof(this.Title), () => this._OriginalTitle); }
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
  [DataGridViewButtonColumn(isEnabledWhenPropertyName: nameof(IsMkvConversionEnabled), onClickMethodName: nameof(ConvertToMkvBackground))]
  [DataGridViewColumnWidth(DataGridViewAutoSizeColumnMode.DisplayedCells)]
  public string ConvertTo => this.IsMkvConversionEnabled ? "Convert" : "Unavailable";

  [Browsable(false)]
  private bool _IsActionPending {
    get => this._isActionPending;
    set {
      if (!this.SetProperty(this.OnPropertyChanged, ref this._isActionPending, value))
        return;
      
      this.OnPropertyChanged(nameof(this.IsMkvConversionEnabled));
      this.OnPropertyChanged(nameof(this.ConvertTo));
    }
  }

  [Browsable(false)]
  public bool IsMkvConversionEnabled => !(this._IsActionPending || this.IsMkvContainer);

  [Browsable(false)]
  public bool IsMkvContainer => ".mkv".Equals(this.MediaFile.File.Extension, StringComparison.OrdinalIgnoreCase);

  [DisplayName("NFO")]
  [DataGridViewColumnWidth((char)3)]
  public bool HasNfo => this.MediaFile.File.WithNewExtension(".nfo").Exists;

  [DataGridViewProgressBarColumn]
  [DataGridViewColumnWidth(DataGridViewAutoSizeColumnMode.DisplayedCells)]
  public float? Progress {
    get => this._progress;
    private set => this.SetProperty(this.OnPropertyChanged,ref this._progress, value);
  }

  public event PropertyChangedEventHandler PropertyChanged;

  private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
    var subscribers = this.PropertyChanged;
    if (subscribers == null)
      return;

    var args = new PropertyChangedEventArgs(propertyName);

    var invoc = MainForm.Invocator;
    if (invoc != null) {
      if (invoc.InvokeRequired) {
        // Use BeginInvoke without waiting to avoid deadlocks in parallel scenarios
        invoc.BeginInvoke(new Action(() => subscribers.Invoke(this, args)), null);
      } else
        subscribers.Invoke(this, args);

      return;
    }

    foreach (var subscriber in subscribers.GetInvocationList()) {
      if (subscriber.Target is ISynchronizeInvoke target)
        if (target.InvokeRequired) {
          target.BeginInvoke(subscriber, [this, args]);
        } else
          subscriber.DynamicInvoke(this, args);
      else
        subscriber.DynamicInvoke(this, args);
    }
  }

  private void OnNeedsCommitChanged() => this.OnPropertyChanged(nameof(this.NeedsCommit));

  private void _RefreshAllProperties() {
    this.OnPropertyChanged(nameof(this.Title));
    this.OnPropertyChanged(nameof(this.Video0StereoscopicMode));
    this.OnPropertyChanged(nameof(this.FileName));
    this.OnPropertyChanged(nameof(this.Container));
    this.OnPropertyChanged(nameof(this.Size));
    this.OnPropertyChanged(nameof(this.Duration));
    this.OnPropertyChanged(nameof(this.Video0Name));
    this.OnPropertyChanged(nameof(this.Audio0Language));
    this.OnPropertyChanged(nameof(this.Audio1Language));
    this.OnPropertyChanged(nameof(this.ConvertTo));
    this.OnPropertyChanged(nameof(this.IsMkvConversionEnabled));
    this.OnPropertyChanged(nameof(this.IsMkvContainer));
    this.OnPropertyChanged(nameof(this.HasNfo));
    this.OnPropertyChanged(nameof(this.Audio0IsDefault));
    this.OnPropertyChanged(nameof(this.Audio1IsDefault));
    this.OnNeedsCommitChanged();
  }

  public void RevertChanges() {
    this.commitData.Clear();
    this._RefreshAllProperties();
  }

  public void ConvertToMkvBackground() => Task.Run(this.ConvertToMkvSync);

  public void ConvertToMkvSync() {
    var sourceFile = this.MediaFile.File;
    var targetFile = sourceFile.WithNewExtension("mkv");
    try {
      this._IsActionPending = true;
      MkvMerge.ConvertToMkv(sourceFile, targetFile, f => this.Progress = f);
      switch (targetFile.Exists) {
        case true
          when MediaFile.FromFile(targetFile) is { } mi
               && mi.VideoStreams.Any()
               && mi.AudioStreams.ToArray() is var targetAudios
               && this.MediaFile.AudioStreams.ToArray() is var sourceAudios
               && sourceAudios.Length==targetAudios.Length
               && (mi.GeneralStream.Duration.TotalSeconds.Round() - this.MediaFile.GeneralStream.Duration.TotalSeconds.Round()).Abs() < 5
          :

          for (var i = 0; i < sourceAudios.Length; ++i)
            if (sourceAudios[i].Language != null)
              MkvPropEdit.SetAudioLanguage(targetFile, sourceAudios[i].Language, (byte)i);
          
          this.MediaFile = mi;
          sourceFile.Attributes &= ~(FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.System);
          sourceFile.TryDelete();
          
          break;
        case true:
          targetFile.TryDelete();
          break;
      }

      this.Progress = null;
    } catch (Exception e) {
#if DEBUG
      throw;
#else
      Console.WriteLine($"Error converting {sourceFile.FullName} to MKV: {e.Message}");
#endif
    } finally {
      this._IsActionPending = false;
    }
  }

  public void Run() => Process.Start(this.MediaFile.File.FullName);

  public void RenameFileToMask(string mask) {
    var sourceFile = this.MediaFile.File;
    var directory = sourceFile.Directory;

    mask = mask.MultipleReplace(new Dictionary<string, string> {
      {"{filename}", sourceFile.Name},
      {"{extension}", sourceFile.Extension[1..]},
      {"{title}", this.Title},
      {"{video:name}", this.Video0Name},
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
#if DEBUG
      throw new NotSupportedException("Can not write this media type.");
#else
      return;
#endif

    const int maxRetries = 3;
    for (var attempt = 1; attempt <= maxRetries; ++attempt) {
      try {

        var data = this.commitData;

        if (data.TryGetValue(nameof(this.Title), out var title))
          MkvPropEdit.SetTitle(file, (string) title);

        if (data.TryGetValue(nameof(this.Video0Name), out var name))
          MkvPropEdit.SetVideoName(file, (string) name);

        if (data.TryGetValue(nameof(this.Video0StereoscopicMode), out var stereoMode))
          MkvPropEdit.SetVideoStereoscopicMode(file, (int) stereoMode);

        if (data.TryGetValue(nameof(this.Audio0Language), out object audio0Language) &&
            (LanguageType)audio0Language != LanguageType.Other)
          MkvPropEdit.SetAudioLanguage(file, _ToCulture((LanguageType)audio0Language));

        if (data.TryGetValue(nameof(this.Audio1Language), out object audio1Language) &&
            (LanguageType)audio1Language != LanguageType.Other)
          MkvPropEdit.SetAudioLanguage(file, _ToCulture((LanguageType)audio1Language), 1);

        if (data.TryGetValue(nameof(this.Audio0IsDefault), out object audio0IsDefault) && audio0IsDefault != null)
          MkvPropEdit.SetAudioDefault(file, 0, (bool)audio0IsDefault);

        if (data.TryGetValue(nameof(this.Audio1IsDefault), out object audio1IsDefault) && audio1IsDefault != null)
          MkvPropEdit.SetAudioDefault(file, 1, (bool)audio1IsDefault);

        data.Clear();
        break;
      } catch (Exception e) {
        if (attempt < maxRetries)
          continue;

  #if DEBUG
        if (!Debugger.IsAttached)
          Debugger.Launch();

        Debugger.Break();
  #endif
        // Handle or log the exception after max retries if necessary
      }
    }

    this.MediaFile = MediaFile.FromFile(file);
  }

  public static bool IsWriteableMediaType(FileInfo file) => file.Extension == ".mkv";

  public static GuiMediaItem FromMediaFile(MediaFile mediaFile) => new(mediaFile);

}