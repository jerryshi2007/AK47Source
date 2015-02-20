<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MaterialControlDataChangedTest.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.MaterialControlDataChangedTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>CloneMaterialControl Test</title>
    <script type="text/javascript">
        function onCloneComponent() {
            var parent = $get("resultDiv");
            var template = $find("MaterialControl1");
            template.cloneAndAppendToContainer(parent);
        }

        function onDataChanged(sender, e) {
            var result = "";
           
            for (var i = 0; i < e.materials.length; i++) {
                var material = e.materials[i];
                result += "<p>" + material._originalName + "：" + material._getDownloadUrl(true) + "<p/>";
            }

            resultDiv.innerHTML = result;
        }

        function onDataChanged1(sender, e) {
            var result = "";

            for (var i = 0; i < e.materials.length; i++) {
                var material = e.materials[i];
                result += "<p>" + material._originalName + "：" + material._getDownloadUrl(true) + "<p/>";
            }

            resultDiv1.innerHTML = result;
        }
       
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div >
       <SOA:MaterialControl ID="MaterialControl1" MaterialUseMode="UploadFile" OnClientMaterialsChanged="onDataChanged" 
        TemplateUrl="~/MaterialControl/Templates/Test.xlsx" RootPathName="GenericProcess" runat="server" AllowEditContent="True" />
    </div>
    <div>
    </div>
    <div id="resultDiv"></div>
    
    <div>
       <SOA:MaterialControl ID="MaterialControl2" MaterialUseMode="UploadFile" OnClientMaterialsChanged="onDataChanged1" FileSelectMode="TraditionalSingle" AutoOpenDocument="False" 
        TemplateUrl="~/MaterialControl/Templates/Test.xlsx" RootPathName="GenericProcess" runat="server" AllowEditContent="False" />
    </div>
    <div>
    </div>
    <div id="resultDiv1"></div>
    </form>
</body>
</html>
