<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestSimpleTabStrip.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.SimpleTabStrip.TestSimpleTabStrip" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="HB" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>无标题页</title>
	<script type="text/javascript">
		function onTabStripClick() {
			if (event.stripElement.key == "text2")
				event.returnValue = window.confirm("确认要切换到这里吗？");
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<asp:Button runat="server" ID="postBackBtn" Text="PostBack" />
	<input type="button" value="Show Input Text..." onclick="alert(document.getElementById('inputText').value)" />
	<input type="button" value="Show Strip Count..." onclick="alert(tabStrip.get_control().strips.length)" />
	<div>
		<HB:SimpleTabStrip ID="tabStrip" runat="server" OnClientTableStripClick="onTabStripClick();"
			SelectedKey="text1">
			<TabStrips>
				<HB:TabStripItem Key="text1" Text="下班" Logo="insert.gif" ControlID="panel1" Tag="下班 Tag" />
				<HB:TabStripItem Key="text2" Text="MSDN" Logo="delete.gif" ControlID="image2" Tag="MSDN Tag" />
				<HB:TabStripItem Key="text3" Text="徐若萱" Logo="image.gif" ControlID="image3" Tag="徐若萱 Tag" />
				<HB:TabStripItem Key="text4" Text="第一次见面" Width="120px" Logo="image.gif" ControlID="image4"
					Tag="第一次见面 Tag" />
			</TabStrips>
		</HB:SimpleTabStrip>
	</div>
	<div style="display: none">
		<div id="panel1" runat="server">
			<input id="inputText" type="text" />
			<div style="font-size: large; font-weight: bold">
				下班啦</div>
			<img id="image1" align="absMiddle" src="offDuty.bmp" alt="offDuty" />
		</div>
		<img id="image2" runat="server" align="absMiddle" src="msdn.bmp" alt="msdn" />
		<img id="image3" runat="server" align="absMiddle" src="xrx04.jpg" style="width: 100%;
			height: 100%" alt="徐若萱" />
		<img id="image4" runat="server" align="absMiddle" src="jianmian.jpg" style="width: 100%;
			height: 100%" alt="第一次见面" />
	</div>
	</form>
</body>
</html>
