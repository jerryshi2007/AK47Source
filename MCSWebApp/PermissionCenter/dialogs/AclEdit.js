$pc.ui.traceWindowWidth();

function alterName(nameA, nameB) {
    return nameA.length > 0 ? nameA : nameB;
}

var contextData = Sys.Serialization.JavaScriptSerializer.deserialize(document.getElementById('postData').value);

var currentSelectedRole = null;

var firstElem = null;

function roleListClick(e) {
    var src = e.srcElement || e.originalTarget;
    if (src.nodeType === 1 && src.nodeName.toUpperCase() == "BUTTON") {
        if ("preventDefault" in e)
            e.preventDefault();
        if ("returnValue" in e)
            e.returnValue = false;
        var id = $pc.getAttr(src, "data-id");
        var actionRole = $pc.getAttr(src, "data-role");
        if (typeof (id) == "string" && id.length > 0) {
            if (actionRole === 'delete') {
                if (confirm("确定要删除这个角色吗?")) {
                    // 从角色列表移除角色
                    for (var i = contextData.Roles.length - 1; i >= 0; i--) {
                        if (contextData.Roles[i].RoleID === id) {
                            contextData.Roles.splice(i, 1);
                            $find("roleGrid").set_dataSource(contextData.Roles);
                            break;
                        }
                    }
                    // 移除容器关联
                    for (i = contextData.AclMembers.length - 1; i >= 0; i--) {
                        if (contextData.AclMembers[i].MemberID == id) {
                            contextData.AclMembers.splice(i, 1);
                        }
                    }
                }
            } else if (actionRole == "view") {
                $pc.showDialog($pc.appRoot + "lists/AppRoleMembersView.aspx?role=" + encodeURIComponent(id), null, null, false, 800, 600, true);
            }


        }
    }

    var curRole = $find("roleGrid").get_selectedData()[0] || null;
    if (curRole != currentSelectedRole) {
        currentSelectedRole = curRole;
        handleCurrentRoleChanged();
    }
}

function handleCurrentRoleChanged() {
    var pmList = $find("permissionGrid");
    if (currentSelectedRole) {
        pmList.set_dataSource(contextData.Permissions);
    } else {
        pmList.set_dataSource(contextData.Permissions);
    }

}

function pmListClick(e) {
    var src = e.srcElement || e.originalTarget;
    if (src.nodeType === 1 && src.nodeName.toUpperCase() == "BUTTON") {
        if ("preventDefault" in e)
            e.preventDefault();
        if ("returnValue" in e)
            e.returnValue = false;

        if ($find("permissionGrid").get_readOnly())
            return; //无效
        //因此
        var enabled = true;
        $pc.descendant(src, "span/input", function () {
            enabled = $pc.getEnabled(this);
        });
        if (!enabled) {
            return; //检查CheckBox是否启用
        }

        if (currentSelectedRole) {

            var pmName = $pc.getAttr(src, "data-name");
            var found = false;
            for (var i = contextData.AclMembers.length - 1; i >= 0; i--) {
                if (contextData.AclMembers[i].ContainerPermission == pmName && contextData.AclMembers[i].MemberID == currentSelectedRole.RoleID) {
                    found = true;
                    // 解除权限
                    contextData.AclMembers.splice(i, 1);
                    $pc.setAttr(src, "data-checked", "0");
                    $pc.descendant(src, "span/input", function () {
                        this.checked = "";
                    });
                    //$pc.removeClass(src.parentNode, "pc-checked");
                    break;
                }
            }

            if (!found) {
                contextData.AclMembers.push({ "ContainerID": contextData.ContainerID, "ContainerPermission": pmName, "MemberID": currentSelectedRole.RoleID, "VersionStartTime": new Date(), "VersionEndTime": new Date(), "Status": 1, "SortID": 0, "ContainerSchemaType": null, "MemberSchemaType": null });
                $pc.setAttr(src, "data-checked", "1");
                $pc.descendant(src, "span/input", function () {
                    this.checked = "checked";
                });
                //$pc.addClass(src.parentNode, "pc-checked");
            }
        }
    }
}

function createRoleGridNewRow(sender, e) {
    e.autoFormat = false;
    switch (e.column.dataField) {
        case "RoleID":
            e.showValueTobeChange = "";

            var itemElem = $HGDomElement.createElementFromTemplate({
                nodeName: "div",
                properties: { className: "pc-item-margin" }
            }, e.container);

            var actionElem = $HGDomElement.createElementFromTemplate({
                nodeName: "span",
                properties: { className: "pc-actions" }
            }, itemElem);

            if (!sender.get_readOnly()) {
                $HGDomElement.createElementFromTemplate({
                    nodeName: "button",
                    properties: { className: "pc-cmd pc-delete", "data-role": "delete", "data-id": e.rowData["RoleID"], title: "删除" }
                }, actionElem);
            }


            $HGDomElement.createElementFromTemplate({
                nodeName: "button",
                properties: { className: "pc-cmd pc-comment", "data-role": "view", "data-id": e.rowData["RoleID"], title: "查看人员" }
            }, actionElem);

            $HGDomElement.createElementFromTemplate({
                nodeName: "span",
                properties: { className: "pc-icon16 Roles" }
            }, itemElem);

            $HGDomElement.createTextNode(alterName(e.rowData["RoleDisplayName"], e.rowData["RoleName"]), itemElem);

            var spanAppElem = $HGDomElement.createElementFromTemplate({
                nodeName: "span",
                properties: { className: "pc-remark" }
            }, itemElem);

            $HGDomElement.createTextNode(alterName(e.rowData["ApplicationDisplayName"], e.rowData["ApplicationName"]), spanAppElem);

            if (e.rowData == contextData.Roles[0]) {
                firstElem = itemElem;
            }

            itemElem = null;
            elem = null;
            actionElem = null;
            spanAppElem = null;

            break;
        default:
            e.autoFormat = true;
    }
}

