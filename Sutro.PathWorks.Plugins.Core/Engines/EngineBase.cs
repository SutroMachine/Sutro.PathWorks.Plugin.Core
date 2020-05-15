using Sutro.Core.Models.Profiles;
using Sutro.PathWorks.Plugins.API;
using Sutro.PathWorks.Plugins.API.Engines;
using Sutro.PathWorks.Plugins.API.Settings;
using Sutro.PathWorks.Plugins.API.Visualizers;
using System.Collections.Generic;

namespace Sutro.PathWorks.Plugins.Core.Engines
{
    public abstract class EngineBase<TSettings> : IEngine<TSettings> where TSettings : IProfile
    {
        public abstract IGenerator<TSettings> Generator { get; }
        public abstract ISettingsManager<TSettings> SettingsManager { get; }
        public abstract List<IVisualizer> Visualizers { get; }

        IGenerator IEngine.Generator => Generator;
        ISettingsManager IEngine.SettingsManager => SettingsManager;
    }
}