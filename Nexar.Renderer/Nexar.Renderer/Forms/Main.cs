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

namespace Nexar.Renderer.Forms
{
    public partial class Main : Form
    {
        private GLControl glControl;

        private NexarHelper NexarHelper { get; }

        private IGetWorkspaces_DesWorkspaces? ActiveWorkspace { get; set; }

        private GlRenderer pcbRenderer;

        private PcbManager pcbManager;

        private ThreadHelper? renderThreadHelper;

        private const int THREAD_PERIOD_MS = 10;

        int glWidth = 1000;
        int glHeight = 800;

        //private INativeInput nativeInput;

        public Main()
        {
            InitializeComponent();

            glControl = new GLControl();

            glControl.Size = new Size(glWidth, glHeight);
            glControl.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left;
            glControl.Location = new Point(0, 0);
            glControl.Load += GlControl_Load;
            glControl.Resize += GlControl_Resize;

            glControl.MouseDown += GlControl_MouseDown;
            glControl.MouseMove += GlControl_MouseMove;
            glControl.MouseUp += GlControl_MouseUp;
            glControl.PreviewKeyDown += GlControl_PreviewKeyDown;

            Controls.Add(glControl);

            NexarHelper = new NexarHelper();

            pcbRenderer = new GlRenderer(glWidth, glHeight, "Nexar Renderer");
            pcbManager = new PcbManager(pcbRenderer);

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
        }

        private void GlControl_Load(object? sender, EventArgs e)
        {
            // Make sure that when the GLControl is resized or needs to be painted,
            // we update our projection matrix or re-render its contents, respectively.
            glControl.Resize += GlControl_Resize;

            pcbRenderer.OnLoad();

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
                    pcbRenderer.Demo_MouseMove(control, pt);
                }
            }
        }

        private void GlControl_MouseDown(object? sender, MouseEventArgs e)
        {
            Control? control = sender as Control;

            if (control != null)
            {
                Point pt = control.PointToClient(Control.MousePosition);
                Point pt2 = new Point(pt.X, control.Height - pt.Y);

                if (e.Button == MouseButtons.Left)
                {
                    pcbRenderer.Demo_MouseDown(control, pt);
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
                    pcbRenderer.Demo_MouseUp(control, pt);
                }
            }
        }

        private void GlControl_Resize(object? sender, EventArgs e)
        {
            pcbRenderer.WindowReshape(Width, Height);
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
            pcbRenderer.OnUpdateFrame(new FrameEventArgs(THREAD_PERIOD_MS / 1000.0F));
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

            pcbRenderer.OnUnload();
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
            if (!pcbRenderer.ActiveKeys.Contains(e.KeyData))
            {
                pcbRenderer.ActiveKeys.Add(e.KeyData);
            }

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            Keys? activeKey = pcbRenderer.ActiveKeys.FirstOrDefault(x => x.Equals(e.KeyData));

            if (activeKey.HasValue)
            {
                pcbRenderer.ActiveKeys.Remove(activeKey.Value);
            }

            base.OnKeyUp(e);
        }

        private void GlControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Left ||
                e.KeyData == Keys.Right ||
                e.KeyData == Keys.Up ||
                e.KeyData == Keys.Down)
            {
                e.IsInputKey = true;
            }
        }

        private async void OpenMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveWorkspace == null)
            {
                MessageBox.Show("Please select a workspace");
            }
            else
            {
                var workspace = new Workspace()
                {
                    Url = ActiveWorkspace.Url
                };

                var projectsForm = new ProjectsForm();
                await projectsForm.LoadProjectsAsync(workspace);
                var result = projectsForm.ShowDialog();

                if (result == DialogResult.OK)
                {
                    if (projectsForm.SelectedDesignProject != null)
                    {
                        Text = string.Format("Nexar.Renderer | {0} | {1}", (ActiveWorkspace?.Name ?? ""), projectsForm.SelectedDesignProject.Name);
                        await pcbManager.OpenPcbDesignAsync(projectsForm.SelectedDesignProject);
                    }
                }
            }
        }

        private async void workspaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await LoadWorkspacesAsync();
        }

        private async Task LoadWorkspacesAsync()
        {
            if ((workspaceToolStripMenuItem.DropDownItems != null) &&
                (workspaceToolStripMenuItem.DropDownItems.Count == 0))
            {
                workspaceToolStripMenuItem.Text = "Loading...";

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
    }
}