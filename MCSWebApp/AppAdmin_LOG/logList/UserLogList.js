<!--
	var m_searchStr;
	var listControl = "";
	var m_count;
	var m_objParam = null;
	var m_nCheckItemCount = 0;
	var type = "UserLogList";
	var titleCaption = "用户日志列表";

	function onDocumentLoad()
	{
		try
		{			
			bindCalendarToInput(hCalendar, frmInput.start_time);
			bindCalendarToInput(hCalendar, frmInput.end_time);
					
			buildParam(getFrameParam());
			
			setNewObjStyle(m_objParam);

			if (m_objParam.ViewRoles == "false")
				toolBar.style.display = "none";
			
			var headXmlDocument = getHeadXmlDoc();
			fillGridCaption(titleCaption, type, headXmlDocument);
			
			listControl = "all";
			
			adminDbGrid.dataXML = queryData();
			
			setAppName();
		}
		catch(e)
		{
			showError(e);
		}	
	}

	function getHeadXmlDoc()
	{
		var sortLen = m_objParam.OriginalSort.length;
		var xmlDoc = createDomDocument(headUserXml.XMLDocument.xml);
		var root = xmlDoc.documentElement;
		var appNode = root.selectSingleNode("Column[@dataFld=\"APP_DISPLAYNAME\"]");
		var opNode = root.selectSingleNode("Column[@dataFld=\"OP_DISPLAYNAME\"]");
		
		if (sortLen == 8)
			root.removeChild(appNode);
		else if(sortLen == 12)
			{
				root.removeChild(appNode);
				root.removeChild(opNode);
			}	
		var headXmlDoc = new Object();
		headXmlDoc.XMLDocument = xmlDoc;
		return headXmlDoc;
	}

	function setAppName()
	{
		var appNameListNode = AppNameXml.documentElement.firstChild;
		setSelectValuesByXml(appNameListNode, frmInput.appName, "DISPLAYNAME", "DISPLAYNAME", false);
	}
	
	function setNewObjStyle(m_objParam)
	{
		switch (m_objParam.OriginalSort.length)
		{
			case 4:
				newObj.title = "新建应用类型";
				newObj.style.display = "inline";
				updateObj.style.display = "none";
				break;
			case 8:
				newObj.title = "新建操作类型";
				newObj.style.display = "inline";
				updateObj.style.display = "inline";
				break;
			case 12:
				newObj.style.display = "none";
				updateObj.style.display = "inline";	
				break;
		}
	}

	function setSelectValuesByXml(xmlNode, objSelect, strKeyName, strValueName, bAddNull, callBack)
	{
		clearSelectControl(objSelect);

		var nodeRow = xmlNode;
		var bIgnor = false;
		
		var newOpt = document.createElement("OPTION");
		newOpt.value = "";
		newOpt.text = " ---- ";

		objSelect.options.add(newOpt);
		
		while (nodeRow != null)
		{
			newOpt = document.createElement("OPTION");
			newOpt.value = nodeRow.selectSingleNode(strKeyName).text;
			newOpt.text = nodeRow.selectSingleNode(strValueName).text;

			nodeRow = nodeRow.nextSibling;
			objSelect.options.add(newOpt);
		}
	}

	function queryData(strLastKey, searchStr)
	{
		mainRow.style.height = "450px";
		
		if (strLastKey == null || strLastKey == "")
		{
			strLastKey = 0;
		}
		
		var time1 = frmInput.start_time.value;
		var time2 = frmInput.end_time.value;
		var xmlDoc = createDomDocument("<UserLogList/>");
		var root = xmlDoc.documentElement;
		
		if(time1 == "")
			time1 = "1753-01-01"
		if(time2 == "")
			time2 = "9999-12-31"
		
		root.setAttribute("start_time", time1);
		root.setAttribute("end_time", time2);	
		root.setAttribute("fileID", frmInput.fileIDInput.value);
		root.setAttribute("userName", frmInput.userInput.value);
		root.setAttribute("appName", frmInput.appName.value);
		root.setAttribute("opTypeName", frmInput.opTypeName.value);
		root.setAttribute("listCtr", listControl);
		root.setAttribute("orgSort", m_objParam.OriginalSort);
		
		if(m_objParam.Guid != "" && listControl == "all")
			root.setAttribute("Guid", m_objParam.Guid);
		else 
			root.setAttribute("Guid", "");
		if(m_objParam.appGuid != "" && listControl == "all")
			root.setAttribute("appGuid", m_objParam.OrgGuid);
		else
			root.setAttribute("appGuid", "");
			
		root.setAttribute("rows", adminDbGrid.limitRows);
		root.setAttribute("lastKey", strLastKey);
		
		setElementStatus();

		var xmlResult = xmlSend("../server/ServerLog.aspx", xmlDoc);
		checkErrorResult(xmlResult);
		listControl = "all";
		
		var countNode = xmlResult.documentElement.lastChild;
		m_count = countNode.text;

		var countPage = parseInt(m_count);
		countPage = Math.floor((m_count - 1) / adminDbGrid.limitRows) + 1;
		countSpan.innerText = "       按条件查询一共 " + m_count + " 条  分 " + String(countPage) + " 页显示";
		xmlResult.documentElement.removeChild(countNode);
		return xmlResult;
		
	}

	function setElementStatus()
	{	
		if (m_objParam.OriginalSort != "0000")
			{
				frmInput.style.display = "none";
			}
		else 
			{
				frmInput.style.display = "inline";
			}
	}

	function onGridCalcCell()
	{
		try
		{			
			switch (event.fieldName)
			{
				
				case "LOG_DATE":
					setTitleCell(event.senderElement, event.nodeText);
					break;
					
				case "OP_USER_DISPLAYNAME":
					setTitleCell(event.senderElement,event.nodeText);
					break;
				case "HOST_IP":
					setTitleCell(event.senderElement,event.nodeText);
					break;
				case "Detail":
					setButtonCell(event.senderElement, event.xmlNode, event.nodeText, "ID", onDetailButtonClick);
					break;
				case "ORIGINAL_DATA":
					setTitleCell(event.senderElement,event.nodeText);
					break;
				default:
					break;				
			}
		}
		catch(e)
		{
			showError(e);
		}
	}

	function onSearchClick()
	{
		try
		{
				listControl = "search";
				adminDbGrid.dataXML = queryData();
		}
		catch(e)
		{
			showError(e);
		}
	}


	function onDetailButtonClick()
	{
		try
		{
			var a = event.srcElement;
			var strLink;	
			strLink = "UserLogDetail.aspx?sortID=" + a.key;
			
			var sFeature = "dialogWidth:750px; dialogHeight:400px;center:yes;help:no;resizable:no;scroll:no;status:no";
		
			var returnValue = showModalDialog(strLink, null, sFeature);
			
			if (returnValue == "refresh")
			{
				adminDbGrid.dataXML = queryData();
			}
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

	function buildParam(strParam)
	{
		var xmlDoc = createDomDocument(strParam);
		var root = xmlDoc.documentElement;

		if(root != null)
		{
			m_objParam = new Object();
			m_objParam.Guid = root.getAttribute("GUID");
			m_objParam.OrgGuid = root.getAttribute("rootOrgGuid");
			
			if(root.getAttribute("rootOrgGuid") == "" && root.getAttribute("GUID") == "")
				m_objParam.type = "noCatch";
			else 
				if (root.getAttribute("rootOrgGuid") == "" && root.getAttribute("GUID") != "")
					m_objParam.type = "APP_LOG_TYPE";
				else m_objParam.type = "APP_OPERATION_TYPE";
			
			m_objParam.DisplayName = root.getAttribute("DISPLAYNAME");
			m_objParam.OriginalSort = root.getAttribute("ORIGINAL_SORT");
			m_objParam.OriginalData = root.getAttribute("ORIGINAL_DATA");
			m_objParam.ViewRoles = root.getAttribute("viewRoles");
			m_objParam.CodeName = root.getAttribute("codeName");
			m_objParam.Description = root.getAttribute("description");
			m_gDragSrc = null;//初始化拖动行
		}
	}

	function onToolbarClick()
	{
		try
		{
			var obj = event.srcElement;
			if (obj.tagName == "IMG")
				doToolbarEvent(obj.id);
		}
		catch(e)
		{
			showError(e);
		}
	}

	function onToolbarMouserOver()
	{
		var obj = event.srcElement;

		if (obj.tagName == "IMG")
		{
			obj.oldClassName = obj.className;
			obj.className = "mouseOver";
		}
	}

	function onToolbarMouserOut()
	{
		var obj = event.srcElement;

		if (obj.tagName == "IMG")
			obj.className = obj.oldClassName;
	}
	
	function doToolbarEvent(strID)
	{
		var xmlResult = null;
		var confirmText = getConfirmText();
		
		switch(strID)
		{
			case "newObj":	xmlResult = newAppObject("insert");	//增加应用或应用类型
			
							if(xmlResult == null)
								parent.refreshPage.value = "";
							else
								parent.refreshPage.value = "insert";
							break;
							
			case "updateObj":	newAppObject("update");	//修改应用或应用类型
							
								if(xmlResult == null)
									parent.refreshPage.value = "";
								else
									parent.refreshPage.value = "update";
								break;
							
			case "refresh":	adminDbGrid.dataXML = queryData();
							parent.refreshPage.value = "refresh";
							break;
							
			case "delete":	if(confirm(confirmText))
							{
								deleteSelectedObjects();
								parent.refreshPage.value = "delete";
							}
							break;
		}
	}
	
	function newAppObject(opType)
	{
		var xmlResult = showAppObjDetailDialog(opType, "", m_objParam.Guid, m_objParam.OrgGuid, m_objParam.OriginalSort, m_objParam.type, m_objParam.DisplayName, m_objParam.CodeName, m_objParam.Description);
		
		return xmlResult;
	}
	
	function showAppObjDetailDialog(strOP, nID, Guid, OrgGuid, strAppLevel, strType, strDisplayname, strCodeName, strDescription)
	{
		var sFeature = "dialogWidth:320px; dialogHeight:300px;center:yes;help:no;resizable:yes;scroll:no;status:no";

		if (strType == "RESOURCES")//对于资源来说，要求的资源显示界面更大一些
			sFeature = "dialogWidth:360px; dialogHeight:320px;center:yes;help:no;resizable:yes;scroll:no;status:no";

		var arg = new Object();

		arg.objID = nID;
		arg.Guid = Guid;
		arg.op = strOP;
		arg.appID = OrgGuid;
		arg.appLevel = strAppLevel;
		arg.type = strType;
		arg.displayname = strDisplayname;
		arg.codeName = strCodeName;
		arg.description = strDescription;
		if (typeof(event.srcElement.canDelete) != "undefined")
			arg.disabled = !event.srcElement.canDelete;
		else
			arg.disabled = false;

		var sPath = "../dialogs/editAppObjects.htm";

		return showModalDialog(sPath, arg, sFeature);		
	}
	
	function getConfirmText()
	{
		var objName = m_objParam.DisplayName;
		var objSort = m_objParam.OriginalSort;
		var objType = "";
		
		if(objSort.length == 8)
			objType = "应用";
		else objType = "应用操作类型";
		
		return "是否删除该" + objType + ": " + objName + "?"; 
	}
	
	function deleteSelectedObjects()
	{
		var xmlDoc = createDomDocument("<Delete/>");
		var root = xmlDoc.documentElement;
		root.setAttribute("orgSort", m_objParam.OriginalSort);
		root.setAttribute("Guid", m_objParam.Guid);
		root.setAttribute("DisplayName", m_objParam.DisplayName);
		var xmlResult = xmlSend("../server/ServerLog.aspx", xmlDoc);
		checkErrorResult(xmlResult);
	}
	
	function onInputParamChange()
	{
		if ((event.propertyName == "value") && (inputParam.value.length > 0))
		{
			m_nCheckItemCount = 0;

			buildParam(inputParam.value);
			setNewObjStyle(m_objParam);
			
			var headXmlDocument = getHeadXmlDoc();
			fillGridCaption(titleCaption, type, headXmlDocument);

			adminDbGrid.dataXML = queryData();
			inputParam.value = "";
		}
	}
		
	function onSelectClick()
	{
		var appSelectName = event.srcElement.value;
		var xmlDoc = createDomDocument("<GetSelectList/>");
		var root = xmlDoc.documentElement;
		root.setAttribute("appDisplayname", appSelectName);
		
		var xmlResult = xmlSend("../server/ServerLog.aspx", xmlDoc);
		checkErrorResult(xmlResult);
		
		var opNameListNode = xmlResult.documentElement.firstChild;
		setSelectValuesByXml(opNameListNode, frmInput.opTypeName, "DISPLAYNAME", "DISPLAYNAME", false);
	}
//-->