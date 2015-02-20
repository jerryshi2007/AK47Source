
$HBRootNS.ServiceImport = function (element) {
	$HBRootNS.ServiceImport.initializeBase(this, [element]);
	this._confirmButtonClientID = null;
	this._serviceAddresseSelectClientID = null;
	this._serviceOperationSelectClientID = null;
	//this._loaded = false;
	//this._applicationLoad$delegate = Function.createDelegate(this, this._applicationLoad);
}

$HBRootNS.ServiceImport.prototype = {
	initialize: function () {
		$HBRootNS.ServiceImport.callBaseMethod(this, 'initialize');

		if (window.dialogArguments) {

		}

		//Sys.Application.add_load(this._applicationLoad$delegate);
	},

	_applicationLoad: function () {
		if (this._loaded == false) {

			this._loaded = true;
		}
	},

	dispose: function () {
		$HBRootNS.ServiceImport.callBaseMethod(this, 'dispose');
	},

	get_confirmButtonClientID: function () {
		return this._confirmButtonClientID;
	},
	set_confirmButtonClientID: function (value) {
		this._confirmButtonClientID = value;
	},

	get_serviceAddresseSelectClientID: function () {
		return this._serviceAddresseSelectClientID;
	},
	set_serviceAddresseSelectClientID: function (value) {
		this._serviceAddresseSelectClientID = value;
	},
	get_serviceOperationSelectClientID: function () {
		return this._serviceOperationSelectClientID;
	},
	set_serviceOperationSelectClientID: function (value) {
		this._serviceOperationSelectClientID = value;
	},

	//回调分析webService
	_AnalysisResult: function () {
		var functionName = $get(this._serviceOperationSelectClientID).value;
		var servicAddressKey = $get(this._serviceAddresseSelectClientID).value;

		if (functionName != "" && servicAddressKey != "") {
			this._invoke("getAnalysisResult",
                     [functionName, servicAddressKey],
                     Function.createDelegate(this, this._invokeCallback),
                     Function.createDelegate(this, this._invokeCallbackError));
		}
		else {
			$get("hf_serviceImport_returnValue").value = "";
		}
	},

	_GetOperationList: function () {
		var addressKey = $get(this._serviceAddresseSelectClientID).value;
		if (addressKey != "") {
			this._resetOperationListCtrl();
			this._invoke("getOperationList",
                     [addressKey],
                     Function.createDelegate(this, this._invokeOperationCallback),
                     Function.createDelegate(this, this._invokeOperationCallbackError));
		}
	},

	_invokeCallback: function (result) {
		$get("hf_serviceImport_returnValue").value = result;
		$get("div_message").innerHTML = '解析成功';
	},

	_invokeCallbackError: function (err) {
		$get("div_message").innerHTML = '[' + err.name + ']' + err.description;
	},

	_invokeOperationCallback: function (result) {
		var operationArr = Sys.Serialization.JavaScriptSerializer.deserialize(result);
		var svcOperationDdl = $get(this._serviceOperationSelectClientID);
		for (var i = 0; i < operationArr.length; i++) {
			var oOption = document.createElement('OPTION');
			oOption.text = operationArr[i];
			oOption.value = operationArr[i];
			svcOperationDdl.options.add(oOption);
		}
	},

	_invokeOperationCallbackError: function (err) {
		alert("远程服务不可用，错误：" + err.description);
	},

	_resetOperationListCtrl: function () {
		var svcOperationDdl = $get(this._serviceOperationSelectClientID);
		for (var i = svcOperationDdl.options.length - 1; i >= 0; i--) {
			svcOperationDdl.options.remove(i);
		}
		var oOption = document.createElement('OPTION');
		oOption.text = '请选择';
		oOption.value = '';
		svcOperationDdl.options.add(oOption);
	},

	showDialog: function (p_params) {
		var params = new Object();

		var resultStr = this._showDialog(params, this._dialogUrl);

		var result = "";
		if (resultStr) {
			result = Sys.Serialization.JavaScriptSerializer.deserialize(resultStr);
		}
		return result;
	}
}

$HBRootNS.ServiceImport.registerClass($HBRootNSName + ".ServiceImport", $HGRootNS.DialogControlBase);

