<!--
//根据屏幕高度来决定Grid的限制行数
function getLimitRowsByScreen()
{
	var nHeight = window.screen.height;
	var nResult = 15;

	if (nHeight >= 768)
		nResult += 5;

	if (nHeight >= 1024)
		nResult += 5;

	if (nHeight >= 1200)
		nResult += 5;

	return nResult;
}

function appendCell(oTR)
{
	var oTD = document.createElement("td");
	oTR.appendChild(oTD);

	return oTD;
}

function isArray(v)
{
	try
	{
		var a = v[0];
		return typeof(a) != "undefined";
	}
	catch(e)
	{
		return false;
	}
}

function clearAllRows(oPart)
{
	while(oPart.rows.length > 0)
	{
		oPart.deleteRow(0);
	}
}

function clearGridHead(oTH)
{
	for (var nn = oTH.rows.length; nn > 0; nn--)
	{
		var myRow = oTH.rows(nn - 1);
		
		if (myRow.datasrc.length > 0)
			oTH.deleteRow(nn - 1); 
	} 
}


//在数据字典数组中查找某一字段的某个属性值
function getAttribByDict(xmlDicts, strFieldName, strAttribName)
{
	var strResult;
	var xmlDict;
	var nDictCount;
  
	if (isArray(xmlDicts))
		nDictCount=xmlDicts.length;
	else
		nDictCount=1;
  
	for(var nn = 0; nn < nDictCount; nn++)
	{
		if (isArray(xmlDicts))
			xmlDict=xmlDicts[nn];
		else
			xmlDict=xmlDicts;

		var nodeRoot =xmlDict.selectSingleNode(".//xsd:sequence");
		var nodeColumn = nodeRoot.selectSingleNode("./xsd:element[@name = \"" + strFieldName + "\"]");                    

		if (nodeColumn != null)
			break;
	}

	if (nodeColumn != null)
	{
		try
		{
			strResult = nodeColumn.attributes.getNamedItem(strAttribName).value;
		}
		catch(e)
		{
			strResult = "";
		}
	}
	else
		strResult = "";    
  
	return strResult;
}

//根据一个XML描述创建表头
//xmlDocXML描述
//thHead 表头对象
function createTableHeadByXml(xmlDoc, oHead, callBack)
{
	clearGridHead(oHead);

	var xmlFirst = xmlDoc.documentElement;

	if (xmlFirst != null) 
	{
		var oTR1 = oHead.insertRow()
		oTR1.datasrc = "AutoBuildRow";
		oTR1.className = "gridHead";

		if (callBack)
			callBack("onRowCreated", oTR1, xmlFirst);

		var doc = document;

		for (var i = 0; i < xmlFirst.childNodes.length; i++)
		{   			
			var node = xmlFirst.childNodes[i];

			var oTD1 = appendCell(oTR1);

			var strImage = getAttrValue(node, "image");

			if (strImage.length > 0)
			{
				with (oTD1.style)
				{
					backgroundImage = "url(" + strImage + ")";
					backgroundPositionX = "center";
					backgroundPositionY = "center";
					backgroundRepeat = "no-repeat";
				}
			}

			oTD1.innerText = getAttrValue(node, "name");
			oTD1.title = getAttrValue(node, "title");
			oTD1.datafld = getAttrValue(node, "dataFld");
			oTD1.datasrc = getAttrValue(node, "dataSrc");

			oTD1.style.width = getAttrValue(node, "width");
			oTD1.style.height = getAttrValue(node, "height");
			oTD1.onclick = onHeaderColumnClick;
			oTD1.callBack = callBack;
			oTD1.xmlNode = node;

			var sortTag = doc.createElement("span");

			with (sortTag.style)
			{
				fontSize = "9pt";
				fontFamily = "Webdings";
				visibility = "hidden";
			}

			sortTag.innerText = 5;

			oTD1.sortTag = sortTag;
			oTD1.appendChild(sortTag);

			if (callBack)
				callBack("onCalc", oTD1, node);
		}
    }
}

