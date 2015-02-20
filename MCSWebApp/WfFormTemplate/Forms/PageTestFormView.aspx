<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PageTestFormView.aspx.cs" Inherits="WfFormTemplate.Forms.PageTestFormView"  %>

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
                        <tr>
                            <td >
                         
                            </td>
                            

                        </tr>
                        <tr>
                            <td id="tdOpinionListView" runat="server">
                                <SOA:OpinionListView runat="server" ID="Opinion">
                                </SOA:OpinionListView>
                            </td>
                        </tr>
                    </table>
                </td>
                
                
            </tr>
        </table>
    </div>
         
    </form>
</body>
</html>