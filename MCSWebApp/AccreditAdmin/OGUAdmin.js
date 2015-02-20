<!--
// ���ڻ�����չ��ʱ���Ƿ�չ�֡�����վ�����á�
var m_ShowTrash = 1;
//Ҫ��չ�ֵ�"������"
var _OGU_RootOrg = "";

//Ҫ��չ�ֵ���ز�������
var _OGU_Params = C_LIST_ORGANIZATION;

//����ĳ�ʼ������Ӧonload�¼�
function onDocumentLoad()
{
	try
	{
		startclock();
		initSplitter(splitterContainer);//splitter.js
		tv.oncontextmenu = onDefaultContextMenu;
		document.title += "(" + currentUserName.value + ")";//���ñ����е����
		
		getRoot(tv, _OGU_Params, null, _OGU_RootOrg, null, null, syncCallBack);
	}
	catch (e)
	{
		showError(e);
	}
}

//�˵�ѡ���ϵ�Ĭ�ϲ���
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

//getRoot��CallBack���ã��ڻ������еĽ�����
function syncCallBack(tv, xmlResult)
{

}

//������˳�����Ӧonunload�¼�
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

//�˵��ĳ�ʼ��
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

//�˵��ĵ����Ӧ
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
				trueThrow(true, "�Բ���û�ж�Ӧ�ڡ�" + a.objectClass + "����ƥ��������ͣ�");
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
			
			if (tNode.xData.waitforLoad == false && bReflash)//��Ϊ�޸��˶���ģϣ£ʣߣΣ��ͣŵ���������ڵ��ϵģ��̣̣ߣУ��ԣȣߣΣ��ͣ�Ҳ���޸�
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


//���б������ͨ������syncData�������˱仯������ͬ��
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

//������Ӧ
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
					// �������վ
					var trashTreeNode = parentNode.getChild().getLastSibling();
					var trashXml = null;
					if (trashTreeNode.xData.xNode.nodeName == "TRASH")
					{
						trashXml = createDomDocument(trashTreeNode.xData.xNode.xml)
						trashTreeNode.remove();
					}
					
					// ��ɲ�������ڵ�
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
					// ��ԭ����վ
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
	/*����ԭ����λ��*/	
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
	/*������λ��*/
	var strResOriginalSort = root.getAttribute("ORIGINAL_SORT");
	var parentNode = searchObjInTreeByAttr(tv, strResOriginalSort, "ORIGINAL_SORT", false);
	if (parentNode && !parentNode.xData.waitforLoad)//Ŀ�Ľڵ㱻չ��
	{
		/*�������վ*/
		var trashTreeNode = parentNode.getChild().getLastSibling();
		var trashXml = null;
		if (trashTreeNode.xData.xNode.nodeName == "TRASH")
		{
			trashXml = createDomDocument(trashTreeNode.xData.xNode.xml)
			trashTreeNode.remove();
		}
		/*��ɲ�������ڵ�*/
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
		
		/*��ԭ����վ*/
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

//���ṹ�ڵ㱻ѡ�ж�Ӧ����Ӧ
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
//			var strTrashOriginalSort = xNode.getAttribute("TRASH_ORIGINAL_SORT");//Ϊ����վ����
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

//��Ի������еĽڵ�ѡ��
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

//���ṹ�нڵ㱻չ����Ӧ����Ӧ
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

//���ṹ�нڵ㱻����Ҽ�ѡ�У�Ĭ�ϵ����˵���
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
