<!--
//var ACCOUNTDISABLE = 0x0002

function setTDSelected(oTD)
{
	var rgn = document.body.createTextRange();
	var nameTD = getRelativeTD(oTD, "DISPLAY_NAME");

	rgn.moveToElementText(nameTD);
	rgn.select();

	return rgn;
}

function setNameCell(xmlNode, oTD, oExtAttr)
{
	var strName = xmlNode.getAttribute("DISPLAY_NAME");

	var a = document.createElement("a");

	a.style.marginLeft = "4px";
	a.innerText = strName;
	a.title = xmlNode.getAttribute("ALL_PATH_NAME");
	a.href = "";
	a.onclick = onNameClick;
	a.guid = xmlNode.getAttribute("GUID");
	a.parentGuid = xmlNode.getAttribute("PARENT_GUID");
	a.objectClass = xmlNode.tagName;
	a.xml = xmlNode.xml;
	
	oTD.guid = a.guid;
	oTD.innerText = "";
	oTD.title = a.title;
	oTD.parentGuid = a.parentGuid;
	oTD.objectClass = a.objectClass;
	
	var oTR = getOwnerTR(oTD);
	oTR.guid = a.guid;
	oTR.xml = a.xml;
	oTR.title = a.title;
	oTR.parentGuid = a.parentGuid;
	oTR.objectClass = a.objectClass;

	oTD.insertAdjacentElement("afterBegin", a);

	var imgSrc = getImgFromClass(xmlNode.tagName);
	
	if (xmlNode.getAttribute("STATUS") != "1")
		imgSrc = getImgFromClass("TRASH_" + xmlNode.tagName);
		
	if (xmlNode.tagName == C_USERS && xmlNode.getAttribute("POSTURAL") != "")
		if (parseInt(xmlNode.getAttribute("POSTURAL")) & 1 != 0)
			imgSrc = getImgFromClass("ForbiddenUser");
	
	imgSrc = "../images/" + imgSrc;
	
	var oSpan = createImgSpan(imgSrc, 16, 16);

	oTD.insertAdjacentElement("afterBegin", oSpan);
}

function appendNewObject(strXml)
{
	var xmlResult = createDomDocument(strXml);
	var firstTable = xmlResult.documentElement.firstChild;

	while(firstTable)
	{
		var row = appendGridRow(firstTable,
								ouUserListBody,
								ouUserHeader.rows(0),
								false,
								gridCallBack);

		initDragDropLine(row);
		
		firstTable = firstTable.nextSibling;
	}
	
	if (ouUserTable != null)
	{
		makeTableSortable(ouUserTable);
		changeGridRowOddEvenColor(ouUserTable);
	}
}

function calcCells(oTD, xmlNode, nodeName, nodeText, oExtAttr)
{
	switch(nodeName)
	{
		case "MAIN_DUTY":	if (xmlNode.getAttribute("SIDELINE"))
							{
								oTD.innerText = "";
								
								if (xmlNode.getAttribute("SIDELINE") == "0")
								{
									var oSpan = document.createElement("span");

									oTD.style.textAlign = "center";
									with (oSpan.style)
									{
										filter = "progid:DXImageTransform.Microsoft.Blur(PixelRadius='1', MakeShadow='false', ShadowOpacity='0.00')";
										fontFamily = "Symbol";
										color = "navy";
										fontSize = "10pt";
									}
									oSpan.innerText = String.fromCharCode(0xb7);
									oTD.insertAdjacentElement("afterBegin", oSpan);
									oTD.title = "主职"
								}
								else
								{
									oTD.title = "兼职";
								}
							}
							break;
		case "SORTID":
		case "SORT_ID":	var oTable = getOwnerTable(oTD);
						if (oTable.listSortId)
						{
							oTD.sortText = oTable.listSortId++;
							oTD.innerText = oTD.sortText;
							oTD.align = "right";
						}
						break;
		case "DISPLAY_NAME":	setNameCell(xmlNode, oTD, oExtAttr);
						break;
		case "RankName":
						if (xmlNode.getAttribute("NAME"))
						{
							var rAttr = xmlNode.getAttribute("RANK_NAME");
							if (rAttr && rAttr.length > 0 && rAttr != " ")
								oTD.innerText = rAttr + "/" + xmlNode.getAttribute("NAME"); 
							else
								oTD.innerText = xmlNode.getAttribute("NAME"); 
							oTD.title = xmlNode.getAttribute("RANK_CODE");
						}
						break;
		case "END_TIME":	setCellEndTime(oTD, xmlNode, nodeName, nodeText, oExtAttr);						
			break;
		case "E_MAIL":	oTD.innerText = "";
		
						if (nodeText && nodeText.length > 0)
						{
							var oSpan = document.createElement("span");

							oTD.style.textAlign = "center";
							with (oSpan.style)
							{
								filter = "progid:DXImageTransform.Microsoft.Blur(PixelRadius='1', MakeShadow='false', ShadowOpacity='0.00')";
								fontFamily = "Symbol";
								color = "navy";
								fontSize = "10pt";
							}
							oSpan.innerText = String.fromCharCode(0xb7);
							oTD.insertAdjacentElement("afterBegin", oSpan);
						}
						break;
	}
}

