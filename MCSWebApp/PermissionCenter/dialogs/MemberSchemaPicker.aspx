<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MemberSchemaPicker.aspx.cs"
	Inherits="PermissionCenter.Dialogs.MemberSchemaPicker" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" class="pcdlg" scroll="no" style="overflow: hidden">
<head runat="server">
	<title>选择要创建的人员类型</title>
	<link rel="icon" href="../favicon.ico" type="image/x-icon" />
	<link rel="Shortcut Icon" href="../favicon.ico" />
	<link href="../styles/dlg.css" rel="stylesheet" type="text/css" />
	<base target="_self" />
</head>
<body class="pcdlg" style="min-height: 0; min-width: 0">
	<form id="form1" runat="server">
	<div class="pcdlg-sky">
		<h1 class="pc-caption">
			选择人员类型</h1>
	</div>
	<div class="pcdlg-content">
		<div class="pc-container5">
			<h1>
				要创建什么类型的人员？
			</h1>
			<div class="pc-required pc-hide" id="theprompt">
				必须选择一个类型
			</div>
			<pc:SchemaDataSource ID="SchemaDataSource1" runat="server">
			</pc:SchemaDataSource>
			<asp:RadioButtonList runat="server" DataMember="users" ID="listSchemas" DataSourceID="SchemaDataSource1"
				DataTextField="Description" DataValueField="Name" RepeatLayout="UnorderedList">
			</asp:RadioButtonList>
		</div>
	</div>
	<div class="pcdlg-floor">
		<div class="pcdlg-button-bar">
			<input type="button" onclick="return onOkClick()" accesskey="S" class="pcdlg-button btn-def"
				value="继续(S)" /><input type="button" accesskey="C" class="pcdlg-button btn-cancel"
					onclick="return onCancelClick();" value="取消(C)" />
		</div>
	</div>
	</form>
	<script type="text/javascript">
		$pc.ui.setRadioAnySelected("listSchemas");
		$pc.ui.traceWindowWidth();
		var dlgOk = false;
		function onOkClick() {
			var val = $pc.ui.getRadioListValue("listSchemas");
			if (val) {
				window.returnValue = val; dlgOk = true; window.close();
			} else {
				$pc.show("theprompt");
			}
		}
		function onCancelClick() {
			window.returnValue = false;
			dlgOk = true;
			window.close();
		}

      
    
	</script>
</body>
</html>
