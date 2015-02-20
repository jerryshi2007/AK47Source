<!--
var m_listObjType = "65535";			// 要求列出的对象类型（机构、人员组、人员）
var m_selectObjType = "65535";		// 列出对象中的可选对象（机构、人员组、人员）
var m_multiSelect = "0";			// 是否允许多选
var m_listObjDelete = "1";		// 要求列出的对象包括被删除内容（正常使用、直接逻辑删除、间接逻辑删除）
var m_postType = "";
var m_rootOrg = "";				// 要求展现的机构树使用的“根机构”（默认为空）
var m_submitTarget = "";
var m_autoExpand = "";			// 机构树中要求自动展开的对象（树结点自动展开）
var m_strNodesSelected = "";	// 默认选定的对象（采用标记记录）
var m_maxLevel = "99999";
var m_orgAccessLevel = "";		// 用于Organization的级别控制
var m_userAccessLevel = "";		// 用于User的级别控制
var m_hideType = "";			// 要求机构树中针对配置信息中的指定配置上的数据信息被屏蔽
var m_showButtons = "0";			// 是否展现按钮
var m_selectSort = "0";			// 选择是否要求记录次序
var m_extAttr = "";				// 要求返回的属性数据

var m_backColor = "";			// 展示界面要求使用的颜色
var m_canSelectRoot = "false";	// 是否允许选择机构树结构的根节点
//var m_nodesSelectedXml = null;
var m_firstChildren = "";
var m_MulitServer = false;//跨WebServer的调用，在跨Server调用的时候采用Clipboard作为数据传输

function onFrameStateChange()
{
	try
	{		
		if (event.srcElement.readyState == "complete" && iFrameDocument("innerFrame").URLUnencoded != "about:blank")
			frmInput.btnOK.disabled = false;
	}
	catch (e)
	{
	
	}
}

function onSaveClick()
{
	try
	{
		var strResult = null;
		if (m_selectSort & 1 != 0 && m_multiSelect & 1 != 0)
			strResult = iFrameDocument("innerFrame").getAllSelectedNodeByUserSort();
		else
			strResult = iFrameDocument("innerFrame").getAllSelectedNode(null);//, m_selectParentOnly);

		if (requestParam.value.length == 0)
			window.returnValue = strResult;
			
		if (m_MulitServer)
		{
			if (strResult == null || strResult == "")
				window.clipboardData.clearData("Text");
			else
				window.clipboardData.setData("Text", strResult);//add by yuanyong 20070131
		}
	}
	catch (e)
	{
		showError(e);
	}
	finally
	{
		window.close();
	}
}

function onCancelClick()
{
	try
	{
		if (m_MulitServer)
			window.clipboardData.clearData("Text");
	}
	catch (e)
	{
		showError(e);
	}
	finally
	{
		window.close();
	}
}

function createIFrame()
{
/*
	frameContainer.innerHTML = 
		"<iframe id='innerFrame' onreadystatechange='onFrameStateChange();' style='BORDER-RIGHT: black 1px solid; PADDING-RIGHT: 0px;" 
		+ " BORDER-TOP: black 1px solid; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; MARGIN: 0px; BORDER-LEFT: black 1px solid; WIDTH: 100%;" 
		+ " PADDING-TOP: 0px; BORDER-BOTTOM: black 1px solid; HEIGHT: 100%; BACKGROUND-COLOR: transparent' frameborder='no' scrolling='no'" 
		+ " src='../exports/OGUTree.aspx?" 
		+ "topControl=dnSelected&maxLevel=" + m_maxLevel + "&backColor=" + m_backColor + "&target=" + m_submitTarget 
		+ "&showButtons=" + m_showButtons + "&multiSelect=" + m_multiSelect + "&listObjType=" + m_listObjType + "&listObjDelete=" + m_listObjDelete 
		+ "&selectObjType=" + m_selectObjType + "&rootOrg=" + m_rootOrg	+ "&autoExpand=" + m_autoExpand + "&extAttr=" + m_extAttr 
		+ "&orgAccessLevel=" + m_orgAccessLevel + "&userAccessLevel=" + m_userAccessLevel + "&hideType=" + m_hideType + "&selectSort=" + m_selectSort 
		+ "&canSelectRoot=" + m_canSelectRoot + "&nodesSelected=" + m_strNodesSelected + "'></iframe>";
*/
		with(innerForm)
		{
			hidTopControl.value = "dnSelected";
			hidMaxLevel.value = m_maxLevel;
			hidBackColor.value = m_backColor;
			hidTarget.value = m_submitTarget;
			hidShowButtons.value = m_showButtons;
			hidMultiSelect.value = m_multiSelect;
			hidListObjType.value = m_listObjType;
			hidListObjDelete.value = m_listObjDelete;
			hidSelectObjType.value = m_selectObjType;
			hidRootOrg.value = m_rootOrg;
			hidAutoExpand.value = m_autoExpand;
			hidExtAttr.value = m_extAttr;
			hidOrgAccessLevel.value = m_orgAccessLevel;
			hidUserAccessLevel.value = m_userAccessLevel;
			hidHideType.value = m_hideType;
			hidSelectSort.value = m_selectSort;
			hidCanSelectRoot.value = m_canSelectRoot;
			hidNodesSelected.value = m_strNodesSelected;
		}
		
		innerForm.submit();
}

