using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Neodynamic.SDK.Web;

namespace WCPMVCCS.Controllers
{
    public class DemoPrintFilePDFController : Controller
    {
        // GET: DemoPrintFile
        public ActionResult Index()
        {
            ViewBag.WCPScript = Neodynamic.SDK.Web.WebClientPrint.CreateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", null, HttpContext.Request.Url.Scheme), Url.Action("PrintFile", "DemoPrintFilePDF", null, HttpContext.Request.Url.Scheme), HttpContext.Session.SessionID);

            return View();
        }

        [AllowAnonymous]
        public void PrintFile(string printerName, string trayName, string paperName, string printRotation, string pagesRange, string printAnnotations, string printAsGrayscale, string printInReverseOrder, string manualDuplexPrinting, string driverDuplexPrinting, string pageSizing, bool autoRotate, bool autoCenter)
        {
            if (manualDuplexPrinting == "true" && driverDuplexPrinting == "true")
            {
                manualDuplexPrinting = "false";
            }

            string fileName = Guid.NewGuid().ToString("N");
            string filePath = filePath = "~/files/mixed-page-orientation.pdf";

            PrintFilePDF file = new PrintFilePDF(System.Web.HttpContext.Current.Server.MapPath(filePath), fileName);
            file.PrintRotation = (PrintRotation)Enum.Parse(typeof(PrintRotation), printRotation); ;
            file.PagesRange = pagesRange;
            file.PrintAnnotations = (printAnnotations == "true");
            file.PrintAsGrayscale = (printAsGrayscale == "true");
            file.PrintInReverseOrder = (printInReverseOrder == "true");
            if (manualDuplexPrinting == "true")
            {
                file.DuplexPrinting = true;
                //file.DuplexPrintingDialogMessage = "Your custom dialog message for duplex printing";
            }
            file.Sizing = (Sizing)Enum.Parse(typeof(Sizing), pageSizing);
            file.AutoCenter = autoCenter;
            file.AutoRotate = autoRotate;

            ClientPrintJob cpj = new ClientPrintJob();
            cpj.PrintFile = file;
            if (printerName == "null")
                cpj.ClientPrinter = new DefaultPrinter();
            else
            {
                if (trayName == "null") trayName = "";
                if (paperName == "null") paperName = "";

                cpj.ClientPrinter = new InstalledPrinter(printerName, true, trayName, paperName);

                if (driverDuplexPrinting == "true")
                    ((InstalledPrinter)cpj.ClientPrinter).Duplex = Duplex.Vertical;
            }

           
            System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
            System.Web.HttpContext.Current.Response.BinaryWrite(cpj.GetContent());
            System.Web.HttpContext.Current.Response.End();
            
        }
    }
}