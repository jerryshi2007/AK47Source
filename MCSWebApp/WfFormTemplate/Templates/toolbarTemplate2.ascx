<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="toolbarTemplate2.ascx.cs"
    Inherits="WebTestProject.Templates.toolbarTemplate2" %>
<style type="text/css">
    .btnText
    {
        padding: 4px 18px;
        font-family: 微软雅黑;
        font-size: 16px;
        color: #efe;
        background-position: 1px 6px;
        background-repeat: no-repeat;
    }
    
    .btnMoveTo
    {
        background-image: url('/MCSWebApp/images/toolbar/send.gif');
    }
    
    .btnMoveToAndNotify
    {
        background-image: url('/MCSWebApp/images/toolbar/sendwithnotice.gif');
    }
    .btnMoveToAndFYI
    {
        background-image: url('/MCSWebApp/images/toolbar/sendwithread.gif');
    }
    .btnMoveToAddApprover
    {
        background-image: url('/MCSWebApp/images/toolbar/addapprover.gif');
    }
    .btnChangeApprover
    {
        background-image: url('/MCSWebApp/images/toolbar/changeapprover.gif');
    }
    
    
    .btnConsign
    {
        background-image: url('/MCSWebApp/images/toolbar/consign.gif');
    }
    .btnCirculate
    {
        background-image: url('/MCSWebApp/images/toolbar/sendwithread.gif');
    }
    .btnTerminate
    {
        background-image: url('/MCSWebApp/images/toolbar/finish.gif');
    }
    .btnReject
    {
        background-image: url('/MCSWebApp/images/toolbar/rejection.gif');
    }
    .btnDoWithdraw
    {
        background-image: url('/MCSWebApp/images/toolbar/doReject.gif');
    }
    .btnDoAbortAndRestart
    {
        background-image: url('/MCSWebApp/images/toolbar/restart.png');
    }
    .btnDoAbort
    {
        background-image: url('/MCSWebApp/images/toolbar/del.gif');
    }
    .btnAssignment
    {
        background-image: url('/MCSWebApp/images/toolbar/jiaoban.gif');
    }
    .btnPress
    {
        background-image: url('/MCSWebApp/images/toolbar/press.gif');
    }
    .btnRead
    {
        background-image: url('/MCSWebApp/images/toolbar/readed.gif');
    }
    
    .btnReturn
    {
        background-image: url('/MCSWebApp/images/toolbar/rejection.gif');
    }
    
    .btnAbort
    {
        background-image: url('/MCSWebApp/images/toolbar/del.gif');
    }
    
    
    .btnAppTrace
    {
        background-image: url('/MCSWebApp/images/toolbar/tracker.gif');
    }
    
    .btnSave
    {
        background-image: url('/MCSWebApp/images/toolbar/save.gif');
    }
    
    .btnPrint
    {
        background-image: url('/MCSWebApp/images/toolbar/print.gif');
    }
