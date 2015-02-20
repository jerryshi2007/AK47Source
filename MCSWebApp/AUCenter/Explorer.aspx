<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Explorer.aspx.cs" Inherits="AUCenter.Explorer" %>

<%@ Register Src="inc/Banner.ascx" TagName="Banner" TagPrefix="au" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>管理单元维护</title>
    <meta http-equiv="X-UA-Compatible" content="IE=7" />
    <link rel="icon" href="favicon.ico" type="image/x-icon" />
    <link rel="Shortcut Icon" href="favicon.ico" />
    <style type="text/css">
        #cateName
        {
            font-size: 14px;
            padding: 5px;
            line-height: 20px;
            font-weight: bold;
        }
    </style>
</head>
<body class="">
    <form id="form1" runat="server">
    <div class="pc-frame-header">
        <au:Banner ID="pcBanner" runat="server" ActiveMenuIndex="1" />
    </div>
    <div class="pc-frame-container">
        <div class="pc-frame-vs" id="vspanel">
            <div class="pc-frame-left" style="overflow: auto">
                <ul class="pc-tree-list" id="tl">
                    <li style="min-width: 100px"><span id="cateName" runat="server" enableviewstate="false">
                        管理架构信息</span> </li>
                    <li style="position: static; zoom: 1; position: absolute; top: 25px; bottom: 0; left: 0;
                        right: 0; overflow: auto;">
                        <!-- 树控件 -->
                        <mcs:DeluxeTree runat="server" CssClass="pc-mcstree1" ID="tree" OnNodeSelecting="onNodeSelecting"
                            OnGetChildrenData="tree_GetChildrenData" Overflow="Visible" CollapseImage=""
                            ExpandImage="" InvokeWithoutViewState="True" NodeCloseImg="" NodeOpenImg="">
                        </mcs:DeluxeTree>
                    </li>
                </ul>
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
            var cate = e.node.get_extendedData();
            if (cate == "schema")
                document.getElementById('frmView').contentWindow.location.replace("AdminUnitList.aspx?schemaId=" + e.node.get_value());
        }
    </script>
</body>
</html>
