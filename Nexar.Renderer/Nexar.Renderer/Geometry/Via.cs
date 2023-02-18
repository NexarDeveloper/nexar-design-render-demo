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
    public class Via : Primitive
    {
        public DesPrimitiveShape PrimitiveShape { get; }
        public float PadDiameterMm { get; }
        public float HoleDiameterMm { get; }
        public List<float> rawVertices { get; } = new List<float>();

        public Via(
            IPcbLayer layer,
            DesPrimitiveShape primitiveShape,
            PointF positionMm,
            float padDiameterMm,
            float holeDiameterMm)
        {
            Layer = layer;
            PrimitiveShape = primitiveShape;
            PadDiameterMm = padDiameterMm;
            HoleDiameterMm = holeDiameterMm;

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
                var result = inflatedPad.Execute(PadDiameterMm * SCALING_FACTOR);
                rawVertices.AddRange(MakeScaledDownVertices(result));
            }

            if (rawVertices.Count > 0)
            {
                Tessellate(rawVertices.ToArray());
            }
        }
    }
}
