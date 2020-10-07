using Sutro.Core.Models.Profiles;
using Sutro.PathWorks.Plugins.API;
using Sutro.PathWorks.Plugins.API.Engines;
using Sutro.PathWorks.Plugins.API.Settings;
using Sutro.PathWorks.Plugins.API.Visualizers;
using System.Collections.Generic;
using System.Reflection;

namespace Sutro.PathWorks.Plugins.Core.Engines
{
    public abstract class EngineBase<TSettings> : IEngine<TSettings> where TSettings : IPrintProfile
    {
        public abstract string Name { get; }
        public virtual string Version => Assembly.GetAssembly(GetType()).GetName().Version.ToString();
        public abstract string Description { get; }

        public abstract IGenerator<TSettings> Generator { get; }
        public abstract ISettingsManager SettingsManager { get; }
        public abstract List<IVisualizer> Visualizers { get; }
        public abstract List<string> FileExtensions { get; }

        IGenerator IEngine.Generator => Generator;
        ISettingsManager IEngine.SettingsManager => SettingsManager;
    }
}