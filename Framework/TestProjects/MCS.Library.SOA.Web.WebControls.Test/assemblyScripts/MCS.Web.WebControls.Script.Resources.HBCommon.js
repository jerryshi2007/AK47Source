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
var $HBRootNS = eval($HBRootNSName);

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

    window.detachEvent("onbeforeunload", $HBRootNS.HBCommon.pageBeforeUnload);
    window.attachEvent("onbeforeunload", $HBRootNS.HBCommon.pageBeforeUnload);
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

$HBRootNS.HBCommon.createObject = function (objectName) {
    try {
        var obj = new ActiveXObject(objectName);

        return obj;
    }
    catch (e) {
        throw Error.create(objectName + "对象创建失败！\n可能是没有将http://"
			+ document.location.host + "加入可信站点。\n错误信息：" + e.message);
    }
}

$HBRootNS.HBCommon.falseThrow = function (bCondition, strDescription) {
    if (!bCondition)
        throw Error.create(strDescription);
}

$HBRootNS.HBCommon.trueThrow = function (bCondition, strDescription) {
    if (bCondition)
        throw Error.create(strDescription);
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
    this._dialogFeature = null;
    this._currentMode = $HBRootNS.ControlShowingMode.Normal;
    this._showingMode = $HBRootNS.ControlShowingMode.Normal;
}

