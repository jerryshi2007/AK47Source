<!--
//要求展现的"根机构"
var _OGU_RootOrg = "";

//要求展现的相关参数配置
var _OGU_Params = C_LIST_ORGANIZATIONS;

//权限参数
var strRoles = "";

	//针对机构树中的节点选择
	function openInnerDocument(strURL, strGuid, strOrgGuid, strDisplayName, strClass, strRoles, strCodeName, strDescription)
	{
		paramValue.value = "<Parent GUID=\"" + strGuid + "\" rootOrgGuid=\"" + strOrgGuid + "\" DISPLAYNAME=\"" + strDisplayName + "\" ORIGINAL_SORT=\"" + strClass + "\" viewRoles=\"" + strRoles + "\" codeName=\"" + strCodeName + "\" description=\"" + strDescription + "\" />";

		var objInputParam = frmContainer.document.all("inputParam");

		if (!objInputParam)
			innerDocTD.innerHTML = "<iframe id='frmContainer' src='" + strURL + "' style='WIDTH:100%;HEIGHT:100%' frameborder='0' scrolling='auto'></iframe>";
		else
			objInputParam.value = paramValue.value;
	}
	
	function tvNodeExpand()
	{
		try
		{
			var activeNode = event.node;
			var bASync = event.fromClick;

			if (activeNode.xData.waitforLoad)
				loadChildren(activeNode, C_LIST_ORGANIZATIONS, null, bASync);
		}
		catch(e)
		{
			showError(e);
		}
	}
	
	function tvNodeSelected()
	{
		try
		{
			var xNode = event.node.xData.xNode;

			if (xNode)
			{
				var strGuid = xNode.getAttribute("GUID");
				var strOrgGuid = xNode.getAttribute("APP_GUID");
				var strDisplayName = xNode.getAttribute("DISPLAYNAME");
				var strClass = xNode.getAttribute("CLASS");
				var strCodeName = xNode.getAttribute("CODE_NAME");
				var strDescription = xNode.getAttribute("DISCRIPTION");
				openInnerDocument("logList/UserLogList.aspx", strGuid, strOrgGuid, strDisplayName, strClass, strRoles, strCodeName, strDescription);
			}
			else
				event.returnValue = false;
		}
		catch(e)
		{
			showError(e);
		}
	}
	
	function tvNodeRightClick()
	{
		/*
		try
		{
			var tNode = event.node;

			if (tNode)
			{
				menuTree.lastNode = tNode;
				menuTree.show(event.x, event.y);
			}
		}
		catch(e)
		{
			showError(e);
		}
		*/
	}

	function getValueFromXNode(xNode, strName, nodeDefault)
	{
		var strResult = xNode.getAttribute(strName);

		if (!strResult || strResult.length == 0)
		{
			var node = nodeDefault.selectSingleNode(strName);
			if (node)
				strResult = node.text;
		}

		return strResult;
	}
	function onDocumentLoad()
	{
		try
		{
			initSplitter(splitterContainer);//splitter.js
			tv.oncontextmenu = onDefaultContextMenu;
			strRoles = rolesValue.value;
			document.title += "(" + currentUserName.value + ")";//设置标题中的身份
			getRoot(tv, _OGU_Params, null, _OGU_RootOrg, null, null, syncCallBack);
		}
		catch(e)
		{
			showError(e);
		}
	}
	
	//菜单选择上的默认操作
	function onDefaultContextMenu()
	{
		try
		{
			event.returnValue = false;	
		}
		catch (e)
		{
			showError(e);
		}
	}
		
	function onDataChanged()
	{
		var refPage = document.all("refreshPage").value;
		if (event.propertyName == "value" && refPage != "")
		{
			document.all("refreshPage").value = "";
			getRoot(tv, _OGU_Params, null, _OGU_RootOrg, null, null, syncCallBack);
		}
	}

	//getRoot的CallBack调用（在机构树中的建立）
	function syncCallBack(tv, xmlResult)
	{

	}
//-->
