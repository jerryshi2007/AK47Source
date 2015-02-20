<!--
// 用于机构树展现时候是否展现“回收站的设置”
var m_ShowTrash = 1;
//要求展现的"根机构"
var _OGU_RootOrg = "";

//要求展现的相关参数配置
var _OGU_Params = C_LIST_ORGANIZATION;

//界面的初始化，响应onload事件
function onDocumentLoad()
{
	try
	{
		startclock();
		initSplitter(splitterContainer);//splitter.js
		tv.oncontextmenu = onDefaultContextMenu;
		document.title += "(" + currentUserName.value + ")";//设置标题中的身份
		
		getRoot(tv, _OGU_Params, null, _OGU_RootOrg, null, null, syncCallBack);
	}
	catch (e)
	{
		showError(e);
	}
}

//菜单选择上的默认操作
function onDefaultContextMenu()
{
	try
	{
		event.returnValue = false;	
	}
	catch (e)
	{
		showError(e);
	}
}

//getRoot的CallBack调用（在机构树中的建立）
function syncCallBack(tv, xmlResult)
{

}

//界面的退出，响应onunload事件
function onDocumentUnload()
{
	try
	{
		if (paramValue.value.length > 0)
			setPersistProperty("lastAccessDN", paramValue.value);
	}
	catch (e)
	{
//		showError(e);
	}
}

//菜单的初始化
function menuBeforePopup()
{
	try
	{
		var tNode = menuTree.lastNode;
		
		switch(event.menuData)
		{
			case "import":
			case "export":	event.disableItem = (tNode.xData.xNode.nodeName == "TRASH");
				break;
		}
	}
	catch (e)
	{
		showError(e);
	}
}

//菜单的点击响应
function menuTreeClick()
{
	var tNode = menuTree.lastNode;

	try
	{
		if (tNode)
		{
			var xNode = tNode.xData.xNode;

			switch(event.menuData)
			{
				case "refresh":	if (xNode.nodeName != "TRASH")
									loadChildren(tNode, C_LIST_ORGANIZATION, null, false);
								break;
				case "syncDepartment":
								break;
				case "ouChart": break;
				case "search":	var xmlDoc = createDomDocument(ADSearchConfig.XMLDocument.xml);
								appendNode(xmlDoc.documentElement, "RootOrg", C_DC_ROOT_NAME);
								var xmlResult = showSelectUsersToRoleDialog(xmlDoc);
								setAutoExpand(top.tv, xmlResult, true);
								break;
				case "property":treeNodeProperty(tNode);
								break;
				case "export":	showSystemDataExport(tNode);
								break;
				case "import":	showSystemDataImport(tNode);
								break;
			}
		}
	}
	catch(e)
	{
		showError(e);
	}
}

function showSystemDataExport(tNode)
{
	try
	{
		var sURL = "./dialogs/PrepareForExport.aspx?allPathName=" + tNode.xData.xNode.getAttribute("ALL_PATH_NAME") + "&guid=" + tNode.xData.xNode.getAttribute("GUID"); 
		
		var iHeight = 360;
		var iTop = (window.screen.height - iHeight) / 2;
		var iWidth = 480;
		var iLeft = (window.screen.width - iWidth) / 2;
		
		var sFeature = "left=" + iLeft + "px,top=" + iTop + "px,width=" + iWidth + "px,height=" + iHeight + "px,menubar=no,toolbar=no,location=no,resizable=yes,scroll:auto,status:no";
		
		window.open(sURL, "PrepareForExport", sFeature);
	}
	catch (e)
	{
		showError(e);
	}
}

function showSystemDataImport(tNode)
{
	try
	{
		var sURL = "./dialogs/PrepareForImport.aspx"; 
		
		var iHeight = 240;
		var iTop = (window.screen.height - iHeight) / 2;
		var iWidth = 480;
		var iLeft = (window.screen.width - iWidth) / 2;
		
		var sFeature = "left=" + iLeft + "px,top=" + iTop + "px,width=" + iWidth + "px,height=" + iHeight + "px,menubar=no,toolbar=no,location=no,resizable=yes,scroll:auto,status:no";
		
		window.open(sURL, "PrepareForImport", sFeature);
		
	}
	catch (e)
	{
		showError(e);
	}
}