function onHeaderColumnClick()
{
	var oTD = getOwnerTD(event.srcElement);

	if (oTD)
	{
		var result = false;

		if (oTD.callBack)
			result = oTD.callBack("onClick", oTD, oTD.xmlNode);

		if (!result)
			tableSort(oTD);
	}
}

//创建表头
//xmlDoc 数据包xml格式
//thHead 表头对象
//xmlDicts 数据字典组
function createTableHead(xmlDoc, oHead, xmlDicts)
{
	clearGridHead(oHead);

	var xmlFirst = xmlDoc.selectSingleNode("//Table");  

	if (xmlFirst != null) 
    {
		var oTR1 = oHead.insertRow()

        oTR1.datasrc="AutoBuildRow";
		oTR1.className = "gridHead";

		for (var i = 0; i < xmlFirst.childNodes.length; i++)
		{
			var oTD1 = oTR1.insertCell();

			oTD1.datafld = xmlFirst.childNodes[i].nodeName;
			oTD1.innerText = getAttribByDict(xmlDicts,xmlFirst.childNodes[i].nodeName, "caption");                 
		}
    }
}

function isAutoBuildRow(oTR)
{
	var bResult = false;

	if (typeof(oTR.datasrc) != "undefined")
		bResult = (oTR.datasrc.indexOf("AutoBuildRow") != -1);

	return bResult;
}

function createGridBody(xmlFirst, oHead, oBody, xmlDicts, nRowCount, oTable, callBackFun)
{
	var oHeadRow = null;

	for (var i = 0; i < oHead.rows.length; i++)
	{
		if (isAutoBuildRow(oHead.rows[i]))
		{
			oHeadRow = oHead.rows[i];
			break;
		}
	}

	if (oHeadRow != null)
		createGridBodyByHead(xmlFirst, oBody, xmlDicts, nRowCount, oHeadRow, oTable, callBackFun);
}

function updateGridCell(xmlTable, oRow, callBackFun)
{
	var xmlChild = xmlTable.firstChild;

	while(xmlChild)
	{
		var oTD = getRelativeTD(oRow, xmlChild.nodeName);

		if (oTD)
			setCellContent(xmlTable, oTD, xmlChild.nodeName, callBackFun);

		xmlChild = xmlChild.nextSibling;
	}
}

function setCellContent(xmlFirst, oTD, strFld, callBackFun)
{
	if (strFld.length > 0)
	{
		var nodeColumn = xmlFirst.selectSingleNode(strFld);
		
		if (nodeColumn == null)
			nodeColumn = xmlFirst.attributes.getNamedItem(strFld);
			
		if (nodeColumn != null)//nodeData
		{
			var strNodeText = nodeColumn.text;

			if (strFld == "NAME" && callBackFun == null)
			{
				if (oTD.all(1) && oTD.all(1).tagName == "A")
				{
					oTD.all(1).innerText = strNodeText;
					oTD.title = strNodeText;
				}
				else
				{
					oTD.dataFld = strFld;
					oTD.innerText = strNodeText;

					oTD.title = oTD.innerText;
				}
			}
			else
			{
				oTD.dataFld = strFld;
				oTD.innerText = strNodeText;
				oTD.nodeText = strNodeText;

				oTD.title = strNodeText;
			}

			if (callBackFun != null)
				callBackFun("onCalc", oTD, xmlFirst, strFld, strNodeText);

			if (!oTD.sortText)
				oTD.sortText = oTD.innerText;
		}
		else 
		{
			if (callBackFun != null)
				callBackFun("onCalc", oTD, xmlFirst, strFld);
		}
	}
	else
	{
		if (callBackFun != null)
				callBackFun("onCalc", oTD, null, "");
	}
}

