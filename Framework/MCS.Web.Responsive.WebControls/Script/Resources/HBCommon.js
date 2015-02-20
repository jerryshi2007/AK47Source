// -------------------------------------------------
// FileName	：	HBCommon.js
// Remark	：	公用类
// -------------------------------------------------
// VERSION		AUTHOR		DATE			CONTENT
// 1.0			张梁		20070731		创建
// 1.0			沈峥		20080503		添加DialogControlBase
// -------------------------------------------------

//=============================================HBCommon start=========================================={==========================================


var $HBRootNSName = "MCS.Web.WebControls";
Type.registerNamespace($HBRootNSName);
var $HBRootNS = MCS.Web.WebControls;

$HBRootNS.HBCommon = function (element) {
    throw Error.invalidOperation();
}

$HBRootNS.HBCommon.registerClass($HBRootNSName + ".HBCommon");


//所有要执行的事件
$HBRootNS.HBCommon.submitValidators = new Array();
//func:要执行的方法
//index:执行顺序
$HBRootNS.HBCommon.registSubmitValidator = function (func, index) {
    var functionObject = new Object();
    functionObject.func = func;
    functionObject.index = index;

    Array.add($HBRootNS.HBCommon.submitValidators, functionObject);

    $HBRootNS.HBCommon.submitValidators = $HBRootNS.HBCommon.submitValidators.sort($HBRootNS.HBCommon.sortSubmitValidators);

    if (window.attachEvent) {
        window.detachEvent("onbeforeunload", $HBRootNS.HBCommon.pageBeforeUnload);
        window.attachEvent("onbeforeunload", $HBRootNS.HBCommon.pageBeforeUnload);
    }
    else {
        window.removeEventListener("beforeunload", $HBRootNS.HBCommon.pageBeforeUnload, false);
        window.addEventListener("beforeunload", $HBRootNS.HBCommon.pageBeforeUnload, false);
    }
}

$HBRootNS.HBCommon.executeValidators = true;

$HBRootNS.HBCommon.pageBeforeUnload = function () {
    for (var i = 0; i < $HBRootNS.HBCommon.submitValidators.length; i++) {
        $HBRootNS.HBCommon.submitValidators[i].func();
    }
}

//反序排列
$HBRootNS.HBCommon.sortSubmitValidators = function (func1, func2) {
    return (func2.index - func1.index);
}

$HBRootNS.HBCommon.createObject = function (objectName, useActiveX) {
    try {
        var obj = null;

        if (useActiveX) {
            var componentHelperActiveX = document.getElementById("componentHelperActiveX");

            if (componentHelperActiveX == null) {
                throw Error.create(objectName + "对象创建失败！\r\n 当前页面未找到ComponentHelperActiveX控件！");
            }

            obj = componentHelperActiveX.CreateObject(objectName);
        }
        else {
            obj = new ActiveXObject(objectName);
        }

        return obj;
    }
    catch (e) {
        throw Error.create(objectName + "对象创建失败！\n可能是没有将http://"
			+ document.location.host + "加入可信站点。\n错误信息：" + e.message);
    }
}

$HBRootNS.HBCommon.CreateLocalServer = function (objectName) {
    try {
        var obj = null;
        var componentHelperActiveX = document.getElementById("componentHelperActiveX");

        if (componentHelperActiveX == null) {
            throw Error.create(objectName + "对象创建失败！\r\n 当前页面未找到ComponentHelperActiveX控件！");
        }

        obj = componentHelperActiveX.CreateLocalServer(objectName);
        return obj;
    }
    catch (e) {
        throw Error.create(objectName + "对象创建失败！\n可能是没有将http://"
			+ document.location.host + "加入可信站点。\n错误信息：" + e.message);
    }
}

$HBRootNS.ControlShowingMode = function () {
    throw Error.invalidOperation();
}

$HBRootNS.ControlShowingMode.prototype =
{
    Normal: 0,
    Dialog: 1
}

$HBRootNS.ControlShowingMode.registerEnum($HBRootNSName + '.ControlShowingMode');

//DialogControlBase
$HBRootNS.DialogControlBase = function (element) {
    $HBRootNS.DialogControlBase.initializeBase(this, [element]);

    this._dialogUrl = null;
    this._dialogArgs = null;
    this._dialogTitle = "";
    this._dialogWidth = "";
    this._dialogHeight = "";
    this._showCancelButton = true;
    this._showConfirmButton = true;
    this._cancelButtonText = "";
    this._confirmButtonText = "";
    this._parentPageControlID = "";
    this._dialogControlID = "";
    this._iframe = null;
    this._currentMode = $HBRootNS.ControlShowingMode.Normal;
    this._showingMode = $HBRootNS.ControlShowingMode.Normal;

    //	this._confirmButtonClientID = "";
    //	this._cancelButtonClientID = "";
    //	this._middleButtonClientID = "";
}

