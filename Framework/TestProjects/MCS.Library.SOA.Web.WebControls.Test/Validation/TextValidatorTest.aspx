<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TextValidatorTest.aspx.cs"
	Inherits="MCS.Library.SOA.Web.WebControls.Test.Validation.TextValidatorTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Text Validator Test</title>
	<script type="text/javascript">
		function onDocumentLoad() {
			$get("integerInput").attachEvent("onchange", onIntegerInputChange);
			$get("numericInput").attachEvent("onchange", onNumericInputChange);
		}

		function onIntegerInputChange() {
			var eventResult = true;

			var validator = new $HBRootNS.IntegerTextValidator();

			var result = validator.validate(event.srcElement.value);

			if (!result.isValid) {
				$showError(result.message);
				eventResult = false;
			}

			event.returnValue = eventResult;
			return eventResult;
		}

		function onNumericInputChange() {
			var validator = new $HBRootNS.NumericTextValidator();

			var result = validator.validate(event.srcElement.value);

			if (!result.isValid)
				$showError(result.message);
		}
	</script>
</head>
<body onload="onDocumentLoad();">
	<form id="serverForm" runat="server">
	<div>
		<asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true">
		</asp:ScriptManager>
	</div>
	<div>
		Integer: <input id="integerInput" type="text" />
	</div>
	<div>
		Number: <input id="numericInput" type="text" />
	</div>
	</form>
</body>
</html>
