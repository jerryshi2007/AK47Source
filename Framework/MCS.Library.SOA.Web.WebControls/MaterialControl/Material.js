// -------------------------------------------------
// FileName	：	Material.js
// Remark	：	文件对象
// -------------------------------------------------
// VERSION		AUTHOR		DATE			CONTENT
// 1.0			张梁		20070724		创建
// -------------------------------------------------

//material版本类型
$HBRootNS.materialVersionType = function () {
	throw Error.invalidOperation();
}

$HBRootNS.materialVersionType.prototype =
{
	Normal: 0, 		//正常
	CopyVersion: 1, //副本
	OtherVerion: 2		//其他版本
}

$HBRootNS.materialVersionType.registerEnum($HBRootNSName + ".materialVersionType");

//文件路径类型
$HBRootNS.pathType = function () {
	throw Error.invalidOperation();
}

$HBRootNS.pathType.prototype =
{
	relative: 0, 	//相对
	absolute: 1		//绝对
}

$HBRootNS.pathType.registerEnum($HBRootNSName + ".pathType");

$HBRootNS.material = function (element) {
	$HBRootNS.material.initializeBase(this, [element]);

	this._materialID = null; 					//唯一标识
	this._department = new Object(); 	//部门
	this._resourceID = null; 			//表单ID
	this._sortID = 0; 					//顺序号
	this._materialClass = null; 			//类别
	this._title = "正文"; 				//标题
	this._pageQuantity = 0; 				//页数
	this._relativeFilePath = null; 		//相对路径
	this._originalName = null; 			//原始名称
	this._creator = new Object(); 		//创建人
	this._lastUploadTag = null; 			//最后上传标记
	this._createDateTime = null; 		//创建时间

	this._wfProcessID = null; 			//工作流流程ID
	this._wfActivityID = null; 			//工作流活动ID
	this._wfActivityName = null; 		//工作流活动名称
	this._parentID = null; 				//父版本ID
	this._sourceMaterial = null; 		//原始附件对象
	this._versionType = $HBRootNS.materialVersionType.Normal; //版本类型
	this._extraData = {}; 				//附加数据
	this._fileIconPath = null; 			//文件图标路径
	this._showFileUrl = ""; 				//显示文件的临时路径

	this._isDraft = false; 				//是否为新文件
	this._applicationType = null; 		//文件类型
	this._downloadTime = null; 			//下载时的原文件在客户端的时间
	this._uploadTime = null; 			//最后上传时文件在客户端的时间
	this._downloaded = false; 			//最新版本是否已下载到本地
	this._localPath = null; 				//文件在本地的全路径
	this._versionImage = null; 			//显示版本的图标对象
	this._operateImage = null; 			//用于提示操作的图标对象
	this._fileIcon = null; 				//文件图标对象
	this._link = null; 					//操作文件使用的超链接对象
	this._officeApp = null; 				//最后一次使用的officeApp对象
	this._officeDoc = null;
	this._localMaterial = null; 			//创建Material对象时对该属性赋初值。下载文件时修改本属性。提交时保存到本地信息文件

	this._container = null; 				//所在的控件
	this._isOfficeDocument = false; 		//是否为office文件

	this._modifyTime = null; 			//修改时间

	//this._localIsModified = false;               //本地是否修改过

	this._lastUploadFileTime = null; //本地文件最后上传时的文件时间
	this._lastSaveFileTime = null; //本地文件最后保存时的文件时间

	this._localDownloadTime = null;
	this._localModifyTime = null;
	this._localLastUploadTag = null; 	//文件在一次操作中的最后上传标记.上传文件时更新本属性。提交表单时更新到本地信息文件

	this._realLocalPath = null;

	this._linkButtonEvents =
		{
			click: Function.createDelegate(this, this.click)
		};
	this._uploadEvents =
		{
			click: Function.createDelegate(this, this._uploadClick)
		};
	this._showVersionEvents =
		{
			click: Function.createDelegate(this, this._showVersionClick)
		};
	this._onOpenFileEvents =
		{
			click: Function.createDelegate(this, this._checkDownloadFileInline)
		}
}

