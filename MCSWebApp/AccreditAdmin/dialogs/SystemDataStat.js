<!--

function onDocumentLoad()
{
	try
	{
	
	}
	catch (e)
	{
		showError(e);
	}
}

function onPrintClick()
{
	var oExcel, oExcelWorkBooks;
	try
	{
		oExcel = new ActiveXObject("Excel.Application");
		oExcel.Visible = true;
		
		oExcelWorkBooks = oExcel.workBooks.Add(getHttpRootDir(document.URL) + "//Model//OGUExportModel.xlt");
//		oExcel.Run("OGUExport.StartOGUDataExport", xmlResultClient.XMLDocument, xmlResultClientParam.XMLDocument);//StartOGUDataExportWithStr
		oExcel.Run("OGUExport.StartOGUDataExportWithStr", xmlResultClient.XMLDocument.xml, xmlResultClientParam.XMLDocument.xml);//StartOGUDataExportWithStr
		
		alert("数据倒换成功！");
	}
	catch (e)
	{
		showError(e);
	}
	finally
	{		
		oExcelWorkBooks = null;
		oExcel = null;		
	}
}

function getHttpRootDir(strURL)
{
	var iLastPos = strURL.lastIndexOf("/");
	var strTemp = strURL.substring(0, iLastPos - 2);
	iLastPos = strTemp.lastIndexOf("/");
	strTemp = strTemp.substring(0, iLastPos);
	
	return strTemp;
}

function showChildren(oTable)
{
	try
	{
		var obj = event.srcElement.parentElement.parentElement.nextSibling;

		if (obj.style.display != "none")
			obj.style.display = "none";
		else
			obj.style.display = "inline";
		
		event.srcElement.innerHTML = obj.style.display == "none" ? "+" : "-";
	}
	catch (e)
	{
		showError(e);
	}
}

function onWordSpanClick()
{
	try
	{
		var app = createOfficeObject("Word.Application");
		var doc = app.documents.add("Normal");

		try
		{
			doc.PageSetup.Orientation = 1;	//wdOrientLandscape
		}
		catch(e)
		{
		}

		copyTableToClipboard();

		doc.activeWindow.selection.paste();
		
		alert("数据倒换成功！");
	}
	catch(e)
	{
		showError(e);
	}
}

function onExcelSpanClick()
{
	try
	{
		var app = createOfficeObject("Excel.Application");
		var workbook = app.workbooks.Add();

		try
		{
			workbook.ActiveSheet.PageSetup.Orientation = 2;	//xlLandscape
		}
		catch(e)
		{
		}

		copyTableToClipboard();

		workbook.activeSheet.paste();
		workbook.activeSheet.Cells
		
		alert("数据倒换成功！");
	}
	catch(e)
	{
		showError(e);
	}
}

//建立office对象
function createOfficeObject(strProgID, bVisible)
{
	var app = createObject(strProgID, "Microsoft Office");

	if (bVisible == null || typeof(bVisible) == "undefined")
		bVisible = true;

	app.visible = bVisible;

	return app;
}

function copyTableToClipboard()
{
	if (allDataTable)
	{
		var selection = document.selection;
		selection.empty();

		var r = document.body.createControlRange();
		r.add(allDataTable);
		r.select();

		r.execCommand("Copy");
		selection.empty();
	}
}



//-->
