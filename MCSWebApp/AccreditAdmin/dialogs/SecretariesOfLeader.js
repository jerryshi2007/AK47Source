<!--
/******************************************************************************************/
//用于记录当前操作人员的权限（判断是否展现toolbar上的图标以及是否展现菜单中的“变灰”选项）
var bShowNewUser = false;
var bShowDelete = false;
var bInitPassword = false;

function showToolBar(strPermission)
{
	if (strPermission != "setNoPermission")
	{
		if (strPermission.length > 0)
		{
			var strArr = strPermission.split(",");
			for (var i = 0; i < strArr.length; i++)
			{
				switch (strArr[i])
				{
					case "SECRETARY_ADD":	bShowNewUser = true;
						break;
					case "SECRETARY_DEL":	bShowDelete = true;
						break;
					case "INIT_USERS_PWD":	bInitPassword = true;
						break;
					default :
						break;
				}
			}
		}
		
		document.all("newUser").style.display = bShowNewUser ? "inline" : "none";
		document.all("delete").style.display = bShowDelete ? "inline" : "none";
	}		
}
/******************************************************************************************/

var m_LeaderGuid = null;

function onDocumentLoad()
{
	try
	{
		showToolBar(userPermission.value);
		
		m_LeaderGuid = LeaderGuid.value;
		createTableHeadByXml(ColumnTitle.XMLDocument, ouUserHeader);
		getChildDataSet();
	}
	catch (e)
	{
		showError(e);
		window.close();
	}
}

function getChildDataSet()
{
	var xmlDoc = createCommandXML("getSecretariesOfLeaders");
		
	var root = xmlDoc.documentElement;
	root.setAttribute("GUID", m_LeaderGuid);
	root.setAttribute("extAttr", "E_MAIL,DESCRIPTION,RANK_CODE,NAME");
	
	var xmlResult = xmlSend("../SysSearch/OGUSearch.aspx", xmlDoc);

	checkErrorResult(xmlResult);
	
	fillDataToGrid(xmlResult);
}

function fillDataToGrid(xmlResult)
{
	ouUserListBody.noSelect = true;

	ouUserTable.refreshData = refreshData;
	ouUserTable.listSortId = 1;
	
	var ouRoot = xmlResult.documentElement;
	
	while (ouRoot != null)
	{
		if (ouRoot.childNodes.length > 0)
			createGridBodyByHead(ouRoot.firstChild, ouUserListBody, null, -1, ouUserHeader.rows(0), ouUserTable, gridCallBack);
		else
			clearAllRows(ouUserListBody);
			
		initDragDrop();
						
		ouRoot = ouRoot.nextSibling;
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

function onToolbarMouserOut()
{
	try
	{
		var obj = event.srcElement;

		if (obj.tagName == "IMG")
			obj.className = obj.oldClassName;
	}
	catch (e)
	{
		showError(e);
	}

}

function onToolbarMouserOver()
{
	try
	{
		var obj = event.srcElement;

		if (obj.tagName == "IMG")
		{
			obj.oldClassName = obj.className;
			obj.className = "mouseOver";
		}
	}
	catch (e)
	{
		showError(e);
	}
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

function doToolbarEvent(strID)
{
	switch(strID)
	{
		case "newUser":	SetSecretaryToLeader();
						break;
		case "refresh":	refreshData();
						break;
		case "search":	var xmlDoc = createDomDocument(ADSearchConfig.XMLDocument.xml);
						appendNode(xmlDoc.documentElement, "RootOrg", C_DC_ROOT_NAME);
						showSelectUsersToRoleDialog(xmlDoc);
						break;
		case "delete":	deleteSelectedObjects();
						break;
	}
}

function deleteSelectedObjects()
{
	var strResult = null;

	var xmlDoc = createDomDocument("<delSecsOfLeader />");

	var root = xmlDoc.documentElement;
		
	root.setAttribute("GUID", m_LeaderGuid);
	
	enumSelection(root);

	trueThrow(root.childNodes.length == 0, "对不起，你没有选定要求删除的对象！");
	
	if (confirm("确定要删除选定的对象吗？"))
	{
		var xmlResult = xmlSend("../sysAdmin/OGUEditer.aspx", xmlDoc);

		checkErrorResult(xmlResult);
		xmlResult = xmlDoc;
		syncDeletedRows(xmlResult, ouUserListBody);

		strResult = xmlResult.xml;
	}
}

function refreshData()
{
	getChildDataSet();
}

function SetSecretaryToLeader()
{
	var xd = createDomDocument("<Config><RootOrg>" + C_DC_ROOT_NAME + "</RootOrg><ShowTimeLimit>true</ShowTimeLimit><Caption>领导设置秘书</Caption></Config>");
	var strXml = showSelectUsersToRoleDialog(xd);

	if (strXml.length > 0)
	{
		xmlSelect = createDomDocument(strXml);
		
		var xmlDoc = createDomDocument("<addSecsToLeader />");
		var root = xmlDoc.documentElement;
		root.setAttribute("GUID", m_LeaderGuid);
		root.setAttribute("USER_ACCESS_LEVEL", xmlSelect.documentElement.getAttribute("ACCESS_LEVEL"));
		root.setAttribute("extAttr", "E_MAIL,DESCRIPTION,RANK_CODE,NAME");
		
		root.setAttribute("START_TIME", xmlSelect.documentElement.getAttribute("START_TIME"));
		root.setAttribute("END_TIME", xmlSelect.documentElement.getAttribute("END_TIME"));
		
		var sNode = xmlSelect.documentElement.firstChild;
		while (sNode)
		{
			root.appendChild(sNode.cloneNode(true));
			sNode = sNode.nextSibling;
		}
		
		var xmlResult = xmlSend("../sysAdmin/OGUEditer.aspx", xmlDoc);
		
		checkErrorResult(xmlResult);
		
		fillDataToGrid(xmlResult);
	}
}

function displaySortOrderSave(bShow)
{
	
}

function listMenuBeforePopup()
{
	try
	{
		switch (event.menuData)
		{
			case "newUser":		event.disableItem = (!bShowNewUser);
				break;
			case "initPassword":event.disableItem = (!bInitPassword);
				break;
			case "delete":		event.disableItem = (!bShowDelete);
				break;
			default :
				break;
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
			case "delete":			if (m_txtSelection.text.length == 0)
									{
										var rgn = document.body.createTextRange();
										rgn.moveToElementText(oTD);
										rgn.select();
									}
									break;
			case "initPassword":	InitPassword(listMenu.row.xml);
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

function oBodyContextMenu()
{
	try
	{
		var oTR = getOwnerTR(event.srcElement);

		listMenu.row = oTR;
		m_txtSelection = document.selection.createRange();
		
		if (m_txtSelection.htmlText == "")
		{
			var oTD = getRelativeTD(oTR, "DISPLAY_NAME")
			var rgn = document.body.createTextRange();
			rgn.moveToElementText(oTD);
			rgn.select();
		}

		listMenu.show(event.x, event.y);
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

//-->