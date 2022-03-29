<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SaveClientInvoiceReport.aspx.cs" Inherits="Nirast.Pcms.Web.Reports.SaveClientInvoiceReport" %>



<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<link href="../css/bootstrap.css" rel="stylesheet" />
<link href="../css/CustomStyles.css" rel="stylesheet" />
<script src="../js/CustomScript.js"></script>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">




    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript" src="~/Scripts/jquery.validate.unobtrusive.min.js"></script>
    <%--<script type="text/javascript">
    var spinnerVisible = false;
    function showProgress() {
        
        if (!spinnerVisible) {
            $("div#spinner").fadeIn("fast");
            spinnerVisible = true;
        }
    };
    function FhideProgress() {
     
        if (spinnerVisible) {
            var spinner = $("div#spinner");
            spinner.stop();
            spinner.fadeOut("fast");
            spinnerVisible = false;
        }
    };
</script>--%>
    <script src="../js/CustomScript.js"></script>

    <script type="text/javascript">
        $(document).ready(function () {
            debugger;
            if ($('#lblmessage').is(":visible")) {
                $('#div1').hide();
            } else {
                $('#div1').show();
            }
            //if ($('#lblmessage').v) {
            //    $('#div1').hide();

            //}
            //else {
            //    $('#div1').show();
            //}
        })
        function onLoadHandler() {
            alert('loaded');
        }
    </script>


    <title></title>

</head>
<body onload="hideProgress();">
    <div style="width: auto;">
        <form id="form1" runat="server">
            <div style="padding-bottom: 10px; padding-left: 700px; padding-top: 5px;">
                <asp:Button ID="Button2" runat="server" Text="Generate Invoice" OnClick="Button2_Click" OnClientClick="showProgress()" class="btn btn-primary " />
            </div>
            <div id="div1" runat="server" class="col-md-12">
                <label class=" label-style no-padding">Email ID</label>
                <asp:TextBox ID="emailTxt" class="labelStyle" Style="width: 300px;" runat="server"></asp:TextBox>
                <asp:Button ID="btnSend" runat="server" Text="Send" OnClick="Btnsend_Click" ValidationGroup="vgSubmit" OnClientClick="showProgress()" class="btn btn-primary " />
                <div>
                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server"
                        ErrorMessage="*Required" ControlToValidate="emailTxt"
                        ValidationGroup="vgSubmit" ForeColor="Red"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2"
                        runat="server" ErrorMessage="Please Enter Valid Email ID"
                        ValidationGroup="vgSubmit" ControlToValidate="emailTxt"
                        CssClass="requiredFieldValidateStyle"
                        ForeColor="Red"
                        ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*">
                    </asp:RegularExpressionValidator>
                </div>
            </div>

            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

            <div class="" style="padding-bottom: 15px;" id="divNotSeperate" runat="server">
                <div class="row">
                    <div class="equal-height-panels">
                        <div class="ui segment">
                            <div id="dvMain" name="dvMain">
                                <div class="clr"></div>
                                <div class="col-md-12">
                                    <div class="signature-pad col-md-12">

                                        <div class="panel panel-group panel-success">
                                            <div class="panel-heading">
                                                <h5>Default Invoice</h5>
                                            </div>
                                            <div class="panel-body">
                                                <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%" Height="900px" AsyncRendering="False"></rsweb:ReportViewer>
                                                <asp:Label ID="lblmessage" runat="server" Visible="false">No records to show!</asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </div>


            <div class="" style="padding-bottom: 15px;" id="divSeperate" runat="server">

                <div class="row">
                    <div class="equal-height-panels">
                        <div class="ui segment">
                            <div id="dvMain" name="dvMain">
                                <div class="clr"></div>
                                <div class="col-md-12">
                                    <div class="signature-pad col-md-12">

                                        <div class="panel panel-group panel-success">
                                            <div class="panel-heading">
                                                <h5>Seperate Invoice</h5>
                                            </div>
                                            <div class="panel-body">
                                                <rsweb:ReportViewer ID="ReportViewer2" runat="server" Width="100%" Height="900px" AsyncRendering="False"></rsweb:ReportViewer>
                                                <asp:Label ID="lblmessagesplit" runat="server" Visible="false">No records to show!</asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="" style="padding-bottom: 15px;" id="divShowpdf" runat="server" visible="false">

                <div class="row">
                    <div class="equal-height-panels">
                        <div class="ui segment">
                            <div id="dvMains" name="dvMains">
                                <div class="clr"></div>
                                <div class="col-md-12">
                                    <div class="signature-pad col-md-12">

                                        <div class="panel panel-group panel-success">
                                            <div class="panel-heading">
                                                <h5>Invoice</h5>
                                            </div>
                                            <div class="panel-body">
                                                <iframe id="ifrmpdfshow" runat="server" style="width: 100%; height: 900px"></iframe>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </div>


            <div id="spinner" runat="server">
            </div>


        </form>
    </div>
</body>
</html>