$HBRootNS.DialogControlBase.prototype = {
    initialize: function () {
        $HBRootNS.DialogControlBase.callBaseMethod(this, 'initialize');
        if (this._currentMode == $HBRootNS.ControlShowingMode.Dialog) {
            this._dataBindFromParent();
        }
    },

    dispose: function () {
        this._dialogArgs = null;
        this._iframe = null;
        $HBRootNS.DialogControlBase.callBaseMethod(this, 'dispose');
    },

    get_currentMode: function () {
        return this._currentMode;
    },

    set_currentMode: function (value) {
        this._currentMode = value;
    },

    get_showingMode: function () {
        return this._showingMode;
    },

    set_showingMode: function (value) {
        this._showingMode = value;
    },

    get_dialogTitle: function () {
        return this._dialogTitle;
    },

    set_dialogTitle: function (value) {
        this._dialogTitle = value;
    },

    get_dialogWidth: function () {
        return this._dialogWidth;
    },

    set_dialogWidth: function (value) {
        this._dialogWidth = value;
    },

    _getAdjustedWidth: function () {
        var cur = this.get_dialogWidth();
        if (cur.indexOf("px") > 0) {
            cur = cur.substring(0, cur.indexOf("px"));
            if (cur > ($(document.body).width() - 10)) {
                cur = $(document.body).width() - 10;
            }
        }

        return cur + "px";
    },

    get_dialogHeight: function () {
        return this._dialogHeight;
    },

    set_dialogHeight: function (value) {
        this._dialogHeight = value;
    },

    get_showCancelButton: function () {
        return this._showCancelButton;
    },

    set_showCancelButton: function (value) {
        this._showCancelButton = value;
    },

    get_showConfirmButton: function () {
        return this._showConfirmButton;
    },

    set_showConfirmButton: function (value) {
        this._showConfirmButton = value;
    },

    get_cancelButtonText: function () {
        return this._cancelButtonText;
    },

    set_cancelButtonText: function (value) {
        this._cancelButtonText = value;
    },

    get_confirmButtonText: function () {
        return this._confirmButtonText;
    },

    set_confirmButtonText: function (value) {
        this._confirmButtonText = value;
    },

    get_dialogArgs: function () {
        return this._dialogArgs;
    },

    set_dialogArgs: function (value) {
        this._dialogArgs = value;
    },

    get_dialogUrl: function () {
        return this._dialogUrl;
    },

    set_dialogUrl: function (value) {
        this._dialogUrl = value;
    },

    get_parentPageControlID: function () {
        return this._parentPageControlID;
    },

    set_parentPageControlID: function (value) {
        this._parentPageControlID = value;
    },

    get_dialogControlID: function () {
        return this._dialogControlID;
    },

    set_dialogControlID: function (value) {
        this._dialogControlID = value;
    },

    //	get_confirmButtonClientID: function () {
    //		return this._confirmButtonClientID;
    //	},

    //	set_confirmButtonClientID: function (value) {
    //		this._confirmButtonClientID = value;
    //	},

    //	get_cancelButtonClientID: function () {
    //		return this._cancelButtonClientID;
    //	},

    //	set_cancelButtonClientID: function (value) {
    //		this._cancelButtonClientID = value;
    //	},

    //	get_middleButtonClientID: function () {
    //		return this._middleButtonClientID;
    //	},

    //	set_middleButtonClientID: function (value) {
    //		this._middleButtonClientID = value;
    //	},

    //	enableConfirmButton: function (enabled) {
    //		this._enableButton(this.get_confirmButtonClientID(), enabled);
    //	},

    //	enableMiddleButton: function (enabled) {
    //		this._enableButton(this.get_middleButtonClientID(), enabled);
    //	},

    //	enableCancelButton: function (enabled) {
    //		this._enableButton(this.get_cancelButtonClientID(), enabled);
    //	},

    //	_getButton: function (buttonID) {
    //		var result = null;

    //		if (buttonID != null && buttonID != "") {
    //			result = $get(buttonID);
    //		}

    //		return result;
    //	},

    //	_enableButton: function (buttonID, enabled) {
    //		var button = $get(buttonID);

    //		if (button != null) {
    //			button.disabled = !enabled;
    //		}
    //	},

    _dataBindFromParent: function () {
        var parentWin = window.parent;
        if (parentWin) {
            var args = parentWin.$find(this._parentPageControlID).get_dialogArgs();
            if (args) {
                this.dataBind(args);
            }
        }
    },

    _confirmButtonClick: function (args) {
        if (this._iframe) {
            var subWin = this._iframe.contentWindow;
            if (subWin) {
                var subCtr = subWin.$find(this._dialogControlID);
                subCtr._onConfirm(args);
            }
        }
        //args.canceled = true;则取消关闭
    },

    _cancelButtonClick: function () {
        if (this._iframe) {
            var subWin = this._iframe.contentWindow;
            if (subWin) {
                var subCtr = subWin.$find(this._dialogControlID);
                subCtr._onCancel();
            }
        }
    },

    //这个是点击确定后在对话框页执行的方法，可以把对话框的返回结果数据放到args.result里，需要子类重写。
    _onConfirm: function (args) { },

    _onCancel: function () { },

    dataBind: function (args) {
        throw new Error("派生控件必须实现dataBind方法");
    },

    _showDialog: function (url, args, onConfirmCallBack, onCancelCallBack) {
        if (!url)
            url = this.get_dialogUrl();

        var parameters = this._parseUrlParameters(url);

        this.adjustDialogParameters(parameters);

        url = this._combineUrlParameters(url, parameters);

        var options = {
            title: this.get_dialogTitle(),
            width: this._getAdjustedWidth(),
            height: this.get_dialogHeight(),
            onOk: Function.createDelegate(this, this._confirmButtonClick),
            onCancel: Function.createDelegate(this, this._cancelButtonClick),
            onOkCallBack: onConfirmCallBack,
            onCancelCallBack: onCancelCallBack,
            okBtn: {
                visible: this.get_showConfirmButton(),
                text: this.get_confirmButtonText()
            },
            cancelBtn: {
                visible: this.get_showCancelButton(),
                text: this.get_cancelButtonText()
            },
            iframe: {
                width: '100%',
                height: '100%',
                src: url
            }
        };

        this.set_dialogArgs(args);

        var box = $HGModalBox.show(options);
        this._iframe = box.iframe;
        //		return window.showModalDialog(url, arg, feature);
    },

    //修饰对话框中参数，可以重载
    adjustDialogParameters: function (parameters) {
    },

    get_cloneableProperties: function () {
        var baseProperties = $HGRootNS.DialogControlBase.callBaseMethod(this, "get_cloneableProperties");
        var currentProperties = ["currentMode", "showingMode", "dialogUrl", "dialogTitle", "dialogWidth", "dialogHeight"
			, "showConfirmButton", "showCancelButton", "confirmButtonText", "cancelButtonText", "dialogControlID", "parentPageControlID"];

        Array.addRange(currentProperties, baseProperties);

        return currentProperties;
    },

    _parseUrlParameters: function (url) {
        var result = {};

        if (url != null && typeof (url) != "undefined" && url != "") {
            var paramString = url;

            var startIndex = url.indexOf("?");

            if (startIndex >= 0)
                paramString = url.substr(startIndex + 1);
            else
                paramString = "";

            var parts = paramString.split("&");

            for (var i = 0; i < parts.length; i++) {
                var part = parts[i];

                if (part.length > 0) {
                    var keyValuePair = part.split("=");

                    var value = "";

                    if (keyValuePair.length > 1)
                        value = keyValuePair[1];

                    result[keyValuePair[0]] = value;
                }
            }
        }

        return result;
    },

    _combineUrlParameters: function (url, parameters) {
        var baseUrl = "";

        if (url != null && typeof (url) != "undefined" && url != "") {
            var startIndex = url.indexOf("?");

            if (startIndex >= 0)
                baseUrl = url.substr(0, startIndex);
            else
                baseUrl = url;
        }

        var strB = new Sys.StringBuilder();

        if (parameters) {
            for (var key in parameters) {
                if (strB.isEmpty() == false)
                    strB.append("&");

                strB.append(key);
                strB.append("=");
                strB.append(parameters[key]);
            }
        }

        if (strB.isEmpty() == false)
            baseUrl = baseUrl + "?" + strB.toString();

        return baseUrl;
    },

    _pseudo: function () {
    }
}

