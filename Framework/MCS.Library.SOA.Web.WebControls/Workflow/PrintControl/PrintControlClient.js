function ToggleToobar(on) {
	var toolbar = document.getElementById('top');
	var bottom = document.getElementById('bottom');
	var doc = document.getElementById('outer');

	if (on) {
		toolbar.style.display = 'block';
		bottom.style.display = 'block';
		doc.style.top = '';
		doc.style.bottom = '';
	} else {
		toolbar.style.display = 'none';
		bottom.style.display = 'none';
		doc.style.top = '0';
		doc.style.bottom = '0';
	}
}

function displayHTML(printContent) {
	var inf = printContent;
	win = window.open("print.htm", 'popup', 'toolbar = no, status = no, scrollbars = yes, resizable = yes');
	win.document.write('<html>' +
        '<link href="/MCSWebApp/WebTestProject/CSS/Style.css" rel="stylesheet" type="text/css" />' +
        '<link href="/MCSWebApp/Css/basePage.css" type="text/css" rel="stylesheet" />' +
        '<link href="/MCSWebApp/Css/baseForm.css" type="text/css" rel="stylesheet" />' +
        '<style type="text/css">.commonTable { background-color:Silver; text-align:left; width:100%; }</' + 'style>' +
        '<body>' + inf + '<script type="text/javascript">function OnAfterPrint(){top.close();}window.onafterprint=OnAfterPrint;window.print();</' + 'script></' + 'body></' + 'html>');
	win.document.close(); // new line
}

function displayMonthAdHTML(printContent) {
	//var inf = printContent.toString().replace(/2500px/g, '930px;font-size:10px;page-break-before:always;margin:20px 0px');
	var inf = printContent.toString().replace(/OVERFLOW-X: auto;/, '').replace(/OVERFLOW-Y: auto;/, '').replace(/HEIGHT: 390px;/, '');
	win = window.open("print.htm", 'popup', 'toolbar = no, status = no, scrollbars = yes, resizable = yes');
	win.document.write('<html>' +
        '<link href="/MCSWebApp/WebTestProject/CSS/Style.css" rel="stylesheet" type="text/css" />' +
        '<link href="/MCSWebApp/Css/basePage.css" type="text/css" rel="stylesheet" />' +
        '<link href="/MCSWebApp/Css/baseForm.css" type="text/css" rel="stylesheet" />' +
        '<style type="text/css">.commonTable1 td, .commonTable1 th{border-color: #C3C3C3;font-size:14pt} </' + 'style>' +
        '<body>' + inf + '<script type="text/javascript">function OnAfterPrint(){top.close();}window.onafterprint=OnAfterPrint;alert("注意：本页面含有较宽表格，请选择横向纸张打印，并且尽量减小页边距。");window.print();</' + 'script></' + 'body></' + 'html>');
	win.document.close(); // new line
}

function displayMonthAdHTMLV1_0(printContent) {
	//var inf = printContent.toString().replace(/2500px/g, '930px;font-size:10px;page-break-before:always;margin:20px 0px');
	var inf = printContent.toString().replace(/2500px/g, '930px;font-size:10px;page-break-before:always;margin:20px 0px');
	win = window.open("print.htm", 'popup', 'toolbar = no, status = no, scrollbars = yes, resizable = yes');
	win.document.write('<html>' +
        '<link href="/MCSWebApp/WebTestProject/CSS/Style.css" rel="stylesheet" type="text/css" />' +
        '<link href="/MCSWebApp/Css/basePage.css" type="text/css" rel="stylesheet" />' +
        '<link href="/MCSWebApp/Css/baseForm.css" type="text/css" rel="stylesheet" />' +
        '<style type="text/css">.commonTable { background-color:Silver; text-align:left; width:100%; } th { font-size:10px; border:1px solid black; page-break-inside:avoid; } </' + 'style>' +
        '<body>' + inf + '<script type="text/javascript">function OnAfterPrint(){top.close();}window.onafterprint=OnAfterPrint;alert("注意：本页面含有较宽表格，请选择横向纸张打印，并且尽量减小页边距。");window.print();</' + 'script></' + 'body></' + 'html>');
	win.document.close(); // new line
}

function OnBeforePrint() {
	ToggleToobar(false);
	window.onafterprint = OnAfterPrint;
}

function OnAfterPrint() {
	ToggleToobar(true);
}

function OnPrint(containWideTable) {
	var printAreaID = "printArea";

	if (document.getElementById("printArea") == null)
		printAreaID = "outer";

	if (containWideTable == '0') {
		displayHTML(document.getElementById(printAreaID).innerHTML);
	} else {
		if (containWideTable == 'V1_0') {
			displayMonthAdHTMLV1_0(document.getElementById(printAreaID).innerHTML);
		} else {
			if (containWideTable == 'V1_1') {
				OnMonthADPrint();
				displayMonthAdHTML(document.getElementById(printAreaID).innerHTML);
			}
		}
	}
}

function OnMonthADPrint() {
	try {
		var editorInnerHtml = document.getElementById('ctrlAdArrangeHtml_Editor').innerHTML;
		var hidValue = document.getElementById('ctrlAdArrangeHtml_HiddenData');
		hidValue.value = '';
		var tbchange = document.getElementById('divParent');
		tbchange.innerHTML = editorInnerHtml;
	}
	catch (err) {
	}
}

window.onbeforeprint = OnBeforePrint;
