<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SimplePostProgressControlTest.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.PostProgressControl.SimplePostProgressControlTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
	TagPrefix="MCS" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>PostProgressControl测试页面</title>
	<script type="text/javascript">
		function onPrepareData(e) {
			$get("statusText").innerText = "";
			e.steps = [1, 3, 5, 7, 9];
			//需要向服务器传递的附加数据
			e.clientExtraPostedData = "Hello world";
		}

		function onCompleted(e) {
			$get("statusText").innerText = "Data changed: " + e.dataChanged;
		}

		function onSerEnabled() {
			var enabled = !($get("doUploadBtn").disabled);

			$get("doUploadBtn").disabled = enabled;
		}
	</script>
</head>
<body>
	<form id="serverForm" runat="server">
	<div>
		<p>
			这个页面演示了通过响应客户端时间OnClientBeforeStart来初始化数据，然后将准备好的数组进行上传。服务器端按照数组的每一项进行处理，处理过程中反馈处理进度。
		</p>
		<p>
			处理完成后，OnClientComplete事件可以返回数据是否更新，客户端由此判断是否进行刷新页面等后续处理。
		</p>
		<p>
			客户端的数据会通过JSON序列化Post到服务器端，服务器端可以通过PrepareData来反序列化数据。如果不响应此事件，会按照默认的反序列化规则。然后应用通过响应DoPostedData来处理数据。
		</p>
		<div>
			<MCS:PostProgressControl runat="server" ID="uploadProgress" DialogTitle="Post data test"
				ControlIDToShowDialog="doUploadBtn" OnClientBeforeStart="onPrepareData" OnClientCompleted="onCompleted"
				OnDoPostedData="uploadProgress_DoPostedData" />
		</div>
		<div>
			<input runat="server" id="doUploadBtn" type="button" value="Upload..." />
			<input id="setEnabledBtn" type="button" value="Enable/Disable" onclick="onSerEnabled();"/>
		</div>
		<div id="statusText">
		</div>
	</div>
	</form>
</body>
</html>