$HBRootNS.DialogControlBase.registerClass($HBRootNSName + ".DialogControlBase", $HGRootNS.ControlBase);

//文件类型
$HBRootNS.officeDocumentType = function () {
    throw Error.invalidOperation();
};

$HBRootNS.officeDocumentType.prototype = {
    word: 1,
    excel: 2,
    powerPoint: 3,
    visio: 4
}

$HBRootNS.officeDocumentType.registerEnum($HBRootNSName + ".officeDocumentType");

$HBRootNS.officeDocument = function () {
    $HBRootNS.officeDocument.initializeBase(this);

    this._localPath = null; 		//文件路径
    this._documentType = null; 	//文件类型
    this._document = null; 		//文件所使用的officeDocument对象
    this._userName = null; 		//用名
    this._visible = true; 		//是否可见
    this._trackRevisions = false; //是否保留修改痕迹
    this._officeApp = null; 		//officeApplication对象
    this._timer = null; 			//用来回收垃圾延时使用
}

$HBRootNS.officeDocument.prototype = {
    initialize: function () {
        $HBRootNS.officeDocument.callBaseMethod(this, 'initialize');

        if ($HBRootNS.fileIO.fileExists(this._localPath) == false)
            throw Error.create("文件" + this._localPath + "不存在");

        this._documentType = $HBRootNS.officeDocument.getProgIDFromFileName(this._localPath);

    },
    dispose: function () {
        this._document = null;
        this._officeApp = null;
        CollectGarbage();

        $HBRootNS.officeDocument.callBaseMethod(this, 'dispose');
    },

    open: function () {
        if (this._officeApp != null)
            this._document = this._findDocument();

        if (this._document == null) {
            this._officeApp = this._createObject(this._documentType);
            this._document = this._openDocument();
        }

        this._officeApp.visible = this._visible;
        this._setUserName();

        switch (this._documentType) {
            case $HBRootNS.officeDocumentType.word:
                this._document.trackRevisions = this._trackRevisions;
                this._officeApp.Activate();
                break;
            case $HBRootNS.officeDocumentType.excel:
                this._officeApp.ActiveWorkbook.Activate();
                break;
        }
        this._officeApp.WindowState = 2;
        this._officeApp.WindowState = 1;

    },

    _openDocument: function () {
        switch (this._documentType) {
            case $HBRootNS.officeDocumentType.word:
                this._officeApp.documents.open(this._localPath);
                break;
            case $HBRootNS.officeDocumentType.powerPoint:
                this._officeApp.visible = true;
                this._officeApp.presentations.open(this._localPath);
                break;
            case $HBRootNS.officeDocumentType.excel:
                this._officeApp.visible = true;
                this._officeApp.workbooks.open(this._localPath);
                break;
            case $HBRootNS.officeDocumentType.visio:
                this._officeApp.documents.open(this._localPath);
                break;
        }

        if (this._officeApp != null)
            return this._officeApp.activeDocument;
        else
            return null;
    },

    _findDocument: function () {
        var doc = null;

        try {
            for (var i = 1; i <= this._officeApp.documents.count; i++) {
                var innerDoc = this._officeApp.documents.item(i);
                var strInnerFile = innerDoc.path + "\\" + innerDoc.name;

                if (strInnerFile.toLowerCase() == this._localPath.toLowerCase()) {
                    doc = this._officeApp.documents.item(i);
                    break;
                }
            }
        }
        catch (e) {
        }

        return doc;
    },

    _setUserName: function () {
        if (this._userName && this._userName.length > 0) {
            var ui = this._userName;

            if (ui.length > 4)
                ui = ui.substr(0, 4);

            switch (this._documentType) {
                case $HBRootNS.officeDocumentType.word:
                    this._officeApp.UserName = this._userName;
                    this._officeApp.UserInitials = ui;
                    break;
                case $HBRootNS.officeDocumentType.powerPoint:
                    break;
                case $HBRootNS.officeDocumentType.visio:
                    this._officeApp.Settings.UserName = this._userName;
                    this._officeApp.Settings.UserInitials = ui;
                    break;
            }
        }
    },

    _createObject: function (documentType) {
        var officeApp;
        var appName = "";

        switch (documentType) {
            case $HBRootNS.officeDocumentType.word:
                appName = "~~Wo##rd^Ap!!plication";
                break;
            case $HBRootNS.officeDocumentType.excel:
                appName = "~~Ex##cel^Ap!!plication";
                break;
            case $HBRootNS.officeDocumentType.powerPoint:
                appName = "~~Po##werPoint^Ap!!plication";
                break;
            case $HBRootNS.officeDocumentType.visio:
                appName = "~~Vi##sio^Ap!!plication";
                break;
            default:
                officeApp = null;
        }

        if (appName != "") {
            appName = appName.replace("~~", "").replace("##", "").replace("!!", "").replace("^", ".");
            officeApp = $HBRootNS.HBCommon.CreateLocalServer(appName);
        }

        return officeApp;
    },

    get_localPath: function () {
        return this._localPath;
    },
    set_localPath: function (value) {
        if (this._localPath != value) {
            this._localPath = value;
            this.raisePropertyChanged("localPath");
        }
    },

    get_documentType: function () {
        return this._documentType;
    },
    set_documentType: function (value) {
        if (this._documentType != value) {
            this._documentType = value;
            this.raisePropertyChanged("documentType");
        }
    },

    get_document: function () {
        return this._document;
    },
    set_document: function (value) {
        if (this._document != value) {
            this._document = value;
            this.raisePropertyChanged("document");
        }
    },

    get_userName: function () {
        return this._userName;
    },
    set_userName: function (value) {
        if (this._userName != value) {
            this._userName = value;
            this.raisePropertyChanged("userName");
        }
    },

    get_visible: function () {
        return this._visible;
    },
    set_visible: function (value) {
        if (this._visible != value) {
            this._visible = value;
            this.raisePropertyChanged("visible");
        }
    },

    get_trackRevisions: function () {
        return this._trackRevisions;
    },
    set_trackRevisions: function (value) {
        if (this._trackRevisions != value) {
            this._trackRevisions = value;
            this.raisePropertyChanged("trackRevisions");
        }
    },

    get_officeApp: function () {
        return this._officeApp;
    },
    set_officeApp: function (value) {
        if (this._officeApp != value) {
            this._officeApp = value;
            this.raisePropertyChanged("officeApp");
        }
    }
}

