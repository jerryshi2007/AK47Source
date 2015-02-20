function LoadWfInfo(controlid) {
	return $get(controlid + '{JSONINFO}').value;
}

function OpenBranchProc(activityid, controlid) {
	var dialog = $find('{DIALOGCONTROLID}');
	var url = dialog._dialogUrl;
	url += '&owner_activityid=' + activityid;

	var result = dialog.start(url);

	if (result) {
		document.getElementById(controlid + '{SILVERLIGHTOBJECTID}').Content.SLM.OpenBranchProcess(result);
	}
}

function GetCultureInfo() {
	if ($NT && $NT.category["{CULTUREINFOCATEGORY}"]) {
		var infoStr = Sys.Serialization.JavaScriptSerializer.serialize($NT.category["{CULTUREINFOCATEGORY}"]);
		return infoStr;
	}
	else {
		return '';
	}
}