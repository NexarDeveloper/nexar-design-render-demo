using Nexar.Renderer.Geometry;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexar.Renderer.Shaders
{
    public class ViaShaderWrapper
    {
        public PrimitiveShader TopLayerPrimitiveShader { get; }
        public PrimitiveShader BottomLayerPrimitiveShader { get; }

        public ViaShaderWrapper()
        {
            TopLayerPrimitiveShader = new PrimitiveShader();
            BottomLayerPrimitiveShader = new PrimitiveShader();
        }

        public void Reset()
        {
            TopLayerPrimitiveShader.Reset();
            BottomLayerPrimitiveShader.Reset();
        }

        public void Initialise()
        {
            TopLayerPrimitiveShader.Initialise();
            BottomLayerPrimitiveShader.Initialise();
        }

        public void AddPrimitive(Primitive primitive)
        {
            TopLayerPrimitiveShader.AddPrimitive(primitive, "top layer", 0.0001F);
            BottomLayerPrimitiveShader.AddPrimitive(primitive, "bottom layer", 0.0001F);
        }

        public void Draw(Matrix4 view, Matrix4 projection)
        {
            TopLayerPrimitiveShader.Draw(view, projection);
            BottomLayerPrimitiveShader.Draw(view, projection);
        }

        public void Dispose()
        {
            TopLayerPrimitiveShader.Dispose();
            BottomLayerPrimitiveShader.Dispose();
        }
    }
}
