using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Management.Models;
using Microsoft.WindowsAzure.Management.Storage;
using Microsoft.WindowsAzure.Management.Storage.Models;
using Microsoft.WindowsAzure.Management.WebSites;
using Microsoft.WindowsAzure.Management.WebSites.Models;
using SampleDataLayer;
using SampleDataLayer.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebSiteIntegrationTests
{
    public class WebSiteTestBase : AzureTestBase<MyPersonalAzureTestConfiguration>, IDisposable
    {
        string webSitePath = @"YOUR-PATH-TO\SampleWebSite\";

        public string WelcomeMessage { get; set; }
        public string Description { get; set; }

        public SubscriptionCloudCredentials Credentials { get; set; }
        public WebSiteManagementClient WebSiteManagementClient { get; set; }
        public StorageManagementClient StorageManagementClient { get; set; }
        public WebSpacesListResponse.WebSpace WebSpace { get; set; }
        public string RandomWebSiteName { get; set; }
        public string RandomStorageAccountName { get; set; }
        public string LocationName { get; set; }
        public string StorageAccountConnectionString { get; set; }

        public void Setup()
        {
            this.Credentials = TokenCredentialHelper.CreateCredential(
                this.Configuration.GetSubscriptionId(),
                this.Configuration.GetClientId(),
                this.Configuration.GetRedirectUrl(),
                this.Configuration.GetTenantId()
                );

            // remember the location into which i want things provisioned
            this.LocationName = LocationNames.NorthCentralUS;

            // create the web site client
            this.WebSiteManagementClient = 
                CloudContext.Clients.CreateWebSiteManagementClient(this.Credentials);

            // create a new random site name
            this.RandomWebSiteName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());

            // get the web space name for a region
            var webSpaces = this.WebSiteManagementClient.WebSpaces.List();
            this.WebSpace = webSpaces.First(x => x.GeoRegion == this.LocationName);

            // create the site
            var newSiteResult = this.WebSiteManagementClient.WebSites.Create(
                this.WebSpace.Name,
                new Microsoft.WindowsAzure.Management.WebSites.Models.WebSiteCreateParameters
                {
                    HostNames = new string[] 
                    { 
                        string.Format("{0}.azurewebsites.net", this.RandomWebSiteName) 
                    },
                    Name = this.RandomWebSiteName,
                    WebSpaceName = this.WebSpace.Name
                });

            // get the site's publish profile
            var publishProfileResult =
                this.WebSiteManagementClient.WebSites.GetPublishProfile(
                    this.WebSpace.Name, this.RandomWebSiteName);

            var profiles = publishProfileResult.PublishProfiles;
            var webDeployProfile = profiles.First(x => x.MSDeploySite != null);

            // publish the profile using web deploy
            new WebDeployPublishingHelper(
                webDeployProfile.PublishUrl,
                webDeployProfile.MSDeploySite,
                webDeployProfile.UserName,
                webDeployProfile.UserPassword,
                webSitePath
                )
                .PublishFolder();

            // create the storage management client
            this.StorageManagementClient = 
                CloudContext.Clients.CreateStorageManagementClient(this.Credentials);

            // create a random storage account name
            this.RandomStorageAccountName = 
                Path.GetFileNameWithoutExtension(Path.GetRandomFileName());

            // create a storage account
            this.StorageManagementClient.StorageAccounts.Create(
                new StorageAccountCreateParameters
                {
                    Name = this.RandomStorageAccountName,
                    Location = LocationNames.WestUS,
                    Label = this.RandomStorageAccountName
                });

            // get the storage account and keys
            var getKeysResult = 
                this.StorageManagementClient.StorageAccounts
                    .GetKeys(this.RandomStorageAccountName);

            // build the connection string
            this.StorageAccountConnectionString =
                string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}",
                    this.RandomStorageAccountName,
                    getKeysResult.SecondaryKey);

            // save the site configuration to storage using the storage sdk
            new StorageHelper(this.StorageAccountConnectionString)
                .SaveConfiguration(
                    new WebSiteMessageConfiguration
                    {
                        WelcomeMessage = this.WelcomeMessage,
                        Description = this.Description
                    });

            // get the site's configuration
            var settingsResult = 
                this.WebSiteManagementClient.WebSites.GetConfiguration(this.WebSpace.Name, this.RandomWebSiteName);

            settingsResult.AppSettings.Add("STORAGE_CONNECTION_STRING", this.StorageAccountConnectionString);

            // update the site's configuration
            this.WebSiteManagementClient.WebSites.UpdateConfiguration(
                this.WebSpace.Name, 
                this.RandomWebSiteName,
                new WebSiteUpdateConfigurationParameters
                {
                    AppSettings = settingsResult.AppSettings
                });
        }

        /// <summary>
        /// Runs when the test is being disposed. 
        /// </summary>
        public void Dispose()
        {
            // delete the site
            var existingSiteToDelete = this.WebSiteManagementClient.WebSites.Get(this.WebSpace.Name,
                this.RandomWebSiteName, new WebSiteGetParameters { });

            if (existingSiteToDelete.WebSite != null)
            {
                this.WebSiteManagementClient.WebSites.Delete(this.WebSpace.Name,
                    this.RandomWebSiteName, new WebSiteDeleteParameters { });
            }

            // delete the storage account
            var existingStorageAccount = 
                this.StorageManagementClient.StorageAccounts.Get(this.RandomStorageAccountName);

            if (existingStorageAccount != null)
            {
                this.StorageManagementClient.StorageAccounts.Delete(this.RandomStorageAccountName);
            }
        }
    }
}
