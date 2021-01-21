Public Class HomeController
    Inherits System.Web.Mvc.Controller
    Function Index() As ActionResult
        ViewData("WCPPDetectionScript") = Neodynamic.SDK.Web.WebClientPrint.CreateWcppDetectionScript(Url.Action("ProcessRequest", "WebClientPrintAPI", Nothing, HttpContext.Request.Url.Scheme), HttpContext.Session.SessionID)

        Return View()
    End Function

    Function Samples() As ActionResult

        Return View()
    End Function

    Function PrintersInfo() As ActionResult
        ViewData("WCPScript") = Neodynamic.SDK.Web.WebClientPrint.CreateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", Nothing, HttpContext.Request.Url.Scheme), "", HttpContext.Session.SessionID)

        Return View()
    End Function
End Class
