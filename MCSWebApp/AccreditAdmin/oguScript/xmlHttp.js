<!--
/*************************************************
	功能：	用于客户端的xml访问
**************************************************/

//建立Automation对象
function createObject(strName, strDescription)
{
	try
	{
		var stm = new ActiveXObject(strName);

		return stm;
	}
	catch(e)
	{
		var strMsg = "您的计算机没有安装" + strName + "，或者您的浏览器为该网页没有设置本地访问权限";

		if (strDescription)
			strMsg += ", " + strDescription;
		throw strMsg;
	}
}

//判断一个对象是否为数组
function paramIsArray(v)
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

//将子节点的值转置为属性
function transformNode(xmlNode, strNewNodeName)
{
	var xmlDoc = createDomDocument("<" + strNewNodeName + "/>");
	var root = xmlDoc.documentElement;

	var node = xmlNode.firstChild;

	while(node)
	{
		root.setAttribute(node.nodeName, node.text);
		node = node.nextSibling;
	}

	return xmlDoc;
}

//将某一个根节点的子节点复制到另一个根节点下
//bRecursive:表示是否复制字节点
//strIgnorNames表示以逗号分割的节点名称，这些名称将不被复制
function cloneNodes(rootDest, rootSrc, bRecursive, strIgnorNames)
{
	var node = rootSrc.firstChild;
	
	var arrNames = null;

	if (strIgnorNames)
		arrNames = strIgnorNames.split(",");

	while (node)
	{
		var bClone = true;
		
		if (arrNames && dataInArray(node.nodeName, arrNames))
			bClone = false;
		
		if (bClone)
			rootDest.appendChild(node.cloneNode(bRecursive));
		
		node = node.nextSibling;
	}		
}

function cloneXmlNode(rootDest, rootSrc, strXPath, bRecursive)
{
	var node = rootSrc.selectSingleNode(strXPath);
	var nodeResult = null;

	if (node)
	{
		nodeResult = rootDest.appendChild(node.cloneNode(bRecursive));
	}

	return nodeResult;
}

//在指定节点上增加一个节点
function appendNode(root, strNodeName)
{
	var xmlDoc = root.ownerDocument;
	var nodeText = "";

	if (arguments.length > 2)
		nodeText = arguments[2];

	var node = xmlDoc.createNode(1, strNodeName, "");

	if (nodeText.toString().length > 0)
		node.text = nodeText;

	root.appendChild(node);

	return node;
}

//在指定节点上增加一个属性
function appendAttr(node, strAttrName)
{
	var xmlDoc = node.ownerDocument;
	var nodeText = "";

	if (arguments.length > 2)
		nodeText = arguments[2];

	var attr = xmlDoc.createAttribute(strAttrName);

	if (nodeText != "")
		attr.value = nodeText;

	node.attributes.setNamedItem(attr);

	return attr;
}

//返回一个节点的属性值，如果没有这个属性，则返回空串
function getAttrValue(node, strAttrName)
{
	var attr = node.attributes.getNamedItem(strAttrName);

	if (attr)
		return attr.value;
	else
		return "";
}

//返回一个节点的内容，如果节点为空，则返回缺省值
function getNodeText(node, strDefault)
{
	var strResult = "";

	if (node)
		strResult = node.text;
	else
	if (strDefault)
		strResult = strDefault;

	return strResult;
}

function getNodeAttribute(node, strAttr)
{
	var attrText = node.getAttribute(strAttr);

	if (!attrText)
		attrText = "";

	return attrText;
}

//从nodeRoot中找到strPath节点的值为strValue
function getSingleNodeWithNodeValue(nodeRoot, strXPath, strValue)
{
	var nodes = nodeRoot.selectNodes(strXPath);
	for (var i = 0; i < nodes.length; i++)
	{
		if (nodes[i].text == strValue)
			return nodes[i];
	}
	return null;
}

function getSingleNodeText(nodeRoot, strPath, strDefault)
{
	var node = nodeRoot.selectSingleNode(strPath);

	return getNodeText(node, strDefault);
}

var innerXmlHttp = null;
var innerHandleStateChange = null;
var innerParam = null;

//发送xmlHttpRequest
function xmlSend(strURL, xmlDoc, callBack, param)
{
	innerXmlHttp = createObject("Msxml2.XmlHttp", "请安装Microsoft最新版本的XML解析器或最新版本的Internet Explore (Internet Explore 6.0 以上版本)");

	var strDoc;

	if (typeof(xmlDoc) == "object")
		strDoc = xmlDoc.xml;
	else
		strDoc = xmlDoc;

	
	var bAysync = false;

	if (callBack)
	{
		bAysync = true;
		innerXmlHttp.onreadystatechange = handleStateChange;
		innerParam = param;
		innerHandleStateChange = callBack;
	}

	innerXmlHttp.open("POST", strURL, bAysync);
	innerXmlHttp.send(strDoc);

	if (bAysync)
		return innerXmlHttp;
	else
	{
		if (innerXmlHttp.responseXML.xml.length > 0)
			return innerXmlHttp.responseXML;
		else
			return createErrorXML(innerXmlHttp.responseText, "");
	}
}

