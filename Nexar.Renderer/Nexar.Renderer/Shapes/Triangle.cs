﻿using System;
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
        public float? V0Z { get; }
        public float V1X { get; }
        public float V1Y { get; }
        public float? V1Z { get; }
        public float V2X { get; }
        public float V2Y { get; }
        public float? V2Z { get; }

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

        public Triangle(
            float v0X,
            float v0Y,
            float v0Z,
            float v1X,
            float v1Y,
            float v1Z,
            float v2X,
            float v2Y,
            float v2Z,
            float? scaleFactor = null,
            float? offsetX = null,
            float? offsetY = null,
            float? offsetZ = null)
        {
            V0X = ScaleDown(v0X, scaleFactor, offsetX);
            V0Y = ScaleDown(v0Y, scaleFactor, offsetY);
            V0Z = ScaleDown(v0Z, scaleFactor, offsetZ);
            V1X = ScaleDown(v1X, scaleFactor, offsetX);
            V1Y = ScaleDown(v1Y, scaleFactor, offsetY);
            V1Z = ScaleDown(v1Z, scaleFactor, offsetZ);
            V2X = ScaleDown(v2X, scaleFactor, offsetX);
            V2Y = ScaleDown(v2Y, scaleFactor, offsetY);
            V2Z = ScaleDown(v2Z, scaleFactor, offsetZ);
        }

        protected float ScaleDown(
            float input, 
            float? scaleFactor = null,
            float? offset = null)
        {
            return (float)(((float)input / (scaleFactor ?? 1)) + (offset ?? 0.0f));
        }
    }
}
