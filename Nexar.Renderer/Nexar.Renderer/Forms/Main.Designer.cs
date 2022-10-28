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
            this.glControl = new OpenTK.WinForms.GLControl();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.workspaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openProjectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.layersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.primitivesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tracksMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.padsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viasMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // glControl
            // 
            this.glControl.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
            this.glControl.APIVersion = new System.Version(3, 3, 0, 0);
            this.glControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glControl.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
            this.glControl.IsEventDriven = true;
            this.glControl.Location = new System.Drawing.Point(0, 24);
            this.glControl.Name = "glControl";
            this.glControl.Profile = OpenTK.Windowing.Common.ContextProfile.Any;
            this.glControl.Size = new System.Drawing.Size(984, 737);
            this.glControl.TabIndex = 0;
            this.glControl.Text = "glControl1";
            this.glControl.Load += new System.EventHandler(this.GlControl_Load);
            this.glControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GlControl_MouseDown);
            this.glControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GlControl_MouseMove);
            this.glControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.GlControl_MouseUp);
            this.glControl.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.GlControl_PreviewKeyDown);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.workspaceToolStripMenuItem,
            this.openProjectMenuItem,
            this.primitivesToolStripMenuItem,
            this.layersToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(984, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // workspaceToolStripMenuItem
            // 
            this.workspaceToolStripMenuItem.Name = "workspaceToolStripMenuItem";
            this.workspaceToolStripMenuItem.Size = new System.Drawing.Size(154, 20);
            this.workspaceToolStripMenuItem.Text = "Click to Load Workspaces";
            this.workspaceToolStripMenuItem.Click += new System.EventHandler(this.workspaceToolStripMenuItem_Click);
            // 
            // openMenuItem
            // 
            this.openProjectMenuItem.Name = "openMenuItem";
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
            this.viasMenuItem});
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
            this.tracksMenuItem.Size = new System.Drawing.Size(180, 22);
            this.tracksMenuItem.Text = "Tracks";
            // 
            // padsMenuItem
            // 
            this.padsMenuItem.Checked = true;
            this.padsMenuItem.CheckOnClick = true;
            this.padsMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.padsMenuItem.Name = "padsMenuItem";
            this.padsMenuItem.Size = new System.Drawing.Size(180, 22);
            this.padsMenuItem.Text = "Pads";
            // 
            // viaMenuItem
            // 
            this.viasMenuItem.Checked = true;
            this.viasMenuItem.CheckOnClick = true;
            this.viasMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viasMenuItem.Name = "viaMenuItem";
            this.viasMenuItem.Size = new System.Drawing.Size(180, 22);
            this.viasMenuItem.Text = "Vias";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 761);
            this.Controls.Add(this.glControl);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nexar.Renderer";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OpenTK.WinForms.GLControl glControl;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem openProjectMenuItem;
        private ToolStripMenuItem workspaceToolStripMenuItem;
        private ToolStripMenuItem layersToolStripMenuItem;
        private ToolStripMenuItem primitivesToolStripMenuItem;
        private ToolStripMenuItem tracksMenuItem;
        private ToolStripMenuItem padsMenuItem;
        private ToolStripMenuItem viasMenuItem;
    }
}