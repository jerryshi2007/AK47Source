<!--
var C_LIST_ORGANIZATION = 1;//列出（或可以选择）机构
var C_LIST_USER = 2;//列出（或可以选择）人员
var C_LIST_GROUP = 4;//列出（或可以选择）人员组
var C_LIST_SIDELINE = 8;//列出（或可以选择）人员兼职对象
var C_LIST_ALL = 65535;//列出（或可以选择）所有对象

var C_LOGIC_COMMON = 1;//正常使用
var C_LOGIC_DIRECT = 2;//直接逻辑删除对象
var C_LOGIC_CONJUNCT_ORG = 4;//因部门导致数据逻辑删除对象
var C_LOGIC_CONJUNCT_USER = 8;//因人员导致数据逻辑删除对象
var C_LOGIC_ALL = 65535;//系统中所有的数据对象

/********************************************************************************/
//功能：刷新机构树结构
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
//功能：对应于Frame的状态改变（目前为空函数）
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
//功能：对应于Save保存按钮操作
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
//功能：获取界面中的所有参数以用于形成相应的URL路径
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
	var strShowTrash = "";	// 展现回收站
	var strOrgClass = "";	// 按照机构类别展示
	var strOrgType = "";	// 按照机构属性展现
	var strShowMyOrg = "";	// 展现当前操作人员所在的机构(cgac\yuan_yong [20041101])
	/**************************************************/
//	var m_extAttr = "";
//	var m_backColor = "";			// 展示界面要求使用的颜色
//	var m_canSelectRoot = false;	// 是否允许选择机构树结构的根节点

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
		if (chkShowMyOrg.checked) strShowMyOrg = "&ShowMyOrg=1";	// 展现当前操作人员所在的机构(cgac\yuan_yong [20041101])
		
		if (selOrgClass.value > 0)	strOrgClass = "&orgClass=" + ((1024*1024-1) - selOrgClass.value);
		if (selOrgType.value > 0)	strOrgType = "&orgType=" + ((1024*1024-1) - selOrgType.value);
	}

	return strListObjType + strSelectObjType + strListObjDel + strPostType + strRootOrg + strMultiSelect 
			+ strTargetFrameName + strAutoExpand + strNodesSelected + "&canSelectRoot=true" 
			+ strMaxLevel + strOrgAccessLevel + strUserAccessLevel + strHideType + strShowButtons 
			+ strSelectSort + strShowTrash + strShowMyOrg + strOrgClass + strOrgType;
}

/********************************************************************************/
//功能：获取机构树展现的URL地址（此时不带参数）
/********************************************************************************/
function getPath()
{
	return "./OGUTree.aspx";
}

/********************************************************************************/
//功能：获取机构树展现的iFrame，其中设置好了机构树的URL地址以及所需要设置的参数
/********************************************************************************/
function getIFrameHTML()
{
	var strHTML = "<iframe id='innerFrame' name='innerFrame' style='BORDER-RIGHT: black 1px solid; PADDING-RIGHT: 0px; BORDER-TOP: black 1px solid; " 
				+ "PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; MARGIN: 0px; BORDER-LEFT: black 1px solid; WIDTH: 100%; PADDING-TOP: 0px; BORDER-BOTTOM: " 
				+ "black 1px solid; HEIGHT: 100%; BACKGROUND-COLOR: transparent' frameborder='no' scrolling='no' src='" + getPath() + "?" + getParam() + "'></iframe>";

	return strHTML;
}

/********************************************************************************/
//功能：从pObj对象中读取attrName对应的属性数据，若读取不到就使用strDefault的指定替代
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
//功能：获取本地保存的配置信息
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
//功能：把配置信息保存在客户端（本地）
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
//功能：界面的初始化
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
//功能：界面的关闭
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
//功能：根据对象监测控件设置根对象
/********************************************************************************/
function changeSourceDN()
{
	if (OGUInput.value.length > 0)
	{
		frmInput.rootOrg.value = OGUInput.value;
	}
}
//-->