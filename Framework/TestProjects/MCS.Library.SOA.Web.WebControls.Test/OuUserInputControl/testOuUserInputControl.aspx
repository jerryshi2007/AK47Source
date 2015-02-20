<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="testOuUserInputControl.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.OuUserInputControl.testOuUserInputControl" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>无标题页</title>
	<script type="text/javascript">
		function onSelectedDataChanged(selectedData) {
			displaySelectedObjects(selectedData);
		}

		function displaySelectedObjects(objs) {
			for (var i = 0; i < objs.length; i++)
				addMessage(objs[i].fullPath);
		}

		function addMessage(msg) {
			result.innerHTML += "<p style='margin:0'>" + msg + "</p>";
		}

		function bind() {

		}
	</script>
</head>
<body>
	<form id="form1" runat="server">
	<div>
		<asp:Button ID="postBackAndSetReadOnly" runat="server" OnClick="postBackAndSetReadOnly_Click"
			Text="PostBack and Set ReadOnly" />
		<cc1:OuUserInputControl ID="OuUserInputControl1" runat="server" InvokeWithoutViewState="true"
			RootPath="机构人员\远洋地产" MultiSelect="true" OnClientSelectedDataChanged="onSelectedDataChanged"
			CanSelectRoot="false" EnableUserPresence="true" SelectMask="All" ListMask="All" />
		<asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="TEST" />
		<asp:LinkButton runat="server" ID="queryBtn" OnClick="Button2_Click" Text="查询" />
	</div>
	<div id="resultContainer">
		<div id="result" contenteditable="true" style="overflow: auto; border: 1px silver solid;
			width: 100%; height: 200px" runat="server">
		</div>
	</div>
	<%--<cc1:OuUserInputControl ID="OuUserInputControl2" runat="server" InvokeWithoutViewState="true"
			MultiSelect="true" OnClientSelectedDataChanged="onSelectedDataChanged" CanSelectRoot="false"
			SelectMask="User" ListMask="All"  />--%>
	</form>
</body>
</html>
