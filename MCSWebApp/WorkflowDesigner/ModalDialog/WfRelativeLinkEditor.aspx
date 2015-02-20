<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WfRelativeLinkEditor.aspx.cs" Inherits="WorkflowDesigner.ModalDialog.WfRelativeLinkEditor" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>相关链接列表</title>
    <base target="_self">
    <link href="/MCSWebApp/Css/WebControls/ClientGrid.css" rel="stylesheet" type="text/css"/>
    <script type="text/javascript" src="../js/jquery-1.4.3.js"></script>
    <script type="text/javascript" src="../js/jquery.json-2.2.js"></script>
    <script type="text/javascript" src="../js/common.js"></script>
    <script type="text/javascript" src="../js/wfweb.js"></script>
    <script type="text/javascript">
        var linkSource;

        function onDocumentLoad() {
            //prepareData();
            var args = window.dialogArguments;
            linkSource = jQuery.parseJSON(args.jsonStr);
			
            createClientGrid();
            bindGrid();
        }

        function prepareData() {
            var result = [];
            for (i = 0; i < 3; i++) {
                result.push({ Key: 'K' + i,
                    Name: 'N' + i,
                    Enabled: true,
                    Description: 'D' + i,
                    Category: 'C' + i,
                    Url: 'U' + i
                });
            }
            return result;
        }

        function createClientGrid() {
            //创建列表对象
            $create($HBRootNS.ClientGrid, 
            {
                pageSize: 5,
                cellPadding: "3px",
                //var data = Sys.Serialization.JavaScriptSerializer.serialize(e.data);
                     
                emptyDataText: "没有记录！",
                columns: [{ selectColumn: true, showSelectAll: true, itemStyle: { width: "10px" }, headerStyle: { width: "10px"} },
                        { dataField: "Key", headerText: "Key", itemStyle: { width: "50px" }, cellDataBound: function (grid, e) {
                            var linkText = "<a href='#' style='color: Black;' onclick='openDetailForm(\"{0}\");'>{0}</a>";
                            e.cell.innerHTML = String.format(linkText, e.data["Key"].toString());
                        }
                        },
                        { dataField: "Name", headerText: "名称", sortExpression: "Name", itemStyle: { width: "50px" }, headerStyle: { width: "100px"} },
				        { dataField: "Enabled", headerText: "是否可用", itemStyle: { width: "50px" }, headerStyle: { width: "100px" }, cellDataBound: function (grid, e) { } },
				        { dataField: "Description", headerText: "描述", itemStyle: { textAlign: "left", width: "100px" }, cellDataBound: function (grid, e) { } },
                        { dataField: "Category", headerText: "类别", itemStyle: { textAlign: "left", width: "50px" }, cellDataBound: function (grid, e) { } },
                        { dataField: "Url", headerText: "Url", itemStyle: { textAlign: "left" }, cellDataBound: function (grid, e) { } }
                    ],
                allowPaging: true,
                autoPaging: true,
                pagerSetting: { mode: $HBRootNS.PageSettingMode.text, position: $HBRootNS.PagerPosition.bottom, firstPageText: "<<", prevPageText: "上页", nextPageText: "下页", lastPageText: ">>" },
                style: { width: "100%" }
            },
		    {pageIndexChanged: onPageIndexChanged, sorted: onSorted, cellDataBound: onCellDataBound },
		    null,
            $get("grid"));
        }
        function bindGrid() {
            var grid = $find("grid");
            setGridControlDataSource(grid, linkSource);
        }

        function setGridControlDataSource(gridControl, dataSource) {
            if (gridControl.get_autoPaging()) {
                var pageIndex = 0;
                var pageSize = 5;
            }
            else {
                var pageIndex = gridControl.get_pageIndex();
                var pageSize = gridControl.get_pageSize();
            }
            gridControl.set_dataSource(dataSource);
        }

        //相应翻页事件
        function onPageIndexChanged(gridControl, e) {
            if (!gridControl.get_autoPaging())
                setGridControlDataSource(gridControl);
        }
        function onSorted(gridControl, e) {
            //实现本页排序
            //           gridControl.get_dataSource().sort(Function.createDelegate(gridControl, dataSourceSort));
            //           gridControl.dataBind();
        }
        //相应表格数据绑定事件
        function onCellDataBound(gridControl, e) {
            var cell = e.cell;
            var dataField = e.column.dataField;
            var data = e.data;
            switch (dataField) {
                case "price":
                    var price = data["price"];
                    if (price < 100)
                        cell.style.color = "red";
                    else
                        cell.style.color = "blue";
                    break;

                case "deleteCommand":
                    e.gridControl = gridControl;
                    var delBtn = $HGDomElement.createElementFromTemplate(
				        {
				            nodeName: "input",
				            properties:
					        {
					            type: "button",
					            value: "删除"
					        },
				            events: { click: Function.createDelegate(e, onGridDeleteBtnClick) }
				        }, cell);
                    break;
            }
        }






        function dataSourceSort(data1, data2) {
            var pName = this.get_sortExpression();
            var sortDirection = this.get_sortDirection();

            v1 = data1[pName];
            v2 = data2[pName];

            if (v1 == v2) return 0;
            if (sortDirection == $HBRootNS.SortDirection.asc)
                result = v1 > v2 ? 1 : -1;
            else
                result = v1 > v2 ? -1 : 1;

            return result;
        }

        function onGridDeleteBtnClick() {
            var gridControl = this.gridControl;

            var data = this.data;

            if ($HGClientMsg.confirm("是否删除本行!")) {
                Array.remove(gridControl.get_dataSource(), data);
                gridControl.dataBind();
            }
        }

        function openDetailForm(linkKey) {
            var sFeature = "dialogWidth:400px; dialogHeight:300px;center:yes;help:no;resizable:no;scroll:no;status:no";

            var linkArg = { jsonStr: jQuery.toJSON(linkSource) };
            WFWeb.Dialog('CreateRelativeLink.aspx', 'key=' + linkKey, sFeature, linkArg, function (r, a) {
                if (r) {
                    linkSource = jQuery.parseJSON(r.jsonStr);
                    bindGrid();
                }
            });
        }

        function del() {
            var grid = $find("grid");
            var selecteditem = grid.get_selectedData();
            if (selecteditem.length <= 0)
                alert("没有选择相关链接。");
            if (selecteditem.length > 0 && confirm("确定删除?")) {
                Array.forEach(selecteditem, function (element, index, array) {
                    linkSource.remove(element.Key, function (obj, val) {
                        if (obj.Key == val) return true;
                        return false;
                    });
                }, null);

                bindGrid();
            }
        }

        function Submit() {
            window.returnValue = { jsonStr: jQuery.toJSON(linkSource) };
            top.close();
        }
    </script>
