DialogHelperChecker = function () {
	this._name = "Check the attachment upload ActivieX control";

	var ax = document.getElementById("actXSelectFile");
	if (ax == null) {
		var axElem = document.createElement("<object id='actXSelectFile' codebase='../../HBWebHelperControl/HBWebHelperControl.CAB#version=1,0,0,13' height='0' width='0' classid='CLSID:C86C48A2-0DAD-41B6-BB85-AAB912FEE3AB' viewastext='viewastext'></object>");

		if (axElem)
			document.body.appendChild(axElem);
	}
}

DialogHelperChecker.prototype = new Checker();

DialogHelperChecker.prototype.check = function () {
	try {
		var ax = document.getElementById("actXSelectFile");

		ax.MultiSelect = true;
		ax.Filter = "";
		//ax.OpenDialog();
	}
	catch (e) {
		this._context.status = "Fail";
		this._context.statusText = e.message;
	}
	finally {
		if (this._context)
			this._context.checkCallBack(this);
	}
}

FSOChecker = function () {
	this._name = "Check FileSystemObject client components";

	var ax = document.getElementById("actXComponentHelper");
	if (ax == null) {
		var axElem = document.createElement("<object id='actXComponentHelper' codebase='../../HBWebHelperControl/HBWebHelperControl.CAB#version=1,0,0,13' height='0' width='0' classid='CLSID:918CFB81-4755-4167-BFC7-879E9DD52C9E' viewastext='viewastext'></object>");

		if (axElem)
			document.body.appendChild(axElem);
	}
}

FSOChecker.prototype = new Checker();

FSOChecker.prototype.check = function () {
	try {
		var componentHelper = document.getElementById("actXComponentHelper");
		var fso = componentHelper.CreateObject("Scripting.FileSystemObject");
	}
	catch (e) {
		this._context.status = "Fail";
		this._context.statusText = e.message;
	}
	finally {
		if (this._context)
			this._context.checkCallBack(this);
	}
}

ADOStreamChecker = function () {
	this._name = "Check ADODB.Stream client components";

	var ax = document.getElementById("actXComponentHelper");
	if (ax == null) {
		var axElem = document.createElement("<object id='actXComponentHelper' codebase='../../HBWebHelperControl/HBWebHelperControl.CAB#version=1,0,0,13' height='0' width='0' classid='CLSID:918CFB81-4755-4167-BFC7-879E9DD52C9E' viewastext='viewastext'></object>");

		if (axElem)
			document.body.appendChild(axElem);
	}
}

ADOStreamChecker.prototype = new Checker();

ADOStreamChecker.prototype.check = function () {
	try {
		var componentHelper = document.getElementById("actXComponentHelper");
		var adodb = componentHelper.CreateObject("ADODB.Stream");
	}
	catch (e) {
		this._context.status = "Fail";
		this._context.statusText = e.message;
	}
	finally {
		if (this._context)
			this._context.checkCallBack(this);
	}
}