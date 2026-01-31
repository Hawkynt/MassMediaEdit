using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hawkynt.NfoFileFormat;
using MassMediaEdit.Abstractions;
using MassMediaEdit.Constants;
using MassMediaEdit.Properties;
using Models;

namespace MassMediaEdit;

public partial class MainForm : Form, IMainView {
  internal static ISynchronizeInvoke? Invocator { get; private set; }

  public MainForm() {
    Invocator = this;
    this.InitializeComponent();
    this.SetFormTitle();
    this._BuildFileRenameMenu();

    this.dgvResults.EnableDoubleBuffering();
    this.dgvResults.EnableExtendedAttributes();
    this.dgvResults.DataSource = this._items;
    this.dgvResults.Sort(this.dgvResults.Columns[0], ListSortDirection.Ascending);

    this.dgvResults.DataBindingComplete += (_, _) => {
      if (this.dgvResults.Columns["FileName"] is { } col)
        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
    };

    foreach (var item in Enum.GetValues(typeof(GuiMediaItem.LanguageType))) {
      this.tscbAudio0Language.Items.Add(item);
      this.tscbAudio1Language.Items.Add(item);
    }
  }

  private readonly SortableBindingList<GuiMediaItem> _items = [];

  private IEnumerable<string> FileRenameMasks => 
    Settings.Default.FileRenameMasks?.Cast<string>() 
    ?? [$"{RenamePlaceholders.Title}.{RenamePlaceholders.Extension}", $"{RenamePlaceholders.VideoName}.{RenamePlaceholders.Extension}"];

  #region IMainView Implementation

  /// <inheritdoc/>
  public IMainPresenter? Presenter { get; set; }

  /// <inheritdoc/>
  public IEnumerable<GuiMediaItem> SelectedItems =>
    this.dgvResults.GetSelectedItems<GuiMediaItem>();

  /// <inheritdoc/>
  public void AddItems(IEnumerable<GuiMediaItem> items) =>
    this.SafelyInvoke(() => {
      try {
        this.dgvResults.SuspendLayout();
        this._items.AddRange(items);
      } finally {
        this.dgvResults.ResumeLayout();
      }
      this.ReapplySorting();
    });

  /// <inheritdoc/>
  public void RemoveItems(IEnumerable<GuiMediaItem> items) => this.SafelyInvoke(() => this._items.RemoveRange(items.ToArray()));

  /// <inheritdoc/>
  public void ClearItems() => this._items.Clear();

  /// <inheritdoc/>
  public void SetLoadingIndicator(string indicatorKey, bool visible) =>
    this.SafelyInvoke(() => {
      var indicator = indicatorKey switch {
        IndicatorKeys.Loading => this.tsslLoadingFiles,
        IndicatorKeys.Committing => this.tsslCommittingChanges,
        IndicatorKeys.Converting => this.tsslConvertingFiles,
        _ => null
      };
      if (indicator is null)
        return;

      indicator.Visible = visible;
      if (!visible)
        indicator.Text = indicatorKey switch {
          IndicatorKeys.Loading => "Loading files...",
          IndicatorKeys.Committing => "Writing changes...",
          IndicatorKeys.Converting => "Converting files...",
          _ => indicator.Text
        };
    });

  /// <inheritdoc/>
  public void SetLoadingProgress(string indicatorKey, int accepted, int processed, int discovered, bool discoveryComplete) =>
    this.SafelyInvoke(() => {
      var indicator = indicatorKey switch {
        IndicatorKeys.Loading => this.tsslLoadingFiles,
        IndicatorKeys.Committing => this.tsslCommittingChanges,
        IndicatorKeys.Converting => this.tsslConvertingFiles,
        _ => null
      };
      if (indicator is null)
        return;

      var suffix = discoveryComplete ? string.Empty : "+";
      indicator.Text = $"Found {accepted} ({processed}/{discovered}{suffix})...";
    });

  /// <inheritdoc/>
  public void RefreshItems(IEnumerable<GuiMediaItem>? items = null) =>
    this.SafelyInvoke(() => this.dgvResults.Refresh());

  /// <inheritdoc/>
  public void ReapplySorting() {
    if (this.dgvResults.SortedColumn is not null)
      this.dgvResults.Sort(
        this.dgvResults.SortedColumn,
        this.dgvResults.SortOrder == SortOrder.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending);
  }

