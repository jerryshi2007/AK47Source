<!--
/******************************************************************************************/
//用于记录当前操作人员的权限（判断是否展现toolbar上的图标以及是否展现菜单中的“变灰”选项）
var bShowNewUser = false;
var bShowNewGroup = false;
var bShowNewOrganization = false;

var bShowMove = false;
var bShowSort = false;

var bShowLogicDelete = false;
var bShowFurbishDelete = false;
var bShowDelete = false;

var bSetSideline = false;
var bInitUserPwd = false;
var bSetMainDuty = false;

function showToolBar(strPermission)
{
	if (strPermission != "setNoPermission")//strPermission == "setNoPermission"表示无权限要求
	{
		if (strPermission.length > 0)
		{
			var strArr = strPermission.split(",");
			for (var i = 0; i < strArr.length; i++)
			{
				switch (strArr[i])
				{
					case "CREATE_USERS":	bShowNewUser = true;
						break;
					case "CREATE_GROUPS":	bShowNewGroup = true;
						break;
					case "CREATE_ORGANIZATIONS":	bShowNewOrganization = true;
						break;
					case "SORT_IN_ORGANIZATIONS":	bShowSort = true;
						break;
					case "LOGIC_DELETE_ORGANIZATIONS":	
					case "LOGIC_DELETE_GROUPS":	
					case "LOGIC_DELETE_USERS":		bShowLogicDelete = true;
						break;
					case "FURBISH_DELETE_ORGANIZATIONS":	
					case "FURBISH_DELETE_GROUPS":
					case "FURBISH_DELETE_USERS":	bShowFurbishDelete = true;
						break;
					case "REAL_DELETE_ORGANIZATIONS":
					case "REAL_DELETE_GROUPS":
					case "REAL_DELETE_USERS":		bShowDelete = true;
						break;
					case "SET_SIDELINE":	bSetSideline = true;
						break;
					case "INIT_USERS_PWD":	bInitUserPwd = true;
						break;
					case "UPDATE_USERS":	bSetMainDuty = true;
						break;
					default :
						break;
				}
			}
			bShowMove = (bShowNewUser || bShowNewGroup || bShowNewOrganization) && bShowDelete;
		}
		
		document.all("newUser").style.display = bShowNewUser ? "inline" : "none";
		document.all("newGroup").style.display = bShowNewGroup ? "inline" : "none";
		document.all("newOrg").style.display = bShowNewOrganization ? "inline" : "none";
		
		document.all("splitNew").style.display = bShowNewOrganization ? "inline" : "none";
		
		document.all("move").style.display = bShowMove ? "inline" : "none";
		document.all("sort").style.display = bShowSort ? "inline" : "none";
		document.all("logicDelete").style.display = bShowLogicDelete ? "inline" : "none";
		
		document.all("splitSearch").style.display = bShowLogicDelete ? "inline" : "none";
		
		document.all("furbishDelete").style.display = bShowFurbishDelete ? "inline" : "none";
		document.all("delete").style.display = bShowDelete ? "inline" : "none";
	}
}
/******************************************************************************************/
var m_Menu = new Array(
			/*00*/"<strong>进入该组织机构</strong>,,,,,enter",		
			/*01*/"-,,,,,",
			/*02*/"新建用户...,,,,../images/newUser.gif,newUser",
			/*03*/"新建人员组...,,,,../images/newGroup.gif,newGroup",
			/*04*/"新建部门机构...,,,,../images/newOrg.gif,newOrg",
			/*05*/"-,,,,,",
			/*06*/"刷新,,,,../images/refresh.gif,refresh",
			/*07*/"查找...,,,,../images/find.gif,search",
			/*08*/"-,,,,,",
			/*09*/"重设密码...,,,,../images/KeyIcon.gif,resetPassword",
			/*10*/"初始化用户密码,,,,,initPassword",
			/*11*/"移动...,,,,../images/move.gif,move",
			/*12*/"保存当前顺序,,,,../images/sort.gif,sort",
			/*13*/"-,,,,,",
			/*14*/"设置兼职部门...,,,,../images/sideline.gif,setSideline",
			/*15*/"设置为主职,,,,,setMainDuty",
			/*16*/"浏览人员兼职情况...,,,,,viewSideline",
			/*17*/"指定秘书....,,,,../images/user.gif,secretary",
			/*18*/"-,,,,,",
			/*19*/"送入回收站,,,,../images/trash.gif,logicDelete",
			/*20*/"还原到原系统,,,,../images/checkUser.gif,furbishDelete",
			/*21*/"删除,,,,../images/delete.gif,delete",
			/*22*/"-,,,,,",
			/*23*/"人员组中成员...,,,,../images/group.gif,enterGroup",
			/*24*/"属性...,,,,../images/property.gif,property");

