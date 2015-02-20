<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BranchProcessTemplateEditorNew.aspx.cs"
	Inherits="WorkflowDesigner.ModalDialog.BranchProcessTemplateEditorNew" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="soa" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<meta http-equiv="Expires" content="0" />
	<meta http-equiv="Cache-Control" content="no-cache" />
	<meta http-equiv="Pragma" content="no-cache" />
	<title>分支流程模版编辑</title>
	<style type="text/css">
		.branch__propertyGrid_Editor_valueCell
		{
			text-align: right;
			width: 40px;
			height: 22px;
			display: inline-block;
		}
		.branch_propertyGrid_Editor_button
		{
			width: 40px;
			height: 22px;
			cursor: hand;
		}
		.branch_propertyGrid_Editor_Resource
		{
			border-style: none none solid none;
			border-bottom-color: #C0C0C0;
			border-bottom-width: thin;
		}
		.tableContent TR TD
		{
			height: 40px;
		}
		.labelTD
		{
			text-align: right;
			padding-right: 4px;
		}
		.inputTD
		{
			text-align: left;
		}
		A
		{
			color: #51bfe0;
			text-decoration: none;
		}
		A:hover
		{
			color: #990099;
			text-decoration: underline;
			font-weight: bold;
		}
		SELECT
		{
			width: 250px;
		}
	</style>
	<script type="text/javascript">
		var branchProcessTemp = {};
		var activities;
		var existBranProcTemp = {};
		//由于流程设计器中用的ShortName 而此处用的是 value(int)
		var subProcessApprovalModeEnumList = [];
		var subProcessResourceModeEnumList = [];

		function onDocumentLoad(sender, args) {
			// var paraData = { jsonStr: null, activities: [], existBranProcessTempJsonStr: "[]" };
			var paraData = window.dialogArguments;
			existBranProcTemp = Sys.Serialization.JavaScriptSerializer.deserialize(paraData.existBranProcessTempJsonStr);
			if (!paraData.jsonStr) {
				branchProcessTemp = Sys.Serialization.JavaScriptSerializer.deserialize(document.getElementById("hiddenBranchProcessTemplate").value);
			} else {
				branchProcessTemp = Sys.Serialization.JavaScriptSerializer.deserialize(paraData.jsonStr);
			}
			activities = paraData.activities;

			subProcessApprovalModeEnumList = [{ TextValue: "NoActivityDecide", NnumberValue: 0 }, { TextValue: "AnyActivityDecide", NnumberValue: 1 }, { TextValue: "LastActivityDecide", NnumberValue: 2}];
			subProcessResourceModeEnumList = [{ TextValue: "DependsOnProcess", NnumberValue: 0 }, { TextValue: "SameWithRoot", NnumberValue: 1 }, { TextValue: "NewCreate", NnumberValue: 2}];

			bindPropertyGrid();
		}

		function findEnumListValue(enumList, itemvalue) {
			var result = itemvalue;
			for (var i = 0; i < enumList.length; i++) {
				var item = enumList[i];
				if (item.TextValue == itemvalue) {
					result = item.NnumberValue;
					break;
				}
				if (item.NnumberValue == itemvalue) {
					result = item.TextValue;
					break;
				}
			}
			return result;
		}

		function bindPropertyGrid() {
			var properties = new Array();
			if (branchProcessTemp.branchProcessTempObj) {
				properties = branchProcessTemp.branchProcessTempObj.Properties;
			}
			else {
				properties = branchProcessTemp.Properties;
			}
			var currentProperties = new Array();
			var currentIitem = new Object();

			for (var i = 0; i < properties.length; i++) {
				var prop = properties[i];
				currentIitem[prop.name] = prop.value;
			}

			var operationDefinitionIsnull = false;

			if (Object.prototype.toString.apply(currentIitem.OperationDefinition) === "[object Object]") {
				if (currentIitem.OperationDefinition) {
					operationDefinitionIsnull = true;
				}
			}
			else if (Object.prototype.toString.apply(currentIitem.OperationDefinition) === "[object String]") {
				if (currentIitem.OperationDefinition != "") {
					currentIitem.OperationDefinition = Sys.Serialization.JavaScriptSerializer.deserialize(currentIitem.OperationDefinition);
					if (currentIitem.OperationDefinition) {
						operationDefinitionIsnull = true;
					}
				}
			}

			for (var i = 0; i < properties.length; i++) {
				var prop = properties[i];
				switch (prop.name) {
					case "Enabled":
						prop.category = "默认值";
						prop.visible = false;
						break;
					case "Description":
						prop.category = "默认值";
						prop.visible = false;
						break;
					case "Name":
						prop.category = "分支流程";
						prop.description = "分支流程模板名称";
						prop.displayName = "分支流程模板名称";
						prop.type = "Enum";
						prop.editorKey = "StandardPropertyEditor";
						prop.sortOrder = 4;
						break;
					case "Key":
						prop.category = "分支流程";
						prop.description = "流程Key";
						prop.editorKey = "StandardPropertyEditor";
						prop.displayName = "流程Key";
						prop.sortOrder = 1;
						if (!prop.value || prop.value == "") {
							prop.readOnly = false;
						}
						// prop.
						break;
					case "CreateResourceMode":
						if (isNaN(prop.value) == false) {
							prop.value = findEnumListValue(subProcessResourceModeEnumList, prop.value);
						}
						break;
					case "SubProcessApprovalMode":
						if (isNaN(prop.value) == false) {
							prop.value = findEnumListValue(subProcessApprovalModeEnumList, prop.value);
						}
						break;
					case "GenerateType":

						if (operationDefinitionIsnull) {
							prop.value = 1;
						}
						break;
					case "OperationDefinition":
						if (operationDefinitionIsnull) {
							prop.visible = true;
						} else {
							prop.visible = false;
						}
						break;
					case "BranchProcessKey":
						if (parseInt(currentIitem.GenerateType) == 0) {
							prop.visible = true;
						} else {
							prop.value = "";
							prop.visible = false;
						}
						break;
					case "BlockingType":
						prop.value = parseInt(prop.value);
						prop.defaultValue = parseInt(prop.defaultValue);
						prop.de
						break;
					case "ExecuteSequence":
						prop.value = parseInt(prop.value);
						prop.defaultValue = parseInt(prop.defaultValue);
						break;
					case "IndependentOpinion":
						var item = prop.value;
						break;
				}
				currentProperties.push(prop);
			}

			var propGridControl = $find("propertyGrid");
			propGridControl.set_properties(currentProperties);
			propGridControl.dataBind();
		}

		function onClickEditor(sender, e) {
			var activeEditor = sender.get_activeEditor();
			var item = activeEditor.get_property();

			switch (item.name) {
				case "Resources":
					openResourceEditor(activeEditor, item);
					break;
				case "BranchProcessKey":
					openProcessListEditor(activeEditor);
					break;
				case "Condition":
					openConditionEditor(activeEditor, item);
					break;
				case "OperationDefinition":
					openServeiceOperationEditor(activeEditor, item);
					break;
				case "CancelSubProcessNotifier":
					openResourceEditor(activeEditor, item);
					break;
				case "RelativeLinks":
					openRelativeLinkEditor(activeEditor, item);
					break;
			}
		}

		function openResourceEditor(activeEditor, item) {
			var arrResources = [];

			if (Object.prototype.toString.apply(item.value) === "[object String]") {
				arrResources = item.value;
			} else if (Object.prototype.toString.apply(item.value) === "[object Object]" || Object.prototype.toString.apply(item.value) === "[object Array]") {
				arrResources = Sys.Serialization.JavaScriptSerializer.serialize(arrResources);
			}

			var paraObj = {
				jsonStr: arrResources,
				Activities: activities
			};
			var sFeature = "dialogWidth:800px; dialogHeight:600px;center:yes;help:no;resizable:no;scroll:no;status:no";
			var result = window.showModalDialog("WfResourceEditor.aspx", paraObj, sFeature);

			if (result) {
				activeEditor.commitValue(result.jsonStr);
			}
		}

		function openConditionEditor(activeEditor, item) {
			var condition = {};

			if (Object.prototype.toString.apply(item.value) === "[object String]") {
				condition = item.value;
			} else if (Object.prototype.toString.apply(item.value) === "[object Object]" || Object.prototype.toString.apply(item.value) === "[object Array]") {
				condition = Sys.Serialization.JavaScriptSerializer.serialize(arrResources);
			}

			var url = "WfConditionEditor.aspx";
			var sFeature = "dialogWidth:500px; dialogHeight:300px;center:yes;help:no;resizable:no;scroll:no;status:no";
			var paraObj = { jsonStr: condition };

			var result = window.showModalDialog(url, paraObj, sFeature);

			if (result) {
				activeEditor.commitValue(result.jsonStr);
			}
		}

		function openProcessListEditor(activeEditor) {
			var url = "WfProcessDescriptorInformationList.aspx?multiselect=false";
			var sFeature = "dialogWidth:800px; dialogHeight:680px;center:yes;help:no;resizable:yes;scroll:no;status:no";

			var result = window.showModalDialog(url, null, sFeature);

			if (result) {
				var processDescList = Sys.Serialization.JavaScriptSerializer.deserialize(result);
				if (processDescList.length != 1) {
					alert('请选择子流程！');
					return;
				}
				activeEditor.commitValue(processDescList);
			}
		}

		function openRelativeLinkEditor(activeEditor, item) {
			var arrLinks = [];

			if (Object.prototype.toString.apply(item.value) === "[object String]") {
				arrLinks = item.value;
			} else if (Object.prototype.toString.apply(item.value) === "[object Object]" || Object.prototype.toString.apply(item.value) === "[object Array]") {
				arrLinks = Sys.Serialization.JavaScriptSerializer.serialize(arrLinks);
			}

			var url = "WfRelativeLinkEditor.aspx";
			var commonFeature = "dialogWidth:800px; dialogHeight:460px;center:yes;help:no;resizable:yes;scroll:no;status:no";

			var linkArg = { jsonStr: arrLinks };

			var result = window.showModalDialog(url, linkArg, commonFeature);

			if (result) {
				activeEditor.commitValue(result.jsonStr);
			}
		}

		function openServeiceOperationEditor(activeEditor, item) {
			var key = Sys.Serialization.JavaScriptSerializer.deserialize(item.value);
			var url = "WfServiceOperationDefEditor.aspx?hasRtn=false&initPara=serviceOP_Paramas";
			var sFeature = "dialogWidth:680px; dialogHeight:460px;center:yes;help:no;resizable:no;scroll:no;status:no";
			var result;
			if (key) {
				result = window.showModalDialog(url,
                { jsonStr: Sys.Serialization.JavaScriptSerializer.serialize(key),
                	existDefJsonStr: Sys.Serialization.JavaScriptSerializer.serialize([])
                }, sFeature);
			} else {
				result = window.showModalDialog(url,
                { jsonStr: null,
                	existDefJsonStr: Sys.Serialization.JavaScriptSerializer.serialize([])
                }, sFeature);
			}

			if (result) {
				var resultObj = Sys.Serialization.JavaScriptSerializer.deserialize(result.jsonStr);
				if (resultObj) {
					if (branchProcessTemp.branchProcessTempObj) {
						branchProcessTemp.branchProcessTempObj.OperationDefinition = resultObj;
					} else {
						branchProcessTemp.OperationDefinition = resultObj;
					}
				}
				activeEditor.commitValue(result.jsonStr);
			}
		}

		function onClick() {
			var result = new Object();
			var properties = $find("propertyGrid").get_properties();
			for (var i = 0; i < properties.length; i++) {
				var prop = properties[i];
				result[prop.name] = prop.value;
			}

			if (result.ProcessKey == "") {
				alert("请输入流程Key");
				return;
			}

			if (result.GenerateType == "1") {
				//调用服务方式
				if (branchProcessTemp.branchProcessTempObj) {
					if (branchProcessTemp.branchProcessTempObj.OperationDefinition == null) {
						alert('请配置需要调用的服务');
						return;
					}
				}
				else {
					if (branchProcessTemp.OperationDefinition == null) {
						alert('请配置需要调用的服务');
						return;
					}
				}
			}

			if (result.BlockingType == null || result.BlockingType == undefined) {
				alert("请选择阻塞类型");
				return;
			}

			if (result.ExecuteSequence == null || result.ExecuteSequence == undefined) {
				alert("请选择串行/并行类型");
				return;
			}

			if (parseInt(result.CreateResourceMode) == NaN) {
				alert("请选择ResourceID生成方式");
				return;
			}

			if (parseInt(result.CreateResourceMode) == NaN) {
				alert("请选择审批流审批方式");
				return;
			}


			if (branchProcessTemp.branchProcessTempObj) {
				branchProcessTemp.branchProcessTempObj.Properties = properties;
				collectRtnData(branchProcessTemp.branchProcessTempObj, result);
			} else {
				for (var i = 0; i < existBranProcTemp.length; i++) {
					if (existBranProcTemp[i].Key == result.ProcessKey) {
						alert("分支流程模板Key重复。");
						return;
					}
				}
				branchProcessTemp.Properties = properties;
				collectRtnData(branchProcessTemp, result);
			}
			window.returnValue = { jsonStr: Sys.Serialization.JavaScriptSerializer.serialize(branchProcessTemp) };

			top.close();
		}

		function collectRtnData(targetObj, convertTemp) {
			if (convertTemp.GenerateType == "0") {
				targetObj.OperationDefinition = null;
			}
			else {
				targetObj.BranchProcessKey = null;
			}
			targetObj.Name = convertTemp.Name;
			// targetObj.Key = convertTemp.BranchProcessKey;  // processKey.value;

			for (var i = 0; i < targetObj.Properties.length; i++) {
				var prop = targetObj.Properties[i];
				switch (prop.name) {
					case "Key":
						// = convertTemp.ProcessKey;
						targetObj.Key = prop.value;
						break;
					case "BranchProcessKey":
						if (convertTemp.GenerateType == "0") {
							prop.value = convertTemp.BranchProcessKey; // processKeyList.value;
							targetObj.BranchProcessKey = convertTemp.BranchProcessKey;
						} else {
							prop.value = "";
						}
						break;
					case "Name":
						targetObj.BranchProcessName = prop.value;
						break;
					case "CreateResourceMode":
						prop.value = findEnumListValue(subProcessResourceModeEnumList, convertTemp.CreateResourceMode);
						break;
					case "SubProcessApprovalMode":
						prop.value = findEnumListValue(subProcessApprovalModeEnumList, convertTemp.SubProcessApprovalMode);
						break;
					//                    case "ExecuteSequence":                             
					//                        targetObj.ExecuteSequence = prop.value;                             
					//                        break;                             
					default:
						if (convertTemp.hasOwnProperty(prop.name)) {
							prop.value = convertTemp[prop.name];
						}
						break;
				}
			} //end for

			if (branchProcessTemp.branchProcessTempObj) {
				branchProcessTemp.branchProcessTempObj.Resources = convertTemp.Resources;
				branchProcessTemp.branchProcessTempObj.Condition = convertTemp.Condition;
			} else {
				branchProcessTemp.Resources = convertTemp.Resources;
				branchProcessTemp.Condition = convertTemp.Condition;
			}

			targetObj.BlockingType = parseInt(convertTemp.BlockingType);
			targetObj.ExecuteSequence = parseInt(convertTemp.ExecuteSequence);
		}

		function onDocumentUnLoad() {

			delete branchProcessTemp;
			delete activities;
			delete existBranProcTemp;

			delete subProcessApprovalModeEnumList;
			delete subProcessResourceModeEnumList;
		}

	</script>
