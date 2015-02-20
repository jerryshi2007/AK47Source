<!--
function onDocumentLoad()
{
	try
	{
		window.returnValue = "";
		
		getExtraDisplayShow(AConfig.XMLDocument, groupContentTable, "Display", "GROUPS");
		
		initDocumentEvents(frmInput);
		initElementsByDict(GROUP_XSD.XMLDocument);
		
		var arg = window.dialogArguments;
		if (arg.op == "Update")
		{
			var xmlDoc = createDomDocument(frmInput.GroupData.value);
			xmlResultFillForm(xmlDoc.documentElement.firstChild, frmInput.elements, GROUP_XSD.XMLDocument);
		}
		else
			frmInput.CREATE_TIME.value = dateToStr(new Date(), DATEPART.DATE);

		bindCalendarToInput(hCalendar, frmInput.CREATE_TIME);
		
		if (frmInput.opPermission.value == "false")
			btnOK.disabled = true;
	}
	catch(e)
	{
		showError(e);
	}
}

function cancelWindow()
{
	window.close();
}

function changeObjName()
{
	try
	{
		frmInput.ALL_PATH_NAME.value = frmInput.parentAllPathName.value + "\\" + frmInput.OBJ_NAME.value;
		if (frmInput.DISPLAY_NAME.value == "")
			frmInput.DISPLAY_NAME.value = frmInput.OBJ_NAME.value;
	}
	catch (e)
	{
		showError(e);
	}
}

function onSaveData()
{
	try
	{
		changeObjName();
		checkInputNull();
		
		trueThrow(	frmInput.OBJ_NAME.value.indexOf("\\") >= 0 || 
					frmInput.DISPLAY_NAME.value.indexOf("\\") >= 0, 
					"对不起，“人员组”的名称中不允许包含字符“\\”！");
		
		frmInput.searchName.value = frmInput.DISPLAY_NAME.value + ' ' + frmInput.OBJ_NAME.value;

		var arg = window.dialogArguments;

		var xmlDoc = null;

		if (arg.op == "Insert")
			xmlDoc = createInsertDoc(frmInput.elements, "GROUPS");
		else
			xmlDoc = createUpdateDoc(frmInput.elements, "GROUPS", "GUID", "=", arg.guid);
			
		var nodeSet = xmlDoc.selectSingleNode(".//SET");

		if (nodeSet && nodeSet.hasChildNodes())
		{
			var bContinue = true;

			if (arg.op == "Update" && nodeSet.selectSingleNode("OBJ_NAME") != null)
				bContinue = window.confirm("修改机构名称可能会影响到对此机构的应用授权信息，您要继续吗？");

			if (bContinue)
			{
				if (arg.op == "Insert")
					appendNode(nodeSet, "PARENT_GUID", arg.parentGuid);
					
				var xmlResult = xmlSend("../sysAdmin/OGUEditer.aspx", xmlDoc);
				
				checkErrorResult(xmlResult);

				window.returnValue = xmlResult.xml;
				window.close();
			}
		}
		else
		{
			window.close();
		}
	}
	catch (e)
	{
		showError(e);
	}
}
//-->