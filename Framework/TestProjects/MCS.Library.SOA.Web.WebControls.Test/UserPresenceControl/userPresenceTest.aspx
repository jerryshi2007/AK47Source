<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="userPresenceTest.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.UserPresenceControl.userPresenceTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>测试UserPrecense控件</title>
	<base target="_self" />
	<script type="text/javascript">
		function onShowDialog() {
			window.showModalDialog("userPresenceTest.aspx");
		}
		function Button1_onclick() {
//		    ChangeImgToImnElement($get("fanhyLogo"), "sip:v-fengll@sinooceanland.com");
//		    ProcessImnMarkers([$get("fanhyLogo")]);

		    ChangeDivToImnElement($get("ucLocation"), "sip:v-fengll@sinooceanland.com");
		    ProcessImnMarkersByDiv([$get("ucLocation")]);
		}

    </script>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		请输入需要显示状态的用户：
	</div>
	<div>
		<SOA:UserPresence runat="server" ID="userPresence" ShowUserDisplayName="true" />
		<input type="button" value="Show Dialog" onclick="onShowDialog();" />
	</div>
	<div>
		<SOA:OuUserInputControl MultiSelect="true" ID="userInput" runat="server" ShowDeletedObjects="true"
			InvokeWithoutViewState="true" Width="240px" SelectMask="User" />
		<asp:Button runat="server" ID="showPresenceBtn" Text="显示状态" OnClick="showPresenceBtn_Click" />
		<asp:Button runat="server" ID="showPresenceWithScript" Text="通过脚本渲染显示状态" OnClick="showPresenceWithScript_Click" />
	    <input id="Button1" type="button" value="客户端设置" onclick="return Button1_onclick()" /></div>
	<div>
		点击“显示状态”后的结果
	</div>
	<div runat="server" id="usersPresenceContainer" style="padding: 8px 8px 8px 8px">
	</div>
    
	<div style="clear:both;">
		点击“通过脚本渲染显示状态”后的结果
	</div>
	<div runat="server" id="usersPresenceScriptResultContainer" style="padding: 8px 8px 8px 8px">
        
	</div> 
     <div style="display:inline;" id="ucLocation">点按钮我就变！</div><span>aaaaaa</span>
	</form>

   
</body>
</html>
