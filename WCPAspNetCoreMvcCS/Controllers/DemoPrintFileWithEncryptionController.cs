using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Neodynamic.SDK.Web;
using Microsoft.AspNetCore.Hosting;

namespace WCPAspNetCoreCS.Controllers
{
    public class DemoPrintFileWithEncryptionController : Controller
    {
        private readonly IHostingEnvironment _hostEnvironment;


        public DemoPrintFileWithEncryptionController(IHostingEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;

        }

        public IActionResult Index()
        {
            ViewData["WCPScript"] = Neodynamic.SDK.Web.WebClientPrint.CreateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", null, Url.ActionContext.HttpContext.Request.Scheme), Url.Action("PrintFile", "DemoPrintFileWithEncryption", null, Url.ActionContext.HttpContext.Request.Scheme), Url.ActionContext.HttpContext.Session.Id);

            return View();
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public IActionResult PrintFile(string useDefaultPrinter, string printerName, string fileType, string wcp_pub_key_base64, string wcp_pub_key_signature_base64)
        {
            string fileName = Guid.NewGuid().ToString("N") + "." + fileType;
            string filePath = null;
            switch (fileType)
            {
                case "PDF":
                    filePath = "/files/LoremIpsum.pdf";
                    break;
                case "TXT":
                    filePath = "/files/LoremIpsum.txt";
                    break;
                case "JPG":
                    filePath = "/files/penguins300dpi.jpg";
                    break;
                case "PNG":
                    filePath = "/files/SamplePngImage.png";
                    break;
            }

            if (filePath != null && string.IsNullOrEmpty(wcp_pub_key_base64) == false)
            {
                PrintFile file = null;
                if (fileType == "PDF")
                {
                    file = new PrintFilePDF(_hostEnvironment.ContentRootPath + filePath, fileName);
                    ((PrintFilePDF)file).PrintRotation = PrintRotation.None;
                    //((PrintFilePDF)file).PagesRange = "1,2,3,10-15";
                    //((PrintFilePDF)file).PrintAnnotations = true;
                    //((PrintFilePDF)file).PrintAsGrayscale = true;
                    //((PrintFilePDF)file).PrintInReverseOrder = true;

                }
                else if (fileType == "TXT")
                {
                    file = new PrintFileTXT(_hostEnvironment.ContentRootPath + filePath, fileName);
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
                    file = new PrintFile(_hostEnvironment.ContentRootPath + filePath, fileName);
                }

                //create an encryption metadata to set to the PrintFile
                EncryptMetadata encMetadata = new EncryptMetadata(wcp_pub_key_base64, wcp_pub_key_signature_base64);

                //set encyption metadata to PrintFile
                file.EncryptMetadata = encMetadata;

                //create ClientPrintJob for printing encrypted file
                ClientPrintJob cpj = new ClientPrintJob();
                cpj.PrintFile = file;

                if (useDefaultPrinter == "checked" || printerName == "null")
                    cpj.ClientPrinter = new DefaultPrinter();
                else
                    cpj.ClientPrinter = new InstalledPrinter(System.Net.WebUtility.UrlDecode(printerName));

                //set the Encryption Metadata
                Response.Cookies.Append("wcp_enc_metadata", encMetadata.Serialize(), new Microsoft.AspNetCore.Http.CookieOptions()
                {
                    Path = "/",
                    HttpOnly = false,
                    IsEssential = true //<- MUST BE SET TO TRUE; otherwise the cookie will not be appended!
                });

                return File(cpj.GetContent(), "application/octet-stream");
            }
            else
            {
                return BadRequest("File not found!");
            }
        }

    }
}