$HBRootNS.material.prototype =
{
	initialize: function () {
		$HBRootNS.material.callBaseMethod(this, "initialize");

		if (this._container._allowEdit && this._container._allowEditContent == true) {

			this._localPath = this._getLocalPath();
			this._realLocalPath = this._localPath;
			this._localLastUploadTag = this._lastUploadTag;
			this.loadLocalData();
		}

		if (this._isDraft) {
			this._title = this._container.get_materialTitle();
		}

		this._isOfficeDocument = $HBRootNS.officeDocument.checkIsOfficeDocument(this._relativeFilePath);

		Array.add($HBRootNS.material.allMaterials, this);
	},

	//        _getLocalPath: function (allowEditContent) {
	//            debugger;
	//            var localPath = "";

	//            if (allowEditContent)
	//                localPath = $HBRootNS.fileIO.getTempDirName() + "\\" + this._container.get_user().logOnName + "\\" + this._materialID + ".";
	//            else
	//                localPath = this._container.get_user().logOnName + "\\" + this._materialID + ".";

	//            if (this._isDraft) {
	//                var templateUrl = this._container.get_templateUrl();
	//                localPath += (templateUrl != "" ? $HBRootNS.fileIO.getFileExt(templateUrl) : "doc");
	//            }
	//            else {
	//                localPath += $HBRootNS.fileIO.getFileExt(this._relativeFilePath);
	//            }

	//            return localPath;
	//        },
	_getLocalPath: function () {
		var localPath = "";
		//这里目录去掉logOnName路径，是因为officeviewer控件没有创建目录的方法。
		localPath = $HBRootNS.fileIO.getTempDirName() + "\\" + this._container.get_user().logOnName + "\\" + this._materialID + ".";
		if (this._isDraft) {
			var templateUrl = this._container.get_templateUrl();
			localPath += (templateUrl != "" ? $HBRootNS.fileIO.getFileExt(templateUrl) : "doc");
		}
		else {
			localPath += $HBRootNS.fileIO.getFileExt(this._relativeFilePath);
		}

		return localPath;
	},

	_initLocalData: function () {
		var data = $HBRootNS.MaterialPersistence.loadData(this._materialID, this._container.get_user().logOnName);

		if (data == null) {
			data = this._createNewLocalData();
			data.uploadTag = this._lastUploadTag;
			$HBRootNS.MaterialPersistence.saveData(this._materialID, data, this._container.get_user().logOnName);
		}

		this._localDownloadTime = data.downloadTime;
		this._localLastUploadTag = data.uploadTag;
		this._lastUploadFileTime = data.lastUploadFileTime;
		this._lastSaveFileTime = data.lastSaveFileTime;
	},

	_clearLocalData: function () {
		$HBRootNS.MaterialPersistence.removeData(this._materialID, this._container.get_user().logOnName);
		this._localDownloadTime = null;
		this._localLastUploadTag = null;
		this._isModified = false;
	},

	_createNewLocalData: function () {
		var objData = { downloadTime: null, lastUploadFileTime: null, lastSaveFileTime: null, uploadTag: null };
		return objData;
	},

	persistLocalData: function (data) {
		var oldData = $HBRootNS.MaterialPersistence.loadData(this._materialID, this._container.get_user().logOnName);
		if (oldData == null) {
			oldData = this._createNewLocalData();
		}
		if (typeof (data.downloadTime) != "undefined") {
			var timeStr = data.downloadTime.format("yyyy-MM-dd HH:mm:ss.fff");
			oldData.downloadTime = timeStr; //变成非引用类型，要不可能序列化报错
			//这里不能设置this的字段，否则会在序列化时产生“不能执行已释放script异常”
			//this._localDownloadTime = data.downloadTime;
		}

		if (typeof (data.lastUploadFileTime) != "undefined") {
			oldData.lastUploadFileTime = data.lastUploadFileTime;
			//this._localIsModified = data.isModified;
		}
		if (typeof (data.lastSaveFileTime) != "undefined") {
			oldData.lastSaveFileTime = data.lastSaveFileTime;
			//this._localIsModified = data.isModified;
		}
		if (typeof (data.uploadTag) != "undefined") {
			oldData.uploadTag = data.uploadTag;
			//this._localLastUploadTag = data.uploadTag;
		}
		//        if (typeof (data.modifyTime) != "undefined") {
		//            var timeStr = data.modifyTime.format("yyyy-MM-dd HH:mm:ss.fff");
		//            oldData.modifyTime = timeStr;
		//            //this._localModifyTime = data.modifyTime;
		//        }

		$HBRootNS.MaterialPersistence.saveData(this._materialID, oldData, this._container.get_user().logOnName);
	},

	loadLocalData: function () {
		var data = $HBRootNS.MaterialPersistence.loadData(this._materialID, this._container.get_user().logOnName);
		if (data != null) {
			if (data.downloadTime != null) {
				if (typeof (data.downloadTime) == "string") {
					this._localDownloadTime = Date.parseInvariant(data.downloadTime, "yyyy-MM-dd HH:mm:ss.fff");
				}
				else {
					this._localDownloadTime = data.downloadTime;
				}
			}

			if (data.lastUploadFileTime != null) {
				this._lastUploadFileTime = data.lastUploadFileTime;
			}

			if (data.lastSaveFileTime != null) {
				this._lastSaveFileTime = data.lastSaveFileTime;
			}

			if (data.uploadTag != null) {
				this._localLastUploadTag = data.uploadTag;
			}

			if (data.modifyTime != null) {
				if (typeof (data.modifyTime) == "string") {
					this._localModifyTime = Date.parseInvariant(data.modifyTime, "yyyy-MM-dd HH:mm:ss.fff");
				}
				else {
					this._localModifyTime = data.modifyTime;
				}
			}

			if (this._lastUploadFileTime == null) {
				var modifyTime = this._getModifyTime();
				this._lastUploadFileTime = modifyTime == null ? null : modifyTime.format("yyyy-MM-dd HH:mm:ss.fff");
			}

			if (this._lastSaveFileTime == null) {
				this._lastSaveFileTime = "";
			}
		}
	},

	setSaveTime: function () {
		var data = new Object();
		data.lastSaveFileTime = this._getModifyTime().format("yyyy-MM-dd HH:mm:ss.fff");
		this.persistLocalData(data);
		this.loadLocalData();
	},

	dispose: function () {
		Array.remove($HBRootNS.material.allMaterials, this);

		if (this._link)
			$clearHandlers(this._link);
		else if (this._fileIcon)
			$clearHandlers(this._fileIcon);

		if (this.get_canEditContent() && this._operateImage)
			$clearHandlers(this._operateImage);

		this._versionImage = null;
		this._operateImage = null;
		this._fileIcon = null;
		this._link = null;
		this._officeApp = null;
		this._officeDoc = null;
		this._localMaterial = null;
		this._department = null;
		this._creator = null;
		this._sourceMaterial = null;

		$HBRootNS.material.callBaseMethod(this, "dispose");
	},

	get_canEditContent: function () {
		var result = this._isDraft && this._container.get_allowEditContent();

		if (result == false) {
			result = this._container.get_allowEdit() && this._container.get_allowEditContent() && this._isOfficeDocument;

			if (result) {
				var currentActivityID = this._container.get_wfActivityID();
				if (currentActivityID != null && currentActivityID != "") {
					var acm = this._container.get_activityControlMode();

					result = (acm & $HBRootNS.materialActivityControlMode.AllowEditContent) != $HBRootNS.materialActivityControlMode.None
							|| currentActivityID == this._wfActivityID;
				}
			}
		}

		return result;
	},

	_clear: function (element) {
		if (this._versionImage) {
			if (element.childNodes.length != 0)
				element.removeChild(this._versionImage);

			this._versionImage = null;
		}

		if (this._operateImage) {
			if (this.get_canEditContent())
				$clearHandlers(this._operateImage);

			if (element.childNodes.length != 0)
				element.removeChild(this._operateImage);

			this._operateImage = null;
		}

		if (this._fileIcon) {
			$clearHandlers(this._fileIcon);

			if (element.childNodes.length != 0)
				element.removeChild(this._fileIcon);

			this._fileIcon = null;
		}

		if (this._link) {
			if (this._container.get_showFileTitle() == true)
				$clearHandlers(this._link);

			if (element.childNodes.length != 0)
				element.removeChild(this._link);

			this._link = null;
		}
	},

	_initInformation: function () {
		if (this._materialID != $HBRootNS.material.newMaterialID && this.get_canEditContent()) {
			if (this._isDraft == true)
				this._localMaterial = this._generateNewMaterial($HBRootNS.fileIO.getFileNameWithExt(this._localPath));
			else
				this._getMaterialInformation();
		}
	},

	generateMaterial: function () {
		var material = new Object();
		material.id = this._materialID;
		material.department = this._department;
		material.resourceID = this._resourceID;
		material.sortID = this._sortID;
		material.materialClass = this._materialClass;
		material.title = this._title;
		material.pageQuantity = this._pageQuantity;
		material.relativeFilePath = this._relativeFilePath;
		material.originalName = this._originalName;
		material.creator = this._creator;
		material.lastUploadTag = this._lastUploadTag;
		material.createDateTime = this._createDateTime;
		material.modifyTime = this._getModifyTime();
		material.isModified = this._isModified;
		material.wfProcessID = this._wfProcessID;
		material.wfActivityID = this._wfActivityID;
		material.wfActivityName = this._wfActivityName;
		material.parentID = this._parentID;
		material.sourceMaterial = this._sourceMaterial;
		material.versionType = this._versionType;
		material.extraData = this._extraData;
		material.fileIconPath = (this._fileIconPath == "" ? this._container.get_defaultFileIconPath() : this._fileIconPath);
		material.showFileUrl = this._showFileUrl;
		return material;
	},

	_showVersionImage: function (element) {
		if (this._isDraft == false && this._container.get_showAllVersions() == true) {
			this._versionImage = $HGDomElement.createElementFromTemplate(
				{
					nodeName: "img",
					properties:
					{
						src: this._container.get_showVersionImagePath(),
						title: "文件版本",
						align: "absmiddle",
						style:
						{
							cursor: "pointer"
						}
					},
					events: this._showVersionEvents
				},
				element
				);
		}
	},

	_showStatusImage: function (element) {
		if (this.get_canEditContent()) {
			var imgeUrl;

			imgeUrl = this._container.get_editImagePath();

			this._operateImage = $HGDomElement.createElementFromTemplate(
				{
					nodeName: "img",
					properties:
					{
						align: "absmiddle",
						src: imgeUrl
					}
				},
				element
				);
		}
		else if (this._container.get_emptyImagePath() != null) {
			this._operateImage = $HGDomElement.createElementFromTemplate(
				{
					nodeName: "img",
					properties:
					{
						align: "absmiddle",
						src: this._container.get_emptyImagePath()
					}
				},
				element);
		}
	},

	_showFileIcon: function (element) {
		if (this.get_canEditContent() == false && this._originalName == null) {
			return;
		}
		var fileIconPath = this._fileIconPath;

		if (fileIconPath == null || fileIconPath == "") {
			if (this._isDraft && this._downloadTime == null)
				fileIconPath = this._container.get_emptyImagePath();
			else
				fileIconPath = this._container.get_defaultFileIconPath();
		}

		this._fileIcon = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "a"
			},
			element);

		if (this.get_canEditContent() == false) {
			var processPageUrl = this._showFileUrl;

			if (processPageUrl == null || processPageUrl == "" || this.isInServerTempFolder())
				processPageUrl = this._getDownloadUrl(true);

			this._fileIcon.href = processPageUrl;
			$addHandlers(this._fileIcon, this._onOpenFileEvents);
			this._fileIcon.target = "_blank";
		}
		else {
			$addHandlers(this._fileIcon, this._linkButtonEvents);
		}

		$HGDomElement.createElementFromTemplate(
			{
				nodeName: "img",
				properties:
				{
					src: fileIconPath,
					border: "0",
					align: "absmiddle",
					style:
					{
						cursor: "pointer",
						paddingRight: "2px"
					},
					title: this._title
				}
			},
			this._fileIcon);
	},

	_getShowTitle: function () {
		var showTitle;
		//var originalTitle = this._title;
		var originalTitle = this._originalName; //修改，将文件名作为title显示

		if (this._isDraft) {
			if (this._downloaded)
				showTitle = (originalTitle != null && originalTitle != "" && this._container.get_materialUseMode() == $HBRootNS.materialUseMode.DraftAndUpload) ? originalTitle : this._container.get_editText();
			else
				showTitle = this._container.get_draftText();
		}
		else if (this._container.get_materialUseMode() == $HBRootNS.materialUseMode.SingleDraft) {
			if (this.get_canEditContent())
				showTitle = this._container.get_editText();
			else
				showTitle = this._container.get_displayText();
		}
		else
			showTitle = originalTitle;

		return showTitle;
	},

	_showFileTitle: function (element) {
		if (this.get_canEditContent() == false && this._originalName == null) {
			return;
		}
		if (this._container.get_showFileTitle() == true || this._isDraft) {
			var showTitle = this._getShowTitle();

			this._link = $HGDomElement.createElementFromTemplate(
				{
					nodeName: "a",
					properties:
					{
						innerText: this._getShowTitle(),
						href: "#",
						title: this._title && this._title != "" ? this._title : showTitle,
						style:
						{
							cursor: "pointer"
						}
					}
				},
				element);

			if (this.get_canEditContent() == false) {
				var processPageUrl = this._showFileUrl;

				if (processPageUrl == null || processPageUrl == "" || this.isInServerTempFolder())
					processPageUrl = this._getDownloadUrl(true);

				this._link.href = processPageUrl;
				$addHandlers(this._link, this._onOpenFileEvents);
				this._link.target = "_blank";
			}
			else {
				$addHandlers(this._link, this._linkButtonEvents);
			}
		}
	},

	_checkDownloadFileInline: function () {
		$HBRootNS.material.checkDownloadFileInline(this._originalName);
	},

	render: function () {
		//this._initInformation();

		var element = this.get_element();
		element.style.marginLeft = "4px";

		var doc = $HGDomElement.get_currentDocument();

		this._clear(element);

		this._showVersionImage(element);

		if (this._container.get_allowEditContent() == true)
			this._showStatusImage(element);

		this._showFileIcon(element);
		this._showFileTitle(element);

		this.refreshStatus();
	},

	click: function () {
		if (this.get_canEditContent() == false)
			return false;

		this._afterClick(false, true);
	},

	_afterClick: function (isPrint, isFirstTime) {
		var executedDownload = false;

		if (this._isDraft) {
			executedDownload = this._downloadTemplate(isPrint);
			this._isOfficeDocument = $HBRootNS.officeDocument.checkIsOfficeDocument(this._realLocalPath);
		}
		else
			executedDownload = this._downloadDocument();

		if (this._isOfficeDocument) {
			this._officeDoc = $create($HBRootNS.officeDocument,
        			{
        				localPath: isPrint == true ? this._getLocalPath(true) : this._realLocalPath,
        				officeApp: this._officeApp,
        				userName: this._container.get_user().displayName,
        				visible: true,
        				trackRevisions: this._container.get_trackRevisions()
        			},
        			null, null, null);

			this._officeApp = this._officeDoc.get_officeApp();
		}

		if (this._isDraft) {
			this._title = this._getNextDraftTitle();

			var data = new Object();
			data.downloadTime = new Date();
			data.uploadTag = "";
			//data.isModified = true;
			this.persistLocalData(data);
			this.loadLocalData();
		}

		if (this._container.raiseDocumentDownload(null, executedDownload, this) == true) {
			if (this._container._editDocumentInCurrentPage == true) {
				this._container.openDocument(this, isFirstTime);
				this._container.raiseDocumentOpen(null);
			}
			else {
				this._officeDoc.open();
				this._container.raiseDocumentOpen(this._officeDoc);
			}
		}
	},

	//打印下载文件使用
	clickForPrint: function () {
		this._afterClick(true);
	},

	_getNextDraftTitle: function () {
		var index = -1;
		var materials = this._container.get_materials();

		if (materials != null) {
			for (var i = 0; i < materials.length; i++) {
				if (materials[i].title.indexOf(this._container.get_materialTitle()) == 0)
					index += 1;
			}
		}

		if (index == 0)
			return this._container.get_materialTitle();
		else
			return String.format(this._container.get_materialTitle() + "({0})", index);
	},

	_uploadClick: function () {
		this._upload();
	},

	_setNewMaterialInfo: function () {
		var xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");

		var templateUrl = this._container.get_templateUrl();
		var newFileName = "newfile." + (templateUrl != "" ? $HBRootNS.fileIO.getFileExt(templateUrl) : "doc");

		var processPageUrl = String.format("{0}?requestType=getnewmaterialinfo&fileName={1}&requestContext={2}&t={3}",
				this._container.get_currentPageUrl(),
				escape(newFileName),
				escape(this._container.get_requestContext()),
                Date.parse(new Date()));

		try {
			xmlhttp.open("GET", processPageUrl, false);
			xmlhttp.send();

			if (xmlhttp.status == 200) {
				var str = xmlhttp.responseText;
				var materialInfo = Sys.Serialization.JavaScriptSerializer.deserialize(str);
				this._materialID = materialInfo.materialID;

				this._fileIconPath = materialInfo.fileIconPath == "" ?
                this._container.get_defaultFileIconPath() : materialInfo.fileIconPath; ;
			}
			else {
				alert("获取新增附件ID出错！");
			}

		}
		catch (e) {
			alert("获取新增附件ID出错——" + e);
		}
	},

	_downloadTemplate: function (isPrint) {
		if (isPrint == false && (this.get_canEditContent() == false || this._container.get_templateUrl() == "" || this._downloaded))
			return false;

		//this._setNewMaterialInfo();

		this._localPath = this._getLocalPath();

		var fileName = $HBRootNS.fileIO.getFileNameWithExt(this._localPath);

		var opType = "Template";

		if (isPrint)
			opType = "Print";

		var processPageUrl = String.format("{0}?requestType=download&rootPathName={1}&fileName={2}&controlID={3}&pathType={4}&userID={5}&opType={6}&filePath={7}&requestContext={8}",
    			this._container.get_currentPageUrl(),
    			this._container.get_rootPathName(),
    			escape(fileName),
    			this._container.get_uniqueID(),
    			$HBRootNS.pathType.absolute,
    			this._container.get_user().id,
    			opType,
    			escape(this._container.get_templateUrl()),
    			escape(this._container.get_requestContext()));

		//var componentHelper = $HGDomElement.get_currentDocument().getElementById(this._container.get_componentHelperActiveXID());
		var returnInfo = $HBRootNS.fileIO.downloadFile(processPageUrl,
            		(isPrint == true ? this._localPath.replace(this._materialID, $HBRootNS.material.newMaterialID) : this._localPath),
					"POST",
					this._container.get_downloadTemplateWithViewState());

		this._localPath = this._localPath.replace(this._materialID, returnInfo.materialID);
		this._realLocalPath = this._localPath;
		this._materialID = returnInfo.materialID;

		var modifyTime = this._getModifyTime();
		this._lastUploadFileTime = ""; //modifyTime.format("yyyy-MM-dd HH:mm:ss.fff");
		this._lastSaveFileTime = this._lastUploadFileTime;

		fileName = $HBRootNS.fileIO.getFileNameWithExt(this._localPath);

		if (returnInfo.fileIconPath != "")
			this._fileIconPath = returnInfo.fileIconPath;
		else
			this._fileIconPath = this._container.get_defaultFileIconPath();

		this._localMaterial = this._generateNewMaterial(fileName);

		var data = new Object();
		data.downloadTime = new Date();
		data.uploadTag = this._lastUploadTag;

		data.lastUploadFileTime = this._lastUploadFileTime;
		data.lastSaveFileTime = this._lastSaveFileTime;

		this.persistLocalData(data);

		this._localMaterial = this._generateNewMaterial(fileName);

		this._downloaded = true;
		this._downloadTime = new Date();

		return true;
	},

	//    _downloadTemplate1: function (isPrint) {
	//        if (isPrint == false && (this.get_canEditContent() == false || this._container.get_templateUrl() == "" || this._downloaded))
	//            return false;

	//        this._setNewMaterialInfo();

	//        this._localPath = this._getLocalPath();

	//        var fileName = $HBRootNS.fileIO.getFileNameWithExt(this._localPath);

	//        var opType = "Template";

	//        if (isPrint)
	//            opType = "Print";

	//        var processPageUrl = String.format("{0}?requestType=download&rootPathName={1}&fileName={2}&controlID={3}&pathType={4}&userID={5}&opType={6}&filePath={7}&requestContext={8}",
	//				this._container.get_currentPageUrl(),
	//				this._container.get_rootPathName(),
	//				escape(fileName),
	//				this._container.get_uniqueID(),
	//				$HBRootNS.pathType.absolute,
	//				this._container.get_user().id,
	//				opType,
	//				escape(this._container.get_templateUrl()),
	//				escape(this._container.get_requestContext()));
	//        processPageUrl = "http://" + window.location.host + processPageUrl;
	//        processPageUrl += "&nameForOV=/" + fileName;

	//        var tempLocalPath = this._container.download(processPageUrl);
	//        this._realLocalPath = tempLocalPath;

	//        var data = new Object();
	//        data.downloadTime = new Date();
	//        data.uploadTag = this._lastUploadTag;
	//        //data.isModified = true;
	//        this.persistLocalData(data);

	//        this._localMaterial = this._generateNewMaterial(fileName);

	//        this._downloaded = true;
	//        this._downloadTime = new Date();

	//        return true;
	//    },

	_getDownloadUrl: function (readonly, showFileUrl) {
		var fileName = $HBRootNS.fileIO.getFileNameWithExt(this._originalName.replace("/", "\\"));

		if (typeof (showFileUrl) == "undefined")
			showFileUrl = this._showFileUrl;

		var processPageUrl = String.format("{0}?requestType=download&rootPathName={1}&fileName={2}&controlID={3}&pathType={4}&userID={5}&filePath={6}&materialID={7}&opType=Document&requestContext={8}",
				this._container.get_currentPageUrl(),
				escape(this._container.get_rootPathName()),
				escape(fileName),
				escape(this._container.get_uniqueID()),
				$HBRootNS.pathType.relative,
				this._container.get_user().id,
				escape(showFileUrl != "" ? showFileUrl : this._relativeFilePath),
				escape(this._materialID),
				escape(this._container.get_requestContext()));

		if (readonly == true)
			processPageUrl += "&fileReadonly=true";

		//老方式不用加
		//        processPageUrl += "&nameForOV=/" + fileName;
		//        processPageUrl = "http://" + window.location.host + processPageUrl; //不加这两行，IE崩溃。。。

		return processPageUrl;
	},

	//    _downloadDocument: function () {
	//        if (this._downloaded)
	//            return false;

	//        var fileName = $HBRootNS.fileIO.getFileNameWithExt(this._localPath);

	//        var processPageUrl = this._getDownloadUrl(false);

	//        //如果和服务器上版本一致,则不需要下载。
	//        if ($HBRootNS.fileIO.fileExists(this._localPath) && this._localMaterial != null
	//			&& this._lastUploadTag <= this._localMaterial.lastUploadTag)
	//            return false;

	//        $HBRootNS.fileIO.downloadFile(processPageUrl, this._getLocalPath(true));

	//        //下载已存在的文件(更新本地Material对象)
	//        this._localMaterial = this.generateMaterial();

	//        this._localMaterial.modifyTime = this._getModifyTime() * 1;

	//        if (this._isOfficeDocument)
	//            this._setMaterialInformation();

	//        this._downloaded = true;
	//        this._downloadTime = new Date();

	//        return true;
	//    },

	_downloadDocument: function () {

		if (this._downloaded)
			return false;

		//var fileName = $HBRootNS.fileIO.getFileNameWithExt(this._localPath);

		var processPageUrl = this._getDownloadUrl(false, "");

		//如果和服务器上版本一致,则不需要下载。
		//        if (this._localPath != "" && this._localMaterial != null
		//			&& this._lastUploadTag <= this._localMaterial.lastUploadTag)
		//            return false;

		//以上逻辑替换为
		if (this._localDownloadTime != null && this._localLastUploadTag != null &&
        this._localLastUploadTag >= this._lastUploadTag) {
			return false;
		}

		var tempLocalPath = this._getLocalPath(true);
		//var componentHelper = document.getElementById(this._container.get_componentHelperActiveXID());
		$HBRootNS.fileIO.downloadFile(processPageUrl, tempLocalPath, "POST", this._container.get_downloadWithViewState());
		//var tempLocalPath = this._container.download(processPageUrl);
		this._realLocalPath = tempLocalPath;

		var modifyTime = this._getModifyTime();
		this._lastUploadFileTime = modifyTime.format("yyyy-MM-dd HH:mm:ss.fff");
		this._lastSaveFileTime = this._lastUploadFileTime;

		var data = new Object();
		data.downloadTime = new Date();
		data.uploadTag = this._lastUploadTag;
		data.lastUploadFileTime = this._lastUploadFileTime;
		data.lastSaveFileTime = this._lastSaveFileTime;

		this.persistLocalData(data);

		//下载已存在的文件(更新本地Material对象)
		this._localMaterial = this.generateMaterial();

		//this._localMaterial.modifyTime = this._getModifyTime() * 1;

		//        if (this._isOfficeDocument)
		//            this._setMaterialInformation();

		this._downloaded = true;
		this._downloadTime = new Date();

		return true;
	},

	_generateNewMaterial: function (fileName) {
		var newMaterial = new Object();
		newMaterial.id = this._materialID;
		newMaterial.department = this._container.get_department();
		newMaterial.resourceID = this._container.get_defaultResourceID();
		newMaterial.sortID = 0;
		newMaterial.materialClass = this._container.get_defaultClass();
		newMaterial.title = "正文";
		newMaterial.pageQuantity = 0;
		newMaterial.relativeFilePath = this._container.get_relativePath() + fileName;
		newMaterial.originalName = fileName;
		newMaterial.creator = this._container.get_user();
		newMaterial.lastUploadTag = null;
		newMaterial.createDateTime = null;
		newMaterial.modifyTime = this._getModifyTime() * 1;
		newMaterial.wfProcessID = this._container.get_wfProcessID();
		newMaterial.wfActivityID = this._container.get_wfActivityID();
		newMaterial.wfActivityName = this._container.get_wfActivityName();
		newMaterial.parentID = null;
		newMaterial.sourceMaterial = null;
		newMaterial.versionType = $HBRootNS.materialVersionType.Normal;
		newMaterial.extraData = {};
		newMaterial.fileIconPath = (this._fileIconPath == "" ? this._container.get_defaultFileIconPath() : this._fileIconPath);
		newMaterial.showFileUrl = "";
		return newMaterial;
	},

	_upload: function () {
		if (this.get_canEditContent() == false)
			return true;

		//        if ($HBRootNS.fileIO.fileExists(this._localPath) == false)
		//            return true;

		var processPageUrl = this._container.get_currentPageUrl()
			+ "?requestType=upload"
			+ "&lockID=" + this._container.get_lockID()
			+ "&userID=" + this._container.get_user().id
			+ "&rootPathName=" + this._container.get_rootPathName();

		var filesToUpload = new Array();
		Array.add(filesToUpload, this.generateSampleMaterial());

		var arg = "dialogHeight : 130px; dialogWidth : 300px; edge : Raised; center : Yes; help : No; resizable : No; status : No; scroll : no";
		var information = { filesToUpload: filesToUpload, container: this._container, requestContext: this._container.get_requestContext() };

		var filesUploaded = window.showModalDialog(this._container.get_dialogUploadFileProcessControlUrl(), information, arg);

		if (filesUploaded && filesUploaded.length != 0) {
			this.operateAfterUpload(filesUploaded[0]);
		}
	},

	generateSampleMaterial: function () {
		var sampleMaterial = new Object();
		sampleMaterial.filePath = this._localPath;
		sampleMaterial.container = this._container;
		sampleMaterial.materialID = this._materialID;
		return sampleMaterial;
	},

	operateAfterUpload: function (fileInformation) {
		this._uploadTime = new Date();

		//this.setLocalUploadTagFromServer();

		var data = new Object();
		data.lastUploadFileTime = this._getModifyTime().format("yyyy-MM-dd HH:mm:ss.fff");
		data.uploadTag = fileInformation.lastUploadTag;
		this.persistLocalData(data);
		this.loadLocalData();


		//todo,why?
		//this._localPath = this._localPath.replace(this._materialID, fileInformation.newMaterialID);

		if (fileInformation.fileIconPath != "")
			this._fileIconPath = fileInformation.fileIconPath;
		else
			this._fileIconPath = this._container.get_defaultFileIconPath();

		this._container.raiseMaterialsChanged();
		this.refreshStatus();
	},

	setLocalUploadTagFromServer: function () {
		var xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");

		var processPageUrl = String.format("{0}?requestType=getlastuploadtag&materialID={1}&requestContext={2}&t={3}",
				this._container.get_currentPageUrl(),
				escape(this._materialID),
				escape(this._container.get_requestContext()),
                Date.parse(new Date()));

		try {
			xmlhttp.open("GET", processPageUrl, false);
			xmlhttp.send();

			if (xmlhttp.status == 200) {
				var tag = xmlhttp.responseText;

				var data = new Object();
				data.uploadTag = tag;
				this.persistLocalData(data);
				this._localLastUploadTag = tag;

			}
			else {
				var date = new Date();
				var data = new Object();
				data.uploadTag = date.format("yyyy-MM-dd HH:mm:ss.fff");
				this.persistLocalData(data);
				this._localLastUploadTag = tag;
			}

		}
		catch (e) {
			var date = new Date();
			var data = new Object();
			data.uploadTag = date.format("yyyy-MM-dd HH:mm:ss.fff");
			this.persistLocalData(data);
			this._localLastUploadTag = tag;
		}
	},

	//    OnWebRequestCompleted: function () {
	//        var xmlhttp = $HBRootNS.material.xmlhttp;
	//        if (xmlhttp.readyState == 4) {
	//            if (xmlhttp.status == 200) {
	//            }
	//            else {
	//                alert("Problem retrieving XML data");
	//            }
	//        }
	//    },

	_showVersionClick: function () {
		var arg = "dialogHeight : 500px; dialogWidth : 450px; edge : Raised; center : Yes; help : No; resizable : No; status : No";

		var information = { rootPathName: this._container.get_rootPathName() };

		//		information.processPageUrl = this._container.get_currentPageUrl();

		var url = String.format("{0}&materialID={1}&rootPathName={2}&controlID={3}&userID={4}",
			this._container.get_dialogVersionControlUrl(),
			this._materialID,
			this._container.get_rootPathName(),
			this._container.get_uniqueID(),
			this._container.get_user().id);

		window.showModalDialog(url, information, arg);
	},

	refreshStatus: function () {
		if (this.get_canEditContent() == false)
			return;

		if (this._link != null)
			this._link.innerText = this._getShowTitle();

		if (this._fileIconPath != null && this._fileIcon.firstChild != null
			&& this._getResourceRelativePath(this._fileIcon.firstChild.src) != this._fileIconPath) {
			this._fileIcon.firstChild.src = this._fileIconPath;
		}

		if (this._operateImage == null)
			return;

		//if (this.get_uploaded() == false && this.get_contentModified() == true) 
		if (this.get_uploaded() == false && this.get_contentModified() == true) {
			if (this._getResourceRelativePath(this._operateImage.src) != this._container.get_uploadImagePath())
				this._operateImage.src = this._container.get_uploadImagePath();

			$clearHandlers(this._operateImage);
			$addHandlers(this._operateImage, this._uploadEvents);

			this._operateImage.style.cursor = "pointer";
			this._operateImage.title = "上传";
		}
		else {
			if (this._getResourceRelativePath(this._operateImage.src) != this._container.get_editImagePath())
				this._operateImage.src = this._container.get_editImagePath();

			$clearHandlers(this._operateImage);

			this._operateImage.style.cursor = "default";
			this._operateImage.title = "可编辑";
		}
	},

	_getResourceRelativePath: function (resourceFullPath) {
		var index = resourceFullPath.indexOf("/WebResource.axd?");

		if (index == -1)
			return resourceFullPath;
		else
			return resourceFullPath.substring(index);
	},

	//    get_uploaded: function () {
	//        if (this.get_canEditContent() == false)
	//            return true;

	//        if (this._isDraft) {
	//            if ($HBRootNS.fileIO.fileExists(this._localPath) == false)
	//                return true;

	//            if (this._uploadTime == null)
	//                return false;
	//            else
	//                return (this._getModifyTime() * 1 <= this._uploadTime * 1);
	//        }
	//        else if (this._uploadTime == null) {
	//            if (this._localMaterial == null)
	//                return true;

	//            if (this._lastUploadTag == this._localMaterial.lastUploadTag)
	//                return (this._modifyTime * 1 > this._getModifyTime() * 1);
	//            else
	//                return (this._lastUploadTag > this._localMaterial.lastUploadTag);
	//        }
	//        else {
	//            //如果曾经上传过，则比较最后上传时间和最后修改时间
	//            return (this._getModifyTime() * 1 <= this._uploadTime * 1);
	//        }
	//    },
	get_uploaded: function () {
		if (this.get_canEditContent() == false)
			return true;

		if (this._isDraft) {
			if (this._lastUploadFileTime == "") {
				return false;
			}
			var modifyTime = this._getModifyTime();
			var modifyTimeFormat = modifyTime == null ? "" : modifyTime.format("yyyy-MM-dd HH:mm:ss.fff");
			if (modifyTimeFormat > this._lastUploadFileTime) {
				return false;
			}
			else {
				return true;
			}
		}

		if (this._localLastUploadTag != null) {
			if (this._lastUploadTag == this._localLastUploadTag) {
				var modifyTime = this._getModifyTime();
				var modifyTimeFormat = modifyTime == null ? "" : modifyTime.format("yyyy-MM-dd HH:mm:ss.fff");
				if (modifyTimeFormat > this._lastUploadFileTime) {
					return false;
				}
				else {
					return true;
				}
			}
			else {
				var modifyTime = this._getModifyTime();
				var modifyTimeFormat = modifyTime == null ? "" : modifyTime.format("yyyy-MM-dd HH:mm:ss.fff");
				if (modifyTimeFormat > this._lastUploadFileTime) {
					return false;
				}
				else {
					return true;
				}
			}
			return false;
		}
		else {
			return true;
		}

		//        if (this.get_canEditContent() == false)
		//            return true;

		//        if (this._isDraft) {
		//            return (this._getModifyTime() * 1 <= this._modifyTime * 1); //!this._localIsModified;
		//        }
		//        else if (this._localLastUploadTag != null) {
		//            if (this._lastUploadTag <= this._localLastUploadTag)//上传过，判断是否修改
		//                return (this._getModifyTime() * 1 <= this._modifyTime * 1); //!this._localIsModified;
		//            else
		//                return false;
		//        }
		//        else {
		//            return (this._getModifyTime() * 1 <= this._modifyTime * 1); // !this._localIsModified;
		//        }
	},

	get_fileExists: function () {
		return $HBRootNS.fileIO.fileExists(this._localPath);
	},

	get_contentModified: function () {//这是管save的
		if (this.get_canEditContent() == false)
			return false;

		if (this._container.get_saveOriginalDraft() && this._isDraft)
			return true;

		if (this._downloaded) {
			return (this._getModifyTime() * 1 > this._downloadTime * 1);
		}
		//        else {
		//            if (this._lastUploadTag == this._localMaterial.lastUploadTag)
		//                return (this._getModifyTime() * 1 > this._localMaterial.modifyTime * 1);
		//            else
		//                return (this._lastUploadTag < this._localMaterial.lastUploadTag);

		var modifyTime = this._getModifyTime();
		var modifyTimeFormat = modifyTime == null ? "" : modifyTime.format("yyyy-MM-dd HH:mm:ss.fff");
		if (modifyTimeFormat > this._lastSaveFileTime) {
			return true;
		}
		else {
			return false;
		}

		//        if (this._lastUploadTag == this._localLastUploadTag) { //第一次下载，未上传过
		//            var modifyTime = this._getModifyTime();
		//            return modifyTime * 1 > this._downloadTime * 1;
		//        }
		//        else {
		//            var modifyTime = this._getModifyTime();
		//            var modifyTimeFormat = modifyTime == null ? "" : modifyTime.format("yyyy-MM-dd HH:mm:ss.fff");
		//            if (modifyTimeFormat > this._lastUploadFileTime) {
		//                return true;
		//            }
		//            else {
		//                return false;
		//            }
		//        }

		//        if (this._lastUploadTag <= this._localLastUploadTag)//上传了,看本地上传时间跟最后上传时间
		//        {
		//            if (this._lastUploadFileTime != "") {
		//                var modifyTime = this._getModifyTime();
		//                var modifyTimeFormat = modifyTime == null ? "" : modifyTime.format("yyyy-MM-dd HH:mm:ss.fff");
		//                if (modifyTimeFormat > this._lastUploadFileTime) {
		//                    return true;
		//                }
		//                else {
		//                    return false;
		//                }
		//            }
		//            else {
		//                return (this._getModifyTime() * 1 > this._modifyTime * 1);
		//            }
		//            //return (this._getModifyTime() * 1 > this._modifyTime * 1); //this._localIsModified;
		//        }
		//        else
		//            return (this._getModifyTime() * 1 > this._modifyTime * 1);
		//        //return true;
		//        //        }
	},

	getResultMaterial: function () {
		if (this._localMaterial == null)
			this._localMaterial = this.generateMaterial();
		else {
			this._localMaterial.title = this._title;
			this._localMaterial.pageQuantity = this._pageQuantity;
			this._localMaterial.sortID = this._sortID;
		}

		if (this._localLastUploadTag != null)
			this._localMaterial.lastUploadTag = this._localLastUploadTag;

		this._localMaterial.modifyTime = this._getModifyTime();

		//this._setMaterialInformation();

		return this._localMaterial;
	},

	//    _setMaterialInformation: function () {
	//        var fileExt = $HBRootNS.fileIO.getFileExt(this._localPath);
	//        var recordFilePath = this._localPath.replace(fileExt, "txt");

	//        if ($HBRootNS.fileIO.fileExists(this._localPath) == true)
	//            $HBRootNS.fileIO.saveFileContent(recordFilePath, escape(Sys.Serialization.JavaScriptSerializer.serialize(this._localMaterial)));
	//    },

	//    _getMaterialInformation: function () {
	//        var objMaterial = null;
	//        var fileExt = $HBRootNS.fileIO.getFileExt(this._localPath);
	//        var recordFilePath = this._localPath.replace(fileExt, "txt");

	//        if ($HBRootNS.fileIO.fileExists(recordFilePath) == false)
	//            return objMaterial;

	//        var fileContent = unescape($HBRootNS.fileIO.readFileContent(recordFilePath));

	//        if (fileContent == null)
	//            return objMaterial;

	//        this._localMaterial = Sys.Serialization.JavaScriptSerializer.deserialize(fileContent);
	//    },

	_getModifyTime: function () {
		var modifyTime = null;
		if (this._realLocalPath) {
			modifyTime = $HBRootNS.fileIO.getFileLastModifiedTime(this._realLocalPath);
		}
		return modifyTime == null ? this._modifyTime : modifyTime;
		//else return this._modifyTime;
	},
	//    _getModifyTime: function () {
	//        if (this._localModifyTime) {
	//            return this._localModifyTime;
	//        }
	//        else return this._modifyTime;
	//    },

	get_materialID: function () {
		return this._materialID;
	},
	set_materialID: function (value) {
		if (this._materialID != value) {
			this._materialID = value;
			this.raisePropertyChanged("materialID");
		}
	},

	get_department: function () {
		return this._department;
	},
	set_department: function (value) {
		if (this._department != value) {
			this._department = value;
			this.raisePropertyChanged("department");
		}
	},

	get_resourceID: function () {
		return this._resourceID;
	},
	set_resourceID: function (value) {
		if (this._resourceID != value) {
			this._resourceID = value;
			this.raisePropertyChanged("resourceID");
		}
	},

	get_sortID: function () {
		return this._sortID;
	},
	set_sortID: function (value) {
		if (this._sortID != value) {
			this._sortID = value;
			this.raisePropertyChanged("sortID");
		}
	},

	get_materialClass: function () {
		return this._materialClass;
	},
	set_materialClass: function (value) {
		if (this._materialClass != value) {
			this._materialClass = value;
			this.raisePropertyChanged("materialClass");
		}
	},

	get_title: function () {
		return this._title;
	},
	set_title: function (value) {
		if (this._title != value) {
			this._title = value;
			this.raisePropertyChanged("title");
		}
	},

	get_pageQuantity: function () {
		return this._pageQuantity;
	},
	set_pageQuantity: function (value) {
		if (this._pageQuantity != value) {
			this._pageQuantity = value;
			this.raisePropertyChanged("pageQuantity");
		}
	},

	get_relativeFilePath: function () {
		return this._relativeFilePath;
	},
	set_relativeFilePath: function (value) {
		if (this._relativeFilePath != value) {
			this._relativeFilePath = value;
			this.raisePropertyChanged("relativeFilePath");
		}
	},

	get_originalName: function () {
		return this._originalName;
	},
	set_originalName: function (value) {
		if (this._originalName != value) {
			this._originalName = value;
			this.raisePropertyChanged("originalName");
		}
	},

	get_creator: function () {
		return this._creator;
	},
	set_creator: function (value) {
		if (this._creator != value) {
			this._creator = value;
			this.raisePropertyChanged("creator");
		}
	},

	get_lastUploadTag: function () {
		return this._lastUploadTag;
	},
	set_lastUploadTag: function (value) {
		if (this._lastUploadTag != value) {
			this._lastUploadTag = value;
			this.raisePropertyChanged("lastUploadTag");
		}
	},

	get_createDateTime: function () {
		return this._createDateTime;
	},
	set_createDateTime: function (value) {
		if (this._createDateTime != value) {
			this._createDateTime = value;
			this.raisePropertyChanged("createDateTime");
		}
	},

	get_modifyTime: function () {
		return this._modifyTime;
	},
	set_modifyTime: function (value) {
		if (this._modifyTime != value) {
			this._modifyTime = value;
			this.raisePropertyChanged("modifyTime");
		}
	},

	get_wfProcessID: function () {
		return this._wfProcessID;
	},
	set_wfProcessID: function (value) {
		if (this._wfProcessID != value) {
			this._wfProcessID = value;
			this.raisePropertyChanged("wfProcessID");
		}
	},

	get_wfActivityID: function () {
		return this._wfActivityID;
	},
	set_wfActivityID: function (value) {
		if (this._wfActivityID != value) {
			this._wfActivityID = value;
			this.raisePropertyChanged("wfActivityID");
		}
	},

	get_wfActivityName: function () {
		return this._wfActivityName;
	},
	set_wfActivityName: function (value) {
		if (this._wfActivityName != value) {
			this._wfActivityName = value;
			this.raisePropertyChanged("wfActivityName");
		}
	},

	get_parentID: function () {
		return this._parentID;
	},
	set_parentID: function (value) {
		if (this._parentID != value) {
			this._parentID = value;
			this.raisePropertyChanged("parentID");
		}
	},

	get_sourceMaterial: function () {
		return this._sourceMaterial;
	},
	set_sourceMaterial: function (value) {
		if (this._sourceMaterial != value) {
			this._sourceMaterial = value;
			this.raisePropertyChanged("sourceMaterial");
		}
	},

	get_versionType: function () {
		return this._versionType;
	},
	set_versionType: function (value) {
		if (this._versionType != value) {
			this._versionType = value;
			this.raisePropertyChanged("versionType");
		}
	},

	get_extraData: function () {
		return this._extraData;
	},
	set_extraData: function (value) {
		if (this._extraData != value) {
			this._extraData = value;
			this.raisePropertyChanged("extraData");
		}
	},

	get_fileIconPath: function () {
		return this._fileIconPath;
	},
	set_fileIconPath: function (value) {
		if (this._fileIconPath != value) {
			this._fileIconPath = value;
			this.raisePropertyChanged("fileIconPath");
		}
	},

	get_showFileUrl: function () {
		return this._showFileUrl;
	},
	set_showFileUrl: function (value) {
		if (this._showFileUrl != value) {
			this._showFileUrl = value;
			this.raisePropertyChanged("_showFileUrl");
		}
	},

	isInServerTempFolder: function () {
		return this._showFileUrl.indexOf("Temp\\") == 0;
	},

	get_isDraft: function () {
		return this._isDraft;
	},
	set_isDraft: function (value) {
		if (this._isDraft != value) {
			this._isDraft = value;
			this.raisePropertyChanged("isDraft");
		}
	},

	set_fileIconPath: function (value) {
		this._fileIconPath = value;
	},

	get_localPath: function () {
		return this._localPath;
	},

	get_container: function () {
		return this._container;
	},

	set_container: function (value) {
		if (this._container != value) {
			this._container = value;
			this.raisePropertyChanged("container");
		}
	},

	get_modifyCheck: function () {
		return this._container.get_modifyCheck();
	},

	get_isOfficeDocument: function () {
		return this._isOfficeDocument;
	}
}

