<!--

function onDocumentLoad()
{
	try
	{
		window.returnValue = "";
		initDocumentEvents(frmInput);
		prepareSelOptions();
	}
	catch (e)
	{
		showError(e);
	}
}

function prepareSelOptions()
{
	var xmlDoc = xmlOption.XMLDocument;
	
	with (frmInput)
	{
		var root = xmlDoc.documentElement.firstChild;
		while (root != null)
		{
			var obj = document.createElement("OPTION");
			obj.value = root.selectSingleNode("Value").text;
			obj.title = obj.text = root.selectSingleNode("Text").text;
			
			selOriginal.options.add(obj);
			
			root = root.nextSibling;
		}
	}
}

function onOKCick()
{
	try
	{
		with(frmInput)
		{
			trueThrow(rootOrganizationGuid.value == "", "对不起，参数：系统没有设置好！");
			falseThrow(ExportOrganization.checked || ExportGroup.checked || ExportUser.checked, "对不起，请选择您要导出的数据类型！");
			falseThrow(selSelected.options.length > 0, "对不起，请选择您需要的数据字段！");
			
			var strSelected = "";
			for (var i = 0; i < selSelected.options.length; i++)
			{
				if (strSelected.length > 0)
					strSelected += ",";
					
				strSelected += selSelected.options[i].value;
			}
			dataColumns.value = strSelected;
		}
		frmInput.submit();
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
		window.close();
	}
	catch (e)
	{
		showError(e);
	}
}

function onSelectMoveAll(selSource, selTarget)
{
	try
	{
		while (selSource.options.length != 0)
			moveOneOption(selSource, 0, selTarget);
	}
	catch (e)
	{
		showError(e);
	}
}

function onSelectMoveMulit(selSource, selTarget, bMulitSelected)
{
	try
	{
		var iPos = 0;
		while (iPos < selSource.options.length)
		{
			if (selSource.options[iPos].selected)
				moveOneOption(selSource, iPos, selTarget);
			else
				iPos++;
		}
	}
	catch (e)
	{
		showError(e);
	}	
}


function moveOneOption(selSource, iPosition, selTarget)
{
	var oldObj = selSource.options[iPosition];
			
	var newObj = document.createElement("OPTION");
	newObj.value = oldObj.value;
	newObj.text = oldObj.text;
	newObj.title = oldObj.title;
	
	selSource.options.remove(iPosition);
	selTarget.options.add(newObj);
}



//-->
