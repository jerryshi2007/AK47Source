<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="toolbarTemplate.ascx.cs"
	Inherits="WebTestProject.Templates.toolbarTemplate" %>
<table class="wftoolbar" cellpadding="0px" cellspacing="0px">
	<tr>
		<td>
			<table class="bar">
				<tr>
					<td>
						<asp:LinkButton ID="toolbarMoveTo" runat="server" category="SOAWebControls" class="invisible">
                            <img src='/MCSWebApp/images/toolbar/send.gif' />发送</asp:LinkButton>
					</td>
					<td>
						<asp:LinkButton ID="toolbarMoveToAndNotify" runat="server" title="送签并且在流程结束时收到通知"
							category="SOAWebControls" class="invisible">
							<img src='/MCSWebApp/images/toolbar/sendwithnotice.gif' />送签并通知
						</asp:LinkButton>
					</td>
					<td>
						<asp:LinkButton ID="toolbarMoveToAndFYI" runat="server" category="SOAWebControls"
							class="invisible">
							<img src='/MCSWebApp/images/toolbar/sendwithread.gif' />送签并传阅
						</asp:LinkButton>
					</td>
					<td>
						<asp:LinkButton ID="toolbarMoveToAddApprover" runat="server" category="SOAWebControls"
							class="invisible">
							<img src='/MCSWebApp/images/toolbar/addapprover.gif' />加签
						</asp:LinkButton>
					</td>
					<td>
						<asp:LinkButton ID="toolbarChangeApprover" runat="server" category="SOAWebControls"
							class="invisible">
							<img src='/MCSWebApp/images/toolbar/changeapprover.gif' />转签
						</asp:LinkButton>
					</td>
					<td>
						<asp:LinkButton ID="toolbarConsign" runat="server" category="SOAWebControls" class="invisible">
							<img src='/MCSWebApp/images/toolbar/consign.gif' />会签
						</asp:LinkButton>
					</td>
					<td>
						<asp:LinkButton ID="toolbarCirculate" runat="server" category="SOAWebControls" class="invisible">
                            <img src='/MCSWebApp/images/toolbar/sendwithread.gif' />传阅
						</asp:LinkButton>
					</td>
					<td>
						<asp:LinkButton ID="toolbarTerminate" runat="server" title="强制结束当前流程" category="SOAWebControls"
							class="invisible">
							<img src='/MCSWebApp/images/toolbar/finish.gif' />结束
						</asp:LinkButton>
					</td>
					<td>
						<asp:LinkButton ID="toolbarReject" runat="server" category="SOAWebControls" class="invisible">
							<img src='/MCSWebApp/images/toolbar/rejection.gif' />退件
						</asp:LinkButton>
					</td>
					<td>
						<asp:LinkButton ID="toolbarDoWithdraw" runat="server" category="SOAWebControls" class="invisible">
							<img src='/MCSWebApp/images/toolbar/doReject.gif' />撤回
						</asp:LinkButton>
					</td>
					<td>
						<asp:LinkButton ID="toolbarDoAbortAndRestart" Visible="false" runat="server" title="作废当前流程，并且生成一份新的表单"
							category="SOAWebControls" class="invisible">
							<img width=16 height=16 src='/MCSWebApp/images/toolbar/restart.png' />重起
						</asp:LinkButton>
					</td>
					<td>
						<asp:LinkButton ID="toolbarDoAbort" runat="server" category="SOAWebControls" class="invisible">
							<img src='/MCSWebApp/images/toolbar/del.gif' />作废
						</asp:LinkButton>
					</td>
					<td>
					</td>
					<td>
						<asp:LinkButton ID="toolbarAssignment" runat="server" category="SOAWebControls" class="invisible">
							<img src='/MCSWebApp/images/toolbar/jiaoban.gif' />交办
						</asp:LinkButton>
					</td>
					<td>
						<asp:LinkButton ID="toolbarPress" runat="server" category="SOAWebControls" class="invisible">
							<img src='/MCSWebApp/images/toolbar/press.gif' />催办
						</asp:LinkButton>
					</td>
					<td>
						<asp:LinkButton ID="toolbarRead" runat="server" category="SOAWebControls" class="invisible">
							<img src='/MCSWebApp/images/toolbar/readed.gif' />已阅
						</asp:LinkButton>
					</td>
					<td>
						<asp:LinkButton ID="toolbarReturn" runat="server" category="SOAWebControls" class="invisible">
							<img src='/MCSWebApp/images/toolbar/rejection.gif' />退件
						</asp:LinkButton>
					</td>
					<td>
						<asp:LinkButton ID="toolbarAbort" runat="server" category="SOAWebControls" class="invisible">
							<img src='/MCSWebApp/images/toolbar/del.gif' />取消
						</asp:LinkButton>
					</td>
					<td>
						<asp:LinkButton ID="toolbarPause" runat="server" category="SOAWebControls" class="invisible">
							<img src='/MCSWebApp/images/toolbar/delay.gif' />暂停
						</asp:LinkButton>
					</td>
					<td>
						<asp:LinkButton ID="toolbarAppTrace" runat="server" category="SOAWebControls" class="invisible">
							<img src='/MCSWebApp/images/toolbar/tracker.gif' />流程跟踪
						</asp:LinkButton>
					</td>
					<td>
						<asp:LinkButton ID="toolbarSave" runat="server" category="SOAWebControls" class="invisible">
							<img src='/MCSWebApp/images/toolbar/save.gif' />保存
						</asp:LinkButton>
					</td>
					<td>
						<asp:LinkButton ID="toolbarPrint" runat="server" category="SOAWebControls" class="invisible">
							<img src='/MCSWebApp/images/toolbar/print.gif' />打印
						</asp:LinkButton>
					</td>
					<td>
						<asp:LinkButton ID="toolbarCopyForm" runat="server" category="SOAWebControls" class="invisible"
							ToolTip="复制当前表单的数据并且启动一个新流程">
							<img src='/MCSWebApp/images/toolbar/copy.gif' />复制
						</asp:LinkButton>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
