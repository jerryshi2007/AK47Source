<!--
var m_arrayMenu = null;

function onSaveClick()
{
	try
	{
		onObjNameChange();
		
		checkInputNull();
		
		trueThrow(frmInput.PERSON_ID.value.length != 0 && frmInput.PERSON_ID.value.length != 7, "对不起，系统要求“人员编号”一律采用7位！");
		
		trueThrow(compareDate(frmInput.START_TIME.value, frmInput.END_TIME.value) > 0, "对不起，设置中的“启动时间”不能晚于“过期时间”！");
		
		trueThrow(	frmInput.OBJ_NAME.value.indexOf("\\") >= 0 || 
					frmInput.DISPLAY_NAME.value.indexOf("\\") >= 0 || 
					frmInput.LOGON_NAME.value.indexOf("\\") >= 0,  
					"对不起，“人员”的名称中不允许包含字符“\\”！");
		
		onSetSearchName();

		var arg = window.dialogArguments;

		var xmlDoc = null;

		switch (arg.op)
		{
			case "AddSideline":
			case "Insert":
				xmlDoc = createInsertDoc(frmInput.elements, "USERS");
				xmlDoc.documentElement.setAttribute("opType", arg.op);
				break;
			case "Update":
				xmlDoc = createUpdateDoc(frmInput.elements, "USERS", "USER_GUID", "=", arg.guid);
				appendNode(xmlDoc.documentElement.selectSingleNode(".//WHERE"), "PARENT_GUID", arg.parentGuid);
				break;
			default :			trueThrow(true, "对不起，系统中没有相应的数据操作类型！");
				break;
		}
		var nodeSet = xmlDoc.selectSingleNode(".//SET");

		if (typeof(frmInput.AD_COUNT) != "undefined")//add by cgac\yuan_yong 2004-11-14
			addCheckBoxNode(nodeSet, frmInput.AD_COUNT);
		if (typeof(frmInput.SYSCONTENT1) != "undefined")
			if (frmInput.SYSCONTENT1.type == "checkbox")
				addCheckBoxNode(nodeSet, frmInput.SYSCONTENT1, "1", "2");

		if (nodeSet && nodeSet.hasChildNodes())
		{
			var bContinue = true;

			if (arg.op == "Update" && nodeSet.selectSingleNode("OBJ_NAME") != null)
				bContinue = window.confirm("修改“对象名称”可能会影响到对此用户在其他系统中的正常使用，您要继续吗？");

			if (bContinue)
			{
				if (arg.op == "Insert")
					appendNode(nodeSet, "PARENT_GUID", arg.parentGuid);
				else
				{
					if (arg.op == "AddSideline")
					{
						appendNode(nodeSet, "PARENT_GUID", arg.sParentGuid);
						appendNode(nodeSet, "USER_GUID", arg.guid);
					}
				}

				var xmlResult = xmlSend("../sysAdmin/OGUEditer.aspx", xmlDoc);

				checkErrorResult(xmlResult);
				
				if (nodeSet.selectSingleNode("RANK_CODE") != null)
					xmlResult.documentElement.firstChild.setAttribute("NAME", frmInput.RANK_CODE.options[frmInput.RANK_CODE.selectedIndex].text);
					
				window.returnValue = xmlResult.xml;
				window.close();
			}
		}
		else
		{
			window.close();
		}
	}
	catch(e)
	{
		showError(e);
	}
}

function onSetSearchName()
{
        frmInput.searchName.value = frmInput.LAST_NAME.value + frmInput.FIRST_NAME.value +' ' + 
                                    frmInput.PINYIN.value +' ' + frmInput.LOGON_NAME.value;
}

function onPosturalChange()
{
	try
	{
		with (frmInput)
		{
			END_TIME.disabled = (event.srcElement.selectedIndex == 2);
		}
	}
	catch (e)
	{
		showError(e);
	}
}

