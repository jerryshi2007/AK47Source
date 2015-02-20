<%@ Page Language="c#" Codebehind="TreeSamplePostPage.aspx.cs" AutoEventWireup="True"
	Inherits="MCS.Applications.AccreditAdmin.exports.TreeSamplePostPage" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
	<title>TreeSamplePostPage</title>
	<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
	<meta name="CODE_LANGUAGE" content="C#">
	<meta name="vs_defaultClientScript" content="JavaScript">
	<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">

	<script type="text/javascript" language="javascript">
			function onDocumentLoad()
			{
				Container.TextBoxData.style.display = "none";
				
				if (top.frmInput)
					top.frmInput.result.innerText = Container.TextBoxData.value;
			}
	</script>

</head>
<body onload="onDocumentLoad()">
	<form id="Container" runat="server">
		<asp:TextBox ID="TextBoxData" runat="server"></asp:TextBox>
	</form>
	<p style="font-size: 9pt; font-family: SimSun">
		<strong>看下面的结果...</strong>
	</p>
</body>
</html>
