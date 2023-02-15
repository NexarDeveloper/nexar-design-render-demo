using Clipper2Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexar.Renderer.Geometry
{
    public class RayCasting
    {
        public bool IsPointInPolygon(PointF point, List<PointF> polygon)
        {
            bool result = false;

            if (polygon.Count > 0)
            {
                var lastPolygonPoint = polygon.Last();

                foreach (var polygonPoint in polygon)
                {
                    if ((polygonPoint.Y < point.Y) && (lastPolygonPoint.Y >= point.Y) ||
                        (lastPolygonPoint.Y < point.Y) && (polygonPoint.Y >= point.Y))
                    {
                        if (polygonPoint.X +
                            ((point.Y - polygonPoint.Y) / (lastPolygonPoint.Y - polygonPoint.Y) *
                            (lastPolygonPoint.X - polygonPoint.X)) <= point.X)
                        {
                            result = !result;
                        }
                    }

                    lastPolygonPoint = polygonPoint;
                }
            }

            return result;
        }
    }
}
