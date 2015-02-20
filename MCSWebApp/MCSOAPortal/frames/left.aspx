<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="left.aspx.cs" Inherits="MCS.OA.Portal.frames.left" %>

<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="CCIC" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="HB" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>菜单和待办入口</title>
	<link href="../css.css" rel="Stylesheet" type="text/css" />
	<script type="text/javascript" src="../JavaScript/taskLink.js"></script>
	<script language="javascript" type="text/javascript">
    
       <!--
		/*第一种形式 第二种形式 更换显示样式*/
		function setTab(name, cursel, n) {
			for (i = 1; i <= n; i++) {
				var menu = document.getElementById(name + i);
				menu.className = i == cursel ? "MenuClick" : "";
			}
		}
		//-->

		/*   var menuData = null; //菜单数据
		function onMenuLinkClick() {
		var o = event.srcElement;
		var mdata;
		var menu = $find('dMenu');
		event.returnValue = false;

		while (o && o.tagName.toLowerCase() != "a")
		o = o.parentElement;


		if (menuData == null) {
		//反序列菜单数据
		menuData = Sys.Serialization.JavaScriptSerializer.deserialize($get('HiddenField_Menu').value);
		}
		for (var i = 0; i < menuData.length; i++) {
		if (menuData[i].nodeID == o.menuID) {
		mdata = menuData[i];
		menu._id = o.menuID;
		break;
		}
		}
		addItemToMenu(o, menu, mdata);
		//showLinkMenu(o,menu);
		}

		//添加菜单项
		function addItemToMenu(a, menu, data) {
		if (menu && data) {
		menu._childNodeContainer = null;
		menu._items = new Array();
		menu._popupChildControl = $create($HGRootNS.PopupControl, { positionElement: a, positioningMode: $HGRootNS.PositioningMode.RightTop }, { "beforeShow": menu._showEvents }, null, null);
		menu._initDynamicChildNodeContainer();
		menu._children = new Array();
		menu._buildItems(data.childItems, menu);
		menu._popupChildControl.show();
		}
		} */

		//		function showLinkMenu(a,menu)
		//		{
		//		    event.returnValue = false;
		//			
		//			if (a)
		//			{
		//				//changed by xuwenzhuo ！更正菜单位置，使它附加到点击控件的旁边。
		//				var frameHeight = 0;;
		//                var bounds  = Sys.UI.DomElement.getBounds(a);

		//                if(window.parent && window.parent.document.frames && window.parent.document.frames.length>=3)
		//					frameHeight = window.parent.document.frames[0].document.body.clientHeight + window.parent.document.frames[1].document.body.clientHeight;

		//                if (menu)
		//					menu.showPopupMenu(bounds.x + bounds.width,bounds.y + bounds.height + frameHeight);		
		//			}
		//		}

		/*   function onPopupMenuClick() {
		var xmlNode = event.data;

		if (xmlNode) {
		var target = xmlNode.getAttribute("target");

		if (target != "" && target != "detail")
		openWindow(xmlNode.getAttribute("url"), xmlNode.getAttribute("target"), xmlNode.getAttribute("feature"));
		else {
		innerAnchor.href = xmlNode.getAttribute("url");

		try {
		innerAnchor.click();
		}
		catch (e) {
		}
		}
		}
		}

		function openWindow(url, target, feature) {
		if (feature != null && feature != "") {
		var screenHeight = window.screen.height;
		var screenWidth = window.screen.width;

		feature = feature.toLowerCase();

		var temps = feature.split(',');
		var tempHeights = temps[0].split('=');
		var tempHeight = tempHeights[1];

		var tempWidths = temps[1].split('=');
		var tempWidth = tempWidths[1];

		tempTop = (screenHeight - tempHeight) / 2;
		tempTop = tempTop * 0.8;
		tempLeft = (screenWidth - tempWidth) / 2;

		feature += ",top=" + tempTop + ",left=" + tempLeft;
		}

		var w = window.open(url, target, feature);

		if (w)
		w.focus();
		}*/

		/*  function onCaptionMouseOver() {
		var oTD = getCaptionTD(event.srcElement);

		if (oTD)
		oTD.className = 'bgMouseOn';
		}

		function onCaptionMouseOut() {
		var oTD = getCaptionTD(event.srcElement);

		if (oTD)
		oTD.className = 'bgMouseOut';
		}

		function getCaptionTD(src) {
		var oTD = src;

		while (oTD && (oTD.tagName.toLowerCase() != "table"))
		oTD = oTD.parentElement;

		return oTD;
		}*/

		function onUserSettingClick() {
			var a = getOwnerTag(event.srcElement, "A");
			event.returnValue = false;

			var sFeature = "dialogWidth:640px; dialogHeight:480px;center:yes;help:no;resizable:no;scroll:no;status:no;menubar:no;";

			showModalDialog(a.href, null, sFeature);
		}

		function onUserDelegationClick() {
			var a = getOwnerTag(event.srcElement, "A");
			event.returnValue = false;

			if (!a.disabled) {
				var sFeature = "dialogWidth: 500px; dialogHeight: 400px; center: yes; help:no; resizable: no; scroll: no; status: no;menubar:no;";
				showModalDialog(a.href, null, sFeature);
			}
		}

		/*   function hideTopMenu() {
		if (expandImage.style.display == 'none') {
		topMenu.style.display = 'none';
		expandImage.style.display = 'inline';
		collapseImage.style.display = 'none';
		}
		else {
		topMenu.style.display = 'inline';
		expandImage.style.display = 'none';
		collapseImage.style.display = 'inline';
		}
		}*/

		function setBan(nTotal, nExpired) {
			var strTitle = "";

			if (nTotal > 0) {
				strTitle = "您有" + nTotal + "项待办事项";

				if (nExpired > 0)
					strTitle += "，其中" + nExpired + "项已过期";

				taskCount.innerText = "(" + nTotal + ")";
			}
			else {
				taskCount.innerText = "";
				strTitle = "您当前没有待办事项";
			}

			daibanItem.title = strTitle;
		}


		function refreshContent(command) {
			var commandInput = false;
			try {
				commandInput = window.parent.document.frames["content"].window.document.getElementById("__commandInput");
			}
			catch (e) {
			}

			if (commandInput)
				commandInput.value = command;
		}

		var intervalParams = new Object();

		intervalParams.lastCheckNotifyTime = null;
		intervalParams.checkNotifyInterval = 120000;
		intervalParams.checkUserTaskCountInterval = 30000;
		intervalParams.checking = false;
		intervalParams.checkUserTaskTag = "";
		intervalParams.lastUserTaskID = "";
		intervalParams.lastUserTaskDeliverTime = new Date();

		UserTaskQueryType =
		{
			UserTaskCount: 0,
			Notify: 1,
			NewTaskArrived: 2
		}

		function queryTaskStatus() {
			if (!intervalParams.checking) {
				var params = new Array(2);

				params[UserTaskQueryType.UserTaskCount] = prepareCheckUserTaskChangeParams();
				params[UserTaskQueryType.Notify] = prepareCheckNotifyParams();

				MCS.OA.Portal.Services.PortalServices.QueryUserTaskStatus(
						params,
						onUserTaskStatusCompleted,
						onUserTaskStatusError);

				intervalParams.checking = true;
			}
		}

		function onUserTaskStatusCompleted(result) {
			intervalParams.checking = false;
			displayUserTaskCount(result[UserTaskQueryType.UserTaskCount]);

			if (result[UserTaskQueryType.UserTaskCount] != "") {
				refreshContent("refreshUserTasks");
				showLatestUserTask(result[UserTaskQueryType.NewTaskArrived]);
			}
		}

		function onUserTaskStatusError(e) {
			intervalParams.checking = false;
		}

		var nofityFeature, notifyUrl, taskid;

		function showLatestUserTask(dataString) {
			if (dataString != "") {
				var data = Sys.Serialization.JavaScriptSerializer.deserialize(dataString);

				if (intervalParams.lastUserTaskID != data.TaskID &&
				    intervalParams.lastUserTaskDeliverTime < data.DeliverTime) {
					var popupCtrl = $find("popupMessageControl");

					if (popupCtrl) {
						popupCtrl.set_showTitle(data.PopupTitle);
						popupCtrl.set_showText(data.TaskTitle);

						nofityFeature = data.Feature;
						taskid = data.TaskID;
						notifyUrl = data.Url;

						popupCtrl.set_positionElement(null);
						popupCtrl.set_positionX(window.screen.width - popupCtrl.get_width());
						popupCtrl.set_positionY(window.screen.height - popupCtrl.get_height());

						popupCtrl.show();
					}

					intervalParams.lastUserTaskID == data.TaskID;
					intervalParams.lastUserTaskDeliverTime = data.DeliverTime;
				}
			}
		}

		function showNotify() {
			if (notifyUrl && taskid) {
				onPopUpClick(notifyUrl, nofityFeature, taskid);

				notifyUrl = null;
				nofityFeature = null;
				taskid = null;
			}
		}

		function displayUserTaskCount(dataString) {
			if (dataString != "") {
				var data;
				eval("data = " + dataString);

				intervalParams.checkUserTaskTag = data.tag;
				//LDM setYue(data.yueCount, data.yueExpiredCount);
				setBan(data.banCount, data.banExpiredCount);
			}
		}

		function prepareCheckUserTaskChangeParams() {
			return intervalParams.checkUserTaskTag;
		}

		function prepareCheckNotifyParams() {
			var result = "";

			if (intervalParams.lastCheckNotifyTime == null) {
				intervalParams.lastCheckNotifyTime = new Date();
				result = "checkNotify";
			}
			else {
				var dtNow = new Date();
				var diff = (dtNow * 1 - intervalParams.lastCheckNotifyTime * 1);

				if (diff > intervalParams.checkNotifyInterval) {
					intervalParams.lastCheckNotifyTime = new Date();
					result = "checkNotify";
				}
			}

			return result;
		}

		function onShowNotifiesLinkClick() {
			event.returnValue = false;

			var elem = event.srcElement;

			while (elem.tagName != "A")
				elem = elem.parentElement;

			if (elem.disabled == false)
				showUserNotifiesDialog();
		}

		function onCheckUserTaskAnchorClick() {
			event.returnValue = false;
			queryTaskStatus();

			return false;
		}

		function onDocumentLoad() {
			displayUserTaskCount(userTaskCount.value);
			window.setInterval(queryTaskStatus, intervalParams.checkUserTaskCountInterval);
		}

		function onUserSettingUrl() {
			var sFeature = "dialogWidth:640px; dialogHeight:480px;center:yes;help:no;resizable:no;scroll:no;status:no;menubar:no;";

			var returnValue = window.showModalDialog("../UserPanel/UserSettingsManager.aspx", null, sFeature);

			if (returnValue == "reload") {
				refreshContent("refresh");
			}
		}

		function onDelegate() {
			var sFeature = "dialogWidth:660px; dialogHeight:480px;center:yes;help:no;resizable:no;scroll:no;status:no;menubar:no;";

			window.showModalDialog("../../OACommonPages/DelegationAuthorized/OriginalDelegationEntry.htm", null, sFeature);
		}

		function pageLoad() {
			if (window.parent && window.parent.document.frames && window.parent.document.frames.length >= 3) {
				frameHeight = document.getElementById("menuList").scrollHeight;
				document.getElementById("main_left").style.height = frameHeight;
			}
		}
	</script>