$HBRootNS.officeDocument.registerClass($HBRootNSName + ".officeDocument", Sys.Component);

$HBRootNS.officeDocument.getProgIDFromFileName = function (fileName) {
    var fileExt = $HBRootNS.fileIO.getFileExt(fileName);
    fileExt = fileExt.toLowerCase();

    var progID;

    switch (fileExt) {
        case "dot":
        case "dotm":
        case "dotx":
        case "doc":
        case "docm":
        case "docx":
        case "rtf":
        case "wiz":
            progID = $HBRootNS.officeDocumentType.word;
            break;
        case "xlsx":
        case "xlsm":
        case "xlsb":
        case "xls":
        case "xltx":
        case "xltm":
        case "xlt":
        case "csv":
        case "xlam":
        case "xla":
        case "xml":
            progID = $HBRootNS.officeDocumentType.excel;
            break;
        case "pptx":
        case "pptm":
        case "ppt":
        case "potx":
        case "potm":
        case "pot":
        case "ppsx":
        case "ppsm":
        case "pps":
        case "ppam":
        case "ppa":
        case "pwz":
            progID = $HBRootNS.officeDocumentType.powerPoint;
            break;
        case "vsd":
        case "vss":
        case "vst":
        case "vsx":
        case "vdx":
        case "vsw":
        case "vtx":
            progID = $HBRootNS.officeDocumentType.visio;
            break;
        default:
            throw Error.create("无法判断" + fileName + "的文件类型");
    }

    return progID;
}

