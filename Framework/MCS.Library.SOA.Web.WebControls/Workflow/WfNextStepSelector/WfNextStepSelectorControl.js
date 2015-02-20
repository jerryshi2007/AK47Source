function onWFNextStepChange() {
	var dataID = event.srcElement.dataID;
	var radioGroupName = event.srcElement.name;
    var radios = document.getElementsByName(radioGroupName);
    for (var i = 0; i < radios.length; i++)
    { radios[i].disabled = true; }
	
	if (dataID) {
		var nextSteps = Sys.Serialization.JavaScriptSerializer.deserialize($get(dataID).value);
		var nextStep = nextSteps[parseInt(event.srcElement.value)];

		$HBRootNS.WfMoveToControl._selectedStep = nextStep;
	}
}