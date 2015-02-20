<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CloneOfficeViewer.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.OfficeViewer.CloneOfficeViewer" %>

<%@ Register assembly="MCS.Library.SOA.Web.WebControls" namespace="MCS.Web.WebControls" tagprefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>CloneOfficeViewer</title>
    <script type="text/javascript">
        function onCloneComponent() {
            var parent = $get("resultDiv");
            var template = $find("OfficeViewerWrapper1");
            template.cloneAndAppendToContainer(parent);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <cc1:OfficeViewerWrapper ID="OfficeViewerWrapper1" runat="server" />

    </div>
     <div><input type="button" value="Clone"  onclick="onCloneComponent();"/></div>
    <div id="resultDiv">
        
    </div>
    </form>
</body>
</html>