$HBRootNS.officeDocument.checkIsOfficeDocument = function (fileName) {
    try {
        $HBRootNS.officeDocument.getProgIDFromFileName(fileName);
        return true;
    }
    catch (e) {
        return false;
    }
}

$HBRootNS.fileIO = function (element) {
    throw Error.invalidOperation();
}

$HBRootNS.fileIO.registerClass($HBRootNSName + ".fileIO");

$HBRootNS.fileIO.errorMessageHeaderName = "errorMessage"; 		//header中错误信息标志
$HBRootNS.fileIO.lastUploadTagMessageHeaderName = "lastUploadTag"; //header中最后上传标记的标志
$HBRootNS.fileIO.fileIconPathMessageHeaderName = "fileIconPath"; //header中文件图片路径
$HBRootNS.fileIO.materialIDMessageHeaderName = "materialID"; 	//materialID
$HBRootNS.fileIO.messageTag = "message"; 						//head中消息的开始标记

$HBRootNS.fileIO.GetADODBStream = function () {
    var objName = "~~AD##ODB^Str!!eam";

    objName = objName.replace("~~", "").replace("##", "").replace("!!", "").replace("^", ".");

    return $HBRootNS.HBCommon.createObject(objName, true);
};

$HBRootNS.fileIO.GetFileObject = function () {
    var objName = "~~Scri##pting^Fi!!leSystemObject";

    objName = objName.replace("~~", "").replace("##", "").replace("!!", "").replace("^", ".");

    return $HBRootNS.HBCommon.createObject(objName, true);
};

//Stream类型
$HBRootNS.fileStreamType = function () {
    throw Error.invalidOperation();
};
$HBRootNS.fileStreamType.prototype = {
    adTypeBinary: 1,    //2进制方式
    adTypeText: 2		//文本方式
}
$HBRootNS.fileStreamType.registerEnum($HBRootNSName + ".fileStreamType");

//Stream读写方式
$HBRootNS.fileStreamMode = function () {
    throw Error.invalidOperation();
};

$HBRootNS.fileStreamMode.prototype = {
    adModeRead: 1,
    adModeReadWrite: 3,
    adModeRecursive: 4194304,
    adModeShareDenyNone: 16,
    adModeShareDenyRead: 4,
    adModeShareDenyWrite: 8,
    adModeShareExclusive: 12,
    adModeUnknown: 0,
    adModeWrite: 2
}
$HBRootNS.fileStreamMode.registerEnum($HBRootNSName + ".fileStreamMode");

//Stream保存方式
$HBRootNS.fileStreamSaveMode = function () {
    throw Error.invalidOperation();
};
$HBRootNS.fileStreamSaveMode.prototype = {
    adSaveCreateNotExist: 1,
    adSaveCreateOverWrite: 2
}
$HBRootNS.fileStreamSaveMode.registerEnum($HBRootNSName + ".fileStreamSaveMode");

