<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="testExtConsignUserSelector.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.UserSelector.testExtConsignUserSelector" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script type="text/javascript">

        function showDialog(ctrlid) {
            var result = $find(ctrlid).showDialog();
            displaySelectedObjects(result);
        }

        function displaySelectedObjects(result) {
            //debugger;
            //userInfo :会签人
            //circulateUserInfo :阅知人
            if (result) {
                if (result.userInfo) {
                    for (var i = 0; i < result.userInfo.length; i++) {
                        addMessage("会签人:" + result.userInfo[i].fullPath);
                    }
                }
                if (result.circulateUserInfo) {
                    for (var i = 0; i < result.circulateUserInfo.length; i++) {
                        addMessage("阅知人:" + result.circulateUserInfo[i].fullPath);
                    }
                }
            }
        }

        function addMessage(msg) {
            result.innerHTML += "<p style='margin:0'>" + msg + "</p>";
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <input type="button" id="Button2" value="ExtConsignUserSelector 单选" onclick="showDialog('ExtConsignUserSelector1');" />
        <br />
        <input type="button" id="Button1" value="ExtConsignUserSelector 多选" onclick="showDialog('ExtConsignUserSelector2');" />
    </div>
    <div>
        <cc1:ExtConsignUserSelector runat="server" ID="ExtConsignUserSelector1" MultiSelect="false" />
    </div>
    <div>
        <cc1:ExtConsignUserSelector runat="server" ID="ExtConsignUserSelector2" MultiSelect="true"
            DialogTitle="请选择人员" />
    </div>
    <br />
    <div id="resultContainer">
        <div id="result" contenteditable="true" style="overflow: auto; border: 1px silver solid;
            width: 60%; height: 200px" runat="server">
        </div>
    </div>
    <br />
    <asp:Button ID="Button3" runat="server" Text="postback" />
    </form>
</body>
</html>
