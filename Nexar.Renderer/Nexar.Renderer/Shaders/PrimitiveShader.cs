using Nexar.Renderer.Geometry;
using Nexar.Renderer.Shapes;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IPcbLayer = Nexar.Client.IGetPcbModel_DesProjectById_Design_WorkInProgress_Variants_Pcb_LayerStack_Stacks_Layers;

namespace Nexar.Renderer.Shaders
{
    public class PrimitiveShader : TriangleShader
    {
        public List<Primitive> AssociatedPrimitives { get; } = new List<Primitive>();

        private float BaseZOffset { get; }

        public PrimitiveShader(float baseZOffset) 
            : base()
        {
            BaseZOffset = baseZOffset;
        }

        public void AddPrimitive(
            Primitive primitive,
            string? forcedLayer = null,
            float? forcedZOffset = null)
        {
            AssociatedPrimitives.Add(primitive);

            float zCoordinate = 0.0F;
            Color4 color = new Color4(0.75F, 0.75F, 0.75F, 1.0F);

            if ((forcedLayer ?? primitive.Layer.Name.ToLower()).Contains("top"))
            {
                color = GetColor(primitive);
                zCoordinate = 0.001F + (forcedZOffset ?? 0.0F);
            }
            else if ((forcedLayer ?? primitive.Layer.Name.ToLower()).Contains("bottom"))
            {
                color = GetColor(primitive);
                zCoordinate = -0.001F + (forcedZOffset ?? 0.0F);
            }

            AddVertices(
                primitive.TessellatedTriangles,
                zCoordinate,
                color);
        }

        public override void Draw(Matrix4 view, Matrix4 projection)
        {

            base.Draw(view, projection);
        }

        private Color4 GetColor(Primitive primitive)
        {
            if (primitive.Layer.Name.ToLower().Contains("top"))
            {
                return new Color4(1.0F, 0.0F, 0.0F, 1.0F);
            }
            else if (primitive.Layer.Name.ToLower().Contains("bottom"))
            {
                return new Color4(0.0F, 0.0F, 1.0F, 1.0F);
            }
            else
            {
                return new Color4(0.75F, 0.75F, 0.75F, 1.0F);
            }
        }
    }
}
