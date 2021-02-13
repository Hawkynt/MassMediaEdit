using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Classes;
using Classes.GUI;

// TODO: get title from nfo
// TODO: get originaltitle from nfo

namespace MassMediaEdit {
  public partial class MainForm : Form {
    private const string _MN_RENAME_FILES_TO = "Rename to {0}";

    private readonly SortableBindingList<GuiMediaItem> _items = new SortableBindingList<GuiMediaItem>();

    // TODO: should be configured by the user and stored in configuration
    private readonly string[] _fileRenameMasks = {
      "{title}.{extension}",
      "{video:name}.{extension}",
    };

    internal static ISynchronizeInvoke Invocator { get; private set; }

    public MainForm() {
      Invocator = this;
      this.InitializeComponent();
      this.SetFormTitle();
      this._BuildFileRenameMenu();

      this.dgvResults.EnableExtendedAttributes();
      this.dgvResults.DataSource = this._items;
      this.dgvResults.Sort(this.dgvResults.Columns[0], ListSortDirection.Ascending);
    }

    private void _BuildFileRenameMenu() {
      var parent = this.tsddbRenameFiles;
      parent.DropDownItems.Clear();
      foreach (var entry in this._fileRenameMasks) {
        parent.DropDownItems.Add(
          new ToolStripMenuItem(
            string.Format(_MN_RENAME_FILES_TO, entry),
            null,
            (_, __) => this.dgvResults.GetSelectedItems<GuiMediaItem>().ForEach<GuiMediaItem>(i => i.RenameFileToMask(entry))
          )
        );
      }
    }

    private void _AddResults(IEnumerable<GuiMediaItem> items) {
      if (items == null)
        return;

      this.SafelyInvoke(
        () => {
          this._items.AddRange(items);
          this._ReapplySorting();
        }
      );
    }

    private void _AddResult(GuiMediaItem item) {
      if (item == null)
        return;

      this.SafelyInvoke(
        () => {
          this._items.Add(item);
          this._ReapplySorting();
        }
      );
    }

    /// <summary>
    /// Reapplies sorting of the dgv.
    /// </summary>
    private void _ReapplySorting() {
      if (this.dgvResults.SortedColumn != null)
        this.dgvResults.Sort(
          this.dgvResults.SortedColumn,
          this.dgvResults.SortOrder == SortOrder.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending);
    }

    /// <summary>
    /// Adds the specified media file.
    /// </summary>
    /// <param name="file">The file.</param>
    internal void AddFile(FileInfo file) => this._AddFiles(new[] { file });

    /// <summary>
    /// Adds the given media files.
    /// </summary>
    /// <param name="files">The files.</param>
    private void _AddFiles(IEnumerable<FileInfo> files) {
      var items = files.AsParallel()
        .Select(MediaFile.FromFile)                               /* read/parse file */
        .Where(m => m.AudioStreams.Any() || m.VideoStreams.Any()) /* only add media files */
        .Select(GuiMediaItem.FromMediaFile)                       /* convert to GUI item instance */
        ;

      foreach (var item in items.WithMergeOptions(ParallelMergeOptions.NotBuffered))
        this._AddResult(item);
    }

    private readonly ConcurrentDictionary<string, int[]> _runningBackgroundTasks = new ConcurrentDictionary<string, int[]>();

    /// <summary>
    /// Executes a background task.
    /// Shows/Hides an indicator and keeps track of actions using the same indicator, identified by a tag name.
    /// </summary>
    /// <param name="tag">The tag.</param>
    /// <param name="task">The task.</param>
    /// <param name="progressIndicator">The progress indicator.</param>
    private void _ExecuteBackgroundTask(string tag, Action task, ToolStripItem progressIndicator) {
      this.Async(() => {

        var taskCounter = this._runningBackgroundTasks.GetOrAdd(tag, () => new[] { 0 });
        try {

          // increment running number of tasks and show indicator
          Interlocked.Increment(ref taskCounter[0]);
          this.SafelyInvoke(() => progressIndicator.Visible = true);

          task();

        } finally {

          // when no more tasks with this tag, hide indicator
          if (Interlocked.Decrement(ref taskCounter[0]) < 1)
            this.SafelyInvoke(() => progressIndicator.Visible = false);
        }
      });
    }

