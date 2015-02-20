
$HBRootNS.CustomDialogControl = function (element) {
	$HBRootNS.CustomDialogControl.initializeBase(this, [element]);
	this._loaded = false;
	this._applicationLoad$delegate = Function.createDelegate(this, this._applicationLoad);
}

$HBRootNS.CustomDialogControl.prototype = {

	initialize: function () {
		$HBRootNS.CustomDialogControl.callBaseMethod(this, 'initialize');
		Sys.Application.add_load(this._applicationLoad$delegate);
	},

	_applicationLoad: function () {
		if (this._loaded == false) {
			this._loaded = true;
		}
	},

	dispose: function () {
		$HBRootNS.CustomDialogControl.callBaseMethod(this, 'dispose');
	},

	/*--------------------get-set-------------------------{*/

	/*---------------------------------------------------}*/



	dataBind: function (args) {
		$get("txtName").value = args.name;
		$get("delegationBeginTime").value = args.begin;
		$get("delegationEndTime").value = args.end;
	},

	//这个是点击确定后在对话框页执行的方法，可以把对话框的返回结果数据放到args.result里。
	_onConfrim: function (args) {
		args.result = this._getConfirmResult();
	},

	_getConfirmResult: function () {
		var result = { name: $get("txtName").value, begin: $get("delegationBeginTime").value, end: $get("delegationEndTime").value };
		return Sys.Serialization.JavaScriptSerializer.serialize(result);
	},

	showDialog: function (args,onConfirmCallBack,onCancelCallBack) {
		this._showDialog(this._dialogUrl, args, onConfirmCallBack, onCancelCallBack);		
	}
};

$HBRootNS.CustomDialogControl.registerClass($HBRootNSName + ".CustomDialogControl", $HGRootNS.DialogControlBase);
