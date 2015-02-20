
// -------------------------------------------------
// FileName	：	LockContextControl.js
// Remark	：	锁控件2
// -------------------------------------------------
// VERSION		AUTHOR		DATE			CONTENT
// 1.0			沈峥		20091012		创建
// -------------------------------------------------

/*
$HBRootNS.lockStatus = function() {
throw Error.invalidOperation();
};
*/

$HBRootNS.LockContextControl = function(element) {
	$HBRootNS.LockContextControl.initializeBase(this, [element]);

	this._enabled = true; 			//是否有效
	var defaultInterval = new Object();
	defaultInterval.TotalMilliseconds = 60 * 1000;
	this._checkInterval = defaultInterval; //检查锁的时间间隔,默认为1分钟
	this._timer = null; 				//定时器
	this._timerCalling = false;
	//this._popupMessage = null; 		//提醒
	//this._playSoundPath = null;
	//this._titleImagePath = null;
	//this._messageIconPath = null;
	this._locks = [];

	this._callBackUrl = null;
}

$HBRootNS.LockContextControl.prototype =
{
	initialize: function() {
		$HBRootNS.LockContextControl.callBaseMethod(this, "initialize");

		if (this._enabled == true) {
			this._timer = $create($HGRootNS.Timer, { interval: this._checkInterval.TotalMilliseconds, enabled: true }, null, null, null);
			this._timer.add_tick(Function.createDelegate(this, this._timerCheckLock));
			/*
			this._popupMessage = $create($HBRootNS.PopUpMessageControl,
			{
			cssPath: this._cssPath,
			enabled: true,
			playSoundPath: this._playSoundPath,
			titleImagePath: this._titleImagePath,
			messageIconPath: this._messageIconPath
			},
			null, null, document.documentElement);
			*/
		}

		Array.add($HBRootNS.LockContextControl.allLocks, this);
	},

	dispose: function() {
		Array.remove($HBRootNS.LockContextControl.allLocks, this);

		this._timer = null;

		$HBRootNS.LockContextControl.callBaseMethod(this, "dispose");
	},

	_onIntervalCallBackCompleted: function(executor, eventArgs) {
		try {
			if (executor.get_responseAvailable()) {
				if (executor.get_statusCode() == "200") {
					var responseData = executor.get_responseData();

					if (responseData.indexOf("$ErrorType") == -1) {
						this._locks = Sys.Serialization.JavaScriptSerializer.deserialize(responseData);
					}
				}
			}
		}
		catch (e) {
		}
	},

	_invoke: function(async, data, command, onCompleted) {
		var webRequest = new Sys.Net.WebRequest();

		var serializedData = Sys.Serialization.JavaScriptSerializer.serialize(data);

		webRequest.set_body(serializedData);
		webRequest.set_httpVerb("POST");
		webRequest.add_completed(Function.createDelegate(this, onCompleted));
		webRequest.set_executor(this._createNewExecutor(async));

		var url = this.get_callBackUrl();

		if (url.indexOf("?") == -1)
			url += "?";
		else
			url += "&";

		url += "cmd=" + command;

		webRequest.set_url(url);

		webRequest.invoke();
	},

	_createNewExecutor: function(async) {
		var executor = new Sys.Net.XMLHttpExecutor2();

		executor.set_async(async);

		return executor;
	},

	_timerCheckLock: function() {
		this._timerCalling = true;
		this._timer.set_enabled(false);
		try {
			this.checkLock();
		}
		catch (e) {
			this._timer.set_enabled(true);
			$showError(e);
		}
		finally {
			this._timer.set_enabled(true);
		}
	},

	checkLock: function(synchronous) {
		if (this._enabled == true && this.get_locks().length > 0) {
			this._invoke(true, this.get_locks(), "checkLock", this._onIntervalCallBackCompleted);
		}
	},

	unlock: function() {

		if (this._enabled == true) {
			try {
				if (this.get_locks().length > 0) {
					this._invoke(false, this.get_locks(), "unlock", this._onIntervalCallBackCompleted);
				}
			}
			catch (e) {
				$showError(e);
			}
		}
	},

	/*
	_checkLockResultError: function(err) {
	try {
	switch (err.name) {
	case "Error":
	alert(String.format("调用函数{0}出现异常:{1}", "CheckLock", err.message));
	break;
	case "System.Exception":
	alert(String.format("调用函数{0}出现异常:{1}", "CheckLock", err.message));
	break;
	default:
	this._popupMessage.set_showTitle("锁失效提醒");
	this._popupMessage.set_showText(err.message);
	this._popupMessage.set_positionElement(null);
	this._popupMessage.set_positionX(window.screen.width - this._popupMessage.get_width());
	this._popupMessage.set_positionY(window.screen.height - this._popupMessage.get_height());
	this._popupMessage.show();
	break;
	}
	}
	catch (e) {
	if (this._timerCalling) {
	this._timer.set_enabled(true);
	this._timerCalling = false;
	}
	$showError(e);
	}
	finally {
	if (this._timerCalling) {
	this._timer.set_enabled(true);
	this._timerCalling = false;
	}
	}
	},

	_checkLockResult: function() {
	},

	_checkUnlockResultError: function(err) {
	switch (err.name) {
	case "Error":
	alert(String.format("调用函数{0}出现错误:{1}", "UnLock", err.message));
	break;
	case "System.Exception":
	alert(String.format("调用函数{0}出现异常:{1}", "UnLock", err.message));
	break;
	default:
	alert(String.format("调用函数{0}出现其他异常:{1}", "UnLock", err.message));
	break;
	}
	},

	_checkUnlockResult: function(result) {
	},
	
	get_titleImagePath: function() {
	return this._titleImagePath;
	},

	set_titleImagePath: function(value) {
	if (this._titleImagePath != value) {
	this._titleImagePath = value;
	this.raisePropertyChanged("titleImagePath");
	}
	},

	get_messageIconPath: function() {
	return this._messageIconPath;
	},

	set_messageIconPath: function(value) {
	if (this._messageIconPath != value) {
	this._messageIconPath = value;
	this.raisePropertyChanged("messageIconPath");
	}
	},

	get_cssPath: function() {
	return this._cssPath;
	},

	set_cssPath: function(value) {
	if (this._cssPath != value) {
	this._cssPath = value;
	this.raisePropertyChanged("cssPath");
	}
	},

	get_playSoundPath: function() {
	return this._playSoundPath;
	},

	set_playSoundPath: function(value) {
	if (this._playSoundPath != value) {
	this._playSoundPath = value;
	this.raisePropertyChanged("playSoundPath");
	}
	},
	*/

	get_callBackUrl: function() {
		return this._callBackUrl;
	},

	set_callBackUrl: function(value) {
		if (this._callBackUrl != value) {
			this._callBackUrl = value;
			this.raisePropertyChanged("callBackUrl");
		}
	},

	get_locks: function() {
		return this._locks;
	},

	set_locks: function(value) {
		this._locks = value;
	},

	get_enabled: function() {
		return this._enabled;
	},

	set_enabled: function(value) {
		if (this._enabled != value) {
			this._enabled = value;
			this.raisePropertyChanged("enabled");
		}
	},

	get_checkInterval: function() {
		return this._checkInterval;
	},

	set_checkInterval: function(value) {
		if (this._checkInterval != value) {
			this._checkInterval = value;
			this.raisePropertyChanged("checkInterval");
		}
	}
}

$HBRootNS.LockContextControl.registerClass($HBRootNSName + ".LockContextControl", $HGRootNS.ControlBase);

$HBRootNS.LockContextControl.allLocks = new Array();
$HBRootNS.LockContextControl.lockSubmitProcessing = false;

//所有的需要AutoPostBack的控件都要求 CausesValidation="True"，否则下面的函数无法执行则会导致页面解锁
$HBRootNS.LockContextControl.lockClientValidate = function(source, arguments) {
	arguments.IsValid = true;

	$HBRootNS.LockContextControl.lockSubmitProcessing = true;
}

$HBRootNS.LockContextControl.UnlockLocks = function() {
	if ($HBRootNS.LockContextControl.lockSubmitProcessing == false) {
		for (var i = 0; i < $HBRootNS.LockContextControl.allLocks.length; i++)
			$HBRootNS.LockContextControl.allLocks[i].unlock();
	}
	else {
		$HBRootNS.LockContextControl.lockSubmitProcessing = false;
	}
}

$HBRootNS.HBCommon.registSubmitValidator($HBRootNS.LockContextControl.UnlockLocks, 2);