using Nexar.Client;
using Nexar.Renderer.Api;
using Nexar.Renderer.DesignEntities;
using Nexar.Renderer.Forms;
using Nexar.Renderer.Managers;
using StrawberryShake;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml.Linq;

namespace Nexar.Renderer.UserControls
{
    using QueryCommentThread = IGetPcbComments_DesProjectById_Design_WorkInProgress_Variants_Pcb_CommentThreads;
    using QueryComment = IGetPcbComments_DesProjectById_Design_WorkInProgress_Variants_Pcb_CommentThreads_Comments;

    public partial class CommentThreads : UserControl
    {
        private readonly List<CommentElement> commentElementUCs = new List<CommentElement>();
        private readonly Dictionary<string, CreateComment> createCommentUCs = new Dictionary<string, CreateComment>();
        private readonly Dictionary<string, TableLayoutPanel> threadToPanelMapping = new Dictionary<string, TableLayoutPanel>();

        public Dictionary<string, Tuple<CommentThread, List<Comment>>> CommentModel { get; } = 
            new Dictionary<string, Tuple<CommentThread, List<Comment>>>();

        private readonly TableLayoutPanel allThreadsTableLayoutPanel;

        private NexarClient NexarClient { get; }
        private PcbManager PcbManager { get; }

        public IOperationResult<IGetPcbModelResult> PcbModel { get; set; } = default!;

        public CommentThreads(
            NexarClient nexarClient,
            PcbManager pcbManager)
        {
            InitializeComponent();

            NexarClient = nexarClient;
            PcbManager = pcbManager;

            allThreadsTableLayoutPanel = new TableLayoutPanel()
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            allThreadsTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            allThreadsTableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

            Controls.Add(allThreadsTableLayoutPanel);

            AutoScroll = true;
        }

        public int GetCommentThreadCount()
        {
            return CommentModel.Count;
        }

        public void LoadCommentThreadsThreadSafe()
        {
            InvokeMethodThreadSafeAsync(LoadCommentThreadsAsync);
        }

        public void UpdateCommentThreadsThreadSafe()
        {
            InvokeMethodThreadSafeAsync(UpdateCommentThreadsAsync);
        }

