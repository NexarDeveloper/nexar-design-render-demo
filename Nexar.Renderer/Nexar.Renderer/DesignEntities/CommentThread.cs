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

        //public List<IPoint2D> Area { get; set; } = new List<IPoint2D>();
        //public List<IPoint2D> LArea { get; set; } = new List<IPoint2D>();
        //public override List<IPoint2D> LHitTestPolygon => LArea;

        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
