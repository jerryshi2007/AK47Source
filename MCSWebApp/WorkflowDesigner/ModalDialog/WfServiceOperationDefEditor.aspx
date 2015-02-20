<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WfServiceOperationDefEditor.aspx.cs"
	Inherits="WorkflowDesigner.ModalDialog.WfServiceOperationDefEditor" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="cc1" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>调用服务编辑</title>
	<base target="_self" />
	<style type="text/css">
		#methodNameHints li
		{
			cursor: pointer;
			display: block;
			float: left;
			padding: 2px 5px 0;
			color: #008F00;
		}
		.wrapper
		{
			margin: 0 110px 0 0;
			padding: 5px;
			text-align: left;
			width: auto;
		}
		
		.button
		{
			display: inline-block;
			margin-right: 10px;
		}
		
		.moreButton
		{
			display: block;
			line-height: 12px;
			height: 16px;
			margin: 2px;
			width: 20px;
			float: right;
			border: solid 1px #345678;
			font-weight: bold;
			text-align: center;
		}
		
		.moreButton:focus
		{
			color: #ff0000;
		}
	</style>
	<link rel="stylesheet" type="text/css" href="../css/dlg.css" />
	<script type="text/javascript" src="../js/jquery-1.4.3.js"></script>
	<script type="text/javascript" src="../js/common.js"></script>
