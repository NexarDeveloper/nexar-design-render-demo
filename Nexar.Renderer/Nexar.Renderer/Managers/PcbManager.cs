using StrawberryShake;
using Nexar.Client.Login;
using Nexar.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Nexar.Renderer.DesignEntities;
using Nexar.Renderer.Api;
using Nexar.Renderer.Visualization;
using Nexar.Renderer.Geometry;

using IPcbLayer = Nexar.Client.IGetPcbModel_DesProjectById_Design_WorkInProgress_Variants_Pcb_LayerStack_Stacks_Layers;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using System.Reflection.Emit;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Reflection.Metadata;
using OpenTk.Tutorial.Tools;
using Newtonsoft.Json.Linq;
using System.Runtime.Intrinsics.Arm;

namespace Nexar.Renderer.Managers
{
    public class PcbManager
    {
        public GlRenderer PcbRenderer { get; }

        private Stopwatch GeneralStopwatch { get; }

        private NexarHelper NexarHelper { get; }

        private PcbStats PcbStats { get; set; } = default!;

        public IOperationResult<IGetPcbModelResult> PcbModel { get; private set; } = default!;

        static bool DisableDrawTracks = false;
        static bool DisableDrawPads = false;
        static bool DisableDrawVias = false;

        private float divisor = 10.0F;
        private float xOffset = 0.0F;
        private float yOffset = 0.0F;

        private NexarClient nexarClient = default!;

        public Project ActiveProject { get; set; } = default!;

        public string DocumentId { get; set; } = string.Empty;

        public string DocumentName { get; set; } = string.Empty;

        public PcbManager(GlRenderer renderer)
        {
            PcbRenderer = renderer;
            NexarHelper = new NexarHelper();
            GeneralStopwatch = new Stopwatch();
        }

        public void Open3DComponentDemo()
        {
            PcbStats = new PcbStats();

            //PcbRenderer.Pcb.Reset();

            //PcbRenderer.Pcb.AddTestPrimitive();
            //PcbRenderer.Pcb.EnabledPcbLayers.Add("Test");
            PcbRenderer.Pcb.AddTestComponent2();
            PcbRenderer.Pcb.FinaliseSetup();

        }

        public async Task OpenPcbDesignAsync(string apiUrl, Project project)
        {
            ActiveProject = project;

            PcbRenderer.Pcb.Reset();

            PcbStats = new PcbStats();

            GeneralStopwatch.Restart();

            await NexarHelper.LoginAsync();
            nexarClient = NexarHelper.GetNexarClient(apiUrl);

            PcbModel = await nexarClient.GetPcbModel.ExecuteAsync(project.Id);
            PcbModel.EnsureNoErrors();

            var pcb = PcbModel?.Data?.DesProjectById?.Design?.WorkInProgress?.Variants.FirstOrDefault()?.Pcb;
            if (pcb is not null)
            {
                DocumentId = pcb.DocumentId ?? string.Empty;
                DocumentName = pcb.DocumentName ?? string.Empty;
            }

            GeneralStopwatch.Stop();
            PcbStats.TimeToLoadPcbFromNexar = GeneralStopwatch.ElapsedMilliseconds;

            LoadLayerStack();
            LoadBoardOutline();
            LoadNetsAndAssociatedPrimitives();
            LoadNoNetPads();

            //PcbRenderer.Pcb.AddTestComponent();

            PcbRenderer.Pcb.FinaliseSetup();

            Debug.WriteLine(PcbStats.ToString());
            Debug.WriteLine(PcbStats.NetToTrackDetail());
            Debug.WriteLine(PcbRenderer.Pcb.GetStats());
        }

        public async Task LoadAdditionalDesignDataAsync()
        {
            await LoadDesignItemsAsync();
            PcbRenderer.Pcb.FinaliseAdditionalDataSetup();
        }

