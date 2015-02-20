<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ActivityKeyEditor.aspx.cs"
	Inherits="WorkflowDesigner.ModalDialog.ActivityKeyEditor" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="cc1" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<base target="_self">
	<title>请输入流程模板的Key</title>
	<script type="text/javascript">
	    var activities;
		function onDocumentLoad() {
			if (window.dialogArguments != null) {
			    activities = window.dialogArguments;
			    bindActivies();
            }
            
        }
        function bindActivies() {
            for (var i = 0; i < activities.length; i++) {
                var ddlactivities = document.getElementById("ddlActivities");
                ddlactivities.appendChild(getOption(activities[i].Key,activities[i].Name));
            }
        }
        function getOption(val, txt) {
            var op = document.createElement("OPTION");
            op.value = val; op.innerHTML = txt;
            return op;
        }


        function onbtnOKClick() {
            //window.returnValue = resources.length;
            window.returnValue = [ document.getElementById("ddlActivities").value ];
            top.close();
        }

    </script>
</head>
<body onload="onDocumentLoad()">
	<form id="serverForm" runat="server" target="">
	<table width="100%" style="height: 100%; width: 100%">
		<tr>
			<td class="gridHead">
				<div class="dialogTitle">
					<span class="dialogLogo">请输入流程活动点的Key</span>
				</div>
			</td>
		</tr>
		<tr>
			<td style="vertical-align: center">
				<div class="dialogContent" id="dialogContent" runat="server" style="width: 100%;
					height: 100%; overflow: auto">
					<!--Put your dialog content here... -->
					<table width="100%" style="height: 100%; width: 100%">
						<tr>
							<td class="label">
								流程活动点的Key
							</td>
							<td valign="middle" align="center">
                                <select id="ddlActivities" style="width: 200px">
                                </select>
							</td>
						</tr>
					</table>
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
                        <td style="text-align: center;"><input type="button" class="formButton" onclick="onbtnOKClick();" value="确定(O)"  id="btnOK" accesskey="O"/></td>
                        <td style="text-align: center;"><input type="button" class="formButton" onclick="top.close();" value="取消(C)" id="btnCancel" accesskey="C"/></td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
	<div>
		<iframe style="display: none" name="innerFrame" />
	</div>
	</form>
</body>
</html>
