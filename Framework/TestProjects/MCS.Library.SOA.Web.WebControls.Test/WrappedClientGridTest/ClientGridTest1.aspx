<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientGridTest1.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest.ClientGridTest1" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ClientGrid客户端测试</title>
    <script type="text/javascript">

        function OnPreHeaderRowCreate(grid, e) {
            var tr = $HGDomElement.createElementFromTemplate(
            {
                nodeName: "tr"
            },
            e.container);
            //----
            var td1 = $HGDomElement.createElementFromTemplate(
            {
                nodeName: "td",
                properties:
                {
                    colSpan: 2,
                    style: { border: "1px solid #fff" },
                    innerText: "二级标题头"
                }
            },
            tr
            );
            //-----
            var td2 = $HGDomElement.createElementFromTemplate(
            {
                nodeName: "td",
                properties:
                {
                    colSpan: 3,
                    style: { border: "1px solid #fff" },
                    innerText: "二级标题头"
                }
            },
            tr
            );

        };

        function GetSum() {
            var grid = $find("clientGrid");
            var gridRows = grid.get_gridRows();
            for (var i = 0; i < grid.get_rowCount(); i++) {
                var result = gridRows[i].get_sum();
                var result1 = gridRows[i].get_avg();
                alert("第" + (i + 1).toString() + "行 求和：" + result + " 求平均值：" + result1);
            }
        }

        function GetColumnVal() {
            var grid = $find("clientGrid");
            var val = grid.get_sum("Amount");
            var avg = grid.get_avg("Amount");
            alert("Amount列的总和为：" + val + " 平均值为：" +  avg);
        }

        function GetDecimalVal() {
            var grid = $find("clientGrid");
            var  value = grid.get_gridRows()[0].get_gridCells()[4].get_data();
        }

        function OnCellCreatedEditor(grid, e) {
            if (e.column.dataField == "BookTitle") {
                e.runDataToEditor = false; //某列不在赋数据源的值
                e.editor.get_editorElement().innerText = "张力";
            }
        }
        //&& grid.get_readOnly()
        function CellCreatingEditor(grid, e) {
            if (e.column.dataField == "Context") {

                var textbox = $HGDomElement.createElementFromTemplate(
                {
                    id: "txtContext",
                    nodeName: "input",
                    properties:
                    {
                        type: "text",
                        style: { border: "1px solid green" }
                    }
                }, e.container);

                textbox.onchange = function () { e.editor.set_dataFieldDataByEvent(textbox.value); }

                var button = $HGDomElement.createElementFromTemplate(
                {
                    id: "btnContext",
                    nodeName: "input",
                    properties:
                    {
                        type: "button",
                        value: "测试"
                    },
                    events: {
                        click: Function.createDelegate(this, function () {
                            alert("button_click");
                            var s = "dataChangeing";
                            //e.editor.get_editorElement().value = s;
                            e.editor.set_dataFieldDataByEvent(s);
                        })
                    }
                }, e.container);
            }

            e.editor.set_editorElement(textbox);


        }

        function DataChanged(grid, e) {
            e.editor.get_editorElement().value = e.rowData["Context"];
        }
    </script>
    <script type="text/javascript">
        function OnCellCreatingEditor(grid, e) {

            switch (e.column.dataField) {
                case "Category":
                    if (grid.get_readOnly() == true) {

                        e.showValueTobeChange = "泉水";
                    }
            }

        }

        function AfterDataBind(grid, e) {
            //            grid.set_showFooter(true);
            //            grid._footerRowLeft.innerText = "sfsf";


            grid.set_showFooterLeft(true);
            var rowLeft = grid.get_footerRowLeft();
            rowLeft.firstChild.colspan = 5;
            rowLeft.firstChild.innerText = "合计:24242";

        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <a href="#" onclick="onEditBook();">Add</a>&nbsp;&nbsp; <a href="#" onclick="onDeleteBook();">
            Delete</a>
    </div>
    <div>
        <HB:ClientGrid runat="server" ID="clientGrid" Caption="设置标题头" Width="570px" ShowEditBar="true"
            OnPreHeaderRowCreate="OnPreHeaderRowCreate" OnCellCreatedEditor="OnCellCreatedEditor"
            OnAfterDataBind="AfterDataBind">
            <Columns>
                <HB:ClientGridColumn DataField="BookNo" HeaderText="BookNo" HeaderStyle="{ width: '120px'}"
                    DataType="String" />
                <HB:ClientGridColumn DataField="BookTitle" HeaderText="BookTitle" DataType="String" />
                <HB:ClientGridColumn DataField="PublishDate" HeaderText="PublishDate" DataType="DateTime"
                    FormatString="{0:yyyy-MM-dd}" />
                <HB:ClientGridColumn DataField="Amount" HeaderText="Amount" ItemStyle="{ width: '80px' }"
                    DataType="Integer" IsStatistic="true" />
                <HB:ClientGridColumn DataField="Money" HeaderText="Money" ItemStyle="{ width: '80px' }"
                    DataType="Decimal" IsStatistic="true">
                    <EditTemplate EditMode="TextBox" />
                </HB:ClientGridColumn>
            </Columns>
        </HB:ClientGrid>
    </div>
    <div>
        <input type="button" value="求被统计列的和与平均值" onclick="GetSum()" />
        <input type="button" value="求指定列的和与平均值" onclick="GetColumnVal()" />
             <input type="button" value="取Decimal值" onclick="GetDecimalVal()" />
        <asp:Button ID="Button2" runat="server" Text="Button" onclick="Button2_Click" />
    </div>
    <br />
    <div>
        <HB:ClientGrid runat="server" ID="clientGrid1" Caption="自定义控件" Width="570px" ToolTip="是否会触发ClientGrid的DataChanged与DataChanging事件"
            OnCellCreatingEditor="CellCreatingEditor" OnDataChanged="DataChanged">
            <%-- ReadOnly="true"--%>
            <Columns>
                <HB:ClientGridColumn DataField="BookNo" HeaderText="序号" HeaderStyle="{ width: '120px'}"
                    DataType="Integer" />
                <HB:ClientGridColumn DataField="Context" HeaderText="内容">
                    <EditTemplate EditMode="None" />
                </HB:ClientGridColumn>
            </Columns>
        </HB:ClientGrid>
    </div>
    <br />
    <div>
        <HB:ClientGrid runat="server" ID="clientGrid12" Caption="自定义控件" Width="570px" ReadOnly="true"
            OnCellCreatingEditor="OnCellCreatingEditor">
            <Columns>
                <HB:ClientGridColumn DataField="BookNo" HeaderText="序号" DataType="Integer" />
                <HB:ClientGridColumn DataField="Category" HeaderText="类型">
                    <EditTemplate EditMode="DropdownList" TemplateControlID="DropDownList1" TemplateControlClientID="DropDownList1" />
                </HB:ClientGridColumn>
            </Columns>
        </HB:ClientGrid>
    </div>
    <div style="display: none">
        <asp:DropDownList ID="DropDownList1" runat="server">
            <asp:ListItem Text="泉水" Value="QUANSHUI"></asp:ListItem>
            <asp:ListItem Text="叮当" Value="DINGDANG"></asp:ListItem>
        </asp:DropDownList>
    </div>
    <br />
    <br />
    <div>
        <SOA:ClientGrid runat="server" ID="AppliedBankAccountGrid" Caption="查看布局" Width="570px"
            AutoWidthOfNotFixeLines="true" ShowEditBar="true">
            <%--OnCellCreatingEditor="OnCellCreatingEditor_AppliedBankAccount"
            OnCellCreatedEditor="OnCellCreatedEditor_AppliedBankAccount" OnDataChanged="OntDataChanged_AppliedBankAccount"
            OnHeadCellCreating="OnHeadCellCreating_AppliedBankAccount" OnPreRowAdd="OnPreRowAdd_AppliedBankAccount"
            OnAfterDataRowCreate="OnAfterDataRowCreate_AppliedBankAccount" OnBeforeDataBind="OnBeforeDataBind_AppliedBankAccount"--%>
            <Columns>
                <SOA:ClientGridColumn DataField="index" HeaderText="序号" DataType="Integer" ItemStyle="{textAlign:'center',width:'48px'}"
                    HeaderStyle="{width:'48px',textAlign:'center'}" EditorStyle="{width:'95%',textAlign:'center'}" />
                <SOA:ClientGridColumn DataField="BankAccountName" DataType="String" HeaderText="账户名称"
                    HeaderStyle="{width:'130px',textAlign:'center'}" ItemStyle="{textAlign:'center',width:'130px'}"
                    EditorStyle="{width:'99%',textAlign:'center'} " IsFixedLine="true">
                    <EditTemplate EditMode="TextBox" />
                </SOA:ClientGridColumn>
                <SOA:ClientGridColumn DataField="ReasonOfNotDefaultCashPool" DataType="String" HeaderText="修改现金池选项原因"
                    HeaderStyle="{width:'180px',textAlign:'center'}" EditorStyle="{textAlign:'left',width:'90%'}"
                    EditorTooltips="" ItemStyle="{textAlign:'center',width:'180px'}">
                    <EditTemplate EditMode="TextBox" />
                </SOA:ClientGridColumn>
                <SOA:ClientGridColumn DataField="ReasonOfNotCashPoolBank" DataType="String" HeaderText="不选择现金池银行的原因"
                    HeaderStyle="{width:'180px',textAlign:'center'}" EditorStyle="{textAlign:'left',width:'90%'}"
                    EditorTooltips="" ItemStyle="{textAlign:'center',width:'180px'}">
                    <EditTemplate EditMode="TextBox" />
                </SOA:ClientGridColumn>
                <SOA:ClientGridColumn DataField="ApplicationReason" DataType="String" HeaderText="申请理由"
                    HeaderStyle="{width:'180px',textAlign:'center'}" EditorStyle="{textAlign:'left',width:'90%'}"
                    EditorTooltips="" ItemStyle="{textAlign:'center',width:'180px'}">
                    <EditTemplate EditMode="TextBox" />
                </SOA:ClientGridColumn>
            </Columns>
        </SOA:ClientGrid>
    </div>
    <br />
    <table width="60%">
        <tr>
            <td>
                <asp:TextBox ID="TextBox1" runat="server" Width="20%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox><asp:Button ID="Button1"
                    runat="server" Text="Button" OnClick="button_OnClick" />
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
