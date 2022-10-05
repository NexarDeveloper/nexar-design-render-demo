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

namespace Nexar.Renderer.Forms
{
    public partial class Main : Form
    {
        private GlRenderer pcbRenderer;

        private PcbManager pcbManager;

        private ThreadHelper? threadHelper;

        private const int THREAD_PERIOD_MS = 100;

        int glWidth = 800;
        int glHeight = 600;

        //private INativeInput nativeInput;

        public Main()
        {
            InitializeComponent();

            pcbRenderer = new GlRenderer(glWidth, glHeight, "Nexar Renderer");
            pcbManager = new PcbManager(pcbRenderer);

            if (threadHelper == null)
            {
                threadHelper = new ThreadHelper(
                    "RenderThread",
                    THREAD_PERIOD_MS,
                    new Action(RenderFrameThreadSafe),
                    ThreadExceptionHandler);

                threadHelper.StartThreads();
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

        private void GlControl_MouseMove(object sender, MouseEventArgs e)
        {
            Control? control = sender as Control;

            if (control != null)
            {
                Point pt = control.PointToClient(Control.MousePosition);
                Point pt2 = new Point(pt.X, control.Height - pt.Y);

                if (e.Button == MouseButtons.Left)
                {
                    pcbRenderer.Demo_MouseMove(sender, pt);
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
                    pcbRenderer.Demo_MouseDown(sender, pt);
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
                    pcbRenderer.Demo_MouseUp(sender, pt);
                }
            }
        }

        private void GlControl_Resize(object? sender, EventArgs e)
        {
            pcbRenderer.WindowReshape(Width, Height);
        }

        private void RenderFrameThreadSafe()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate
                {
                    RenderFrame();
                }));
            }
            else
            {
                RenderFrame();
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
            if (threadHelper != null)
            {
                threadHelper.StopThreads();
                threadHelper = null;
            }

            pcbRenderer.OnUnload();
            base.OnClosing(e);
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
            var workspace = new Workspace()
            {
                Url = Environment.GetEnvironmentVariable("NEXAR_WORKSPACE_URL") ?? throw new InvalidOperationException("Please set environment 'NEXAR_WORKSPACE_URL'")
            };

            var projectsForm = new ProjectsForm();
            await projectsForm.LoadProjectsAsync(workspace);
            var result = projectsForm.ShowDialog();

            if (result == DialogResult.OK)
            {
                if (projectsForm.SelectedDesignProject != null)
                {
                    Text = string.Format("Nexar.Renderer - {0}", projectsForm.SelectedDesignProject.Name);
                    await pcbManager.OpenPcbDesignAsync(projectsForm.SelectedDesignProject);
                }
            }
        }
    }
}