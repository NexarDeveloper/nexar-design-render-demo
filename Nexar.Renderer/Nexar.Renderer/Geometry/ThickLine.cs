using Clipper2Lib;
using Nexar.Renderer.Shapes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IPcbLayer = Nexar.Client.IGetPcbModel_DesProjectById_Design_WorkInProgress_Variants_Pcb_LayerStack_Stacks_Layers;

namespace Nexar.Renderer.Geometry
{
    public class ThickLine : Track
    {
        public ThickLine(
            IPcbLayer? layer,
            PointF beginMm,
            PointF endMm)
            : base(layer, beginMm, endMm, 0.02f)
        {
        }
    }
}
