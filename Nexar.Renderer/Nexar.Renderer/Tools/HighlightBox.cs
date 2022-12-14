using Nexar.Renderer.Shaders;
using OpenTK;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTk.Tutorial.Tools
{
    public class HighlightBox : IDisposable
    {
        private const int LINE_LEFT_START_X = 0;
        private const int LINE_LEFT_START_Y = 1;
        private const int LINE_LEFT_END_X = 6;
        private const int LINE_LEFT_END_Y = 7;
        private const int LINE_RIGHT_START_X = 12;
        private const int LINE_RIGHT_START_Y = 13;
        private const int LINE_RIGHT_END_X = 18;
        private const int LINE_RIGHT_END_Y = 19;
        private const int LINE_TOP_START_X = 24;
        private const int LINE_TOP_START_Y = 25;
        private const int LINE_TOP_END_X = 30;
        private const int LINE_TOP_END_Y = 31;
        private const int LINE_BOTTOM_START_X = 36;
        private const int LINE_BOTTOM_START_Y = 37;
        private const int LINE_BOTTOM_END_X = 42;
        private const int LINE_BOTTOM_END_Y = 43;

        private float[] highlightBoxVertices =
        {
            0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f
        };

        private LineShader highlightLineShader;

        public HighlightBox()
        {
            highlightLineShader = new LineShader(
                highlightBoxVertices,
                Color4.Purple,
                "dottedLineShader.vert",
                "dottedLineShader.frag");

            highlightLineShader.Initialise();
        }

        public Tuple<float, float> XYStart
        {
            set
            {
                highlightBoxVertices[LINE_LEFT_START_X] = value.Item1;
                highlightBoxVertices[LINE_LEFT_START_Y] = value.Item2;
                highlightBoxVertices[LINE_LEFT_END_X] = value.Item1;
                highlightBoxVertices[LINE_LEFT_END_Y] = value.Item2;

                highlightBoxVertices[LINE_BOTTOM_START_X] = value.Item1;
                highlightBoxVertices[LINE_BOTTOM_START_Y] = value.Item2;
                highlightBoxVertices[LINE_BOTTOM_END_X] = value.Item1;
                highlightBoxVertices[LINE_BOTTOM_END_Y] = value.Item2;

                highlightBoxVertices[LINE_RIGHT_START_X] = value.Item1;
                highlightBoxVertices[LINE_RIGHT_START_Y] = value.Item2;
                highlightBoxVertices[LINE_RIGHT_END_X] = value.Item1;
                highlightBoxVertices[LINE_RIGHT_END_Y] = value.Item2;

                highlightBoxVertices[LINE_TOP_START_X] = value.Item1;
                highlightBoxVertices[LINE_TOP_START_Y] = value.Item2;
                highlightBoxVertices[LINE_TOP_END_X] = value.Item1;
                highlightBoxVertices[LINE_TOP_END_Y] = value.Item2;
            }
        }

        public Tuple<float, float> XYEnd
        {
            set
            {
                highlightBoxVertices[LINE_LEFT_END_Y] = value.Item2;
                highlightBoxVertices[LINE_BOTTOM_END_X] = value.Item1;

                highlightBoxVertices[LINE_RIGHT_START_X] = value.Item1;
                highlightBoxVertices[LINE_RIGHT_END_X] = value.Item1;
                highlightBoxVertices[LINE_RIGHT_END_Y] = value.Item2;

                highlightBoxVertices[LINE_TOP_START_Y] = value.Item2;
                highlightBoxVertices[LINE_TOP_END_X] = value.Item1;
                highlightBoxVertices[LINE_TOP_END_Y] = value.Item2;
            }
        }

        public void ResetHighlightBox()
        {
            for (int index = 0; index < highlightBoxVertices.Length; index += 6)
            {
                highlightBoxVertices[index] = 0.0F;
                highlightBoxVertices[index + 1] = 0.0F;
            }
        }

        public void Draw(Matrix4 view, Matrix4 projection)
        {
            highlightLineShader.Draw(view, projection);
        }

        public void Dispose()
        {
            highlightLineShader.Dispose();
        }
    }
}
