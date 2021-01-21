﻿<%@ Page Title="Advanced TIF Printing" Language="C#" MasterPageFile="~/MasterPage.master" %>


<script runat="server">


</script>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .glyphicon-refresh-animate {
            -animation: spin .7s infinite linear;
            -webkit-animation: spin2 .7s infinite linear;
        }

        @-webkit-keyframes spin2 {
            from {
                -webkit-transform: rotate(0deg);
            }

            to {
                -webkit-transform: rotate(360deg);
            }
        }

        @keyframes spin {
            from {
                transform: scale(1) rotate(0deg);
            }

            to {
                transform: scale(1) rotate(360deg);
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">

    <div class="container">
        <div class="row">

            <div class="col-md-12">
                <h3><a href="Samples.aspx" class="btn btn-md btn-danger"><i class="fa fa-chevron-left"></i></a>&nbsp;Advanced TIF Printing</h3>
                <p>
                    With <strong>WebClientPrint for ASP.NET</strong> solution you can <strong>print TIF files</strong> right to any installed printer at the client side with advanced settings.
                </p>

                <div class="form-group well">
                    <h4>Click on <strong>"Get Printers Info"</strong> button to get Printers Name, Supported Papers and Trays</h4>
                    <div class="row">

                        <div class="col-md-3">
                            <a onclick="javascript:jsWebClientPrint.getPrintersInfo(); $('#spinner').css('visibility', 'visible');" class="btn btn-success">Get Printers Info...</a>
                        </div>
                        <div class="col-md-9">
                            <h3 id="spinner" style="visibility: hidden"><span class="label label-info"><span class="glyphicon glyphicon-refresh glyphicon-refresh-animate"></span>Please wait a few seconds...</span></h3>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-3">
                            <label for="lstPrinters">Printers:</label>
                            <select name="lstPrinters" id="lstPrinters" onchange="showSelectedPrinterInfo();" class="form-control"></select>
                        </div>
                        <div class="col-md-3">
                            <label for="lstPrinterTrays">Supported Trays:</label>
                            <select name="lstPrinterTrays" id="lstPrinterTrays" class="form-control"></select>
                        </div>
                        <div class="col-md-3">
                            <label for="lstPrinterPapers">Supported Papers:</label>
                            <select name="lstPrinterPapers" id="lstPrinterPapers" class="form-control"></select>
                        </div>
                        <div class="col-md-3">
                            <label for="lstPrintRotation">Print Rotation (Clockwise):</label>
                            <select name="lstPrintRotation" id="lstPrintRotation" class="form-control">
                                <option>None</option>
                                <option>Rot90</option>
                                <option>Rot180</option>
                                <option>Rot270</option>
                            </select>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-3">
                            <label for="txtPagesRange">Pages Range: [e.g. 1,2,3,10-13]</label>
                            <input type="text" class="form-control" id="txtPagesRange">
                        </div>
                        <div class="col-md-3">
                              <label><input id="chkPrintInReverseOrder" type="checkbox" value="">Print In Reverse Order?</label>
                            
                        </div>
                        <div class="col-md-3">
                            <div class="checkbox">
                              <label id="lblDriverDuplexPrinting"><input id="chkDriverDuplexPrinting" type="checkbox" value="">Use Driver Duplex Printing?</label>
                            </div>
                            <div class="checkbox">
                              <label><input id="chkManualDuplexPrinting" type="checkbox" value="">Use Manual Duplex Printing?</label>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="checkbox">
                              <label><input id="chkPrintAsGrayscale" type="checkbox" value="">Print As Grayscale?</label>
                            </div>
                        </div>
                        
                    </div>
                    <div class="row">
                        <div class="col-md-3">
                            <label for="lstPrintRotation">Page Sizing:</label>
                            <select name="lstPrintRotation" id="lstPageSizing" class="form-control">
                                <option>None</option>
                                <option>Fit</option>
                            </select>
                        </div>
                        <div class="col-md-3">
                            <div class="checkbox">
                              <label><input id="chkAutoCenter" type="checkbox" value="">Auto Center?</label>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="checkbox">
                              <label><input id="chkAutoRotate" type="checkbox" value="">Auto Rotate?</label>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        
                        <div class="col-md-12">
                             <a class="btn btn-success btn-lg pull-right" onclick="javascript:jsWebClientPrint.print('printerName=' + encodeURIComponent($('#lstPrinters').val()) + '&trayName=' + encodeURIComponent($('#lstPrinterTrays').val()) + '&paperName=' + encodeURIComponent($('#lstPrinterPapers').val()) + '&printRotation=' + $('#lstPrintRotation').val() + '&pagesRange=' + encodeURIComponent($('#txtPagesRange').val()) + '&printAsGrayscale=' + $('#chkPrintAsGrayscale').prop('checked') + '&printInReverseOrder=' + $('#chkPrintInReverseOrder').prop('checked') + '&manualDuplexPrinting=' + $('#chkManualDuplexPrinting').prop('checked') + '&driverDuplexPrinting=' + ($('#chkDriverDuplexPrinting').prop('disabled') ? 'false' : $('#chkDriverDuplexPrinting').prop('checked')) + '&pageSizing=' + $('#lstPageSizing').val() + '&autoRotate=' + ($('#chkAutoRotate').prop('disabled') ? 'false' : $('#chkAutoRotate').prop('checked')) + '&autoCenter=' + ($('#chkAutoCenter').prop('disabled') ? 'false' : $('#chkAutoCenter').prop('checked')));"><strong>Print TIF...</strong></a>
                        </div>
                    </div>
                    <hr />
                    <h4>TIF File Sample Preview - <strong>2 Pages</strong></h4>
                    <iframe id="ifPreview" style="width: 100%; height: 500px;" frameborder="0" src="//docs.google.com/gview?url=http://webclientprint.azurewebsites.net/files/patent2pages.tif&embedded=true"></iframe>

                </div>

            </div>


        </div>
    </div>


    <script type="text/javascript">

        var clientPrinters = null;

        var wcppGetPrintersTimeout_ms = 60000; //60 sec
        var wcppGetPrintersTimeoutStep_ms = 500; //0.5 sec
        function wcpGetPrintersOnSuccess() {
            $('#spinner').css('visibility', 'hidden');
            // Display client installed printers
            if (arguments[0].length > 0) {
                if (JSON) {
                    try {
                        clientPrinters = JSON.parse(arguments[0]);
                        if (clientPrinters.error) {
                            alert(clientPrinters.error)
                        } else {
                            var options = '';
                            for (var i = 0; i < clientPrinters.length; i++) {
                                options += '<option>' + clientPrinters[i].name + '</option>';
                            }
                            $('#lstPrinters').html(options);
                            $('#lstPrinters').focus();

                            showSelectedPrinterInfo();
                        }
                    } catch (e) {
                        alert(e.message)
                    }
                }


            } else {
                alert("No printers are installed in your system.");
            }
        }
        function wcpGetPrintersOnFailure() {
            $('#spinner').css('visibility', 'hidden');
            // Do something if printers cannot be got from the client
            alert("No printers are installed in your system.");
        }


        function showSelectedPrinterInfo() {
            // get selected printer index
            var idx = $("#lstPrinters")[0].selectedIndex;
            // get supported trays
            var options = '';
            if (clientPrinters[idx].trays) {
                for (var i = 0; i < clientPrinters[idx].trays.length; i++) {
                    options += '<option>' + clientPrinters[idx].trays[i] + '</option>';
                }
            }
            $('#lstPrinterTrays').html(options);
            // get supported papers
            options = '';
            if (clientPrinters[idx].papers) {
                for (var i = 0; i < clientPrinters[idx].papers.length; i++) {
                    options += '<option>' + clientPrinters[idx].papers[i] + '</option>';
                }
            }
            $('#lstPrinterPapers').html(options);

            // update duplex option
            $('#chkDriverDuplexPrinting').attr('checked', clientPrinters[idx].duplex);
            $('#chkDriverDuplexPrinting').attr('disabled', !clientPrinters[idx].duplex);
            $('#lblDriverDuplexPrinting').attr('style', clientPrinters[idx].duplex ? '' : 'text-decoration: line-through;');

        }

    </script>


    
        
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderScripts" runat="Server">

    <%-- Register the WebClientPrint script code --%>
    <%=Neodynamic.SDK.Web.WebClientPrint.CreateScript(HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + ":" + HttpContext.Current.Request.Url.Port + "/WebClientPrintAPI.ashx", HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + ":" + HttpContext.Current.Request.Url.Port + "/DemoPrintFileTIFHandler.ashx", HttpContext.Current.Session.SessionID)%>
</asp:Content>

