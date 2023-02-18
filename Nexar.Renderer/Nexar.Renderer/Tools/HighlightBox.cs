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
                Color4.White,
                "dottedLineShader.vert",
                "dottedLineShader.frag");

            highlightLineShader.Initialise();
        }

        public PointF XyStart { get; set; }
        public PointF XyEnd { get; set; }

        public bool StartComplete = false;
        public bool BoxComplete = false;

        public Tuple<float, float> XyStartVertices
        {
            set
            {
                XyStart = new PointF(value.Item1, value.Item2);

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

                StartComplete = true;
            }
        }

        public Tuple<float, float> XyEndVertices
        {
            set
            {
                XyEnd = new PointF(value.Item1, value.Item2);

                highlightBoxVertices[LINE_LEFT_END_Y] = value.Item2;
                highlightBoxVertices[LINE_BOTTOM_END_X] = value.Item1;

                highlightBoxVertices[LINE_RIGHT_START_X] = value.Item1;
                highlightBoxVertices[LINE_RIGHT_END_X] = value.Item1;
                highlightBoxVertices[LINE_RIGHT_END_Y] = value.Item2;

                highlightBoxVertices[LINE_TOP_START_Y] = value.Item2;
                highlightBoxVertices[LINE_TOP_END_X] = value.Item1;
                highlightBoxVertices[LINE_TOP_END_Y] = value.Item2;

                if (StartComplete)
                {
                    BoxComplete = true;
                }
            }
        }

        public void ResetHighlightBox()
        {
            for (int index = 0; index < highlightBoxVertices.Length; index += 6)
            {
                highlightBoxVertices[index] = 0.0F;
                highlightBoxVertices[index + 1] = 0.0F;
            }

            StartComplete = false;
            BoxComplete = false;
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
