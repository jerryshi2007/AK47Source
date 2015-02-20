<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="selectReplaceAssignees.aspx.cs"
	Inherits="MCS.OA.CommonPages.AppTrace.selectReplaceAssignees" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="HB" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>请选择替换用户</title>
	<base target="_self" />
	<link href="../CSS/Dialog.css" type="text/css" rel="stylesheet" />
	<link href="../CSS/Other.css" type="text/css" rel="stylesheet" />
	<link href="../CSS/style.css" type="text/css" rel="stylesheet" />
	<script type="text/javascript">
		function onBtnOKClick() {
			try {
				var result = {
					originalUser: $find("originalUser").get_selectedSingleData(),
					targetUser: $find("targetUser").get_selectedSingleData()
				};

				if (result.originalUser == null)
					throw Error.create("请选择原用户");

				if (result.targetUser == null)
					throw Error.create("请选择现用户");

				if (result.originalUser.id === result.targetUser.id)
					throw Error.create("原用户和现用户不能相同");

				window.returnValue = Sys.Serialization.JavaScriptSerializer.serialize(result);

				top.close();
			}
			catch (e) {
				$showError(e);
			}
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<div id="dcontainer">
		<table cellpadding="0" cellspacing="0" style="width: 100%; height: 100%;" border="0">
			<tr style="height: 45px;">
				<td valign="top">
					<div id="dheader">
						<h1>
							选择替换用户
						</h1>
					</div>
				</td>
			</tr>
			<tr>
				<td>
					<div id="dcontent">
						<table cellspacing="0" cellpadding="0" style="height: 100%; width: 96%;" border="0">
							<tr>
								<td style="width: 100px; text-align: right; font-weight: bold">
									原用户：
								</td>
								<td>
									<HB:OuUserInputControl ID="originalUser" runat="server" MultiSelect="false" Width="200px"
										ShowDeletedObjects="true" SelectMask="User" ListMask="Organization,User" ClassName="inputStyle" />
								</td>
							</tr>
							<tr>
								<td style="width: 100px; text-align: right; font-weight: bold">
									现用户：
								</td>
								<td>
									<HB:OuUserInputControl ID="targetUser" runat="server" MultiSelect="false" Width="200px"
										SelectMask="User" ListMask="Organization,User" ClassName="inputStyle" />
								</td>
							</tr>
						</table>
					</div>
				</td>
			</tr>
			<tr>
				<td style="height: 80px;" valign="middle">
					<div id="dfooter">
						<p style="vertical-align: middle; height: 40px;">
							<input accesskey="O" class="formButton" id="btnOK" onclick="onBtnOKClick();" type="button"
								value="确定(O)" name="btnOK" />
							<input accesskey="C" class="formButton" id="btnClose" onclick="top.close();" type="button"
								value="取消(C)" name="btnCancel" />
						</p>
					</div>
				</td>
			</tr>
		</table>
	</div>
	</form>
</body>
</html>
