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
using System.Reflection.Emit;

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

        private PrimitiveShader boardOutlineShader = new PrimitiveShader(0.0f);
        private PrimitiveShader componentOutlineShader = new PrimitiveShader(0.0f);
        private PrimitiveShader commentAreaShader = new PrimitiveShader(0.0f);
        private ViaShaderWrapper viaShader = new ViaShaderWrapper();

        private Dictionary<string, PrimitiveShader> layerMappedTrackShader = new Dictionary<string, PrimitiveShader>();
        private Dictionary<string, PrimitiveShader> layerMappedPadShader = new Dictionary<string, PrimitiveShader>();

        public List<IPcbLayer> PcbLayers { get; private set; } = new List<IPcbLayer>();

        public List<string> EnabledPcbLayers { get; } = new List<string>();

        public bool DisableTracks { get; set; } = false;
        public bool DisablePads { get; set; } = false;
        public bool DisableVias { get; set; } = false;
        public bool DisableComponentOutlines { get; set; } = false;
        public bool DisableCommentAreas { get; set; } = false;

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

        public void AddTestPrimitive()
        {
            var track = new Track(
                null!,
                new PointF(-1.0F, -1.0F),
                new PointF(1.0F, 1.0F),
                0.1F);

            if (!layerMappedTrackShader.ContainsKey("Test"))
            {
                layerMappedTrackShader.Add("Test", new PrimitiveShader(0.0F));
            }

            layerMappedTrackShader["Test"].AddPrimitive(
                track,
                Color.Red,
                0.0F);

            layerMappedTrackShader.Values.ToList().ForEach(x => x.Initialise());
        }

        public void AddTrack(
            IPcbLayer layer,
            float beginX,
            float beginY,
            float endX,
            float endY,
            float width)
        {
            var track = new Track(
                layer,
                new PointF(beginX, beginY),
                new PointF(endX, endY),
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
            float sizeX,
            float sizeY,
            float positionX,
            float positionY,
            decimal rotation,
            float holeSize)
        {
            var pad = new Pad(
                layer,
                primitiveShape,
                padType,
                new PointF(sizeX, sizeY),
                new PointF(positionX, positionY),
                rotation,
                holeSize);

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
            float positionX,
            float positionY,
            float padDiameter,
            float holeDiameter)
        {
            var via = new Via(
                layer,
                primitiveShape,
                new PointF(positionX, positionY),
                padDiameter,
                holeDiameter);

            var layerInfo = GetLayerInfo(layer);
            viaShader.AddPrimitive(layer, via, layerInfo.ZOffset);
        }

        public void AddBoardOutline(
            float beginX,
            float beginY,
            float endX,
            float endY)
        {
            var line = new ThickLine(
                null,
                new PointF(beginX, beginY),
                new PointF(endX, endY));

            boardOutlineShader.AddPrimitive(line, Color4.Purple, 0.0f);
        }

        public void AddComponentOutline(
            float beginX,
            float beginY,
            float endX,
            float endY)
        {
            var line = new ThickLine(
                null,
                new PointF(beginX, beginY),
                new PointF(endX, endY));

            componentOutlineShader.AddPrimitive(line, new Color4(1.0f, 0.65f, 0.0f, 1.0f), 0.0f);
        }

        float segmentSize = 0.1f;
        float segmentSolid = 0.035f;

        public void AddCommentArea(
            float beginX,
            float beginY,
            float endX,
            float endY)
        {
            Color4 boxColor = new Color4(53, 113, 209, 255);

            float argA, argB, argC, argD;

            // We will assume a bounding rectangle with side on the X and Y axis
            if (beginX != endX)
            {
                float start = beginX;
                float stop = endX;

                if (beginX > endX)
                {
                    start = endX;
                    stop = beginX;
                }

                // Size is in X direction
                for (float segmentPoint = start; segmentPoint < stop; segmentPoint += segmentSize)
                {
                    CreateLine(segmentPoint, beginY, Math.Min(segmentPoint + segmentSolid, stop), endY, boxColor);
                }
            }
            else
            {
                float start = beginY;
                float stop = endY;

                if (beginY > endY)
                {
                    start = endY;
                    stop = beginY;
                }

                // Side is in Y direction
                for (float segmentPoint = start; segmentPoint < stop; segmentPoint += segmentSize)
                {
                    CreateLine(beginX, segmentPoint, endX, Math.Min(segmentPoint + segmentSolid, stop), boxColor);
                }
            }
        }

        private void CreateLine(float posX1, float posY1, float posX2, float posY2, Color4 lineColor)
        {
            var line = new ThickLine(
                null,
                new PointF(posX1, posY1),
                new PointF(posX2, posY2));

            commentAreaShader.AddPrimitive(line, lineColor, 0.01f);
        }

        public void Reset()
        {
            boardOutlineShader.Reset();
            boardOutlineShader.Dispose();

            componentOutlineShader.Reset();
            componentOutlineShader.Dispose();

            commentAreaShader.Reset();
            commentAreaShader.Dispose();

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
            boardOutlineShader.Initialise();
            layerMappedTrackShader.Values.ToList().ForEach(x => x.Initialise());
            layerMappedPadShader.Values.ToList().ForEach(x => x.Initialise());
            viaShader.Initialise();
        }

        public void FinaliseAdditionalDataSetup()
        {
            componentOutlineShader.Initialise();
        }

        public void FinaliseCommentAreaSetup()
        {
            commentAreaShader.Initialise();
        }

        public void Draw(Matrix4 view, Matrix4 projection)
        {
            boardOutlineShader.Draw(view, projection);
            
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

            if (!DisableComponentOutlines)
            {
                componentOutlineShader.Draw(view, projection);
            }

            if (!DisableCommentAreas)
            {
                commentAreaShader.Draw(view, projection);
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
            boardOutlineShader.Dispose();
            layerMappedTrackShader.Values.ToList().ForEach(x => x.Dispose());
            layerMappedPadShader.Values.ToList().ForEach(x => x.Dispose());
            viaShader.Dispose();
            componentOutlineShader.Dispose();
            commentAreaShader.Dispose();
        }
    }
}
