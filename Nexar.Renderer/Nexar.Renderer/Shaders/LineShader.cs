using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Nexar.Renderer.Shaders
{
    public class LineShader : Shader
    {
        private object drawLock = new object();

        private const int ARRAY_ELEMENT_LENGTH = 6;

        protected OpenGlBuffer buffer = new OpenGlBuffer();

        const int START_R = 3;
        const int START_G = 4;
        const int START_B = 5;
        const int END_R = 9;
        const int END_G = 10;
        const int END_B = 11;

        protected float[] _vertices;

        protected uint[] indices =
        {
            0, 1, 2
        };

        private bool initComplete = false;

        public LineShader(
            float[] vertices, 
            Color4 color, 
            string? shaderVert = null, 
            string? shaderFrag = null)
            : base(
                  shaderVert ?? "lineShader.vert", 
                  shaderFrag ?? "lineShader.frag")
        {
            if (vertices.Length % 12 != 0)
            {
                throw new ArgumentException(string.Format("Invalid number of vertices, expected factor 12 found factor {0}", vertices.Length % 12));
            }

            _vertices = vertices;

            for (int verticeIndex = 0; verticeIndex < _vertices.Length; verticeIndex += 12)
            {
                _vertices[verticeIndex + START_R] = color.R;
                _vertices[verticeIndex + START_G] = color.G;
                _vertices[verticeIndex + START_B] = color.B;
                _vertices[verticeIndex + END_R] = color.R;
                _vertices[verticeIndex + END_G] = color.G;
                _vertices[verticeIndex + END_B] = color.B;
            }
        }

        public void Reset()
        {
            lock (drawLock)
            {
                initComplete = false;
                _vertices = default!;
            }
        }

        public void Initialise()
        {
            buffer.VertexArray = GL.GenVertexArray();
            GL.BindVertexArray(buffer.VertexArray);

            buffer.VertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer.VertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StreamDraw);

            buffer.ElementBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, buffer.ElementBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            var positionLocation = GL.GetAttribLocation(Handle, "aPosition");

            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, ARRAY_ELEMENT_LENGTH * sizeof(float), 0);

            var colorLocation = GL.GetAttribLocation(Handle, "aColor");

            GL.EnableVertexAttribArray(colorLocation);
            GL.VertexAttribPointer(colorLocation, 3, VertexAttribPointerType.Float, false, ARRAY_ELEMENT_LENGTH * sizeof(float), 3 * sizeof(float));

            initComplete = true;
        }

        public void Draw(Matrix4 view, Matrix4 projection)
        {
            if (initComplete)
            {
                lock (drawLock)
                {

                    GL.BindBuffer(BufferTarget.ArrayBuffer, buffer.VertexBuffer);
                    GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StreamDraw);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, buffer.ElementBuffer);
                    GL.BindVertexArray(buffer.VertexArray);

                    Use();
                    SetMatrix4("model", Matrix4.Identity);
                    SetMatrix4("view", view);
                    SetMatrix4("projection", projection);

                    GL.DrawArrays(PrimitiveType.Lines, 0, _vertices.Length / ARRAY_ELEMENT_LENGTH);
                }
            }
        }
    }
}
