<!--
var C_LIST_ORGANIZATION = 1;
var C_LIST_USER = 2;
var C_LIST_GROUP = 4;
var C_LIST_SIDELINE = 8;
var C_LIST_ROLE = 64;
var C_LIST_ALL = 65535;
//var ACCOUNTDISABLE = 0x0002;	

var m_extAttr = null;

function setListType(xmlDoc, listObjectType)
{
	var m;

	if (listObjectType)
		m = listObjectType;
	else
		m = C_LIST_ORGANIZATION;

	appendAttr(xmlDoc.documentElement, "listObjectType", m);
}

function loadChildrenData(treeNode, xNode, listObjectType, callBack, bAsync, argObj)
{
	var xmlDoc = createCommandXML("getOrganizationChildren");//, xNode.getAttribute("GUID"));
	var root = xmlDoc.documentElement;
	root.setAttribute("rootOrgGuid", xNode.getAttribute("GUID"));
	
	setListType(xmlDoc, listObjectType);

	if (m_extAttr)
		xmlDoc.documentElement.setAttribute("extAttr", m_extAttr);

	var params = new Object();
	
	params.xNode = xNode;
	params.treeNode = treeNode;
	params.callBack = callBack;

	var xmlResult = null;
	
	setRequestAttributeByArg(root, argObj);

	if (bAsync)
		xmlResult = xmlSend("/" + C_ACCREDIT_ADMIN_ROOT_URI + "/sysSearch/OGUSearch.aspx", xmlDoc, afterLoadChildrenData, params);
	else
	{
		xmlResult = xmlSend("/" + C_ACCREDIT_ADMIN_ROOT_URI + "/sysSearch/OGUSearch.aspx", xmlDoc);

		afterLoadChildrenData(xmlResult, params);
	}
}

function afterLoadChildrenData(xmlResult, params)
{
	try
	{
		checkErrorResult(xmlResult);

		var xNode = params.xNode;

		while(xNode.childNodes.length > 0)
			xNode.removeChild(xNode.firstChild);

		var root = xmlResult.documentElement;
		var xmlSub = root.firstChild;
		
		while (xmlSub != null)
		{
			while (xmlSub.childNodes.length > 0)
				xNode.appendChild(xmlSub.childNodes[0]);

			var treeNode = params.treeNode;
			treeNode.xData.waitforLoad = false;
			treeNode.removeChildren();

			showChildrenMember(xNode, treeNode, params.callBack);
			
			xmlSub = xmlSub.nextSibling;
		}
	}
	catch(e)
	{
		showError(e);
	}
}

function loadChildren(activeNode, nMask, callBack, bAsync, argObj)
{
	var xNode = activeNode.xData.xNode;
	loadChildrenData(activeNode, xNode, nMask, callBack, bAsync, argObj);
}

function showChildrenMember(xNode, nFather, callBack)
{
	var imgName;
	
	if (m_ShowTrash)
	{
		var trashNode = OGUTreeAddTrash(xNode);
		xNode.appendChild(trashNode.cloneNode(true));
	}
	var node = xNode.firstChild;

	while(node)
	{
		var objClass = node.tagName;
		
		if (objClass != "ROLES" || node.childNodes.length != 0)
		{				
			var imgTemp = getImgFromClass(objClass);
			
			if (node.getAttribute("STATUS") != null && node.getAttribute("STATUS") != "" && node.getAttribute("STATUS") != "1")
				imgTemp = getImgFromClass("TRASH_" + node.tagName);
			
			if (objClass == C_USERS && node.getAttribute("POSTURAL") != "")
				if (parseInt(node.getAttribute("POSTURAL")) & 1 != 0)
					imgTemp = getImgFromClass("ForbiddenUser");

			var nMb = nFather.add("tvwChild", "", node.getAttribute("DISPLAY_NAME"), "/" + C_ACCREDIT_ADMIN_ROOT_URI + "/images/" + imgTemp);

			nMb.xData.xNode = node;
			nMb.xData.waitforLoad = true;

			setNodeTitle(nMb, node);

			if (node.tagName == C_ORGANIZATIONS)
				addLoadingNode(nMb);

			if (callBack)
				callBack(nMb, node);
				
			var childNode = node.firstChild;
			
			if (childNode)
			{
				if (nMb.getChild())
					nMb.getChild().remove();
					
				nMb.xData.waitforLoad = false;
				showChildrenMember(node, nMb, callBack);
			}
		}
		
		node = node.nextSibling;
	}
	
	nFather.setExpanded(true);
}

function addLoadingNode(n)
{
	n.xData.waitforLoad = true;
	n.add("tvwChild", "", "载入中...");
}

