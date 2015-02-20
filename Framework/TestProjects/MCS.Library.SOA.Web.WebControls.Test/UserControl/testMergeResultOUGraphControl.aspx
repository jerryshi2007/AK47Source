<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="testMergeResultOUGraphControl.aspx.cs"
	Inherits="MCS.OA.Web.WebControls.Test.UserControl.testMergeResultOUGraphControl" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="HB" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>测试机构人员树控件合并多选结果</title>

	<script type="text/javascript">
		function onShowUserMultiSelectorResult() {
			displaySelectedObjects($find("userMultiSelector").get_selectedObjects());
		}

		function displaySelectedObjects(objs) {
			result.innerHTML = "";

			for (var i = 0; i < objs.length; i++)
				addMessage(objs[i].fullPath);
		}

		function addMessage(msg) {
			result.innerHTML += "<p style='margin:0'>" + msg + "</p>";
		}
	</script>

</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<HB:UserOUGraphControl ID="userMultiSelector" runat="server" Width="500" Height="400px"
			SelectMask="All" ShowingMode="Normal" BorderStyle="solid" BorderColor="black"
			BorderWidth="1" MultiSelect="true" RootExpanded="true" MergeSelectResult="true"
			ShowDeletedObjects="true" />
	</div>
	<div>
		<input type="button" value="显示选择结果" onclick="onShowUserMultiSelectorResult();" />
		<asp:Button runat="server" ID="postBackBtn" Text="Post Back" Width="120"/>
	</div>
	<table style="width: 100%">
		<tr>
			<td>
				<div id="resultContainer">
					<div id="result" contenteditable="true" style="overflow: auto; border: 1px silver solid;
						width: 100%; height: 200px" runat="server">
					</div>
				</div>
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
