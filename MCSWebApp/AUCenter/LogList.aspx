<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogList.aspx.cs" Inherits="AUCenter.LogList" %>

<%@ Register Src="inc/Banner.ascx" TagName="Banner" TagPrefix="au" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>权限中心-日志列表</title>
    <link rel="icon" href="favicon.ico" type="image/x-icon" />
    <link rel="Shortcut Icon" href="favicon.ico" />
    <base target="_self" />
</head>
<body style="overflow: hidden">
    <form id="form1" runat="server">
    <div class="pc-frame-header">
        <au:Banner ID="Banner1" runat="server" ActiveMenuIndex="3" />
    </div>
    <div class="pc-frame-container">
        <div class="pc-frame-vs" id="vspanel">
            <div class="pc-frame-left" style="overflow: auto; zoom: 1">
                <!-- 树控件 -->
                <mcs:DeluxeTree runat="server" ID="tree" OnNodeSelecting="onNodeSelecting">
                </mcs:DeluxeTree>
                <asp:HiddenField ID="navOUID" runat="server" Value="" />
            </div>
            <div class="pc-frame-right">
                <iframe frameborder="0" id="frmView" style="height: 100%; width: 100%" src="about:blank">
                    您的浏览器必须支持IFrame！ </iframe>
            </div>
            <div class="pc-frame-splitter-mask" style="z-index: 13">
                <div class="pc-frame-splitter" style="z-index: 14" unselectable="on">
                </div>
            </div>
        </div>
    </div>
    <div id="ldpg" class="pc-loader" style="display: block">
    </div>
    <div style="display: none">
        <input type="hidden" runat="server" id="shuttlePoint" />
        <asp:Button runat="server" ID="shuttleTrigger" OnClick="ShuttleClick" />
    </div>
    </form>
    <script type="text/javascript">
        $pc.ui.configFrame("vspanel");
        $pc.ui.listMenuBehavior("listMenu");
        var regG = /^G(\.\w+(\.\w+)?)?$/i;

        function shuttle(timePoint) {
            document.getElementById("shuttlePoint").value = timePoint;
            document.getElementById("shuttleTrigger").click();
        }

        function onNodeSelecting(sender, e) {
            var cate = e.node.get_value();
            if (regG.test(cate)) {
                $get("frmView").src = $pc.appRoot + "LogView.aspx?catelog=" + cate.substring(2);
            }

            showLoader(true);
            //$get("result").innerText = e.node.get_value() + ": " + e.node.get_text() + "\n" + $get("result").innerText;
        }

        function showLoader(show) {
            var ele = $get("ldpg");
            ele.style.display = show ? "block" : "none";
            ele = null;
        }

        Sys.Application.add_init(function () {
            var tree = $find("tree");
            if (tree) {
                tree.selectNode(tree.get_nodes()[0]);
                tree.get_element().style.zoom = 1; // 树在页面刷新之后消失
            }
            tree = null;
            $get("frmView").src = $pc.appRoot + "LogView.aspx";
            showLoader(true);

        });
    </script>
</body>
</html>
