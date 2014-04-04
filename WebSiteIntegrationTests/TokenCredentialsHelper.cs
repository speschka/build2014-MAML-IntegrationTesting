using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSiteIntegrationTests
{
    public class TokenCredentialHelper
    {
        public static string GetAuthorizationHeader(string clientId, string redirectUri, string tenant = "common")
        {
            AuthenticationResult result = null;

            var context = new AuthenticationContext(string.Format("https://login.windows.net/{0}", tenant));
            var thread = new Thread(() =>
            {
                result = context.AcquireToken(
                    clientId: clientId,
                    redirectUri: new Uri(redirectUri),
                    resource: "https://management.core.windows.net/",
                    promptBehavior: PromptBehavior.Auto);
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Name = "AquireTokenThread";
            thread.Start();
            thread.Join();
            return result.CreateAuthorizationHeader().Substring("Bearer ".Length);
        }

        public static TokenCloudCredentials CreateCredential(
            string subscriptionId,
            string clientId,
            string redirectUri,
            string tenant = "common")
        {
            var token = TokenCredentialHelper.GetAuthorizationHeader(clientId, redirectUri, tenant);
            var cred = new TokenCloudCredentials(subscriptionId, token);
            return cred;
        }
    }
}
