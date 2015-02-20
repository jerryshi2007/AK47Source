<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WfVariableEditor.aspx.cs"
	Inherits="WorkflowDesigner.ModalDialog.WfVariableEditor" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>变量编辑</title>
	<script type="text/javascript" src="../js/jquery-1.4.3.js"></script>
	<script type="text/javascript" src="../js/jquery.validate.js"></script>
	<script type="text/javascript">

		var cnmsg =
        { required: "必须输入"
        }

		jQuery.extend(jQuery.validator.messages, cnmsg);

		var wfVariable = {};

		var existVars = {};
		function onDocumentLoad() {
			var paraData = window.dialogArguments;
			existVars = Sys.Serialization.JavaScriptSerializer.deserialize(paraData.existVarJsonStr);

			if (!paraData.jsonStr) {
				wfVariable = Sys.Serialization.JavaScriptSerializer.deserialize(document.getElementById("hiddenVariableTemplate").value);
			} else {
				wfVariable = Sys.Serialization.JavaScriptSerializer.deserialize(paraData.jsonStr);

				setPage(wfVariable.variableObj);
			}
			//            $.validator.addMethod('validateKey', function (v, e) { 
			//                return /(\'|\\|\/|\*|\:|\?|\"|\<|\>|\|)+/.test(v);
			//            }, 'Key中不能存在以下字符：\' \\ \/ \: \? \" \< \> \| \*');
		}

		function setPage(variableObj) {
			var varkey = document.getElementById("variableKey");
			varkey.disabled = true;
			var varName = document.getElementById("variableName");
			var varDesc = document.getElementById("variableDescription");
			var varValue = document.getElementById("variableValue");
			var varDatatype = document.getElementById("ddlVariableDatatype");
			varkey.value = variableObj.Key;
			varName.value = variableObj.Name;
			varDesc.value = variableObj.Description;
			varValue.value = variableObj.OriginalValue;
			varDatatype.value = variableObj.OriginalType;
		}

		function onbtnOKClick() {
			var varkey = document.getElementById("variableKey");
			var varName = document.getElementById("variableName");
			var varDesc = document.getElementById("variableDescription");
			var varValue = document.getElementById("variableValue");
			var varDatatype = document.getElementById("ddlVariableDatatype");

			if (!validate()) {
				return false;
			}
			if (wfVariable.variableObj) {
				wfVariable.variableObj.Key = varkey.value;
				for (var i = 0; i < wfVariable.variableObj.Properties.length; i++) {
					var prop = wfVariable.variableObj.Properties[i];
					if (prop.name == "Key") {
						prop.value = varkey.value;
					}
					if (prop.name == "Name") {
						prop.value = varName.value;
					}
					if (prop.name == "Description") {
						prop.value = varDesc.value;
					}
				}
				wfVariable.variableObj.Name = varName.value;

				wfVariable.variableObj.Description = varDesc.value;
				wfVariable.variableObj.OriginalValue = varValue.value;
				wfVariable.variableObj.OriginalType = varDatatype.value;
			} else {
				for (var i = 0; i < existVars.length; i++) {
					if (existVars[i].Key == varkey.value) {
						alert("变量Key重复。");
						return;
					}
				}

				wfVariable.Key = varkey.value;
				for (var i = 0; i < wfVariable.Properties.length; i++) {
					var prop = wfVariable.Properties[i];
					if (prop.name == "Key") {
						prop.value = varkey.value;
					}
					if (prop.name == "Name") {
						prop.value = varName.value;
					}
					if (prop.name == "Description") {
						prop.value = varDesc.value;
					}
				}
				wfVariable.Name = varName.value;
				wfVariable.Description = varDesc.value;
				wfVariable.OriginalValue = varValue.value;
				wfVariable.OriginalType = varDatatype.value;
			}
			window.returnValue = { jsonStr: Sys.Serialization.JavaScriptSerializer.serialize(wfVariable) };
			top.close();
		}

		function validate() {
			var boolResult;
			boolResult = ($("#form1").validate().element("#variableKey") &&
            $("#form1").validate().element("#variableValue") &&
            $("#form1").validate().element("#ddlVariableDatatype")
            )
			var varKey = $('#variableKey').val()
			var reg = /(\'|\\|\/|\*|\:|\?|\"|\<|\>|\|)+/

			if (reg.test(varKey)) {
				alert('Key中不能存在以下字符：\' \\ \/ \: \? \" \< \> \| \*')
				boolResult = false;
			}

			return boolResult;
		}
	</script>
</head>
<body onload="onDocumentLoad();">
	<form id="form1" runat="server">
	<asp:ScriptManager ID="ScriptManager1" runat="server">
	</asp:ScriptManager>
	<table width="100%" style="height: 100%; width: 100%">
		<tr>
			<td class="gridHead">
				<div class="dialogTitle">
					<span class="dialogLogo">变量编辑</span>
				</div>
			</td>
		</tr>
		<tr>
			<td style="vertical-align: center">
				<div class="dialogContent" id="dialogContent" runat="server" style="width: 100%;
					height: 100%; overflow: auto">
					<!--Put your dialog content here... -->
					<table width="100%" style="height: 100%; width: 100%;">
						<tr style="height: 10px;">
							<td style="width: 100px;">
							</td>
							<td>
							</td>
						</tr>
						<tr>
							<td class="label" valign="middle">
								Key:
							</td>
							<td valign="middle">
								<input id="variableKey" class="required validateKey" />
							</td>
						</tr>
						<tr>
							<td class="label" valign="middle">
								名称:
							</td>
							<td valign="middle">
								<input id="variableName" />
							</td>
						</tr>
						<tr>
							<td class="label" valign="middle">
								类型:
							</td>
							<td valign="middle">
								<cc1:HBDropDownList ID="ddlVariableDatatype" runat="server" class="required">
								</cc1:HBDropDownList>
							</td>
						</tr>
						<tr>
							<td class="label" valign="middle">
								值:
							</td>
							<td valign="middle">
								<input id="variableValue" style="width: 256px" class="required" />
							</td>
						</tr>
						<tr>
							<td class="label" valign="middle">
								描述:
							</td>
							<td valign="middle">
								<input id="variableDescription" style="width: 256px" />
							</td>
						</tr>
					</table>
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
							<input id="btnOK" type="button" value="确定(O)" accesskey="O" onclick="onbtnOKClick();"
								class="formButton" />
						</td>
						<td style="text-align: center;">
							<input id="btnCancel" type="button" value="取消(C)" accesskey="C" onclick="top.close();"
								class="formButton" />
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
	<input type="hidden" runat="server" id="hiddenVariableTemplate" />
	</form>
</body>
</html>