function appendGridRow(xmlFirst, oBody, oHeadRow, bAddNullRow, callBackFun)
{
	var oRow = oBody.insertRow();

	if (xmlFirst != null)
	{
		oRow.xmlRow = xmlFirst;
		oRow.datasrc = "AutoBuildRow";
		oRow.onmouseover = changeClassHover;
		oRow.onmouseout = changeClassMoveOut;
		oRow.callFunc = callBackFun;
	}

	for (var i = 0; i < oHeadRow.cells.length; i++)
	{
		var oTD = oRow.insertCell();

		oTD.oncontextmenu = tContextMenu;
		oTD.onmouseup = tMouseUp;
		oTD.onclick = tClick;
		oTD.ondblclick = tDblClick

		//oTD.style.cursor = "default";

		oTD.callFunc = callBackFun;
		oTD.style.wordBreak = "break-all";

		var columnCell = oHeadRow.childNodes[i];

		//var columnStyle = columnCell.style;

		//oTD.style.width = columnStyle.width;
		//oTD.style.height = columnStyle.height;

		if (xmlFirst != null)
		{
			var strFld = columnCell.datafld;

			setCellContent(xmlFirst, oTD, strFld, callBackFun);
		}
		else
		if (bAddNullRow)
			oTD.innerText = " ";			
	}

	return oRow;
}

function createGridBodyByHead(xmlFirst, oBody, xmlDicts, nRowCount, oHeadRow, oTable, callBackFun)
{
	oTable.gridBody = oBody;

	var nn = 0;

	clearAllRows(oBody);

	oBody.pageSize = nRowCount;

	if (nRowCount < 0)
	{
		while (xmlFirst)
		{
			appendGridRow(xmlFirst, oBody, oHeadRow, false, callBackFun);

			xmlFirst = xmlFirst.nextSibling;
		}
	}
	else
	{
		while (nn < nRowCount)
		{
			appendGridRow(xmlFirst, oBody, oHeadRow, true, callBackFun);

			if (xmlFirst)
				xmlFirst = xmlFirst.nextSibling;
			nn++;
		}
	}

	if (oTable != null)
	{
		makeTableSortable(oTable);
		changeGridRowOddEvenColor(oTable);
	}

	return nn;
}

//删除Grid中的一行, oT可以是TD，也可以是TR
function deleteGridRow(oT)
{
	var oTR = oT;

	if (oT.tagName == "TD")
		oTR = getOwnerTR(oT);

	if (isAutoBuildRow(oTR))
	{
		var oBody = getOwnerBody(oTR);

		if (oBody.pageSize > 0)
		{
			var oNewTR = oBody.insertRow();

			for (var i = 0; i < oTR.cells.length; i++)
			{
				var oNewCell = oNewTR.insertCell();
				oNewCell.innerText = " ";
			}
		}

		oBody.deleteRow(oTR.rowIndex - 1);
		changeGridRowOddEvenColor(getOwnerTable(oBody));
	}
}

//寻找和本TD同一行的指定字段的TD
function getRelativeTD(td, fldName)
{
	var tr = getOwnerTR(td);
	var ret = null;

	if (tr)
	{
		for (var i = 0; i < tr.cells.length; i++)
		{
			var tempTD = tr.cells[i];

			if (typeof(tempTD.dataFld) != "undefined")
			{
				if (tempTD.dataFld == fldName)
				{
					ret = tempTD;
					break;
				}
			}
		}
	}

	return ret;
}

function changeGridRowOddEvenColor(oTable)
{
	if (oTable)
	{
		var oBody = oTable.gridBody;

		if (typeof(oBody) == "object")
		{
			for (var i = 0; i < oBody.rows.length; i++)
			{
				var oRow = oBody.rows[i];

				if (typeof(oRow.datasrc) != "undefined")
				{
					if (oRow.datasrc.indexOf("AutoBuildRow") >= 0)
					{
						if (oRow.selected)
							oRow.className = "gridSelectedHighlight";
						else
						{
							if ((i % 2) == 0)	//Odd row
								oRow.className = "gridOddRow";
							else				//Even row
								oRow.className = "gridEvenRow";

							oRow.oldClassName = oRow.className;
						}
					}
				}
			}
		}
	}
}

function changeClassHover()
{
	var vobj = getOwnerTR(window.event.srcElement);

	if (vobj)
		setTRStyle(vobj);
}

function changeClassMoveOut()
{
	var vobj = getOwnerTR(window.event.srcElement);

	if (vobj)
		resetTRStyle(vobj);
}

function resetTRStyle(vobj)
{
	if (vobj.className != "gridSelectedHighlight")
		vobj.className = vobj.oldClassName;
}

