# Using the Microsoft Azure Management Libraries for Integration Testing #
The code in this repository was used to demonstrate usage of MAML to enable integration testing in Microsoft Azure. 

The code contains a test harness that provisions a new Web Site and Storage Account in Microsoft Azure. Then, it saves data into the storage account and publishes the web site. Once the site is published, the storage account connection string is obtained via the Storage Management Client in MAML, and the connection string is then set as one of the Web Site's configuration settings. The site is then browsed and evaluated for proper data display. Then, the Web Site and Storage Accounts are deleted. 

To watch this demo in action see the [//build/ 2014 session's recording on Channel 9](http://channel9.msdn.com/Events/Build/2014/3-621). 

## Setup ##
To run this demo you'll need to set up an Azure Active Directory application, and edit the code in the file **MyPersonalConfiguration.cs** in the **WebSiteIntegrationTests** project. This code is below for your reference.

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

The second (and final) step in the setup process is to change the path to the **SampleWebSite** project contained in this solution. This code is in the file **WebSiteTestsBase.cs** and is visible below for reference:

	string webSitePath = @"YOUR-PATH-TO\SampleWebSite\";

[This YouTube video](https://www.youtube.com/watch?v=hG6a8oyxynA&feature=youtu.be) contains a walk-through of the MAML Demo Project Template Visual Studio Extension, which also makes use of Azure Active Directory if you need guidance on how to set up an Azure Active Directory for use with the Microsoft Azure Management API. 