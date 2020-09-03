using Sutro.Core.Settings.Part;
using Sutro.PathWorks.Plugins.API.Settings;
using Sutro.PathWorks.Plugins.Core.UserSettings;
using System.Collections.Generic;
using System.Linq;

namespace Sutro.PathWorks.Plugins.Core.Settings
{
    public class PartProfileManagerFFF : PartProfileManagerBase<PartProfileFFF>
    {
        public override List<PartProfileFFF> FactoryProfiles => PartProfileFactoryFFF.EnumerateDefaults().ToList();

        public override UserSettingCollectionBase<PartProfileFFF> UserSettings => new PartUserSettingsFFF<PartProfileFFF>();
    }
}