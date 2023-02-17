namespace Nexar.Renderer.UserControls
{
    partial class CreateComment
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.postCommentButton = new System.Windows.Forms.Button();
            this.commentTextBox = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.Controls.Add(this.postCommentButton, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.commentTextBox, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(588, 50);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // postCommentButton
            // 
            this.postCommentButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.postCommentButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.postCommentButton.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.postCommentButton.Location = new System.Drawing.Point(531, 24);
            this.postCommentButton.Name = "postCommentButton";
            this.postCommentButton.Size = new System.Drawing.Size(54, 23);
            this.postCommentButton.TabIndex = 1;
            this.postCommentButton.Text = "Reply";
            this.postCommentButton.UseVisualStyleBackColor = true;
            // 
            // commentTextBox
            // 
            this.commentTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.commentTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.commentTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.commentTextBox.ForeColor = System.Drawing.Color.White;
            this.commentTextBox.Location = new System.Drawing.Point(53, 3);
            this.commentTextBox.MaximumSize = new System.Drawing.Size(10000, 10000);
            this.commentTextBox.MaxLength = 2000;
            this.commentTextBox.MinimumSize = new System.Drawing.Size(0, 10);
            this.commentTextBox.Multiline = true;
            this.commentTextBox.Name = "commentTextBox";
            this.commentTextBox.Size = new System.Drawing.Size(472, 44);
            this.commentTextBox.TabIndex = 4;
            // 
            // CreateComment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "CreateComment";
            this.Size = new System.Drawing.Size(594, 56);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Button postCommentButton;
        private TextBox commentTextBox;
    }
}
