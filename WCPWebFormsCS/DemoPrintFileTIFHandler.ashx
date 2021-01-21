<%@ WebHandler Language="C#" Class="DemoPrintFileTIFHandler" %>

using System;
using System.Web;

using Neodynamic.SDK.Web;

public class DemoPrintFileTIFHandler : IHttpHandler {

    /*############### IMPORTANT!!! ############
    If your website requires AUTHENTICATION, then you MUST configure THIS Handler file
    to be ANONYMOUS access allowed!!!
      ######################################### */

    public void ProcessRequest (HttpContext context) {

        if (WebClientPrint.ProcessPrintJob(context.Request.Url.Query))
        {

            string printerName = context.Server.UrlDecode(context.Request["printerName"]);
            string trayName = context.Server.UrlDecode(context.Request["trayName"]);
            string paperName = context.Server.UrlDecode(context.Request["paperName"]);

            string printRotation = context.Server.UrlDecode(context.Request["printRotation"]);
            string pagesRange = context.Server.UrlDecode(context.Request["pagesRange"]);

            bool printAsGrayscale = (context.Request["printAsGrayscale"] == "true");
            bool printInReverseOrder = (context.Request["printInReverseOrder"] == "true");
            bool manualDuplexPrinting = (context.Request["manualDuplexPrinting"] == "true");
            bool driverDuplexPrinting = (context.Request["driverDuplexPrinting"] == "true");

            if (manualDuplexPrinting && driverDuplexPrinting)
            {
                manualDuplexPrinting = false;
            }

            string pageSizing = context.Server.UrlDecode(context.Request["pageSizing"]);
            bool autoRotate = (context.Request["autoRotate"] == "true");
            bool autoCenter = (context.Request["autoCenter"] == "true");

            string fileName = Guid.NewGuid().ToString("N");
            string filePath = "~/files/patent2pages.tif";

            PrintFileTIF file = new PrintFileTIF(context.Server.MapPath(filePath), fileName);
            file.PrintRotation = (PrintRotation)Enum.Parse(typeof(PrintRotation), printRotation);
            file.PagesRange = pagesRange;
            file.PrintAsGrayscale = printAsGrayscale;
            file.PrintInReverseOrder = printInReverseOrder;
            if (manualDuplexPrinting)
            {
                file.DuplexPrinting = manualDuplexPrinting;
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
                if (driverDuplexPrinting)
                    ((InstalledPrinter)cpj.ClientPrinter).Duplex = Duplex.Vertical;
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