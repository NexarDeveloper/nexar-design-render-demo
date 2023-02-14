using Nexar.Renderer.DesignEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nexar.Renderer.Forms
{
    public partial class CreateComment : UserControl
    {
        private readonly CommentThread commentThread;

        public CreateComment(CommentThread commentThread)
        {
            InitializeComponent();

            this.commentThread = commentThread;

            commentTextBox.TextChanged += CommentTextBox_TextChanged;
            postCommentButton.Click += PostCommentButton_Click;

        }

        private void CommentTextBox_TextChanged(object? sender, EventArgs e)
        {
            AdaptSize();
            postCommentButton.Enabled = (commentTextBox.Text.Trim() != string.Empty);
        }

        private void PostCommentButton_Click(object? sender, EventArgs e)
        {
        }

        public void Reset()
        {
            commentTextBox.Text = string.Empty;
            postCommentButton.Text = "Reply";
            commentTextBox.Enabled = true;
            postCommentButton.Enabled = true;
        }

        public void AdaptSize()
        {
            int textHeight = GetTextHeight(commentTextBox);
            int desiredContainerHeight = Math.Max(56, textHeight + 20);

            if (MinimumSize.Height != desiredContainerHeight)
            {
                MinimumSize = new Size(10, desiredContainerHeight);
                MaximumSize = new Size(10000, desiredContainerHeight);
            }
        }

        private int GetTextHeight(TextBox tBox)
        {
            return TextRenderer.MeasureText(tBox.Text, tBox.Font, tBox.ClientSize,
                     TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl).Height;
        }
    }
}
