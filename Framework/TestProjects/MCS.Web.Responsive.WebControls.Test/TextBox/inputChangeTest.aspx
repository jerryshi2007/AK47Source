<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="inputChangeTest.aspx.cs"
	Inherits="MCS.Web.Responsive.WebControls.Test.TextBox.inputChangeTest" %>

<html>
<head runat="server">
	<title>测试属性改变事件</title>
	<script type="text/javascript">
		function onPropertyChange() {
			var element = event.srcElement;
			result.innerText = element.value;
		}

		function onChangeDataClick() {
			document.getElementById("dataInput").value = new Date();
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<input type="text" id="dataInput" onpropertychange="onPropertyChange();" oninput="onPropertyChange();" />
	</div>
	<div>
		<input type="button" onclick="onChangeDataClick();" value="Change Data" />
	</div>
	<div id="result">
	</div>
	</form>
</body>
</html>
