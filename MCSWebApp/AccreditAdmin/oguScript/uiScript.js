<!--
var C_ADD = 1;
var C_DELETE = 2;
var C_UPDATE = 3;
var C_WARNING = 4;
var C_STOPPING = 5;

//Disabled contextmenu event

if (document.oncontextmenu == null)
	document.oncontextmenu = onDefaultContextMenu;

function errorTrap(sMsg, sUrl, sLine)
{
	alert(sMsg + "\n 在文件: " + sUrl + "相关脚本的第" + sLine + "行");
	
	return true;
}

window.onerror = errorTrap;

function getAppMinorVersion()
{
	return navigator.appMinorVersion;
}

function onDefaultContextMenu()
{
	var obj = event.srcElement;

	if (obj.tagName == "TEXTAREA" || (obj.tagName == "INPUT" && (obj.type == "text" || obj.type == "file")))
		event.returnValue = true;
	else
		event.returnValue = false;

	return event.returnValue;
}

function jAlert(strMessage)
{
	alert(strMessage);
}

function frameWindow(strName)
{
	return top.window.frames(strName).window;
}

function frameDocument(strName)
{
	return top.window.frames(strName).window.document;
}

function iFrameDocument(strID)
{
	return document.frames(strID).document;
}

//从URL中分析出其根目录（紧跟在Server名称后面的目录）
function getRootDir(strURL)
{
	var nServer = strURL.indexOf("//") + 2;
	var nFileStart = strURL.length - 1;

	while ((nServer < strURL.length) && (strURL.substr(nServer, 1) != "/"))
			nServer++;

	while ((nFileStart >= nServer) && (strURL.substr(nFileStart, 1) != "/"))
		nFileStart--;

	if (nFileStart > nServer)
	{
		var nRoot = nServer + 1;

		while ((nRoot < strURL.length) && (strURL.substr(nRoot, 1) != "/"))
			nRoot++;

		return strURL.substring(nServer + 1, nRoot);
	}
	else
		return "";
}

//从URL中分析出其目录（忽略掉文件名）
function getCurrentDir(strURL)
{
	var nFileStart = strURL.length - 1;

	while ((nFileStart >= 0) && (strURL.substr(nFileStart, 1) != "/"))
		nFileStart--;

	return strURL.substring(0, nFileStart + 1);
}

//得到某个元素的起始点的X坐标
function absLeft(ele)
{
	var e = ele, left = 0;

	while(e.tagName != "BODY")
	{
		left += e.offsetLeft;
		e = e.offsetParent;
	}

	return left;
}

//得到某个元素的起始点的Y坐标
function absTop(ele)
{
	var e = ele,top = 0;

	while(e.tagName != "BODY")
	{
		top += e.offsetTop;
		e = e.offsetParent;
	}

	return top;
}

function onCalendarImgMouseOver()
{
	var obj = event.srcElement;

	if (obj.tagName == "IMG")
	{
		obj.oldClassName = obj.className;

		obj.className = "mouseOverNoMove";
		//obj.style.top = "2px";
	}
}

function onCalendarImgMouseOut()
{
	var obj = event.srcElement;

	if (obj.tagName == "IMG")
	{
		obj.className = obj.oldClassName;
	}
}

function onCalendarImgClick()
{
	var hCalendar = window.event.srcElement.objCalendar;
	var objInput = window.event.srcElement.objInput;

	if (!objInput.readOnly && !objInput.disabled)
	{
		hCalendar.style.pixelLeft = absLeft(objInput);
		hCalendar.style.pixelTop = absTop(objInput) + objInput.offsetHeight;

		if (hCalendar.style.pixelTop + hCalendar.offsetHeight > document.body.offsetHeight)
			hCalendar.style.pixelTop -= (objInput.offsetHeight + hCalendar.offsetHeight);

		hCalendar.srcInput = objInput;

		var dtInput = objInput.value;

		if (dtInput.length > 0)
		{
			var arrDate = dtInput.split(" ");
			var arr = arrDate[0].split("-");

			var dt = new Date(arr[0], arr[1] - 1, arr[2]);

			hCalendar.dateSelected = dt;
		}

		hCalendar.show(true);
	}
}

function onDateSelected()
{
	var dt = event.date;

	var year = dt.getFullYear();
	var month = dt.getMonth() + 1;
	var day = dt.getDate();

	hCalendar.srcInput.value = year + "-" + month + "-" + day;
	hCalendar.srcInput.formatValue = hCalendar.srcInput.value;

	//hCalendar.srcInput.fireEvent("onchange");//add by Zhou weihai
}

