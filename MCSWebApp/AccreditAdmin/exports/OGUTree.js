<!--
/********************************************************************************/
//���ܣ��ڶ�ѡ����°�ѡ����򱣴�ѡ����������б�ѡ�����
/********************************************************************************/
var m_SelectSortXml = createDomDocument("<NodesSelected/>"); 

/********************************************************************************/
//���ܣ����ݶ�Ӧ��Form�ύ
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
//���ܣ�Form�ύ����
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
// ���ܣ���Ӧ���ݶ�������������Ӧ�Ķ�������
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
//���ܣ����γɻ������Ĺ����а������е�Ҫ���Զ�չ���ڵ����ú�
/********************************************************************************/
function syncCallBack(tv, xmlResult)
{
	setAutoExpand(tv, m_nodesSelectedXml, false);//forever changed 2004-10-15
	setAutoExpand(tv, m_autoExpand);
	if ((m_selectSort & 1) != 0)
		m_SelectSortXml = createDomDocument(getAllSelectedNode());
}

/********************************************************************************/
//���ܣ����ṹ�����������еĽڵ㴴���ص���Ӧ����
/********************************************************************************/
function nodeCreateCallBack(nMb, xmlNode)
{
	if ((m_multiSelect & 1) != 0)//��ѡ
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
//���ܣ������ѡ����µĸ�ѡ��ѡ�л�ȡ���Ļص�����
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
//���ܣ���ȡ�������е���һ���ڵ㣨������һ���ڵ�ͻ�ȡ���ڵ����һ���ڵ㣩
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
//���ܣ����������еĽڵ�node��Ӧ������ӵ�root�����ݣ��ڵ���ȥ
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
//���ܣ�����ѡ������ϡ����ջ������򡱷��ؽ��
/********************************************************************************/
function getAllSelectedNode(nodeSelect, bSelectParentOnly)
{
	var xmlDoc = createDomDocument("<NodesSelected/>");
	var root = xmlDoc.documentElement;
	if ((m_multiSelect & 1) != 0)//��ѡ
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
//���ܣ���Ӧ��ѡ������ϵġ�����ѡ����򡱷��ؽ��
/********************************************************************************/
function getAllSelectedNodeByUserSort()
{
	return m_SelectSortXml.xml;
}

/********************************************************************************/
//���ܣ���Ӧ�ڻ������ϲ��������˫���Ĳ���
/********************************************************************************/
function tvNodeDoubleClick()
{
	var node = event.node;

	event.returnValue = (node.children > 0);
}

/********************************************************************************/
//���ܣ���Ӧ�ڻ������Ľڵ㱻ѡ�в���
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
//���ܣ���Ӧ�ڻ�������չ�����������߹رգ�
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
			argObj.orgClass = m_orgClass;		// Ҫ��չ�ֻ���������(cgac\yuan_yong 20041030)
			argObj.orgType = m_orgType;			// Ҫ��չ�ֻ���������(cgac\yuan_yong 20041030)
		
			loadChildren(n, m_listObjType, nodeCreateCallBack, bASync, argObj);
		}
	}
	catch(e)
	{
		showError(e);
	}
}

/********************************************************************************/
//���ܣ��������ó�ʼ�������Լ�Form��ʽ
//		ȷ���Ƿ�չ�ֽ����еİ�ťButton
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
//���ܣ���ʼ�������Լ�����
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
		argObj.orgClass = m_orgClass;		// Ҫ��չ�ֻ���������(cgac\yuan_yong 20041030)
		argObj.orgType = m_orgType;			// Ҫ��չ�ֻ���������(cgac\yuan_yong 20041030)

		getRoot(tv, m_listObjType, nodeCreateCallBack, rootObj, strXml, m_extAttr, syncCallBack, argObj);
	}
	catch(e)
	{
		showError(e);
	}
}

var m_maxLevel = 99999;
var m_backColor = "";			// չʾ����Ҫ��ʹ�õ���ɫ

var m_submitType = "";
var m_submitData = "";

