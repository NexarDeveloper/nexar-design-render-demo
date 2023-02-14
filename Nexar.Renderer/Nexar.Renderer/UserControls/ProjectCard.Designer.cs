namespace Nexar.Renderer
{
    partial class ProjectCard
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.thumbnail = new System.Windows.Forms.PictureBox();
            this.projectName = new System.Windows.Forms.Label();
            this.projectDescription = new System.Windows.Forms.Label();
            this.openProjectButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.thumbnail)).BeginInit();
            this.SuspendLayout();
            // 
            // thumbnail
            // 
            this.thumbnail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.thumbnail.InitialImage = global::Nexar.Renderer.Properties.Resources.loading;
            this.thumbnail.Location = new System.Drawing.Point(3, 3);
            this.thumbnail.Name = "thumbnail";
            this.thumbnail.Size = new System.Drawing.Size(294, 189);
            this.thumbnail.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.thumbnail.TabIndex = 0;
            this.thumbnail.TabStop = false;
            // 
            // projectName
            // 
            this.projectName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.projectName.ForeColor = System.Drawing.Color.White;
            this.projectName.Location = new System.Drawing.Point(3, 195);
            this.projectName.Name = "projectName";
            this.projectName.Size = new System.Drawing.Size(294, 23);
            this.projectName.TabIndex = 1;
            this.projectName.Text = "Loading...";
            this.projectName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // projectDescription
            // 
            this.projectDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.projectDescription.ForeColor = System.Drawing.Color.White;
            this.projectDescription.Location = new System.Drawing.Point(3, 223);
            this.projectDescription.Name = "projectDescription";
            this.projectDescription.Size = new System.Drawing.Size(294, 23);
            this.projectDescription.TabIndex = 2;
            this.projectDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // openProjectButton
            // 
            this.openProjectButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.openProjectButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.openProjectButton.ForeColor = System.Drawing.Color.Black;
            this.openProjectButton.Location = new System.Drawing.Point(3, 249);
            this.openProjectButton.Name = "openProjectButton";
            this.openProjectButton.Size = new System.Drawing.Size(294, 23);
            this.openProjectButton.TabIndex = 3;
            this.openProjectButton.Text = "Open";
            this.openProjectButton.UseVisualStyleBackColor = false;
            // 
            // ProjectCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.Controls.Add(this.openProjectButton);
            this.Controls.Add(this.projectDescription);
            this.Controls.Add(this.projectName);
            this.Controls.Add(this.thumbnail);
            this.Name = "ProjectCard";
            this.Size = new System.Drawing.Size(300, 277);
            ((System.ComponentModel.ISupportInitialize)(this.thumbnail)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private Button openProjectButton;
        public PictureBox thumbnail;
        public Label projectName;
        public Label projectDescription;
    }
}