$HBRootNS.fileIO.downloadFile = function (processPageUrl, localPath, method, withFormData) {
    this._checkParams(processPageUrl, localPath);

    if (typeof (method) == "undefined")
        method = "POST";

    var formPostData = null;

    if (method.toLowerCase() == "post" && withFormData) {
        __theFormPostData = "";
        _Original_WebForm_InitCallback();
        formPostData = __theFormPostData;

        if (theForm["__EVENTVALIDATION"]) {
            formPostData += "&__EVENTVALIDATION=" + WebForm_EncodeCallback(theForm["__EVENTVALIDATION"].value);
        }
    }

    var downloaded = false;

    try {
        var xmlHttp = $HBRootNS.HBCommon.createObject("Msxml2.XMLHTTP");

        xmlHttp.open(method, processPageUrl, false);

        if (formPostData != null)
            xmlHttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        xmlHttp.send(formPostData);

        if (xmlHttp.status >= 400)
            throw Error.create($HBRootNS.fileIO.checkXmlHttpError(xmlHttp));

        var errorMessage = this._getInformationFromResponse(xmlHttp, this.errorMessageHeaderName);
        if (errorMessage != "")
            throw Error.create(errorMessage);

        downloaded = true;

        var folderPath = localPath.substring(0, localPath.lastIndexOf("\\"));

        this.createFolder(folderPath);

        var stream = $HBRootNS.fileIO.GetADODBStream();
        stream.Type = $HBRootNS.fileStreamType.adTypeBinary;
        stream.Mode = $HBRootNS.fileStreamMode.adModeReadWrite;
        stream.Open();
        if (typeof (xmlHttp.responseBody) != "undefined")
            stream.Write(xmlHttp.responseBody);

        var fileIconPath
			= $HBRootNS.fileIO._getInformationFromResponse(xmlHttp, $HBRootNS.fileIO.fileIconPathMessageHeaderName);

        var materialID
			= $HBRootNS.fileIO._getInformationFromResponse(xmlHttp, $HBRootNS.fileIO.materialIDMessageHeaderName);

        //如果是新的文件，将替换ID
        var filePath = (materialID == "" ? localPath : localPath.replace($HBRootNS.material.newMaterialID, materialID));

        stream.saveToFile(filePath, $HBRootNS.fileStreamSaveMode.adSaveCreateOverWrite);

        var returnInfo = new Object();
        returnInfo.fileIconPath = fileIconPath;
        returnInfo.materialID = materialID;

        return returnInfo;
    }
    catch (e) {
        if (downloaded) {
            throw Error.create("写文件失败" + localPath + "！\n可能是没有将http://"
			+ document.location.host + "加入可信站点。\n错误信息：\n" + e.message);
        }
        else {
            throw Error.create(e.message);
        }
    }
    finally {
        stream.close();
    }
}

$HBRootNS.fileIO._xmlHttp = null;
$HBRootNS.fileIO._currentUploadFileIndex = 0;
$HBRootNS.fileIO._uploadedFiles = new Array();
$HBRootNS.fileIO._filesToUpload = new Array();

$HBRootNS.fileIO.uploadFiles = function (filesToUpload, fileMaxSize, controlID) {
    if (this._filesToUpload.length == 0) {
        if (filesToUpload == null || filesToUpload.length == 0)
            return;

        this._filesToUpload = filesToUpload;
    }

    var currentFile = this._filesToUpload[this._currentUploadFileIndex];
    var localPath = currentFile.filePath;

    if (fileMaxSize == null)
        fileMaxSize = 0;

    if (controlID == null)
        controlID = currentFile.container.get_uniqueID();

    var processPageUrl = currentFile.container.get_currentPageUrl()
		+ "?requestType=upload"
		+ "&lockID=" + currentFile.container.get_lockID()
		+ "&userID=" + currentFile.container.get_user().id
		+ "&rootPathName=" + currentFile.container.get_rootPathName()
		+ "&fileMaxSize=" + fileMaxSize
		+ "&controlID=" + controlID;

    this._checkParams(processPageUrl, localPath);

    if (currentFile.materialID && currentFile.materialID.length > 20) {
        processPageUrl += "&fileName=" + escape(this.getFileNameWithExt(localPath));
    }
    else {
        if ($HBRootNS.officeDocument.checkIsOfficeDocument(localPath)) {
            var userLogOnName = currentFile.container.get_user().logOnName;
            localPath = this._copyFileToTempFolder(localPath, $HBRootNS.material.newMaterialID, userLogOnName);

            processPageUrl += "&fileName=" + escape(this.getFileNameWithExt(localPath));
        }
        else {
            processPageUrl += "&fileName=" + escape($HBRootNS.material.newMaterialID + "." + this.getFileExt(localPath));
        }
    }

    try {
        var stream = $HBRootNS.fileIO.GetADODBStream();

        stream.type = $HBRootNS.fileStreamType.adTypeBinary;
        stream.open();

        try {
            var destPath = "";
            if ($HBRootNS.officeDocument.checkIsOfficeDocument(localPath) && $HBRootNS.fileIO.isFileOpened(localPath) == true) {
                destPath = localPath;
                var curName = $HBRootNS.fileIO.getFileNameWithoutExt(destPath);
                destPath = destPath.replace(curName, "copyFileToUpload" + new Date().format("yyMMddHHmmssfff"));
                $HBRootNS.fileIO.copyFile(localPath, destPath);
            }

            $HBRootNS.fileIO._xmlHttp = $HBRootNS.HBCommon.createObject("Msxml2.XMLHTTP");

            this._loadStreamFile(stream, destPath == "" ? localPath : destPath);

            if (fileMaxSize > 0 && stream.size > fileMaxSize) {
                alert("文件大小不能超过" + fileMaxSize + "字节");
                $HBRootNS.fileIO.returnAndCloseWindow();
            }

            stream.position = 0;

            $HBRootNS.fileIO._xmlHttp.onreadystatechange = this._handleStateChange;
            $HBRootNS.fileIO._xmlHttp.open("POST", processPageUrl, true);
            $HBRootNS.fileIO._xmlHttp.send(stream.read(stream.size));

            if (destPath != "") {
                $HBRootNS.fileIO.deleteFile(destPath);
            }
        }
        finally {
            stream.close();
        }
    }
    catch (e) {
        alert("上传文件" + currentFile.filePath + "时发生错误：\n" + e.message + "\n 可能是文件过大");
        $HBRootNS.fileIO.returnAndCloseWindow();
    }
}

