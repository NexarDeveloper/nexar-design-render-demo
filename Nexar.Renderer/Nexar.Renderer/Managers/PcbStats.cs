using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Nexar.Renderer.Managers
{
    public class PcbStats
    {
        public long TimeToLoadPcbFromNexar { get; set; } = 0;
        public long TimeToCreateAllPrimitives { get; set; } = 0;
        public long TimeToCreateTracks { get; set; } = 0;
        public long TimeToCreatePads { get; set; } = 0;
        public long TimeToCreateVias { get; set; } = 0;
        public int TotalNets { get; set; } = 0;
        public int TotalTracks { get; set; } = 0;
        public int TotalPads { get; set; } = 0;
        public int TotalVias { get; set; } = 0;
        public Dictionary<string, int> TrackToNetCount { get; } = new Dictionary<string, int>();

        public void IncrementCountForNet(string net)
        {
            if (!TrackToNetCount.ContainsKey(net))
            {
                TrackToNetCount.Add(net, 0);
            }

            TrackToNetCount[net]++;
        }

        public override string ToString()
        {
            return string.Format(
                "Pcb Load Time: {0}" + Environment.NewLine +
                "Time To Create All Primitives: {1}" + Environment.NewLine +
                "Time To Create Tracks: {2}" + Environment.NewLine +
                "Time To Create Pads: {3}" + Environment.NewLine +
                "Time To Create Vias: {4}" + Environment.NewLine +
                "Total Nets: {5}" + Environment.NewLine +
                "Total Tracks: {6}" + Environment.NewLine +
                "Total Pads: {7}" + Environment.NewLine +
                "Total Vias: {8}" + Environment.NewLine,
                FormattedTime(TimeToLoadPcbFromNexar),
                FormattedTime(TimeToCreateAllPrimitives),
                FormattedTime(TimeToCreateTracks),
                FormattedTime(TimeToCreatePads),
                FormattedTime(TimeToCreateVias),
                TotalNets,
                TotalTracks,
                TotalPads,
                TotalVias);
        }

        public string NetToTrackDetail()
        {
            var sb = new StringBuilder();

            foreach (var element in TrackToNetCount)
            {
                sb.AppendLine(string.Format(
                    "{0}: {1}",
                    element.Key.PadRight(20, ' '),
                    element.Value));
            }

            return sb.ToString();
        }

        private string FormattedTime(long timeMs)
        {
            TimeSpan ts = new TimeSpan(timeMs * 10000);
            return ts.ToString("c");
        }
    }
}
