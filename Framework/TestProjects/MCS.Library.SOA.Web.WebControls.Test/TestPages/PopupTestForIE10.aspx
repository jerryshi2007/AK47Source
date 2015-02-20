<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PopupTestForIE10.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.TestPages.PopupTestForIE10" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="javascript" type="text/javascript">
// <![CDATA[

        function btnPop_onclick() {
            var pop = _createNewPopup(window);
            pop.show(0, 18, 100, 100,document.getElementById("inputText"));

        }

        function _createNewPopup(pWin) {
            var popup = pWin.createPopup();
            var doc = popup.document;

            doc.writeln("<html><head>");
            //Sys.UI.DomEvent.writePopupWindowEventDelegate(doc.parentWindow);
            doc.writeln("</head><body onselect=\"return false\"><select><option>111</option><option>112</option></select></body></html>");
            doc.body.style.border = "none";
            doc.body.style.margin = "0px";
            doc.body.scroll = "no";
            //doc.body.style.filter = "progid:DXImageTransform.Microsoft.Fade(duration=0.1,overlap=0.75)";

            return popup;
        }

// ]]>
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <input id="inputText" type="text" />
        <input id="btnPop" type="button" value="弹..." onclick="return btnPop_onclick()" /></div>
    </form>
</body>
</html>
