<!--
	
	var xmlUserInfo; 
	var m_parentParams = null;	//父窗口传递过来的参数
	var m_curNodeKey = "";//在右窗显示细节的结点key值	
	function getParentParams()
	{
		if (window.parent != this)
		{
			var params = window.parent.parentParams;
			
			if (params)
				m_parentParams = createDomDocument(params.value);
		}
	}

	//从父页面得到标题
	function getCaption()
	{
		var strCaption = "";

		if (m_parentParams)
			strCaption = getSingleNodeText(m_parentParams.documentElement, "Caption", "");
		
		return strCaption;
	}

	//从父页面得到缺省得应用程序名称
	function getDefaultAppCodeName()
	{
		var strDefaultAppName = "";

		if (m_parentParams)
			strDefaultAppName = getSingleNodeText(m_parentParams.documentElement, "DefaultAppName", "");
		
		return strDefaultAppName;
	}

	//从父页面得到受限制的应用程序信息
	function getRestrictAppNames()
	{
		var strNames = "";

		if (m_parentParams)
			strNames = getSingleNodeText(m_parentParams.documentElement, "RestrictAppNames", "");
		
		return strNames;
	}

/*
	//向WebServer发送请求，得到查询结果
	function queryInfo(strType, appID, refAppID, resourceFlag)
	{	
		var xmlDoc;	
		if (resourceFlag == "RESOURCES")
			xmlDoc = createDomDocument("<queryResourceList/>");
		else
			xmlDoc = createDomDocument("<queryList/>");
		appendAttr(xmlDoc, xmlDoc.documentElement, "type", strType);

		if (appID)
			appendAttr(xmlDoc, xmlDoc.documentElement, "appID", appID);

		if (refAppID)
			appendAttr(xmlDoc, xmlDoc.documentElement, "refAppID", refAppID);
			
		var xmlResult = xmlSend("./AppAdminOP/AppDBReader.aspx", xmlDoc);
		checkErrorResult(xmlResult);
			
		return xmlResult;
	}
	
	//查询子资源的集合
	function queryResource(strType, appID, refAppID)
	{		
		var xmlDoc = createDomDocument("<queryResource/>");
		appendAttr(xmlDoc, xmlDoc.documentElement, "type", strType);

		if (appID)
			appendAttr(xmlDoc, xmlDoc.documentElement, "appID", appID);

		if (refAppID)
			appendAttr(xmlDoc, xmlDoc.documentElement, "refAppID", refAppID);

		var xmlResult = xmlSend("./AppAdminOP/AppDBReader.aspx", xmlDoc);
		checkErrorResult(xmlResult);

		return xmlResult;
	}
*/
	//查询strRestrictNames中指定的应用集合
	function queryRestrictAppInfo(strRestrictNames)
	{
		var xmlDoc = createDomDocument("<queryRestrictApps/>");
		appendAttr(xmlDoc, xmlDoc.documentElement, "restrictNames", strRestrictNames);

		var xmlResult = xmlSend("./AppAdminOP/AppDBReader.aspx", xmlDoc);
		checkErrorResult(xmlResult);

		return xmlResult;
	}

	//加载某个子节点数据
	function loadAppChildren(n)
	{
		var xmlResult;
		var xmlResult2;
		if (n.xData.type == C_NODE_TYPE_A7)//应用集合
		{
			var xmlResult = queryApplication(n.xData.appID);
			checkErrorResult(xmlResult);

			n.xData.waitforLoad = false;
			n.removeChildren();
			n.xData.xml = xmlResult;
			showApplications(xmlResult.documentElement.firstChild, n);
		}
		else//其它集合结点
		{
			//进行查询
			switch (n.xData.type)
			{
				case C_NODE_TYPE_A2://应用服务范围集合
					xmlResult = queryScope( n.xData.appID );
					break;
				case C_NODE_TYPE_A3://自授权角色集合
					xmlResult = querySelfRole( n.xData.appID );
					break;
				case C_NODE_TYPE_A4://自授权功能树
					xmlResult = querySelfFunc( n.xData.appID );
					break;
				case C_NODE_TYPE_A5://应用角色集合
					xmlResult = queryAppRole( n.xData.appID );
					break;
				case C_NODE_TYPE_A6://应用功能树
					xmlResult = queryAppFunc( n.xData.appID );
					break;
				case C_NODE_TYPE_B42://自授权FunctionSet
					if (getSingleNodeText(n.xData.DomNode,"LOWEST_SET","") == "y")
						xmlResult = queryFuncSet2Function( n.xData.appID, getSingleNodeText( n.xData.DomNode, "ID", ""));
					else 
						xmlResult = querySelfFunc( n.xData.appID, getSingleNodeText( n.xData.DomNode, "ID", "") );
					xmlResult2 = queryFuncSet2Role( n.xData.appID, getSingleNodeText( n.xData.DomNode, "ID", "") );
					break;
				case C_NODE_TYPE_B62://应用授权FunctionSet
					if (getSingleNodeText(n.xData.DomNode,"LOWEST_SET","") == "y")
						xmlResult = queryFuncSet2Function( n.xData.appID, getSingleNodeText( n.xData.DomNode, "ID", ""));
					else 
						xmlResult = queryAppFunc( n.xData.appID, getSingleNodeText( n.xData.DomNode, "ID", "") );
					xmlResult2 = queryFuncSet2Role( n.xData.appID, getSingleNodeText( n.xData.DomNode, "ID", "") );
					break;
				
				case C_NODE_TYPE_B31://自授权角色
				case C_NODE_TYPE_B51://应用角色
					xmlResult = queryRole2Exp( n.xData.appID, getSingleNodeText( n.xData.DomNode, "ID", "") );
					break;

				case C_NODE_TYPE_B41://自授权功能
				case C_NODE_TYPE_B61://应用功能
					xmlResult = queryFunction2Role( n.xData.appID, getSingleNodeText( n.xData.DomNode, "ID", "") );
					break;
				case C_OBJ_TYPE_A1://人员
				case C_OBJ_TYPE_A2://机构	
				case C_OBJ_TYPE_A3://组
					xmlResult = queryExpScope( n.xData.appID, getSingleNodeText( n.xData.DomNode, "ID", "") );
					break;
			}
			
			checkErrorResult(xmlResult);
			if (xmlResult2) 
				checkErrorResult(xmlResult2);
			
			n.xData.waitforLoad = false;
			n.removeChildren();
			if (xmlResult2)
			{
				n.xData.xml = xmlResult2;
				n.xData.xml2 = xmlResult;
			}
			else
				n.xData.xml = xmlResult;
			
			switch (n.xData.type)
			{
				
				case C_NODE_TYPE_B31://自授权角色
				case C_NODE_TYPE_B51://应用角色
						if (getUseScope(n) == "n") return;
						break;

				case C_OBJ_TYPE_A1://人员
				case C_OBJ_TYPE_A2://机构	
				case C_OBJ_TYPE_A3://组
				case C_NODE_TYPE_A2://应用服务范围集合
				case C_NODE_TYPE_B41://自授权功能
				case C_NODE_TYPE_B61://应用功能
						return;
						break;
			}

		
			//去掉<Table1>结点，显示"功能集合"的内容时，<Table1>是功能集合所拥有的所有功能
			var xmlShow = createDomDocument("<DataSet/>");
			cloneNodes(xmlShow.documentElement, xmlResult.documentElement, true, "Table1");

			showAppChildrenMember(xmlShow, n);			
		}
	}
	//添加服务范围、角色、功能等子节点的操作
	//杨睿2004-4-7
	function addItemNode(node, nFather, strFatherType, index)
	{
		
		var strType		= getTypeFromFather(strFatherType, node);
		var strName		= getSingleNodeText( node, "DISPLAY_NAME", "");
		if (strName == "")
			strName	= getSingleNodeText( node, "NAME", "");

		var nMb = nFather.add("tvwChild",nFather.key+"/"+index, strName, "./images/" + getImgFromType(strType));
		
		nMb.xData.type		= strType;
		nMb.xData.DomNode	= node;
		nMb.xData.appID		= getAppID( nMb );
		nMb.xData.appLevel	= getAppLevel( nMb );
		nMb.xData.appCodeName	= nFather.xData.appCodeName;
		nMb.xData.useScope	= nFather.xData.useScope;
		switch ( strType )
		{
			case C_NODE_TYPE_B31://自授权角色
			case C_NODE_TYPE_B51://应用角色
			case C_OBJ_TYPE_A1:		//人员
			case C_OBJ_TYPE_A2:		//机构	
			case C_OBJ_TYPE_A3:		//组
							nMb.xData.waitforLoad = true;
							if (getUseScope(nMb) == "y")
							{
								addLoadingNode(nMb);
							}
							break;
			
			case C_NODE_TYPE_B41://自授权功能
			case C_NODE_TYPE_B61://应用功能
							nMb.xData.waitforLoad = true;
							break;
							
			case C_NODE_TYPE_B42://自授权功能集合
			case C_NODE_TYPE_B62://应用功能集合
							nMb.xData.waitforLoad = true;
							addLoadingNode(nMb);
							break;
			
			default:
							nMb.xData.waitforLoad = false;
		}

	}

	function showAppChildrenMember(xNode, nFather)
	{
		var node = xNode.selectSingleNode(".//Table");
		var i = 1;
		while(node)
		{
			//只显示功能集合中有的功能
			if (nFather.xData.type == C_NODE_TYPE_B42 || nFather.xData.type == C_NODE_TYPE_B62)
				if ( getSingleNodeText(nFather.xData.DomNode, "LOWEST_SET", "") == "y" && getSingleNodeText(node, "FUNC_SET_ID", "") == "" )
				{
					node = node.nextSibling;
					continue;
				}
			
			if (nFather.xData.type == C_OBJ_TYPE_A1 || nFather.xData.type == C_OBJ_TYPE_A2 || nFather.xData.type == C_OBJ_TYPE_A3)
				if ( getSingleNodeText(node, "EXP_ID", "") == "" )
				{
					node = node.nextSibling;
					continue;
				}

			addItemNode(node, nFather, nFather.xData.type, i);
			node = node.nextSibling;
			i++;
		}
	}

	//设置当前节点为正在载入的节点
	function addLoadingNode(n)
	{
		n.xData.waitforLoad = true;
		n.add("tvwChild", "", "载入中...");
	}

	//为结构树增加一个应用的子节点，被showApplications（）调用
	//杨睿２００４－４－７
	function addApplicationSubNode(appNode, strName, strType, strImage)
	{
		var nMb = appNode.add("tvwChild", appNode.key+"/"+strType, strName, "./images/" + strImage);
		nMb.xData.type			= strType;
		nMb.xData.DomNode		= appNode.xData.DomNode;
		nMb.xData.appID			= appNode.xData.appID;
		nMb.xData.appLevel		= appNode.xData.appLevel;
		nMb.xData.appCodeName	= appNode.xData.appCodeName;
		nMb.xData.useScope		= appNode.xData.useScope;
		
		addLoadingNode(nMb);//展开后载入数据
	}

	//显示应用树
	//杨睿２００４－４－７
	function showApplications(eleRoot, nFather, bManagedApp, bMaskFuncs)
	{
		var node = eleRoot;
	
		while(node)
		{
			var strCodeName = node.selectSingleNode("CODE_NAME").text;
			
			//if (nAppID != C_APP_ADMIN_ID || !bManagedApp)
			{

				var strAppName		= getSingleNodeText(node, "NAME", "");
				var strAppCodeName	= getSingleNodeText(node, "CODE_NAME", "");
				var strAppId		= getSingleNodeText(node, "ID", "");
				var strAppLevel		= getSingleNodeText(node, "RESOURCE_LEVEL", "");
				var strUseScope		= getSingleNodeText(node, "USE_SCOPE", "");
				
				//设置图标
				var strImage;
				strImage = "application.gif";
				if (strCodeName == C_APP_ADMIN_CODE_NAME)
					strImage = "computer.gif";
				
				//增加子结点
				var nApp = nFather.add("tvwChild", nFather.key+"/"+strAppId, strAppName, "./images/" + strImage);
				if (strCodeName == C_APP_ADMIN_CODE_NAME)
					nApp.xData.type				= C_NODE_TYPE_B71;
				else
					nApp.xData.type				= C_NODE_TYPE_B72;
				nApp.xData.appID			= strAppId;
				nApp.xData.appCodeName		= strAppCodeName;
				nApp.xData.appLevel			= strAppLevel;
				nApp.xData.useScope			= strUseScope;
				nApp.xData.DomNode			= node;
				nApp.xData.waitforLoad		= false;
				

				if (getCodeName(node) == C_APP_ADMIN_CODE_NAME)
					addApplicationSubNode(nApp, "角色", C_NODE_TYPE_A3, "folder.gif");
				if (getCodeName(node) != C_APP_ADMIN_CODE_NAME)
				{
					if (getSingleNodeText(node, "USE_SCOPE") == "y" && getSingleNodeText(node, "APP_SCOPES") == "True")
						addApplicationSubNode(nApp, "服务范围", C_NODE_TYPE_A2, "folder.gif");
					
					if (getSingleNodeText(node, "SELF_MAINTAIN_FUNC") == "True" )
					{
						addApplicationSubNode(nApp, "自授权角色", C_NODE_TYPE_A3, "folder.gif");
						addApplicationSubNode(nApp, "自授权功能", C_NODE_TYPE_A4, "folder.gif");
					}

					if (getSingleNodeText(node, "APP_ROLES") == "True" )
						addApplicationSubNode(nApp, "应用授权角色", C_NODE_TYPE_A5, "folder.gif");
					
					if (getSingleNodeText(node, "APP_FUNCTIONS") == "True" )
						addApplicationSubNode(nApp, "应用授权功能", C_NODE_TYPE_A6, "folder.gif");
					
					if (getSingleNodeText(node, "ADD_SUBAPP") == "y" )
						addApplicationSubNode(nApp, "子应用", C_NODE_TYPE_A7, "folder.gif");
				}
			}

			node = node.nextSibling;
		}
	}

	//展开缺省的应用程序
	function setDefaultApplication()
	{
		var strAppName = getDefaultAppCodeName();

		if (strAppName.length > 0)
		{
			var nodeApp = tv.Nodes[0].firstChild;

			while(nodeApp)
			{
				if (nodeApp.xData.codeName == strAppName)
				{
					tv.selectedItem = nodeApp;
					nodeApp.setExpanded(true);
					nodeApp.firstChild.setExpanded(true);
					break;
				}
				nodeApp = nodeApp.getNext();
			}
		}
	}

	//初始化授权树的结构
	function getRoot()
	{
		//获得应用列表
		var xmlResult = null;
/*
		if (m_parentParams)
		{
			var strRestrictNames = getRestrictAppNames();
			xmlResult = queryRestrictAppInfo(strRestrictNames);
		}
		else
			xmlResult = queryInfo("APPLICATIONS");
*/
		xmlResult = queryApplication("");
		var eleRoot = xmlResult.documentElement;


//		var nRoot = tv.Nodes.add("", "", "root", "应用程序", "images/drct.gif");
		var nRoot = tv.Nodes[0];
//		tv.Nodes.clear();
		nRoot.removeChildren();

		nRoot.xData.type = C_NODE_TYPE_A1;
		nRoot.xData.appID = "";
		nRoot.xData.appCodeName = "";
		nRoot.xData.appLevel = "";
		nRoot.xData.useScope = "";
		nRoot.xData.waitforLoad = false;
		nRoot.xData.xml = xmlResult;
		nRoot.xData.DomNode = createDomDocument("<Table/>");

		showApplications(eleRoot.firstChild, nRoot);
		nRoot.setExpanded(true);

		//setDefaultApplication();
	}

