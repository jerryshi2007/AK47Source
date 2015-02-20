<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DictionaryToPropertiesTest.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.PropertyGrid.DictionaryToPropertiesTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>字典转换成属性编辑器的测试</title>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<asp:ScriptManager ID="scriptManager" runat="server" EnableScriptGlobalization="true" />
	</div>
	<div>
		<SOA:PropertyGrid runat="server" ID="propertyGrid" Width="300px" Height="600px" DisplayOrder="ByCategory"
			ReadOnly="false" />
	</div>
	<p>
	</p>
	<div>
		<asp:Button runat="server" ID="bT" Text="回调" OnClick="bT_Click" />
	</div>
	<div>
		<b>Dictionary Items</b>
	</div>
	<div id="dictionaryItems" runat="server">
	</div>
	</form>
</body>
</html>
