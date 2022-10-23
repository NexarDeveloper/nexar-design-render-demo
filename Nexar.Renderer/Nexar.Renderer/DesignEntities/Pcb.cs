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

namespace Nexar.Renderer.DesignEntities
{
    public class Pcb
    {
        private List<SingleLineShader> boardOutlineShaders = new List<SingleLineShader>();
        private PrimitiveShader trackShader = new PrimitiveShader();
        private PrimitiveShader padShader = new PrimitiveShader();
        private ViaShaderWrapper viaShader = new ViaShaderWrapper();

        public bool DisableTracks { get; set; } = false;
        public bool DisablePads { get; set; } = false;
        public bool DisableVias { get; set; } = false;

        public string GetStats()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Geometry data");
            sb.AppendLine(string.Format("Track Shader Triangle Count:    {0}", CountTriangles(trackShader)));
            sb.AppendLine(string.Format("Pad Shader Triangle Count:      {0}", CountTriangles(padShader)));
            sb.AppendLine(string.Format("Via Shader Triangle Count:      {0}",
                CountTriangles(viaShader.TopLayerPrimitiveShader) +
                CountTriangles(viaShader.BottomLayerPrimitiveShader)));

            return sb.ToString();
        }

        private long CountTriangles<T>(T shader) where T : PrimitiveShader
        {
            long triangleCount = 0;
            shader.AssociatedPrimitives.ForEach(x => triangleCount += x.TessellatedTriangles.Count);
            return triangleCount;
        }

        public void AddTrack(
            string layer,
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

            trackShader.AddPrimitive(track);
        }

        public void AddPad(
            string layer,
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
            string layer,
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

            viaShader.AddPrimitive(via);
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

            trackShader.Reset();
            padShader.Reset();
            viaShader.Reset();
        }

        public void FinaliseSetup()
        {
            boardOutlineShaders.ForEach(x => x.Initialise());
            trackShader.Initialise();
            padShader.Initialise();
            viaShader.Initialise();
        }

        public void Draw(Matrix4 view, Matrix4 projection)
        {
            boardOutlineShaders.ForEach(x => x.Draw(view, projection));

            if (!DisableTracks)
            {
                trackShader.Draw(view, projection);
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
            trackShader.Dispose();
            padShader.Dispose();
            viaShader.Dispose();
        }
    }
}