$HBRootNS.fileIO.returnAndCloseWindow = function () {
    window.returnValue = $HBRootNS.fileIO._uploadedFiles;
    window.close();
}

$HBRootNS.fileIO._handleStateChange = function () {
    if ($HBRootNS.fileIO._xmlHttp.readyState == 4) {
        var currentFile = $HBRootNS.fileIO._filesToUpload[$HBRootNS.fileIO._currentUploadFileIndex];

        if ($HBRootNS.fileIO._xmlHttp.status >= 400) {
            alert("上传文件" + currentFile.filePath + "时发生错误：\n"
				+ $HBRootNS.fileIO.checkXmlHttpError($HBRootNS.fileIO._xmlHttp));
            $HBRootNS.fileIO.returnAndCloseWindow();
        }
        else {
            var errorMessage = $HBRootNS.fileIO._getInformationFromResponse($HBRootNS.fileIO._xmlHttp, $HBRootNS.fileIO.errorMessageHeaderName);
            if (errorMessage != "") {
                alert("上传文件" + currentFile.filePath + "时发生错误：\n" + errorMessage);

                $HBRootNS.fileIO.deleteFile($HBRootNS.fileIO._getTempFilePath(currentFile.filePath, $HBRootNS.material.newMaterialID, currentFile.container.get_user().logOnName));
                $HBRootNS.fileIO.returnAndCloseWindow();
            }
            else {
                var lastUploadTag = $HBRootNS.fileIO._getInformationFromResponse($HBRootNS.fileIO._xmlHttp, $HBRootNS.fileIO.lastUploadTagMessageHeaderName);
                var fileIconPath = $HBRootNS.fileIO._getInformationFromResponse($HBRootNS.fileIO._xmlHttp, $HBRootNS.fileIO.fileIconPathMessageHeaderName);
                var newMaterialID = $HBRootNS.fileIO._getInformationFromResponse($HBRootNS.fileIO._xmlHttp, $HBRootNS.material.newMaterialID);

                var returnInfo = new Object();

                returnInfo.filePath = currentFile.filePath;
                returnInfo.tempFilePath =
					$HBRootNS.fileIO._getTempFilePath(currentFile.filePath, $HBRootNS.material.newMaterialID, currentFile.container.get_user().logOnName);
                returnInfo.lastUploadTag = lastUploadTag;
                returnInfo.fileIconPath = fileIconPath;
                returnInfo.newMaterialID = newMaterialID;

                var newFilePath = returnInfo.tempFilePath.replace($HBRootNS.material.newMaterialID, newMaterialID);
                $HBRootNS.fileIO.moveFile(returnInfo.tempFilePath, newFilePath);

                Array.add($HBRootNS.fileIO._uploadedFiles, returnInfo);

                if ($HBRootNS.fileIO._currentUploadFileIndex < $HBRootNS.fileIO._filesToUpload.length - 1) {
                    $HBRootNS.fileIO._currentUploadFileIndex += 1;
                    $HBRootNS.fileIO.uploadFiles();
                }
                else {
                    $HBRootNS.fileIO.returnAndCloseWindow();
                }
            }
        }
    }
}

$HBRootNS.fileIO.checkXmlHttpError = function (xmlHttp) {
    return ("状态码：" + xmlHttp.status + "\n" + "响应状态：" + xmlHttp.statusText);
}

$HBRootNS.fileIO._getTempFilePath = function (filePath, fileID, userLogOnName) {
    var tempFolderPath = $HBRootNS.fileIO.getTempDirName() + "\\" + userLogOnName;
    $HBRootNS.fileIO.createFolder(tempFolderPath);

    if (tempFolderPath.toLowerCase() == filePath.substring(0, filePath.lastIndexOf("\\")).toLowerCase())
        return filePath;
    else if (userLogOnName) {
        return (tempFolderPath + "\\" + fileID + "." + $HBRootNS.fileIO.getFileExt(filePath));
    }
    else {
        return (tempFolderPath + fileID + "." + $HBRootNS.fileIO.getFileExt(filePath));
    }
}

$HBRootNS.fileIO._copyFileToTempFolder = function (filePath, fileID, userLogOnName) {
    var newPath = this._getTempFilePath(filePath, fileID, userLogOnName);

    if (filePath != newPath)
        $HBRootNS.fileIO.copyFile(filePath, newPath);

    return newPath;
}

$HBRootNS.fileIO._checkParams = function (processPageUrl, localPath) {
    if (processPageUrl == null || processPageUrl == "")
        throw Error.create("请求页面地址为空");

    if (localPath == null || localPath == "")
        throw Error.create("文件路径为空");
}

$HBRootNS.fileIO._getInformationFromResponse = function (xmlHttp, headerName) {
    var headerInformation = xmlHttp.getResponseHeader(headerName);
    var message = "";

    if (headerInformation) {
        headerInformation = unescape(decodeURI(headerInformation));

        var start = headerInformation.indexOf(this.messageTag) + this.messageTag.length;

        message = headerInformation.substring(start + 1, headerInformation.length);
    }

    return message;
}

