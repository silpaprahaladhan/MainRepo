<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UpdateUserInvoice.aspx.cs" Inherits="Nirast.Pcms.Web.Reports.UpdateUserInvoice" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<link href="../css/bootstrap.css" rel="stylesheet" />
<link href="../css/CustomStyles.css" rel="stylesheet" />
<script src="../js/CustomScript.js"></script>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    



    
<script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
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
function onLoadHandler() {
    alert('loaded');
}
</script>


    <title></title>

</head>
<body  onload="hideProgress();">
    <div style="width:auto;">
    <form id="form1" runat="server" >
        <div style="padding-bottom: 10px;padding-left: 700px;padding-top: 5px;">
     
            </div>
               <div id="div1" runat="server" class="col-md-12">
                <label class=" label-style no-padding">Email ID</label><span class="mandatoryField">*</span>               
                <asp:TextBox ID="emailTxt" class="labelStyle" style="width:300px;" runat="server"></asp:TextBox>
&nbsp;<span class="col-md-12 no-padding labelStyle info" title="">Report will be sent to the above Email ID</span>
                <span class="field-validation-valid text-danger required" data-valmsg-for="EmailAddress" data-valmsg-replace="true"></span>
                <span id="result"></span>
                <asp:Button ID="btnSend" runat="server" Text="Send" 	OnClick="Btnsend_Click" OnClientClick="showProgress()" class="btn btn-primary " />
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
                                            <div class="panel-heading"><h5>Default Invoice</h5></div>
                                            <div class="panel-body" >
                                                  <rsweb:ReportViewer ID="ReportViewer1"  runat="server" Width="100%"  Height="900px"  AsyncRendering="False" > </rsweb:ReportViewer>
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
        
          
             

            <div class="" style="padding-bottom: 15px;" id="divShowpdf" runat="server" visible="false">

                <div class="row">
                    <div class="equal-height-panels">
                        <div class="ui segment">
                            <div id="dvMains" name="dvMains">
                                <div class="clr"></div>
                                <div class="col-md-12">
                                    <div class="signature-pad col-md-12">

                                        <div class="panel panel-group panel-success">
                                            <div class="panel-heading"><h5>Invoice</h5></div>
                                            <div class="panel-body" >
                                                <iframe id="ifrmpdfshow" runat="server" style="width: 100%; height: 900px" ></iframe>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </div>
       
        
        <div id="spinner" runat="server" >
                     
                    </div>


  
        
    </form>

      </div>
    
</body>
</html>

