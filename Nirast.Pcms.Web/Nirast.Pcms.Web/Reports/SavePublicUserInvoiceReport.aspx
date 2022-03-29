<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SavePublicUserInvoiceReport.aspx.cs" Inherits="Nirast.Pcms.Web.Reports.SavePublicUserInvoiceReport" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../css/bootstrap.css" rel="stylesheet" />
<link href="../css/CustomStyles.css" rel="stylesheet" />
<script src="../js/CustomScript.js"></script>
    <title></title>
    <style type="text/css">
        .auto-style1 {
            position: relative;
            min-height: 1px;
            float: left;
            width: 100%;
            left: 0px;
            top: -11px;
            height: 115px;
            padding-left: 15px;
            padding-right: 15px;
        }
        .auto-style2 {
            position: relative;
            min-height: 1px;
            float: left;
            width: 100%;
            left: 0px;
            top: -14px;
            height: 1011px;
            padding-left: 15px;
            padding-right: 15px;
        }
        .auto-style3 {
            left: 0px;
            top: -2px;
            height: 116px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
      <div style="padding-bottom: 10px;padding-left: 700px;padding-top: 5px;">
     
            </div>
               <div id="div1" runat="server" class="col-md-12">
                <label class=" label-style no-padding">Email ID</label><span class="mandatoryField">*</span>               
                <asp:TextBox ID="emailTxt" class="labelStyle" style="width:300px;" runat="server"></asp:TextBox>

                <span class="field-validation-valid text-danger required" data-valmsg-for="EmailAddress" data-valmsg-replace="true"></span>
                <span id="result"></span>
                <asp:Button ID="btnSend" runat="server" Text="Send" OnClick="Btnsend_Click" OnClientClick="showProgress()" class="btn btn-primary " />
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


<%--
                    
        <div style="padding-top: 5px;" id="div1" runat="server" class="col-md-12">
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
         <div class="clr"></div>
       --%>
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

       
    </form>
</body>
   

  
</html>