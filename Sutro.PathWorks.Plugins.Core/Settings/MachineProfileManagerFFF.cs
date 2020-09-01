using Sutro.Core.Settings.Machine;
using Sutro.PathWorks.Plugins.API.Settings;
using System.Collections.Generic;
using System.Linq;

namespace Sutro.PathWorks.Plugins.Core.Settings
{
    public class MachineProfileManagerFFF : MachineProfileManagerBase<MachineProfileFFF>
    {
        public override List<MachineProfileFFF> FactoryProfiles =>
            FactoryPrintProfiles.EnumerateFactoryProfiles().Select(p => p.MachineProfile).ToList();

        public override IUserSettingCollection<MachineProfileFFF> UserSettings =>
            new MachineUserSettingsFFF<MachineProfileFFF>();
    }
}