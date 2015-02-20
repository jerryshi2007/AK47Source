<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintControlTest.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.Workflow.Print.PrintControlTest" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="MCS" %><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
        <link href="ss1.css" rel="stylesheet" type="text/css"/>

    <script type="text/javascript">
        function onPrint() {
            //alert("print !")
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="show"><span>asdadad</span></div>
    <div class="hide">noprint</div>

    <div>
        <input runat="server" id="print" value="test" type="button" />
        <MCS:PrintControl runat="server" OnPrint="onPrint" ID="printCtl" Visible="true" TargetControlID="print"></MCS:PrintControl>
    </div>
    </form>
</body>
</html>
