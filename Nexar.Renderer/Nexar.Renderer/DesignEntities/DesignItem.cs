using Nexar.Renderer.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexar.Renderer.DesignEntities
{
    public class DesignItem
    {
        public string Id { get; }
        public string Designator { get; }
        public string Comment { get; }

        public List<PointF> PolygonVertices { get; } = new List<PointF>();
        public Tuple<Point, Point> BoundingRectangleCoords { get; }

        private readonly RayCasting rayCasting = new RayCasting();

        public DesignItem(
            string id,
            string designator, 
            string comment,
            Tuple<Point, Point> boundingRectangleCoords,
            float pos1X,
            float pos1Y,
            float pos2X,
            float pos2Y)
        {
            Id = id;
            Designator = designator;
            Comment = comment;
            BoundingRectangleCoords = boundingRectangleCoords;

            float minX = Math.Min(pos1X, pos2X);
            float maxX = Math.Max(pos1X, pos2X);
            float minY = Math.Min(pos1Y, pos2Y);
            float maxY = Math.Max(pos1Y, pos2Y);

            PolygonVertices.Add(new PointF(minX, maxY));
            PolygonVertices.Add(new PointF(pos2X, pos2Y));
            PolygonVertices.Add(new PointF(maxX, minY));
            PolygonVertices.Add(new PointF(pos1X, pos1Y));
        }

        public bool HitTest(PointF location)
        {
            return rayCasting.IsPointInPolygon(location, PolygonVertices);
        }
    }
}
