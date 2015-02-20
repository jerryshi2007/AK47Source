<!--
	function onOKClick()
	{
		try
		{
			window.returnValue = dnList.options[dnList.selectedIndex].srcNode;
			window.close();
		}
		catch (e)
		{
			showError(e);
		}
	}

	function onCancelClick()
	{
		try
		{
			window.returnValue = null;
			window.close();
		}
		catch (e)
		{
			showError(e);
		}
	}

	function initOGUSelect(xmlDoc)
	{
		var xmlNode = xmlDoc.documentElement.firstChild;
		var bFirst = true;

		while (xmlNode)
		{
			var newOpt = document.createElement("OPTION");

			newOpt.value = xmlNode.getAttribute("ALL_PATH_NAME");
			newOpt.srcNode = xmlNode;
			newOpt.text = xmlNode.getAttribute("DISPLAY_NAME") + "(" + newOpt.value + ")";

			if (bFirst)
			{
				bFirst = false;
				newOpt.selected = true;
			}

			dnList.options.add(newOpt);

			xmlNode = xmlNode.nextSibling;
		}
	}

	function onDocumentLoad()
	{
		try
		{
			window.returnValue = null;
			initDocumentEvents();

			if (window.dialogArguments)
				initOGUSelect(window.dialogArguments);
			//else
			//{
			//	var strSearchDoc = window.clipboardData.getData("Text");
			//	if (strSearchDoc != null && strSearchDoc.length > 0)
			//	{
			//		window.clipboardData.clearData("Text");
			//		try
			//		{
			//			var xmlSearchDoc = createDomDocument(strSearchDoc);
			//			initOGUSelect(xmlSearchDoc);
			//		}
			//		catch{}//对于该异常数据，只能产生于非正常处理数据
			//	}
			//}
		}
		catch (e)
		{
			showError(e);
		}
	}
//-->
