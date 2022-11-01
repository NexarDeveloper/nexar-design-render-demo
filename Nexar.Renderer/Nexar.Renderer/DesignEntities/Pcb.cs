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

namespace Nexar.Renderer.DesignEntities
{
    public class Pcb
    {
        private List<SingleLineShader> boardOutlineShaders = new List<SingleLineShader>();
        //private PrimitiveShader trackShader = new PrimitiveShader();
        private PrimitiveShader padShader = new PrimitiveShader(0.0F);
        private ViaShaderWrapper viaShader = new ViaShaderWrapper();

        private Dictionary<string, PrimitiveShader> layerMappedTrackShader = new Dictionary<string, PrimitiveShader>();

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

            long viaShaderTriangleCount = 0;
            viaShader.ViaLayerShaderMapping.Values.ToList().ForEach(x => viaShaderTriangleCount += CountTriangles(x));

            var sb = new StringBuilder();
            sb.AppendLine("Geometry data");
            sb.AppendLine(string.Format("Track Shader Triangle Count:    {0}", trackShaderTriangleCount));
            sb.AppendLine(string.Format("Pad Shader Triangle Count:      {0}", CountTriangles(padShader)));
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

            layerMappedTrackShader[layer.Name].AddPrimitive(track);

            //trackShader.AddPrimitive(track);
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

            padShader.AddPrimitive(pad);
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

            viaShader.AddPrimitive(layer, via);
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

            padShader.Reset();
            viaShader.Reset();
        }

        public void FinaliseSetup()
        {
            boardOutlineShaders.ForEach(x => x.Initialise());
            layerMappedTrackShader.Values.ToList().ForEach(x => x.Initialise());
            padShader.Initialise();
            viaShader.Initialise();
        }

        public void Draw(Matrix4 view, Matrix4 projection)
        {
            boardOutlineShaders.ForEach(x => x.Draw(view, projection));

            if (!DisableTracks)
            {
                foreach (var mappedLayer in layerMappedTrackShader)
                {
                    if (EnabledPcbLayers.Contains(mappedLayer.Key))
                    {
                        mappedLayer.Value.Draw(view, projection);
                    }
                }
            }

            if (!DisablePads)
            {
                padShader.Draw(view, projection);
            }

            if (!DisableVias)
            {
                viaShader.Draw(view, projection);
            }
        }

        public void Dispose()
        {
            boardOutlineShaders.ForEach(x => x.Dispose());
            layerMappedTrackShader.Values.ToList().ForEach(x => x.Dispose());
            padShader.Dispose();
            viaShader.Dispose();
        }
    }
}
