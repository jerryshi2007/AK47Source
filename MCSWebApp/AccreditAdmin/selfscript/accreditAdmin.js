<!--
var C_OGU_ORIGINAL_SORT = "000000";
var C_APP_ADMIN_ID = 1;

var C_USERS = "USERS";
var C_ORGANIZATIONS = "ORGANIZATIONS";
var C_GROUPS = "GROUPS";

function getFrameParam()
{
	if (typeof(secFrm) == "object" && secFrm.value == "2" && parent.window.paramValue2)
			return parent.window.paramValue2.value;
	
	if (parent.window.paramValue)
		return parent.window.paramValue.value;
	
	return "";
}

function setSyncData(value)
{
	if (parent.window.syncData)
		parent.window.syncData.value = value;
}

function getImgFromClass(strClass)
{
	var imgName = "";
	switch(strClass)
	{
		case "TRASH_ORGANIZATIONS":	imgName = "trashOrg.gif";
									break;
		case "TRASH_GROUPS":		imgName = "trashGroup.gif";
									break;
		case "TRASH_USERS":			imgName = "trashUser.gif";
									break;
		case "ForbiddenUser":		imgName = "ForbiddenUser.gif";
									break;
		case "GROUPS":
		case "group":				imgName = "group.gif";
									break;
		case "person":
		case "CN":
		case "USERS":
		case "USER":				imgName = "user.gif";
									break;
		case "userAccountControl":	imgName = "accountDisabled.gif";
									break;
		case "ORGANIZATIONS":		imgName = "Organization.gif";
									break;
		case "TRASH":				imgName = "trash.gif";
									break;
		case "DC":
		case "dc":
		case "domain":
		case "DOMAIN":				imgName = "domain.gif";
									break;
		case "organizationalUnit":
		case "OU":					imgName = "ou.gif";
									break;
		case "role":
		case "ROLE":
		case "ROLES":
		case "ROLES_TO_FUNCTIONS":	imgName = "role.gif";
									break;
		case "FUNCTIONS":			imgName = "function.gif";
									break;
		case "MANAGED_APP":
		case "APPLICATIONS":
		case "OPERATIONS":
		case "RESOURCES":			imgName = "application.gif";
									break;
		case "ResourceItem":		imgName = "ResourceItem.gif";
									break;
		case "ResourceFolder":		imgName = "ResourceFolder.gif";
									break;
		case "ResourceFile":		imgName = "ResourceFile.gif";
									break;
	}

	return imgName;
}

function getNameFromType(strClass)
{
	var strName = "";

	switch(strClass)
	{
		case "FUNCTIONS":
						strName = "功能";
						break;
		case "APPLICATIONS":
						strName = "应用程序";
						break;
		case "RESOURCES":
						strName = "资源";
						break;
		case "ROLES":	strName = "角色";
						break;
		case "USERS_TO_ROLES":
						strName = "用户或机构";
						break;
	}

	return strName;
}

function getNameFromAllPathName(strAllPathName)
{
	var arrSeg = strAllPathName.split("\\");

	return arrSeg[arrSeg.length - 1];
}

function showOUDetailDialog(strOP, strParentGuid, strOrgGuid)
{
	var sFeature = "dialogWidth:408px; dialogHeight:480px;center:yes;help:no;resizable:yes;scroll:no;status:no";

	var arg = new Object();

	arg.guid = strOrgGuid;
	arg.parentGuid = strParentGuid;
	arg.op = strOP;

	var sPath = "/" + C_ACCREDIT_ADMIN_ROOT_URI + "/dialogs/EditOrganization.aspx?parentGuid=" + strParentGuid + "&opType=" + strOP;
	if (strOrgGuid != null)
		sPath += "&objGuid=" + strOrgGuid;
		
	return showModalDialog(sPath, arg, sFeature);// 《机构人员管理子系统》内部专用
}

function showGroupDetailDialog(strOP, strParentGuid, strGroupGuid)
{
	var sFeature = "dialogWidth:408px; dialogHeight:480px;center:yes;help:no;resizable:yes;scroll:no;status:no";

	var arg = new Object();

	arg.guid = strGroupGuid;
	arg.parentGuid = strParentGuid;
	arg.op = strOP;

	var sPath = "/" + C_ACCREDIT_ADMIN_ROOT_URI + "/dialogs/EditGroup.aspx?parentGuid=" + strParentGuid + "&opType=" + strOP;
	if (strGroupGuid != null)
		sPath += "&objGuid=" + strGroupGuid;
		
	return showModalDialog(sPath, arg, sFeature);//《机构人员管理子系统》内部专用
}

