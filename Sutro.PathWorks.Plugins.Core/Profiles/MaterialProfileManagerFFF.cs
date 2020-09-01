using Sutro.Core.Settings.Material;
using Sutro.PathWorks.Plugins.API.Settings;
using System.Collections.Generic;
using System.Linq;

namespace Sutro.PathWorks.Plugins.Core.Settings
{
    public class MaterialProfileManagerFFF : MaterialProfileManagerBase<MaterialProfileFFF>
    {
        public override List<MaterialProfileFFF> FactoryProfiles =>
            FactoryPrintProfiles.EnumerateFactoryProfiles().Select(p => p.Material).ToList();

        public override IUserSettingCollection<MaterialProfileFFF> UserSettings =>
            new MaterialUserSettingsFFF<MaterialProfileFFF>();
    }
}