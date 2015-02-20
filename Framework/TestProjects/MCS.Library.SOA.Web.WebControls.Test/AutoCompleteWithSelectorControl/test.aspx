<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="test.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.AutoCompleteWithSelectorControl.test" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="mcs" %>
<%@ Register TagPrefix="cc1" Namespace="MCS.Web.WebControls" %>
<%@ Import Namespace="MCS.Library.OGUPermission" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<script type="text/javascript">

	    var page = {
	        selectData: function (parameters) {
	            alert(0);
	        }
	    };

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
	    function btnSetReadOnly_onclick() {
	        var control = $find("CommonAutoCompleteWithSelectorControl1");
	        var readOnly = control.get_readOnly();
	        control.set_readOnly(!readOnly);
	    }

	    function btnSetDisabled_onclick() {
	        var control = $find("CommonAutoCompleteWithSelectorControl1");
	        var enabled = control.get_enabled();
	        control.set_enabled(!enabled);
	    }

	</script>
</head>
<body>
	<form id="form1" runat="server">

	<div>
    <p>可输入1、2、3、*测试</p>
		<mcs:CommonAutoCompleteWithSelectorControl ID="CommonAutoCompleteWithSelectorControl1" runat="server"
			ClientDataKeyName="Code" ClientDataDisplayPropName="Name" ClientDataDescriptionPropName="Detail" 
            DataTextFields="Name,Detail"
			OnClientSelectData="page.selectData"
            OnGetDataSource="CommonAutoCompleteWithSelectorControl1_GetDataSource"
			OnValidateInput="CommonAutoCompleteWithSelectorControl1_ValidateInput" Width="400px" />
     <br />
		<asp:Button ID="Button1" runat="server" Text="PostBack" OnClick="Button1_Click" />
		<input id="btnSetReadOnly" type="button" value="Set ReadOnly" onclick="return btnSetReadOnly_onclick()" />
		<input id="btnSetDisabled" type="button" value="Set Disabled" onclick="return btnSetDisabled_onclick()" /></div>

    <div>
    <input type="button" onclick="onCloneComponent();" value="Clone Component" />
    </div>
    <div id="container">
	</div>
	</form>
</body>
</html>
