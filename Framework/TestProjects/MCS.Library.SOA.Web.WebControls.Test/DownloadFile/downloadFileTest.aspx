<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="downloadFileTest.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.DownloadFile.downloadFileTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Download File Test</title>
	<script type="text/javascript">
		function onDownloadClick() {
			var localPath = $HBRootNS.fileIO._getTempFilePath("downloadTestFile.txt", "downloadTestFile", "shenzheng");
			$HBRootNS.fileIO.downloadFile(theForm.action, localPath, "POST", true);

			$HGClientMsg.alert(localPath, "", "信息");
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<SOA:ComponentHelperWrapper runat="server" ID="componentHelper" />
	</div>
	<div>
		<asp:TextBox runat="server" ID="UserName" Text="Shen Zheng" />
	</div>
	<div>
		<asp:ScriptManager runat="server" EnableScriptGlobalization="true">
		</asp:ScriptManager>
		<input type="button" onclick="onDownloadClick()" value="Download..." />
	</div>
	</form>
</body>
</html>
