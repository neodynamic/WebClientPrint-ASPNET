Imports System.Web.Mvc

Imports Neodynamic.SDK.Web

Namespace Controllers
    Public Class DemoPrintFileController
        Inherits Controller

        ' GET: DemoPrintFile
        Function Index() As ActionResult

            ViewData("WCPScript") = Neodynamic.SDK.Web.WebClientPrint.CreateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", Nothing, HttpContext.Request.Url.Scheme), Url.Action("PrintFile", "DemoPrintFile", Nothing, HttpContext.Request.Url.Scheme), HttpContext.Session.SessionID)

            Return View()
        End Function

        <AllowAnonymous> Public Sub PrintFile(useDefaultPrinter As String, printerName As String, fileType As String)

            Dim fileName As String = Guid.NewGuid().ToString("N") + "." + fileType
            Dim filePath As String = Nothing
            Select Case fileType

                Case "PDF"
                    filePath = "~/files/LoremIpsum.pdf"

                Case "TXT"
                    filePath = "~/files/LoremIpsum.txt"

                Case "DOC"
                    filePath = "~/files/LoremIpsum.doc"

                Case "XLS"
                    filePath = "~/files/SampleSheet.xls"

                Case "JPG"
                    filePath = "~/files/penguins300dpi.jpg"

                Case "PNG"
                    filePath = "~/files/SamplePngImage.png"

                Case "TIF"
                    filePath = "~/files/patent2pages.tif"

            End Select


            If (filePath <> Nothing) Then
                Dim file As PrintFile
                If (fileType = "PDF") Then
                    file = New PrintFilePDF(System.Web.HttpContext.Current.Server.MapPath(filePath), fileName)
                    DirectCast(file, PrintFilePDF).PrintRotation = PrintRotation.None
                    'DirectCast(file, PrintFilePDF).PagesRange = "1,2,3,10-15"
                    'DirectCast(file, PrintFilePDF).PrintAnnotations = True
                    'DirectCast(file, PrintFilePDF).PrintAsGrayscale = True
                    'DirectCast(file, PrintFilePDF).PrintInReverseOrder = True
                ElseIf (fileType = "TXT") Then
                    file = New PrintFileTXT(System.Web.HttpContext.Current.Server.MapPath(filePath), fileName)
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
                    file = New PrintFile(System.Web.HttpContext.Current.Server.MapPath(filePath), fileName)
                End If

                Dim cpj As New ClientPrintJob()
                cpj.PrintFile = file
                If (useDefaultPrinter = "checked" OrElse printerName = "null") Then
                    cpj.ClientPrinter = New DefaultPrinter()
                Else
                    cpj.ClientPrinter = New InstalledPrinter(System.Web.HttpUtility.UrlDecode(printerName))
                End If

                System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream"
                System.Web.HttpContext.Current.Response.BinaryWrite(cpj.GetContent())
                System.Web.HttpContext.Current.Response.End()

            End If


        End Sub
    End Class
End Namespace