function treeNodeProperty(tNode)
{
	try
	{
		var strParentGuid = tNode.xData.xNode.getAttribute("PARENT_GUID")
		var strGuid = tNode.xData.xNode.getAttribute("GUID")
		
		switch (tNode.xData.xNode.nodeName)
		{
			case "ORGANIZATIONS":
				strXml = showOUDetailDialog("Update", strParentGuid, strGuid);
				break;
			case "GROUPS":
				strXml = showGroupDetailDialog("Update", strParentGuid, strGuid);
				break;
			case "USERS":
				strXml = showUSERDetailDialog("Update", strParentGuid, strGuid, null);
				break;
			default :
				trueThrow(true, "对不起，没有对应于“" + a.objectClass + "”的匹配对象类型！");
				break;
		}

		if (strXml != null && strXml.length > 0)
		{			
			var xmlResult = createDomDocument(strXml);
			var eleNode = xmlResult.documentElement.firstChild;
			
			var tvNodeData = tNode.xData.xNode;
			var bReflash = false;
			for (var i = 0; i < eleNode.attributes.length; i++)
			{
				if (eleNode.attributes.item(i).nodeName != "OriginalSort")
					tvNodeData.setAttribute(eleNode.attributes.item(i).nodeName, eleNode.attributes.item(i).nodeValue);
				if (eleNode.attributes.item(i).nodeName == "OBJ_NAME")
					bReflash = true;
			}
			tNode.putText(tvNodeData.getAttribute("DISPLAY_NAME"));
			setNodeTitle(tNode, tvNodeData);
			
			if (tNode.xData.waitforLoad == false && bReflash)//因为修改了对象的ＯＢＪ＿ＮＡＭＥ导致其子孙节点上的ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ也会修改
				loadChildren(tNode, C_LIST_ORGANIZATION, null, false);
		}
	}
	catch(e)
	{
		showError(e);
	}
	finally
	{
		event.cancelBubble = true;
		event.returnValue = false;

		return event.returnValue;
	}
}


//和列表区域的通信区（syncData）发生了变化的数据同步
function onSyncDataChange()
{
	try
	{
		if ((event.propertyName == "value") && (syncData.value.length > 0))
		{
			doSyncData(syncData.value);
			syncData.value = "";
		}
	}
	catch (e)
	{
		showError(e);
	}
}

//操作响应
function doSyncData(strXml)
{
	var xmlDoc = createDomDocument(strXml);

	switch(xmlDoc.documentElement.nodeName)
	{
		case "furbishDelete":
		case "Insert":	syncInsertOP(xmlDoc);
						break;
		case "Update":	syncUpdateOP(xmlDoc);
						break;
		case "logicDelete":
		case "Delete":	syncDeleteOP(xmlDoc);
						break;
		case "Move":	syncMoveOP(xmlDoc);
						break;
	}
}

function syncUpdateOP(xmlDoc)
{
	var root = xmlDoc.documentElement;
	var eleNode = root.firstChild;
	while (eleNode != null)
	{
		var strOriginalSort = eleNode.getAttribute("OriginalSort");
		if (strOriginalSort && strOriginalSort.length > 0)
		{
			var treeNode = searchObjInTreeByAttr(tv, strOriginalSort, "ORIGINAL_SORT", false);
			if (treeNode != null)
			{
				var tvNodeData = treeNode.xData.xNode;
				for (var i = 0; i < eleNode.attributes.length; i++)
				{
					if (eleNode.attributes.item(i).nodeName != "OriginalSort")
						tvNodeData.setAttribute(eleNode.attributes.item(i).nodeName, eleNode.attributes.item(i).nodeValue);
				}
				treeNode.putText(tvNodeData.getAttribute("DISPLAY_NAME"));
				setNodeTitle(treeNode, tvNodeData);
			}
		}
		eleNode = eleNode.nextSibling;
	}
}

function syncDeleteOP(xmlDoc)
{
	var xmlNode = xmlDoc.documentElement.firstChild;

	while(xmlNode)
	{
		if (xmlNode.tagName == "ORGANIZATIONS")
		{
			var strOriginalSort = xmlNode.getAttribute("ORIGINAL_SORT");
			var treeNode = searchObjInTreeByAttr(tv, strOriginalSort, "ORIGINAL_SORT", false);
			if (treeNode)
				treeNode.remove();
		}
		xmlNode = xmlNode.nextSibling;
	}
}