</head>
<body>
	<form id="form1" runat="server">
	<asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True">
	</asp:ScriptManager>
	<table width="100%" style="width: 100%; height: 100%">
		<tr>
			<td class="gridHead">
				<div class="dialogTitle">
					<span class="dialogLogo">分支流程模版编辑</span>
				</div>
			</td>
		</tr>
		<tr>
			<td style="vertical-align: top">
				<div class="dialogContent" id="dialogContent" runat="server" style="width: 100%;
					height: 100%; overflow: auto">
					<soa:PropertyGrid runat="server" OnClientClickEditor="onClickEditor" ID="propertyGrid"
						Width="100%" Height="100%" DisplayOrder="ByAlphabet" />
				</div>
			</td>
		</tr>
		<tr>
			<td class="gridfileBottom">
			</td>
		</tr>
		<tr>
			<td style="height: 40px; text-align: center; vertical-align: middle">
				<table style="width: 100%; height: 100%">
					<tr>
						<td style="text-align: center;">
							<input type="button" id="confirmButton" value="确定(O)" accesskey="O" class="formButton"
								onclick="onClick();" />
						</td>
						<td style="text-align: center;">
							<input type="button" onclick="top.close();" value="取消(C)" id="btnCancel" accesskey="C"
								class="formButton" />
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
	<input type="hidden" runat="server" id="hiddenBranchProcessTemplate" />
	<%--<input type="hidden" runat="server" id="hiddenBrnachConfigProperties" />--%>
	<input type="hidden" runat="server" id="hiddenKey" />
	<script type="text/javascript" src="../js/WFObjectListPropertyEditor.js"></script>
	<script type="text/javascript" src="../js/BranchProcessPropertyEditor.js">
	</script>
	</form>
	<script type="text/javascript">
		Sys.Application.add_load(onDocumentLoad);

		Sys.Application.add_unload(onDocumentUnLoad);
	</script>
</body>
</html>
