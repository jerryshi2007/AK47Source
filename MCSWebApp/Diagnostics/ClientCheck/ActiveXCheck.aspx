<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ActiveXCheck.aspx.cs" Inherits="Diagnostics.ClientCheck.ActiveXCheck" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<head id="Head1" runat="server">
	<title>Validate Client Active X Functions</title>
	<script type="text/javascript">
		function showDialog() {
			var dialogHelper = document.getElementById('dialogHelperActiveX');
			dialogHelper.Title = "测试程序";
			dialogHelper.MultiSelect = true;
			if (dialogHelper.OpenDialog()) {
				alert(dialogHelper.FileName);
			}
		}

		function getWindowsTempPath() {
			var componentHelper = document.getElementById('componentHelperActiveX');
			var fileObj = componentHelper.CreateObject("Scripting.FileSystemObject");

			alert(fileObj.GetSpecialFolder(2).Path);
		}

		function openWordApplication() {
			var componentHelper = document.getElementById('componentHelperActiveX');
			var word = componentHelper.CreateLocalServer("Word.Application");

			word.Visible = true;
		}

		function getComputerName() {
			var componentHelper = document.getElementById('componentHelperActiveX');
			var network = componentHelper.CreateObject("WScript.Network");

			alert(network.ComputerName);
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<object id="dialogHelperActiveX" classid="clsid:C86C48A2-0DAD-41B6-BB85-AAB912FEE3AB"
			width="0" height="0" codebase="/MCSWebApp/HBWebHelperControl//HBWebHelperControl.CAB#version=1,0,0,13">
		</object>
		<object id="componentHelperActiveX" classid="clsid:918CFB81-4755-4167-BFC7-879E9DD52C9E"
			width="0" height="0" codebase="/MCSWebApp/HBWebHelperControl/HBWebHelperControl.CAB#version=1,0,0,13">
		</object>
		<input type="button" value="showDialog" onclick="showDialog();" />
		<input type="button" value="getWindowsTempPath" onclick="getWindowsTempPath();" />
		<input type="button" value="openWordApplication" onclick="openWordApplication();" />
		<input type="button" value="getComputerName" onclick="getComputerName();" />
	</div>
	</form>
</body>
</html> 