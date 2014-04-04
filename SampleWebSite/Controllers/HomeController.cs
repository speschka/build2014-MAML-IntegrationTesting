using SampleDataLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SampleWebSite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var connectionString = 
                ConfigurationManager.AppSettings["STORAGE_CONNECTION_STRING"];

            var webSiteConfiguration = new StorageHelper(connectionString).GetConfiguration();

            ViewBag.message = webSiteConfiguration.WelcomeMessage;
            ViewBag.description = webSiteConfiguration.Description;

            return View();
        }
    }
}