function getReversePath(strDN, strSplitChar, strIgnoreTag)
{
	var arrParts = strDN.split(",");
	var strB = "";
	var spc = ".";

	if (strSplitChar)
		spc = strSplitChar;

	for (var i = arrParts.length - 1; i >= 0; i--)
	{
		if (strB.length > 0)
			strB += spc;

		var strKeyValue = arrParts[i].split("=");

		if (strKeyValue[0] != strIgnoreTag)
			strB += strKeyValue[1];
	}

	return strB;
}

function recursiveExpand(tree, root, strAutoExpand)
{
	var node = root.firstChild;

	while(node)
	{
		var	xNode = node.xData.xNode;
		var strReverseDN = getReversePath(xNode.getAttribute("dn"));

		if (strReverseDN == strAutoExpand.substr(0, strReverseDN.length))
		{
			node.setExpanded(true, false);

			tree.selectedItem = node;

			if (strReverseDN.length != strAutoExpand.length)
				recursiveExpand(tree, node, strAutoExpand);

			break;
		}
		node = node.getNext();
	}
}

function setAutoExpand(tree, xmlAutoExpand, bFireEvent)
{
	var oldItem = tree.selectedItem;
	var xmlDoc = null;

	if (typeof(xmlAutoExpand) == "object")
		xmlDoc = xmlAutoExpand;
	else
	{
		if (xmlAutoExpand.length > 0)
			xmlDoc = createDomDocument(xmlAutoExpand);
	}

	if (xmlDoc)
	{
		var xNode = xmlDoc.documentElement.firstChild;

		while(xNode)
		{
			var strAllPathName = xNode.getAttribute("ALL_PATH_NAME");
			if (strAllPathName && strAllPathName.length > 0)
				searchObjInTreeByAttr(tree, strAllPathName, "ALL_PATH_NAME", true);
				
			xNode = xNode.nextSibling;
		}

		if (bFireEvent && oldItem != tree.selectedItem && tree.selectedItem)
			if (!tree.selectNode(tree.selectedItem))
				tree.selectedItem = oldItem;
	}
}

/*
function setAutoExpand2(tree, strDN, bFireEvent)
{
	var oldItem = tree.selectedItem;

	var strAutoExpand = getReversePath(strDN);
	var root = tree.Nodes[0];

	recursiveExpand(tree, root, strAutoExpand);

	if (bFireEvent && oldItem != tree.selectedItem && tree.selectedItem)
		if (!tree.selectNode(tree.selectedItem))
			tree.selectedItem = oldItem;
}
*/

function setNodeTitle(node, xNode)
{
	var allPathName = xNode.getAttribute("ALL_PATH_NAME");
	node.span.title = node.getText() + ": " + allPathName;
}

function onXmlRequestReady(xmlResult, param)
{
	try
	{
		doAfterOpenRoot(param.tree, xmlResult, param.callBack);

		if (param.syncCallBack)
			param.syncCallBack(param.tree, xmlResult);
	}
	catch(e)
	{
		showError(e);
	}
}

function preloadTreeImages()
{
	var strDir = "/" + C_ACCREDIT_ADMIN_ROOT_URI + "/images/";

	for (var i = 0; i < arguments.length; i++)
	{
		var img = document.createElement("img");
		img.src = strDir + arguments[i];
	}
}

function getRoot(tree, listObjectType, callBack, rootOrg, xmlChildren, extAttr, syncCallBack, argObj)
{
	tree.Nodes.add("", "", "", "正在加载...");
	
//	preloadTreeImages("group.gif",	"ou.gif", "user.gif");

	var xmlResult = null;

	var oSyncCallBack = null;

	if (syncCallBack)
	{
		var param = new Object();

		param.tree = tree;
		param.callBack = callBack;
		param.syncCallBack = syncCallBack;

		oSyncCallBack = onXmlRequestReady;
	}
		

	if (!xmlChildren)
	{
		//设置参数
		var xmlDoc = createCommandXML("getRoot");
		var root = xmlDoc.documentElement;

		setListType(xmlDoc, listObjectType);

		if (rootOrg && typeof(rootOrg) == "object")
		{
			root.setAttribute("rootOrg", rootOrg.AllPathName);
//			root.setAttribute("rootOrgGuid", rootOrg.OrgGuid);
		}
		if (extAttr)
		{
			root.setAttribute("extAttr", extAttr);
			m_extAttr = extAttr;
		}

		setRequestAttributeByArg(xmlDoc.documentElement, argObj);

		xmlResult = xmlSend("/" + C_ACCREDIT_ADMIN_ROOT_URI + "/sysSearch/OGUSearch.aspx", xmlDoc, oSyncCallBack, param);
	}
	else
	{
		//同步访问取得结果
		xmlResult = createDomDocument(xmlChildren);
		
		if (oSyncCallBack)
			oSyncCallBack(xmlResult, param);
	}

	if (!syncCallBack)
		doAfterOpenRoot(tree, xmlResult, callBack);
}