function onPinyinSpanPopup()
{
	if (m_arrayMenu)
	{
		var x = absLeft(frmInput.LOGON_NAME);
		var y = absTop(pinyinSpan) + pinyinSpan.offsetHeight;
		pinyinMenu.show(x, y);
	}
}

function onNewPinyinSpanPopup()
{
	if (m_arrayMenu)
	{
		var x = absLeft(frmInput.PINYIN);
		var y = absTop(newPinyinSpan) + newPinyinSpan.offsetHeight;
		pinyinMenu.show(x, y);
	}
}

function pinyinMenuClick()
{
	frmInput.LOGON_NAME.value = event.menuData;
	frmInput.PINYIN.value = event.menuData;
}

function getPinYin(strValue)
{
	try
	{
		var strResult = strValue;

		var xmlDoc = createCommandXML("getPinYin", strValue);
		var xmlResult = xmlSend("../sysSearch/OGUSearch.aspx", xmlDoc);

		var root = xmlResult.documentElement;
		m_arrayMenu = new Array(root.childNodes.length);

		for (var i = 0; i < m_arrayMenu.length; i++)
		{
			var node = root.childNodes[i];
			var strPinYin = node.getAttribute("pinyin");

			if (i == 0)
				strResult = strPinYin;

			m_arrayMenu[i] = strPinYin + ",,,,," + strPinYin;
		}

		if (m_arrayMenu)
			pinyinMenu.buildMenu(m_arrayMenu);
		
		return strResult;
	}
	catch(e)
	{
		showError(e);
	}
}

function addCheckBoxNode(xmlNodeSet, chk, strCheckedValue, strUnCheckedValue)
{
	if (typeof(strCheckedValue) == "undefined")
		strCheckedValue = "1";
	if (typeof(strUnCheckedValue) == "undefined")
		strUnCheckedValue = "0";
		
	if (chk.checked != chk.oldValue)
	{
		var node = xmlNodeSet.selectSingleNode(chk.id);
		if (node == null)
			node = appendNode(xmlNodeSet, chk.id);
		node.text = chk.checked ? strCheckedValue : strUnCheckedValue;
	}
	else
	{
		var node = xmlNodeSet.selectSingleNode(chk.id);
		if (node != null)
			xmlNodeSet.removeChild(node);
	}
}

function setCheckBoxValueByNode(nodeSet, chk, strCheckedValue, strUnCheckedValue)
{
	if (typeof(strCheckedValue) == "undefined")
		strCheckedValue = "1";
	if (typeof(strUnCheckedValue) == "undefined")
		strUnCheckedValue = "0";
		
	var node = nodeSet.selectSingleNode(chk.id);

	if (node)
		if (node.text == strCheckedValue)
			chk.checked = true;
		else
			chk.checked = false;
	else
		chk.checked = false;

	chk.oldValue = chk.checked;
}

function onNameChange()
{
	try
	{
		if (frmInput.FIRST_NAME.value != "" && frmInput.LAST_NAME.value != "" && frmInput.OBJ_NAME.value == "")
		{
			frmInput.OBJ_NAME.value = frmInput.LAST_NAME.value + frmInput.FIRST_NAME.value;
			onObjNameChange();
		}
	}
	catch (e)
	{
		showError(e);
	}
}

function onObjNameChange()
{
	try
	{
		with (frmInput)
		{
			ALL_PATH_NAME.value = parentAllPathName.value + "\\" + OBJ_NAME.value;
			if (DISPLAY_NAME.value.length == 0)
				DISPLAY_NAME.value = OBJ_NAME.value;
			
			if (LOGON_NAME.value.length == 0)
				LOGON_NAME.value = getPinYin(OBJ_NAME.value);		
			
			if (PINYIN.value.length == 0)
				PINYIN.value = getPinYin(OBJ_NAME.value);
				
		}
	}
	catch (e)
	{
		showError(e);
	}
}

