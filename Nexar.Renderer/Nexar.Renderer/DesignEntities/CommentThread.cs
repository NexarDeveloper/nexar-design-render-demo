using Clipper2Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Nexar.Renderer.DesignEntities
{
    public class CommentThread
    {
        public string CommentThreadId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.MinValue;

        public DateTime ModifiedAt { get; set; } = DateTime.MinValue;

        public List<Comment> Comments { get; set; } = new List<Comment>();

        public List<PointF> PolygonVertices { get; } = new List<PointF>();

        public CommentThread(
            string commentThreadId,
            float pos1X,
            float pos1Y,
            float pos2X,
            float pos2Y)
        {
            CommentThreadId = commentThreadId;

            float minX = Math.Min(pos1X, pos2X);
            float maxX = Math.Max(pos1X, pos2X);
            float minY = Math.Min(pos1Y, pos2Y);
            float maxY = Math.Max(pos1Y, pos2Y);

            PolygonVertices.Add(new PointF(minX, maxY));
            PolygonVertices.Add(new PointF(maxX, maxY));
            PolygonVertices.Add(new PointF(maxX, minY));
            PolygonVertices.Add(new PointF(minX, minY));
        }
    }
}