function createPMGridNewRow(sender, e) {
    e.autoFormat = false;
    var pmName = e.rowData["Name"];
    switch (e.column.dataField) {
        case "Name":
            e.showValueTobeChange = "";
            var itemElem = $HGDomElement.createElementFromTemplate({
                nodeName: "button",
                properties: { className: "pc-pm-button", "data-name": pmName }
            }, e.container);

            var checkElem = $HGDomElement.createElementFromTemplate({
                nodeName: "span",
                properties: { className: "pc-checkmark pc-checked" }
            }, itemElem);

            var checkBox = $HGDomElement.createElementFromTemplate({
                nodeName: "input",
                properties: { type: "checkbox" }
            }, checkElem);

            var actionElem = $HGDomElement.createElementFromTemplate({
                nodeName: "span",
                properties: { className: "pc-actions" }
            }, itemElem);

            $HGDomElement.createTextNode(e.rowData["DisplayName"], itemElem);

            var spanAppElem = $HGDomElement.createElementFromTemplate({
                nodeName: "span",
                properties: { className: "pc-remark" }
            }, itemElem);

            $HGDomElement.createTextNode(e.rowData["Description"], spanAppElem);

            if (currentSelectedRole) {
                var roleID = currentSelectedRole.RoleID;

                for (var i = contextData.AclMembers.length - 1; i >= 0; i--) {
                    if (contextData.AclMembers[i].MemberID == roleID && contextData.AclMembers[i].ContainerPermission == pmName) {
                        // 授权
                        if ($pc.getAttr(itemElem, "data-checked") != "1") {
                            $pc.setAttr(itemElem, "data-checked", "1");
                            //$pc.addClass(itemElem.parentNode, "pc-checked");
                            checkBox.checked = "checked";
                        }
                    }
                }
            }

            elem = null;
            actionElem = null;
            spanAppElem = null;
            checkBox = null;


            break;
        default:
            e.autoFormat = true;
            break;
    }
}

function addRoleClick() {
    var grid = $find("roleGrid");
    if (grid) {
        var path = $pc.appRoot + "dialogs/RoleSearchDialog.aspx";
        if (window.location.search.indexOf('?') == 0) {
            var paramsArr = window.location.search.substring(1).split('&');
            var params = {};
            for (var i = 0; i < paramsArr.length; i++) {
                var kv = paramsArr[i].split("=");
                if (kv.length == 2) {
                    params[decodeURIComponent(kv[0])] = decodeURIComponent(kv[1]);
                }
            }
        }


        if ("forapp" in params) {
            if (params['forapp'] == '1')
                path += "?appId=" + encodeURIComponent(params['id']);
        }

        if ($pc.showDialog(path, { window: window, inputElem: "actionData" }, null, false, 800, 600, true) == true) {
            var data = Sys.Serialization.JavaScriptSerializer.deserialize($pc.get("actionData").value);
            for (i = 0; i < data.length; i++) {
                var role = data[i];
                var exists = false;
                for (var j = 0; j < contextData.Roles.length; j++) {
                    if (contextData.Roles[j].RoleID == role.RoleID) {
                        exists = true;
                        break;
                    }
                }
                if (!exists) {
                    contextData.Roles.push(role);
                }
            }
            grid.set_dataSource(contextData.Roles);

            if (currentSelectedRole) {
                grid.set_selectedData([currentSelectedRole]);
            }
        }
    }
}

function onOkClick() {
    document.getElementById("postData").value = Sys.Serialization.JavaScriptSerializer.serialize(contextData.AclMembers);
    document.getElementById("btPostSave").click();
}

function onCancelClick() {
    window.returnValue = '';
    window.close();
}

function prepareData(e) {
    var inherit = document.getElementById("chkInherit");
    if (inherit) {
        e.steps = [contextData.AclMembers, "same", inherit.checked ? "inherit" : "none"];
    } else {
        e.steps = [contextData.AclMembers, "same"];
    }
    e.clientExtraPostedData = contextData.ContainerID;
    inherit = null;
}

function prepareData2(e) {
    var inherit = document.getElementById("chkInherit");
    e.steps = [contextData.AclMembers, "all", ];
    e.clientExtraPostedData = contextData.ContainerID;
    if (inherit) {
        e.steps[2] = inherit.checked;
    }
}

function onSaveComplete(e) {
    if (e.dataChanged)
        window.close();
}

Sys.Application.add_init(function () {

    function hideGridColumn(gridId) {
        var grid = document.getElementById(gridId);

        // hide column header
        $pc.descendant(grid, "tbody[-0]/tr/td/table/tbody/tr/td/table", function () {
            $pc.descendant(this, "tbody", function () {
                this.style.display = "none";
            });
        });

        grid = null;
    }

    hideGridColumn("roleGrid");
    hideGridColumn("permissionGrid");

    $find("roleGrid").set_dataSource(contextData.Roles);

    $find("permissionGrid").set_dataSource(contextData.Permissions);


    window.setTimeout(function () {
        if (firstElem) {
            firstElem.click();
            firstElem = null;
        }
    }, 200);

});