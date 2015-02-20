<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="testConsignControl.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.Workflow.testConsignControl" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function showDialog(ctrlid) {
            
        }

        function displaySelectedObjects(result) {
            //debugger;
            if (result) {
                if (result.opinion.length > 0) {
                    addMessage(result.opinion);
                }
                for (var i = 0; i < result.users.length; i++) {
                    addMessage(result.users[i].fullPath);
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
        
        <asp:Button ID="btn_show" runat="server" Text="Show.." />
        <%--<cc1:WfActivityDescriptorEditor runat="server" ID="WfActivityDescriptorEditor1" />--%>
        <cc1:WfMoveToControl runat="server" ID="WfMoveToControl1" AutoShowResoureUserSelector="true" />
        <cc1:WfConsignControl runat="server" ID="WfConsignControl1" TargetControlID="btn_show" />
    </div>
    <br />
    <div id="resultContainer">
        <div id="result" contenteditable="true" style="overflow: auto; border: 1px silver solid;
            width: 60%; height: 200px" runat="server">
        </div>
    </div>
    </form>
</body>
</html>
