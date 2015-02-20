<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PaymentDetailsGridTest.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest.PaymentDetailsGridTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">

        window.onresize = function () { alert(); };

        function PaymentItemOption() {
            var option4 = $HGDomElement.createElementFromTemplate(
					{
					    nodeName: "option",
					    properties:
						{
						    value: "华盛家园一期工程项目",
						    text: "华盛家园一期工程项目"
						}
					}
				);
            var option5 = $HGDomElement.createElementFromTemplate(
					{
					    nodeName: "option",
					    properties:
						{
						    value: "垂虹园二期项目",
						    text: "垂虹园二期项目"
						}
					}
				);

            return [option4, option5];
        }

        function CurrencyOption() {
            var option1 = $HGDomElement.createElementFromTemplate(
					{
					    nodeName: "option",
					    properties:
						{
						    value: 1,
						    text: "人民币"
						}
					}
				);
            var option2 = $HGDomElement.createElementFromTemplate(
					{
					    nodeName: "option",
					    properties:
						{
						    value: 0.8,
						    text: "港元"
						}
					}
				);
            var option3 = $HGDomElement.createElementFromTemplate(
					{
					    nodeName: "option",
					    properties:
						{
						    value: 6.5,
						    text: "美元"
						}
					}
				);

            return [option1, option2, option3];
        }
    </script>
    <script type="text/javascript">
        //获取选中项数据
        function showselectedData() {
            var grid = $find("clientGrid1");

            grid.dataBind();

            var selecteddata = grid.get_selectedData();
            var dataSource = grid.get_dataSource();

            alert("selecteddata" + $Serializer.serialize(selecteddata) + "\n\ndatasource:" + $Serializer.serialize(dataSource));

            //debugger;
        }
        function getData() {
            var data = $find("").get_dataSource();
        }

        function onAddNewRow() {
            $find('clientGrid1').addNewRow();
        }

        function onDeleteInvoices2() {
            event.returnValue = false;

            var gridCtrl = $find("clientGrid1");
            var selectedData = gridCtrl.get_selectedData();

            if (selectedData.length > 0) {
                if (window.confirm("Do you want to delete the selected data ?")) {
                    syncDeletedData2(selectedData);
                }
            }
            return false;
        }

        function syncDeletedData2(selectedData) {
            var gridCtrl = $find("clientGrid1");
            for (var i = 0; i < selectedData.length; i++) {
                Array.remove(gridCtrl.get_dataSource(), selectedData[i]);
            }
            gridCtrl.set_dataSource(gridCtrl.get_dataSource());
        }
    </script>
    <script type="text/javascript">
        //事件处理
        function OnDataChanged1(gridControl, e) {

            //alert(e.editor._value);
            //alert(e.editor._displayValue);
            //alert(e.editor.get_gridCell().get_data());

            //alert("aspx:OnDataChanged1");
            if (e.gridRow) {
                if (e.column.dataField == "RMB" || e.column.dataField == "Currency" || e.column.dataField == "ExchangeRate") {

                    if (e.column.dataField == "Currency")
                        e.rowData["ExchangeRate"] = e.rowData["Currency"];

                    var result = e.rowData["ExchangeRate"] * e.rowData["RMB"];

                    e.rowData["Totle"] = result;
                    e.gridRow.rowDataBind(e.rowData);
                }
            }

            if (e.column.dataField == "PaymentItem") {
                var rmb = e.editor.get_otherEditorByDataField("Currency");
                alert("当前币种汇率：" + rmb._editorElement.value);
            }

            if (e.column.dataField == "INT") {

            }
        }

        function OnDataChanging(gridControl, e) {
            var editor = e.editor.get_editorElement();

            //创建控件的时候 可以修改默认值 还可以修改当前的容器
            switch (e.column.dataField) {
                case "index":
                    e.valueTobeChange = "eee";
                    break;
                case "INT":
                    var fefefefe = "";
                    break;
            }
        }    
    </script>
    <script type="text/javascript">
        function OnDataFormatting1(gridControl, e) {
            var editor = e.editor.get_editorElement();
            switch (e.column.dataField) {
                case "index":
                    editor.innerText = "kkk";
                    break;
            }
        }
        function OnInitializeEditor(gridControl, e) {
            var editor = e.editor.get_editorElement();
            switch (e.column.dataField) {
                case "index":
                    editor.innerText = "test";
                    break;
            }
        }
        function OnHeadCellCreating(gridControl, e) {
            if (e.dataField == "PaymentItem") {
                //e.cell.innerHTML = e.cell.innerText + "<span style='color:red'>*</span>";
            }
        }
        function OnRowDelete(grid, e) {
            alert("deletedData" + $Serializer.serialize(e.deletedData) + "\n\ncurrentData:" + $Serializer.serialize(e.currentData));
        }
    </script>
    <script type="text/javascript">
        var i = 0;
        function OnCellCreatingEditor(gridControl, e) {

            //gridControl.set_captionElementInnerHTML("<span style='color:red'>fff</span>");

            //e.rowData["fefefef"]

            if (e.column.dataField == "Totle") {
                var currentTbxID = "theTbxID_" + i.toString();  //指定一个一一对应的ID..

                var textbox = $HGDomElement.createElementFromTemplate(
                	{
                	    nodeName: "input",
                	    properties:
                	    {
                	        id: currentTbxID,
                	        type: "text",
                	        title: e.rowData["Totle"],
                	        disabled: "disabled"
                	    },
                	    style:
                        {
                            border: "1px solid red"
                        }
                	},
                    e.container);

                var button = $HGDomElement.createElementFromTemplate(
					{
					    nodeName: "input",
					    properties:
						{
						    type: "button",
						    value: "选择人员",
						    mytbxid: currentTbxID
						},
					    events: {
					        click: Function.createDelegate(gridControl, function () {
					            var rmb = e.editor.get_otherEditorByDataField("Currency");

					            e.editor.get_editorElement().value = "90.00";
					            e.editor.set_dataFieldData("90.00");
					        })
					    }
					},
                    e.container
				);
                e.editor.set_editorElement(textbox);

                //var cell = e.editor.get_gridCell();
                //var cellData = cell.get_data();
                //alert(cellData);

                i++;
            }

            if (e.column.dataField == "Currency") {
                //var theselect = document.getElementById("ddlCurrencyList");
                //theselect.value = e.rowData["Currency"];

                //if (theselect.selectedIndex != -1)
                //    e.showValueTobeChange = theselect.options[theselect.selectedIndex].text;
            }

            if (e.column.dataField == "index") {
                e.autoFormat = false;
                e.showValueTobeChange = e.rowData.rowIndex;
            }

            if (e.column.dataField == "RMB" || e.column.dataField == "Currency" || e.column.dataField == "ExchangeRate") {
                //这开始
                if (e.column.dataField == "Currency")
                    e.rowData["ExchangeRate"] = Number(e.rowData["Currency"]);

                var result = e.rowData["ExchangeRate"] * e.rowData["RMB"];

                e.rowData["Totle"] = result;
            }
            if (e.column.dataField == "a") {
                var fscssc = "";
            }
        }
    </script>
    <script type="text/javascript">
        function OnCellCreatedEditor(gridControl, e) {
            var editor = e.editor.get_editorElement();
            var optionArry = [];
            switch (e.column.dataField) {
                case "PaymentItem":
                    optionArry = PaymentItemOption();
                    break;
            }
            for (var i = 0; i < optionArry.length; i++) {
                editor.add(optionArry[i]);
            }
            if (e.column.dataField == "RMB") {
                //e.editor.get_editorElement().disabled = true;
            }
        }

        function OnEditBarRowCreating(gridControl, e) {
            //e.container.style["padding-left"] = "20px";
            //e.container.innerHTML = "<input type='button' value='获取查看选择数据' onclick='showselectedData();' />";
            //document.getElementById("form1").setAttribute()
            var button = $HGDomElement.createElementFromTemplate(
					{
					    nodeName: "input",
					    properties:
						{
						    type: "button",
						    value: "选择人员"
						}
					},
                    e.container
				);
        }
    </script>
    <script type="text/javascript">
        function OnSelectCheckboxCreated(grid, e) {
            //e.checkbox.checked = true;
            //grid._checkboxSelectChanged(e.checkbox);

            grid.defaultSelection(e, true);
        }
    </script>
    <style type="text/css">
        .linbinCss .caption
        {
            background-color: #fefefe;
        }
        .linbinCss .header
        {
            background-color: #e5e5e5;
        }
        .linbinCss tr
        {
            background-color: #eee;
        }
        
        .clientGrid .caption
        {
            /*text-align: center;*/
        }
    </style>
