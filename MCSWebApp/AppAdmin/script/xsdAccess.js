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
	if (nodeColumn == null)
		nodeColumn = nodeRoot.selectSingleNode("./xs:element[@name = \"" + strColumnName + "\"]");
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
	var resultNode = xmlDoc.selectSingleNode(".//xsd:sequence");
	if (resultNode == null)
		resultNode = xmlDoc.selectSingleNode(".//xs:sequence");
	return resultNode;
}

//Get the column node
function getXSDColumnNode(xmlDoc, strColumnName)
{
	var resultNode = xmlDoc.selectSingleNode(".//xsd:sequence/xsd:element[@name = \"" + strColumnName + "\"]");
	if (resultNode == null)
		resultNode = xmlDoc.selectSingleNode(".//xs:sequence/xs:element[@name = \"" + strColumnName + "\"]");
	return resultNode;
}

//-->