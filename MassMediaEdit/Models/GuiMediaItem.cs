using System;
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
using Classes;
using Libraries;
using MassMediaEdit;
using MassMediaEdit.Constants;

namespace Models;

/// <summary>
/// Represents a media item in the GUI with change tracking and property change notifications.
/// Serves as the ViewModel/Model in the MVP pattern for binding to DataGridView.
/// </summary>
public sealed partial class GuiMediaItem : INotifyPropertyChanged {
  
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

  private static LanguageType _FromCulture(CultureInfo? culture) {
    if (culture is null)
      return LanguageType.None;

    return culture.ThreeLetterISOLanguageName switch {
      LanguageCodes.German => LanguageType.German,
      LanguageCodes.English => LanguageType.English,
      LanguageCodes.Japanese => LanguageType.Japanese,
      LanguageCodes.Spanish => LanguageType.Spanish,
      LanguageCodes.French => LanguageType.French,
      LanguageCodes.Russian => LanguageType.Russian,
      _ => LanguageType.Other
    };
  }

  private static CultureInfo? _ToCulture(LanguageType language) 
    => language switch {
      LanguageType.None => null,
      LanguageType.Other => null,
      LanguageType.German => new(LanguageCodesShort.German),
      LanguageType.English => new(LanguageCodesShort.English),
      LanguageType.Spanish => new(LanguageCodesShort.Spanish),
      LanguageType.Japanese => new(LanguageCodesShort.Japanese),
      LanguageType.French => new(LanguageCodesShort.French),
      LanguageType.Russian => new(LanguageCodesShort.Russian),
      _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
    };

  #endregion

  [Browsable(false)]
  public MediaFile MediaFile
  {
    get;
    private set
    {
      field = value;
      this._PopulateCachedValues(value);
      this._RefreshAllProperties();
    }
  }

  private readonly Dictionary<string, object?> _commitData = [];

  private GuiMediaItem(MediaFile mediaFile) => this.MediaFile = mediaFile;

  [Browsable(false)]
  public bool IsReadOnly => !this.IsMkvContainer;

  [DisplayName("Changed")]
  [DataGridViewColumnWidth(56)]
  [DataGridViewCellStyle(backColorPropertyName:nameof(_CommitColor))]
  public bool NeedsCommit => this._commitData.Count > 0;
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
  public string? Container => this.MediaFile.GeneralStream?.Codec;

  private string? _OriginalTitle => this.MediaFile.GeneralStream?.Title;

  [DataGridViewColumnWidth((char) 20)]
  [DataGridViewConditionalReadOnly(nameof(IsReadOnly))]
  public string? Title {
    get => (string?)this._commitData.GetValueOrDefault(nameof(this.Title), () => this._OriginalTitle);
    set {
      value = value.DefaultIfNullOrWhiteSpace()?.Trim();

      if (value == this.Title)
        return;

      if (value == this._OriginalTitle)
        this._commitData.Remove(nameof(this.Title));
      else
        this._commitData.AddOrUpdate(nameof(this.Title), value);

      this.OnPropertyChanged();
      this.OnNeedsCommitChanged();
    }
  }

  private const string ConvertButtonText = "Convert";
  private const string UnavailableButtonText = "Unavailable";

  [DisplayName("Convert to MKV")]
  [DataGridViewButtonColumn(isEnabledWhenPropertyName: nameof(IsMkvConversionEnabled), onClickMethodName: nameof(ConvertToMkvBackground))]
  [DataGridViewColumnWidth(DataGridViewAutoSizeColumnMode.DisplayedCells)]
  public string ConvertTo => this.IsMkvConversionEnabled ? ConvertButtonText : UnavailableButtonText;

  [Browsable(false)]
  private bool _IsActionPending
  {
    get;
    set
    {
      if (!this.SetProperty(this.OnPropertyChanged, ref field, value))
        return;

      this.OnPropertyChanged(nameof(this.IsMkvConversionEnabled));
      this.OnPropertyChanged(nameof(this.ConvertTo));
    }
  }

  [Browsable(false)]
  public bool IsMkvConversionEnabled => !(this._IsActionPending || this.IsMkvContainer);

  [Browsable(false)]
  public bool IsMkvContainer => FileExtensions.Mkv.Equals(this.MediaFile.File.Extension, StringComparison.OrdinalIgnoreCase);

