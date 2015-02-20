<%@ Page Title="AD反向同步到权限中心" Language="C#" MasterPageFile="~/dialogs/MaintainMaster.Master"
	AutoEventWireup="true" CodeBehind="ADReverseSync.aspx.cs" Inherits="PermissionCenter.Dialogs.ADReverseSync" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<ul class="pc-tabs-header3">
		<li><a href="Maintain.aspx">常规</a></li><li><a href="ADSync.aspx">AD同步</a></li><li
			class="pc-active"><a href="ADReverseSync.aspx">AD反向同步</a></li>
	</ul>
	<div class="pc-tabs-content3">
		<div class="pc-tabs-content pc-active" style="clear: none">
			<fieldset class="pc-profile-group">
				<legend>活动目录反向同步</legend>
				<div>
					<asp:LinkButton runat="server" class="pc-button" ID="btnSync" OnClick="btnSync_Click">
							<span class="pc-icon16" style="background-image: url('../images/ad.png');"></span>
							立即同步<span class="pc-icon-loader pc-hide" id="Span1"></span></asp:LinkButton><span
								class="pc-description">将AD中用户的Lync地址和邮件地址同步回权限中心。</span>
				</div>
				<div>
					<iframe width="100%" height="500" frameborder="0" marginwidth="100%" marginheight="100%"
						style="height: 500px" scrolling="auto" src='<%=GetIframeUrl() %>'>你的浏览器不支持iframe，请更换最新现代浏览器。
					</iframe>
				</div>
			</fieldset>
		</div>
	</div>
</asp:Content>