function handleStateChange()
{
	if (innerXmlHttp.readyState == 4)
	{
		if (innerHandleStateChange)
			innerHandleStateChange(innerXmlHttp.responseXML, innerParam);

		innerHandleStateChange = null;
	}
}

//建立xmlDocument对象
function createDomDocument()
{
	var xmlData;
	
	try
	{
		xmlData = createObject("Msxml2.DOMDocument");
	}
	catch(e)
	{
		xmlData = createObject("Msxml.DOMDocument");
	}
	
	xmlData.async = false;

	if (arguments.length > 0)
	{
		var xml = arguments[0];
		if (typeof(xml) == "string")
			xmlData.loadXML(xml);
		else
		if (typeof(xml) == "object")
			xmlData.loadXML(xml.xml);
	}
	
	return xmlData;
}

//建立一条命令
function createCommandXML(strCommand)
{
	var xmlData = createDomDocument();

	var root = xmlData.createElement("ClientCommand");

	var nodeCommand = appendNode(root, "COMMAND");
	appendAttr(nodeCommand, "name", strCommand);

	xmlData.documentElement = root;

	if (arguments.length > 1)
	{
		if (paramIsArray(arguments[1]) == false)
		{
			for (var i = 1; i < arguments.length; i++)
				if (typeof(arguments[i]) != "undefined")
					appendNode(nodeCommand, "PARAM", arguments[i]);
		}
		else
		{
			for (var i = 0; i < arguments[1].length; i++)
				appendNode(nodeCommand, "PARAM", arguments[1][i]);
		}
	}

	return xmlData;
}

//从一个节点生成仅包含一个节点(包括该节点的属性)的XML文档对象
function createDomDocumentFromNode(node)
{
	var xmlDoc = createDomDocument("<" + node.nodeName + "/>");
	var root = xmlDoc.documentElement;

	for (var i = 0; i < node.attributes.length; i++)
	{
		var attr = node.attributes[i];

		root.setAttribute(attr.nodeName, attr.value);
	}

	return xmlDoc;
}

//取得Server端XML Http Request的结果
function getSingleResult(xmlDoc)
{
	var n;
	var i;

	for (var i = 0; i < xmlDoc.childNodes.length; i++)
	{
		n =  xmlDoc.childNodes(i);

		var nValue = xmlDoc.selectSingleNode(".//Value");

		var strResult = "";

		if (nValue != null)
			strResult = nValue.text;

		if (n.nodeName == "ResponseError")
			throw createErrorObj(xmlDoc);
	}

	return strResult;
}

//检查Server端XML Http Request的结果
function checkErrorResult(xmlDoc, information)
{
	if (xmlDoc.documentElement.nodeName == "ResponseError")
	{
		//throw xmlDoc.selectSingleNode("Value").text;
		throw createErrorObj(xmlDoc);
	}
	else
	if ((xmlDoc.documentElement.nodeName == "ResponseOK") && (information != null))
	{
		alert(information);
	}
}

//建立错误对象，对象包含错误信息和错误栈的情况
function createErrorObj(xmlDoc)
{
	var eObj = new Object();

	eObj.message = xmlDoc.selectSingleNode(".//Value").text;
	eObj.stack = "";
	if (xmlDoc.selectSingleNode(".//Stack"))
		eObj.stack = xmlDoc.selectSingleNode(".//Stack").text;

	var clientNode = xmlDoc.selectSingleNode(".//Client");

	if (clientNode)
		eObj.client = clientNode.text;
	else
		eObj.client = "";

	return eObj;	
}

//构造一个和服务器返回信息相同的XML
function createErrorXML(errValue, errStack)
{
	var xmlDoc = createDomDocument("<ResponseError/>");

	appendNode(xmlDoc.documentElement, "Value", errValue);
	appendNode(xmlDoc.documentElement, "Stack", errStack);
	appendNode(xmlDoc.documentElement, "Client", "true");

	return xmlDoc;
}

//从错误对象中得到错误信息
function getErrorMessage(e)
{
	if (typeof(e) == "object")
	{
		if (!e.number)
			return e.message;
		else
			throw e;
	}
	else
		return e;
}

function isHttpError(e)
{
	if (typeof(e) == "object")
	{
		if (typeof(e.client) != "undefined")
			return e.client.length > 0;
		else
			return false;
	}
	else
		return false;
}

