<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SystemAdmin.aspx.cs" Inherits="MCS.OA.Portal.frames.SystemAdmin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>系统管理</title>
	<script type="text/javascript" src="../javascript/taskLink.js"></script>
	<style type="text/css">
		.unUse
		{
			color: #ddd;
			cursor: no-drop;
		}
		.ddItem
		{
			float: left;
		}
		.ddItem *
		{
			width: 100%;
			float: left;
		}
		.ddItem li
		{
			width: 110px;
			background: url(../img/bg_ul.gif) center no-repeat;
			line-height: 30px;
			margin: 1px;
			float: left;
			text-align: center;
		}
		.ddItem p
		{
			height: 30px;
			line-height: 30px;
			text-indent: 2em;
			width: 100%;
			padding-left: 3px;
		}
		.ddItem p a
		{
			width: 120px;
			background: url(../img/icon1.gif) left no-repeat;
			text-indent: 24px;
		}
		.tagContent
		{
			display: none;
		}
		#tagContent .selectTag1
		{
			display: block;
		}
	</style>
	<link rel="Stylesheet" href="../css.css" />
	<script type="text/javascript">
		function onDraftLinkClick() {
			var a = event.srcElement;

			event.returnValue = false;

			var feature = "height=600,width=800,status=no,resizable=yes,toolbar=no,menubar=no,location=no,scrollbars=yes";

			window.open(a.href, "_blank", feature);
			event.cancelBubble = true;
		}

		function selectTag(showContent, selfObj) {
			// 操作标签
			var tag = document.getElementById("tags").getElementsByTagName("li");

			for (var i = 0; i < tag.length; i++) {
				tag[i].className = "";
			}

			selfObj.parentNode.className = "selectTag";

			// 操作内容
			for (var i = 0; j = document.getElementById("tagContent" + i); i++) {
				j.style.display = "none";
			}

			document.getElementById(showContent).style.display = "block";
		}

		//测试临时流程选择
		function onSelectProcessUsers() {
			event.returnValue = false;

			window.open("../../WebTestProject/workflow/SelectProcessUsers.aspx", "selectApprover", 'height=500px, width=600px, top=100, left=300, toolbar=no, menubar=no, scrollbars=no, resizable=no,location=no, status=no');
		}

		function onGlobalSettingsClick() {
			event.returnValue = false;
			var feature = "dialogWidth:720px; dialogHeight:540px;center:yes;help:no;resizable:yes;scroll:no;status:no";

			window.showModalDialog(event.srcElement.href, null, feature);

			return false;
		}
	</script>
</head>
<body style="background-color: #f8f8f8">
	<div id="topContainer" style="width: 99%; margin: 8px 0 0 5px; border: solid 1px #ddd;
		background-color: White;">
		<div style="width: 100%; text-indent: 2em; font-size: 11pt; font-weight: 600; background: url(../images/h2_icon.gif) no-repeat 5px 5px;
			line-height: 30px; padding-bottom: 0px; border-bottom: solid 1px silver;">
			<asp:Label ID="LblTitle" runat="server" Text="系统管理"></asp:Label>
		</div>
		<div id="mainframe" style="width: 100%;">
			<dl>
				<dt>服务流程</dt>
				<dd class="ddItem" style="width: 100%; background-color: White;">
					<ul id="tags">
						<li class="selectTag"><a onclick="selectTag('tagContent0',this)" href="#">流程管理</a></li>
						<li><a onclick="selectTag('tagContent2',this)" href="#">用户和权限中心</a></li>
						<li><a onclick="selectTag('tagContent1',this)" href="#">日志和监控</a></li>
					</ul>
					<div id="tagContent">
						<div class="tagContent selectTag1" id="tagContent0">
							<p>
								<a href="/MCSWebApp/WorkflowDesigner/default.aspx" target="_blank">流程模板定义</a> <a
									href="/MCSWebApp/WorkflowDesigner/modaldialog/ExtGlobalParametersEditor.aspx"
									onclick="onGlobalSettingsClick();">全局设置</a> <a id="ADMINISTRATION2" href="/MCSWebApp/WorkflowDesigner/PlanScheduleDialog/JobList.aspx?showEditBtn=false">
										作业定义</a> <a id="ADMINISTRATION3" href="/MCSWebApp/WorkflowDesigner/PlanScheduleDialog/ScheduleList.aspx?showEditBtn=false">
											计划定义</a> <a id="taskList" href="/MCSWebApp/WorkflowDesigner/PlanScheduleDialog/TaskMonitor.aspx">
												任务监控</a> <a id="A1" href="/MCSWebApp/OACommonPages/AppTrace/ProcessAdjustment.aspx">
													流程调整</a><a href="/MCSWebApp/OACommonPages/AsyncPersist/Default.aspx">流程持久化任务</a>
													<a href="/MCSWebApp/OACommonPages/AppTrace/Category.aspx">分类权限查询</a>
							</p>
						</div>
						<div class="tagContent" id="tagContent1">
							<p>
								<a href="/MCSWebApp/OACommonPages/UserOperationLog/UserOperationLogView.aspx">操作日志</a><a
									style="width: 120px" href="/MCSWebApp/OACommonPages/ThreadStatus/ServiceThreadStatus.aspx">服务进程</a></p>
						</div>
						<div class="tagContent" id="tagContent2">
							<p>
								<a href="/MCSWebApp/PermissionCenter/default.aspx" target="_blank">机构人员管理</a> <a
									href="/MCSWebApp/PermissionCenter/lists/AllApps.aspx" target="_blank">授权管理</a>
								<a href="/MCSWebApp/OACommonPages/WFGroup/WFGroupManager.aspx">群组管理</a> <a href="/MCSWebApp/OACommonPages/WFPost/WFPostManager.aspx">
									岗位管理</a> <a href="/MCSWebApp/MCSOAPortal/Mechanism/RemoveMechanismCache.aspx">清除缓存</a>
								<a href="/MCSWebApp/MCSOAPortal/ReplaceAssignee/ReplaceAssigneeList.aspx">修改待办人</a>
								<a href="/MCSWebApp/SinoOcean.Seagull2.PersonnelAdjustment/MemberRole/MemberRoleView.aspx">批量替换角色成员</a>
								<a href="/MCSWebApp/SinoOcean.Seagull2.PersonnelAdjustment/MatrixRole/MemberMatrix.aspx">批量替换矩阵成员</a>
								<a href="/MCSWebApp/SinoOcean.Seagull2.PersonnelAdjustment/ProcessDefinitionImpl/FlowList.aspx">批量替换固定成员</a>
							</p>
						</div>
					</div>
				</dd>
			</dl>
		</div>
		<div class="clear">
		</div>
	</div>
</body>
</html>
