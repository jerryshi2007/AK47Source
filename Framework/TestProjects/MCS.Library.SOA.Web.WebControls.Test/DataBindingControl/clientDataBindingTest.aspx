<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="clientDataBindingTest.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.DataBindingControl.clientDataBindingTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="SOA" %>
<%@ Register Assembly="MCS.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Client DataBinding Test</title>
	<script type="text/javascript">
		function onBindData() {
			var bindingControl = $find("bindingControl");
			var data = prepareData();

			bindingControl.dataBind(data);
		}

		function onCollectData() {
			var bindingControl = $find("bindingControl");

			var data = bindingControl.collectData();

			$get("result").innerText = dataToString(data);
		}

		function dataToString(data) {
			var strB = new Sys.StringBuilder();

			appendPropertyValue(strB, data, "TimeInput");
			appendPropertyValue(strB, data, "dateInput");
			appendPropertyValue(strB, data, "IntegerInput");
			appendUser(strB, data, "User");

			return strB.toString();
		}

		function appendUser(strB, data, propertyName) {
			var user = data[propertyName];

			if (user) {
				strB.append(propertyName + " id: " + user.id + "\n");
				strB.append(propertyName + " name: " + user.name + "\n");
				strB.append(propertyName + " fullPath: " + user.fullPath + "\n");
			}
		}

		function appendPropertyValue(strB, data, propertyName) {
			strB.append(propertyName + ": " + data[propertyName] + "\n");
		}

		function prepareData() {
			var data = {
				TimeInput: new Date(1972, 3, 26, 12, 45, 50),
				dateInput: new Date(2012, 3, 26),
				User: { id: "80f4464f-e912-40c9-9502-c369a0d935ee", name: "樊海云", displayName: "樊海云", fullPath: "机构人员\\远洋地产\\集团总部\\战略发展部\\樊海云" }
			};

			return data;
		}

		function onClientDataBinding(bindingCtrl, e) {
			/*
			设置e.cancel == true会禁用平台默认的绑定，然后由你自己的绑定机制来接管。
			你可以使用e.control, e.bindingItem 和 e.data 这些属性来访问控件和数据的值。e.bindingItem包含了很多绑定信息。
			一般在这个事件中给e.control中设置值
			*/
		}

		function onClientCollectData(bindingCtrl, e) {
			/*
			设置e.cancel == true会禁用平台默认的收集数据，然后由你自己的收集机制来接管。
			你可以使用e.control, e.bindingItem 和 e.data 这些属性来访问控件和数据的值。e.bindingItem包含了很多绑定信息。
			一般在这个事件中给e.data中设置值
			*/
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<SOA:DataBindingControl runat="server" ID="bindingControl" IsValidateOnSubmit="true"
			AutoBinding="false" ValidateUnbindProperties="false" AllowClientCollectData="true"
			OnClientDataBinding="onClientDataBinding" OnClientCollectData="onClientCollectData">
			<ItemBindings>
				<SOA:DataBindingItem ControlID="timeInput" DataPropertyName="TimeInput" IsValidateOnBlur="False"
					ControlPropertyName="Value" ClientIsHtmlElement="false" ClientPropName="get_value"
					ClientSetPropName="set_value" />
				<SOA:DataBindingItem ControlID="dateInput" DataPropertyName="DateInput" ControlPropertyName="Value"
					AutoFormatOnBlur="true" ClientIsHtmlElement="false" ClientPropName="get_value"
					ClientDataPropertyName="dateInput" ClientSetPropName="set_value" />
				<SOA:DataBindingItem ControlID="integerInput" DataPropertyName="IntegerInput" IsValidateOnBlur="true"
					AutoFormatOnBlur="true" />
				<SOA:DataBindingItem ControlID="OuUserInputControl1" DataPropertyName="User" ControlPropertyName="SelectedSingleData"
					ClientPropName="get_selectedSingleData" ClientSetPropName="set_selectedSingleData"
					AutoFormatOnBlur="true" ClientIsHtmlElement="false" />
			</ItemBindings>
		</SOA:DataBindingControl>
	</div>
	<div>
		<div>
			Time Input:</div>
		<div>
			<MCS:DeluxeDateTime ID="timeInput" runat="server" TimeAutoComplete="true" TimeMask="99:99:99"
				TimeAutoCompleteValue="00:00:00"></MCS:DeluxeDateTime>
		</div>
		<div>
			Date Input:</div>
		<div>
			<MCS:DeluxeCalendar ID="dateInput" runat="server">
			</MCS:DeluxeCalendar>
		</div>
		<div>
			<asp:Label runat="server" ID="postedDateTime"></asp:Label>
		</div>
		<div>
			Integer:</div>
		<div>
			<asp:TextBox runat="server" ID="integerInput"></asp:TextBox>
		</div>
		<div>
			SimpleDataType :</div>
		<div>
			User:
			<br />
			<SOA:OuUserInputControl ID="OuUserInputControl1" runat="server" ReadOnly="false" />
		</div>
		<div>
			<input type="button" value="Collect Data" onclick="onCollectData()" />
			<input type="button" value="Bind Data" onclick="onBindData()" />
		</div>
		<div>
			<textarea id="result" style="width: 480px; height: 250px"></textarea>
		</div>
	</form>
</body>
</html>
