<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Left.aspx.cs" Inherits="MCS.OA.CommonPages.UserInfoExtend.Left" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="javascript" type="text/javascript">
        function onSelectNode(sender, e) {
            window.returnValue = false;
            var clickObj = event.screlement;

            var Nodeid = e.object.id;
            var url = "SearchContent.aspx?id=" + Nodeid;

            window.open(url, "SearchContent");
        }

    </script>
</head>
<body>
    <form id="serverForm" runat="server" style="margin: 0px;">
    <div>
        <HB:UserOUGraphControl ID="userSelector" runat="server" ShowingMode="Normal" RootExpanded="true"
            Width="100%" Height="430px" ListMask="Organization" OnLoadingObjectToTreeNode="LoadObectjToTreeNode"
            SelectMask="Organization" OnNodeSelecting="onSelectNode" />
    </div>
    </form>
</body>
</html>
