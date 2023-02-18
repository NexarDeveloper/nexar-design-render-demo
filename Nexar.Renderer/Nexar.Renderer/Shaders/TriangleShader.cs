using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nexar.Renderer.Shapes;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Nexar.Renderer.Shaders
{
    public class TriangleShader : Shader
    {
        private const int ARRAY_ELEMENT_LENGTH = 6;

        private readonly object drawLock = new object();

        protected OpenGlBuffer buffer = new OpenGlBuffer();

        const int RED = 3;
        const int GREEN = 4;
        const int BLUE = 5;

        protected List<float> vertices = new List<float>();

        private float[] _vertices = default!;

        private bool initComplete = false;

        protected uint[] indices =
        {
            0, 1, 2
        };

        public TriangleShader(
            string? shaderVert = null,
            string? shaderFrag = null,
            string? shaderGeom = null)
            : base(
                  shaderVert ?? "triangleShader.vert",
                  shaderFrag ?? "triangleShader.frag")
        {
        }

        public void AddVertices(List<Triangle> triangles, float zCoordinate, Color4 color)
        {
            var shaderVertices = new List<float>();

            foreach (var triangle in triangles)
            {
                shaderVertices.AddRange(
                    new float[] { triangle.V0X, triangle.V0Y, zCoordinate, 0.0F, 0.0F, 0.0F });
                shaderVertices.AddRange(
                    new float[] { triangle.V1X, triangle.V1Y, zCoordinate, 0.0F, 0.0F, 0.0F });
                shaderVertices.AddRange(
                    new float[] { triangle.V2X, triangle.V2Y, zCoordinate, 0.0F, 0.0F, 0.0F });
            }

            for (int verticeIndex = 0; verticeIndex < shaderVertices.Count; verticeIndex += 6)
            {
                shaderVertices[verticeIndex + RED] = color.R;
                shaderVertices[verticeIndex + GREEN] = color.G;
                shaderVertices[verticeIndex + BLUE] = color.B;
            }

            vertices.AddRange(shaderVertices);
        }

        public void Reset()
        {
            lock (drawLock)
            {
                initComplete = false;
                vertices.Clear();
                _vertices = default!;
            }
        }

        public void Initialise()
        {
            if (vertices.Count > 0)
            {
                _vertices = vertices.ToArray();

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

                initComplete = true;
            }
        }

        public virtual void Draw(Matrix4 view, Matrix4 projection)
        {
            if (initComplete)
            {
                lock (drawLock)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, buffer.VertexBuffer);
                    GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, buffer.ElementBuffer);
                    GL.BindVertexArray(buffer.VertexArray);

                    Use();
                    SetMatrix4("model", Matrix4.Identity);
                    SetMatrix4("view", view);
                    SetMatrix4("projection", projection);

                    GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Length / ARRAY_ELEMENT_LENGTH);
                    //GL.DrawElements(PrimitiveType.Triangles, _vertices.Length / ARRAY_ELEMENT_LENGTH, DrawElementsType.UnsignedInt, 0);
                }
            }
        }
    }
}
