<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AclEdit.aspx.cs" Inherits="PermissionCenter.Dialogs.AclEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" class="pcdlg" scroll="no" style="overflow: hidden">
<head id="Head1" runat="server">
    <title>ACL控制</title>
    <link rel="icon" href="../favicon.ico" type="image/x-icon" />
    <link rel="Shortcut Icon" href="../favicon.ico" />
    <link href="../styles/dlg.css" rel="stylesheet" type="text/css" />
    <base target="_self" />
    <style type="text/css">
        .pc-scrollgrid
        {
            display: block; /*height: 130px;*/
            overflow: auto;
            overflow-x: hidden;
            border: solid 2px #dfdfdf;
            padding-right: 15px;
        }
        
        .pc-item-margin
        {
            border-bottom: dotted 1px #808080;
            padding: 2px 5px;
            cursor: pointer;
        }
        
        .pc-actions
        {
            right: 0px;
            top: 0;
            display: none;
            width: 60px;
            text-align: right;
            float: right;
        }
        
        .selectedItem .pc-item-margin
        {
            background-color: #1BA1E2;
            color: #ffffff;
        }
        
        .hoveringItem .pc-actions, .selectedItem .pc-actions
        {
            display: inline-block;
            text-align: right;
        }
        .pc-actions .pc-cmd
        {
            display: inline-block;
            border: 0;
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
        
        .pc-actions .pc-cmd.pc-comment
        {
            background-position: -128px -96px;
        }
        
        .pc-actions .pc-cmd:hover
        {
            background-image: url('../images/ui-icons_cd0a0a_256x240.png');
        }
        
        .pc-remark
        {
            padding-left: 10px;
            color: #bfbfbf;
        }
        
        
        .pc-pm-button
        {
            width: 100%;
            border: 0;
            text-align: left;
            cursor: pointer;
            margin-bottom: 2px;
        }
        
        .pc-checked .pc-pm-button
        {
            background: #1BA1E2;
            color: #ffffff;
        }
        
        .pc-checkmark
        {
            display: block;
            float: left;
            height: auto;
            width: auto;
        }
        
        .pc-checked .pc-checkmark
        {
            background: transparent url("../images/artwork/check.png");
        }
    </style>
    <pc:HeaderControl runat="server">
    </pc:HeaderControl>
</head>
<body class="pcdlg" style="min-height: 0; min-width: 0">
    <form id="form1" runat="server" style="min-width: 200px">
    <pc:SceneControl runat="server" />
    <asp:ScriptManager runat="server" EnableScriptGlobalization="true" />
    <div class="pcdlg-sky">
        <h1 class="pc-caption">
            ACL控制<span class="pc-timepointmark">
                <mcs:TimePointDisplayControl ID="tpd1" runat="server" />
            </span>
        </h1>
    </div>
    <div class="pcdlg-content">
        <pc:BannerNotice runat="server" ID="notice" />
        <div class="pc-container5">
            <div>
                对象名称<span id="spObjName" runat="server" style="padding-left: 5px; font-weight: bold;"></span>
            </div>
            <div class="pc-rolelist">
                <div>
                    角色
                </div>
                <div class="pc-scrollgrid" onclick="roleListClick(event)">
                    <soa:ClientGrid runat="server" ID="roleGrid" OnCellCreatingEditor="createRoleGridNewRow"
                        AutoBindOnLoad="false" Style="width: 100%">
                        <Columns>
                            <soa:ClientGridColumn DataField="RoleID" DataType="String" />
                        </Columns>
                    </soa:ClientGrid>
                </div>
                <div style="text-align: right" class="pc-container5">
                    <button class="pcdlg-button" type="button" onclick="addRoleClick()" id="btnAddRole"
                        runat="server">
                        <i class=""></i>添加角色...
                    </button>
                </div>
            </div>
            <div class="pc-pmlist">
                <div>
                    授权用户的权限
                </div>
                <div class="pc-scrollgrid" onclick="pmListClick(event);" style="height: auto; overflow: hidden;">
                    <soa:ClientGrid runat="server" ID="permissionGrid" AutoBindOnLoad="false" Style="width: 100%"
                        OnCellCreatingEditor="createPMGridNewRow">
                        <Columns>
                            <soa:ClientGridColumn DataField="Name" DataType="String" />
                        </Columns>
                    </soa:ClientGrid>
                </div>
            </div>
        </div>
    </div>
    <div class="pcdlg-floor">
        <div class="pcdlg-button-bar" style="overflow: auto">
            <asp:CheckBox Text="可继承" runat="server" ID="chkInherit" title="当勾选时，表示替换继承子对象权限时，被父级权限替换。" />
            <asp:Button Text="替换继承子对象(R)" runat="server" ID="buttonOverride" AccessKey="R" CssClass="pcdlg-button btn-override"
                Width="150" />
            <asp:Button Text="替换所有子对象(A)" runat="server" ID="buttonOverrideAll" AccessKey="R"
                CssClass="pcdlg-button btn-overrideAll" Width="150" />
            <input type="button" id="buttonSave" runat="server" onclick="onOkClick();" accesskey="S"
                class="pcdlg-button btn-def" value="保存(S)" /><input type="button" accesskey="C" class="pcdlg-button btn-cancel"
                    onclick="return onCancelClick();" value="关闭(C)" />
        </div>
    </div>
    <div>
        <soa:PostProgressControl runat="server" ID="ppcOverride" ControlIDToShowDialog="buttonOverride"
            OnClientBeforeStart="prepareData" DialogHeaderText="替换授权信息" DialogTitle="替换授权信息"
            OnDoPostedData="DoOverridePostData" />
        <soa:PostProgressControl runat="server" ID="ppcOverrideAll" ControlIDToShowDialog="buttonOverrideAll"
            OnClientBeforeStart="prepareData2" DialogHeaderText="替换授权信息" DialogTitle="替换授权信息"
            OnDoPostedData="DoOverridePostData" />
        <input type="hidden" runat="server" id="postData" />
        <input type="hidden" runat="server" id="hfInherit" />
        <div style="display: none">
            <soa:SubmitButton runat="server" ID="btPostSave" OnClick="HandleSaveClick" OnClientClick="onOkClick"
                AccessKey="S" Text="保存(S)" CssClass="pcdlg-button btn-def" RelativeControlID="buttonSave"
                PopupCaption="正在保存..." />
        </div>
    </div>
    </form>
    <div style="display: none">
        <input type="hidden" id="actionData" runat="server" />
    </div>
    <script src="AclEdit.js" type="text/javascript"></script>
    <asp:Literal runat="server" ID="extScript" Mode="PassThrough">
		<script type="text/javascript">
		    //			var contextData = { ContainerID: "abc" };
		    //			contextData.Permissions = [{ "Name": "AddChildren", "DisplayName": "增加子对象", "Description": "" }, { "Name": "DeleteChildren", "DisplayName": "删除子对象", "Description": "" }, { "Name": "UpdateChildren", "DisplayName": "修改子对象", "Description": "" }, { "Name": "EditPermissionsOfChildren", "DisplayName": "修改子对象的权限", "Description": "" }, { "Name": "EditMembersOfGroups", "DisplayName": "修改群组中的人员", "Description": ""}];
		    //			contextData.AclMembers = [{ "ContainerID": "C5D914AC-9C53-42DC-8A32-5E7652C4691C", "ContainerPermission": "EditPermissionsOfChildren", "MemberID": "B7C4C54C-C241-4704-9079-230CE9F61B53", "VersionStartTime": "\/Date(1352123000167)\/", "VersionEndTime": "\/Date(253392422400000)\/", "Status": 1, "SortID": 0, "ContainerSchemaType": "Organizations", "MemberSchemaType": "Roles"}];
		    //			contextData.Roles = [{ "RoleID": "B7C4C54C-C241-4704-9079-230CE9F61B53", "RoleName": "系统维护员", "RoleDisplayName": "系统维护员", "RoleCodeName": "系统维护员", "ApplicationID": "D6AD7E87-0343-43A0-A5C9-93BD1432A260", "ApplicationName": "机构人员管理", "ApplicationDisplayName": "机构人员管理"}];
		</script>
    </asp:Literal>
</body>
</html>
