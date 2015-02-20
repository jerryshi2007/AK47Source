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
				
			//alert(xmlDoc.xml);

			var xmlResult = xmlSend("../XmlRequestService/XmlAOSWriteRequest.aspx", xmlDoc);
						
			checkErrorResult(xmlResult);
			
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
		
		
		return xmlDoc;
	}
	
	function getInsertData(xmlDoc)
	{
		/***************************************************************
		//文档格式
			<Insert>
				<SCOPES>
					<SET>
						<EXPRESSION>curDepartScope(curUserId)</EXPRESSION>
						<APP_ID>ff6332da-4654-45cf-9247-97b5b60998ab</APP_ID>
						<NAME>本部门</NAME>
						<DESCRIPTION>本部门</DESCRIPTION>
						<INHERITED>n</INHERITED>
						<CLASSIFY>Y</CLASSIFY>
					</SET>
				</SCOPES>
			</Insert>
		***************************************************************/

		xmlDoc = createInsertDoc(frmInput.elements, m_objParam.type);
		if (xmlDoc != null && xmlDoc.documentElement.selectSingleNode(".//DESCRIPTION") == null)
			appendNode(xmlDoc, xmlDoc.documentElement.selectSingleNode(".//SET"), "DESCRIPTION");

			appendNode(xmlDoc, xmlDoc.documentElement.firstChild.firstChild, "INHERITED", "n");
			appendNode(xmlDoc, xmlDoc.documentElement.firstChild.firstChild, "CLASSIFY", "y");
		
		return xmlDoc;
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
				//topCaption.innerText = getNameFromType(getTypeFromFather(m_objParam.fatherNodeType)) + "编辑";
				//logoSpan.style.backgroundImage = "url(../images/32/" + getImgFromType(m_objParam.fatherNodeType) + ")";
				
				//setDataSrc(frmInput, m_objParam.type);
				
				if (m_objParam.op == "insert")
				{
					frmInput.chkCurDepart.checked	= true;
					frmInput.APP_ID.value		= m_objParam.appID;
					frmInput.EXPRESSION.value	= "curDepartScope(curUserId)";
					frmInput.NAME.value			= "本部门";
					frmInput.DESCRIPTION.value	= "本部门";
					frmInput.btnDefine.disabled	= true;
				}				
				loadXmlDict();
			}
		}
		catch(e)
		{
			showError(e);
		}
	}

	function loadXmlDict()
	{
		if (m_xmlDict == null)
		{
			m_xmlDict = document.createElement("XML");
			document.body.insertBefore(m_xmlDict);
			m_xmlDict.src ="../xsd/Scopes.xsd";
			
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
//				else if (m_objParam.appID)
//						frmInput.APP_ID.value = m_objParam.appID
					
			}
		}
		catch(e)
		{
			showError(e);
		}
		finally
		{
			//初始化完成
			m_inOnLoad = false;
		}
	}

	function getAppObjectDetail()
	{
		
		xmlResult = queryObj( m_objParam.type, m_objParam.objID, m_objParam.appID );
		
		checkErrorResult(xmlResult);
		
		
		xmlResultFillForm(xmlResult.selectSingleNode(".//Table"), frmInput.elements, m_xmlDict.XMLDocument);
		
		var node = xmlResult.selectSingleNode(".//EXPRESSION");

		if (node)
		{
			var Exp = node.text;
			//Exp =
		}
	}
	
	function checkChanged()
	{
		if (m_inOnLoad) return;
		if (event.propertyName == "checked") 
		{
			if (event.srcElement.checked == true)
			{
				switch(event.srcElement.id)
				{
					case "chkCurDepart":
								frmInput.EXPRESSION.value	= "curDepartScope(curUserId)";
								frmInput.NAME.value			= "本部门";
								frmInput.DESCRIPTION.value	= "本部门";
								frmInput.btnDefine.disabled	= true;
								break;
					case "chkCurCustoms":
								frmInput.EXPRESSION.value	= "curCustomsScope(curUserId)";
								frmInput.NAME.value			= "本关区";
								frmInput.DESCRIPTION.value	= "本关区";
								frmInput.btnDefine.disabled	= true;
								break;
					case "chkUserDefine":
								if(m_objParam.op == "insert")
								{
									frmInput.EXPRESSION.value	= "";
									frmInput.NAME.value			= "";
									frmInput.DESCRIPTION.value	= "";
									frmInput.btnDefine.disabled	= false;
								}
								else
								{
									frmInput.EXPRESSION.value	= "";
									frmInput.NAME.value			= "";
									frmInput.DESCRIPTION.value	= "";
									frmInput.btnDefine.disabled	= false;
								}
								break;
				}
			}
		}
	}
	
	function btnDefineClick()
	{
       
        var arg = new Object();
        arg.listObjType = 1;         //1=OU, 2=User, 4=Group
        arg.multiSelect = 0;//（1、允许多选；0、不允许多选；）
        //arg.rootOrg = C_OGU_ROOT_NAME;//设置选择最大范围
        arg.rootOrg = C_DC_ROOT_NAME;//设置选择最大范围
        arg.canSelectRoot = "true";
        var xmlResult = showSelectOUScopeDialog(arg);
        
		/*********************************************************
			//返回信息的格式
			<NodesSelected ACCESS_LEVEL="">
				<object 
					OBJECTCLASS="ORGANIZATIONS" 
					POSTURAL="" 
					RANK_NAME=""
					STATUS="1" 
					ALL_PATH_NAME="中国海关\01海关总署\" 
					GLOBAL_SORT="000000000000" 
					ORIGINAL_SORT="000000000000" 
					DISPLAY_NAME="海关总署" 
					OBJ_NAME="01海关总署" 
					LOGON_NAME="" 
					PARENT_GUID="e588c4c6-4097-4979-94c2-9e2429989932" 
					GUID="567e75f7-59b9-477b-9053-9772bc30eae5"
				/>
			</NodesSelected>
		************************************************************/
		if (xmlResult)
		{
			if (typeof(xmlResult) == "string")
				xmlResult = createDomDocument(xmlResult);
		
			//alert(xmlResult.xml);
			var root = xmlResult.documentElement;	
			if(root.childNodes.length > 1)
			{
				alert("不可多选");
				return;
			}
			
			if (getNodeAttribute(root.firstChild, "OBJECTCLASS") != "ORGANIZATIONS")
			{
				alert("只能选择'组织机构'作为机构服务范围！");
				return;
			}
			
			frmInput.NAME.value			= getNodeAttribute(root.firstChild, "DISPLAY_NAME");
			frmInput.DESCRIPTION.value	= getNodeAttribute(root.firstChild, "ALL_PATH_NAME");
			frmInput.EXPRESSION.value	= "userDefineScope(\""+getNodeAttribute(root.firstChild, "GUID")+"\")";
		}
		
	}
	
//-->