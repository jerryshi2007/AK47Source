<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OUNotFound.aspx.cs" Inherits="PermissionCenter.lists.OUNotFound" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>指定的组织不存在</title>
</head>
<body>
	<form id="form1" runat="server" style="height: 100%">
	<div style="width: 100%; margin: 0; vertical-align: middle; text-align: center;">
		<p>
			无法找到ID为<span runat="server" id="txtID"></span>的组织。</p>
		<p>
			指定了无效的ID，或该组织在当前时间上下文中不存在或已删除，请从左边的组织树中选择其他组织。</p>
	</div>
	<input type="hidden" runat="server" id="hfOuId" />
	</form>
	<script type="text/javascript">
		if (window.parent.showLoader)
			window.parent.showLoader(false);

		function refreshOwnerTree() {
			if (typeof (window.parent.selectRoot) === 'function') {
				window.parent.selectRoot();
			} 
		}

		window.setTimeout(refreshOwnerTree, 2000);
	</script>
</body>
</html>
