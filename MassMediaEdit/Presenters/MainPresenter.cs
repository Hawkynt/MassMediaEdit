using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Classes;
using MassMediaEdit.Abstractions;
using MassMediaEdit.Constants;
using MassMediaEdit.Utilities;
using Models;

namespace MassMediaEdit.Presenters;

/// <summary>
/// Main presenter implementing the MVP pattern for the media editor.
/// </summary>
public sealed class MainPresenter : IMainPresenter {
  private static readonly TimeSpan MinimumDuration = TimeSpan.FromSeconds(1);

  private readonly IBackgroundTaskRunner _backgroundTaskRunner;
  private readonly IUiSynchronizer _uiSynchronizer;

  private IMainView? _view;

  /// <summary>
  /// Creates a new instance of the main presenter.
  /// </summary>
  public MainPresenter(
    IBackgroundTaskRunner backgroundTaskRunner,
    IUiSynchronizer uiSynchronizer) {
    this._backgroundTaskRunner = backgroundTaskRunner ?? throw new ArgumentNullException(nameof(backgroundTaskRunner));
    this._uiSynchronizer = uiSynchronizer ?? throw new ArgumentNullException(nameof(uiSynchronizer));
  }

  /// <inheritdoc/>
  public void Initialize(IMainView view) {
    this._view = view ?? throw new ArgumentNullException(nameof(view));
    view.Presenter = this;

    // Wire up events
    view.FilesDropped += (_, e) => this.OnFilesDropped(e.Paths);
    view.RemoveSelectedRequested += (_, _) => this.RemoveItems(view.SelectedItems);
    view.ClearAllRequested += (_, _) => this.ClearAll();
    view.CommitSelectedRequested += (_, _) => this.CommitChanges(view.SelectedItems);
    view.RevertSelectedRequested += (_, _) => this.RevertChanges(view.SelectedItems);
    view.ConvertToMkvRequested += (_, _) => this.ConvertToMkv(view.SelectedItems);
    view.TitleFromFilenameRequested += (_, _) => this.SetTitleFromFilename(view.SelectedItems);
    view.VideoNameFromFilenameRequested += (_, _) => this.SetVideoNameFromFilename(view.SelectedItems);
    view.FixTitleAndNameRequested += (_, _) => this.FixTitleAndName(view.SelectedItems);
    view.RecoverSpacesRequested += (_, _) => this.RecoverSpaces(view.SelectedItems);
    view.RemoveBracketContentRequested += (_, _) => this.RemoveBracketContent(view.SelectedItems);
    view.SwapTitleAndNameRequested += (_, _) => this.SwapTitleAndName(view.SelectedItems);
    view.ClearTitleRequested += (_, _) => this.ClearTitle(view.SelectedItems);
    view.ClearVideoNameRequested += (_, _) => this.ClearVideoName(view.SelectedItems);
    view.RenameFilesRequested += (_, e) => this.RenameFiles(view.SelectedItems, e.Mask);
    view.AudioLanguageChanged += (_, e) => this.SetAudioLanguage(view.SelectedItems, e.TrackIndex, e.Language);
  }

  /// <inheritdoc/>
  public void OnFilesDropped(string[] paths) {
    if (paths is not { Length: > 0 })
      return;

    var fileSystemInfos = paths
      .Select(static p => File.Exists(p) ? (FileSystemInfo)new FileInfo(p) : new DirectoryInfo(p))
      .Where(static i => i.Exists)
      .ToArray();

    if (fileSystemInfos.Length == 0)
      return;

    _ = this._backgroundTaskRunner.RunAsync(
      TaskTags.DragDrop,
      () => this.ProcessDroppedItems(fileSystemInfos),
      () => this._view?.SetLoadingIndicator(IndicatorKeys.Loading, true),
      () => this._view?.SetLoadingIndicator(IndicatorKeys.Loading, false));
  }

  /// <inheritdoc/>
  public void AddFile(FileInfo file) {
    ArgumentNullException.ThrowIfNull(file);
    this.ProcessDroppedItems([file]);
  }

  private void ProcessDroppedItems(FileSystemInfo[] items) {
    var files = items
      .SelectMany(static i => i switch {
        FileInfo fi => [fi],
        DirectoryInfo di => di.EnumerateFiles("*.*", SearchOption.AllDirectories),
        _ => []
      });

    var mediaItems = files
      .AsParallel()
      .WithDegreeOfParallelism(Environment.ProcessorCount)
      .Select(MediaFile.FromFile)
      .Where(m => (m.AudioStreams.Any() || m.VideoStreams.Any()) && m.GeneralStream?.Duration > MinimumDuration)
      .Select(GuiMediaItem.FromMediaFile)
      .ToList();

    foreach (var chunk in mediaItems.Chunk(64))
      this._uiSynchronizer.Invoke(() => {
        this._view?.AddItems(chunk);
        this._view?.ReapplySorting();
      });
  }

  /// <inheritdoc/>
  public void RemoveItems(IEnumerable<GuiMediaItem> items) {
    var itemsArray = items.ToArray();
    if (itemsArray.Length == 0)
      return;

    this._view?.RemoveItems(itemsArray);
  }

