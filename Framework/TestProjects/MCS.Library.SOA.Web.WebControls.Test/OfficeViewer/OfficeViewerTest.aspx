<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OfficeViewerTest.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.OfficeViewer.OfficeViewerTest" %>

<%@ Register assembly="MCS.Library.SOA.Web.WebControls" namespace="MCS.Web.WebControls" tagprefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function openFile() {
            var officeViewer = $find('OfficeViewerWrapper1').get_viewer();
            officeViewer.OpenExcel("D:\\111.xlsx");
        }
        function browseFile() {
            var officeViewer = $find('OfficeViewerWrapper1').get_viewer();
            officeViewer.OpenFileDialog();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
       <input type="button" onclick="openFile();" value="打开" />
       <input type="button" onclick="browseFile();" value="浏览" />
        <cc1:OfficeViewerWrapper ID="OfficeViewerWrapper1" runat="server" Width="100%" Height="640px" ShowToolbars="true" 
        />
    <script type="text/javascript" language="javascript" for="OfficeViewerWrapper1_Viewer" event="BeforeDocumentSaved()" >
    alert(0); 
    </script>
 <%--   <script type="text/javascript">
       $addHandler($get("OfficeViewerWrapper1_Viewer"), "BeforeDocumentSaved()", function () { alert(0); });

    </script>--%>

    </div>
    </form>
</body>
</html>
