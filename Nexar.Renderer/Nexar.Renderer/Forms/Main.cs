using System;
using System.ComponentModel;
using System.Windows.Forms;
using Nexar.Renderer.DesignEntities;
using Nexar.Renderer.Geometry;
using Nexar.Renderer.Managers;
using Nexar.Renderer.Utilities;
using Nexar.Renderer.Visualization;
using Microsoft.Extensions.DependencyInjection;
using Nexar.Client;
using Nexar.Client.Login;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.WinForms;
using System.Security.Policy;
using System.Runtime.CompilerServices;
using Nexar.Renderer.Api;
using StrawberryShake;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

using IPcbLayer = Nexar.Client.IGetPcbModel_DesProjectById_Design_WorkInProgress_Variants_Pcb_LayerStack_Stacks_Layers;
using Microsoft.VisualBasic.Devices;
using Nexar.Renderer.UserControls;

namespace Nexar.Renderer.Forms
{
    public partial class Main : Form
    {
        private GLControl glControl;

        private NexarHelper NexarHelper { get; }

        private IGetWorkspaces_DesWorkspaces? ActiveWorkspace { get; set; }

        private PcbManager pcbManager;

        private ThreadHelper? renderThreadHelper;

        private CommentThreads commentThreads;

        private const int THREAD_PERIOD_MS = 10;

        private int glWidth = 1200;
        private int glHeight = 800;

        private bool testPrimitiveEnabled = false;

        private DesignItem? SelectedComponent { get; set; }

        public Main()
        {
            InitializeComponent();

            glControl = new GLControl();

            glControl.Size = new Size(glWidth, glHeight);
            glControl.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left;
            glControl.Location = new Point(0, 0);
            glControl.Load += GlControl_Load;
            glControl.Resize += GlControl_Resize;
            glControl.Click += GlControl_Click;

            glControl.MouseDown += GlControl_MouseDown;
            glControl.MouseMove += GlControl_MouseMove;
            glControl.MouseUp += GlControl_MouseUp;
            glControl.MouseWheel += GlControl_MouseWheel;
            glControl.PreviewKeyDown += GlControl_PreviewKeyDown;

            splitContainer.Panel1.Controls.Add(glControl);
            tracksMenuItem.CheckedChanged += TracksMenuItem_CheckedChanged;
            padsMenuItem.CheckedChanged += PadsMenuItem_CheckedChanged;
            viasMenuItem.CheckedChanged += ViasMenuItem_CheckedChanged;
            componentOutlinesMenuItem.CheckedChanged += ComponentOutlinesMenuItem_CheckedChanged;
            showCommentsMenuItem.CheckedChanged += CommentsMenuItem_CheckedChanged;

            NexarHelper = new NexarHelper();

            // Very hacky circular reference
            var glRenderer = new GlRenderer(glWidth, glHeight, "Nexar Renderer");
            pcbManager = new PcbManager(glRenderer);
            glRenderer.MouseUpCallback = CreateCommentWithArea;

            if (renderThreadHelper == null)
            {
                renderThreadHelper = new ThreadHelper(
                    "RenderThread",
                    THREAD_PERIOD_MS,
                    new Action<object>(RenderFrameThreadSafe),
                    ThreadExceptionHandler,
                    null, 
                    CloseApplication);

                renderThreadHelper.StartThreads();
            }

            commentThreads = new CommentThreads()
            {
                Dock = DockStyle.Fill
                //Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right
            };

            splitContainer.Panel2.Controls.Add(commentThreads);

            /*splitContainer.Panel2.Controls.Add(new CommentElement()
            {
                Dock = DockStyle.Top
                //Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right
            });*/
        }

        private void GlControl_Click(object? sender, EventArgs e)
        {
            splitContainer.Panel1.Focus();
        }

        private void GlControl_Load(object? sender, EventArgs e)
        {
            // Make sure that when the GLControl is resized or needs to be painted,
            // we update our projection matrix or re-render its contents, respectively.
            glControl.Resize += GlControl_Resize;

            pcbManager.PcbRenderer.OnLoad();

            if (testPrimitiveEnabled)
            {
                pcbManager.PcbRenderer.Pcb.AddTestPrimitive();
                pcbManager.PcbRenderer.Pcb.EnabledPcbLayers.Add("Test");
            }

            // Ensure that the viewport and projection matrix are set correctly initially.
            GlControl_Resize(glControl, EventArgs.Empty);
        }

