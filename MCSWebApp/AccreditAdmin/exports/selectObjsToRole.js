<!--
var m_interfaceParam = null;

var m_bFrameLoaded = false;
var m_xmlCondition = null;

var m_txtSelection = null;

var m_showOGUTree = true;

var m_MulitServer = false;////��WebServer�ĵ��ã��ڿ�Server���õ�ʱ�����Clipboard��Ϊ���ݴ���

/********************************************************************************/
//���ܣ��˵�չ�е�����Ԥ����
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
//���ܣ���Ӧ�˵��еĲ˵���ѡ�в���
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
//���ܣ��˵�չ��
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
//���ܣ�Frame״̬�ı䣨��Ӧ��״̬�ı�Ϊcomplete��
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
//���ܣ���elem��Ӧ�ı�ѡ������ӵ����xmlDoc�µ�root��
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
//���ܣ���ȡxmlSrc��������������
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
//���ܣ���ȡ���б�ѡ�е����ݶ���
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
//���ܣ���Ӧ�����еġ�ȷ�������ݲ���
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
				trueThrow(ouUserListBody.rows.length > 0, "��ѡ������Ҫ�Ķ����ѡ����ٰ���ȷ������\n\n�������Ҫѡ���밴��ȡ�����˳���");
			else
				trueThrow(ouUserListBody.rows.length > 0, "��ѡ������Ҫ�Ķ�����ٰ���ȷ������\n\n�������Ҫѡ���밴��ȡ�����˳���");
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
//���ܣ������б��еĸ����н����������һ��checkbox��Ӧ����
/********************************************************************************/
function setLastCell(xmlNode, oTD)
{
	oTD.innerText = "";

	var chk = document.createElement("<input type='checkbox'>");

	chk.style.border = "none";
	oTD.insertAdjacentElement("afterBegin", chk);
}

/********************************************************************************/
//���ܣ��б����������������Ϣ����
/********************************************************************************/
function setDescriptionCell(xmlNode, oTD)
{
	oTD.innerText = getReversePath(xmlNode.selectSingleNode("dn").text, "\\", "DC");
}

/********************************************************************************/
//���ܣ��б�����е��лص�����
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
//���ܣ�����ˢ��
/********************************************************************************/
function refreshData()
{
	doQuery(m_xmlCondition);
}

/********************************************************************************/
//���ܣ�����Ե����ݲ�ѯ
/********************************************************************************/
function doQuery(xmlCondition)
{  
	try
	{
		with (frmInput)
		{
			
			trueThrow(queryName.value.length == 0, "�Բ����������ѯ�������ƣ�");
			trueThrow(queryName.value.indexOf("*") >= 0 
				|| queryName.value.indexOf("%") >= 0 
				|| queryName.value.indexOf(" ") >= 0,
				"�Բ���ϵͳ�����ѯ�в��ܳ��ֿո�*��%��");
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
		trueThrow(xmlResult.documentElement.childNodes.length == 0, "�Բ���ϵͳ��û������Ҫ���ҵĶ���\n\n������ͨ���޸Ĳ�ѯ���������µĲ�ѯ��")
		
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
//���ܣ������еĲ�����CTRL + E�ļ��̲�����Ӧִ�����ݲ�ѯ��
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
//���ܣ����ݲ�ѯ��ť�ĵ��
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
//���ܣ���Ӧ�ڱ���ѯ����ѡ��Ķ�Ӧ����
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
//���ܣ���ѯ����ѡ�񣨲�ѯ��ʽ���ǻ�������ѡ��ʽ��
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
//���ܣ����ò�ѯ���͵Ķ�Ӧ����
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
//���ܣ�������������Ӧ��Frame����
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
//���ܣ���ȡ������չ�ֵ����в���
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
//���ܣ���Ӧ��ALL_PATH_NAME�ؼ�������ֵ���޸�
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
//���ܣ�ѡ���ѯ����ʱ��İ�����������ε�һЩѡ��
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
//���ܣ�����obj�������������һ�����ݲ�ѯ����
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
//���ܣ�������������γ�һ�β�ѯ�����в�ѯ����
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
//���ܣ����ݽ�������ռ������γɲ�ѯ����
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
//���ܣ������˳���ʱ���ڿͻ��˱��沿����Ϣ����
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
//���ܣ��ӿͻ��˻�ȡ���ֵ��ѱ�����Ϣ����
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
//���ܣ��Ӷ���pObj�ж�ȡattrName��Ӧ���ԣ�Ĭ��ʹ��""�滻��
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
//���ܣ���pObj�����ж�ȡpName��Ӧ�����������elem����ΪattrName����
/********************************************************************************/
function readInputValue(elem, attrName, pObj, pName)
{
	var value = readAttribute(pObj, pName);

	if (value.length > 0)
		elem.setAttribute(attrName, value);
}

/********************************************************************************/
//���ܣ�
/********************************************************************************/
function readCheckValue(checkCtrl, pObj, attrName)
{
	var value = readAttribute(pObj, attrName);

	if (value.length > 0)
		checkCtrl.checked = (value == "true");
}

/********************************************************************************/
//���ܣ�
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
//���ܣ�
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
//���ܣ�
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
//���ܣ�
/********************************************************************************/
function setDisplayStyle(elem, bShow)
{
	if (bShow)
		elem.style.display = "inline";
	else
		elem.style.display = "none";
}

/********************************************************************************/
//���ܣ�
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
//���ܣ�
/********************************************************************************/
function setRootOrg(arg)
{
	var node = arg.documentElement.selectSingleNode("RootOrg");
	trueThrow(node == null, "�Բ���û�����ø�����ѡ��Χ����");
	
	with (frmInput)
	{
		var showNode = arg.documentElement.selectSingleNode("BottomRow");
		if (showNode && showNode.text.toLowerCase() == "false")
			m_showOGUTree = false;//�����в�ʹ�û�����
			
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
//���ܣ�����Ҫ�������������ƣ���ѯ���ƣ�
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
//���ܣ��ĵ�����ĳ�ʼ������ӽ�ɫ�г�Ա���߻�����Աϵͳ�еĶ����ѯ��
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