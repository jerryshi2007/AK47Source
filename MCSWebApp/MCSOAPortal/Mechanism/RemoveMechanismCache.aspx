<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RemoveMechanismCache.aspx.cs" Inherits="MCS.OA.Portal.Mechanism.RemoveMechanismCache" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title id="Title1" runat="server" category="Portal">机构人员与授权缓存管理</title>

	<script type="text/javascript">
		function alertSuccessful() {
			alert(document.getElementById("promptHidden").value);
		}
	</script>

</head>
<body>
	<input runat="server" id="promptHidden" category="Portal" type="hidden" value="操作已成功！" />
	<form id="form1" runat="server">
	<div>
		<asp:Button runat="server" category="Portal" ID="ButtonRemoveOguCache" Text="清除机构人员缓存"
			CssClass="portalButton" Style="width: 180px" OnClick="ButtonRemoveOguCache_Click"
			Width="160px" />
		<asp:Button runat="server" category="Portal" ID="ButtonRemovePermissionCache" Text="清除授权的缓存"
			CssClass="portalButton" Style="width: 180px" OnClick="ButtonRemovePermissionCache_Click"
			Width="160px" />
	</div>
	</form>
</body>
</html>