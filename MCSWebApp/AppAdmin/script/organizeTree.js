<!--
	var C_LIST_OU = 1;
	var C_LIST_USER = 2;
	var C_LIST_GROUP = 4;
	var C_LIST_DOMAIN = 8;
	var C_LIST_ROLE = 64;
	var C_LIST_ALL = 65535;
	var ACCOUNTDISABLE = 0x0002;	
	
	var m_extAttr = null;

	function setListMask(xmlDoc, nMask)
	{
		var m;

		if (nMask)
			m = nMask;
		else
			m = C_LIST_OU;

		appendAttr(xmlDoc, xmlDoc.documentElement, "mask", m);
	}

	function getDisplayName(node)
	{
		var strDisplayName = node.getAttribute("displayName");

		if (!strDisplayName || strDisplayName.length == 0)
			strDisplayName = node.getAttribute("name");

		return strDisplayName;
	}

	function loadChildrenData(treeNode, xNode, nMask, callBack, bAsync, argObj)
	{
		var xmlDoc = createCommandXML("getChild", xNode.getAttribute("dn"));

		setListMask(xmlDoc, nMask);

		if (m_extAttr)
			xmlDoc.documentElement.setAttribute("extAttr", m_extAttr);

		var params = new Object();
		
		params.xNode = xNode;
		params.treeNode = treeNode;
		params.callBack = callBack;

		var xmlResult = null;
		
		setRequestAttributeByArg(xmlDoc.documentElement, argObj);

		if (bAsync)
			xmlResult = xmlSend("/" + C_ACCREDIT_ADMIN_ROOT_URI + "/Anonymous/ADQuery.aspx",
								xmlDoc,
								afterLoadChildrenData,
								params);
		else
		{
			xmlResult = xmlSend("/" + C_ACCREDIT_ADMIN_ROOT_URI + "/Anonymous/ADQuery.aspx",
								xmlDoc);

			afterLoadChildrenData(xmlResult, params);
		}
	}

	function afterLoadChildrenData(xmlResult, params)
	{
		try
		{
			checkErrorResult(xmlResult);

			var xNode = params.xNode;

			while(xNode.childNodes.length > 0)
				xNode.removeChild(xNode.firstChild);

			var xmlSub = xmlResult.documentElement;

			while(xmlSub.childNodes.length > 0)
				xNode.appendChild(xmlSub.childNodes[0]);

			var treeNode = params.treeNode;
			treeNode.xData.waitforLoad = false;
			treeNode.removeChildren();

			showChildrenMember(xNode, treeNode, params.callBack);
		}
		catch(e)
		{
			showError(e);
		}
	}

	function loadChildren(n, nMask, callBack, bAsync, showSideline)
	{
		var xNode = n.xData.xNode;
		loadChildrenData(n, xNode, nMask, callBack, bAsync, showSideline);
	}

	function showChildrenMember(xNode, nFather, callBack)
	{
		var imgName;

		var node = xNode.firstChild;
		
		var accessLevel = getAttrValue(xNode, "accessLevel");

		while(node)
		{
			var level = getAttrValue(node, "personalTitle");
			if (node.tagName == "CN" && accessLevel && (!level || parseInt(accessLevel) > parseInt(level)))
			{
				node = node.nextSibling;
				continue;
			}
				
			if (accessLevel)
				appendAttr(node.ownerDocument, node, "accessLevel", accessLevel);
			
			var imgTemp;
			if (node.getAttribute("objectClass") == "person" && node.getAttribute("userAccountControl") != null && (parseInt(node.getAttribute("userAccountControl")) & ACCOUNTDISABLE) != 0)//账号被禁用
				imgTemp = getImgFromClass("userAccountControl");
			else
				imgTemp = getImgFromClass(node.getAttribute("objectClass"));
				
			var nMb = nFather.add("tvwChild", "", getDisplayName(node), 
						"/" + getRootDir(document.URLUnencoded) + "/images/" + imgTemp);

			nMb.xData.xNode = node;
			nMb.xData.waitforLoad = true;

			setNodeTitle(nMb, node);

			if (node.tagName != "CN")
				addLoadingNode(nMb);

			if (callBack)
				callBack(nMb, node);
				
			var childNode = node.firstChild;
			
			if (childNode)
			{
				nMb.getChild().remove();
				nMb.xData.waitforLoad = false;
				showChildrenMember(node, nMb, callBack);
			}

			node = node.nextSibling;
		}
		
		nFather.setExpanded(true);
	}

	function addLoadingNode(n)
	{
		n.xData.waitforLoad = true;
		n.add("tvwChild", "", "载入中...");
	}

	function getReversePath(strDN, strSplitChar, strIgnoreTag)
	{
		var arrParts = strDN.split(",");
		var strB = "";
		var spc = ".";

		if (strSplitChar)
			spc = strSplitChar;

		for (var i = arrParts.length - 1; i >= 0; i--)
		{
			if (strB.length > 0)
				strB += spc;

			var strKeyValue = arrParts[i].split("=");

			if (strKeyValue[0] != strIgnoreTag)
				strB += strKeyValue[1];
		}

		return strB;
	}

	function recursiveExpand(tree, root, strAutoExpand)
	{
		var node = root.firstChild;

		while(node)
		{
			var	xNode = node.xData.xNode;
			var strReverseDN = getReversePath(xNode.getAttribute("dn"));

			if (strReverseDN == strAutoExpand.substr(0, strReverseDN.length))
			{
				node.setExpanded(true, false);

				tree.selectedItem = node;

				if (strReverseDN.length != strAutoExpand.length)
					recursiveExpand(tree, node, strAutoExpand);

				break;
			}
			node = node.getNext();
		}
	}

	function setAutoExpand(tree, xmlDNs, bFireEvent)
	{
		var oldItem = tree.selectedItem;
		var xmlDoc = null;

		if (typeof(xmlDNs) == "object")
			xmlDoc = xmlDNs;
		else
		if (xmlDNs.length > 0)
			xmlDoc = createDomDocument(xmlDNs);

		if (xmlDoc)
		{
			var xNode = xmlDoc.documentElement.firstChild;

			while(xNode)
			{
				var strDN = xNode.getAttribute("dn");

				var strAutoExpand = getReversePath(strDN);
				var root = tree.Nodes[0];

				recursiveExpand(tree, root, strAutoExpand);
				xNode = xNode.nextSibling;
			}

			if (bFireEvent && oldItem != tree.selectedItem && tree.selectedItem)
				if (!tree.selectNode(tree.selectedItem))
					tree.selectedItem = oldItem;
		}
	}

	function setAutoExpand2(tree, strDN, bFireEvent)
	{
		var oldItem = tree.selectedItem;

		var strAutoExpand = getReversePath(strDN);
		var root = tree.Nodes[0];

		recursiveExpand(tree, root, strAutoExpand);

		if (bFireEvent && oldItem != tree.selectedItem && tree.selectedItem)
			if (!tree.selectNode(tree.selectedItem))
				tree.selectedItem = oldItem;
	}

	function setNodeTitle(node, xNode)
	{
		var dn = xNode.getAttribute("dn")

		node.span.title = node.getText() + ": " + dn;
	}

	function onXmlRequestReady(xmlResult, param)
	{
		try
		{
			doAfterOpenRoot(param.tree, xmlResult, param.callBack);

			if (param.syncCallBack)
				param.syncCallBack(param.tree, xmlResult);
		}
		catch(e)
		{
			showError(e);
		}
	}

	function preloadTreeImages()
	{
		var strDir = "/" + C_ACCREDIT_ADMIN_ROOT_URI + "/images/";

		for (var i = 0; i < arguments.length; i++)
		{
			var img = document.createElement("img");
			img.src = strDir + arguments[i];
		}
	}

	function getRoot(tree, nMask, callBack, rootObj, xmlChildren, extAttr, syncCallBack, argObj)
	{
		tree.Nodes.add("", "", "", "正在加载...");

		preloadTreeImages("domain.gif",	"ou.gif", "user.gif");

		var xmlResult = null;

		var oSyncCallBack = null;

		if (syncCallBack)
		{
			var param = new Object();

			param.tree = tree;
			param.callBack = callBack;
			param.syncCallBack = syncCallBack;

			oSyncCallBack = onXmlRequestReady;
		}
			

		if (!xmlChildren)
		{
			//设置参数
			var xmlDoc = createCommandXML("getRoot");

			setListMask(xmlDoc, nMask);

			if (rootObj)
				xmlDoc.documentElement.setAttribute("dn", rootObj.dn);

			if (extAttr)
			{
				xmlDoc.documentElement.setAttribute("extAttr", extAttr);
				m_extAttr = extAttr;
			}

			setRequestAttributeByArg(xmlDoc.documentElement, argObj);

			xmlResult = xmlSend("/" + C_ACCREDIT_ADMIN_ROOT_URI + "/Anonymous/ADQuery.aspx", xmlDoc, oSyncCallBack, param);
		}
		else
		{
			//同步访问取得结果
			xmlResult = createDomDocument(xmlChildren);
			
			if (oSyncCallBack)
				oSyncCallBack(xmlResult, param);
		}

		if (!syncCallBack)
			doAfterOpenRoot(tree, xmlResult, callBack);
	}

	function setRequestAttributeByArg(elem, argObj)
	{
		if (argObj && argObj.showSideline == "true")
				elem.setAttribute("showSideline", argObj.showSideline);

		if (argObj && argObj.ouAccessLevel)
			elem.setAttribute("ouAccessLevel", argObj.ouAccessLevel);

		if (argObj && argObj.userAccessLevel)
			elem.setAttribute("userAccessLevel", argObj.userAccessLevel);
			
		if (argObj && argObj.maskType)
			elem.setAttribute("maskType", argObj.maskType);
	}

	function doAfterOpenRoot(tree, xmlResult, callBack)
	{
		checkErrorResult(xmlResult);

		tree.Nodes.clear();

		var strName = C_DC_ROOT_NAME;
		var strImg = "domain.gif";

		var eleRoot = xmlResult.documentElement;
		var strType = eleRoot.getAttribute("objectClass");

		if (strType != "domain")
		{
			strName = eleRoot.getAttribute("displayName");

			if (strName.length == 0)
				strName = eleRoot.getAttribute("name");
		}

		var strImg = getImgFromClass(strType);

		var nRoot = tree.Nodes.add("", "", "", strName, "/" + C_ACCREDIT_ADMIN_ROOT_URI + "/images/" + strImg);

		nRoot.xData.xNode = eleRoot;
		nRoot.xData.waitforLoad = false;

		if (callBack)
			callBack(nRoot, eleRoot);

		showChildrenMember(eleRoot, nRoot, callBack);
	}
//->