$HBRootNS.material.registerClass($HBRootNSName + ".material", $HGRootNS.ControlBase);

$HBRootNS.material.createMaterial = function (properties) {
	var span = $HGDomElement.get_currentDocument().createElement("span");

	var material = $create($HBRootNS.material, properties, null, null, span);

	return material;
}

$HBRootNS.material.newMaterialID = "newMaterialID"; //新起草的文件使用的临时ID
$HBRootNS.material.downloadFrameName = "_materialFrame"; //下载文件使用的frame的ID
$HBRootNS.material.allMaterials = new Array(); 	//页面上所有material对象的集合
$HBRootNS.material.proecssedUpload = false; 		//标记是否已经处理提交前的数据和文件
$HBRootNS.material.checkedOpened = false; 		//标记是否已经检查过文件是否有打开的
$HBRootNS.material.openInlineFileExt = ""; 		//在IE中打开的文件拓展名
$HBRootNS.material.allowUnlocks = true; 			//是否允许解锁
$HBRootNS.material.downloadFileInFrame = false;     //是否点击了链接，引起了要在IFRAME中下载文件的请求.点击那些要在IFRAME中下载文件的链接时置为TRUE

//刷新所有material对象
$HBRootNS.material.refreshAllStatus = function () {
	for (var i = 0; i < $HBRootNS.material.allMaterials.length; i++)
		$HBRootNS.material.allMaterials[i].refreshStatus();
}

