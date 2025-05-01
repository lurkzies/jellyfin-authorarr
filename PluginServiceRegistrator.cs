using MediaBrowser.Controller;
using MediaBrowser.Controller.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin.Plugin.Authorarr
{
    /// <summary>
    /// Registers your plugin’s services with Jellyfin’s DI container.
    /// </summary>
    public class PluginServiceRegistrator : IPluginServiceRegistrator
    {
        /// <summary>
        /// Registers the plugin's services.
        /// </summary>
        /// <param name="serviceCollection">The service collection to add to.</param>
        /// <param name="applicationHost">Provides host-level services.</param>
        public void RegisterServices(
            IServiceCollection serviceCollection,
            IServerApplicationHost applicationHost)
        {
            serviceCollection.AddSingleton<AuthorMetadataProvider>();
        }
    }
}