/*
	//这里是父页面要求采用设置参数strParam在其子页面中，
	//通过设置改参数来实现子、父页面中数据的同步
	function openInnerDocument(strURL, strParam)
	{
		paramValue.value = strParam;

		var objInputParam = frmContainer.document.all("inputParam");

		if (!objInputParam)
			innerDocTD.innerHTML = "<iframe id='frmContainer' src='" + strURL + "' " + 
				"style='WIDTH:100%;HEIGHT:100%' frameborder='0' scrolling='auto'></iframe>";
		else
			objInputParam.value = strParam;
	}
*/
	//这里是父页面要求采用设置参数strParam在其子页面中，
	//通过设置改参数来实现子、父页面中数据的同步
	//新的方法，杨睿２００４－４－９
	function openInnerDocument(strURL, node)
	{
		if (!node.xData.xml)
		{
			inputXML.value = "";
			innerDocTD.innerHTML = "<iframe id='frmContainer' " + 
				"style='WIDTH:100%;HEIGHT:100%' frameborder='0' scrolling='auto'></iframe>";
		}
		else
		{
			setInputContent( node );
		
			showSubPage(strURL);
		}	
	}

	//展开应用树结点
	function tvNodeExpand()
	{
		try
		{
			var n = event.node;

			if (n.xData.waitforLoad)
			{
				loadAppChildren(n);
			}
			
		}
		catch(e)
		{
			showError(e);
		}
	}

	//授权树中结点被选中的操作
	function tvNodeSelected()
	{
		try
		{
//原有的代码:		
/*			var xNode = event.node.xData.xNode;

			if (xNode && typeof(xNode) == "object")
			{
				var nAppID = 0;
				var nObjID = 0;
				var refAppID = 0;
				var resourceFlag = 0;
				var nAppLevel = 0;
				var appCodeName = 0;

				if (xNode.appID)
					nAppID = xNode.appID;

				if (nAppID == 0 && getRestrictAppNames().length > 0)
					event.returnValue = false;
				else
				{
					if (xNode.objID)
						nObjID = xNode.objID;

					if (xNode.refAppID)
						refAppID = xNode.refAppID;

					if (xNode.resourceFlag)
						resourceFlag = xNode.resourceFlag;

					if (xNode.appLevel)
						nAppLevel = xNode.appLevel;

					if (xNode.appCodeName)
						appCodeName = xNode.appCodeName;

					openInnerDocument("./itemList/appObjectList.aspx", xNode.type + "&" + nAppID + "&" + nAppLevel +
										"&" + nObjID + "&" + refAppID + "&" + resourceFlag + "&" + appCodeName);
				}
			}
			else
			if (typeof(xNode) == "number" && event.node.key)	//Application
				event.node.setExpanded(!event.node.getExpanded());
			else
				event.returnValue = false;
*/	
//新的代码:杨睿2004-4-9
			if ( m_curNodeKey == event.node.key )
				return;
			m_curNodeKey = event.node.key ;
			event.node.setExpanded(true);
			//var xml = event.node.xData.xml;
			//if (xml) 
				openInnerDocument("./itemList/appObjectList.aspx", event.node);
			
		}
		catch(e)
		{
			showError(e);
		}
	}

   
	//刷新子树结构
	function refreshSubNode(n)
	{
		if (!n.xData.waitforLoad)
		{
			var type = n.xData.type;
			var xNode = n.xData.DomNode;
			switch( type )
			{
				case C_NODE_TYPE_A1: 	//根结点
										getRoot();
										break;
				
				case C_NODE_TYPE_A2:	//应用服务范围集合
				case C_NODE_TYPE_A3:	//自授权角色集合
				case C_NODE_TYPE_A4:	//自授权功能树
				case C_NODE_TYPE_A5:	//应用角色集合
				case C_NODE_TYPE_A6:	//应用功能树
				case C_NODE_TYPE_A7:	//应用集合
				case C_NODE_TYPE_B42:	//自授权功能集合
				case C_NODE_TYPE_B62:	//应用功能集合
				case C_NODE_TYPE_B31:	//自授权角色
				case C_NODE_TYPE_B41:	//自授权功能
				case C_NODE_TYPE_B51:	//应用角色
				case C_NODE_TYPE_B61:	//应用功能
				case C_OBJ_TYPE_A1:		//人员
				case C_OBJ_TYPE_A2:		//机构	
				case C_OBJ_TYPE_A3:		//组
										loadAppChildren(n);
										break;
				
				case C_NODE_TYPE_B21:	//机构服务范围
				case C_NODE_TYPE_B22:	//数据服务范围
				case C_NODE_TYPE_B71:	//通用授权应用
				case C_NODE_TYPE_B72:	//一般应用
										//alert("no refresh");
										break;
			}
			return n;
		}
	}

	//授权树中的应用导出操作
	function exportAppData(n)
	{
		var xNode = n.xData.xNode;
		var appID = null;

		if (typeof(xNode) != "object")
			appID = xNode;

		if (appID)
		{
			window.location = "view-source:" + getCurrentDir(document.URLUnencoded) + 
								"dialogs/exportAppData.aspx?appID=" + appID;
		}
	}

	//授权树的应用倒入操作
	function importAppData(n)
	{
		var sFeature = "dialogWidth:480px; dialogHeight:360px;center:yes;help:no;resizable:no;scroll:no;status:no";

		var sPath = "/" + getRootDir(document.URLUnencoded) + "/dialogs/importAppData.htm";

		var strXml = showModalDialog(sPath, null, sFeature);

		if (strXml.length > 0)
		{
			var xmlDoc = createDomDocument(strXml);
			xmlDoc.documentElement.setAttribute("appID", n.xData.xNode);

			var xmlResult = xmlSend("./AppAdminOP/AppDBWriter.aspx", xmlDoc.xml);

			checkErrorResult(xmlResult);

			if (xmlResult) refreshSubNode(n);
		}
	}

	//在授权树中展现人员列表在对应的角色下
	function showUsersInRoleFromNode(tNode)
	{
		var type = tNode.xData.xNode.type;
		var xNode = tNode.xData.xNode;
		var appID;

		if (typeof(tNode.xData.xNode) == "number")
			appID = tNode.xData.xNode;
		else
			appID = xNode.appID;

		var roleID = null;

		if (type == "USERS_TO_ROLES")
			roleID = xNode.objID;
			
		var refApp = null;
		if (tNode.xData.refAppType && tNode.xData.refAppType == "true")
			refApp = "true";
		showUsersInRole(appID, roleID, refApp);
	}

	//
	function showUsersInRole(appID, roleID, refApp)
	{
		var strURL = "./dialogs/roleUserChart.aspx?appID=" + appID;

		if (roleID)
			strURL += "&roleID=" + roleID;
		if (refApp)
			strURL += "&refApp=" + refApp;

		window.open(strURL);
	}

	//初始化菜单之前的控制
	function menuBeforePopup()
	{
		var tNode = tv.getItemByKey(m_curNodeKey);
		var type = tNode.xData.type;

		switch(event.menuData)
		{
			/*
			case "export":	event.disableItem = (typeof(xNode) == "object" || (tNode.xData.refAppType && tNode.xData.refAppType == "true"));
							break;
			case "import":	event.disableItem = (typeof(tNode.xData.xNode) != "number" || (tNode.xData.refAppType && tNode.xData.refAppType == "true"));
							break;
			case "showUsersInRole":
							event.disableItem = typeof(tNode.xData.xNode) != "number" && type != "ROLES" && type != "USERS_TO_ROLES";
							break;
			case "viewFuncs":
			case "editRoles":event.disableItem = (tNode.xData.resourceFlag != "RESOURCES" && tNode.xData.resourceFlag != "APPLICATIONS");
							break;
			case "viewRoles":event.disableItem = tNode.xData.resourceFlag != "RESOURCES";
							break;
			*/
			case "refresh":	
							event.disableItem = (!tNode.xData.xml)
							break;
			case "property":
							switch (type)
							{
								case C_NODE_TYPE_A1: 	//根结点
								case C_NODE_TYPE_A2:	//应用服务范围集合
								case C_NODE_TYPE_A3:	//自授权角色集合
								case C_NODE_TYPE_A4:	//自授权功能树
								case C_NODE_TYPE_A5:	//应用角色集合
								case C_NODE_TYPE_A6:	//应用功能树
								case C_NODE_TYPE_A7:	//子应用集合
								case C_OBJ_TYPE_A1:		//人员
								case C_OBJ_TYPE_A2:		//机构	
								case C_OBJ_TYPE_A3:		//组
														event.disableItem = true;
														break;

								case C_NODE_TYPE_B21:	//机构服务范围
								case C_NODE_TYPE_B22:	//数据服务范围
								case C_NODE_TYPE_B31:	//自授权角色
								case C_NODE_TYPE_B41:	//自授权功能
								case C_NODE_TYPE_B42:	//自授权功能集合
								case C_NODE_TYPE_B51:	//应用角色
								case C_NODE_TYPE_B61:	//应用功能
								case C_NODE_TYPE_B62:	//应用功能集合
								case C_NODE_TYPE_B71:	//通用授权应用
								case C_NODE_TYPE_B72:	//一般应用
														event.disableItem = false;
														break;
							}

							break;
			case "showFuncSetContent":
							event.disableItem = ( (type != C_NODE_TYPE_B42 && type != C_NODE_TYPE_B62) || !tNode.xData.xml2);
							break;
			case "modifyFuncForRole":
							event.disableItem = (type != C_NODE_TYPE_B51 && type != C_NODE_TYPE_B31);		//角色
							break;
			case "queryUserAppRoles":
							event.disableItem = true;
							switch (type)
							{
								//case C_NODE_TYPE_A1: 	//根结点
								case C_NODE_TYPE_B71:	//通用授权应用
								case C_NODE_TYPE_B72:	//一般应用
														event.disableItem = false;
														break;
							}
							break;
							
		}
	}
	
	//菜单被点击的的事件响应	
	function menuTreeClick()
	{
		
		var tNode = tv.getItemByKey(m_curNodeKey);
		var type = tNode.xData.type;

		try
		{
			if (tNode)
			{
				switch(event.menuData)
				{
					case "refresh":	refresh(tNode);
									break;
					/*
					case "import":	importAppData(tNode);
									break;
					case "export":	exportAppData(tNode);
									break;
					case "viewFuncs":viewFuncUser(tNode);
									break;
					case "editRoles":editRoles(tNode);
									break;
					case "viewRoles":viewRoles(tNode);
									break;
					case "showUsersInRole":
									showUsersInRoleFromNode(tNode);
									break;
					case "searchResource":
									searchResource();									
									break;
					*/
					case "showFuncSetContent":
									setInputContent2( tNode );
									showSubPage("./itemList/appObjectList.aspx");
									break;
					case "property":
									updateProperty( tNode );
									break;
					case "modifyFuncForRole":
									modifyRTF( tNode )
									break;
					case "queryUserAppRoles":
									//queryUserRoles( tNode );
									break;
									
				}
			}
		}
		catch(e)
		{
			showError(e);
		}
	}	