//从错误对象中得到错误栈信息
function getErrorStack(e)
{
	if (typeof(e) == "object")
		return e.stack;
	else
		return "";
}

//建立一个Insert的Xml Document
function createInsertDoc(collection, tableName, getOperation)
{
	var xmlData = createDomDocument();

	var root = xmlData.createElement("Insert");	//区分查询、插入、修改的标记

	var nodeCommand = appendNode(root, tableName);
	var node = appendNode(nodeCommand, "SET");
		
	getInfoFromCollection(node, collection, getOperation)
	
	xmlData.documentElement = root;
	
	return xmlData;
}

//如果在查询条件下，第三个参数必须不能为空
function getInfoFromCollection(xmlNode, collection, getOperation)
{
	var obj = new Object();
	
	for (var i = 0; i < collection.length; i++)
	{
		var currObj = collection(i);

		if ((currObj.dataFld || currObj.id) && 
			(currObj.dataSrc == xmlNode.parentNode.nodeName))
		{
			var strValue = originalValue(currObj);

			if (currObj.tagName=="INPUT" && currObj.type == "checkbox")
			{
				if (currObj.checked)
					strValue = "y";
				else
					strValue = "n";
			}

			var strFld = currObj.dataFld;

			if (!strFld)
				strFld = currObj.id;

			obj.dataFld = strFld;
			obj.oper = null;
			obj.value = originalValue(currObj);

			if (strValue.length > 0) //判断输入框是否空
			{
				if (getOperation)
					obj = getOperation(currObj, obj);

				if (xmlNode.nodeName == "WHERE") //判断条件的where 部分有效
				{
					if (obj.oper != null)
					{
						var TempNode = appendNode(xmlNode, obj.dataFld, obj.value)
					
						appendAttr(TempNode, "operator", obj.oper);
					}
				}
				else
				{	//在insert,update时有效
					if (isUndefined(typeof(currObj.oldValue)) == true)
					{
						appendNode(xmlNode, strFld, obj.value);
					}
					else
					if (currObj.oldValue != obj.value)
					{
						appendNode(xmlNode, strFld, obj.value);
					}
				}
			}
			else//特殊情况，在update时输入空，但新旧值不同时有效
			if ((currObj.oldValue) && (currObj.oldValue != strValue))
			{
				if (getOperation)
					obj = getOperation(currObj, obj);

				appendNode(xmlNode, currObj.dataFld, obj.value);
			}
				
		}
	}
}
	
/*********************
/*一组操作Select的函数
/*********************/
function createSelectDoc(DisplayFields, RowCount, tableName, conditions, getOperation)
{
	var xmlSelDoc = createDomDocument("<DOCUMENT><SCHEMA/></DOCUMENT>");
	
	var schemaNode = xmlSelDoc.selectSingleNode(".//SCHEMA");
	
	appendAttr(schemaNode, "rowcount", RowCount);
	
	var tableNode = findTableFromXml(xmlSelDoc, tableName, true);
	
	addDisplayFields(tableNode, DisplayFields);

	if (arguments[3] != null)
		addConditionFields(tableNode, conditions, getOperation);
	
	return xmlSelDoc;
	
}

//为xml文件中增加标志，作为服务端解析的依据
function addFlagForServer(xmlDoc, Param)
{
	if ((xmlDoc != null) && (Param != ""))		
		appendAttr(xmlDoc.documentElement, "param", Param);
}

//在xml文档中找到指定表名称的节点，查询xml，schemaFlag = true
function findTableFromXml(xmlDoc, tableName, schemaFlag)
{
	var tableNode;
	
	if (schemaFlag == true)
	{
		tableNode = xmlDoc.selectSingleNode(".//SCHEMA/" + tableName);
		if (tableNode == null)
			tableNode = appendNode(xmlDoc.selectSingleNode(".//SCHEMA"), tableName);
	}
	else
	{
		tableNode = xmlDoc.selectSingleNode(".//" + tableName);
		if (tableNode == null)
			tableNode = appendNode(xmlDoc.documentElement, tableName);
	}
	
	return tableNode;
}

//增加在查询中要显示的字段
function addDisplayFields(tableNode, DisplayFields)
{
	//增加display节点
	var displayNode = appendNode(tableNode, "DISPLAY");
	
	if (paramIsArray(DisplayFields) == true )
	{
		for (var i = 0;i < DisplayFields.length; i++)
			appendNode(displayNode, DisplayFields[i]);
	}
	else
	{	
		if ((DisplayFields) && (DisplayFields != ""))
		{
			var TempNode = appendNode(displayNode, DisplayFields);
			if (arguments.length == 3)
				TempNode.text = arguments[2];
		}
	}
}


