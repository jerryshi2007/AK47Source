<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="testExtOuUserInputControl.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.UserSelector.testExtOuUserInputControl" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script type="text/javascript">

        function GetResults(resultCtrl, ctrlid) {
            var consignUsers = $find(ctrlid).get_consignUsers(); //会签
            var circulators = $find(ctrlid).get_circulators(); //传阅
            displaySelectedObjects(resultCtrl, consignUsers, circulators);
        }

        function displaySelectedObjects(resultCtrl, consignUsers, circulators) {
            //debugger;
            //userInfo :会签
            //circulateUserInfo :传阅
            if (consignUsers) {
                if (consignUsers.length > 0) {
                    for (var i = 0; i < consignUsers.length; i++) {
                        addMessage(resultCtrl, "会签:" + consignUsers[i].fullPath);
                    }
                }
            }
            if (circulators) {
                if (circulators.length > 0) {
                    for (var i = 0; i < circulators.length; i++) {
                        addMessage(resultCtrl, "传阅:" + circulators[i].fullPath);
                    }
                }
            }
        }

        function addMessage(resultCtrl, msg) {
            document.getElementById(resultCtrl).innerHTML += "<p style='margin:0'>" + msg + "</p>";
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <cc1:ExtOuUserInputControl runat="server" ID="ExtOuUserInputControl1" MultiSelect="false" />
    </div>
    <div>
        <input id="Button1" type="button" value="GetResults.." onclick="GetResults('result','ExtOuUserInputControl1')" />
    </div>
    <br />
    <div id="resultContainer">
        <div id="result" contenteditable="true" style="overflow: auto; border: 1px silver solid;
            width: 100%; height: 200px" runat="server">
        </div>
    </div>
    <br />
    <div>
        <cc1:ExtOuUserInputControl runat="server" ID="ExtOuUserInputControl2" MultiSelect="true" />
    </div>
    <div>
        <input id="Button2" type="button" value="GetResults.." onclick="GetResults('result2','ExtOuUserInputControl2')" />
    </div>
    <br />
    <div id="resultContainer2">
        <div id="result2" contenteditable="true" style="overflow: auto; border: 1px silver solid;
            width: 100%; height: 200px" runat="server">
        </div>
    </div>
    <br />
    <asp:Button ID="Button3" runat="server" Text="postback" />
    </form>
</body>
</html>
