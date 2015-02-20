<!--
var m_interfaceParam = null;

var m_bFrameLoaded = false;
var m_xmlCondition = null;

var m_txtSelection = null;

var m_showOGUTree = true;

var m_MulitServer = false;////跨WebServer的调用，在跨Server调用的时候采用Clipboard作为数据传输

/********************************************************************************/
//功能：菜单展列的数据预处理
/********************************************************************************/
function listMenuBeforePopup()
{
	try
	{
	
	}
	catch (e)
	{
		showError(e);
	}
}

/********************************************************************************/
//功能：对应菜单中的菜单项选中操作
/********************************************************************************/
function listMenuClick()
{
	try
	{
		if (m_txtSelection)
			m_txtSelection.select();

		switch (event.menuData)
		{
			case "logicDelete":logicDeleteObjectFromSelection(ouUserListBody);
							break;
			case "delete":	deleteObjectFromSelection(ouUserListBody);
							break;
			case "move":	if (listMenu.row)
							{
								var xmlDoc = createDomDocument(listMenu.row.xml);
								
								var strSrcOrgGuid = xmlDoc.documentElement.getAttribute("PARENT_GUID");
								moveObjectFromSelection(ouUserListBody, strSrcOrgGuid);
							}
							break;
			case "property":
							if (listMenu.row)
								onNameClick(listMenu.row);
							break;
		}
	}
	catch(e)
	{
		showError(e);
	}
}

