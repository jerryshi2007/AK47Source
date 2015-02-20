<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="JobEditor.aspx.cs" Inherits="WorkflowDesigner.PlanScheduleDialog.JobEditor" %>

<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>作业编辑</title>
	<base target="_self" />
	<style type="text/css">
		th
		{
			font-size: 12px;
			font-weight: bold;
		}
		.tbl
		{
			width: 100%;
			margin-top: 2px;
		}
		.tbl td
		{
			height: 20px;
		}
		.tdName
		{
			width: 80px;
			text-align: right;
		}
	</style>
</head>
<body>
	<form id="form1" runat="server">
	<table style="height: 100%; width: 100%">
		<tr>
			<td class="gridHead">
				<div class="dialogTitle">
					<span class="dialogLogo">作业编辑</span>
				</div>
			</td>
		</tr>
		<tr>
			<td style="vertical-align: middle; height: 100%;">
				<div class="dialogContent" id="dialogContent" runat="server" style="width: 100%;
					height: 100%; overflow: auto">
					<fieldset title="">
						<legend class="label">基本信息</legend>
						<table class="tbl">
							<tr>
								<th style="text-align: right;">
									作业名称:
								</th>
								<td>
									<input id="txtName" type="text" runat="server" style="width: 300px" />
								</td>
								<th style="text-align: right;">
									作业类型:
								</th>
								<td>
									<SOA:HBDropDownList ID="ddlType" runat="server">
									</SOA:HBDropDownList>
								</td>
								<th style="text-align: right;">
									是否启用:
								</th>
								<td>
									<select id="ddlEnabled" runat="server">
										<option value="true">是</option>
										<option value="false">否</option>
									</select>
								</td>
							</tr>
							<tr>
								<th style="text-align: right;">
									作业描述:
								</th>
								<td colspan="3">
									<input id="txtDesc" type="text" runat="server" style="width: 100%" />
								</td>
								<th style="text-align: right;">
									分类
								</th>
								<td>
									<input id="txtCategory" type="text" runat="server" style="width: 100px" maxlength="10" />
								</td>
							</tr>
						</table>
					</fieldset>
					<br />
					<div id="divStartWorkflow" style="display: none;">
						<fieldset title="">
							<legend class="label">流程信息设置</legend>
							<table class="tbl">
								<tr>
									<th style="text-align: right;">
										流程模板号:
									</th>
									<td style="line-height: 20px">
										<SOA:HBDropDownList ID="ddlProcess" runat="server" DataValueField="Key" DataTextField="Text">
										</SOA:HBDropDownList>
										<a href="#" class="button" style="padding: 2px; border: solid 1px #0; line-height: 25px;
											display: inline-block;" onclick="openProcessList();">选择</a>
									</td>
									<td style="text-align: right;">
										发起人:
									</td>
									<td>
										<SOA:OuUserInputControl MultiSelect="false" ID="OuUserInputControl" runat="server"
											Width="250" ShowDeletedObjects="true" InvokeWithoutViewState="true" MergeSelectResult="false"
											SelectMask="User" />
									</td>
								</tr>
							</table>
						</fieldset>
						<br />
					</div>
					<div id="divInvokingService" style="display: none;">
						<fieldset title="">
							<legend class="label">Web服务设置</legend>
							<div style="background-color: #C0C0C0">
								<a href="#" onclick="createSvcOperation();">
									<img src="/MCSWebApp/Images/appIcon/15.gif" alt="添加" border="0" />
								</a><a href="#" onclick="removeInvokingService();">
									<img src="/MCSWebApp/Images/16/delete.gif" alt="删除" border="0" />
								</a>
							</div>
							<SOA:ClientGrid ID="invokingServiceGrid" runat="server" PageSize="10" AllowPaging="true"
								AutoPaging="true" ShowEditBar="false" Width="100%" OnClientCellDataBound="invokingServiceGridCellBound">
								<Columns>
									<SOA:ClientGridColumn SelectColumn="true" ShowSelectAll="true" ItemStyle="{width:'30px',TEXT-ALIGN: 'center' }"
										HeaderStyle="{width:'30px'}" />
									<SOA:ClientGridColumn DataField="Key" HeaderText="Key" DataType="String" ItemStyle="{width:'90px',word-wrap:'break-word'}">
										<EditTemplate EditMode="None" />
									</SOA:ClientGridColumn>
									<SOA:ClientGridColumn DataField="AddressDef" HeaderText="服务地址" DataType="String"
										HeaderStyle="{TEXT-ALIGN:'center'}" ItemStyle="{width:'500px',word-wrap:'break-word'}">
										<EditTemplate EditMode="None" />
									</SOA:ClientGridColumn>
									<SOA:ClientGridColumn DataField="OperationName" HeaderText="方法名称" DataType="String"
										HeaderStyle="{width:'80px'}" ItemStyle="{width:'80px',word-wrap:'break-word'}">
										<EditTemplate EditMode="None" />
									</SOA:ClientGridColumn>
									<SOA:ClientGridColumn DataField="Params" HeaderText="参数个数" DataType="String" HeaderStyle="{width:'80px'}"
										ItemStyle="{width:'80px',TEXT-ALIGN:'center',word-wrap:'break-word'}">
										<EditTemplate EditMode="None" />
									</SOA:ClientGridColumn>
								</Columns>
							</SOA:ClientGrid>
						</fieldset>
						<br />
					</div>
					<fieldset title="">
						<legend class="label">执行计划设置</legend>
						<div style="background-color: #C0C0C0">
							<a href="javascript:void(0);" onclick="openScheduleDialog();">
								<img src="../images/document_new.png" alt="新建" border="0" />
							</a><a href="javascript:void(0);" onclick="openScheduleListDialog();">
								<img src="/MCSWebApp/Images/appIcon/15.gif" alt="添加" border="0" />
							</a><a href="javascript:void(0);" onclick="removeSchedule();">
								<img src="/MCSWebApp/Images/16/delete.gif" alt="删除" border="0" />
							</a>
						</div>
						<SOA:ClientGrid runat="server" PageSize="10" AllowPaging="true" ID="detailGrid" Caption=""
							ShowEditBar="false" Width="100%" OnClientCellDataBound="scheduleCellBound" AutoPaging="true">
							<Columns>
								<SOA:ClientGridColumn SelectColumn="true" ShowSelectAll="true" ItemStyle="{width:'50px',TEXT-ALIGN: 'center' }"
									HeaderStyle="{width:'50px'}" />
								<SOA:ClientGridColumn DataField="Name" HeaderText="计划名称" DataType="String" HeaderStyle="{width:'200px'}"
									ItemStyle="{width:'200px'}">
									<EditTemplate EditMode="None" />
								</SOA:ClientGridColumn>
								<SOA:ClientGridColumn DataField="Description" HeaderText="描述" HeaderStyle="{width:'600px'}"
									ItemStyle="{width:'600px'}">
									<EditTemplate EditMode="None" />
								</SOA:ClientGridColumn>
							</Columns>
						</SOA:ClientGrid>
					</fieldset>
				</div>
			</td>
		</tr>
		<tr>
			<td class="gridfileBottom" style="height: 40px; text-align: center; vertical-align: middle">
				<table style="width: 100%; height: 100%">
					<tr>
						<td style="text-align: center;">
							<input type="button" class="formButton" onclick="onOKBtnClick();" runat="server"
								value="确定(O)" id="btnOK" accesskey="O" />
							<SOA:SubmitButton runat="server" ID="btnConfirm" Style="display: none" OnClick="btnConfirm_Click"
								RelativeControlID="btnOK" PopupCaption="正在保存..." />
						</td>
						<td style="text-align: center;">
							<input type="button" class="formButton" onclick="top.close();" value="关闭(C)" id="btnCancel"
								accesskey="C" />
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
	<asp:HiddenField ID="hiddenJobText" runat="server" />
	<div style="display: none">
		<asp:Button ID="hiddenServerBtn" runat="server" Text="Button" OnClick="btnRefresh_Click" />
		<input id="hdServiceDefinition" runat="server" />
	</div>
	</form>
	<script src="../js/jquery-1.4.3.js" type="text/javascript"></script>
	<script src="JobEditor.js" type="text/javascript"></script>
	<script type="text/javascript" src="../js/common.js"></script>
	<script type="text/javascript">
		//task related
		var jobEditor;
		Sys.Application.add_load(function () {
			jobEditor = new JobSchedule.JobEditor(jQuery("#txtName"), jQuery("#txtDesc"), jQuery("#ddlEnabled"), $find("detailGrid"));
			var ctlType = jQuery('#ddlType');
			taskTypeChange();
			ctlType.change(taskTypeChange);

			allSvcOperationDef = $find('invokingServiceGrid').get_dataSource();
		});

		function taskTypeChange() {
			switch (jQuery('#ddlType').val()) {
				case '0':
					jQuery('#divStartWorkflow').show();
					jQuery('#divInvokingService').hide();
					break;
				case '1':
					jQuery('#divStartWorkflow').hide();
					jQuery('#divInvokingService').show();
					break;
				default:
					jQuery('#divStartWorkflow').hide();
					jQuery('#divInvokingService').hide();
					alert('不支持此类型作业！');
					break;
			}
		}

		function onOKBtnClick() {
			if ($get('txtName').value.trim() == '') {
				alert('请输入作业名称！');
				$get('txtName').focus();
				return false;
			}

			var taskType = $get('ddlType').value;

			if (taskType == '0') {
				if ($find('OuUserInputControl').get_selectedOuUserData().length == 0) {
					alert('请选择作业发起人！');
					return false;
				}
			}
			else if (taskType == '1') {
				if ($find('invokingServiceGrid').get_rowCount() == 0) {
					alert('请至少定义一个要执行的Web服务！');
					return false;
				}
				else {
					$get('hdServiceDefinition').value = Sys.Serialization.JavaScriptSerializer.serialize($find('invokingServiceGrid').get_dataSource())
				}
			}

			if ($find('detailGrid').get_rowCount() == 0) {
				alert('请至少选择一条执行计划！');
				return false;
			}
			$get("btnConfirm").click();
		}

		function openProcessList() {
			var url = "../modaldialog/WfProcessDescriptorInformationList.aspx?multiselect=false";
			var sFeature = "dialogWidth:800px; dialogHeight:680px;center:yes;help:no;resizable:yes;scroll:no;status:no";

			var result = window.showModalDialog(url, null, sFeature);
			if (result) {
				var processDescList = Sys.Serialization.JavaScriptSerializer.deserialize(result);
				if (processDescList.length != 1) {
					alert('请选择子流程！');
					return;
				}

				var processKeyControl = $get('ddlProcess');
				for (var i = 0; i < processKeyControl.options.length; i++) {
					if (processKeyControl.options[i].value == processDescList[0].Key) {
						processKeyControl.value = processDescList[0].Key;
						return;
					}
				}

				var item = document.createElement('option');
				item.value = processDescList[0].Key;
				item.text = processDescList[0].Key + '-' + processDescList[0].Name;
				processKeyControl.options.add(item);
				processKeyControl.value = item.value;
			}
		}
	</script>
	<script type="text/javascript">
		//job schedules related methods
		function openScheduleListDialog() {
			var sFeature = "dialogWidth:800px; dialogHeight:460px;center:yes;help:no;resizable:yes;scroll:no;status:no";
			var result;
			result = window.showModalDialog("ScheduleList.aspx", null, sFeature);

			if (result) {
				var schedules = Sys.Serialization.JavaScriptSerializer.deserialize(result.jsonStr);
				var currSchedules = $find("detailGrid").get_dataSource();
				if (schedules) {
					for (var i = 0; i < schedules.length; i++) {
						currSchedules.remove(schedules[i].ID, function (o, v) {
							if (o.ID == v) {
								return true;
							}
							return false;
						})

						currSchedules.push(schedules[i]);
					}
				}
				$find("detailGrid").set_dataSource(currSchedules);
				//jobEditor.Get_Job().Schedules.push(schedule);
			}
		}

		function openScheduleDialog(editKey) {
			var sFeature = "dialogWidth:800px; dialogHeight:460px;center:yes;help:no;resizable:yes;scroll:no;status:no";
			var result;
			var url = String.format("ScheduleEditor.aspx?scheduleId={0}", editKey || '');
			result = window.showModalDialog(url, null, sFeature);

			if (result) {
				var theOne = Sys.Serialization.JavaScriptSerializer.deserialize(result);
				var currSchedules = $find("detailGrid").get_dataSource();
				if (theOne) {
					var hitTest = false;
					for (var i = currSchedules.length - 1; i >= 0; i--) {
						if (currSchedules[i].ID == theOne.ID) {
							currSchedules[i] = theOne;
							hitTest = true;
							break;
						}
					}
					if (!hitTest) {
						currSchedules.push(theOne);
					}

				}
				$find("detailGrid").set_dataSource(currSchedules);
				//jobEditor.Get_Job().Schedules.push(schedule);
			}
		}

		function removeSchedule() {
			var schedules = $find("detailGrid").get_dataSource(); ;
			var selectedSchedules = $find("detailGrid").get_selectedData();

			if (selectedSchedules.length <= 0) {
				alert("请选择要删除的数据。");
				return;
			}
			if (!(selectedSchedules.length > 0 && confirm("确定删除？"))) {
				return;
			}
			for (var i = 0; i < selectedSchedules.length; i++) {
				if (Array.contains(schedules, selectedSchedules[i])) {
					Array.remove(schedules, selectedSchedules[i])
				}
			}
			$find("detailGrid").set_dataSource(schedules);
		}
	</script>
	<script type="text/javascript">
		//invoking service related methods
		var allSvcOperationDef = [];

		function openServiceDefDialog(key) {
			var url = "/MCSWebApp/WorkflowDesigner/ModalDialog/WfServiceOperationDefEditor.aspx?hasRtn=false";
			var sFeature = "dialogWidth:680px; dialogHeight:460px;center:yes;help:no;resizable:no;scroll:no;status:no";
			var result;

			if (typeof (key) === "number") {
				var def = allSvcOperationDef[key];

				result = window.showModalDialog(url,
                {
                	jsonStr: Sys.Serialization.JavaScriptSerializer.serialize(def),
                	existDefJsonStr: Sys.Serialization.JavaScriptSerializer.serialize(allSvcOperationDef)
                },
				sFeature);

			} else if ("" != key) {
				var opDef = allSvcOperationDef.get(key, function (o, v) {
					return o.Key == v;
				});

				result = window.showModalDialog(url,
                {
                	jsonStr: Sys.Serialization.JavaScriptSerializer.serialize(opDef),
                	existDefJsonStr: Sys.Serialization.JavaScriptSerializer.serialize(allSvcOperationDef)
                },
				sFeature);
			} else {
				result = window.showModalDialog(url,
                {
                	jsonStr: null,
                	existDefJsonStr: Sys.Serialization.JavaScriptSerializer.serialize(allSvcOperationDef)
                },
				sFeature);
			}

			if (result) {
				var resultObj = Sys.Serialization.JavaScriptSerializer.deserialize(result.jsonStr);
				if (resultObj) {
					var fnCompare = function (o, v) {
						return o.Key == v;
					};

					var isExist = allSvcOperationDef.has(resultObj.Key, fnCompare);

					if (isExist == false) {
						allSvcOperationDef.push(resultObj);
					}
					else {
						allSvcOperationDef.remove(resultObj.Key, fnCompare);
						allSvcOperationDef.push(resultObj);
					}
				}
				bindGrid(allSvcOperationDef);
			}
		}

		function bindGrid(dataSource) {
			var gridDatasource = dataSource ? dataSource : [];
			var grid = $find("invokingServiceGrid");
			grid.set_dataSource(gridDatasource);
			//grid.dataBind();
			grid = null;
			gridDatasource = null;
		}

		function createSvcOperation() {
			openServiceDefDialog("");
		}

		function removeInvokingService() {
			var grid = $find("invokingServiceGrid");
			var selectedData = grid.get_selectedData();
			if (selectedData.length <= 0)
				alert("请选择要删除的数据。");
			if (!(selectedData.length > 0 && confirm("确定删除？"))) {
				return;
			}
			for (var i = 0; i < selectedData.length; i++) {
				var element = selectedData[i];
				allSvcOperationDef.remove(element.Key, function (o, v) {
					return o.Key == v;
				});
			}
			bindGrid(allSvcOperationDef);
		}

		function getServiceItemLink(e, text) {
			var lnkNode;
			lnkNode = document.createElement("a");
			lnkNode.href = "javascript:void(0);";
			e.cell.replaceChild(lnkNode, e.cell.firstChild);
			lnkNode.appendChild(document.createTextNode(text));

			if (e.data["Key"]) {
				lnkNode.onclick = function () { eval("openServiceDefDialog('" + e.data["Key"] + "')") };
			} else {
				var ind = Array.indexOf(allSvcOperationDef, e.data);
				if (ind >= 0) {
					lnkNode.onclick = function () { eval("openServiceDefDialog(" + ind + ")"); };
				}
			}
			lnkNode = null;
		}

		function invokingServiceGridCellBound(g, e) {
			var dataFieldName = e.column.dataField;

			switch (dataFieldName) {
				case 'Key':
					getServiceItemLink(e, e.data["Key"] || "(未定义Key)");
					break;
				case 'AddressDef':
					getServiceItemLink(e, e.data[dataFieldName] == null ? "<没有地址>" : e.data[dataFieldName].Address.toString());
					break;
				case 'Params':
					e.cell.innerText = e.data["Params"].length;
					break;
			}

			lnkNode = null;
		}

		function scheduleCellBound(g, e) {
			var dataFieldName = e.column.dataField;

			switch (dataFieldName) {
				case 'Name':
					var linkText = "<a href='#' style='color: Black;' onclick='openScheduleDialog(\"{1}\");'>{0}</a>";

					if (typeof (e.data.ID) != "undefined")
						e.cell.innerHTML = String.format(linkText, e.data["Name"].toString(), e.data["ID"].toString());
					break;
				case "Description":
					linkText = "<a href='#' style='color: Black;' onclick='openScheduleDialog(\"{1}\");'>{0}</a>";

					if (typeof (e.data.ID) != "undefined")
						e.cell.innerHTML = String.format(linkText, e.data["Description"].toString(), e.data["ID"].toString());

					break;
			}
		}
	</script>
</body>
</html>
