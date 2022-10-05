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
    public class SingleLineShader : LineShader
    {
        const int START_X = 0;
        const int START_Y = 1;
        const int START_Z = 2;
        const int END_X = 6;
        const int END_Y = 7;
        const int END_Z = 8;

        public SingleLineShader(Color4 color, string? shaderVert = null, string? shaderFrag = null)
            : base(
                new float[]
                {
                    0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f,
                    0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f
                }, 
                color, 
                shaderVert, 
                shaderFrag)
        {
        }

        public void UpdateVertices(
            float? startX = null,
            float? startY = null,
            float? startZ = null,
            float? endX = null,
            float? endY = null,
            float? endZ = null)
        {
            if (startX.HasValue)
            {
                _vertices[START_X] = startX.Value;
            }

            if (startY.HasValue)
            {
                _vertices[START_Y] = startY.Value;
            }

            if (startZ.HasValue)
            {
                _vertices[START_Z] = startZ.Value;
            }

            if (endX.HasValue)
            {
                _vertices[END_X] = endX.Value;
            }

            if (endY.HasValue)
            {
                _vertices[END_Y] = endY.Value;
            }

            if (endZ.HasValue)
            {
                _vertices[END_Z] = endZ.Value;
            }
        }
    }
}
