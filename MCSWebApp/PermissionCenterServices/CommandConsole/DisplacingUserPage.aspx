<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DisplacingUserPage.aspx.cs"
    Inherits="PermissionCenter.CommandConsole.DisplacingUserPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>替换人员测试页面</title>
    <style type="text/css">
        label.label
        {
            width: 200px;
            display: inline-block;
        }
        
        .execute-status
        {
            color: #f00;
            padding: 5px;
        }
        
        .input
        {
            width: 400px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager runat="server" ID="sm" EnableScriptGlobalization="true">
        <Services>
            <asp:ServiceReference Path="~/Services/UserDisplacingService.asmx" />
        </Services>
    </asp:ScriptManager>
    <div>
        <fieldset>
            <legend>替换角色人员</legend>
            <select id="selection">
                <option value="DisplaceUserInProcessDescriptor">DisplaceUserInProcessDescriptor</option>
                <option value="DisplaceUserInProcessMatrix">DisplaceUserInProcessMatrix</option>
                <option value="DisplaceUserInRole">DisplaceUserInRole</option>
                <option value="DisplaceUserInRoleMatrix">DisplaceUserInRoleMatrix</option>
                <option value="DisplaceUserInProcess">DisplaceUserInProcess</option>
            </select>
            <div>
                参数
                <div id="fields">
                </div>
            </div>
            <div>
                <input type="button" id="execute" value="执行" /><span class="execute-status" id="executeStatus"></span>
            </div>
        </fieldset>
    </div>
    <script type="text/javascript">
        Sys.Application.add_init(function () {

            var allManager = {
                DisplaceUserInProcessDescriptor: {
                    parameters: [
                        { name: "processDespKey", type: "string", description: "" },
                        { name: "userID", type: "string", description: "" },
                        { name: "displacingUserIDArray", type: "stringArray" }
                    ]
                },

                DisplaceUserInProcessMatrix: {
                    parameters: [
                        { name: "processDespKey", type: "string", description: "" },
                        { name: "userID", type: "string", description: "" },
                        { name: "displacingUserIDArray", type: "stringArray" }
                    ]
                },

                DisplaceUserInRole: {
                    parameters: [
                        { name: "roleID", type: "string", description: "" },
                        { name: "userID", type: "string", description: "" },
                        { name: "displacingUserIDArray", type: "stringArray" }
                    ]
                },

                DisplaceUserInRoleMatrix: {
                    parameters: [
                        { name: "roleID", type: "string", description: "" },
                        { name: "userID", type: "string", description: "" },
                        { name: "displacingUserIDArray", type: "stringArray" }
                    ]
                },

                DisplaceUserInProcess: {
                    parameters: [
                        { name: "processID", type: "string", description: "" },
                        { name: "userID", type: "string", description: "" },
                        { name: "displacingUserIDArray", type: "stringArray" }
                    ]
                }
            }

            var toggleFunction = function () {
                var key = $get("selection").value;
                var parameters = allManager[key].parameters;
                var fields = $get("fields");
                var label, input, p;
                fields.innerHTML = '';
                for (var i = 0; i < parameters.length; i++) {
                    p = parameters[i];
                    label = document.createElement("label");
                    fields.appendChild(label);
                    label.title = p.name;
                    fields.appendChild(label);
                    label.className = "label";
                    label.appendChild(document.createTextNode(p.name));
                    input = document.createElement("input");
                    input.type = "text";
                    input.className = "input";
                    if (input.getAttribute) {
                        input.setAttribute("key", p.name);
                    } else {
                        input["key"] = p.name;
                    }
                    fields.appendChild(input);
                    fields.appendChild(document.createElement("br"));
                }

            }

            var collectParameters = function () {
                var p = {};
                var k;
                var elem = $get("fields").firstChild;
                while (elem) {
                    if (elem.nodeType === 1 && elem.nodeName.toUpperCase() === "INPUT") {
                        if (elem.getAttribute) {
                            k = elem.getAttribute("key");
                        } else {
                            k = elem["key"];
                        }

                        p[k] = elem.value;
                    }

                    elem = elem.nextSibling;
                }

                return p;
            }

            var handleDataReceive = function (data) {
                $get("executeStatus").innerHTML = '';
                $get("executeStatus").appendChild(document.createTextNode("WebService返回了结果:" + data));
            }

            var handleDataError = function (err) {
                $get("executeStatus").innerHTML = '';
                $get("executeStatus").appendChild(document.createTextNode("发生了错误。。。" + err.get_message()));
            }

            $addHandler($get("selection"), "change", toggleFunction);
            $addHandler($get("execute"), "click", function () {
                var method = $get("selection").value;
                var parameters = allManager[method].parameters;
                var inputValues = collectParameters();
                var target = [];
                var p, v;
                for (var i = 0; i < parameters.length; i++) {
                    p = parameters[i];
                    v = inputValues[p.name];
                    if (p.type === "string") {
                        target.push(v);
                    } else if (p.type === "stringArray") {
                        target.push(v.split(","));
                    }
                }

                target.push(handleDataReceive);
                target.push(handleDataError);

                var m = PermissionCenter.Services.UserDisplacingService[method];
                if (m) {
                    $get("executeStatus").innerHTML = '';
                    $get("executeStatus").appendChild(document.createTextNode("正在请求WebService。。。"));
                    m.apply(this, target);
                }
            });

            toggleFunction();
        });
    
    </script>
    </form>
</body>
</html>
