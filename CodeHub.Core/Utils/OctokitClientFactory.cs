using System;
using System.Net.Http;
using Octokit;
using Octokit.Internal;
using CodeHub.Core.Services;

namespace CodeHub.Core.Utils
{
    public static class OctokitClientFactory
    {
        public static Func<HttpClientHandler> CreateMessageHandler = () => new HttpClientHandler();

        public static GitHubClient Create(Uri domain, Credentials credentials)
        {
            // Decorate the HttpClient
            //IHttpClient httpClient = new HttpClientAdapter();
            //httpClient = new OctokitCacheClient(httpClient);
            var client = new HttpClientAdapter(CreateMessageHandler);
            var httpClient = new OctokitNetworkClient(client, MvvmCross.Platform.Mvx.Resolve<INetworkActivityService>());

            var connection = new Connection(
                new ProductHeaderValue("CodeHub"),
                domain,
                new InMemoryCredentialStore(credentials),
                httpClient,
                new SimpleJsonSerializer());
            return new GitHubClient(connection);
        }
    }
}

