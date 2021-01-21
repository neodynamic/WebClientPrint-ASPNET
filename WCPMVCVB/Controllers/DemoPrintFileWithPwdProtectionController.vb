Imports System.Web.Mvc

Imports Neodynamic.SDK.Web

Namespace Controllers
    Public Class DemoPrintFileWithPwdProtectionController
        Inherits Controller

        ' GET: DemoPrintFileWithEncryption
        Function Index() As ActionResult

            ViewData("WCPScript") = Neodynamic.SDK.Web.WebClientPrint.CreateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", Nothing, HttpContext.Request.Url.Scheme), Url.Action("PrintFile", "DemoPrintFileWithPwdProtection", Nothing, HttpContext.Request.Url.Scheme), HttpContext.Session.SessionID)

            Return View()
        End Function

        <AllowAnonymous> Public Sub PrintFile(useDefaultPrinter As String, printerName As String, fileType As String, wcp_pub_key_base64 As String, wcp_pub_key_signature_base64 As String)

            Dim fileName As String = Guid.NewGuid().ToString("N") + "." + fileType
            Dim filePath As String = Nothing
            Select Case fileType
                Case "PDF"
                    filePath = "~/files/LoremIpsum-PasswordProtected.pdf"
                    Exit Select
                Case "DOC"
                    filePath = "~/files/LoremIpsum-PasswordProtected.doc"
                    Exit Select
                Case "XLS"
                    filePath = "~/files/SampleSheet-PasswordProtected.xls"
                    Exit Select

            End Select


            If (filePath <> Nothing AndAlso String.IsNullOrEmpty(wcp_pub_key_base64) = False) Then

                'ALL the test files are protected with the same password for demo purposes 
                'This password will be encrypted And stored in file metadata
                Dim plainTextPassword As String = "ABC123"

                    'create print file with password protection
                    Dim file As PrintFile
                    If (fileType = "PDF") Then
                        file = New PrintFilePDF(System.Web.HttpContext.Current.Server.MapPath(filePath), fileName)
                        DirectCast(file, PrintFilePDF).Password = plainTextPassword
                        'DirectCast(file, PrintFilePDF).PrintRotation = PrintRotation.None
                        'DirectCast(file, PrintFilePDF).PagesRange = "1,2,3,10-15"
                        'DirectCast(file, PrintFilePDF).PrintAnnotations = True
                        'DirectCast(file, PrintFilePDF).PrintAsGrayscale = True
                        'DirectCast(file, PrintFilePDF).PrintInReverseOrder = True
                    ElseIf (fileType = "DOC") Then
                        file = New PrintFileDOC(System.Web.HttpContext.Current.Server.MapPath(filePath), fileName)
                        DirectCast(file, PrintFileDOC).Password = plainTextPassword
                        'DirectCast(file, PrintFileDOC).PagesRange = "1,2,3,10-15"
                        'DirectCast(file, PrintFileDOC).PrintInReverseOrder = True
                    Else
                        file = New PrintFileXLS(System.Web.HttpContext.Current.Server.MapPath(filePath), fileName)
                        DirectCast(file, PrintFileXLS).Password = plainTextPassword
                        'DirectCast(file, PrintFileXLS).PagesFrom = 1
                        'DirectCast(file, PrintFileXLS).PagesTo = 5
                    End If

                'create an encryption metadata to set to the PrintFile
                Dim encMetadata As New EncryptMetadata(wcp_pub_key_base64, wcp_pub_key_signature_base64)

                'set encyption metadata to PrintFile 
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
                    System.Web.HttpContext.Current.Response.BinaryWrite(cpj.GetContent())
                    System.Web.HttpContext.Current.Response.End()



            End If

        End Sub
    End Class
End Namespace