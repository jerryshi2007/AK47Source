<!--
var C_LIST_ORGANIZATION = 1;//�г��������ѡ�񣩻���
var C_LIST_USER = 2;//�г��������ѡ����Ա
var C_LIST_GROUP = 4;//�г��������ѡ����Ա��
var C_LIST_SIDELINE = 8;//�г��������ѡ����Ա��ְ����
var C_LIST_ALL = 65535;//�г��������ѡ�����ж���

var C_LOGIC_COMMON = 1;//����ʹ��
var C_LOGIC_DIRECT = 2;//ֱ���߼�ɾ������
var C_LOGIC_CONJUNCT_ORG = 4;//���ŵ��������߼�ɾ������
var C_LOGIC_CONJUNCT_USER = 8;//����Ա���������߼�ɾ������
var C_LOGIC_ALL = 65535;//ϵͳ�����е����ݶ���

/********************************************************************************/
//���ܣ�ˢ�»������ṹ
/********************************************************************************/
function onRefreshTree()
{
	try
	{
		var strXML = frmInput.xmlArea.innerText;
		var strResultParam = "";
		
		if (strXML.length == 0)
		{
			strResultParam = getIFrameHTML();
			frmInput.result.value = strResultParam;
			frameContainer.innerHTML = strResultParam;
		}
		else
		{
			strResultParam = getPath() + "?" + getParam();
			frmXml.firstChildren.value = strXML;
			frmXml.action = strResultParam;
			frmInput.result.value = strResultParam;
			frmXml.submit();
		}
	}
	catch (e)
	{
		showError(e);
	}
}

/********************************************************************************/
//���ܣ���Ӧ��Frame��״̬�ı䣨ĿǰΪ�պ�����
/********************************************************************************/
function onFrameStateChange()
{
	try
	{
		//if (event.srcElement.readyState == "complete")
		//	frmInput.btnOK.disabled = false;
	}
	catch (e)
	{
		showError(e);
	}
}

/********************************************************************************/
//���ܣ���Ӧ��Save���水ť����
/********************************************************************************/
function onSaveClick()
{
	try
	{
		window.returnValue = iFrameDocument("innerFrame").getAllSelectedNode();
		window.close();
	}
	catch (e)
	{
		showError(e);
	}
}

