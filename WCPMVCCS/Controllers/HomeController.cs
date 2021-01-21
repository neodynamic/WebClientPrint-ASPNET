using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WCPMVCCS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            ViewBag.WCPPDetectionScript = Neodynamic.SDK.Web.WebClientPrint.CreateWcppDetectionScript(Url.Action("ProcessRequest", "WebClientPrintAPI", null, HttpContext.Request.Url.Scheme), HttpContext.Session.SessionID);

            return View();
        }

        public ActionResult Samples()
        {
            return View();
        }

        public ActionResult PrintersInfo()
        {
            ViewBag.WCPScript = Neodynamic.SDK.Web.WebClientPrint.CreateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", null, HttpContext.Request.Url.Scheme), "", HttpContext.Session.SessionID);

            return View();
        }
    }
}