//上传allMaterials
$HBRootNS.material.allMaterialsUpload = function () {
	var filesToUpload = new Array();
	for (var i = 0; i < $HBRootNS.material.allMaterials.length; i++) {
		if ($HBRootNS.material.allMaterials[i].get_contentModified() == true && $HBRootNS.material.allMaterials[i].get_uploaded() == false)
			Array.add(filesToUpload, $HBRootNS.material.allMaterials[i].generateSampleMaterial());
	}

	if (filesToUpload.length != 0) {
		var arg = "dialogHeight : 120px; dialogWidth : 300px; edge : Raised; center : Yes; help : No; resizable : No; status : No";
		var information = { filesToUpload: filesToUpload, container: filesToUpload[0].container };

		var filesUploaded = window.showModalDialog(filesToUpload[0].container.get_dialogUploadFileProcessControlUrl(), information, arg);
		if (filesUploaded) {
			for (var i = 0; i < filesUploaded.length; i++) {
				var material = $HBRootNS.material._findInAllMaterials(filesUploaded[i].filePath);
				if (material)
					material.operateAfterUpload(filesUploaded[i]);
			}
		}
	}

	$HBRootNS.material.proecssedUpload = true;

	return true;
}

$HBRootNS.material._findInAllMaterials = function (filePath) {
	var material = null;

	for (var i = 0; i < $HBRootNS.material.allMaterials.length; i++) {
		material = $HBRootNS.material.allMaterials[i];

		if (material.get_localPath() == filePath)
			return material;
		else
			material = null;
	}

	return material;
}

