using Clipper2Lib;
using Nexar.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IPcbLayer = Nexar.Client.IGetPcbModel_DesProjectById_Design_WorkInProgress_Variants_Pcb_LayerStack_Stacks_Layers;

namespace Nexar.Renderer.Geometry
{
    public class Pad : Primitive
    {
        public DesPrimitiveShape PrimitiveShape { get; }
        public DesPadType PadType { get; }
        public decimal Rotation { get; }
        public float HoleSizeM { get; }
        public List<PointF> PolygonVertices { get; } = new List<PointF>();
        public List<float> rawVertices { get; } = new List<float>();

        public Pad(
            IPcbLayer layer,
            DesPrimitiveShape primitiveShape,
            DesPadType padType,
            PointF sizeMm,
            PointF positionMm,
            decimal rotation,
            float holeSizeMm)
        {
            Layer = layer;
            PrimitiveShape = primitiveShape;
            PadType = padType;
            Rotation = rotation;
            HoleSizeM = holeSizeMm;

            if (primitiveShape == DesPrimitiveShape.Round)
            {
                float expansionScalar = 0.0001F;

                var inflatedPointVertices = new List<PointF>();
                inflatedPointVertices.Add(new PointF() { X = positionMm.X - expansionScalar, Y = positionMm.Y - expansionScalar });
                inflatedPointVertices.Add(new PointF() { X = positionMm.X + expansionScalar, Y = positionMm.Y - expansionScalar });
                inflatedPointVertices.Add(new PointF() { X = positionMm.X + expansionScalar, Y = positionMm.Y + expansionScalar });
                inflatedPointVertices.Add(new PointF() { X = positionMm.X - expansionScalar, Y = positionMm.Y + expansionScalar });

                Paths64 paths = new Paths64();
                var scaledUpPath = MakeScaledUpPath(inflatedPointVertices);
                paths.Add(scaledUpPath);
                ClipperOffset inflatedPad = new();
                inflatedPad.AddPaths(paths, JoinType.Round, EndType.Joined);
                Paths64 result = new Paths64();
                var outerResult = inflatedPad.Execute(sizeMm.X * SCALING_FACTOR);
                result.AddRange(outerResult);

                if (holeSizeMm > 0.0F)
                {
                    var innerResult = inflatedPad.Execute(holeSizeMm * SCALING_FACTOR);
                    result.AddRange(innerResult);
                }

                rawVertices.AddRange(MakeScaledDownVertices(result));
            }
            else
            {
                float halfPadX = sizeMm.X * 0.5F;
                float halfPadY = sizeMm.Y * 0.5F;

                PolygonVertices.Add(new PointF() { X = positionMm.X - halfPadX, Y = positionMm.Y - halfPadY });
                PolygonVertices.Add(new PointF() { X = positionMm.X + halfPadX, Y = positionMm.Y - halfPadY });
                PolygonVertices.Add(new PointF() { X = positionMm.X + halfPadX, Y = positionMm.Y + halfPadY });
                PolygonVertices.Add(new PointF() { X = positionMm.X - halfPadX, Y = positionMm.Y + halfPadY });

                if (Rotation != 0.0M)
                {
                    var rotatedPolygons = new List<PointF>();
                    var padCentre = new PointF() { X = positionMm.X, Y = positionMm.Y };
                    PolygonVertices.ForEach(x => rotatedPolygons.Add(Rotate(x, padCentre, (float)Rotation)));
                    PolygonVertices.Clear();
                    PolygonVertices.AddRange(rotatedPolygons);
                }

                PolygonVertices.ForEach(x =>
                {
                    rawVertices.Add(x.X);
                    rawVertices.Add(x.Y);
                });
            }

            Tessellate(rawVertices.ToArray());
        }
    }
}
