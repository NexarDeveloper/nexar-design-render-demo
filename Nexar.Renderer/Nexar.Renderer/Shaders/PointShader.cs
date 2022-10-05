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
    public class PointShader : Shader
    {
        private const int ARRAY_ELEMENT_LENGTH = 6;

        protected OpenGlBuffer buffer = new OpenGlBuffer();

        const int RED = 3;
        const int GREEN = 4;
        const int BLUE = 5;

        protected float[] _vertices;

        protected uint[] indices =
        {
            0, 1, 2
        };

        public PointShader(
            float[] vertices,
            Color4 color,
            string? shaderVert = null,
            string? shaderFrag = null)
            : base(
                  shaderVert ?? "pointShader.vert",
                  shaderFrag ?? "pointShader.frag")
        {
            if (vertices.Length % 6 != 0)
            {
                throw new ArgumentException(string.Format("Invalid number of vertices, expected factor 6 found factor {0}", vertices.Length % 6));
            }

            _vertices = vertices;

            for (int verticeIndex = 0; verticeIndex < _vertices.Length; verticeIndex += 6)
            {
                _vertices[verticeIndex + RED] = color.R;
                _vertices[verticeIndex + GREEN] = color.G;
                _vertices[verticeIndex + BLUE] = color.B;
            }
        }

        public void Initialise()
        {
            buffer.VertexArray = GL.GenVertexArray();
            GL.BindVertexArray(buffer.VertexArray);

            buffer.VertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer.VertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            buffer.ElementBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, buffer.ElementBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            var positionLocation = GL.GetAttribLocation(Handle, "aPosition");

            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, ARRAY_ELEMENT_LENGTH * sizeof(float), 0);

            var colorLocation = GL.GetAttribLocation(Handle, "aColor");

            GL.EnableVertexAttribArray(colorLocation);
            GL.VertexAttribPointer(colorLocation, 3, VertexAttribPointerType.Float, false, ARRAY_ELEMENT_LENGTH * sizeof(float), 3 * sizeof(float));
        }

        public void Draw(Matrix4 view, Matrix4 projection)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer.VertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, buffer.ElementBuffer);
            GL.BindVertexArray(buffer.VertexArray);

            Use();
            SetMatrix4("model", Matrix4.Identity);
            SetMatrix4("view", view);
            SetMatrix4("projection", projection);

            GL.DrawArrays(PrimitiveType.Points, 0, _vertices.Length / ARRAY_ELEMENT_LENGTH);
        }
    }
}
