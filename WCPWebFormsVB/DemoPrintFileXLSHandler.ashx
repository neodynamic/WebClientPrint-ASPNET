<%@ WebHandler Language="VB" Class="DemoPrintFileXLSHandler" %>

Imports System
Imports System.Web

Imports Neodynamic.SDK.Web

Public Class DemoPrintFileXLSHandler : Implements IHttpHandler


    '############### IMPORTANT!!! ############
    ' If your website requires AUTHENTICATION, then you MUST configure THIS Handler file
    ' to be ANONYMOUS access allowed!!!
    '######################################### 


    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        If WebClientPrint.ProcessPrintJob(context.Request.Url.Query) Then

            Dim printerName As String = context.Server.UrlDecode(context.Request("printerName"))

            Dim pagesFrom As String = context.Server.UrlDecode(context.Request("pagesFrom"))
            Dim pagesTo As String = context.Server.UrlDecode(context.Request("pagesTo"))

            Dim fileName As String = Guid.NewGuid().ToString("N")
            Dim filePath As String = "~/files/Project-Scheduling-Monitoring-Tool.xls"

            Dim file As New PrintFileXLS(context.Server.MapPath(filePath), fileName)
            If (String.IsNullOrEmpty(pagesFrom) = False) Then
                file.PagesFrom = Integer.Parse(pagesFrom)
            End If
            If (String.IsNullOrEmpty(pagesTo) = False) Then
                file.PagesTo = Integer.Parse(pagesTo)
            End If


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