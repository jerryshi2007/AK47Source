<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelectProcessUsers.aspx.cs"
	Inherits="WfFormTemplate.Forms.SelectProcessUsers" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>选择流程用户</title>
	<link href="../css/css.css" rel="stylesheet" type="text/css" />
	<script type="text/javascript">
		function onNextButtonClick() {
			var gotoNextStep = $find('processUsers').get_selectedOuUserData().length > 0;

			if (!gotoNextStep)
				alert("请选择人员");

			return gotoNextStep;
		}

		function focusUserInput() {
			var selector = $find('processUsers');
			selector.focusInputArea();
		}

		function onApplicationLoad() {
			focusUserInput();
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<div class="t-layout">
		<div class="t-caption">
			自由流程定义
		</div>
		<div>
			<div class="t-prompt">
				选择审批人</div>
			<soa:OuUserInputControl ID="processUsers" runat="server" MultiSelect="True" SelectMask="User"
				ListMask="Organization,User" ClassName="inputStyle" AllowSelectDuplicateObj="true" />
		</div>
		<div class="t-prompt">
			<p>
				说明：自由流程是工作人员在创建流程时，可以自由地定义流程中的环节。
			</p>
		</div>
		<div class="t-button-panel">
			<asp:Button AccessKey="N" runat="server" ID="btnOK" Text="下一步(N)" CssClass="formButton"
				OnClientClick="return onNextButtonClick();" OnClick="btnOK_Click" />
			<input accesskey="C" type="button" class="formButton" value="关闭(C)" onclick="window.close();" />
		</div>
	</div>
	</form>
	<script type="text/javascript">
		Sys.Application.add_load(onApplicationLoad);
	</script>
</body>
</html>
