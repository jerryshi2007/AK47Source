<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserSettingsManager.aspx.cs"
	Inherits="MCS.OA.Portal.UserPanel.UserSettingsManager" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="HB" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
	Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="DeluxeWorks" %>
<%@ Register TagPrefix="cc1" Namespace="MCS.Web.WebControls" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>个人设置</title>
	<base target="_self" />
	<meta http-equiv="Cache-Control" content="no-cache" />
	<meta http-equiv="Pragma" content="no-cache" />
	<link href="../css/Dialog.css" rel="stylesheet" type="text/css" />
	<style type="text/css">
		.clientGrid .mainTable
		{
			background-color: #EEE;
		}
		.selectedCell
		{
			color: white;
			background-color: #BA3333;
			border: silver 1px solid;
		}
		.categoryCell
		{
			border-right: silver 1px solid;
			border-top: silver 1px solid;
			font-weight: bold;
			border-left: silver 1px solid;
			color: gray;
			border-bottom: silver 1px solid;
			background-color: #DDD;
		}
		.propertyCell
		{
			border-right: silver 1px solid;
			border-top: silver 1px solid;
			border-left: silver 1px solid;
			border-bottom: silver 1px solid;
			background-color: #FFF;
		}
		.valueInput
		{
			border-right: medium none;
			border-top: medium none;
			border-left: medium none;
			border-bottom: medium none;
			font-family: simsun;
		}
	</style>
	<script language="javascript" type="text/javascript">

		var dataSource;

		function nodeClick(sender, e) {
			if (e.node && e.node.get_value && e.node.get_value()) {
				AddControl(e.node.get_value());
			}
		}

		function pageLoad() {
			AddControl();
		}

		function AddControl(category) {
			//读取隐藏域中的JSON对象到dataSource
			dataSource = Sys.Serialization.JavaScriptSerializer.deserialize(document.all('txtDataSource').value);

			if (typeof (category) == "undefined" && dataSource.Categories.length > 0)
				category = dataSource.Categories[0].Name;

			if (dataSource) {
				for (var i = 0; i < dataSource.Categories.length; i++) {
					if (dataSource.Categories[i].Name == category) {
						var propertyGrid = $find("PropertyGrid1");
						propertyGrid.set_properties(dataSource.Categories[i].Properties);
						propertyGrid.dataBind();

						break;
					}
				}
			}
			else {
				e.cancel = true;
			}
		}

		function stripscript(s) {
			var reg = new RegExp("^[\u4E00-\u9FA5]+$");
			var pattern = new RegExp("[`~!@#$^&*()=|{}':;',\\[\\].<>/?~！@#￥……&*（）&/;—|{}【】‘；：”“'。，、？]");
			var rs = "";
			for (var i = 0; i < s.length; i++) {
				rs = rs + s.substr(i, 1).replace(pattern, '');
			}
			var gs = "";
			for (var j = 0; j < rs.length; j++) {
				gs = gs + rs.substr(j, 1).replace(reg, '');
			}
			return gs;
		}

		function OnClientEditorValidated(sender, e) //当用户修改完后就及时更新到dataSource
		{
			if (dataSource) {
				for (var i = 0; i < dataSource.Categories.length; i++) {
					for (var j = 0; j < dataSource.Categories[i].Properties.length; j++) {
						//e.property.defaultValue
						if (e.property.name == dataSource.Categories[i].Properties[j].name) {
							if (e.property.value > 0) {
								dataSource.Categories[i].Properties[j].value = e.property.value;
								break;
							}
							else {
								dataSource.Categories[i].Properties[j].value = e.property.defaultValue;
							}
						}
					}
				}
			}
		}

		function updateHiddenText() {
			if (dataSource) {

				/*for (var i = 0; i < dataSource.Categories.length; i++) {
				for (var j = 0; j < dataSource.Categories[i].Properties.length; j++) {
				var validatorValue = stripscript(dataSource.Categories[i].Properties[j].value);
				if (validatorValue <= 0 || validatorValue >= 100 || validatorValue == "") {
				if (dataSource.Categories[i].Properties[j].dataType == 9 || dataSource.Categories[i].Properties[j].dataType == 15) {
				alert("输入的页码必须是在1-100之间");
				event.returnValue = false;
				return false;
				}
				if (dataSource.Categories[0].Properties[3].value == "") {
				dataSource.Categories[0].Properties[3].value = dataSource.Categories[0].Properties[3].defaultValue;
				}
				}
				}
				*/

				try {
					var reValue = $HGRootNS.PropertyEditorControlBase.ValidateProperties();

					if (reValue.isValid == true) {	//返回是否通过
						var txtHiddleValue = Sys.Serialization.JavaScriptSerializer.serialize(dataSource);
						document.getElementById("txtDataSource").value = txtHiddleValue;
					} else {
						throw reValue.errorMessages;
					}
				}
				catch (e) {
					alert(e);
				}
			}
		}
	</script>
	<script src="usersettings.js" type="text/javascript"></script>