//检查是否有打开的文件
$HBRootNS.material.checkAllMaterialsOpend = function () {
	for (var i = 0; i < $HBRootNS.material.allMaterials.length; i++) {
		var container = $HBRootNS.material.allMaterials[i]._container;
		if (container && container._allowEdit && container._allowEditContent && container._editDocumentInCurrentPage == true &&
        container._materialUseMode == $HBRootNS.materialUseMode.SingleDraft) {
			var viewer = container._get_officeViewerWrapperViewer();
			viewer.SetAppFocus();
			var isDirty = viewer.IsDirty();

			if (isDirty == true) {
				return 2;
			}
		}
		else {
			if ($HBRootNS.fileIO.isFileOpened($HBRootNS.material.allMaterials[i]._realLocalPath) == true
            		    && $HBRootNS.material.allMaterials[i].get_container().get_autoCheck() == true)
				return 1;
			//continue;
		}
	}
	return 0;
}

//检查是否有未上传的文件
$HBRootNS.material.checkAllMaterialsUpLoaded = function () {
	if ($HBRootNS.material.downloadFileInFrame == true) {
		$HBRootNS.material.downloadFileInFrame = false;
		return;
	}

	if ($HBRootNS.material.checkedOpened == false && $HBRootNS.material.checkAllMaterialsOpend() == 1) {
		event.returnValue = $NT.getText("SOAWebControls", "您有正在打开的文档文件，当前的页面不能关闭，请您保存文件后再关闭页面！");
		$HBRootNS.material.allowUnlocks = false;
		return;
	}
	else {
		for (var i = 0; i < $HBRootNS.material.allMaterials.length; i++) {
			if ($HBRootNS.material.allMaterials[i].get_modifyCheck() == true && $HBRootNS.material.allMaterials[i].get_contentModified() == true
				&& $HBRootNS.material.allMaterials[i].get_uploaded() == false
				&& $HBRootNS.material.allMaterials[i].get_container().get_autoCheck() == true) {
				event.returnValue = $NT.getText("SOAWebControls", "您编辑了文件但没有保存当前文件，请您保存文件后再关闭页面！");
				$HBRootNS.material.checkedOpened = false;
				$HBRootNS.material.proecssedUpload = false;
				$HBRootNS.material.allowUnlocks = false;
				return;
			}
		}
	}

	$HBRootNS.material.allowUnlocks = true;
}