</head>
<body>
    <table id="table1111">
    </table>
    <form id="form1" runat="server">
    <input type="checkbox" id="fefefewfwffe" />
    <input value="test" type="button" onclick="" style="border: 1px solid red; background-color: blue;
        text-align: right;" />
    <br />
    <br />
    <div id="divpppp">
        <a href="#" onclick="onAddNewRow();">Add...</a>&nbsp;&nbsp; <a href="#" onclick="onDeleteInvoices2();">
            Delete...</a>
    </div>
    <br />
    <div>
        <HB:ClientGrid runat="server" ID="clientGrid1" Caption="差旅费申报单   gggggggg" PageSize="4"
            Width="100%" AllowPaging="true" AutoPaging="true" OnDataChanging="OnDataChanging"
            OnDataChanged="OnDataChanged1" OnCellCreatingEditor="OnCellCreatingEditor" OnHeadCellCreating="OnHeadCellCreating"
            OnRowDelete="OnRowDelete" ShowEditBar="false" RowHeightWithFixeLines="30" AutoWidthOfNotFixeLines="true"
            OnSelectCheckboxCreated="OnSelectCheckboxCreated" ReadOnly="true">
            <Columns>
                <HB:ClientGridColumn SelectColumn="true" ShowSelectAll="true" ItemStyle="{width:'50px',height:'30'}" />
                <HB:ClientGridColumn DataField="index" HeaderText="行" SortExpression="index" DataType="Integer"
                    EditorStyle="{border: '1px solid #ccc',textAlign:'left',width:'20px'}" ItemStyle="{width:'100px',height:'30px'}"
                    IsFixedLine="false" />
                <HB:ClientGridColumn DataField="PaymentItem" HeaderText="费用项目" EditorTooltips="费用项目费用项目"
                    ItemStyle="{width:'200px',height:'30px'}" IsFixedLine="true">
                    <EditTemplate EditMode="DropdownList" TemplateControlID="DropDownList1" />
                </HB:ClientGridColumn>
                <HB:ClientGridColumn MaxLength="5" DataField="RMB" HeaderText="金额" DataType="Decimal"
                    EditorStyle="{width:'100px',border: '5px solid #ccc',textAlign:'right'}" FormatString="{0:N2}"
                    HeaderStyle="{width:'200px'}" ItemStyle="{width:'200px',height:'30px'}" IsFixedLine="false">
                    <EditTemplate EditMode="TextBox" />
                </HB:ClientGridColumn>
                <HB:ClientGridColumn DataField="INT" HeaderText="INT" DataType="Integer" HeaderStyle="{width:'250px'}"
                    ItemStyle="{width:'250px'}">
                    <EditTemplate EditMode="TextBox" />
                </HB:ClientGridColumn>
                <HB:ClientGridColumn DataField="Currency" HeaderText="币种" FormatString="{0:N2}" HeaderStyle="{width:'250px'}"
                    ItemStyle="{width:'250px'}">
                    <EditTemplate EditMode="DropdownList" TemplateControlID="ddlCurrencyList" />
                </HB:ClientGridColumn>
                <HB:ClientGridColumn DataField="ExchangeRate" HeaderText="汇率" DataType="Decimal"
                    SortExpression="ExchangeRate" FormatString="{0:N2}" EditorStyle="{textAlign:'left'}"
                    EditorTooltips="这是汇率" HeaderStyle="{width:'100px'}" ItemStyle="{width:'100px',height:'30px'}"
                    IsFixedLine="false">
                </HB:ClientGridColumn>
                <HB:ClientGridColumn DataField="Totle" HeaderText="金额（￥）" DataType="Decimal" FormatString="{0:N2}"
                    HeaderStyle="{width:'300px'}" ItemStyle="{width:'300px',height:'30px'}">
                </HB:ClientGridColumn>
                <HB:ClientGridColumn HeaderText="href" HeaderStyle="{width:'300px'}" ItemStyle="{width:'300px',height:'30px'}"
                    DataField="a">
                    <EditTemplate EditMode="A" DefaultTextOfA="fefefefefe" DefaultHrefOfA="http://www.baidu.com"
                        TargetOfA="_self" />
                </HB:ClientGridColumn>
            </Columns>
        </HB:ClientGrid>
        <br />
        <a target="_self" style="height: 30px"></a>
        <div>
            <asp:DropDownList ID="ddlCurrencyList" runat="server">
                <asp:ListItem Text="" Value=""></asp:ListItem>
                <asp:ListItem Text="美元" Value="6.50"></asp:ListItem>
                <asp:ListItem Text="日元" Value="0.17"></asp:ListItem>
                <asp:ListItem Text="韩元" Value="0.002"></asp:ListItem>
                <asp:ListItem Text="港币" Value="0.91"></asp:ListItem>
                <asp:ListItem Text="人民币" Value="1"></asp:ListItem>
                <asp:ListItem Text="英镑" Value="11.20"></asp:ListItem>
                <asp:ListItem Text="欧元" Value="8.20"></asp:ListItem>
            </asp:DropDownList>
        </div>
        <br />
        <input type="button" value="getvalue" onclick="showselectedData();" />
    </div>
    <div>
        <asp:Button runat="server" ID="postButton" Text="Post Back" OnClick="postButton_Click" />
    </div>
    <div>
        <asp:DropDownList ID="DropDownList1" runat="server">
            <asp:ListItem Text="华盛家园一期工程项目" Value="华盛家园一期工程项目"></asp:ListItem>
            <asp:ListItem Text="垂虹园二期项目" Value="垂虹园二期项目"></asp:ListItem>
        </asp:DropDownList>
    </div>
    <br />
    <br />
    </form>
</body>
</html>
