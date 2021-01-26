using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransientHandler.Web.Models;

namespace TransientHandler.Web.Controllers
{
    public class HomeController : Controller
    {
        private static Models.ConfigurationSettings _settings;

        public HomeController()
        {
            if (_settings == null)
            {
                _settings = ConfigurationSettingsManager.Load();
                //_settings = ConfigurationSettingsManager.LoadFromBlobStorage();
            }
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            if (_settings != null)
            {
                ViewBag.DatabaseServer = _settings.databaseServer;
                ViewBag.DatabaseName = _settings.databaseName;
                ViewBag.DatabasePort = _settings.databasePort;
            }

            return View();
        }
    }
}
