using Nexar.Renderer.Shaders;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexar.Renderer.Tools
{
    public class Axis
    {
        List<LineShader> axis = new List<LineShader>();

        public Axis()
        {
            var xAxisShader = new SingleLineShader(new Color4(1.0f, 0.0f, 0.0f, 1.0f));
            xAxisShader.Initialise();
            xAxisShader.UpdateVertices(startX: -1.0f, endX: 1.0f);
            axis.Add(xAxisShader);

            var yAxisShader = new SingleLineShader(new Color4(0.0f, 1.0f, 0.0f, 1.0f));
            yAxisShader.Initialise();
            yAxisShader.UpdateVertices(startY: -1.0f, endY: 1.0f);
            axis.Add(yAxisShader);

            var zAxisShader = new SingleLineShader(new Color4(0.0f, 0.0f, 1.0f, 1.0f));
            zAxisShader.Initialise();
            zAxisShader.UpdateVertices(startZ: -1.0f, endZ: 1.0f);
            axis.Add(zAxisShader);
        }

        public void Draw(Matrix4 view, Matrix4 projection)
        {
            axis.ForEach(x => x.Draw(view, projection));
        }

        public void Dispose()
        {
            axis.ForEach(x => x.Dispose());
        }
    }
}
