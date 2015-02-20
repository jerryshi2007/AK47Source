<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OUExplorer.aspx.cs" Inherits="PermissionCenter.OrgExplorer" %>

<%--怪异模式测试时将文档声明置于服务器注释块内--%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" style="">
<head id="Head1" runat="server">
	<title>权限中心-组织</title>
	<link rel="icon" href="../favicon.ico" type="image/x-icon" />
	<link rel="Shortcut Icon" href="../favicon.ico" />
	<style type="text/css">
		i.pc-tool16, i.pc-tool16-refresh, i.pc-tool16-search
		{
			display: inline-block;
			width: 16px;
			height: 16px;
			line-height: 0;
			overflow: hidden;
			cursor: pointer;
		}
		
		i.pc-tool16-refresh
		{
			background: transparent url("../images/sprites.png") no-repeat scroll 0 0;
		}
		
		i.pc-tool16-search
		{
			background: transparent url("../images/sprites.png") no-repeat scroll 0 -20px;
		}
	</style>
</head>
<body style="overflow: hidden">
	<form id="form1" runat="server">
	<div class="pc-frame-header">
		<pc:Banner ID="pcBanner" runat="server" ActiveMenuIndex="2" OnTimePointChanged="ReloadTree" />
	</div>
	<div class="pc-frame-container">
		<div class="pc-frame-vs" id="vspanel">
			<div class="pc-frame-left">
				<div style="position: relative">
					<div style="position: absolute; right: 0;">
						<div class="pc-item-cmd">
							<span class="pc-icon-tool"></span><i class="pc-arrow"></i>
						</div>
					</div>
				</div>
				<div style="display: none" id="toolsHome">
					<div id="tools">
						<div style="position: absolute; right: 0">
							<i class="pc-tool16-search" title="查找" onclick="findInCurrent();"></i><i class="pc-tool16-refresh"
								title="刷新" onclick="refreshCurrent();"></i>
						</div>
					</div>
				</div>
				<ul class="pc-tree-list" id="tl">
					<li style="min-width: 100px"><a href="DissociatedList.aspx" target="frmView" onclick="return onGroupItemClick(this);">
						无组织人员</a> </li>
					<li style="min-width: 100px"><a href="DeletedMemberList.aspx" target="frmView" onclick="return onGroupItemClick(this);"
						style="background-image: url('../images/RecycleBin.png'); background-position: right center;
						background-repeat: no-repeat;">人员回收站</a> </li>
					<li style="position: static; zoom: 1; position: absolute; top: 55px; bottom: 0; left: 0;
						right: 0; overflow: auto;">
						<!-- 树控件 -->
						<mcs:DeluxeTree runat="server" CssClass="pc-mcstree1" ID="tree" OnNodeSelecting="onNodeSelecting"
							OnGetChildrenData="tree_GetChildrenData" Overflow="Visible">
						</mcs:DeluxeTree>
					</li>
				</ul>
				<asp:HiddenField ID="navOUID" runat="server" Value="" />
			</div>
			<div class="pc-frame-right">
				<iframe frameborder="0" id="frmView" name="frmView" style="height: 100%; width: 100%;"
					src="about:blank">您的浏览器必须支持IFrame！ </iframe>
			</div>
			<div class="pc-frame-splitter-mask" style="z-index: 13">
				<div class="pc-frame-splitter" style="z-index: 14" unselectable="on">
				</div>
			</div>
		</div>
	</div>
	<pc:Footer ID="footer" runat="server" />
	<div id="ldpg" class="pc-loader" style="display: block">
	</div>
	<div style="display: none">
		<asp:HiddenField runat="server" ID="lastVisitOrg" />
		<asp:Button Text="刷新" ID="btnRefresh" runat="server" OnClick="ReloadTree" />
	</div>
	</form>
	<script type="text/javascript">

		$pc.ui.configFrame("vspanel");
		$pc.ui.listMenuBehavior("listMenu");
		var thisWin = this.window;

		function onGroupItemClick(a) {
			$get("frmView").src = a.href;
			showLoader(true);
			return false;
		}

		function onNodeSelecting(sender, e) {

			$get("frmView").src = $pc.appRoot + "lists/OUExplorerView.aspx?ou=" + e.node.get_value();
			showLoader(true);
			$get("lastVisitOrg")['value'] = e.node.get_value();
		}

		function selectRoot() {
			var tree = $find("tree");

			tree.selectNode(tree.get_nodes()[0]);

			onNodeSelecting(tree, { node: tree.get_nodes()[0] });
			tree = null;
		}

		function getCurrentNodeId() {
			var rst = '';
			var elem = $find("tree");
			var node;
			if (elem) {
				node = elem.get_selectedNode();
				if (node) {
					rst = node.get_value();
				}
			}
			elem = null;
			node = null;
			return rst;
		}

		function showLoader(show) {
			var ele = $get("ldpg");
			ele.style.display = show ? "block" : "none";
			ele = null;
		}

		var afterExpandCallback = null; //异步加载树节点回调

		function resetTools() {
			document.getElementById("toolsHome").appendChild(document.getElementById("tools"));
		}

		function refreshCurrent() {

			var node = $find("tree").get_selectedNode();
			if (node) {
				reloadNode(node.get_value());
			}
			node = null;
		}

		function findInCurrent() {
			var node = $find("tree").get_selectedNode();
			if (node != null && node.get_value()) {
				showLoader(1);
				$get("frmView").src = $pc.appRoot + "lists/OUSearch.aspx?ou=" + encodeURIComponent(node.get_value());
			}
			node = null;
		}

		Sys.Application.add_init(function () {

			function handleOnSelect(n) {
				if (!n.get_isUpdating() && n.get_value()) {
					n.get_element().insertBefore(document.getElementById("tools"), n.get_element().firstChild);
				} else {
					resetTools();
				}
			}

			var tree = $find("tree");
			if (tree) {
				//tree.selectNode(tree.get_nodes()[0]);

				tree.add_nodeAfterExpand(function (s, e) {
					if (afterExpandCallback) {
						afterExpandCallback.apply(s);
					}
				});

				tree.add_nodeSelecting(function (s, e) {
					handleOnSelect(e.node);
				});

				tree.get_element().style.overflow = 'hidden';
			}

			if (tree.get_selectedNode()) {
				handleOnSelect(tree.get_selectedNode());
			}

			tree.get_element().style.overflow = 'visible';

			tree = null;
			$get("frmView").src = $pc.appRoot + "lists/OUExplorerView.aspx?ou=" + $get("navOUID").value;
			showLoader(true);

		});

		function selectNode(key, reload, parentHint) {
			var success = false;
			resetTools();

			afterExpandCallback = null;

			function visitTree(tree, parentNode, key, reloadNode, superKeyHint) {
				for (var node = parentNode.get_firstChild(); node; node = node.get_nextNode()) {
					if (node.get_value() == key) {
						tree.selectNode(node);
						if (!node.get_isUpdating() && node.get_value()) {
							node.get_element().insertBefore(document.getElementById("tools"), node.get_element().firstChild);
						}
						success = true;
						if (reloadNode) {
							$pc.console.info("重新加载");
							node.set_childNodesLoadingType(1);
							node.reloadChildren();
							return;
						}
						break;
					} else if (typeof (superKeyHint) == 'string' && node.get_value() == superKeyHint && node.get_expanded() == false) {
						var aTree = tree;
						afterExpandCallback = function () {
							afterExpandCallback = null;
							success = false;
							visitTree(aTree, aTree.get_nodes()[0], key, false, superKeyHint);
							aTree = null;

						}
						node.set_expanded(true);
						success = true;
						break;
					}
					if (success) {
						break;
					} else if (node.get_hasChildNodes()) {
						visitTree(tree, node, key, reloadNode, superKeyHint);
					}
				}
				node = null;
			}

			var tree = $find("tree");

			if (tree) {
				if (tree.get_nodes().length > 0) {
					var root = tree.get_nodes()[0];
					if (root.get_value() === key) {
						if (reload) {
							resetTools();
							root.set_subNodesLoaded(false);
							root.set_childNodesLoadingType(1);
							root.reloadChildren();
						}
					} else {
						visitTree(tree, root, key, reload, parentHint);
					}

					root = null;
				}
			}

			tree = null

			$get("lastVisitOrg")['value'] = key;
		}

		function reloadNode(nodeKey) {
			resetTools();
			selectNode(nodeKey, true);
		}

		function hardReload() {
			resetTools();
			document.getElementById("btnRefresh").click();
		}

		//		(function () {
		//			var tools = document.getElementById("tools");
		//			$pc.bindEvent($pc.get("tree"), "click", function (e) {

		//				var src = e.srcElement;
		//				if (src.nodeType === 1 && src.nodeName.toUpperCase() === 'TD') {
		//					src.parentNode.lastChild.appendChild(tools);
		//				}
		//			});
		//		})();


	</script>
</body>
</html>
