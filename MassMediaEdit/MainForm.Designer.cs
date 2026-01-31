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
      System.Windows.Forms.ToolStrip tsMainStrip;
      System.Windows.Forms.ToolStripMenuItem tsmiClearItems;
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
      System.Windows.Forms.ToolStripMenuItem tsmiRemoveItem;
      System.Windows.Forms.ToolStripMenuItem tsmiTitleFromNfoTitle;
      System.Windows.Forms.ToolStripMenuItem tsmiTitleFromNfoOriginalTitle;
      System.Windows.Forms.ToolStripMenuItem tsmiNameFromNfoTitle;
      System.Windows.Forms.ToolStripMenuItem tsmiNameFromNfoOriginalTitle;
      System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
      this.tsslDragDropInfo = new System.Windows.Forms.ToolStripStatusLabel();
      this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
      this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
      this.tsmiCopyFilePath = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmiOpenContainingFolder = new System.Windows.Forms.ToolStripMenuItem();
      this.dgvResults = new System.Windows.Forms.DataGridView();
      this.tsmiConvertToMkv = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmiCommitSelected = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmiRevertChanges = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmiAudio0 = new System.Windows.Forms.ToolStripMenuItem();
      this.tscbAudio0Language = new System.Windows.Forms.ToolStripComboBox();
      this.tsmiAudio1 = new System.Windows.Forms.ToolStripMenuItem();
      this.tscbAudio1Language = new System.Windows.Forms.ToolStripComboBox();
      this.tsddbRenameFiles = new System.Windows.Forms.ToolStripDropDownButton();
      this.tsddbRenameFolders = new System.Windows.Forms.ToolStripDropDownButton();
      this.tsddbTagsFromName = new System.Windows.Forms.ToolStripDropDownButton();
      this.tsmiTitleFromFileName = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmiVideoNameFromFileName = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmiFixTitleAndName = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmiClearTitle = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmiClearVideoName = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmiSwapTitleAndName = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmiRecoverSpaces = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmiRemoveBracketContent = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmiNfo = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmiAutoFillFromFileName = new System.Windows.Forms.ToolStripMenuItem();
      this.tsslCommittingChanges = new System.Windows.Forms.ToolStripStatusLabel();
      this.tsslLoadingFiles = new System.Windows.Forms.ToolStripStatusLabel();
      this.tsslConvertingFiles = new System.Windows.Forms.ToolStripStatusLabel();
      statusStrip1 = new System.Windows.Forms.StatusStrip();
      cmsItems = new System.Windows.Forms.ContextMenuStrip(this.components);
      toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      tsMainStrip = new System.Windows.Forms.ToolStrip();
      tsmiClearItems = new System.Windows.Forms.ToolStripMenuItem();
      tsmiRemoveItem = new System.Windows.Forms.ToolStripMenuItem();
      tsmiTitleFromNfoTitle = new System.Windows.Forms.ToolStripMenuItem();
      tsmiTitleFromNfoOriginalTitle = new System.Windows.Forms.ToolStripMenuItem();
      tsmiNameFromNfoTitle = new System.Windows.Forms.ToolStripMenuItem();
      tsmiNameFromNfoOriginalTitle = new System.Windows.Forms.ToolStripMenuItem();
      toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      statusStrip1.SuspendLayout();
      cmsItems.SuspendLayout();
      tsMainStrip.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).BeginInit();
      this.SuspendLayout();
      // 
      // statusStrip1
      // 
      statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslCommittingChanges,
            this.tsslLoadingFiles,
            this.tsslDragDropInfo,
            this.tsslConvertingFiles});
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
            this.tsmiRevertChanges,
            this.toolStripSeparator3,
            this.tsmiAudio0,
            this.tsmiAudio1,
            this.tsmiConvertToMkv,
            this.toolStripSeparator4,
            this.tsmiCopyFilePath,
            this.tsmiOpenContainingFolder});
      cmsItems.Name = "cmsItems";
      cmsItems.Size = new System.Drawing.Size(181, 192);
      cmsItems.Opening += new System.ComponentModel.CancelEventHandler(this.cmsItems_Opening);
      // 
      // toolStripSeparator1
      // 
      toolStripSeparator1.Name = "toolStripSeparator1";
      toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
      // 
      // toolStripSeparator3
      // 
      this.toolStripSeparator3.Name = "toolStripSeparator3";
      this.toolStripSeparator3.Size = new System.Drawing.Size(177, 6);
      // 
      // tsMainStrip
      // 
      tsMainStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      tsMainStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
      tsMainStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsddbRenameFiles,
            this.tsddbRenameFolders,
            this.tsddbTagsFromName});
      tsMainStrip.Location = new System.Drawing.Point(0, 0);
      tsMainStrip.Name = "tsMainStrip";
      tsMainStrip.Size = new System.Drawing.Size(818, 31);
      tsMainStrip.TabIndex = 2;
      tsMainStrip.Text = "toolStrip1";
      // 
      // dgvResults
      // 
      this.dgvResults.AllowDrop = true;
      this.dgvResults.AllowUserToAddRows = false;
      this.dgvResults.AllowUserToDeleteRows = false;
      this.dgvResults.AllowUserToResizeRows = false;
      this.dgvResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
      this.dgvResults.BackgroundColor = System.Drawing.SystemColors.Window;
      this.dgvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgvResults.ContextMenuStrip = cmsItems;
      this.dgvResults.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dgvResults.Location = new System.Drawing.Point(0, 31);
      this.dgvResults.Name = "dgvResults";
      this.dgvResults.RowHeadersVisible = false;
      this.dgvResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.dgvResults.Size = new System.Drawing.Size(818, 374);
      this.dgvResults.TabIndex = 0;
      this.dgvResults.SelectionChanged += new System.EventHandler(this.dgvResults_SelectionChanged);
      this.dgvResults.DragDrop += new System.Windows.Forms.DragEventHandler(this.dgvResults_DragDrop);
      this.dgvResults.DragEnter += new System.Windows.Forms.DragEventHandler(this.dgvResults_DragEnter);
      // 
      // tsmiConvertToMkv
      // 
      this.tsmiConvertToMkv.Image = global::MassMediaEdit.Properties.Resources._22x22_Convert_MKV;
      this.tsmiConvertToMkv.Name = "tsmiConvertToMkv";
      this.tsmiConvertToMkv.Size = new System.Drawing.Size(180, 22);
      this.tsmiConvertToMkv.Text = "Convert to MKV";
      this.tsmiConvertToMkv.Click += new System.EventHandler(this.tsmiConvertToMkv_Click);
      //
      // toolStripSeparator4
      //
      this.toolStripSeparator4.Name = "toolStripSeparator4";
      this.toolStripSeparator4.Size = new System.Drawing.Size(177, 6);
      //
      // tsmiCopyFilePath
      //
      this.tsmiCopyFilePath.Image = global::MassMediaEdit.Properties.Resources._24x24_Copy;
      this.tsmiCopyFilePath.Name = "tsmiCopyFilePath";
      this.tsmiCopyFilePath.Size = new System.Drawing.Size(180, 22);
      this.tsmiCopyFilePath.Text = "Copy File Path";
      this.tsmiCopyFilePath.Click += new System.EventHandler(this.tsmiCopyFilePath_Click);
      //
      // tsmiOpenContainingFolder
      //
      this.tsmiOpenContainingFolder.Image = global::MassMediaEdit.Properties.Resources._24x24_Open_Folder;
      this.tsmiOpenContainingFolder.Name = "tsmiOpenContainingFolder";
      this.tsmiOpenContainingFolder.Size = new System.Drawing.Size(180, 22);
      this.tsmiOpenContainingFolder.Text = "Open Containing Folder";
      this.tsmiOpenContainingFolder.Click += new System.EventHandler(this.tsmiOpenContainingFolder_Click);
      //
      // tsmiClearItems
      // 
      tsmiClearItems.Image = ((System.Drawing.Image)(resources.GetObject("tsmiClearItems.Image")));
      tsmiClearItems.Name = "tsmiClearItems";
      tsmiClearItems.Size = new System.Drawing.Size(180, 22);
      tsmiClearItems.Text = "Clear";
      tsmiClearItems.Click += new System.EventHandler(this.tsmiClearItems_Click);
      // 
      // tsmiRemoveItem
      // 
      tsmiRemoveItem.Image = global::MassMediaEdit.Properties.Resources._24x24_Delete__2_;
      tsmiRemoveItem.Name = "tsmiRemoveItem";
      tsmiRemoveItem.Size = new System.Drawing.Size(180, 22);
      tsmiRemoveItem.Text = "Remove";
      tsmiRemoveItem.Click += new System.EventHandler(this.tsmiRemoveItem_Click);
      // 
      // tsmiCommitSelected
      // 
      this.tsmiCommitSelected.Image = global::MassMediaEdit.Properties.Resources._16x16_Blue_Disk;
      this.tsmiCommitSelected.Name = "tsmiCommitSelected";
      this.tsmiCommitSelected.Size = new System.Drawing.Size(180, 22);
      this.tsmiCommitSelected.Text = "Commit";
      this.tsmiCommitSelected.Click += new System.EventHandler(this.tsmiCommitSelected_Click);
      // 
      // tsmiRevertChanges
      // 
      this.tsmiRevertChanges.Image = global::MassMediaEdit.Properties.Resources._16x16_Undo;
      this.tsmiRevertChanges.Name = "tsmiRevertChanges";
      this.tsmiRevertChanges.Size = new System.Drawing.Size(180, 22);
      this.tsmiRevertChanges.Text = "Revert";
      this.tsmiRevertChanges.Click += new System.EventHandler(this.tsmiRevertChanges_Click);
      // 
      // tsmiAudio0
      // 
      this.tsmiAudio0.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tscbAudio0Language});
      this.tsmiAudio0.Image = global::MassMediaEdit.Properties.Resources._16x16_Note__green_;
      this.tsmiAudio0.Name = "tsmiAudio0";
      this.tsmiAudio0.Size = new System.Drawing.Size(180, 22);
      this.tsmiAudio0.Text = "Audio Stream 1";
      // 
      // tscbAudio0Language
      // 
      this.tscbAudio0Language.Name = "tscbAudio0Language";
      this.tscbAudio0Language.Size = new System.Drawing.Size(121, 23);
      this.tscbAudio0Language.SelectedIndexChanged += new System.EventHandler(this.tscbAudio1Language_SelectedIndexChanged);
      // 
      // tsmiAudio1
      // 
      this.tsmiAudio1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tscbAudio1Language});
      this.tsmiAudio1.Image = global::MassMediaEdit.Properties.Resources._16x16_Note__red_;
      this.tsmiAudio1.Name = "tsmiAudio1";
      this.tsmiAudio1.Size = new System.Drawing.Size(180, 22);
      this.tsmiAudio1.Text = "Audio Stream 2";
      // 
      // tscbAudio1Language
      // 
      this.tscbAudio1Language.Name = "tscbAudio1Language";
      this.tscbAudio1Language.Size = new System.Drawing.Size(121, 23);
      this.tscbAudio1Language.SelectedIndexChanged += new System.EventHandler(this.tscbAudio2Language_SelectedIndexChanged);
      // 
      // tsddbRenameFiles
      // 
      this.tsddbRenameFiles.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.tsddbRenameFiles.Enabled = false;
      this.tsddbRenameFiles.Image = ((System.Drawing.Image)(resources.GetObject("tsddbRenameFiles.Image")));
      this.tsddbRenameFiles.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tsddbRenameFiles.Name = "tsddbRenameFiles";
      this.tsddbRenameFiles.Size = new System.Drawing.Size(37, 28);
      this.tsddbRenameFiles.Text = "Rename Files";
      this.tsddbRenameFiles.Click += new System.EventHandler(this.tsddbRenameFiles_Click);
      // 
      // tsddbRenameFolders
      // 
      this.tsddbRenameFolders.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.tsddbRenameFolders.Enabled = false;
      this.tsddbRenameFolders.Image = global::MassMediaEdit.Properties.Resources._24x24_Rename_Folders;
      this.tsddbRenameFolders.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tsddbRenameFolders.Name = "tsddbRenameFolders";
      this.tsddbRenameFolders.Size = new System.Drawing.Size(37, 28);
      this.tsddbRenameFolders.Text = "Rename Source Folder";
      this.tsddbRenameFolders.Click += new System.EventHandler(this.tsddbRenameFolders_Click);
      // 
      // tsddbTagsFromName
      // 
      this.tsddbTagsFromName.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.tsddbTagsFromName.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiTitleFromFileName,
            this.tsmiVideoNameFromFileName,
            this.tsmiFixTitleAndName,
            this.tsmiClearTitle,
            this.tsmiClearVideoName,
            this.tsmiSwapTitleAndName,
            this.tsmiRecoverSpaces,
            this.tsmiRemoveBracketContent,
            this.tsmiNfo,
            toolStripSeparator2,
            this.tsmiAutoFillFromFileName});
      this.tsddbTagsFromName.Enabled = false;
      this.tsddbTagsFromName.Image = global::MassMediaEdit.Properties.Resources._24x24_Tags_From_Data;
      this.tsddbTagsFromName.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tsddbTagsFromName.Name = "tsddbTagsFromName";
      this.tsddbTagsFromName.Size = new System.Drawing.Size(37, 28);
      this.tsddbTagsFromName.Text = "Tags From Name";
      this.tsddbTagsFromName.DropDownOpening += new System.EventHandler(this.tsddbTagsFromName_DropDownOpening);
      this.tsddbTagsFromName.Click += new System.EventHandler(this.tsddbTagsFromName_Click);
      // 
      // tsmiTitleFromFileName
      // 
      this.tsmiTitleFromFileName.Name = "tsmiTitleFromFileName";
      this.tsmiTitleFromFileName.Size = new System.Drawing.Size(205, 22);
      this.tsmiTitleFromFileName.Text = "Title From Filename";
      this.tsmiTitleFromFileName.Click += new System.EventHandler(this.tsmiTitleFromFilename_Click);
      // 
      // tsmiVideoNameFromFileName
      // 
      this.tsmiVideoNameFromFileName.Name = "tsmiVideoNameFromFileName";
      this.tsmiVideoNameFromFileName.Size = new System.Drawing.Size(205, 22);
      this.tsmiVideoNameFromFileName.Text = "Name From Filename";
      this.tsmiVideoNameFromFileName.Click += new System.EventHandler(this.tsmiVideoNameFromFileName_Click);
      // 
      // tsmiFixTitleAndName
      // 
      this.tsmiFixTitleAndName.Name = "tsmiFixTitleAndName";
      this.tsmiFixTitleAndName.Size = new System.Drawing.Size(205, 22);
      this.tsmiFixTitleAndName.Text = "Fix Title/Name";
      this.tsmiFixTitleAndName.Click += new System.EventHandler(this.tsmiFixTitleAndName_Click);
      // 
      // tsmiClearTitle
      // 
      this.tsmiClearTitle.Name = "tsmiClearTitle";
      this.tsmiClearTitle.Size = new System.Drawing.Size(205, 22);
      this.tsmiClearTitle.Text = "Clear Title";
      this.tsmiClearTitle.Click += new System.EventHandler(this.tsmiClearTitle_Click);
      // 
      // tsmiClearVideoName
      // 
      this.tsmiClearVideoName.Name = "tsmiClearVideoName";
      this.tsmiClearVideoName.Size = new System.Drawing.Size(205, 22);
      this.tsmiClearVideoName.Text = "Clear Name";
      this.tsmiClearVideoName.Click += new System.EventHandler(this.tsmiClearVideoName_Click);
      // 
      // tsmiSwapTitleAndName
      // 
      this.tsmiSwapTitleAndName.Name = "tsmiSwapTitleAndName";
      this.tsmiSwapTitleAndName.Size = new System.Drawing.Size(205, 22);
      this.tsmiSwapTitleAndName.Text = "Swap Title && Name";
      this.tsmiSwapTitleAndName.Click += new System.EventHandler(this.tsmiSwapTitleAndName_Click);
      // 
      // tsmiRecoverSpaces
      // 
      this.tsmiRecoverSpaces.Name = "tsmiRecoverSpaces";
      this.tsmiRecoverSpaces.Size = new System.Drawing.Size(205, 22);
      this.tsmiRecoverSpaces.Text = "Recover Spaces";
      this.tsmiRecoverSpaces.Click += new System.EventHandler(this.tsmiRecoverSpaces_Click);
      // 
      // tsmiRemoveBracketContent
      // 
      this.tsmiRemoveBracketContent.Name = "tsmiRemoveBracketContent";
      this.tsmiRemoveBracketContent.Size = new System.Drawing.Size(205, 22);
      this.tsmiRemoveBracketContent.Text = "Remove Bracket Content";
      this.tsmiRemoveBracketContent.Click += new System.EventHandler(this.tsmiRemoveBracketContent_Click);
      // 
      // tsmiNfo
      // 
      this.tsmiNfo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            tsmiTitleFromNfoTitle,
            tsmiTitleFromNfoOriginalTitle,
            tsmiNameFromNfoTitle,
            tsmiNameFromNfoOriginalTitle});
      this.tsmiNfo.Enabled = false;
      this.tsmiNfo.Name = "tsmiNfo";
      this.tsmiNfo.Size = new System.Drawing.Size(205, 22);
      this.tsmiNfo.Text = "From NFO Metadata";
      // 
      // tsmiTitleFromNfoTitle
      // 
      tsmiTitleFromNfoTitle.Name = "tsmiTitleFromNfoTitle";
      tsmiTitleFromNfoTitle.Size = new System.Drawing.Size(217, 22);
      tsmiTitleFromNfoTitle.Text = "Title From \"Title\"";
      tsmiTitleFromNfoTitle.Click += new System.EventHandler(this.tsmiTitleFromNfoTitle_Click);
      // 
      // tsmiTitleFromNfoOriginalTitle
      // 
      tsmiTitleFromNfoOriginalTitle.Name = "tsmiTitleFromNfoOriginalTitle";
      tsmiTitleFromNfoOriginalTitle.Size = new System.Drawing.Size(217, 22);
      tsmiTitleFromNfoOriginalTitle.Text = "Title From \"Original Title\"";
      tsmiTitleFromNfoOriginalTitle.Click += new System.EventHandler(this.tsmiTitleFromNfoOriginalTitle_Click);
      // 
      // tsmiNameFromNfoTitle
      // 
      tsmiNameFromNfoTitle.Name = "tsmiNameFromNfoTitle";
      tsmiNameFromNfoTitle.Size = new System.Drawing.Size(217, 22);
      tsmiNameFromNfoTitle.Text = "Name From \"Title\"";
      tsmiNameFromNfoTitle.Click += new System.EventHandler(this.tsmiNameFromNfoTitle_Click);
      // 
      // tsmiNameFromNfoOriginalTitle
      // 
      tsmiNameFromNfoOriginalTitle.Name = "tsmiNameFromNfoOriginalTitle";
      tsmiNameFromNfoOriginalTitle.Size = new System.Drawing.Size(217, 22);
      tsmiNameFromNfoOriginalTitle.Text = "Name From \"Original Title\"";
      tsmiNameFromNfoOriginalTitle.Click += new System.EventHandler(this.tsmiNameFromNfoOriginalTitle_Click);
      // 
      // toolStripSeparator2
      // 
      toolStripSeparator2.Name = "toolStripSeparator2";
      toolStripSeparator2.Size = new System.Drawing.Size(202, 6);
      // 
      // tsmiAutoFillFromFileName
      // 
      this.tsmiAutoFillFromFileName.Name = "tsmiAutoFillFromFileName";
      this.tsmiAutoFillFromFileName.Size = new System.Drawing.Size(205, 22);
      this.tsmiAutoFillFromFileName.Text = "Auto-Fill from Filename";
      this.tsmiAutoFillFromFileName.Click += new System.EventHandler(this.tsmiAutoFillFromFileName_Click);
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
      // tsslConvertingFiles
      // 
      this.tsslConvertingFiles.Image = global::MassMediaEdit.Properties.Resources._16x11_Loading_Animation;
      this.tsslConvertingFiles.Name = "tsslConvertingFiles";
      this.tsslConvertingFiles.Size = new System.Drawing.Size(115, 17);
      this.tsslConvertingFiles.Text = "Converting files...";
      this.tsslConvertingFiles.Visible = false;
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(818, 427);
      this.Controls.Add(this.dgvResults);
      this.Controls.Add(tsMainStrip);
      this.Controls.Add(statusStrip1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "MainForm";
      this.Text = "MainForm";
      statusStrip1.ResumeLayout(false);
      statusStrip1.PerformLayout();
      cmsItems.ResumeLayout(false);
      tsMainStrip.ResumeLayout(false);
      tsMainStrip.PerformLayout();
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
    private System.Windows.Forms.ToolStripDropDownButton tsddbRenameFiles;
    private System.Windows.Forms.ToolStripDropDownButton tsddbRenameFolders;
    private System.Windows.Forms.ToolStripDropDownButton tsddbTagsFromName;
    private System.Windows.Forms.ToolStripMenuItem tsmiTitleFromFileName;
    private System.Windows.Forms.ToolStripMenuItem tsmiVideoNameFromFileName;
    private System.Windows.Forms.ToolStripMenuItem tsmiFixTitleAndName;
    private System.Windows.Forms.ToolStripMenuItem tsmiClearTitle;
    private System.Windows.Forms.ToolStripMenuItem tsmiClearVideoName;
    private System.Windows.Forms.ToolStripMenuItem tsmiSwapTitleAndName;
    private System.Windows.Forms.ToolStripMenuItem tsmiRecoverSpaces;
    private System.Windows.Forms.ToolStripMenuItem tsmiRemoveBracketContent;
    private System.Windows.Forms.ToolStripMenuItem tsmiAutoFillFromFileName;
    private System.Windows.Forms.ToolStripMenuItem tsmiNfo;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    private System.Windows.Forms.ToolStripMenuItem tsmiAudio0;
    private System.Windows.Forms.ToolStripMenuItem tsmiAudio1;
    private System.Windows.Forms.ToolStripComboBox tscbAudio0Language;
    private System.Windows.Forms.ToolStripComboBox tscbAudio1Language;
    private System.Windows.Forms.ToolStripMenuItem tsmiConvertToMkv;
    private System.Windows.Forms.ToolStripStatusLabel tsslConvertingFiles;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
    private System.Windows.Forms.ToolStripMenuItem tsmiCopyFilePath;
    private System.Windows.Forms.ToolStripMenuItem tsmiOpenContainingFolder;
  }
}