  /// <inheritdoc/>
  public void ClearAll() => this._view?.ClearItems();

  /// <inheritdoc/>
  public void CommitChanges(IEnumerable<GuiMediaItem> items) {
    var itemsToCommit = items.Where(static i => i.NeedsCommit && !i.IsReadOnly).ToArray();
    if (itemsToCommit.Length == 0)
      return;

    _ = this._backgroundTaskRunner.RunAsync(
      TaskTags.Commit,
      () => {
        Parallel.ForEach(itemsToCommit, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
          static item => item.CommitChanges());
      },
      () => this._view?.SetLoadingIndicator(IndicatorKeys.Committing, true),
      () => this._view?.SetLoadingIndicator(IndicatorKeys.Committing, false));
  }

  /// <inheritdoc/>
  public void RevertChanges(IEnumerable<GuiMediaItem> items) =>
    Parallel.ForEach(items.ToArray(), static item => item.RevertChanges());

  /// <inheritdoc/>
  public void ConvertToMkv(IEnumerable<GuiMediaItem> items) {
    var itemsToConvert = items.Where(static i => i.IsMkvConversionEnabled).ToArray();
    if (itemsToConvert.Length == 0)
      return;

    _ = this._backgroundTaskRunner.RunAsync(
      TaskTags.Convert,
      () => {
        Parallel.ForEach(itemsToConvert, new ParallelOptions { MaxDegreeOfParallelism = 2 },
          static item => item.ConvertToMkvSync());
      },
      () => this._view?.SetLoadingIndicator(IndicatorKeys.Converting, true),
      () => this._view?.SetLoadingIndicator(IndicatorKeys.Converting, false));
  }

  /// <inheritdoc/>
  public void SetTitleFromFilename(IEnumerable<GuiMediaItem> items) =>
    Parallel.ForEach(items.Where(static i => !i.IsReadOnly).ToArray(),
      static item => item.Title = item.MediaFile.File.GetFilenameWithoutExtension());

  /// <inheritdoc/>
  public void SetVideoNameFromFilename(IEnumerable<GuiMediaItem> items) =>
    Parallel.ForEach(items.Where(static i => !i.IsReadOnly).ToArray(),
      static item => item.Video0Name = item.MediaFile.File.GetFilenameWithoutExtension());

  /// <inheritdoc/>
  public void FixTitleAndName(IEnumerable<GuiMediaItem> items) =>
    Parallel.ForEach(items.Where(static i => !i.IsReadOnly).ToArray(), static item => {
      var extracted = TextManipulation.ExtractSeasonEpisode(item.Title)
                      ?? TextManipulation.ExtractSeasonEpisode(item.Video0Name);

      if (extracted is not var (season, episode, title))
        return;

      item.Title = TextManipulation.FormatSeasonEpisode(season, episode);
      if (!string.IsNullOrWhiteSpace(title))
        item.Video0Name = title;
    });

  /// <inheritdoc/>
  public void RecoverSpaces(IEnumerable<GuiMediaItem> items) =>
    Parallel.ForEach(items.Where(static i => !i.IsReadOnly).ToArray(), static item => {
      item.Title = TextManipulation.RecoverSpaces(item.Title);
      item.Video0Name = TextManipulation.RecoverSpaces(item.Video0Name);
    });

  /// <inheritdoc/>
  public void RemoveBracketContent(IEnumerable<GuiMediaItem> items) =>
    Parallel.ForEach(items.Where(static i => !i.IsReadOnly).ToArray(), static item => {
      item.Title = TextManipulation.RemoveBracketContent(item.Title);
      item.Video0Name = TextManipulation.RemoveBracketContent(item.Video0Name);
    });

  /// <inheritdoc/>
  public void SwapTitleAndName(IEnumerable<GuiMediaItem> items) =>
    Parallel.ForEach(items.Where(static i => !i.IsReadOnly).ToArray(),
      static item => (item.Video0Name, item.Title) = (item.Title, item.Video0Name));

  /// <inheritdoc/>
  public void ClearTitle(IEnumerable<GuiMediaItem> items) =>
    Parallel.ForEach(items.Where(static i => !i.IsReadOnly).ToArray(),
      static item => item.Title = null);

  /// <inheritdoc/>
  public void ClearVideoName(IEnumerable<GuiMediaItem> items) =>
    Parallel.ForEach(items.Where(static i => !i.IsReadOnly).ToArray(),
      static item => item.Video0Name = null);

  /// <inheritdoc/>
  public void RenameFiles(IEnumerable<GuiMediaItem> items, string mask) {
    ArgumentException.ThrowIfNullOrEmpty(mask);

    foreach (var item in items.ToArray())
      item.RenameFileToMask(mask);
  }

  /// <inheritdoc/>
  public void SetAudioLanguage(IEnumerable<GuiMediaItem> items, int trackIndex, GuiMediaItem.LanguageType language) {
    var editableItems = items.Where(i => !i.IsReadOnly && (trackIndex == 0 ? i.HasAudio0 : i.HasAudio1)).ToArray();

    Parallel.ForEach(editableItems, item => {
      if (trackIndex == 0)
        item.Audio0Language = language;
      else
        item.Audio1Language = language;
    });
  }
}
