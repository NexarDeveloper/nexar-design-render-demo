using Clipper2Lib;
using Nexar.Renderer.Geometry;
using Nexar.Renderer.Shaders;
using Nexar.Client;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IPcbLayer = Nexar.Client.IGetPcbModel_DesProjectById_Design_WorkInProgress_Variants_Pcb_LayerStack_Stacks_Layers;
using System.Windows.Forms;

namespace Nexar.Renderer.DesignEntities
{
    public class Pcb
    {
        private class LayerInfo
        {
            public float ZOffset { get; set; }
            public Color4 Color { get; set; }
        }

        private readonly List<LayerInfo> TwoLayerInfo = new()
        {
            new LayerInfo() { ZOffset = 0.001F, Color = new Color4(1.0F, 0.0F, 0.0F, 1.0F) },
            new LayerInfo() { ZOffset = -0.001F, Color = new Color4(0.0F, 0.0F, 1.0F, 1.0F) }
        };

        private readonly List<LayerInfo> FourLayerInfo = new()
        {
            new LayerInfo() { ZOffset = 0.003F, Color = new Color4(1.0F, 0.0F, 0.0F, 1.0F) },
            new LayerInfo() { ZOffset = 0.001F, Color = new Color4(0.0F, 1.0F, 1.0F, 1.0F) },
            new LayerInfo() { ZOffset = -0.001F, Color = new Color4(0.0F, 1.0F, 0.0F, 1.0F) },
            new LayerInfo() { ZOffset = -0.003F, Color = new Color4(0.0F, 0.0F, 1.0F, 1.0F) }
        };

        private readonly List<LayerInfo> SixLayerInfo = new()
        {
            new LayerInfo() { ZOffset = 0.005F, Color = new Color4(1.0F, 0.0F, 0.0F, 1.0F) },
            new LayerInfo() { ZOffset = 0.003F, Color = new Color4(0.0F, 1.0F, 1.0F, 1.0F) },
            new LayerInfo() { ZOffset = 0.001F, Color = new Color4(1.0F, 0.0F, 1.0F, 1.0F) },
            new LayerInfo() { ZOffset = -0.001F, Color = new Color4(1.0F, 0.6F, 0.0F, 1.0F) },
            new LayerInfo() { ZOffset = -0.003F, Color = new Color4(0.0F, 1.0F, 0.0F, 1.0F) },
            new LayerInfo() { ZOffset = -0.005F, Color = new Color4(0.0F, 0.0F, 1.0F, 1.0F) }
        };

        private readonly LayerInfo unknownLayerInfo = new LayerInfo() { ZOffset = 0.0F, Color = new Color4(0.75F, 0.75F, 0.75F, 1.0F) };

        private LayerInfo GetLayerInfo(IPcbLayer pcbLayer)
        {
            LayerInfo layerInfo;

            try
            {
                switch (PcbLayers.Count)
                {
                    case 2:
                    {
                        layerInfo = TwoLayerInfo[PcbLayers.IndexOf(pcbLayer)];
                        break;
                    }
                    case 4:
                    {
                        layerInfo = FourLayerInfo[PcbLayers.IndexOf(pcbLayer)];
                        break;
                    }
                    case 6:
                    {
                        layerInfo = SixLayerInfo[PcbLayers.IndexOf(pcbLayer)];
                        break;
                    }
                    default:
                    {
                        layerInfo = unknownLayerInfo;
                        break;
                    }
                }
            }
            catch
            {
                layerInfo = unknownLayerInfo;
            }

            return layerInfo;
        }

        private List<SingleLineShader> boardOutlineShaders = new List<SingleLineShader>();
        //private PrimitiveShader trackShader = new PrimitiveShader();
        //private PrimitiveShader padShader = new PrimitiveShader(0.0F);
        private ViaShaderWrapper viaShader = new ViaShaderWrapper();

        private Dictionary<string, PrimitiveShader> layerMappedTrackShader = new Dictionary<string, PrimitiveShader>();
        private Dictionary<string, PrimitiveShader> layerMappedPadShader = new Dictionary<string, PrimitiveShader>();
        //private Dictionary<string, PrimitiveShader> layerMappedViaShader = new Dictionary<string, PrimitiveShader>();

        public List<IPcbLayer> PcbLayers { get; private set; } = new List<IPcbLayer>();

        public List<string> EnabledPcbLayers { get; } = new List<string>();

        public bool DisableTracks { get; set; } = false;
        public bool DisablePads { get; set; } = false;
        public bool DisableVias { get; set; } = false;

        public void InitialiseLayerStack(List<IPcbLayer> pcbLayers)
        {
            PcbLayers = pcbLayers.ToList();
        }

        public string GetStats()
        {
            long trackShaderTriangleCount = 0;
            layerMappedTrackShader.Values.ToList().ForEach(x => trackShaderTriangleCount += CountTriangles(x));

            long padShaderTriangleCount = 0;
            layerMappedPadShader.Values.ToList().ForEach(x => padShaderTriangleCount += CountTriangles(x));

            long viaShaderTriangleCount = 0;
            viaShader.ViaLayerShaderMapping.Values.ToList().ForEach(x => viaShaderTriangleCount += CountTriangles(x));

            var sb = new StringBuilder();
            sb.AppendLine("Geometry data");
            sb.AppendLine(string.Format("Track Shader Triangle Count:    {0}", trackShaderTriangleCount));
            sb.AppendLine(string.Format("Pad Shader Triangle Count:      {0}", padShaderTriangleCount));
            sb.AppendLine(string.Format("Via Shader Triangle Count:      {0}", viaShaderTriangleCount));

            return sb.ToString();
        }

