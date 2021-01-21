using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Neodynamic.SDK.Web;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;

namespace WCPAspNetCoreCS.Controllers
{
    [Authorize]
    public class WebClientPrintAPIController : Controller
    {
        //IMPORTANT NOTE >>>>>>>>>>
        // We're going to use MemoryCache to store users related staff like
        // the list of printers and they have the WCPP client utility installed
        // BUT you can change it based on your dev needs!!!
        // For instance, you could use a Distributed Cache instead!
        //>>>>>>>>>>>>>>>>>>>>>>>>>
        private readonly IMemoryCache _MemoryCache;

        public WebClientPrintAPIController(IMemoryCache memCache)
        {
            _MemoryCache = memCache;
        }

        [AllowAnonymous]
        public IActionResult ProcessRequest()
        {
            //get session ID
            string sessionID = HttpContext.Request.Query["sid"].ToString();

            //get Query String
            string queryString = HttpContext.Request.QueryString.Value;

            try
            {
                //Determine and get the Type of Request 
                RequestType prType = WebClientPrint.GetProcessRequestType(queryString);

                if (prType == RequestType.GenPrintScript ||
                    prType == RequestType.GenWcppDetectScript)
                {
                    //Let WebClientPrint to generate the requested script
                    byte[] script = WebClientPrint.GenerateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", null, HttpContext.Request.Scheme), queryString);

                    return File(script, "application/x-javascript", "WebClientPrintScript");
                }
                else if (prType == RequestType.ClientSetWcppVersion)
                {
                    //This request is a ping from the WCPP utility
                    //so store the session ID indicating it has the WCPP installed
                    //also store the WCPP Version if available
                    string wcppVersion = HttpContext.Request.Query["wcppVer"];
                    if (string.IsNullOrEmpty(wcppVersion))
                        wcppVersion = "1.0.0.0";

                    _MemoryCache.Set(sessionID + "wcppInstalled", wcppVersion);
                }
                else if (prType == RequestType.ClientSetInstalledPrinters)
                {
                    //WCPP Utility is sending the installed printers at client side
                    //so store this info with the specified session ID
                    string printers = HttpContext.Request.Query["printers"].ToString();
                    if (!string.IsNullOrEmpty(printers) && printers.Length > 0)
                        printers = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(printers));

                    _MemoryCache.Set(sessionID + "printers", printers);

                }
                else if (prType == RequestType.ClientSetInstalledPrintersInfo)
                {
                    //WCPP Utility is sending the client installed printers with detailed info
                    //so store this info with the specified session ID
                    //Printers Info is in JSON format
                    string printersInfo = HttpContext.Request.Form["printersInfoContent"];

                    if (string.IsNullOrEmpty(printersInfo) == false)
                        printersInfo = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(printersInfo));

                    _MemoryCache.Set(sessionID + "printersInfo", printersInfo);


                }
                else if (prType == RequestType.ClientGetWcppVersion)
                {
                    //return the WCPP version for the specified sid if any
                    bool sidWcppVersion = (_MemoryCache.Get<string>(sessionID + "wcppInstalled") != null);

                    return Ok(sidWcppVersion ? _MemoryCache.Get<string>(sessionID + "wcppInstalled") : "");

                }
                else if (prType == RequestType.ClientGetInstalledPrinters)
                {
                    //return the installed printers for the specified sid if any
                    bool sidHasPrinters = (_MemoryCache.Get<string>(sessionID + "printers") != null);

                    return Ok(sidHasPrinters ? _MemoryCache.Get<string>(sessionID + "printers") : "");
                }
                else if (prType == RequestType.ClientGetInstalledPrintersInfo)
                {
                    //return the installed printers with detailed info for the specified Session ID (sid) if any
                    bool sidHasPrinters = (_MemoryCache.Get<string>(sessionID + "printersInfo") != null);

                    return Ok(sidHasPrinters ? _MemoryCache.Get<string>(sessionID + "printersInfo") : "");
                }

            }
            catch
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}