/********************************************************************************/
//���ܣ���ȡ�����е����в����������γ���Ӧ��URL·��
/********************************************************************************/
function getParam()
{
	var nListObjType = 0;
	var strListObjType = "";
	var nSelectObjType = 0;
	var strSelectObjType = "";
	var nMultiSelect = 0;
	var strMultiSelect = "";
	var nListObjDel = C_LOGIC_COMMON;
	var strListObjDel = "";
	var strPostType = "";
	var strRootOrg = "";
	var strTargetFrameName = "";
	var strAutoExpand = "";
	var strNodesSelected = "";
	var strMaxLevel = "";
	var strOrgAccessLevel = "";
	var strUserAccessLevel = "";
	var strHideType = "";
	var strShowButtons = "";
	var strSelectSort = "";
	var strShowTrash = "";	// չ�ֻ���վ
	var strOrgClass = "";	// ���ջ������չʾ
	var strOrgType = "";	// ���ջ�������չ��
	var strShowMyOrg = "";	// չ�ֵ�ǰ������Ա���ڵĻ���(cgac\yuan_yong [20041101])
	/**************************************************/
//	var m_extAttr = "";
//	var m_backColor = "";			// չʾ����Ҫ��ʹ�õ���ɫ
//	var m_canSelectRoot = false;	// �Ƿ�����ѡ��������ṹ�ĸ��ڵ�

	with(frmInput)
	{
		if (chkOU.checked)		nListObjType |= C_LIST_ORGANIZATION;
		if (chkUser.checked)	nListObjType |= C_LIST_USER;
		if (chkGroup.checked)	nListObjType |= C_LIST_GROUP;
		if (chkSideline.checked)nListObjType |= C_LIST_SIDELINE;
		strListObjType = "listObjType=" + nListObjType;

		if (chkSelectOU.checked)	nSelectObjType |= C_LIST_ORGANIZATION;
		if (chkSelectUser.checked)	nSelectObjType |= C_LIST_USER;
		if (chkSelectGroup.checked)	nSelectObjType |= C_LIST_GROUP;
		strSelectObjType = "&selectObjType=" + nSelectObjType;
		
		if (chkDirDelete.checked)	nListObjDel |= C_LOGIC_DIRECT;
		if (chkOrgDelete.checked)	nListObjDel |= C_LOGIC_CONJUNCT_ORG;
		if (chkUserDelete.checked)	nListObjDel |= C_LOGIC_CONJUNCT_USER;
		strListObjDel = "&listObjDelete=" + nListObjDel;
		
		if (postData.value.length > 0)	strPostType = "&postURL=" + postData.value;
		else	strPostType = "&topControl=dnSelected";

		if (rootOrg.value.length > 0)	strRootOrg = "&rootOrg=" + rootOrg.value;

		if (chkMultiSelect.checked)	nMultiSelect = 1;
		if (targetFrame.value.length > 0)
		{
			nMultiSelect |= 2;
			strTargetFrameName = "&target=" + targetFrame.value;
		}
		if (nMultiSelect)	strMultiSelect = "&multiSelect=" + nMultiSelect;

		if (autoExpand.innerText.length > 0)	strAutoExpand = "&autoExpand=" + autoExpand.innerText;
		if (nodesSelected.innerText.length > 0)	strNodesSelected = "&nodesSelected=" + nodesSelected.innerText;

		var nMaxLevel = parseInt(maxLevel.value);
		if (!isNaN(nMaxLevel))	strMaxLevel = "&maxLevel=" + nMaxLevel;

		if (orgSelectAccessLevel.value.length > 0)	strOrgAccessLevel = "&orgAccessLevel=" + orgSelectAccessLevel.value;
		if (userSelectAccessLevel.value.length > 0)	strUserAccessLevel = "&userAccessLevel=" + userSelectAccessLevel.value;
		//if (chkHideType.checked)	strHideType = "&hideType=addrBookMask";
		if (selHideType.value.length > 0) strHideType = "&hideType=" + selHideType.value;
		if (chkHideButton.checked)	strShowButtons = "&showButtons=1"
		if (chkSelectSort.checked)	strSelectSort = "&selectSort=1";
		if (chkShowTrash.checked) strShowTrash = "&showTrash=1";
		if (chkShowMyOrg.checked) strShowMyOrg = "&ShowMyOrg=1";	// չ�ֵ�ǰ������Ա���ڵĻ���(cgac\yuan_yong [20041101])
		
		if (selOrgClass.value > 0)	strOrgClass = "&orgClass=" + ((1024*1024-1) - selOrgClass.value);
		if (selOrgType.value > 0)	strOrgType = "&orgType=" + ((1024*1024-1) - selOrgType.value);
	}

	return strListObjType + strSelectObjType + strListObjDel + strPostType + strRootOrg + strMultiSelect 
			+ strTargetFrameName + strAutoExpand + strNodesSelected + "&canSelectRoot=true" 
			+ strMaxLevel + strOrgAccessLevel + strUserAccessLevel + strHideType + strShowButtons 
			+ strSelectSort + strShowTrash + strShowMyOrg + strOrgClass + strOrgType;
}

/********************************************************************************/
//���ܣ���ȡ������չ�ֵ�URL��ַ����ʱ����������
/********************************************************************************/
function getPath()
{
	return "./OGUTree.aspx";
}

/********************************************************************************/
//���ܣ���ȡ������չ�ֵ�iFrame���������ú��˻�������URL��ַ�Լ�����Ҫ���õĲ���
/********************************************************************************/
function getIFrameHTML()
{
	var strHTML = "<iframe id='innerFrame' name='innerFrame' style='BORDER-RIGHT: black 1px solid; PADDING-RIGHT: 0px; BORDER-TOP: black 1px solid; " 
				+ "PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; MARGIN: 0px; BORDER-LEFT: black 1px solid; WIDTH: 100%; PADDING-TOP: 0px; BORDER-BOTTOM: " 
				+ "black 1px solid; HEIGHT: 100%; BACKGROUND-COLOR: transparent' frameborder='no' scrolling='no' src='" + getPath() + "?" + getParam() + "'></iframe>";

	return strHTML;
}

/********************************************************************************/
//���ܣ���pObj�����ж�ȡattrName��Ӧ���������ݣ�����ȡ������ʹ��strDefault��ָ�����
/********************************************************************************/
function readAttribute(pObj, attrName, strDefault)
{
	var attr = pObj.getAttribute(attrName);

	if (attr)
		return attr;
	else
	{
		if (strDefault)
			return strDefault;
	}
	
	return "";
}

