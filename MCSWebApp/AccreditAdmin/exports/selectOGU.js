<!--
var m_listObjType = "65535";			// Ҫ���г��Ķ������ͣ���������Ա�顢��Ա��
var m_selectObjType = "65535";		// �г������еĿ�ѡ���󣨻�������Ա�顢��Ա��
var m_multiSelect = "0";			// �Ƿ������ѡ
var m_listObjDelete = "1";		// Ҫ���г��Ķ��������ɾ�����ݣ�����ʹ�á�ֱ���߼�ɾ��������߼�ɾ����
var m_postType = "";
var m_rootOrg = "";				// Ҫ��չ�ֵĻ�����ʹ�õġ�����������Ĭ��Ϊ�գ�
var m_submitTarget = "";
var m_autoExpand = "";			// ��������Ҫ���Զ�չ���Ķ���������Զ�չ����
var m_strNodesSelected = "";	// Ĭ��ѡ���Ķ��󣨲��ñ�Ǽ�¼��
var m_maxLevel = "99999";
var m_orgAccessLevel = "";		// ����Organization�ļ������
var m_userAccessLevel = "";		// ����User�ļ������
var m_hideType = "";			// Ҫ������������������Ϣ�е�ָ�������ϵ�������Ϣ������
var m_showButtons = "0";			// �Ƿ�չ�ְ�ť
var m_selectSort = "0";			// ѡ���Ƿ�Ҫ���¼����
var m_extAttr = "";				// Ҫ�󷵻ص���������

var m_backColor = "";			// չʾ����Ҫ��ʹ�õ���ɫ
var m_canSelectRoot = "false";	// �Ƿ�����ѡ��������ṹ�ĸ��ڵ�
//var m_nodesSelectedXml = null;
var m_firstChildren = "";
var m_MulitServer = false;//��WebServer�ĵ��ã��ڿ�Server���õ�ʱ�����Clipboard��Ϊ���ݴ���

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
		if (arg)//dialogArguments��ʽ��������
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
				var strClipData = window.clipboardData.getData("Text");//��WebServer����clipBoard��Ϊ���ݴ���
				
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
					//ϵͳ�Ҳ�����Ӧ���������ݣ�����Ĭ����������
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