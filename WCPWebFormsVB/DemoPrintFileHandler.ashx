<%@ WebHandler Language="VB" Class="DemoPrintFileHandler" %>

Imports System
Imports System.Web

Imports Neodynamic.SDK.Web

Public Class DemoPrintFileHandler : Implements IHttpHandler


    '############### IMPORTANT!!! ############
    ' If your website requires AUTHENTICATION, then you MUST configure THIS Handler file
    ' to be ANONYMOUS access allowed!!!
    '######################################### 


    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        If WebClientPrint.ProcessPrintJob(context.Request.Url.Query) Then
            Dim useDefaultPrinter As Boolean = (context.Request("useDefaultPrinter") = "checked")
            Dim printerName As String = context.Server.UrlDecode(context.Request("printerName"))

            Dim fileType As String = context.Request("filetype")

            Dim fileName As String = (Guid.NewGuid().ToString("N") & ".") + fileType
            Dim filePath As String = Nothing
            Select Case fileType
                Case "PDF"
                    filePath = "~/files/LoremIpsum.pdf"
                    Exit Select
                Case "TXT"
                    filePath = "~/files/LoremIpsum.txt"
                    Exit Select
                Case "DOC"
                    filePath = "~/files/LoremIpsum.doc"
                    Exit Select
                Case "XLS"
                    filePath = "~/files/SampleSheet.xls"
                    Exit Select
                Case "JPG"
                    filePath = "~/files/penguins300dpi.jpg"
                    Exit Select
                Case "PNG"
                    filePath = "~/files/SamplePngImage.png"
                    Exit Select
                Case "TIF"
                    filePath = "~/files/patent2pages.tif"
                    Exit Select
            End Select

            If filePath IsNot Nothing Then
                Dim file As PrintFile
                If (fileType = "PDF") Then
                    file = New PrintFilePDF(context.Server.MapPath(filePath), fileName)
                    DirectCast(file, PrintFilePDF).PrintRotation = PrintRotation.None
                    'DirectCast(file, PrintFilePDF).PagesRange = "1,2,3,10-15"
                    'DirectCast(file, PrintFilePDF).PrintAnnotations = True
                    'DirectCast(file, PrintFilePDF).PrintAsGrayscale = True
                    'DirectCast(file, PrintFilePDF).PrintInReverseOrder = True
                ElseIf (fileType = "TXT") Then
                    file = New PrintFileTXT(context.Server.MapPath(filePath), fileName)
                    DirectCast(file, PrintFileTXT).PrintOrientation = PrintOrientation.Portrait
                    DirectCast(file, PrintFileTXT).FontName = "Arial"
                    DirectCast(file, PrintFileTXT).FontSizeInPoints = 12 ' Point Unit!!!
                    'DirectCast(file, PrintFileTXT).TextColor = "#ff00ff"
                    'DirectCast(file, PrintFileTXT).TextAlignment = TextAlignment.Center
                    'DirectCast(file, PrintFileTXT).FontBold = True
                    'DirectCast(file, PrintFileTXT).FontItalic = True
                    'DirectCast(file, PrintFileTXT).FontUnderline = True
                    'DirectCast(file, PrintFileTXT).FontStrikeThrough = True
                    'DirectCast(file, PrintFileTXT).MarginLeft = 1 ' INCH Unit!!!
                    'DirectCast(file, PrintFileTXT).MarginTop = 1 ' INCH Unit!!!
                    'DirectCast(file, PrintFileTXT).MarginRight = 1 ' INCH Unit!!!
                    'DirectCast(file, PrintFileTXT).MarginBottom = 1 ' INCH Unit!!!
                Else
                    file = New PrintFile(context.Server.MapPath(filePath), fileName)
                End If

                Dim cpj As New ClientPrintJob()
                cpj.PrintFile = file
                If useDefaultPrinter OrElse printerName = "null" Then
                    cpj.ClientPrinter = New DefaultPrinter()
                Else
                    cpj.ClientPrinter = New InstalledPrinter(printerName)
                End If

                context.Response.ContentType = "application/octet-stream"
                context.Response.BinaryWrite(cpj.GetContent())
                context.Response.End()


            End If
        End If

    End Sub


    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class