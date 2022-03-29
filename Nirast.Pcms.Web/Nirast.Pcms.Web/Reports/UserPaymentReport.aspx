<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserPaymentReport.aspx.cs" Inherits="Nirast.Pcms.Web.Reports.UserPaymentReports" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
        <style>
         table {
        width:100%;
        height:100%
        }
         
    </style>

    
</head>
<body><div style="width:auto;">
    <form id="form1" runat="server" >
        
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <rsweb:ReportViewer ID="ReportViewer1"  runat="server" Width="100%" Height="100%" AsyncRendering="False" SizeToReportContent="True"> </rsweb:ReportViewer>
        <asp:Label ID="lblmessage" runat="server" Visible="false">No records to show!</asp:Label>
    </form></div>
</body>
</html>