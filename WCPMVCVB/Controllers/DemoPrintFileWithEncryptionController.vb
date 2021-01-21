Imports System.Web.Mvc

Imports Neodynamic.SDK.Web

Namespace Controllers
    Public Class DemoPrintFileWithEncryptionController
        Inherits Controller

        ' GET: DemoPrintFileWithEncryption
        Function Index() As ActionResult

            ViewData("WCPScript") = Neodynamic.SDK.Web.WebClientPrint.CreateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", Nothing, HttpContext.Request.Url.Scheme), Url.Action("PrintFile", "DemoPrintFileWithEncryption", Nothing, HttpContext.Request.Url.Scheme), HttpContext.Session.SessionID)

            Return View()
        End Function

        <AllowAnonymous> Public Sub PrintFile(useDefaultPrinter As String, printerName As String, fileType As String, wcp_pub_key_base64 As String, wcp_pub_key_signature_base64 As String)

            Dim fileName As String = Guid.NewGuid().ToString("N") + "." + fileType
            Dim filePath As String = Nothing
            Select Case fileType

                Case "PDF"
                    filePath = "~/files/LoremIpsum.pdf"

                Case "TXT"
                    filePath = "~/files/LoremIpsum.txt"

                Case "JPG"
                    filePath = "~/files/penguins300dpi.jpg"

                Case "PNG"
                    filePath = "~/files/SamplePngImage.png"

            End Select


            If (filePath <> Nothing AndAlso String.IsNullOrEmpty(wcp_pub_key_base64) = False) Then

                'create print file to be encrypted
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

                'create an encryption metadata to set to the PrintFile
                Dim encMetadata As New EncryptMetadata(wcp_pub_key_base64, wcp_pub_key_signature_base64)

                'set encyption metadata to PrintFile to ENCRYPT the Password to unlock the file
                file.EncryptMetadata = encMetadata

                'create ClientPrintJob for printing encrypted file
                Dim cpj As New ClientPrintJob()
                    cpj.PrintFile = file
                    If (useDefaultPrinter = "checked" OrElse printerName = "null") Then
                        cpj.ClientPrinter = New DefaultPrinter()
                    Else
                        cpj.ClientPrinter = New InstalledPrinter(System.Web.HttpUtility.UrlDecode(printerName))
                    End If

                System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream"

                'set the ClientPrintJob content
                System.Web.HttpContext.Current.Response.BinaryWrite(cpj.GetContent())
                'set the Encryption Metadata
                System.Web.HttpContext.Current.Response.Cookies.Add(New HttpCookie("wcp_enc_metadata", encMetadata.Serialize()))

                System.Web.HttpContext.Current.Response.End()



            End If

        End Sub
    End Class
End Namespace