function onDocumentUnload()
{
	if (document.frames["innerFrame"].tv)
		document.frames["innerFrame"].tv.clearRef();
}

function onDocumentLoad()
{	
	var arg = window.dialogArguments;

	try
	{
		if (arg)//dialogArguments方式调用数据
		{
			if (arg.listObjType)		m_listObjType = arg.listObjType;
			if (arg.selectObjType)		m_selectObjType = arg.selectObjType;
			if (arg.multiSelect)		m_multiSelect = arg.multiSelect;
			if (arg.listObjDelete)		m_listObjDelete = arg.listObjDelete;
			if (arg.postType)			m_postType = arg.postType;
			if (arg.rootOrg)			m_rootOrg = arg.rootOrg;
			if (arg.submitTarget)		m_submitTarget = arg.submitTarget;
			if (arg.autoExpand)			m_autoExpand = arg.autoExpand;
			if (arg.strNodesSelected)	m_strNodesSelected = arg.strNodesSelected;
			if (arg.maxLevel)			m_maxLevel = arg.maxLevel;
			if (arg.orgAccessLevel)		m_orgAccessLevel = arg.orgAccessLevel;
			if (arg.userAccessLevel)	m_userAccessLevel = arg.userAccessLevel;
			if (arg.hideType)			m_hideType = arg.hideType;
			if (arg.showButtons)		m_showButtons = arg.showButtons;
			if (arg.selectSort)			m_selectSort = arg.selectSort;
			if (arg.extAttr)			m_extAttr = arg.extAttr;
			
			if (arg.backColor)			m_backColor = arg.backColor;
			if (arg.canSelectRoot)		m_canSelectRoot = arg.canSelectRoot;
			
			if (arg.firstChildren)		m_firstChildren = arg.firstChildren;
		}
		else
		{
			var xmlDoc = null;
			var root = null;
			if (requestParam.value.length > 0 && requestParam.value != "<Params />")
			{
				xmlDoc = createDomDocument(requestParam.value);
				root = xmlDoc.documentElement;
			}
			else
			{
				var strClipData = window.clipboardData.getData("Text");//跨WebServer采用clipBoard作为数据传输
				
				try
				{
					xmlDoc = createDomDocument(strClipData);
					if (xmlDoc != null && xmlDoc.documentElement != null)
					{
						root = xmlDoc.documentElement;
						window.clipboardData.clearData("Text");
						m_MulitServer = true;
					}
				}
				catch(e)
				{
					//系统找不到相应的配置数据，采用默认配置数据
				}
			}
			if (root != null)
			{
				m_listObjType = getNodeText(root.selectSingleNode("listObjType"), m_listObjType);
				m_selectObjType = getNodeText(root.selectSingleNode("selectObjType"), m_selectObjType);
				m_multiSelect = getNodeText(root.selectSingleNode("multiSelect"), m_multiSelect);
				m_listObjDelete = getNodeText(root.selectSingleNode("listObjDelete"), m_listObjDelete);
				m_postType = getNodeText(root.selectSingleNode("postType"), m_postType);
				m_rootOrg = getNodeText(root.selectSingleNode("rootOrg"), m_rootOrg);
				m_submitTarget = getNodeText(root.selectSingleNode("submitTarget"), m_submitTarget);
				m_autoExpand = getNodeText(root.selectSingleNode("autoExpand"), m_autoExpand);
				m_strNodesSelected = getNodeText(root.selectSingleNode("strNodesSelected"), m_strNodesSelected);
				m_maxLevel = getNodeText(root.selectSingleNode("maxLevel"), m_maxLevel);
				m_orgAccessLevel = getNodeText(root.selectSingleNode("orgAccessLevel"), m_orgAccessLevel);
				m_userAccessLevel = getNodeText(root.selectSingleNode("userAccessLevel"), m_userAccessLevel);
				m_hideType = getNodeText(root.selectSingleNode("hideType"), m_hideType);
				m_showButtons = getNodeText(root.selectSingleNode("showButtons"), m_showButtons);
				m_selectSort = getNodeText(root.selectSingleNode("selectSort"), m_selectSort);
				m_extAttr = getNodeText(root.selectSingleNode("extAttr"), m_extAttr);
				
				m_backColor = getNodeText(root.selectSingleNode("backColor"), m_backColor);
				m_canSelectRoot = getNodeText(root.selectSingleNode("canSelectRoot"), m_canSelectRoot);
				
				m_firstChildren = getNodeText(root.selectSingleNode("firstChildren"), m_firstChildren);
			}
		}
		
		if (m_firstChildren != null && m_firstChildren.length > 0)
			m_firstChildren = "<DataTable>" + m_firstChildren + "</DataTable>";
			
		requestParam.value = "";
		createIFrame();
		window.returnValue = "";
	}
	catch(e)
	{
		showError(e);
	}
}
//-->