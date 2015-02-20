
<!--
	var m_xmlDict = null;
	var m_objParam = null;

	function onSaveClick()
	{
		try
		{
			trueThrow(m_objParam.disabled, "对不起，这里不能作修改！");
			trueThrow(frmInput.NAME.value == "", "名称输入不得为空");
			trueThrow(frmInput.CODE_NAME.value == "", "英文标识输入不得为空");

			checkInputNull();
			
			var xmlDoc = null;

			if (m_objParam.op == "insert")
				xmlDoc = getInsertData();
			else
				xmlDoc = getUpdateData();

			var objNode = xmlDoc.documentElement.firstChild;
			while (objNode)
			{
				appendAttr(objNode, "appGuid", m_objParam.Guid);
				appendAttr(objNode, "strSort",  m_objParam.appLevel);
				objNode = objNode.nextSibling;
			}
			

			var xmlResult = xmlSend("../server/ServerLog.aspx", xmlDoc);
						
			checkErrorResult(xmlResult);
			
			if (m_objParam.type == "ROLES" || m_objParam.type == "FUNCTIONS")
				appendNode(xmlResult.documentElement, "RESOURCE_LEVEL", m_objParam.appLevel);

			window.returnValue = xmlResult;
			window.close();
		}
		catch(e)
		{
			if (typeof(e) != "object" && e == "closeWindow")
				window.close();
				
			showError(e);
		}
	}
	
	function getUpdateData()
	{
		var tableName = "";
		if (m_objParam.appLevel.length == 8)
			tableName = "APP_LOG_TYPE";
		else 
			tableName = "APP_OPERATION_TYPE";
			
		var xmlDoc = createUpdateDoc(frmInput.elements, tableName, "GUID", "=", m_objParam.Guid);
			
		appendNode(xmlDoc.documentElement.firstChild.selectSingleNode("WHERE"), "APP_ID", originalValue(frmInput.APP_ID));
		
		return xmlDoc;
	}
	
	function getInsertData()
	{
		var xmlDoc = createInsertDoc(frmInput.elements, m_objParam.type);
		if (xmlDoc != null && xmlDoc.documentElement.selectSingleNode(".//DESCRIPTION") == null)
			appendNode(xmlDoc.documentElement.selectSingleNode(".//SET"), "DESCRIPTION");
		
		return xmlDoc;
	}

	function selectType()
	{
		if (event.srcElement.checked == true)
		{
			frmInput.RES_TYPE.value = event.srcElement.value;
			frmInput.RES_TYPE.originalValue = event.srcElement.value;
		}
	}

	function onCodeNameKeyPress()
	{
		if (event.keyCode >= 97 && event.keyCode <= 122)
			event.keyCode -= 32;

		if ((event.keyCode > 57 && event.keyCode < 65) ||
			(event.keyCode < 48) ||
			(event.keyCode > 90 && event.keyCode != 95) )
		{
			event.keyCode = 0;
		}
	}

	function onCodeNameChange()
	{
		event.returnValue = true;
		var obj = event.srcElement;

		var strValue = obj.value.toUpperCase();

		for (var i = 0; i < strValue.length; i++)
		{
			var str = strValue.substr(i, 1);

			var bCheck = (str >= "A" && str <= "Z") || (str >= "0" && str <= "9") ||
							(str == "_");

			if (bCheck == false)
			{
				alert("英文标识必须是字母、数字和下划线，并且不能含有空格");
				event.returnValue = false;
				break;
			}	
		}

		if (event.returnValue)
			obj.value = obj.value.toUpperCase();

		return event.returnValue;
	}

	function setInterface(xmlDoc)
	{
		if (m_objParam.appID == 1)
		{
			for (var i = 0; i < frmInput.elements.length; i++)
			{
				var obj = frmInput.elements(i);

				if (obj.tagName == "INPUT" && obj.type != "button" && obj.type != "hidden")
				{
					obj.readOnly = true;
					obj.className = "readOnly";
				}
			}

			frmInput.btnOK.disabled = true;
			frmInput.btnCancel.focus();
		}
	}

	function onDocumentLoad()
	{
		try
		{
			m_objParam = window.dialogArguments;
			window.returnValue = false;
			initDocumentEvents(frmInput);

			if (m_objParam)
			{
				topCaption.innerText = "应用" + (m_objParam.appLevel.length == 12 ? "操作" : "") + "类型：" + m_objParam.displayname;
							
				logoSpan.style.backgroundImage = "url(../images/" + getImgFromClass(m_objParam.type) + ")";
				
				if (m_objParam.op == "update")
				{
					frmInput.NAME.value = m_objParam.displayname;
					frmInput.CODE_NAME.value = m_objParam.codeName ;
					frmInput.DESCRIPTION.value = m_objParam.description;
				}
				setDataSrc(frmInput, m_objParam.appLevel);
			}
		}
		catch(e)
		{
			showError(e);
		}
	}

	function setDataSrc(inputFrom, sort)
	{
		if ((sort.length == 8 && m_objParam.op == "update") || (sort.length == 4 && m_objParam.op == "insert"))
		{
			with (inputFrom)
			{	
				NAME.dataSrc = "APP_LOG_TYPE";
				CODE_NAME.dataSrc = "APP_LOG_TYPE";
				DESCRIPTION.dataSrc = "APP_LOG_TYPE";
			}
		}
	}
	
//-->