$HBRootNS.material.initDownloadFileFrame = function () {
	var frame = $get($HBRootNS.material.downloadFrameName);

	if (frame == null) {
		frame = window.document.createElement("iframe");
		frame.name = $HBRootNS.material.downloadFrameName;
		frame.id = $HBRootNS.material.downloadFrameName;
		frame.style.display = "none";
		window.document.body.appendChild(frame);
	}
}

$HBRootNS.material.checkDownloadFileInline = function (fileName) {
	var link = event.srcElement;

	if (link.href.toLowerCase().indexOf("http:") < 0) {
		var fileExts = $HBRootNS.material.openInlineFileExt.toLowerCase();
		var fileExt = $HBRootNS.fileIO.getFileExt(fileName.toLowerCase());

		var result = false;

		var fileExts1 = fileExts.split(';');

		if ($HBRootNS.material.findInFileExts(fileExts1, fileExt) == true)
			result = true;

		var fileExts2 = fileExts.split(',');

		if ($HBRootNS.material.findInFileExts(fileExts2, fileExt) == true)
			result = true;

		if (link.tagName.toLowerCase() == "img")
			link = link.parentNode;

		if (result == true) {
			link.target = '_blank';
		}
		else {
			link.target = $HBRootNS.material.downloadFrameName;
			$HBRootNS.material.downloadFileInFrame = true;
		} 
	}
}