$HBRootNS.fileIO._loadStreamFile = function (stream, fileName) {
    try {
        stream.loadFromFile(fileName);
    }
    catch (e) {
        var C_ERROR_FILE_NOT_OPEN = -2146825286;

        if (e.number == C_ERROR_FILE_NOT_OPEN) {
            var strError = "文件" + fileName + "不能打开";

            strError += "，请检查该文件是否被其它应用程序打开";

            throw Error.create(strError);
        }
        else
            throw Error.create("请将http://" + document.location.host + "加入可信站点。\n错误信息：" + e.message);
    }
}

$HBRootNS.fileIO.getFileLastModifiedTime = function (fileName) {
    if ($HBRootNS.fileIO.fileExists(fileName) == false)
        return null;

    var fso = $HBRootNS.fileIO.GetFileObject();

    if (fso == null)
        return;

    var f = fso.getFile(fileName);

    return new Date(f.dateLastModified * 1);
}

$HBRootNS.fileIO.getTempDirName = function () {
    var fso = $HBRootNS.fileIO.GetFileObject();

    if (fso == null)
        return;

    var tempFolder = 2; //获得系统临时文件夹的系统变量

    var callScript = "fso.Get~~SpecialFolder(tempFolder)";
    callScript = callScript.replace("~~", "");

    return eval(callScript);
}

$HBRootNS.fileIO.getFileNameWithExt = function (fileName) {
    var nFileNameStart = fileName.lastIndexOf("\\");
    return fileName.substring(nFileNameStart + 1, fileName.length);
}

$HBRootNS.fileIO.getFileNameWithoutExt = function (fileName) {
    var nFileNameStart = fileName.lastIndexOf("\\");
    var nFileNameEnd = fileName.lastIndexOf(".");

    if (nFileNameEnd == -1)
        nFileNameEnd = fileName.length;

    return fileName.substring(nFileNameStart + 1, nFileNameEnd);
}

$HBRootNS.fileIO.getFileExt = function (fileName) {
    var nFileNameStart = fileName.lastIndexOf(".");

    if (nFileNameStart == -1)
        return "";

    return fileName.substring(nFileNameStart + 1, fileName.length);
}

$HBRootNS.fileIO.fileExists = function (fileName) {
    var bExists = false;

    try {
        var fso = $HBRootNS.fileIO.GetFileObject();
        bExists = fso.fileExists(fileName);
    }
    catch (e) {
    }

    return bExists;
}

$HBRootNS.fileIO.folderExists = function (folderName) {
    var bExists = false;
    var fso = $HBRootNS.fileIO.GetFileObject();
    bExists = fso.folderExists(folderName);
    return bExists;
}

$HBRootNS.fileIO.createFolder = function (folderName) {
    if (this.folderExists(folderName) == false) {
        var fso = $HBRootNS.fileIO.GetFileObject();

        if (fso != null)
            fso.CreateFolder(folderName);
    }
}

$HBRootNS.fileIO.readFileContent = function (fileName) {
    if (this.fileExists(fileName) == false)
        return null;

    var fso = $HBRootNS.fileIO.GetFileObject();
    var file = fso.GetFile(fileName);
    var ts = file.OpenAsTextStream(1);
    var text = ts.readAll();
    ts.close();
    return text;

}

$HBRootNS.fileIO.saveFileContent = function (fileName, content) {
    var fso = $HBRootNS.fileIO.GetFileObject();

    var f = fso.CreateTextFile(fileName, true);

    try {
        f.Write(content);
    }
    finally {
        f.Close();
    }
}

$HBRootNS.fileIO.deleteFile = function (filePath) {
    if ($HBRootNS.fileIO.fileExists(filePath) == true) {
        var fso = $HBRootNS.fileIO.GetFileObject();

        fso.DeleteFile(filePath);
    }
}

$HBRootNS.fileIO.copyFile = function (fromFilePath, toFilePath) {
    var fso = $HBRootNS.fileIO.GetFileObject();

    if (fso.fileExists(fromFilePath)) {
        var file = fso.getFile(fromFilePath);
        file.Copy(toFilePath);
    }
}

$HBRootNS.fileIO.moveFile = function (fileName, newFileName) {
    if (fileName != newFileName) {
        var fso = $HBRootNS.fileIO.GetFileObject();

        if (fso.fileExists(fileName)) {
            var file = fso.getFile(fileName);
            file.move(newFileName);
        }
    }
}

$HBRootNS.fileIO.isFileOpened = function (fileName) {
    var bOpened = false;

    try {
        if (fileName != null && fileName.length > 0) {
            var fso = $HBRootNS.fileIO.GetFileObject();

            if (fso.fileExists(fileName)) {
                var file = fso.getFile(fileName);

                try {
                    file.move(fileName);
                }
                catch (e) {
                    bOpened = true;
                }
            }
        }
    }
    catch (e) {
    }

    return bOpened;
}

//=============================================end==========================================}==========================================
