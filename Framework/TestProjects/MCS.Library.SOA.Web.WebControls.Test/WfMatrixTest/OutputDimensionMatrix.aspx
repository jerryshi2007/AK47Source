<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OutputDimensionMatrix.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.WfMatrixTest.OutputDimensionMatrix" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="MCS" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>上传下载Matrix测试</title>
	<script type="text/javascript">
		function onOpenDialogClick() {
			alert($find("uploadProgress").showDialog());
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server" target="_blank">
	<div>
		<MCS:UploadProgressControl runat="server" ID="uploadProgress" DialogTitle="Upload Matrix"
			OnDoUploadProgress="uploadProgress_DoUploadProgress" />
	</div>
	<div>
		<asp:Button runat="server" ID="generateButton" Text="Generate Definition Excel..."
			OnClick="generateButton_Click" />
		<asp:Button runat="server" ID="generateMatrixButton" Text="Generate Matrix Excel..."
			OnClick="generateMatrixButton_Click" />
		<input type="button" value="Import Matrix From Excel..." onclick="onOpenDialogClick();" />
	</div>
	</form>
</body>
</html>