var m_showButtons = 0;			// �Ƿ�չ�ְ�ť
var m_multiSelect = 0;			// �Ƿ������ѡ
var m_listObjType = 65535;		// Ҫ���г��Ķ������ͣ���������Ա�顢��Ա��
var m_listObjDelete = 65535;	// Ҫ���г��Ķ��������ɾ�����ݣ�����ʹ�á�ֱ���߼�ɾ��������߼�ɾ����
var m_selectObjType = 65535;	// �г������еĿ�ѡ���󣨻�������Ա�顢��Ա��
var m_rootOrg = "";				// Ҫ��չ�ֵĻ�����ʹ�õġ�����������Ĭ��Ϊ�գ�
var m_autoExpand = "";			// ��������Ҫ���Զ�չ���Ķ���������Զ�չ����
var m_extAttr = "";				// Ҫ�󷵻ص���������
var m_orgAccessLevel = "";		// ����Organization�ļ������
var m_userAccessLevel = "";		// ����User�ļ������
var m_hideType = "";			// Ҫ������������������Ϣ�е�ָ�������ϵ�������Ϣ������
var m_selectSort = 0;			// ѡ���Ƿ�Ҫ���¼����
var m_canSelectRoot = false;	// �Ƿ�����ѡ��������ṹ�ĸ��ڵ�
var m_strNodesSelected = "";	// Ĭ��ѡ���Ķ��󣨲��ñ�Ǽ�¼��
var m_ShowTrash = 0;			// �Ƿ�չ�ֻ������еĻ���վ
var m_nodesSelectedXml = null;
var m_orgClass = "";			// Ҫ��չ�ֻ���������(cgac\yuan_yong 20041030)
var m_orgType = "";				// Ҫ��չ�ֻ���������(cgac\yuan_yong 20041030)
//var m_ShowMyOrg = 0;			// �Ƿ�չ�ֵ�ǰ������Ա���ڵĻ���(cgac\yuanyong [20041101])

/********************************************************************************/
//���ܣ���ʼ������
/********************************************************************************/
function InitPage()
{

	m_maxLevel = LMaxLevel.value;
	m_backColor = LBackColor.value;					// չʾ����Ҫ��ʹ�õ���ɫ
	
	m_submitType = LSumitType.value;
	m_submitData = LSumitData.value;
	
	m_showButtons = parseInt(LShowButtons.value);	// �Ƿ�չ�ְ�ť
	m_multiSelect = parseInt(LMultiSelect.value);	// �Ƿ������ѡ
	m_listObjType = parseInt(LListObjType.value);	// Ҫ���г��Ķ������ͣ���������Ա�顢��Ա��
	m_listObjDelete = parseInt(LListObjDelete.value);// Ҫ���г��Ķ��������ɾ�����ݣ�����ʹ�á�ֱ���߼�ɾ��������߼�ɾ����
	m_selectObjType = parseInt(LSelectObjType.value);// �г������еĿ�ѡ���󣨻�������Ա�顢��Ա��
	m_rootOrg = LRootOrg.value;						// Ҫ��չ�ֵĻ�����ʹ�õġ�����������Ĭ��Ϊ�գ�
	m_autoExpand = LAutoExpand.value;				// ��������Ҫ���Զ�չ���Ķ���������Զ�չ����
	m_extAttr = LExtAttr.value;						// Ҫ�󷵻ص���������
	m_orgAccessLevel = LOrgAccessLevel.value;		// ����Organization�ļ������
	m_userAccessLevel = LUserAccessLevel.value;		// ����User�ļ������
	m_hideType = LHideType.value;					// Ҫ������������������Ϣ�е�ָ�������ϵ�������Ϣ������
	m_selectSort = parseInt(LSelectSort.value);		// ѡ���Ƿ�Ҫ���¼����
	m_canSelectRoot = LCanSelectRoot.value.toLowerCase() == "true";			// �Ƿ�����ѡ��������ṹ�ĸ��ڵ�
	m_strNodesSelected = LNodesSelected.value;		// Ĭ��ѡ���Ķ��󣨲��ñ�Ǽ�¼��
	m_ShowTrash = parseInt(LShowTrash.value);		// �Ƿ�չ�ֻ������еĻ���վ
//	m_ShowMyOrg = parseInt(LShowMyOrg.value);		// �Ƿ�չ�ֵ�ǰ������Ա���ڵĻ���(cgac\yuanyong [20041101])
	m_orgClass = LOrgClass.value;					// Ҫ��չ�ֻ���������(cgac\yuan_yong 20041030)
	m_orgType = LOrgType.value;						// Ҫ��չ�ֻ���������(cgac\yuan_yong 20041030)
	
	treeForm.target = LTarget.value;
	
	m_nodesSelectedXml = null;
	if (m_strNodesSelected.length > 0)
		m_nodesSelectedXml = createDomDocument(m_strNodesSelected);
	
}
//-->