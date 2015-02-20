<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TemplateView.aspx.cs" Inherits="WfFormTemplate.Forms.TemplateView" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>表单模版</title>
    <link type="text/css" href="/MCSWebApp/CSS/toolbar.css" rel="stylesheet" />
    <link href="../css/css.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        function onValueChanged() {
            $HBRootNS.WfMoveToControl.refreshCurrentProcess();
        }
    </script>
</head>
<body>
    <form id="serverForm" runat="server">
    <div>
        <SOA:LockContextControl runat="server" ID="lockContext" />
        <SOA:DataBindingControl runat="server" ID="bindingControl" AutoBinding="true" IsValidateOnSubmit="false">
            <ItemBindings>
                <SOA:DataBindingItem DataPropertyName="Subject" ControlID="Subject" Direction="Both"
                    CollectToProcessParameters="true" />
                <SOA:DataBindingItem ControlID="AdministrativeUnit" DataPropertyName="AdministrativeUnit"
                    CollectToProcessParameters="true" />
                <SOA:DataBindingItem ControlID="CostCenter" DataPropertyName="CostCenter" CollectToProcessParameters="true" />
                <SOA:DataBindingItem ControlID="Amount" DataPropertyName="Amount" FormatDefaultValueToEmpty="false"
                    CollectToProcessParameters="true" />
                <SOA:DataBindingItem ControlID="attachmentsControl" ControlPropertyName="Materials"
                    Direction="dataToControl" DataPropertyName="Attachments">
                </SOA:DataBindingItem>
                <SOA:DataBindingItem ControlID="opinionListView" DataPropertyName="Opinions" Direction="DataToControl" />
            </ItemBindings>
        </SOA:DataBindingControl>
    </div>
    <div>
        <SOA:WfMoveToControl ID="moveToControl" runat="server" ControlIDToMoveTo="toolbarMoveTo"
            ControlIDToSave="toolbarSave" OnAfterCreateExecutor="moveToControl_AfterCreateExecutor" OnProcessChanged="moveToControl_ProcessChanged" />
        <SOA:WfWithdrawControl runat="server" ID="withdrawControl" TargetControlID="toolbarDoWithdraw"
            AllowWithdrawWhenClosed="true" />
        <SOA:WfCirculateControl ID="circulateControl" runat="server" TargetControlID="toolbarCirculate" />
        <SOA:WfRuntimeViewerWrapperControl ID="WfRuntimeViewerWrapperControl1" runat="server"
            TargetControlID="toolbarAppTrace" DialogTitle="流程跟踪" />
        <SOA:WfConsignControl ID="consignControl" runat="server" TargetControlID="toolbarConsign" />
        <SOA:WfAddApproverControl ID="addApprovecontrol" runat="server" TargetControlID="toolbarMoveToAddApprover"
            AddApproverMode="AppendCurrentActivity,AreAssociatedActivities" />
        <SOA:WfReturnControl ID="returnControl" runat="server" TargetControlID="toolbarReturn" />
        <SOA:WfAbortControl ID="abortControl" runat="server" TargetControlID="toolbarAbort"
            NeedAbortReason="true" />
        <SOA:WfPauseControl ID="pauseControl" runat="server" TargetControlID="toolbarPause" />
    </div>
    <div id="top">
        <SOA:WfToolBar ID="toolbar" runat="server" TemplatePath="../Templates/toolbarTemplate.ascx" />
    </div>
    <div id="outer">
        <div class="header">
            <div class="header-rbg">
                <div class="header-lbg">
                    <asp:Label ID="lbTaskName" runat="server">表单模版</asp:Label></div>
            </div>
        </div>
        <table class="nav">
            <tr>
                <td class="title">
                    <p>
                        流程</p>
                    <p>
                        导航</p>
                </td>
                <td>
                    <SOA:ProcessNavigator ID="procNavigator" runat="server" />
                </td>
            </tr>
        </table>
        <div class="t-form-content">
            <div class="lefttitle t-form-caption">
                表单模版
            </div>
            <table class="t-form">
                <tr class="t-leading">
                    <th class="t-form-hcell">
                    </th>
                    <td class="t-form-cell">
                    </td>
                    <th class="t-form-hcell">
                    </th>
                    <td class="t-form-cell">
                    </td>
                    <th class="t-form-hcell">
                    </th>
                    <td class="t-form-cell">
                    </td>
                </tr>
                <tr>
                    <th class="fieldtitle">
                        计划名称
                    </th>
                    <td colspan="5" class="t-text-field">
                        <div class="t-width-wrapper">
                            <SOA:HBTextBox runat="Server" ID="Subject" Width="100%" />
                        </div>
                    </td>
                </tr>
                <tr>
                    <th class="fieldtitle">
                        附件
                    </th>
                    <td colspan="5">
                        <SOA:MaterialControl ID="attachmentsControl" runat="server" Width="100%" MaterialTableShowMode="inline"
                            AllowEdit="true" AllowEditContent="true" RootPathName="GenericProcess" DefaultClass="Attachments" />
                    </td>
                </tr>
                <tr>
                    <th class="fieldtitle">
                        管理单元
                    </th>
                    <td>
                        <SOA:HBDropDownList ID="AdministrativeUnit" runat="server" Height="20px" Width="80%"
                            onchange="onValueChanged();">
                            <asp:ListItem Value="Group" Text="集团总部管理单元" />
                            <asp:ListItem Value="BJ_Unit" Text="北京管理单元" />
                        </SOA:HBDropDownList>
                    </td>
                    <th class="fieldtitle">
                        成本中心
                    </th>
                    <td>
                        <SOA:HBDropDownList ID="CostCenter" runat="server" Height="20px" Width="80%" onchange="onValueChanged();">
                            <asp:ListItem Value="1001" Text="1001" />
                            <asp:ListItem Value="1002" Text="1002" />
                            <asp:ListItem Value="1003" Text="1003" />
                        </SOA:HBDropDownList>
                    </td>
                    <th class="fieldtitle">
                        金额
                    </th>
                    <td>
                        <SOA:HBDropDownList ID="Amount" runat="server" Height="20px" Width="80%" onchange="onValueChanged();">
                            <asp:ListItem Value="0" Text="0" />
                            <asp:ListItem Value="5000" Text="5000" />
                            <asp:ListItem Value="10000" Text="10000" />
                        </SOA:HBDropDownList>
                    </td>
                </tr>
                <tr>
                    <th colspan="6" class="lefttitle">
                        <img alt="" src="../Images/icon_01.gif" />审批
                    </th>
                </tr>
                <tr class="commonRow">
                    <td colspan="6">
                        <SOA:OpinionListView runat="server" ID="opinionListView" TitleShowMode="ShowActivityNameAndTitle"
                            Width="100%" EnableUserPresence="true" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <SOA:CustomerServiceExecutiveLink runat="server" />
    </form>
</body>
</html>
