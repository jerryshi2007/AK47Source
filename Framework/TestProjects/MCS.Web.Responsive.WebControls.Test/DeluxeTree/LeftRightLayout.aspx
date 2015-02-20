<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LeftRightLayout.aspx.cs" Inherits="MCS.Web.Responsive.WebControls.Test.DeluxeTree.LeftRightLayout" %>

<%@ Register Assembly="MCS.Web.Responsive.WebControls" Namespace="MCS.Web.Responsive.WebControls"
    TagPrefix="DeluxeWorks" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>左右布局对话框</title>
    <link href="../css/layout.css" rel="stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript" src="/jquery/jquery-2.0.3.js"></script>
    <script type="text/javascript">

        function drag(elem, stylearea) {
            var mouse, oWidth;
            mouse = {
                mouseup: function () {
                    $(document).unbind("mousemove", mouse.mousemove);
                    $(document).unbind("mouseup", mouse.mouseup);
                },
                mousemove: function (ev) {
                    var oEvent = ev || event;
                    $(elem).width(oEvent.clientX + "px");
                }
            };

            $(stylearea).mousedown(event, function (ev) {
                var oEvent = ev || event;
                oWidth = $(elem).width();
                $(document).bind("mousemove", mouse.mousemove);
                $(document).bind("mouseup", mouse.mouseup);
            });
        }

        $(document).ready(function () {
            drag($("#left").get(0), $("#slider").get(0));
        });
    </script>
    
    <script type="text/javascript" language="javascript">

        function onTreeNodeSelecting(sender, e) {
            //e.cancel = true;
            //alert("Selected");
        }

        function onTreeNodeContextMenu(sender, e) {
            addMessage("context menu: " + e.node.get_text());
            //e.defaultContextMenu = true;
        }

        function onTreeNodeDblClick(sender, e) {
            addMessage("dbl click: " + e.node.get_text());

            if (e.node.get_children().length > 0)
                e.node.set_expanded(!e.node.get_expanded());
        }

        function onTreeNodeAfterExpand(sender, e) {
            addMessage("after expand: " + e.node.get_text());
        }

        function onTreeNodeBeforeExpand(sender, e) {
            addMessage("before expand: " + e.node.get_text());

            if (e.node.get_value() == "loading") {
                addMessage("loading");

                e.node.clearChildren();

                for (var i = 0; i < Math.random() * 5; i++) {
                    var properties = { text: "动态子节点" + i };
                    e.node.appendChild($HGRootNS.DeluxeTreeNode.createNode(properties));
                }
                e.node.set_value("normal");
            }
        }

        function onTreeNodeCheckBoxBeforeClick(sender, e) {
            addMessage("check before click: " + e.node.get_text() + "; element checked: " + e.eventElement.checked);
            //e.cancel = true;
        }

        function onTreeNodeCheckBoxAfterClick(sender, e) {
            addMessage("check after click: " + e.node.get_text() + "; node checked:  " + e.node.get_checked());
        }

        function addMessage(msg) {
            right.innerHTML += "<p style='margin:0'>" + msg + "</p>";
        }

        function showMultiSelectedNodes() {
            var tree = $find("tree");
            var selectedNodes = tree.get_multiSelectedNodes();
            var strB = new Sys.StringBuilder();

            for (var i = 0; i < selectedNodes.length; i++) {
                strB.appendLine(selectedNodes[i].get_text());
            }

            right.innerText = strB.toString();

            if (tree.get_selectedNode())
                result.innerText += "\n单选:" + tree.get_selectedNode().get_text();
        }

        function onChangeNodeText() {
            var text = $get("nodeText").value;

            var selectedNode = $find("tree").get_selectedNode();

            if (selectedNode) {
                selectedNode.set_text(text);
            }
        }

        function onClearChildren() {
            var selectedNode = $find("tree").get_selectedNode();

            if (selectedNode)
                selectedNode.clearChildren(true, true);
        }

        function onReloadChildren() {
            var selectedNode = $find("tree").get_selectedNode();

            if (selectedNode)
                selectedNode.reloadChildren();
        }

    </script>
</head>
<body>
    <form id="serverForm" runat="server" style="height: 100%">
        <div style="height:100%; padding:5px;overflow:hidden;">
        <div class="row" style="height: 100%; ">
            <div id="left" style="width: 220px; height: 100%; float: left; overflow: auto; padding: 5px 10px 5px 10px">
              
              <DeluxeWorks:DeluxeTree ID="tree" runat="server" OnNodeSelecting="onTreeNodeSelecting" ShowLines="False"
                        OnNodeContextMenu="onTreeNodeContextMenu" OnNodeDblClick="onTreeNodeDblClick"
                        OnNodeAfterExpand="onTreeNodeAfterExpand" OnGetChildrenData="tree_GetChildrenData"
                        OnNodeCheckBoxBeforeClick="onTreeNodeCheckBoxBeforeClick" OnNodeCheckBoxAfterClick="onTreeNodeCheckBoxAfterClick"
                         CallBackContext="Test Context"
                        NodeIndent="16">
                        <Nodes>
                            <DeluxeWorks:DeluxeTreeNode Text="第一个节点" ShowCheckBox="True" NodeCloseImg="" NodeOpenImg=""
                                  Checked="True" NavigateUrl="" Target="" />
                            <DeluxeWorks:DeluxeTreeNode Text="第二个节点" NodeCloseImg="" NodeOpenImg="" ChildNodesLoadingType="LazyLoading"
                                  NavigateUrl="" Target="" Selected="True" />
                            <DeluxeWorks:DeluxeTreeNode NodeCloseImg="" NodeOpenImg="" Text="加载子节点会出现异常" NavigateUrl=""
                                Target="" ChildNodesLoadingType="LazyLoading">
                            </DeluxeWorks:DeluxeTreeNode>
                            <DeluxeWorks:DeluxeTreeNode NodeCloseImg="" NodeOpenImg="" Text="很多子节点，小心打开！" NavigateUrl=""
                                  Target="" ChildNodesLoadingType="LazyLoading">
                            </DeluxeWorks:DeluxeTreeNode>
                        </Nodes>
                    </DeluxeWorks:DeluxeTree>
            </div>
            <div id="slider" style="width: 4px; background-color: rgb(187, 185, 185); float: left; height: 100%; cursor: ew-resize"></div>
            <div id="right">
            </div>
            </div>
        </div>
    </form>
</body>
</html>