<%@ WebHandler Language="C#" Class="DemoPrintFileDOCHandler" %>

using System;
using System.Web;

using Neodynamic.SDK.Web;

public class DemoPrintFileDOCHandler : IHttpHandler {

    /*############### IMPORTANT!!! ############
    If your website requires AUTHENTICATION, then you MUST configure THIS Handler file
    to be ANONYMOUS access allowed!!!
      ######################################### */

    public void ProcessRequest (HttpContext context) {

        if (WebClientPrint.ProcessPrintJob(context.Request.Url.Query))
        {

            string printerName = context.Server.UrlDecode(context.Request["printerName"]);
            string pagesRange = context.Server.UrlDecode(context.Request["pagesRange"]);

            bool printInReverseOrder = (context.Request["printInReverseOrder"] == "true");
            bool duplexPrinting = (context.Request["duplexPrinting"] == "true");

            string fileName = Guid.NewGuid().ToString("N");
            string filePath = filePath = "~/files/Sample-Employee-Handbook.doc";

            PrintFileDOC file = new PrintFileDOC(context.Server.MapPath(filePath), fileName);
            file.PagesRange = pagesRange;
            file.PrintInReverseOrder = printInReverseOrder;
            file.DuplexPrinting = duplexPrinting;
            //file.DuplexPrintingDialogMessage = "Your custom dialog message for duplex printing";

            ClientPrintJob cpj = new ClientPrintJob();
            cpj.PrintFile = file;
            if (printerName == "null")
                cpj.ClientPrinter = new DefaultPrinter();
            else
            {
                cpj.ClientPrinter = new InstalledPrinter(printerName);
            }

            context.Response.ContentType = "application/octet-stream";
            context.Response.BinaryWrite(cpj.GetContent());
            context.Response.End();

        }

    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}