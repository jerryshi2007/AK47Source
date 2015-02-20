<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="clientSetDataTest.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.OuUserInputControl.clientSetDataTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>客户端设置用户信息的测试</title>
	<script type="text/javascript">
		function onDocumentLoad() {
			window.setTimeout(function () {
				var data = Sys.Serialization.JavaScriptSerializer.deserialize($get("userData").value);
				var selector = $find("ouUserSelector");
				selector.set_selectedSingleData(data);
				selector.dataBind();
			}, 100);
		}
	</script>
</head>
<body onload="onDocumentLoad();">
	<form id="serverForm" runat="server">
	<div>
		请选择人员
	</div>
	<div style="width: 400px">
		<SOA:OuUserInputControl ID="ouUserSelector" runat="server" InvokeWithoutViewState="true"
			RootPath="机构人员\远洋地产" MultiSelect="true" CanSelectRoot="false" EnableUserPresence="true"
			SelectMask="All" ListMask="All" />
	</div>
	</form>
</body>
</html>
