<!--
/********************************************************************************/
//功能：在多选情况下按选择次序保存选择过程中所有被选择对象
/********************************************************************************/
var m_SelectSortXml = createDomDocument("<NodesSelected/>"); 

/********************************************************************************/
//功能：数据对应的Form提交
/********************************************************************************/
function onSubmitClick(bSubmitData, nodeSelect)
{
	try
	{
		treeForm.action = m_submitData;

		if (bSubmitData)
		{
			if ((m_selectSort & 1) == 0)
				treeForm.data.value = getAllSelectedNode(nodeSelect);
			else
				treeForm.data.value = getAllSelectedNodeByUserSort();
		}
		else
			treeForm.data.value = "";

		treeForm.submit();
	}
	catch (e)
	{
		showError(e);
	}
}

/********************************************************************************/
//功能：Form提交数据
/********************************************************************************/
function submitData(xNode, node)
{
	if (m_submitType == "topControl")
	{
		var submitData = window.parent.document.all(m_submitData);

		if (submitData)
		{
			submitData.value = xNode.xml;
		}
	}
	else
	{
		if ((m_multiSelect & 2) != 0)
		{
			onSubmitClick(true, node);
		}
	}
}

/********************************************************************************/
// 功能：对应数据对象类型设置相应的对象掩码
/********************************************************************************/
function objectClassToMask(strClass)
{
	var nIndexMask = 0;

	switch (strClass)
	{
		case "USERS":	nIndexMask = C_LIST_USER;
						break;
		case "ORGANIZATIONS" : nIndexMask = C_LIST_ORGANIZATION;
						break;
		case "GROUPS":	nIndexMask = C_LIST_GROUP;
						break;
		case "ROLES":	nIndexMask = C_LIST_ROLE;
						break;
	}

	return nIndexMask;
}

/********************************************************************************/
//功能：在形成机构树的过程中把设置中的要求自动展开节点设置好
/********************************************************************************/
function syncCallBack(tv, xmlResult)
{
	setAutoExpand(tv, m_nodesSelectedXml, false);//forever changed 2004-10-15
	setAutoExpand(tv, m_autoExpand);
	if ((m_selectSort & 1) != 0)
		m_SelectSortXml = createDomDocument(getAllSelectedNode());
}

/********************************************************************************/
//功能：树结构被创建过程中的节点创建回调响应函数
/********************************************************************************/
function nodeCreateCallBack(nMb, xmlNode)
{
	if ((m_multiSelect & 1) != 0)//多选
	{
		var bCheckBox = true;
		
		var strClass = xmlNode.nodeName;
		var nIndexMask = objectClassToMask(strClass);

		if (m_selectObjType & nIndexMask)
		{
			nMb.setCheckbox(true);
			if (m_selectSort & 1 != 0)
				nMb.checkbox.onclick = checkboxClickCallBack;
		}
		else
			nMb.setCheckbox(false);
	}

	if (nMb.level >= m_maxLevel)
		nMb.removeChildren();
	
	if (m_nodesSelectedXml && (m_multiSelect & 1) != 0)
	{
		var strAllPathName = xmlNode.getAttribute("ALL_PATH_NAME");
		if (strAllPathName && strAllPathName.length > 0)
		{
			var re = /\\/g;
			strAllPathName = strAllPathName.replace(re, "\\\\");
			var root = m_nodesSelectedXml.documentElement;
			var node = root.selectSingleNode("object[@ALL_PATH_NAME=\"" + strAllPathName + "\"]");
			if (node)
				nMb.checkbox.checked = true;
		}
	}
}

/********************************************************************************/
//功能：允许多选情况下的复选框被选中或被取消的回调操作
/********************************************************************************/
function checkboxClickCallBack() 
{
	var src = event.srcElement;
	while (src.node == null)
	{
		src = src.parentElement;
	}
	var srcNode = src.node;

	var root = m_SelectSortXml.documentElement;
	
	var xml = srcNode.xData.xNode;
	var strAllPathName = xml.getAttribute("ALL_PATH_NAME");
	var re = /\\/g;
	strAllPathName = strAllPathName.replace(re, "\\\\");
	var objNode = root.selectSingleNode("object[@ALL_PATH_NAME=\"" + strAllPathName + "\"]");
	
	if (srcNode.getChecked())
	{
		if (objNode == null)
			addSelectedNode(root, srcNode);
	}
	else
	{
		if (objNode != null)
			root.removeChild(objNode);
	}
}

/********************************************************************************/
//功能：获取机构树中的下一个节点（若无下一个节点就获取父节点的下一个节点）
/********************************************************************************/
function getNextNode(node) 
{ 
    if (node.next) 
    {
		node = node.next;
	}
	else
	{
        if (node.parent) 
			node = node.parent.next; 
        else
			node = null;
	}
    
    return node; 
} 

