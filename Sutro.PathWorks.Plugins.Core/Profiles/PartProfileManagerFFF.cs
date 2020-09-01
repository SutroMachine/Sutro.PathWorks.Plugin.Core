using Sutro.Core.Settings.Part;
using Sutro.PathWorks.Plugins.API.Settings;
using System.Collections.Generic;
using System.Linq;

namespace Sutro.PathWorks.Plugins.Core.Settings
{
    public class PartProfileManagerFFF : PartProfileManagerBase<PartProfileFFF>
    {
        public override List<PartProfileFFF> FactoryProfiles =>
            FactoryPrintProfiles.EnumerateFactoryProfiles().Select(p => p.Part).ToList();

        public override IUserSettingCollection<PartProfileFFF> UserSettings =>
            new PartUserSettingsFFF<PartProfileFFF>();
    }
}