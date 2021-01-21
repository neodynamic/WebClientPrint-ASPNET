using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Neodynamic.SDK.Web;
using Microsoft.AspNetCore.Hosting;

namespace WCPAspNetCoreCS.Controllers
{
    public class DemoPrintFileDOCController : Controller
    {

        private readonly IHostingEnvironment _hostEnvironment;


        public DemoPrintFileDOCController(IHostingEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;

        }

        public IActionResult Index()
        {
            ViewData["WCPScript"] = Neodynamic.SDK.Web.WebClientPrint.CreateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", null, Url.ActionContext.HttpContext.Request.Scheme), Url.Action("PrintFile", "DemoPrintFileDOC", null, Url.ActionContext.HttpContext.Request.Scheme), Url.ActionContext.HttpContext.Session.Id);

            return View();
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public IActionResult PrintFile(string printerName, string pagesRange, string printInReverseOrder, string manualDuplexPrinting)
        {
            string fileName = Guid.NewGuid().ToString("N");
            string filePath = filePath = "/files/Sample-Employee-Handbook.doc";

            PrintFileDOC file = new PrintFileDOC(_hostEnvironment.ContentRootPath + filePath, fileName);
            file.PagesRange = pagesRange;
            file.PrintInReverseOrder = (printInReverseOrder == "true");
            file.DuplexPrinting = (manualDuplexPrinting == "true");
            //file.DuplexPrintingDialogMessage = "Your custom dialog message for duplex printing";
            

            ClientPrintJob cpj = new ClientPrintJob();
            cpj.PrintFile = file;
            if (printerName == "null")
                cpj.ClientPrinter = new DefaultPrinter();
            else
            {
                cpj.ClientPrinter = new InstalledPrinter(System.Net.WebUtility.UrlDecode(printerName));
            }

            return File(cpj.GetContent(), "application/octet-stream");

        }

    }
}