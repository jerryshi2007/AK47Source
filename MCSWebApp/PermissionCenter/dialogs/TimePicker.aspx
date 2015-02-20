<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TimePicker.aspx.cs" Inherits="PermissionCenter.Dialogs.TimePicker" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" scroll="no" style="overflow: hidden">
<head runat="server">
	<title>指定时间点</title>
	<link rel="icon" href="../favicon.ico" type="image/x-icon" />
	<link rel="Shortcut Icon" href="../favicon.ico" />
	<link href="../styles/dlg.css" rel="stylesheet" type="text/css" />
	<%-- <script src="../scripts/MicrosoftAjax.debug.js" type="text/javascript"></script>--%>
</head>
<body class="pcdlg" style="min-width: 0; overflow: hidden;">
	<form id="form1" runat="server">
	<div class="pcdlg-sky">
		<h1 class="pc-caption">
			指定时间点</h1>
	</div>
	<div class="pcdlg-content">
		<div class="container5">
			<div class="pc-required pc-hide" id="theprompt">
				指定的时间无效
			</div>
			<div style="height: 50px; text-align: center; vertical-align: middle; margin: auto;
				padding: 15px;">
				<mcs:DeluxeDateTime runat="server" ID="datePicker" Animated="True" DateAutoComplete="True"
					TimeAutoComplete="True" OnClientValueChanged="onPickTime" />
			</div>
		</div>
	</div>
	<div class="pcdlg-floor">
		<div class="pcdlg-button-bar">
			<input type="button" onclick="return onOkClick()" accesskey="O" class="pcdlg-button btn-def"
				value="确定(O)" /><input type="button" accesskey="C" class="pcdlg-button btn-cancel"
					onclick="return onCancelClick();" value="取消(C)" />
		</div>
	</div>
	</form>
	<script type="text/javascript">
		$pc.show("theprompt");
		$pc.setVisible("theprompt", false);

		function onPickTime(picker) {
			if (Date.isMinDate(picker.get_value()) == false)
				$pc.setVisible("theprompt", false);
			else {
				$pc.setVisible("theprompt", true);
			}
		}

		function onOkClick() {
			var selectedTime = $find("datePicker").get_value();

			if (Date.isMinDate(selectedTime) == false) {
				window.returnValue = selectedTime.getTime();
				window.close();
			}
			else
				$pc.setVisible("theprompt", true);
		}

		function onCancelClick() {
			window.returnValue = '';
			window.close();
		}
	</script>
</body>
</html>