var m_objParam = null;
var m_txtSelection = null;

var m_MenuCreated = false;//标识Menu是否被创建

var m_MenuOrganization = new Array(0,1,2,3,4,5,6,7,8,11,12,19,22,24);
var m_MenuUser = new Array(2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,22,24);
var m_MenuGroup = new Array(2,3,4,5,6,7,8,11,12,19,22,23,24);
var m_MenuTrash = new Array(6,7,8,20,21,22,24);

function buildMenuByType(strType)
{
	m_MenuCreated = true;
	var objArray = null;
		
	switch (strType)
	{
		case "ORGANIZATIONS":
						objArray = buildMenuByNum(m_MenuOrganization);
						break;
		case "USERS":		
						objArray = buildMenuByNum(m_MenuUser);
						break;
		case "GROUPS":		
						objArray = buildMenuByNum(m_MenuGroup);
						break;
		case "TRASH":		
						objArray = buildMenuByNum(m_MenuTrash);
						break;
		default:		m_MenuCreated = false;
						break;
	}

	if (objArray)
		listMenu.buildMenu(objArray);
}

function buildMenuByNum(arr)
{
	var arrReturn = new Array(arr.length);
	for (var i = 0; i < arr.length; i++)
	{
		arrReturn[i] = m_Menu[arr[i]];
	}
	
	return arrReturn;
}

function listMenuBeforePopup()
{
	try
	{
		var xmlDoc = createDomDocument(listMenu.row.xml);
		var objectClass = xmlDoc.documentElement.tagName;
		var eMail = xmlDoc.documentElement.getAttribute("E_MAIL");

		if (userPermission.value != "setNoPermission")//strPermission == "setNoPermission"表示无权限要求
		{
			switch (event.menuData)
			{
				case "enter":			event.disableItem = (objectClass != "ORGANIZATIONS" || m_objParam.OriginalSort == "");
					break;
				case "viewSideline":
				case "secretary":
				case "resetPassword":	event.disableItem = (objectClass != "USERS" || m_objParam.OriginalSort == "");
					break;
				case "initPassword":	event.disableItem = (objectClass != "USERS" || m_objParam.OriginalSort == "" || !bInitUserPwd);
					break;
				case "setSideline":		event.disableItem = (objectClass != "USERS" || m_objParam.OriginalSort == "" || xmlDoc.documentElement.getAttribute("SIDELINE") == "1" || !bSetSideline);
					break;
				case "setMainDuty":		event.disableItem = (objectClass != "USERS" || m_objParam.OriginalSort == "" || xmlDoc.documentElement.getAttribute("SIDELINE") == "0" || !bSetMainDuty);
					break;
				case "mailbox":			event.disableItem = (objectClass != "USERS" || (eMail && eMail.length > 0));
					break;
				case "furbishDelete":	event.disableItem = (m_objParam.OriginalSort != "" || !bShowFurbishDelete);
					break;
				case "delete" :			event.disableItem = (m_objParam.OriginalSort != "" || !bShowDelete);
					break;
				case "newUser":			event.disableItem = (m_objParam.OriginalSort == "" || !bShowNewUser);
					break;
				case "newGroup":		event.disableItem = (m_objParam.OriginalSort == "" || !bShowNewGroup);
					break;
				case "newOrg":			event.disableItem = (m_objParam.OriginalSort == "" || !bShowNewOrganization);
					break;
				case "move":			event.disableItem = (m_objParam.OriginalSort == "" || !bShowMove);
					break;
				case "sort":			event.disableItem = (m_objParam.OriginalSort == "" || !bShowSort);
					break;
				case "logicDelete":		event.disableItem = (m_objParam.OriginalSort == "" || !bShowLogicDelete);
					break;
				case "enterGroup":		event.disableItem = (objectClass != "GROUPS");
					break;
			}
		}
	}
	catch(e)
	{
		showError(e);
	}
}

