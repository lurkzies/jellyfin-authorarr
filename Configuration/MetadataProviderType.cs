namespace Jellyfin.Plugin.Authorarr.Configuration
{
    /// <summary>
    /// Enumerates the available metadata providers.
    /// </summary>
    public enum MetadataProviderType
    {
        /// <summary>
        /// The OpenLibrary metadata provider.
        /// </summary>
        OpenLibrary,

        /// <summary>
        /// The Wikipedia metadata provider.
        /// </summary>
        Wikipedia,

        /// <summary>
        /// The Goodreads metadata provider.
        /// </summary>
        Goodreads
    }
}