  /// <inheritdoc/>
  public void ShowError(string title, string message) =>
    this.SafelyInvoke(() => MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error));

  /// <inheritdoc/>
  public bool ShowConfirmation(string title, string message) {
    var result = DialogResult.No;
    this.SafelyInvoke(() =>
      result = MessageBox.Show(this, message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question));
    return result == DialogResult.Yes;
  }

  #region IMainView Events

  /// <inheritdoc/>
  public event EventHandler<FilesDroppedEventArgs>? FilesDropped;

  /// <inheritdoc/>
  public event EventHandler? RemoveSelectedRequested;

  /// <inheritdoc/>
  public event EventHandler? ClearAllRequested;

  /// <inheritdoc/>
  public event EventHandler? CommitSelectedRequested;

  /// <inheritdoc/>
  public event EventHandler? RevertSelectedRequested;

  /// <inheritdoc/>
  public event EventHandler? ConvertToMkvRequested;

  /// <inheritdoc/>
  public event EventHandler? TitleFromFilenameRequested;

  /// <inheritdoc/>
  public event EventHandler? VideoNameFromFilenameRequested;

  /// <inheritdoc/>
  public event EventHandler? FixTitleAndNameRequested;

  /// <inheritdoc/>
  public event EventHandler? RecoverSpacesRequested;

  /// <inheritdoc/>
  public event EventHandler? RemoveBracketContentRequested;

  /// <inheritdoc/>
  public event EventHandler? SwapTitleAndNameRequested;

  /// <inheritdoc/>
  public event EventHandler? ClearTitleRequested;

  /// <inheritdoc/>
  public event EventHandler? ClearVideoNameRequested;

  /// <inheritdoc/>
  public event EventHandler<RenameRequestedEventArgs>? RenameFilesRequested;

  /// <inheritdoc/>
  public event EventHandler<AudioLanguageChangedEventArgs>? AudioLanguageChanged;

  #endregion

  #endregion

  private void _BuildFileRenameMenu() {
    var parent = this.tsddbRenameFiles;
    parent.DropDownItems.Clear();
    foreach (var entry in this.FileRenameMasks) {
      parent.DropDownItems.Add(
        new ToolStripMenuItem(
          string.Format(Resources.Menu_RenameFilesTo, entry),
          null,
          (_, _) => this.RenameFilesRequested?.Invoke(this, new RenameRequestedEventArgs(entry))
        )
      );
    }
  }

  #region events

  /// <summary>
  /// Allows drops when they're FileDrops.
  /// </summary>
  private void dgvResults_DragEnter(object? _, DragEventArgs e)
    => e.Effect = e.Data?.GetDataPresent(DataFormats.FileDrop) == true ? DragDropEffects.Link : DragDropEffects.None;

  /// <summary>
  /// Adds all files and recursively all folders' files if dropped.
  /// </summary>
  private void dgvResults_DragDrop(object? _, DragEventArgs e) {
    if (e.Data?.GetData(DataFormats.FileDrop) is not string[] files || files.Length == 0)
      return;

    this.FilesDropped?.Invoke(this, new FilesDroppedEventArgs(files));
  }

  /// <summary>
  /// Removes selected items from dgv.
  /// </summary>
  private void tsmiRemoveItem_Click(object? _, EventArgs __) =>
    this.RemoveSelectedRequested?.Invoke(this, EventArgs.Empty);

  /// <summary>
  /// Removes all items from dgv.
  /// </summary>
  private void tsmiClearItems_Click(object? _, EventArgs __) =>
    this.ClearAllRequested?.Invoke(this, EventArgs.Empty);

  /// <summary>
  /// Commits changes in all selected items.
  /// </summary>
  private void tsmiCommitSelected_Click(object? _, EventArgs __) =>
    this.CommitSelectedRequested?.Invoke(this, EventArgs.Empty);

  /// <summary>
  /// Reverts any changes made in selected items.
  /// </summary>
  private void tsmiRevertChanges_Click(object? _, EventArgs __) =>
    this.RevertSelectedRequested?.Invoke(this, EventArgs.Empty);

  /// <summary>
  /// Enables/Disables context menu entries, depending on current application state.
  /// </summary>
  private void cmsItems_Opening(object? _, CancelEventArgs e) {
    this._duringMenuPreset = true;

    var selectedItems = this.SelectedItems.ToArray();
    e.Cancel = selectedItems.Length == 0;

    this.tsmiCommitSelected.Enabled =
      this.tsmiRevertChanges.Enabled =
        selectedItems.Any(static i => i.NeedsCommit);

    if (this.tsmiAudio0.Enabled = selectedItems.Any(static i => i.HasAudio0))
      this.tscbAudio0Language.SelectedItem = selectedItems.Select(static i => i.Audio0Language).Distinct().OneOrDefault();
    else
      this.tscbAudio0Language.SelectedItem = null;

    if (this.tsmiAudio1.Enabled = selectedItems.Any(static i => i.HasAudio1))
      this.tscbAudio1Language.SelectedItem = selectedItems.Select(static i => i.Audio1Language).Distinct().OneOrDefault();
    else
      this.tscbAudio1Language.SelectedItem = null;

    this.tsmiConvertToMkv.Enabled = selectedItems.Any(static i => i.IsMkvConversionEnabled);
    this.tsmiOpenContainingFolder.Enabled = selectedItems.Length == 1;

    this._duringMenuPreset = false;
  }

  private void tsmiCopyFilePath_Click(object? _, EventArgs __) {
    var paths = this.SelectedItems.Select(static i => i.MediaFile.File.FullName).ToArray();
    if (paths.Length == 0)
      return;

    var text = string.Join(Environment.NewLine, paths);
    Clipboard.SetText(text);
  }

  private void tsmiOpenContainingFolder_Click(object? _, EventArgs __) {
    var selectedItems = this.SelectedItems.ToArray();
    if (selectedItems.Length != 1)
      return;

    var file = selectedItems[0].MediaFile.File;
    if (!file.Exists)
      return;

    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo {
      FileName = "explorer.exe",
      Arguments = $"/select,\"{file.FullName}\"",
      UseShellExecute = true
    });
  }

  /// <summary>
  /// Handles the SelectionChanged event of the dgvResults control.
  /// </summary>
  private void dgvResults_SelectionChanged(object? _, EventArgs __) {
    var isAnyFileSelected = this.SelectedItems.Any();
    this.tsddbTagsFromName.Enabled =
      this.tsddbRenameFiles.Enabled =
        this.tsddbRenameFolders.Enabled =
          isAnyFileSelected;
  }

  /// <summary>
  /// Handles the Click event of the tsmiTitleFromFilename control.
  /// </summary>
  private void tsmiTitleFromFilename_Click(object? _, EventArgs __) =>
    this.TitleFromFilenameRequested?.Invoke(this, EventArgs.Empty);

  /// <summary>
  /// Handles the Click event of the tsmiVideNameFromFileName control.
  /// </summary>
  private void tsmiVideoNameFromFileName_Click(object? _, EventArgs __) =>
    this.VideoNameFromFilenameRequested?.Invoke(this, EventArgs.Empty);

  /// <summary>
  /// Handles the Click event of the tsmiFixTitleAndName control.
  /// </summary>
  private void tsmiFixTitleAndName_Click(object? _, EventArgs __) =>
    this.FixTitleAndNameRequested?.Invoke(this, EventArgs.Empty);

  private void tsddbTagsFromName_DropDownOpening(object? sender, EventArgs e)
    => this.tsmiNfo.Enabled = this.SelectedItems.Any(static i => i.HasNfo);

  private void _WorkWithNfo(Action<Movie, GuiMediaItem> movieAction, Action<EpisodeDetails, GuiMediaItem> episodeAction) {
    Parallel.ForEach(
      this.SelectedItems.Where(static i => i is { HasNfo: true, IsReadOnly: false }).ToArray(),
      item => {
        var file = item.MediaFile.File;
        var movie = NfoFile.LoadMovieOrNull(file);
        if (movie is not null)
          movieAction(movie, item);

        var episode = NfoFile.LoadEpisodeOrNull(file);
        if (episode is not null)
          episodeAction(episode, item);
      }
    );
  }

  private void tsmiTitleFromNfoTitle_Click(object? _, EventArgs __)
    => this._WorkWithNfo((m, i) => i.Title = m.Title, (e, i) => i.Title = e.Title);

  private void tsmiTitleFromNfoOriginalTitle_Click(object? _, EventArgs __)
    => this._WorkWithNfo((m, i) => i.Title = m.OriginalTitle, (e, i) => i.Title = e.OriginalTitle);

  private void tsmiNameFromNfoTitle_Click(object? _, EventArgs __)
    => this._WorkWithNfo((m, i) => i.Video0Name = m.Title, (e, i) => i.Video0Name = e.Title);

  private void tsmiNameFromNfoOriginalTitle_Click(object? _, EventArgs __)
    => this._WorkWithNfo((m, i) => i.Video0Name = m.OriginalTitle, (e, i) => i.Video0Name = e.OriginalTitle);

  private void tsddbTagsFromName_Click(object? _, EventArgs __) {
    // TODO: this should open a window where one can build his template
  }

  private void tsddbRenameFiles_Click(object? _, EventArgs __) {
    // TODO: this should open a window where one can build his template
  }

  private void tsddbRenameFolders_Click(object? _, EventArgs __) {
    // TODO: this should open a window where one can build his template
  }

  /// <summary>
  /// Handles the Click event of the tsmiClearTitle control.
  /// </summary>
  private void tsmiClearTitle_Click(object? _, EventArgs __) =>
    this.ClearTitleRequested?.Invoke(this, EventArgs.Empty);

  /// <summary>
  /// Handles the Click event of the tsmiClearVideoName control.
  /// </summary>
  private void tsmiClearVideoName_Click(object? _, EventArgs __) =>
    this.ClearVideoNameRequested?.Invoke(this, EventArgs.Empty);

  /// <summary>
  /// Handles the Click event of the tsmiSwapTitleAndName control.
  /// </summary>
  private void tsmiSwapTitleAndName_Click(object? _, EventArgs __) =>
    this.SwapTitleAndNameRequested?.Invoke(this, EventArgs.Empty);

  /// <summary>
  /// Handles the Click event of the tsmiRecoverSpaces control.
  /// </summary>
  private void tsmiRecoverSpaces_Click(object? _, EventArgs __) =>
    this.RecoverSpacesRequested?.Invoke(this, EventArgs.Empty);

  /// <summary>
  /// Handles the Click event of the tsmiRemoveBracketContent control.
  /// </summary>
  private void tsmiRemoveBracketContent_Click(object? _, EventArgs __) =>
    this.RemoveBracketContentRequested?.Invoke(this, EventArgs.Empty);

  private bool _duringMenuPreset;

  private void tscbAudio1Language_SelectedIndexChanged(object? s, EventArgs _) {
    if (this._duringMenuPreset)
      return;

    if (s is not ToolStripComboBox { SelectedItem: GuiMediaItem.LanguageType value })
      return;

    this.AudioLanguageChanged?.Invoke(this, new AudioLanguageChangedEventArgs(0, value));
  }

  private void tscbAudio2Language_SelectedIndexChanged(object? s, EventArgs _) {
    if (this._duringMenuPreset)
      return;

    if (s is not ToolStripComboBox { SelectedItem: GuiMediaItem.LanguageType value })
      return;

    this.AudioLanguageChanged?.Invoke(this, new AudioLanguageChangedEventArgs(1, value));
  }

  private void tsmiConvertToMkv_Click(object? _, EventArgs __) =>
    this.ConvertToMkvRequested?.Invoke(this, EventArgs.Empty);

  #endregion

  #region statics

  private void tsmiAutoFillFromFileName_Click(object? sender, EventArgs e) {
    this.TitleFromFilenameRequested?.Invoke(this, EventArgs.Empty);
    this.FixTitleAndNameRequested?.Invoke(this, EventArgs.Empty);
    this.RecoverSpacesRequested?.Invoke(this, EventArgs.Empty);
    this.RemoveBracketContentRequested?.Invoke(this, EventArgs.Empty);
  }

  #endregion
}

internal static class EnumerableExtensions {
  public static T? OneOrDefault<T>(this IEnumerable<T>? @this, T? defaultValue = default) {
    if (@this is null)
      return defaultValue;

    using var enumerator = @this.GetEnumerator();
    if (!enumerator.MoveNext())
      return defaultValue;

    var result = enumerator.Current;
    if (enumerator.MoveNext())
      return defaultValue;

    return result;
  }
}