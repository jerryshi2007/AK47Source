<!--	
	var xmlUserInfo; 
	var m_allRowCount = 0;//列表总行数
	var m_maxPage = 0;
	var m_listMaxCount = 0;//分页显示数
	var m_curPage = 1;//当前页号
	var m_xmlData = null;

	
	var FUNC_APPRES_INSERT_CHILD = "ORIGINAL_RES_ADD_CHILD";
	var FUNC_APPRES_MODIFY = "ORIGINAL_RES_UPDATE";
	var FUNC_APPRES_DELETE = "ORIGINAL_RES_DELETE";
	var FUNC_ROLE_INSERT_DELETE = "ORIGINAL_ROLE_ADD_DELETE";
	var FUNC_ROLE_MODIFY = "ORIGINAL_ROLE_UPDATE";
	var FUNC_FUNC_INSERT_DELETE = "ORIGINAL_FUNC_ADD_DELETE";
	var FUNC_FUNC_MODIFY = "ORIGINAL_FUNC_UPDATE";
	var FUNC_RTF_ADMIN = "ORIGINAL_RTF_ADMIN";
	var FUNC_UTR_ADMIN = "ORIGINAL_UTR_ADMIN";

	var m_appListMenu = new Array(
								"新建应用程序...,,,,../images/insert.gif,newObj",
								"删除,,,,../images/delete.gif,delete",
								"属性...,,,,../images/property.gif,property"
							);

	var m_scopeListMenu = new Array(
								//"新建数据服务范围...,,,,../images/newScope2.gif,newScope2",
								//"继承数据服务范围...,,,,../images/newScope2Copy.gif,newScope2Copy",
								"新建机构服务范围...,,,,../images/insert.gif,newObj",
								//"继承机构服务范围...,,,,../images/insertCopy.gif,newObjCopy",
								"删除,,,,../images/delete.gif,delete"
							);

	var m_roleListMenu = new Array(
								"新建角色...,,,,../images/insert.gif,newObj",
								//"继承角色...,,,,../images/insertCopy.gif,newObjCopy",
								"删除,,,,../images/delete.gif,delete",
								"属性...,,,,../images/property.gif,property"
							);

	var m_funcListMenu = new Array(
								"新建功能...,,,,../images/insertFunc.gif,newFunction",
								//"继承功能...,,,,../images/insertFuncCopy.gif,newFunctionCopy",
								"新建功能集合...,,,,../images/insert.gif,newObj",
								//"继承功能集合...,,,,../images/insertCopy.gif,newObjCopy",
								"删除,,,,../images/delete.gif,delete",
								"属性...,,,,../images/property.gif,property"
							);

	var m_funcSetListMenu = new Array(
								"新建功能集合...,,,,../images/insert.gif,newObj",
								//"继承功能集合...,,,,../images/insertCopy.gif,newObjCopy",
								"删除,,,,../images/delete.gif,delete",
								"属性...,,,,../images/property.gif,property"
							);

	var m_userListMenu = new Array(
								"新建被授权对象...,,,,../images/insert.gif,newObj",
								//"继承被授权对象...,,,,../images/insertCopy.gif,newObjCopy",
								"删除,,,,../images/delete.gif,delete"
							);
	var m_funcToSetListMenu = new Array(
								"属性...,,,,../images/property.gif,property"
							);
	var m_created = false;

	var m_refAppPermissions = "";	//本管理本应用程序的权限
	var m_bIsAdminUser = false;		//是否是全局管理员

	var C_FORBID_DELETE = "不允许删除系统内置的应用程序、角色或功能";
	var m_objParam = null;
	var m_nCheckItemCount = 0;

	var m_txtSelection = null;
	
	var inputXNode;
	var inputXml;
	var m_showFuncSetContent; //是否要显示功能集合中的内容
	

	function getPermissionID(strType, strOP)
	{
		var strResult = "noMatched";
		var strOP = strOP.toLowerCase();

		switch(strType)
		{
			case "ROLES":	if (strOP == "add")
								strResult = FUNC_ROLE_INSERT_DELETE;
							else
								strResult = FUNC_ROLE_MODIFY;
							break;
			case "FUNCTIONS":if (strOP == "add")
								strResult = FUNC_FUNC_INSERT_DELETE;
							else
								strResult = FUNC_FUNC_MODIFY;
							break;
			case "RESOURCES":if (strOP == "add")
								strResult = FUNC_APPRES_INSERT_CHILD;
							else
								strResult = FUNC_APPRES_MODIFY;
							break;
			case "ROLES_TO_FUNCTIONS":
							strResult = FUNC_RTF_ADMIN;
							break;
			case "USERS_TO_ROLES":
							strResult = FUNC_UTR_ADMIN;
							break;
		}

		return strResult;
	}

	function hasPermission(strPermissionCodeName)
	{
		return true;
		/*
		var bResult = m_bIsAdminUser;
		
		if (!bResult && m_refAppPermissions)
			bResult = m_refAppPermissions.indexOf(strPermissionCodeName) != -1;

		return bResult;
		*/
	}

	function doEnterOperation(oTD)
	{
		var xmlDoc = createDomDocument("<ENTER/>");
		var root = xmlDoc.documentElement;

		if (m_objParam.type == "APPLICATIONS" || m_objParam.type == "RESOURCES")
		{
			root.setAttribute("appID", oTD.objID);
			root.setAttribute("type", "RESOURCES");
			root.setAttribute("appLevel", oTD.all(1).appLevel);
			root.setAttribute("objID", oTD.objID);
		}
		else
		{
			root.setAttribute("appID", m_objParam.appID);
			root.setAttribute("type", m_objParam.type);
			root.setAttribute("appLevel", m_objParam.appResLevel);
			root.setAttribute("objID", oTD.objID);
		}

		if (m_objParam.refAppID)
			root.setAttribute("refAppID", m_objParam.refAppID);

		setSyncData(xmlDoc.xml);
	}
	
	function menuObjectBeforePopup()
	{
			/*					"新建功能...,,,,../images/insertFunc.gif,newFunction",
								"继承功能...,,,,../images/insertFuncCopy.gif,newFunctionCopy",
								"新建功能集合...,,,,../images/insert.gif,newObj",
								"继承功能集合...,,,,../images/insertCopy.gif,newObjCopy",
								"删除,,,,../images/delete.gif,delete"
								*/
								
		switch (event.menuData)
		{
			/*
			case "enter" :	event.disableItem = (secFrm.value == "2");
							break;
			case "searchGroup":	
							var oTD = getRelativeTD(menuObject.row, "NAME");							
							event.disableItem = (oTD.childNodes[1].objectClass != "group");
							break;*/
			
			case "newScope2":
							event.disableItem = (newScope2.style.display == "none");
							break;
			case "newScope2Copy":
							event.disableItem = (newScope2Copy.style.display == "none");
							break;
			case "newObj":	
							event.disableItem = (newObj.style.display == "none");
							break;
			case "newObjCopy":
							event.disableItem = (newObjCopy.style.display == "none");
							break;
			case "newFunction":
							event.disableItem = (newFunc.style.display == "none");
							break;
			case "newFunctionCopy":
							event.disableItem = (newFuncCopy.style.display == "none");
							break;
			case "delete":	
							event.disableItem = (deleteObj.style.display == "none");
							break;
			case "property":event.disableItem = false;//!hasPermission(getPermissionID(m_objParam.type, "update"));
							break;
		}
	}
	

	function menuObjectClick()
	{
		try
		{
			if (m_txtSelection)
				m_txtSelection.select();

			var oTD = getRelativeTD(menuObject.row, "NAME");

			switch (event.menuData)
			{
				/*case "enter":	doEnterOperation(oTD);
								break;*/
				case "delete":	
								if (m_txtSelection.text.length == 0)
								{
									var rgn = document.body.createTextRange();
									rgn.moveToElementText(oTD);
									rgn.select();
								}
								deleteSelectedObjects();
								break;
				case "property":
//								if (m_txtSelection.text.length == 0)
//								{
//									var rgn = document.body.createTextRange();
//									rgn.moveToElementText(oTD);
//									rgn.select();
//								}
								showObjProperty(oTD.childNodes[1]);
								break;
				default:		doToolbarEvent(event.menuData);
								break;
			}
		}
		catch(e)
		{
			showError(e);
		}
	}

	function onCheckBoxClick()
	{
		try
		{
			var chk = event.srcElement;

			setCheckBoxClick(chk);
			setSaveSpan();
		}
		catch(e)
		{
			showError(e);
		}
	}

	function setCheckBoxClick(chk)
	{
		if (chk.checked != chk.oldValue)
			m_nCheckItemCount++;
		else
			m_nCheckItemCount--;
	}

	function setSaveSpan()
	{
		if (m_nCheckItemCount > 0)
			saveSpan.style.display = "inline";
		else
			saveSpan.style.display = "none";
	}

	function showObjProperty(obj)
	{
		var xNode = getNodeFromID(obj.objID);
		var strSelectedType = getTypeFromFather(m_objParam.fatherNodeType, xNode);

		var strType				= "";
		var strClassify			= "";
		switch (strSelectedType)
		{
			//应用树叶结点类型：node.xData.type
			case C_NODE_TYPE_B21:	//机构服务范围
									strClassify = "y";
									strType		= "SCOPES";
									break;
			case C_NODE_TYPE_B22:	//数据服务范围
									strClassify = "n";
									strType		= "SCOPES";
									break;
			case C_NODE_TYPE_B31:	//自授权角色
									strClassify = "y";
									strType		= "ROLES";
									break;
			case C_NODE_TYPE_B51:	//应用角色
									strClassify = "n";
									strType		= "ROLES";
									break;
			case C_NODE_TYPE_B41:	//自授权功能
									strClassify = "y";
									strType		= "FUNCTIONS";
									break;
			case C_NODE_TYPE_B42:	//自授权功能集合
									strClassify = "y";
									strType		= "FUNCTION_SETS";
									break;
			case C_NODE_TYPE_B62:	//应用功能集合
									strClassify = "n";
									strType		= "FUNCTION_SETS";
									break;
			case C_NODE_TYPE_B61:	//应用功能
									strClassify = "n";
									strType		= "FUNCTIONS";
									break;
			case C_NODE_TYPE_B71:	//通用授权应用
									strClassify = "";
									strType		= "APPLICATIONS";
									break;
			case C_NODE_TYPE_B72:	//一般应用
									strClassify = "";
									strType		= "APPLICATIONS";
									break;
			//被授权对象的类型
			case C_OBJ_TYPE_A1:		//人员
									strClassify = "0";
									strType		= "EXPRESSIONS";
									break;
			case C_OBJ_TYPE_A2:		//机构	
									strClassify = "1";
									strType		= "EXPRESSIONS";
									break;
			case C_OBJ_TYPE_A3:		//组
									strClassify = "2";
									strType		= "EXPRESSIONS";
									break;
		}
	
		var arg = new Object();
		arg.type			= strType;
		arg.op				= "update";
		arg.appID			= m_objParam.appID//tNode.xData.appID;
		arg.appResLevel		= obj.appLevel;//tNode.xData.appLevel;
		arg.codeName		= getSingleNodeText( xNode, "CODE_NAME", "" );
		arg.objID			= obj.objID;
		arg.id				= arg.objID;
		arg.classify		= strClassify;
		arg.disabled		= isPreDefineObj(strSelectedType);
		arg.fatherNodeType	= m_objParam.fatherNodeType;

		var xmlResult = showAppObjDetailDialog(arg);
		
		if (xmlResult)
		{
//			if (inputXNode.value.length > 0)
//			{
//				var xmlDoc = createDomDocument(inputXNode.value);
//				m_curNodeKey = getSingleNodeText(xmlDoc.documentElement, "nodeKey", m_curNodeKey);
//				tNode = tv.getItemByKey(m_curNodeKey);
//			}
			refreshList();
		}
		return;
		/*
		var strType				= m_objParam.type;
		var strObjID			= m_objParam.objID;
		var strAppID			= m_objParam.appID;
		var strResourceLevel	= m_objParam.resourceLevel;
		var strCodeName			= m_objParam.codeName; 
		var xmlResult = showAppObjDetailDialog("update", strObjID, strType, strAppID, strResourceLevel, strCodeName);
		
		if (xmlResult)
			doSyncData(xmlResult.xml);
		break;
		*/
		if (strType != "USERS_TO_ROLES")
		{
			var xmlResult = null;
			if (obj.appLevel)
				xmlResult = showAppObjDetailDialog("update", obj.objID, strType, m_objParam.appID, m_objParam.resourceFlag, obj.appLevel, m_objParam.appCodeName);
			else
				xmlResult = showAppObjDetailDialog("update", obj.objID, strType, m_objParam.appID, m_objParam.resourceFlag, m_objParam.appLevel, m_objParam.appCodeName);

			if (xmlResult)
			{
				if (xmlResult.documentElement.selectSingleNode(".//RES_TYPE"))
				{
					switch (parseInt(xmlResult.documentElement.selectSingleNode(".//RES_TYPE").text))
					{
						case 0 : strResType = "APPLICATIONS";
							break;
						case 1 : strResType = "ResourceItem";
							break;
						case 2 : strResType = "ResourceFolder";
							break;
						default: strResType = "ResourceFile";
							break;
					}
					getRelativeTD(obj, "NAME").firstChild.style.backgroundImage = "url(../images/" + getImgFromClass(strResType) + ")";
				}
				
				//updateGridCell(xmlResult.documentElement, getOwnerTR(obj));
				
				//setSyncData(xmlResult.xml);
			}
		}
		else
		{
			showUserFunctionsDialog(obj, m_objParam);
		}
	}

	function onNameClick()
	{
		try
		{
			try
			{				
				var obj = event.srcElement;
				
				if (!event.ctrlKey)
				{
					var rgn = document.body.createTextRange();
					var nameTD = getRelativeTD(getOwnerTR(obj), "NAME");

					rgn.moveToElementText(nameTD);
					rgn.select();
				}
				
				if (secFrm && secFrm.value == "2")
				{
					parent.window.syncValue.value = "USERS_TO_ROLES&" + m_objParam.appID + "&" + m_objParam.appLevel + "&" + obj.objID + "&0&RESOURCES";
					parent.window.userList.innerHTML = "角色(" + obj.innerText + ")下的人员";
				}
				else					
					showObjProperty(obj);
				
				event.returnValue = false;
			}
			catch(e)
			{
				showError(e);
			}
		}
		finally
		{
			event.cancelBubble = true;
		}
	}

	function setNameCell(xmlNode, oTD)
	{
		if (!oTD.dataFld)
			oTD.dataFld = "NAME";
		var strName = oTD.innerText;
		var imgSrc = "../images/" + getImgFromType(getTypeFromFather(m_objParam.fatherNodeType, xmlNode));

		var bHasPermission = hasPermission(getPermissionID(m_objParam.type, "update"));

		var a = null;

/*		if (bHasPermission || m_objParam.type == "ROLES" || m_objParam.type == "USERS_TO_ROLES" )
		{
			a = document.createElement("a");
			a.onclick = onNameClick;
			a.href = "";
		}
		else*/
			a = document.createElement("span");

		a.style.marginLeft = "4px";
		a.objID = getSingleNodeText( xmlNode, "ID", "")
		if (a.objID == "")
			a.objID = getSingleNodeText( xmlNode, "GUID", "")

		if (xmlNode.selectSingleNode("RESOURCE_LEVEL"))
			a.appLevel = xmlNode.selectSingleNode("RESOURCE_LEVEL").text;
		a.tag = xmlNode.tagName;
			
		if (xmlNode.selectSingleNode(".//APP_ID"))
		{
			oTD.parentElement.canDelete = true;
			a.canDelete = true;
			
			if (!bHasPermission && m_objParam.type == "ROLES")
			{
				oTD.parentElement.canDelete = false;
				a.canDelete = false;
				a.style.color = "#a9a9a9";
			}
		}
		
		////////////////////////forever add 20030829 begin
		var inheritNode = xmlNode.selectSingleNode(".//INHERIT");
		if (xmlNode.nodeName != "RESOURCES" && inheritNode && inheritNode.text == "y")
		{
			oTD.parentElement.canDelete = false;
			a.canDelete = false;
			a.style.color = "#000066";
		}
		////////////////////////forever add 20030829 end

		if (m_objParam.fatherNodeType == C_NODE_TYPE_B31	//自授权角色
			|| m_objParam.fatherNodeType == C_NODE_TYPE_B51)	//应用角色
		{
			var nodeDisplay = xmlNode.selectSingleNode("DISPLAY_NAME");

			if (nodeDisplay && nodeDisplay.text.length > 0)
				a.innerText = nodeDisplay.text;

			//imgSrc = "../images/" + getImgFromClass(xmlNode.selectSingleNode("OBJECT_CLASS").text);
			//a.objectClass = xmlNode.selectSingleNode("OBJECT_CLASS").text;
		}
		else
		{
			a.innerText = oTD.innerText;
			//imgSrc = "../images/" + getImgFromClass(m_objParam.type);
		}

		oTD.objID = a.objID;
		var oTR = getOwnerTR(oTD);

		oTR.objID = a.objID;
		oTD.innerText = " ";

		oTD.insertAdjacentElement("afterBegin", a);

		var oSpan = document.createElement("SPAN");

		with (oSpan)
		{
			style.position = "relative";
			style.width = 16;
			style.height = 16;

			style.backgroundImage = "url(" + imgSrc + ")";
			style.backgroundPosition = "center center";
			style.backgroundRepeat = "no-repeat";
		}

		oTD.insertAdjacentElement("afterBegin", oSpan);
	}

	function setLastCell(xmlNode, oTD, strCol)
	{
		oTD.innerText = "";
		var chk = document.createElement("<input type='checkbox'>");

		chk.oldValue = (xmlNode.selectSingleNode(strCol).text.length > 0);

		chk.RelationType = strCol;
		chk.MainID	= getSingleNodeText(inputXNode, "ID", "");
		if (chk.MainID == "")
			chk.MainID = getSingleNodeText(inputXNode, "GUID", "");
		chk.SubID	= getSingleNodeText(xmlNode, "ID", "");
		
		if (chk.SubID == "")
			chk.SubID = getSingleNodeText(xmlNode, "GUID", "");
		
		var relationType="", mainName="", subName="";
		switch (m_objParam.fatherNodeType)
		{
			case C_NODE_TYPE_B41:	//自授权功能
			case C_NODE_TYPE_B42:	//自授权功能集合
									chk.disabled = true;
									break;
		}

		//chk.disabled = !hasPermission(FUNC_RTF_ADMIN);// || parseInt(xmlNode.selectSingleNode(".//APP_ID").text) != parseInt(m_objParam.appID);
		chk.onclick = onCheckBoxClick;
		chk.style.border = "none";

		oTD.insertAdjacentElement("afterBegin", chk);
		chk.checked = chk.oldValue;
	}

	function gridCallBack(strCmd, oTD, xmlNode, nodeName, nodeText)
	{
		try
		{
			switch(strCmd)
			{
				case "onCalc":	
					switch (nodeName)	
					{
						case "NAME":setNameCell(xmlNode, oTD);
							break;
						case "DESCRIPTION":
							if (m_objParam.type == "USERS_TO_ROLES")
								oTD.innerText = getOwnerFromDN(xmlNode.selectSingleNode("ID").text);
							break;
//						case "CODE_NAME":
//							//if (m_objParam.type == "USERS_TO_ROLES")
//								oTD.innerText = xmlNode.selectSingleNode("CODE_NAME");
//							break;
//						case "ACCESS_LEVEL_NAME":
//							oTD.innerText = xmlNode.selectSingleNode("ACCESS_LEVEL_NAME");
//							break;
						case "ACCESS_LEVEL":
							var nodeAccessLevel = xmlNode.selectSingleNode("ACCESS_LEVEL");

							if (nodeAccessLevel)
								oTD.innerText = getValueFromDataSet(LEVEL_CODE, "LEVEL_ID", nodeAccessLevel.text, "LEVEL_NAME");
							break;
						case "SORT_ID":
							{
								var oTable = getOwnerTable(oTD);
								
								if (oTable.listSortId)
								{
									oTD.sortText = oTable.listSortId++;
									oTD.innerText = oTD.sortText;
									oTD.align = "right";
								}
								break;
							}
						case "FUNC_SET_ID":
							setLastCell(xmlNode, oTD, "FUNC_SET_ID");
							break;
						case "FUNC_ID":
							setLastCell(xmlNode, oTD, "FUNC_ID");
							break;
						case "EXP_ID":
							setLastCell(xmlNode, oTD, "EXP_ID");
							break;
						default:
							break;
					}
					break;
				case "onClick":	
					if (!event.ctrlKey)
					{
						var rgn = document.body.createTextRange();
						var nameTD = getRelativeTD(oTD, "NAME");

						if ( nameTD != null )
						{
							rgn.moveToElementText(nameTD);
							rgn.select();
						}
					}
			}
		}
		catch(e)
		{
			showError(e);
		}
	}

	function createQueryXml()
	{
		var xmlDoc = null;

		if (m_objParam.type == "RESOURCES")
		{
			xmlDoc = createDomDocument("<queryResource/>");
		}
		else
		{
			if (m_objParam.type != "ROLES_TO_FUNCTIONS" && m_objParam.type != "USERS_TO_ROLES")
				xmlDoc = createDomDocument("<getAppObjectInfo/>");
			else
			{
				var strRoot = "<getFunctionsOfRolesInfo/>";

				if (m_objParam.type == "USERS_TO_ROLES")
					strRoot = "<getUsersOfRolesInfo/>";

				xmlDoc = createDomDocument(strRoot);
				appendAttr(xmlDoc, xmlDoc.documentElement, "ID", m_objParam.objID);
			}
		}

		return xmlDoc;
	}

	//从数据库中查询到列表信息
	function getChildDataSet()
	{
		inputXml	= createDomDocument(getInputXmlText());
		inputXNode		= createDomDocument(getInputXNodeText()).documentElement;
		buildParam( inputXNode );
		
		
		
		m_xmlData = createDomDocument("<DataSet/>");
		
		//去掉<Table1>结点，显示"功能集合"的内容时，<Table1>是功能集合所拥有的所有功能
		cloneNodes(m_xmlData.documentElement, inputXml.documentElement, true, "Table1");
		
		var rootResult = m_xmlData.documentElement;
		
		//if ( m_objParam.fatherNodeType == "" || m
		initPageCount(m_xmlData);
		setInterfaceByPermission( m_objParam.fatherNodeType );
		showList(m_xmlData, 1);

	}
	
	
	//显示当前页
	function showList(xmlData, iPage)
	{
		roleFunctionTableBody.noSelect = true;
		
		roleFunctionTable.listSortId = 1;
		
		var iFistIndex = (iPage - 1) * m_listMaxCount;
		
		var firstNode = xmlData.documentElement.selectSingleNode("./Table["+iFistIndex+"]")
		
		
		
		if ( firstNode == null )
		{
			firstNode = xmlData.documentElement.firstChild;
			iPage = 1;
		}
		curPageIndex.value = iPage;
		
		createGridBodyByHead(firstNode, 
							roleFunctionTableBody,
							null,
							m_listMaxCount,
							roleFunctionTableHead.rows(0),
							roleFunctionTable,
							gridCallBack);

		m_nCheckItemCount = 0;
		
	}
	
	//分页初始化
	function initPageCount(xmlData)
	{
	
		m_allRowCount = xmlData.documentElement.childNodes.length;
		m_maxPage = Math.round( (m_allRowCount-1) / m_listMaxCount - 0.5 ) + 1
		m_curPage = 1;
		
		rowCount.innerText = m_allRowCount;
		curPage.innerText = m_curPage
		totalPage.innerText = m_maxPage;
		curPageIndex.value = m_curPage;
		
		if ( m_maxPage <= 1 )
			paginationTD.style.display = "none";
			
	}

	//只能输入数字	
	function checkNumber()
	{
		if ( event.keyCode < 48 || event.keyCode > 57 )
		{
			event.keyCode = 0;
		}
	}
	
	//翻页
	function onGoPage(strPageIndex)
	{
		var iPageIndex = 1;
		if ( strPageIndex == null )
		{
			if ( curPageIndex.value == "" )
				iPageIndex = 1;
			else
				iPageIndex = parseInt( curPageIndex.value );
		}
		else
		{
			switch (strPageIndex)
			{
				case "first" :
					iPageIndex = 1;
					break;
				case "pre" :
					iPageIndex = m_curPage - 1;
					break;
				case "next" :
					iPageIndex = m_curPage + 1;
					break;
				case "last" :
					iPageIndex = m_maxPage;
					break;
			}
		}
		
		if ( iPageIndex < 1 )
		{
			//alert("最小显示第"+1+"页");
			iPageIndex = 1;
		}
		if ( iPageIndex > m_maxPage )
		{
			//alert("最大显示第"+m_maxPage+"页");
			iPageIndex = m_maxPage;
		}
		
		if ( iPageIndex != m_curPage )
		{
			showList( m_xmlData, iPageIndex );
			m_curPage = iPageIndex ;
			curPage.innerText = m_curPage
		}
		curPageIndex.value = iPageIndex;
			
	}
	
	
	//翻页
	function doChangePage(oPageIndex, oShowCount)
	{
		if ( oPageIndex == null )
		{
			oPageIndex == curPageIndex.value;
		
		}
		if ( oShowCount == null )
		{
			oShowCount = curShowRowCount.value ;
		}
		
		var iPageIndex ;
		var iShowCount ;
		
		if ( typeof(oPageIndex) == "string" )
		{
			iPageIndex = parseInt( curPageIndex.value );
		}
		else
			iPageIndex = oPageIndex;
		
		if ( strPageIndex == "" )
			iPage = 0;
		else
			iPage = parseInt( curPageIndex.value );
		
			
		curPageIndex.value = iPage ;
		if ( iPage < 1 )
		{
			alert("最小显示第"+1+"页");
			curPageIndex.value = 1;
		}
		if ( iPage > m_maxPage )
		{
			alert("最大显示第"+m_maxPage+"页");
			curPageIndex.value = m_maxPage;
		}
		
		if ( iPage != m_curPage )
		{
			showList( m_xmlData, curPageIndex.value );
			m_curPage = iPage ;
		}
	}

	function resetRolesToFunctions()
	{
		var container = roleFunctionTableBody;

		for (var i = 0; i < container.all.length; i++)
		{
			var chk = container.all(i);

			if (chk.tagName == "INPUT" && chk.type == "checkbox")
				if (chk.checked != chk.oldValue)
					chk.oldValue = chk.checked;
		}

		m_nCheckItemCount = 0;
	}

	function newAppObject(resLevel)
	{
		var arg = new Object();

		arg.type			= m_objParam.type;
		arg.op				= "insert";
		arg.appID			= m_objParam.appID;
		arg.appResLevel		= m_objParam.appResLevel;
		//arg.codeName		= strCodeName;
		if (resLevel)
			arg.resLevel	= resLevel;
		else 
			arg.resLevel	= "";
		arg.classify		= m_objParam.classify;
		arg.inherited		= m_objParam.inherited;
		arg.disabled		= false;
		arg.fatherNodeType	= m_objParam.fatherNodeType;



		var xmlResult = showAppObjDetailDialog(arg);

		if (xmlResult) 
			refreshList();
		/*
		if (xmlResult)
		{
			appendGridRow(xmlResult.documentElement, 
							roleFunctionTableBody,
							roleFunctionTableHead.rows(0),
							false,
							gridCallBack);
			changeGridRowOddEvenColor(roleFunctionTable);
			setSyncData(xmlResult.xml);
		}
		*/
	}

	//在角色中添加新用户或组织机构
	function newUserInRole()
	{
		var xNode = showSelectUsersToRoleDialog();

		if (xNode.length > 0)
		{
			var xmlAD = createDomDocument(xNode);
			var xmlDoc = createDomDocument("<Insert/>");

			var root = xmlAD.documentElement;

			if (m_objParam.refAppID)
				xmlDoc.documentElement.setAttribute("refAppID", m_objParam.refAppID);
			if (m_objParam.appID)
				xmlDoc.documentElement.setAttribute("appID", m_objParam.appID);

			for (var i = 0; i < root.childNodes.length; i++)
			{
				var nodeRecord = appendNode(xmlDoc, xmlDoc.documentElement, "USERS_TO_ROLES");
				appendAttr(xmlDoc, nodeRecord, "parentLevel", m_objParam.appLevel);
				var nodeSet = appendNode(xmlDoc, nodeRecord, "SET");
				var strObjectClass = root.childNodes[i].getAttribute("objectClass");

				appendNode(xmlDoc, nodeSet, "ROLE_ID", m_objParam.objID);
				
				appendNode(xmlDoc, nodeSet, "USER_ID", root.childNodes[i].getAttribute("dn"));
				
				var strDisplayName = root.childNodes[i].getAttribute("displayName");
				if (strDisplayName.length == 0)
					strDisplayName = root.childNodes[i].getAttribute("name");
				appendNode(xmlDoc, nodeSet, "DISPLAY_NAME", strDisplayName);

				if (strObjectClass == "organizationalUnit" || strObjectClass == "domain")
					appendNode(xmlDoc, nodeSet, "ACCESS_LEVEL", root.getAttribute("access_level"));

				appendNode(xmlDoc, nodeSet, "OBJECT_CLASS", strObjectClass);
			}

			var xmlResult = xmlSend("../AppAdminOP/AppDBWriter.aspx", xmlDoc);

			checkErrorResult(xmlResult);

			var nodes = xmlResult.selectNodes(".//SET");
			for (var j = 0; j < nodes.length; j++)
			{
				for(var iRowPos = 0; iRowPos < roleFunctionTableBody.rows.length; iRowPos++)
				{
					var strObjDN = roleFunctionTableBody.rows[iRowPos].xmlRow.selectSingleNode("ID").text;
					if (nodes[j].selectSingleNode("ID").text == strObjDN)
						break;
				}
				if (iRowPos < roleFunctionTableBody.rows.length)
					updateGridCell(nodes[j], roleFunctionTableBody.rows[iRowPos], gridCallBack);
				else
					appendGridRow(nodes[j], 
								roleFunctionTableBody,
								roleFunctionTableHead.rows(0),
								false,
								gridCallBack);
			}
			changeGridRowOddEvenColor(roleFunctionTable);
		}
	}

	function syncDeletedRows(xmlDoc)
	{
		var i = 0;
		var root = xmlDoc.documentElement;
		while(i < roleFunctionTableBody.rows.length)
		{
			var row = roleFunctionTableBody.rows[i];
			var bDeleted = false;

			if (root.firstChild.nodeName == "RESOURCES")
			{
				strResLevel = getRelativeTD(row, "NAME").all(1).appLevel;
				node = getSingleNodeWithNodeValue(root, ".//RESOURCE_LEVEL", strResLevel);
			}
			else if ((root.firstChild.nodeName == "ROLES" || root.firstChild.nodeName == "FUNCTIONS") && root.selectSingleNode(".//WHERE"))
			{
				strID = row.objID;
				node = getSingleNodeWithNodeValue(root, ".//ID", strID)
			}
			else if (root.firstChild.nodeName == "USERS_TO_ROLES" && root.selectSingleNode(".//WHERE"))
			{
				strID = row.objID;
				node = getSingleNodeWithNodeValue(root, ".//USER_ID", strID)
			}
			else
			{
				strID = row.objID;
				node = root.selectSingleNode(".//" + m_objParam.type + "[@id=\"" + strID + "\"]");
			}

			if (node)
			{
				roleFunctionTableBody.deleteRow(row.rowIndex - 1);
				bDeleted = true;
			}

			if (!bDeleted)
				i++;
		}

		changeGridRowOddEvenColor(getOwnerTable(roleFunctionTableBody));
	}

	function enumSelection(root)
	{
		var selection = document.selection;

		var rgn = selection.createRange();

		tempDiv.innerHTML = rgn.htmlText;

		var nodeCount = 0;
		var strObjIDs = "";
		for (var i = 0; i < tempDiv.childNodes.length; i++)
		{
			var obj = tempDiv.childNodes[i];
			if (obj.objID)
			{
				if ( strObjIDs.indexOf(obj.objID) == -1 )
				{ 
					
					trueThrow(obj.objID == C_APP_ADMIN_ID, C_FORBID_DELETE);
					
					strObjIDs += obj.objID + ";";
					
					var xNode = getNodeFromID(obj.objID);
					
					var strSelectedType = getTypeFromFather(m_objParam.fatherNodeType, xNode);
					
					switch(strSelectedType)
					{
						case C_NODE_TYPE_B21:	//机构服务范围
												m_objParam.classify = "y";
												m_objParam.type		= "SCOPES";
												break;
						case C_NODE_TYPE_B22:	//数据服务范围
												m_objParam.classify = "n";
												m_objParam.type		= "SCOPES";
												break;
						case C_NODE_TYPE_B31:	//自授权角色
												m_objParam.classify = "y";
												m_objParam.type		= "ROLES";
												break;
						case C_NODE_TYPE_B41:	//自授权功能
												m_objParam.classify = "y";
												m_objParam.type		= "FUNCTIONS";
												break;
						case C_NODE_TYPE_B42:	//自授权功能集合
												m_objParam.classify = "y";
												m_objParam.type		= "FUNCTION_SETS";
												break;
						case C_NODE_TYPE_B51:	//应用角色
												m_objParam.classify = "n";
												m_objParam.type		= "ROLES";
												break;
						case C_NODE_TYPE_B61:	//应用功能
												m_objParam.classify = "n";
												m_objParam.type		= "FUNCTIONS";
												break;
						case C_NODE_TYPE_B62:	//应用功能集合
												m_objParam.classify = "n";
												m_objParam.type		= "FUNCTION_SETS";
												break;
						case C_NODE_TYPE_B72:	//一般应用
												m_objParam.classify = "";
												m_objParam.type		= "APPLICATIONS";
												break;
						//被授权对象的类型
						case C_OBJ_TYPE_A1:		//人员
												m_objParam.classify = "0";
												m_objParam.type		= "EXPRESSIONS";
												break;
						case C_OBJ_TYPE_A2:		//机构	
												m_objParam.classify = "1";
												m_objParam.type		= "EXPRESSIONS";
												break;
						case C_OBJ_TYPE_A3:		//组
												m_objParam.classify = "2";
												m_objParam.type		= "EXPRESSIONS";
												break;
					}
					
					
					appendNode(root.ownerDocument, root, m_objParam.type);
					nodeCount++;
					
					switch(m_objParam.type)
					{
							case "APPLICATIONS":
											var childrenCount	= parseInt(getSingleNodeText(xNode, "CHILDREN_COUNT"));
											var appName		= getSingleNodeText(xNode, "NAME");
											trueThrow(childrenCount > 0,	"对不起，应用("+appName+")有子应用，\n\n这里不能做删除操作！");
											/*var n = xNode.firstChild;
											while (n)
											{
												root.firstChild.appendChild(n.cloneNode(true));
												n = n.nextSibling;
											}*/
											addConditionFields(root.ownerDocument, root.childNodes[nodeCount-1], "ID", "=", obj.objID);
											addConditionFields(root.ownerDocument, root.childNodes[nodeCount-1], "CHILDREN_COUNT", "=", 0);
											break;
											
							case "SCOPES":
											addConditionFields(root.ownerDocument, root.childNodes[nodeCount-1], "ID", "=", obj.objID);
											break;
							case "ROLES":
											addConditionFields(root.ownerDocument, root.childNodes[nodeCount-1], "ID", "=", obj.objID);
											break;
							case "FUNCTIONS":
											addConditionFields(root.ownerDocument, root.childNodes[nodeCount-1], "ID", "=", obj.objID);
											break;
							case "FUNCTION_SETS":
											addConditionFields(root.ownerDocument, root.childNodes[nodeCount-1], "ID", "=", obj.objID);
											break;
							case "EXPRESSIONS":
											addConditionFields(root.ownerDocument, root.childNodes[nodeCount-1], "ID", "=", obj.objID);
											break;
					}
				
				}//of if
				
			}
		}
	}

	function deleteSelectedObjects()
	{
		trueThrow(m_objParam.appID == C_APP_ADMIN_ID && m_objParam.fatherNodeType != C_NODE_TYPE_B31, C_FORBID_DELETE);

		var xmlDoc = createDomDocument("<Delete/>");
		var root = xmlDoc.documentElement;
		var writerPath = "";
		
		enumSelection(root);

		if (root.firstChild)
		{
			if (confirm("确定要删除选定的对象吗？"))
			{
				if( m_objParam.type != "APPLICATIONS")
					if (confirm("如果对象被继承，是否删除继承的对象？\n选'确定'为是，不选或选'取消'为否"))
						appendAttr(xmlDoc, root, "deleteSubApp", "y");
					else
						appendAttr(xmlDoc, root, "deleteSubApp", "n");
						
				switch(m_objParam.type)
				{
					case "APPLICATIONS":
								//appendAttr(xmlDoc, root, "type", m_objParam.type);
								writerPath = "../XmlRequestService/XmlAOSWriteRequest.aspx";
								break;
					case "SCOPES":
								appendAttr(xmlDoc, xmlDoc.firstChild, "app_code_name", m_objParam.appCodeName);
								appendAttr(xmlDoc, xmlDoc.firstChild, "use_scope", m_objParam.useScope);
								writerPath = "../XmlRequestService/XmlAOSWriteRequest.aspx";
								break;
					case "ROLES":
								writerPath = "../XmlRequestService/XmlRFDWriteRequest.aspx";
								break;
					case "FUNCTIONS":
								writerPath = "../XmlRequestService/XmlRFDWriteRequest.aspx";
								break;
					case "FUNCTION_SETS":
								writerPath = "../XmlRequestService/XmlRFDWriteRequest.aspx";
								break;
					case "EXPRESSIONS":
								appendAttr(xmlDoc, xmlDoc.firstChild, "app_code_name", m_objParam.appCodeName);
								appendAttr(xmlDoc, xmlDoc.firstChild, "use_scope", m_objParam.useScope);
								if (m_objParam.fatherNodeType == C_NODE_TYPE_B31)//自授权角色
									appendAttr(xmlDoc, xmlDoc.firstChild, "role_classify", "y");
								if (m_objParam.fatherNodeType == C_NODE_TYPE_B51)//应用角色
									appendAttr(xmlDoc, xmlDoc.firstChild, "role_classify", "n");
								writerPath = "../XmlRequestService/XmlAOSWriteRequest.aspx";
								break;
				}

				//alert(xmlDoc.xml);
				//return;
				var xmlResult = xmlSend(writerPath, xmlDoc);

				checkErrorResult(xmlResult);
				//界面刷新
				if (xmlResult) 
					refreshList();
			}
		}
	}

	function checkAndDeleteUserInRole()
	{
		if (confirm("此操作将删除角色中已经无效的用户、组或机构，是否继续？"))
		{
			var xmlDoc = createDomDocument("<checkAndDeleteUserInRole/>");
			xmlDoc.documentElement.setAttribute("app_code_name", m_objParam.appCodeName);
			xmlDoc.documentElement.setAttribute("app_id", m_objParam.appID);
			if ( m_objParam.fatherNodeType == C_NODE_TYPE_A3 )//自授权角色集合
				xmlDoc.documentElement.setAttribute("classify", "y");
			if ( m_objParam.fatherNodeType == C_NODE_TYPE_A5 )//应用授权角色集合
				xmlDoc.documentElement.setAttribute("classify", "n");
			var xmlResult = xmlSend("../XmlRequestService/XmlAOSWriteRequest.aspx", xmlDoc);
			checkErrorResult(xmlResult);	
			var iCount = parseInt( getSingleNodeText(xmlResult.documentElement,".//DEL_EXP_COUNT","0") );
			
			if ( iCount == 0 )
				alert ("没有无效的被授权对象。") ;
			else
				alert ("共删除了"+iCount+"个无效的被授权对象。");
				

		}
	}

	function doToolbarEvent(strID)
	{
		switch(strID)
		{
			case "newObj":	
							newObjects(false);
							break;
			case "newObjCopy":
							newObjects(true);
							break;
			case "newScope2":
							newObjects(false, "scope2");
							break;
			case "newScope2Copy":
							newObjects(true, "scope2");
							break;
			case "newFunc":
							newObjects(false, "func");
							break;
			case "newFuncCopy":
							newObjects(true, "func");
							break;
			case "deleteObj":	
							deleteSelectedObjects();
							break;
			case "save":	
							updateRelation();
							break;
			case "refresh":
							refreshList();	
							break;
			case "checkUser":
							checkAndDeleteUserInRole();
							break;
/*			case "searchGroup":
							showAllPersonInGroup();
							break;*/
		}
	}
	
	function showAllPersonInGroup()
	{
		showGroupUsersDialog(event.srcElement.row.objID);
		return;
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


	function onInputParamChange()
	{
		if ((event.propertyName == "value") && (inputParam.value.length > 0))
		{
			m_nCheckItemCount = 0;

			buildParam(inputParam.value);

			getChildDataSet();
			inputParam.value = "";
		}
	}

	function buildMenuByType(strType)
	{
		m_created = true;
		var objArray = null;
		
//		if (secFrm && secFrm.value == "2")
//			strType = null;//forever
		switch (strType)
		{
			//应用树集合结点类型：node.xData.type
			case C_NODE_TYPE_A1: 	//根结点
			case C_NODE_TYPE_A7:	//子应用集合
									objArray = m_appListMenu;
									break;
			case C_NODE_TYPE_A2:	//应用服务范围集合
									objArray = m_scopeListMenu;
									break;
			case C_NODE_TYPE_A3:	//自授权角色集合
			case C_NODE_TYPE_A5:	//应用角色集合
									objArray = m_roleListMenu;
									break;
			case C_NODE_TYPE_A4:	//自授权功能树
			case C_NODE_TYPE_A6:	//应用功能树
									objArray = m_funcListMenu;
									break;
			

			//应用树叶结点类型：node.xData.type
			case C_NODE_TYPE_B21:	//机构服务范围
			case C_NODE_TYPE_B22:	//数据服务范围
									m_created = false;
									break;
			case C_NODE_TYPE_B31:	//自授权角色
			case C_NODE_TYPE_B51:	//应用角色
									objArray = m_userListMenu;
									break;
			case C_NODE_TYPE_B41:	//自授权功能
			case C_NODE_TYPE_B61:	//应用功能
									m_created = false;
									break;
			case C_NODE_TYPE_B42:	//自授权功能集合
			case C_NODE_TYPE_B62:	//应用功能集合
									if (m_showFuncSetContent == "true")
										if (getSingleNodeText(inputXNode,"LOWEST_SET","") != "y")
											objArray = m_funcSetListMenu;
										else
											objArray = m_funcToSetListMenu
									else
										m_created = false;
									break;
			case C_NODE_TYPE_B71:	//通用授权应用
			case C_NODE_TYPE_B72:	//一般应用
									m_created = false;
									break;
			
			default:				m_created = false;
		}
		/*	
		switch (strType)
		{
			case "APPLICATIONS":
							objArray = m_appListMenu;
							break;
			case "RESOURCES":
							objArray = m_resourceListMenu;
							break;
			case "ROLES":	objArray = m_roleListMenu;
							break;
			case "FUNCTIONS":
							objArray = m_funcListMenu;
							break;
			case "USERS_TO_ROLES":
							objArray = m_userListMenu;
							break;
			default:		m_created = false;
		}
	*/
		if (objArray)
			menuObject.buildMenu(objArray);
	}

	function setInterfaceByPermission(strType)
	{
		toolBar.style.display = "inline";

		if (strType)
		{
			var newSocpe2CopyDisplay= "none";
			var newScope2Display	= "none";
			var newFuncDisplay		= "none";
			var newFuncCopyDisplay	= "none";
			var newSpanDisplay		= "inline";
			var newCopySpanDisplay	= "inline";
			var deleteSpanDisplay	= "inline";
			var checkUserSpanDisplay= "none";
			
			switch(strType)
			{
				case C_NODE_TYPE_A2://应用服务范围集合
											createTableHeadByXml(ColumnTitleAppScopes.XMLDocument, roleFunctionTableHead);
											newSocpe2CopyDisplay= "inline";
											newScope2Display	= "inline";
											newObjCopy.title	= "继承机构服务范围";
											newObj.title		= "新建机构服务范围";
											break;
				case C_NODE_TYPE_A3://自授权角色集合
											checkUserSpanDisplay= "inline";
				case C_NODE_TYPE_A4://自授权功能树
											createTableHeadByXml(ColumnTitleApp.XMLDocument, roleFunctionTableHead);
											newObjCopy.title	= "继承功能集合";
											newObj.title		= "新建功能集合";
											newFuncDisplay		= "none";
											newFuncCopyDisplay	= "none";
											newSpanDisplay		= "none";
											newCopySpanDisplay	= "none";
											deleteSpanDisplay	= "none";
											break;
				case C_NODE_TYPE_A5://应用角色集合
											createTableHeadByXml(ColumnTitleApp.XMLDocument, roleFunctionTableHead);
											newObjCopy.title	= "继承角色";
											newObj.title		= "新建角色";
											if (m_objParam.appID == C_APP_ADMIN_ID)
											{
												newSpanDisplay		= "none";
												newCopySpanDisplay	= "none";
												deleteSpanDisplay	= "none";
											}
											checkUserSpanDisplay= "inline";
											break;
				case C_NODE_TYPE_A6://应用功能树
											createTableHeadByXml(ColumnTitleApp.XMLDocument, roleFunctionTableHead);
											newObjCopy.title	= "继承功能集合";
											newObj.title		= "新建功能集合";
											newFuncDisplay		= "inline";
											newFuncCopyDisplay	= "inline";
											break;
			
				case C_NODE_TYPE_A1://根结点
				case C_NODE_TYPE_A7://子应用集合
											createTableHeadByXml(ColumnTitleApp.XMLDocument, roleFunctionTableHead);
											newObjCopy.title	= "继承应用";
											newObj.title		= "新建应用";
											newCopySpanDisplay	= "none";
											break;
				case C_NODE_TYPE_B42://自授权FunctionSet
											newObjCopy.title	= "继承功能集合";
											newObj.title		= "新建功能集合";
											if ( m_showFuncSetContent == "" )
											{
												createTableHeadByXml(ColumnTitleRolesFuncs.XMLDocument, roleFunctionTableHead);
											}
											else
											{
												if (getSingleNodeText(inputXNode,"LOWEST_SET","") == "y")
												{
													createTableHeadByXml(ColumnTitleFunSetFuncs.XMLDocument, roleFunctionTableHead);
												}
												else
													createTableHeadByXml(ColumnTitleApp.XMLDocument, roleFunctionTableHead);
											}
											newFuncDisplay		= "none";
											newFuncCopyDisplay	= "none";
											newSpanDisplay		= "none";
											newCopySpanDisplay	= "none";
											deleteSpanDisplay	= "none";
											break;
				case C_NODE_TYPE_B62://应用授权FunctionSet
											newObjCopy.title	= "继承功能集合";
											newObj.title		= "新建功能集合";
											if ( m_showFuncSetContent == "" )
											{
												createTableHeadByXml(ColumnTitleRolesFuncs.XMLDocument, roleFunctionTableHead);
												newSpanDisplay		= "none";
												newCopySpanDisplay	= "none";
												deleteSpanDisplay	= "none";
											}
											else
											{
												if (getSingleNodeText(inputXNode,"LOWEST_SET","") == "y")
												{
													createTableHeadByXml(ColumnTitleFunSetFuncs.XMLDocument, roleFunctionTableHead);
													newSpanDisplay		= "none";
													newCopySpanDisplay	= "none";
													deleteSpanDisplay	= "none";
												}
												else
													createTableHeadByXml(ColumnTitleApp.XMLDocument, roleFunctionTableHead);
											}
											break;
				case C_NODE_TYPE_B31://自授权角色
				case C_NODE_TYPE_B51://应用角色
//											createTableHeadByXml(ColumnTitleUsers.XMLDocument, roleFunctionTableHead);
											createTableHeadByXml(ColumnTitleRolesUsers.XMLDocument, roleFunctionTableHead);
											newObjCopy.title	= "继承被授权对象";
											newObj.title		= "新建被授权对象";
											if (m_objParam.appID == C_APP_ADMIN_ID)
											{
												newCopySpanDisplay	= "none";
											}
											break;

				case C_NODE_TYPE_B41://自授权功能
				case C_NODE_TYPE_B61://应用功能
											createTableHeadByXml(ColumnTitleRolesFuncs.XMLDocument, roleFunctionTableHead);
											newSpanDisplay		= "none";
											newCopySpanDisplay	= "none";
											deleteSpanDisplay	= "none";
											break;
				case C_OBJ_TYPE_A1:		//人员
				case C_OBJ_TYPE_A2:		//机构	
				case C_OBJ_TYPE_A3:		//组
											createTableHeadByXml(ColumnTitleUserScopes.XMLDocument, roleFunctionTableHead);
											newSpanDisplay		= "none";
											newCopySpanDisplay	= "none";
											deleteSpanDisplay	= "none";
											break;

											
			}

			//第一级应用没有继承
			if (m_objParam.appResLevel.length == 3)
			{
				newSocpe2CopyDisplay= "none";
				newFuncCopyDisplay	= "none";
				newCopySpanDisplay	= "none";
			}
			
			
			newScope2Copy.style.display = "none";
//			newScope2Copy.style.display = newSocpe2CopyDisplay;
			newScope2.style.display		= "none";
//			newScope2.style.display		= newScope2Display;
			newFuncCopy.style.display	= "none";
//			newFuncCopy.style.display	= newFuncCopyDisplay;
			newFunc.style.display		= newFuncDisplay;
			newObjCopy.style.display	= "none";
//			newObjCopy.style.display	= newCopySpanDisplay;
			newObj.style.display		= newSpanDisplay;
			deleteObj.style.display		= deleteSpanDisplay;
			checkUser.style.display		= checkUserSpanDisplay;
			setSaveSpan();

			buildMenuByType(strType);
		}
	}

	function oBodyContextMenu()
	{
		try
		{
			if (m_created)
			{
				m_txtSelection = document.selection.createRange();

				var oTR = getOwnerTR(event.srcElement);

				menuObject.row = oTR;

				menuObject.show(event.x, event.y);
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

	function onDocumentLoad()
	{
		try
		{
			xmlUserInfo = createDomDocument(parent.window.userInfo.value);
			m_listMaxCount = parseInt(listMaxCount.value);
			getChildDataSet();
		}
		catch(e)
		{
			showError(e);
		}
	}
	
	//获得应显示数据的ＸＭＬ文档
	function getInputXmlText()
	{
		if (parent.window.inputXML)
			return parent.window.inputXML.value;
		return "";
	}
	
	//获得显示数据父结点属性XML文档
	function getInputXNodeText()
	{
		if (parent.window.inputXNode)
			return parent.window.inputXNode.value;
		return "";
	}
	
	function buildParam( xNode )
	{
		m_showFuncSetContent	= getSingleNodeText(xNode, "showFuncSetContent", "");

		m_objParam = new Object();

		m_objParam.fatherNodeType= getSingleNodeText(xNode, "nodeType", "");
		m_objParam.objID		= getSingleNodeText(xNode, "ID", "");
		m_objParam.appID		= getSingleNodeText(xNode, "appID", "");
		m_objParam.appCodeName	= getSingleNodeText(xNode, "appCodeName", "");
		m_objParam.appResLevel	= getSingleNodeText(xNode, "appLevel", "");
		m_objParam.useScope		= getSingleNodeText(xNode, "useScope", "");
		m_objParam.resLevel		= getSingleNodeText(xNode, "RESOURCE_LEVEL", "");
		m_objParam.codeName		= getSingleNodeText(xNode, "CODE_NAME", "");
	}
	
	function getNodeFromID( strObjID )
	{
		var xNode =inputXml.selectSingleNode(".//Table");
		var strID;
		while(xNode)
		{
			strID = getSingleNodeText( xNode, "ID");
			if (strID == strObjID) return xNode;
			xNode = xNode.nextSibling;
		}
		return null;
	}						
	
	//刷新列表
	function refreshList()
	{
			parent.window.inputXML.onfocus();//刷新数据，在父窗体中执行						
			getChildDataSet();
			setSaveSpan();
	}
	
	//新建操作
	function newObjects(isInherited, strType)
	{
		if (isInherited) 
			m_objParam.inherited= "y";
		else 
			m_objParam.inherited= "n";
		m_objParam.classify		= "";
		m_objParam.type			= "";
		switch (m_objParam.fatherNodeType)
		{
			//应用树集合结点类型：node.xData.type
			case C_NODE_TYPE_A1: 	//根结点
			case C_NODE_TYPE_A7:	//子应用集合
									m_objParam.classify = "";
									m_objParam.type		= "APPLICATIONS";
									newAppObject();
									break;
			case C_NODE_TYPE_A2:	//应用服务范围集合
									if (strType == "scope2")
										m_objParam.classify = "n";
									else
										m_objParam.classify = "y";
									m_objParam.type		= "SCOPES";
									newAppObject();
									break;
			
			case C_NODE_TYPE_A3:	//自授权角色集合
									m_objParam.classify = "y";
									m_objParam.type		= "ROLES";
									newAppObject();
									break;
			case C_NODE_TYPE_A4:	//自授权功能树
									m_objParam.classify = "y";
									if (strType=="func")
										m_objParam.type	= "FUNCTIONS";
									else
										m_objParam.type	= "FUNCTION_SETS";
									newAppObject();
									break;
			
			case C_NODE_TYPE_A5:	//应用角色集合
									m_objParam.classify = "n";
									m_objParam.type		= "ROLES";
									newAppObject();
									break;
			case C_NODE_TYPE_A6:	//应用功能树
									m_objParam.classify = "n";
									if (strType == "func")
										m_objParam.type	= "FUNCTIONS";
									else
										m_objParam.type	= "FUNCTION_SETS";
									newAppObject();
									break;
			

			//应用树叶结点类型：node.xData.type
									break;
			case C_NODE_TYPE_B31:	//自授权角色
			case C_NODE_TYPE_B51:	//应用角色
									m_objParam.classify = "";
									m_objParam.type		= "EXPRESSIONS";
									newUsers();
									//newAppObject();
									break;
			case C_NODE_TYPE_B42:	//自授权功能集合
									m_objParam.classify = "y";
									m_objParam.type		= "FUNCTION_SETS";
									newAppObject(m_objParam.resLevel);
									break;
			case C_NODE_TYPE_B62:	//应用功能集合
									m_objParam.classify = "n";
									m_objParam.type		= "FUNCTION_SETS";
									newAppObject(m_objParam.resLevel);
									break;
		}
	}
	
	
	//更新关系（角色-功能、角色-功能集合、功能-功能集合）
	function updateRelation()
	{
		var xmlDoc;
		var writerPath="";
		
		switch(m_objParam.fatherNodeType)
		{
			case C_NODE_TYPE_B41:	//自授权功能
			case C_NODE_TYPE_B61:	//应用功能
									xmlDoc = createRelationXml("RTF", "FUNC_ID", "ROLE_ID");
									writerPath = "../XmlRequestService/XmlRFDWriteRequest.aspx";
									break;
			case C_NODE_TYPE_B42:	//自授权功能集合
			case C_NODE_TYPE_B62:	//应用功能集合
									if (m_showFuncSetContent == "true")
									{
										xmlDoc = createRelationXml("FSTF", "FUNC_SET_ID", "FUNC_ID");
									}
									else
									{
										xmlDoc = createRelationXml("RTFS", "FUNC_SET_ID", "ROLE_ID");
									}
									writerPath = "../XmlRequestService/XmlRFDWriteRequest.aspx";
									break;
			//被授权对象的类型
			case C_OBJ_TYPE_A1:		//人员
			case C_OBJ_TYPE_A2:		//机构	
			case C_OBJ_TYPE_A3:		//组
									xmlDoc = createRelationXml("ETS", "EXP_ID", "SCOPE_ID");
									writerPath = "../XmlRequestService/XmlAOSWriteRequest.aspx";
									break;
									
		}
		
		
		var xmlResult;
		
		if (writerPath != "")
		{
			xmlResult = xmlSend(writerPath, xmlDoc);
			checkErrorResult(xmlResult);
		}

		
		if (xmlResult)
			refreshList();
	}

	//创建角色功能关系更新命令
	function createRTFXml()
	{
		var xmlDoc = createDomDocument("<RTF/>");
		var root = xmlDoc.documentElement;
		var container = roleFunctionTableBody;

		for (var i = 0; i < container.all.length; i++)
		{
			var chk = container.all(i);

			if (chk.tagName == "INPUT" && chk.type == "checkbox")
				if (chk.checked != chk.oldValue)
				{
					if (chk.checked)
					{
						var node = root.selectSingleNode("Insert");
						if (!node) 
						{
							node = appendNode(xmlDoc, root, "Insert");
						}
						node = appendNode(xmlDoc, node, "ROLE_TO_FUNCTIONS");
						node = appendNode(xmlDoc, node, "SET");
						appendNode(xmlDoc, node, "FUNC_ID", chk.MainID);
						appendNode(xmlDoc, node, "ROLE_ID", chk.SubID);
					}
					if (!chk.checked)
					{
						var node = root.selectSingleNode("Delete");
						if (!node) 
						{
							node = appendNode(xmlDoc, root, "Delete");
						}
						node = appendNode(xmlDoc, node, "ROLE_TO_FUNCTIONS");
						addConditionFields(xmlDoc, node, "FUNC_ID", "=", chk.MainID);
						addConditionFields(xmlDoc, node, "ROLE_ID", "=", chk.SubID);
					}
				}
		}

		return xmlDoc;
	}

	//创建功能-功能集合关系更新命令
	function createFSTFXml()
	{
		var xmlDoc = createDomDocument("<FSTF/>");
		var root = xmlDoc.documentElement;
		var container = roleFunctionTableBody;

		for (var i = 0; i < container.all.length; i++)
		{
			var chk = container.all(i);

			if (chk.tagName == "INPUT" && chk.type == "checkbox")
				if (chk.checked != chk.oldValue)
				{
					if (chk.checked)
					{
						var node = root.selectSingleNode("Insert");
						if (!node) 
						{
							node = appendNode(xmlDoc, root, "Insert");
						}
						node = appendNode(xmlDoc, node, "FUNC_SET_TO_FUNCS");
						node = appendNode(xmlDoc, node, "SET");
						appendNode(xmlDoc, node, "FUNC_SET_ID", chk.MainID);
						appendNode(xmlDoc, node, "FUNC_ID", chk.SubID);
					}
					if (!chk.checked)
					{
						var node = root.selectSingleNode("Delete");
						if (!node) 
						{
							node = appendNode(xmlDoc, root, "Delete");
						}
						node = appendNode(xmlDoc, node, "FUNC_SET_TO_FUNCS");
						addConditionFields(xmlDoc, node, "FUNC_SET_ID", "=", chk.MainID);
						addConditionFields(xmlDoc, node, "FUNC_ID", "=", chk.SubID);
					}
				}
		}

		return xmlDoc;
	}

	//创建关系更新命令
	function createRelationXml(strType, strMain, strSub)
	{
		var strTableName;
		var xmlDoc;
		switch(strType)
		{
			case "RTF":
						xmlDoc		= createDomDocument("<RTF/>");
						strTableName= "ROLE_TO_FUNCTIONS"; 
						break;
			case "RTFS":
						xmlDoc		= createDomDocument("<RTFS/>");
						strTableName= "ROLE_TO_FUNCTIONS"; 
						break;
			case "FSTF":
						xmlDoc		= createDomDocument("<FSTF/>");
						strTableName= "FUNC_SET_TO_FUNCS"; 
						break;
			case "ETS":
						xmlDoc		= createDomDocument("<ETS/>");
						appendAttr(xmlDoc, xmlDoc.firstChild, "app_code_name", m_objParam.appCodeName);
						appendAttr(xmlDoc, xmlDoc.firstChild, "use_scope", m_objParam.useScope);
						strTableName= "EXP_TO_SCOPES"; 
						break;
		}
		
		var root = xmlDoc.documentElement;
		var container = roleFunctionTableBody;
		
		var bAddAttr = false;

		for (var i = 0; i < container.all.length; i++)
		{
			var chk = container.all(i);

			if (chk.tagName == "INPUT" && chk.type == "checkbox")
				if (chk.checked != chk.oldValue)
				{
					if (chk.checked)
					{
						var node = root.selectSingleNode("Insert");
						if (!node) 
						{
							node = appendNode(xmlDoc, root, "Insert");
						}
						node = appendNode(xmlDoc, node, strTableName);
						node = appendNode(xmlDoc, node, "SET");
						appendNode(xmlDoc, node, strMain, chk.MainID);
						appendNode(xmlDoc, node, strSub, chk.SubID);
					}
					if (!chk.checked)
					{
						if ( !bAddAttr && (strType == "RTF" || strType == "RTFS"))
						{
							if (confirm("如果关系被继承，是否删除继承的关系？\n选'确定'为是，不选或选'取消'为否"))
								appendAttr(xmlDoc, xmlDoc.firstChild, "deleteSubApp", "y");
							else
								appendAttr(xmlDoc, xmlDoc.firstChild, "deleteSubApp", "n");
						}
						
						var node = root.selectSingleNode("Delete");
						if (!node) 
						{
							node = appendNode(xmlDoc, root, "Delete");
						}
						node = appendNode(xmlDoc, node, strTableName);
						addConditionFields(xmlDoc, node, strMain, "=", chk.MainID);
						addConditionFields(xmlDoc, node, strSub, "=", chk.SubID);
					}
				}
		}

		return xmlDoc;
	}
	
	function btnClick()
	{
		var sFeature = "dialogWidth:320px; dialogHeight:300px;center:yes;help:no;resizable:yes;scroll:no;status:no";
		var arg = new Object();
		return showModalDialog("../dialogs/editScope1.aspx", arg, sFeature);
	}
	
	
	//新建被授权对象
	function newUsers()
	{
		try
		{
			var xd = createDomDocument("<Config><RootOrg/><BottomRow>true</BottomRow></Config>");

			if (m_objParam.useScope == "y")
			{
				//得到当前用户机构服务范围
				var xmlResult; 
				xmlResult = queryUserFuncScopes(m_objParam.appCodeName, "ADD_OBJECT_FUNC", 3, 1);
				//***************************************
				//返回数据格式
				//<DataSet>
				//	<Table>
				//		<ORGANIZATIONS>中国海关\02广东分署\01办公室</ORGANIZATIONS>
				//		<ORGANIZATIONS>中国海关\01海关总署</ORGANIZATIONS>
				//		<ORGANIZATIONS>中国海关\01海关总署\27信息中心\应用开发二部</ORGANIZATIONS>
				//		<ORGANIZATIONS>中国海关\01海关总署\16国际合作司</ORGANIZATIONS>
				//	</Table>
				//</DataSet>
				//***************************************
		
				if (getAttrValue( xmlUserInfo.firstChild, "AdminUser" ) == "true")
				{
					var node = xmlUserInfo.selectSingleNode(".//OuUsers[Sideline='False']");
					var allpath = getSingleNodeText(node, ".//AllPathName", "");
					var pos = allpath.lastIndexOf("\\");
					if ( pos >= 0 )
						allpath = allpath.substring(0, pos);
						
					var i;
					for( i = 0; i < xmlResult.firstChild.firstChild.childNodes.length; i++)
					{
						if (xmlResult.firstChild.firstChild.childNodes[i].text == allpath)
							break;
					}
					
					if ( i >= xmlResult.firstChild.firstChild.childNodes.length )
						appendNode( xmlResult, xmlResult.firstChild.firstChild, "ORGANIZATIONS", allpath);
				}

				if (xmlResult.firstChild.firstChild.childNodes.length > 0)
					cloneNodes(xd.selectSingleNode(".//RootOrg"), xmlResult.firstChild.firstChild, true, "");
				else
				{
					alert("当前用户：没有被设定有效的服务范围，不能增加被授权对象");
					return;
				}	
			}
			else
					appendNode(xd, xd.selectSingleNode(".//RootOrg"), "ORGANIZATIONS", C_DC_ROOT_NAME);	
			
			
			xmlResult = showSelectUsersToRoleDialog(xd);
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
				
				//确定RoleId
				var strRoleId = getSingleNodeText(inputXNode, "ID");
				
				//确定级别
				var strUserRank = getNodeAttribute(root, "ACCESS_LEVEL");
				if (strUserRank != "")
					strUserRank = "UserRank(\""+strUserRank+"\", \"\>=\")";
				
				var strType, strObjId, strParentId, strAllPath, strExp, strClassify;
				
			/*********************************************************
				//添加被授权对象的格式
				<Insert app_code_name="ASDF" use_scope="y" role_classify="y">
					<EXPRESSIONS ALL_PATH_NAME=\"中国海关\01海关总署\00署领导\杨国勋" OBJ_ID="5e3aa542-29c3-40b5-b4cc-617045223c22">
						<SET>
							<EXPRESSION>BelongTo(USERS, "5e3aa542-29c3-40b5-b4cc-617045223c22", "65eb8160-f0fa-4f1c-8984-2649788fe1d0")</EXPRESSION>
							<ROLE_ID>ec16e6b8-5a94-4b9c-963c-08ace45dffd7</ROLE_ID>
							<NAME>杨国勋</NAME>
							<INHERITED>n</INHERITED>
							<CLASSIFY>0</CLASSIFY>
						</SET>
					</EXPRESSIONS>
				</Insert>
			************************************************************/
				
				var xmlDoc = createDomDocument("<Insert/>");
				appendAttr(xmlDoc, xmlDoc.firstChild, "app_code_name", m_objParam.appCodeName);
				appendAttr(xmlDoc, xmlDoc.firstChild, "use_scope", m_objParam.useScope);
				if (m_objParam.fatherNodeType == C_NODE_TYPE_B31)//自授权角色
					appendAttr(xmlDoc, xmlDoc.firstChild, "role_classify", "y");
				if (m_objParam.fatherNodeType == C_NODE_TYPE_B51)//应用角色
					appendAttr(xmlDoc, xmlDoc.firstChild, "role_classify", "n");
				
				var nodeSet;
				for(var i = 0; i < root.childNodes.length; i++)
				{
					
					strObjId		= getNodeAttribute(root.childNodes[i], "GUID");

						
						
					strType			= getNodeAttribute(root.childNodes[i], "OBJECTCLASS");
					strAllPath		= getNodeAttribute(root.childNodes[i], "ALL_PATH_NAME");
					strParentId		= getNodeAttribute(root.childNodes[i], "PARENT_GUID");
					strName			= getNodeAttribute(root.childNodes[i], "DISPLAY_NAME");
					switch(strType)
					{
						case "USERS":
							strClassify = "0";
							break;
						case "ORGANIZATIONS":
							strClassify = "1";
							break;
						case "GROUPS":
							strClassify = "2";
							break;
					}
					
					strExp			= "BelongTo("+strType+", \""+strObjId+"\", \""+strParentId+"\")";
					if (strType != "USERS" && strUserRank != "")
						strExp = strExp + "\&\&" + strUserRank;
						
					//跳过已存在的被授权对象
//					if (isExistExp(strClassify, strAllPath, strObjId))
//						continue;
					nodeSet = appendNode(xmlDoc, xmlDoc.documentElement, "EXPRESSIONS");
					appendAttr(xmlDoc, nodeSet, "ALL_PATH_NAME", strAllPath);
					appendAttr(xmlDoc, nodeSet, "OBJ_ID", strObjId);
					nodeSet = appendNode(xmlDoc, nodeSet, "SET");
					appendNode(xmlDoc, nodeSet, "EXPRESSION", strExp);
					appendNode(xmlDoc, nodeSet, "ROLE_ID", strRoleId);
					appendNode(xmlDoc, nodeSet, "NAME", strName);
					appendNode(xmlDoc, nodeSet, "INHERITED", m_objParam.inherited);
					appendNode(xmlDoc, nodeSet, "CLASSIFY", strClassify);
				}
				//alert(xmlDoc.xml);
				if (xmlDoc.firstChild.childNodes.length > 0)
				{
					var xmlInsert = xmlSend("../XmlRequestService/XmlAOSWriteRequest.aspx", xmlDoc);
								
					checkErrorResult(xmlInsert);
					refreshList();
				}
			}
		}
		catch(e)
		{
			showError(e);
		}
	}
	
	//是否已经存在此表达式
	function isExistExp(strClassify, strAllPath, strObjId)
	{
		var root = inputXml.documentElement;
		
		for (var i = 0; i < root.childNodes.length; i++)
		{
			if ( strObjId == getSingleNodeText(root.childNodes[i], ".//GUID") )
				return true;
		}
		return false;
	}

	
//-->