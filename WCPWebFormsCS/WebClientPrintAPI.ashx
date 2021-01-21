<%@ WebHandler Language="C#" Class="WebClientPrintAPI" %>

using System;
using System.Web;
using System.IO;

using Neodynamic.SDK.Web;

public class WebClientPrintAPI : IHttpHandler {

    //*********************************
    // IMPORTANT NOTE 
    // In this sample we store users related stuff (like
    // the list of printers and whether they have the WCPP 
    // client utility installed) in the Application cache
    // object part of ASP.NET BUT you can change it to 
    // another different storage (like a DB or file server)!
    // which will be required in Load Balacing scenarios
    //*********************************

    public void ProcessRequest (HttpContext context) {
        //get session ID
        string sessionID = (context.Request["sid"] != null) ? context.Request["sid"].ToString() : null;

        //get Query String
        string queryString = context.Request.Url.Query;

        try
        {
            //Determine and get the Type of Request 
            RequestType prType = WebClientPrint.GetProcessRequestType(queryString);

            if (prType == RequestType.GenPrintScript ||
                prType == RequestType.GenWcppDetectScript)
            {
                //Let WebClientPrint to generate the requested script
                byte[] script = WebClientPrint.GenerateScript(context.Request.Url.AbsoluteUri.Replace(queryString, ""), queryString);

                context.Response.ContentType = "text/javascript";
                context.Response.BinaryWrite(script);
            }
            else if (prType == RequestType.ClientSetWcppVersion)
            {
                //This request is a ping from the WCPP utility
                //so store the session ID indicating this user has the WCPP installed
                //also store the WCPP Version if available
                string wcppVersion = context.Request["wcppVer"];
                if (string.IsNullOrEmpty(wcppVersion))
                    wcppVersion = "1.0.0.0";

                context.Application.Set(sessionID + "wcppInstalled", wcppVersion);
            }
            else if (prType == RequestType.ClientSetInstalledPrinters)
            {
                //WCPP Utility is sending the installed printers at client side
                //so store this info with the specified session ID
                string printers = context.Request["printers"];
                if (string.IsNullOrEmpty(printers) == false)
                    printers = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(printers));

                context.Application.Set(sessionID + "printers", printers);

            }
            else if (prType == RequestType.ClientSetInstalledPrintersInfo)
            {
                //WCPP Utility is sending the client installed printers with detailed info
                //so store this info with the specified session ID
                //Printers Info is in JSON format
                string printersInfo = context.Request.Params["printersInfoContent"];

                if (string.IsNullOrEmpty(printersInfo) == false)
                    printersInfo = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(printersInfo));

                context.Application.Set(sessionID + "printersInfo", printersInfo);

            }
            else if (prType == RequestType.ClientGetWcppVersion)
            {
                //return the WCPP version for the specified Session ID (sid) if any
                bool sidWcppVersion = (context.Application[sessionID + "wcppInstalled"] != null);

                context.Response.ContentType = "text/plain";
                context.Response.Write(sidWcppVersion ? context.Application[sessionID + "wcppInstalled"] : "");

            }
            else if (prType == RequestType.ClientGetInstalledPrinters)
            {
                //return the installed printers for the specified Session ID (sid) if any
                bool sidHasPrinters = (context.Application[sessionID + "printers"] != null);

                context.Response.ContentType = "text/plain";
                context.Response.Write(sidHasPrinters ? context.Application[sessionID + "printers"] : "");

            }
            else if (prType == RequestType.ClientGetInstalledPrintersInfo)
            {
                //return the installed printers with detailed info for the specified Session ID (sid) if any
                bool sidHasPrinters = (context.Application[sessionID + "printersInfo"] != null);

                context.Response.ContentType = "text/plain";
                context.Response.Write(sidHasPrinters ? context.Application[sessionID + "printersInfo"] : "");

            }

        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/plain";
            context.Response.Write(ex.Message + " - " + ex.StackTrace);
        }



    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}