Imports System.Web.Mvc

Imports Neodynamic.SDK.Web


Namespace Controllers
    Public Class DemoPrintFileDOCController
        Inherits Controller

        ' GET: DemoPrintFilePDF
        Function Index() As ActionResult

            ViewData("WCPScript") = Neodynamic.SDK.Web.WebClientPrint.CreateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", Nothing, HttpContext.Request.Url.Scheme), Url.Action("PrintFile", "DemoPrintFileDOC", Nothing, HttpContext.Request.Url.Scheme), HttpContext.Session.SessionID)

            Return View()
        End Function


        <AllowAnonymous> Public Sub PrintFile(printerName As String, pagesRange As String, printInReverseOrder As String, duplexPrinting As String)

            Dim fileName As String = Guid.NewGuid().ToString("N")
            Dim filePath As String = "~/files/Sample-Employee-Handbook.doc"

            Dim File As New PrintFileDOC(System.Web.HttpContext.Current.Server.MapPath(filePath), fileName)
            File.PagesRange = pagesRange
            File.PrintInReverseOrder = (printInReverseOrder = "true")
            File.DuplexPrinting = (duplexPrinting = "true")
            File.DuplexPrintingDialogMessage = "Your custom dialog message for duplex printing"

            Dim cpj As New ClientPrintJob()
            cpj.PrintFile = File
            If (printerName = "null") Then
                cpj.ClientPrinter = New DefaultPrinter()
            Else
                cpj.ClientPrinter = New InstalledPrinter(printerName)
            End If

            System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream"
            System.Web.HttpContext.Current.Response.BinaryWrite(cpj.GetContent())
            System.Web.HttpContext.Current.Response.End()

        End Sub

    End Class
End Namespace