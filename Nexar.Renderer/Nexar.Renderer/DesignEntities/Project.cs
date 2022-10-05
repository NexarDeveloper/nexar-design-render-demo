using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexar.Renderer.DesignEntities
{
    public class Project
    {
        public string Name { get; set; } = default!;
        public string Id { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string PreviewUrl { get; set; } = default!;
        public Workspace? Workspace { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
