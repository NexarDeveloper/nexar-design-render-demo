namespace Nexar.Renderer.Forms
{
    partial class CreateCommentThread
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.commentTextBox = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.createCommentThreadButton = new System.Windows.Forms.Button();
            this.cancelCommentButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.commentTextBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(1, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(298, 178);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // commentTextBox
            // 
            this.commentTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.commentTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.commentTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.commentTextBox.ForeColor = System.Drawing.Color.White;
            this.commentTextBox.Location = new System.Drawing.Point(3, 3);
            this.commentTextBox.MaximumSize = new System.Drawing.Size(0, 10000);
            this.commentTextBox.MaxLength = 2000;
            this.commentTextBox.MinimumSize = new System.Drawing.Size(0, 10);
            this.commentTextBox.Multiline = true;
            this.commentTextBox.Name = "commentTextBox";
            this.commentTextBox.Size = new System.Drawing.Size(292, 136);
            this.commentTextBox.TabIndex = 4;
            this.commentTextBox.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.commentTextBox_PreviewKeyDown);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.createCommentThreadButton, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.cancelCommentButton, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 145);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(292, 30);
            this.tableLayoutPanel2.TabIndex = 5;
            // 
            // createCommentThreadButton
            // 
            this.createCommentThreadButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.createCommentThreadButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.createCommentThreadButton.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.createCommentThreadButton.Location = new System.Drawing.Point(179, 3);
            this.createCommentThreadButton.Name = "createCommentThreadButton";
            this.createCommentThreadButton.Size = new System.Drawing.Size(110, 24);
            this.createCommentThreadButton.TabIndex = 3;
            this.createCommentThreadButton.Text = "Create";
            this.createCommentThreadButton.UseVisualStyleBackColor = true;
            // 
            // cancelCommentButton
            // 
            this.cancelCommentButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.cancelCommentButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cancelCommentButton.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.cancelCommentButton.Location = new System.Drawing.Point(3, 3);
            this.cancelCommentButton.Name = "cancelCommentButton";
            this.cancelCommentButton.Size = new System.Drawing.Size(110, 24);
            this.cancelCommentButton.TabIndex = 2;
            this.cancelCommentButton.Text = "Cancel";
            this.cancelCommentButton.UseVisualStyleBackColor = true;
            // 
            // CreateCommentThread
            // 
            this.AcceptButton = this.createCommentThreadButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.CancelButton = this.cancelCommentButton;
            this.ClientSize = new System.Drawing.Size(300, 180);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "CreateCommentThread";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "CreateCommentThread";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private TextBox commentTextBox;
        private TableLayoutPanel tableLayoutPanel2;
        private Button createCommentThreadButton;
        private Button cancelCommentButton;
    }
}