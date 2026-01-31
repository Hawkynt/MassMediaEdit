using System;
using System.Collections.Generic;
using Models;

namespace MassMediaEdit.Abstractions;

/// <summary>
/// Defines the contract for the main media editor view in the MVP pattern.
/// </summary>
public interface IMainView {
  /// <summary>
  /// Gets or sets the presenter handling this view's logic.
  /// </summary>
  IMainPresenter? Presenter { get; set; }

  /// <summary>
  /// Gets the currently selected media items.
  /// </summary>
  IEnumerable<GuiMediaItem> SelectedItems { get; }

  /// <summary>
  /// Adds media items to the view's display.
  /// </summary>
  /// <param name="items">The items to add.</param>
  void AddItems(IEnumerable<GuiMediaItem> items);

  /// <summary>
  /// Removes media items from the view's display.
  /// </summary>
  /// <param name="items">The items to remove.</param>
  void RemoveItems(IEnumerable<GuiMediaItem> items);

  /// <summary>
  /// Clears all media items from the view.
  /// </summary>
  void ClearItems();

  /// <summary>
  /// Shows or hides a loading indicator.
  /// </summary>
  /// <param name="indicatorKey">The key identifying the indicator.</param>
  /// <param name="visible">Whether the indicator should be visible.</param>
  void SetLoadingIndicator(string indicatorKey, bool visible);

  /// <summary>
  /// Updates the loading progress display.
  /// </summary>
  /// <param name="indicatorKey">The key identifying the indicator.</param>
  /// <param name="processed">Number of files processed.</param>
  /// <param name="discovered">Number of files discovered so far.</param>
  /// <param name="discoveryComplete">Whether file discovery has completed.</param>
  void SetLoadingProgress(string indicatorKey, int processed, int discovered, bool discoveryComplete);

  /// <summary>
  /// Refreshes the display of the specified items.
  /// </summary>
  /// <param name="items">The items to refresh, or <see langword="null"/> to refresh all.</param>
  void RefreshItems(IEnumerable<GuiMediaItem>? items = null);

  /// <summary>
  /// Reapplies the current sorting to the item list.
  /// </summary>
  void ReapplySorting();

  /// <summary>
  /// Shows an error message to the user.
  /// </summary>
  /// <param name="title">The error title.</param>
  /// <param name="message">The error message.</param>
  void ShowError(string title, string message);

  /// <summary>
  /// Shows a confirmation dialog to the user.
  /// </summary>
  /// <param name="title">The dialog title.</param>
  /// <param name="message">The confirmation message.</param>
  /// <returns><see langword="true"/> if the user confirms; otherwise, <see langword="false"/>.</returns>
  bool ShowConfirmation(string title, string message);

  #region Events

  /// <summary>
  /// Raised when files are dropped onto the view.
  /// </summary>
  event EventHandler<FilesDroppedEventArgs>? FilesDropped;

  /// <summary>
  /// Raised when the user requests to remove selected items.
  /// </summary>
  event EventHandler? RemoveSelectedRequested;

  /// <summary>
  /// Raised when the user requests to clear all items.
  /// </summary>
  event EventHandler? ClearAllRequested;

  /// <summary>
  /// Raised when the user requests to commit changes for selected items.
  /// </summary>
  event EventHandler? CommitSelectedRequested;

  /// <summary>
  /// Raised when the user requests to revert changes for selected items.
  /// </summary>
  event EventHandler? RevertSelectedRequested;

  /// <summary>
  /// Raised when the user requests to convert selected items to MKV.
  /// </summary>
  event EventHandler? ConvertToMkvRequested;

  /// <summary>
  /// Raised when the user requests to set title from filename for selected items.
  /// </summary>
  event EventHandler? TitleFromFilenameRequested;

  /// <summary>
  /// Raised when the user requests to set video name from filename for selected items.
  /// </summary>
  event EventHandler? VideoNameFromFilenameRequested;

  /// <summary>
  /// Raised when the user requests to fix title and video name for selected items.
  /// </summary>
  event EventHandler? FixTitleAndNameRequested;

  /// <summary>
  /// Raised when the user requests to recover spaces in title/name for selected items.
  /// </summary>
  event EventHandler? RecoverSpacesRequested;

  /// <summary>
  /// Raised when the user requests to remove bracket content from title/name for selected items.
  /// </summary>
  event EventHandler? RemoveBracketContentRequested;

  /// <summary>
  /// Raised when the user requests to swap title and video name for selected items.
  /// </summary>
  event EventHandler? SwapTitleAndNameRequested;

  /// <summary>
  /// Raised when the user requests to clear title for selected items.
  /// </summary>
  event EventHandler? ClearTitleRequested;

  /// <summary>
  /// Raised when the user requests to clear video name for selected items.
  /// </summary>
  event EventHandler? ClearVideoNameRequested;

  /// <summary>
  /// Raised when the user requests to rename files using a mask.
  /// </summary>
  event EventHandler<RenameRequestedEventArgs>? RenameFilesRequested;

  /// <summary>
  /// Raised when the user changes the audio language for a track.
  /// </summary>
  event EventHandler<AudioLanguageChangedEventArgs>? AudioLanguageChanged;

  #endregion
}

/// <summary>
/// Event arguments for files being dropped onto the view.
/// </summary>
public sealed class FilesDroppedEventArgs(string[] paths) : EventArgs {
  /// <summary>
  /// Gets the paths of the dropped files and folders.
  /// </summary>
  public string[] Paths { get; } = paths;
}

/// <summary>
/// Event arguments for file rename requests.
/// </summary>
public sealed class RenameRequestedEventArgs(string mask) : EventArgs {
  /// <summary>
  /// Gets the rename mask to apply.
  /// </summary>
  public string Mask { get; } = mask;
}

/// <summary>
/// Event arguments for audio language changes.
/// </summary>
public sealed class AudioLanguageChangedEventArgs(int trackIndex, GuiMediaItem.LanguageType language) : EventArgs {
  /// <summary>
  /// Gets the zero-based index of the audio track.
  /// </summary>
  public int TrackIndex { get; } = trackIndex;

  /// <summary>
  /// Gets the new language type.
  /// </summary>
  public GuiMediaItem.LanguageType Language { get; } = language;
}
