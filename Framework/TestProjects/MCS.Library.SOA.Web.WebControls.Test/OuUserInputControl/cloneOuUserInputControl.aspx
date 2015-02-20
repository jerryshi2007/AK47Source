<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="cloneOuUserInputControl.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.OuUserInputControl.cloneOuUserInputControl" %>

<%@ Register Namespace="MCS.Web.WebControls" Assembly="MCS.Library.SOA.Web.WebControls"
	TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Clone OuUserInputControl</title>
	<script type="text/javascript">
		function onCloneComponent() {
			var parent = $get("container");

			var template = $find("userInputTemplate");

			template.cloneAndAppendToContainer(parent);
		}

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
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<SOA:OuUserInputControl ID="userInputTemplate" runat="server" InvokeWithoutViewState="true" RootPath="机构人员\远洋地产\其他单位"
			Width="320px" MultiSelect="true" OnClientSelectedDataChanged="onSelectedDataChanged"
			MergeSelectResult="false" CanSelectRoot="false" SelectMask="Organization,User"
			ListMask="Organization,User" /> 
	</div>
	<div>
		<input type="button" onclick="onCloneComponent();" value="Clone Component" />
		<input type="button" onclick="$find('userInputTemplate').set_enabled(!$find('userInputTemplate').get_enabled());" value="Set enabled/disabled" />
	</div>
	<div id="resultContainer">
		<div id="result" contenteditable="true" style="overflow: auto; border: 1px silver solid;
			width: 100%; height: 160px" runat="server">
		</div>
	</div>
	<div id="container">
	</div>
	</form>
</body>
</html>