function setRequestAttributeByArg(elem, argObj)
{
	if (argObj && argObj.orgAccessLevel)
		elem.setAttribute("orgAccessRankCN", argObj.orgAccessLevel);

	if (argObj && argObj.userAccessLevel)
		elem.setAttribute("userAccessRankCN", argObj.userAccessLevel);
		
	if (argObj && argObj.hideType)
		elem.setAttribute("hideType", argObj.hideType);
		
	if (argObj && argObj.listObjDelete)
		elem.setAttribute("listObjectDelete", argObj.listObjDelete);
		
	if (argObj && argObj.orgClass)		// 要求展现机构的类型(cgac\yuan_yong 20041030)
		elem.setAttribute("orgClass", argObj.orgClass);
		
	if (argObj && argObj.orgType)		// 要求展现机构的属性(cgac\yuan_yong 20041030)
		elem.setAttribute("orgType", argObj.orgType);
}

//机构人员树机构使用的根树初始化
function doAfterOpenRoot(tree, xmlResult, callBack)
{
	checkErrorResult(xmlResult);

	tree.Nodes.clear();
	var root = xmlResult.documentElement;
	
	var eleRoot = root.firstChild;
	while (eleRoot != null)
	{
		var strName = eleRoot.getAttribute("DISPLAY_NAME");
		var strType = eleRoot.tagName;
		var strImg = getImgFromClass(strType);

		var nRoot = tree.Nodes.add("", "", "", strName, "/" + C_ACCREDIT_ADMIN_ROOT_URI + "/images/" + strImg);

		nRoot.xData.xNode = eleRoot;
		nRoot.xData.waitforLoad = false;

		if (callBack)
			callBack(nRoot, eleRoot);
		
		showChildrenMember(eleRoot, nRoot, callBack);
		
		eleRoot = eleRoot.nextSibling;
	}
}

// 根据情况给每个机构节点下增加一个“回收站”的节点
// 方便实现系统要求的“回收站机制”
function OGUTreeAddTrash(parentNode)
{
	var xmlDoc = createDomDocument("<TRASH />");
	var root = xmlDoc.documentElement;
	for (var i = 0; i < parentNode.attributes.length; i++)
	{
		var strName = parentNode.attributes.item(i).nodeName;
		var strValue = parentNode.attributes.item(i).nodeValue;
		
		switch (strName)
		{
			case "ORIGINAL_SORT":
			case "INNER_SORT":
			case "GLOBAL_SORT":		root.setAttribute(strName, "");
									strName = "TRASH_" + strName;
									break;
			case "ALL_PATH_NAME":	strValue = parentNode.getAttribute("ALL_PATH_NAME") + "\\回收站";
									break;
			case "DISPLAY_NAME":
			case "OBJ_NAME":		strValue = "回收站";
									break;
			default:				break;
		}
		
		root.setAttribute(strName, strValue);
	}
	
	return root;
}

//在树结构中查询具有strColumnName属性上对应值strColumnValue指定的节点
// 可用ALL_PATH_NAME, ORGINIAL_SORT, GLOBAL_SORT
function searchObjInTreeByAttr(tree, strColumnValue, strColumnName, bAutoExpand)
{
	var treeResult = null
	
	if (strColumnValue.length > 0)
	{
		treeResult = tree.Nodes[0];
		
		if (strColumnValue != treeResult.xData.xNode.getAttribute(strColumnName))
		{
			var strParentColumnValue = "";
			switch (strColumnName)
			{
				case "ALL_PATH_NAME":
					strParentColumnValue = strColumnValue.substring(0, strColumnValue.lastIndexOf("\\"));
					break;
				case "ORIGINAL_SORT":
				case "GLOBAL_SORT":
					strParentColumnValue = strColumnValue.substring(0, strColumnValue.length - C_OGU_ORIGINAL_SORT.length);
					break;					
			}
			
			var parentTree = searchObjInTreeByAttr(tree, strParentColumnValue, strColumnName, bAutoExpand);
			
			if (parentTree != null)
				treeResult = parentTree.getChild();
			else
				treeResult = null;
		}
	}
	
	while (treeResult && treeResult.xData && typeof(treeResult.xData.xNode) != "undefined" && treeResult.xData.xNode.getAttribute(strColumnName) != strColumnValue)
		treeResult = treeResult.getNext();
	
	if (treeResult && (treeResult.xData == null || typeof(treeResult.xData.xNode) == "undefined"))
		treeResult = null;
	
	if (treeResult != null && bAutoExpand)
	{
		treeResult.setExpanded(true, false);
		tree.selectedItem = treeResult;
	}
	return treeResult;
}
//-->
