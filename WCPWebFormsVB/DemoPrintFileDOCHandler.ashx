<%@ WebHandler Language="VB" Class="DemoPrintFileDOCHandler" %>

Imports System
Imports System.Web

Imports Neodynamic.SDK.Web

Public Class DemoPrintFileDOCHandler : Implements IHttpHandler


    '############### IMPORTANT!!! ############
    ' If your website requires AUTHENTICATION, then you MUST configure THIS Handler file
    ' to be ANONYMOUS access allowed!!!
    '######################################### 


    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        If WebClientPrint.ProcessPrintJob(context.Request.Url.Query) Then

            Dim printerName As String = context.Server.UrlDecode(context.Request("printerName"))

            Dim pagesRange As String = context.Server.UrlDecode(context.Request("pagesRange"))

            Dim printInReverseOrder As Boolean = (context.Request("printInReverseOrder") = "true")
            Dim duplexPrinting As Boolean = (context.Request("duplexPrinting") = "true")

            Dim fileName As String = Guid.NewGuid().ToString("N")
            Dim filePath As String = "~/files/Sample-Employee-Handbook.doc"

            Dim file As New PrintFileDOC(context.Server.MapPath(filePath), fileName)
            file.PagesRange = pagesRange
            file.PrintInReverseOrder = printInReverseOrder
            file.DuplexPrinting = duplexPrinting
            'file.DuplexPrintingDialogMessage = "Your custom dialog message for duplex printing"


            Dim cpj As New ClientPrintJob()
            cpj.PrintFile = file
            If printerName = "null" Then
                cpj.ClientPrinter = New DefaultPrinter()
            Else
                cpj.ClientPrinter = New InstalledPrinter(printerName)
            End If

            context.Response.ContentType = "application/octet-stream"
            context.Response.BinaryWrite(cpj.GetContent())
            context.Response.End()


        End If

    End Sub


    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class