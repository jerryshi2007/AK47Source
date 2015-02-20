<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConditionHistory.aspx.cs"
	Inherits="PermissionCenter.Dialogs.ConditionHistory" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" class="pcdlg" scroll="no" style="overflow: hidden">
<head runat="server">
	<title>条件编辑历史</title>
	<link rel="icon" href="../favicon.ico" type="image/x-icon" />
	<link rel="Shortcut Icon" href="../favicon.ico" />
	<link href="../styles/dlg.css" rel="stylesheet" type="text/css" />
	<base target="_self" />
	<pc:HeaderControl ID="HeaderControl1" runat="server">
	</pc:HeaderControl>
</head>
<body class="pcdlg">
	<form id="form1" runat="server">
	<asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true" />
	<div class="pcdlg-sky">
		<h1 class="pc-caption">
			条件编辑历史
		</h1>
	</div>
	<div class="pcdlg-content">
		<pc:BannerNotice runat="server" ID="notice" />
		<soa:DataBindingControl runat="server" ID="binding1">
			<ItemBindings>
				<soa:DataBindingItem DataPropertyName="DisplayName" ControlID="spObjName" ControlPropertyName="InnerText"
					Direction="DataToControl" />
			</ItemBindings>
		</soa:DataBindingControl>
		<div class="pc-container5">
			<div>
				对象名称<span id="spObjName" runat="server" style="padding-left: 5px; font-weight: bold;"></span>
			</div>
			<div class="">
				<div>
					条件历史
				</div>
				<div>
					<asp:Repeater runat="server" ID="timeList" DataSourceID="ObjectDataSource1">
						<HeaderTemplate>
							<dl id="timeList" class="pc-time-list">
						</HeaderTemplate>
						<ItemTemplate>
							<dd data-vs='<%#Eval("VersionStartTime","{0:yyyy-MM-dd HH:mm:ss}") %>' data-ve='<%#Eval("VersionEndTime","{0:yyyy-MM-dd HH:mm:ss}") %>'
								class='<%# ((int)Eval("Status")== 1)?"pc-time-item":"pc-time-item pc-deleted" %>'>
								<span class="pc-time-desc">
									<%# Server.HtmlEncode((string)Eval("Description")) %></span>
								<div class="pc-time-exp">
									<%# Server.HtmlEncode((string)Eval("Condition")) %>
								</div>
							</dd>
						</ItemTemplate>
						<FooterTemplate>
							</dl>
						</FooterTemplate>
					</asp:Repeater>
					<asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="GetConditionHistoryEntries"
						TypeName="MCS.Library.SOA.DataObjects.Security.Adapters.HistoryAdapter">
						<SelectParameters>
							<asp:QueryStringParameter Name="id" QueryStringField="id" Type="String" />
							<asp:Parameter Name="type" Type="String" />
						</SelectParameters>
					</asp:ObjectDataSource>
				</div>
			</div>
		</div>
	</div>
	<div class="pcdlg-floor">
		<div class="pcdlg-button-bar">
			<input type="button" class="pcdlg-button" onclick="top.close();" value="关闭(C)" accesskey="C" />
		</div>
	</div>
	<script type="text/javascript">
		$pc.ui.configHistoryList("timeList");
	</script>
	</form>
</body>
</html>
