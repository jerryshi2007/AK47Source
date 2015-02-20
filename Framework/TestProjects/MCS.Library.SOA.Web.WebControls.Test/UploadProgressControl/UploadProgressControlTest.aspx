<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UploadProgressControlTest.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.UploadProgressControl.UploadProgressControlTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="MCS" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>上传控件的测试</title>
	<script type="text/javascript">
		function onPrepareData(e) {
			//向Server传递数据
			e.postedData = $get("postedData").value;
		}

		function onCompleted(e) {
			alert(e.dataChanged);
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<input id="postedData" type="text" value="Test Data" />
	<div>
		<MCS:UploadProgressControl runat="server" ID="uploadProgress" DialogTitle="Upload Matrix"
			OnClientBeforeStart="onPrepareData"
			ControlIDToShowDialog="doUploadBtn" PostedData="Haha" OnDoUploadProgress="uploadProgress_DoUploadProgress"
			OnBeforeNormalPreRender="uploadProgress_BeforeNormalPreRender"
			OnClientCompleted="onCompleted" />
	</div>
	<div>
		<input runat="server" id="doUploadBtn" type="button" value="Upload..." />
	</div>
	<div>
		这里应该是一个Grid，根据上传结果决定是否刷新
	</div>
	</form>
</body>
</html>