function modifyRTF( tNode )
	{
		var appID			= tNode.xData.appID;
		var objID			= getID( tNode );
		var readOnly		= "false";
		if (tNode.xData.type == C_NODE_TYPE_B31 )	//自授权角色
			readOnly = "true";

		
		var bResult = showModalDialog("/" + C_APP_ADMIN_ROOT_URI + "/dialogs/FunctionForRole.aspx?rID="+objID+"&aID="+appID+"&read="+readOnly, "", "center:yes;help:no;status:no;resizable:yes;dialogWidth:700px")
		if (bResult)
			refresh(tNode);
	}
	
	
function queryUserRoles( tNode )
	{
		var appName	
		var appScope		
		if ( tNode.xData.appCodeName != null )
		{
			appName = tNode.xData.appCodeName;
			appScope = tNode.xData.useScope;
		}
		window.open("/" + C_APP_ADMIN_ROOT_URI + "/dialogs/QueryUserAppRoles.aspx?aName="+appName+"&aScope="+appScope, "Material", "menubar=no,toolbar=no,location=no,resizable=yes");
		//var bResult = showModalDialog("/" + C_APP_ADMIN_ROOT_URI + "/dialogs/QueryUserAppRoles.aspx?aName="+appName, "center:yes;help:no;status:no;resizable:yes;dialogWidth:700px")
	}

