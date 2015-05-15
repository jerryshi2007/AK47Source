<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GenerateOrgTree.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.TreeGraphTest.GenerateOrgTree" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>动态生成树型图</title>
</head>
<body>
    <form id="serverForm" runat="server">
        <div>
            <asp:Button runat="server" ID="redirectToChart" Text="生成图形" OnClick="redirectToChart_Click" />
        </div>
    </form>
</body>
</html>
