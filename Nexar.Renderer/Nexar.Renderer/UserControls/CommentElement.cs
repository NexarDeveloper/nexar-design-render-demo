using Nexar.Client;
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
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;

namespace Nexar.Renderer.UserControls
{
    public partial class CommentElement : UserControl
    {
        private NexarClient NexarClient { get; }
        private PcbManager PcbManager { get; }
        private CommentThreads Owner { get; }

        private ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
        private ToolStripItem itemEdit;
        private ToolStripItem itemSave;
        private ToolStripItem itemCancel;
        private ToolStripItem itemDelete;

        public CommentThread? Thread { get; private set; }
        public Comment? Comment { get; private set; }

        public CommentElement(
            CommentThreads owner,
            NexarClient nexarClient,
            PcbManager pcbManager)
        {
            InitializeComponent();

            Owner = owner;
            NexarClient = nexarClient;
            PcbManager = pcbManager;

            itemEdit = new ToolStripMenuItem("Edit");
            itemSave = new ToolStripMenuItem("Save");
            itemCancel = new ToolStripMenuItem("Cancel");
            itemDelete = new ToolStripMenuItem("Delete");

            menuButton.Click += MenuButton_Click;

            itemEdit.Click += ItemEdit_Click;
            itemSave.Click += ItemSave_Click;
            itemCancel.Click += ItemCancel_Click;
            itemDelete.Click += ItemDelete_Click;

            commentTextBox.TextChanged += CommentTextBox_TextChanged;

            ConfigureCommentEditMode(true);
        }

        private void CommentTextBox_TextChanged(object? sender, EventArgs e)
        {
            AdaptSize();
        }

        private void MenuButton_Click(object? sender, EventArgs e)
        {
            contextMenuStrip.Show(MousePosition);
        }

        private void ItemEdit_Click(object? sender, EventArgs e)
        {
            ConfigureCommentEditMode(false);
        }

        private void ItemCancel_Click(object? sender, EventArgs e)
        {
            ConfigureCommentEditMode(true);
        }

        private async void ItemDelete_Click(object? sender, EventArgs e)
        {
            if (Comment != null && Thread != null)
            {
                var confirm = MessageBox.Show(
                    "Are you sure you want to delete this comment?",
                    "Confirm Deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    try
                    {
                        if (Owner.CommentModel.ContainsKey(Thread.CommentThreadId))
                        {
                            var deleteCommentInput = new DesDeleteCommentInput();
                            deleteCommentInput.EntityId = PcbManager.ActiveProject.Id;
                            deleteCommentInput.CommentThreadId = Thread.CommentThreadId;
                            deleteCommentInput.CommentId = Comment.CommentId;

                            var deleteCommentResult = await NexarClient.DeleteComment.ExecuteAsync(deleteCommentInput);
                            deleteCommentResult.EnsureNoErrors();

                            // Check if this was the last comment, and if so, delete the entire thread
                            if (Owner.CommentModel[Thread.CommentThreadId].Count == 1)
                            {
                                var deleteCommentThreadInput = new DesDeleteCommentThreadInput();
                                deleteCommentThreadInput.EntityId = PcbManager.ActiveProject.Id;
                                deleteCommentThreadInput.CommentThreadId = Thread.CommentThreadId;

                                var deleteCommenThreadResult = await NexarClient.DeleteCommentThread.ExecuteAsync(deleteCommentThreadInput);
                                deleteCommenThreadResult.EnsureNoErrors();
                            }

                            await Owner.UpdateCommentThreadsAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            ex.Message,
                            "Error Deleting Comment",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ItemSave_Click(object? sender, EventArgs e)
        {
            //if (Comment.Text != commentTextBox.Text)
            {
                commentTextBox.Enabled = false;

                /*
                var designManager = DIBindings.Resolve<IDesignManager>();
                var nexarClient = designManager.GetNexarClient();

                var commentInput = new DesUpdateCommentInput()
                {
                    CommentThreadId = Thread.CommentThreadId,
                    CommentId = Comment.CommentId,
                    EntityId = designManager.ActiveProjectId,
                    Text = commentTextBox.Text
                };

                await nexarClient.UpdateComment.ExecuteAsync(commentInput);

                // HACK while no subscriptions
                await designManager.RefreshCommentsAsync(Landscape?.Model?.VPCB);
                */
            }

            ConfigureCommentEditMode(true);
        }

        private void ConfigureCommentEditMode(bool readOnly)
        {
            contextMenuStrip.Items.Clear();
            commentTextBox.ReadOnly = readOnly;
            commentTextBox.Enabled = true;

            if (readOnly)
            {
                commentTextBox.BorderStyle = BorderStyle.None;
                contextMenuStrip.Items.Add(itemEdit);
                contextMenuStrip.Items.Add(itemDelete);
            }
            else
            {
                commentTextBox.BorderStyle = BorderStyle.FixedSingle;
                contextMenuStrip.Items.Add(itemSave);
                contextMenuStrip.Items.Add(itemCancel);
            }
        }


        public void LoadComment(CommentThread thread, Comment comment, bool reload = false)
        {
            if ((Thread == null) || reload)
            {
                Thread = thread;
                Comment = comment;

                commentUserLabel.Text = string.Format("{0} {1}", comment.FirstName, comment.LastName);
                //commentUserLabel.ToolTip = comment.Username;
                commentUpdatedLabel.Text = string.Format("{0}, {1}", comment.ModifiedAt.ToString("m"), comment.ModifiedAt.ToString("t"));
                commentTextBox.Text = comment.Text;
                avatarPictureBox.ImageLocation = comment.PictureUrl;
                AdaptSize();

                //commentText.EditValueChanged += CommentText_EditValueChanged;
            }
        }

        public void AdaptSize()
        {
            int textHeight = GetTextHeight(commentTextBox);
            int desiredContainerHeight = textHeight + 76;

            if (MinimumSize.Height != desiredContainerHeight)
            {
                MinimumSize = new Size(10, desiredContainerHeight);
                MaximumSize = new Size(10000, desiredContainerHeight);
            }

            /*if (commentTextBox.MinimumSize.Height != textHeight)
            {
                commentTextBox.MinimumSize = new Size(10, textHeight);
                commentTextBox.MaximumSize = new Size(10000, textHeight);
                //MinimumSize = new Size(MinimumSize.Width, textHeight + 28);
                //MaximumSize = new Size(MaximumSize.Width, textHeight + 28);
            }*/
        }

        private int GetTextHeight(TextBox tBox)
        {
            return TextRenderer.MeasureText(tBox.Text, tBox.Font, tBox.ClientSize,
                     TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl).Height;
        }
    }
}
