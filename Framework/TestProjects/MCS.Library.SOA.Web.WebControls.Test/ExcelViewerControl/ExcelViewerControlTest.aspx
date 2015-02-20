<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExcelViewerControlTest.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.ExcelViewerControl.ExcelViewerControlTest" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
	<script type="text/javascript">
		function Test() {
			var oa1 = document.all.OA1;
			//var path = oa1.HttpDownloadFileToTempDir('http://localhost/MCSWebApp/StepByStep/1.xlsx');
			//alert(path);
			var path = 'C:\\Users\\ddz\\AppData\\Local\\Temp\\OA\\1.xlsx';
			document.all.OA1.Open(path);
		}
	</script>
</head>
<body>
    <form id="form1" runat="server">
    <div> 
    <%--<soa:ExcelViewerControl id="excelViewer" runat="server" ></soa:ExcelViewerControl>--%>
    </div>
   <input id="btnTest" type="button" onclick="Test();" />
	<asp:DropDownList ID="DropDownList1" runat="server">
	</asp:DropDownList>
    </form>
</body>
</html>
