using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexar.Renderer.Shapes
{
    public class Triangle
    {
        public float V0X { get; }
        public float V0Y { get; }
        public float V1X { get; }
        public float V1Y { get; }
        public float V2X { get; }
        public float V2Y { get; }

        public Triangle(
            float v0X, 
            float v0Y, 
            float v1X, 
            float v1Y, 
            float v2X, 
            float v2Y)
        {
            V0X = v0X;
            V0Y = v0Y;
            V1X = v1X;
            V1Y = v1Y;
            V2X = v2X;
            V2Y = v2Y;
        }
    }
}