function initDatetimeImgAndInput(hCalendar, objInput)
{
	var img = document.createElement("IMG");

	var sPath = "/" + C_ACCREDIT_ADMIN_ROOT_URI + "/images/calendarDrop.gif";

	img.src = sPath;
	img.style.width = "16px";
	img.style.height = "14px";

	img.className = "toolButton";
	img.style.position = "relative";
	img.align = "absmiddle";

	img.onmouseover = onCalendarImgMouseOver;
	img.onmouseout = onCalendarImgMouseOut;
	img.onclick = onCalendarImgClick;

	img.objInput = objInput;
	img.objCalendar = hCalendar;

	img.showByState = objInput.showByState;
	img.showCondition = objInput.showCondition;

	if (!objInput.onchange)
		objInput.onchange = onDateCheck;

	if (objInput.nextSibling)	
		objInput.parentNode.insertBefore(img, objInput.nextSibling);
	else
		objInput.parentNode.appendChild(img);
}

function initCalendar(hCalendar)
{
	//hCalendar.rootDir = getRootDir(document.URLUnencoded);
	hCalendar.ondateclick = onDateSelected;
}

//将日期输入字段绑定一个日历牌
function bindCalendarToInput(hCalendar, objInput)
{
	initDatetimeImgAndInput(hCalendar, objInput);

	initCalendar(hCalendar);
}

//将所有日期字段绑定到一个日历牌(必须执行过initElementsByDict)
function bindCalendarToAllInput(hCalendar)
{
	var allDateTimeBind = document.all;
	if (arguments.length > 1)
		allDateTimeBind = arguments[1];
		
	for (var i = 0; i < allDateTimeBind.length; i++)
	{
		var obj = allDateTimeBind[i];

		if (isUndefined(typeof(obj.nodeColumn)) == false)
		{
			var strType = getXSDColumnAttr(obj.nodeColumn, "type")
			var strType = strType.split(":")[1];

			if (isXSDDatetimeType(strType))
			{
				initDatetimeImgAndInput(hCalendar, obj);
			}
		}
	}

	initCalendar(hCalendar);
}

//从传来的字符串中解析出参数
function getParaFromStr(strText)
{
	var strPara = new Array();
	
	strPara = strText.split("&");
	
	return strPara;
}

//显示自制对话框********************
//function showCmdDialog(xmlDoc)
//{
//	var sFeature = "dialogWidth:280px; dialogHeight:180px;center:yes;help:no;resizable:no;scroll:no;status:no";
//	var sPath = "/" + getRootDir(document.URLUnencoded) + "/CommonDialog/cmdDialog.htm";
//
//	return showModalDialog(sPath, xmlDoc, sFeature);//Deleted
//}

//function alert(strMsg, iconType)
//{		
//	return showCmdDialog(getXmlDocForDialog(strMsg, iconType, "alert"));
//}
//
//function confirm(strMsg, bDefaultYes, iconType)
//{	
//	return showCmdDialog(getXmlDocForDialog(strMsg, iconType, "confirm", bDefaultYes));
//}

function showError(e, iconType)
{
	var strMsg = getErrorMessage(e);
	alert(strMsg);
//	if (isHttpError(e))
//	{
//		document.body.innerHTML = getErrorMessage(e);
//	}
//	else
//	{			
//		var strMsg = getErrorMessage(e);			
//
//		return showCmdDialog(getXmlDocForDialog(strMsg, iconType, "stop"));
//	}
}	

//function getXmlDocForDialog(strMsg, iconType, type, bDefaultYes)
//{		
//	if (bDefaultYes == null)
//		bDefaultYes = false;
//		
//	var arrayInfo = getInfo(strMsg, iconType);
//	
//	var xmlDoc = createDomDocument("<MessageBox/>");
//	
//	appendNode(xmlDoc.documentElement, "Type", type);			
//	appendNode(xmlDoc.documentElement, "Message", arrayInfo[0]);
//	appendNode(xmlDoc.documentElement, "IconType", arrayInfo[1]);
//	appendNode(xmlDoc.documentElement, "DefaultYes", bDefaultYes);
//	
//	return xmlDoc;
//}

//function getInfo(strMsg, iconType)
//{		
//	return (new Array(getStrMsg(strMsg), getIconType(iconType))) ;
//}
	