/*	
	function searchResource()
	{
		var sPath = "/" + C_APP_ADMIN_ROOT_URI + "/dialogs/findSource.htm";
		
		var dHeight = parseInt(window.screen.height * 0.4);
		var dWidth = parseInt(window.screen.width * 0.4)
		var sFeature = "dialogWidth:" + dWidth + "px; dialogHeight:" + dHeight + "px;center:yes;help:no;resizable:yes;scroll:no;status:no";
		
		var xmlDoc = showModalDialog(sPath, null, sFeature);
		if (xmlDoc != null)
		{
			var xmlResult = xmlSend("./AppAdminOP/AppDBReader.aspx", xmlDoc.xml);
			checkErrorResult(xmlResult);
			var root = xmlResult.documentElement;
			
			trueThrow(root.childNodes.length == 0, "对不起，没有找到您指定的资源！");
			
			var strResourceLevel = null;
			if (root.childNodes.length > 1)
			{
				strResourceLevel = showModalDialog("./dialogs/selectSearchResource.htm", xmlResult, sFeature);
				trueThrow(strResourceLevel == null, "对不起，您没有选择指定的资源！");
			}
			else			
				strResourceLevel = root.firstChild.selectSingleNode("RESOURCE_LEVEL").text;
				
			var rootApp = tv.Nodes[0];
			
			for(var i = 1; i <= strResourceLevel.length / 4; i++)
			{
				var appLevel = strResourceLevel.substring(0, 4 * i);
				
				var appNode = getResByLevel(rootApp, appLevel);
				
				if (appNode == null)
				{
					appNode = getResByLevel(rootApp, appLevel.substring(0, appLevel.length - 4));
					if (appNode)
					{
						appNode.setExpanded(true, false);				
						rfRoot = getRoleFunctionRoot(appNode, "RESOURCES");
						rfRoot.setExpanded(true, false);
						appNode = getResByLevel(rootApp, appLevel);
					}
				}
				if (appNode)
				{
					appNode.setExpanded(true, false);
					rfNode = getRoleFunctionRoot(appNode, "RESOURCES");
					if (appLevel != strResourceLevel)
						rfNode.setExpanded(true, false);
				}
			}			
		}
	}
*/	
	function editRoles(tNode)
	{
		var xData = tNode.xData;
		
		var arg = new Object();
		
//		arg.objID = xData.xNode;
//		arg.codeName = xData.codeName;
		arg.parentLevel = xData.parentLevel;
//		arg.resourceFlag = xData.resourceFlag;
//		arg.appName = xData.appName;
		
//		arg.roleCodeName = "ORIGINAL_ADMIN,CURRENT_RESOURCE_READER";
		
		var sPath = "/" + C_APP_ADMIN_ROOT_URI + "/dialogs/editResRoles.htm";
		var sFeature = "dialogWidth:800px; dialogHeight:600px;center:yes;help:no;resizable:yes;scroll:no;status:no";
		
		var xmlResult = showModalDialog(sPath, arg, sFeature);
	}
	
	function viewRoles(tNode)
	{
		var xData = tNode.xData;
		
		var arg = new Object();
		arg.resLevel = xData.parentLevel;
		
		var sPath = "/hgzs_web/docAdmin/userRoleView.htm";
		//var dWidth = parseInt(window.screen.width * 0.4);
		var dHeight = parseInt(window.screen.height * 0.7);
		var dWidth = parseInt(dHeight * 0.85)
		var sFeature = "dialogWidth:" + dWidth + "px; dialogHeight:" + dHeight + "px;center:yes;help:no;resizable:yes;scroll:auto;status:no";
		
		var xmlResult = showModalDialog(sPath, arg, sFeature);
	}
	
	function viewFuncUser(tNode)
	{
		var xData = tNode.xData;
		
		var xmlDoc = createDomDocument("<searchResFuncUser/>");
		var root = xmlDoc.documentElement;
		appendAttr(xmlDoc, root, "parentLevel", xData.parentLevel);

		var funcNode = appendNode(xmlDoc, root, "func");
		appendNode(xmlDoc, funcNode, "funcCodeName", "ORIGINAL_RES_ADD_CHILD");
		appendNode(xmlDoc, funcNode, "displayName", "增加子栏目或文件");
		
		funcNode = appendNode(xmlDoc, root, "func");
		appendNode(xmlDoc, funcNode, "funcCodeName", "ORIGINAL_RES_UPDATE");
		appendNode(xmlDoc, funcNode, "displayName", "修改栏目或文件属性");
		
		funcNode = appendNode(xmlDoc, root, "func");
		appendNode(xmlDoc, funcNode, "funcCodeName", "ORIGINAL_RES_DELETE");
		appendNode(xmlDoc, funcNode, "displayName", "删除栏目或文件");
		
		funcNode = appendNode(xmlDoc, root, "func");
		appendNode(xmlDoc, funcNode, "funcCodeName", "ORIGINAL_UTR_ADMIN");
		appendNode(xmlDoc, funcNode, "displayName", "角色中用户的调整");
		
		var sPath = "/" + C_APP_ADMIN_ROOT_URI + "/dialogs/funcUserChart.htm";
		var sFeature = "dialogWidth:800px; dialogHeight:600px;center:yes;help:no;resizable:yes;scroll:no;status:no";
		
		var arg = new Object();
		arg.dataXml = xmlDoc.xml;
		var xmlResult = showModalDialog(sPath, arg, sFeature);
	}

	function tvNodeRightClick()
	{
		try
		{
			var tNode = event.node;

			if (tNode)
			{
				//显示菜单
				//menuTree.lastNode = tNode;
				m_curNodeKey = tNode.key;
				menuTree.show(event.x, event.y);
			}
		}
		catch(e)
		{
			showError(e);
		}
	}

	function onDocumentLoad()
	{
		try
		{
			//获得使用者身份
			
			xmlUserInfo = createDomDocument(userInfo.value);
			//产生标题
			document.title += "(" + xmlUserInfo.selectSingleNode(".//UserDisplayName").text + ")";//设置标题中的身份

			initSplitter(splitterContainer);
			tv.Nodes.add("", "", "root", "应用程序", "images/drct.gif");
			getRoot();
		}
		catch(e)
		{
			showError(e);
		}
	}

	//在授权树的结构中采用RESOURCE_LEVEL的结构查询对应的应用节点或者资源节点所在的位置
	function getResByLevel(root, level)
	{
		var node = root.getChild();
		
		while (node)
		{
			var parentLevel = node.xData.parentLevel;
			
			if (parentLevel == level)
				return node;
			else
			{
				if (parentLevel && parentLevel == level.substring(0, parentLevel.length))
					return getResByLevel(getRoleFunctionRoot(node, "RESOURCES"), level);
				else
					node = node.getNext();
			}
		}
		return null;
	}

	function getRoleFunctionRoot(nodeApp, strType)
	{
		var nodeRoot = nodeApp.getChild();
		
		while (nodeRoot && nodeRoot.xData.xNode.type != strType)
			nodeRoot = nodeRoot.getNext();

		return nodeRoot;
	}

	function getRoleFunctionByID(nodeRoot, strType, id)
	{
		var node = nodeRoot.getChild();

		while(node)
		{
			if ((strType != "RESOURCES" && node.xData.xNode.objID == id) || 
				(strType == "RESOURCES" && node.xData.xNode == id))
				break;
			
			node = node.getNext();
		}

		return node;
	}

	function syncApplicationByRoot(xmlDoc, root, nodeBase, bManaged)
	{
		var strName = null;

		if (xmlDoc.documentElement.selectSingleNode("NAME"))
			strName = xmlDoc.documentElement.selectSingleNode("NAME").text;

		switch(xmlDoc.documentElement.getAttribute("op"))
		{
			case "Insert":	if (!nodeBase)
								showApplications(xmlDoc.documentElement, root, bManaged);
							break;
			case "Update":	if (nodeBase && strName)
								nodeBase.putText(strName);
							if (nodeBase)
							{
								var strContainer = getSingleNodeText(xmlDoc.documentElement, "CONTAINER", "");
								if (strContainer == "y")
									addApplicationNode(nodeBase, nodeBase.xData.xNode, nodeBase.xData.parentLevel, "资源", "RESOURCES", null, "folder.gif", null, "RESOURCES");
								else
									if(strContainer == "n")
									{
										var resourcesRoot = getRoleFunctionRoot(nodeBase, "RESOURCES");
										if (resourcesRoot)
											resourcesRoot.remove();
									}
							}
							break;
		}
	}
	
	function syncApplication(xmlDoc)
	{
		var appLevel = xmlDoc.documentElement.selectSingleNode("RESOURCE_LEVEL").text;
		var root = tv.Nodes[0];

		var nodeBase = getResByLevel(root, appLevel);

		syncApplicationByRoot(xmlDoc, root, nodeBase);

		root = tv.getItemByKey("MANAGED_APP");

		if (root)
		{
			nodeBase = getResByLevel(root, appLevel);
			syncApplicationByRoot(xmlDoc, root, nodeBase, true);
		}
	}

	function syncDeleteApplication(rootElem)
	{
		elem = rootElem.firstChild;

		while(elem)
		{
			var root = tv.Nodes[0];
			var appLevel = elem.getAttribute("RESOURCE_LEVEL");
			var node = getResByLevel(root, appLevel);

			if (node)
				node.remove();

			root = tv.getItemByKey("MANAGED_APP");

			if (root)
			{
				node = getResByLevel(root, appLevel);

				if (node)
					node.remove();
			}
			
			elem = elem.nextSibling;
		}
	}
	
	function syncDeleteResource(rootElem)
	{
		var elem = rootElem.firstChild;
		
		var root = tv.Nodes[0];
		var strResLevel = elem.selectSingleNode(".//RESOURCE_LEVEL").text;
		var nodeParentRes = getResByLevel(root, strResLevel.substring(0, strResLevel.length - 4));
		nodeParentRes = getRoleFunctionRoot(nodeParentRes, "RESOURCES");
		if (nodeParentRes.xData.waitforLoad == false)
		{
			while (elem)
			{
				strResLevel = elem.selectSingleNode(".//RESOURCE_LEVEL").text;
				var node = getResByLevel(root, strResLevel);
				if (node)
					node.remove();
					
				elem = elem.nextSibling;
			}
		}
	}

	function syncDeleteRolesFuncs(strType, strAppLevel, rootElem)
	{
		if (strAppLevel == null)
			strAppLevel = rootElem.firstChild.getAttribute("parentLevel");
			
		var nodeApp = getResByLevel(tv.Nodes[0], strAppLevel);
		if (nodeApp)
		{
			var nodeRoot = getRoleFunctionRoot(nodeApp, strType);

			if (nodeRoot.xData.waitforLoad == false)
			{
				var elem = rootElem.firstChild;

				while(elem)
				{
					var strID = elem.getAttribute("id");
					if (strID)
						node = getRoleFunctionByID(nodeRoot, strType, elem.getAttribute("id"));
					else
						node = getRoleFunctionByID(nodeRoot, strType, elem.firstChild.selectSingleNode(".//ID").text);
						
					if (node)
						node.remove();

					elem = elem.nextSibling;
				}
			}
		}
	}

	function syncDeleteOP(xmlDoc)
	{
		var nodeEle = xmlDoc.documentElement.firstChild;
		switch (nodeEle.nodeName)
		{
			case "RESOURCES":
				syncDeleteResource(xmlDoc.documentElement);
				break;
			default:
				var strType = xmlDoc.documentElement.getAttribute("type");
				var strID = xmlDoc.documentElement.getAttribute("id");
				var node = null;

				switch(strType)
				{
					case "MANAGED_APP":
					case "APPLICATIONS":	syncDeleteApplication(xmlDoc.documentElement);
											break;
					case "ROLES":
					case "FUNCTIONS":		syncDeleteRolesFuncs(strType, 
												xmlDoc.documentElement.firstChild.getAttribute("RESOURCE_LEVEL"),
												xmlDoc.documentElement);
											break;
					case "RESOURCES":		
											syncDeleteResource(xmlDoc.documentElement);
											break;
				}
				break;
		}
	}

	function syncRolesAndFunctions(xmlDoc)
	{
		var strID = xmlDoc.documentElement.selectSingleNode("ID").text;
		var strAppLevel = xmlDoc.documentElement.selectSingleNode("RESOURCE_LEVEL").text;
		var strType = xmlDoc.documentElement.nodeName;

		var strName = null;

		if (xmlDoc.documentElement.selectSingleNode("NAME"))
			strName = xmlDoc.documentElement.selectSingleNode("NAME").text;

		var nodeApp = getResByLevel(tv.Nodes[0], strAppLevel);

		if (nodeApp)
		{
			var nodeRoot = getRoleFunctionRoot(nodeApp, strType);

			if (nodeRoot.xData.waitforLoad == false)
			{
				var node = getRoleFunctionByID(nodeRoot, strType, strID);

				switch(xmlDoc.documentElement.getAttribute("op"))
				{
					case "Insert":	if (!node)
										if (xmlDoc.documentElement.nodeName == "RESOURCES")
											showApplications(xmlDoc.documentElement, nodeRoot, null, false);
										else
											addRoleAndFunctionNode(xmlDoc.documentElement, nodeRoot, strType);
									break;
					case "Update":	if (node && strName)
										node.putText(strName);
									break;
				}
			}
		}
	}
	
	function syncResource(xmlDoc)
	{
		var strResLevel = xmlDoc.documentElement.selectSingleNode("RESOURCE_LEVEL").text;
		var root = tv.Nodes[0];
		
		var strName = null;

		if (xmlDoc.documentElement.selectSingleNode("NAME"))
			strName = xmlDoc.documentElement.selectSingleNode("NAME").text;
			
		var nodeRoot = getResByLevel(root, strResLevel.substring(0, strResLevel.length - 4));
		var resourcesRoot = getRoleFunctionRoot(nodeRoot, "RESOURCES");
		if (resourcesRoot.xData.waitforLoad == false)
		{
			var node = getResByLevel(root, strResLevel);
			
			switch (xmlDoc.documentElement.getAttribute("op"))
			{
				case "Insert":	if (!node)
									showApplications(xmlDoc.documentElement, resourcesRoot, null, false);
								break;
				case "Update":	if (node && xmlDoc.documentElement.selectSingleNode("RES_TYPE"))
								{
									switch (parseInt(xmlDoc.documentElement.selectSingleNode("RES_TYPE").text))
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
									node.imgIcon.style.backgroundImage  = "url(./images/" + getImgFromClass(strResType) + ")";
								}
								if (node && strName)
									node.putText(strName);
								else
									if (node)
									{
										var strContainer = getSingleNodeText(xmlDoc.documentElement, "CONTAINER", "");
										if (strContainer == "y")
										{
											nodeRoot = getResByLevel(root, strResLevel);
											addApplicationNode(nodeRoot, nodeRoot.xData.xNode, nodeRoot.xData.parentLevel, "资源", "RESOURCES", null, "folder.gif", null, "RESOURCES");
										}
										else
											if(strContainer == "n")
											{
												nodeRoot = getResByLevel(root, strResLevel);
												resourcesRoot = getRoleFunctionRoot(nodeRoot, "RESOURCES");
												resourcesRoot.remove();
											}
									}
								break;
			}
		}
	}

	//进入某结点
	function enterNode(tree, xmlDoc)
	{
		var root = xmlDoc.documentElement;

		var appID = root.getAttribute("appID");
		var type = root.getAttribute("type");
		var objID = root.getAttribute("objID");
		var refAppID = root.getAttribute("refAppID");
		var appLevel = root.getAttribute("appLevel");

		var appNode = null;
		var rootApp = null;

		if (refAppID)
		{
			appID = refAppID;
			rootApp = tree.getItemByKey("MANAGED_APP");
		}
		else
			rootApp = tree.Nodes[0];

		var rfNode = null;
		var appNode = getResByLevel(rootApp, appLevel);

		if (type == "RESOURCES")
		{
			if (appNode == null)
			{
				appNode = getResByLevel(rootApp, appLevel.substring(0, appLevel.length - 4));
				if (appNode)
				{
					appNode.setExpanded(true, false);				
					rfRoot = getRoleFunctionRoot(appNode, type);
					rfRoot.setExpanded(true, false);
					appNode = getResByLevel(rootApp, appLevel);
				}
			}
			if (appNode)
			{
				appNode.setExpanded(true, false);
				rfNode = getRoleFunctionRoot(appNode, type);
			}
		}
		else
		{
			if (appNode)
			{
				rfRoot = getRoleFunctionRoot(appNode, type);
				rfRoot.setExpanded(true, false);
				rfNode = getRoleFunctionByID(rfRoot, type, objID);
			}
		}
		if (rfNode)
			tree.selectNode(rfNode);
	}

	function doSyncData(strXml)
	{
		var xmlDoc = createDomDocument(strXml);

		switch(xmlDoc.documentElement.nodeName)
		{
			case "Delete":	syncDeleteOP(xmlDoc);
							break;
			case "APPLICATIONS":
							syncApplication(xmlDoc);
							break;
			case "FUNCTIONS":
			case "ROLES":	syncRolesAndFunctions(xmlDoc);
							break;
			case "RESOURCES":
							syncResource(xmlDoc);
							break;
			case "ENTER":	enterNode(tv, xmlDoc);
							break;
		}
	}

	//当同步数据发生变化
	function onSyncDataChange()
	{
		if ((event.propertyName == "value") && (syncData.value.length > 0))
		{
			doSyncData(syncData.value);
			syncData.value = "";
		}
	}
	
	//获得CODE_NAME属性
	//杨睿2004-4-13
	function getCodeName(Node)
	{
		if (typeof(Node) == "object")
		{
			if (Node.xData && Node.xData.DomNode)
				return getSingleNodeText( Node.xData.DomNode, "CODE_NAME", "");
			if (Node.xml)
				return getSingleNodeText( Node, "CODE_NAME", "" );
		}
		return "";
	}
	
	//获得app_id
	function getAppID( Node )
	{
		var strAppID = "";
		strAppID = getSingleNodeText( Node.xData.DomNode, "APP_ID", "");
		
		if (strAppID == "")
			strAppID = getSingleNodeText(Node.parent.xData.DomNode, "APP_ID", "");
		
		return strAppID;
	}
	
	//获得id
	function getID( Node )
	{
		if (typeof(Node) == "object")
		{
			if (Node.xData && Node.xData.DomNode)
				return getSingleNodeText( Node.xData.DomNode, "ID", "");
			if (Node.xml)
				return getSingleNodeText( Node, "ID", "" );
		}
		return "";
	}

	//获得appLevel
	function getAppLevel( Node )
	{
		var strAppLevel = "";
		while (Node)
		{
			if (Node.xData.type == C_NODE_TYPE_B71 || Node.xData.type == C_NODE_TYPE_B72)
			{
				strAppLevel = getSingleNodeText( Node.xData.DomNode, "RESOURCE_LEVEL", "");
				break;
			}
			Node = Node.parent;
		}
		
		return strAppLevel;
	}
	
	//由appObjectList.aspx页面调用inputXML.onfoucs()来实现数据的更新
	function autoRefresh()
	{
		var xmlDoc = createDomDocument(inputXNode.value);
		m_curNodeKey = getSingleNodeText(xmlDoc.documentElement, "nodeKey", m_curNodeKey);
	    var tempKey = m_curNodeKey;
	    var tvNode = tv.getItemByKey(tempKey);
	    if (!tvNode)
			tvNode = tv.Nodes[0];
	    refresh( tvNode );
	    
	    tvNode = tv.getItemByKey(tempKey);
	    if (!tvNode)
			tvNode = tv.Nodes[0];
		
		if (inputXNode.value.indexOf("showFuncSetContent") == -1)
			setInputContent( tvNode );
		else
			setInputContent2( tvNode );
	}
	
	//设置传入数据
	function setInputContent( node )
	{
			if (!node.xData.xml) 
				loadAppChildren(node);
			inputXML.value = node.xData.xml.xml;

			var xmlDoc;
					
			if (node.xData.DomNode)
				xmlDoc = createDomDocument(node.xData.DomNode.xml);
			else if (node.getParent().xData.DomNode)
				xmlDoc = createDomDocument(node.getParent().xData.DomNode.xml);
			else
				xmlDoc = createDomDocument("<Table/>");
				
			if (node.xData.type)
			{
				appendNode(xmlDoc, xmlDoc.documentElement, "nodeType", node.xData.type);
				appendNode(xmlDoc, xmlDoc.documentElement, "appID", node.xData.appID);
				appendNode(xmlDoc, xmlDoc.documentElement, "appCodeName", node.xData.appCodeName);
				appendNode(xmlDoc, xmlDoc.documentElement, "appLevel", node.xData.appLevel);
				appendNode(xmlDoc, xmlDoc.documentElement, "useScope", node.xData.useScope);
				appendNode(xmlDoc, xmlDoc.documentElement, "nodeKey", node.key);
			}
		
			inputXNode.value = xmlDoc.xml;

	}

	function setInputContent2( node )
	{
			inputXML.value = node.xData.xml2.xml;

			var xmlDoc;
					
			if (node.xData.DomNode)
				xmlDoc = createDomDocument(node.xData.DomNode.xml);
			else if (node.getParent().xData.DomNode)
				xmlDoc = createDomDocument(node.getParent().xData.DomNode.xml);
			else
				xmlDoc = createDomDocument("<Table/>");
				
			if (node.xData.type)
			{
				appendNode(xmlDoc, xmlDoc.documentElement, "nodeType", node.xData.type);
				appendNode(xmlDoc, xmlDoc.documentElement, "appID", node.xData.appID);
				appendNode(xmlDoc, xmlDoc.documentElement, "appCodeName", node.xData.appCodeName);
				appendNode(xmlDoc, xmlDoc.documentElement, "appLevel", node.xData.appLevel);
				appendNode(xmlDoc, xmlDoc.documentElement, "showFuncSetContent", "true");
				appendNode(xmlDoc, xmlDoc.documentElement, "useScope", node.xData.useScope);
				appendNode(xmlDoc, xmlDoc.documentElement, "nodeKey", node.key);
			}
		
			inputXNode.value = xmlDoc.xml;

	}
	
	//在右栏中显示详细内容
	function showSubPage(strURL)
	{
			innerDocTD.innerHTML = "<iframe id='frmContainer' src='" + strURL + "' " + 
				"style='WIDTH:100%;HEIGHT:100%' frameborder='0' scrolling='auto'></iframe>";
	}
	
	
	
	//刷新某应用树结点
	function refresh(tvNode)
	{
		try
		{
			var arrayKey		=new Array();
			arrayKey.push(tvNode.key);
			
			var bFlag = true;
			tv.selectedItem = null;
			while(bFlag)
			{
				//tvNode = tvNode.getParent();
				switch (tvNode.xData.type)
				{
					//应用树集合结点类型：node.xData.type
					case C_NODE_TYPE_A1: 	//根结点
											bFlag = false;
											break;
					case C_NODE_TYPE_A7:	//子应用集合
											bFlag = false;
											if (arrayKey.length==1)
											{
												tvNode = tvNode.getParent();
												arrayKey.push(tvNode.key);
												tvNode = tvNode.getParent();
												arrayKey.push(tvNode.key);
											}
											break;

					//应用树叶结点类型：node.xData.type
					case C_NODE_TYPE_A2:	//应用服务范围集合
					case C_NODE_TYPE_A3:	//自授权角色集合
					case C_NODE_TYPE_A4:	//自授权功能树
					case C_NODE_TYPE_A5:	//应用角色集合
					case C_NODE_TYPE_A6:	//应用功能树
					case C_NODE_TYPE_B21:	//机构服务范围
					case C_NODE_TYPE_B22:	//数据服务范围
					case C_NODE_TYPE_B31:	//自授权角色
					case C_NODE_TYPE_B51:	//应用角色
					case C_NODE_TYPE_B41:	//自授权功能
					case C_NODE_TYPE_B42:	//自授权功能集合
					case C_NODE_TYPE_B61:	//应用功能
					case C_NODE_TYPE_B62:	//应用功能集合
					case C_NODE_TYPE_B71:	//通用授权应用
					case C_NODE_TYPE_B72:	//一般应用
					case C_OBJ_TYPE_A1:		//人员
					case C_OBJ_TYPE_A2:		//机构	
					case C_OBJ_TYPE_A3:		//组
											tvNode = tvNode.getParent();
											if (tvNode)
												arrayKey.push(tvNode.key);
											break;
				}
			}		
			
			var strKey;
			var curNode;
			var len = arrayKey.length;
			for(var i=0; i<len; i++)
			{
				strKey		= arrayKey.pop();
				curNode		= tv.getItemByKey(strKey);

				if (curNode)
				{
					refreshSubNode(curNode);
					curNode.setExpanded(true);
				}
			}
			//展开最后的结点
			curNode		= tv.getItemByKey(strKey);
			if (curNode)
			{ 
				curNode.setExpanded(true);
			}
		}
		catch(e)
		{
			showError(e);
		}
		
	}
	
	//获得应用的USE_SCOPE属性
	function getUseScope( node )
	{
		while(node)
		{
			if (node.xData.type == C_NODE_TYPE_B71 || node.xData.type == C_NODE_TYPE_B72)
				return getSingleNodeText(node.xData.DomNode, "USE_SCOPE", "n");
			node = node.parent;
		}
		return "n";
	}
	
	//更新属性
	function updateProperty(tNode)
	{
		var strType				= "";
		var strClassify			= "";
		switch (tNode.xData.type)
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
		arg.appID			= tNode.xData.appID;
		arg.appResLevel		= tNode.xData.appLevel;
		arg.codeName		= getCodeName( tNode );
		arg.objID			= getID( tNode );
		arg.id				= arg.objID;
		arg.classify		= strClassify;
		//arg.inherited		= "";
		//arg.resLevel		= 
		arg.disabled		= isPreDefineObj(tNode.xData.type);
		arg.fatherNodeType	= tNode.xData.type;

		var xmlResult = showAppObjDetailDialog(arg);
		
		if (xmlResult)
		{
//			if (inputXNode.value.length > 0)
//			{
//				var xmlDoc = createDomDocument(inputXNode.value);
//				m_curNodeKey = getSingleNodeText(xmlDoc.documentElement, "nodeKey", m_curNodeKey);
//				tNode = tv.getItemByKey(m_curNodeKey);
//			}
			refresh(tNode);
		}
	}
	

//-->

