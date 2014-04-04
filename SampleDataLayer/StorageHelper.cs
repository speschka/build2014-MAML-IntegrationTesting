using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using SampleDataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleDataLayer.Models
{
    public class WebSiteMessageConfiguration
    {
        public string WelcomeMessage { get; set; }
        public string Description { get; set; }
    }
}

namespace SampleDataLayer
{
    public class WebSiteMessageConfigurationRecord : TableEntity
    {
        public WebSiteMessageConfigurationRecord()
        {
            this.PartitionKey = StorageHelper.DefaultPartitionKey;
            this.RowKey = Guid.NewGuid().ToString();
            this.Timestamp = DateTime.Now;
        }

        public string WelcomeMessage { get; set; }
        public string Description { get; set; }
    }

    public class StorageHelper
    {
        internal const string DefaultPartitionKey = "default";

        public StorageHelper(string connectionString)
        {
            this.StorageAccount =
                CloudStorageAccount.Parse(connectionString);

            this.TableClient = this.StorageAccount.CreateCloudTableClient();

            this.ConfigurationTable = this.TableClient.GetTableReference("people");
        }

        public CloudStorageAccount StorageAccount { get; set; }

        public CloudTableClient TableClient { get; set; }

        public CloudTable ConfigurationTable { get; set; }

        public void SaveConfiguration(WebSiteMessageConfiguration configuration)
        {
            this.ConfigurationTable.CreateIfNotExists();

            var operation = TableOperation.Insert(MapModelToRecord(configuration));
            this.ConfigurationTable.Execute(operation);
        }

        public WebSiteMessageConfiguration GetConfiguration()
        {
            try
            {
                var query = this.ConfigurationTable.CreateQuery<WebSiteMessageConfigurationRecord>();
                var config = this.ConfigurationTable.ExecuteQuery<WebSiteMessageConfigurationRecord>(query).ToList();
                return MapRecordToModel(config.First());
            }
            catch
            {
                return null;
            }
        }

        internal static WebSiteMessageConfigurationRecord MapModelToRecord(WebSiteMessageConfiguration model)
        {
            return new WebSiteMessageConfigurationRecord
            {
                Description = model.Description,
                WelcomeMessage = model.WelcomeMessage
            };
        }

        internal static WebSiteMessageConfiguration MapRecordToModel(WebSiteMessageConfigurationRecord record)
        {
            return new WebSiteMessageConfiguration
            {
                Description = record.Description,
                WelcomeMessage = record.WelcomeMessage
            };
        }
    }
}