    #region events 

    /// <summary>
    /// Allows drops when they're FileDrops.
    /// </summary>
    /// <param name="_">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.Forms.DragEventArgs" /> instance containing the event data.</param>
    private void dgvResults_DragEnter(object _, DragEventArgs e)
          => e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Link : DragDropEffects.None
          ;

    /// <summary>
    /// Adds all files and recursively all folders' files if dropped.
    /// </summary>
    /// <param name="_">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.Forms.DragEventArgs" /> instance containing the event data.</param>
    private void dgvResults_DragDrop(object _, DragEventArgs e) {
      var files = e.Data.GetData(DataFormats.FileDrop) as string[];
      var infos =
        files?
        .Select(i => File.Exists(i) ? (FileSystemInfo)new FileInfo(i) : new DirectoryInfo(i))
        .Where(i => i.Exists)
        .ToArray()
        ;

      if (infos == null || infos.Length < 1)
        return;

      this._ExecuteBackgroundTask("DragDrop", () => {
        this._AddFiles(infos.OfType<FileInfo>());
        foreach (var info in infos.OfType<DirectoryInfo>())
          this._AddFiles(info.EnumerateFiles("*.*", SearchOption.AllDirectories));
      }, this.tsslLoadingFiles);
    }

    /// <summary>
    /// Removes selected items from dgv.
    /// </summary>
    /// <param name="_">The source of the event.</param>
    /// <param name="__">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    private void tsmiRemoveItem_Click(object _, EventArgs __)
          => this._items.RemoveRange(this.dgvResults.GetSelectedItems<GuiMediaItem>())
          ;

    /// <summary>
    /// Removes all items from dgv.
    /// </summary>
    /// <param name="_">The source of the event.</param>
    /// <param name="__">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    private void tsmiClearItems_Click(object _, EventArgs __)
          => this._items.Clear()
          ;

    /// <summary>
    /// Commits changes in all selected items.
    /// </summary>
    /// <param name="_">The source of the event.</param>
    /// <param name="__">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    private void tsmiCommitSelected_Click(object _, EventArgs __) {
        var items = this.dgvResults.GetSelectedItems<GuiMediaItem>().ToArray();
        this._items.RemoveRange(items);

