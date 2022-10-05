using Clipper2Lib;
using Nexar.Renderer.Shapes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexar.Renderer.Geometry
{
    public class Track : Primitive
    {
        public List<PointF> LineVertices { get; } = new List<PointF>();

        public List<PointF> PolygonVertices { get; } = new List<PointF>();

        public List<float> rawVertices { get; } = new List<float>();

        public Track(
            string layer,
            PointF beginMm,
            PointF endMm,
            double width)
        {
            Layer = layer;

            Paths64 subject = new Paths64();
            
            var newPath = CreateRectangleFromLine(beginMm, endMm);

            subject.Add(newPath);

            ClipperOffset inflatedTrack = new();

            inflatedTrack.AddPaths(subject, JoinType.Round, EndType.Joined);
            var result = inflatedTrack.Execute(width * SCALING_FACTOR);

            rawVertices.Clear();

            if ((result != null) && (result.Count > 0))
            {
                foreach (var element in result[0])
                {
                    LineVertices.Add(
                        new PointF(
                            ScaleDown(element.X),
                            ScaleDown(element.Y)));

                    rawVertices.Add(ScaleDown(element.X));
                    rawVertices.Add(ScaleDown(element.Y));
                }
            }

            Tessellate(rawVertices.ToArray());
        }

        private Path64 CreateRectangleFromLine(PointF beginMm, PointF endMm)
        {
            float expansionScalar = 0.0001F;

            double deltaX = Math.Abs(beginMm.X - endMm.X);
            double deltaY = Math.Abs(beginMm.Y - endMm.Y);

            var angleFromYAxisRads = Math.Atan(deltaX / deltaY);
            var angleFromYAxisDegs = angleFromYAxisRads * (180 / Math.PI);

            var xComp = Math.Cos(angleFromYAxisRads) * expansionScalar;
            var yComp = Math.Sin(angleFromYAxisRads) * expansionScalar;

            PointF beginLine1Mm = new PointF((float)(beginMm.X + xComp), (float)(beginMm.Y - yComp));
            PointF endLine1Mm = new PointF((float)(endMm.X + xComp), (float)(endMm.Y - yComp));
            PointF beginLine2Mm = new PointF((float)(beginMm.X - xComp), (float)(beginMm.Y + yComp));
            PointF endLine2Mm = new PointF((float)(endMm.X - xComp), (float)(endMm.Y + yComp));

            PolygonVertices.Clear();
            PolygonVertices.Add(beginLine1Mm);
            PolygonVertices.Add(endLine1Mm);
            PolygonVertices.Add(endLine2Mm);
            PolygonVertices.Add(beginLine2Mm);

            return MakeScaledUpPath(
                beginLine1Mm,
                endLine1Mm,
                endLine2Mm,
                beginLine2Mm);
        }
    }
}