function setCellEndTime(oTD, xmlNode)
{
	if (xmlNode.tagName == "USERS" || xmlNode.tagName == "OU_USERS")
	{
		var strStartTime = xmlNode.getAttribute("START_TIME").split(" ")[0];
		var strEndTime = xmlNode.getAttribute("END_TIME").split(" ")[0];
		oTD.innerText = strStartTime + "至" + strEndTime;
		if (strEndTime.length > 4 && strEndTime.substring(0, 4) == "9999")
		{
			oTD.innerText = "[永不停用]";
		}
		oTD.title = oTD.innerText;
	}
}

function onNameClick(obj)
{
	try
	{
		var a = null;

		if (obj)
			a = obj;
		else
			a = event.srcElement;

		var strXml = "";

		if (!event.ctrlKey && !event.shiftKey)
		{
			var rgn = document.body.createTextRange();
			var nameTD = getRelativeTD(getOwnerTR(a), "DISPLAY_NAME");

			rgn.moveToElementText(nameTD);
			rgn.select();
		}

		switch (a.objectClass)
		{
			case "ORGANIZATIONS":
				strXml = showOUDetailDialog("Update", a.parentGuid, a.guid);
				break;
			case "GROUPS":
				strXml = showGroupDetailDialog("Update", a.parentGuid, a.guid);
				break;
			case "USERS":
				strXml = showUSERDetailDialog("Update", a.parentGuid, a.guid, null);
				break;
			default :
				trueThrow(true, "对不起，没有对应于“" + a.objectClass + "”的匹配对象类型！");
				break;
		}

		if (strXml != null && strXml.length > 0)
		{			
			var xmlResult = createDomDocument(strXml);
			var root = xmlResult.documentElement.firstChild;
			
			var oTR = getOwnerTR(a);
			if (oTR != null)
			{
				var oNode = oTR.xmlRow;
				for (var i = 0; i < root.attributes.length; i++)
				{
					var attr = root.attributes.item(i);
					oNode.setAttribute(attr.nodeName, attr.nodeValue);
				}
				updateGridCellWithAttributes(oNode, oTR, ouUserHeader.firstChild, true, gridCallBack, "SORT_ID");
					
				root.setAttribute("OriginalSort", oNode.getAttribute("ORIGINAL_SORT"));
			}
			setSyncData(xmlResult.xml);
		}
	}
	catch(e)
	{
		showError(e);
	}
	finally
	{
		event.cancelBubble = true;
		event.returnValue = false;

		return event.returnValue;
	}
}

function enumSelection(root)
{
	var selection = document.selection;

	var rgn = selection.createRange();
	
	var strTemp = rgn.htmlText.substring(rgn.htmlText.indexOf("TBODY"));
	
	tempDiv.innerHTML = strTemp.substring(strTemp.indexOf("<TR") - 1);

	for (var i = 0; i < tempDiv.childNodes.length; i++)
	{
		var obj = tempDiv.childNodes[i];

		if (obj.tagName == "A")
		{
			var xmlDoc = createDomDocument(obj.xml);
			var node = xmlDoc.documentElement.cloneNode(true);

			root.appendChild(node);
		}
	}
}

