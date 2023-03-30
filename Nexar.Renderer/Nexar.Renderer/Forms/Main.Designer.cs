namespace Nexar.Renderer.Forms
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.workspaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openProjectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.layersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.primitivesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tracksMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.padsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viasMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.componentOutlinesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commentAreaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commentsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showCommentsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshCommentsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.currentProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.testMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.load3DTestComponentsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.workspaceToolStripMenuItem,
            this.openProjectMenuItem,
            this.layersToolStripMenuItem,
            this.primitivesToolStripMenuItem,
            this.commentsMenuItem,
            this.testMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.mainMenu.Size = new System.Drawing.Size(1184, 24);
            this.mainMenu.TabIndex = 1;
            this.mainMenu.Text = "mainMenu";
            // 
            // workspaceToolStripMenuItem
            // 
            this.workspaceToolStripMenuItem.Name = "workspaceToolStripMenuItem";
            this.workspaceToolStripMenuItem.Size = new System.Drawing.Size(154, 20);
            this.workspaceToolStripMenuItem.Text = "Click to Load Workspaces";
            this.workspaceToolStripMenuItem.Click += new System.EventHandler(this.WorkspaceToolStripMenuItem_Click);
            // 
            // openProjectMenuItem
            // 
            this.openProjectMenuItem.Name = "openProjectMenuItem";
            this.openProjectMenuItem.Size = new System.Drawing.Size(88, 20);
            this.openProjectMenuItem.Text = "Open Project";
            this.openProjectMenuItem.Click += new System.EventHandler(this.OpenProjectMenuItem_Click);
            // 
            // layersToolStripMenuItem
            // 
            this.layersToolStripMenuItem.Name = "layersToolStripMenuItem";
            this.layersToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.layersToolStripMenuItem.Text = "Layers";
            // 
            // primitivesToolStripMenuItem
            // 
            this.primitivesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tracksMenuItem,
            this.padsMenuItem,
            this.viasMenuItem,
            this.componentOutlinesMenuItem,
            this.commentAreaMenuItem});
            this.primitivesToolStripMenuItem.Name = "primitivesToolStripMenuItem";
            this.primitivesToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.primitivesToolStripMenuItem.Text = "Primitives";
            // 
            // tracksMenuItem
            // 
            this.tracksMenuItem.Checked = true;
            this.tracksMenuItem.CheckOnClick = true;
            this.tracksMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tracksMenuItem.Name = "tracksMenuItem";
            this.tracksMenuItem.Size = new System.Drawing.Size(185, 22);
            this.tracksMenuItem.Text = "Tracks";
            // 
            // padsMenuItem
            // 
            this.padsMenuItem.Checked = true;
            this.padsMenuItem.CheckOnClick = true;
            this.padsMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.padsMenuItem.Name = "padsMenuItem";
            this.padsMenuItem.Size = new System.Drawing.Size(185, 22);
            this.padsMenuItem.Text = "Pads";
            // 
            // viasMenuItem
            // 
            this.viasMenuItem.Checked = true;
            this.viasMenuItem.CheckOnClick = true;
            this.viasMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viasMenuItem.Name = "viasMenuItem";
            this.viasMenuItem.Size = new System.Drawing.Size(185, 22);
            this.viasMenuItem.Text = "Vias";
            // 
            // componentOutlinesMenuItem
            // 
            this.componentOutlinesMenuItem.Checked = true;
            this.componentOutlinesMenuItem.CheckOnClick = true;
            this.componentOutlinesMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.componentOutlinesMenuItem.Name = "componentOutlinesMenuItem";
            this.componentOutlinesMenuItem.Size = new System.Drawing.Size(185, 22);
            this.componentOutlinesMenuItem.Text = "Component Outlines";
            // 
            // commentAreaMenuItem
            // 
            this.commentAreaMenuItem.Checked = true;
            this.commentAreaMenuItem.CheckOnClick = true;
            this.commentAreaMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.commentAreaMenuItem.Name = "commentAreaMenuItem";
            this.commentAreaMenuItem.Size = new System.Drawing.Size(185, 22);
            this.commentAreaMenuItem.Text = "Comment Areas";
            // 
            // commentsMenuItem
            // 
            this.commentsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showCommentsMenuItem,
            this.refreshCommentsMenuItem});
            this.commentsMenuItem.Name = "commentsMenuItem";
            this.commentsMenuItem.Size = new System.Drawing.Size(78, 20);
            this.commentsMenuItem.Text = "Comments";
            // 
            // showCommentsMenuItem
            // 
            this.showCommentsMenuItem.CheckOnClick = true;
            this.showCommentsMenuItem.Name = "showCommentsMenuItem";
            this.showCommentsMenuItem.Size = new System.Drawing.Size(180, 22);
            this.showCommentsMenuItem.Text = "Show";
            // 
            // refreshCommentsMenuItem
            // 
            this.refreshCommentsMenuItem.Name = "refreshCommentsMenuItem";
            this.refreshCommentsMenuItem.Size = new System.Drawing.Size(180, 22);
            this.refreshCommentsMenuItem.Text = "Refresh";
            // 
            // splitContainer
            // 
            this.splitContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer.Location = new System.Drawing.Point(0, 24);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.splitContainer.Panel2Collapsed = true;
            this.splitContainer.Size = new System.Drawing.Size(1184, 737);
            this.splitContainer.SplitterDistance = 788;
            this.splitContainer.SplitterWidth = 8;
            this.splitContainer.TabIndex = 2;
            this.splitContainer.TabStop = false;
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.SystemColors.Control;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.currentProgressBar});
            this.statusStrip.Location = new System.Drawing.Point(0, 739);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1184, 22);
            this.statusStrip.TabIndex = 3;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(39, 17);
            this.statusLabel.Text = "Ready";
            // 
            // currentProgressBar
            // 
            this.currentProgressBar.Name = "currentProgressBar";
            this.currentProgressBar.Size = new System.Drawing.Size(100, 16);
            this.currentProgressBar.Visible = false;
            // 
            // testMenuItem
            // 
            this.testMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.load3DTestComponentsMenuItem});
            this.testMenuItem.Name = "testMenuItem";
            this.testMenuItem.Size = new System.Drawing.Size(39, 20);
            this.testMenuItem.Text = "Test";
            // 
            // load3DTestComponentsMenuItem
            // 
            this.load3DTestComponentsMenuItem.Name = "load3DTestComponentsMenuItem";
            this.load3DTestComponentsMenuItem.Size = new System.Drawing.Size(212, 22);
            this.load3DTestComponentsMenuItem.Text = "Load 3D Test Components";
            this.load3DTestComponentsMenuItem.Click += new System.EventHandler(this.load3DTestComponentsMenuItem_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 761);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.mainMenu);
            this.KeyPreview = true;
            this.MainMenuStrip = this.mainMenu;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nexar.Renderer";
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private MenuStrip mainMenu;
        private ToolStripMenuItem openProjectMenuItem;
        private ToolStripMenuItem workspaceToolStripMenuItem;
        private ToolStripMenuItem layersToolStripMenuItem;
        private ToolStripMenuItem primitivesToolStripMenuItem;
        private ToolStripMenuItem tracksMenuItem;
        private ToolStripMenuItem padsMenuItem;
        private ToolStripMenuItem viasMenuItem;
        private SplitContainer splitContainer;
        private ToolStripMenuItem commentsMenuItem;
        private ToolStripMenuItem showCommentsMenuItem;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private ToolStripProgressBar currentProgressBar;
        private ToolStripMenuItem componentOutlinesMenuItem;
        private ToolStripMenuItem refreshCommentsMenuItem;
        private ToolStripMenuItem commentAreaMenuItem;
        private ToolStripMenuItem testMenuItem;
        private ToolStripMenuItem load3DTestComponentsMenuItem;
    }
}