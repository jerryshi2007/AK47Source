<%@ Page Title="权限中心同步到AD" Language="C#" MasterPageFile="~/dialogs/MaintainMaster.Master"
	AutoEventWireup="true" CodeBehind="ADSync.aspx.cs" Inherits="PermissionCenter.Dialogs.ADSync" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<ul class="pc-tabs-header3 clearfix">
		<li><a href="Maintain.aspx">常规</a></li><li class="pc-active"><a href="ADSync.aspx">AD同步</a></li><li>
			<a href="ADReverseSync.aspx">AD反向同步</a></li>
	</ul>
	<div class="pc-tabs-content3">
		<div class="pc-tabs-content pc-active" style="clear: none">
			<fieldset class="pc-profile-group">
				<legend>活动目录同步</legend>
				<div>
					<asp:LinkButton runat="server" CssClass="pc-button" ID="btnSync" OnClick="btnSync_Click">
							<span class="pc-icon16" style="background-image: url('../images/ad.png');"></span>
							立即同步<span class="pc-icon-loader pc-hide" id="Span1"></span></asp:LinkButton><span
								class="pc-description">将权限中心中的用户和组织数据，同步到AD控制器。</span>
				</div>
				<div>
					<iframe width="100%" height="500" frameborder="0" marginwidth="100%" marginheight="100%"
						style="height: 500px" scrolling="auto" src='<%=GetIframeUrl() %>'>你的浏览器不支持iframe，请更换最新现代浏览器。
					</iframe>
				</div>
			</fieldset>
		</div>
	</div>
	<script type="text/javascript">
		window.setInterval(function () {

		}, 3000);
	
	</script>
</asp:Content>