function showUsersInGroupDialog(strGroupGuid)
{
	var sPath = "/" + C_ACCREDIT_ADMIN_ROOT_URI + "/dialogs/UsersInGroup.aspx?Guid=" + strGroupGuid;
	
//	var arg = new Object();
//	arg.guid = strGroupGuid;
//	var sPath = "/" + C_XAD_DIR + "/dialogs/Container.aspx?strUrl='/" + C_XAD_DIR + "/dialogs/UsersInGroup.aspx?Guid=" + strGroupGuid + "'";
//	return showModalDialog(sPath, arg, sFeature);//Deleted
	var iHeight = 570;
	var iTop = (window.screen.height - iHeight) / 2;
	var iWidth = 800;
	var iLeft = (window.screen.width - iWidth) / 2;

	var oFeature = "top=" + iTop + "px,left=" + iLeft + "px,height=570px,width=760px,status=no,toolbar=no,menubar=no,location=no,resizable=yes";
	window.open(sPath, null, oFeature);
}

function showSecretariesOfLeader(strUserGuid)
{	
	var sPath = "/" + C_ACCREDIT_ADMIN_ROOT_URI + "/dialogs/SecretariesOfLeader.aspx?Guid=" + strUserGuid;
	
//	var arg = new Object();
//	arg.guid = strUserGuid;
//	var sFeature = "dialogWidth:760px; dialogHeight:570px;center:yes;help:no;resizable:yes;scroll:no;status:no";
//	return showModalDialog(sPath, arg, sFeature);//Deleted
	var iHeight = 570;
	var iTop = (window.screen.height - iHeight)/2;
	var iWidth = 760;
	var iLeft = (window.screen.width - iWidth)/2;
	
	var oFeature = "top=" + iTop + "px,left=" + iLeft + "px,height=570px,width=760px,status=no,toolbar=no,menubar=no,location=no";
	window.open(sPath, null, oFeature);
}

function showViewSideline(strUserGuid)
{
	var sPath = "/" + C_ACCREDIT_ADMIN_ROOT_URI + "/dialogs/ViewUserSideline.aspx?Guid=" + strUserGuid;
	
	var sFeature = "dialogWidth:760px; dialogHeight:570px;center:yes;help:no;resizable:yes;scroll:no;status:no";
	
	return showModalDialog(sPath, null, sFeature);//《机构人员管理子系统》内部专用
}

//function createMailBoxDialog(strUserDN, strLogOnName)
//{
//	var sFeature = "dialogWidth:480px; dialogHeight:360px;center:yes;help:no;resizable:yes;scroll:no;status:no";
//
//	var arg = new Object();
//
//	arg.dn = strUserDN;
//	arg.alias = strLogOnName.split("@")[0];
//
//	var sPath = "/" + C_ACCREDIT_ADMIN_ROOT_URI + "/dialogs/selectMailBox.htm";
//
//	return showModalDialog(sPath, arg, sFeature);//Deleted
//}

function changeAlias(strUserDN)
{
	var xmlDoc = createCommandXML("changeAlias", strUserDN);
	var xmlResult = xmlSend("../ADAccess/ADSupport.aspx", xmlDoc);

	checkErrorResult(xmlResult);
}

function showUSERDetailDialog(strOP, strParentGuid, strUserGuid, strSParentGuid)
{
	var sFeature = "dialogWidth:480px; dialogHeight:720px;center:yes;help:no;resizable:yes;scroll:auto;status:no";

	var arg = new Object();

	arg.guid = strUserGuid;//
	arg.parentGuid = strParentGuid;
	arg.sParentGuid = strSParentGuid;//要求设置的兼职部门
	arg.op = strOP;

	var sPath = "/" + C_ACCREDIT_ADMIN_ROOT_URI + "/dialogs/EditUser.aspx?parentGuid=" + strParentGuid + "&opType=" + strOP;
	if (strUserGuid != null)
		sPath += "&objGuid=" + strUserGuid;
	if (strSParentGuid != null)
		sPath += "&SParentGuid=" + strSParentGuid;
		
	var strXml = showModalDialog(sPath, arg, sFeature);//《机构人员管理子系统》内部专用

	return strXml;
}

//function showAppObjDetailDialog(strOP, nID, strType, strAppID, strResFlag, strAppLevel, appCodeName)
//{
//	var sFeature = "dialogWidth:320px; dialogHeight:300px;center:yes;help:no;resizable:yes;scroll:no;status:no";
//
//	if (strType == "RESOURCES")//对于资源来说，要求的资源显示界面更大一些
//		sFeature = "dialogWidth:360px; dialogHeight:320px;center:yes;help:no;resizable:yes;scroll:no;status:no";
//
//	var arg = new Object();
//
//	arg.objID = nID;
//	arg.type = strType;
//	arg.op = strOP;
//	arg.appID = strAppID;
//	arg.resourceFlag = strResFlag;
//	arg.appLevel = strAppLevel;
//	arg.appCodeName = appCodeName;
//	if (typeof(event.srcElement.canDelete) != "undefined")
//		arg.disabled = !event.srcElement.canDelete;
//	else
//		arg.disabled = false;
//
//	var sPath = "/" + C_ACCREDIT_ADMIN_ROOT_URI + "/dialogs/editAppObjects.htm";
//
//	return showModalDialog(sPath, arg, sFeature);//Deleted
//}

