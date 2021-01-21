Imports System.Web.Mvc

Imports Neodynamic.SDK.Web

Namespace Controllers
    Public Class DemoPrintCommandsController
        Inherits Controller

        ' GET: DemoPrintCommands
        Function Index() As ActionResult

            ViewData("WCPScript") = Neodynamic.SDK.Web.WebClientPrint.CreateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", Nothing, HttpContext.Request.Url.Scheme), Url.Action("PrintCommands", "DemoPrintCommands", Nothing, HttpContext.Request.Url.Scheme), HttpContext.Session.SessionID)

            Return View()
        End Function

        'sid: user session id who is requesting a ClientPrintJob
        <AllowAnonymous>
        Public Sub PrintCommands(sid As String)
            If WebClientPrint.ProcessPrintJob(System.Web.HttpContext.Current.Request.Url.Query) Then
                Dim app As HttpApplicationStateBase = HttpContext.Application

                'Create a ClientPrintJob obj that will be processed at the client side by the WCPP
                Dim cpj As New ClientPrintJob()

                'get printer commands for this user id
                Dim printerCommands As Object = app(sid + PRINTER_COMMANDS)
                If printerCommands IsNot Nothing Then
                    cpj.PrinterCommands = printerCommands.ToString()
                    cpj.FormatHexValues = True
                End If

                'get printer settings for this user id
                Dim printerTypeId As Integer = CInt(app(sid + PRINTER_ID))

                If printerTypeId = 0 Then
                    'use default printer
                    cpj.ClientPrinter = New DefaultPrinter()
                ElseIf printerTypeId = 1 Then
                    'show print dialog
                    cpj.ClientPrinter = New UserSelectedPrinter()
                ElseIf printerTypeId = 2 Then
                    'use specified installed printer
                    cpj.ClientPrinter = New InstalledPrinter(app(sid + INSTALLED_PRINTER_NAME).ToString())
                ElseIf printerTypeId = 3 Then
                    'use IP-Ethernet printer
                    cpj.ClientPrinter = New NetworkPrinter(app(sid + NET_PRINTER_HOST).ToString(), Integer.Parse(app(sid + NET_PRINTER_PORT).ToString()))
                ElseIf printerTypeId = 4 Then
                    'use Parallel Port printer
                    cpj.ClientPrinter = New ParallelPortPrinter(app(sid + PARALLEL_PORT).ToString())
                ElseIf printerTypeId = 5 Then
                    'use Serial Port printer
                    cpj.ClientPrinter = New SerialPortPrinter(app(sid + SERIAL_PORT).ToString(), Integer.Parse(app(sid + SERIAL_PORT_BAUDS).ToString()), DirectCast([Enum].Parse(GetType(SerialPortParity), app(sid + SERIAL_PORT_PARITY).ToString()), SerialPortParity), DirectCast([Enum].Parse(GetType(SerialPortStopBits), app(sid + SERIAL_PORT_STOP_BITS).ToString()), SerialPortStopBits), Integer.Parse(app(sid + SERIAL_PORT_DATA_BITS).ToString()), DirectCast([Enum].Parse(GetType(SerialPortHandshake), app(sid + SERIAL_PORT_FLOW_CONTROL).ToString()), SerialPortHandshake))
                End If

                'Send ClientPrintJob back to the client

                System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream"
                System.Web.HttpContext.Current.Response.BinaryWrite(cpj.GetContent())
                System.Web.HttpContext.Current.Response.End()

            End If

        End Sub


        Const PRINTER_ID As String = "-PID"
        Const INSTALLED_PRINTER_NAME As String = "-InstalledPrinterName"
        Const NET_PRINTER_HOST As String = "-NetPrinterHost"
        Const NET_PRINTER_PORT As String = "-NetPrinterPort"
        Const PARALLEL_PORT As String = "-ParallelPort"
        Const SERIAL_PORT As String = "-SerialPort"
        Const SERIAL_PORT_BAUDS As String = "-SerialPortBauds"
        Const SERIAL_PORT_DATA_BITS As String = "-SerialPortDataBits"
        Const SERIAL_PORT_STOP_BITS As String = "-SerialPortStopBits"
        Const SERIAL_PORT_PARITY As String = "-SerialPortParity"
        Const SERIAL_PORT_FLOW_CONTROL As String = "-SerialPortFlowControl"
        Const PRINTER_COMMANDS As String = "-PrinterCommands"

        <HttpPost()>
        <AllowAnonymous>
        Public Sub ClientPrinterSettings(sid As String, pid As String, installedPrinterName As String, netPrinterHost As String, netPrinterPort As String, parallelPort As String,
                                             serialPort As String, serialPortBauds As String, serialPortDataBits As String, serialPortStopBits As String, serialPortParity As String, serialPortFlowControl As String,
                                             printerCommands As String)
            Try
                Dim app As HttpApplicationStateBase = HttpContext.Application

                'save settings in the global Application obj

                'save the type of printer selected by the user
                Dim i As Integer = Integer.Parse(pid)
                app(sid + PRINTER_ID) = i

                If i = 2 Then
                    app(sid + INSTALLED_PRINTER_NAME) = installedPrinterName
                ElseIf i = 3 Then
                    app(sid + NET_PRINTER_HOST) = netPrinterHost
                    app(sid + NET_PRINTER_PORT) = netPrinterPort
                ElseIf i = 4 Then
                    app(sid + PARALLEL_PORT) = parallelPort
                ElseIf i = 5 Then
                    app(sid + SERIAL_PORT) = serialPort
                    app(sid + SERIAL_PORT_BAUDS) = serialPortBauds
                    app(sid + SERIAL_PORT_DATA_BITS) = serialPortDataBits
                    app(sid + SERIAL_PORT_FLOW_CONTROL) = serialPortFlowControl
                    app(sid + SERIAL_PORT_PARITY) = serialPortParity
                    app(sid + SERIAL_PORT_STOP_BITS) = serialPortStopBits
                End If

                'save the printer commands specified by the user
                app(sid + PRINTER_COMMANDS) = printerCommands
            Catch ex As Exception
                Throw ex
            End Try
        End Sub
    End Class
End Namespace