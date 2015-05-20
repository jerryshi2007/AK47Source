<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="WfFormTemplate._default" %>

<%@ Register TagPrefix="CCPC" Namespace="MCS.Library.Web.Controls" Assembly="MCS.Library.Passport" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>入口</title>
	<script type="text/javascript">
		function onStartFreeProcess() {
			event.returnValue = false;
			window.open(event.srcElement.href, "selectApprover", 'height=300px, width=600px, top=100, left=300, toolbar=no, menubar=no, scrollbars=no, resizable=yes,location=no, status=no');
			return false;
		}

		function onStartSelectedProcess() {
			event.returnValue = false;

			var url = "/MCSWebApp/WorkflowDesigner/modaldialog/WfProcessDescriptorInformationList.aspx?multiselect=false";
			var sFeature = "dialogWidth:800px; dialogHeight:600px;center:yes;help:no;resizable:yes;scroll:no;status:no";

			var result = window.showModalDialog(url, null, sFeature);
			if (result) {
				var processDescList = Sys.Serialization.JavaScriptSerializer.deserialize(result);

				if (processDescList.length > 0) {
					var key = processDescList[0].Key;

					window.open("Forms/TemplateController.ashx?processDescKey=" + key, "_blank", 'height=600px, width=800px, top=100, left=300, toolbar=no, menubar=no, scrollbars=no, resizable=yes,location=no, status=no');
				}
			}

			return false;
		}

		function onStartAdministrativeProcess() {
			event.returnValue = false;
			window.open(event.srcElement.href, "startAdministrativeUnitProcess", 'height=300px, width=600px, top=100, left=300, toolbar=no, menubar=no, scrollbars=no, resizable=yes,location=no, status=no');
			return false;
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<asp:ScriptManager runat="server" EnableScriptGlobalization="true">
		</asp:ScriptManager>
	</div>
	<div>
		<CCPC:SignInLogoControl runat="server" ID="SignInLogo" ReturnUrl="~/Default.aspx"
			AutoRedirect="True" />
	</div>
	<div style="height: 40px">
	</div>
	<div>
		<ul>
			<li><a href="Forms/TemplateController.ashx" target="selectApprover" onclick="onStartFreeProcess()">
				启动动态流程</a></li>
			<li><a href="Forms/StartAdministrativeUnitProcess.aspx?ru=/MCSWebApp/WfFormTemplate/Forms/TemplateController.ashx"
				target="selectApprover" onclick="onStartAdministrativeProcess()">启动管理单元流程</a></li>
			<li><a href="#" target="selectApprover" onclick="onStartSelectedProcess()">启动已存在的流程</a></li>
		</ul>
	</div>
	<div style="height: 40px">
	</div>
	<div>
		<ul>
			<li><a href="/MCSWebApp/MCSOAPortal/default.aspx" target="WorkflowCenter">流程中心</a></li>
			<li><a href="/MCSWebApp/PermissionCenter" target="PermissionCenter">权限中心</a></li>
			<li><a href="/MCSWebApp/WorkflowDesigner/Default.aspx" target="WorkflowDesigner">流程模版</a></li>
			<li><a href="List/UnCompletedTaskList.aspx">待办</a></li>
			<li><a href="/MCSWebApp/MCSOAPortal/TaskList/CompletedTaskList.aspx?process_status=Running">流转中</a></li>
			<li><a href="/MCSWebApp/WorkflowDesigner/PlanScheduleDialog/TaskMonitor.aspx" target="TaskMonitor">任务监控</a></li>
		</ul>
	</div>
	</form>
</body>
</html>