//获得strMsg
function getStrMsg(strMsg)
{		
	if (typeof(strMsg) == "number")	
		strMsg = strMsg.toString();
	
	switch(strMsg)
	{			
		case "1":	strMsg = "增加成功！";
					break;
					
		case "2":	strMsg = "删除成功！";
					break;
		
		case "3": 	strMsg = "修改成功！";
					break;
		
		case "4":	strMsg = "警告！！";
					break;
		
		case "5":	strMsg ="停止！！";
					break;	
								
	}
	
	return strMsg;								
}

//获得图标类型
function getIconType(iconType)
{				
	switch(iconType)
	{
		case "stopIcon":			
		case "warnIcon":			
		case "askIcon":	
					break;
		default:
					iconType = "defalut";
	}
	
	return iconType;				
}

//得到文件名，包含后缀，但不包含路径部分
//例如getFileName("c:\\windows\\notepad.exe")，结果为notepad.exe
function getFileNameWithExt(strFile)
{
	var nFileNameStart = strFile.lastIndexOf("\\");

	return strFile.substring(nFileNameStart + 1, strFile.length);
}

//得到文件名，不包含后缀，也不包含路径部分
//例如getFileName("c:\\windows\\notepad.exe")，结果为notepad
function getFileName(strFile)
{
	var strFileName = getFileNameWithExt(strFile);

	var nExtStart = strFileName.lastIndexOf(".");

	if (nExtStart == -1)
		nExtStart = 0;

	return strFileName.substring(0, nExtStart);
}

//得到文件的路径和文件名，但是不包括扩展名。
//例如getFileNameWithPath("c:\\windows\\notepad.exe")，结果为c:\windows\notepad
function getFileNameWithPath(strFile)
{
	var nExtStart = strFile.lastIndexOf(".");

	if (nExtStart == -1)
		nExtStart = 0;

	return strFile.substring(0, nExtStart);
}

//得到文件的扩展名，不包含“.”
function getFileType(strFile)
{
	var strFileName = getFileNameWithExt(strFile);

	var nExtStart = strFile.lastIndexOf(".");

	var strType = "";

	if (nExtStart >= 0)
		strType = strFile.substring(nExtStart + 1, strFile.length).toLowerCase();

	return strType;
}

//得到文件的路径部分。
//例如getFilePath("c:\\windows\\notepad.exe")，结果为c:\windows，不包含最后的"\"
function getFilePath(strFile)
{
	var nFileNameStart = strFile.lastIndexOf("\\");

	if (nFileNameStart == -1)
		nFileNameStart = 0;

	return strFile.substring(0, nFileNameStart);
}

//根据文件名返回文件的应用名称
function getAppNameByFileName(strFileName)
{
	var strType = getFileType(strFileName);
	var strAppName = "";

	switch(strType)
	{
		case "dot":
		case "doc":
					strAppName = "Microsoft Word";
					break;
		case "ppt":
		case "pps":
		case "ppa":
					strAppName = "Microsoft PowerPoint";
					break;
		case "csv":
		case "xla":
		case "xls":
		case "xlw":
					strAppName = "Microsoft Excel";
					break;
	}

	return strAppName;
}

function getIconByFileName(strFile, bIsBig)
{
	var strDir = "/" + C_ACCREDIT_ADMIN_ROOT_URI + "/images/";

	if (bIsBig)
		strDir += "32/";

	var strResult = "WordPad.gif";

	var strType = getFileType(strFile);

	switch(strType)
	{
		case "jpg":
		case "jpeg":
		case "gif":
		case "tif":
		case "tiff":
					strResult = "image.gif";
					break;
		case "bmp":
		case "pcx":
		case "wmf":
					strResult = "bmp.gif";
					break;
		case "dot":
		case "doc":
					strResult = "word.gif";
					break;
		case "zip":	strResult = "winzip.gif";
					break;
		case "pdf":	strResult = "pdf.gif";
					break;
		case "xsd":
		case "xml":
		case "mht":
		case "html":
		case "htm":	strResult = "htm.gif";
					break;
		case "ppt":
		case "pps":
		case "ppa":
					strResult = "ppt.gif";
					break;
		case "gd":
		case "gw":
		case "gw2":
		case "ps2":
		case "s2":
		case "s72":
		case "s92":
					strResult = "shusheng.gif";
					break;
		case "mp3":
		case "wav":
		case "wma":
					strResult = "sound.gif";
					break;
		case "mpeg":
		case "mpg":
		case "avi":
		case "wmv":
					strResult = "wmp.gif";
					break;
		case "csv":
		case "xla":
		case "xls":
		case "xlw":
					strResult = "excel.gif";
					break;
	}

	return strDir + strResult;
}

