using Nexar.Client;
using Nexar.Renderer.Api;
using Nexar.Renderer.Managers;
using StrawberryShake;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nexar.Renderer.Forms
{
    public enum E_CommentType
    {
        Component,
        Area
    }

    public partial class CreateCommentThread : Form
    {
        public E_CommentType CommentType { get; }

        private NexarClient NexarClient { get; }
        private PcbManager PcbManager { get;}
        private string? Id { get; }
        private Tuple<Point, Point> Area { get; }

        public CreateCommentThread(
            NexarClient nexarClient,
            E_CommentType commentType,
            PcbManager pcbManager,
            Tuple<Point, Point> area,
            string? id = null)
        {
            InitializeComponent();

            PcbManager = pcbManager;
            CommentType = commentType;
            Id = id;
            Area = area;

            NexarClient = nexarClient;

            KeyDown += CreateCommentThread_KeyDown;
            createCommentThreadButton.Click += CreateCommentThreadButton_Click;
        }

        private async void CreateCommentThread_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (await ExecuteAsync())
                {
                    DialogResult = DialogResult.OK;
                }
            }
        }

        private void commentTextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                e.IsInputKey = true;
            }
        }

        private async Task<bool> ExecuteAsync()
        {
            Enabled = false;

            bool success;
            var commentThreadInput = new DesCreateCommentThreadInput();
            var area = new DesRectangleInput();
            area.Pos1 = new DesPosition2DInput() { X = Area.Item1.X, Y = Area.Item1.Y };
            area.Pos2 = new DesPosition2DInput() { X = Area.Item2.X, Y = Area.Item2.Y };

            commentThreadInput.DocumentType = DesDocumentType.Pcb;
            commentThreadInput.CommentContextType = (CommentType == E_CommentType.Component ? DesCommentContextType.Component : DesCommentContextType.Area);
            commentThreadInput.EntityId = PcbManager.ActiveProject.Id;
            commentThreadInput.DocumentId = PcbManager.DocumentId;
            commentThreadInput.DocumentName = PcbManager.DocumentName;
            commentThreadInput.ItemAsDesignItemId = Id;
            commentThreadInput.Text = commentTextBox.Text.Trim();
            commentThreadInput.Area = area;

            try
            {
                var result = await NexarClient.CreateCommentThread.ExecuteAsync(commentThreadInput);
                result.EnsureNoErrors();
                success = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message, 
                    "Error Creating Comment Thread", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                
                success = false;
            }

            return success;
        }

        private async void CreateCommentThreadButton_Click(object? sender, EventArgs e)
        {
            if (await ExecuteAsync())
            {
                DialogResult = DialogResult.OK;
            }
        }
    }
}
