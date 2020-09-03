using Sutro.Core.Settings.Material;
using Sutro.PathWorks.Plugins.Core.UserSettings;
using System.Collections.Generic;
using System.Linq;

namespace Sutro.PathWorks.Plugins.Core.Settings
{
    public class MaterialProfileManagerFFF : MaterialProfileManagerBase<MaterialProfileFFF>
    {
        public override List<MaterialProfileFFF> FactoryProfiles { get; } = MaterialProfileFactoryFFF.EnumerateDefaults().ToList();

        public override UserSettingCollectionBase<MaterialProfileFFF> UserSettings { get; } = new MaterialUserSettingsFFF<MaterialProfileFFF>();
    }
}