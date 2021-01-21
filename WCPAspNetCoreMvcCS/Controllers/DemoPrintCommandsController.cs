using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Neodynamic.SDK.Web;

namespace WCPAspNetCoreCS.Controllers
{
    public class DemoPrintCommandsController : Controller
    {
        //We're going to use MemoryCache BUT you can change it based on your dev needs!!!
        private readonly IMemoryCache _MemoryCache;

        public DemoPrintCommandsController(IMemoryCache memCache)
        {
            _MemoryCache = memCache;

        }

        public IActionResult Index()
        {

            ViewData["WCPScript"] = Neodynamic.SDK.Web.WebClientPrint.CreateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", null, Url.ActionContext.HttpContext.Request.Scheme), Url.Action("PrintCommands", "DemoPrintCommands", null, Url.ActionContext.HttpContext.Request.Scheme), Url.ActionContext.HttpContext.Session.Id);

            return View();
        }

        const string PRINTER_ID = "-PID";
        const string INSTALLED_PRINTER_NAME = "-InstalledPrinterName";
        const string NET_PRINTER_HOST = "-NetPrinterHost";
        const string NET_PRINTER_PORT = "-NetPrinterPort";
        const string PARALLEL_PORT = "-ParallelPort";
        const string SERIAL_PORT = "-SerialPort";
        const string SERIAL_PORT_BAUDS = "-SerialPortBauds";
        const string SERIAL_PORT_DATA_BITS = "-SerialPortDataBits";
        const string SERIAL_PORT_STOP_BITS = "-SerialPortStopBits";
        const string SERIAL_PORT_PARITY = "-SerialPortParity";
        const string SERIAL_PORT_FLOW_CONTROL = "-SerialPortFlowControl";
        const string PRINTER_COMMANDS = "-PrinterCommands";

        [HttpPost]
        public void ClientPrinterSettings(string sid,
                                             string pid,
                                             string installedPrinterName,
                                             string netPrinterHost,
                                             string netPrinterPort,
                                             string parallelPort,
                                             string serialPort,
                                             string serialPortBauds,
                                             string serialPortDataBits,
                                             string serialPortStopBits,
                                             string serialPortParity,
                                             string serialPortFlowControl,
                                             string printerCommands)
        {
            try
            {


                //save settings in the global cahe obj

                //save the type of printer selected by the user
                int i = int.Parse(pid);
                _MemoryCache.Set(sid + PRINTER_ID, i.ToString());

                if (i == 2)
                {
                    _MemoryCache.Set(sid + INSTALLED_PRINTER_NAME, installedPrinterName);
                }
                else if (i == 3)
                {
                    _MemoryCache.Set(sid + NET_PRINTER_HOST, netPrinterHost);
                    _MemoryCache.Set(sid + NET_PRINTER_PORT, netPrinterPort);
                }
                else if (i == 4)
                {
                    _MemoryCache.Set(sid + PARALLEL_PORT, parallelPort);
                }
                else if (i == 5)
                {
                    _MemoryCache.Set(sid + SERIAL_PORT, serialPort);
                    _MemoryCache.Set(sid + SERIAL_PORT_BAUDS, serialPortBauds);
                    _MemoryCache.Set(sid + SERIAL_PORT_DATA_BITS, serialPortDataBits);
                    _MemoryCache.Set(sid + SERIAL_PORT_FLOW_CONTROL, serialPortFlowControl);
                    _MemoryCache.Set(sid + SERIAL_PORT_PARITY, serialPortParity);
                    _MemoryCache.Set(sid + SERIAL_PORT_STOP_BITS, serialPortStopBits);
                }

                //save the printer commands specified by the user
                _MemoryCache.Set(sid + PRINTER_COMMANDS, printerCommands);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //sid: user session id who is requesting a ClientPrintJob
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public IActionResult PrintCommands(string sid)
        {
            try
            {
                //Create a ClientPrintJob obj that will be processed at the client side by the WCPP
                ClientPrintJob cpj = new ClientPrintJob();

                //get printer commands for this user id
                object printerCommands = _MemoryCache.Get<string>(sid + PRINTER_COMMANDS);
                if (printerCommands != null)
                {
                    cpj.PrinterCommands = printerCommands.ToString();
                    cpj.FormatHexValues = true;
                }

                //get printer settings for this user id
                int printerTypeId = int.Parse(_MemoryCache.Get<string>(sid + PRINTER_ID));

                if (printerTypeId == 0) //use default printer
                {
                    cpj.ClientPrinter = new DefaultPrinter();
                }
                else if (printerTypeId == 1) //show print dialog
                {
                    cpj.ClientPrinter = new UserSelectedPrinter();
                }
                else if (printerTypeId == 2) //use specified installed printer
                {
                    cpj.ClientPrinter = new InstalledPrinter(_MemoryCache.Get<string>(sid + INSTALLED_PRINTER_NAME));
                }
                else if (printerTypeId == 3) //use IP-Ethernet printer
                {
                    cpj.ClientPrinter = new NetworkPrinter(_MemoryCache.Get<string>(sid + NET_PRINTER_HOST), int.Parse(_MemoryCache.Get<string>(sid + NET_PRINTER_PORT)));
                }
                else if (printerTypeId == 4) //use Parallel Port printer
                {
                    cpj.ClientPrinter = new ParallelPortPrinter(_MemoryCache.Get<string>(sid + PARALLEL_PORT));
                }
                else if (printerTypeId == 5) //use Serial Port printer
                {
                    cpj.ClientPrinter = new SerialPortPrinter(_MemoryCache.Get<string>(sid + SERIAL_PORT),
                                                              int.Parse(_MemoryCache.Get<string>(sid + SERIAL_PORT_BAUDS)),
                                                              (SerialPortParity)Enum.Parse(typeof(SerialPortParity), _MemoryCache.Get<string>(sid + SERIAL_PORT_PARITY)),
                                                              (SerialPortStopBits)Enum.Parse(typeof(SerialPortStopBits), _MemoryCache.Get<string>(sid + SERIAL_PORT_STOP_BITS)),
                                                              int.Parse(_MemoryCache.Get<string>(sid + SERIAL_PORT_DATA_BITS)),
                                                              (SerialPortHandshake)Enum.Parse(typeof(SerialPortHandshake), _MemoryCache.Get<string>(sid + SERIAL_PORT_FLOW_CONTROL)));
                }

                //Send ClientPrintJob back to the client
                return File(cpj.GetContent(), "application/octet-stream");

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }



    }
}