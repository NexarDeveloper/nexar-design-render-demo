﻿using Nexar.Client;
using Nexar.Renderer.DesignEntities;
using Nexar.Renderer.Managers;
using StrawberryShake;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nexar.Renderer.UserControls
{
    public partial class CreateComment : UserControl
    {
        private CommentThreads Owner { get; }
        private CommentThread Thread { get; }
        private NexarClient NexarClient { get; }
        private PcbManager PcbManager { get; }


        public CreateComment(
            CommentThreads owner,
            CommentThread thread,
            NexarClient nexarClient,
            PcbManager pcbManager)
        {
            InitializeComponent();

            Owner = owner;
            Thread = thread;
            NexarClient = nexarClient;
            PcbManager = pcbManager;

            postCommentButton.Enabled = false;

            commentTextBox.TextChanged += CommentTextBox_TextChanged;
            postCommentButton.Click += PostCommentButton_Click;

        }

        private void CommentTextBox_TextChanged(object? sender, EventArgs e)
        {
            AdaptSize();
            postCommentButton.Enabled = (commentTextBox.Text.Trim() != string.Empty);
        }

        private async void PostCommentButton_Click(object? sender, EventArgs e)
        {
            commentTextBox.Enabled = false;

            var createCommentInput = new DesCreateCommentInput()
            {
                CommentThreadId = Thread.CommentThreadId,
                EntityId = PcbManager.ActiveProject.Id,
                Text = commentTextBox.Text
            };

            var createCommentResult = await NexarClient.CreateComment.ExecuteAsync(createCommentInput);
            createCommentResult.EnsureNoErrors();

            await Owner.UpdateCommentThreadsAsync();
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
