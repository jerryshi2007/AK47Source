<!--
/******************************************************************************************/
//���ڼ�¼��ǰ������Ա��Ȩ�ޣ��ж��Ƿ�չ��toolbar�ϵ�ͼ���Լ��Ƿ�չ�ֲ˵��еġ���ҡ�ѡ�
var bShowNewUser = false;
var bShowSort = false;
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
					case "GROUP_ADD_USERS":	bShowNewUser = true;
						break;
					case "GROUP_DEL_USERS":	bShowDelete = true;
						break;
					case "SORT_IN_GROUP":	bShowSort = true;
						break;
					case "INIT_USERS_PWD":	bInitPassword = true;
						break;
					default :
						break;
				}
			}
		}
		
		document.all("newUser").style.display = bShowNewUser ? "inline" : "none";
		document.all("sort").style.display = bShowSort ? "inline" : "none";
		document.all("delete").style.display = bShowDelete ? "inline" : "none";
	}		
}
/******************************************************************************************/
var m_GroupGuid = null;
function onDocumentLoad()
{
	try
	{
		m_GroupGuid = groupGuid.value;
		displaySortOrderSave(false);
		createTableHeadByXml(ColumnTitle.XMLDocument, ouUserHeader);
		m_PageSize = txtPageSize.value;
		reFreshPageData(true);
	}
	catch (e)
	{
		showError(e);
		window.close();
	}
}

function fillDataToGrid(xmlResult)
{
	ouUserListBody.noSelect = true;

	ouUserTable.refreshData = refreshData;
	ouUserTable.listSortId = m_PageNo * m_PageSize + 1;
	
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
	
	displaySortOrderSave(false);
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
							break;
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
		case "newUser":	AddUsersToGroups();
						break;
		case "refresh":	refreshData();
						break;
		case "search":	//var xmlDoc = createDomDocument(ADSearchConfig.XMLDocument.xml);
						//appendNode(xmlDoc.documentElement, "RootOrg", C_OGU_ROOT_NAME);
						//showSelectUsersToRoleDialog(xmlDoc);
						searchUsersInGroup();
						break;
		case "sort":	if (confirm("ȷ��Ҫ���浱ǰ��Ա���г�Ա����ʾ˳����"))
							saveSortOrder();
						break;
		case "delete":	deleteSelectedObjects();
						break;
		case "save":	saveSortOrder();
						break;
	}
}

function searchUsersInGroup()
{
	trueThrow(txtSearchObj.value.replace("*", "") == "", "�Բ���������Ҫ���ѯ��������ƣ�");
	m_PageNo = 0;
	m_SearchName = txtSearchObj.value.replace("*", "");
	reFreshPageData(true);
}

function saveSortOrder()
{
	falseThrow(m_SearchName == "", "��ǰ������ʾΪ��ѯ״̬�����ܱ���ô���");
	var xmlDoc = createDomDocument("<GroupSort />");
	var root = xmlDoc.documentElement;
		
	root.setAttribute("GUID", m_GroupGuid);
	root.setAttribute("extAttr", "E_MAIL,DESCRIPTION,RANK_CODE,NAME");

	for (var i = 0; i < ouUserListBody.rows.length; i++)
	{
		var oTR = ouUserListBody.rows[i];

		if (oTR && oTR.guid)
		{
			var userNode = appendNode(root, "USERS");
			appendNode(userNode, "USER_PARENT_GUID", oTR.parentGuid);
			appendNode(userNode, "USER_GUID", oTR.guid);
		}
	}

	if (xmlDoc.documentElement.childNodes.length > 0)
	{
		m_SearchName = "";
		root.setAttribute("extAttr", "E_MAIL,DESCRIPTION,RANK_CODE,NAME");
		root.setAttribute("PageNo", m_PageNo);
		root.setAttribute("PageSize", m_PageSize);
		root.setAttribute("PageSort", m_SortColumnName);
		root.setAttribute("GetCount", "true");
		root.setAttribute("SearchName", m_SearchName);
		
		var xmlResult = xmlSend("../sysAdmin/OGUEditer.aspx", xmlDoc);

		checkErrorResult(xmlResult);
		
		root = xmlResult.documentElement;
		m_RecordCount = root.getAttribute("GetCount");
		m_PageCount = m_RecordCount == 0 ? 1 : Math.floor((m_RecordCount - 1) / m_PageSize) + 1;
		spUserCount.innerText = m_RecordCount;
		spUserPage.innerText = (m_PageNo + 1) + "/" + m_PageCount
		
		fillDataToGrid(xmlResult);
		reFreshPageIndex();
	}
}

function deleteSelectedObjects()
{
	var strResult = null;

	var xmlDoc = createDomDocument("<delUsersFromGroups />");

	var root = xmlDoc.documentElement;
	
	root.setAttribute("GUID", m_GroupGuid);
	
	enumSelection(root);

	trueThrow(root.childNodes.length == 0, "�Բ�����û��ѡ��Ҫ��ɾ���Ķ���");
	
	if (confirm("ȷ��Ҫɾ��ѡ���Ķ�����"))
	{
		m_PageNo = 0;
		m_SearchName = "";
		root.setAttribute("extAttr", "E_MAIL,DESCRIPTION,RANK_CODE,NAME");
		root.setAttribute("PageNo", m_PageNo);
		root.setAttribute("PageSize", m_PageSize);
		root.setAttribute("PageSort", m_SortColumnName);
		root.setAttribute("GetCount", "true");
		root.setAttribute("SearchName", m_SearchName);
		
		var xmlResult = xmlSend("../sysAdmin/OGUEditer.aspx", xmlDoc);

		checkErrorResult(xmlResult);
		
		m_SearchName = "";
		root = xmlResult.documentElement;
		m_RecordCount = root.getAttribute("GetCount");
		m_PageCount = m_RecordCount == 0 ? 1 : Math.floor((m_RecordCount - 1) / m_PageSize) + 1;
		spUserCount.innerText = m_RecordCount;
		spUserPage.innerText = (m_PageNo + 1) + "/" + m_PageCount
		
		fillDataToGrid(xmlResult);
		reFreshPageIndex();
	}
}

