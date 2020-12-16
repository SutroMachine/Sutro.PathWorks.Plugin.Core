using Sutro.PathWorks.Plugins.API.Generators;
using System.Collections.Generic;
using System.Linq;

namespace Sutro.PathWorks.Plugins.Core.Engines
{
    public class GCodeInfo : IGCodeInfo
    {
        public GCodeInfo(IReadOnlyCollection<string> materialUsageEstimate, IReadOnlyCollection<string> printTimeEstimate)
        {
            MaterialUsageEstimate = materialUsageEstimate.ToList();
            PrintTimeEstimate = printTimeEstimate.ToList();
        }

        public IReadOnlyList<string> MaterialUsageEstimate { get; }

        public IReadOnlyList<string> PrintTimeEstimate { get; }
    }
}