$HBRootNS.material.findInFileExts = function (fileExts, fileExt) {
	if (fileExts.length) {
		for (var i = 0; i < fileExts.length; i++) {
			if (fileExt == fileExts[i])
				return true;
		}
	}

	return false;
}

$HBRootNS.IEPersistence = function (element) {
	throw Error.invalidOperation();
};

$HBRootNS.IEPersistence.registerClass($HBRootNSName + ".IEPersistence");

$HBRootNS.IEPersistence.getclientPersisterID = function () {
	var rawID = $HBRootNSName + "_IEPersistence_Obj";
	return rawID.replace(/\./g, '_');
};

$HBRootNS.IEPersistence.getPersistObject = function () {
	var id = $HBRootNS.IEPersistence.getclientPersisterID();
	var oPersist = document.all(id);

	if (oPersist == null) {
		oPersist = document.createElement("<div style='behavior:url(#default#userdata)' id='" + id + "'></div>");
		document.body.insertBefore(oPersist);
	}

	return oPersist;
};

$HBRootNS.IEPersistence.UserDataKey = "IEUserDataForMaterial";

$HBRootNS.IEPersistence.saveData = function (key, value) {
	if (key == $HBRootNS.material.newMaterialID) {
		return;
	}
	var key = "K" + key;
	var oPersist = $HBRootNS.IEPersistence.getPersistObject();
	if (oPersist) {
		var oTimeNow = new Date();
		oTimeNow.setDate(oTimeNow.getDate() + 20); //根据存储数据大小判断，大概可以存1000多条，20天内本地保留更改的文件应该不会超过这个数目。
		var sExpirationDate = oTimeNow.toUTCString();
		oPersist.expires = sExpirationDate;

		var valueToPersist = value == null ? null : Sys.Serialization.JavaScriptSerializer.serialize(value);
		oPersist.setAttribute(key, valueToPersist);
		oPersist.save($HBRootNS.IEPersistence.UserDataKey);
	}
};

