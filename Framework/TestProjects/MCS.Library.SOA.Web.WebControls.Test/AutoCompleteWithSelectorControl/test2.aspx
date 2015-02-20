<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="test2.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.AutoCompleteWithSelectorControl.test2" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="mcs" %>
<%@ Register TagPrefix="cc1" Namespace="MCS.Web.WebControls" %>
<%@ Register TagPrefix="SOA" Namespace="MCS.Web.WebControls" %>
<%@ Import Namespace="MCS.Library.OGUPermission" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<script type="text/javascript">
		function selectData(sender, e) {
			//这个方法仅作为演示，实际应用中实现应该是从Server查询，返回序列化结果，注意要带__type。

			var result = [];

			for (var i = 0; i < 5; i++) {
				var obj = {};
				obj.__type = "MCS.Library.SOA.Web.WebControls.Test.AutoCompleteWithSelectorControl.CommonData, MCS.Library.SOA.Web.WebControls.Test";
				obj.Code = "id" + i;
				obj.Name = "name" + i;
				result.push(obj);
			}

			var control = $find("CommonAutoCompleteWithSelectorControl1");
			var sFeature = "dialogWidth:500px; dialogHeight:300px;center:yes;help:no;resizable:yes;scroll:no;status:no";

			result.nameTable = $NT;
			result.keyName = control.get_dataKeyName();
			result.displayPropName = control.get_dataDisplayPropName();

			var resultStr = window.showModalDialog(control.get_selectObjectDialogUrl(), result, sFeature);

			var obj = result[resultStr];


			e.resultValue = obj == null ? "" : Sys.Serialization.JavaScriptSerializer.serialize([obj]);

		}

		function dataChanged(data) {

		}

		function setData() {
			var data = [];

			for (var i = 0; i < 5; i++) {
				var obj = {};
				obj.__type = "MCS.Library.SOA.Web.WebControls.Test.AutoCompleteWithSelectorControl.CommonData, MCS.Library.SOA.Web.WebControls.Test";
				obj.Code = "id" + i;
				obj.Name = "name" + i;
				data.push(obj);
			}

			var control = $find("CommonAutoCompleteWithSelectorControl1");
			control.set_selectedData(data);
			control.dataBind();
		}

		function onCloneComponent() {
			var parent = $get("container");

			var template = $find("CommonAutoCompleteWithSelectorControl1");

			template.cloneAndAppendToContainer(parent);
		}
		function onSelectData(sender, e) {
			//这个方法仅作为演示，实际应用根据具体情况实现，返回序列化结果，注意要带__type。
			var curData = e.Data;
			var dialogUrl = "url"; //这里是应用提供的对话框页面路径。
			var sFeature = "dialogWidth:500px; dialogHeight:300px;center:yes;help:no;resizable:yes;scroll:no;status:no";
			var resultStr = window.showModalDialog(dialogUrl, curData, sFeature);
			var result = Sys.Serialization.JavaScriptSerializer.serialize(resultStr);
			e.resultValue = result;
		}

		function onCreatingEditor(grid, e) {
			if (e.column.dataField == "Field1") {
				var parent = e.editor.get_htmlCell();
				var template = $find("CommonAutoCompleteWithSelectorControl1");
				var newControl = template.cloneAndAppendToContainer(parent);
				newControl.add_selectedDataChanged((function () {
					var editor = e.editor;
					return function (data) {
						editor.set_dataFieldDataByEvent(data);
					};
				})());
			}
			else if (e.column.dataField == "Field2") {
				var parent = e.editor.get_htmlCell();
				var template = $find("CommonAutoCompleteWithSelectorControl1");
				var newControl = template.cloneAndAppendToContainer(parent);
				newControl.add_selectedDataChanged((function () {
					var editor = e.editor;
					return function (data) {
						editor.set_dataFieldDataByEvent(data);
					};
				})());
			}
		}
		function onValueChanged(value) {			
			var control = $find("CommonAutoCompleteWithSelectorControl1");
			control.clearData();
			control.set_context(value);
		}
	</script>
</head>
<body>
	<form id="form1" runat="server">
	<div>
		<p>
			可输入1、2、3、*测试</p>
		<mcs:HBRadioButtonList ID="HBRadioButtonList1" runat="server" 
			RepeatDirection="Horizontal" RepeatLayout="Flow" OnClientValueChanged="onValueChanged">
			<asp:ListItem Value="1">1</asp:ListItem>
			<asp:ListItem Value="2">2</asp:ListItem>
		</mcs:HBRadioButtonList>		
		<mcs:CommonAutoCompleteWithSelectorControl ID="CommonAutoCompleteWithSelectorControl1"
			runat="server" ClientDataKeyName="Code" ClientDataDisplayPropName="Name" ClientDataDescriptionPropName="Detail"
			DataTextFields="Name,Detail" ShowSelector="True" PopupListWidth="500" OnClientSelectData="selectData" ShowCheckButton="False" ShowCheckIcon="False"
			OnGetDataSource="CommonAutoCompleteWithSelectorControl1_GetDataSource" OnValidateInput="CommonAutoCompleteWithSelectorControl1_ValidateInput"
			Width="200px"  />
		<br />
		<asp:Button ID="Button1" runat="server" Text="PostBack" OnClick="Button1_Click" />
		<input type="button" onclick="setData();" value="脚本设置selectedData" />
	</div>
	<div>
		<input type="button" onclick="onCloneComponent();" value="Clone Component" />
	</div>
	<div id="container">
		<mcs:ClientGrid ID="ClientGrid1" runat="server" ShowEditBar="true" OnCellCreatingEditor="onCreatingEditor">
			<Columns>
				<mcs:ClientGridColumn DataField="Field1" HeaderText="Field1" DataType="String" ItemStyle="{textAlign:'left'}"
					HeaderStyle="{width:'200px',textAlign:'center',fontWeight:'bold'}">
				</mcs:ClientGridColumn>
				<mcs:ClientGridColumn DataField="Field2" HeaderText="Field1" DataType="String" ItemStyle="{textAlign:'left'}"
					HeaderStyle="{width:'200px',textAlign:'center',fontWeight:'bold'}">
				</mcs:ClientGridColumn>
			</Columns>
		</mcs:ClientGrid>
	</div>
	</form>
</body>
</html>