function listMenuClick()
{
	try
	{
		if (m_txtSelection)
			m_txtSelection.select();

		var oTD = getRelativeTD(listMenu.row, "DISPLAY_NAME");

		switch (event.menuData)
		{
			case "enter":			setAutoExpand(top.tv, "<enter>" + listMenu.row.xml + "</enter>", true);
									break;
			case "enterGroup":		showUsersInGroupDialog(listMenu.row.guid);
									break;
			case "setSideline":		setUserSideline(listMenu.row);
									break;
			case "setMainDuty":		setUserMainDuty(listMenu.row);
									break;
			case "delete":	
			case "logicDelete":
			case "furbishDelete":
			case "move":			if (m_txtSelection.text.length == 0)
									{
										var rgn = document.body.createTextRange();
										rgn.moveToElementText(oTD);
										rgn.select();
									}
									break;
			case "resetPassword":	showResetPasswordDialog(listMenu.row.xml);
									break;
			case "initPassword":	InitPassword(listMenu.row.xml);
									break;
			case "secretary":		showSecretariesOfLeader(listMenu.row.guid);
									//SetSecretaryOfLeader(listMenu.row);
									break;
			case "viewSideline":	showViewSideline(listMenu.row.guid);
									break;
			case "mailbox":			//var xmlDoc = createDomDocument(listMenu.row.xml);
									//var root = xmlDoc.documentElement;
									//var samAccountName = root.getAttribute("samAccountName");
									//var homeMDB = createMailBoxDialog(listMenu.row.dn, samAccountName);
									//if (homeMDB)
									//{
									//	var oTable = getOwnerTable(listMenu.row);
									//	if (oTable && oTable.refreshData)
									//		oTable.refreshData();
									//}
									break;
		}

		if (event.menuData == "property")
			onNameClick(listMenu.row);
		else
			doToolbarEvent(event.menuData);
	}
	catch(e)
	{
		showError(e);
	}
}

function SetSecretaryOfLeader(oTR)
{	
	var arg = new Object();

	arg.listObjType = 3;	
	arg.multiSelect = 0;
	arg.extAttr = "samAccountName";
	var strXml = showSelectUserDialog(arg);
	if (strXml.length > 0)
	{										
		var xmlDoc = createDomDocument("<InsertSecretary/>");
		var root = xmlDoc.documentElement;
		
		var xmlLeader = createDomDocument(oTR.xml);
		appendNode(root, "LEADER_DN", oTR.dn);
		appendNode(root, "LEADER_ID", xmlLeader.documentElement.getAttribute("samAccountName"));
		
		var xmlSecretary = createDomDocument(strXml);
		var rootSec = xmlSecretary.documentElement.firstChild;
		appendNode(root, "SECRETARY_DN", rootSec.getAttribute("dn"));
		trueThrow(rootSec.getAttribute("samAccountName").length <= 0, "对不起，您选择的是部门，请选择人员！");
		appendNode(root, "SECRETARY_ID", rootSec.getAttribute("samAccountName"));
		
		var xmlResult = xmlSend("../AppAdminOP/AppDBWriter.aspx", xmlDoc);
		checkErrorResult(xmlResult);
	}								
}

function setUserSideline(oTR)
{
	var oSelectOu = selectOU();

	if (oSelectOu)
	{
		var sOuChild = oSelectOu.documentElement.firstChild;
		
		var strSParentGuid = sOuChild.getAttribute("GUID");//兼职部门的GUID
		
		var xmlDoc = createDomDocument(oTR.xml);
		
		var strUserGuid = xmlDoc.documentElement.getAttribute("GUID");
		var strParentGuid = xmlDoc.documentElement.getAttribute("PARENT_GUID");
		
		var strResult = showUSERDetailDialog("AddSideline", strParentGuid, strUserGuid, strSParentGuid);
		
		if (strResult && strResult.length > 0)
			alert("兼职设置成功！");
	}
}

function setUserMainDuty(oTR)
{
	if (confirm("您确定要把当前职务设置为主要职务（取消原来的主要职务）？"))
	{
		var rowXml = createDomDocument(oTR.xml);
		var rowRoot = rowXml.documentElement;
		
		var xmlDoc = createDomDocument("<setMainDuty/>");
		var root = xmlDoc.documentElement;
		
		var mNode = appendNode(root, "MainDuty");
		appendNode(mNode, "USER_GUID", rowRoot.getAttribute("GUID"));
		appendNode(mNode, "PARENT_GUID", rowRoot.getAttribute("PARENT_GUID"));
		
		var xmlResult = xmlSend("../sysAdmin/OGUEditer.aspx", xmlDoc);
		checkErrorResult(xmlResult);
		
		rowRoot.setAttribute("SIDELINE", "0");
		oTR.xml = rowXml.xml;
	}
}

function oBodyContextMenu()
{
	try
	{
		var oTR = getOwnerTR(event.srcElement);
		if (m_objParam.OriginalSort == "")
			buildMenuByType("TRASH");
		else
			buildMenuByType(oTR.objectClass);
			
		if (m_MenuCreated)
		{
			listMenu.row = oTR;
			m_txtSelection = document.selection.createRange()
			
			if (m_txtSelection.htmlText == "")
			{
				var oTD = getRelativeTD(oTR, "DISPLAY_NAME")
				var rgn = document.body.createTextRange();
				rgn.moveToElementText(oTD);
				rgn.select();
			}

			listMenu.show(event.x, event.y);
		}
	}
	catch(e)
	{
		showError(e);
	}
	finally
	{
		event.returnValue = false;
	}
}