//增加条件的字段(Where部分)
//第二个参数可以为数据集和字符串,如果是数据集后面是回调函数（可选），如果是字符串
//第二个参数是字段名，第三个是操作，第四个是数值，第五个是类型（可选）
function addConditionFields(tableNode)
{	
	var conditionNode = tableNode.selectSingleNode("WHERE");
	
	if (conditionNode == null)
		conditionNode = appendNode(tableNode, "WHERE");
	
	if (arguments.length == 3)
		getInfoFromCollection(conditionNode, arguments[1], arguments[2])
	else
		if (arguments.length > 3)
		{
			var TempNode = appendNode(conditionNode, arguments[1], arguments[3]);
			
			if ((arguments[2] != null) && (arguments[2] != ""))
				appendAttr(TempNode, "operator", arguments[2]);
			
			if (arguments.length == 5)
				appendAttr(TempNode, "type", arguments[4]);
		}
}

//建立一个Update的XML Document
//第一个参数是数据集，第二个参数是数据表名，第三个参数是回调函数（可选）
//如果第三个参数是字段名，第四个参数是操作符，第五个参数是该字段的值
function createUpdateDoc(collection, tableName)
{
	var xmlDoc = createDomDocument("<Update/>");
	var tableNode = findTableFromXml(xmlDoc, tableName);
	var node = appendNode(tableNode, "SET");
	
	getInfoFromCollection(node, collection);
	
	if (arguments.length > 3)
	{
		addConditionFields(tableNode, arguments[2], arguments[3], arguments[4])
	}
	else
	{
		if (arguments.length == 3)
		{
			addConditionFields(tableNode, collection, arguments[2]);
		}
		else
		{
			addConditionFields(tableNode, collection);
		}
	}

	return xmlDoc;
}

//建立一个Update的XML Document
//第一个参数是数据集，第二个参数是数据表名，第三个参数是回调函数（可选）
//如果第四个参数是Where条件的字段名，第五个参数是操作符，第六个参数是该字段的值
function createUpdateDoc2(collection, tableName, getOperation, strWhereField, strOP, strWhereValue)
{
	var xmlDoc = createDomDocument("<Update/>");
	var tableNode = findTableFromXml(xmlDoc, tableName);
	var node = appendNode(tableNode, "SET");

	getInfoFromCollection(node, collection, getOperation);

	if (strWhereField)
		addConditionFields(tableNode, strWhereField, strOP, strWhereValue);

	return xmlDoc;
}

//根据一个xsd文件中的条件创建一个规范的update SQL对应的XMLDocument
//collection：对应数据的数据集和
//tableName：对应数据表的表名称
//xsdDict：对应的数据字典
//getOperation：回调函数
//这里的where条件由数据字典中的isKey决定
function createUpdateDocbyXSD(collection, tableName, xsdDict, getOperation)
{
	var xmlDoc = createDomDocument("<Update/>");	
	var tableNode = findTableFromXml(xmlDoc, tableName);
	var setNode = appendNode(tableNode, "SET");
	var whereNode = appendNode(tableNode, "WHERE");
	
	for (var i = 0; i < collection.length; i++)
	{
		var currObj = collection(i);
		
		if ((currObj.dataFld || currObj.id) && (currObj.dataSrc == tableName))
		{			
			var obj =  new Object();
			obj.dataFld = currObj.dataFld;
			obj.oper = null;
			obj.value = originalValue(currObj);

			if (obj.value.length > 0) //判断输入框是否空
			{
				if (getOperation)
					obj = getOperation(currObj, obj);
					
				if (getBoolParam(getAttribByDict(INFO_FILE, obj.dataFld, "isKey")))
					addConditionFields(tableNode, obj.dataFld, "=", obj.value);
				else
					appendNode(setNode, obj.dataFld, obj.value);
			}
			else//update时输入空，但新旧值不同时有效
			if ((currObj.oldValue) && (currObj.oldValue != obj.value))
			{
				if (getOperation)
					obj = getOperation(currObj, obj);

				appendNode(setNode, currObj.dataFld, obj.value);
			}
		}
	}
	return xmlDoc;
}

//Update的SET部分
function getUpdateFormCollection(xmlNode, collection)
{
	for (i = 0; i < collection.length; i++)
	{
		var currObj = collection(i);

		if ((isUndefined(typeof(currObj.dataFld)) == false) && (currObj.dataFld.length > 0) && (currObj.dataSrc == xmlNode.parentNode.nodeName) && (currObj.value != currObj.oldValue))
			appendNode(xmlNode, currObj.dataFld, originalValue(currObj));
	}
}

