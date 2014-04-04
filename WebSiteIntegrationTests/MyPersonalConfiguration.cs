
namespace WebSiteIntegrationTests
{
    public class MyPersonalAzureTestConfiguration : ITestConfigurationBase
    {
        public string GetSubscriptionId()
        {
            return "[YOUR AZURE SUBSCRIPTION ID]";
        }

        public string GetTenantId()
        {
            return "[YOUR ACTIVE DIRECTORY TENANT ID]";
        }

        public string GetClientId()
        {
            return "[YOUR ACTIVE DIRECTORY APPLICATION CLIENT ID]";
        }

        public string GetRedirectUrl()
        {
            return "[YOUR ACTIVE DIRECTORY APPLICATION REDIRECT URL]";
        }
    }
}