function syncDeletedRows(xmlDoc, oBody)
{
	var i = 0;

	while(i < oBody.rows.length)
	{
		var row = oBody.rows[i];
		var bDeleted = false;

		if (row.xml)
		{
			var rowXml = createDomDocument(row.xml);
			var strOriginalSort = rowXml.documentElement.getAttribute("ORIGINAL_SORT");
			
			var CompareValue = "ORIGINAL_SORT";
			if (xmlDoc.documentElement.tagName == "Move")
				CompareValue = "OLD_ORIGINAL_SORT";
				
			var	node = xmlDoc.documentElement.selectSingleNode(".//" + rowXml.documentElement.tagName + "[@" + CompareValue + "=\"" + strOriginalSort + "\"]");

			if (node)
			{
				oBody.deleteRow(row.rowIndex - 1);
				bDeleted = true;
			}
		}

		if (!bDeleted)
			i++;
	}

	changeGridRowOddEvenColor(getOwnerTable(oBody));

	var nodeError = xmlDoc.selectSingleNode(".//Error");

	if (nodeError)
		throw nodeError.text;
}

function deleteObjectFromSelection(oBody)
{
	var strResult = null;

	var xmlDoc = createDomDocument("<realDelete />");

	var root = xmlDoc.documentElement;
	
	enumSelection(root);

	trueThrow(root.childNodes.length == 0, "对不起，你没有选定要求删除的对象！");
	
	if (confirm("确定要删除选定的对象吗？"))
	{
		var xmlResult = xmlSend("../sysAdmin/OGUEditer.aspx", xmlDoc);

		checkErrorResult(xmlResult);
		xmlResult = xmlDoc;
		syncDeletedRows(xmlResult, oBody);

		strResult = xmlResult.xml;
	}

	return strResult;
}

function logicDeleteObjectFromSelection(oBody)
{
	var strResult = null;

	var xmlDoc = createDomDocument("<logicDelete />");

	var root = xmlDoc.documentElement;
	
	enumSelection(root);

	trueThrow(root.childNodes.length == 0, "对不起，你没有选定要求删除的对象！");
	
	if (confirm("确定要把被选定的对象送入回收站中吗？"))
	{
		var xmlResult = xmlSend("../sysAdmin/OGUEditer.aspx", xmlDoc);

		checkErrorResult(xmlResult);
		xmlResult = xmlDoc;
		syncDeletedRows(xmlResult, oBody);

		strResult = xmlResult.xml;
	}

	return strResult;
}

function furbishDeleteObjectFromSelection(oBody)
{
	var strResult = null;
	var xmlDoc = createDomDocument("<furbishDelete />");
	var root = xmlDoc.documentElement;
	
	enumSelection(root);
	
	trueThrow(root.childNodes.length == 0, "对不起，你没有选定要求恢复的对象！");

	if (confirm("确定把所选对象都恢复到正常使用状态吗？"))
	{
		var xmlResult = xmlSend("../sysAdmin/OGUEditer.aspx", xmlDoc);

		checkErrorResult(xmlResult);
		xmlResult = xmlDoc;
		
		var vNode = xmlDoc.documentElement.firstChild;
		while(vNode)
		{
			vNode.setAttribute("STATUS", "1");
			vNode = vNode.nextSibling;
		}
		syncDeletedRows(xmlResult, oBody);

		strResult = xmlResult.xml;
	}
	
	return strResult;
}

function selectOU()
{
	var arg = new Object();
	var xmlResult = null;

	arg.listObjType = 1;
	arg.multiSelect = 0;

	var strXml = showSelectUserDialog(arg);

	if (strXml.length > 0)
		xmlResult = createDomDocument(strXml);

	return xmlResult;
}

