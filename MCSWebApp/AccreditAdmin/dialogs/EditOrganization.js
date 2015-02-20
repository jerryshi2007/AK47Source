<!--
function onDocumentLoad()
{
	try
	{
		window.returnValue = "";
		getExtraDisplayShow(AConfig.XMLDocument, orgContentTable, "Display", "ORGANIZATIONS");
		
		initDocumentEvents(frmInput);
		initElementsByDict(ORGANIZATION_XSD.XMLDocument);
		
		var arg = window.dialogArguments;
		if (arg.op == "Update")
		{
			var xmlDoc = createDomDocument(frmInput.organizationData.value);
			xmlResultFillForm(xmlDoc.documentElement.firstChild, frmInput.elements, ORGANIZATION_XSD.XMLDocument);
		}
		else
		{
			frmInput.CREATE_TIME.value = dateToStr(new Date(), DATEPART.DATE);
		}

		bindCalendarToInput(hCalendar, frmInput.CREATE_TIME);
		
		if (frmInput.opPermission.value == "false")
			btnOK.disabled = true;
	}
	catch(e)
	{
		showError(e);
		window.close();
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
					"对不起，“机构”的名称中不允许包含字符“\\”！");

		var arg = window.dialogArguments;
		
		frmInput.searchName.value = frmInput.DISPLAY_NAME.value + ' ' + frmInput.OBJ_NAME.value;
		
		var xmlDoc = null;

		if (arg.op == "Insert")
			xmlDoc = createInsertDoc(frmInput.elements, C_ORGANIZATIONS);
		else
			xmlDoc = createUpdateDoc(frmInput.elements, C_ORGANIZATIONS, "GUID", "=", arg.guid);
			
		var nodeSet = xmlDoc.selectSingleNode(".//SET");
		

		if (nodeSet && nodeSet.hasChildNodes())
		{
			var bContinue = true;

			if (arg.op == "Update" && nodeSet.selectSingleNode("OBJ_NAME") != null)
				bContinue = window.confirm("修改“机构名称”可能会影响到此机构在其他系统中的正常使用，您要继续吗？");

			if (bContinue)
			{
				if (arg.op == "Insert")
					appendNode(nodeSet, "PARENT_GUID", arg.parentGuid);

				var xmlResult = xmlSend("../sysAdmin/OGUEditer.aspx", xmlDoc);
				
				checkErrorResult(xmlResult);
				
				if (nodeSet.selectSingleNode("RANK_CODE") != null)
					xmlResult.documentElement.firstChild.setAttribute("NAME", frmInput.RANK_CODE.options[frmInput.RANK_CODE.selectedIndex].text);

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