using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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
public sealed class MainPresenter(
  IBackgroundTaskRunner backgroundTaskRunner,
  IUiSynchronizer uiSynchronizer
) : IMainPresenter {
  private static readonly TimeSpan MinimumDuration = TimeSpan.FromSeconds(1);

  private static readonly HashSet<string> MediaExtensions = new(StringComparer.OrdinalIgnoreCase) {
    // Video
    ".mkv", ".mp4", ".avi", ".mov", ".wmv", ".flv", ".webm", ".m4v", ".mpg", ".mpeg",
    ".3gp", ".3g2", ".ogv", ".ts", ".m2ts", ".mts", ".vob", ".divx", ".xvid", ".rm",
    ".rmvb", ".asf", ".f4v", ".h264", ".h265", ".hevc",
    // Audio
    ".mp3", ".wav", ".flac", ".aac", ".ogg", ".wma", ".m4a", ".opus", ".aiff", ".ape",
    ".alac", ".dts", ".ac3", ".eac3", ".mka", ".mid", ".midi", ".ra", ".amr",
    // Images (for completeness, though MediaInfo may not process all)
    ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".tif", ".webp", ".heic", ".heif"
  };

  private readonly IBackgroundTaskRunner _backgroundTaskRunner = backgroundTaskRunner ?? throw new ArgumentNullException(nameof(backgroundTaskRunner));
  private readonly IUiSynchronizer _uiSynchronizer = uiSynchronizer ?? throw new ArgumentNullException(nameof(uiSynchronizer));

  private IMainView? _view;

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
    const int filesPerBatch = 20;
    const int uiUpdateIntervalMs = 250;

    var fileQueue = new BlockingCollection<FileInfo>(new ConcurrentQueue<FileInfo>());
    var batchQueue = new BlockingCollection<FileInfo[]>(new ConcurrentQueue<FileInfo[]>());
    var resultQueue = new ConcurrentQueue<GuiMediaItem>();
    var processedCount = 0;
    var discoveredCount = 0;
    var discoveryComplete = false;

    void UpdateProgress() =>
      this._uiSynchronizer.Invoke(() =>
        this._view?.SetLoadingProgress(IndicatorKeys.Loading, processedCount, discoveredCount, discoveryComplete));

    void PushItemsToUi(List<GuiMediaItem> itemsToPush, bool applySort) {
      if (itemsToPush.Count == 0)
        return;
      var chunk = itemsToPush.ToArray();
      itemsToPush.Clear();
      this._uiSynchronizer.Invoke(() => {
        this._view?.AddItems(chunk);
        if (applySort)
          this._view?.ReapplySorting();
      });
    }

    // Producer: enumerate files in background, filtering by extension
    var producerTask = Task.Run(() => {
      try {
        foreach (var item in items) {
          var files = item switch {
            FileInfo fi => [fi],
            DirectoryInfo di => di.EnumerateFiles("*.*", SearchOption.AllDirectories),
            _ => Enumerable.Empty<FileInfo>()
          };

          foreach (var file in files) {
            if (!MediaExtensions.Contains(file.Extension))
              continue;

            fileQueue.Add(file);
            Interlocked.Increment(ref discoveredCount);

            if (discoveredCount % 50 == 0)
              UpdateProgress();
          }
        }
      } finally {
        fileQueue.CompleteAdding();
        discoveryComplete = true;
        UpdateProgress();
      }
    });

    // Batcher: collect files into batches, with timeout to avoid waiting too long
    var batcherTask = Task.Run(() => {
      var batchBuffer = new List<FileInfo>(filesPerBatch);
      var batchTimeout = TimeSpan.FromMilliseconds(500);

      while (!fileQueue.IsCompleted) {
        if (fileQueue.TryTake(out var file, batchTimeout)) {
          batchBuffer.Add(file);
          if (batchBuffer.Count >= filesPerBatch) {
            batchQueue.Add(batchBuffer.ToArray());
            batchBuffer.Clear();
          }
        } else if (batchBuffer.Count > 0) {
          // Timeout - send partial batch to start processing sooner
          batchQueue.Add(batchBuffer.ToArray());
          batchBuffer.Clear();
        }
      }

      // Drain any remaining files
      while (fileQueue.TryTake(out var file))
        batchBuffer.Add(file);

      if (batchBuffer.Count > 0)
        batchQueue.Add(batchBuffer.ToArray());
      batchQueue.CompleteAdding();
    });

    // Parallel consumers: process batches
    var consumerCount = Math.Max(1, Environment.ProcessorCount - 1);
    var consumerTasks = Enumerable.Range(0, consumerCount).Select(_ => Task.Run(() => {
      foreach (var batch in batchQueue.GetConsumingEnumerable()) {
        var results = MediaFile.FromFiles(batch)
          .Where(static m => (m.AudioStreams.Any() || m.VideoStreams.Any()) && m.GeneralStream?.Duration > MinimumDuration)
          .Select(GuiMediaItem.FromMediaFile);

        foreach (var item in results)
          resultQueue.Enqueue(item);

        Interlocked.Add(ref processedCount, batch.Length);
        UpdateProgress();
      }
    })).ToArray();

    // UI updater: push results to UI as soon as available
    var pendingItems = new List<GuiMediaItem>();
    var lastSortTime = DateTime.UtcNow;
    var sortInterval = TimeSpan.FromSeconds(2);

    while (!Task.WaitAll(consumerTasks, uiUpdateIntervalMs)) {
      while (resultQueue.TryDequeue(out var item))
        pendingItems.Add(item);

      if (pendingItems.Count > 0) {
        var shouldSort = DateTime.UtcNow - lastSortTime > sortInterval;
        PushItemsToUi(pendingItems, shouldSort);
        if (shouldSort)
          lastSortTime = DateTime.UtcNow;
      }
    }

    // Drain remaining results
    while (resultQueue.TryDequeue(out var item))
      pendingItems.Add(item);

    // Final push with sorting
    PushItemsToUi(pendingItems, applySort: true);

    producerTask.Wait();
    batcherTask.Wait();
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