function displaySortOrderSave(bVisible)
{
	var strDisplay = "none";

	if (bVisible && m_objParam.OriginalSort)//回收站中无须排序
		strDisplay = "inline";

	saveSpan.style.display = strDisplay;
	save.style.display = strDisplay;
}

function onSaveSortOrder()
{
	try
	{
		var xmlDoc = createDomDocument("<Sort/>");
		var root = xmlDoc.documentElement;

		root.setAttribute("OrgGuid", m_objParam.OrgGuid);

		for (var i = 0; i < ouUserListBody.rows.length; i++)
		{
			var oTR = ouUserListBody.rows[i];

			if (oTR && oTR.guid)
				appendNode(root, oTR.objectClass, oTR.guid);
		}

		if (xmlDoc.documentElement.childNodes.length > 0)
		{
			var xmlResult = xmlSend("../sysAdmin/OGUEditer.aspx", xmlDoc);

			checkErrorResult(xmlResult);
			
			reDisplaySortID();
		}

		displaySortOrderSave(false);
	}
	catch(e)
	{
		showError(e);
	}
}

function gridCallBack(strCmd, oTD, xmlNode, nodeName, nodeText)
{
	try
	{
		switch(strCmd)
		{
			case "onCalc":	calcCells(oTD, xmlNode, nodeName, nodeText);
							break;
			case "onClick":	if (!event.ctrlKey && !event.shiftKey)
							{
								var rgn = document.body.createTextRange();
								var nameTD = getRelativeTD(oTD, "DISPLAY_NAME");
								rgn.moveToElementText(nameTD);
								rgn.select();
							}
		}
	}
	catch(e)
	{
		showError(e);
	}
}

function getChildDataSet()
{
	var strGuid = m_objParam.OrgGuid;
	
	if (strGuid && strGuid.length > 0)
	{
		var xmlDoc = createCommandXML("getOrganizationChildren");
			
		var root = xmlDoc.documentElement;
		root.setAttribute("rootOrgGuid", strGuid);
		root.setAttribute("extAttr", "E_MAIL,DESCRIPTION,RANK_CODE,NAME,START_TIME,END_TIME");
		if (m_objParam.OriginalSort.length == 0)
		{
			root.setAttribute("listObjectDelete", "2");
			root.setAttribute("oguDepth", "0");
		}	
		var xmlResult = xmlSend("../SysSearch/OGUSearch.aspx", xmlDoc);

		checkErrorResult(xmlResult);

		ouUserListBody.noSelect = true;

		ouUserTable.refreshData = refreshData;
		ouUserTable.listSortId = 1;
		
		var ouRoot = xmlResult.documentElement.firstChild;
		
		while (ouRoot != null)
		{
			if (ouRoot.childNodes.length > 0)
				createGridBodyByHeadAsync(ouRoot.firstChild, ouUserListBody, null, ouUserHeader.rows(0), ouUserTable, gridCallBack, initDragDrop);
			else
				clearAllRows(ouUserListBody);
							
			ouRoot = ouRoot.nextSibling;
		}
	}
	
	setToolbarFace();
}

function reDisplaySortID()
{
	var oTBody = ouUserListBody;
	
	for (var i = 0; i < oTBody.rows.length;)
	{
		var oTD = oTBody.rows[i++].cells[0];
		oTD.innerText = i;
		oTD.sortText = i;
	}
}

function setToolbarFace()
{
	if (m_objParam.OriginalSort.length == 0)
	{
		listToolbar.style.display = "none";		
		trashToolBar.style.display = "inline";
	}
	else
	{
		listToolbar.style.display = "inline";
		trashToolBar.style.display = "none";
	}
	
	subTitle.innerText = m_objParam.DisplayName;
	subTitle.title = m_objParam.AllPathName;
}

function newOrg()
{
	var strXml = showOUDetailDialog("Insert", m_objParam.OrgGuid, null);

	
	if (strXml && strXml.length > 0)
	{
		appendNewObject(strXml);
		setSyncData(strXml);
	}
}

function newUser()
{
	var strXml = showUSERDetailDialog("Insert", m_objParam.OrgGuid, null, null);

	if (strXml && strXml.length > 0)
	{
		appendNewObject(strXml);
		setSyncData(strXml);
	}
}

