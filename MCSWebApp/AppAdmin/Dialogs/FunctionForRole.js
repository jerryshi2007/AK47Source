function onDocumentLoad()
{
	
	for(var i = 0; i < dgFuncList.rows.length; i ++ )
	{
		dgFuncList.rows[i].cells[0].width = "15%";
		if ( i == 0 ) 
		{
			dgFuncList.rows[i].className = "gridHead";
			dgFuncList.rows[i].cells[0].all.item(0).onclick = checkAll;
		}
		else
		{
			//设样式
			if ( (i % 2) != 0 )
				dgFuncList.rows[i].className = "gridOddRow"; 
			else
				dgFuncList.rows[i].className = "gridEvenRow";
				
			//设事件
			dgFuncList.rows[i].onmouseover = changeClassHover;
			dgFuncList.rows[i].onmouseout = changeClassMoveOut;
			dgFuncList.rows[i].onclick = checkItem;
			
			//加图标
			var imgSrc = "../images/";
			if (dgFuncList.rows[i].TYPE == "F")
				imgSrc += "function.gif";
			else
				imgSrc += "functionSet.gif";
			
			
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

			dgFuncList.rows[i].cells[0].innerHTML += oSpan.outerHTML;
			
			//进行缩进
			for (var j = 0; j < (dgFuncList.rows[i].RESOURCE_LEVEL.length / 3); j++ )
				dgFuncList.rows[i].cells[0].innerHTML = "&nbsp;&nbsp;&nbsp;"+dgFuncList.rows[i].cells[0].innerHTML
				
		}
	}
}

function checkAll()
{
	
	var bCheck = dgFuncList.rows[0].cells[0].all.item(0).checked;
	for(var i = 1; i < dgFuncList.rows.length; i ++ )
	{
		dgFuncList.rows[i].cells[0].all.item(0).checked = bCheck;
	}
	SetCheckCount();		
}

function checkItem()
{
	var oCheck = event.srcElement;
	if ( oCheck.tagName.toUpperCase() != "INPUT")
		return;
	var oRow = oCheck.parentElement.parentElement;
	var index = oRow.rowIndex
	var rLevel = oRow.RESOURCE_LEVEL;
	var bCheck = oCheck.checked;
	
	for( var i = index + 1; i < dgFuncList.rows.length; i ++ )
	{
		if ( dgFuncList.rows[i].RESOURCE_LEVEL.length <= rLevel.length)
			break;
		else
		{
			if (  dgFuncList.rows[i].RESOURCE_LEVEL.indexOf(rLevel) == 0 )
			{
				dgFuncList.rows[i].cells[0].all.item(0).checked = bCheck;
			}
		}
		
	}
	SetCheckCount();
}

function SetCheckCount()
{
	var m_nCheckItemCount = 0;
	for(var i = 1; i < dgFuncList.rows.length; i ++ )
	{
		var oRow = dgFuncList.rows[i];
		var oCheck = oRow.cells[0].all.item(0);
		if (oRow.TYPE.toUpperCase() == "F")
		{
			var oldValue = ( oRow.ROLE_ID != "" )
				
			if ( oldValue != oCheck.checked )
				m_nCheckItemCount++;
			
		}
	}
	if (m_nCheckItemCount == 0)
		document.all.btnSave.disabled = true;
	else
		document.all.btnSave.disabled = false;
}

function SaveClick()
{
	var xmlUpdate = getXmlDoc();
	var xmlResult;
	xmlResult = xmlSend("../XmlRequestService/XmlRFDWriteRequest.aspx", xmlUpdate);
	try
	{
	checkErrorResult(xmlResult);
	}
	catch(e)
	{
		alert(e.message);
	}
	
	alert("保存修改成功！");
	window.returnValue = true;
	window.close();
}

function getXmlDoc()
{
	var xmlDoc = createDomDocument("<RTF/>");
	var root = xmlDoc.documentElement;
	
	var strTableName = "ROLE_TO_FUNCTIONS";
	var strRoleID	 = document.all.hdRoleID.value;
	var bAsk = ( parseInt(document.all.hdSupAppCount.value) > 0 );
	var bAddAttr = false;
	
	for(var i = 1; i < dgFuncList.rows.length; i ++ )
	{
		var row = dgFuncList.rows[i];
		var chk = row.cells[0].all.item(0);
		if (row.TYPE.toUpperCase() == "F")
		{
			var oldValue = ( row.ROLE_ID != "" )
				
			if ( oldValue != chk.checked )
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
					appendNode(xmlDoc, node, "ROLE_ID", strRoleID);
					appendNode(xmlDoc, node, "FUNC_ID", row.FUNC_ID);
				}
				if (!chk.checked)
				{
					if ( bAsk )
					{
						bAddAttr = confirm("如果关系被继承，是否删除继承的关系？\n选'确定'为是，不选或选'取消'为否");
						bAsk = false;
					}
					if (bAddAttr)
						appendAttr(xmlDoc, xmlDoc.firstChild, "deleteSubApp", "y");
					else
						appendAttr(xmlDoc, xmlDoc.firstChild, "deleteSubApp", "n");
					
					var node = root.selectSingleNode("Delete");
					if (!node) 
					{
						node = appendNode(xmlDoc, root, "Delete");
					}
					node = appendNode(xmlDoc, node, strTableName);
					addConditionFields(xmlDoc, node, "ROLE_ID", "=", row.ROLE_ID);
					addConditionFields(xmlDoc, node, "FUNC_ID", "=", row.FUNC_ID);
				}
				
			}
			
		}

	}

	return xmlDoc;
	

}