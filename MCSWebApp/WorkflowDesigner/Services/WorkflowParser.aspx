<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorkflowParser.aspx.cs"
	Inherits="WorkflowDesigner.Services.WorkflowParser" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>流程分析</title>
	<style type="text/css">
		#result
		{
			width: 540px;
			border: solid 1px silver;
		}
		#processDespText
		{
			width: 540px;
			height: 480px;
		}
		#parseBtn
		{
			width: 80px;
			padding-top: 4px;
		}
	</style>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		本页面用来解析流程描述信息，用于分析流程描述的正确性
	</div>
	<div>
		请上传流程二进制文件：<asp:FileUpload runat="server" ID="uploader" />
	</div>
	<div>
		请填写流程描述信息
		<asp:DropDownList runat="server" ID="encodingSelector">
			<asp:ListItem Text="GB2312" Value="gb2312" />
			<asp:ListItem Text="UTF8" Value="utf-8" />
		</asp:DropDownList>
		<asp:Button runat="server" ID="parseBtn" Text="分析" OnClick="parseBtn_Click" />
	</div>
	<div>
		<asp:TextBox runat="server" ID="processDespText" TextMode="MultiLine">
		</asp:TextBox>
	</div>
	<div>
		分析结果
	</div>
	<div>
		<div id="result" runat="server">
		</div>
	</div>
	</form>
</body>
</html>
