<%@ Page Language="c#" CodeBehind="EditObjAttr.aspx.cs" AutoEventWireup="True" Inherits="MCS.Applications.AppAdmin.Dialogs.EditObjAttr" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
    <title>应用属性编辑</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
    <link href="../Css/Input.css" type="text/css" rel="stylesheet" />
    <script type="text/javascript" language="javascript" src="../script/validate.js"></script>
    <script type="text/javascript" language="javascript" src="../script/xmlHttp.js"></script>
    <script type="text/javascript" language="javascript" src="../script/xsdAccess.js"></script>
    <script type="text/javascript" language="javascript" src="../script/uiScript.js"></script>
    <script type="text/javascript" language="javascript" src="../Script/ApplicationRoot.js"></script>
    <script type="text/javascript" language="javascript" src="editObjAttr.js"></script>
    <script type="text/javascript">
        //m_objParam属性[appID,appResLevel,clasify,codeName,disabled,fatherNodeType,id,objID,op,type]
        function SetRoleLink() {
            if (m_objParam["type"] == "ROLES") {
                document.getElementById("RoleLink").style.display = "inline"
            }
        }
        function RolePropertyDefinition() {
            var url = "../../WorkflowDesigner/MatrixModalDialog/RolePropertyExtension.aspx?AppID=" + m_objParam["appID"] + "&RoleID=" + m_objParam["id"];
            var sFeature = "dialogWidth:400px; dialogHeight:300px;center:yes;help:no;resizable:yes;scroll:no;status:no";
            m_objParam.Role_Name = document.getElementById("NAME").value;
            m_objParam.Role_CodeName = document.getElementById("CODE_NAME").value;
            m_objParam.Role_Description = document.getElementById("DESCRIPTION").value;
            window.showModalDialog(url, m_objParam, sFeature);

        }
    </script>
</head>
<body onload="onDocumentLoad();SetRoleLink();" class="modeal" ms_positioning="GridLayout">
    <form id="frmInput" method="post" runat="server">
    <input id="APP_ID" datafld="APP_ID" datasrc="ROLES" type="hidden" name="APP_ID">
    <input id="ID" datafld="ID" datasrc="ROLES" type="hidden" name="ID">
    <input id="CLASSIFY" datafld="CLASSIFY" datasrc="ROLES" type="hidden" name="CLASSIFY">
    <input id="RESOURCE_LEVEL" datafld="RESOURCE_LEVEL" datasrc="FUNCTION_SETS" type="hidden"
        name="RESOURCE_LEVEL">
    <table style="width: 100%; height: 100%">
        <tr>
            <td style="height: 32px">
                <span id="logoSpan" style="background-position: center center; background-image: url(../images/32/role.gif);
                    width: 32px; background-repeat: no-repeat; height: 32px"></span><font size="4"><strong
                        id="topCaption"></strong></font>
                <hr>
            </td>
        </tr>
        <tr>
            <td>
                <table style="width: 100%; height: 100%" class="modalEditable">
                    <tr>
                        <td style="width: 80px; height: 24px" align="right">
                            <strong>名称</strong>
                        </td>
                        <td>
                            <input id="NAME" name="NAME" type="text" datafld="NAME" datasrc="ROLES" onchange="onNameChange();">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 80px; height: 24px" align="right">
                            <strong>英文标识</strong>
                        </td>
                        <td>
                            <input id="CODE_NAME" name="CODE_NAME" type="text" datafld="CODE_NAME" datasrc="ROLES"
                                onkeypress="onCodeNameKeyPress();" onchange="return onCodeNameChange();">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 80px; height: 24px" align="right">
                            <strong>描述</strong>
                        </td>
                        <td>
                            <textarea id="DESCRIPTION" name="DESCRIPTION" datafld="DESCRIPTION" datasrc="ROLES"
                                style="width: 100%; height: 100%"></textarea>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 24px">
                        </td>
                        <td>
                            <span id="inheritedSpan" style="display: inline">
                                <input id="INHERITED" type="checkbox" datafld="INHERITED" datasrc="ROLES" style="border: none"
                                    name="INHERITED">继承上级授权</span> <span id="delegateSpan" style="display: none">
                                        <input type="checkbox" id="ALLOW_DELEGATE" datafld="ALLOW_DELEGATE" datasrc="ROLES"
                                            style="border: none" name="ALLOW_DELEGATE">允许委派</span> &nbsp;&nbsp;&nbsp;
                            <a id="RoleLink" href="#" onclick="RolePropertyDefinition()" style="display: none;">
                                角色属性扩展</a> <span id="leafSpan" style="display: none">
                                    <input id="LOWEST_SET" type="checkbox" datafld="LOWEST_SET" datasrc="FUNCTION_SETS"
                                        style="border: none" name="LOWEST_SET">与功能直接相关</span>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="height: 10px">
                <hr>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="height: 24px">
                <table style="width: 100%; height: 100%" cellspacing="0">
                    <tr>
                        <td align="center">
                            <input id="btnOK" name="btnOK" accesskey="O" type="button" value="确定(O)" style="width: 80px"
                                onclick="onSaveClick();">
                        </td>
                        <td align="center">
                            <input id="btnCancel" name="btnCancel" accesskey="C" type="button" value="取消(C)"
                                style="width: 80px" onclick="window.close();">
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
