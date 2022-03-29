<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientInvoiceReport.aspx.cs" Inherits="Nirast.Pcms.Web.Reports.ClientInvoiceReport" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<link href="../css/bootstrap.css" rel="stylesheet" />
<link href="../css/CustomStyles.css" rel="stylesheet" />
<script src="../js/CustomScript.js"></script>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">






    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript">
        var spinnerVisible = false;
        function showProgress() {

            if (!spinnerVisible) {
                $("div#spinner").fadeIn("fast");
                spinnerVisible = true;
            }
        };
        function hideProgress() {

            if (spinnerVisible) {
                var spinner = $("div#spinner");
                spinner.stop();
                spinner.fadeOut("fast");
                spinnerVisible = false;
            }
        };
    </script>

    <style>
        table {
            width: 100%;
            height: 100%
        }
    </style>
    <title></title>

</head>
<body onload="hideProgress();">
    <button class="tablink" onclick="openPage('1', this,'#2359a7')" id="defaultOpen">Default Invoice</button>
    <button class="tablink" onclick="openPage('2', this,'#2359a7')">Seperate Invoice</button>
    <div style="width: auto;">
        <form id="form1" runat="server">
            <div style="padding-bottom: 10px; padding-left: 700px; padding-top: 5px;">
                <asp:Button ID="Button2" runat="server" Text="Generate Invoice" OnClick="Button2_Click" class="btn btn-primary " Visible="false" />

            </div>


            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>



            <div id="1" class="tabcontent" style="padding-bottom: 15px;">
                <div class="row">
                    <div class="panel-body">
                        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%" Height="900px" AsyncRendering="False"></rsweb:ReportViewer>
                        <asp:Label ID="lblmessage" runat="server" style="color:black !important;" Visible="false">No records to show!</asp:Label>
                    </div>
                </div>
            </div>


            <div id="2" class="tabcontent" style="padding-bottom: 15px;">
                <div class="row">
                    <div class="equal-height-panels">
                        <div class="panel-body">
                            <rsweb:ReportViewer ID="ReportViewer2" runat="server" Width="100%" Height="900px" AsyncRendering="False"></rsweb:ReportViewer>
                            <asp:Label ID="lblmessagesplit" runat="server" style="color:black !important;"  Visible="false">No records to show!</asp:Label>
                        </div>
                    </div>
                </div>
            </div>


            <div id="spinner" runat="server">
            </div>




        </form>

    </div>

    <script>
        function openPage(pageName, elmnt, color) {
            var i, tabcontent, tablinks;
            tabcontent = document.getElementsByClassName("tabcontent");
            for (i = 0; i < tabcontent.length; i++) {
                tabcontent[i].style.display = "none";
            }
            tablinks = document.getElementsByClassName("tablink");
            for (i = 0; i < tablinks.length; i++) {
                tablinks[i].style.backgroundColor = "";
            }
            document.getElementById(pageName).style.display = "block";
            elmnt.style.backgroundColor = color;
        }

        // Get the element with id="defaultOpen" and click on it
        document.getElementById("defaultOpen").click();
    </script>
    <style>
        .tablink {
            background-color: #777;
            color: white;
            float: left;
            border: none;
            outline: none;
            cursor: pointer;
            padding: 14px 16px;
            font-size: 17px;
            width: 50%;
        }

        /* Style the tab content (and add height:100% for full page content) */
        .tabcontent {
            color: white;
            display: none;
            padding: 100px 20px;
            height: 100%;
        }
    </style>

</body>
</html>
