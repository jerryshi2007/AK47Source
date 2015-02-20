<!--
//Get attribute from a column node
function getXSDColumnAttr(nodeColumn, strAttr)
{
	try
	{
		return nodeColumn.attributes.getNamedItem(strAttr).value;
	}
	catch(e)
	{
		return "";
	}
}

//Get a column attribute from columns root node
function getXSDColumnFromRoot(nodeRoot, strColumnName)
{
	var nodeColumn = nodeRoot.selectSingleNode("./xsd:element[@name = \"" + strColumnName + "\"]");

	return nodeColumn;
}

//Get a column attribute from columns root node
function getXSDAttrFromRoot(nodeRoot, strColumnName, strAttr)
{
	var nodeColumn = getXSDColumnFromRoot(nodeRoot, strColumnName);

	return getXSDColumnAttr(nodeColumn, strAttr);
}

//Get the columns root node
function getXSDColumnsRoot(xmlDoc)
{
	return xmlDoc.selectSingleNode(".//xsd:sequence");
}

//Get the column node
function getXSDColumnNode(xmlDoc, strColumnName)
{
	return xmlDoc.selectSingleNode(".//xsd:sequence/xsd:element[@name = \"" + strColumnName + "\"]");
}

//-->