        public Tuple<Point, Point> GetHighlightArea()
        {
            int scaledStartX = ScaleValueFromMmToNativeUnits(ScalePositionGlToMm((decimal)PcbRenderer.HighlightBox.XyStart.X, xOffset, divisor));
            int scaledStartY = ScaleValueFromMmToNativeUnits(ScalePositionGlToMm((decimal)PcbRenderer.HighlightBox.XyStart.Y, yOffset, divisor));
            int scaledEndX = ScaleValueFromMmToNativeUnits(ScalePositionGlToMm((decimal)PcbRenderer.HighlightBox.XyEnd.X, xOffset, divisor));
            int scaledEndY = ScaleValueFromMmToNativeUnits(ScalePositionGlToMm((decimal)PcbRenderer.HighlightBox.XyEnd.Y, yOffset, divisor));          

            return new Tuple<Point, Point>(
                new Point(
                    Math.Min(scaledStartX, scaledEndX),
                    Math.Max(scaledStartY, scaledEndY)),
                new Point(
                    Math.Max(scaledStartX, scaledEndX),
                    Math.Min(scaledStartY, scaledEndY)));
        }

        public float GetHighlightedAreaMm()
        {
            if (PcbRenderer.HighlightBox.BoxComplete)
            {
                float xSizeMm = ScaleValueGlToMm((decimal)(
                    Math.Abs(
                        Math.Abs(PcbRenderer.HighlightBox.XyStart.X) -
                        Math.Abs(PcbRenderer.HighlightBox.XyEnd.X))), xOffset, divisor);
                
                float ySizeMm = ScaleValueGlToMm((decimal)(
                    Math.Abs(
                        Math.Abs(PcbRenderer.HighlightBox.XyStart.Y) -
                        Math.Abs(PcbRenderer.HighlightBox.XyEnd.Y))), xOffset, divisor);

                return (xSizeMm * ySizeMm);
            }

            return 0.0f;
        }

        private void LoadLayerStack()
        {
            var layers = PcbModel?.Data?.DesProjectById?.Design?.WorkInProgress?.Variants.FirstOrDefault()?.Pcb?.LayerStack?.Stacks.FirstOrDefault()?.Layers;

            if (layers != null)
            {
                var pcbLayers = new List<IPcbLayer>();

                foreach (var layer in layers)
                {
                    if ((layer.LayerType == DesLayerType.Signal) ||
                        (layer.LayerType == DesLayerType.Plane))
                    {
                        pcbLayers.Add(layer);
                    }
                }

                PcbRenderer.Pcb.InitialiseLayerStack(pcbLayers);
            }
        }

        private void LoadBoardOutline()
        {
            var vertices = PcbModel?.Data?.DesProjectById?.Design?.WorkInProgress?.Variants.FirstOrDefault()?.Pcb?.Outline?.Vertices;

            if ((vertices != null) && (vertices.Count > 0))
            {
                float minX = (float)vertices.Min(x => x.XMm);
                float minY = (float)vertices.Min(x => x.YMm);
                float maxX = (float)vertices.Max(x => x.XMm);
                float maxY = (float)vertices.Max(x => x.YMm);

                xOffset = ((maxX - minX) / 2) + minX;
                yOffset = ((maxY - minY) / 2) + minY;

                IGetPcbModel_DesProjectById_Design_WorkInProgress_Variants_Pcb_Outline_Vertices? firstVertice = null;
                IGetPcbModel_DesProjectById_Design_WorkInProgress_Variants_Pcb_Outline_Vertices? lastVertice = null;

                foreach (var vertice in vertices)
                {
                    if (firstVertice == null)
                    {
                        firstVertice = vertice;
                    }

                    if (lastVertice != null)
                    {
                        PcbRenderer.Pcb.AddBoardOutline(
                            ScalePositionMmToGl(lastVertice.XMm, xOffset, divisor),
                            ScalePositionMmToGl(lastVertice.YMm, yOffset, divisor),
                            ScalePositionMmToGl(vertice.XMm, xOffset, divisor),
                            ScalePositionMmToGl(vertice.YMm, yOffset, divisor));
                    }

                    lastVertice = vertice;
                }

                if (lastVertice != null && firstVertice != null)
                {
                    PcbRenderer.Pcb.AddBoardOutline(
                        ScalePositionMmToGl(lastVertice.XMm, xOffset, divisor),
                        ScalePositionMmToGl(lastVertice.YMm, yOffset, divisor),
                        ScalePositionMmToGl(firstVertice.XMm, xOffset, divisor),
                        ScalePositionMmToGl(firstVertice.YMm, yOffset, divisor));
                }
            }
        }

        private List<DesignItem> AllComponents { get; } = new List<DesignItem>();

