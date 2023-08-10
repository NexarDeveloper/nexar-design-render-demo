using Microsoft.Extensions.DependencyInjection;
using Nexar.Client;
using Nexar.Client.Login;
using StrawberryShake;

namespace Nexar.Renderer.Api
{
    public static class NexarHelper
    {
        public static string ApiServiceUrl { get; set; } = "https://eu.api.nexar.com/graphql";

        private static string accessToken = string.Empty;
        private static string ClientId { get; }
        private static string ClientSecret { get; }

        private static Dictionary<string, NexarClient> nexarClientCache = new Dictionary<string, NexarClient>();

        static NexarHelper()
        {
            ClientId = Environment.GetEnvironmentVariable("NEXAR_CLIENT_ID") ?? throw new InvalidOperationException("Please set environment 'NEXAR_CLIENT_ID'");
            ClientSecret = Environment.GetEnvironmentVariable("NEXAR_CLIENT_SECRET") ?? throw new InvalidOperationException("Please set environment 'NEXAR_CLIENT_SECRET'");
        }

        public static async Task<IReadOnlyList<IMyWorkspace>?> LoginAsync()
        {
            // get the saved token
            accessToken = Properties.Settings.Default.NexarOAuthToken;

            // login and try to get workspaces, retry once on invalid token
            bool retryLogin = true;
            while (true)
            {
                // get yet missing token
                if (string.IsNullOrEmpty(accessToken))
                    accessToken = await AttemptLoginAsync();

                try
                {
                    // fetch workspaces
                    var client = GetNexarClient();
                    var res = await client.GetWorkspaces.ExecuteAsync();
                    res.EnsureNoErrors();

                    // success, save the new token
                    if (accessToken != Properties.Settings.Default.NexarOAuthToken)
                    {
                        Properties.Settings.Default.NexarOAuthToken = accessToken;
                        Properties.Settings.Default.Save();
                    }

                    // return workspaces
                    return res.Data!.DesWorkspaces;
                }
                catch (Exception ex)
                {
                    if (retryLogin &&
                        ex is GraphQLClientException error &&
                        error.Errors.Any(x => x.Code == "AuthExpiredToken" || x.Code == "AuthInvalidToken" || x.Code == "AUTH_NOT_AUTHORIZED"))
                    {
                        accessToken = string.Empty;
                        retryLogin = false;
                        continue;
                    }

                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                    return null;
                }
            }
        }

        private static async Task<string> AttemptLoginAsync()
        {
            var login = await LoginHelper.LoginAsync(ClientId, ClientSecret, new string[] { "user.access", "design.domain", "supply.domain" });
            return login.AccessToken;
        }

        public static NexarClient GetNexarClient(string? apiServiceUrl = null)
        {
            if (!nexarClientCache.ContainsKey(apiServiceUrl ?? ApiServiceUrl))
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection
                    .AddNexarClient()
                    .ConfigureHttpClient(httpClient =>
                    {
                        httpClient.BaseAddress = new Uri(ApiServiceUrl);
                        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                    });
                var services = serviceCollection.BuildServiceProvider();
                nexarClientCache.Add(apiServiceUrl ?? ApiServiceUrl, services.GetRequiredService<NexarClient>());
            }

            return nexarClientCache[apiServiceUrl ?? ApiServiceUrl];
        }
    }
}
