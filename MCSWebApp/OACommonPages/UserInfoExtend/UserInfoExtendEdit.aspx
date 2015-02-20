<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserInfoExtendEdit.aspx.cs" Inherits="MCS.OA.CommonPages.UserInfoExtend.UserInfoExtendEdit" %>


<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="CCIC" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>用户扩展信息修改</title>
	<base target="_self" />
	<link href="../CSS/Ajax.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/Dialog.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/htc.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/ItemDetail.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/Login.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/Other.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/style.css" type="text/css" rel="stylesheet" />
    
	<style type="text/css">
		table
		{
			 
			 width:96%;
	    }
	</style>

    <script language="javascript" type="text/javascript">
        function ValiditionData(filed, length) {
            if (filed.value != "" && filed.id != 'labNation')
                 filed.value = filed.value.replace(/\D/g, '');
            if (filed.value.length > 13) {
                filed.value = "";
                 alert("字符不能过长");
             }
        }
    </script>
</head>
<body>
	<form id="serverForm" runat="server" target="innerFrame">
	<div>
    <HB:DataBindingControl runat="server" ID="bindingControl" AutoValidate="true" IsValidateOnSubmit="true">
			<ItemBindings>
				<HB:DataBindingItem ControlID="labName" DataPropertyName="DisplayName" />
				<HB:DataBindingItem ControlID="radlistGender" DataPropertyName="Gender" ControlPropertyName="SelectedValue" />
				<HB:DataBindingItem ControlID="labNation" DataPropertyName="Nation" IsValidateOnBlur="true" />
				<HB:DataBindingItem ControlID="labMobile" DataPropertyName="Mobile" />
				<HB:DataBindingItem ControlID="labOffice" DataPropertyName="OfficeTel" />
				<HB:DataBindingItem ControlID="DCBirthday" DataPropertyName="Birthday" ControlPropertyName="Value"
					ClientPropName="value" Format="{0:yyyy-MM-dd}" />
				<HB:DataBindingItem ControlID="DCWorkTime" DataPropertyName="StartWorkTime" ControlPropertyName="Value"
					ClientPropName="value" Format="{0:yyyy-MM-dd}" />
				<HB:DataBindingItem ControlID="labIntranet" DataPropertyName="IntranetEmail" />
				<HB:DataBindingItem ControlID="labInternet" DataPropertyName="InternetEmail" />
				<HB:DataBindingItem ControlID="labIMAddress" DataPropertyName="IMAddress" />
				<HB:DataBindingItem ControlID="labMemo" DataPropertyName="MEMO" />
			</ItemBindings>
		</HB:DataBindingControl>
    </div>
    <div id="dcontainer">
		<table cellpadding="0" cellspacing="0" style="width:100%; height:100%;" border="0">
		<tr style="line-height:64px;background-color:White;"><td valign="top">
			<div id="dheader">
				<h1>
				   <asp:Label ID="labName" runat="server" Text="Label"></asp:Label> 的个人信息
				</h1>
			</div>
			</td>
			</tr>
			<tr style="line-height:64px;background-color:White;">
				<td>
					<div id="dcontent">
		
	<table cellspacing="1" cellpadding="0" style="background-color: #D2D2D2;" > 
		<tr style="line-height:64px;background-color:White;">
			<td class="fim_l">性别：</td>
			<td>
                <asp:RadioButtonList ID="radlistGender" Width="83%"  BorderWidth="2px" runat="server" RepeatColumns="2">
                </asp:RadioButtonList>
			</td>
			<td class="fim_l">民族：</td>
			<td>
				<asp:TextBox ID="labNation" runat="server" Width="140px" onkeyup="ValiditionData(this,5)"></asp:TextBox>
			</td>
		</tr>
		<tr  style="line-height:64px;background-color:White;">
			<td class="fim_l">手机号：</td>
			<td>
				<asp:TextBox ID="labMobile" runat="server" Width="140px" onkeyup="ValiditionData(this,12)"></asp:TextBox>
			</td>
			<td class="fim_l">办公室电话：</td>
			<td>
				<asp:TextBox ID="labOffice" runat="server" Width="140px" onkeyup="ValiditionData(this,12)"></asp:TextBox>
			</td>
		</tr>
		<tr  style="line-height:64px;background-color:White;">
			<td class="fim_l">出生日期：</td>
			<td>
				<CCIC:DeluxeCalendar ID="DCBirthday" runat="server" Width="140px">
				</CCIC:DeluxeCalendar>
			</td>
			<td class="fim_l">入职日期：</td>
			<td>
				<CCIC:DeluxeCalendar ID="DCWorkTime" runat="server" Width="140px">
				</CCIC:DeluxeCalendar>
			</td>
		</tr>
		<tr  style="line-height:64px;background-color:White;">
			<td class="fim_l">内网EMail：</td>
			<td>
				<asp:TextBox ID="labIntranet" runat="server" Width="140px"></asp:TextBox>
			</td>
			<td class="fim_l">外网EMail：</td>
			<td>
				<asp:TextBox ID="labInternet" runat="server" Width="140px"></asp:TextBox>
			</td>
		</tr>
		<tr  style="line-height:64px;background-color:White;">
			<td class="fim_l">即时消息：</td>
			<td>
				<asp:TextBox ID="labIMAddress" runat="server" Width="140px"></asp:TextBox>
			</td>
			<td class="fim_l">备注：</td>
			<td>
				<asp:TextBox ID="labMemo" runat="server" Width="140px"></asp:TextBox>
			</td>
		</tr>
	</table>
		
		</div>
				</td>
			</tr>
			<tr  style="line-height:64px;background-color:White;">
				<td style="height:80px;" valign="middle">
				<div id="dfooter">
	<p style="vertical-align:middle; height:40px;">
		<asp:Button ID="BtnSave" CssClass="formButton" runat="server" Text="保存(S)" OnClientClick="ValiditionData" OnClick="submitBtn_Click" />
		<input accesskey="C" id="btnClose" class="formButton" onclick="window.close();" type="button" value="关闭(C)" name="btnClose"/>
	</p>
	</div>
				</td>
			</tr>
		</table>
	</div>
	</form>
	<iframe style="display: none" id="innerFrame" name="innerFrame"></iframe>
</body>
</html>