function createImgSpan(strImg, nWidth, nHeight)
{
	var span = document.createElement("span");

	with (span)
	{
		if (!nWidth)
			nWidth = 16;

		if (!nHeight)
			nHeight = 16;

		style.width = nWidth;
		style.height = nHeight;

		style.backgroundImage = "url(" + strImg + ")";
		style.backgroundPosition = "center center";
		style.backgroundRepeat = "no-repeat";
	}

	return span;
}

function createTextSpan(strText, nWidth, nHeight)
{
	var oSpan = document.createElement("span");

	with (oSpan)
	{
		innerText = strText;

		if (!nHeight)
			style.height = 16;
	}

	return oSpan;
}

function createFileTypeSpan(strFileName)
{
	var strImg = getIconByFileName(strFileName);

	return createImgSpan(strImg);
}

function createLinkText(strText, strHref)
{
	var a = document.createElement("a");

	with (a)
	{
		innerText = strText;
		a.href = strHref;
	}

	return a;
}

function createImgLinkText(paremtElement, strImg, strText, strHref)
{
	var img = createImgSpan(strImg);

	paremtElement.appendChild(img);

	var a = document.createElement("a");

	with (a)
	{
		style.marginLeft = "4px";
		innerText = strText;
		a.href = strHref;
	}

	paremtElement.appendChild(a);

	return a;
}

//打开一个标准的打开附件的窗口
function openMaterialWindow(strURL)
{
	return window.open(strURL, "Material", "menubar=yes,toolbar=no,location=no,resizable=yes");
}


function getExtraDisplayShow(xmlDoc, oTable, strNodeName, strObjType)
{
	var root = xmlDoc.documentElement.selectSingleNode(strNodeName);
	if (root)
	{
		var oRoot = root.selectSingleNode(strObjType);
		if (oRoot)
		{
			var oNodes = oRoot.selectNodes("ITEM[Show=\"y\"]");
			
			var i = 0;
			while (i < oNodes.length)
			{
				var iNode= oNodes[i];
				var oTR = oTable.insertRow();
				var oTD = oTR.insertCell();
				oTD.align = "right";
				oTD.style.width = "80px";
				oTD.style.height = "24px";
				oTD.innerHTML = "<strong>" + iNode.selectSingleNode("Text").text + "</strong>:";
				
				oTD = oTR.insertCell();
				
				var strType = "";
				if (iNode.selectSingleNode("Type"))
					strType = iNode.selectSingleNode("Type").text;
					
				var obj;
				switch (strType.toLowerCase())
				{
					case "select":
						obj = document.createElement("SELECT");
						var pNodes = iNode.selectSingleNode("Options");
						if (pNodes)
						{
							if (pNodes.getAttribute("withNull") == "1")
							{
								var nObj = document.createElement("OPTION")
								nObj.value = "";
								nObj.text = "---";
								obj.options.add(nObj);
							}
							var pNode = pNodes.firstChild;
							while (pNode)
							{
								var pObj = document.createElement("OPTION");
								pObj.value = pNode.getAttribute("value");
								pObj.text = pNode.getAttribute("text")
								obj.options.add(pObj);
								
								pNode = pNode.nextSibling;
							}
						}						
						break;
					case "textarea":
						obj = document.createElement("TEXTAREA");
						obj.style.width = ((strObjType == "OU_USERS" || strObjType == "USERS") ? "90%" : "95%");
						break;
					default:
						obj = document.createElement("INPUT");
						if (strType == "radio" || strType == "checkbox")
							obj.style.border = "none";
						else
							obj.style.width = ((strObjType == "OU_USERS" || strObjType == "USERS") ? "90%" : "95%");
						obj.type = strType;
						break;
				}
				
				obj.id = iNode.selectSingleNode("ID").text;
				obj.name = obj.id;
				obj.dataFld = obj.id;
				obj.dataSrc = (strObjType == "OU_USERS" ? "USERS" : strObjType);
				
				var titleNode = iNode.selectSingleNode("Title");
				if (titleNode)
					obj.title = titleNode.text;
				var valueNode = iNode.selectSingleNode("Value");
				if (valueNode)
					obj.value = valueNode.text;
				var classNode = iNode.selectSingleNode("Class");
				if (classNode)
					obj.className = classNode.text;
				
				oTD.appendChild(obj);
				
				i++;
			}
		}
	}
}
//-->