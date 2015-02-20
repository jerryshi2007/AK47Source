<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CommandInputTest.aspx.cs"
	Inherits="MCS.Web.Responsive.WebControls.Test.CommandInput.CommandInputTest" %>

<!DOCTYPE html>
<html>
<head runat="server">
	<title>CommandInput的测试页面</title>
	<script language="javascript" type="text/javascript">
		var commandInputID = '<asp:Literal ID="Literal1" runat="server"></asp:Literal>';
		function onClick() {
			document.getElementById(commandInputID).value = document.getElementById("textInput").value;
		}

		function onCommandInput(commandInputControl, e) {
			switch (e.commandValue) {
				case "close":
					e.stopCommand = true; //设置后，不再执行默认的处理
					alert("close命令终止");
					break;

				case "refresh":
					alert("刷新");
					//执行默认的处理
					break;

				case "command001":
					//没有默认的处理
					alert("command001");
					break;
			}
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<res:CommandInput runat="server" ID="commandInput" OnClientCommandInput="onCommandInput" />
	</div>
	<div>
		输入命令：<input type="text" id="textInput" /><input type="button" onclick="onClick()"
			value="确认" />
	</div>
	<div>
		<input type="button" value="打开窗口" onclick="window.open('CommandInputOpen.aspx')" />
	</div>
	</form>
</body>
</html>