        this._ExecuteBackgroundTask("Commit", () => {
            var partitioner = Partitioner.Create(items, true);

            var bag = new ConcurrentBag<GuiMediaItem>();
            var bufferDuration = TimeSpan.FromSeconds(2);
            var updateTicks = (long) (bufferDuration.TotalSeconds * Stopwatch.Frequency);
            var nextUpdate = Stopwatch.GetTimestamp() + updateTicks;

            void Update() {
                var curr = new List<GuiMediaItem>(bag.Count);
                while (bag.TryTake(out var i))
                    curr.Add(i);

                this.SafelyInvoke(() => this._items.AddRange(curr));
            }

            Parallel.ForEach(
                partitioner.GetDynamicPartitions(),
                new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount},
                (i, state) => {
                    i.CommitChanges();
                    bag.Add(i);
                    if (Stopwatch.GetTimestamp() < nextUpdate)
                        return;

                    var old = nextUpdate;
                    if (Interlocked.CompareExchange(ref nextUpdate, Stopwatch.GetTimestamp() + updateTicks, old) == old)
                        Update();
                }
            );

            Update();

            //Parallel.ForEach(items, i => i.CommitChanges());
            //this.SafelyInvoke(() => this._items.AddRange(items));

        }, this.tsslCommittingChanges);
    }

    /// <summary>
    /// Reverts any changes made in selected items.
    /// </summary>
    /// <param name="_">The source of the event.</param>
    /// <param name="__">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    private void tsmiRevertChanges_Click(object _, EventArgs __)
             => this.dgvResults.GetSelectedItems<GuiMediaItem>().ForEach<GuiMediaItem>(i => i.RevertChanges())
          ;

    /// <summary>
    /// Enables/Disables context menu entries, depending on current application state.
    /// </summary>
    /// <param name="_">The source of the event.</param>
    /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs" /> instance containing the event data.</param>
    private void cmsItems_Opening(object _, CancelEventArgs e) {
      var selectedItems = this.dgvResults.GetSelectedItems<GuiMediaItem>().ToArray();
      e.Cancel = !selectedItems.Any();
      this.tsmiCommitSelected.Enabled =
        this.tsmiRevertChanges.Enabled =
        selectedItems.Any(i => i.NeedsCommit);
    }

    /// <summary>
    /// Handles the SelectionChanged event of the dgvResults control.
    /// </summary>
    /// <param name="_">The source of the event.</param>
    /// <param name="__">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    private void dgvResults_SelectionChanged(object _, EventArgs __) {
      var isAnyFileSelected = this.dgvResults.GetSelectedItems<GuiMediaItem>().Any();
      this.tsddbTagsFromName.Enabled =
        this.tsddbRenameFiles.Enabled =
        this.tsddbRenameFolders.Enabled =
        isAnyFileSelected
        ;
    }

    /// <summary>
    /// Handles the Click event of the tsmiTitleFromFilename control.
    /// </summary>
    /// <param name="_">The source of the event.</param>
    /// <param name="__">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    private void tsmiTitleFromFilename_Click(object _, EventArgs __) {
      foreach (var item in this.dgvResults.GetSelectedItems<GuiMediaItem>().Where(i => !i.IsReadOnly))
        item.Title = item.MediaFile.File.GetFilenameWithoutExtension();
    }

    /// <summary>
    /// Handles the Click event of the tsmiVideNameFromFileName control.
    /// </summary>
    /// <param name="_">The source of the event.</param>
    /// <param name="__">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    private void tsmiVideoNameFromFileName_Click(object _, EventArgs __) {
      foreach (var item in this.dgvResults.GetSelectedItems<GuiMediaItem>().Where(i => !i.IsReadOnly))
        item.Video0Name = item.MediaFile.File.GetFilenameWithoutExtension();
    }

    /// <summary>
    /// Handles the Click event of the tsmiFixTitleAndName control.
    /// </summary>
    /// <param name="_">The source of the event.</param>
    /// <param name="__">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    private void tsmiFixTitleAndName_Click(object _, EventArgs __) {
      var regex = new Regex(@"s(?<season>\d+)e(?<episode>\d+)(?:(?:\s*[-\._]\s*)|(?:\s+))(?<title>.*?)\s*$", RegexOptions.IgnoreCase);
      foreach (var item in this.dgvResults.GetSelectedItems<GuiMediaItem>().Where(i => !i.IsReadOnly)) {
        var match = regex.Match(item.Title ?? string.Empty);
        if (!match.Success)
          match = regex.Match(item.Video0Name ?? string.Empty);

        if (!match.Success)
          continue;

        item.Title = $"s{match.Groups["season"].Value}e{match.Groups["episode"].Value}";
        var name = match.Groups["title"].Value;
        if (name.IsNotNullOrWhiteSpace())
          item.Video0Name = name;
      }
    }

    private void tsddbTagsFromName_Click(object _, EventArgs __) {
      // TODO: this should open a window where one can build his template
    }

    private void tsddbRenameFiles_Click(object _, EventArgs __) {
      // TODO: this should open a window where one can build his template
    }

    private void tsddbRenameFolders_Click(object _, EventArgs __) {
      // TODO: this should open a window where one can build his template
    }

    /// <summary>
    /// Handles the Click event of the tsmiClearTitle control.
    /// </summary>
    /// <param name="_">The source of the event.</param>
    /// <param name="__">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    private void tsmiClearTitle_Click(object _, EventArgs __) {
      foreach (var item in this.dgvResults.GetSelectedItems<GuiMediaItem>().Where(i => !i.IsReadOnly))
        item.Title = null;
    }

    /// <summary>
    /// Handles the Click event of the tsmiClearVideoName control.
    /// </summary>
    /// <param name="_">The source of the event.</param>
    /// <param name="__">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    private void tsmiClearVideoName_Click(object _, EventArgs __) {
      foreach (var item in this.dgvResults.GetSelectedItems<GuiMediaItem>().Where(i => !i.IsReadOnly))
        item.Video0Name = null;
    }

    /// <summary>
    /// Handles the Click event of the tsmiSwapTitleAndName control.
    /// </summary>
    /// <param name="_">The source of the event.</param>
    /// <param name="__">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    private void tsmiSwapTitleAndName_Click(object _, EventArgs __) {
      foreach (var item in this.dgvResults.GetSelectedItems<GuiMediaItem>().Where(i => !i.IsReadOnly)) {
        var temp = item.Video0Name;
        item.Video0Name = item.Title;
        item.Title = temp;
      }
    }

    /// <summary>
    /// Handles the Click event of the tsmiRecoverSpaces control.
    /// </summary>
    /// <param name="_">The source of the event.</param>
    /// <param name="__">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    private void tsmiRecoverSpaces_Click(object _, EventArgs __) {
      foreach (var item in this.dgvResults.GetSelectedItems<GuiMediaItem>().Where(i => !i.IsReadOnly)) {
        item.Video0Name = _RecoverSpacesFrom(item.Video0Name);
        item.Title = _RecoverSpacesFrom(item.Title);
      }
    }

    /// <summary>
    /// Handles the Click event of the tsmiRemoveBracketContent control.
    /// </summary>
    /// <param name="_">The source of the event.</param>
    /// <param name="__">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    private void tsmiRemoveBracketContent_Click(object _, EventArgs __) {
      foreach (var item in this.dgvResults.GetSelectedItems<GuiMediaItem>().Where(i => !i.IsReadOnly)) {
        item.Video0Name = _RemoveBracketContentFrom(item.Video0Name);
        item.Title = _RemoveBracketContentFrom(item.Title);
      }
    }

    #endregion

    #region statics

    /// <summary>
    /// Tries to recover spaces from a text without ones.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns></returns>
    private static string _RecoverSpacesFrom(string text) {

      // no text - no result
      if (text == null)
        return null;

      // if already spaces in it, we won't recover anything
      if (text.Contains(" "))
        return text;

      // replace url type space
      if (text.Contains("%20"))
        return text.Replace("%20", " ");

      // try characters which are normally used to replace a space character
      const string charactersKnownToReplaceSpace = "_.%-+";
      foreach (var character in charactersKnownToReplaceSpace)
        if (text.Contains(character))
          return text.Replace(character, ' ');

      // couldn't find anything to recover
      return text;
    }

    /// <summary>
    /// Removes bracketed content from the given text.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns></returns>
    private static string _RemoveBracketContentFrom(string text) {
      var regex = new Regex(@"\s*((\(.*?\))|(\[\.*?])|(\{.*?\})|(\<.*?\>))\s*");
      if (text == null)
        return null;

      return regex.Replace(text, string.Empty);
    }

    private void tsmiAutoFillFromFileName_Click(object sender, EventArgs e) {
      this.tsmiTitleFromFilename_Click(sender, e);
      this.tsmiFixTitleAndName_Click(sender, e);
      this.tsmiFixTitleAndName_Click(sender, e);
      this.tsmiRecoverSpaces_Click(sender, e);
      this.tsmiRemoveBracketContent_Click(sender, e);
    }

    #endregion

  }
}