        public PointF ConvertGlCoordToMm(PointF glValue)
        {
            float scaledX = ScalePositionGlToMm((decimal)glValue.X, xOffset, divisor);
            float scaledY = ScalePositionGlToMm((decimal)glValue.Y, yOffset, divisor);

            return new PointF(scaledX, scaledY);
        }

        public DesignItem? GetComponentForLocation(PointF location)
        {
            return AllComponents.FirstOrDefault(x => x.HitTest(location));
        }

        private async Task LoadDesignItemsAsync()
        {
            GeneralStopwatch.Restart();

            AllComponents.Clear();

            Stopwatch componentStopwatch = new Stopwatch();

            string? cursor = null;
            bool hasPage = true;
            while (hasPage)
            {
                var itemResult = await nexarClient.GetDesignItems.ExecuteAsync(ActiveProject.Id, cursor, 100);
                itemResult.EnsureNoErrors();
                    
                var designItems = itemResult.Data?.DesProjectById?.Design?.WorkInProgress?.Variants.FirstOrDefault()?.Pcb?.DesignItems;

                if (designItems?.Nodes is null)
                    break;

                componentStopwatch.Start();

                foreach (var designItem in designItems.Nodes)
                {
                    if (designItem.Area != null)
                    {
                        PcbStats.TotalDesignItems++;

                        var component = new DesignItem(
                                designItem.Id,
                                designItem.Designator,
                                "", //designItem.Comment,
                                new Tuple<Point, Point>(
                                    new Point(designItem.Area.Pos1.X, designItem.Area.Pos1.Y),
                                    new Point(designItem.Area.Pos2.X, designItem.Area.Pos2.Y)),
                                (float)designItem.Area.Pos1.XMm,
                                (float)designItem.Area.Pos1.YMm,
                                (float)designItem.Area.Pos2.XMm,
                                (float)designItem.Area.Pos2.YMm);

                        AllComponents.Add(component);

                        PointF? firstVertice = null;
                        PointF? lastVertice = null;

                        foreach (var vertice in component.PolygonVertices)
                        {
                            if (firstVertice == null)
                            {
                                firstVertice = vertice;
                            }

                            if (lastVertice != null)
                            {
                                PcbRenderer.Pcb.AddComponentOutline(
                                    ScalePositionMmToGl((decimal)lastVertice.Value.X, xOffset, divisor),
                                    ScalePositionMmToGl((decimal)lastVertice.Value.Y, yOffset, divisor),
                                    ScalePositionMmToGl((decimal)vertice.X, xOffset, divisor),
                                    ScalePositionMmToGl((decimal)vertice.Y, yOffset, divisor));
                            }

                            lastVertice = vertice;
                        }

                        if (lastVertice != null && firstVertice != null)
                        {
                            PcbRenderer.Pcb.AddComponentOutline(
                                ScalePositionMmToGl((decimal)lastVertice.Value.X, xOffset, divisor),
                                ScalePositionMmToGl((decimal)lastVertice.Value.Y, yOffset, divisor),
                                ScalePositionMmToGl((decimal)firstVertice.Value.X, xOffset, divisor),
                                ScalePositionMmToGl((decimal)firstVertice.Value.Y, yOffset, divisor));
                        }
                    }

                    if (!string.IsNullOrEmpty(designItem.Mesh3D?.GlbFile?.DownloadUrl))
                    {
                        await PcbRenderer.Pcb.Add3DComponentBodyAsync(
                            xOffset, 
                            yOffset, 
                            designItem.Mesh3D.GlbFile.DownloadUrl);
                    }
                }

                componentStopwatch.Stop();

                cursor = designItems.PageInfo.EndCursor;
                hasPage = designItems.PageInfo.HasNextPage;
            }
            
            GeneralStopwatch.Stop();

            PcbStats.TimeToCreateComponents = componentStopwatch.ElapsedMilliseconds;
        }