</head>
<body>
	<form id="serverForm" runat="server">

	<table width="100%" style="height: 100%; width: 100%">
		<tr>
			<td class="gridHead">
				<div class="dialogTitle">
					<span class="dialogLogo">相关链接列表</span>
				</div>
			</td>
		</tr>
		<tr>
			<td style="vertical-align: center">
				<div class="dialogContent" id="dialogContent" runat="server" style="width: 100%;
					height: 100%; overflow: auto">
					<!--Put your dialog content here... -->
					<table width="100%" style="height: 100%; width: 100%">
						<tr>
							<td>
                                <table style="width: 100%; height: 100%; vertical-align:top;">
                                    <tr >
                                        <td style="height:30px; background-color:#C0C0C0">
                                        <a href="#" onclick="openDetailForm('');"><img src="../../../MCSWebApp/Images/appIcon/15.gif" alt="新建" border="0" /></a>
                                        <a href="#" onclick="del();"><img src="../../../MCSWebApp/Images/16/delete.gif" alt="删除"" border="0" /></a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3"  style="width: 100%;height: 100%; vertical-align:top;" >
                                                <table id="grid" align="center" ></table>
                                        </td>
                                    </tr>
                                </table>
							</td>
						</tr>
					</table>
				</div>
			</td>
		</tr>
		<tr>
			<td class="gridfileBottom">
			</td>
		</tr>
		<tr>
			<td style="height: 40px; text-align: center; vertical-align: middle">
				<table style="width: 100%; height: 100%">
					<tr>
                                <td style="text-align: center;"><input type="button" class="formButton" onclick="Submit();" value="确定(O)"  id="btnOK" accesskey="O"/></td>
                                <td style="text-align: center;"><input type="button" class="formButton" onclick="top.close();" value="取消(C)" id="btnCancel" accesskey="C"/></td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
    
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
	</form>
	<script type="text/javascript">
	    Sys.Application.add_load(function () {
	        onDocumentLoad();
	    });
	</script>
</body></html>