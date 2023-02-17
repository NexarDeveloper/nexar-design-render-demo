using Nexar.Client;
using Nexar.Renderer.Api;
using Nexar.Renderer.DesignEntities;
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
using System.Windows.Forms.VisualStyles;
using System.Xml.Linq;

namespace Nexar.Renderer.UserControls
{
    public partial class CommentThreads : UserControl
    {
        private readonly List<CommentElement> commentElementUCs = new List<CommentElement>();
        private readonly Dictionary<string, CreateComment> createCommentUCs = new Dictionary<string, CreateComment>();
        private readonly Dictionary<string, TableLayoutPanel> threadToPanelMapping = new Dictionary<string, TableLayoutPanel>();

        private readonly Dictionary<string, List<Comment>> commentModel = new Dictionary<string, List<Comment>>();

        private readonly TableLayoutPanel allThreadsTableLayoutPanel;

        private NexarHelper NexarHelper { get; }

        public IOperationResult<IGetPcbModelResult> PcbModel { get; set; } = default!;

        public CommentThreads()
        {
            InitializeComponent();

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

            NexarHelper = new NexarHelper();
        }

        public void LoadCommentThreadsThreadSafe()
        {
            InvokeMethodThreadSafe(LoadCommentThreads);
        }

        public void UpdateCommentThreadsThreadSafe()
        {
            InvokeMethodThreadSafeAsync(UpdateCommentThreadsAsync);
        }

        private void InvokeMethodThreadSafe(Action method)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    method();
                }));
            }
            else
            {
                method();
            }
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

        private void LoadCommentThreads()
        {
            try
            {
                SuspendLayout();

                DeleteComments();

                var modelCommentThreads = PcbModel?.Data?.DesProjectById?.Design?.WorkInProgress?.Variants[0]?.Pcb?.CommentThreads;

                if (modelCommentThreads != null)
                {
                    var commentThreads = new List<CommentThread>();

                    foreach (var modelCommentThread in modelCommentThreads.OrderBy(x => x.CreatedAt))
                    {
                        var thread = new CommentThread()
                        {
                            CommentThreadId = modelCommentThread.CommentThreadId
                        };

                        commentThreads.Add(thread);

                        AddCommentThreadLayoutPanel(thread);

                        foreach (var modelComment in modelCommentThread.Comments)
                        {
                            var comment = new Comment()
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

                            thread.Comments.Add(comment);
                            AddCommentElementUCToLayoutPanel(thread, comment);
                        }

                        AddCreateCommentUCToLayoutPanel(thread);
                    }

                    UpdateLocalModel(commentThreads);
                    ResumeLayout();
                }
            }
            catch
            {
                //AppLogger.Error(string.Format("Exception loading comment threads: {0}", ex.Message));
            }
        }

        private async Task UpdateCommentThreadsAsync()
        {
            try
            {
                string? projectId = PcbModel?.Data?.DesProjectById?.Id;

                if (projectId != null)
                {
                    await NexarHelper.LoginAsync();
                    var nexarClient = NexarHelper.GetNexarClient();

                    var pcbComments = await nexarClient.GetPcbComments.ExecuteAsync(projectId);
                    var modelCommentThreads = pcbComments?.Data?.DesProjectById?.Design?.WorkInProgress?.Variants.FirstOrDefault()?.Pcb?.CommentThreads;

                    if (modelCommentThreads != null)
                    {
                        var commentThreads = new List<CommentThread>();

                        foreach (var modelCommentThread in modelCommentThreads.OrderBy(x => x.CreatedAt))
                        {
                            var thread = new CommentThread()
                            {
                                CommentThreadId = modelCommentThread.CommentThreadId
                            };

                            commentThreads.Add(thread);

                            if (threadToPanelMapping.ContainsKey(modelCommentThread.CommentThreadId))
                            {
                                foreach (var modelComment in modelCommentThread.Comments)
                                {
                                    var comment = new Comment()
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

                                    thread.Comments.Add(comment);
                                }

                                ProcessCommentModelChangesForThread(
                                    thread,
                                    commentModel[thread.CommentThreadId]);
                            }
                            else
                            {
                                AddCommentThreadLayoutPanel(thread);

                                foreach (var modelComment in modelCommentThread.Comments)
                                {
                                    var comment = new Comment()
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

                                    thread.Comments.Add(comment);
                                    AddCommentElementUCToLayoutPanel(thread, comment);
                                }

                                AddCreateCommentUCToLayoutPanel(thread);
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

        private void AddCommentThreadLayoutPanel(CommentThread commentThread)
        {
            var commentThreadLayoutPanel = CreateCommentThreadLayoutPanel();
            allThreadsTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            allThreadsTableLayoutPanel.Controls.Add(commentThreadLayoutPanel);
            //allThreadsTableLayoutPanel.SetRow(commentThreadLayoutPanel, 0);
            threadToPanelMapping.Add(commentThread.CommentThreadId, commentThreadLayoutPanel);
        }

        private void AddCommentElementUCToLayoutPanel(CommentThread thread, Comment comment)
        {
            var commentElement = new CommentElement() { Dock = DockStyle.Top };
            //commentElement.Height = 300;
            commentElement.AutoScroll = true;
            commentElement.LoadComment(thread, comment);
            commentElementUCs.Add(commentElement);

            threadToPanelMapping[thread.CommentThreadId].RowStyles.Add(new RowStyle(SizeType.AutoSize));
            threadToPanelMapping[thread.CommentThreadId].Controls.Add(commentElement);
        }

        private void AddCreateCommentUCToLayoutPanel(CommentThread thread)
        {
            var newCommentElement = new CreateComment(thread) { Dock = DockStyle.Top };
            //newCommentElement.Height = 300;
            newCommentElement.AutoScroll = true;

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
            commentModel.Clear();

            foreach (var thread in commentThreads)
            {
                foreach (var comment in thread.Comments)
                {
                    if (!commentModel.ContainsKey(thread.CommentThreadId))
                    {
                        commentModel.Add(thread.CommentThreadId, new List<Comment>());
                    }

                    commentModel[thread.CommentThreadId].Add(comment);
                }
            }
        }

        private TableLayoutPanel CreateCommentThreadLayoutPanel()
        {
            var commentThreadTableLayoutPanel = new TableLayoutPanel()
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                //Height = 500,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new System.Windows.Forms.Padding(0, 0, 0, 0)
            };

            commentThreadTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            return commentThreadTableLayoutPanel;
        }
    }
}
