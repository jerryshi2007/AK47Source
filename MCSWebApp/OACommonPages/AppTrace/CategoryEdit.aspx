<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CategoryEdit.aspx.cs" Inherits="MCS.OA.CommonPages.AppTrace.CategoryEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>分类配置</title>
    <link href="../../css/overrides.css" rel="stylesheet" type="text/css" />
    <link href="../../css/templatecss.css" rel="stylesheet" type="text/css" />
    <base target="_self" />
    <style type="text/css">
        .property-area
        {
            display: block;
            border: 1px solid silver;
            padding: 2px;
            line-height: 16px;
            position: relative;
            height: 16px;
            outline: #000000 solid 1px;
            overflow: hidden;
        }
        
        .property-text
        {
            display: inline-block;
            -o-text-overflow: ellipsis;
            overflow: hidden;
            padding-right: 20px;
            height: 16px;
            overflow: hidden;
            line-height: 16px;
        }
        
        .property-button-wrapper
        {
            display: block;
            float: right;
            position: relative;
        }
        .property-button
        {
            position: absolute;
            right: 0;
            top: 0;
            height: 16px;
            width: 16px;
            padding: 0;
            display: block; /*border: 1px solid #000000;*/
            text-align: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager runat="server" EnableScriptGlobalization="true" />
    <div class="t-layout">
        <div class="t-caption">
            分类配置
        </div>
    </div>
    <div style="margin: 5px; line-height: 23px; width: 200px">
        <div>
            请注意：如果选择的应用，模块和权限已经存在，将被替换。
        </div>
        <div>
            选择应用
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*"
                ControlToValidate="ddApp" ForeColor="Red">此项必填</asp:RequiredFieldValidator>
        </div>
        <div>
            <asp:DropDownList runat="server" ID="ddApp" Width="100%" AutoPostBack="True" DataSourceID="ObjectDataSource1"
                DataTextField="Name" DataValueField="CodeName">
            </asp:DropDownList>
            <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="QueryAvaliableApplicationNames"
                TypeName="MCS.OA.CommonPages.AppTrace.CategorySearchSource"></asp:ObjectDataSource>
        </div>
        <div>
            选择模块
            <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*"
				ControlToValidate="ddPg" ForeColor="Red">此项必填</asp:RequiredFieldValidator>--%>
        </div>
        <div>
            <asp:DropDownList ID="ddPg" runat="server" Width="100%" DataSourceID="ObjectDataSource2"
                DataTextField="Name" DataValueField="CodeName">
            </asp:DropDownList>
            <asp:ObjectDataSource ID="ObjectDataSource2" runat="server" SelectMethod="QueryAvaliableProgramNames"
                TypeName="MCS.OA.CommonPages.AppTrace.CategorySearchSource">
                <SelectParameters>
                    <asp:ControlParameter ControlID="ddApp" Name="appName" PropertyName="SelectedValue"
                        Type="String" />
                </SelectParameters>
            </asp:ObjectDataSource>
        </div>
        <div>
            选择权限
        </div>
        <div>
            <asp:RadioButton Text="表单查看者" runat="server" ID="chkViewer" GroupName="ado1" Checked="True" />
            <asp:RadioButton Text="表单流程调整者" runat="server" ID="chkAdjuster" GroupName="ado1" />
        </div>
        <div>
            选择角色
            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="inputRole"
                ErrorMessage="*" ForeColor="Red">此项必填</asp:RequiredFieldValidator>
        </div>
        <div>
            <div class="property-area">
                <div class="property-button-wrapper">
                    <button class="property-button" type="button" title="选择角色" onclick="selectRole();">
                        …</button>
                </div>
                <span id="roleNameLabel" runat="server" class="property-text">未选择</span>
                <div style="display: none">
                    <input type="hidden" id="demoArea" value="" />
                    <asp:TextBox ID="inputRole" runat="server" />
                    <asp:HiddenField ID="inputRoleName" runat="server" />
                </div>
            </div>
        </div>
        <div class="t-button-panel">
            <asp:Button AccessKey="O" runat="server" ID="btnOK" Text="确定(O)" CssClass="formButton"
                OnClick="OkClick" />
            <input accesskey="C" type="button" class="formButton" value="关闭(C)" onclick="window.close();" />
        </div>
    </div>
    </form>
    <script type="text/javascript">
        function selectRole() {
            var param = { window: window, inputElem: "demoArea" };

            var feature = "dialogWidth:800px; dialogHeight:640px; center:yes; help:no; resizable:no;status:no;";

            var result = window.showModalDialog("/MCSWebApp/PermissionCenter/dialogs/RoleSearchDialog.aspx", param, feature);

            param = null;

            if (result) {
                var roles = Sys.Serialization.JavaScriptSerializer.deserialize($get("demoArea").value);
                if (roles.length > 0) {
                    $get("inputRole").value = roles[0].RoleID;
                    $get("inputRoleName").value = roles[0].RoleName;
                    document.getElementById("roleNameLabel").replaceChild(document.createTextNode(roles[0].RoleName), document.getElementById("roleNameLabel").firstChild);
                    $get("inputRole").blur();

                }
            }

        }
    </script>
</body>
</html>
