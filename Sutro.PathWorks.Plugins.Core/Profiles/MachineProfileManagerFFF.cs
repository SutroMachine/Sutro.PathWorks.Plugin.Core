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
        public MachineProfileManagerFFF()
        {
            FactoryProfiles = new List<MachineProfileFFF>();
            FactoryProfiles.AddRange(MachineProfilesFactoryFFF.RepRap.EnumerateDefaults());
            FactoryProfiles.AddRange(MachineProfilesFactoryFFF.Makerbot.EnumerateDefaults());
            FactoryProfiles.AddRange(MachineProfilesFactoryFFF.Prusa.EnumerateDefaults());
            FactoryProfiles.AddRange(MachineProfilesFactoryFFF.Monoprice.EnumerateDefaults());
            FactoryProfiles.AddRange(MachineProfilesFactoryFFF.Flashforge.EnumerateDefaults());
            FactoryProfiles.AddRange(MachineProfilesFactoryFFF.Printrbot.EnumerateDefaults());
        }


        public override List<MachineProfileFFF> FactoryProfiles { get; }

        public override UserSettingCollectionBase<MachineProfileFFF> UserSettings => new MachineUserSettingsFFF<MachineProfileFFF>();
    }
}