/********************************************************************************/
//功能：菜单展现
/********************************************************************************/
function oBodyContextMenu()
{
	try
	{
		var oTR = getOwnerTR(event.srcElement);

		listMenu.row = oTR;
		m_txtSelection = setTDSelected(oTR);

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

/********************************************************************************/
//功能：Frame状态改变（对应于状态改变为complete）
/********************************************************************************/
function onFrameStateChange()
{
	try
	{
		if (event.srcElement.readyState == "complete")
		{
			m_bFrameLoaded = true;
			frmInput.btnOK.disabled = false;
		}
	}
	catch(e)
	{
	}
}

/********************************************************************************/
//功能：把elem对应的被选择项添加到结果xmlDoc下的root中
/********************************************************************************/
function addSelectNode(xmlSrc, root, elem)
{
	var oTR = getOwnerTR(elem);
	var xNode = createDomDocument(oTR.xml).documentElement;

	if (xmlSrc)
	{
		var strAllPathName = xNode.getAttribute("ALL_PATH_NAME");
		var re = /\\/g;
		var strTemp = strAllPathName.replace(re, "\\\\");
		if (root.selectSingleNode("object[@ALL_PATH_NAME=\"" + strTemp + "\"]"))
			return;
	}

	var node = appendNode(root, "object");

	for (var i = 0; i < xNode.attributes.length; i++)
	{
		node.setAttribute(xNode.attributes.item(i).nodeName, xNode.attributes.item(i).nodeValue);
	}
}

/********************************************************************************/
//功能：获取xmlSrc的所有内容属性
/********************************************************************************/
function getListSelectedNode(xmlSrc)
{
	var xmlDoc = null;

	if (xmlSrc)
		xmlDoc = xmlSrc;
	else
		xmlDoc = createDomDocument("<NodesSelected/>");

	var root = xmlDoc.documentElement;
	var container = ouUserListBody;
	var bMultiSelect = checkArg(m_interfaceParam, "MultiSelect", true);

	if (bMultiSelect)
	{
		for (var i = 0; i < container.all.length; i++)
		{
			var chk = container.all(i);

			if (chk.tagName == "INPUT" && chk.type == "checkbox")
				if (chk.checked)
					addSelectNode(xmlSrc, root, chk);
		}
	}
	else
	{
		for (var i = 0; i < ouUserListBody.rows.length; i++)
		{
			var oRow = ouUserListBody.rows[i];

			if (oRow.selected)
			{
				addSelectNode(xmlSrc, root, oRow);
				break;
			}
		}
	}

	return xmlDoc;
}

/********************************************************************************/
//功能：获取所有被选中的数据对象
/********************************************************************************/
function getAllSelectedNode()
{
	var strXml = "";
	var xmlDoc = null;

	if (m_bFrameLoaded)
	{
		strXml = iFrameDocument("innerFrame").getAllSelectedNode();

		if (strXml.length > 0)
			xmlDoc = createDomDocument(strXml);
	}

	xmlDoc = getListSelectedNode(xmlDoc);
	xmlDoc.documentElement.setAttribute("ACCESS_LEVEL", frmInput.USER_RANK.value);
	xmlDoc.documentElement.setAttribute("START_TIME", frmInput.startDate.value);
	xmlDoc.documentElement.setAttribute("END_TIME", frmInput.endDate.value);

	return xmlDoc;
}

/********************************************************************************/
//功能：对应界面中的“确定”数据操作
/********************************************************************************/
function onSaveClick()
{
	try
	{
		var xmlDoc = getAllSelectedNode();
		var strXml = "";

		if (xmlDoc.documentElement.childNodes.length > 0)
			strXml = xmlDoc.xml;
		else
		{
			if (checkArg(m_interfaceParam, "MultiSelect", true))
				trueThrow(ouUserListBody.rows.length > 0, "请选中您需要的对象的选框后再按“确定”！\n\n如果不需要选择请按“取消”退出！");
			else
				trueThrow(ouUserListBody.rows.length > 0, "请选中您需要的对象后再按“确定”！\n\n如果不需要选择请按“取消”退出！");
		}

		window.returnValue = strXml;
		if (m_MulitServer)
		{
			if (strXml == null || strXml == "")
				window.clipboardData.clearData("Text");
			else
				window.clipboardData.setData("Text", strXml);//add by yuanyong 20070131
		}

		window.close();
	}
	catch (e)
	{
		showError(e);
	}
}

function onCancelClick()
{
	try
	{
		if (m_MulitServer)
			window.clipboardData.clearData("Text");
			
		window.close();
	}
	catch (e)
	{
		showError(e);
	}
}

/********************************************************************************/
//功能：界面列表中的各个行结果的最后添加一项checkbox对应操作
/********************************************************************************/
function setLastCell(xmlNode, oTD)
{
	oTD.innerText = "";

	var chk = document.createElement("<input type='checkbox'>");

	chk.style.border = "none";
	oTD.insertAdjacentElement("afterBegin", chk);
}

/********************************************************************************/
//功能：列表界面中设置描述信息数据
/********************************************************************************/
function setDescriptionCell(xmlNode, oTD)
{
	oTD.innerText = getReversePath(xmlNode.selectSingleNode("dn").text, "\\", "DC");
}

/********************************************************************************/
//功能：列表界面中的行回调函数
/********************************************************************************/
function gridCallBack(strCmd, oTD, xmlNode, nodeName, nodeText)
{
	switch(strCmd)
	{
		case "onCalc":	if (nodeName.length == 0)
						{
							if (checkArg(m_interfaceParam, "MultiSelect", true))
								setLastCell(xmlNode, oTD);
						}
						else
						if (nodeName == "description")
							setDescriptionCell(xmlNode, oTD);
						else
						{
							calcCells(oTD, xmlNode, nodeName, nodeText);
							oTD.title = getSingleNodeText(xmlNode, "dn", oTD.title);
						}
						break;
		case "onClick":	
						{
							setTDSelected(oTD);
							break;
						}
	}
						
}

/********************************************************************************/
//功能：数据刷新
/********************************************************************************/
function refreshData()
{
	doQuery(m_xmlCondition);
}

/********************************************************************************/
//功能：针对性的数据查询
/********************************************************************************/
function doQuery(xmlCondition)
{  
	try
	{
		with (frmInput)
		{
			
			trueThrow(queryName.value.length == 0, "对不起，请输入查询对象名称！");
			trueThrow(queryName.value.indexOf("*") >= 0 
				|| queryName.value.indexOf("%") >= 0 
				|| queryName.value.indexOf(" ") >= 0,
				"对不起，系统对象查询中不能出现空格、*和%！");
			btnOK.disabled = true;
			btnCancel.disabled = true;
		}
		
		var xmlDoc = null;

		if (xmlCondition)
			xmlDoc = xmlCondition;
		else
			xmlDoc = getXmlFromInterface();

		m_xmlCondition = xmlDoc;
        
		var xmlResult = xmlSend("../sysSearch/OGUSearch.aspx", xmlDoc);
        
		checkErrorResult(xmlResult);
		
		clearAllRows(ouUserListBody);
		trueThrow(xmlResult.documentElement.childNodes.length == 0, "对不起，系统中没有您需要查找的对象\n\n您可以通过修改查询条件进行新的查询！")
		
		ouUserTable.listSortId = 1;
		createGridBodyByHead(xmlResult.documentElement.firstChild, 
							ouUserListBody,
							null,
							-1,
							ouUserHeader.rows(0),
							ouUserTable,
							gridCallBack);
	}
	finally
	{
		with (frmInput)
		{
			btnOK.disabled = false;
			btnCancel.disabled = false;
		}
	}
}

/********************************************************************************/
//功能：界面中的操作（CTRL + E的键盘操作对应执行数据查询）
/********************************************************************************/
function onDocumentKeyPress()
{
	try
	{
		if (frmInput.querySearch.checked)
			if (event.keyCode == 5)	//'CTRL+E
				doQuery();
	}
	catch(e)
	{
		showError(e);
	}
}

/********************************************************************************/
//功能：数据查询按钮的点击
/********************************************************************************/
function onQueryClick()
{
	try
	{
		doQuery();
	}
	catch(e)
	{
		showError(e);
	}
	finally
	{
		event.returnValue = false;
		return false;
	}
}

/********************************************************************************/
//功能：对应于被查询对象选择的对应操作
/********************************************************************************/
function onRadioButtonClick()
{
	try
	{
		disablPersonInput();
		showRankCodeSelect();
	}
	catch (e)
	{
		showError(e);
	}
}

/********************************************************************************/
//功能：查询类型选择（查询方式还是机构树的选择方式）
/********************************************************************************/
function onQueryTypeChick()
{
	var currentBtn = event.srcElement;
	var lastBtn = null;

	if (currentBtn.id == "queryTree")
		lastBtn = frmInput.querySearch;
	else
		lastBtn = frmInput.queryTree;

	lastBtn.checked = false;
	currentBtn.checked = true;

	setQueryType();
	
	showRankCodeSelect();
}

/********************************************************************************/
//功能：设置查询类型的对应操作
/********************************************************************************/
function setQueryType()
{
	with(frmInput)
	{
		if (queryTree.checked)
		{
			if (!m_bFrameLoaded)
				createTreeFrame();

			frameContainer.style.display = "inline";
			querySearchDiv.style.display = "none";
			btnOK.disabled = !m_bFrameLoaded;
		}
		else
		{
			frameContainer.style.display = "none";
			querySearchDiv.style.display = "inline";
			btnOK.disabled = false;

			queryName.focus();
		}
	}
}

/********************************************************************************/
//功能：建立机构树对应的Frame内容
/********************************************************************************/
function createTreeFrame()
{
	frameContainer.innerHTML = 
		"<iframe id='innerFrame' onreadystatechange='onFrameStateChange();' style='BORDER-RIGHT: black 1px solid; " 
		+ "PADDING-RIGHT: 0px; BORDER-TOP: black 1px solid; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; MARGIN: 0px; " 
		+ "BORDER-LEFT: black 1px solid; WIDTH: 100%; PADDING-TOP: 0px; BORDER-BOTTOM: black 1px solid; HEIGHT: 100%; " 
		+ "BACKGROUND-COLOR: transparent' frameborder='no' scrolling='no' " 
		+ "src='../exports/OGUTree.aspx?" + getFrameParams() + "'></iframe>";
}

/********************************************************************************/
//功能：获取机构树展现的所有参数
/********************************************************************************/
function getFrameParams()
{
	with (frmInput)
	{
		var strRootOrg = "&rootOrg=" + ALL_PATH_NAME.value;
	//	if (m_showOGUTree)
		if (selectBelongTo.style.display == "inline")
			strRootOrg = "&rootOrg=" + selectBelongTo.value;
			
		var strOrgAccessLevel = "&orgAccessLevel=" + ORGANIZATION_RANK_SEARCH.value;
		var strUserAccessLevel = "&userAccessLevel=" + USER_RANK_SEARCH.value;
		var strParams = "topControl=dnSelected&listMask=7&multiSelect=1&canSelectRoot=true" 
			+ strRootOrg + strOrgAccessLevel + strUserAccessLevel;
		
		return strParams;
	}
}

/********************************************************************************/
//功能：对应于ALL_PATH_NAME控件的属性值的修改
/********************************************************************************/
function belongToChange()
{
	try
	{
		m_bFrameLoaded = false;
	}
	catch (e)
	{
		showError(e);
	}
}

/********************************************************************************/
//功能：选择查询对象时候的伴随操作（屏蔽掉一些选择）
/********************************************************************************/
function disablPersonInput(bFlag)
{
	var bDisabled = true;

	if (bFlag != null)
		bDisabled = bFlag;

	with(frmInput)
	{
		if (USERS.checked)
			bDisabled = false;

		disabledItem(new Array(firstPerson), bDisabled);
	}
}

/********************************************************************************/
//功能：根据obj对象的属性增加一个数据查询条件
/********************************************************************************/
function appendCondition(obj, root)
{    
	if (obj.disabled == false)
	{
		var value = null;
		var bAppend = false;
   
		if (obj.tagName == "INPUT" || obj.tagName == "oguinput" || obj.tagName == "SELECT")
		{
		
			if (obj.type == "text" || obj.type == "hidden" || obj.tagName == "SELECT" || obj.tagName == "oguinput")
			{			    
				value = obj.value;
				bAppend = value.length > 0;
			}
			else
			{
				if (obj.type == "radio" || obj.type == "checkbox")
				{
					value = obj.checked;
					bAppend = value;
				}
			}
		}

		if (bAppend)
		{
			var strFld = obj.dataFld;

			if (!strFld)
				strFld = obj.id;

			appendNode(root, strFld, value);
		}
	}
}

/********************************************************************************/
//功能：根据数据组合形成一次查询的所有查询条件
/********************************************************************************/
function appendConditions(objArray, root)
{
	for (var i = 0; i < objArray.length; i++)
	{
		if (objArray[i] != null)
			appendCondition(objArray[i], root);
	}
}

/********************************************************************************/
//功能：根据界面各个空间属性形成查询条件
/********************************************************************************/
function getXmlFromInterface()
{
	var xmlDoc = createCommandXML("queryOGUByCondition");
	var root = xmlDoc.documentElement.firstChild;
    
	with(frmInput)
	{
		var RANK_SEARCH = null;
		if (ORGANIZATIONS.checked)
			RANK_SEARCH = ORGANIZATION_RANK_SEARCH;
		else
		{
			if (USERS.checked)
				RANK_SEARCH = USER_RANK_SEARCH;
		}
		
		var objOrgRoot = ALL_PATH_NAME;
		if (selectBelongTo.style.display != "none")
			objOrgRoot = selectBelongTo;

		appendConditions(new Array(objOrgRoot, queryName, ORGANIZATIONS, USERS, GROUPS, firstPerson, RANK_SEARCH), root);
	} 
   
	return xmlDoc;
}

/********************************************************************************/
//功能：界面退出的时候在客户端保存部分信息数据
/********************************************************************************/
function savePageSettings()
{
	with(frmInput)
	{
		persistObj.setAttribute("queryTree", queryTree.checked);
		persistObj.setAttribute("querySearch", querySearch.checked);
		persistObj.setAttribute("ORGANIZATIONS", ORGANIZATIONS.checked);
		persistObj.setAttribute("USERS", USERS.checked);
		persistObj.setAttribute("GROUPS", GROUPS.checked);
		persistObj.setAttribute("firstPerson", firstPerson.checked);
	}
	persistObj.save("oXMLBranch");
}

/********************************************************************************/
//功能：从客户端获取部分的已保留信息数据
/********************************************************************************/
function loadPageSettings()
{
	persistObj.load("oXMLBranch");

	with(frmInput)
	{
		readCheckValue(queryTree, persistObj, "queryTree");
		readCheckValue(querySearch, persistObj, "querySearch");
		readCheckValue(ORGANIZATIONS, persistObj, "ORGANIZATIONS");
		readCheckValue(USERS, persistObj, "USERS");
		readCheckValue(GROUPS, persistObj, "GROUPS");
		readCheckValue(firstPerson, persistObj, "firstPerson");

		if (ORGANIZATIONS.checked || GROUPS.checked)
			disablPersonInput();
		else
			disablPersonInput(false);
			
//		showRankCodeSelect();
	}
}

function showRankCodeSelect()
{
	with(frmInput)
	{
		if (queryTree.checked)
		{
			showOrgRank.style.display = "inline";
			showUserRank.style.display = "inline";
		}
		else
		{
			if (ORGANIZATIONS.checked)
			{
				showOrgRank.style.display = "inline";
				showUserRank.style.display = "none";
			}
			if (USERS.checked)
			{
				showOrgRank.style.display = "none";
				showUserRank.style.display = "inline";
			}
			if (GROUPS.checked)
			{
				showOrgRank.style.display = "none";
				showUserRank.style.display = "none";
			}
		}
	}
}

/********************************************************************************/
//功能：从对象pObj中读取attrName对应属性（默认使用""替换）
/********************************************************************************/
function readAttribute(pObj, attrName)
{
	var attr = pObj.getAttribute(attrName);

	if (attr)
		return attr;
	else
		return "";
}

/********************************************************************************/
//功能：从pObj对象中读取pName对应属性送入对象elem中作为attrName属性
/********************************************************************************/
function readInputValue(elem, attrName, pObj, pName)
{
	var value = readAttribute(pObj, pName);

	if (value.length > 0)
		elem.setAttribute(attrName, value);
}

/********************************************************************************/
//功能：
/********************************************************************************/
function readCheckValue(checkCtrl, pObj, attrName)
{
	var value = readAttribute(pObj, attrName);

	if (value.length > 0)
		checkCtrl.checked = (value == "true");
}

/********************************************************************************/
//功能：
/********************************************************************************/
function onDocumentUnload()
{
	try
	{
		savePageSettings();
	}
	catch (e)
	{
		showError(e);
	}
}

/********************************************************************************/
//功能：
/********************************************************************************/
function getArgValue(arg, tagName, defaultValue)
{
	var result = false;
	var node = null;

	if (arg)
		node = arg.documentElement.selectSingleNode(tagName);

	if (node)
		result = node.text;
	else
		if (defaultValue)
			result = defaultValue;

	return result;
}

/********************************************************************************/
//功能：
/********************************************************************************/
function checkArg(arg, tagName, defaultValue)
{
	var result = getArgValue(arg, tagName, defaultValue);

	if (typeof(result) == "string")
		return result.toLowerCase() == "true";
	else
		return result;
}

/********************************************************************************/
//功能：
/********************************************************************************/
function setDisplayStyle(elem, bShow)
{
	if (bShow)
		elem.style.display = "inline";
	else
		elem.style.display = "none";
}

/********************************************************************************/
//功能：
/********************************************************************************/
function setInterfaceByXml(arg)
{
	if (arg)
	{
		var value = checkArg(arg, "QueryOGU", true);

		with(frmInput)
		{
			querySearch.checked = value;
			queryTree.checked = !value;

			//ALL_PATH_NAME.value = getArgValue(arg, "RootOrg", "");
			setRootOrg(arg);
			
			if (getArgValue(arg, "ShowTimeLimit", false))
				timeLimitTR.style.display = "inline";
		}

		logo.style.backgroundImage = getArgValue(arg, "Logo", logo.style.backgroundImage);
		logo.style.width = getArgValue(arg, "LogoWidth", 32);
		logo.style.height = getArgValue(arg, "LogoHeight", 32);

		Caption.innerText = getArgValue(arg, "Caption", Caption.innerText);
		setDisplayStyle(bottomRow, checkArg(arg, "BottomRow", true));
	}
	logo.style.display = "inline";
	Caption.style.display = "inline";
	
	frmInput.ORGANIZATION_RANK_SEARCH.onchange = onRankChanged;
	frmInput.USER_RANK_SEARCH.onchange = onRankChanged;
}

/********************************************************************************/
//功能：
/********************************************************************************/
function setRootOrg(arg)
{
	var node = arg.documentElement.selectSingleNode("RootOrg");
	trueThrow(node == null, "对不起，没有设置根对象（选择范围）！");
	
	with (frmInput)
	{
		var showNode = arg.documentElement.selectSingleNode("BottomRow");
		if (showNode && showNode.text.toLowerCase() == "false")
			m_showOGUTree = false;//界面中不使用机构树
			
		if (m_showOGUTree && node.firstChild.tagName)
		{
			selectBelongTo.style.display = "inline";
			ALL_PATH_NAME.style.display = "none";
			
			var eleNode = node.firstChild;
			while (eleNode)
			{
				var newOpt = document.createElement("OPTION");
				
				newOpt.value = eleNode.text;
				newOpt.text = eleNode.text;
				
				selectBelongTo.options.add(newOpt);
				
				eleNode = eleNode.nextSibling;
			}
		}
		else
		{
			ALL_PATH_NAME.style.display = "inline";
			selectBelongTo.style.display = "none";
			ALL_PATH_NAME.value = node.text;
		}
	}
}

/********************************************************************************/
//功能：设置要求的行政界别限制（查询限制）
/********************************************************************************/
function onRankChanged()
{   
	try
	{
		if (frmInput.queryTree.checked)
			createTreeFrame();
	}
	catch (e)
	{
		showError(e);
	}
}

/********************************************************************************/
//功能：文档界面的初始化（添加角色中成员或者机构人员系统中的对象查询）
/********************************************************************************/
function onDocumentLoad()
{
	try
	{
		loadPageSettings();

		if (window.dialogArguments == null)
		{
			m_interfaceParam = window.clipboardData.getData("Text");
			window.clipboardData.clearData("Text");	
			m_MulitServer = true;		
		}
		else
			m_interfaceParam = window.dialogArguments;

		if (typeof(m_interfaceParam) == "string")
			m_interfaceParam = createDomDocument(m_interfaceParam);

		initDocumentEvents();

		createTableHeadByXml(ColumnTitleOU.XMLDocument, ouUserHeader);

		setInterfaceByXml(m_interfaceParam);

		setQueryType();
		
		showRankCodeSelect();
		
		bindCalendarToInput(hCalendar, frmInput.startDate);
		bindCalendarToInput(hCalendar, frmInput.endDate);
		
		frmInput.startDate.value = dateToStr(new Date(), DATEPART.DATE);
		frmInput.endDate.value = dateToStr(new Date(), DATEPART.DATE);

		window.returnValue = "";
	}
	catch (e)
	{
		showError(e);
	}
}
//-->