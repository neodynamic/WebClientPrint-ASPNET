<%@ WebHandler Language="VB" Class="WebClientPrintAPI" %>

Imports System
Imports System.Web

Imports Neodynamic.SDK.Web


Public Class WebClientPrintAPI : Implements IHttpHandler

    '*********************************
    ' IMPORTANT NOTE 
    ' In this sample we store users related stuff (like
    ' the list of printers and whether they have the WCPP 
    ' client utility installed) in the Application cache
    ' object part of ASP.NET BUT you can change it to 
    ' another different storage (like a DB or file server)!
    ' which will be required in Load Balacing scenarios
    '*********************************

    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        'get session ID
        Dim sessionID As String = ""
        If (context.Request("sid") IsNot Nothing) Then
            sessionID = context.Request("sid")
        End If

        'get Query String
        Dim queryString As String = context.Request.Url.Query

        Try
            'Determine and get the Type of Request 
            Dim prType As RequestType = WebClientPrint.GetProcessRequestType(queryString)

            If prType = RequestType.GenPrintScript OrElse prType = RequestType.GenWcppDetectScript Then
                'Let WebClientPrint to generate the requested script
                Dim script As Byte() = WebClientPrint.GenerateScript(context.Request.Url.AbsoluteUri.Replace(queryString, ""), queryString)

                context.Response.ContentType = "text/javascript"
                context.Response.BinaryWrite(script)
            ElseIf prType = RequestType.ClientSetWcppVersion Then
                'This request is a ping from the WCPP utility
                'so store the session ID indicating this user has the WCPP installed
                'also store the WCPP Version if available
                Dim wcppVersion As String = context.Request("wcppVer")
                If String.IsNullOrEmpty(wcppVersion) Then
                    wcppVersion = "1.0.0.0"
                End If

                context.Application.Set(sessionID & "wcppInstalled", wcppVersion)
            ElseIf prType = RequestType.ClientSetInstalledPrinters Then
                'WCPP Utility is sending the installed printers at client side
                'so store this info with the specified session ID
                Dim printers As String = context.Request("printers")
                If Not String.IsNullOrEmpty(printers) Then
                    printers = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(printers))
                End If

                context.Application.Set(sessionID & "printers", printers)

            ElseIf prType = RequestType.ClientSetInstalledPrintersInfo Then
                'WCPP Utility is sending the installed printers at client side
                'so store this info with the specified session ID
                'Printers Info is in JSON format
                Dim printersInfo As String = context.Request.Params("printersInfoContent")
                If Not String.IsNullOrEmpty(printersInfo) Then
                    printersInfo = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(printersInfo))
                End If

                context.Application.Set(sessionID & "printersInfo", printersInfo)

            ElseIf prType = RequestType.ClientGetWcppVersion Then
                'return the WCPP version for the specified Session ID (sid) if any
                Dim sidWcppVersion As Boolean = (context.Application(sessionID & "wcppInstalled") IsNot Nothing)

                context.Response.ContentType = "text/plain"
                If (sidWcppVersion) Then
                    context.Response.Write(context.Application(sessionID & "wcppInstalled").ToString())
                End If

            ElseIf prType = RequestType.ClientGetInstalledPrinters Then
                'return the installed printers for the specified Session ID (sid) if any
                Dim sidHasPrinters As Boolean = (context.Application(sessionID & "printers") IsNot Nothing)

                context.Response.ContentType = "text/plain"

                If (sidHasPrinters) Then
                    context.Response.Write(context.Application(sessionID & "printers").ToString())
                End If

            ElseIf prType = RequestType.ClientGetInstalledPrintersInfo Then
                'return the installed printers with detailed info for the specified Session ID (sid) if any
                Dim sidHasPrinters As Boolean = (context.Application(sessionID & "printersInfo") IsNot Nothing)

                context.Response.ContentType = "text/plain"

                If (sidHasPrinters) Then
                    context.Response.Write(context.Application(sessionID & "printersInfo").ToString())
                End If
            End If
        Catch ex As Exception
            context.Response.StatusCode = 500
            context.Response.ContentType = "text/plain"
            context.Response.Write(ex.Message + " - " + ex.StackTrace)
        End Try



    End Sub


    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class