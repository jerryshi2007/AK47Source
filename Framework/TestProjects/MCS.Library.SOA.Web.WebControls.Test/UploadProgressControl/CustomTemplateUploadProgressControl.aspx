<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustomTemplateUploadProgressControl.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.UploadProgressControl.CustomTemplateUploadProgressControl" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="MCS" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>上传控件的测试</title>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<MCS:UploadProgressControl runat="server" ID="uploadProgress" DialogTitle="Upload Matrix"
			ControlIDToShowDialog="doUploadBtn" OnDoUploadProgress="uploadProgress_DoUploadProgress"
			OnBeforeNormalPreRender="uploadProgress_BeforeNormalPreRender" 
			onloadingdialogcontent="uploadProgress_LoadingDialogContent" />po
	</div>
	<div>
		<input runat="server" id="doUploadBtn" type="button" value="Upload..." />
	</div>
	<div>
		这里应该是一个Grid，根据上传结果决定是否刷新
	</div>
	</form>
</body>
</html>
