<%@ WebHandler Language="C#" Class="DemoPrintFileXLSHandler" %>

using System;
using System.Web;

using Neodynamic.SDK.Web;

public class DemoPrintFileXLSHandler : IHttpHandler {

    /*############### IMPORTANT!!! ############
    If your website requires AUTHENTICATION, then you MUST configure THIS Handler file
    to be ANONYMOUS access allowed!!!
      ######################################### */

    public void ProcessRequest (HttpContext context) {

        if (WebClientPrint.ProcessPrintJob(context.Request.Url.Query))
        {

            string printerName = context.Server.UrlDecode(context.Request["printerName"]);
            string pagesFrom = context.Server.UrlDecode(context.Request["pagesFrom"]);
            string pagesTo = context.Server.UrlDecode(context.Request["pagesTo"]);

            string fileName = Guid.NewGuid().ToString("N");
            string filePath = "~/files/Project-Scheduling-Monitoring-Tool.xls";

            PrintFileXLS file = new PrintFileXLS(context.Server.MapPath(filePath), fileName);
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