        public void LoadCommentAreas(List<CommentThread> commentThreads, CommentThread? activeThread = null)
        {
            PcbRenderer.Pcb.ResetComments();

            foreach (var commentThread in commentThreads)
            {
                PointF? firstVertice = null;
                PointF? lastVertice = null;

                foreach (var vertice in commentThread.PolygonVertices)
                {
                    if (firstVertice == null)
                    {
                        firstVertice = vertice;
                    }

                    if (lastVertice != null)
                    {
                        PcbRenderer.Pcb.AddCommentArea(
                            ScalePositionMmToGl((decimal)lastVertice.Value.X, xOffset, divisor),
                            ScalePositionMmToGl((decimal)lastVertice.Value.Y, yOffset, divisor),
                            ScalePositionMmToGl((decimal)vertice.X, xOffset, divisor),
                            ScalePositionMmToGl((decimal)vertice.Y, yOffset, divisor),
                            commentThread.CommentThreadId == activeThread?.CommentThreadId);
                    }

                    lastVertice = vertice;
                }

                if (lastVertice != null && firstVertice != null)
                {
                    PcbRenderer.Pcb.AddCommentArea(
                        ScalePositionMmToGl((decimal)lastVertice.Value.X, xOffset, divisor),
                        ScalePositionMmToGl((decimal)lastVertice.Value.Y, yOffset, divisor),
                        ScalePositionMmToGl((decimal)firstVertice.Value.X, xOffset, divisor),
                        ScalePositionMmToGl((decimal)firstVertice.Value.Y, yOffset, divisor),
                        commentThread.CommentThreadId == activeThread?.CommentThreadId);
                }
            }

            PcbRenderer.Pcb.FinaliseCommentAreaSetup();
        }

        private void LoadNetsAndAssociatedPrimitives()
        {
            GeneralStopwatch.Restart();

            Stopwatch trackStopwatch = new Stopwatch();
            Stopwatch padStopwatch = new Stopwatch();
            Stopwatch viaStopwatch = new Stopwatch();

            var nets = PcbModel?.Data?.DesProjectById?.Design?.WorkInProgress?.Variants[0].Pcb?.Nets;

            if (nets != null)
            {
                foreach (var net in nets)
                {
                    PcbStats.TotalNets++;

                    if (!DisableDrawTracks)
                    {
                        foreach (var track in net.Tracks)
                        {
                            if (track.Layer != null)
                            {
                                trackStopwatch.Start();

                                var layer = PcbRenderer.Pcb.PcbLayers.First(x => x.Name == track.Layer.Name);

                                PcbRenderer.Pcb.AddTrack(
                                    layer,
                                    ScalePositionMmToGl(track.Begin.XMm, xOffset, divisor),
                                    ScalePositionMmToGl(track.Begin.YMm, yOffset, divisor),
                                    ScalePositionMmToGl(track.End.XMm, xOffset, divisor),
                                    ScalePositionMmToGl(track.End.YMm, yOffset, divisor),
                                    ScalePositionMmToGl(track.Width.XMm, 0.0F, divisor));

                                trackStopwatch.Stop();
                                PcbStats.TotalTracks++;
                                PcbStats.IncrementCountForNet(net.Name);
                            }
                        }
                    }

                    if (!DisableDrawPads)
                    {
                        foreach (var pad in net.Pads)
                        {
                            if (pad.Layer != null)
                            {
                                padStopwatch.Start();

                                var layer = PcbRenderer.Pcb.PcbLayers.First(x => x.Name == pad.Layer.Name);
                                AddPad(pad, layer);
                                padStopwatch.Stop();
                                PcbStats.TotalPads++;
                            }
                            else
                            {
                                // Hack, if layer is null we will assume it is multi layer top and bottom (needs fix on API side)
                                padStopwatch.Start();

                                PcbRenderer.Pcb.PcbLayers.ToList().ForEach(x =>
                                {
                                    AddPad(pad, x);
                                    PcbStats.TotalPads++;
                                });

                                padStopwatch.Stop();
                            }
                        }
                    }

                    if (!DisableDrawVias)
                    {
                        foreach (var via in net.Vias)
                        {
                            if ((via.BeginLayer != null) && (via.EndLayer != null))
                            {
                                viaStopwatch.Start();

                                var beginLayer = PcbRenderer.Pcb.PcbLayers.First(x => x.Name == via.BeginLayer.Name);

                                PcbRenderer.Pcb.AddVia(
                                    beginLayer,
                                    via.Shape ?? DesPrimitiveShape.Round,
                                    ScalePositionMmToGl(via.Position.XMm, xOffset, divisor),
                                    ScalePositionMmToGl(via.Position.YMm, yOffset, divisor),
                                    ScalePositionMmToGl(via.PadDiameter.XMm, 0.0F, divisor),
                                    ScalePositionMmToGl(via.HoleDiameter.XMm, 0.0F, divisor));

                                var endLayer = PcbRenderer.Pcb.PcbLayers.First(x => x.Name == via.EndLayer.Name);

                                PcbRenderer.Pcb.AddVia(
                                    endLayer,
                                    via.Shape ?? DesPrimitiveShape.Round,
                                    ScalePositionMmToGl(via.Position.XMm, xOffset, divisor),
                                    ScalePositionMmToGl(via.Position.YMm, yOffset, divisor),
                                    ScalePositionMmToGl(via.PadDiameter.XMm, 0.0F, divisor),
                                    ScalePositionMmToGl(via.HoleDiameter.XMm, 0.0F, divisor));

                                viaStopwatch.Stop();
                                PcbStats.TotalVias++;
                            }
                        }
                    }
                }
            }

            GeneralStopwatch.Stop();
            PcbStats.TimeToCreateAllPrimitives = GeneralStopwatch.ElapsedMilliseconds;

            PcbStats.TimeToCreateTracks = trackStopwatch.ElapsedMilliseconds;
            PcbStats.TimeToCreatePads = padStopwatch.ElapsedMilliseconds;
            PcbStats.TimeToCreateVias = viaStopwatch.ElapsedMilliseconds;
        }

