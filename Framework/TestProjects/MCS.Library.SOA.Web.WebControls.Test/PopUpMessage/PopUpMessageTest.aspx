<%@ Page Language="C#" AutoEventWireup="true" Codebehind="PopUpMessageTest.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.PopUpMessage.PopUpMessageTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>消息提醒测试</title>

	<script type="text/javascript">
	    function test()
	    {
	        alert("");
	    }
	    
	    function show()
	    {
	        var mess = $find('PopUpMessageControl1') ;
	        
	        //mess.set_positionElementID("div1");
	       // mess.set_positionElement($get("div1"));
	        mess.set_positionX( window.screen.width - mess.get_width());
	        mess.set_positionY( window.screen.height - mess.get_height());
	        mess.show();
	    }
	</script>

</head>
<body style="margin: 0 0 0 0">
	<form id="form1" runat="server">
		<table style="width: 100%; height: 100%">
			<tr>
				<td style="vertical-align: top">
					<input type="button" value="show" onclick="show();" />
					<input type="button" value="showTitle" onclick='$find("PopUpMessageControl1").set_showTitle("新待办事项");' />
					
					<cc1:PopUpMessageControl ID="PopUpMessageControl1" runat="server" ShowText="汉字<br>ffff"
						OnClick="test" PlaySoundPath="msg.wav"   />
				</td>
			</tr>
			<tr>
				<td style="height: 1px">
					<div id="div1">
						ssssss</div>
				</td>
			</tr>
		</table>
	</form>
	<script type="text/javascript">
	 window.setInterval(show,6000);
	</script>
</body>
</html>
