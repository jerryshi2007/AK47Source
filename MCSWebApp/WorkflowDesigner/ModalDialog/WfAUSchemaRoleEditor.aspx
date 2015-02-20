<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WfAUSchemaRoleEditor.aspx.cs"
    Inherits="WorkflowDesigner.ModalDialog.WfAUSchemaRoleEditor" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="soa" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="mcs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>管理架构角色选择</title>
    <base target="_self" />
    <link href="../css/dlg.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .center
        {
            text-align: center;
        }
        
        .itemheader
        {
            height: 15px;
            padding: 5px 0 2px;
            line-height: 10px;
        }
    </style>
</head>
<body class="pcdlg">
    <form id="form1" runat="server" onkeydown="handleKey(event);">
    <div class="pcdlg-sky">
        <div class="dialogTitle">
            <span class="dialogLogo">管理架构角色编辑</span>
        </div>
    </div>
    <div class="pcdlg-content">
        <div style="margin: 25px 25px; line-height: 20px;">
            <div class="itemheader">
                管理架构角色
            </div>
            <div style="display: none">
                <mcs:DropDownBox runat="server" ID="ddd">
                </mcs:DropDownBox>
            </div>
            <div>
                <div id="dropDownContainer" class="mcsc-drop-sorter">
                    <ul>
                        <li><span id="selectedRoleName"></span><s class="mcsc-seldown" style="cursor: pointer"
                            id="dropHandle"><s class="mcsc-arrow"></s></s></li>
                    </ul>
                    <div class="mcsc-drop-container" style="width: 100%; height: 150px; overflow: auto;">
                        <input type="hidden" id="selectedRoleID" />
                        <input type="hidden" id="selectedRoleJson" />
                        <div style="padding: 5px;">
                            <mcs:DeluxeTree runat="server" ID="tree" OnGetChildrenData="tree_GetChildrenData"
                                OnNodeSelecting="onSelectNode">
                            </mcs:DeluxeTree>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="pcdlg-floor">
        <div class="pcdlg-button-bar">
            <input type="button" onclick="onOkClick();" value="确定(O)" id="btnOK" accesskey="O"
                disabled="disabled" runat="server" class="formButton" />
            <input id="btnCancel" type="button" value="取消(C)" accesskey="C" onclick="top.close();"
                class="formButton" />
        </div>
    </div>
    </form>
    <script type="text/javascript">
        function onSelectNode(s, e) {
            if (e.node.get_extendedDataKey() == 'AUSchemaRoles') {
                Sys.UI.DomElement.toggleCssClass($get("dropDownContainer"), "hover");
                $get("selectedRoleID").value = e.node.get_value();
                $get("selectedRoleName").innerHTML = '';
                $get("selectedRoleName").appendChild(document.createTextNode(e.node.get_text()));

                $get("selectedRoleJson").value = e.node.get_extendedData();
                $get("btnOK").disabled = false;
            }
        }

        $addHandler($get("dropHandle"), "click", function () {
            Sys.UI.DomElement.toggleCssClass($get("dropDownContainer"), "hover");
        });

        function onOkClick() {
            var result = $get("selectedRoleJson").value;
            if (result.length) {
                window.returnValue = result;
                window.close();
            }
        }

        function handleKey(e) {
            if (e.ctrlKey && e.shiftKey && e.keyCode == 67) {
                if (confirm("要进行调试吗?")) {
                    throw new Error("开始调试中断");
                }
            }
        }
    
    </script>
</body>
</html>
