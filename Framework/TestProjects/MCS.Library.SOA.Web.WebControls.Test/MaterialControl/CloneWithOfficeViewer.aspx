<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CloneWithOfficeViewer.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.CloneWithOfficeViewer" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>CloneMaterialControl Test</title>
    <script type="text/javascript">
        function onCloneComponent() {
            var parent = $get("resultDiv");
            var template = $find("MaterialControl1");
            template.cloneAndAppendToContainer(parent);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
       <SOA:MaterialControl ID="MaterialControl1" MaterialUseMode="SingleDraft" AutoOpenDocument="False" EditDocumentInCurrentPage="False"
        TemplateUrl="~/MaterialControl/Templates/Test.xlsx" RootPathName="GenericProcess" runat="server" />
    </div>
    <div><input type="button" value="Clone"  onclick="onCloneComponent();"/>
    </div>
    <div id="resultDiv"></div>
    </form>
</body>
</html>

