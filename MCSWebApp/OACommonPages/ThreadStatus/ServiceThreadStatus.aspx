<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServiceThreadStatus.aspx.cs"
	Inherits="MCS.OA.CommonPages.ThreadStatus.ServiceThreadStatus" %>

<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>服务进程状态监测</title>
	<script type="text/javascript">
		function onDetailMessageClick() {
			var a = getOwnerTag(event.srcElement, "A");

			event.returnValue = false;

			var sFeature = "dialogWidth:460px; dialogHeight:360px;center:yes;help:no;resizable:yes;scroll:no;status:no";

			var arg = new Object();
			arg.message = a.innerText;

			window.showModalDialog("ServiceThreadStatusDetailMessage.aspx?threadName=" + escape(a.threadName), arg, sFeature);
		}

		function onDocumentLoad() {
			window.setInterval(onRefresh, 10000);
		}

		function onRefresh() {
			if (serverForm.readyState == "complete")
				document.all("refreshButton").click();
		}

		function getOwnerTag(element, strTag) {
			while (element.tagName && element.tagName.toUpperCase() != strTag.toUpperCase()) {
				element = element.parentNode;

				if (element == null)
					break;
			}

			return (element);
		}
        
	</script>
</head>
<body onload="onDocumentLoad()" style="background-color: #f8f8f8;">
	<form id="serverForm" runat="server">
	<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="True" EnableScriptGlobalization="True"
		EnableScriptLocalization="True">
	</asp:ScriptManager>
	<div id="container">
		<div style="border-bottom: groove 1px Silver; overflow: hidden; text-align: left;">
			<div style="padding-left: 32px; font-size: 11pt; font-weight: 600; background: url(../images/h2_icon.gif) no-repeat 5px 5px;
				line-height: 30px; padding-bottom: 0px">
				<span id="Span1" runat="server" category="OACommons">服务进程状态监测</span>
			</div>
		</div>
		<div style="margin: 5px;" id="divSearch">
			<table style="background-color: Silver; width: 99%; text-align: left" cellspacing="1px"
				border="0" cellpadding="0">
				<tr style="background-color: White">
					<td style="text-align: center; background-color: #f2f8f8; height: 30px; width: 80px;">
						<span id="Span2" runat="server" category="OACommons">服务器</span> :
					</td>
					<td style="text-align: center; background-color: #f7fbfa; width: 220px;">
						<asp:DropDownList ID="ddlServers" runat="server" Width="200px">
						</asp:DropDownList>
					</td>
					<td style="text-align: left; background-color: #f2f8f8; height: 30px;">
						&nbsp;&nbsp;
						<asp:LinkButton runat="server" ID="queryBtn" OnClick="queryBtn_Click">
							<img id="Img1" src="../../Images/16/search.gif" runat="server" category="Portal"
								alt="查询" style="border: 0;" /><span id="Span3" runat="server" category="Portal">查询</span></asp:LinkButton>
					</td>
				</tr>
			</table>
		</div>
		<div style="margin: 5px;">
			<table style="width: 99%;" cellpadding="0" cellspacing="0">
				<tr>
					<td style="vertical-align: top">
						<asp:UpdatePanel ID="updatePanel" runat="server" UpdateMode="Conditional">
							<ContentTemplate>
								<MCS:DeluxeGrid ID="serviceStatusGrid" runat="server" Width="100%" AutoGenerateColumns="false"
									Category="OACommons" CssClass="dataList" OnRowDataBound="serviceStatusGrid_RowDataBound"
									EmptyDataText="没有线程信息">
									<HeaderStyle CssClass="head" />
									<RowStyle CssClass="item" />
									<Columns>
										<asp:BoundField DataField="Params" HeaderText="线程名称">
											<HeaderStyle Width="10%" />
											<ItemStyle HorizontalAlign="Left" />
										</asp:BoundField>
										<asp:BoundField DataField="Status" HeaderText="执行状态">
											<HeaderStyle Width="10%"></HeaderStyle>
											<ItemStyle HorizontalAlign="Center" />
										</asp:BoundField>
										<asp:BoundField DataField="LastPollTime" HeaderText="最后轮询时间">
											<ItemStyle HorizontalAlign="Left" />
											<HeaderStyle Width="20%" />
										</asp:BoundField>
										<asp:BoundField DataField="LastMessage" HeaderText="最后的消息">
											<HeaderStyle Width="30%"></HeaderStyle>
											<ItemStyle HorizontalAlign="Left" />
										</asp:BoundField>
										<asp:BoundField DataField="LastExceptionMessage" HeaderText="最后的异常">
											<HeaderStyle Width="30%"></HeaderStyle>
											<ItemStyle HorizontalAlign="Left" />
										</asp:BoundField>
									</Columns>
								</MCS:DeluxeGrid>
								<asp:Label ID="errorMessage" ForeColor="red" Font-Bold="true" runat="server"></asp:Label>
								<asp:Button ID="refreshButton" Style="display: none" runat="server" Text="Click...">
								</asp:Button>
							</ContentTemplate>
						</asp:UpdatePanel>
					</td>
				</tr>
			</table>
		</div>
	</div>
	</form>
</body>
</html>