//根据界面得到查询的XML
//注意:这里的elemContainer要求是一个容器（Form或者是table等），这就有很大的资源浪费在判断td,tr等无用的资源上
function getConditionXml(elemContainer, callback)
{
	var xmlDoc = createDomDocument("<Conditions/>");
	var root = xmlDoc.documentElement;

	for (var i = 0; i < elemContainer.all.length; i++)
	{
		var curObj = elemContainer.all(i);

		var strOP = curObj.opMode;
		var strFuncName = curObj.funcName;
		var strType = curObj.dataType;
		var strTable = curObj.table;

		var strFld = "";

		if (curObj.tagName == "OGUInput")
			strFld = curObj.id;
		else
		{
			if (curObj.tagName != "OPTION")
				strFld = curObj.dataFld;
		}

		var strValue = "";

		if (curObj.tagName == "INPUT" || curObj.tagName == "SELECT" || curObj.tagName == "OGUInput")
			strValue = originalValue(curObj);
		else
			strValue = curObj.innerText;

		if (strFld && strFld.length > 0)
		{
			if (strValue && strValue.length > 0)
				appendConditionNode(root, curObj, strFld, strType, strValue, strOP, strFuncName, strTable, callback);
		}
	}

	return xmlDoc;
}

function getConditionXml2(elemCondition, callback)
{
	var xmlDoc = createDomDocument("<Conditions/>");
	var root = xmlDoc.documentElement;
	for (var i = 0; i < elemCondition.length; i++)
	{
		var curObj = elemCondition(i);
		
		var strOP = curObj.opMode;
		var strFuncName = curObj.funcName;
		var strType = curObj.dataType;
		var strTable = curObj.table;
		
		var strFld = "";
		if (curObj.tagName == "OGUInput")
			strFld = curObj.id;
		else
			if (curObj.tagName != "OPTION")
				strFld = curObj.dataFld;
		var strValue = "";
		if (curObj.tagName == "INPUT" || curObj.tagName == "SELECT" || curObj.tagName == "OGUInput")
			strValue = originalValue(curObj);
		else
			strValue = curObj.innerText;
		
		if ((strFld && strFld.length > 0) && (strValue && strValue.length > 0))
			appendConditionNode(root, curObj, strFld, strType, strValue, strOP, strFuncName, strTable, callback)
	}
	return xmlDoc;
}

function appendConditionNode(rootCondition, elem, strFld, strType, strValue, strOP, strFuncName, strTable, callback)
{
	var node = appendNode(rootCondition, strFld);

	if (strOP == "like")
	{
		node.setAttribute("originalValue", strValue);
		strValue = "%" + strValue + "%";
	}

	node.text = strValue;

	if (strType)
		node.setAttribute("type", strType);

	if (strOP)
		node.setAttribute("op", strOP);

	if (strFuncName)
		node.setAttribute("funcName", strFuncName);
	
	if (strTable)
		node.setAttribute("table", strTable);

	if (callback)
		callback(elem, node);
}

//搜查界面项的查询项
function searchSelectItem(tableName, xmlDict)
{
	var fieldName = new Array();
	var j = 0;
	
	for (i = 0; i < document.all.length; i++)
	{
		var currObj = document.all[i];
		
		if ((isUndefined(typeof(currObj.dataFld)) == false) && (currObj.dataFld.length > 0) && (currObj.dataSrc == tableName))
		{
			var fieldRoot = getXSDColumnsRoot(xmlDict);
			var nodeColumn = getXSDColumnFromRoot(fieldRoot, currObj.dataFld);
			
			if (nodeColumn != null)
			{
				fieldName[j] = currObj.id;
				j++;
			}
		}
	}
	
	return fieldName;
}

//向界面填充内容，第一个参数是查询内容(xml Document)根结点的子节点,第二个参数是数据集
function xmlResultFillForm(xmlNode, collection, xmlDict, getOperation)
{   
	var fieldRoot = getXSDColumnsRoot(xmlDict);
  
	for (var i = 0; i < xmlNode.childNodes.length; i++)
	{
		var nodeCurrent = xmlNode.childNodes(i);
		var nodeColumn = getXSDColumnFromRoot(fieldRoot, nodeCurrent.nodeName);
        
		for (var j = 0; j < collection.length; j++)
		{
			var currObj = collection[j];
           
			if ((isUndefined(typeof(currObj.dataFld)) == false) && (currObj.dataFld.length > 0) && 
				(currObj.dataFld == nodeCurrent.nodeName))
			{
				var tagName = currObj.tagName;
               
				if ((tagName != "TEXTAREA") && 
					(tagName != "LABEL") && 
					(tagName != "SPAN") && 
					(tagName != "DIV"))
				{ 
					if (nodeColumn != null)
					{
						var strType = getXSDColumnAttr(nodeColumn, "type");
						var strType = strType.split(":")[1];

						currObj.oldValue = nodeCurrent.text;
						currObj.value = currObj.oldValue;

						if (currObj.tagName != "SELECT" && isXSDIntType(strType))
						{
							currObj.oldValue = currObj.value;
							formatNumberObj(currObj, STD_AMOUNT_FORMAT);
						}
						else
						if (isXSDNumericType(strType))
						{
							currObj.oldValue = currObj.value;
							formatNumberObj(currObj, STD_NUMBER_FORMAT);
						}
						else
						if (isXSDDatetimeType(strType))
						{
							var dtFormat = getXSDColumnAttr(nodeColumn, "format");
							var bIncludeTime = dtFormat.toLowerCase() == "datetime";

							if (!bIncludeTime)
								currObj.oldValue = removeTime(nodeCurrent.text);
							else
								currObj.oldValue = nodeCurrent.text;

							currObj.value = currObj.oldValue;
						}
					}
					if (currObj.type == "checkbox")
					{
						if (nodeCurrent.text == "y")
							currObj.checked = true;
						else
							currObj.checked = false;
					}
				}
				else 
				{
					currObj.innerText = nodeCurrent.text;
					currObj.oldValue = currObj.innerText;
				}

				if (getOperation)
					getOperation(currObj, nodeCurrent, nodeColumn);
			}
		}
	}				
}

