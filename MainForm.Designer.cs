namespace MassMediaEdit {
  partial class MainForm {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      System.Windows.Forms.StatusStrip statusStrip1;
      System.Windows.Forms.ContextMenuStrip cmsItems;
      System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
      System.Windows.Forms.ToolStripMenuItem tsmiClearItems;
      System.Windows.Forms.ToolStripMenuItem tsmiRemoveItem;
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
      this.tsslDragDropInfo = new System.Windows.Forms.ToolStripStatusLabel();
      this.dgvResults = new System.Windows.Forms.DataGridView();
      this.tsmiCommitSelected = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmiRevertChanges = new System.Windows.Forms.ToolStripMenuItem();
      this.tsslCommittingChanges = new System.Windows.Forms.ToolStripStatusLabel();
      this.tsslLoadingFiles = new System.Windows.Forms.ToolStripStatusLabel();
      statusStrip1 = new System.Windows.Forms.StatusStrip();
      cmsItems = new System.Windows.Forms.ContextMenuStrip(this.components);
      toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      tsmiClearItems = new System.Windows.Forms.ToolStripMenuItem();
      tsmiRemoveItem = new System.Windows.Forms.ToolStripMenuItem();
      statusStrip1.SuspendLayout();
      cmsItems.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).BeginInit();
      this.SuspendLayout();
      // 
      // statusStrip1
      // 
      statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslCommittingChanges,
            this.tsslLoadingFiles,
            this.tsslDragDropInfo});
      statusStrip1.Location = new System.Drawing.Point(0, 405);
      statusStrip1.Name = "statusStrip1";
      statusStrip1.Size = new System.Drawing.Size(818, 22);
      statusStrip1.TabIndex = 1;
      statusStrip1.Text = "statusStrip1";
      // 
      // tsslDragDropInfo
      // 
      this.tsslDragDropInfo.Name = "tsslDragDropInfo";
      this.tsslDragDropInfo.Size = new System.Drawing.Size(229, 17);
      this.tsslDragDropInfo.Text = "Please Drag && Drop Files into this Window";
      // 
      // cmsItems
      // 
      cmsItems.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            tsmiClearItems,
            toolStripSeparator1,
            tsmiRemoveItem,
            this.tsmiCommitSelected,
            this.tsmiRevertChanges});
      cmsItems.Name = "cmsItems";
      cmsItems.Size = new System.Drawing.Size(119, 98);
      cmsItems.Opening += new System.ComponentModel.CancelEventHandler(this.cmsItems_Opening);
      // 
      // toolStripSeparator1
      // 
      toolStripSeparator1.Name = "toolStripSeparator1";
      toolStripSeparator1.Size = new System.Drawing.Size(115, 6);
      // 
      // dgvResults
      // 
      this.dgvResults.AllowDrop = true;
      this.dgvResults.AllowUserToAddRows = false;
      this.dgvResults.AllowUserToDeleteRows = false;
      this.dgvResults.AllowUserToResizeRows = false;
      this.dgvResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
      this.dgvResults.BackgroundColor = System.Drawing.SystemColors.Window;
      this.dgvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgvResults.ContextMenuStrip = cmsItems;
      this.dgvResults.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dgvResults.Location = new System.Drawing.Point(0, 0);
      this.dgvResults.Name = "dgvResults";
      this.dgvResults.RowHeadersVisible = false;
      this.dgvResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.dgvResults.Size = new System.Drawing.Size(818, 405);
      this.dgvResults.TabIndex = 0;
      this.dgvResults.DragDrop += new System.Windows.Forms.DragEventHandler(this.dgvResults_DragDrop);
      this.dgvResults.DragEnter += new System.Windows.Forms.DragEventHandler(this.dgvResults_DragEnter);
      // 
      // tsmiClearItems
      // 
      tsmiClearItems.Image = global::MassMediaEdit.Properties.Resources.Clear;
      tsmiClearItems.Name = "tsmiClearItems";
      tsmiClearItems.Size = new System.Drawing.Size(118, 22);
      tsmiClearItems.Text = "Clear";
      tsmiClearItems.Click += new System.EventHandler(this.tsmiClearItems_Click);
      // 
      // tsmiRemoveItem
      // 
      tsmiRemoveItem.Image = global::MassMediaEdit.Properties.Resources._24x24_Delete__2_;
      tsmiRemoveItem.Name = "tsmiRemoveItem";
      tsmiRemoveItem.Size = new System.Drawing.Size(118, 22);
      tsmiRemoveItem.Text = "Remove";
      tsmiRemoveItem.Click += new System.EventHandler(this.tsmiRemoveItem_Click);
      // 
      // tsmiCommitSelected
      // 
      this.tsmiCommitSelected.Image = global::MassMediaEdit.Properties.Resources._16x16_Blue_Disk;
      this.tsmiCommitSelected.Name = "tsmiCommitSelected";
      this.tsmiCommitSelected.Size = new System.Drawing.Size(118, 22);
      this.tsmiCommitSelected.Text = "Commit";
      this.tsmiCommitSelected.Click += new System.EventHandler(this.tsmiCommitSelected_Click);
      // 
      // tsmiRevertChanges
      // 
      this.tsmiRevertChanges.Image = global::MassMediaEdit.Properties.Resources._16x16_Undo;
      this.tsmiRevertChanges.Name = "tsmiRevertChanges";
      this.tsmiRevertChanges.Size = new System.Drawing.Size(118, 22);
      this.tsmiRevertChanges.Text = "Revert";
      this.tsmiRevertChanges.Click += new System.EventHandler(this.tsmiRevertChanges_Click);
      // 
      // tsslCommittingChanges
      // 
      this.tsslCommittingChanges.Image = global::MassMediaEdit.Properties.Resources._16x11_Loading_Animation;
      this.tsslCommittingChanges.Name = "tsslCommittingChanges";
      this.tsslCommittingChanges.Size = new System.Drawing.Size(118, 17);
      this.tsslCommittingChanges.Text = "Writing changes...";
      this.tsslCommittingChanges.Visible = false;
      // 
      // tsslLoadingFiles
      // 
      this.tsslLoadingFiles.Image = global::MassMediaEdit.Properties.Resources._16x11_Loading_Animation;
      this.tsslLoadingFiles.Name = "tsslLoadingFiles";
      this.tsslLoadingFiles.Size = new System.Drawing.Size(99, 17);
      this.tsslLoadingFiles.Text = "Loading files...";
      this.tsslLoadingFiles.Visible = false;
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(818, 427);
      this.Controls.Add(this.dgvResults);
      this.Controls.Add(statusStrip1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "MainForm";
      this.Text = "MainForm";
      statusStrip1.ResumeLayout(false);
      statusStrip1.PerformLayout();
      cmsItems.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.DataGridView dgvResults;
    private System.Windows.Forms.ToolStripStatusLabel tsslDragDropInfo;
    private System.Windows.Forms.ToolStripMenuItem tsmiCommitSelected;
    private System.Windows.Forms.ToolStripMenuItem tsmiRevertChanges;
    private System.Windows.Forms.ToolStripStatusLabel tsslCommittingChanges;
    private System.Windows.Forms.ToolStripStatusLabel tsslLoadingFiles;
  }
}