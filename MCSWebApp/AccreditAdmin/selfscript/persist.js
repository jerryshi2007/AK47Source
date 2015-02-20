<!--
function getPersistObject()
{
	var oPersist = document.all("oPersistObject");

	if (oPersist == null)
	{
		oPersist = document.createElement("<div style='behavior:url(#default#userdata)' id='oPersistObject'></div>");
		document.body.insertBefore(oPersist);
	}

	return oPersist;
}

function getPersistXML()
{
	var oPersist = getPersistObject();

	return oPersist.XMLDocument;
}

function setPersistProperty(strProperty, strValue)
{
	var oPersist = getPersistObject();

	oPersist.setAttribute(strProperty, strValue);
	oPersist.save(C_ACCREDIT_ADMIN_ROOT_URI);
}

function getPersistProperty(strProperty, strDefault)
{
	var oPersist = getPersistObject();

	oPersist.load(C_ACCREDIT_ADMIN_ROOT_URI);

	var value = oPersist.getAttribute(strProperty);

	if (value == null)
		if (strDefault)
			value = strDefault;
		else
			value = "";

	return value;
}

function loadTreeStatus(tv, strUserID)
{
	var oPersist = getPersistObject();

	oPersist.load(C_ACCREDIT_ADMIN_ROOT_URI + "_" + tv.id);

	var xmlDoc = oPersist.XMLDocument;
	
	var	nodeUser = xmlDoc.selectSingleNode(".//UserProfile[@id = \"" + strUserID + "\"]");
	
	if (nodeUser != null)
	{
		var nodeTree = nodeUser.selectSingleNode(tv.id);
		
		if (nodeTree != null)
		{
			var node = nodeTree.firstChild;

			while(node != null)
			{
				var keyAttr = node.attributes.getNamedItem("key");
				
				if (keyAttr != null )
				{
					var nTree = tv.getItemByKey(keyAttr.value);

					if (nTree != null)
						nTree.setExpanded(true, false);
				}

				node = node.nextSibling;
			}
		}
	}
}

//±£´æÊ÷µÄ×´Ì¬
function saveTreeStatus(tv, strUserID)
{
	var oPersist = getPersistObject();
	var xmlDoc = oPersist.XMLDocument;

	var	nodeUser = xmlDoc.selectSingleNode(".//UserProfile[@id = \"" + strUserID + "\"]");

	if (nodeUser == null)
	{
		nodeUser = appendNode(xmlDoc.documentElement, "UserProfile");

		var uIDattr = xmlDoc.createAttribute("id");
		uIDattr.value = strUserID;

		nodeUser.attributes.setNamedItem(uIDattr);
	}

	var nodeTree = nodeUser.selectSingleNode(tv.id);

	if (nodeTree == null)
		nodeTree = appendNode(nodeUser, tv.id);

	while(nodeTree.firstChild != null)
		nodeTree.removeChild(nodeTree.firstChild);

	for (var i = 0; i < tv.Nodes.length; i++)
	{
		var treeNode = tv.Nodes[i];

		if (treeNode.getExpanded())
		{
			if (typeof(treeNode.key) == "string")
			{
				var newNode = appendNode(nodeTree, "n");

				var keyAttr = xmlDoc.createAttribute("key");
				
				keyAttr.value = treeNode.key;

				newNode.attributes.setNamedItem(keyAttr);
			}
		}
	}

	oPersist.save(C_ACCREDIT_ADMIN_ROOT_URI + "_" + tv.id);
}

//-->
