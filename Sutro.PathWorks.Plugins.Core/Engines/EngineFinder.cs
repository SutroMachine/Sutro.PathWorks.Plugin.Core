using Sutro.PathWorks.Plugins.API.Engines;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;

namespace Sutro.PathWorks.Plugins.Core.Engines
{
    public class EngineFinder
    {
#pragma warning disable CS0649

        [ImportMany(typeof(IEngine))]
        private readonly IEnumerable<Lazy<IEngine, IEngineData>> engines;

#pragma warning restore CS0649

        private readonly CompositionContainer container;

        public Dictionary<string, Lazy<IEngine, IEngineData>> Engines { get; private set; }

        public EngineFinder(string path, Action<string> logger = null)
        {
            // An aggregate catalog that combines multiple catalogs
            var catalog = CreateCatalog(path, logger);

            // Create the CompositionContainer with the parts in the catalog
            if (!ConstructContainer(catalog, logger, out container))
                return;

            Engines = CreateEngineDictionary(container, logger);
        }

        private Dictionary<string, Lazy<IEngine, IEngineData>> CreateEngineDictionary(CompositionContainer container, Action<string> logger)
        {
            var result = new Dictionary<string, Lazy<IEngine, IEngineData>>();
            if (engines == null)
            {
                logger?.Invoke("No engines found");
            }
            else
            {
                foreach (var e in engines)
                {
                    if (!result.ContainsKey(e.Metadata.Name))
                    {
                        result.Add(e.Metadata.Name.ToLower(), e);
                        logger?.Invoke("Found engine: " + e.Metadata.Name);
                    }
                }
            }
            return result;
        }

        private bool ConstructContainer(AggregateCatalog catalog, Action<string> logger, out CompositionContainer container)
        {
            container = new CompositionContainer(catalog);

            // Fill the imports of this object
            try
            {
                container.ComposeParts(this);
            }
            catch (ReflectionTypeLoadException e)
            {
                logger?.Invoke("Composition loader exceptions:");
                foreach (var a in e.LoaderExceptions)
                {
                    logger?.Invoke(a.ToString());
                    return false;
                }
            }
            catch (CompositionException e)
            {
                logger?.Invoke(e.ToString());
                return false;
            }
            return true;
        }

        private static AggregateCatalog CreateCatalog(string path, Action<string> logger)
        {
            var catalog = new AggregateCatalog();

            // This iteration is required because of the bundling done by Fody.Costura
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                catalog.Catalogs.Add(new AssemblyCatalog(asm));
            }

            // Confirm file path exists
            if (!Directory.Exists(path))
            {
                logger?.Invoke("Invalid path passed to EngineFinder constructor");
                logger?.Invoke(Path.GetFullPath(path));
                return null;
            }

            // Add catalogs from file path
            catalog.Catalogs.Add(new DirectoryCatalog(path));
            foreach (var p in Directory.EnumerateDirectories(path, "*", SearchOption.AllDirectories))
            {
                catalog.Catalogs.Add(new DirectoryCatalog(p));
            }
            return catalog;
        }
    }
}