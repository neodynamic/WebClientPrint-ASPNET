using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Neodynamic.SDK.Web;

namespace WCPMVCCS.Controllers
{
    public class DemoPrintFileXLSController : Controller
    {
        // GET: DemoPrintFile
        public ActionResult Index()
        {
            ViewBag.WCPScript = Neodynamic.SDK.Web.WebClientPrint.CreateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", null, HttpContext.Request.Url.Scheme), Url.Action("PrintFile", "DemoPrintFileXLS", null, HttpContext.Request.Url.Scheme), HttpContext.Session.SessionID);

            return View();
        }

        [AllowAnonymous]
        public void PrintFile(string printerName, string pagesFrom, string pagesTo)
        {
            string fileName = Guid.NewGuid().ToString("N");
            string filePath = "~/files/Project-Scheduling-Monitoring-Tool.xls";

            PrintFileXLS file = new PrintFileXLS(System.Web.HttpContext.Current.Server.MapPath(filePath), fileName);
            if (string.IsNullOrEmpty(pagesFrom) == false)
                file.PagesFrom = int.Parse(pagesFrom);
            if (string.IsNullOrEmpty(pagesTo) == false)
                file.PagesTo = int.Parse(pagesTo);

            ClientPrintJob cpj = new ClientPrintJob();
            cpj.PrintFile = file;
            if (printerName == "null")
                cpj.ClientPrinter = new DefaultPrinter();
            else
            {
                cpj.ClientPrinter = new InstalledPrinter(printerName);
            }

           
            System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
            System.Web.HttpContext.Current.Response.BinaryWrite(cpj.GetContent());
            System.Web.HttpContext.Current.Response.End();
            
        }
    }
}