$HBRootNS.IEPersistence.loadData = function (key) {
	if (key == $HBRootNS.material.newMaterialID) {
		return null;
	}
	var key = "K" + key;
	var oPersist = $HBRootNS.IEPersistence.getPersistObject();
	if (oPersist) {
		oPersist.load($HBRootNS.IEPersistence.UserDataKey);
		var value = oPersist.getAttribute(key);
		return value == null ? null : Sys.Serialization.JavaScriptSerializer.deserialize(value);
	}
};

$HBRootNS.IEPersistence.removeData = function (key) {
	var key = "K" + key;
	var oPersist = $HBRootNS.IEPersistence.getPersistObject();
	if (oPersist) {
		oPersist.load($HBRootNS.IEPersistence.UserDataKey);
		oPersist.removeAttribute(key);
		oPersist.save($HBRootNS.IEPersistence.UserDataKey);
	}
};

$HBRootNS.MaterialPersistence = function (element) {
	throw Error.invalidOperation();
};

$HBRootNS.MaterialPersistence.registerClass($HBRootNSName + ".MaterialPersistence");

$HBRootNS.MaterialPersistence.PersistMode = "File";

$HBRootNS.MaterialPersistence.saveData = function (key, value, userName) {
	if ($HBRootNS.MaterialPersistence.PersistMode == "File") {
		var filePath = $HBRootNS.fileIO._getTempFilePath("\\file.txt", key, userName);
		$HBRootNS.fileIO.saveFileContent(filePath, Sys.Serialization.JavaScriptSerializer.serialize(value));
	}
	else {
		$HBRootNS.IEPersistence.saveData(key, value);
	}
};

$HBRootNS.MaterialPersistence.loadData = function (key, userName) {
	if ($HBRootNS.MaterialPersistence.PersistMode == "File") {
		var filePath = $HBRootNS.fileIO._getTempFilePath("\\file.txt", key, userName);
		var fileContent = $HBRootNS.fileIO.readFileContent(filePath);
		return fileContent == null ? null : Sys.Serialization.JavaScriptSerializer.deserialize(fileContent);
	}
	else {
		$HBRootNS.IEPersistence.loadData(key);
	}
};

$HBRootNS.MaterialPersistence.removeData = function (key, userName) {
	if ($HBRootNS.MaterialPersistence.PersistMode == "File") {
		var filePath = $HBRootNS.fileIO._getTempFilePath("\\file.txt", key, userName);
		$HBRootNS.fileIO.deleteFile(filePath);
	}
	else {
		$HBRootNS.IEPersistence.removeData(key);
	}
};