</head>
<body class="portal" onload="onDocumentLoad();" style="background-color: #f8f8f8;
	margin-top: 8px; overflow-x: hidden; overflow-y: visible">
	<input type="hidden" runat="server" id="userTaskCount" />
	<a style="display: none" target="detail" id="innerAnchor"></a><a style="display: none"
		id="checkUserTaskAnchor" onclick="onCheckUserTaskAnchorClick();"></a>
	<img src="../images/captionFG.gif" style="display: none" />
	<form id="LeftMenuBarForm" runat="server">
	<asp:ScriptManager runat="server" ID="scriptManagerId" EnableScriptGlobalization="true">
		<Services>
			<asp:ServiceReference Path="../Services/PortalServices.asmx" />
		</Services>
	</asp:ScriptManager>
	<div>
		<div id="main_left">
			<ul id="menuList" runat="server">
				<li id="daibanItem">
					<asp:Label runat="server" ID="userName" Visible="false"></asp:Label>
					<a href="../TaskList/UnCompletedTaskList.aspx" target="content">待办</a><span id="taskCount"></span>
				</li>
				<li><a href="../TaskList/CompletedTaskList.aspx?process_status=Running" target="content">
					流转中</a> </li>
				<li><a href="../TaskList/CompletedTaskList.aspx?process_status=Completed" target="content">
					已办结</a> </li>
				<li><a href="../frames/DrawDocument.aspx " target="content">拟单</a> </li>
				<li><a href="#" onclick="onDelegate();">授权委派</a></li>
				<li><a href="#" onclick="onUserSettingUrl();">个人设置</a></li>
				<li><a href="../../Diagnostics/ClientCheck/check.aspx" target="content">帮助</a></li>
			</ul>
			<div id="linkContainer" runat="server" oncontextmenu="event.returnValue = false;"
				class="MenuBackground">
				<CCIC:DeluxeMenu ID="dMenu" runat="server" Orientation="Vertical" imgheight="32"
					StaticDisplayLevels="0">
				</CCIC:DeluxeMenu>
				<asp:HiddenField ID="HiddenField_Menu" runat="server" />
			</div>
			<div>
				<%--<HB:PopUpMessageControl ID="popupMessageControl" runat="server" ShowTime="0:0:5"
					OnClick="showNotify" PlaySoundPath="msg.wav" />--%>
				<HB:PopUpMessageControl ID="popupMessageControl" runat="server" ShowTime="0:0:5"
					OnClick="showNotify" />
			</div>
			<iframe name="CrossDomain" style="visibility: hidden; width: 1px; height: 1px; position: absolute;
				z-index: -500"></iframe>
		</div>
	</div>
	</form>
</body>
</html>