</style>
<table class="wftoolbar" cellpadding="0px" cellspacing="0px">
    <tr>
        <td>
            <table class="bar">
                <tr style="text-align: center;">
                    <td>
                        <asp:LinkButton ID="toolbarMoveTo" runat="server" category="OAExtWebControls" class="invisible">
                            <span  class="btnMoveTo btnText">发送</span> 
                        </asp:LinkButton>
                    </td>
                    <td>
                        <asp:LinkButton ID="toolbarMoveToAndNotify" runat="server" title="送签并且在流程结束时收到通知"
                            category="OAExtWebControls" class="invisible">
                            <span class="btnMoveToAndNotify btnText">送签并通知</span>
                        </asp:LinkButton>
                    </td>
                    <td>
                        <asp:LinkButton ID="toolbarMoveToAndFYI" runat="server" category="OAExtWebControls"
                            class="invisible">
                            <span class="btnMoveToAndFYI btnText">送签并传阅</span>
                        </asp:LinkButton>
                    </td>
                    <td>
                        <asp:LinkButton ID="toolbarMoveToAddApprover" runat="server" category="OAExtWebControls"
                            class="invisible">
                            <span class="btnMoveToAddApprover btnText">加签</span>
							<%--<img style='vertical-align:bottom;' src='/MCSWebApp/images/toolbar/addapprover.gif' />--%>
                        </asp:LinkButton>
                    </td>
                    <td>
                        <asp:LinkButton ID="toolbarChangeApprover" runat="server" category="OAExtWebControls"
                            class="invisible">
                            <span class="btnChangeApprover btnText">转签</span>

							<%--<img style='vertical-align:bottom;' src='/MCSWebApp/images/toolbar/changeapprover.gif' />--%>
                        </asp:LinkButton>
                    </td>
                    <td>
                        <asp:LinkButton ID="toolbarConsign" runat="server" category="OAExtWebControls" class="invisible">
                            <span class="btnConsign btnText">会签</span>
							<%--<img style='vertical-align:bottom;' src='/MCSWebApp/images/toolbar/consign.gif' />--%>
                        </asp:LinkButton>
                    </td>
                    <td>
                        <asp:LinkButton ID="toolbarCirculate" runat="server" category="OAExtWebControls"
                            class="invisible">
                            <span class="btnCirculate btnText">传阅</span>
                            <%--<img style='vertical-align:bottom;' src='/MCSWebApp/images/toolbar/sendwithread.gif' />--%>
                        </asp:LinkButton>
                    </td>
                    <td>
                        <asp:LinkButton ID="toolbarTerminate" runat="server" title="强制结束当前流程" category="OAExtWebControls"
                            class="invisible">
                            <span class="btnTerminate btnText">结束</span>
							<%--<img style='vertical-align:bottom;' src='/MCSWebApp/images/toolbar/finish.gif' />--%> 
                        </asp:LinkButton>
                    </td>
                    <td>
                        <asp:LinkButton ID="toolbarReject" runat="server" category="OAExtWebControls" class="invisible">
                            <span class="btnReject btnText">退回</span>
							<%--<img style='vertical-align:bottom;' src='/MCSWebApp/images/toolbar/rejection.gif' />--%>
                        </asp:LinkButton>
                    </td>
                    <td>
                        <asp:LinkButton ID="toolbarDoWithdraw" runat="server" category="OAExtWebControls"
                            class="invisible">
                            <span class="btnDoWithdraw btnText">撤回</span>
							<%--<img style='vertical-align:bottom;' src='/MCSWebApp/images/toolbar/doReject.gif' />--%>
                        </asp:LinkButton>
                    </td>
                    <td>
                        <asp:LinkButton ID="toolbarDoAbortAndRestart" Visible="false" runat="server" title="作废当前流程，并且生成一份新的表单"
                            category="OAExtWebControls" class="invisible">
                            <span class="btnDoAbortAndRestart btnText">重启</span>
							<%--<img style='vertical-align:bottom;' width=16 height=16 src='/MCSWebApp/images/toolbar/restart.png' />--%> 
                        </asp:LinkButton>
                    </td>
                    <td>
                        <asp:LinkButton ID="toolbarDoAbort" runat="server" category="OAExtWebControls" class="invisible">
                            <span class="btnDoAbort btnText">作废</span>
							<%--<img style='vertical-align:bottom;' src='/MCSWebApp/images/toolbar/del.gif' />--%>
                        </asp:LinkButton>
                    </td>
                    <td>
                    </td>
                    <td>
                        <asp:LinkButton ID="toolbarAssignment" runat="server" category="OAExtWebControls"
                            class="invisible">
                            <span class="btnAssignment btnText">交办</span>
							<%--<img style='vertical-align:bottom;' src='/MCSWebApp/images/toolbar/jiaoban.gif' />--%>
                        </asp:LinkButton>
                    </td>
                    <td>
                        <asp:LinkButton ID="toolbarPress" runat="server" category="OAExtWebControls" class="invisible">
                            <span class="btnPress btnText">催办</span>
							<%--<img style='vertical-align:bottom;' src='/MCSWebApp/images/toolbar/press.gif' />--%>
                        </asp:LinkButton>
                    </td>
                    <td>
                        <asp:LinkButton ID="toolbarRead" runat="server" category="OAExtWebControls" class="invisible">
                            <span class="btnRead btnText">已阅</span>
							<%--<img style='vertical-align:bottom;' src='/MCSWebApp/images/toolbar/readed.gif' />--%>
                        </asp:LinkButton>
                    </td>
                    <td>
                        <asp:LinkButton ID="toolbarReturn" runat="server" category="OAExtWebControls" class="invisible">
                            <span class="btnReturn btnText">退回</span>
							<%--<img style='vertical-align:bottom;' src='/MCSWebApp/images/toolbar/rejection.gif' />--%>
                        </asp:LinkButton>
                    </td>
                    <td>
                        <asp:LinkButton ID="toolbarAbort" runat="server" category="OAExtWebControls" class="invisible">
                            <span class="btnAbort btnText">取消</span>
							<%--<img style='vertical-align:bottom;' src='/MCSWebApp/images/toolbar/del.gif' />--%>
                        </asp:LinkButton>
                    </td>
                    <td>
                        <asp:LinkButton ID="toolbarAppTrace" runat="server" category="OAExtWebControls" class="invisible">
                            <span class="btnAppTrace btnText">流程跟踪</span>
							<%--<img style='vertical-align:bottom;' src='/MCSWebApp/images/toolbar/tracker.gif' />--%>
                        </asp:LinkButton>
                    </td>
                    <td>
                        <asp:LinkButton ID="toolbarSave" runat="server" category="OAExtWebControls" class="invisible">
                            <span class="btnSave btnText">保存</span>
							<%--<img style='vertical-align:bottom;' src='/MCSWebApp/images/toolbar/save.gif' />--%>
                        </asp:LinkButton>
                    </td>
                    <td>
                        <asp:LinkButton ID="toolbarPrint" runat="server" category="OAExtWebControls" class="invisible">
                            <span class="btnPrint btnText">打印</span>
							<%--<img style='vertical-align:bottom;' src='/MCSWebApp/images/toolbar/print.gif' />--%>
                        </asp:LinkButton>
                    </td>
                    <td>
                        <asp:LinkButton ID="toolbarCopy" runat="server" category="OAExtWebControls" class="invisible">
                            <span class="btnSave btnText">复制</span>
                        </asp:LinkButton>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
