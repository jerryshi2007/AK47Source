<!--
	var m_objParam = null; //��¼����ҳ��Ĳ���
	var m_xmlDict = null;
	
	function onDocumentLoad()
	{
		try
		{
			m_objParam = window.dialogArguments; //�����а���type, appID, appResLevel,classify, inherited, disabled, op,reslevel,
			//id--
			
			/////////programming test data; begin
			/*if (m_objParam == null)
				m_objParam = new Object();
			m_objParam.type = "ROLES";
			m_objParam.appID = "9f586e6a-5ee2-4cd8-bbf3-2e09c296024f";
			//m_objParam.appResLevel = "003";
			m_objParam.resLevel = ""; //�����ϵ�resource_level
			m_objParam.classify = "n";
			m_objParam.inherited = "n";
			m_objParam.disabled = false;
			m_objParam.op = "insert";
			m_objParam.id = "";*/
			///////test data end.
			
			window.returnValue = false;
			initDocumentEvents(frmInput);
			
			if (m_objParam)
			{
				topCaption.innerText = getNameFromType(m_objParam.type) + "�༭";
				logoSpan.style.backgroundImage = "url(../images/32/" + getImageFromClass(m_objParam.type) + ")";
				
				setDataSrc(frmInput, m_objParam.type);		//���ø���input��datasrc��ֵ
				
				initInterface();
				loadXmlDict();				
			}
		}
		catch(e)
		{
			showError(e);
		}
	}
	
	//���ý������ʾ��Ϣ
	function initInterface()
	{
		//����ǽ�ɫ����ʾ����ί�ɵ�ѡ��
		if (m_objParam.type == "ROLES") 
			delegateSpan.style.display = "inline";				
		else
			delegateSpan.style.display = "none";									
		
		//�����Ƿ���Ҷ�ӹ��ܼ���
		if (m_objParam.type == "FUNCTION_SETS")
			leafSpan.style.display = "inline";
		else
			leafSpan.style.display = "none";
			
		inheritedSpan.style.display = "none";
		
		//���ü̳е�ֵ					
		//if (m_objParam.inherited == "1")		
		//	frmInput.INHERITED.checked = true;		
		//else		
		//	frmInput.INHERITED.checked = false;						
		
		//�̳��Ƿ��ǿ�ѡ��
		//if (m_objParam.op == "insert")
		//	frmInput.INHERITED.disabled = true;
		//else
		//	frmInput.INHERITED.disabled = false;				
	}
	
	
	//----------------------------------
	function loadXmlDict()
	{
		if (m_xmlDict == null)
		{
			m_xmlDict = document.createElement("XML");
			document.body.insertBefore(m_xmlDict);

			m_xmlDict.src ="../xsd/" + m_objParam.type + ".xsd";
			
			
			//m_xmlDict.onreadystatechange = onXmlLoad; //delete by yuanyong 20071207
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
				else if (m_objParam.appID && m_objParam.type != "APPLICATIONS")
				{
					frmInput.APP_ID.value = m_objParam.appID;
					frmInput.CLASSIFY.value = m_objParam.classify;		
					
					if (m_objParam.type == "FUNCTION_SETS")
						frmInput.RESOURCE_LEVEL.value = m_objParam.resLevel;			
				}
			}
		}
		catch(e)
		{
			showError(e);
		}
	}
	
	function getAppObjectDetail()
	{
		var xmlDoc = createDomDocument("<getObjInfo />");
			
		appendAttr(xmlDoc, xmlDoc.documentElement, "type", m_objParam.type);
		
		//if (m_objParam.appID.Length > 0)
		//	appendAttr(xmlDoc, xmlDoc.documentElement, "app_id", m_objParam.appID);
		
		if (m_objParam.id )
			appendAttr(xmlDoc, xmlDoc.documentElement, "id", m_objParam.id);
			
		var xmlResult = xmlSend("../XmlRequestService/XmlReadRequest.aspx", xmlDoc); 
		
		checkErrorResult(xmlResult);
		
		xmlResultFillForm(xmlResult.selectSingleNode(".//Table"), frmInput.elements, m_xmlDict.XMLDocument);
		
		var appIDNode = xmlResult.selectSingleNode(".//APP_ID");
		if (appIDNode)
			frmInput.btnOK.disabled = (m_objParam.disabled || (appIDNode.text != m_objParam.appID));		
		
		var nodeDelegate = xmlResult.selectSingleNode(".//ALLOW_DELEGATE");
		if (nodeDelegate)
			frmInput.ALLOW_DELEGATE.checked = (nodeDelegate.text == "y");
			
		setInterface(xmlResult);
	}
	
	function setInterface(xmlDoc)
	{
		if (m_objParam.appID == "11111111")
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
	
	//���ø�control��datasrc
	function setDataSrc(inputForm, dataSrc) 
	{
		with(inputForm)
		{
			APP_ID.dataSrc = dataSrc;
			ID.dataSrc = dataSrc;
			CLASSIFY.dataSrc = dataSrc;
			
			NAME.dataSrc = dataSrc;
			CODE_NAME.dataSrc = dataSrc;
			DESCRIPTION.dataSrc = dataSrc;
			//INHERITED.dataSrc = dataSrc;
			//RESOURCE_LEVEL.dataSrc = dataSrc;
			//ALLOW_DELEGATE.dataSrc = dataSrc;						
		}
	}
		
	function onSaveClick()
	{
		try
		{
			trueThrow(m_objParam.disabled, "�Բ������ﲻ�����޸ģ�");
			
			checkInputNull();
			
			var xmlDoc = null;
			
			if (m_objParam.op == "insert")
				xmlDoc = getInsertData(xmlDoc);
			else
				xmlDoc = getUpdateData(xmlDoc);
				
			var xmlResult = xmlSend("../XmlRequestService/XmlRFDWriteRequest.aspx", xmlDoc);
			
			checkErrorResult(xmlResult);
			
//			if (m_objParam.type == "ROLES" || m_objParam.type == "FUNCTIONS")
//				appendNode(xmlResult, xmlResult.documentElement, "RESOURCE_LEVEL", m_objParam.appResLevel);
				
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
	
	function getInsertData(xmlDoc)
	{
		xmlDoc = createInsertDoc(frmInput.elements, m_objParam.type);
		
		if (xmlDoc != null && xmlDoc.documentElement.selectSingleNode(".//DESCRIPTION") == null)
			appendNode(xmlDoc, xmlDoc.documentElement.selectSingleNode(".//SET"), "DESCRIPTION");
			
		//if (xmlDoc != null && xmlDoc.documentElement.selectSingleNode(".//RESOURCE_LEVEL") == null)
			//appendNode(xmlDoc, xmlDoc.documentElement.selectSingleNode(".//SET"), "RESOURCE_LEVEL");
		
		return xmlDoc;
	}
	
	function getUpdateData(xmlDoc)
	{
		xmlDoc = createUpdateDoc(frmInput.elements, m_objParam.type, "ID", "=", originalValue(frmInput.ID));
		
		if (m_objParam.type == "RESOURCES")
			appendAttr(xmlDoc, xmlDoc.documentElement, "appCodeName", m_objParam.appCodeName);
			
		trueThrow(xmlDoc.documentElement.selectSingleNode(".//SET").childNodes.length == 0, "closeWindow");
		
		return xmlDoc;
	}
	
	function onCodeNameKeyPress()
	{
		if (event.keyCode >= 97 && event.keyCode <= 122)
			event.keyCode -= 32;
		
		if ((event.keyCode > 57 && event.keyCode < 65) || 
			(event.keyCode < 48) ||
			(event.keyCode > 90 && event.keyCode != 95))
			event.keyCode = 0;
	}
	
	function onCodeNameChange()
	{
		event.returnValue = true;
		var obj = event.srcElement;
		
		var strValue = obj.value.toUpperCase();
		
		for ( var i = 0; i < strValue.length; i++)
		{
			var str = strValue.substr(i,1);
			
			var bCheck = (str >="A" && str <= "Z") || (str >= "0" && str <= "9") || (str =="_");
			
			if (bCheck == false)
			{
				alert("Ӣ�ı�ʶ��������ĸ�����ֺ��»��ߣ����Ҳ��ܺ��пո�");
				event.returnValue = false;
				break;
			}
		}
		
		if (event.returnValue)
			obj.value = obj.value.toUpperCase();
		
		return event.returnValue;
	}
	
	//ȡ�ñ��༭���������
	function getNameFromType(strClass)
	{
		var strName = "";

		switch(strClass)
		{
			case "FUNCTIONS":
							strName = "����";
							break;
			case "FUNCTION_SETS":
							strName = "���ܼ���";
							break;
			case "APPLICATIONS":
							strName = "Ӧ�ó���";
							break;
			case "RESOURCES":
							strName = "��Դ";
							break;
			case "ROLES":	strName = "��ɫ";
							break;
			case "USERS_TO_ROLES":
							strName = "�û������";
							break;
		}

		return strName;
	}

	//�������Ͳ�ͬȡ��ͬ��ͼ��
	function getImageFromClass(strClass)
	{
		var imgName = "";
		switch(strClass)
		{
			case "group":	imgName = "group.gif";
							break;
			case "person":
			case "CN":
			case "USER":	imgName = "user.gif";
							break;
			case "userAccountControl"://�˺Ž���
							imgName = "accountDisabled.gif";
							break;
			case "DC":
			case "dc":
			case "domain":
			case "DOMAIN":	imgName = "domain.gif";
							break;
			case "organizationalUnit":
			case "OU":		imgName = "ou.gif";
							break;
			case "role":
			case "ROLE":
			case "ROLES":
			case "ROLES_TO_FUNCTIONS":
							imgName = "role.gif";
							break;
			case "FUNCTIONS":
			case "FUNCTION_SETS":
							imgName = "function.gif";
							break;
			case "MANAGED_APP":
			case "APPLICATIONS":
			case "RESOURCES":
							imgName = "application.gif";
							break;
			case "ResourceItem":
							imgName = "ResourceItem.gif";
							break;
			case "ResourceFolder":
							imgName = "ResourceFolder.gif";
							break;
			case "ResourceFile":
							imgName = "ResourceFile.gif";
							break;
		}

		return imgName;
	}
	
	function onNameChange()
	{
	}		

//-->