        private void GlControl_MouseMove(object? sender, MouseEventArgs e)
        {
            Control? control = sender as Control;

            if (control != null)
            {
                Point pt = control.PointToClient(Control.MousePosition);
                Point pt2 = new Point(pt.X, control.Height - pt.Y);

                if (e.Button == MouseButtons.Left)
                {
                    pcbManager.PcbRenderer.Demo_MouseMove(control, pt);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    pcbManager.PcbRenderer.MousePan(control, pt);
                }
            }
        }

        private void GlControl_MouseDown(object? sender, MouseEventArgs e)
        {
            Control? control = sender as Control;

            if (control != null)
            {
                Point pt = control.PointToClient(Control.MousePosition);

                if (e.Button == MouseButtons.Left)
                {
                    pcbManager.PcbRenderer.Demo_MouseDown(control, pt);
                }
            }
        }

        private void GlControl_MouseUp(object? sender, MouseEventArgs e)
        {
            Control? control = sender as Control;

            if (control != null)
            {
                Point pt = control.PointToClient(Control.MousePosition);

                if (e.Button == MouseButtons.Left)
                {
                    pcbManager.PcbRenderer.Demo_MouseUp(control, pt);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    var glCoord = pcbManager.PcbRenderer.GetXYOnZeroZPlane(pt);
                    var mmCoord = pcbManager.ConvertGlCoordToMm(glCoord);

                    SelectedComponent = pcbManager.GetComponentForLocation(mmCoord);

                    if (SelectedComponent != null)
                    {
                        ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
                        ToolStripItem createCommentThread = new ToolStripMenuItem(string.Format("Add Comment to '{0}'", SelectedComponent.Designator));
                        createCommentThread.Click += CreateCommentThread_Click;
                        contextMenuStrip.Items.Add(createCommentThread);
                        contextMenuStrip.Show(MousePosition);
                    }
                }
            }
        }

        private void CreateCommentWithArea(Point location)
        {
            var highlightArea = pcbManager.GetHighlightArea();

            var createCommentThread = new CreateCommentThread(
                NexarHelper.GetNexarClient(ActiveWorkspace?.Location.ApiServiceUrl),
                E_CommentType.Area,
                pcbManager,
                highlightArea);

            createCommentThread.Location = Cursor.Position;
            createCommentThread.ShowDialog();

            if (createCommentThread.DialogResult == DialogResult.OK)
            {
                commentThreads.UpdateCommentThreadsThreadSafe();
            }
        }

        private void CreateCommentThread_Click(object? sender, EventArgs e)
        {
            if (SelectedComponent != null)
            {               
                var createCommentThread = new CreateCommentThread(
                    NexarHelper.GetNexarClient(ActiveWorkspace?.Location.ApiServiceUrl),
                    E_CommentType.Component,
                    pcbManager,
                    SelectedComponent.BoundingRectangleCoords,
                    SelectedComponent.Id);
                createCommentThread.Location = Cursor.Position;
                createCommentThread.ShowDialog();

                if (createCommentThread.DialogResult == DialogResult.OK)
                {
                    commentThreads.UpdateCommentThreadsThreadSafe();
                }
            }
        }

        private void GlControl_MouseWheel(object? sender, MouseEventArgs e)
        {
            pcbManager.PcbRenderer.ZoomRequest = -e.Delta;
        }

        private void GlControl_Resize(object? sender, EventArgs e)
        {
            if (splitContainer.Panel2Collapsed)
            {
                pcbManager.PcbRenderer.WindowReshape(Width, Height);
            }
            else
            {
                pcbManager.PcbRenderer.WindowReshape(
                    splitContainer.Panel1.Width,
                    Height);
            }
        }

        private void RenderFrameThreadSafe(object threadLock)
        {
            lock (threadLock)
            {
                if (InvokeRequired)
                {
                    Invoke(new MethodInvoker(delegate
                    {
                        RenderFrame();
                    }));
                }
                else
                {
                    RenderFrame();
                }
            }
        }

        private void RenderFrame()
        {
            glControl.MakeCurrent();
            pcbManager.PcbRenderer.OnUpdateFrame(new FrameEventArgs(THREAD_PERIOD_MS / 1000.0F));
            glControl.Invalidate();
            glControl.SwapBuffers();
        }

        private void ThreadExceptionHandler(Exception ex)
        {
            MessageBox.Show(string.Format("Exception in RenderThread: {0}", ex.Message));
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            new Thread(() =>
            {
                BeginShutdown();
            }).Start();

            pcbManager.PcbRenderer.OnUnload();
            e.Cancel = true;
        }

        private void BeginShutdown()
        {
            if (renderThreadHelper != null)
            {
                renderThreadHelper.StopThreads();
                renderThreadHelper = null;
            }
        }

