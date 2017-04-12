using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Classes;
using Classes.GUI;

namespace MassMediaEdit {
  public partial class MainForm : Form {

    private readonly SortableBindingList<GuiMediaItem> _items = new SortableBindingList<GuiMediaItem>();

    public MainForm() {
      this.InitializeComponent();
      this.SetFormTitle();

      this.dgvResults.EnableExtendedAttributes();
      this.dgvResults.DataSource = this._items;
      this.dgvResults.Sort(this.dgvResults.Columns[0], ListSortDirection.Ascending);
    }

    private void _AddResults(IEnumerable<GuiMediaItem> items) {
      if (items == null)
        return;

      this.SafelyInvoke(() => this._items.AddRange(items));
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

      this._AddResults(items.ToArray());

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
    private void tsmiRemoveItem_Click(object _, System.EventArgs __)
          => this._items.RemoveRange(this.dgvResults.GetSelectedItems<GuiMediaItem>())
          ;

    /// <summary>
    /// Removes all items from dgv.
    /// </summary>
    /// <param name="_">The source of the event.</param>
    /// <param name="__">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    private void tsmiClearItems_Click(object _, System.EventArgs __)
          => this._items.Clear()
          ;

    /// <summary>
    /// Commits changes in all selected items.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    private void tsmiCommitSelected_Click(object sender, System.EventArgs e) {
      var items = this.dgvResults.GetSelectedItems<GuiMediaItem>().ToArray();
      this._items.RemoveRange(items);

      this._ExecuteBackgroundTask("Commit", () => {
        Parallel.ForEach(items, i => i.CommitChanges());
        this.SafelyInvoke(() => this._items.AddRange(items));
      }, this.tsslCommittingChanges);
    }

    /// <summary>
    /// Reverts any changes made in selected items.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    private void tsmiRevertChanges_Click(object sender, System.EventArgs e)
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

    #endregion

  }
}
