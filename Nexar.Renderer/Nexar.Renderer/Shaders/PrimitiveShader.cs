using Nexar.Renderer.Geometry;
using Nexar.Renderer.Shapes;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Color4 color,
            float zOffset)
        {
            AssociatedPrimitives.Add(primitive);

            AddVertices(
                primitive.TessellatedTriangles,
                BaseZOffset + zOffset,
                color);
        }

        public override void Draw(Matrix4 view, Matrix4 projection)
        {
            base.Draw(view, projection);
        }
    }
}
