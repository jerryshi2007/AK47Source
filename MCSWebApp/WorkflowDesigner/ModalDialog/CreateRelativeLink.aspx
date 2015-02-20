<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateRelativeLink.aspx.cs"
    Inherits="WorkflowDesigner.ModalDialog.CreateRelativeLink" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>相关链接编辑</title>
    <script type="text/javascript" src="../js/jquery-1.4.3.js"></script>
    <script type="text/javascript" src="../js/jquery.json-2.2.js"></script>
    <script type="text/javascript" src="../js/common.js"></script>
    <script type="text/javascript" src="../js/wfweb.js"></script>
    <script type="text/javascript">
        var linkSource

        $().ready(function () {
            var modifyLinkKey = $("#modifykey").val();

            var args = window.dialogArguments;
            linkSource = jQuery.parseJSON(args.jsonStr);
            if (modifyLinkKey == '') $("#linkKey").val(CreateKey());
            else {
                var linkObj = linkSource.get(modifyLinkKey, function (o, v) {
                    if (o.Key == v) return true;
                    return false;
                });
                Bind(linkObj);
            }
        });

        function Submit() {
            if (!Valid()) return;

            var modifyLinkKey = $("#modifykey").val();
            var obj = $.parseJSON($("#relativeLinkTemplate").val());

            WFWeb.ModifyWfObject(obj,'Key',$("#linkKey").val());
            WFWeb.ModifyWfObject(obj,'Name',$("#linkName").val());
            WFWeb.ModifyWfObject(obj,'Enabled',$("#linkEnabled").val());
            WFWeb.ModifyWfObject(obj,'Description',$("#linkDescription").val());
            WFWeb.ModifyWfObject(obj,'Category',$("#linkCategory").val());
            WFWeb.ModifyWfObject(obj,'Url',$("#linkUrl").val());

            if (modifyLinkKey != '') {
                linkSource.remove(modifyLinkKey, function (o, v) {
                    if (o.Key == v) return true;
                    return false;
                });

            };

            linkSource.push(obj);
            window.returnValue = { jsonStr: jQuery.toJSON(linkSource) };
            top.close();
        }

        function CreateKey() {
            var i = 0;
            while (true) {
                var key = "RLink" + i;
                if (!linkSource.has(key, function (o, val) {
                    if (o.Key == val) return true;
                    return false;
                })) {
                    return key;
                }
                i++;
            }
        }

        function Valid() {
            return true;
        }

        function Bind(linkObj) {
            $("#linkKey").val(linkObj.Key);
            $("#linkName").val(linkObj.Name);
            $("#linkEnabled").val(linkObj.Enabled);
            $("#linkDescription").val(linkObj.Description);
            $("#linkCategory").val(linkObj.Category);
            $("#linkUrl").val(linkObj.Url);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <table width="100%" style="height: 100%; width: 100%">
        <tr>
            <td class="gridHead">
                <div class="dialogTitle">
                    <span class="dialogLogo">新建相关链接</span>
                </div>
            </td>
        </tr>
        <tr>
            <td style="vertical-align: center">
                <div class="dialogContent" id="dialogContent" runat="server" style="width: 100%;
                    height: 100%; overflow: auto">
                    <!--Put your dialog content here... -->
                    <table width="100%" style="height: 100%; width: 100%;">
                        <tr style="height: 10px;">
                            <td>
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" valign="middle">
                                Key&nbsp;
                            </td>
                            <td valign="middle">
                                <input id="linkKey" disabled />
                            </td>
                        </tr>
                        <tr>
                            <td class="label" valign="middle">
                                名称
                            </td>
                            <td valign="middle">
                                <input id="linkName" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label" valign="middle">
                                是否可用
                            </td>
                            <td valign="middle">
                                <select id="linkEnabled">
                                    <option selected="selected" value="true">True</option>
                                    <option value="false">False</option>
                                </select>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" valign="middle">
                                描述
                            </td>
                            <td valign="middle">
                                <input id="linkDescription" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label" valign="middle">
                                类别
                            </td>
                            <td valign="middle">
                                <input id="linkCategory" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label" valign="middle">
                                URL&nbsp;
                            </td>
                            <td valign="middle">
                                <input id="linkUrl" />
                            </td>
                        </tr>
                    </table>
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
                            <input id="btnOK" type="button" value="确定(O)" accesskey="O" onclick="Submit();" class="formButton" />
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
    <input id="modifykey" type="hidden" value="<%=Request["key"] %>" />
    <asp:HiddenField ID="relativeLinkTemplate" runat="server" />
    </form>
</body>
</html>
