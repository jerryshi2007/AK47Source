<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DynamicFormView.aspx.cs" Inherits="WfFormTemplate.Forms.DynamicFormView"  %>

<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="SOA" %>
<%@ Register TagPrefix="cc1" Namespace="MCS.Web.WebControls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <base target="_self" />
    <title>动态表单</title>
    <link href="../css/form.css" rel="stylesheet" type="text/css" />
    <link type="text/css" href="/MCSWebApp/CSS/toolbar.css" rel="stylesheet" />
	<link href="../css/css.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        function onValueChanged() {
            $HBRootNS.WfMoveToControl.refreshCurrentProcess();
        }

        function onCreatingEditor(grid, e) {
            if (e.column.dataField == "Data1") {
                var parent = e.editor.get_htmlCell();
                var template = $find("CommonAutoCompleteWithSelectorControl1");
                var newControl = template.cloneAndAppendToContainer(parent);

                newControl.add_selectedDataChanged(function (data) {
                    e.editor.set_dataFieldDataByEvent(data);
                });
            }
        }

        function onDataChanged(grid, e) {
            alert(e);
        }
      
    </script>
</head>
<body>
    <form id="serverForm" runat="server">
    <div>
        <asp:ScriptManager ID="scriptManager" runat="server" EnableScriptGlobalization="true">
        </asp:ScriptManager>  
    </div>
    <div>
        <SOA:WfMoveToControl ID="moveToControl" runat="server" ControlIDToMoveTo="toolbarMoveTo"
            ClientCheckSelectdUsers="false" ControlIDToSave="toolbarSave" OnAfterCreateExecutor="moveToControl_AfterCreateExecutor"
            OnProcessChanged="moveToControl_ProcessChanged" />
        <SOA:WfReturnControl ID="returnControl" runat="server" TargetControlID="toolbarReturn" />
        <SOA:WfWithdrawControl ID="withdrwaControl" runat="server" TargetControlID="toolbarDoWithdraw" />
        <SOA:WfCopyFormControl ID="WfCopyFormControl1" runat="server" TargetControlID="toolbarCopy">
        </SOA:WfCopyFormControl>
        <SOA:WfRuntimeViewerWrapperControl ID="WfRuntimeViewerWrapperControl1" runat="server"
            TargetControlID="toolbarAppTrace" DialogTitle="流程跟踪" />
    </div>
    <div id="top">
        <SOA:WfToolBar ID="toolbar1" runat="server" TemplatePath="../Templates/toolbarTemplate2.ascx" />
    </div>
    <div id="outer">
        <table width="95%" border="0" align="center" cellpadding="0" cellspacing="0" class="header">
            <tr>
                <td width="134">
                    <img src="../img/task_03.jpg" width="134" height="46" style="vertical-align: bottom" />
                </td>
                <td class="rightbg">
                    &nbsp;
                    <asp:Label ID="lbTaskName" runat="server"></asp:Label>
                </td>
            </tr>
        </table>
        <table width="95%" border="0" align="center" cellpadding="0" cellspacing="0" class="nav">
            <tr>
                <td class="title">
                    <p>
                        流程</p>
                    <p>
                        导航</p>
                </td>
                <td>
                    <SOA:ProcessNavigator ID="procNavigator" runat="server" Width="100%" />
                </td>
            </tr>
        </table>
        <table width="95%" border="0" align="center" cellpadding="0" cellspacing="0">
            <tr>
                <td valign="top">
                    <table class="outerTable" style="width: 100%" border="0" align="center" cellpadding="0"
                        cellspacing="0">                        
                     <%--   <tr>
                            <td>
                                <div id="div_ddls" runat="server">

                                    <SOA:HBRadioButtonList ID="HBRadioButtonList1" runat="server" OnClientValueChanged="onValueChanged">
                                    <asp:ListItem Text="!Finish" Value="0"></asp:ListItem>
                                     <asp:ListItem Text="Finish" Selected="True" Value="1"></asp:ListItem>
                                    </SOA:HBRadioButtonList>
                                    <SOA:OuUserInputControl ID="OuUserInputControl1" runat="server" OnClientSelectedDataChanged="onValueChanged" />
                                    <SOA:CommonAutoCompleteWithSelectorControl ID="CommonAutoCompleteWithSelectorControl1"
                                        runat="server" ClientDataKeyName="Code" ClientDataDisplayPropName="Name" ClientDataDescriptionPropName="Detail" 
                                        DataTextFields="Name,Detail" ShowCheckIcon="false" ShowSelector="false" PopupListWidth="300"
                                        ongetdatasource="CommonAutoCompleteWithSelectorControl1_GetDataSource" />
                                </div>
                            </td>
                        </tr>
                        <tr>
                        <td>
                        
                            <SOA:ClientGrid ID="ClientGrid1" runat="server" OnCellCreatingEditor="onCreatingEditor"  OnDataChanged="onDataChanged" ShowEditBar="true">
                            <Columns>
                            <SOA:ClientGridColumn DataField="Data1" HeaderText="选数据"
                            ItemStyle="{width:'96%'}">                            
                            </SOA:ClientGridColumn>

                             <SOA:ClientGridColumn DataField="Data2" HeaderText="选数据2"
                                ItemStyle="{width:'96%'}">
                                <EditTemplate EditMode="TextBox" />                            
                            </SOA:ClientGridColumn>
                            </Columns>
                            </SOA:ClientGrid>
                        </td>
                        </tr>--%>
                        <tr>
                            <td >
                                    <div style="display:none">
        <SOA:HBDropDownList ID="ddlForGrid" runat="server">
            <Items>
                <asp:ListItem Value="1">1</asp:ListItem>
                <asp:ListItem Value="2">2</asp:ListItem>
                <asp:ListItem Value="3">3</asp:ListItem>
            </Items>
        </SOA:HBDropDownList></div>
                                 <SOA:PropertyForm runat="server" ID="propertyForm" Width="100%" />
                                <%--<SOA:UEditorWrapper ID="UEditorWrapper1" runat="server" />--%>
                            </td>
                            

                        </tr>
                        <tr>
                            <td id="tdOpinionListView" runat="server">
                                <SOA:SimpleOpinionListView ID="SimpleOpinionListView1" runat="server">
                                </SOA:SimpleOpinionListView>
                                <SOA:OpinionListView runat="server" ID="Opinion">
                                </SOA:OpinionListView>
                            </td>
                        </tr>
                    </table>
                </td>
                
                
            </tr>
        </table>
    </div>
            <SOA:RelativeLink ID="RelativeLink1" runat="server" RelativeLinkPosition="Right"  RelativeLinkStatus="Collapsed" MoreLinkCategory="fffffff" AlwaysVerticalCenter="False" DockContainer="outer" ExtendContent="鸟儿飞过旷野。一批又一批成,群的鸟儿接连不断地飞了过去。" />        
    </form>
</body>
</html>