function newGroup()
{
	var strXml = showGroupDetailDialog("Insert", m_objParam.OrgGuid, null);
	if (strXml && strXml.length > 0)
	{
		appendNewObject(strXml);
		setSyncData(strXml);
	}
}

function deleteSelectedObjects()
{
	var strResult = deleteObjectFromSelection(ouUserListBody);

	if (strResult)
		setSyncData(strResult);
}

function logicDeleteSelectedObjects()
{
	var strResult = logicDeleteObjectFromSelection(ouUserListBody);

	if (strResult)
		setSyncData(strResult);
}

function furbishDeleteSelectedObjects()
{
	var strResult = furbishDeleteObjectFromSelection(ouUserListBody);
	
	if (strResult)
		setSyncData(strResult);
}

function moveSelectedObjects()
{
	var strResult = moveObjectFromSelection(ouUserListBody, m_objParam.OrgGuid);// getFrameParam());

	if (strResult)
		setSyncData(strResult);
}

function refreshData()
{
	getChildDataSet();
	displaySortOrderSave(false);
}

function doToolbarEvent(strID)
{
	switch(strID)
	{
		case "newUser":	newUser();
						break;
		case "newGroup": newGroup();
						break;
		case "newOrg":	newOrg();
						break;
		case "refresh":	refreshData();
						break;
		case "search":	var xmlDoc = createDomDocument(ADSearchConfig.XMLDocument.xml);
						appendNode(xmlDoc.documentElement, "RootOrg", m_objParam.AllPathName.replace("\\回收站", ""));
						var xmlResult = showSelectUsersToRoleDialog(xmlDoc);
						
//						var xd = createDomDocument("<Config><RootOrg><ORGANIZATIONS>" + m_objParam.AllPathName +"</ORGANIZATIONS><ORGANIZATIONS>" + C_OGU_ROOT_NAME + "</ORGANIZATIONS></RootOrg></Config>");
//						var xmlResult = showSelectUsersToRoleDialog(xd);
						setAutoExpand(top.tv, xmlResult, true);
						break;
		case "sort":	if (confirm("确定要保存当前组织机构和人员的显示顺序吗？"))
							onSaveSortOrder();
						break;
		case "logicDelete":		logicDeleteSelectedObjects();
								break;
		case "furbishDelete":	furbishDeleteSelectedObjects();
								break;
		case "delete":	deleteSelectedObjects();
						break;
		case "save":	onSaveSortOrder();
						break;
		case "move":	moveSelectedObjects();
						break;
	}
}

function onToolbarMouserOver()
{
	var obj = event.srcElement;

	if (obj.tagName == "IMG")
	{
		obj.oldClassName = obj.className;
		obj.className = "mouseOver";
	}
}

function onToolbarMouserOut()
{
	var obj = event.srcElement;

	if (obj.tagName == "IMG")
		obj.className = obj.oldClassName;
}

function onToolbarClick()
{
	try
	{
		var obj = event.srcElement;

		if (obj.tagName == "IMG")
			doToolbarEvent(obj.id);
	}
	catch(e)
	{
		showError(e);
	}
}

function onInputParamChange()
{
	try
	{
		if ((event.propertyName == "value") && (inputParam.value.length > 0))
		{
			buildParam(inputParam.value);
			setInterface();
			getChildDataSet();
			inputParam.value = "";
		}
	}
	catch(e)
	{
		showError(e);
	}
}

function setInterface()
{
	displaySortOrderSave(false);

	createTableHeadByXml(ColumnTitleOU.XMLDocument, ouUserHeader);
}

function buildParam(strParam)
{
	var xmlDoc = createDomDocument(strParam);
	var root = xmlDoc.documentElement;

	m_objParam = new Object();
	m_objParam.OrgGuid = root.getAttribute("rootOrgGuid");
	m_objParam.DisplayName = root.getAttribute("DISPLAY_NAME");
	m_objParam.AllPathName = root.getAttribute("ALL_PATH_NAME");
	m_objParam.OriginalSort = root.getAttribute("ORIGINAL_SORT");
//	m_objParam.TrashOriginalSort = root.getAttribute("TRASH_ORIGINAL_SORT");
	
	m_gDragSrc = null;//初始化拖动行
}

function onDocumentSelectStart()
{
	var bResult = true;

	if (event.ctrlKey)
		bResult = false;

	event.returnValue = bResult;

	return bResult;	
}

function onDocumentLoad()
{
	try
	{
		showToolBar(userPermission.value);
		
		buildParam(getFrameParam());
		setInterface();
		getChildDataSet();
	}
	catch(e)
	{
		showError(e);
	}
}
//-->