//向界面填充内容，第一个参数是查询内容(xml Document)根结点的子节点,第二个参数是数据集(仅仅做数据的填充，不设置oldValue等特殊属性)
function xmlDataFillForm(xmlNode, collection, xmlDict, getOperation)
{
	var fieldRoot = getXSDColumnsRoot(xmlDict);

	for (var i = 0; i < xmlNode.childNodes.length; i++)
	{
		var nodeCurrent = xmlNode.childNodes(i);
		var nodeColumn = getXSDColumnFromRoot(fieldRoot, nodeCurrent.nodeName);

		for (var j = 0; j < collection.length; j++)
		{
			var currObj = collection[j];

			if ((isUndefined(typeof(currObj.dataFld)) == false) && (currObj.dataFld.length > 0) && (currObj.dataFld == nodeCurrent.nodeName))
			{
				var tagName = currObj.tagName;

				if ((tagName != "TEXTAREA") && (tagName != "LABEL") && (tagName != "SPAN") && (tagName != "DIV"))
				{
					if (nodeColumn != null)
					{
						var strType = getXSDColumnAttr(nodeColumn, "type");
						var strType = strType.split(":")[1];

						currObj.value = nodeCurrent.text;

						if (isXSDIntType(strType))
							formatNumberObj(currObj, STD_AMOUNT_FORMAT);
						else
							if (isXSDNumericType(strType))
								formatNumberObj(currObj, STD_NUMBER_FORMAT);
							else
								if (isXSDDatetimeType(strType))
								{
									var dtFormat = getXSDColumnAttr(nodeColumn, "format");
									var bIncludeTime = dtFormat.toLowerCase() == "datetime";

									if (!bIncludeTime)
										currObj.value = removeTime(nodeCurrent.text);
									else
										currObj.value = nodeCurrent.text;
								}
					}
					if (currObj.type == "checkbox")
					{
						if (nodeCurrent.text == "y")
							currObj.checked = true;
						else
							currObj.checked = false;
					}
				}
				else 
					currObj.innerText = nodeCurrent.text;

				if (getOperation)
					getOperation(currObj, nodeCurrent, nodeColumn);
			}
		}
	}				
}

//将界面和数据字典相关元素的oldValue属性置为value的值
function initOldValueToValue(collection, getOperation)
{
	for (var j = 0; j < collection.length; j++)
	{
		var currObj = collection(j);

		if ((isUndefined(typeof(currObj.dataFld)) == false) && (currObj.dataFld.length > 0))
		{
			var strFld = currObj.dataFld;

			if (!strFld)
				strFld = currObj.id;

			var obj = new Object();

			obj.dataFld = strFld;
			obj.oper = null;
			obj.value = originalValue(currObj);

			if ((currObj.tagName != "TEXTAREA") && 
				(currObj.tagName != "LABEL") && 
				(currObj.tagName != "SPAN") && 
				(currObj.tagName != "DIV"))
			{
				if (currObj.type == "checkbox")
				{
					if (currObj.checked)
						currObj.oldValue = "y";
					else
						currObj.oldValue = "n";
				}
				else
				{
					if (getOperation)
						obj = getOperation(currObj, obj);

					currObj.oldValue = obj.value;
				}
			}
			else 
			{
				obj.value = currObj.innerText;

				if (getOperation)
					obj = getOperation(currObj, obj);

				currObj.oldValue = obj.value;
			}
		}
	}			
}

//界面上复选框click事件
function onTypeCheckClick(currObj)
{
	if(currObj.checked)
		currObj.value = "Y";
	else
		currObj.value = "N";	
}

//清空界面上的项,此函数已经废弃请调用emptyFormItem
function EmptyFormItem(collection, getNoEmptyItem)
{
	emptyFormItem(collection, getNoEmptyItem);
}

