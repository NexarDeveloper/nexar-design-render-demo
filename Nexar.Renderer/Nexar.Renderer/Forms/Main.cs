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

namespace Nexar.Renderer.Forms
{
    public partial class Main : Form
    {
        private NexarHelper NexarHelper { get; }

        private IGetWorkspaces_DesWorkspaces? ActiveWorkspace { get; set; }

        //private GlRenderer pcbManager.PcbRenderer;

        private PcbManager pcbManager;

        private ThreadHelper? renderThreadHelper;

        private const int THREAD_PERIOD_MS = 50;

        int glWidth = 800;
        int glHeight = 600;

        //private INativeInput nativeInput;

        public Main()
        {
            InitializeComponent();

            tracksMenuItem.CheckedChanged += TracksMenuItem_CheckedChanged;
            padsMenuItem.CheckedChanged += PadsMenuItem_CheckedChanged;
            viasMenuItem.CheckedChanged += ViasMenuItem_CheckedChanged;

            NexarHelper = new NexarHelper();

            pcbManager = new PcbManager(new GlRenderer(glWidth, glHeight, "Nexar Renderer"));

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

            pcbManager.PcbRenderer.OnLoad();

            // Ensure that the viewport and projection matrix are set correctly initially.
            GlControl_Resize(glControl, EventArgs.Empty);
        }

        private void GlControl_MouseMove(object sender, MouseEventArgs e)
        {
            Control? control = sender as Control;

            if (control != null)
            {
                Point pt = control.PointToClient(Control.MousePosition);
                Point pt2 = new Point(pt.X, control.Height - pt.Y);

                if (e.Button == MouseButtons.Left)
                {
                    pcbManager.PcbRenderer.Demo_MouseMove(sender, pt);
                }
            }
        }

        private void GlControl_MouseDown(object sender, MouseEventArgs e)
        {
            Control? control = sender as Control;

            if (control != null)
            {
                Point pt = control.PointToClient(Control.MousePosition);
                Point pt2 = new Point(pt.X, control.Height - pt.Y);

                if (e.Button == MouseButtons.Left)
                {
                    pcbManager.PcbRenderer.Demo_MouseDown(sender, pt);
                }
            }
        }

        private void GlControl_MouseUp(object sender, MouseEventArgs e)
        {
            Control? control = sender as Control;

            if (control != null)
            {
                Point pt = control.PointToClient(Control.MousePosition);

                if (e.Button == MouseButtons.Left)
                {
                    pcbManager.PcbRenderer.Demo_MouseUp(sender, pt);
                }
            }
        }

        private void GlControl_Resize(object? sender, EventArgs e)
        {
            pcbManager.PcbRenderer.WindowReshape(Width, Height);
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
            if (!pcbManager.PcbRenderer.ActiveKeys.Contains(e.KeyData))
            {
                pcbManager.PcbRenderer.ActiveKeys.Add(e.KeyData);
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

        private async void OpenProjectMenuItem_Click(object sender, EventArgs e)
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
                        LoadLayers();
                    }
                }
            }
        }

        private void LoadLayers()
        {
            layersToolStripMenuItem.DropDownItems.Clear();
            var items = new List<ToolStripMenuItem>();

            foreach (var layer in pcbManager.PcbLayers)
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

        private void LayerToolStripMenuItem_CheckedChanged(object? sender, EventArgs e)
        {
            var toolStripMenuItem = sender as ToolStripMenuItem;
            var layer = toolStripMenuItem?.Tag as IPcbLayer;

            if ((toolStripMenuItem != null) && (layer != null))
            {
                if (toolStripMenuItem.Checked)
                {
                    pcbManager.PcbRenderer.Pcb.EnabledPcbLayers.Add(layer.Name);
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
    }
}