  [DisplayName("NFO")]
  [DataGridViewColumnWidth((char)3)]
  public bool HasNfo => this.MediaFile.File.WithNewExtension(FileExtensions.Nfo).Exists;

  [DataGridViewProgressBarColumn]
  [DataGridViewColumnWidth(DataGridViewAutoSizeColumnMode.DisplayedCells)]
  public float? Progress
  {
    get;
    private set => this.SetProperty(this.OnPropertyChanged, ref field, value);
  }

  public event PropertyChangedEventHandler? PropertyChanged;

  private void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
    var subscribers = this.PropertyChanged;
    if (subscribers is null)
      return;

    var args = new PropertyChangedEventArgs(propertyName);

    var invoc = MainForm.Invocator;
    if (invoc is not null) {
      if (invoc.InvokeRequired) {
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
    this._commitData.Clear();
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
            if (sourceAudios[i].Language is not null)
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
      {RenamePlaceholders.Filename, sourceFile.Name},
      {RenamePlaceholders.Extension, sourceFile.Extension[1..]},
      {RenamePlaceholders.Title, this.Title ?? string.Empty},
      {RenamePlaceholders.VideoName, this.Video0Name ?? string.Empty},
    });

    var targetFile = directory?.File(mask);
    if (targetFile is null)
      return;

    if (string.Equals(sourceFile.FullName, targetFile.FullName, StringComparison.OrdinalIgnoreCase))
      return;

    if (targetFile.Exists) {
      var baseName = Path.GetFileNameWithoutExtension(targetFile.Name);
      var extension = targetFile.Extension;
      var counter = 1;

      do {
        var newName = $"{baseName} ({counter}){extension}";
        targetFile = directory!.File(newName);
        ++counter;
      } while (targetFile.Exists && counter < 1000);

      if (targetFile.Exists)
        throw new IOException($"Cannot rename '{sourceFile.Name}': unable to find a unique target filename.");
    }

    var targetPath = targetFile.FullName;
    sourceFile.MoveTo(targetPath);
    
    this.MediaFile = MediaFile.FromFile(new FileInfo(targetPath));
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

        var data = this._commitData;

        if (data.TryGetValue(nameof(this.Title), out var title))
          MkvPropEdit.SetTitle(file, (string?)title);

        if (data.TryGetValue(nameof(this.Video0Name), out var name))
          MkvPropEdit.SetVideoName(file, (string?)name);

        if (data.TryGetValue(nameof(this.Video0StereoscopicMode), out var stereoMode))
          MkvPropEdit.SetVideoStereoscopicMode(file, (int)stereoMode!);

        if (data.TryGetValue(nameof(this.Audio0Language), out var audio0Language) &&
            (LanguageType)audio0Language! != LanguageType.Other)
          MkvPropEdit.SetAudioLanguage(file, _ToCulture((LanguageType)audio0Language));

        if (data.TryGetValue(nameof(this.Audio1Language), out var audio1Language) &&
            (LanguageType)audio1Language! != LanguageType.Other)
          MkvPropEdit.SetAudioLanguage(file, _ToCulture((LanguageType)audio1Language), 1);

        if (data.TryGetValue(nameof(this.Audio0IsDefault), out var audio0IsDefault) && audio0IsDefault is not null)
          MkvPropEdit.SetAudioDefault(file, 0, (bool)audio0IsDefault);

        if (data.TryGetValue(nameof(this.Audio1IsDefault), out var audio1IsDefault) && audio1IsDefault is not null)
          MkvPropEdit.SetAudioDefault(file, 1, (bool)audio1IsDefault);

        data.Clear();
        break;
      } catch (Exception) {
        if (attempt < maxRetries)
          continue;

#if DEBUG
        if (!Debugger.IsAttached)
          Debugger.Launch();

        Debugger.Break();
#endif
      }
    }

    this.MediaFile = MediaFile.FromFile(file);
  }

  public static bool IsWriteableMediaType(FileInfo file) => file.Extension.Equals(FileExtensions.Mkv, StringComparison.OrdinalIgnoreCase);

  public static GuiMediaItem FromMediaFile(MediaFile mediaFile) => new(mediaFile);

}