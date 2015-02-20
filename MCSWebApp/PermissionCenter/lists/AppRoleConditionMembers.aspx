<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AppRoleConditionMembers.aspx.cs"
    Inherits="PermissionCenter.AppRoleConditionMembers" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" scroll="no" style="overflow: hidden">
<head runat="server">
    <title>权限中心-角色条件人员</title>
    <link rel="icon" href="../favicon.ico" type="image/x-icon" />
    <link rel="Shortcut Icon" href="../favicon.ico" />
    <base target="_self" />
    <pc:HeaderControl ID="HeaderControl1" runat="server">
    </pc:HeaderControl>
    <style type="text/css">
        .pc-actions
        {
            display: none;
            width: 100px;
            height: 100%;
        }
        .hoveringItem .pc-actions
        {
            display: block;
        }
        .pc-actions .pc-cmd
        {
            display: block;
            float: right;
            width: 16px;
            height: 16px;
            margin: 4px 4px;
            background: transparent url('../images/ui-icons_888888_256x240.png') no-repeat scroll;
        }
        .pc-actions .pc-cmd.pc-delete
        {
            background-position: -32px -192px;
        }
        .pc-actions .pc-cmd.pc-check
        {
            background-position: -208px -192px;
        }
        
        .pc-actions .pc-cmd.pc-edit
        {
            background-position: -64px -96px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <pc:SceneControl ID="SceneControl1" runat="server" />
    <soa:DataBindingControl runat="server" ID="binding1">
        <ItemBindings>
            <soa:DataBindingItem DataPropertyName="RoleID" ControlID="hfID" ControlPropertyName="Value"
                Direction="DataToControl" />
            <soa:DataBindingItem DataPropertyName="VisibleName" ControlID="roleName" ControlPropertyName="Text"
                Direction="DataToControl" />
            <soa:DataBindingItem DataPropertyName="RoleID" ControlID="hfGroupId" ControlPropertyName="Value"
                Direction="DataToControl" />
            <soa:DataBindingItem DataPropertyName="RoleID" ControlID="lnkDynamic" ControlPropertyName="NavigateUrl"
                Format="~/lists/AppRoleConditionMembers.aspx?role={0}" Direction="DataToControl" />
            <soa:DataBindingItem DataPropertyName="RoleID" ControlID="lnkConst" ControlPropertyName="NavigateUrl"
                Format="~/lists/AppRoleMembers.aspx?role={0}" Direction="DataToControl" />
            <soa:DataBindingItem DataPropertyName="RoleID" ControlID="lnkPreview" ControlPropertyName="NavigateUrl"
                Format="~/lists/AppRoleMembersView.aspx?role={0}" Direction="DataToControl" />
        </ItemBindings>
    </soa:DataBindingControl>
    <asp:ScriptManager ID="scriptManager" runat="server" EnableScriptGlobalization="true">
        <Services>
            <asp:ServiceReference Path="~/Services/ConditionSvc.asmx" />
        </Services>
    </asp:ScriptManager>
    <div class="pc-banner">
        <h1 class="pc-caption">
            角色成员-<asp:Literal ID="roleName" runat="server" Mode="Encode"></asp:Literal><span
                style="float: right">
                <soa:RoleMatrixEntryControl ID="roleMatrixEntryControl" runat="server" Visible="true"
                    EnableAccessTicket="true" />
            </span><span class="pc-timepointmark">
                <mcs:TimePointDisplayControl ID="TimePointDisplayControl1" runat="server">
                </mcs:TimePointDisplayControl>
            </span>
        </h1>
    </div>
    <pc:BannerNotice ID="notice" runat="server"></pc:BannerNotice>
    <ul class="pc-tabs-header">
        <li>
            <asp:HyperLink ID="lnkConst" runat="server" Text="固定成员" />
        </li>
        <li class="pc-active">
            <asp:HyperLink ID="lnkDynamic" runat="server" Text="条件成员" />
        </li>
        <li>
            <asp:HyperLink ID="lnkPreview" runat="server" Text="预览成员" />
        </li>
    </ul>
    <div class="pc-frame-container">
        <asp:HiddenField ID="hfGroupId" runat="server" />
        <div class="pc-container5">
            <div id="panPrompt">
                <span class="pc-required">
                    <asp:Literal ID="msg" runat="server" EnableViewState="False" Mode="Encode" ViewStateMode="Disabled"></asp:Literal></span>
                <div style="display: none">
                    <asp:Button runat="server" ID="btnRecalc" OnClick="Nop" />
                    <soa:PostProgressControl runat="server" ID="calcProgress" OnClientBeforeStart="onPrepareData"
                        OnClientCompleted="postProcess" ControlIDToShowDialog="btnRecalc" OnDoPostedData="ProcessCaculating"
                        DialogHeaderText="正在计算..." DialogTitle="计算进度" />
                </div>
            </div>
            <div class="pc-hide" id="panExec" style="background: #efefef">
                <div>
                    在按保存按钮之前，对列表的修改并不会被保存。
                </div>
                <asp:Button runat="server" Text="保存" ID="btnSave" OnClick="SaveClick" CssClass="pc-button"
                    OnClientClick="doSubmit();" />
                <asp:CheckBox Text="保存后自动计算动态人员" runat="server" ID="chkAutoCalc" ViewStateMode="Disabled" />
            </div>
            <div class="pc-hide" id="panWait">
                <div class="pc-icon-loader" style="float: left">
                </div>
                请稍候……
            </div>
            <div class="pc-grid-container">
                <soa:ClientGrid runat="server" ID="gridMain" AutoBindOnLoad="True" Caption="条件列表"
                    OnCellCreatedEditor="handleCellCreating" Width="100%">
                    <Columns>
                        <soa:ClientGridColumn SelectColumn="true" ItemStyle="{width:'10px'}" HeaderStyle="{width:'10px'}">
                        </soa:ClientGridColumn>
                        <soa:ClientGridColumn DataField="Description" HeaderText="描述" HeaderStyle="{width:'200px'}">
                        </soa:ClientGridColumn>
                        <soa:ClientGridColumn HeaderText="" DataField="OwnerID" DataType="String" HeaderStyle="{width:'100px'}">
                        </soa:ClientGridColumn>
                        <soa:ClientGridColumn DataField="Condition" HeaderText="条件表达式">
                        </soa:ClientGridColumn>
                    </Columns>
                </soa:ClientGrid>
            </div>
        </div>
        <div style="display: none">
            <asp:HiddenField ID="actionData" runat="server" EnableViewState="False" />
            <input type="hidden" runat="server" id="hfID" />
        </div>
        <pc:Footer ID="footer" runat="server" />
    </div>
    <div class="pc-overlay-mask pc-hide" id="overlay">
    </div>
    <%--  <div class="pc-overlay-panel pc-hide" id="panExp">
            <div style="width: 500px; height: 300px; text-align: center; vertical-align: middle;
                margin: auto; margin-top: 30px;">
                <iframe id="winExp" src="../dialogs/ExpressionEditor.aspx" style="width: 100%; height: 100%">
                </iframe>
            </div>
        </div>--%>
    </form>
    <script type="text/javascript">
        $pc.ui.traceWindowWidth();

        function doSubmit() {
            $pc.removeClass("panWait", "pc-hide");
            $pc.addClass("panExec", "pc-hide");
        }

        var service = new PermissionCenter.Services.ConditionSvc();

        var context = null;
        var editorLoaded = false;
        var wacher = {
            doOk: function () {
                if (context)
                    context.handleOk();
            },
            doCancel: function () {
                if (context)
                    context.handleCancel();
            },
            doCheck: function () {
                if (context)
                    context.handleCheck();
            }, doUnloadDialog: function () {
                hideMask();
            }, getSchemaType: function () {
                return "Roles";
            }
        };

        function editContext(rowData, properties) {
            if (rowData) {
                this._data = rowData;
            }
            if (properties) {
                for (var p in properties) {
                    if (p === 'beforeShowEditor')
                        this.beforeShowEditor = properties[p];
                    else if (p === 'handleOk')
                        this.handleOk = properties[p];
                    else if (p === 'handleCancel')
                        this.handleCancel = properties[p];
                    else if (p === 'handleCheck')
                        this.handleCheck = properties[p];
                }

            }
        }

        editContext.prototype = {
            beforeShowEditor: function () {
                if (editorLoaded) {
                    var win = getEditorWindow();
                    if (win) {
                        win.setMessage("");
                        win.setData("", "");
                        win.clearCheckResult();
                    }
                    win = null;

                    return true;
                } else {
                    return false;
                }

            }, handleOk: function () {
                hideEditor();
                $pc.addClass("panPrompt", "pc-hide");
                $pc.removeClass("panExec", "pc-hide");
                $pc.addClass("panWait", "pc-hide");

            }, handleCancel: function () {
                hideEditor();

            }, handleCheck: function () {
                if (editorLoaded) {
                    var win = getEditorWindow();
                    if (win) {
                        service.ValidateExpression(win.getData().exp, function (data) {
                            getEditorWindow().setCheckResult(data);
                        }, function (err) {
                            getEditorWindow().setCheckResult(false);
                        }, null);
                    }
                    win = null;

                    return true;
                } else {
                    return false;
                }

            }

        }

        var contextNew = new editContext(null, {
            handleOk: function () {
                var data = getEditorWindow().getData();
                var rowData = {
                    Condition: data.exp,
                    Description: data.desc,
                    OwnerID: null,
                    rowIndex: -1,
                    SortID: 0,
                    Type: '',
                    VersionEndTime: new Date(),
                    VersionStartTime: new Date()
                };
                var grid = $find("gridMain");
                var src = grid.get_dataSource();
                src.push(rowData);
                grid.set_dataSource(src);
                hideEditor();
                $pc.addClass("panPrompt", "pc-hide");
                $pc.removeClass("panExec", "pc-hide");
                $pc.addClass("panWait", "pc-hide");

            }
        });

        function initExpressionEditor(win) {
            win.registerAdapter(wacher);
            editorLoaded = true;
        }

        function showEditor() {
            if (editorLoaded) {
                // $get("overlay").style.display = "block";
                //$get("panExp").style.display = "block";
            }

        }

        function hideEditor() {
            if (dialogAgent.enabled) {

                dialogAgent.clientWindow.close();
                dialogAgent.clientWindow = null;

            } else if (editorLoaded) {
                hideMask();
                //$get("panExp").style.display = "none";
            }
        }

        function hideMask() {
            // $get("overlay").style.display = "none";
        }

        function getEditorWindow() {
            if (dialogAgent.enabled) {
                //对话框模式打开的
                return dialogAgent.clientWindow;
            } else if (editorLoaded) {
                return $get("winExp").contentWindow;
            }
            return null;
        }

        var dialogAgent = {
            enabled: false,
            initExpressionEditor: function (win) { window.initExpressionEditor(win); this.enabled = true; },
            clientWindow: null,
            onDialogLoad: function () {
                if (context.beforeShowEditor())
                    window.showEditor();
            }

        };

        Sys.Application.add_init(function () {
            var grid = $find("gridMain");
            var readOnly = grid.get_readOnly();

            //定制工具啦
            var toolDiv = document.createElement("div");

            grid.get_captionElement().appendChild(toolDiv);
            toolDiv.className = "pc-listmenu";

            var li = document.createElement("li");
            var lnkCmd = document.createElement("a");

            toolDiv.appendChild(li);
            li.appendChild(lnkCmd);
            lnkCmd.className = "list-cmd";
            lnkCmd.href = "javascript:void(0);";
            lnkCmd.appendChild(document.createTextNode("添加"));
            if (!readOnly) {
                $addHandler(lnkCmd, "click", function () {
                    context = contextNew;
                    $pc.showDialog("../dialogs/ExpressionEditor.aspx", dialogAgent, null, false, 400, 300, false);
                }, false);
            }


            li = document.createElement("li");
            lnkCmd = document.createElement("a");

            toolDiv.appendChild(li);
            li.appendChild(lnkCmd);
            lnkCmd.className = "list-cmd";
            lnkCmd.href = "javascript:void(0);";
            lnkCmd.appendChild(document.createTextNode("删除"));
            if (!readOnly) {
                $addHandler(lnkCmd, "click", function () {
                    var data = grid.get_selectedData();
                    var src = grid.get_dataSource();
                    if (data) {

                        if (data.length > 0 && confirm("确实要删除所选的条件？")) {
                            while (data.length > 0) {
                                var p = data.pop();
                                for (var i = src.length - 1; i >= 0; i--) {
                                    if (src[i] == p) {
                                        src.splice(i, 1);
                                        break;
                                    }
                                }
                            }

                            grid.set_dataSource(src);
                            $pc.addClass("panPrompt", "pc-hide");
                            $pc.removeClass("panExec", "pc-hide");
                        }
                    }
                }, false);
            }


            li = document.createElement("li");
            lnkCmd = document.createElement("a");

            toolDiv.appendChild(li);
            li.appendChild(lnkCmd);
            lnkCmd.className = "list-cmd";
            lnkCmd.href = "javascript:void(0);";
            lnkCmd.appendChild(document.createTextNode("历史"));
            $addHandler(lnkCmd, "click", function () {
                $pc.popups.conditonHistory($pc.get("hfID").value);
            });

            lnkCmd = null;

        });

        function handleRowDelete(e) {
            var grid = $find("gridMain");
            var data = this.rowData;
            var src = grid.get_dataSource();
            for (var i = src.length - 1; i >= 0; i--) {
                if (src[i] == data) {
                    if (confirm("删除这个条件？")) {
                        src.splice(i, 1);
                        break;
                    } else {
                        return false;
                    }
                }
            }

            grid.set_dataSource(src);
            grid = null;
            data = null;
            src = null;
            $pc.addClass("panPrompt", "pc-hide");
            $pc.removeClass("panExec", "pc-hide");

        }

        function handleRowEdit(e) {
            context = new editContext(this.rowData, {
                beforeShowEditor: function () {
                    if (editorLoaded) {
                        var win = getEditorWindow();
                        if (win) {
                            win.setMessage("");
                            win.setData(this._data.Description, this._data.Condition);
                            win.clearCheckResult();
                        }
                        win = null;

                        return true;
                    } else {
                        return false;
                    }
                }, handleOk: function () {
                    var editor = getEditorWindow();
                    var grid = $find("gridMain");
                    if (editor) {
                        var data = editor.getData();
                        this._data.Condition = data.exp;
                        this._data.Description = data.desc;
                        grid.set_dataSource(grid.get_dataSource());
                    }
                    hideEditor();
                    $pc.addClass("panPrompt", "pc-hide");
                    $pc.removeClass("panExec", "pc-hide");
                    $pc.addClass("panWait", "pc-hide");
                }
            });
            $pc.showDialog("../dialogs/ExpressionEditor.aspx", dialogAgent, null, false, 400, 300, false);
        }

        function handleRowCheck(e) {
            service.ValidateExpression(this.rowData.Condition, function (data) {
                alert(data ? "校验通过" : "校验失败");
            }, function (err) {
                alert("校验遇到错误");
            }, null);
        }

        function handleCellCreating(src, arg) {

            if (arg.column.dataField == 'OwnerID') {
                arg.editor.get_editorElement.style = "display:none";
                arg.editor.get_gridCell().get_htmlCell().firstChild.style.display = "none";
            }
            if (arg.column.dataField == 'OwnerID' && src.get_readOnly() == false) {

                arg.editor.get_editorElement.style = "display:none";
                var toolC = document.createElement("div");
                toolC.className = "pc-actions";
                arg.editor.get_gridCell().get_htmlCell().firstChild.style.display = "none";
                arg.editor.get_gridCell().get_htmlCell().appendChild(toolC);

                var lnkCmd = document.createElement("a");
                lnkCmd.className = "pc-cmd pc-delete";
                lnkCmd.title = "删除";
                lnkCmd.href = "javascript:void(0);";
                toolC.appendChild(lnkCmd);
                $addHandler(lnkCmd, "click", Function.createDelegate(arg, handleRowDelete));

                lnkCmd = document.createElement("a");
                lnkCmd.className = "pc-cmd pc-edit";
                lnkCmd.title = "修改";
                lnkCmd.href = "javascript:void(0);";
                toolC.appendChild(lnkCmd);
                $addHandler(lnkCmd, "click", Function.createDelegate(arg, handleRowEdit));

                lnkCmd = document.createElement("a");
                lnkCmd.className = "pc-cmd pc-check";
                lnkCmd.title = "检查";
                lnkCmd.href = "javascript:void(0);";
                toolC.appendChild(lnkCmd);
                $addHandler(lnkCmd, "click", Function.createDelegate(arg, handleRowCheck));

                lnkCmd = null;
                toolC = null;
            }

        }

        function onPrepareData(e) {

            e.steps = [1];
        }

        function postProcess(e) {
            document.getElementById("lnkPreview").click();
        }
    
    </script>
    <asp:Literal runat="server" ID="postScript" Mode="Transform"></asp:Literal>
</body>
</html>
