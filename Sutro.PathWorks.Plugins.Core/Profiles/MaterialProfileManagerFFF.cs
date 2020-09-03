using Sutro.Core.Settings.Material;
using Sutro.PathWorks.Plugins.API.Settings;
using Sutro.PathWorks.Plugins.Core.UserSettings;
using System.Collections.Generic;
using System.Linq;

namespace Sutro.PathWorks.Plugins.Core.Settings
{
    public class MaterialProfileManagerFFF : MaterialProfileManagerBase<MaterialProfileFFF>
    {
        public override List<MaterialProfileFFF> FactoryProfiles => MaterialProfileFactoryFFF.EnumerateDefaults().ToList();

        public override UserSettingCollectionBase<MaterialProfileFFF> UserSettings => new MaterialUserSettingsFFF<MaterialProfileFFF>();
    }
}