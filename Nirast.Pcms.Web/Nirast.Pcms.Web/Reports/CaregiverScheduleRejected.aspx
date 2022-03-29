<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CaregiverScheduleRejected.aspx.cs" Inherits="Nirast.Pcms.Web.Reports.CaregiverScheduleRejected" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        table {
            width: 100%;
            height: 100%
        }

        #ReportViewer1_fixedTable > tbody > tr > td > div {
            overflow: scroll !important;
        }
    </style>

</head>
<body>
    <div style="width: auto; height: 700px;">
        <form id="form1" runat="server">

            <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="false"></asp:ScriptManager>
            <rsweb:ReportViewer ID="scheduleRejectedReport" runat="server" Width="100%" Height="900px" AsyncRendering="False" KeepSessionAlive="false"></rsweb:ReportViewer>
            <asp:Label ID="lblmessage" runat="server" Visible="false">No records to show!</asp:Label>

        </form>
    </div>
</body>
</html>