function setTRStyle(vobj)
{
	if (vobj.className != "gridSelectedHighlight")
	{
		vobj.oldClassName = vobj.className;
		vobj.className = "gridHighlight";

		if (vobj.callFunc)
			vobj.callFunc("onMouseOverRow", vobj);
	}
}

function getOwnerTR(element)
{
	while (element && element.tagName.toUpperCase() != "TR")//行 TD单元格 TH表头
	{
		element = element.parentNode;

		if (element == null)
			break;
	}

	return element;
}

function getOwnerBody(element)
{
	if (element)
	{
		var tag = element.tagName.toUpperCase();

		while (tag != "TABLE" && tag != "TBODY" && tag != "TH" && tag != "THEAD" && tag != "TFOOT")
		{
			element = element.parentNode;

			if (element == null)
				break;

			tag = element.tagName.toUpperCase();
		}
	}

	return element;
}

function getOwnerTable(element)
{
	while (element && element.tagName.toUpperCase() != "TABLE")//行 TD单元格 TH表头
	{
		element = element.parentNode;

		if (element == null)
			break;
	}

	return(element);
}

function tContextMenu()
{
	var myColumn = window.event.srcElement;  

	if (myColumn.callFunc != null) 
		myColumn.callFunc("onContextMenu", myColumn);

	return window.event.returnValue;
}

function tMouseUp()
{
	var myColumn = window.event.srcElement;  

	if (myColumn.callFunc != null) 
		myColumn.callFunc("onMouseUp", myColumn);
}

function tDblClick()
{
	var myColumn = window.event.srcElement;  

	if (myColumn.callFunc != null) 
		myColumn.callFunc("onDblClick", myColumn);
}

function dbGridSelectedCallback(tr, arg)
{
	if (tr.selected)
	{
		tr.selected = false;
		tr.className = tr.oldClass;
	}
}

function setRowHighlight(tr, bHighlight)
{
	resetTRStyle(tr);

	if (bHighlight)
	{
		tr.oldClass = tr.className;
		tr.className = "gridSelectedHighlight";
	}
	else
	{
		tr.className = tr.oldClass;
	}
}

function setRowSelected(tr, bSelected, bClearAll)
{
	if (bClearAll)
		enumSelectedRows(tr, "clearSelected", dbGridSelectedCallback)

	tr.selected = bSelected;
	setRowHighlight(tr, bSelected);
}

function tClick()
{
	var myColumn = window.event.srcElement;  
	var oTR = getOwnerTR(myColumn);

	if (oTR)
	{
		if (myColumn.callFunc != null)
			myColumn.callFunc("onClick", myColumn, oTR.xmlRow, myColumn.dataFld, myColumn.nodeText);

		var body = getOwnerBody(myColumn);

		if (!body.noSelect)
		{
			var bMultiSelect = body.multiSelect && event.ctrlKey;

			if (oTR.datasrc && oTR.datasrc.indexOf("AutoBuildRow") >= 0)
			{
				var bSelected;
			
				if (oTR.selected)
					bSelected = false;
				else
					bSelected = true;

				setRowSelected(oTR, bSelected, !bMultiSelect);
			}
		}
	}
}

function makeTableSortable(tableId)
{
    var mTab = null;

	if (typeof(tableId) == "string")
		mtab = document.getElementById(tableId);
    else
        mtab = tableId;

	if (mtab.rows.length > 1)
	{
		var row = mtab.rows.item(0);

		for (var nI = 0; nI < row.cells.length; nI++)
		{
			var cell = row.cells(nI);

			if (cell.onclick == null)
				cell.onclick = tableSort;

			cell.style.cursor = "hand";
		}
	}
}

function getOwnerTD(element)
{
	while (element && element.tagName.toUpperCase() != "TD" && element.tagName.toUpperCase() != "TH")
	{
		element = element.parentNode;
		if (element == null)
			break;
	}

	return element;
}

function otdStyleTocall(myColumn, imageURL, strTitle, xmlNode)
{		
	if (arguments.length == 2)
		myColumn.xmlNode = arguments[1];
	else
	{
		myColumn.style.backgroundPosition ="center"; 
		myColumn.style.backgroundAttachment = "fixed";
		myColumn.style.backgroundRepeat = "no-repeat";	
		myColumn.style.backgroundImage = "url(" + imageURL + ")";	
		myColumn.title = strTitle;
		myColumn.xmlNode = xmlNode;
	}	
}

