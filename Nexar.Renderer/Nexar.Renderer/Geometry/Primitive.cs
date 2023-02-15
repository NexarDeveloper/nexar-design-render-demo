using Clipper2Lib;
using Nexar.Renderer.Shapes;
using IdentityModel.OidcClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IPcbLayer = Nexar.Client.IGetPcbModel_DesProjectById_Design_WorkInProgress_Variants_Pcb_LayerStack_Stacks_Layers;

namespace Nexar.Renderer.Geometry
{
    public abstract class Primitive
    {
        public const int SCALING_FACTOR = 10000;
        public IPcbLayer Layer { get; protected set; } = default!;

        public List<PointF> PolygonVertices { get; } = new List<PointF>();

        public List<Triangle> TessellatedTriangles { get; } = new List<Triangle>();

        private readonly RayCasting rayCasting = new RayCasting();

        public bool HitTest(PointF location)
        {
            return rayCasting.IsPointInPolygon(location, PolygonVertices);
        }

        protected void Tessellate(float[] inputData)
        {
            // Create an instance of the tessellator. Can be reused.
            var tess = new LibTessDotNet.Tess();

            // Construct the contour from inputData.
            // A polygon can be composed of multiple contours which are all tessellated at the same time.
            int numPoints = inputData.Length / 2;
            var contour = new LibTessDotNet.ContourVertex[numPoints];
            for (int i = 0; i < numPoints; i++)
            {
                // NOTE : Z is here for convenience if you want to keep a 3D vertex position throughout the tessellation process but only X and Y are important.
                contour[i].Position = new LibTessDotNet.Vec3(inputData[i * 2], inputData[i * 2 + 1], 0);
                // Data can contain any per-vertex data, here a constant color.
                contour[i].Data = Color.Azure;
            }
            // Add the contour with a specific orientation, use "Original" if you want to keep the input orientation.
            tess.AddContour(contour, LibTessDotNet.ContourOrientation.Original);

            // Tessellate!
            // The winding rule determines how the different contours are combined together.
            // See http://www.glprogramming.com/red/chapter11.html (section "Winding Numbers and Winding Rules") for more information.
            // If you want triangles as output, you need to use "Polygons" type as output and 3 vertices per polygon.
            tess.Tessellate(LibTessDotNet.WindingRule.EvenOdd, LibTessDotNet.ElementType.Polygons, 3, VertexCombine);

            // Same call but the last callback is optional. Data will be null because no interpolated data would have been generated.
            //tess.Tessellate(LibTessDotNet.WindingRule.EvenOdd, LibTessDotNet.ElementType.Polygons, 3); // Some vertices will have null Data in this case.

            TessellatedTriangles.Clear();

            //Debug.Print("Output triangles:");
            int numTriangles = tess.ElementCount;

            for (int i = 0; i < numTriangles; i++)
            {
                var v0 = tess.Vertices[tess.Elements[i * 3]].Position;
                var v1 = tess.Vertices[tess.Elements[i * 3 + 1]].Position;
                var v2 = tess.Vertices[tess.Elements[i * 3 + 2]].Position;

                var triangle = new Triangle(
                    v0.X,
                    v0.Y,
                    v1.X,
                    v1.Y,
                    v2.X,
                    v2.Y);

                TessellatedTriangles.Add(triangle);

                //Debug.Print("#{0} ({1:F1},{2:F1}) ({3:F1},{4:F1}) ({5:F1},{6:F1})", i, v0.X, v0.Y, v1.X, v1.Y, v2.X, v2.Y);
            }
        }

        protected static object VertexCombine(LibTessDotNet.Vec3 position, object[] data, float[] weights)
        {
            // Fetch the vertex data.
            var colors = new Color[] { (Color)data[0], (Color)data[1], (Color)data[2], (Color)data[3] };
            // Interpolate with the 4 weights.
            var rgba = new float[] {
                (float)colors[0].R * weights[0] + (float)colors[1].R * weights[1] + (float)colors[2].R * weights[2] + (float)colors[3].R * weights[3],
                (float)colors[0].G * weights[0] + (float)colors[1].G * weights[1] + (float)colors[2].G * weights[2] + (float)colors[3].G * weights[3],
                (float)colors[0].B * weights[0] + (float)colors[1].B * weights[1] + (float)colors[2].B * weights[2] + (float)colors[3].B * weights[3],
                (float)colors[0].A * weights[0] + (float)colors[1].A * weights[1] + (float)colors[2].A * weights[2] + (float)colors[3].A * weights[3]
            };
            // Return interpolated data for the new vertex.
            return Color.FromArgb((int)rgba[3], (int)rgba[0], (int)rgba[1], (int)rgba[2]);
        }

        protected int ScaleUp(double input, int? scaleFactor = null)
        {
            return (int)(input * (scaleFactor ?? SCALING_FACTOR));
        }

        protected float ScaleDown(long input, int? scaleFactor = null)
        {
            return (float)((float)input / (scaleFactor ?? SCALING_FACTOR));
        }

        protected Path64 MakeScaledUpPath(List<PointF> points)
        {
            if (points.Count == 4)
            {
                return MakeScaledUpPath(
                    points[0],
                    points[1],
                    points[2],
                    points[3]);
            }

            throw new Exception("4 points required to make scaled up path");
        }

        protected Path64 MakeScaledUpPath(
            PointF point1,
            PointF point2,
            PointF point3,
            PointF point4)
        {
            Point pointA = new Point(
                ScaleUp(point1.X),
                ScaleUp(point1.Y));

            Point pointB = new Point(
                ScaleUp(point2.X),
                ScaleUp(point2.Y));

            Point pointC = new Point(
                ScaleUp(point3.X),
                ScaleUp(point3.Y));

            Point pointD = new Point(
                ScaleUp(point4.X),
                ScaleUp(point4.Y));

            return Clipper.MakePath(
                new long[]
                {
                    pointA.X,
                    pointA.Y,
                    pointB.X,
                    pointB.Y,
                    pointC.X,
                    pointC.Y,
                    pointD.X,
                    pointD.Y
                });
        }

        protected List<float> MakeScaledDownVertices(Paths64 results)
        {
            var rawVertices = new List<float>();

            if ((results != null) && (results.Count > 0))
            {
                foreach (var result in results)
                {
                    foreach (var element in result)
                    {
                        rawVertices.Add(ScaleDown(element.X));
                        rawVertices.Add(ScaleDown(element.Y));
                    }
                }
            }

            return rawVertices;
        }

        protected PointF Rotate(PointF point, PointF rotationPoint, float angleDeg)
        {
            var angleRad = DegreesToRadians(angleDeg);

            float transX = rotationPoint.X + ((point.X - rotationPoint.X) * (float)Math.Cos(angleRad)) - ((point.Y - rotationPoint.Y) * (float)Math.Sin(angleRad));
            float transY = rotationPoint.Y + ((point.X - rotationPoint.X) * (float)Math.Sin(angleRad)) + ((point.Y - rotationPoint.Y) * (float)Math.Cos(angleRad));

            return new PointF() { X = transX, Y = transY };
        }

        protected float DegreesToRadians(float degrees)
        {
            return degrees * (float)(Math.PI / 180.0);
        }
    }
}
