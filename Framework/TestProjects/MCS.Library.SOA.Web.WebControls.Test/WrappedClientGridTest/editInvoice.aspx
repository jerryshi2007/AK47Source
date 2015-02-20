<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="editInvoice.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest.editInvoice" %>

<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="CCIC" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edit Invoice</title>
    <script type="text/javascript">
        function onConfirm() {
            var result = {
                InvoiceNo: $get("InvoiceNo").value,
                VendorName: $get("VendorName").value,
                InvoiceDate: $find("InvoiceDate").get_value(),
                Amount: parseFloat($get("Amount").value)
            };

            window.returnValue = Sys.Serialization.JavaScriptSerializer.serialize(result);
            window.close();
        }

        function pageLoad() {
            var selectedData = window.dialogArguments;

            if (selectedData) {
                $get("InvoiceNo").value = selectedData.InvoiceNo;
                $get("VendorName").value = selectedData.VendorName;
                $find("InvoiceDate").set_value(selectedData.InvoiceDate);
                $get("Amount").value = selectedData.Amount;
            }
        }
    </script>
</head>
<body>
    <form id="serverForm1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true">
            <Scripts>
            </Scripts>
        </asp:ScriptManager>
    </div>
    <div>
        <table style="width: 100%; height: 100%">
            <tr>
                <td style="height: 36px">
                    <strong style="font-size: 24px">Invoice Detail</strong>
                    <hr />
                </td>
            </tr>
            <tr>
                <td>
                    <table style="width: 100%;">
                        <tr>
                            <td style="width: 120px">
                                Invoice Number:
                            </td>
                            <td>
                                <input type="text" id="InvoiceNo" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Vendor:
                            </td>
                            <td>
                                <input type="text" id="VendorName" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Invoice Date:
                            </td>
                            <td>
                                <CCIC:DeluxeCalendar ID="InvoiceDate" runat="server">
                                </CCIC:DeluxeCalendar>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Amount:
                            </td>
                            <td>
                                <input type="text" id="Amount" style="text-align: right" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="height: 2px">
                    <hr />
                </td>
            </tr>
            <tr>
                <td style="height: 32px; text-align: center">
                    <table style="width: 100%">
                        <tr>
                            <td style="text-align: center">
                                <input type="button" value="OK" accesskey="O" style="width: 64px" onclick="onConfirm();" />
                            </td>
                            <td style="text-align: center">
                                <input type="button" value="Cancel" accesskey="C" style="width: 64px" onclick="window.close();" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