function syncInsertOP(xmlDoc)
{
	var eleRoot = xmlDoc.documentElement.firstChild;
	while (eleRoot)
	{
		var strOriginalSort = eleRoot.getAttribute("ORIGINAL_SORT");
		var strParentOriginalSort = strOriginalSort.substring(0, strOriginalSort.length - C_OGU_ORIGINAL_SORT.length);
		var parentNode = searchObjInTreeByAttr(tv, strParentOriginalSort, "ORIGINAL_SORT", false);

		if (parentNode && !parentNode.xData.waitforLoad)
		{
			var xNode = parentNode.xData.xNode;
			if (xNode.nodeName == C_ORGANIZATIONS)
			{
	//			if (xmlDoc.documentElement.nodeName == "furbishDelete")
	//			{
	//				loadChildren(parentNode, C_LIST_ORGANIZATION, null, false);
	//			}
	//			else
				{
					// 清除回收站
					var trashTreeNode = parentNode.getChild().getLastSibling();
					var trashXml = null;
					if (trashTreeNode.xData.xNode.nodeName == "TRASH")
					{
						trashXml = createDomDocument(trashTreeNode.xData.xNode.xml)
						trashTreeNode.remove();
					}
					
					// 完成插入机构节点
//					while(eleRoot)
					{
						if (eleRoot.nodeName == C_ORGANIZATIONS)
						{
							var strName = eleRoot.getAttribute("DISPLAY_NAME");
							var imgTemp = getImgFromClass(eleRoot.tagName);
							var nMb = parentNode.add("tvwChild", "", strName, "/" + C_ACCREDIT_ADMIN_ROOT_URI + "/images/" + imgTemp);

							nMb.xData.xNode = eleRoot;
							addLoadingNode(nMb);

							setNodeTitle(nMb, eleRoot);
						}
//						eleRoot = eleRoot.nextSibling;
					}
					// 还原回收站
					if (trashXml)
					{
						var trashRoot = trashXml.documentElement;
						strName = trashRoot.getAttribute("DISPLAY_NAME");
						imgTemp = getImgFromClass(trashRoot.tagName);
						nMb = parentNode.add("tvwChild", "", strName, "/" + C_ACCREDIT_ADMIN_ROOT_URI + "/images/" + imgTemp);

						nMb.xData.xNode = trashRoot;

						setNodeTitle(nMb, trashRoot);
					}
				}
			}
		}
		
		eleRoot = eleRoot.nextSibling;
	}
}

function syncMoveOP(xmlDoc)
{
	var root = xmlDoc.documentElement;
	/*清理原来的位置*/	
	var xmlNode = root.firstChild;

	while(xmlNode)
	{
		if (xmlNode.tagName == C_ORGANIZATIONS)
		{
			var strOriginalSort = xmlNode.getAttribute("OLD_ORIGINAL_SORT");
			var treeNode = searchObjInTreeByAttr(tv, strOriginalSort, "ORIGINAL_SORT", false);
			if (treeNode)
				treeNode.remove();
		}
		xmlNode = xmlNode.nextSibling;
	}
	/*放入新位置*/
	var strResOriginalSort = root.getAttribute("ORIGINAL_SORT");
	var parentNode = searchObjInTreeByAttr(tv, strResOriginalSort, "ORIGINAL_SORT", false);
	if (parentNode && !parentNode.xData.waitforLoad)//目的节点被展开
	{
		/*清除回收站*/
		var trashTreeNode = parentNode.getChild().getLastSibling();
		var trashXml = null;
		if (trashTreeNode.xData.xNode.nodeName == "TRASH")
		{
			trashXml = createDomDocument(trashTreeNode.xData.xNode.xml)
			trashTreeNode.remove();
		}
		/*完成插入机构节点*/
		xmlNode = root.firstChild;
		while(xmlNode)
		{
			if (xmlNode.nodeName == C_ORGANIZATIONS)
			{
			
				var strName = xmlNode.getAttribute("DISPLAY_NAME");
				var imgTemp = getImgFromClass(xmlNode.tagName);
				var nMb = parentNode.add("tvwChild", "", strName, "/" + C_ACCREDIT_ADMIN_ROOT_URI + "/images/" + imgTemp);

				nMb.xData.xNode = xmlNode;
				addLoadingNode(nMb);

				setNodeTitle(nMb, xmlNode);
			}
			xmlNode = xmlNode.nextSibling;
		}
		
		/*还原回收站*/
		if (trashXml)
		{
			var trashRoot = trashXml.documentElement;
			strName = trashRoot.getAttribute("DISPLAY_NAME");
			imgTemp = getImgFromClass(trashRoot.tagName);
			nMb = parentNode.add("tvwChild", "", strName, "/" + C_ACCREDIT_ADMIN_ROOT_URI + "/images/" + imgTemp);

			nMb.xData.xNode = trashRoot;

			setNodeTitle(nMb, trashRoot);
		}
	}
}

