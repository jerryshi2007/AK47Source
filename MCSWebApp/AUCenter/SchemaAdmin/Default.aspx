<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AUCenter.SchemaAdmin.Default"
    Title="管理架构维护" %>

<%@ Register Src="../inc/Banner.ascx" TagName="Banner" TagPrefix="au" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>管理架构维护</title>
    <link rel="icon" href="../favicon.ico" type="image/x-icon" />
    <link rel="Shortcut Icon" href="../favicon.ico" />
</head>
<body style="overflow: hidden">
    <form id="form1" runat="server">
    <div class="pc-frame-header">
        <au:Banner ID="Banner1" runat="server" ActiveMenuIndex="2" />
    </div>
    <div class="pc-frame-container">
        <div class="pc-frame-vs" id="vspanel">
            <div class="pc-frame-left" style="overflow: auto">
                <!-- 树控件 -->
                <mcs:DeluxeTree runat="server" CssClass="pc-mcstree1" ID="tree" OnNodeSelecting="onNodeSelecting"
                    OnGetChildrenData="tree_GetChildrenData" Overflow="Visible" CollapseImage=""
                    ExpandImage="" InvokeWithoutViewState="True" NodeCloseImg="" NodeOpenImg="">
                </mcs:DeluxeTree>
            </div>
            <div class="pc-frame-right">
                <iframe frameborder="0" id="frmView" name="frmView" style="height: 100%; width: 100%;"
                    src="Nomatch.aspx">您的浏览器必须支持IFrame！ </iframe>
            </div>
            <div class="pc-frame-splitter-mask" style="z-index: 13">
                <div class="pc-frame-splitter" style="z-index: 14" unselectable="on">
                </div>
            </div>
        </div>
    </div>
    </form>
    <script type="text/javascript">
        $pc.ui.configFrame("vspanel");

        function onNodeSelecting(s, e) {
            document.getElementById('frmView').contentWindow.location.replace("SchemaList.aspx?category=" + e.node.get_value());
        }
    </script>
</body>
</html>