/********************************************************************************/
//功能：将机构树中的节点node对应数据添加到root（数据）节点中去
/********************************************************************************/
function addSelectedNode(root, node)
{
	var xmlNode = node.xData.xNode;

	var newChild = appendNode(root, "object");

	for (var i = 0; i < xmlNode.attributes.length; i++)
	{
		newChild.setAttribute(xmlNode.attributes[i].nodeName, xmlNode.attributes[i].nodeValue);
	}
}

/********************************************************************************/
//功能：对于选择操作上“按照机构次序”返回结果
/********************************************************************************/
function getAllSelectedNode(nodeSelect, bSelectParentOnly)
{
	var xmlDoc = createDomDocument("<NodesSelected/>");
	var root = xmlDoc.documentElement;
	if ((m_multiSelect & 1) != 0)//多选
	{
		if (bSelectParentOnly)
		{
			var node = tv.Nodes[0];
			while (node)
			{
				if (node.getChecked())
				{
					addSelectedNode(root, node);
					node = getNextNode(node); 
				}
				else
				{
					if (node.firstChild)
						node = node.firstChild;
					else
						node = getNextNode(node); 
				}					
			}
		}
		else
		{
			for (var i = 0; i < tv.Nodes.length; i++)
			{
				var node = tv.Nodes[i];
				if (node.getChecked())
					addSelectedNode(root, node);
			}
		}
	}
	else
	{
		var node = null;
		if (nodeSelect)
			node = nodeSelect;
		else
			node = tv.selectedItem;
		if (node)
			addSelectedNode(root, node);
	}
	if (root)
		return xmlDoc.xml;
	else
		return "";
}

/********************************************************************************/
//功能：对应于选择操作上的“按照选择次序”返回结果
/********************************************************************************/
function getAllSelectedNodeByUserSort()
{
	return m_SelectSortXml.xml;
}

/********************************************************************************/
//功能：对应于机构树上采用了鼠标双击的操作
/********************************************************************************/
function tvNodeDoubleClick()
{
	var node = event.node;

	event.returnValue = (node.children > 0);
}

/********************************************************************************/
//功能：对应于机构树的节点被选中操作
/********************************************************************************/
function tvNodeSelected()
{
	try
	{
		var xNode = event.node.xData.xNode;
		var bCanSubmit = false;

		if (xNode)
		{
			var strClass = xNode.nodeName;
			var nIndexMask = objectClassToMask(strClass);

			if (strClass == "ORGANIZATIONS" || strClass == "USERS"  || m_canSelectRoot)
			{
				bCanSubmit = ((m_selectObjType & nIndexMask) != 0);

				if (bCanSubmit)
					submitData(xNode, event.node);
			}
		}

		event.returnValue = bCanSubmit;
	}
	catch(e)
	{
		showError(e);
	}
}

/********************************************************************************/
//功能：对应于机构树的展开操作（或者关闭）
/********************************************************************************/
function tvNodeExpand()
{
	try
	{
		var n = event.node;
		var bASync = event.fromClick;

		if (n.xData.waitforLoad)
		{
			var argObj = new Object();
			argObj.orgAccessLevel = m_orgAccessLevel;
			argObj.userAccessLevel = m_userAccessLevel;
			argObj.hideType = m_hideType;
			argObj.listObjDelete = m_listObjDelete;
			argObj.orgClass = m_orgClass;		// 要求展现机构的类型(cgac\yuan_yong 20041030)
			argObj.orgType = m_orgType;			// 要求展现机构的属性(cgac\yuan_yong 20041030)
		
			loadChildren(n, m_listObjType, nodeCreateCallBack, bASync, argObj);
		}
	}
	catch(e)
	{
		showError(e);
	}
}

/********************************************************************************/
//功能：根据设置初始化界面以及Form方式
//		确定是否展现界面中的按钮Button
/********************************************************************************/
function setInterfaceByType(strType)
{
	if (m_backColor.length > 0)
		document.body.style.backgroundColor = m_backColor;

	if (strType == "topControl")
	{
		buttonLine.style.display = "none";
	}
	else
	{
		if (strType == "post" && (m_showButtons))
		{
			buttonLine.style.display = "inline";
		}
	}
}

/********************************************************************************/
//功能：初始化界面以及数据
/********************************************************************************/
function onDocumentLoad()
{
	try
	{
		InitPage();
		
		setInterfaceByType(m_submitType);
		
		document.getAllSelectedNodeByUserSort = getAllSelectedNodeByUserSort;
		document.getAllSelectedNode = getAllSelectedNode;

		var rootObj = null;

		if (m_rootOrg.length > 0)
		{
			rootObj = new Object();
			rootObj.AllPathName = m_rootOrg;
		}
		
		if (parent.m_firstChildren)
			Container.TBSubChildData.value = parent.m_firstChildren;

		var strXml = Container.TBSubChildData.value;

		if (strXml.length == 0)
			strXml = null;
			
		if (m_nodesSelectedXml)
			m_SelectSortXml = m_nodesSelectedXml;

		var argObj = new Object();
		argObj.orgAccessLevel = m_orgAccessLevel;
		argObj.userAccessLevel = m_userAccessLevel;
		argObj.hideType = m_hideType;
		argObj.listObjDelete = m_listObjDelete;
		argObj.orgClass = m_orgClass;		// 要求展现机构的类型(cgac\yuan_yong 20041030)
		argObj.orgType = m_orgType;			// 要求展现机构的属性(cgac\yuan_yong 20041030)

		getRoot(tv, m_listObjType, nodeCreateCallBack, rootObj, strXml, m_extAttr, syncCallBack, argObj);
	}
	catch(e)
	{
		showError(e);
	}
}

