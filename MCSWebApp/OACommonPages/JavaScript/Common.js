function onLinkClick(url, feature) {
	if (!feature)
		feature = 'height=520, width=680, top=0, lef t=0, toolbar=no, menubar=no, scrollbars=no,resizable=no,location=no, statu s=no';
	//var returnValue = window.showModalDialog(url, '', feature);
	var returnValue = window.open(url, '_als', feature);
	//    if (returnValue == "refresh") window.location.reload(true);
}
function ShowMessage(message) {
	alert(message);
	return false;
}
function refresh() {
	window.location.reload(true);
}

function refreshParent() {
	try
	{
		if (window.opener && window.opener.location)
			window.opener.location.reload();
	}
	catch(e)
	{
	}	
}

function setControlValuesToObject(arg) {
	if (arguments.length > 1)
		for (var i = 1; i < arguments.length; i++)
		setControlValueToObject(arg, arguments[i]);
}

function setControlValueToObject(arg, controlID, valuePropertyName) {
	if (!valuePropertyName)
		valuePropertyName = "value";
	eval("arg." + controlID + "=$get(\"" + controlID + "\")." + valuePropertyName);
}

function setObjectToControlValues(result, controlID) {
	if (arguments.length > 1)
		for (var i = 1; i < arguments.length; i++)
		setObjectToControlValue(result, arguments[i]);
}

function setObjectToControlValue(result, controlID, valuePropertyName) {
	if (!valuePropertyName)
		valuePropertyName = "value";

	eval("$get(\"" + controlID + "\")." + valuePropertyName + "=result." + controlID);
}

function initialFormCategoris(controlId) {
	var formCategoris = document.getElementById(controlId).value;
	if (formCategoris == "") {
		document.getElementById(controlId).value = "全部";

	}
}

//验证日期起始时间小于结束时间
function verifyTime(beginTimeID, endTimeID) {
	var begin = document.getElementById(beginTimeID).value;
	var end = document.getElementById(endTimeID).value;
	var result = true;
	//有一个为空就不用验证
	if ((begin != "") && (end != "")) {
		var arrBegin = begin.split("-");
		var arrEnd = end.split("-");
		//构造Date对象
		var dateBegin = new Date(arrBegin[0], arrBegin[1] - 1, arrBegin[2]);
		var dateEnd = new Date(arrEnd[0], arrEnd[1] - 1, arrEnd[2]);


		if (dateBegin > dateEnd) {
			alert("起始时间不能大于结束时间");
			result = false;
		}
	}

	return result;
}

function verifyUserInput(verifyUserInput) {
	var oucontrol = $find(verifyUserInput);
	var result = oucontrol.checkData();

	if (result == false) {
		alert("请输入正确的人员或机构");
	}
	return result;
}



