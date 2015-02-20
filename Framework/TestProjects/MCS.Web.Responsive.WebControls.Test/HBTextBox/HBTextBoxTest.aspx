<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HBTextBoxTest.aspx.cs"
    Inherits="MCS.Web.Responsive.WebControls.Test.HBTextBox.HBTextBoxTest" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>HBTextBox Test</title>
    <script type="text/javascript">
        function test(parameters) {
            $HBTextBox.setValue("HBTextBox1", "aabb");
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <res:HBTextBox ID="HBTextBox1" runat="server" TextMode="SingleLine" ReadOnly="True"
            KeepControlWhenReadOnly="True"></res:HBTextBox>
        <input type="button" value="Test" onclick="test();" />
    </div>
    </form>
</body>
</html>
