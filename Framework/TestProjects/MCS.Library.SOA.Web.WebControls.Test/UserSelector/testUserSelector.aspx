<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="testUserSelector.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.UserSelector.testUserSelector1" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<script type="text/javascript">
		var userList = [];
		function showDialog(ctrlid) {
			var result = $find(ctrlid).showDialog();
			displaySelectedObjects(result);
		}

		function showDialog1() {
			var arg = {};
			arg.opinion = 'userList';
			arg.users = userList;
			var result = $find('UserSelector_MultiSelect').showDialog(arg);
			if (result) {
				userList = result.users;
			}
			displaySelectedObjects(result);
		}

		function displaySelectedObjects(result) {
			//debugger;
			if (result) {
				if (result.opinion.length > 0) {
					addMessage(result.opinion);
				}
				for (var i = 0; i < result.users.length; i++) {
					addMessage(result.users[i].fullPath);
				}
			}
		}

		function addMessage(msg) {
			result.innerHTML += "<p style='margin:0'>" + msg + "</p>";
		}

	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<input type="button" id="Button2" value="UserSelector_MultiSelect" onclick="showDialog('UserSelector_MultiSelect');" />
	</div>
	<div>
		<cc1:UserSelector runat="server" ID="UserSelector_MultiSelect" MultiSelect="true"
			DialogHeaderText="人员选择" DialogTitle="人员选择" ShowingMode="Normal" ShowOpinionInput="false"
			ListMask="All" />
	</div>
	<br />
	<div>
		<input type="button" id="Button1" value="UserSelector_Not_MultiSelect" onclick="showDialog('UserSelector_Not_MultiSelect');" />
	</div>
	<div>
		<cc1:UserSelector runat="server" ID="UserSelector_Not_MultiSelect" MultiSelect="false"
			DialogHeaderText="人员选择" DialogTitle="人员选择" ShowingMode="Dialog" ShowOpinionInput="false"
			ListMask="All" InvokeWithoutViewState="false" />
		<input type="button" onclick="showDialog1();" />
	</div>
	<div id="resultContainer">
		<div id="result" contenteditable="true" style="overflow: auto; border: 1px silver solid;
			width: 100%; height: 200px" runat="server">
		</div>
	</div>
	<br />
	<asp:Button ID="Button3" runat="server" Text="postback" />
	</form>
</body>
</html>
