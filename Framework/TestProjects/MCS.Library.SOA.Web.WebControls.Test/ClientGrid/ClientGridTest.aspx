<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientGridTest.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.HBGrid.HBGridTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>无标题页</title>
    <style type="text/css" >
        tr.fixed-style>*
        {
            background: #123456;
            color:#ffffff;
            text-decoration: underline overline line-through blink;
        }
        
        tr.hover-item *
        {
            background: #00ffff;
            color: #0000ff;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <script type="text/javascript">
        function initGrid() {
            //创建列表对象
            $create($HBRootNS.ClientGrid,
				{
				    autoBindOnLoad: false,
				    autoPaging: false,
				    renderBatchSize: 10,
				    pageSize: 10,
				    cellPadding: "3px",
				    caption: "列表",
				    emptyDataText: "没有记录！",
				    columns: [{ selectColumn: true, showSelectAll: true, headerStyle: { width: "30px"} },
								{ dataField: "id", headerText: "id值", sortExpression: "id", headerStyle: { width: "50px" }, itemStyle: { width: "50px"} },
								{ dataField: "name", headerText: "name值", sortExpression: "name", cellDataBound: function (grid, e) { e.cell.innerText = e.data["name"] + "abc"; } },
								{ dataField: "price", headerText: "price值" },
								{ dataField: "deleteCommand", headerText: "删除", itemStyle: { width: "50px", textAlign: "center"}}],
				    keyFields: ["id"],
				    allowPaging: true,
				    autoPaging: true,
				    pagerSetting: { mode: $HBRootNS.PageSettingMode.text, position: $HBRootNS.PagerPosition.topAndBottom, firstPageText: "<<", prevPageText: "上页", nextPageText: "下页", lastPageText: ">>" },
				    style: { width: "800px" }
				},
				{
				    pageIndexChanged: onPageIndexChanged,
				    sorted: onSorted,
				    cellDataBound: onCellDataBound,
				    afterDataBind: onAfterDataBind
				},
				null,
                $get("grid"));
        }

        //获取选中项数据
        function showselectedData() {
            var grid = $find("grid");
            alert($Serializer.serialize(grid.get_selectedData()));
        }

        //设置列表属性，绑定数据
        function dataBind() {
            var grid = $find("grid");
            grid.set_caption("列表1");

            grid.set_rowCount(201);
            grid.set_showFooter(true);
            setGridControlDataSource(grid);
        }

        //相应翻页事件
        function onPageIndexChanged(gridControl, e) {
            if (!gridControl.get_autoPaging())
                setGridControlDataSource(gridControl);
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
						},
						cell
					);
                    break;
            }
        }

        function onSorted(gridControl, e) {
            //实现本页排序
            //gridControl.get_dataSource().sort(Function.createDelegate(gridControl, dataSourceSort));
            //gridControl.dataBind();
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

        function onAfterDataBind(grid, e) {
            $get("commentDiv").style.visibility = "hidden";
        }

        function setGridControlDataSource(gridControl) {
            if (gridControl.get_autoPaging()) {
                var pageIndex = 0;
                var pageSize = 501;
            }
            else {
                var pageIndex = gridControl.get_pageIndex();
                var pageSize = gridControl.get_pageSize();
            }
            gridControl.set_dataSource(getDataSource(pageIndex, pageSize));
        }

        function getDataSource(pageIndex, pageSize) {
            var dataSource = [];
            for (var i = 0; i < pageSize; i++) {
                var num = pageIndex * pageSize + i;
                var data = { id: num, name: "name" + num };
                data.get_price = function () { return this.id + 50; };
                Array.add(dataSource, data);
            }
            return dataSource;
        }

        function getHugeData() {
            var dataSource = [];
            for (var i = 0; i < 2; i++) {
                var data = { id: i + "_" + "aaabbb", name: "name" + i };
                data.get_price = function () { return this.id + 50; };
                Array.add(dataSource, data);
            }

            return dataSource;
        }

        function hugeDataBind() {
            $get("commentDiv").style.visibility = "visible";
            var grid = $find("grid");
            grid.set_caption("列表2");
            grid.set_autoPaging(false);
            grid.set_dataSource(getHugeData());
        }

        Sys.Application.add_init(initGrid);
        Sys.Application.add_load(function () { hugeDataBind(); });

        function onPreRowAdd(s, e) {
            if (e.rowData.ID == 'a')
                e.htmlRow.htmlRowLeft.className += ' fixed-style';
        }

        Sys.Application.add_load(function () {
            $find('newGrid').set_dataSource([{ ID: 'a', Name: '张三' }, { ID: 'b', Name: '李四'}]);
        });
    </script>
    <input type="button" value="数据绑定" onclick="dataBind()" />
    <input type="button" value="大数据绑定" onclick="hugeDataBind()" />
    <input type="button" value="选择结果" onclick="showselectedData()" />
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true">
            <%--<Scripts>
				<asp:ScriptReference Path="test.js" />
			</Scripts>--%>
        </asp:ScriptManager>
    </div>
    <table>
        <tr>
            <td>
                <table id="grid" align="center">
                </table>
            </td>
            <td style="vertical-align: top">
                <div id="commentDiv" style="visibility: hidden">
                    <img src="girl.gif" />
                    <div>
                        正在渲染...</div>
                </div>
            </td>
        </tr>
    </table>
    <SOA:ClientGrid runat="server" ID="gridWorkPlan" ShowEditBar="true" AllowPaging="false"
        AutoPaging="false" ShowCheckBoxColumn="true" AutoWidthOfNotFixeLines="true">
        <Columns>
            <SOA:ClientGridColumn DataField="CheckNo" SelectColumn="true" ShowSelectAll="true"
                HeaderStyle="{width:'30px',textAlign:'center',fontWeight:'bold'}" ItemStyle="{width:'30px',textAlign:'center'}">
            </SOA:ClientGridColumn>
            <SOA:ClientGridColumn DataField="rowIndex" HeaderText="序号" DataType="Integer" ItemStyle="{width:'30px',textAlign:'center'}"
                HeaderStyle="{width:'30px',textAlign:'center',fontWeight:'bold'}">
            </SOA:ClientGridColumn>
            <SOA:ClientGridColumn DataField="Phase" HeaderText="阶段" ItemStyle="{width:'120px',textAlign:'center'}"
                MaxLength="50" HeaderStyle="{width:'120px',textAlign:'center',fontWeight:'bold'}"
                EditorStyle="{textAlign:'center'}">
                <EditTemplate EditMode="TextBox" />
            </SOA:ClientGridColumn>
            <SOA:ClientGridColumn DataField="StartDate" DataType="DateTime" HeaderText="开始日期"
                FormatString="{0:yyyy-MM-dd}" ItemStyle="{width:'120px',textAlign:'center'}"
                HeaderStyle="{width:'120px',textAlign:'center',fontWeight:'bold'}" EditorStyle="{width:'100px',textAlign:'center'}">
                <EditTemplate EditMode="DateInput" />
            </SOA:ClientGridColumn>
            <SOA:ClientGridColumn DataField="EndDate" DataType="DateTime" HeaderText="结束日期" FormatString="{0:yyyy-MM-

dd}" ItemStyle="{width:'120px',textAlign:'center'}" HeaderStyle="{width:'120px',textAlign:'center',fontWeight:'bold'}"
                EditorStyle="{width:'100px',textAlign:'center'}">
                <EditTemplate EditMode="DateInput" />
            </SOA:ClientGridColumn>
            <SOA:ClientGridColumn DataField="WorkDay" DataType="Decimal" HeaderText="工作用时(工作日)"
                ItemStyle="{width:'150px',textAlign:'right'}" HeaderStyle="{width:'150px',textAlign:'center',fontWeight:'bold'}"
                EditorStyle="{width:'150px',textAlign:'right'}" MaxLength="10">
                <EditTemplate EditMode="TextBox" />
            </SOA:ClientGridColumn>
            <SOA:ClientGridColumn DataField="WorkContent" DataType="String" HeaderText="工作内容"
                MaxLength="200" ItemStyle="{width:'260px',textAlign:'center'}" HeaderStyle="{width:'260px',textAlign:'center',fontWeight:'bold'}"
                EditorStyle="{width:'260px',textAlign:'left'}">
                <EditTemplate EditMode="TextBox" />
            </SOA:ClientGridColumn>
            <SOA:ClientGridColumn DataField="Budget" DataType="Decimal" HeaderText="预算(万元)" ItemStyle="{width:'100px',textAlign:'right'}"
                HeaderStyle="{width:'100px',textAlign:'center',fontWeight:'bold'}" EditorStyle="{width:'100px',textAlign:'right'}"
                MaxLength="10" FormatString="{0:N2}">
                <EditTemplate EditMode="TextBox" />
            </SOA:ClientGridColumn>
            <SOA:ClientGridColumn DataField="WorkResult" DataType="String" HeaderText="工作成果"
                MaxLength="200" ItemStyle="{width:'280px',textAlign:'center'}" HeaderStyle="{width:'280px',textAlign:'center',fontWeight:'bold'}"
                EditorStyle="{width:'280px',textAlign:'left'}">
                <EditTemplate EditMode="TextBox" />
            </SOA:ClientGridColumn>
        </Columns>
    </SOA:ClientGrid>
    <fieldset>
        <legend>ClientGrid悬停测试</legend>
        <div>
        <p>
        由于Css应用的顺序问题，SelectedItemCssClass和HoveringItemCssClass通常被加载到最后。因此必须协调一下其他样式表，以及样式的优先级问题。
        </p>
        </div>
        <SOA:ClientGrid runat="server" ID="newGrid" ShowEditBar="true" AllowPaging="false"
            AutoBindOnLoad="false" AutoPaging="false" ShowCheckBoxColumn="true" AutoWidthOfNotFixeLines="true"
            OnBeforeDataRowCreate="onPreRowAdd" HoveringItemCssClass="hover-item" SelectedItemCssClass="selected-item">
            <Columns>
                <SOA:ClientGridColumn DataType="String" DataField="Name" HeaderText="Name" />
            </Columns>
        </SOA:ClientGrid>
    </fieldset>
    </form>
</body>
</html>
