using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Nexar.Renderer.DesignEntities;
using Nexar.Renderer.Forms;
using Nexar.Renderer.Managers;
using Nexar.Renderer.Tools;
using OpenTk.Tutorial.Tools;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.WinForms;

using GLF = OpenTK.Windowing.GraphicsLibraryFramework;

namespace Nexar.Renderer.Visualization
{
    public class GlRenderer
    {
        private Stopwatch timer;

        private Axis axis = default!;
        public HighlightBox HighlightBox = default!;

        public Pcb Pcb { get; set; } = default!;

        private double time;

        private Matrix4 view;
        private Matrix4 projection;

        private float speed = 1.5f;
        private float speed2 = 15.0f;

        private Vector3 cameraPosition = new Vector3(0.0f, 0.0f, 5.0f);
        private Vector3 front = -Vector3.UnitZ;
        private Vector3 up = Vector3.UnitY;

        private float cameraYaw = 0.0f;
        private float cameraDistance = 5.0f;

        private int Width;
        private int Height;

        public Action<Point>? MouseUpCallback { get; set; }

        // TODO: Come back to native input (commented out as reminder)
        //private INativeInput nativeInput;

        /*public INativeInput NativeInput
        {
            get
            {
                return nativeInput;
            }

            set
            {
                nativeInput = value;
                nativeInput.KeyDown += NativeInput_KeyDown;
            }
        }*/

        private void NativeInput_KeyDown(KeyboardKeyEventArgs obj)
        {
            if (obj.Key == GLF.Keys.Up)
            {

            }
        }

        public GlRenderer(int width, int height, string title)
        {
            timer = new Stopwatch();
            timer.Start();

            Width = width;
            Height = height;
        }

        public PointF GetXYOnZeroZPlane(Point location)
        {
            var xy = ObjectPicker.GetXYOnZeroZPlane(location, view, projection);
            return new PointF(xy.Item1, xy.Item2);
        }

        public void Demo_MouseDown(object sender, Point location)
        {
            HighlightBox.XyStartVertices = ObjectPicker.GetXYOnZeroZPlane(location, view, projection);
        }

        public void Demo_MouseMove(object sender, Point location)
        {
            HighlightBox.XyEndVertices = ObjectPicker.GetXYOnZeroZPlane(location, view, projection);
        }

        public void Demo_MouseUp(object sender, Point location)
        {
            MouseUpCallback?.Invoke(location);
            HighlightBox.ResetHighlightBox();
        }

        public void MousePan(object sender, Point location)
        {

        }

        public void OnLoad()
        {
            GL.ClearColor(0.117f, 0.117f, 0.117f, 1.0f);

            axis = new Axis();
            HighlightBox = new HighlightBox();
            Pcb = new Pcb();

            //projection = Matrix4.CreateOrthographic(MathHelper.DegreesToRadians(45f) * 5, (Width / (float)Height) * 5, 0.1f, 100.0f);
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Width / (float)Height, 0.1f, 100.0f);

            GL.Enable(EnableCap.DepthTest);
        }

        public void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            axis.Dispose();
            HighlightBox.Dispose();
            Pcb.Dispose();
        }

        public List<Keys> ActiveKeys { get; } = new List<Keys>();

        public int ZoomRequest { get; set; } = 0;

        public void OnUpdateFrame(FrameEventArgs e)
        {
            time += 12.0 * 0.008; // e.Time;

            Vector3 cameraTarget = Vector3.Zero;
            Vector3 cameraUp = Vector3.UnitY;

            view = Matrix4.LookAt(
                cameraPosition,
                cameraTarget,
                cameraUp);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //DrawAxisLines();
            DrawPcb();
            DrawHighlightBox();

            if (ActiveKeys != null)
            {
                if (ActiveKeys.Any(x => x == Keys.Escape))
                {
                    //Exit();
                }

                // Camera speed
                speed = 15.0f;
                speed2 = 150.0f;

                if (ActiveKeys.Any(x => x == Keys.LShiftKey))
                {
                    speed *= 3.0f;
                    speed2 *= 3.0f;
                }

                // Reset
                if (ActiveKeys.Any(x => x == Keys.R))
                {
                    cameraYaw = 0.0f;
                    cameraDistance = 5.0f;
                    cameraPosition = new Vector3(0.0f, 0.0f, 5.0f);
                    front = new Vector3(0.0f, 0.0f, -1.0f);
                    up = new Vector3(0.0f, 1.0f, 0.0f);
                }

                // Forward 
                if (ActiveKeys.Any(x => x == Keys.W))
                {
                    cameraDistance -= speed * (float)e.Time;
                    cameraPosition += front * speed * (float)e.Time;
                }

                // Backward
                if (ActiveKeys.Any(x => x == Keys.S))
                {
                    cameraDistance += speed * (float)e.Time;
                    cameraPosition -= front * speed * (float)e.Time;
                }

                // Forward or backward
                if (ZoomRequest != 0)
                {
                    float scaledRequest = ZoomRequest * 0.01f;
                    cameraDistance += scaledRequest;
                    cameraPosition -= front * scaledRequest;
                    ZoomRequest = 0;
                }

                // Up
                if (ActiveKeys.Any(x => x == Keys.Up))
                {
                    cameraPosition += up * speed * (float)e.Time;
                }

                // Down
                if (ActiveKeys.Any(x => x == Keys.Down))
                {
                    cameraPosition -= up * speed * (float)e.Time;
                }

                // Left
                if (ActiveKeys.Any(x => x == Keys.Left))
                {
                    cameraYaw -= speed2 * (float)e.Time;
                }

                // Right
                if (ActiveKeys.Any(x => x == Keys.Right))
                {
                    cameraYaw += speed2 * (float)e.Time;
                }

                cameraPosition.X = cameraDistance * (float)Math.Sin(MathHelper.DegreesToRadians(cameraYaw));
                cameraPosition.Z = cameraDistance * (float)Math.Cos(MathHelper.DegreesToRadians(cameraYaw));
            }
        }

        private void DrawAxisLines()
        {
            axis?.Draw(view, projection);
        }

        private void DrawPcb()
        {
            Pcb?.Draw(view, projection);
        }

        private void DrawHighlightBox()
        {
            HighlightBox?.Draw(view, projection);
        }

        public void WindowReshape(int width, int height)
        {
            // Make the projection matrix active
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            // The "Graphics" width and height contain the desired pixel resolution to keep.
            GL.Ortho(0.0, Width, Height, 0.0, 1.0, -1.0);

            // Calculate the proper aspect ratio to use based on window ratio
            var ratioX = width / (float)Width;
            var ratioY = height / (float)Height;
            var ratio = ratioX < ratioY ? ratioY : ratioX;

            // Calculate the width and height that the will be rendered to
            var viewWidth = Convert.ToInt32(Width * ratio);
            var viewHeight = Convert.ToInt32(Height * ratio);

            // Calculate the position, which will apply proper "pillar" or "letterbox" 
            var viewX = Convert.ToInt32((width - Width * ratio) / 2);
            var viewY = Convert.ToInt32((height - Height * ratio) / 2);

            // Apply the viewport and switch back to the GL_MODELVIEW matrix mode
            GL.Viewport(viewX, viewY, viewWidth, viewHeight);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }
    }
}
