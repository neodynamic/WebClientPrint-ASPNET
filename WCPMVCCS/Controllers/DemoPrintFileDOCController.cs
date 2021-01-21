using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Neodynamic.SDK.Web;

namespace WCPMVCCS.Controllers
{
    public class DemoPrintFileDOCController : Controller
    {
        // GET: DemoPrintFile
        public ActionResult Index()
        {
            ViewBag.WCPScript = Neodynamic.SDK.Web.WebClientPrint.CreateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", null, HttpContext.Request.Url.Scheme), Url.Action("PrintFile", "DemoPrintFileDOC", null, HttpContext.Request.Url.Scheme), HttpContext.Session.SessionID);

            return View();
        }

        [AllowAnonymous]
        public void PrintFile(string printerName, string pagesRange, string printInReverseOrder, string duplexPrinting)
        {
            string fileName = Guid.NewGuid().ToString("N");
            string filePath = filePath = "~/files/Sample-Employee-Handbook.doc";

            PrintFileDOC file = new PrintFileDOC(System.Web.HttpContext.Current.Server.MapPath(filePath), fileName);
            file.PagesRange = pagesRange;
            file.PrintInReverseOrder = (printInReverseOrder == "true");
            file.DuplexPrinting = (duplexPrinting == "true");
            //file.DuplexPrintingDialogMessage = "Your custom dialog message for duplex printing";

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