        private long CountTriangles<T>(T shader) where T : PrimitiveShader
        {
            long triangleCount = 0;
            shader.AssociatedPrimitives.ForEach(x => triangleCount += x.TessellatedTriangles.Count);
            return triangleCount;
        }

        public void AddTrack(
            IPcbLayer layer,
            float beginXMm,
            float beginYMm,
            float endXMm,
            float endYMm,
            float width)
        {
            var track = new Track(
                layer,
                new PointF(beginXMm, beginYMm),
                new PointF(endXMm, endYMm),
                width);

            if (!layerMappedTrackShader.ContainsKey(layer.Name))
            {
                layerMappedTrackShader.Add(layer.Name, new PrimitiveShader(0.0F));
            }

            var layerInfo = GetLayerInfo(layer);
            layerMappedTrackShader[layer.Name].AddPrimitive(
                track,
                layerInfo.Color,
                layerInfo.ZOffset);
        }

        public void AddPad(
            IPcbLayer layer,
            DesPrimitiveShape primitiveShape,
            DesPadType padType,
            float sizeXMm,
            float sizeYMm,
            float positionXMm,
            float positionYMm,
            decimal rotation,
            float holeSizeMm)
        {
            var pad = new Pad(
                layer,
                primitiveShape,
                padType,
                new PointF(sizeXMm, sizeYMm),
                new PointF(positionXMm, positionYMm),
                rotation,
                holeSizeMm);

            if (!layerMappedPadShader.ContainsKey(layer.Name))
            {
                layerMappedPadShader.Add(layer.Name, new PrimitiveShader(0.0F));
            }

            var layerInfo = GetLayerInfo(layer);
            layerMappedPadShader[layer.Name].AddPrimitive(
                pad,
                layerInfo.Color,
                layerInfo.ZOffset);
        }

        public void AddVia(
            IPcbLayer layer,
            DesPrimitiveShape primitiveShape,
            float positionXMm,
            float positionYMm,
            float padDiameterMm,
            float holeDiameterMm)
        {
            var via = new Via(
                layer,
                primitiveShape,
                new PointF(positionXMm, positionYMm),
                padDiameterMm,
                holeDiameterMm);

            var layerInfo = GetLayerInfo(layer);
            viaShader.AddPrimitive(layer, via, layerInfo.ZOffset);
        }

        public void AddOutline(
            float startX,
            float startY,
            float endX,
            float endY)
        {
            var verticeShader = new SingleLineShader(new Color4(1.0f, 0.0f, 1.0f, 1.0f));
            verticeShader.Initialise();
            verticeShader.UpdateVertices(
                startX: startX,
                startY: startY,
                endX: endX,
                endY: endY);

            boardOutlineShaders.Add(verticeShader);
        }

        public void Reset()
        {
            boardOutlineShaders.ForEach(x => x.Reset());
            boardOutlineShaders.ForEach(x => x.Dispose());
            boardOutlineShaders.Clear();

            layerMappedTrackShader.Values.ToList().ForEach(x => x.Reset());
            layerMappedTrackShader.Values.ToList().ForEach(x => x.Dispose());
            layerMappedTrackShader.Clear();

            layerMappedPadShader.Values.ToList().ForEach(x => x.Reset());
            layerMappedPadShader.Values.ToList().ForEach(x => x.Dispose());
            layerMappedPadShader.Clear();

            viaShader.Reset();
        }

        public void FinaliseSetup()
        {
            boardOutlineShaders.ForEach(x => x.Initialise());
            layerMappedTrackShader.Values.ToList().ForEach(x => x.Initialise());
            layerMappedPadShader.Values.ToList().ForEach(x => x.Initialise());
            viaShader.Initialise();
        }

        public void Draw(Matrix4 view, Matrix4 projection)
        {
            boardOutlineShaders.ForEach(x => x.Draw(view, projection));

            if (!DisableTracks)
            {
                DrawLayerMappedPrimitives(view, projection, layerMappedTrackShader);
            }

            if (!DisablePads)
            {
                DrawLayerMappedPrimitives(view, projection, layerMappedPadShader);
            }

            if (!DisableVias)
            {
                viaShader.Draw(view, projection);
            }
        }

        private void DrawLayerMappedPrimitives(
            Matrix4 view, Matrix4 projection,
            Dictionary<string, PrimitiveShader> layerMappedShader)
        {
            foreach (var mappedLayer in layerMappedShader)
            {
                if (EnabledPcbLayers.Contains(mappedLayer.Key))
                {
                    mappedLayer.Value.Draw(view, projection);
                }
            }
        }

        public void Dispose()
        {
            boardOutlineShaders.ForEach(x => x.Dispose());
            layerMappedTrackShader.Values.ToList().ForEach(x => x.Dispose());
            layerMappedPadShader.Values.ToList().ForEach(x => x.Dispose());
            viaShader.Dispose();
        }
    }
}
