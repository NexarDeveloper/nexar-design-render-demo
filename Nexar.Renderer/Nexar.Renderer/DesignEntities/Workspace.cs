using StrawberryShake;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexar.Renderer.DesignEntities
{
    public class Workspace
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Url { get; set; } = default!;
        public Location Location { get; set; }
    }
}
