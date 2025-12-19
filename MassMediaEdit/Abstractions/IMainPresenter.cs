using System.Collections.Generic;
using System.IO;
using Models;

namespace MassMediaEdit.Abstractions;

/// <summary>
/// Defines the contract for the main presenter in the MVP pattern.
/// </summary>
public interface IMainPresenter {
  /// <summary>
  /// Initializes the presenter with the associated view.
  /// </summary>
  /// <param name="view">The view to bind to.</param>
  void Initialize(IMainView view);

  /// <summary>
  /// Handles files being dropped onto the view.
  /// </summary>
  /// <param name="paths">The paths of dropped files and folders.</param>
  void OnFilesDropped(string[] paths);

  /// <summary>
  /// Handles a request to add a single file.
  /// </summary>
  /// <param name="file">The file to add.</param>
  void AddFile(FileInfo file);

  /// <summary>
  /// Handles a request to remove the specified items.
  /// </summary>
  /// <param name="items">The items to remove.</param>
  void RemoveItems(IEnumerable<GuiMediaItem> items);

  /// <summary>
  /// Handles a request to clear all items.
  /// </summary>
  void ClearAll();

  /// <summary>
  /// Handles a request to commit changes for the specified items.
  /// </summary>
  /// <param name="items">The items to commit.</param>
  void CommitChanges(IEnumerable<GuiMediaItem> items);

  /// <summary>
  /// Handles a request to revert changes for the specified items.
  /// </summary>
  /// <param name="items">The items to revert.</param>
  void RevertChanges(IEnumerable<GuiMediaItem> items);

  /// <summary>
  /// Handles a request to convert the specified items to MKV format.
  /// </summary>
  /// <param name="items">The items to convert.</param>
  void ConvertToMkv(IEnumerable<GuiMediaItem> items);

  /// <summary>
  /// Handles a request to set title from filename for the specified items.
  /// </summary>
  /// <param name="items">The items to update.</param>
  void SetTitleFromFilename(IEnumerable<GuiMediaItem> items);

  /// <summary>
  /// Handles a request to set video name from filename for the specified items.
  /// </summary>
  /// <param name="items">The items to update.</param>
  void SetVideoNameFromFilename(IEnumerable<GuiMediaItem> items);

  /// <summary>
  /// Handles a request to fix title and video name for the specified items.
  /// </summary>
  /// <param name="items">The items to fix.</param>
  void FixTitleAndName(IEnumerable<GuiMediaItem> items);

  /// <summary>
  /// Handles a request to recover spaces in title/name for the specified items.
  /// </summary>
  /// <param name="items">The items to update.</param>
  void RecoverSpaces(IEnumerable<GuiMediaItem> items);

  /// <summary>
  /// Handles a request to remove bracket content from title/name for the specified items.
  /// </summary>
  /// <param name="items">The items to update.</param>
  void RemoveBracketContent(IEnumerable<GuiMediaItem> items);

  /// <summary>
  /// Handles a request to swap title and video name for the specified items.
  /// </summary>
  /// <param name="items">The items to update.</param>
  void SwapTitleAndName(IEnumerable<GuiMediaItem> items);

  /// <summary>
  /// Handles a request to clear title for the specified items.
  /// </summary>
  /// <param name="items">The items to update.</param>
  void ClearTitle(IEnumerable<GuiMediaItem> items);

  /// <summary>
  /// Handles a request to clear video name for the specified items.
  /// </summary>
  /// <param name="items">The items to update.</param>
  void ClearVideoName(IEnumerable<GuiMediaItem> items);

  /// <summary>
  /// Handles a request to rename files using the specified mask.
  /// </summary>
  /// <param name="items">The items to rename.</param>
  /// <param name="mask">The rename mask to apply.</param>
  void RenameFiles(IEnumerable<GuiMediaItem> items, string mask);

  /// <summary>
  /// Handles a request to change audio language for the specified items.
  /// </summary>
  /// <param name="items">The items to update.</param>
  /// <param name="trackIndex">The zero-based index of the audio track.</param>
  /// <param name="language">The new language type.</param>
  void SetAudioLanguage(IEnumerable<GuiMediaItem> items, int trackIndex, GuiMediaItem.LanguageType language);
}
