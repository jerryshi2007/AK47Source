<!--
var m_Type;
var deleteGuidXml = null;

function fillGridCaption(titleCaption, strType, headXMLIn, dbGrid)
{
	m_Type = strType;
	inDbGrid = adminDbGrid;
	if(dbGrid)
		inDbGrid = dbGrid;
	with(inDbGrid)
	{
		if(headXMLIn)
			headerXML = headXMLIn.XMLDocument;
		else
			headerXML = headXml.XMLDocument;
		showCaption = true;
		
		caption = "<span style='filter:;BACKGROUND-POSITION: center center; BACKGROUND-IMAGE: url(../images/edit.gif);BACKGROUND-REPEAT: no-repeat;width:20px;height:20;'></span>" +
							"<label style='position:relative;top:0px;margin-left:4px' id ='adminTitle'>" + titleCaption + "</label> ";
		//变更变量命名从spanSearch -> spanRefresh
		var spanRefresh = document.createElement("span");
		with (spanRefresh)
		{
			style.width = 16;
			style.height = 16;
			style.display = "none";

			style.backgroundImage = "url(../images/refresh.gif)";
			style.backgroundPosition = "center center";
			style.backgroundRepeat = "no-repeat";
			style.marginLeft = "16px";
			style.cursor = "hand";
			title = "刷新";					
			onclick = onRefreshData;
		}

		captionElement.appendChild(spanRefresh);
		
		var spanCount = document.createElement("span");
		with (spanCount)
		{
			style.height = 16;
			style.marginRight = "16px";
			id = "countSpan";
		}
		captionElement.appendChild(spanCount);
	}
}

function onDNlLinkClick()
{
	try
	{
		var a = event.srcElement;
		var strLink;	
		strLink = "RemainEquipmentDistibute.aspx?guid=" + a.key;
		var arg = new Object();
		arg.guid = a.key;
		arg.type = m_Type;
		arg.workFlag = a.workFlag;
		var sFeature = "dialogWidth:600px; dialogHeight:200px;center:yes;help:no;resizable:no;scroll:no;status:no";
	
		var returnValue = showModalDialog(strLink, arg, sFeature);
		
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


//刷新
function onRefreshData()
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
//作用：响应search上onclick事件
//返回: null
//参数：null
//版本：1.0
function onSearchData()
{
	try
	{
		//指定打开页面的链接地址
		var strLink = "Search.htm";
		//初始化传入页面的参数
		var arg = new Object();
		arg.title = document.title;
		arg.page = m_Type;
		m_searchStr = showDialog(arg, strLink);
		if (m_searchStr != null && m_searchStr != "")
		{
			adminDbGrid.dataXML = queryData("listAll", "" , m_searchStr);
			//searchData(m_searchStr);
		}
	}
	catch(e)
	{
		showError(e);
	}		
}

//下一页
function onGridNextPage()
{
	try
	{
		var lastKey = adminDbGrid.currentPage * adminDbGrid.limitRows;
		event.returnValue = queryData(lastKey);
	}
	catch(e)
	{
		showerror(e);
	}	
}

function showDialog(arg, strLink)
{
	var Dwidth = window.screen.width * 0.60;
	var Dheight = window.screen.height * 0.60;
	var sFeature = "dialogWidth:" + Dwidth + "px; dialogHeight:" + Dheight + "px;center:yes;help:no;resizable:yes;scroll:yes;status:no";
	
	return showModalDialog(strLink, arg, sFeature);
}

function setTitleCell(oTD, nodeText)
{
	var currObj = oTD;
	oTD.innerText = nodeText;
}

function setOpCell(oTD, xmlNode, nodeText, fieldName)
{	
	oTD.innerText = "";
	var chk = document.createElement("input");
	chk.type = "checkBox";
	chk.style.border = "none";
	chk.id = getSingleNodeText(xmlNode, "GUID");
	chk.onclick = saveGuid;
	oTD.insertAdjacentElement("afterBegin", chk);
}

//设置链接属性
function setLinkCell(oTD, xmlNode, cellText, workFlag,callBack)
{
	var currObj = oTD;
	var a = document.createElement("a");
	a.href = "";
	a.onclick = callBack;									
	a.key = getSingleNodeText(xmlNode, "GUID");					//关键字
	a.workFlag = workFlag;
	a.innerText = cellText;											//显示名称
	oTD.innerText = "";
	oTD.insertAdjacentElement("afterBegin", a);						//显示	
}


function setButtonCell(oTD, xmlNode, cellText, idName,callBack, butText)
{
	var inText = "详细";
	if(butText)
		inText = butText;
	var currObj = oTD;
	currObj.align = "middle";
	var a = document.createElement("button");
	a.value = inText;
	a.height = "95%" 
	a.width = "100%" 
	a.onclick = callBack;									
	a.key = getSingleNodeText(xmlNode, idName);					//关键字
	oTD.innerText = "";
	oTD.insertAdjacentElement("afterBegin", a);						//显示	
}

//-->
