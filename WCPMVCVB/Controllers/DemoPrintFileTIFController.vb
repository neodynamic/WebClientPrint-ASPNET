Imports System.Web.Mvc

Imports Neodynamic.SDK.Web


Namespace Controllers
    Public Class DemoPrintFileTIFController
        Inherits Controller

        ' GET: DemoPrintFileTIF
        Function Index() As ActionResult

            ViewData("WCPScript") = Neodynamic.SDK.Web.WebClientPrint.CreateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", Nothing, HttpContext.Request.Url.Scheme), Url.Action("PrintFile", "DemoPrintFileTIF", Nothing, HttpContext.Request.Url.Scheme), HttpContext.Session.SessionID)

            Return View()
        End Function


        <AllowAnonymous> Public Sub PrintFile(printerName As String, trayName As String, paperName As String, printRotation As String, pagesRange As String, printAsGrayscale As String, printInReverseOrder As String, manualDuplexPrinting As String, driverDuplexPrinting As String, pageSizing As String, autoRotate As Boolean, autoCenter As Boolean)

            If (manualDuplexPrinting = "true" AndAlso driverDuplexPrinting = "true") Then
                manualDuplexPrinting = "false"
            End If

            Dim fileName As String = Guid.NewGuid().ToString("N")
            Dim filePath As String = "~/files/patent2pages.tif"

            Dim File As New PrintFileTIF(System.Web.HttpContext.Current.Server.MapPath(filePath), fileName)
            File.PrintRotation = [Enum].Parse(GetType(PrintRotation), printRotation)
            File.PagesRange = pagesRange
            File.PrintAsGrayscale = (printAsGrayscale = "true")
            File.PrintInReverseOrder = (printInReverseOrder = "true")
            If (manualDuplexPrinting = "true") Then
                File.DuplexPrinting = True
                File.DuplexPrintingDialogMessage = "Your custom dialog message for duplex printing"
            End If
            File.Sizing = [Enum].Parse(GetType(Sizing), pageSizing)
            File.AutoCenter = autoCenter
            File.AutoRotate = autoRotate

            Dim cpj As New ClientPrintJob()
            cpj.PrintFile = File
            If (printerName = "null") Then
                cpj.ClientPrinter = New DefaultPrinter()
            Else
                If (trayName = "null") Then trayName = ""
                If (paperName = "null") Then paperName = ""

                cpj.ClientPrinter = New InstalledPrinter(printerName, True, trayName, paperName)

                If (driverDuplexPrinting = "true") Then
                    DirectCast(cpj.ClientPrinter, InstalledPrinter).Duplex = Duplex.Vertical
                End If

            End If

            System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream"
            System.Web.HttpContext.Current.Response.BinaryWrite(cpj.GetContent())
            System.Web.HttpContext.Current.Response.End()

        End Sub

    End Class
End Namespace