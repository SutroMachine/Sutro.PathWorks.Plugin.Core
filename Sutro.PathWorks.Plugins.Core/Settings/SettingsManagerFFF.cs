using Sutro.Core.Models.Profiles;
using Sutro.Core.Settings;
using Sutro.PathWorks.Plugins.API.Settings;

namespace Sutro.PathWorks.Plugins.Core.Settings
{
    public class SettingsManagerFFF : ISettingsManager
    {
        public IMachineProfileManager MachineProfileManager => new MachineProfileManagerFFF();

        public IMaterialProfileManager MaterialProfileManager => new MaterialProfileManagerFFF();

        public IPartProfileManager PartProfileManager => new PartProfileManagerFFF();

        public IPrintProfile CreateSettingsInstance()
        {
            return new PrintProfileFFF();
        }
    }
}