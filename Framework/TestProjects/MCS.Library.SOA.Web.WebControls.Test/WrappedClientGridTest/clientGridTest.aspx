<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="clientGridTest.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest.clientGridTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>客户端Grid测试</title>
    <style type="text/css">
        .header
        {
            background-color: Silver;
            color: White;
        }
    </style>
    <script type="text/javascript">
        function onCellDataBound(gridControl, e) {
            var cell = e.cell;
            var dataField = e.column.dataField;
            var data = e.data;
            switch (dataField) {
                case "Amount":
                    cell.innerText = String.format("{0:N}", e.data["Amount"]);
                    break;

                case "InvoiceDate":
                    var date = data["InvoiceDate"];
                    cell.innerText = String.format("{0:yyyy-MM-dd}", date);
                    break;

                case "updateCommand":
                    e.gridControl = gridControl;
                    var delBtn = $HGDomElement.createElementFromTemplate(
					{
					    nodeName: "input",
					    properties:
						{
						    type: "button",
						    value: "Edit..."
						},
					    events:
						{
						    click: Function.createDelegate(e, onGridEditBtnClick)
						}
					}, cell);

                    break;
            }
        }

        function onDeleteInvoices() {
            event.returnValue = false;

            var gridCtrl = $find("clientGrid");
            var selectedData = gridCtrl.get_selectedData();

            if (selectedData.length > 0) {
                if (window.confirm("Do you want to delete the selected data ?")) {
                    syncDeletedData(selectedData);
                }
            }
            return false;
        }

        function onGridEditBtnClick() {
            var selectedData = this.data;

            onEditInvoice(selectedData);
        }

        function onEditInvoice(selectedData) {
            var sFeature = "dialogWidth: 420px; dialogHeight: 250px; edge: Raised; center: Yes; help: No; status: No;";

            var result = window.showModalDialog("editInvoice.aspx", selectedData, sFeature);

            if (result) {
                var gridCtrl = $find("clientGrid");

                var invoice = Sys.Serialization.JavaScriptSerializer.deserialize(result)

                var index = findInvoiceIndexByNumber(invoice);

                if (index == -1)
                    gridCtrl.get_dataSource().push(invoice);
                else
                    gridCtrl.get_dataSource()[index] = invoice;

                gridCtrl.set_dataSource(gridCtrl.get_dataSource());
            }
        }

        function syncDeletedData(selectedData) {
            var gridCtrl = $find("clientGrid");
            for (var i = 0; i < selectedData.length; i++) {
                Array.remove(gridCtrl.get_dataSource(), selectedData[i]);
            }
            gridCtrl.set_dataSource(gridCtrl.get_dataSource());
        }

        function findInvoiceIndexByNumber(invoice) {
            var gridCtrl = $find("clientGrid");

            var result = -1;

            for (var i = 0; i < gridCtrl.get_dataSource().length; i++) {
                if (gridCtrl.get_dataSource()[i].InvoiceNo == invoice.InvoiceNo) {
                    result = i;
                    break;
                }
            }

            return result;
        }
    </script>
    <script type="text/javascript">
        function onCellDataBound1(gridControl, e) {
            var cell = e.cell;
            var dataField = e.column.dataField;
            var data = e.data;
            switch (dataField) {
                case "WorkItemID":
                    cell.innerText = data["WorkItemID"];
                    break;
                case "WorkItemName":
                    //cell.innerText = data["WorkItemName"];
                    break;
                case "EnabledEdit":
                    //var select = $HGDomElement.createElementFromTemplate({
                    //    nodeName: "select",
                    //    properties: {
                    //        option: {
                    //            text: "true",
                    //            value: "true"
                    //        }
                    //    }
                    //}, cell);

                    var selectHTML = "<select change='selectItemChange(this)' dataField='EnabledEdit'><option value='true'>true</option><option value='false'>false</option></select>";
                    cell.innerHTML = selectHTML;
                    //debugger;
                    //cell.innerText = data["EnabledEdit"];
                    break;
                case "ProcessID":
                    cell.innerText = data["ProcessID"];
                    break;
            }
        }
    </script>
    <script type="text/javascript">
        //获取选中项数据
        function showselectedData() {
            var grid = $find("clientGrid1");

            var selecteddata = grid.get_selectedData();

            alert($Serializer.serialize(grid.get_selectedData()));
            //debugger;
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

        function selectItemChange(selectObj) {

        }
    </script>
</head>
<body>
    <form id="serverForm" runat="server">
    <div>
        <a href="#" onclick="onEditInvoice();">Add...</a>&nbsp;&nbsp;<a href="#" onclick="onDeleteInvoices();">&nbsp;&nbsp;
            Delete...</a>
    </div>
    <div>
        <HB:ClientGrid runat="server" ID="clientGrid" Caption="Test" Width="100%" OnClientCellDataBound="onCellDataBound">
            <Columns>
                <HB:ClientGridColumn SelectColumn="true" ShowSelectAll="true" />
                <HB:ClientGridColumn DataField="InvoiceNo" HeaderText="Invoice Number" SortExpression="InvoiceNo"
                    HeaderStyle="{ width: '120px'}" />
                <HB:ClientGridColumn DataField="VendorName" HeaderText="Vendor Name" SortExpression="VendorName" />
                <HB:ClientGridColumn DataField="InvoiceDate" HeaderText="InvoiceDate" SortExpression="InvoiceDate" />
                <HB:ClientGridColumn DataField="Amount" HeaderText="Amount" ItemStyle="{ width: '80px', textAlign: 'right' }" />
                <HB:ClientGridColumn DataField="updateCommand" ItemStyle="{width: '50px'}" />
            </Columns>
        </HB:ClientGrid>
    </div>
    <br />
    <div>
        <a href="#" onclick="onAddNewRow();">Add...</a>&nbsp;&nbsp;<%--<a href="#" onclick="onRowEdit();">Edit...</a>&nbsp;&nbsp;--%>
        <a href="#" onclick="onDeleteInvoices2();">Delete...</a>
    </div>
    <div>
        <HB:ClientGrid runat="server" ID="clientGrid1" Caption="这里是标题：workitem" Width="100%"
            AllowPaging="true" AutoPaging="true" PageSize="10" OnClientCellDataBound="onCellDataBound1">
            <Columns>
                <HB:ClientGridColumn SelectColumn="true" ShowSelectAll="true" />
                <HB:ClientGridColumn DataField="WorkItemID" HeaderText="WORKITEM_ID" SortExpression="WorkItemID" />
                <HB:ClientGridColumn DataField="WorkItemName" HeaderText="WORKITEM_NAME" EditorStyle="{backgroundColor:'#eee'}">
                    <EditTemplate EditMode="TextBox" />
                </HB:ClientGridColumn>
                <HB:ClientGridColumn DataField="EnabledEdit" HeaderText="ENABLED_EDIT" />
                <HB:ClientGridColumn DataField="ProcessID" HeaderText="PROCESS_ID" EditorStyle="{backgroundColor:'red'}" />
            </Columns>
        </HB:ClientGrid>
        <br />
        <input type="button" value="getvalue" onclick="showselectedData();" />
    </div>
    <br />
    <br />
    <div>
        <table id="grid" style="width: 100%; height: 100%">
        </table>
    </div>
    <div>
        <asp:Button runat="server" ID="postBackBtn" Text="Post Back" />
    </div>
    </form>
    <script type="text/javascript">
        /*
        Sys.Application.add_load(function() {
        var gridCtrl = $find("clientGrid");
        gridCtrl.set_columns = [{ selectColumn: true, showSelectAll: true, headerStyle: { width: "30px"} },
        { dataField: "InvoiceNo", headerText: "Invoice Number", sortExpression: "InvoiceNo", headerStyle: { width: "120px"} },
        { dataField: "VendorName", headerText: "Vendor Name", sortExpression: "VendorName" },
        { dataField: "InvoiceDate", headerText: "InvoiceDate", sortExpression: "InvoiceDate" },
        { dataField: "Amount", headerText: "Amount", itemStyle: { width: "80px", textAlign: "right" }, cellDataBound: function(grid, e) { e.cell.innerText = String.format("{0:N}", e.data["Amount"]) } },
        { dataField: "updateCommand", headerText: "", itemStyle: { width: "50px", textAlign: "center"} }
        ];
        gridCtrl.cellPadding = "3px";
        gridCtrl.set_keyFields = ["InvoiceNo"];
        //m_invoices = Sys.Serialization.JavaScriptSerializer.deserialize($get("invoiceHidden").value);
        var data = [];
        gridCtrl.set_dataSource(data);
        });
        */
    </script>
</body>
</html>
