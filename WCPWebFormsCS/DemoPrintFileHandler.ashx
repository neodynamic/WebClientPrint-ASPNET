<%@ WebHandler Language="C#" Class="DemoPrintFileHandler" %>

using System;
using System.Web;

using Neodynamic.SDK.Web;

public class DemoPrintFileHandler : IHttpHandler {

    /*############### IMPORTANT!!! ############
    If your website requires AUTHENTICATION, then you MUST configure THIS Handler file
    to be ANONYMOUS access allowed!!!
      ######################################### */

    public void ProcessRequest (HttpContext context) {

        //if (WebClientPrint.ProcessPrintJob(context.Request.Url.Query))
        if (WebClientPrint.ProcessPrintJob(context.Request.Url.Query))
        {
            bool useDefaultPrinter = (context.Request["useDefaultPrinter"] == "checked");
            string printerName = context.Server.UrlDecode(context.Request["printerName"]);

            string fileType = context.Request["filetype"];

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
                    file = new PrintFilePDF(context.Server.MapPath(filePath), fileName);
                    ((PrintFilePDF)file).PrintRotation = PrintRotation.None;
                    //((PrintFilePDF)file).PagesRange = "1,2,3,10-15";
                    //((PrintFilePDF)file).PrintAnnotations = true;
                    //((PrintFilePDF)file).PrintAsGrayscale = true;
                    //((PrintFilePDF)file).PrintInReverseOrder = true;

                }
                else if (fileType == "TXT")
                {
                    file = new PrintFileTXT(context.Server.MapPath(filePath), fileName);
                    ((PrintFileTXT)file).PrintOrientation = PrintOrientation.Portrait;
                    ((PrintFileTXT)file).FontName = "Arial";
                    ((PrintFileTXT)file).FontSizeInPoints = 12;
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
                    file = new PrintFile(context.Server.MapPath(filePath), fileName);
                }
                ClientPrintJob cpj = new ClientPrintJob();
                cpj.PrintFile = file;
                if (useDefaultPrinter || printerName == "null")
                    cpj.ClientPrinter = new DefaultPrinter();
                else
                    cpj.ClientPrinter = new InstalledPrinter(printerName);

                context.Response.ContentType = "application/octet-stream";
                context.Response.BinaryWrite(cpj.GetContent());
                context.Response.End();
            }


        }
        else
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "text/plain";
            context.Response.Write(context.Request.Url.Query);
            context.Response.End();
        }

    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}