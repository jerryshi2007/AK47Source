<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataBindingControlTestPage.aspx.cs"
	Inherits="MCS.OA.Web.WebControls.Test.DataBindingControl.DataBindingControlTestPage" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="HB" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="CCIC" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>数据绑定测试控件</title>
	<style type="text/css">
		.label
		{
			font-weight: bold;
		}
	</style>

	<script type="text/javascript">
		function onValidateGroup1() {
			var result = $HBRootNS.DataBindingControl.checkBindingControlDataByGroup(1);

			if (!result.isValid)
				alert(result.errorMessages.join("\n"));
		}
	</script>

</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<HB:DataBindingControl runat="server" ID="bindingControl" IsValidateOnSubmit="true"
			AutoBinding="false" ValidateUnbindProperties="false" AllowClientCollectData="true">
			<ItemBindings>
				<HB:DataBindingItem ControlID="userNameInput" DataPropertyName="UserName" IsValidateOnBlur="False" />
				<HB:DataBindingItem ControlID="userAgeInput" ControlPropertyName="Text" DataPropertyName="UserAge"
					Format="{0:#,###}" ValidationGroup="1" IsValidateOnBlur="true" ClientDataType="Number" />
				<HB:DataBindingItem ControlID="birthdayInput" DataPropertyName="Birthday" Format="{0:yyyy-MM-dd}"
					ControlPropertyName="Value" ClientDataType="DateTime" />
				<HB:DataBindingItem ControlID="birthdayInput2" DataPropertyName="Birthday" Format="{0:yyyy-MM-dd}"
					ControlPropertyName="Text" />
				<HB:DataBindingItem ControlID="passawayDayInput" DataPropertyName="PassawayDay" Format="{0:yyyy-MM-dd}"
					ControlPropertyName="Value" ClientIsHtmlElement="false" ClientPropName="get_value"
					ClientSetPropName="set_value" />
				<HB:DataBindingItem ControlID="creatorInput" DataPropertyName="Creator.Name" ControlPropertyName="Text"
					ValidationGroup="1" />
				<HB:DataBindingItem ControlID="creatorSelector" DataPropertyName="Creator" ControlPropertyName="SelectedSingleData"
					ClientIsHtmlElement="false" ClientPropName="get_selectedSingleData" />
				<HB:DataBindingItem ControlID="usersSelector" DataPropertyName="Users" ControlPropertyName="SelectedOuUserData"
					ClientIsHtmlElement="false" ClientPropName="get_selectedOuUserData" />
				<HB:DataBindingItem ControlID="temperSelector" ControlPropertyName="SelectedItem.Value"
					DataPropertyName="Temper">
				</HB:DataBindingItem>
				<HB:DataBindingItem ControlID="genderSelector" ControlPropertyName="SelectedValue"
					DataPropertyName="Gender" ClientPropName="value">
				</HB:DataBindingItem>
				<HB:DataBindingItem ControlID="genderDesp" ControlPropertyName="Text" DataPropertyName="Gender"
					Direction="DataToControl">
				</HB:DataBindingItem>
				<HB:DataBindingItem ControlID="sizeSelector" ControlPropertyName="SelectedValue"
					DataPropertyName="Size">
				</HB:DataBindingItem>
				<HB:DataBindingItem ControlID="sizeDesp" ControlPropertyName="Text" DataPropertyName="Size"
					Direction="DataToControl">
				</HB:DataBindingItem>
				<HB:DataBindingItem ControlID="TextBox_Income" ControlPropertyName="Text" DataPropertyName="Income"
					Format="{0:#,##0.00}" ClientDataType="Float" AutoFormatOnBlur="false">
				</HB:DataBindingItem>
				<HB:DataBindingItem ControlID="beverageSelect" ControlPropertyName="SelectedValue"
					DataPropertyName="Beverages">
				</HB:DataBindingItem>
			</ItemBindings>
		</HB:DataBindingControl>
		<HB:DataBindingControl runat="server" ID="DataBindingControl2" IsValidateOnSubmit="true"
			AutoBinding="false" ValidateUnbindProperties="false">
			<ItemBindings>
				<HB:DataBindingItem ControlID="userName2" DataPropertyName="UserName2" />
			</ItemBindings>
		</HB:DataBindingControl>
	</div>
	<div>
		<table style="width: 100%">
			<tr>
				<td style="width: 120px">
					<label class="label">
						用户名称
					</label>
				</td>
				<td>
					<asp:TextBox runat="server" ID="userNameInput"></asp:TextBox>
				</td>
			</tr>
			<tr>
				<td>
					<label class="label">
						用户年龄</label>
				</td>
				<td>
					<asp:TextBox runat="server" ID="userAgeInput"></asp:TextBox>
				</td>
			</tr>
			<tr>
				<td>
					<label class="label">
						性别</label>
				</td>
				<td>
					<asp:DropDownList runat="server" ID="genderSelector" Visible="false">
					</asp:DropDownList>
					<asp:Label ID="genderDesp" runat="server" />
				</td>
			</tr>
			<tr>
				<td>
					<label class="label">
						尺寸</label>
				</td>
				<td>
					<asp:DropDownList runat="server" ID="sizeSelector">
					</asp:DropDownList>
					<asp:Label ID="sizeDesp" runat="server" />
				</td>
			</tr>
			<tr>
				<td>
					<label class="label">
						生日</label>
				</td>
				<td>
					<CCIC:DeluxeDateTime ID="birthdayInput" runat="server" TimeAutoComplete="True" TimeMask="99:99:99">
					</CCIC:DeluxeDateTime>
				</td>
			</tr>
			<tr>
				<td>
					<label class="label">
						生日</label>
				</td>
				<td>
					<asp:TextBox runat="server" ID="birthdayInput2"></asp:TextBox>
				</td>
			</tr>
			<tr>
				<td>
					<label class="label">
						光荣日</label>
				</td>
				<td>
					<CCIC:DeluxeCalendar ID="passawayDayInput" runat="server" Enabled="false" />
					<input type="button" value="Clear" onclick="$find('passawayDayInput').set_value(new Date(2010, 5, 13));" />
					<input type="button" value="Get Value" onclick="alert($find('passawayDayInput').get_value());" />
				</td>
			</tr>
			<tr>
				<td>
					<label class="label">
						创建者</label>
				</td>
				<td>
					<div style="width: 256">
						<HB:OuUserInputControl runat="server" ID="creatorSelector" MultiSelect="false" SelectMask="User"
							ListMask="User,Organization" InvokeWithoutViewState="true" />
					</div>
					<div>
						<asp:TextBox runat="server" ID="creatorInput"></asp:TextBox>
					</div>
				</td>
			</tr>
			<tr>
				<td>
					<label class="label">
						办理人</label>
				</td>
				<td>
					<div style="width: 256">
						<HB:OuUserInputControl runat="server" ID="usersSelector" MultiSelect="true" SelectMask="User"
							ListMask="User,Organization" InvokeWithoutViewState="true" />
					</div>
				</td>
			</tr>
			<tr>
				<td>
					<label class="label">
						附件</label>
				</td>
				<td>
					<HB:MaterialControl ID="InvoiceAttachments" runat="server" Width="100%" MaterialTableShowMode="inline"
						FileMaxSize="1048576" RootPathName="GenericProcess" />
				</td>
			</tr>
			<tr>
				<td>
					<label class="label">
						脾气</label>
				</td>
				<td>
					<asp:DropDownList runat="server" ID="temperSelector" Visible="false">
						<asp:ListItem Value="b" Text="Bad"></asp:ListItem>
						<asp:ListItem Value="g" Text="Good"></asp:ListItem>
					</asp:DropDownList>
				</td>
			</tr>
			<tr>
				<td>
					<label class="label">
						收入</label>
				</td>
				<td>
					<asp:TextBox runat="server" ID="TextBox_Income"></asp:TextBox>
				</td>
			</tr>
			<tr>
				<td>
					<label class="label">
						饮料</label>
				</td>
				<td>
					<asp:CheckBoxList runat="server" ID="beverageSelect" />
				</td>
			</tr>
			<tr>
				<td>
					<label class="label">
						User Name2</label>
				</td>
				<td>
					<asp:TextBox runat="server" ID="userName2"></asp:TextBox>
				</td>
			</tr>
		</table>
	</div>
	<div>
		<input type="button" value="客户端收集数据" onclick="onClientCollectData();" />
		<input type="button" value="客户端校验Group1" onclick="onValidateGroup1();" />
		<input type="button" value="客户端校验DataBinding Control2" onclick="checkControl2();" />
		<asp:Button ID="collectDataBtn" runat="server" Text="Collect Data" Width="120px"
			OnClick="collectDataBtn_Click" />
		<input type="button" value="asdf" onclick="checkdata()" />
	</div>
	</form>
	<!------- 外部客户端验证测试 ------->

	<script type="text/javascript">
		function onClientCollectData() {
			var bindingControl = $find('bindingControl');

			var data = bindingControl.collectData();

			bindingControl.dataBind(data);
		}

		var bcontrol;
		//客户端验证全部数据
		function checkdata() {
			var revalue = $HBRootNS.DataBindingControl.checkBindingControlData();
			if (revalue.isValid)//返回是否通过
			{
				alert('通过！');
			}
			else {
				alert(revalue.errorMessages);
			}
		}

		function checkControl2() {
			var bindingControl2 = $find("DataBindingControl2");

			bindingControl2.checkAllData();

			if (bindingControl2.errorMessages.length > 0)
				alert(bindingControl2.errorMessages.join("\n"));
		}

		function test() {
			if (document.getElementById('creatorInput').value.length <= 0) {
				alert('创建人不能为空！');
				return false;
			}
			return true;
		}
		//初始化
		Sys.Application.add_load(function() {
			bcontrol = $find('bindingControl'); //获取绑定控件
			bcontrol.addExEvent(document.getElementById('creatorInput'), test, false, '创建人不能为空！', 1); //注册外部验证事件params: target control ,callback function ,isvalidateonblur,errorMsg
		});
	</script>

	<!------- 外部客户端验证测试 ------->
</body>
</html>
