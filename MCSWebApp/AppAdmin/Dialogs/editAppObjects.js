<!--
	var m_xmlDict = null;
	var m_objParam = null;
	var m_inOnLoad = true;//在窗体载入过程中
	function onSaveClick()
	{
		try
		{
			trueThrow(m_objParam.disabled, "对不起，这里不能作修改！");

			checkInputNull();

			var xmlDoc = null;

			if (m_objParam.op == "insert")
				xmlDoc = getInsertData(xmlDoc);
			else
				xmlDoc = getUpdateData(xmlDoc);

			var objNode = xmlDoc.documentElement.firstChild;
			while (objNode)
			{
				appendAttr(xmlDoc, objNode, "parentLevel", m_objParam.appResLevel);
				objNode = objNode.nextSibling;
			}

			var xmlResult = xmlSend("../XmlRequestService/XmlAOSWriteRequest.aspx", xmlDoc);
						
			checkErrorResult(xmlResult);
			
			
			if (m_objParam.type == "ROLES" || m_objParam.type == "FUNCTIONS")
				appendNode(xmlResult, xmlResult.documentElement, "RESOURCE_LEVEL", m_objParam.appResLevel);

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
	
	function getUpdateData(xmlDoc)
	{
		xmlDoc = createUpdateDoc(frmInput.elements, m_objParam.type, "ID", "=", originalValue(frmInput.objID));
			
		if (m_objParam.type == "RESOURCES")
			appendAttr(xmlDoc, xmlDoc.documentElement, "appCodeName", m_objParam.appCodeName);

		trueThrow(xmlDoc.documentElement.selectSingleNode(".//SET").childNodes.length == 0, "closeWindow");				
		
//		if (m_objParam.type != "APPLICATIONS" && m_objParam.type != "RESOURCES")
//			appendNode(xmlDoc, xmlDoc.documentElement.firstChild.selectSingleNode("WHERE"), "APP_ID", originalValue(frmInput.APP_ID));
		
		return xmlDoc;
	}
	
	function getInsertData(xmlDoc)
	{
		xmlDoc = createInsertDoc(frmInput.elements, m_objParam.type);
		if (xmlDoc != null && xmlDoc.documentElement.selectSingleNode(".//DESCRIPTION") == null)
			appendNode(xmlDoc, xmlDoc.documentElement.selectSingleNode(".//SET"), "DESCRIPTION");

		if (xmlDoc && xmlDoc.documentElement.firstChild.nodeName == "APPLICATIONS")
		{
			//add application
			appendAttr(xmlDoc, xmlDoc.documentElement.firstChild, "parentID", m_objParam.appID);
			if (xmlDoc != null && xmlDoc.documentElement.selectSingleNode(".//APP_ID") != null)
				xmlDoc.documentElement.firstChild.firstChild.removeChild( xmlDoc.documentElement.selectSingleNode(".//APP_ID") );
		}	
		
		return xmlDoc;
	}
	
	/*function getPublicDiv(xmlDoc)
	{
		if (m_objParam.appLevel.length == 4 && (m_objParam.type == "ROLES" || m_objParam.type == "FUNCTIONS"))
		{
			var root = xmlDoc.documentElement;
			if (frmInput.publicRF.checked == true)
			{
				appendNode(xmlDoc, root.firstChild.firstChild, "ORIGINAL_ID", "1");
				root.selectSingleNode(".//APP_ID").text = "1";
			}
			else if (frmInput.privateRF.checked == true)
				appendNode(xmlDoc, root.firstChild.firstChild, "ORIGINAL_ID", "1");
		}
	}*/
	
	/*function getResourceCopyUsers(xmlDoc)
	{
		if (m_objParam.type == "RESOURCES")
		{
			var inheritNode = xmlDoc.documentElement.selectSingleNode(".//INHERIT");
			if (inheritNode && inheritNode.text == "n")
			{
				var bCopyRoleUser = confirm("当前资源权限信息不继承，请确认复制人员的权限信息！");
				if (bCopyRoleUser)
					appendAttr(xmlDoc, inheritNode, "copyRoleUser", "y");
				else
					appendAttr(xmlDoc, inheritNode, "copyRoleUser", "n");
			}
		}
	}*/
	
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
		//'通用授权'应用
		if (m_objParam.fatherNodeType == C_NODE_TYPE_B71)
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
		//一般应用
		if (m_objParam.fatherNodeType == C_NODE_TYPE_B72)
		{
			 if (parseInt(getSingleNodeText(xmlDoc, "//CHILDREN_COUNT", "")) > 0)
				frmInput.addSubapp.disabled = true;
		}
		
		frmInput.btnOK.disabled = m_objParam.disabled;
		
	}

	function getAppObjectDetail()
	{
		
		xmlResult = queryObj( m_objParam.type, m_objParam.objID, m_objParam.appID );
		
		checkErrorResult(xmlResult);
		
		
		xmlResultFillForm(xmlResult.selectSingleNode(".//Table"), frmInput.elements, m_xmlDict.XMLDocument);
		
		var appIDNode = xmlResult.selectSingleNode(".//APP_ID");
		if (appIDNode)
			frmInput.btnOK.disabled = (m_objParam.disabled || (parseInt(appIDNode.text) != parseInt(m_objParam.appID)));
		
		
		var node = xmlResult.selectSingleNode(".//INHERITED_STATE");

		if (node)
		{
			var iState = parseInt(node.text);
			frmInput.cbScope.checked	= ( (iState&1) > 0);
			frmInput.cbRole.checked		= ( (iState&2) > 0);
			frmInput.cbFunction.checked	= ( (iState&4) > 0);
			frmInput.cbRTF.checked		= ( (iState&8) > 0);
			frmInput.cbObject.checked	= ( (iState&16) > 0);
		}

		node = xmlResult.selectSingleNode(".//ADD_SUBAPP");
		if (node)
			frmInput.addSubapp.checked	= (node.text == "y");
		 
		node = xmlResult.selectSingleNode(".//USE_SCOPE");
		if (node)
			frmInput.useScope.checked	= (node.text == "y");

		node = xmlResult.selectSingleNode(".//INHERITED");
		if (node)
			frmInput.INHERITED.checked	= (node.text == "y");

		frmInput.cbRTF.disabled		= !(frmInput.cbRole.checked && frmInput.cbFunction.checked)
		frmInput.cbObject.disabled	= !frmInput.cbRole.checked;

		setInterface(xmlResult);
	}

	function loadXmlDict()
	{
		if (m_xmlDict == null)
		{
			m_xmlDict = document.createElement("XML");
			document.body.insertBefore(m_xmlDict);
			//决定选用的数据字典
			switch (m_objParam.type)
			{
				case "SCOPES":
										m_xmlDict.src ="../xsd/Scopes.xsd";
										break;
				case "ROLES":
										m_xmlDict.src ="../xsd/roles.xsd";
										break;
				case "FUNCTIONS":
										m_xmlDict.src ="../xsd/functions.xsd";
										break;
				case "FUNCTION_SETS":
										m_xmlDict.src ="../xsd/function_sets.xsd";
										break;
				case "APPLICATIONS":
										m_xmlDict.src ="../xsd/applications.xsd";
										break;
			}
			//m_xmlDict.onreadystatechange = onXmlLoad;//delete by yuanyong 20071207
			if (m_xmlDict.readyState == "complete")// add by yuanyong 20071207
				onXmlLoad();// add by yuanyong 20071207
			else// add by yuanyong 20071207
				m_xmlDict.onreadystatechange = onXmlLoad;// add by yuanyong 20071207
		}
	}

	function onXmlLoad()
	{
		try
		{
			//if (m_xmlDict.readyState == "interactive")//delete by yuanyong 20071207
			if (m_xmlDict.readyState == "interactive" || m_xmlDict.readyState == "complete")//add by yuanyong 20071207
			{
				initElementsByDict(m_xmlDict.XMLDocument, frmInput);
				
				if (m_objParam && m_objParam.op == "update")
					getAppObjectDetail();
				else if (m_objParam.appID)
						frmInput.APP_ID.value = m_objParam.appID
					
			}
		}
		catch(e)
		{
			showError(e);
		}
		finally
		{
			m_inOnLoad = false;
		}
	}

	function onDocumentLoad()
	{
		try
		{
			m_inOnLoad = true;
			m_objParam = window.dialogArguments;
			window.returnValue = false;
			initDocumentEvents(frmInput);

			if (m_objParam)
			{
				//if (m_objParam.type == "MANAGED_APP")
				//	m_objParam.type = "APPLICATIONS";

				topCaption.innerText = getNameFromType(getTypeFromFather(m_objParam.fatherNodeType)) + "编辑";
				logoSpan.style.backgroundImage = "url(../images/32/" + getImgFromType(m_objParam.fatherNodeType) + ")";
				
				setDataSrc(frmInput, m_objParam.type);
				
				if (m_objParam.op == "insert")
				{
					if (m_objParam.fatherNodeType != C_NODE_TYPE_A1)//根结点
					{
						frmInput.cbScope.checked	= true;
						frmInput.cbRole.checked		= true;
						frmInput.cbFunction.checked	= true;
						frmInput.cbRTF.checked		= true;
						frmInput.cbObject.checked	= true;
						
						formatNumberObj(frmInput.INHERITED_STATE, STD_NUMBER_FORMAT, getInteritedState());
					}
					frmInput.INHERITED.checked	= true;
					//frmInput.addSubapp.checked	= true;
					frmInput.useScope.checked	= true;

				}				
				initInterface();
				loadXmlDict();
			}
		}
		catch(e)
		{
			showError(e);
		}
	}
	
	function initInterface()
	{
	
			inheritDiv.style.display		= "inline";
			inheritedStateDiv.style.display = "none";
			delegationDiv.style.display		= "none";
			lowestDiv.style.display			= "none";
			subAppDiv.style.display			= "none";
			scopeDiv.style.display			= "none";
			switch (m_objParam.type)
			{
				case "APPLICATIONS":
										inheritDiv.style.display = "none";
										if (m_objParam.op == "insert" && m_objParam.appResLevel != "")
											inheritedStateDiv.style.display = "inline";
										if (m_objParam.op == "update" && m_objParam.appResLevel.length > 3)
											inheritedStateDiv.style.display = "inline";
										if (m_objParam.objID != C_APP_ADMIN_ID)
										{
											subAppDiv.style.display	= "inline";
											scopeDiv.style.display	= "inline";
										}
										break;
				case "ROLES":
				case "FUNCTIONS":
				case "FUNCTION_SETS":
				case "SCOPES":
										break;
			}
			
	}
/*	
	function ctrlToPrivateDiv()
	{
		if (m_objParam.appLevel.length == 4	&& (m_objParam.type == "ROLES" || m_objParam.type == "FUNCTIONS"))
		{
			if (event.srcElement.checked == true)
				privateDiv.style.display = "none";
			else if (m_objParam.type == "ROLES")
					privateDiv.style.display = "inline";
		}
	}
*/
	function setDataSrc(inputFrom, dataSrc)
	{
		with (inputFrom)
		{	
			APP_ID.dataSrc = dataSrc;
			objID.dataSrc = dataSrc;
			NAME.dataSrc = dataSrc;
			CODE_NAME.dataSrc = dataSrc;
			DESCRIPTION.dataSrc = dataSrc;
			/*
			if (m_objParam.type == "RESOURCES")
			{
				RES_TYPE.dataSrc = dataSrc;
				CONTAINER.dataSrc = dataSrc;
				INHERIT.dataSrc = dataSrc;
			}

			if (m_objParam.type == "APPLICATIONS")
			{
				CAN_DELEGATION.dataSrc = dataSrc;
				CONTAINER.dataSrc = dataSrc;
			}
			else
				delegationDiv.style.display = "none";*/
		}
	}
	
	function getInteritedState()
	{
		var iState = 0;
		if (frmInput.cbScope.checked == true && !frmInput.cbScope.disabled)
			iState = iState + parseInt(frmInput.cbScope.value);
		if (frmInput.cbRole.checked	== true && !frmInput.cbRole.disabled)
			iState = iState + parseInt(frmInput.cbRole.value);
		if (frmInput.cbFunction.checked	== true && !frmInput.cbFunction.disabled)
			iState = iState + parseInt(frmInput.cbFunction.value);
		if (frmInput.cbRTF.checked == true && !frmInput.cbRTF.disabled)
			iState = iState + parseInt(frmInput.cbRTF.value);
		if (frmInput.cbObject.checked == true && !frmInput.cbObject.disabled)
			iState = iState + parseInt(frmInput.cbObject.value);
		var value = ""+iState+"";
		return value;
	}
	
	function checkChanged()
	{
		if (m_inOnLoad) return;
		if (event.propertyName == "checked") 
		{
			switch(event.srcElement.id)
			{
				case "cbRole":
							frmInput.cbObject.disabled	= !frmInput.cbRole.checked;
							if (frmInput.cbObject.disabled)
								frmInput.cbObject.checked = false;
							frmInput.cbRTF.disabled		= !(frmInput.cbRole.checked && frmInput.cbFunction.checked)
							if(frmInput.cbRTF.disabled)
								frmInput.cbRTF.checked = false;
							break;
				case "cbFunction":
							frmInput.cbRTF.disabled		= !(frmInput.cbRole.checked && frmInput.cbFunction.checked)
							if(frmInput.cbRTF.disabled)
								frmInput.cbRTF.checked = false;
							break;
			}
			formatNumberObj(frmInput.INHERITED_STATE, STD_NUMBER_FORMAT, getInteritedState());
		}
	}
//-->