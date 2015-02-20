<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SimpleTreeTest.aspx.cs"
    Inherits="MCS.Web.Responsive.WebControls.Test.DeluxeTree.SimpleTreeTest" %>

<%@ Register Assembly="MCS.Web.Responsive.WebControls" Namespace="MCS.Web.Responsive.WebControls"
    TagPrefix="DeluxeWorks" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Simple Tree Test</title>
    <%--   <link href="../css/treeview.css" rel="stylesheet" type="text/css" />--%>
    <link href="../css/widget.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        
    </style>
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
            result.innerHTML += "<p style='margin:0'>" + msg + "</p>";
        }

        function showMultiSelectedNodes() {
            var tree = $find("tree");
            var selectedNodes = tree.get_multiSelectedNodes();
            var strB = new Sys.StringBuilder();

            for (var i = 0; i < selectedNodes.length; i++) {
                strB.appendLine(selectedNodes[i].get_text());
            }

            result.innerText = strB.toString();

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
    <form id="serverForm" runat="server">
    <div class="container" style="background-color: white;">
        <div id="widget-box" class="widget-box">
            <div id="widget-header" class="widget-header">
            </div>
            <div id="widget-body" class="widget-body">
                <div id="widget-content" class="widget-content" style="height: 400px; overflow-y: scroll">
                    <div class="form-group">
                        <input class="form-control" id="nodeText" style="width: 200px; display: inline;"
                            type="text" value="" placeholder="节点名称" />
                        <input class="btn btn-default" type="button" value="设置节点文本" onclick="onChangeNodeText();" />
                    </div>
                    <DeluxeWorks:DeluxeTree ID="tree" runat="server" OnNodeSelecting="onTreeNodeSelecting" ShowLines="False"
                        OnNodeContextMenu="onTreeNodeContextMenu" OnNodeDblClick="onTreeNodeDblClick"
                        OnNodeAfterExpand="onTreeNodeAfterExpand" OnGetChildrenData="tree_GetChildrenData"
                        OnNodeCheckBoxBeforeClick="onTreeNodeCheckBoxBeforeClick" OnNodeCheckBoxAfterClick="onTreeNodeCheckBoxAfterClick"
                        NodeCloseImg="closeImg.gif" NodeOpenImg="openImg.gif"
                         CallBackContext="Test Context"
                        NodeIndent="16">
                        <Nodes>
                            <DeluxeWorks:DeluxeTreeNode Text="第一个节点" ShowCheckBox="True" NodeCloseImg="" NodeOpenImg=""
                                  Checked="True" NavigateUrl="" Target="" />
                            <DeluxeWorks:DeluxeTreeNode Expanded="True" Text="第二个节点" NodeCloseImg="" NodeOpenImg=""
                                  SubNodesLoaded="True" NavigateUrl="" Target="">
                                <Nodes>
                                    <DeluxeWorks:DeluxeTreeNode Text="我是text" Html="<B>A</B>lert <B>A</B>lert <B>A</B>lert <B>A</B>lert <B>A</B>lert <B>A</B>lert <B>A</B>lert <B>A</B>lert <B>A</B>lert <B>A</B>lert <B>A</B>lert"
                                        NodeCloseImg="" NodeOpenImg="" NavigateUrl="javascript:alert(&quot;Hello, 李安(Turtle)&quot;)"
                                        TextNoWrap="false" NodeVerticalAlign="Top" Target="" EnableToolTip="false" />
                                    <DeluxeWorks:DeluxeTreeNode NodeCloseImg="" NodeOpenImg="" Text="www.sina.com.cn"
                                        NavigateUrl="http://www.sina.com.cn" Target="innerFrame">
                                    </DeluxeWorks:DeluxeTreeNode>
                                    <DeluxeWorks:DeluxeTreeNode NodeCloseImg="" NodeOpenImg="" Text="www.microsoft.com"
                                        NavigateUrl="http://www.microsoft.com" Target="innerFrame">
                                    </DeluxeWorks:DeluxeTreeNode>
                                </Nodes>
                            </DeluxeWorks:DeluxeTreeNode>
                            <DeluxeWorks:DeluxeTreeNode Text="第三个节点" NodeCloseImg="" NodeOpenImg="" ChildNodesLoadingType="LazyLoading"
                                  NavigateUrl="" Target="" Selected="True" />
                            <DeluxeWorks:DeluxeTreeNode Text="节点的图标为切割图片"  >
                                <DeluxeWorks:DeluxeTreeNode Text="在线" ImgWidth="16px" ImgHeight="18px" NodeCloseImg="msn-icon4.gif"
                                    NodeOpenImg="msn-icon4.gif" ChildNodesLoadingType="LazyLoading" />
                                <DeluxeWorks:DeluxeTreeNode Text="隐身" ImgWidth="16px" ImgHeight="18px" ImgMarginLeft="-16px"
                                    NodeCloseImg="msn-icon4.gif" NodeOpenImg="msn-icon4.gif" ChildNodesLoadingType="LazyLoading" />
                                <DeluxeWorks:DeluxeTreeNode Text="忙碌" ImgWidth="16px" ImgHeight="18px" ImgMarginLeft="-32px"
                                    NodeCloseImg="msn-icon4.gif" NodeOpenImg="msn-icon4.gif" ChildNodesLoadingType="LazyLoading" />
                                <DeluxeWorks:DeluxeTreeNode Text="离开" ImgWidth="16px" ImgHeight="18px" ImgMarginLeft="-48px"
                                    NodeCloseImg="msn-icon4.gif" NodeOpenImg="msn-icon4.gif" ChildNodesLoadingType="LazyLoading" />
                            </DeluxeWorks:DeluxeTreeNode>
                            <DeluxeWorks:DeluxeTreeNode Text="在线" ImgWidth="16px" ImgHeight="18px" ImgMarginTop="2px" NodeCloseImg="msn-icon4.gif"
                                NodeOpenImg="msn-icon4.gif" ChildNodesLoadingType="LazyLoading" />
                            <DeluxeWorks:DeluxeTreeNode NodeCloseImg="" NodeOpenImg="" Text="加载子节点会出现异常" NavigateUrl=""
                                Target="" ChildNodesLoadingType="LazyLoading">
                            </DeluxeWorks:DeluxeTreeNode>
                            <DeluxeWorks:DeluxeTreeNode NodeCloseImg="" NodeOpenImg="" Text="很多子节点，小心打开！" NavigateUrl=""
                                  Target="" ChildNodesLoadingType="LazyLoading">
                            </DeluxeWorks:DeluxeTreeNode>
                        </Nodes>
                    </DeluxeWorks:DeluxeTree>
                </div>
            </div>
            <div id="container" runat="server">
                <input class="btn btn-default" type="button" value="Show selected nodes" onclick="showMultiSelectedNodes();" />
                <input class="btn btn-default" type="button" value="Clear Children" onclick="onClearChildren();" />
                <input class="btn btn-default" type="button" value="Reload Children" onclick="onReloadChildren();" />
                <asp:Button ID="postBackBtn" runat="server" Text="PostBack" CssClass="btn btn-default" />
            </div>
            <div id="dynamicTreeContainer" runat="server">
            </div>
            <div id="resultContainer">
                <div id="result" contenteditable="true" style="overflow: auto; border: 1px silver solid;
                    width: 100%; height: 200px">
                </div>
            </div>
            <iframe style="border: 1px solid black; width: 100%; height: 150px" name="innerFrame"
               frameborder="0"></iframe>
        </div>
    </div>
    </form>
</body>
</html>
