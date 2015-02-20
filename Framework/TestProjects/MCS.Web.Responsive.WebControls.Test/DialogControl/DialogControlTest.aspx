<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DialogControlTest.aspx.cs"
    Inherits="MCS.Web.Responsive.WebControls.Test.DialogControl.DialogControlTest" %>

<%@ Register Assembly="MCS.Web.Responsive.WebControls.Test" Namespace="MCS.Web.Responsive.WebControls"
    TagPrefix="mcs" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>DialogControlTest</title>
    <script type="text/javascript">
        function test() {
            var dialogArgs = { name: "", begin: "", end: "" };
            if ($("#result").val()) {
                dialogArgs = Sys.Serialization.JavaScriptSerializer.deserialize($("#result").val());
            }

            $find("customDialog").showDialog(dialogArgs,
                function (sender, args) {
                    //确定
                    $("#result").val(args.result);
                },
                function () {
                    //取消
                }
            );
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div style="padding: 5px 0 0 5px;">
        <mcs:CustomDialogControl ID="customDialog" runat="server" DialogTitle="设置委托人" />
        <input type="button" class="btn btn-default" value="Test" onclick="test();" />
        <input type="hidden" id="result" runat="server"/>
        <asp:Button ID="btnPostBack" runat="server" Text="PostBack" CssClass="btn btn-default"
            onclick="btnPostBack_Click" />
    </div>
    </form>
</body>
</html>
