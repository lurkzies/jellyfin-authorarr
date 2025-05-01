using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.Authorarr.Configuration
{
    /// <summary>
    /// Plugin configuration for Authorarr.
    /// </summary>
    public class PluginConfiguration : BasePluginConfiguration
    {
        /// <summary>
        /// Gets or sets the selected metadata provider.
        /// </summary>
        public MetadataProviderType SelectedProvider { get; set; }
            = MetadataProviderType.OpenLibrary;
    }
}