//清空界面上的项
function emptyFormItem(collection, getNoEmptyItem)
{
	var obj = new Object();
	
	for (i = 0; i < collection.length; i++)
	{
		var currObj = collection(i);
		
		obj.isEmpty = true;
		
		if (arguments.length == 2)
			obj = getNoEmptyItem(currObj, obj);
		
		if (obj.isEmpty == true)
		{
			if (currObj.tagName == "SELECT")
				currObj.selectedIndex = 0;
				
			if (currObj.tagName == "TEXTAREA")
				currObj.innerText = "";
				
			if (currObj.tagName == "INPUT")
			{
				if (currObj.type == "checkbox")
					currObj.checked = false;
					
				if (currObj.type == "text")
					currObj.value = "";
			}
			
			currObj.oldValue = "";
		}
	}
}

//设置对象的disabled属性,参数item可以是一个对象也可是对象数组
function disabledItem(item, strBoolean, itemType)
{
	if (paramIsArray(item) == true)
	{
		for (i = 0; i < item.length; i++)
		{
			var currItem = item[i];
			
			if (arguments.length == 3 && currItem.type == itemType)
				currItem.disabled = strBoolean;		
			else
				currItem.disabled = strBoolean;			
		}
	}
	else
	{	
		if (arguments.length == 3 && currItem.type == itemType)
			item.disabled = strBoolean;	
		else
			item.disabled = strBoolean;
	}		
}

//创建一个Delete的XML文档
function createDelDoc(collection, tableName)
{
	var xmlDoc = createDomDocument("<Delete/>");
	var tableNode = findTableFromXml(xmlDoc, tableName);
	
	if (arguments.length > 3)
		addConditionFields(tableNode, arguments[2], arguments[3], arguments[4])
	else
		if (arguments.length == 4)
			addConditionFields(tableNode, collection, arguments[2]);
		else
			addConditionFields(tableNode, collection);

	return xmlDoc;
}

function createDelXSDDoc(collection, tableName, callback)
{
	var xmlDoc = createDomDocument("<Delete/>");
	var tableNode = findTableFromXml(xmlDoc, tableName);
	var whereNode = appendNode(tableNode, "WHERE");
	for (var i = 0; i < collection.length; i++)
	{
		var curObj = collection(i);	
		if (getBoolParam(getAttribByDict(INFO_FILE, curObj.dataFld, "isKey")))
		{
			if (callback)
				calback(curObj);
			appendNode(whereNode, curObj.dataFld, originalValue(curObj));
		}
	}
	
	return xmlDoc;
}

//disable窗体上的输入项
function setItemDisabled(collection, tableName, strBoolean)
{
	for (i = 0; i < collection.length; i++)
	{
		var currObj = collection[i];
		if ((isUndefined(typeof(currObj.dataFld)) == false) && (currObj.dataFld.length > 0) && (currObj.dataSrc == tableName))
		{	
			if (currObj.tagName == "TEXTAREA")
				currObj.readOnly = strBoolean;
			else
				currObj.disabled = strBoolean;
		}
	}
}

//向数据字典中动态的增加数据
function addXmlNodeIntoDic(xmlDoc, sName, sType, sCaption)
{
	var xmlNode = xmlDoc.selectSingleNode(".//xsd:element[@name=\"" + sName + "\"]");
	
	if (xmlNode != null) 
		return;
	xmlNode = xmlDoc.selectSingleNode(".//xsd:sequence");
	
	var xmlTempNode = xmlDoc.createNode(1,"xsd:element", "http://www.w3.org/2001/XMLSchema");
	xmlNode.appendChild(xmlTempNode);
	
	appendAttr(xmlTempNode, "name", sName);
	appendAttr(xmlTempNode, "type", sType);
	appendAttr(xmlTempNode, "caption", sCaption);
}


//向数据集中动态添加数据
function addXmlNodeIntoDSXml(xmlNode, sName, sValue)
{	
	while (xmlNode != null)
	{
		if (xmlNode.selectSingleNode(sName) == null)
			appendNode(xmlNode, sName, sValue);
			
		xmlNode = xmlNode.nextSibling;
	}
	
}
//清空一个Select控件
function clearSelectControl(objSelect)
{
	while (objSelect.options.length > 0)
	{
		objSelect.options.remove(0);
	}
}

//为Select增加一个空项
function addNullOption(objSelect)
{
	var newOpt = document.createElement("OPTION");

	newOpt.value = "";
	newOpt.text = "-";
	objSelect.options.add(newOpt);
}

