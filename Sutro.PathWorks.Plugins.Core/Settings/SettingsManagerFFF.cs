using Sutro.Core.Models.Profiles;
using Sutro.Core.Settings;
using Sutro.PathWorks.Plugins.API.Settings;

namespace Sutro.PathWorks.Plugins.Core.Settings
{
    public class SettingsManagerFFF : ISettingsManager
    {
        public IProfileManager<IMachineProfile> MachineProfileManager =>
            new MachineProfileManagerFFF();

        public IProfileManager<IMaterialProfile> MaterialProfileManager =>
            (IProfileManager<IMaterialProfile>)new MaterialProfileManagerFFF();

        public IProfileManager<IPartProfile> PartProfileManager =>
            (IProfileManager<IPartProfile>)new PartProfileManagerFFF();

        public IPrintProfile CreateSettingsInstance()
        {
            return new PrintProfileFFF();
        }
    }
}