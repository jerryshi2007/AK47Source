var m_currentSimulationResult = null;
var m_currentStatus = "NotRunning";
var m_lastestResult = null;
var m_processParameters = [];

var WfProcessStatus = {
	Running: 0,
	Completed: 1,
	Aborted: 2,
	NotRunning: 3
};

function onPauseClick() {
	m_currentStatus = "Paused";
	initButtons();
}

function onResumeClick() {
	m_currentStatus = "Running";

	delayCallMoveTo();
	initButtons();
}

function onFinishClick() {
	m_currentStatus = "NotRunning";

	appendHtmlMessage("<div style='color: red'>已中止</div>");
	initButtons();
}

function onEditProcessParamsClick() {
	event.returnValue = false;

	var feature = "dialogWidth:810px; dialogHeight:600px;center:yes;help:no;resizable:yes;scroll:no;status:no";

	var data = { jsonStr: Sys.Serialization.JavaScriptSerializer.serialize(m_processParameters) };
	var result = window.showModalDialog(event.srcElement.href, data, feature);

	if (result)
		m_processParameters = Sys.Serialization.JavaScriptSerializer.deserialize(result.jsonStr);

	return true;
}

function getViewProcessFeature() {
	var width = 800;
	var height = 580;

	var left = (window.screen.width - width) / 2;
	var top = (window.screen.height - height) / 2;

	return "width=" + width + ",height=" + height + ",left=" + left + ",top=" + top + ",status=no,resizable=yes,toolbar=no,menubar=no,location=no,scrollbars=yes";
}

function onViewProcessStatusClick() {
	event.returnValue = false;

	window.open("/MCSWebApp/OACommonPages/AppTrace/appTraceViewer.aspx?simulation=true&processID=" + m_lastestResult.ProcessID,
		"simulation_viewProcess",
		getViewProcessFeature());

	return false;
}

function initButtons() {
	if ($get("processDescKeyHidden").value == "") {
		$get("startBtn").disabled = true;
		$get("finishBtn").style.display = "none";
	}
	else {
		$get("startBtn").disabled = false;

		switch (m_currentStatus) {
			case "NotRunning":
				$get("startBtn").style.display = "inline";
				$get("pauseBtn").style.display = "none";
				$get("resumeBtn").style.display = "none";
				$get("finishBtn").style.visibility = "hidden";
				break;
			case "Running":
				$get("startBtn").style.display = "none";
				$get("pauseBtn").style.display = "inline";
				$get("resumeBtn").style.display = "none";
				$get("finishBtn").style.visibility = "visible";
				break;
			case "Paused":
				$get("startBtn").style.display = "none";
				$get("pauseBtn").style.display = "none";
				$get("resumeBtn").style.display = "inline";
				$get("finishBtn").style.visibility = "visible";
				break;
		}
	}

	if (m_lastestResult != null)
		$get("viewProcessLink").style.display = "inline";
	else
		$get("viewProcessLink").style.display = "none";
}

//从界面控件构造参数
function buildSimulationParameters() {
	var result = { EnableServiceCall: false, Variables: [] };

	result.EnableServiceCall = $get("enableServiceCallBtn").checked;
	result.Variables = m_processParameters;
	result.Creator = $find("processCreator").get_selectedSingleData();

	return Sys.Serialization.JavaScriptSerializer.serialize(result);
}

function initBeforeStart() {
	$get("dialogContent").innerHTML = "";
}

function callStart() {
	initBeforeStart();

	var simulationParams = buildSimulationParameters();

	WorkflowDesigner.Simulation.WorkflowSimulationService.StartSimulation($get("processDescKeyHidden").value, simulationParams,
		onCallbackCompleted, onCallbackError);

	m_currentStatus = "Running";
	initButtons();
}

function delayCallMoveTo() {
	if (m_currentStatus == "Running") {
		var simulationParams = buildSimulationParameters();

		WorkflowDesigner.Simulation.WorkflowSimulationService.MoveToSimulation(m_lastestResult.ProcessID, simulationParams,
		onCallbackCompleted, onCallbackError);

		initButtons();
	}
}

function onCallbackCompleted(result) {
	m_lastestResult = result;

	appendHtmlMessage(result.OutputString);

	if (result.ProcessStatus == WfProcessStatus.Running)
		window.setTimeout(delayCallMoveTo, $get("callbackInterval").value * 1000);
	else
		m_currentStatus = "NotRunning";

	initButtons();
}

function onCallbackError(e) {
	m_currentStatus = "NotRunning";

	initButtons();
	$showError(e);
}

function appendHtmlMessage(message) {
	$get("dialogContent").innerHTML += message;
}