function enumSelectedRows(oTableObj, arg, callBack)
{
	var oTable = getOwnerTable(oTableObj);
	var nCount = 0;

	if (oTable)
	{
		var oBody = oTable.gridBody;

		if (oBody)
		{
			var i = 0;

			while(i < oBody.rows.length)
			{
				var row = oBody.rows[i];
				var bNotInc = false;

				if (row.selected)
				{
					nCount++;

					if (callBack)
						bNotInc = callBack(row, arg);
				}

				if (!bNotInc)	
					i++;
			}
		}
	}

	return nCount;
}

//Sort
function tableSort(obj, bByNumeric)
{
	var sortCol = obj;
	
	if (!sortCol)
		sortCol = getOwnerTD(window.event.srcElement)

	if (sortCol)
	{
		var nSortColIdx = sortCol.cellIndex;
		var tab = getOwnerTable(sortCol);

		setSortColumn(sortCol);
		
		var funSort;

		if (sortCol.sortDesc)
			funSort =  bByNumeric ? sortNumDesc : sortDesc;//降序排列
		else
			funSort =  bByNumeric ? sortNumAsc : sortAsc;

		tableBubbleSort(tab, nSortColIdx, 1, funSort);

		changeGridRowOddEvenColor(tab);
	}
}

function setSortColumn(sortCol)
{
	var nSortColIdx = sortCol.cellIndex;
	var tab = getOwnerTable(sortCol);

	if (tab)
	{
		if (tab.sortColIdx == nSortColIdx)
		{
			if (sortCol.sortDesc)
				sortCol.sortDesc = false;
			else
				sortCol.sortDesc = true;
		}
		else
		{
			var oldSortCol = getOwnerTR(sortCol).cells(tab.sortColIdx);
			oldSortCol.sortTag.style.visibility = "hidden";
			tab.sortColIdx = nSortColIdx;
			sortCol.sortDesc = false;
		}

		sortCol.sortTag.style.visibility = "visible";

		if (sortCol.sortDesc)
			sortCol.sortTag.innerText = "5";
		else
			sortCol.sortTag.innerText = "6";
	}
}

function sortDesc(vValue1, vValue2)
{
	return (vValue1 > vValue2);
}

function sortAsc(vValue1, vValue2)
{
	return (vValue1 < vValue2);
}

function sortNumAsc(vValue1, vValue2)
{
	vValue1 = Number(getRidOfChar(vValue1, ','));
	vValue2 = Number(getRidOfChar(vValue2, ','));

	return (vValue1 < vValue2);
}

function sortNumDesc(vValue1, vValue2)
{
	vValue1 = Number(getRidOfChar(vValue1, ','));
	vValue2 = Number(getRidOfChar(vValue2, ','));

	return (vValue1 > vValue2);
}

function getRidOfChar(str, chr)
{
	var aBuf;
	var strBuf = "";
	var nI;

	aBuf = str.split(chr);
	
	if (aBuf != null)
	{
		for (nI = 0; nI < aBuf.length; nI++)
			strBuf = strBuf + aBuf[nI];

		return strBuf;
	}
	else
		return "";
}

function getSortText(oTD)
{
	if (typeof(oTD.sortText) != "undefined")
		return oTD.sortText;
	else
		return oTD.innerText;
}

//排序
function tableBubbleSort(tab, nSortColIdx, nFromRow, functionSort)
{
	var end = tab.rows.length - 1 ;

	for ( var aa = 1; end > 1 && aa < end && isAutoBuildRow(tab.rows.item(aa)); aa++)
		for (var bb = aa + 1; bb <= end && isAutoBuildRow(tab.rows.item(bb)); bb++)
			if (functionSort(getSortText(tab.rows.item(aa).cells.item(nSortColIdx)), getSortText(tab.rows.item(bb).cells.item(nSortColIdx))))
				tab.moveRow(bb, aa);
}
//-->