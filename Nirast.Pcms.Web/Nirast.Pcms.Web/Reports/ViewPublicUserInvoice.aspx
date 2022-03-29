<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewPublicUserInvoice.aspx.cs" Inherits="Nirast.Pcms.Web.Reports.ViewPublicUserInvoice" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
            <div class="" style="padding-bottom: 15px;" id="divShowpdf" runat="server" >

                <div class="row">
                    <div class="equal-height-panels">
                        <div class="ui segment">
                            <div id="dvMains" name="dvMains">
                                <div class="clr"></div>
                                <div class="col-md-12">
                                    <div class="signature-pad col-md-12">

                                        <div class="panel panel-group panel-success">
                                           
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
