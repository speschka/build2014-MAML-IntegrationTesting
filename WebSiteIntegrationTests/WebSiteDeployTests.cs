﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace WebSiteIntegrationTests
{
    public class WebSiteDeployTests : IUseFixture<WebSiteTestBase>
    {
        private WebSiteTestBase _sut;
        private string _welcomeHtmlTemplate = "<h2 class=\"welcomeMessage\">{0}</h2>";
        private string _descriptionHtmlTemplate = "<p class=\"description\">{0}</p>";

        public void SetFixture(WebSiteTestBase data)
        {
            _sut = data;

            _sut.WelcomeMessage = "Welcome";
            _sut.Description = "This site was generated by an xunit test";

            _sut.Setup();
        }

        [Fact]
        public void WebSiteDisplaysTheCorrectMessages()
        {
            var url = string.Format("http://{0}.azurewebsites.net", _sut.RandomWebSiteName);
            var httpClient = new HttpClient();
            var html = httpClient.GetAsync(url).Result.Content.ReadAsStringAsync().Result;

            var welcomeMessage = string.Format(_welcomeHtmlTemplate, _sut.WelcomeMessage);
            var description = string.Format(_descriptionHtmlTemplate, _sut.Description);

            Assert.Contains(welcomeMessage, html);
            Assert.Contains(description, html);
        }
    }
}