/*
function syncAppendAttr(elem, nodeSet, strName)
{
	var node = nodeSet.selectSingleNode(strName);

	if (node)
		elem.setAttribute(strName, node.text);
}*/

//树结构节点被选中对应的响应
function tvNodeSelected()
{
	try
	{
		var xNode = event.node.xData.xNode;

		if (xNode)
		{
			var strGuid = xNode.getAttribute("GUID");
			var strAllPathName = xNode.getAttribute("ALL_PATH_NAME");
			var strDisplayName = xNode.getAttribute("DISPLAY_NAME");
			var strOriginalSort = xNode.getAttribute("ORIGINAL_SORT");
//			var strTrashOriginalSort = xNode.getAttribute("TRASH_ORIGINAL_SORT");//为回收站设置
			openInnerDocument("./itemList/OGUList.aspx", strGuid, strDisplayName, strAllPathName, strOriginalSort);//, strTrashOriginalSort);
		}
		else
			event.returnValue = false;
	}
	catch(e)
	{
		showError(e);
	}
}

//针对机构树中的节点选择
function openInnerDocument(strURL, strGuid, strDisplayName, strAllPathName, strOriginalSort)//, strTrashOriginalSort)
{
	//	paramValue.value = "<Parent rootOrgGuid=\"" + strGuid + "\" DISPLAY_NAME=\"" + strDisplayName + "\" ALL_PATH_NAME=\"" + strAllPathName + "\" ORIGINAL_SORT=\"" + strOriginalSort + "\" TRASH_ORIGINAL_SORT=\"" + strTrashOriginalSort + "\" />";
	//paramValue.value = "<Parent rootOrgGuid=\"" + strGuid + "\" DISPLAY_NAME=\"" + strDisplayName + "\" ALL_PATH_NAME=\"" + strAllPathName + "\" ORIGINAL_SORT=\"" + strOriginalSort + "\" />";
	var xmlDoc = createDomDocument("<Parent/>");
	var root = xmlDoc.documentElement;
	
	root.setAttribute("rootOrgGuid", strGuid);
	root.setAttribute("DISPLAY_NAME", strDisplayName);
	root.setAttribute("ALL_PATH_NAME", strAllPathName);
	root.setAttribute("ORIGINAL_SORT", strOriginalSort);
	
	var objInputParam = frmContainer.document.all("inputParam");

	paramValue.value = xmlDoc.xml;

	if (!objInputParam)
		innerDocTD.innerHTML = "<iframe id='frmContainer' src='" + strURL + "' style='WIDTH:100%;HEIGHT:100%' frameborder='0' scrolling='auto'></iframe>";
	else
		objInputParam.value = paramValue.value;
}

//树结构中节点被展开对应的响应
function tvNodeExpand()
{
	try
	{
		var activeNode = event.node;
		var bASync = event.fromClick;

//		if (activeNode.xData.waitforLoad)//changed by yuanyong 2005-4-11
		if (activeNode.xData.waitforLoad && activeNode.xData.xNode.nodeName != "TRASH")//changed by yuanyong 2005-4-11
			loadChildren(activeNode, C_LIST_ORGANIZATION, null, bASync);
	}
	catch(e)
	{
		showError(e);
	}
}

//树结构中节点被鼠标右键选中（默认弹出菜单）
function tvNodeRightClick()
{
	try
	{
		var tNode = event.node;

		if (tNode)
		{
			menuTree.lastNode = tNode;
			menuTree.show(event.x, event.y);
		}
	}
	catch(e)
	{
		showError(e);
	}
}

//-->
