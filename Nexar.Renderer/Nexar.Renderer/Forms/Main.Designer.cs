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
            mainMenu = new MenuStrip();
            workspaceToolStripMenuItem = new ToolStripMenuItem();
            openProjectMenuItem = new ToolStripMenuItem();
            layersToolStripMenuItem = new ToolStripMenuItem();
            primitivesToolStripMenuItem = new ToolStripMenuItem();
            tracksMenuItem = new ToolStripMenuItem();
            padsMenuItem = new ToolStripMenuItem();
            viasMenuItem = new ToolStripMenuItem();
            componentOutlinesMenuItem = new ToolStripMenuItem();
            componentBodyMenuItem = new ToolStripMenuItem();
            commentAreaMenuItem = new ToolStripMenuItem();
            commentsMenuItem = new ToolStripMenuItem();
            showCommentsMenuItem = new ToolStripMenuItem();
            refreshCommentsMenuItem = new ToolStripMenuItem();
            splitContainer = new SplitContainer();
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            currentProgressBar = new ToolStripProgressBar();
            mainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // mainMenu
            // 
            mainMenu.Items.AddRange(new ToolStripItem[] { workspaceToolStripMenuItem, openProjectMenuItem, layersToolStripMenuItem, primitivesToolStripMenuItem, commentsMenuItem });
            mainMenu.Location = new Point(0, 0);
            mainMenu.Name = "mainMenu";
            mainMenu.RenderMode = ToolStripRenderMode.Professional;
            mainMenu.Size = new Size(1184, 24);
            mainMenu.TabIndex = 1;
            mainMenu.Text = "mainMenu";
            // 
            // workspaceToolStripMenuItem
            // 
            workspaceToolStripMenuItem.Name = "workspaceToolStripMenuItem";
            workspaceToolStripMenuItem.Size = new Size(154, 20);
            workspaceToolStripMenuItem.Text = "Click to Load Workspaces";
            workspaceToolStripMenuItem.Click += WorkspaceToolStripMenuItem_Click;
            // 
            // openProjectMenuItem
            // 
            openProjectMenuItem.Name = "openProjectMenuItem";
            openProjectMenuItem.Size = new Size(88, 20);
            openProjectMenuItem.Text = "Open Project";
            openProjectMenuItem.Click += OpenProjectMenuItem_Click;
            // 
            // layersToolStripMenuItem
            // 
            layersToolStripMenuItem.Name = "layersToolStripMenuItem";
            layersToolStripMenuItem.Size = new Size(52, 20);
            layersToolStripMenuItem.Text = "Layers";
            // 
            // primitivesToolStripMenuItem
            // 
            primitivesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tracksMenuItem, padsMenuItem, viasMenuItem, componentOutlinesMenuItem, componentBodyMenuItem, commentAreaMenuItem });
            primitivesToolStripMenuItem.Name = "primitivesToolStripMenuItem";
            primitivesToolStripMenuItem.Size = new Size(71, 20);
            primitivesToolStripMenuItem.Text = "Primitives";
            // 
            // tracksMenuItem
            // 
            tracksMenuItem.Checked = true;
            tracksMenuItem.CheckOnClick = true;
            tracksMenuItem.CheckState = CheckState.Checked;
            tracksMenuItem.Name = "tracksMenuItem";
            tracksMenuItem.Size = new Size(185, 22);
            tracksMenuItem.Text = "Tracks";
            // 
            // padsMenuItem
            // 
            padsMenuItem.Checked = true;
            padsMenuItem.CheckOnClick = true;
            padsMenuItem.CheckState = CheckState.Checked;
            padsMenuItem.Name = "padsMenuItem";
            padsMenuItem.Size = new Size(185, 22);
            padsMenuItem.Text = "Pads";
            // 
            // viasMenuItem
            // 
            viasMenuItem.Checked = true;
            viasMenuItem.CheckOnClick = true;
            viasMenuItem.CheckState = CheckState.Checked;
            viasMenuItem.Name = "viasMenuItem";
            viasMenuItem.Size = new Size(185, 22);
            viasMenuItem.Text = "Vias";
            // 
            // componentOutlinesMenuItem
            // 
            componentOutlinesMenuItem.Checked = true;
            componentOutlinesMenuItem.CheckOnClick = true;
            componentOutlinesMenuItem.CheckState = CheckState.Checked;
            componentOutlinesMenuItem.Name = "componentOutlinesMenuItem";
            componentOutlinesMenuItem.Size = new Size(185, 22);
            componentOutlinesMenuItem.Text = "Component Outlines";
            // 
            // componentBodyMenuItem
            // 
            componentBodyMenuItem.Checked = true;
            componentBodyMenuItem.CheckOnClick = true;
            componentBodyMenuItem.CheckState = CheckState.Checked;
            componentBodyMenuItem.Name = "componentBodyMenuItem";
            componentBodyMenuItem.Size = new Size(185, 22);
            componentBodyMenuItem.Text = "Component Bodies";
            // 
            // commentAreaMenuItem
            // 
            commentAreaMenuItem.Checked = true;
            commentAreaMenuItem.CheckOnClick = true;
            commentAreaMenuItem.CheckState = CheckState.Checked;
            commentAreaMenuItem.Name = "commentAreaMenuItem";
            commentAreaMenuItem.Size = new Size(185, 22);
            commentAreaMenuItem.Text = "Comment Areas";
            // 
            // commentsMenuItem
            // 
            commentsMenuItem.DropDownItems.AddRange(new ToolStripItem[] { showCommentsMenuItem, refreshCommentsMenuItem });
            commentsMenuItem.Name = "commentsMenuItem";
            commentsMenuItem.Size = new Size(78, 20);
            commentsMenuItem.Text = "Comments";
            // 
            // showCommentsMenuItem
            // 
            showCommentsMenuItem.CheckOnClick = true;
            showCommentsMenuItem.Name = "showCommentsMenuItem";
            showCommentsMenuItem.Size = new Size(113, 22);
            showCommentsMenuItem.Text = "Show";
            // 
            // refreshCommentsMenuItem
            // 
            refreshCommentsMenuItem.Name = "refreshCommentsMenuItem";
            refreshCommentsMenuItem.Size = new Size(113, 22);
            refreshCommentsMenuItem.Text = "Refresh";
            // 
            // splitContainer
            // 
            splitContainer.BackColor = Color.FromArgb(45, 45, 45);
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.FixedPanel = FixedPanel.Panel2;
            splitContainer.Location = new Point(0, 24);
            splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.BackColor = Color.FromArgb(30, 30, 30);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.BackColor = Color.FromArgb(30, 30, 30);
            splitContainer.Panel2Collapsed = true;
            splitContainer.Size = new Size(1184, 737);
            splitContainer.SplitterDistance = 788;
            splitContainer.SplitterWidth = 8;
            splitContainer.TabIndex = 2;
            splitContainer.TabStop = false;
            // 
            // statusStrip
            // 
            statusStrip.BackColor = SystemColors.Control;
            statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel, currentProgressBar });
            statusStrip.Location = new Point(0, 739);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(1184, 22);
            statusStrip.TabIndex = 3;
            statusStrip.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(39, 17);
            statusLabel.Text = "Ready";
            // 
            // currentProgressBar
            // 
            currentProgressBar.Name = "currentProgressBar";
            currentProgressBar.Size = new Size(100, 16);
            currentProgressBar.Visible = false;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1184, 761);
            Controls.Add(statusStrip);
            Controls.Add(splitContainer);
            Controls.Add(mainMenu);
            KeyPreview = true;
            MainMenuStrip = mainMenu;
            Name = "Main";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Nexar.Renderer";
            mainMenu.ResumeLayout(false);
            mainMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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
        private ToolStripMenuItem componentBodyMenuItem;
    }
}