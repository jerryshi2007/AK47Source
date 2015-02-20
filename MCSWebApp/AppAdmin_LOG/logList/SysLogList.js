
<!--
var m_searchStr
var m_count;
function onDocumentLoad()
{
	try
	{
		var type = "SysLogList";
		var titleCaption = "系统日志列表";
		
		bindCalendarToInput(hCalendar, frmInput.start_time);
		bindCalendarToInput(hCalendar, frmInput.end_time);
		
		fillGridCaption(titleCaption, type, headSysXml);
		adminDbGrid.dataXML = queryData();
				
		}
	catch(e)
	{
		showError(e);
	}	
}

function queryData(strLastKey, searchStr)
{
	if (strLastKey == null || strLastKey == "")
	{
		strLastKey = 0;
	}
	var time1 = frmInput.start_time.value;
	var time2 = frmInput.end_time.value;
	var xmlDoc = createDomDocument("<SysLogList/>");
	var root = xmlDoc.documentElement;
	if(time1 == "")
		time1 = "1753-01-01"
	if(time2 == "")
		time2 = "9999-12-31"
		
	root.setAttribute("start_time", time1);
	root.setAttribute("end_time", time2);

	root.setAttribute("rows", adminDbGrid.limitRows);
	root.setAttribute("lastKey", strLastKey);

	var xmlResult = xmlSend("../server/ServerLog.aspx", xmlDoc);
	checkErrorResult(xmlResult);
	
	var countNode = xmlResult.documentElement.lastChild;
	m_count = countNode.text;

	var countPage = parseInt(m_count);
	countPage = Math.floor((m_count - 1) / adminDbGrid.limitRows) + 1;
	countSpan.innerText = "       按条件查询一共 " + m_count + " 条  分 " + String(countPage) + " 页显示";
	xmlResult.documentElement.removeChild(countNode);
	return xmlResult;
}

function onGridCalcCell()
{
	try
	{			
		switch (event.fieldName)
		{
			
			case "LOG_DATE":
				setTitleCell(event.senderElement, event.nodeText);
				break;
				
			case "USER_LOGONNAME":
				setTitleCell(event.senderElement,event.nodeText);
				break;
			case "HOST_IP":
				setTitleCell(event.senderElement,event.nodeText);
				break;
				/*
			case "SUBCUST":
				if(event.nodeText)
					event.senderElement.innerText = getNameFromDN(event.nodeText);
				break;	
				*/
			case "Detail":
				setButtonCell(event.senderElement, event.xmlNode, event.nodeText, "ID", onDetailButtonClick);
			default:
				break;				
		}
	}
	catch(e)
	{
		showError(e);
	}
}

function onSearchClick()
{
	try
	{
			adminDbGrid.dataXML = queryData();
	}
	catch(e)
	{
		showError(e);
	}


}

function onDetailButtonClick()
{
	try
	{
		var a = event.srcElement;
		var strLink;	
		strLink = "SysLogDetail.aspx?sortID=" + a.key;
		
		var sFeature = "dialogWidth:620px; dialogHeight:400px;center:yes;help:no;resizable:no;scroll:no;status:no";
	
		var returnValue = showModalDialog(strLink, null, sFeature);
		
		if (returnValue == "refresh")
		{
			adminDbGrid.dataXML = queryData();
		}
	}
	catch(e)
	{
		showError(e);
	}
	finally
	{
		event.returnValue = false;
	}
}

//-->