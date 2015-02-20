<!--
function onDocumentLoad()
{
	try
	{
		createTableHeadByXml(ColumnTitle.XMLDocument, ouUserHeader);
		var xmlDoc = createDomDocument(rsUserMessDetail.value);
		
		var root = xmlDoc.documentElement;
		while (root != null)
		{
			createGridBodyByHead(root.firstChild, ouUserListBody, null, -1, ouUserHeader.rows(0), ouUserTable, gridCallBack);
			
			initDragDrop();
							
			root = root.nextSibling;
		}
	}
	catch (e)
	{
		showError(e);
	}
}

function gridCallBack(strCmd, oTD, xmlNode, nodeName, nodeText)
{
	try
	{
		switch(strCmd)
		{
			case "onCalc":	calcCells(oTD, xmlNode, nodeName, nodeText);
							break;
			case "onClick":	if (!event.ctrlKey && !event.shiftKey)
							{
								var rgn = document.body.createTextRange();
								var nameTD = getRelativeTD(oTD, "DISPLAY_NAME");
								rgn.moveToElementText(nameTD);
								rgn.select();
							}
							break;
		}
	}
	catch(e)
	{
		showError(e);
	}
}

//-->