        private void CloseApplication()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    Application.Exit();
                }));
            }
            else
            {
                Application.Exit();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!splitContainer.Panel2.ContainsFocus)
            {
                if (!pcbManager.PcbRenderer.ActiveKeys.Contains(e.KeyData))
                {
                    pcbManager.PcbRenderer.ActiveKeys.Add(e.KeyData);
                }
            }

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            Keys? activeKey = pcbManager.PcbRenderer.ActiveKeys.FirstOrDefault(x => x.Equals(e.KeyData));

            if (activeKey.HasValue)
            {
                pcbManager.PcbRenderer.ActiveKeys.Remove(activeKey.Value);
            }

            base.OnKeyUp(e);
        }

        private void GlControl_PreviewKeyDown(object? sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Left ||
                e.KeyData == Keys.Right ||
                e.KeyData == Keys.Up ||
                e.KeyData == Keys.Down)
            {
                e.IsInputKey = true;
            }
        }

        private async void workspaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await LoadWorkspacesAsync();
        }

        private async Task LoadWorkspacesAsync()
        {
            try
            {
                mainMenu.Enabled = false;

                if ((workspaceToolStripMenuItem.DropDownItems != null) &&
                    (workspaceToolStripMenuItem.DropDownItems.Count == 0))
                {
                    workspaceToolStripMenuItem.Text = "Loading...";
                    StatusBusy("Loading workspace list...");

                    await NexarHelper.LoginAsync();
                    var nexarClient = NexarHelper.GetNexarClient();

                    var workspaces = await nexarClient.GetWorkspaces.ExecuteAsync();

                    workspaces.EnsureNoErrors();

                    if (workspaces?.Data != null)
                    {
                        var items = new List<ToolStripMenuItem>();

                        foreach (var workspace in workspaces.Data.DesWorkspaces)
                        {
                            var toolStripMenuItem = new ToolStripMenuItem();
                            toolStripMenuItem.Name = workspace.Id;
                            toolStripMenuItem.Text = workspace.Name;
                            toolStripMenuItem.Tag = workspace;
                            toolStripMenuItem.Click += ToolStripMenuItem_Click;
                            items.Add(toolStripMenuItem);
                        }

                        workspaceToolStripMenuItem.DropDownItems.AddRange(items.ToArray());
                    }

                    workspaceToolStripMenuItem.Text = "Workspaces";
                    StatusReady();

                    var defaultWorkspace = workspaces?.Data?.DesWorkspaces.FirstOrDefault(x => x.IsDefault == true);

                    if (defaultWorkspace != null)
                    {
                        ActiveWorkspace = defaultWorkspace;
                        Text = string.Format("Nexar.Renderer | {0}", (ActiveWorkspace?.Name ?? ""));
                        var defaultWorkspaceToolItem = workspaceToolStripMenuItem.DropDownItems.Find(defaultWorkspace.Id, true).FirstOrDefault() as ToolStripMenuItem;

                        if (defaultWorkspaceToolItem != null)
                        {
                            defaultWorkspaceToolItem.Checked = true;
                        }
                    }
                }
            }
            finally
            {
                mainMenu.Enabled = true;
            }
        }

        private async void OpenProjectMenuItem_Click(object sender, EventArgs e)
        {
            await LoadProjectsAsync();
        }

        private async Task LoadProjectsAsync()
        {
            try
            {
                mainMenu.Enabled = false;

                if (ActiveWorkspace == null)
                {
                    MessageBox.Show("Please select a workspace");
                }
                else
                {
                    var workspace = new Workspace()
                    {
                        Url = ActiveWorkspace.Url,
                        ApiUrl = ActiveWorkspace.Location.ApiServiceUrl
                    };

                    StatusBusy(string.Format("Loading projects in workspace '{0}'...", ActiveWorkspace.Name));
                    var projectsForm = new ProjectsForm();
                    await projectsForm.LoadProjectsAsync(workspace);
                    var result = projectsForm.ShowDialog();
                    StatusReady();

                    if (result == DialogResult.OK)
                    {
                        if (projectsForm.SelectedDesignProject != null)
                        {
                            Text = string.Format("Nexar.Renderer | {0} | {1}", (ActiveWorkspace?.Name ?? ""), projectsForm.SelectedDesignProject.Name);
                            StatusBusy(string.Format("Opening '{0}'...", projectsForm.SelectedDesignProject.Name));
                            
                            await pcbManager.OpenPcbDesignAsync(
                                workspace.ApiUrl,
                                projectsForm.SelectedDesignProject);
                            
                            StatusReady();
                            LoadLayers();
                            StatusBusy("Loading additional design data...");
                            await pcbManager.LoadAdditionalDesignDataAsync();
                            StatusReady();

                            // TODO: Fix this
                            commentThreads.PcbModel = pcbManager.PcbModel;
                            commentThreads.LoadCommentThreadsThreadSafe();

                            splitContainer.Panel2Collapsed =
                                (pcbManager.PcbModel?.Data?.DesProjectById?.Design?.WorkInProgress?.Variants[0]?.Pcb?.CommentThreads?.Count == 0);
                        }
                    }
                }
            }
            finally
            {
                mainMenu.Enabled = true;
            }
        }

        private void LoadLayers()
        {
            layersToolStripMenuItem.DropDownItems.Clear();
            var items = new List<ToolStripMenuItem>();
            pcbManager.PcbRenderer.Pcb.EnabledPcbLayers.Clear();

            foreach (var layer in pcbManager.PcbRenderer.Pcb.PcbLayers)
            {
                var toolStripMenuItem = new ToolStripMenuItem();
                toolStripMenuItem.Name = layer.Name.Replace(" ", "_");
                toolStripMenuItem.Text = layer.Name;
                toolStripMenuItem.Tag = layer;
                toolStripMenuItem.Click += ToolStripMenuItem_Click;
                toolStripMenuItem.Checked = true;
                toolStripMenuItem.CheckState = CheckState.Checked;
                toolStripMenuItem.CheckOnClick = true;
                toolStripMenuItem.CheckedChanged += LayerToolStripMenuItem_CheckedChanged;
                items.Add(toolStripMenuItem);

                pcbManager.PcbRenderer.Pcb.EnabledPcbLayers.Add(layer.Name);
            }

            layersToolStripMenuItem.DropDownItems.AddRange(items.ToArray());
        }

        private void ToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            var toolStripMenuItem = sender as ToolStripMenuItem;
            var workspace = toolStripMenuItem?.Tag as IGetWorkspaces_DesWorkspaces;

            if ((toolStripMenuItem != null) && (workspace != null))
            {
                foreach (ToolStripMenuItem item in workspaceToolStripMenuItem.DropDownItems)
                {
                    item.Checked = false;
                }

                toolStripMenuItem.Checked = true;
                ActiveWorkspace = workspace;
                Text = string.Format("Nexar.Renderer | {0}", ActiveWorkspace.Name);
            }
        }

        private void LayerToolStripMenuItem_CheckedChanged(object? sender, EventArgs e)
        {
            var toolStripMenuItem = sender as ToolStripMenuItem;
            var layer = toolStripMenuItem?.Tag as IPcbLayer;

            if ((toolStripMenuItem != null) && (layer != null))
            {
                if (toolStripMenuItem.Checked)
                {
                    if (!pcbManager.PcbRenderer.Pcb.EnabledPcbLayers.Contains(layer.Name))
                    {
                        pcbManager.PcbRenderer.Pcb.EnabledPcbLayers.Add(layer.Name);
                    }
                }
                else
                {
                    pcbManager.PcbRenderer.Pcb.EnabledPcbLayers.Remove(layer.Name);
                }
            }
        }


        private void TracksMenuItem_CheckedChanged(object? sender, EventArgs e)
        {
            pcbManager.PcbRenderer.Pcb.DisableTracks = (!tracksMenuItem.Checked);
        }

        private void PadsMenuItem_CheckedChanged(object? sender, EventArgs e)
        {
            pcbManager.PcbRenderer.Pcb.DisablePads = (!padsMenuItem.Checked);
        }

        private void ViasMenuItem_CheckedChanged(object? sender, EventArgs e)
        {
            pcbManager.PcbRenderer.Pcb.DisableVias = (!viasMenuItem.Checked);
        }

        private void ComponentOutlinesMenuItem_CheckedChanged(object? sender, EventArgs e)
        {
            pcbManager.PcbRenderer.Pcb.DisableComponentOutlines = (!componentOutlinesMenuItem.Checked);
        }

        private void CommentsMenuItem_CheckedChanged(object? sender, EventArgs e)
        {
            splitContainer.Panel2Collapsed = (!showCommentsMenuItem.Checked);
        }

        private void StatusBusy(string comment)
        {
            statusLabel.Text = comment;
            currentProgressBar.Visible = true;
            currentProgressBar.Style = ProgressBarStyle.Marquee;
            statusStrip.Refresh();
        }

        private void StatusReady()
        {
            statusLabel.Text = "Ready";
            currentProgressBar.Visible = false;
            statusStrip.Refresh();
        }
    }
}