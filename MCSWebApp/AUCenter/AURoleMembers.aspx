<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AURoleMembers.aspx.cs"
    Inherits="AUCenter.AURoleMembers" %>

<%@ Register TagPrefix="mcs" Namespace="MCS.Web.WebControls" Assembly="MCS.Library.Passport" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>角色人员</title>
    <link href="Styles/dlg.css" rel="stylesheet" type="text/css" />
    <link href="Styles/pccom.css" rel="stylesheet" type="text/css" />
    <link href="Styles/aumain.css" rel="stylesheet" type="text/css" />
    <base target="_self" />
</head>
<body class="au-full">
    <form id="form1" runat="server">
    <au:SceneControl ID="SceneControl1" runat="server" />
    <div class="pcdlg-sky">
        <h1 class="pc-caption au-title-bar">
            <img src="Images/icon_01.gif" alt="图标" />
            <asp:Label runat="server" ID="schemaInfoLabel" />
            - 角色人员<span class="pc-timepointmark">
                <mcs:TimePointDisplayControl ID="tpdc" runat="server" />
            </span>
        </h1>
    </div>
    <div class="pcdlg-content">
        <asp:ScriptManager runat="server" ID="sm" EnablePageMethods="true" EnableScriptGlobalization="true" />
        <div class="au-progress" id="progress">
        </div>
        <div class="pc-listmenu-container">
        </div>
        <div class="pc-container5">
            <div class="pc-grid-container">
                <soa:ClientGrid runat="server" AutoBindOnLoad="false" AllowPaging="true" AutoPaging="true"
                    ID="grid" Width="100%" OnCellCreatedEditor="onCellCreatedEditor">
                    <Columns>
                        <soa:ClientGridColumn DataField="Name" HeaderText="角色名称" HeaderStyle="{width:'100px' }" ItemStyle="{ textAlign:'center' }" />
                        <soa:ClientGridColumn DataField="CodeName" HeaderText="角色代码名称" HeaderStyle="{width:'100px' }" ItemStyle="{ textAlign:'center' }" />
                        <soa:ClientGridColumn DataField="ID" HeaderText="矩阵" HeaderStyle="{width:'100px' }"
                            ItemStyle="{width:'100px', textAlign:'center'}">
                            <EditTemplate EditMode="A" DefaultTextOfA="矩阵" HrefFieldOfA="ID" />
                        </soa:ClientGridColumn>
                        <soa:ClientGridColumn DataField="Users" HeaderText="人员" HeaderStyle="{width:'auto'}">
                            <EditTemplate EditMode="OuUserInput" />
                        </soa:ClientGridColumn>
                    </Columns>
                </soa:ClientGrid>
            </div>
            <div style="display: none">
                <input type="hidden" runat="server" id="unitIDField" />
                <input type="hidden" runat="server" id="initialData" />
                <input type="hidden" runat="server" id="postData" />
                <asp:Button Text="submit" runat="server" ID="commitButton" OnClick="CommitClick" />
                <soa:PostProgressControl runat="server" ID="postControl" OnClientBeforeStart="doPrepare"
                    OnDoPostedData="postControl_DoPostedData" DialogTitle="角色人员操作" OnClientCompleted="doRefresh" />
                <mcs:AccessTicketHtmlAnchor runat="server" ID="ticketGenerator" href="#" OnClientAccquiredAccessTicket="onAccquiredTicket" />
            </div>
        </div>
    </div>
    <div class="pcdlg-floor">
        <div class="pcdlg-button-bar">
            <input type="button" value="保存修改" class="pcdlg-button" runat="server" id="btnSubmit"
                onclick="doSubmit();" />
            <input type="button" value="关闭" class="pcdlg-button" runat="server" onclick="window.close();" />
        </div>
    </div>
    <script type="text/javascript">
        $pc.ui.gridBehavior("gridMain", "hover");

        var initialData = null;

        function onCellCreatedEditor(s, e) {
            if (e.column.dataField == 'ID') {
                var elem = e.editor.get_editorElement();
                if (elem) {
                    elem.href = "javascript:void(0);";
                    elem.target = "_self";
                    $pc.setAttr(elem, "data-roleId", e.rowData['ID']);
                    $pc.setAttr(elem, "data-schemaRoleId", e.rowData['SchemaRoleID']);
                    $pc.bindEvent(elem, "click", $pc.createDelegate(elem, function (event) { assignTicket(this); }));
                }
            }
        }

        Sys.Application.add_load(function () {
            initialData = Sys.Serialization.JavaScriptSerializer.deserialize($get("initialData").value);
            $find("grid").set_dataSource(Sys.Serialization.JavaScriptSerializer.deserialize($get("initialData").value));
        });

        function doPrepare(e) {

            var dataSource = $find("grid").get_dataSource();
            var changeSet = [];
            for (var k = dataSource.length - 1; k >= 0; k--) {
                var target = dataSource[k];
                var src = initialData[k];
                var toBeAdded = [];
                var toBeDeleted = [];
                var i, j;
                var idArray = [];
                var srcIdArray = [];
                var a;
                var exists;
                for (i = target.Users.length - 1; i >= 0; i--) {
                    idArray.push(target.Users[i].id);
                }

                for (i = src.Users.length - 1; i >= 0; i--) {
                    srcIdArray.push(src.Users[i].id);
                }

                for (i = srcIdArray.length - 1; i >= 0; i--) {
                    a = srcIdArray[i];
                    exists = false;
                    for (j = idArray.length - 1; j >= 0; j--) {
                        if (idArray[j] == a) {
                            exists = true;
                            break;
                        }
                    }

                    if (!exists)
                        toBeDeleted.push(a);
                }

                for (i = idArray.length - 1; i >= 0; i--) {
                    a = idArray[i];
                    exists = false;
                    for (j = srcIdArray.length - 1; j >= 0; j--) {
                        if (srcIdArray[j] == a) {
                            exists = true;
                            break;
                        }
                    }

                    if (!exists)
                        toBeAdded.push(a);
                }

                if (toBeAdded.length > 0 || toBeDeleted.length > 0) {
                    for (var i = toBeAdded.length - 1; i >= 0; i--) {
                        changeSet.push({ RoleID: target.ID, SchemaRoleID: target.SchemaRoleID, UserID: toBeAdded[i], Type: 0 });
                    }

                    for (i = toBeDeleted.length - 1; i >= 0; i--) {
                        changeSet.push({ RoleID: target.ID, SchemaRoleID: target.SchemaRoleID, UserID: toBeDeleted[i], Type: 1 });
                    }
                }
            }

            if (changeSet.length) {
                e.clientExtraPostedData = $get("unitIDField").value;
                e.steps = changeSet;
            } else {
                alert("没有对人员进行任何修改，无需提交");
                e.cancel = true;
            }
        }

        function doRefresh() {
            window.close();
        }

        function doSubmit() {
            $find("postControl").showDialog();
        }

        function assignTicket(a) {
            var link = $get("ticketGenerator");
            link.href = document.location.protocol + "//" + document.location.hostname + "/MCSWebApp/WorkflowDesigner/MatrixModalDialog/RolePropertyExtension.aspx?RoleID=" + $pc.getAttr(a, "data-roleId") + "&editMode=normal&definitionID=" + $pc.getAttr(a, "data-schemaRoleId");
            link.click();
            return false;
        }

        function onAccquiredTicket(a) {
            return $pc.modalPopup(a, 460, 240);
        }
        
    </script>
    </form>
</body>
</html>