function showSelectUserDialog(arg)
{
	var sFeature = "dialogWidth:360px; dialogHeight:400px;center:yes;help:no;resizable:yes;scroll:no;status:no";

	var sPath = "/" + C_ACCREDIT_ADMIN_ROOT_URI + "/exports/selectOGU.aspx";

	window.clipboardData.clearData("Text");
	return showModalDialog(sPath, arg, sFeature);//文本返回
}

//xmlDoc:传入的Xml结构参数
//strUrl：待处理的Url地址，要求最后地址内容“/exports/selectOGU.aspx”
function showSelectUserDialogMulitServer(xmlDoc, strUrl)//跨服务器数据调用
{
	var sFeature = "dialogWidth:360px; dialogHeight:400px;center:yes;help:no;resizable:yes;scroll:no;status:no";
	var strXml = xmlDoc;
	if (typeof(xmlDoc) == "object")
		strXml = xmlDoc.xml;
	window.clipboardData.setData("Text", strXml);
	showModalDialog(strUrl, null, sFeature);
	
	var strResult = window.clipboardData.getData("Text");
	window.clipboardData.clearData("Text");
	return strResult;//文本返回
}

function showSelectUsersToRoleDialog(arg)
{
	var sFeature = "dialogWidth:680px; dialogHeight:560px;center:yes;help:no;resizable:yes;scroll:no;status:no";

	var sPath = "/" + C_ACCREDIT_ADMIN_ROOT_URI + "/exports/selectObjsToRole.aspx";

	window.clipboardData.clearData("Text");
	return showModalDialog(sPath, arg, sFeature);//文本返回
}

//xmlDoc:传入的Xml结构参数
//strUrl：待处理的Url地址，要求最后地址内容“/exports/selectObjsToRole.aspx”
function showSelectUsersToRoleDialogMulitServer(xmlDoc, strUrl)//跨服务器数据调用
{
	var sFeature = "dialogWidth:680px; dialogHeight:560px;center:yes;help:no;resizable:yes;scroll:no;status:no";

	var strXml = xmlDoc;
	if (typeof(xmlDoc) == "object")
		strXml = xmlDoc.xml;
	window.clipboardData.setData("Text", strXml);
	
	showModalDialog(strUrl, null, sFeature);
	
	var strResult = window.clipboardData.getData("Text");
	window.clipboardData.clearData("Text");
	return strResult;//文本返回
}

function showGroupUsersDialog(groupDN)
{
	var sFeature = "dialogWidth:570px; dialogHeight:480px;center:yes;help:no;resizable:yes;scroll:no;status:no";

	var sPath = "/" + C_ACCREDIT_ADMIN_ROOT_URI + "/dialogs/listGroupUsers.aspx?groupDN=" + groupDN;

	return showModalDialog(sPath, groupDN, sFeature);//《机构人员管理子系统》内部专用
}

//function showUserFunctionsDialog(objID, appID, objName, objectClass, objRefAppID)
//function showUserFunctionsDialog(obj, mObj)
//{
//	var sFeature = "dialogWidth:360px; dialogHeight:420px;center:yes;help:no;resizable:yes;scroll:no;status:no";
//
//	var sPath = "/" + C_ACCREDIT_ADMIN_ROOT_URI + "/dialogs/userFunctions.htm";
//
//	var arg = new Object();
//	
//	arg.objID = obj.objID;
//	arg.objName = obj.innerText;
//	arg.objectClass = obj.objectClass;
//	
//	arg.appID = mObj.appID;
//	arg.refAppID = mObj.refAppID;
//	
//	arg.appLevel = mObj.appLevel;
//	/*arg.objID = objID;
//	arg.appID = appID;
//	arg.objName = objName;
//	arg.objectClass = objectClass;
//	arg.refAppID = objRefAppID;*/
//
//	return showModalDialog(sPath, arg, sFeature);//Deleted
//}

function showResetPasswordDialog(strXml)
{
	trueThrow(strXml.length == 0, "没有给指定对象！");
	
	var sFeature = "dialogWidth:480px; dialogHeight:360px;center:yes;help:no;resizable:yes;scroll:no;status:no";

	var xmlDoc = createDomDocument(strXml);
	var root = xmlDoc.documentElement;
	
	var obj = new Object();
	obj.guid = root.getAttribute("GUID");
	//obj.displayName = root.getAttribute("DISPLAY_NAME")
	var sPath = "/" + C_ACCREDIT_ADMIN_ROOT_URI + "/dialogs/changeUserPwd.aspx?userGuid=" + obj.guid;
	
	return showModalDialog(sPath, null, sFeature);//共同使用，跨服务器不受影响
}
//-->