</head>
<body class="pcdlg">
	<form id="form1" runat="server">
	<div class="pcdlg-sky">
		<div class="gridHead" style="line-height: 28px">
			<div class="dialogTitle">
				<span class="dialogLogo">调用服务编辑</span>
			</div>
		</div>
	</div>
	<div class="pcdlg-content">
		<asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true"
			EnablePageMethods="true">
			<Services>
				<asp:ServiceReference Path="~/Services/ServiceForClient.asmx" />
			</Services>
		</asp:ScriptManager>
		<div style="display: none">
			<asp:Button ID="btnRefresh" runat="server" OnClick="btnRefresh_Click" />
		</div>
		<table width="100%" style="height: 100%; width: 100%;" class="dialogContent">
			<tr>
				<td style="width: 100px">
				</td>
				<td>
				</td>
			</tr>
			<tr>
				<td class="label" valign="middle">
					Key:
				</td>
				<td valign="middle">
					<div class="wrapper">
						<input id="opKey" style="width: 100%" disabled />
					</div>
				</td>
			</tr>
			<tr>
				<td class="label" valign="middle">
					服务Key:
				</td>
				<td valign="middle">
					<div class="wrapper">
						<div style="display: none">
							<asp:HiddenField runat="server" ID="selectedKey" />
						</div>
						<a href="javascript:void(0);" class="moreButton" onclick="openServicesAddressDialog();">
							...</a>
						<div style="border: 1px solid #345678; padding: 2px 32px 2px 2px" runat="server"
							id="svcAddressKey">
							&nbsp;
						</div>
					</div>
				</td>
			</tr>
			<tr>
				<td>
				</td>
				<td>
					<div class="wrapper">
						<div id="svcAddressValue" style="white-space: normal; word-break: break-all; position: static;
							width: 100%">
						</div>
					</div>
				</td>
			</tr>
			<tr>
				<td class="label" valign="middle">
					调用方法名称:
				</td>
				<td valign="middle">
					<div class="wrapper">
						<input id="opName" style="width: 100%" />
					</div>
					<div>
						<div style="text-align: left">
							<a href="javascript:void(0);" onclick="discover('methodName');" class="button">&gt;&gt;提示方法名</a><a
								href="javascript:void(0);" onclick="discover('parameters');" class="button">&gt;&gt;提示参数</a></div>
					</div>
				</td>
			</tr>
			<tr>
				<td>
				</td>
				<td>
					<div id="methodHintPan" style="display: none">
						<p>
							可用的方法名(点击后隐藏)：
						</p>
						<ul id="methodNameHints">
						</ul>
					</div>
				</td>
			</tr>
			<tr>
				<td>
				</td>
				<td>
					<table>
						<tr>
							<td class="label" valign="middle">
								超时时间:
							</td>
							<td valign="middle">
								<MCS:DeluxeTime ID="timeInput" runat="server" TimeAutoComplete="true" TimeMask="99:99:99"
									TimeAutoCompleteValue="00:00:00">
								</MCS:DeluxeTime>
							</td>
							<td class="label" valign="middle">
								流程持久化时保存:
							</td>
							<td>
								<input type="checkbox" id="invokeWhenPersist" />
							</td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
				<td class="label" valign="top">
					调用参数定义:
				</td>
				<td valign="top">
					<cc1:ClientGrid runat="server" ID="detailGrid" Caption="" ShowEditBar="true" Width="100%">
						<Columns>
							<cc1:ClientGridColumn SelectColumn="true" ShowSelectAll="true" ItemStyle="{width:'50px',TEXT-ALIGN: 'center' }"
								HeaderStyle="{width:'50px'}" />
							<cc1:ClientGridColumn DataField="Name" HeaderText="参数名称" DataType="String">
								<EditTemplate EditMode="TextBox" />
							</cc1:ClientGridColumn>
							<cc1:ClientGridColumn DataField="Type" HeaderText="数据类型" DataType="Enum">
								<EditTemplate EditMode="DropdownList" TemplateControlID="dataTypeDropDownList" />
							</cc1:ClientGridColumn>
							<cc1:ClientGridColumn DataField="Value" HeaderText="参数值" DataType="String">
								<EditTemplate EditMode="TextBox" />
							</cc1:ClientGridColumn>
						</Columns>
					</cc1:ClientGrid>
					<div id="Div1" runat="server" style="display: none">
						<asp:DropDownList ID="dataTypeDropDownList" runat="server" />
					</div>
				</td>
			</tr>
			<tr id="trRtn" runat="server">
				<td class="label" valign="middle">
					返回值变量名:
				</td>
				<td valign="middle">
					<div class="wrapper">
						<input id="rtnVar" style="width: 100%" />
					</div>
				</td>
			</tr>
		</table>
	</div>
	<div class="pcdlg-floor">
		<table style="width: 100%; height: 100%">
			<tr>
				<td style="text-align: center;">
					<input id="btnOK" type="button" value="确定(O)" accesskey="O" onclick="onbtnOKClick();"
						class="formButton" />
				</td>
				<td style="text-align: center;">
					<input id="btnCancel" type="button" value="取消(C)" accesskey="C" onclick="window.close();"
						class="formButton" />
				</td>
			</tr>
		</table>
	</div>
	<input type="hidden" runat="server" id="hiddenSvcOperationTemplate" />
	</form>
	<script type="text/javascript">
		var svcOperation = {};
		var existOperationDef = [];
		Sys.Application.add_load(function () {
			var paraData = window.dialogArguments;

			if (paraData) {
				existOperationDef = Sys.Serialization.JavaScriptSerializer.deserialize(paraData.existDefJsonStr);

				if (!paraData.jsonStr) {
					svcOperation = Sys.Serialization.JavaScriptSerializer.deserialize($("#hiddenSvcOperationTemplate").val());
					svcOperation.Key = CreateKey();
					$("#opKey").val(svcOperation.Key);
				} else {
					svcOperation = Sys.Serialization.JavaScriptSerializer.deserialize(paraData.jsonStr);
				}

				setPage();
			}

			$("#methodNameHints li").live("click", function () {
				$("#opName").val($(this).text());
			});

			$("#methodHintPan").click(function () { hideHint(); });
		});

		function hideHint() {
			$("#methodHintPan").hide();
		}

		function CreateKey() {
			var i = 0;
			while (true) {
				var key = "OperationKey" + i;
				if (!existOperationDef.has(key, function (o, val) {
					if (o.Key == val) return true;
					return false;
				})) {
					return key;
				}
				i++;
			}
		}

		function setPage() {
			$("#opKey").val(svcOperation.Key);

			if (svcOperation.AddressDef) {
				$("#selectedKey").val(svcOperation.AddressDef.Key);
				$("#svcAddressKey").text(svcOperation.AddressDef.Key);
				$("#svcAddressValue").text(svcOperation.AddressDef.Address);
			}

			$("#opName").val(svcOperation.OperationName);
			$("#rtnVar").val(svcOperation.RtnXmlStoreParamName);

			if (svcOperation.InvokeWhenPersist)
				$get("invokeWhenPersist").checked = svcOperation.InvokeWhenPersist;

			var grid = $find("detailGrid");

			grid.set_autoBindOnLoad(false);
			grid.set_dataSource(svcOperation.Params || []);

			if (svcOperation.TimeOut) {
				$find("timeInput").set_TimeValue(svcOperation.TimeOut);
			}
		}

		function onbtnOKClick() {
			var addressKey = $("#selectedKey").val();
			if (addressKey == null || addressKey == undefined || addressKey.length === 0) {
				alert("请选择服务地址");
				$("#selectedKey").focus();

				return false;
			}

			if ($("#opName").val().trim() == "") {
				alert("请输入调用方法名称");
				$("#opName").focus();

				return false;
			}

			if (!svcOperation.AddressDef) {
				alert('未选择地址定义');

				return false;
			}

			svcOperation.OperationName = $("#opName").val().trim();
			svcOperation.RtnXmlStoreParamName = $("#rtnVar").val().trim();
			svcOperation.InvokeWhenPersist = $get("invokeWhenPersist").checked;
			svcOperation.Params = $find("detailGrid").get_dataSource();
			svcOperation.TimeOut = $find("timeInput").get_TimeValue();

			window.returnValue = { jsonStr: Sys.Serialization.JavaScriptSerializer.serialize(svcOperation) };
			top.close();
		}

		function openServicesAddressDialog() {
			var sFeature = "dialogWidth:800px; dialogHeight:560px;center:yes;help:no;resizable:no;scroll:no;status:no";
			var result;
			result = window.showModalDialog("ServicesAddressList.aspx", null, sFeature);
			if (result) {
				$("#selectedKey").val(result.Key);
				$("#svcAddressKey").text(result.Key);
				$("#svcAddressValue").text(result.Address);
				svcOperation.AddressDef = result;
			}
		}

		function discover(what) {
			var addressKey = $("#selectedKey").val();
			if (addressKey == null || addressKey == undefined || addressKey.length === 0 || !svcOperation.AddressDef) {
				alert("请选择服务地址");
				$("#selectedKey").focus();
				return false;
			}

			var addDef = svcOperation.AddressDef;

			var httpMethod = null;

			if (addDef) {

				switch (addDef.RequestMethod) {
					case 0:
						httpMethod = "HttpGet";
						break;
					case 1:
						httpMethod = "HttpPost";
						break;
					case 2:
						httpMethod = "Soap"
						break;
					default:
						break;
				}

				if (what == "methodName") {
					$("#methodHintPan").hide();
					PageMethods.DiscoverMethods(addDef.Address, httpMethod, function (ok) {
						$("#methodHintPan").show();
						var ul = $("#methodNameHints").html("");

						for (var kk = 0; kk < ok.length; kk++) {
							ul.append($("<li></li>").text(ok[kk]));
						}
					}, function (err) {
						alert("探查方法失败");
					});
				} else if (what == "parameters") {
					var methodName = $("#opName").val();
					if (!methodName.trim().length) {
						alert("请指定调用方法名称");
						$("#opName").focus();
						return false;
					}

					PageMethods.DiscoverParameters(addDef.Address, httpMethod, methodName, function (ok) {
						var sss = $find("detailGrid").get_dataSource();
						sss.length = 0;
						var size = ok.length;
						for (var i = 0; i < size; i++) {
							sss.push(ok[i]);
						}
						$find("detailGrid").set_dataSource(sss);
					}, function (err) {
						alert("探查参数失败");
					});
				}
			}
		}
	</script>
</body>
</html>
