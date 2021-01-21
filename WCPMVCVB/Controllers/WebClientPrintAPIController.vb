Imports System.Web.Mvc

Imports Neodynamic.SDK.Web

Namespace Controllers
    Public Class WebClientPrintAPIController
        Inherits Controller

        ' GET: WebClientPrintAPI
        Function Index() As ActionResult
            Return View()
        End Function

        '*********************************
        ' IMPORTANT NOTE 
        ' In this sample we store users related stuff (like
        ' the list of printers and whether they have the WCPP 
        ' client utility installed) in the Application cache
        ' object part of ASP.NET BUT you can change it to 
        ' another different storage (like a DB or file server)!
        ' which will be required in Load Balacing scenarios
        '*********************************


        <AllowAnonymous>
        Public Sub ProcessRequest()
            'get session ID
            Dim sessionID As String = (If(HttpContext.Request("sid") IsNot Nothing, HttpContext.Request("sid"), Nothing))

            'get Query String
            Dim queryString As String = HttpContext.Request.Url.Query

            Try
                'Determine and get the Type of Request 
                Dim prType As RequestType = WebClientPrint.GetProcessRequestType(queryString)

                If prType = RequestType.GenPrintScript OrElse prType = RequestType.GenWcppDetectScript Then
                    'Let WebClientPrint to generate the requested script
                    Dim script As Byte() = WebClientPrint.GenerateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", Nothing, HttpContext.Request.Url.Scheme), queryString)

                    HttpContext.Response.ContentType = "text/javascript"
                    HttpContext.Response.BinaryWrite(script)
                    HttpContext.Response.End()

                ElseIf prType = RequestType.ClientSetWcppVersion Then
                    'This request is a ping from the WCPP utility
                    'so store the session ID indicating it has the WCPP installed
                    'also store the WCPP Version if available
                    Dim wcppVersion As String = HttpContext.Request("wcppVer")
                    If String.IsNullOrEmpty(wcppVersion) Then
                        wcppVersion = "1.0.0.0"
                    End If

                    HttpContext.Application.Set(sessionID & "wcppInstalled", wcppVersion)

                ElseIf prType = RequestType.ClientSetInstalledPrinters Then
                    'WCPP Utility is sending the installed printers at client side
                    'so store this info with the specified session ID
                    Dim printers As String = HttpContext.Request("printers")
                    If String.IsNullOrEmpty(printers) = False Then
                        printers = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(printers))
                    End If


                    HttpContext.Application.Set(sessionID & "printers", printers)

                ElseIf prType = RequestType.ClientSetInstalledPrintersInfo Then
                    'WCPP Utility is sending the installed printers at client side
                    'so store this info with the specified session ID
                    'Printers Info is in JSON format
                    Dim printersInfo As String = Httpcontext.Request.Params("printersInfoContent")
                    If Not String.IsNullOrEmpty(printersInfo) Then
                        printersInfo = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(printersInfo))
                    End If

                    HttpContext.Application.Set(sessionID & "printersInfo", printersInfo)


                ElseIf prType = RequestType.ClientGetWcppVersion Then
                    'return the WCPP version for the specified sid if any
                    Dim sidWcppVersion As Boolean = (HttpContext.Application(sessionID & "wcppInstalled") IsNot Nothing)

                    HttpContext.Response.ContentType = "text/plain"
                    If (sidWcppVersion) Then
                        HttpContext.Response.Write(HttpContext.Application(sessionID & "wcppInstalled").ToString())
                    End If
                    HttpContext.Response.End()

                ElseIf prType = RequestType.ClientGetInstalledPrinters Then
                    'return the installed printers for the specified sid if any
                    Dim sidHasPrinters As Boolean = (HttpContext.Application(sessionID & "printers") IsNot Nothing)

                    HttpContext.Response.ContentType = "text/plain"
                    If (sidHasPrinters) Then
                        HttpContext.Response.Write(HttpContext.Application(sessionID & "printers").ToString())
                    End If
                    HttpContext.Response.End()

                ElseIf prType = RequestType.ClientGetInstalledPrintersInfo Then
                    'return the installed printers with detailed info for the specified Session ID (sid) if any
                    Dim sidHasPrinters As Boolean = (HttpContext.Application(sessionID & "printersInfo") IsNot Nothing)

                    HttpContext.Response.ContentType = "text/plain"

                    If (sidHasPrinters) Then
                        HttpContext.Response.Write(HttpContext.Application(sessionID & "printersInfo").ToString())
                    End If
                    HttpContext.Response.End()
                End If
            Catch ex As Exception
                HttpContext.Response.StatusCode = 500
                HttpContext.Response.ContentType = "text/plain"
                HttpContext.Response.Write(ex.Message + " - StackTrace: " + ex.StackTrace)
                HttpContext.Response.End()
            End Try


        End Sub

    End Class
End Namespace