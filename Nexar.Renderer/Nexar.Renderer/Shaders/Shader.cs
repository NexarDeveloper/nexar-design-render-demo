using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexar.Renderer
{
    public class Shader : IDisposable
    {
        public int Handle { get; }

        public Shader(string vertexPath, string fragmentPath, string? geometryPath = null)
        {
            int vertexShader = LoadShader(vertexPath, ShaderType.VertexShader);
            int fragmentShader = LoadShader(fragmentPath, ShaderType.FragmentShader);
            int geometryShader = -1;
            
            if (geometryPath != null)
            {
                geometryShader = LoadShader(geometryPath, ShaderType.GeometryShader);
            }

            // Deploy shader to GPU
            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, vertexShader);

            if (geometryShader != -1)
            {
                GL.AttachShader(Handle, geometryShader);
            }

            GL.AttachShader(Handle, fragmentShader);


            GL.LinkProgram(Handle);

            // Clean up!
            GL.DetachShader(Handle, vertexShader);

            if (geometryShader != -1)
            {
                GL.DetachShader(Handle, geometryShader);
            }

            GL.DetachShader(Handle, fragmentShader);

            GL.DeleteShader(fragmentShader);

            if (geometryShader != -1)
            {
                GL.DeleteShader(geometryShader);
            }

            GL.DeleteShader(vertexShader);


        }

        private int LoadShader(string shaderPath, ShaderType shaderType)
        {
            // Load shaders from source
            string shaderSource;

            using (StreamReader reader = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Shaders\GpuShaders\", shaderPath), Encoding.UTF8))
            {
                shaderSource = reader.ReadToEnd();
            }

            // Create shaders and bind to compiled source
            int shader = GL.CreateShader(shaderType);
            GL.ShaderSource(shader, shaderSource);

            // Compile shaders
            GL.CompileShader(shader);

            string infoLogVert = GL.GetShaderInfoLog(shader);
            if (infoLogVert != string.Empty)
            {
                Console.WriteLine(infoLogVert);
#if DEBUG
                throw new Exception(infoLogVert);
#endif
            }

            return shader;
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        public void SetMatrix4(string name, Matrix4 data)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.UniformMatrix4(location, true, ref data);
        }

        public void SetMatrix2(string name, Matrix2 data)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.UniformMatrix2(location, true, ref data);
        }

        public void SetFloat(string name, float data)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.Uniform1(location, 1, ref data);
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteProgram(Handle);

                disposedValue = true;
            }
        }

        ~Shader()
        {
            GL.DeleteProgram(Handle);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
