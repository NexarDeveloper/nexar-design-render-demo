using Nexar.Renderer.Geometry;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IPcbLayer = Nexar.Client.IGetPcbModel_DesProjectById_Design_Variants_Pcb_LayerStack_Stacks_Layers;

namespace Nexar.Renderer.Shaders
{
    public class ViaShaderWrapper
    {
        public Dictionary<string, PrimitiveShader> ViaLayerShaderMapping { get; } = new Dictionary<string, PrimitiveShader>();

        public void Reset()
        {
            ViaLayerShaderMapping.Values.ToList().ForEach(x => x.Reset());
        }

        public void Initialise()
        {
            ViaLayerShaderMapping.Values.ToList().ForEach(x => x.Initialise());
        }

        public void AddPrimitive(IPcbLayer layer, Primitive primitive, float zOffset)
        {
            if (!ViaLayerShaderMapping.ContainsKey(layer.Name))
            {
                ViaLayerShaderMapping.Add(layer.Name, new PrimitiveShader(0.0F));
            }

            ViaLayerShaderMapping[layer.Name].AddPrimitive(
                primitive,
                new Color4(0.75F, 0.75F, 0.75F, 1.0F),
                (zOffset < 0.0F ? zOffset - 0.0001F : zOffset + 0.0001F));
        }

        public void Draw(Matrix4 view, Matrix4 projection)
        {
            ViaLayerShaderMapping.Values.ToList().ForEach(x => x.Draw(view, projection));
        }

        public void Dispose()
        {
            ViaLayerShaderMapping.Values.ToList().ForEach(x => x.Dispose());
        }
    }
}
