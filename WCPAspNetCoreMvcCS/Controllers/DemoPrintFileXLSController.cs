using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Neodynamic.SDK.Web;
using Microsoft.AspNetCore.Hosting;

namespace WCPAspNetCoreCS.Controllers
{
    public class DemoPrintFileXLSController : Controller
    {

        private readonly IHostingEnvironment _hostEnvironment;


        public DemoPrintFileXLSController(IHostingEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;

        }

        public IActionResult Index()
        {
            ViewData["WCPScript"] = Neodynamic.SDK.Web.WebClientPrint.CreateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", null, Url.ActionContext.HttpContext.Request.Scheme), Url.Action("PrintFile", "DemoPrintFileXLS", null, Url.ActionContext.HttpContext.Request.Scheme), Url.ActionContext.HttpContext.Session.Id);

            return View();
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public IActionResult PrintFile(string printerName, string pagesFrom, string pagesTo)
        {
            string fileName = Guid.NewGuid().ToString("N");
            string filePath = filePath = "/files/Project-Scheduling-Monitoring-Tool.xls";

            PrintFileXLS file = new PrintFileXLS(_hostEnvironment.ContentRootPath + filePath, fileName);
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
                cpj.ClientPrinter = new InstalledPrinter(System.Net.WebUtility.UrlDecode(printerName));
            }

            return File(cpj.GetContent(), "application/octet-stream");

        }

    }
}