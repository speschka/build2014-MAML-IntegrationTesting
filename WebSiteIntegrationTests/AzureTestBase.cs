using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSiteIntegrationTests
{
    public abstract class AzureTestBase<T> where T : ITestConfigurationBase, new()
    {
        public T Configuration { get; private set; }

        public AzureTestBase()
        {
            this.Configuration = new T();
        }
    }

    /// <summary>
    /// Class used for configuring the tests
    /// </summary>
    public interface ITestConfigurationBase
    {
        string GetSubscriptionId();
        string GetTenantId();
        string GetClientId();
        string GetRedirectUrl();
    }
}
