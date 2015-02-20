<!--
function getXmlParam(xml)
{
	var xmlDoc = createDomDocument();

	if (typeof(xml) == "string")
		xmlDoc.loadXML(xml);
	else
	if (typeof(xml) == "object")
		xmlDoc.loadXML(xml.xml);

	return xmlDoc;
}

function getBoolParam(bValue)
{
	return deserializeBool(bValue);
}

function getIntParam(value)
{
	return deserializeNumber(value);
}

function serializeBool(bValue)
{
	return bValue ? "true" : "false";
}

function serializeNumber(nNum)
{
	return nNum.toString();
}

function deserializeBool(bValue)
{
	var bResult = false;

	switch (typeof(bValue))
	{
		case "string":	{
							var nValue = parseInt(bValue);

							if (isNaN(nValue))
								bResult = (bValue.toLowerCase() == "true");
							else
								bResult = (bValue != 0);
						}
						break;
		case "boolean":	bResult = bValue;
						break;
		case "number":	bResult = (bValue != 0);
						break;
	}

	return bResult;
}

function deserializeNumber(value)
{
	var nValue = 0;

	switch (typeof(value))
	{
		case "string":	nValue = parseInt(value);
						break;
		case "boolean":	if (value)
							nValue = -1;
						else
							nValue = 0;
						break;
		case "number":	nValue = value;
						break;
	}

	return nValue;
}
//-->