function refreshData()
{
	m_SearchName = "";
	txtSearchObj.value = "";
	reFreshPageData(true);
	displaySortOrderSave(false);
}

function AddUsersToGroups()
{
	var xd = createDomDocument("<Config><RootOrg>" + C_DC_ROOT_NAME + "</RootOrg><Caption>��Ա���мӳ�Ա</Caption></Config>");
	var strXml = showSelectUsersToRoleDialog(xd);

	if (strXml.length > 0)
	{
		xmlSelect = createDomDocument(strXml);
		
		var xmlDoc = createDomDocument("<addObjectsToGroups />");
		var root = xmlDoc.documentElement;
		root.setAttribute("USER_ACCESS_LEVEL", xmlSelect.documentElement.getAttribute("ACCESS_LEVEL"));
		root.setAttribute("GUID", m_GroupGuid);
		root.setAttribute("extAttr", "E_MAIL,DESCRIPTION,RANK_CODE,NAME");

		var sNode = xmlSelect.documentElement.firstChild;
		while (sNode)
		{
			root.appendChild(sNode.cloneNode(true));
			sNode = sNode.nextSibling;
		}
		
		m_SearchName = "";
		root.setAttribute("PageNo", m_PageNo);
		root.setAttribute("PageSize", m_PageSize);
		root.setAttribute("PageSort", m_SortColumnName);
		root.setAttribute("GetCount", "true");
		root.setAttribute("SearchName", m_SearchName);
		
		var xmlResult = xmlSend("../sysAdmin/OGUEditer.aspx", xmlDoc);
		
		checkErrorResult(xmlResult);
		root = xmlResult.documentElement;
		m_RecordCount = root.getAttribute("GetCount");
		m_PageCount = m_RecordCount == 0 ? 1 : Math.floor((m_RecordCount - 1) / m_PageSize) + 1;
		spUserCount.innerText = m_RecordCount;
		spUserPage.innerText = (m_PageNo + 1) + "/" + m_PageCount
		
		fillDataToGrid(xmlResult);
		reFreshPageIndex();
	}
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
			case "sort":		event.disableItem = (!bShowSort);
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
			case "resetPassword":	showResetPasswordDialog(listMenu.row.xml);
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

function displaySortOrderSave(bVisible)
{
	var strDisplay = "none";

	if (bVisible && m_SearchName == "")
		strDisplay = "inline";

	saveSpan.style.display = strDisplay;
	save.style.display = strDisplay;
}

//===================================================================================
var m_PageNo = 0;
var m_PageCount = 1;
var m_PageSize = 15;
var m_RecordCount = 0;
var m_SortColumnName = "GROUP_USERS.INNER_SORT";
var m_SearchName = "";
function onGotoPage(action)
{
	try
	{
		var bRefresh = true;
		if (!document.all(action).disabled)
		{
			switch(action)
			{
				case "firstPage":	m_PageNo = 0;
									break;
				case "prePage":		m_PageNo--;
									break;
				case "nextPage":	m_PageNo++;
									break;
				case "lastPage":	m_PageNo = m_PageCount - 1;
									break;
				case "aimPage":		trueThrow(m_PageCount < txtAimPage.value, "��ע�⣺��ǰ���ҳ��Ϊ" + m_PageCount + "!");
									bRefresh = m_PageNo != txtAimPage.value - 1;
									m_PageNo = txtAimPage.value - 1;
									break;
				default :			trueThrow(true, "�Բ���" + action + "��������ϵͳ�б�׼������");
									break;
			}
			if (bRefresh)
				reFreshPageData(false);
		}
	}
	catch (e)
	{
		showError(e);
	}
	finally
	{
		return false;
		event.returnValue = false;
	}
}

function reFreshPageData(bGetCount)
{
	var xmlDoc = createCommandXML("getUsersInGroupsInPage");
		
	var root = xmlDoc.documentElement;
	root.setAttribute("GUID", m_GroupGuid);
	root.setAttribute("extAttr", "E_MAIL,DESCRIPTION,RANK_CODE,NAME");
	root.setAttribute("PageNo", m_PageNo);
	root.setAttribute("PageSize", m_PageSize);
	root.setAttribute("PageSort", m_SortColumnName);
	root.setAttribute("SearchName", m_SearchName);
	if (bGetCount)
		root.setAttribute("GetCount", "true");
	
	var xmlResult = xmlSend("../SysSearch/OGUSearch.aspx", xmlDoc);

	checkErrorResult(xmlResult);
	if (bGetCount)
	{
		root = xmlResult.documentElement;
		m_RecordCount = root.getAttribute("GetCount");
		m_PageCount = m_RecordCount == 0 ? 1 : Math.floor((m_RecordCount - 1) / m_PageSize) + 1;
	}
	spUserCount.innerText = m_RecordCount;
	spUserPage.innerText = (m_PageNo + 1) + "/" + m_PageCount
	fillDataToGrid(xmlResult);
	reFreshPageIndex();
}

function reFreshPageIndex()
{
	prePage.disabled = firstPage.disabled = m_PageNo == 0;
	nextPage.disabled = lastPage.disabled = m_PageNo == m_PageCount - 1;
}
//-->