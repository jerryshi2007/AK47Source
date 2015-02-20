<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DownloadMatrix.aspx.cs"
	Inherits="WorkflowDesigner.MatrixModalDialog.DownloadMatrix" ViewStateMode="Disabled" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>正在跳转</title>
	<meta http-equiv="refresh" runat="server" id="meta1" />
</head>
<body>
	<form id="form1" runat="server">
	<div>
		如果您的下载没有在5秒内开始或者无法正常下载，请点击<asp:HyperLink runat="server" ID="lnkTo">此处</asp:HyperLink>。
	</div>
	</form>
</body>
</html>