function SetInterfaceByOpType()
{
	var arg = window.dialogArguments;
	with (frmInput)
	{
		switch (arg.op)
		{
			case "AddSideline":	
			case "Insert":
				CREATE_TIME.value = formatToday();
				START_TIME.value = formatToday();
				END_TIME.value = formatDate(AddDayDate(new Date(), GetThisMonthEndDate().getDate()));
				break;
			case "Update":
				break;
		}
		
		if (SIDELINE.value == "1")
		{
			FIRST_NAME.disabled = true;
			LAST_NAME.disabled = true;
			LOGON_NAME.disabled = true;
			
//			AD_COUNT.disabled = true;	//changed by cgac\yuan_yong 2004-11-14
				
			RANK_CODE.disabled = true;
			userTitle.innerText += "（兼职）";
		}
		
		bindCalendarToInput(hCalendar, CREATE_TIME);
		bindCalendarToInput(hCalendar, START_TIME);
		bindCalendarToInput(hCalendar, END_TIME);
	}
}

function onSetUserAttributes()
{
	try
	{
		with (frmInput)
		{
			var obj = event.srcElement;
			
			if (ATTRIBUTES.value.length == 0)
				ATTRIBUTES.value = "0";
				
			if (obj.checked)
				ATTRIBUTES.value = parseInt(ATTRIBUTES.value) + parseInt(obj.value);
			else
				ATTRIBUTES.value = parseInt(ATTRIBUTES.value) - parseInt(obj.value);
				
			formatNumberObj(ATTRIBUTES, STD_YEAR_FORMAT);
		}
	}
	catch (e)
	{
		showError(e);
	}
}

function initUserAttributes()
{
	with (frmInput)
	{
		if (ATTRIBUTES.value.length == 0)
			ATTRIBUTES.value = "0";
			
		var oValue = parseInt(ATTRIBUTES.value);
		
		for (var i = 0; i < 6; i++)
		{
			if ((oValue & Math.pow(2, i)) != 0)
				document.all("ATTRIBUTES_" + i).checked = true;
		}
	}
}

function onDocumentLoad()
{
	try
	{
		window.returnValue = "";
		
		getExtraDisplayShow(AConfig.XMLDocument, userContentTable, "Display", "USERS");
		getExtraDisplayShow(AConfig.XMLDocument, userContentTable, "Display", "OU_USERS");
		
		initDocumentEvents(frmInput);
		initElementsByDict(USER_XSD.XMLDocument);
		
		var arg = window.dialogArguments;
		switch (arg.op)
		{
			case "Insert":
				break;
			case "AddSideline":	
				var xmlDoc = createDomDocument(frmInput.userData.value);
				var tableNode = xmlDoc.documentElement.firstChild;
				
				xmlDataFillForm(tableNode, frmInput.elements, USER_XSD.XMLDocument);
				if (typeof(frmInput.AD_COUNT) != "undefined")
					setCheckBoxValueByNode(tableNode, frmInput.AD_COUNT);
				if (typeof(frmInput.SYSCONTENT1) != "undefined")
					if (frmInput.SYSCONTENT1.type == "checkbox")
						setCheckBoxValueByNode(tableNode, frmInput.SYSCONTENT1)
				
				initUserAttributes();				
				break;
			case "Update":
				var xmlDoc = createDomDocument(frmInput.userData.value);
				var tableNode = xmlDoc.documentElement.firstChild;
				
				xmlResultFillForm(tableNode, frmInput.elements, USER_XSD.XMLDocument);
				if (typeof(frmInput.AD_COUNT) != "undefined")
					setCheckBoxValueByNode(tableNode, frmInput.AD_COUNT);
				if (typeof(frmInput.SYSCONTENT1) != "undefined")
					if (frmInput.SYSCONTENT1.type == "checkbox")
						setCheckBoxValueByNode(tableNode, frmInput.SYSCONTENT1)
				
				initUserAttributes();				
				break;
		}
		
		SetInterfaceByOpType();
		
		if (frmInput.opPermission.value == "false")
			btnOK.disabled = true;
	}
	catch(e)
	{
		showError(e);
		window.close();
	}
}
//-->