var m_maxLevel = 99999;
var m_backColor = "";			// 展示界面要求使用的颜色

var m_submitType = "";
var m_submitData = "";

var m_showButtons = 0;			// 是否展现按钮
var m_multiSelect = 0;			// 是否允许多选
var m_listObjType = 65535;		// 要求列出的对象类型（机构、人员组、人员）
var m_listObjDelete = 65535;	// 要求列出的对象包括被删除内容（正常使用、直接逻辑删除、间接逻辑删除）
var m_selectObjType = 65535;	// 列出对象中的可选对象（机构、人员组、人员）
var m_rootOrg = "";				// 要求展现的机构树使用的“根机构”（默认为空）
var m_autoExpand = "";			// 机构树中要求自动展开的对象（树结点自动展开）
var m_extAttr = "";				// 要求返回的属性数据
var m_orgAccessLevel = "";		// 用于Organization的级别控制
var m_userAccessLevel = "";		// 用于User的级别控制
var m_hideType = "";			// 要求机构树中针对配置信息中的指定配置上的数据信息被屏蔽
var m_selectSort = 0;			// 选择是否要求记录次序
var m_canSelectRoot = false;	// 是否允许选择机构树结构的根节点
var m_strNodesSelected = "";	// 默认选定的对象（采用标记记录）
var m_ShowTrash = 0;			// 是否展现机构树中的回收站
var m_nodesSelectedXml = null;
var m_orgClass = "";			// 要求展现机构的类型(cgac\yuan_yong 20041030)
var m_orgType = "";				// 要求展现机构的属性(cgac\yuan_yong 20041030)
//var m_ShowMyOrg = 0;			// 是否展现当前操作人员所在的机构(cgac\yuanyong [20041101])

/********************************************************************************/
//功能：初始化数据
/********************************************************************************/
function InitPage()
{

	m_maxLevel = LMaxLevel.value;
	m_backColor = LBackColor.value;					// 展示界面要求使用的颜色
	
	m_submitType = LSumitType.value;
	m_submitData = LSumitData.value;
	
	m_showButtons = parseInt(LShowButtons.value);	// 是否展现按钮
	m_multiSelect = parseInt(LMultiSelect.value);	// 是否允许多选
	m_listObjType = parseInt(LListObjType.value);	// 要求列出的对象类型（机构、人员组、人员）
	m_listObjDelete = parseInt(LListObjDelete.value);// 要求列出的对象包括被删除内容（正常使用、直接逻辑删除、间接逻辑删除）
	m_selectObjType = parseInt(LSelectObjType.value);// 列出对象中的可选对象（机构、人员组、人员）
	m_rootOrg = LRootOrg.value;						// 要求展现的机构树使用的“根机构”（默认为空）
	m_autoExpand = LAutoExpand.value;				// 机构树中要求自动展开的对象（树结点自动展开）
	m_extAttr = LExtAttr.value;						// 要求返回的属性数据
	m_orgAccessLevel = LOrgAccessLevel.value;		// 用于Organization的级别控制
	m_userAccessLevel = LUserAccessLevel.value;		// 用于User的级别控制
	m_hideType = LHideType.value;					// 要求机构树中针对配置信息中的指定配置上的数据信息被屏蔽
	m_selectSort = parseInt(LSelectSort.value);		// 选择是否要求记录次序
	m_canSelectRoot = LCanSelectRoot.value.toLowerCase() == "true";			// 是否允许选择机构树结构的根节点
	m_strNodesSelected = LNodesSelected.value;		// 默认选定的对象（采用标记记录）
	m_ShowTrash = parseInt(LShowTrash.value);		// 是否展现机构树中的回收站
//	m_ShowMyOrg = parseInt(LShowMyOrg.value);		// 是否展现当前操作人员所在的机构(cgac\yuanyong [20041101])
	m_orgClass = LOrgClass.value;					// 要求展现机构的类型(cgac\yuan_yong 20041030)
	m_orgType = LOrgType.value;						// 要求展现机构的属性(cgac\yuan_yong 20041030)
	
	treeForm.target = LTarget.value;
	
	m_nodesSelectedXml = null;
	if (m_strNodesSelected.length > 0)
		m_nodesSelectedXml = createDomDocument(m_strNodesSelected);
	
}
//-->