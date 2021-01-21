using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Neodynamic.SDK.Web;

namespace WCPMVCCS.Controllers
{
    public class DemoPrintFileController : Controller
    {
        // GET: DemoPrintFile
        public ActionResult Index()
        {
            ViewBag.WCPScript = Neodynamic.SDK.Web.WebClientPrint.CreateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", null, HttpContext.Request.Url.Scheme), Url.Action("PrintFile", "DemoPrintFile", null, HttpContext.Request.Url.Scheme), HttpContext.Session.SessionID);

            return View();
        }

        [AllowAnonymous]
        public void PrintFile(string useDefaultPrinter, string printerName, string fileType)
        {
            string fileName = Guid.NewGuid().ToString("N") + "." + fileType;
            string filePath = null;
            switch (fileType)
            {
                case "PDF":
                    filePath = "~/files/LoremIpsum.pdf";
                    break;
                case "TXT":
                    filePath = "~/files/LoremIpsum.txt";
                    break;
                case "DOC":
                    filePath = "~/files/LoremIpsum.doc";
                    break;
                case "XLS":
                    filePath = "~/files/SampleSheet.xls";
                    break;
                case "JPG":
                    filePath = "~/files/penguins300dpi.jpg";
                    break;
                case "PNG":
                    filePath = "~/files/SamplePngImage.png";
                    break;
                case "TIF":
                    filePath = "~/files/patent2pages.tif";
                    break;
            }

            if (filePath != null)
            {
                PrintFile file = null;

                if (fileType == "PDF")
                {
                    file = new PrintFilePDF(System.Web.HttpContext.Current.Server.MapPath(filePath), fileName);
                    ((PrintFilePDF)file).PrintRotation = PrintRotation.None;
                    //((PrintFilePDF)file).PagesRange = "1,2,3,10-15";
                    //((PrintFilePDF)file).PrintAnnotations = true;
                    //((PrintFilePDF)file).PrintAsGrayscale = true;
                    //((PrintFilePDF)file).PrintInReverseOrder = true;
                    
                }
                else if (fileType == "TXT")
                {
                    file = new PrintFileTXT(System.Web.HttpContext.Current.Server.MapPath(filePath), fileName);
                    ((PrintFileTXT)file).PrintOrientation = PrintOrientation.Portrait;
                    ((PrintFileTXT)file).FontName = "Arial";
                    ((PrintFileTXT)file).FontSizeInPoints = 12; // Point Unit!!!
                    //((PrintFileTXT)file).TextColor = "#ff00ff";
                    //((PrintFileTXT)file).TextAlignment = TextAlignment.Center;
                    //((PrintFileTXT)file).FontBold = true;
                    //((PrintFileTXT)file).FontItalic = true;
                    //((PrintFileTXT)file).FontUnderline = true;
                    //((PrintFileTXT)file).FontStrikeThrough = true;
                    //((PrintFileTXT)file).MarginLeft = 1; // INCH Unit!!!
                    //((PrintFileTXT)file).MarginTop = 1; // INCH Unit!!!
                    //((PrintFileTXT)file).MarginRight = 1; // INCH Unit!!!
                    //((PrintFileTXT)file).MarginBottom = 1; // INCH Unit!!!
                }
                else
                {
                    file = new PrintFile(System.Web.HttpContext.Current.Server.MapPath(filePath), fileName);
                }
                
                ClientPrintJob cpj = new ClientPrintJob();
                cpj.PrintFile = file;
                if (useDefaultPrinter == "checked" || printerName == "null")
                    cpj.ClientPrinter = new DefaultPrinter();
                else
                    cpj.ClientPrinter = new InstalledPrinter(System.Web.HttpUtility.UrlDecode(printerName));

                System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
                System.Web.HttpContext.Current.Response.BinaryWrite(cpj.GetContent());
                System.Web.HttpContext.Current.Response.End();
            }
        }

    }
}