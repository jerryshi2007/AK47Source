
	var m_sourceID = ""; //delegation source User
	var m_sourceLogonName = "";
	var m_bLoadFirst = false;
	
	var m_lastAppIndex = -1; 
	var m_lastRoleIndex = -1;
	
	function onDocumentLoad()
	{
		try
		{
			initDocumentEvents();
			initElementsByDict(DF_XSD.XMLDocument);
			
			if (!m_bLoadFirst)
			{
				bindCalendarToInput(hCalendar, START_TIME);
				bindCalendarToInput(hCalendar, END_TIME);
				hCalendar.attachEvent("ondateclick", onDateClick);
				
				START_TIME.attachEvent("onchange", onDateChange);
				END_TIME.attachEvent("onchange", onDateChange);
				
				m_bLoadFirst = true;
			}
			
			initDate("", ""); //
			
			if (SOURCE_ID.value.length > 0)
			{
				m_sourceID = SOURCE_ID.value;
				m_sourceLogonName = SOURCE_LOGON_NAME.value;
				
				//loadAppData(m_sourceLogonName, idType.value, appID.value); //idType表示appID是ID还是CODE_NAME
				loadAppData(m_sourceLogonName);
				
				if (appID.value.length > 0)
					appSelect.value = appID.value;
					
				if (appSelect.selectedIndex == -1 && appSelect.options.length > 0)
					appSelect.selectedIndex = 0;
					
				if (appSelect.selectedIndex != -1)
					fillAppData(); //----------------
					
				m_lastAppIndex = appSelect.selectedIndex;
				appSelect.disabled = false;
				appSelect.style.backgroundColor = "#ffffff";
				OGUInput.disabled = false;
				//disabledDate(false);
			}
			else
			{
				appSelect.disabled = true;
				appSelect.style.backgroundColor = "#dddddd";
				OGUInput.disabled = true;
				disabledDate(true);
			}
		}
		catch(e)
		{
			showError(e);
		}
	}
	
	function initDate(startTime, endTime)
	{
		START_TIME.value = startTime;
		END_TIME.value = endTime;
		
		START_TIME.oldValue = startTime;
		END_TIME.oldValue = endTime;
	}
	
	function disableDate(bDisabled, startTime, endTime)
	{
		START_TIME.disabled = bDisabled;
		END_TIME.disabled = bDisabled;
		
		if (bDisabled)
		{
			initDate("", "");
			START_TIME.style.backgroundColor = "#dddddd";
			END_TIME.style.backgroundColor = "#dddddd";
		}
		else
		{
			if (startTime && endTime)
				initDate(startTime, endTime);
			else
			{
				var nowStr = dateToStr(new Date());
				initDate(nowStr, nowStr);
			}
			
			START_TIME.style.backgroundColor = "#ffffff";
			END_TIME.style.backgroundColor = "#ffffff";
		}
	}
	
	function dateToStr(dateObj)
	{
		return dateObj.getFullYear() + "-" + ((dateObj.getMonth()+1) > 9 ? (dateObj.getMonth()+1) : "0" + (dateObj.getMonth()+1)) + "-" + (dateObj.getDate() > 9 ? dateObj.getDate() : "0" + dateObj.getDate());
	}
	
	
	function fillAppData()
	{
		m_lastAppIndex = appSelect.selectedIndex;
		appCodeName.value = appSelect.options[m_lastAppIndex].value;
			
		loadRoleData(m_sourceLogonName, appCodeName.value);
		
		if (roleSelect.selectedIndex == -1 && roleSelect.options.length > 0)
			roleSelect.selectedIndex = 0;
			
		if (roleSelect.selectedIndex != -1)
			fillRoleData();
			
		m_lastRoleIndex = roleSelect.selectedIndex;
	}
	
	function fillRoleData()
	{		
		var delegateID = roleSelect.options[roleSelect.selectedIndex].delegateID;
		if (delegateID)
		{
			/*var xmlDoc = createDomDocument("<getUserAllPathName />");
			xmlDoc.documentElement.setAttribute("guid", delegateID);
			
			var xmlResult = xmlSend("../XmlRequestService/XmlReadRequest.aspx", xmlDoc);
			
			checkErrorResult(xmlResult);
			
			OGUInput.value = xmlResult.documentElement.getAttribute("allPathName");
			*/
		
			OGUInput.value = delegateID;
		} 
		else
			OGUInput.value = "";
		
		if (OGUInput.value)
			disableDate(false, roleSelect.options[roleSelect.selectedIndex].START_TIME, roleSelect.options[roleSelect.selectedIndex].END_TIME);
		else
			disableDate(true);
	}

	function loadAppData(strLogonName)
	{
		var xmlDoc = createDomDocument("<getDelegationApps />");
		var root = xmlDoc.documentElement;
		
		root.setAttribute("logonName", strLogonName);
		
		var xmlResult = xmlSend("../XmlRequestService/XmlReadRequest.aspx", xmlDoc); ///get all applications that can be delegated
		
		checkErrorResult(xmlResult);
		
		userNameSpan.innerText = xmlResult.documentElement.getAttribute("displayName");
		
		//fillSelect(xmlResult, "CODE_NAME", "APP");
		fillAppSelect(xmlResult);
	}
	
	//查询某应用下的所有可被当前用户委派的角色，并组成select控件
	function loadRoleData(strLogonName, appCodeName)
	{
		var xmlDoc = createDomDocument("<getAppDelegationRoles />");
		var root = xmlDoc.documentElement;
		
		root.setAttribute("logonName", strLogonName);
		root.setAttribute("appCodeName", appCodeName);
		
		var xmlResult = xmlSend("../XmlRequestService/XmlReadRequest.aspx", xmlDoc);
		
		checkErrorResult(xmlResult);
		
		//fillSelect(xmlResult, "ID", "ROLES");
		fillRoleSelect(xmlResult);
	}
	
	/*function fillSelect(xmlReslut, strIDType, AppOrRole)
	{
		var nodeTable = xmlReslut.selectSingleNode(".//Table");
		
		var idFld = "ID";
		
		if (strIDType.toLowerCase() == "code_name")
			idFld = "CODE_NAME";
		
		if (AppOrRole == "ROLES")	
			setSelectValuesByTable(nodeTable, roleSelect, idFld, "NAME", false, callBackFill);
		else
			setSelectValuesByTable(nodeTable, appSelect, idFld, "NAME", false);
	}
	*/
	function fillAppSelect(xmlResult)
	{
		var nodeTable = xmlResult.selectSingleNode(".//Table");
		
		if (!(nodeTable))
		{
			alert("当前用户没有任何应用的角色可以委派");
			btnOK.disabled = true;
			//OGUInput.disabled = true;
			//START_TIME.disabled = true;
			//END_TIME.disabled = true;
		}
		else
		{
			btnOK.disabled = false;
		}
	
		setSelectValuesByTable(nodeTable, appSelect, "CODE_NAME", "NAME", false);
		
	}
	
	function fillRoleSelect(xmlResult)
	{
		var nodeTable = xmlResult.selectSingleNode(".//Table");
		
		if (!(nodeTable))
		{
			alert("没有角色可以委派");
			btnOK.disabled = true;
		}
		else
		{
			btnOK.disabled = false;
		}
		
		setSelectValuesByTable(nodeTable, roleSelect, "ID", "NAME", false, callBackFill);
	}
	
	function callBackFill(option, nodeRow)
	{
		option.delegateID = nodeRow.selectSingleNode("TARGET_ID").text;
		option.START_TIME = nodeRow.selectSingleNode("START_TIME").text.split(" ")[0];
		option.END_TIME = nodeRow.selectSingleNode("END_TIME").text.split(" ")[0];
	}
	
	//选择应用发生更改时的操作
	function onAppSelectChange()
	{
		try
		{
			//alert("Index:" + appSelect.selectedIndex + "\nValue:" + appSelect.options[appSelect.selectedIndex].value);
			m_lastAppIndex = appSelect.selectedIndex;
			appCodeName.value = appSelect.options[m_lastAppIndex].value;
			
			loadRoleData(m_sourceLogonName, appCodeName.value);
			
			if (roleSelect.selectedIndex == -1 && roleSelect.options.length > 0)
				roleSelect.selectedIndex = 0;
				
			if (roleSelect.selectedIndex != -1)
				fillRoleData();
			
			m_lastRoleIndex = roleSelect.selectedIndex;
		}
		catch(e)
		{
			appSelect.selectedIndex = m_lastAppIndex;
			showError(e);
		}
	}
	
	//选择角色发生更改时的操作  
	function onRoleSelectChange()
	{
		try
		{
			if (checkSave(OGUInput.getAttribute("GUID")))
			{
				m_lastRoleIndex = roleSelect.selectedIndex;
				//fillRoleData(appID.value, idType.value, roleSelect.options[roleSelect.selectedIndex].value);
				fillRoleData();
			}
			else
				roleSelect.selectedIndex = m_lastRoleIndex;
		}
		catch(e)
		{
			roleSelect.selectedIndex = m_lastRoleIndex;
			showError(e);
		}
	}
	
	function checkSave(currID)
	{
		if (!btnOK.disabled)
		{
			var alertMsg = "";
			
			if (roleSelect.options[m_lastRoleIndex].delegateID != currID)
				alertMsg += "被委派人或组织机构"; 
			else
			{
				if (roleSelect.options[m_lastRoleIndex].START_TIME != START_TIME.value
					|| roleSelect.options[m_lastRoleIndex].END_TIME != END_TIME.value)
				{
					alertMsg += "委派有效期";
				}
			}
			
			if (alertMsg)
			{
				if (confirm("您修改了" + alertMsg + "，需要保存吗？"))
				{
					//if (!saveLastChange(idType.value))
					if (!saveLastChange())
						return false;
					
					btnOK.disabled = true;
				}
			}
		}
		
		return true;
	}
	
	function onDateClick()
	{
		var oInput = hCalendar.srcInput;
		var timeType = oInput.id;
		
		var oldID;
		if (timeType == "START_TIME")		
			//oldID = roleSelect.options[m_lastRoleIndex].START_TIME.value;
			oldID = roleSelect.options[m_lastRoleIndex].START_TIME;
		else
			oldID = roleSelect.options[m_lastRoleIndex].END_TIME;
		
		if (oInput.value != oldID)
		{
			btnOK.disabled = false;
		}
	}
	
	function onDateChange()
	{
		var oInput = event.srcElement;
		var oldID = roleSelect.options[m_lastRoleIndex].attributes.getNamedItem(oInput.id).value;
		
		if(oInput.value != oldID)
		{
			btnOK.disabled = false;
		}
	}
	
	function onSaveClick()
	{
		try
		{
			//saveLastChange(idType.value);
			saveLastChange();
		}
		catch(e)
		{
			showError(e);
		}
	}
	
	//save the result
	function saveLastChange()
	{
		if (m_lastRoleIndex != -1)
		{
			if (checkDataInput())
				return false;
				
			var appCodeName = appSelect.options[m_lastAppIndex].value;
			var roleID = roleSelect.options[m_lastRoleIndex].value;
			var oldID = roleSelect.options[m_lastRoleIndex].delegateID;
			var newID = OGUInput.getAttribute("GUID");
			
			var xmlDoc = createDomDocument("<changeDelegateID />");
			
			var root = xmlDoc.documentElement;
			
			root.setAttribute("sourceID", m_sourceID);
			root.setAttribute("appCodeName", appCodeName);
			root.setAttribute("roleID", roleID);
			root.setAttribute("oldID", oldID);
			root.setAttribute("newID", newID);
			//root.setAttribute("idType", strIDType);
			
			root.setAttribute("START_TIME", START_TIME.value);
			root.setAttribute("END_TIME", END_TIME.value);
			
			var xmlResult = xmlSend("../XmlRequestService/XmlRFDWriteRequest.aspx", xmlDoc);
			
			checkErrorResult(xmlResult);
			
			//appSelect.options[m_lastIndex].delegateDN = newDN;
			//appSelect.options[m_lastIndex].START_TIME = START_TIME.value;
			//appSelect.options[m_lastIndex].END_TIME = END_TIME.value;
			roleSelect.options[m_lastRoleIndex].delegateID = newID;
			roleSelect.options[m_lastRoleIndex].START_TIME = START_TIME.value;
			roleSelect.options[m_lastRoleIndex].END_TIME = END_TIME.value;
			
			btnOK.disabled = true;
		}
		
		return true;
	}
	
	function checkDataInput()
	{
		var msg = "";
		
		//if (OGUInput.value)
		if (OGUInput.getAttribute("GUID"))
		{
			if (!START_TIME.value || !END_TIME.value)
				msg = "有效期不能有空值！";
			else
			{
				var nCompare = compareStrDate(START_TIME.value, END_TIME.value);
				if (nCompare > 0)
					msg = "有效期开始时间不能大于结束时间";
				if (nCompare == NaN)
					msg = "有效期带有非法的日期格式！";
			}
		}
		
		if (msg)
		{
			alert(msg);
			return true;
		}
		
		return false;
	}
	
	function compareStrDate(strDate1, strDate2)
	{
		try
		{
			return strToDate(strDate1) * 1 - strToDate(strDate2) * 1;
		}
		catch(e)
		{
			return NaN;
		}
	}
	
	/*	
	function changeTargetID()
	{
	}
	
	function onTargetIDClick()
	{
		//window.
		var sFeature = "dialogWidth:550px; dialogHeight:500px; center:yes; help:no; resizable:yes; scroll:no; status:no";
		var sPath = "../Dialogs/";  
		var arg = new Object();
		
		var returnResultObj = showModalDialog(sPath, arg, sFeature);
		
	}
	*/
	
	function changeTargetOGU()
	{
		var oldID= roleSelect.options[m_lastRoleIndex].delegateID;
		
		//if (OGUInput.value)
		if (OGUInput.getAttribute("GUID"))
			disableDate(false, START_TIME.value, END_TIME.value);
		else
			disableDate(true);
			
		if (OGUInput.getAttribute("GUID") != oldID || START_TIME.value != roleSelect.options[m_lastRoleIndex].START_TIME || END_TIME.value != roleSelect.options[m_lastRoleIndex].END_TIME)
			btnOK.disabled = false;
		else
			btnOK.disabled = true;
	}

//-->
