<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MatrixDimensionDefinitionEditor.aspx.cs"
    Inherits="WorkflowDesigner.MatrixModalDialog.MatrixDimensionDefinitionEditor" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <base target="_self">
    <title>矩阵维度编辑</title>
    <script type="text/javascript" src="../js/wfweb.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true">
    </asp:ScriptManager>
    <table width="100%" style="height: 100%; width: 100%">
        <tr>
            <td style="vertical-align: center">
                <div class="dialogContent" id="dialogContent" runat="server" style="width: 100%;
                    height: 100%; overflow: auto">
                    <table width="100%" style="width: 100%">
                        <tr>
                            <td class="gridHead">
                                <div class="dialogTitle">
                                    <span class="dialogLogo">矩阵定义信息</span>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <tr>
                                            <td style="width: 80px">
                                                矩阵定义Key
                                            </td>
                                            <td>
                                                <asp:TextBox runat="server" ID="txtMatrixKey" onkeypress="onKeyPress();" onchange="checkKey();" />&nbsp;&nbsp;矩阵Key不能含有空格
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                矩阵定义名称
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtmatrixName" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                矩阵定义描述
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMatrixDesc" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                是否启用
                                            </td>
                                            <td>
                                                <select runat="server" id="ddlEnabled">
                                                    <option value="true">是</option>
                                                    <option value="false">否</option>
                                                </select>
                                            </td>
                                        </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <table width="100%" style="width: 100%">
                        <tr>
                            <td class="gridHead">
                                <div class="dialogTitle">
                                    <span class="dialogLogo">矩阵维度定义</span>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <SOA:ClientGrid runat="server" OnCellCreatingEditor="rowCreatingEditor" ID="detailGrid"
                                    Caption="" ShowEditBar="true" Width="100%">
                                    <Columns>
                                        <SOA:ClientGridColumn SelectColumn="true" ShowSelectAll="true" ItemStyle="{width:'50px',TEXT-ALIGN: 'center' }"
                                            HeaderStyle="{width:'50px'}" />
                                        <SOA:ClientGridColumn DataField="Name" HeaderText="名称" DataType="String">
                                            <EditTemplate EditMode="TextBox" />
                                        </SOA:ClientGridColumn>
                                        <SOA:ClientGridColumn DataField="DataType" HeaderText="数据类型" HeaderStyle="{width:'160px'}"
                                            ItemStyle="{width:'160px'}">
                                            <EditTemplate EditMode="DropdownList" TemplateControlID="dataTypeDropDownList" />
                                        </SOA:ClientGridColumn>
                                        <SOA:ClientGridColumn DataField="DimensionKey" HeaderText="维度Key" DataType="String"
                                            HeaderStyle="{width:'160px'}" ItemStyle="{width:'160px'}">
                                            <EditTemplate EditMode="TextBox" />
                                        </SOA:ClientGridColumn>
                                        <SOA:ClientGridColumn DataField="SortOrder" HeaderText="排序依据" DataType="Integer"
                                            HeaderStyle="{width:'160px'}" ItemStyle="{width:'160px'}">
                                            <EditTemplate EditMode="TextBox" />
                                        </SOA:ClientGridColumn>
                                        <SOA:ClientGridColumn DataField="Description" HeaderText="描述" HeaderStyle="{width:'160px'}"
                                            ItemStyle="{width:'160px'}">
                                            <EditTemplate EditMode="TextBox" />
                                        </SOA:ClientGridColumn>
                                    </Columns>
                                </SOA:ClientGrid>
                            </td>
                        </tr>
                    </table>
                    <div runat="server" style="display: none">
                        <asp:DropDownList ID="dataTypeDropDownList" runat="server" />
                    </div>
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
                        <td style="text-align: center;">
                            <input id="btnOK" type="button" value="确定(O)" accesskey="O" onclick="onClick();"
                                class="formButton" />
                            <SOA:SubmitButton runat="server" ID="btnConfirm" Style="display: none" OnClick="btnConfirm_Click"
                                RelativeControlID="btnOK" PopupCaption="正在保存..." />
                        </td>
                        <td style="text-align: center;">
                            <input id="btnCancel" type="button" value="取消(C)" accesskey="C" onclick="top.close();"
                                class="formButton" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <script type="text/javascript">
        var dimensionKey = 0;
        function onDocumentLoad() {
            if(document.getElementById("hiddenMatrixDimDefJsonData").value != "") {
                var matrixDef = 
                    Sys.Serialization.JavaScriptSerializer.deserialize(document.getElementById("hiddenMatrixDimDefJsonData").value);
                $get("txtMatrixKey").value = matrixDef.Key;
                $get("txtMatrixKey").disabled = true;
                $get("hiddenMatrixDefKey").value = matrixDef.Key;
                $get("txtmatrixName").value = matrixDef.Name;
                $get("txtMatrixDesc").value = matrixDef.Description;
                $find("detailGrid").set_autoBindOnLoad(false);
				$find("detailGrid").set_dataSource(matrixDef.Dimensions);
            }
        }
        function onClick() {
			if(checkKey()){
				$get("btnConfirm").click();
			}
        }
        function onKeyPress() {
             if (event.keyCode<33 || event.keyCode>126) { 
                event.returnValue = false; 
            }
        }
        function checkKey() {
			var txtKeyControl = $get('txtMatrixKey');
			var key = txtKeyControl.value;
			if (!WFWeb.Utils.checkInputKey(key)) {
            	alert('Key只能由字母、数字、下划线组成');
				txtKeyControl.focus();
            	return false;
            }
            
            if (key != "") {
                $get("btnOK").disabled = true;
                CallServer(key,"");
            }
			return true;
        }

        function CallServer(arg,context) {
            <%= ClientScript.GetCallbackEventReference(this, "arg", "ReceiveServerData", "context") %>
        }
        function ReceiveServerData(result, context) {
            if (result == false) {
                alert("矩阵key已存在，请重新输入。");
            }else {
                $get("btnOK").disabled = false;
            }
        }

        function rowCreatingEditor(clientGrid, e) {
            switch (e.column.dataField) {
                case "DimensionKey":
                if (e.rowData.DimensionKey) {
                    e.valueTobeChange = e.rowData.DimensionKey;
                    return;
                }
                dimensionKey++;
                e.valueTobeChange = dimensionKey;
                break;

            }
        }
    </script>
    <input type="hidden" id="hiddenMatrixDimDefJsonData" runat="server" />
    <input type="hidden" id="hiddenMatrixDefKey" runat="server" />
    </form>
</body>
<script type="text/javascript">
    Sys.Application.add_load(function () {
        onDocumentLoad();
    });
</script>
</html>