        private void InvokeMethodThreadSafeAsync(Func<Task> method)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(async delegate
                {
                    await method();
                }));
            }
            else
            {
                method();
            }
        }

        public async Task LoadCommentThreadsAsync()
        {
            try
            {
                SuspendLayout();
                DeleteComments();
                await UpdateCommentThreadsAsync();
                ResumeLayout();
            }
            catch
            {
                //AppLogger.Error(string.Format("Exception loading comment threads: {0}", ex.Message));
            }
        }

        public async Task UpdateCommentThreadsAsync()
        {
            try
            {
                string? projectId = PcbModel?.Data?.DesProjectById?.Id;

                if (projectId != null)
                {
                    var pcbComments = await NexarClient.GetPcbComments.ExecuteAsync(projectId);
                    pcbComments.EnsureNoErrors();
                    var modelCommentThreads = pcbComments?.Data?.DesProjectById?.Design?.WorkInProgress?.Variants.FirstOrDefault()?.Pcb?.CommentThreads;

                    if (modelCommentThreads != null)
                    {
                        var commentThreads = new List<CommentThread>();
                        var existingThreadIds = new List<string>();

                        foreach (var modelCommentThread in modelCommentThreads.OrderBy(x => x.CreatedAt))
                        {
                            existingThreadIds.Add(modelCommentThread.CommentThreadId);

                            var thread = new CommentThread(
                                modelCommentThread.CommentThreadId,
                                (float)modelCommentThread.Context.Area.Pos1.XMm,
                                (float)modelCommentThread.Context.Area.Pos1.YMm,
                                (float)modelCommentThread.Context.Area.Pos2.XMm,
                                (float)modelCommentThread.Context.Area.Pos2.YMm);

                            commentThreads.Add(thread);

                            if (threadToPanelMapping.ContainsKey(modelCommentThread.CommentThreadId))
                            {
                                // Panel for comment thread exists, add new comment to it
                                foreach (var modelComment in modelCommentThread.Comments)
                                {
                                    var comment = CommentFromModelComment(modelComment);
                                    thread.Comments.Add(comment);
                                }

                                ProcessCommentModelChangesForThread(
                                    thread,
                                    CommentModel[thread.CommentThreadId].Item2);
                            }
                            else
                            {
                                // Create the comment thread panel and add the first comment to it
                                AddCommentThread(thread, modelCommentThread);
                            }
                        }

                        for (int index = threadToPanelMapping.Count - 1; index >= 0; index--)
                        {
                            var threadToPanelMappingElement = threadToPanelMapping.ElementAt(index);

                            if (!existingThreadIds.Contains(threadToPanelMappingElement.Key))
                            {
                                allThreadsTableLayoutPanel.Controls.Remove(threadToPanelMappingElement.Value);
                                threadToPanelMapping.Remove(threadToPanelMappingElement.Key);
                            }
                        }

                        UpdateLocalModel(commentThreads);
                    }
                }
            }
            catch
            {
                //AppLogger.Error(string.Format("Exception updating comment threads: {0}", ex.Message));
            }
        }

        private void AddCommentThread(
            CommentThread thread,
            QueryCommentThread modelCommentThread)
        {
            AddCommentThreadLayoutPanel(thread);

            foreach (var modelComment in modelCommentThread.Comments)
            {
                var comment = CommentFromModelComment(modelComment);
                thread.Comments.Add(comment);
                AddCommentElementUCToLayoutPanel(thread, comment);
            }

            AddCreateCommentUCToLayoutPanel(thread);
        }

        private Comment CommentFromModelComment(QueryComment modelComment)
        {
            return new Comment()
            {
                CommentId = modelComment.CommentId,
                CreatedAt = modelComment.CreatedAt.DateTime,
                FirstName = modelComment.ModifiedBy?.FirstName ?? string.Empty,
                LastName = modelComment.ModifiedBy?.LastName ?? string.Empty,
                ModifiedAt = modelComment.ModifiedAt.DateTime,
                PictureUrl = modelComment.ModifiedBy?.PictureUrl ?? string.Empty,
                Text = modelComment.Text,
                Username = modelComment.ModifiedBy?.Email ?? string.Empty
            };
        }

        private void AddCommentThreadLayoutPanel(CommentThread commentThread)
        {
            var commentThreadLayoutPanel = CreateCommentThreadLayoutPanel();
            allThreadsTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            allThreadsTableLayoutPanel.Controls.Add(commentThreadLayoutPanel);
            //allThreadsTableLayoutPanel.SetRow(commentThreadLayoutPanel, 0);
            threadToPanelMapping.Add(commentThread.CommentThreadId, commentThreadLayoutPanel);
        }

        private TableLayoutPanel CreateCommentThreadLayoutPanel()
        {
            var commentThreadTableLayoutPanel = new TableLayoutPanel()
            {
                //BackColor = Color.LimeGreen,
                //CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                //Dock = DockStyle.Top,
                AutoSize = true,
                Margin = new Padding(5),
                //Height = 500,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new System.Windows.Forms.Padding(0, 0, 0, 0)
            };

            commentThreadTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            commentThreadTableLayoutPanel.Click += CommentThreadTableLayoutPanel_Click;

            return commentThreadTableLayoutPanel;
        }

        private void CommentThreadTableLayoutPanel_Click(object? sender, EventArgs e)
        {
            threadToPanelMapping.Values.ToList().ForEach(x => x.BackColor = Color.Transparent);
            var active = sender as TableLayoutPanel;

            if (active != null) 
            {
                active.BackColor = Color.LimeGreen;
            }
        }

        private void AddCommentElementUCToLayoutPanel(CommentThread thread, Comment comment)
        {
            var commentElement = new CommentElement(
                this,
                NexarClient,
                PcbManager) 
            { 
                Dock = DockStyle.Top 
            };
            
            commentElement.AutoScroll = true;
            commentElement.LoadComment(thread, comment);
            commentElement.Click += CommentElement_Click;
            commentElement.ElementClick += CommentElement_Click;
            commentElementUCs.Add(commentElement);

            threadToPanelMapping[thread.CommentThreadId].RowStyles.Add(new RowStyle(SizeType.AutoSize));
            threadToPanelMapping[thread.CommentThreadId].Controls.Add(commentElement);
        }

        private void CommentElement_Click(object? sender, EventArgs e)
        {
            var parentLayoutPanel = (sender as CommentElement)?.Parent as TableLayoutPanel;
            
            if (parentLayoutPanel == null)
            {
                parentLayoutPanel = (sender as CreateComment)?.Parent as TableLayoutPanel;
            }

            if (parentLayoutPanel != null)
            {
                CommentThreadTableLayoutPanel_Click(parentLayoutPanel, e);
            }
        }

        private void AddCreateCommentUCToLayoutPanel(CommentThread thread)
        {
            var newCommentElement = new CreateComment(
                this,
                thread,
                NexarClient,
                PcbManager) 
            { 
                Dock = DockStyle.Top 
            };

            newCommentElement.AutoScroll = true;
            newCommentElement.ElementClick += CommentElement_Click;

            createCommentUCs.Add(thread.CommentThreadId, newCommentElement);

            threadToPanelMapping[thread.CommentThreadId].RowStyles.Add(new RowStyle(SizeType.AutoSize));
            threadToPanelMapping[thread.CommentThreadId].Controls.Add(newCommentElement);
        }

        private void DeleteComments()
        {
            allThreadsTableLayoutPanel.Controls.Clear();
            allThreadsTableLayoutPanel.RowStyles.Clear();

            commentElementUCs.Clear();
            createCommentUCs.Clear();
            threadToPanelMapping.Clear();
        }

        private void ProcessCommentModelChangesForThread(CommentThread thread, List<Comment> existing)
        {
            var newComments = thread.Comments.Where(x => !existing.Any(y => y.CommentId == x.CommentId)).ToList();
            var deletedComments = existing.Where(x => !thread.Comments.Any(y => y.CommentId == x.CommentId)).ToList();
            var updatedComments = thread.Comments.Except(newComments).Where(x => !thread.Comments.Any(y => y.ModifiedAt == x.ModifiedAt)).ToList();

            if ((newComments.Count > 0) ||
                (deletedComments.Count > 0) ||
                (updatedComments.Count > 0))
            {
                threadToPanelMapping[thread.CommentThreadId].SuspendLayout();

                threadToPanelMapping[thread.CommentThreadId].Controls.Remove(createCommentUCs[thread.CommentThreadId]);

                foreach (var comment in newComments)
                {
                    AddCommentElementUCToLayoutPanel(thread, comment);
                }

                foreach (var comment in deletedComments)
                {
                    var commentElement = commentElementUCs.FirstOrDefault(x => x.Comment!.CommentId == comment.CommentId);
                    threadToPanelMapping[thread.CommentThreadId].Controls.Remove(commentElement);
                    threadToPanelMapping[thread.CommentThreadId].RowStyles.RemoveAt(threadToPanelMapping[thread.CommentThreadId].RowStyles.Count - 1);
                }

                foreach (var comment in updatedComments)
                {
                    var commentElement = commentElementUCs.FirstOrDefault(x => x.Comment!.CommentId == comment.CommentId);
                    commentElement!.LoadComment(thread, comment, true);
                }

                threadToPanelMapping[thread.CommentThreadId].Controls.Add(createCommentUCs[thread.CommentThreadId]);
                createCommentUCs[thread.CommentThreadId].Reset();

                threadToPanelMapping[thread.CommentThreadId].ResumeLayout();
            }
        }

        private void UpdateLocalModel(List<CommentThread> commentThreads)
        {
            CommentModel.Clear();

            foreach (var thread in commentThreads)
            {
                foreach (var comment in thread.Comments)
                {
                    if (!CommentModel.ContainsKey(thread.CommentThreadId))
                    {
                        CommentModel.Add(
                            thread.CommentThreadId, 
                            new Tuple<CommentThread, List<Comment>>(
                                thread,
                                new List<Comment>()));
                    }

                    CommentModel[thread.CommentThreadId].Item2.Add(comment);
                }
            }
        }
    }
}
