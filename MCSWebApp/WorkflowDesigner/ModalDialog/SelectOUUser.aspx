<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelectOUUser.aspx.cs" Inherits="WorkflowDesigner.ModalDialog.SelectOUUser" %>
<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls" TagPrefix="MCS" %>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>资源输入</title>
    	<script type="text/javascript">
    	    var selectedUserData;
    	    function onSelectedDataChanged(selectedData) {
    	        //displaySelectedObjects(selectedData);
    	        selectedUserData = selectedData;
    	    }

    	    function displaySelectedObjects(objs) {
    	        for (var i = 0; i < objs.length; i++)
    	            addMessage(objs[i].fullPath);
    	    }

    	    function addMessage(msg) {
    	        result.innerHTML += "<p style='margin:0'>" + msg + "</p>";
    	    }

    	    function onClick() {
    	        window.returnValue = { jsonStr: Sys.Serialization.JavaScriptSerializer.serialize(selectedUserData) };
    	        top.close();
            }
	</script>

</head>
<body>
    <form id="form1" runat="server">

    	<table width="100%" style="height: 100%; width: 100%">
		<tr>
			<td class="gridHead">
				<div class="dialogTitle">
					<span class="dialogLogo">请选择资源</span>
				</div>
			</td>
		</tr>
		<tr>
			<td style="vertical-align: middle; text-align:center;">
            	<div class="dialogContent" id="dialogContent" runat="server" style="width: 80%; vertical-align:middle
					height: 100%; overflow: auto; text-align:center;">
					<!--Put your dialog content here... -->
                     <MCS:OuUserInputControl MultiSelect="true" ID="OuUserInputControl" runat="server"
			            ShowDeletedObjects="true" InvokeWithoutViewState="true" 
                                    OnClientSelectedDataChanged="onSelectedDataChanged"  MergeSelectResult="True" 
                                    SelectMask="User" />

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
							<input type="button" onclick="onClick();" class="formButton"  value="确定(O)"  id="Button2" accesskey="O"/>
						</td>
						<td style="text-align: center;">
							<input type="button" onclick="top.close();" class="formButton"  value="取消(C)" id="Button1" accesskey="C"/>
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>

    </form>
</body>
</html>
