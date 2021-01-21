Imports System.Web.Mvc

Imports Neodynamic.SDK.Web


Namespace Controllers
    Public Class DemoPrintFileXLSController
        Inherits Controller

        ' GET: DemoPrintFilePDF
        Function Index() As ActionResult

            ViewData("WCPScript") = Neodynamic.SDK.Web.WebClientPrint.CreateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", Nothing, HttpContext.Request.Url.Scheme), Url.Action("PrintFile", "DemoPrintFileXLS", Nothing, HttpContext.Request.Url.Scheme), HttpContext.Session.SessionID)

            Return View()
        End Function


        <AllowAnonymous> Public Sub PrintFile(printerName As String, pagesFrom As String, pagesTo As String)

            Dim fileName As String = Guid.NewGuid().ToString("N")
            Dim filePath As String = "~/files/Project-Scheduling-Monitoring-Tool.xls"

            Dim File As New PrintFileXLS(System.Web.HttpContext.Current.Server.MapPath(filePath), fileName)
            If (String.IsNullOrEmpty(pagesFrom) = False) Then
                File.PagesFrom = Integer.Parse(pagesFrom)
            End If
            If (String.IsNullOrEmpty(pagesTo) = False) Then
                File.PagesTo = Integer.Parse(pagesTo)
            End If

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