function moveObjectFromSelection(oBody, strSrcOrgGuid)
{
	var xmlDoc = createDomDocument("<Move/>");
	var root = xmlDoc.documentElement;
	var strResult = null;

	enumSelection(root);
	
	trueThrow(root.childNodes.length == 0, "对不起，你没有选定要求移动的对象！");

	if (root.childNodes.length > 0)
	{
		var oSelectOu = selectOU();

		if (oSelectOu)
		{
			var sOuChild = oSelectOu.documentElement.firstChild;
			
			var strOrgGuid = sOuChild.getAttribute("GUID");
			if (strOrgGuid != strSrcOrgGuid)
			{
				root.setAttribute("GUID", strOrgGuid);
				root.setAttribute("ORIGINAL_SORT", sOuChild.getAttribute("ORIGINAL_SORT"));
				
				var xmlResult = xmlSend("../sysAdmin/OGUEditer.aspx", xmlDoc);

				checkErrorResult(xmlResult);
				
				syncDeletedRows(xmlResult, oBody);

				strResult = xmlResult.xml;
			}
		}
	}

	return strResult;
}

function InitPassword(strXml)
{
	var xmlRow = createDomDocument(strXml);
	var root = xmlRow.documentElement;
							
	if (confirm("你确定要把\"" + root.getAttribute("DISPLAY_NAME") + "\"的密码重新设置为初始密码么？"))
	{
		var xmlDoc = createDomDocument("<InitPassword/>")
		appendNode(xmlDoc.documentElement, "GUID", root.getAttribute("GUID"));
		appendNode(xmlDoc.documentElement, "PARENT_GUID", root.getAttribute("PARENT_GUID"));
		
		var xmlResult = xmlSend("../sysAdmin/OGUEditer.aspx", xmlDoc);

		checkErrorResult(xmlResult);
		
		alert("密码重置成功！");
	}
}
/*************************************************************/
var m_gDragSrc;
var m_gMoveDragStart = false;

function list_OnMouseMove()
{
	if (event.button == 1 || event.button == 2)
	{
		if (event.srcElement.tagName != "A" && event.srcElement.tagName != "IMG" && event.ctrlKey || event.button == 2)
		{
			m_gMoveDragStart = true;

			document.selection.empty();
			
			var oTR = getOwnerTR(event.srcElement);

			oTR.dragDrop();
		}
	}
}

function list_OnDragStart()
{
	event.returnValue = false;

	if (event.ctrlKey || m_gMoveDragStart)
	{
		var oTD = getRelativeTD(event.srcElement, "DISPLAY_NAME");

		try
		{
			if (oTD && oTD.guid)
			{
				event.dataTransfer.effectAllowed = "move";
				m_gDragSrc = getOwnerTR(event.srcElement);
				window.status = oTD.guid;
				event.returnValue = true;
			}
			else
			{
				m_gDragSrc = null;
				event.returnValue = false;
			}
		}
		catch(e)
		{
		}
	}
}

function list_OnDragOver()
{
	event.returnValue = false;
	event.dataTransfer.dropEffect = "move";
}

function list_OnDrop()
{
	event.returnValue = false;

	if (m_gDragSrc)
	{
		event.dataTransfer.dropEffect = "move";

		var tbody = getOwnerBody(event.srcElement); 
		var tarIndex = getOwnerTR(event.srcElement).rowIndex;

		if (tbody.rows[tarIndex - 1].guid)
		{
			var srcTR = getOwnerTR(m_gDragSrc);
			var srcIndex = srcTR.rowIndex;

			if (tarIndex == srcIndex)
				return;

			if (tarIndex > srcIndex)
			{
				for (var i = srcIndex + 1; i <= tarIndex; i++)
					srcTR.swapNode(tbody.rows(i - 1));
			}
			else
			{
				for (var i = srcIndex - 1; i >= tarIndex; i--)
					srcTR.swapNode(tbody.rows(i - 1));
			}

			changeGridRowOddEvenColor(ouUserTable);

			displaySortOrderSave(true);
		}
	}
	
	m_gMoveDragStart = false;
}

function initDragDrop()
{
	for (var i = 0; i < ouUserListBody.rows.length; i++)
	{
		var row = ouUserListBody.rows[i];

		initDragDropLine(row);
	}
}

function initDragDropLine(row)
{
	row.onmousemove = list_OnMouseMove;
	row.ondragstart = list_OnDragStart;
	row.ondragover = list_OnDragOver;
	row.ondrop = list_OnDrop;
}
/*************************************************************/
//-->