</head>
<body>
	<form id="form1" runat="server" target="_innerFrame">
	<asp:ScriptManager ID="scriptManager" runat="server" EnableScriptGlobalization="true">
	</asp:ScriptManager>
	<div style="display: none">
		<input type="hidden" runat="server" id="hiddenSource" />
		<asp:TextBox ID="txtDataSource" runat="server" Height="1px" TextMode="MultiLine"
			Style="display: none" Width="3px"></asp:TextBox>
	</div>
	<div id="dcontainer">
		<div id="dcontent">
			<table cellspacing="0" border="0" cellpadding="0" style="width: 100%; height: 100%">
				<tr>
					<td colspan="2" style="height: 42px">
						<div id="dheader">
							<h1>
								个人设置
							</h1>
						</div>
					</td>
				</tr>
				<tr valign="top">
					<td style="width: 160px; border-right: solid 1px #ccc;">
						<table cellpadding="0" cellspacing="0" style="width: 100%;" border="0">
							<tr valign="top">
								<td style="width: 100%">
									<DeluxeWorks:DeluxeTree ID="tree" runat="server" BorderStyle="None" NodeCloseImg="closeImg.gif"
										NodeOpenImg="openImg.gif">
									</DeluxeWorks:DeluxeTree>
								</td>
							</tr>
						</table>
					</td>
					<td>
						<table style="width: 100%" cellpadding="0" cellspacing="0">
							<tr>
								<td style="height: 20px">
									<table style="width: 100%" cellpadding="0" cellspacing="2">
										<tr>
											<td style="font-weight: bold; font-size: 14px;">
												<span id="appDescription"></span>属性:
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td valign="top">
									<div style="overflow: auto; width: 100%">
										<table id="grid" style="width: 100%; border-collapse: collapse; text-align: left;"
											cellpadding="0" cellspacing="0">
											<HB:PropertyGrid ID="PropertyGrid1" runat="server" DisplayOrder="ByCategory" Width="460px"
												Height="350px" OnClientEditorValidated="OnClientEditorValidated" OnBindEditorDropdownList="onBindEditorDropdownList" />
										</table>
									</div>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colspan="2">
						<div id="dfooter">
							<asp:Button ID="btnSubmit" OnClientClick="" runat="server" OnClick="BtnUpdateUserSetting"
								Text="确定(O)" AccessKey="O" CssClass="portalButton" />
							<input id="closeButton" accesskey="C" class="portalButton" type="button" value="关闭(C)"
								onclick="window.close();" />
						</div>
					</td>
				</tr>
			</table>
		</div>
	</div>
	<script src="../JavaScript/propertyEditors.js" type="text/javascript"></script>
	</form>
	<iframe style="display: none" id="innerFrame" name="_innerFrame"></iframe>
	<script type="text/javascript">
		Sys.Application.add_load(onDocumentLoad);
	</script>
</body>
</html>
