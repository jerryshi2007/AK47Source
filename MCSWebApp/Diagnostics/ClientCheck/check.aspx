<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="check.aspx.cs" Inherits="Diagnostics.ClientCheck.check" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<meta http-equiv="X-UA-Compatible" content="IE=7" />
	<meta http-equiv="X-UA-Compatible" content="IE=EmulateIE7"/>
	<title>Client Check</title>

	<script type="text/javascript" src="checkerBase.js"></script>

	<script type="text/javascript" src="checkerItems.js"></script>

	<script type="text/javascript">
		function onDocumentLoad() {
			initAllCheckers();
			renderAllChecks();
		}

		function renderAllChecks() {
			var checkers = Checker.get_allCheckers();

			for (var i = 0; i < checkers.length; i++) {
				var li = document.createElement("li");
				checkerList.appendChild(li);

				var checker = checkers[i];

				var text = document.createElement("span");
				li.appendChild(text);
				text.innerText = checker.get_name();

				var statusText = document.createElement("span");
				statusText.style.marginLeft = "8px";

				li.appendChild(statusText);
				checker.set_element(statusText);
			}
		}

		function initAllCheckers() {
			Checker.clearAllCheckers();

			checkerList.innerHTML = "";

			var dhc = new DialogHelperChecker();

			Checker.registerChecker(dhc);

			var fso = new FSOChecker();

			Checker.registerChecker(fso);

			var asChecker = new ADOStreamChecker();

			Checker.registerChecker(asChecker);
		}

		var _currentChecker =
		function execCheck(checker, context) {
			context.checkCallBack = checkCallBack;
			checker.set_context(context);

			createStatusText(checker.get_element());

			_currentChecker = checker;

			window.setTimeout(callCheck, 1);
		}

		function callCheck() {
			_currentChecker.check();
		}

		function checkCallBack(checker) {
			var context = checker.get_context();

			var statusText = checker.get_element();

			statusText.innerHTML = "";

			var img = document.createElement("img");
			img.align = "absMiddle";

			if (context.status == "OK")
				img.src = "../images/ok.gif"
			else
				img.src = "../images/fail.gif"

			statusText.appendChild(img);

			var status = document.createElement("span");
			status.innerText = context.statusText;
			statusText.appendChild(status);

			var checkers = Checker.get_allCheckers();

			if (context.index < checkers.length - 1) {
				var newContext = { index: context.index + 1, status: "OK", statusText: "Succeed" };

				execCheck(checkers[newContext.index], newContext);
			}
		}

		function createStatusText(statusText) {
			statusText.innerHTML = "";

			var img = document.createElement("img");
			img.src = "../images/hourglass.gif"
			img.align = "absMiddle";

			statusText.appendChild(img);

			var status = document.createElement("span");
			status.innerText = "Checking";
			statusText.appendChild(status);
		}

		function onCheckStart() {
			event.returnValue = false;

			initAllCheckers();
			renderAllChecks();

			context = { index: 0, status: "OK", statusText: "Succeed" };
			var checkers = Checker.get_allCheckers();

			if (checkers.length > 0)
				execCheck(checkers[0], context);

			return false;
		}
	</script>

</head>
<body onload="onDocumentLoad();">
	<form id="serverForm" runat="server">
	<div>
		<a href="#" onclick="onCheckStart();">Start check...</a>
	</div>
	<div>
		<ul id="checkerList">
		</ul>
	</div>
	<div>
		<p>
			IF check failed. Please configuration your computer with this tool.
		</p>
		<p style="margin-left:350px; text-decoration:none;"><a href="../../hta/autoConfig.hta" target="_blank">
				Download...</a>
		</p>
	</div>
	</form>
</body>
</html>