        private void LoadNoNetPads()
        {
            var noNetPads = PcbModel?.Data?.DesProjectById?.Design?.WorkInProgress?.Variants.FirstOrDefault()?.Pcb?.Pads.Where(x => x.Net?.Name == null);

            if (noNetPads != null)
            {
                foreach (var pad in noNetPads)
                {
                    if (pad.Layer != null)
                    {
                        var layer = PcbRenderer.Pcb.PcbLayers.First(x => x.Name == pad.Layer.Name);
                        AddPad(pad, layer);
                        PcbStats.TotalPads++;
                    }
                    else
                    {
                        // Hack, if layer is null we will assume it is multi layer top and bottom (needs fix on API side)
                        PcbRenderer.Pcb.PcbLayers.ToList().ForEach(x =>
                        {
                            AddPad(pad, x);
                            PcbStats.TotalPads++;
                        });
                    }
                }
            }
        }

        private void AddPad(IGetPcbModel_DesProjectById_Design_WorkInProgress_Variants_Pcb_Nets_Pads pad, IPcbLayer layer)
        {
            PcbRenderer.Pcb.AddPad(
                layer,
                pad.Shape ?? DesPrimitiveShape.Rectangle,
                pad.PadType,
                ScalePositionMmToGl(pad.Size.XMm, 0.0F, divisor),
                ScalePositionMmToGl(pad.Size.YMm, 0.0F, divisor),
                ScalePositionMmToGl(pad.Position.XMm, xOffset, divisor),
                ScalePositionMmToGl(pad.Position.YMm, yOffset, divisor),
                pad.Rotation ?? 0.0M,
                ScalePositionMmToGl(pad.HoleSize?.XMm ?? 0.0M, 0.0F, divisor));
        }

        private void AddPad(IGetPcbModel_DesProjectById_Design_WorkInProgress_Variants_Pcb_Pads pad, IPcbLayer layer)
        {
            PcbRenderer.Pcb.AddPad(
                layer,
                pad.Shape ?? DesPrimitiveShape.Rectangle,
                pad.PadType,
                ScalePositionMmToGl(pad.Size.XMm, 0.0F, divisor),
                ScalePositionMmToGl(pad.Size.YMm, 0.0F, divisor),
                ScalePositionMmToGl(pad.Position.XMm, xOffset, divisor),
                ScalePositionMmToGl(pad.Position.YMm, yOffset, divisor),
                pad.Rotation ?? 0.0M,
                ScalePositionMmToGl(pad.HoleSize?.XMm ?? 0.0M, 0.0F, divisor));
        }

        private float ScalePositionMmToGl(decimal value, float offset, float divisor)
        {
            return (((float)value) - offset) / divisor;
        }

        private float ScalePositionGlToMm(decimal value, float offset, float divisor)
        {
            return (((float)value) * divisor) + offset;
        }

        private float ScaleValueGlToMm(decimal value, float offset, float divisor)
        {
            return (((float)value) * divisor);
        }

        private int ScaleValueFromMmToNativeUnits(float mmValue)
        {
            return (int)Math.Round((mmValue / 0.00000254F), 3, MidpointRounding.AwayFromZero);
        }
    }
}
