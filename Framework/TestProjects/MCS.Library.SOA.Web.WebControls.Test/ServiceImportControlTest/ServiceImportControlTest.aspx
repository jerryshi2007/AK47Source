<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServiceImportControlTest.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.ServiceImportControlTest.ServiceImportControlTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function showDialog() {
            var returnValue = $find("ServiceImport1").showDialog();
            alert(returnValue);
        }
    
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <HB:ServiceImport runat="server" ID="ServiceImport1" />
    </div>
    
    <div>
        <input type="button" id="button" value="showDialog" onclick="showDialog();" />
    </div>

    </form>
</body>
</html>
