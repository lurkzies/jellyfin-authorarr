using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Authorarr.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Authorarr
{
    /// <summary>
    /// Provides remote metadata for authors using the selected metadata provider.
    /// </summary>
    public sealed class AuthorMetadataProvider :
        IRemoteMetadataProvider<Person, PersonLookupInfo>,
        IRemoteSearchProvider<PersonLookupInfo>,
        IDisposable
    {
        private readonly HttpClient _httpClient = new();
        private readonly ILogger<AuthorMetadataProvider> _logger;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorMetadataProvider"/> class.
        /// </summary>
        /// <param name="logger">The logger to write diagnostic messages to.</param>
        public AuthorMetadataProvider(ILogger<AuthorMetadataProvider> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public string Name => "Authorarr";

        /// <inheritdoc/>
        public Task<HttpResponseMessage> GetImageResponse(
            string url,
            CancellationToken cancellationToken)
        {
            return _httpClient.GetAsync(url, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<MetadataResult<Person>> GetMetadata(
            PersonLookupInfo info,
            CancellationToken cancellationToken)
        {
            var result = new MetadataResult<Person>
            {
                Item = new Person { Name = info.Name }
            };

            if (string.IsNullOrWhiteSpace(info.Name))
            {
                _logger.LogWarning("Authorarr: No name provided.");
                return result;
            }

            switch (Plugin.Instance?.Configuration.SelectedProvider)
            {
                case MetadataProviderType.OpenLibrary:
                {
                    await PopulateFromOpenLibrary(
                        result,
                        info,
                        cancellationToken)
                    .ConfigureAwait(false);
                    break;
                }

                case MetadataProviderType.Wikipedia:
                {
                    _logger.LogInformation("Wikipedia support not yet implemented.");
                    break;
                }

                case MetadataProviderType.Goodreads:
                {
                    _logger.LogInformation("Goodreads support not yet implemented.");
                    break;
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(
            PersonLookupInfo info,
            CancellationToken cancellationToken)
        {
            var results = new List<RemoteSearchResult>();

            if (string.IsNullOrWhiteSpace(info.Name))
            {
                return results;
            }

            switch (Plugin.Instance?.Configuration.SelectedProvider)
            {
                case MetadataProviderType.OpenLibrary:
                {
                    var searchUrl =
                        $"https://openlibrary.org/search/authors.json?q={Uri.EscapeDataString(info.Name)}";
                    var response = await _httpClient
                        .GetAsync(searchUrl, cancellationToken)
                        .ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();

                    var stream = await response.Content
                        .ReadAsStreamAsync(cancellationToken)
                        .ConfigureAwait(false);
                    using var json = await JsonDocument
                        .ParseAsync(stream, cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                    var docs = json.RootElement.GetProperty("docs");

                    if (docs.GetArrayLength() > 0)
                    {
                        var author = docs[0];
                        var key = author.GetProperty("key").GetString();
                        var olid = key?.Split('/').Last();

                        var result = new RemoteSearchResult
                        {
                            Name = info.Name,
                            SearchProviderName = Name,
                            ImageUrl = !string.IsNullOrEmpty(olid)
                                ? $"https://covers.openlibrary.org/a/olid/{olid}-L.jpg"
                                : null,
                            ProviderIds = !string.IsNullOrEmpty(olid)
                                ? new Dictionary<string, string>
                                  {
                                      { "OpenLibrary", olid }
                                  }
                                : null
                        };

                        results.Add(result);
                    }

                    break;
                }

                case MetadataProviderType.Wikipedia:
                {
                    _logger.LogInformation("Wikipedia support not yet implemented.");
                    break;
                }

                case MetadataProviderType.Goodreads:
                {
                    _logger.LogInformation("Goodreads support not yet implemented.");
                    break;
                }
            }

            return results;
        }

        private async Task PopulateFromOpenLibrary(
            MetadataResult<Person> result,
            PersonLookupInfo info,
            CancellationToken cancellationToken)
        {
            try
            {
                var searchUrl =
                    $"https://openlibrary.org/search/authors.json?q={Uri.EscapeDataString(info.Name)}";
                var response = await _httpClient
                    .GetAsync(searchUrl, cancellationToken)
                    .ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var stream = await response.Content
                    .ReadAsStreamAsync(cancellationToken)
                    .ConfigureAwait(false);
                using var json = await JsonDocument
                    .ParseAsync(stream, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                var docs = json.RootElement.GetProperty("docs");

                if (docs.GetArrayLength() > 0)
                {
                    var author = docs[0];
                    var key = author.GetProperty("key").GetString();
                    var olid = key?.Split('/').Last();

                    if (!string.IsNullOrEmpty(olid))
                    {
                        result.Item.SetProviderId("OpenLibrary", olid);
                    }

                    if (author.TryGetProperty("top_work", out var topWork))
                    {
                        result.Item.Overview =
                            $"Best known for: {topWork.GetString()}";
                    }

                    result.HasMetadata = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Authorarr: Error retrieving author from OpenLibrary");
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _httpClient.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
