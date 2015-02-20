function addNamespace(strName)
{
	var doc = document;
	var bFound = false;

	for(var i = 0; i < doc.namespaces.length; i++)
	{
		if (doc.namespaces.item(i).name.toLowerCase() == strName.toLowerCase())
		{
			bFound = true;
			break;
		}
	}

	if (bFound == false)
		doc.namespaces.add(strName);
}