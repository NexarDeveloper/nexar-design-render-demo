using Microsoft.Extensions.DependencyInjection;
using Nexar.Client;
using Nexar.Client.Login;
using StrawberryShake;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexar.Renderer.Api
{
    public class NexarHelper
    {
        public string ApiServiceUrl { get; set; } = "https://eu.api.nexar.com/graphql";

        private string accessToken = string.Empty;
        private string ClientId { get; }
        private string ClientSecret { get; }

        public NexarHelper()
        {
            ClientId = Environment.GetEnvironmentVariable("NEXAR_CLIENT_ID") ?? throw new InvalidOperationException("Please set environment 'NEXAR_CLIENT_ID'");
            ClientSecret = Environment.GetEnvironmentVariable("NEXAR_CLIENT_SECRET") ?? throw new InvalidOperationException("Please set environment 'NEXAR_CLIENT_SECRET'");
        }

        public async Task LoginAsync()
        {
            accessToken = Properties.Settings.Default.NexarOAuthToken;

            if (string.IsNullOrEmpty(accessToken))
            {
                accessToken = await AttemptLoginAsync();
            }

            try
            {
                var client = GetNexarClient();
                var workspaces = await client.GetWorkspaces.ExecuteAsync();
                workspaces.EnsureNoErrors();
            }
            catch (GraphQLClientException ex)
            {
                if (ex.Errors.Any(x => x.Code == "AuthExpiredToken"))
                {
                    accessToken = await AttemptLoginAsync();
                }
                else
                {
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
            }

            if (accessToken != Properties.Settings.Default.NexarOAuthToken)
            {
                Properties.Settings.Default.NexarOAuthToken = accessToken;
                Properties.Settings.Default.Save();
            }
        }

        private async Task<string> AttemptLoginAsync()
        {
            var login = await LoginHelper.LoginAsync(ClientId, ClientSecret, new string[] { "user.access", "design.domain", "supply.domain" });
            return login.AccessToken;
        }

        public NexarClient GetNexarClient()
        {
            var nexarToken = "Bearer " + accessToken;

            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddNexarClient()
                .ConfigureHttpClient(httpClient =>
                {
                    httpClient.BaseAddress = new Uri(ApiServiceUrl);
                    httpClient.DefaultRequestHeaders.Add("Authorization", nexarToken);
                });
            var services = serviceCollection.BuildServiceProvider();
            var nexarClient = services.GetRequiredService<NexarClient>();

            return nexarClient;
        }
    }
}
