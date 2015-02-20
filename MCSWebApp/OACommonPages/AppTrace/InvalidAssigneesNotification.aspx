<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvalidAssigneesNotification.aspx.cs"
	Inherits="MCS.OA.CommonPages.AppTrace.InvalidAssigneesNotification" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<%@ Import Namespace="MCS.Library.OGUPermission" %>
<%@ Import Namespace="MCS.Library.SOA.DataObjects" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>流程调整</title>
	<script type="text/javascript">

		function getDefaultTaskFeature() {
			var width = 820;
			var height = 700;

			var left = (window.screen.width - width) / 2;
			var top = (window.screen.height - height) / 2;

			return "width=" + width + ",height=" + height + ",left=" + left + ",top=" + top + ",status=no,resizable=yes,toolbar=no,menubar=no,location=no,scrollbars=yes";
		}

		function onLinkClick(url) {
			event.returnValue = false;
			//var a = event.srcElement;
			if (url != '') {

				var feature = getDefaultTaskFeature();

				window.open(url, "_blank", feature);
			}
			event.cancelBubble = true;

			return false;
		}

		function onClick() {
			document.getElementById("SubmitbtnSearch").click()
		}
	</script>
</head>
<body style="background-color: #f8f8f8;">
	<form id="form1" runat="server">
	<table width="100%" style="width: 100%; height: 100%">
		<tr>
			<td class="gridHead">
				<div class="dialogTitle">
					<span class="dialogLogo">待调整流程列表</span>
				</div>
			</td>
		</tr>
		<tr>
			<td style="vertical-align: top">
				<div class="dialogContent" id="dialogContent" runat="server" style="width: 100%;
					height: 100%; overflow: auto">
					<MCS:DeluxeGrid ID="dataGrid" runat="server" ShowExportControl="false" PageSize="10"
						TitleFontSize="Small" Width="98%" DataSourceID="objectDataSource" AllowPaging="true"
						AllowSorting="true" AutoGenerateColumns="false" DataKeyNames="ProcessID" ShowCheckBoxes="false"
						CheckBoxPosition="Left" PagerSettings-Mode="NextPreviousFirstLast" PagerSettings-PreviousPageText="上一页"
						PagerSettings-NextPageText="下一页" PagerSettings-Position="TopAndBottom" CaptionAlign="Right"
						CssClass="dataList" TitleCssClass="title" GridTitle="流程调整">
						<Columns>
							<asp:TemplateField HeaderText="流程名称" SortExpression="PROCESS_NAME">
								<ItemTemplate>
									<%#HttpUtility.HtmlEncode((string)Eval("ProcessName"))%>
								</ItemTemplate>
								<ItemStyle HorizontalAlign="Left" />
							</asp:TemplateField>
							<asp:TemplateField HeaderText="当前环节">
								<ItemTemplate>
								    <label><%#HttpUtility.HtmlEncode((string)Eval("ActivityKey"))%></label>
									<SOA:WfStatusControl ID="WfStatusControl1" runat="server" ProcessID='<%# Eval("ProcessID")%>'
										DisplayMode="CurrentUsers" EnableUserPresence="true">
									</SOA:WfStatusControl>
								</ItemTemplate>
								<ItemStyle HorizontalAlign="Left" />
							</asp:TemplateField>
							<asp:TemplateField HeaderText="流程中异常人员">
								<ItemTemplate>
									<%#HttpUtility.HtmlEncode((string)Eval("AllUsers"))%>
								</ItemTemplate>
								<ItemStyle HorizontalAlign="Left" />
							</asp:TemplateField>
						</Columns>
						<HeaderStyle CssClass="head" />
						<RowStyle CssClass="item" />
						<AlternatingRowStyle CssClass="aitem" />
						<SelectedRowStyle CssClass="selecteditem" />
						<PagerStyle CssClass="pager" />
						<EmptyDataTemplate>
							暂时没有您需要的数据
						</EmptyDataTemplate>
						<PagerSettings Mode="NextPreviousFirstLast" NextPageText="下一页" Position="TopAndBottom"
							PreviousPageText="上一页"></PagerSettings>
					</MCS:DeluxeGrid>
					<SOA:DeluxeObjectDataSource ID="objectDataSource" runat="server" EnablePaging="True"
						TypeName="MCS.OA.CommonPages.AppTrace.InvalidAssignessDataSource">
					</SOA:DeluxeObjectDataSource>
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
							<input type="button" id="confirmButton" value="转已办(O)" accesskey="O" runat="server" class="formButton"
								onclick="onClick();" />
							<SOA:SubmitButton runat="server" ID="SubmitbtnSearch" Style="display: none" OnClick="Submitbtn_Click"
								RelativeControlID="confirmButton" PopupCaption="正在转已办..." />
						</td>
						<td style="text-align: center;">
							<input type="button" onclick="top.close();" value="取消(C)" id="btnCancel" accesskey="C"
								class="formButton" />
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
