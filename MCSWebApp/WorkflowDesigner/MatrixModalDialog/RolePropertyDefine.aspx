<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RolePropertyDefine.aspx.cs"
    Inherits="MCS.Applications.AppAdmin.Dialogs.RolePropertyDefine" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <base target="_self" />
    <title>角色扩展属性编辑</title>
    <script type="text/javascript">
        function BindForm() {
            var m_objParam = window.dialogArguments;
            document.getElementById("txtRoleID").value = m_objParam["Role_Name"];
            document.getElementById("txtRoleName").value = m_objParam["Role_CodeName"];
            document.getElementById("txtRoleDescription").value = m_objParam["Role_Description"] || '';
        }
    </script>
</head>
<body onload="BindForm()">
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
                                    <span class="dialogLogo">角色信息</span>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td style="width: 80px">
                                            角色ID
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtRoleID" ReadOnly="true" />&nbsp;&nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 80px">
                                            角色名称
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtRoleName" ReadOnly="true" />&nbsp;&nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 80px">
                                            角色描述
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtRoleDescription" ReadOnly="true" />&nbsp;&nbsp;
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
                                    <span class="dialogLogo">角色扩展属性定义</span>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <SOA:ClientGrid runat="server" OnCellCreatingEditor="rowCreatingEditor" ID="detailGrid"
                                    OnCellCreatedEditor="OnCellCreatedEditor" Caption="" ShowEditBar="true" Width="100%"
                                    OnBeforeSaveClientState="OnBeforeSaveClientState">
                                    <Columns>
                                        <SOA:ClientGridColumn SelectColumn="true" ShowSelectAll="true" ItemStyle="{width:'50px',TEXT-ALIGN: 'center' }"
                                            HeaderStyle="{width:'50px'}" />
                                        <SOA:ClientGridColumn DataField="Name" HeaderText="名称" DataType="String" HeaderStyle="{width:'160px'}"
                                            ItemStyle="{width:'160px'}">
                                            <EditTemplate EditMode="TextBox" />
                                        </SOA:ClientGridColumn>
                                        <SOA:ClientGridColumn DataField="DataType" HeaderText="数据类型" HeaderStyle="{width:'160px'}"
                                            ItemStyle="{width:'160px'}">
                                            <EditTemplate EditMode="DropdownList" TemplateControlID="dataTypeDropDownList" />
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
                    <div id="Div1" runat="server" style="display: none">
                        <asp:DropDownList ID="dataTypeDropDownList" runat="server" />
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td class="gridfileBottom">
            </td>
        </tr>
        <tr runat="server" id="under">
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

        function onClick() {
            if (CheckData()) {
                $get("btnConfirm").click();
            }
        }
        function onKeyPress() {
            if (event.keyCode < 33 || event.keyCode > 126) {
                event.returnValue = false;
            }
        }
        function checkKey(inputKey) {
            var key = inputKey.value;
            if (key != "") {
                $get("btnOK").disabled = true;
                CallServer(key, "");
            }
        }


        function ReceiveServerData(result, context) {
            if (result == false) {
                alert("矩阵key已存在，请重新输入。");
            } else {
                $get("btnOK").disabled = false;
            }
        }

        function rowCreatingEditor(clientGrid, e) {
            switch (e.column.dataField) {
                case "SortOrder":
                    e.valueTobeChange = dimensionKey++;
                    break;
            }
        }
        
        function CallServer(arg,context) {
            <%= ClientScript.GetCallbackEventReference(this, "arg", "ReceiveServerData", "context") %>
        }

        function OnCellCreatedEditor(grid, e) {
            switch (e.column.dataField) {
                case "DataType":
                    //setTimeout("SetDefaultValue(e.editor.get_editorElement())",100);   
                    break;
            }
        }

        function SetDefaultValue(item) {

            item.value = "6";
        }

        function OnBeforeSaveClientState(grid, e) {
            var dataSource = e.dataSource;
            var keys = new Array();
            for (var i = 0; i < dataSource.length; i++) {

                if (dataSource[i].Name == "") {
                    dataSource[i].Name = dataSource[i].Description;
                }

                if (ContainKey(keys, dataSource[i].Name)) {
                    dataSource[i].Name = dataSource[i].Name + dataSource[i].SortOrder;
                }

                keys.push(dataSource[i].Name);
            }
        }

        function CheckData() {
            var dataSource=$find("detailGrid").get_dataSource();
            for (var i = 0; i < dataSource.length; i++) {
                if (dataSource[i].Name == "") {
                    alert("名称不能为空！");
                    return false;
                }
                else {
                    var regex = new RegExp("^(\\d+).*");
                    if (regex.test(dataSource[i].Name)) {
                        alert("名称首字母不能为数字！");
                        return false;
                    }
                }
            }
            return true;
        }

        function ContainKey(arr, key) {
            for (var i = 0; i < arr.length; i++) {
                if (arr[i] == key) {
                    return true;
                }
            }
            return false;
        }
    </script>
    <input type="hidden" id="hiddenMatrixDimDefJsonData" runat="server" />
    <input type="hidden" id="hiddenMatrixDefKey" runat="server" />
    </form>
</body>
</html>
