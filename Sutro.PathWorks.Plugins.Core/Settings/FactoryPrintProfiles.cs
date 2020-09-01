using Sutro.Core.Settings;
using Sutro.Core.Settings.Info;
using System.Collections.Generic;

namespace Sutro.PathWorks.Plugins.Core.Settings
{
    public static class FactoryPrintProfiles
    {
        public static IEnumerable<PrintProfileFFF> EnumerateFactoryProfiles()
        {
            var factory_profiles = new List<PrintProfileFFF>();

            factory_profiles.AddRange(RepRapSettings.EnumerateDefaults());
            factory_profiles.AddRange(FlashforgeSettings.EnumerateDefaults());
            factory_profiles.AddRange(PrusaSettings.EnumerateDefaults());
            factory_profiles.AddRange(MakerbotSettings.EnumerateDefaults());
            factory_profiles.AddRange(MonopriceSettings.EnumerateDefaults());
            factory_profiles.AddRange(PrintrbotSettings.EnumerateDefaults());

            return factory_profiles;
        }
    }
}