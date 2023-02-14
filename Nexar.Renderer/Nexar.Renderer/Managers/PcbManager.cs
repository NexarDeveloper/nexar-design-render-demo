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

        public PcbManager(GlRenderer renderer)
        {
            PcbRenderer = renderer;
            NexarHelper = new NexarHelper();
            GeneralStopwatch = new Stopwatch();
        }

        public async Task OpenPcbDesignAsync(Project project)
        {            
            PcbRenderer.Pcb.Reset();

            PcbStats = new PcbStats();

            GeneralStopwatch.Restart();

            await NexarHelper.LoginAsync();
            var nexarClient = NexarHelper.GetNexarClient();

            PcbModel = await nexarClient.GetPcbModel.ExecuteAsync(project.Id);
            PcbModel.EnsureNoErrors();

            GeneralStopwatch.Stop();
            PcbStats.TimeToLoadPcbFromNexar = GeneralStopwatch.ElapsedMilliseconds;

            LoadLayerStack();
            LoadBoardOutline();
            LoadNetsAndAssociatedPrimitives();
            LoadNoNetPads();

            PcbRenderer.Pcb.FinaliseSetup();

            Debug.WriteLine(PcbStats.ToString());
            Debug.WriteLine(PcbStats.NetToTrackDetail());
            Debug.WriteLine(PcbRenderer.Pcb.GetStats());
        }

        private void LoadLayerStack()
        {
            var layers = PcbModel?.Data?.DesProjectById?.Design?.WorkInProgress?.Variants[0]?.Pcb?.LayerStack?.Stacks[0]?.Layers;

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
            var vertices = PcbModel?.Data?.DesProjectById?.Design?.WorkInProgress?.Variants[0].Pcb?.Outline?.Vertices;

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
                        PcbRenderer.Pcb.AddOutline(
                            ScaleValue(lastVertice.XMm, xOffset, divisor),
                            ScaleValue(lastVertice.YMm, yOffset, divisor),
                            ScaleValue(vertice.XMm, xOffset, divisor),
                            ScaleValue(vertice.YMm, yOffset, divisor));
                    }

                    lastVertice = vertice;
                }

                if (lastVertice != null && firstVertice != null)
                {
                    PcbRenderer.Pcb.AddOutline(
                        ScaleValue(lastVertice.XMm, xOffset, divisor),
                        ScaleValue(lastVertice.YMm, yOffset, divisor),
                        ScaleValue(firstVertice.XMm, xOffset, divisor),
                        ScaleValue(firstVertice.YMm, yOffset, divisor));
                }
            }
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
                                    ScaleValue(track.Begin.XMm, xOffset, divisor),
                                    ScaleValue(track.Begin.YMm, yOffset, divisor),
                                    ScaleValue(track.End.XMm, xOffset, divisor),
                                    ScaleValue(track.End.YMm, yOffset, divisor),
                                    ScaleValue(track.Width.XMm, 0.0F, divisor));

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
                                    ScaleValue(via.Position.XMm, xOffset, divisor),
                                    ScaleValue(via.Position.YMm, yOffset, divisor),
                                    ScaleValue(via.PadDiameter.XMm, 0.0F, divisor),
                                    ScaleValue(via.HoleDiameter.XMm, 0.0F, divisor));

                                var endLayer = PcbRenderer.Pcb.PcbLayers.First(x => x.Name == via.EndLayer.Name);

                                PcbRenderer.Pcb.AddVia(
                                    endLayer,
                                    via.Shape ?? DesPrimitiveShape.Round,
                                    ScaleValue(via.Position.XMm, xOffset, divisor),
                                    ScaleValue(via.Position.YMm, yOffset, divisor),
                                    ScaleValue(via.PadDiameter.XMm, 0.0F, divisor),
                                    ScaleValue(via.HoleDiameter.XMm, 0.0F, divisor));

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
            var noNetPads = PcbModel?.Data?.DesProjectById?.Design?.WorkInProgress?.Variants[0].Pcb?.Pads.Where(x => x.Net?.Name == null);

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
                ScaleValue(pad.Size.XMm, 0.0F, divisor),
                ScaleValue(pad.Size.YMm, 0.0F, divisor),
                ScaleValue(pad.Position.XMm, xOffset, divisor),
                ScaleValue(pad.Position.YMm, yOffset, divisor),
                pad.Rotation ?? 0.0M,
                ScaleValue(pad.HoleSize?.XMm ?? 0.0M, 0.0F, divisor));
        }

        private void AddPad(IGetPcbModel_DesProjectById_Design_WorkInProgress_Variants_Pcb_Pads pad, IPcbLayer layer)
        {
            PcbRenderer.Pcb.AddPad(
                layer,
                pad.Shape ?? DesPrimitiveShape.Rectangle,
                pad.PadType,
                ScaleValue(pad.Size.XMm, 0.0F, divisor),
                ScaleValue(pad.Size.YMm, 0.0F, divisor),
                ScaleValue(pad.Position.XMm, xOffset, divisor),
                ScaleValue(pad.Position.YMm, yOffset, divisor),
                pad.Rotation ?? 0.0M,
                ScaleValue(pad.HoleSize?.XMm ?? 0.0M, 0.0F, divisor));
        }

        private float ScaleValue(decimal value, float offset, float divisor)
        {
            return (((float)value) - offset) / divisor;
        }
    }
}