$HBRootNS.DialogControlBase.prototype = {
    initialize: function () {
        $HBRootNS.DialogControlBase.callBaseMethod(this, 'initialize');
    },

    dispose: function () {
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

    get_dialogUrl: function () {
        return this._dialogUrl;
    },

    set_dialogUrl: function (value) {
        this._dialogUrl = value;
    },

    get_dialogFeature: function () {
        return this._dialogFeature;
    },

    set_dialogFeature: function (value) {
        this._dialogFeature = value;
    },

    _showDialog: function (arg, url) {
        if (!url)
            url = this.get_dialogUrl();

        return window.showModalDialog(url, arg, this.get_dialogFeature());
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

        switch (documentType) {
            case $HBRootNS.officeDocumentType.word:
                officeApp = $HBRootNS.HBCommon.createObject("Word.Application");
                break;
            case $HBRootNS.officeDocumentType.excel:
                officeApp = $HBRootNS.HBCommon.createObject("Excel.Application");
                break;
            case $HBRootNS.officeDocumentType.powerPoint:
                officeApp = $HBRootNS.HBCommon.createObject("PowerPoint.Application");
                break;
            case $HBRootNS.officeDocumentType.visio:
                officeApp = $HBRootNS.HBCommon.createObject("Visio.Application");
                break;
            default:
                officeApp = null;
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

$HBRootNS.fileIO.downloadFile = function (processPageUrl, localPath) {
    this._checkParams(processPageUrl, localPath);

    try {
        var xmlHttp = $HBRootNS.HBCommon.createObject("Msxml2.XMLHTTP");

        xmlHttp.open("POST", processPageUrl, false);
        xmlHttp.send(null);

        if (xmlHttp.status >= 400)
            throw Error.create($HBRootNS.fileIO.checkXmlHttpError(xmlHttp));

        var errorMessage = this._getInformationFromResponse(xmlHttp, this.errorMessageHeaderName);
        if (errorMessage != "")
            throw Error.create(errorMessage);

        var folderPath = localPath.substring(0, localPath.lastIndexOf("\\"));

        this.createFolder(folderPath);

        var stream = $HBRootNS.HBCommon.createObject("ADODB.Stream");
        stream.Type = $HBRootNS.fileStreamType.adTypeBinary;
        stream.Mode = $HBRootNS.fileStreamMode.adModeReadWrite;
        stream.Open();
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
        throw Error.create("写文件失败" + localPath + "！\n可能是没有将http://"
			+ document.location.host + "加入可信站点。\n错误信息：\n" + e.message);
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
        controlID = currentFile.container.get_controlID();

    var processPageUrl = currentFile.container.get_currentPageUrl()
		+ "?requestType=upload"
		+ "&lockID=" + currentFile.container.get_lockID()
		+ "&userID=" + currentFile.container.get_user().id
		+ "&rootPathName=" + currentFile.container.get_rootPathName()
		+ "&fileMaxSize=" + fileMaxSize
		+ "&controlID=" + controlID;

    this._checkParams(processPageUrl, localPath);

    if ($HBRootNS.officeDocument.checkIsOfficeDocument(localPath)) {
        var userLogOnName = currentFile.container.get_user().logOnName;
        localPath = this._copyFileToTempFolder(localPath, $HBRootNS.material.newMaterialID, userLogOnName);

        processPageUrl += "&fileName=" + escape(this.getFileNameWithExt(localPath));
    }
    else {
        processPageUrl += "&fileName=" + escape($HBRootNS.material.newMaterialID + "." + this.getFileExt(localPath));
    }

    try {
        var stream = $HBRootNS.HBCommon.createObject("ADODB.Stream");

        stream.type = $HBRootNS.fileStreamType.adTypeBinary;
        stream.open();

        try {
            $HBRootNS.fileIO._xmlHttp = $HBRootNS.HBCommon.createObject("Msxml2.XMLHTTP");

            this._loadStreamFile(stream, localPath);

            stream.position = 0;

            $HBRootNS.fileIO._xmlHttp.onreadystatechange = this._handleStateChange;

            $HBRootNS.fileIO._xmlHttp.open("POST", processPageUrl, true);
            $HBRootNS.fileIO._xmlHttp.send(stream);
        }
        finally {
            stream.close();
        }
    }
    catch (e) {
        alert("上传文件" + currentFile.filePath + "时发生错误：\n" + e.message);
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
    else
        return (tempFolderPath + "\\" + fileID + "." + $HBRootNS.fileIO.getFileExt(filePath));
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

    var fso = $HBRootNS.HBCommon.createObject("Scripting.FileSystemObject");

    if (fso == null)
        return;

    var f = fso.getFile(fileName);

    return new Date(f.dateLastModified * 1);
}

$HBRootNS.fileIO.getTempDirName = function () {
    var fso = $HBRootNS.HBCommon.createObject("Scripting.FileSystemObject");

    if (fso == null)
        return;

    var tempFolder = 2; //获得系统临时文件夹的系统变量

    return fso.GetSpecialFolder(tempFolder);
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
        var fso = $HBRootNS.HBCommon.createObject("Scripting.FileSystemObject");
        bExists = fso.fileExists(fileName);
    }
    catch (e) {
    }

    return bExists;
}

$HBRootNS.fileIO.folderExists = function (folderName) {
    var bExists = false;
    var fso = $HBRootNS.HBCommon.createObject("Scripting.FileSystemObject");
    bExists = fso.folderExists(folderName);
    return bExists;
}

$HBRootNS.fileIO.createFolder = function (folderName) {
    if (this.folderExists(folderName) == false) {
        var fso = $HBRootNS.HBCommon.createObject("Scripting.FileSystemObject");

        if (fso != null)
            fso.CreateFolder(folderName);
    }
}

$HBRootNS.fileIO.readFileContent = function (fileName) {
    if (this.fileExists(fileName) == false)
        return null;

    var fso = $HBRootNS.HBCommon.createObject("Scripting.FileSystemObject");
    var file = fso.GetFile(fileName);
    var ts = file.OpenAsTextStream(1);

    return ts.readAll();
}

$HBRootNS.fileIO.saveFileContent = function (fileName, content) {
    var fso = $HBRootNS.HBCommon.createObject("Scripting.FileSystemObject");

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
        var fso = $HBRootNS.HBCommon.createObject("Scripting.FileSystemObject");

        fso.DeleteFile(filePath);
    }
}

$HBRootNS.fileIO.copyFile = function (fromFilePath, toFilePath) {
    var fso = $HBRootNS.HBCommon.createObject("Scripting.FileSystemObject");

    if (fso.fileExists(fromFilePath)) {
        var file = fso.getFile(fromFilePath);
        file.Copy(toFilePath);
    }
}

$HBRootNS.fileIO.moveFile = function (fileName, newFileName) {
    var fso = $HBRootNS.HBCommon.createObject("Scripting.FileSystemObject");

    if (fso.fileExists(fileName)) {
        var file = fso.getFile(fileName);
        file.move(newFileName);
    }
}

$HBRootNS.fileIO.isFileOpened = function (fileName) {
    var bOpened = false;

    try {
        if (fileName != null && fileName.length > 0) {
            var fso = $HBRootNS.HBCommon.createObject("Scripting.FileSystemObject");

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

//=============================================校验相关 start=========================================={==========================================

//======校验涉及的数据类型-开始={=========
$HBRootNS.ValidationDataType = function (element) {
    throw Error.invalidOperation();
}

$HBRootNS.ValidationDataType.prototype = {
    Object: 1,
    Boolean: 3,
    Integer: 9,
    Decimal: 15,
    DateTime: 16,
    String: 18,
    Enum: 20
}

$HBRootNS.ValidationDataType.registerEnum($HBRootNSName + ".ValidationDataType");
//=========校验涉及的数据类型-完成=}========

//=========文本校验器的基类-开始={========
$HBRootNS.TextValidatorBase = function (dataType) {
    this._dataType = $HBRootNS.ValidationDataType.String;
}

$HBRootNS.TextValidatorBase.prototype = {
    validate: function (text) {
        throw Error.notImplemented();
    },

    get_dataType: function () {
        return this._dataType;
    },

    createTextValidationResult: function (text) {
        return { message: "", normalizedText: text, isValid: true };
    }
}

$HBRootNS.TextValidatorBase.registerClass($HBRootNSName + ".TextValidatorBase");

//Numeric check: numericCheck(nr, [intDigit], [fracDigit], [minValue], [maxValue])
$HBRootNS.TextValidatorBase._numericCheck = function (nr) {
    var nArgs = arguments.length;
    var nCount = 0;
    var nPointIndex = -1;
    var nSignIndex = -1;

    for (var i = 0; i < nr.length; i++) {
        var ch = nr.substr(i, 1);

        if (ch < "0" || ch > "9") {
            if (ch == ".") {
                if (nPointIndex != -1)
                    throw Error.create("数字类型只能有一个小数点");
                else
                    nPointIndex = i;
            }
            else
                if (ch == "-" || ch == "+") {
                    if (nSignIndex != -1)
                        throw Error.create("数字类型只能有一个\"" + ch + "\"");
                    else
                        nSignIndex = i;
                }
                else
                    if (ch != ",")	//过滤掉数字
                        throw Error.create("必需输入合法的数字");
        }
    }

    if (nPointIndex == -1)
        nPointIndex = nr.length;

    if (nArgs > 1) { //参数个数大于1
        var nNumber = nr * 1;
        var intDigit = arguments[1];
        var fracDigit;
        var minValue;
        var maxvalue;

        if (nArgs > 2) {
            fracDigit = arguments[2];
            if (nArgs > 3) {
                minValue = arguments[3];
                if (nArgs > 4)
                    maxValue = arguments[4];
            }
        }
    }

    $HBRootNS.HBCommon.trueThrow(typeof (intDigit) != "undefined" && (nr.substring(0, nPointIndex) * 1).toString().length > intDigit,
		 "整数部分的位数不能超过" + intDigit + "位");

    var strFrac = nr.substring(nPointIndex + 1, nr.length);

    if (strFrac.length > 0) {
        strFrac = "0." + strFrac;
        $HBRootNS.HBCommon.trueThrow(typeof (fracDigit) != "undefined" && (strFrac * 1).toString().length - 2 > fracDigit,
			"小数部分的位数不能超过" + fracDigit + "位");
    }

    if (typeof (minValue) != "undefined" && typeof (maxValue) != "undefined") {
        $HBRootNS.HBCommon.trueThrow((nr * 1) < minValue || (nr * 1) > maxValue, "数字必需在" + minValue + "和" + maxValue + "之间");
    }
    else
        if (typeof (minValue) != "undefined") {
            $HBRootNS.HBCommon.trueThrow((nr * 1) < minValue, "数字必需大于等于" + minValue);
        }
        else
            if (typeof (maxValue) != "undefined") {
                $HBRootNS.HBCommon.trueThrow((nr * 1) > maxValue, "数字必需小于等于" + maxValue);
            }
}

//=========文本校验器的基类-完成=}========

//=========整数文本校验器-开始={========
$HBRootNS.IntegerTextValidator = function () {
    $HBRootNS.IntegerTextValidator.initializeBase(this, [$HBRootNS.ValidationDataType.Integer]);
}

$HBRootNS.IntegerTextValidator.prototype = {
    validate: function (text) {
        var text = text.replace(/,/g, '');
        var result = this.createTextValidationResult(text);

        try {
            $HBRootNS.TextValidatorBase._numericCheck(text, 12, 0);
        }
        catch (e) {
            result.isValid = false;
            result.message = e.message;
        }
        return result;
    }
}

$HBRootNS.IntegerTextValidator.registerClass($HBRootNSName + ".IntegerTextValidator", $HBRootNS.TextValidatorBase);
//=========整数文本校验器-完成=}========

//=========数字文本校验器-开始={========
$HBRootNS.NumericTextValidator = function () {
    $HBRootNS.NumericTextValidator.initializeBase(this, [$HBRootNS.ValidationDataType.Integer]);
}

$HBRootNS.NumericTextValidator.prototype = {
    validate: function (text) {
        var text = text.replace(/,/g, '');
        var result = this.createTextValidationResult(text);

        try {
            $HBRootNS.TextValidatorBase._numericCheck(text);
        }
        catch (e) {
            result.isValid = false;
            result.message = e.message;
        }

        return result;
    }
}

$HBRootNS.NumericTextValidator.registerClass($HBRootNSName + ".NumericTextValidator", $HBRootNS.TextValidatorBase);
//=========数字文本校验器-完成=}========

$HBRootNS.ValidationDataType._textValidators = [];

$HBRootNS.ValidationDataType.registerTextValidator = function (dType, vdt) {
    $HBRootNS.ValidationDataType._textValidators.push({ dataType: dType, validator: vdt });
}

$HBRootNS.ValidationDataType.getTextValidator = function (dType) {
    var result = null;

    for (var i = 0; i < $HBRootNS.ValidationDataType._textValidators.length; i++) {
        if ($HBRootNS.ValidationDataType._textValidators[i].dataType == dType) {
            result = $HBRootNS.ValidationDataType._textValidators[i];
            break;
        }
    }

    return result;
}

$HBRootNS.ValidationDataType.get_strongTypeValue = function (dataType, text) {
    var result = text;

    switch (dataType) {
        case $HBRootNS.ValidationDataType.Integer:
            text = text == "" ? 0 : text;
            result = parseInt(text);
            break;
        case $HBRootNS.ValidationDataType.Decimal:
            text = text == "" ? 0 : text;
            result = parseFloat(text);
            break;
    }

    return result;
}

//注册默认的文本校验器
$HBRootNS.ValidationDataType.registerTextValidator($HBRootNS.ValidationDataType.Integer, new $HBRootNS.IntegerTextValidator());
$HBRootNS.ValidationDataType.registerTextValidator($HBRootNS.ValidationDataType.Decimal, new $HBRootNS.NumericTextValidator());

//=========控件校验绑定校验器的基类-开始={========
$HBRootNS.ValidationBinderBase = function () {
    this._formatString = "";
    this._dataType = $HBRootNS.ValidationDataType.String;
    this._control = null;
}

$HBRootNS.ValidationBinderBase.prototype = {
    bind: function () {
        throw Error.notImplemented();
    },

    get_formatString: function () {
        return this._formatString;
    },

    set_formatString: function (value) {
        this._formatString = value;
    },

    get_dataType: function () {
        return this._dataType;
    },

    set_dataType: function (value) {
        this._dataType = value;
    },

    get_control: function () {
        return this._control;
    },

    set_control: function (value) {
        this._control = value;
    },

    get_controlValue: function () {
        throw Error.notImplemented();
    },

    set_controlValue: function (value) {
        throw Error.notImplemented();
    },

    checkControl: function () {
    },

    add_dataChange: function (handler) {
        this.get_events().addHandler('dataChange', handler);
    },

    remove_dataChange: function (handler) {
        this.get_events().removeHandler('dataChange', handler);
    },

    raise_dataChange: function (normalizedText, strongTypeValue) {
        var handler = this.get_events().getHandler("dataChange");

        if (handler) {
            var e = {};

            e.binder = this;
            e.normalizedText = normalizedText;
            e.strongTypeValue = strongTypeValue;

            handler(this, e);
        }
    },

    pseudo: function () {
    }
}

$HBRootNS.ValidationBinderBase.registerClass($HBRootNSName + ".ValidationBinderBase", Sys.Component);
//=========控件校验绑定校验器的基类-完成=}========

//=========文本控件校验绑定校验器-开始={========
$HBRootNS.TextBoxValidationBinder = function () {
    $HBRootNS.TextBoxValidationBinder.initializeBase(this);

    this._formatString = "";
    this._dataType = $HBRootNS.ValidationDataType.String;
    this._control = null;
    this._currentValidator = null;
    this._originalText = null;
}

$HBRootNS.TextBoxValidationBinder.prototype = {

    bind: function () {
        this.checkControl();

        this._originalText = this.get_controlValue();

        $addHandlers(this._control, {
            change: Function.createDelegate(this, this._onInputTextChange),
            keypress: Function.createDelegate(this, this._onInputTextKeyPressed)
        });
    },

    get_controlValue: function () {
        return this.get_control().value;
    },

    get_controlUnformattedValue: function () {
        var rawValue = this.get_control().value;
        var result = rawValue;

        switch (this.get_dataType()) {
            case $HBRootNS.ValidationDataType.Integer:
            case $HBRootNS.ValidationDataType.Decimal:
                result = rawValue.toString().replace(/,/g, '');
                break;
        }

        return result;
    },

    set_controlValue: function (value) {
        this.get_control().value = value;
    },

    _onInputTextChange: function (e) {
        try {
            var normalizedText = this._validateText();
            var controlValue = normalizedText;
            var strongTypeValue = $HBRootNS.ValidationDataType.get_strongTypeValue(this.get_dataType(), controlValue);
            if (this.get_formatString() != null && this.get_formatString() != "") {
                controlValue = String.format(this.get_formatString(), strongTypeValue);
            }

            this.raise_dataChange(controlValue, strongTypeValue);

            //这里需要重新的从input中获得新的值，因为可能用户会响应change事件处理了input的值(linbin)
            normalizedText = this._validateText();
            controlValue = normalizedText;
            strongTypeValue = $HBRootNS.ValidationDataType.get_strongTypeValue(this.get_dataType(), controlValue);
            if (this.get_formatString() != null && this.get_formatString() != "") {
                controlValue = String.format(this.get_formatString(), strongTypeValue);
            }

            this.set_controlValue(controlValue);

            this._originalText = controlValue;
        }
        catch (e) {
            $showError(e);
            this.set_controlValue(this._originalText);

            var ctrl = this.get_control();

            window.setTimeout(
                function () {
                    ctrl.focus();
                },
            0);
        }
    },

    _validateText: function () {
        var normalizedText = this.get_controlValue();

        if (this._currentValidator == null || this._currentValidator.dataType != this.get_dataType()) {
            this._currentValidator = $HBRootNS.ValidationDataType.getTextValidator(this.get_dataType());
        }

        if (this._currentValidator) {
            var validateResult = this._currentValidator.validator.validate(this.get_controlUnformattedValue());

            if (!validateResult.isValid)
                throw Error.create(validateResult.message);

            normalizedText = validateResult.normalizedText;
        }

        return normalizedText;
    },

    _onInputTextKeyPressed: function (e) {
        switch (this.get_dataType()) {
            case $HBRootNS.ValidationDataType.Integer:
                if ((e.rawEvent.keyCode < 48 || e.rawEvent.keyCode > 57)) {
                    e.rawEvent.keyCode = 0;
                    e.rawEvent.returnValue = false;
                }
                break;
            case $HBRootNS.ValidationDataType.Decimal:
                if ((e.rawEvent.keyCode < 48 || e.rawEvent.keyCode > 57) && e.rawEvent.keyCode != 46) {
                    e.rawEvent.keyCode = 0;
                    e.rawEvent.returnValue = false;
                }
                break;
        }
    },

    checkControl: function () {
        $HBRootNS.HBCommon.falseThrow(this.get_control() != null && this.get_control().tagName == "INPUT",
			"TextBoxValidationBinder对应的Control不能为空，且必须是INPUT");
    }
}

$HBRootNS.TextBoxValidationBinder.registerClass($HBRootNSName + ".TextBoxValidationBinder", $HBRootNS.ValidationBinderBase);
//=========文本控件校验绑定校验器=}========

//=============================================校验相关end }==========================================
