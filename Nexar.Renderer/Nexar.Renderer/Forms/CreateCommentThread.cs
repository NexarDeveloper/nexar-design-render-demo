using Nexar.Client;
using Nexar.Renderer.Api;
using Nexar.Renderer.Managers;
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
    public enum E_CommentType
    {
        Component,
        Area
    }

    public partial class CreateCommentThread : Form
    {
        public E_CommentType CommentType { get; }

        private NexarHelper NexarHelper { get; }
        private NexarClient NexarClient { get; }
        private PcbManager PcbManager { get;}
        private string Id { get; } = string.Empty;

        public CreateCommentThread(
            E_CommentType commentType,
            PcbManager pcbManager,
            string? id = null)
        {
            InitializeComponent();

            PcbManager = pcbManager;
            CommentType = commentType;
            Id = id ?? string.Empty;

            NexarHelper = new NexarHelper();
            NexarClient = NexarHelper.GetNexarClient();

            KeyDown += CreateCommentThread_KeyDown;
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
            bool success = false;
            var commentThreadInput = new DesCreateCommentThreadInput();

            commentThreadInput.EntityId = PcbManager.ActiveProject.ProjectId;
            commentThreadInput.DocumentId = PcbManager.DocumentId;
            commentThreadInput.ObjectId = DecodeNodeId(Id).PcbUniqueId;
            commentThreadInput.Text = commentTextBox.Text.Trim();

            try
            {
                await NexarClient.CreateCommentThread.ExecuteAsync(commentThreadInput);
                success = true;
            }
            catch
            {
                success = false;
            }

            return success;
        }

        public record NodeId(
            string WorkspaceUrl,
            string ProjectId,
            string ReleaseGuid,
            string SchUniqueId,
            string PcbUniqueId);

        public static NodeId DecodeNodeId(string nodeId)
        {
            byte[] data = Convert.FromBase64String(nodeId);
            string decodedString = Encoding.UTF8.GetString(data);

            var decodedId = decodedString.Split('|');

            if (decodedId.Length != 5)
                throw new InvalidDataException("Invalid design item node ID.");

            return new NodeId(
                decodedId[0],
                decodedId[1],
                decodedId[2],
                decodedId[3],
                decodedId[4]);
        }
    }
}