/*************************************
根据一个记录集的XML填充一个Select的值
xmlElement:		XML数据岛ID
objSelect:		Select控件的ID
strKeyName:		关键字段的名称
strValueName:	值字段的名称
bAddNull:		是否增加一个空项
**************************************/
function setSelectValuesByXML(xmlElement, objSelect, strKeyName, strValueName, bAddNull, callback)
{
	if (bAddNull)
		addNullOption(objSelect);

	var rs = xmlElement.recordset;

	rs.movefirst();

	while (!rs.eof)
	{
		var newOpt = document.createElement("OPTION");

		newOpt.value = rs.Fields(strKeyName).value;
		newOpt.text = rs.Fields(strValueName).value;

		var bAdd = true;

		if (callback)
			bAdd = callback(rs, newOpt);

		if (bAdd != false)
			objSelect.options.add(newOpt);

		rs.movenext();
	}
}

/*************************************
根据一个记录集的XML查找指定的值
xmlElement:		XML数据岛ID
strKeyName:		关键字段的名称
strValueName:	值字段的名称
**************************************/
function getValueFromDataSet(xmlElement, strKeyName, strKeyValue, strValueName)
{
	var strResult = "";
	var rs = xmlElement.recordset;

	rs.movefirst();

	while(!rs.eof)
	{
		if (rs.Fields(strKeyName).value == strKeyValue)
		{
			strResult = rs.Fields(strValueName).value;
			break;
		}
		rs.movenext();
	}

	return strResult;
}

/*************************************
根据一个记录集的XML填充一个Select的值
xmlTable:		XML中第一个Table节点
objSelect:		Select控件的ID
strKeyName:		关键字段的名称
strValueName:	值字段的名称
bAddNull:		是否增加一个空项
**************************************/
function setSelectValuesByTable(xmlTable, objSelect, strKeyName, strValueName, bAddNull, callBack)
{
	clearSelectControl(objSelect);

	if (bAddNull)
		addNullOption(objSelect);

	var nodeRow = xmlTable;
	var bIgnor = false;

	while (nodeRow != null)
	{
		var newOpt = document.createElement("OPTION");
		newOpt.value = nodeRow.selectSingleNode(strKeyName).text;
		newOpt.text = nodeRow.selectSingleNode(strValueName).text;

		if (callBack)
			bIgnor = callBack(newOpt, nodeRow);

		if (!bIgnor)
			objSelect.options.add(newOpt);

		nodeRow = nodeRow.nextSibling;
	}
}

//从查询的xml中解析出Key值,放入数组里
function getKeyByXmlIntoArray(xmlDoc)
{
	var xmlNode = xmlDoc.selectSingleNode(".//KEYVALUE");
	
	var sKeyArray = new Array();
	
	if ((xmlNode) && (xmlNode.text != ""))
	{
		sKeyArray = xmlNode.text.split(";");
	}
	return sKeyArray;
}

//丛数组中取出一段数据
function getValueByArray(sValueArray, iNo, iCount)
{
	var sValue = "";
	var iBeginNo = iNo * iCount;
	var iEndNo = (iNo + 1) * iCount;
	
	for (var i = iBeginNo; i < iEndNo; i++)
	{	
		if (i < sValueArray.length)
		{
			if (i == iBeginNo)
				sValue += sValueArray[i];
			else
				sValue += ";" + sValueArray[i];
		}
		else
			break;
	}
	return sValue;
}

//数据修改成功后，修改相应的grid和xml中的数据
function editXmlGridFromUpdate(myColumn, xmlUpdateDoc)
{
	var setNode = xmlUpdateDoc.selectSingleNode(".//SET");
	
	var fieldNode = setNode.firstChild;
	
	var TD;
	while (fieldNode != null)
	{
		TD = getRelativeTD(myColumn, fieldNode.nodeName);
		
		if (TD != null)
		{
			 if (TD.firstChild.tagName == "A")
				TD.firstChild.innerText = fieldNode.text;
			else
				TD.innerText = fieldNode.text;
		}
		var xmlNode = myColumn.xmlNode.selectSingleNode(fieldNode.nodeName);
		if (xmlNode != null)
			xmlNode.text = fieldNode.text;
		
		fieldNode = fieldNode.nextSibling;
	}
}
//取得小于当前值的最小的整数
function getMinInt(num)
{
	if (parseInt(num) == num)
		return num -1;
	else
		return parseInt(num);
	
}

//设置对象可见或不可见
function visibilityItem(item, strBoolean)
{
	if (paramIsArray(item))
	{
		for (i = 0; i < item.length; i++)
		{
			var currItem = item[i];		
				
			if (strBoolean == true)	
				currItem.style.visibility = "visible";
			else
				currItem.style.visibility = "hidden";
		}
	}
	else
	{
		if (strBoolean == true)	
			item.style.visibility = "visible";
		else
			item.style.visibility = "hidden";
	}				
	
}

function getMaxInt(num)
{
	if (parseInt(num) == num)
		return num;
	else
		return Math.ceil(num);
}
//-->