/********************************************************************************/
//���ܣ���ȡ���ر����������Ϣ
/********************************************************************************/
function loadPersistData()
{
	persistObj.load("oXMLBranch");

	with(frmInput)
	{
		postData.value = readAttribute(persistObj, "postData", postData.value);
		rootOrg.value = readAttribute(persistObj, "rootOrg", rootOrg.value);
		targetFrame.value = readAttribute(persistObj, "targetFrame", targetFrame.value);
		xmlArea.innerText = readAttribute(persistObj, "xmlArea", xmlArea.innerText);
		autoExpand.innerText = readAttribute(persistObj, "autoExpand", autoExpand.innerText);
		maxLevel.value = readAttribute(persistObj, "maxLevel", maxLevel.value);
		orgSelectAccessLevel.value = readAttribute(persistObj, "orgAccessLevel", orgSelectAccessLevel.value);
		userSelectAccessLevel.value = readAttribute(persistObj, "userAccessLevel", userSelectAccessLevel.value);
	}
}

/********************************************************************************/
//���ܣ���������Ϣ�����ڿͻ��ˣ����أ�
/********************************************************************************/
function savePersistData()
{
	with(frmInput)
	{
		persistObj.setAttribute("postData", postData.value);
		persistObj.setAttribute("rootOrg", rootOrg.value);
		persistObj.setAttribute("targetFrame", targetFrame.value);
		persistObj.setAttribute("xmlArea", xmlArea.innerText);
		persistObj.setAttribute("autoExpand", autoExpand.innerText);
		persistObj.setAttribute("maxLevel", maxLevel.value);
		persistObj.setAttribute("orgAccessLevel", orgSelectAccessLevel.value);
		persistObj.setAttribute("userAccessLevel", userSelectAccessLevel.value);
	}
	persistObj.save("oXMLBranch");
}

/********************************************************************************/
//���ܣ�����ĳ�ʼ��
/********************************************************************************/
function onDocumentLoad()
{
//	setSelectValuesByXML(OU_LEVEL_CODE, frmInput.orgSelectAccessLevel, "LEVEL_ID", "LEVEL_NAME", true);
//	setSelectValuesByXML(LEVEL_CODE, frmInput.userSelectAccessLevel, "LEVEL_ID", "LEVEL_NAME", true);
	
	try
	{	
		if (frmInput.rootOrg.value.length > 0)
			OGUInput.value = frmInput.rootOrg.value;
		
		loadPersistData();
		setHideType();
		onRefreshTree();
		window.returnValue = "";
	}
	catch (e)
	{
		showError(e);
	}
}

function setHideType()
{
	with (frmInput)
	{
		var newOpt = document.createElement("OPTION");
		newOpt.value = "";
		newOpt.text = "----";
		selHideType.options.add(newOpt);

		var root = ignoreObjs.XMLDocument.documentElement;
		var elem = root.firstChild;
		while (elem)
		{
			newOpt = document.createElement("OPTION");
			newOpt.value = elem.getAttribute("name");
			newOpt.text = elem.getAttribute("name");
			selHideType.options.add(newOpt);
			elem = elem.nextSibling;
		}
	}
		
	root = xmlOrgAttr.XMLDocument.documentElement;
	classRoot = root.firstChild;
	with (frmInput)
	{
		newOpt = document.createElement("OPTION");
		newOpt.value = "";
		newOpt.text = "----";
		selOrgClass.options.add(newOpt);
		
		elem = classRoot.firstChild;
		while (elem)
		{
			newOpt = document.createElement("OPTION");
			newOpt.value = elem.getAttribute("ID");
			newOpt.text = elem.getAttribute("NAME");
			selOrgClass.options.add(newOpt);
			elem = elem.nextSibling;
		}
	}
	
	typeRoot = classRoot.nextSibling;
	with (frmInput)
	{
		newOpt = document.createElement("OPTION");
		newOpt.value = "";
		newOpt.text = "----";
		selOrgType.options.add(newOpt);
		
		elem = typeRoot.firstChild;
		while (elem)
		{
			newOpt = document.createElement("OPTION");
			newOpt.value = elem.getAttribute("ID");
			newOpt.text = elem.getAttribute("NAME");
			selOrgType.options.add(newOpt);
			elem = elem.nextSibling;
		}
	}
}

/********************************************************************************/
//���ܣ�����Ĺر�
/********************************************************************************/
function onDocumentUnload()
{
	try
	{
		savePersistData();
	}
	catch (e)
	{
		showError(e);
	}
}

/********************************************************************************/
//���ܣ����ݶ�����ؼ����ø�����
/********************************************************************************/
function changeSourceDN()
{
	if (OGUInput.value.length > 0)
	{
		frmInput.rootOrg.value = OGUInput.value;
	}
}
//-->