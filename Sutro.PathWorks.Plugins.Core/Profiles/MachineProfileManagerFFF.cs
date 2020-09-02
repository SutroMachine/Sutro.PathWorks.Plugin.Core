using Sutro.Core.Models.Profiles;
using Sutro.Core.Settings.Machine;
using Sutro.PathWorks.Plugins.API.Settings;
using Sutro.PathWorks.Plugins.Core.UserSettings;
using System.Collections.Generic;
using System.Linq;

namespace Sutro.PathWorks.Plugins.Core.Settings
{
    public class MachineProfileManagerFFF : MachineProfileManagerBase<MachineProfileFFF>
    {
        public override List<MachineProfileFFF> FactoryProfiles =>
            FactoryPrintProfiles.EnumerateFactoryProfiles().Select(p => p.Machine).ToList();

        public override UserSettingCollectionBase<MachineProfileFFF> UserSettings => new MachineUserSettingsFFF<MachineProfileFFF>();
    }
}