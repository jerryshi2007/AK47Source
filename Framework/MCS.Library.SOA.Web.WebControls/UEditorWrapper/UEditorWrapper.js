
$HBRootNS.UEditorWrapper = function (element) {
	$HBRootNS.UEditorWrapper.initializeBase(this, [element]);

	this._editor = null;
	this._postedDataFormName = null;
	this._editorContainerClientID = null;
	this._editorOptions = null;
	this._fileSelectMode = null;
	this._dialogUploadFileProcessControlUrl = null;
	this._currentPageUrl = null; 				//页面地址
	this._controlID = null;
	this._rootPathName = null; 					//服务器上保存文件的根路径的配置节点名称
	this._relativePath = null; 					//服务器上保存文件的目录(相对路径)
	this._showImageHandlerUrl = null;
	this._lockID = null;

	this._imageNodes = null;
	this._uploadedImages = [];
	this._newUploadedImages = [];
	this._docProperty = null;

	this._autoDownloadUploadImages = null;
	this._autoUploadImages = null;
	this._readOnly = null;
}

$HBRootNS.UEditorWrapper._allEditors = [];

$HBRootNS.UEditorWrapper.prototype =
{
	initialize: function () {
		this._imageNodes = [];
		this._docProperty = {};
		$HBRootNS.UEditorWrapper.callBaseMethod(this, 'initialize');
	},

	manualUpload: function (arrUrl) {
		if (this._imageNodes.length > 0) {
			var message = String.format("您有{0}张本地图片没有转存，单击确定转存。", this._imageNodes.length)
			var continueToSave = window.confirm(message);
			if (!continueToSave)
				return;
			return this._innerUpload(arrUrl);

		}
	},

	_innerUpload: function (arrUrl) {
		var isSuccess = true;
		var arg = "dialogHeight : 120px; dialogWidth : 310px; edge : Raised; center : Yes; help : No; resizable : No; status : No; scroll : no";
		var filesToUpload = new Array();

		for (var i = 0; i < this._imageNodes.length; i++) {
			var innerImageNodes = this._imageNodes[i];
			var localPath = innerImageNodes.attributes.word_img;
			var fileToUpload = {};
			fileToUpload.filePath = localPath.replace("file:///", "");
			fileToUpload.container = this;
			Array.add(filesToUpload, fileToUpload);
		}
		try {
			this.uploadFiles(filesToUpload);
		} catch (e) {
			alert(str.format("上传时发生错误: {0}", e.message));
		}
	},

	_innerDownload: function (fileToDownload) {
		try {
			for (var i = 0; i < fileToDownload.length; i++) {
				$HBRootNS.fileIO.downloadFile(fileToDownload[i].requestUrl, fileToDownload[i].localPath, "GET");
			}
			return true;
		} catch (e) {
			alert("下载时发生了错误:" + e.Message);
			return false;
		}
	},

	dispose: function () {
		Array.remove($HBRootNS.UEditorWrapper._allEditors, this);
		$HBRootNS.UEditorWrapper.callBaseMethod(this, 'dispose');
	},

	loadClientState: function (value) {
		try {

		}
		catch (e) {
			this.get_element().innerText = e.message;
		}
		var options = this.get_editorOptions();
		if (value) {
			if (value != "") {
				this._docProperty = Sys.Serialization.JavaScriptSerializer.deserialize(value);
			}
		}
		var args = $HBRootNS.UEditorWrapper.GetArgs();
		if (options) {
			this._checkUEditorObject();
			if (location.search.indexOf("DialogUploadFileProcessControl") < 0) {
				this._editor = this._renderEditor(options);
				if (this._readOnly == true) {
					this._editor.document.body.disabled = true;
					this._editor.document.body.contentEditable = false;
					this._editor.document.body.disabled = false;
				}
			}
			Array.add($HBRootNS.UEditorWrapper._allEditors, this);
		}
	},

	saveClientState: function () {
	},

	get_postedDataFormName: function () {
		return this._postedDataFormName;
	},

	set_postedDataFormName: function (value) {
		this._postedDataFormName = value;
	},

	get_editorContainerClientID: function () {
		return this._editorContainerClientID;
	},

	set_editorContainerClientID: function (value) {
		this._editorContainerClientID = value;
	},

	get_editorOptions: function () {
		return this._editorOptions;
	},

	set_editorOptions: function (value) {
		this._editorOptions = value;
	},

	get_editor: function () {
		return this._editor;
	},

	get_fileSelectMode: function () {
		return this._fileSelectMode;
	},

	set_fileSelectMode: function (value) {
		if (this._fileSelectMode != value) {
			this._fileSelectMode = value;
			this.raisePropertyChanged("fileSelectMode");
		}
	},

	get_dialogUploadFileProcessControlUrl: function () {
		return this._dialogUploadFileProcessControlUrl;
	},

	set_dialogUploadFileProcessControlUrl: function (value) {
		this._dialogUploadFileProcessControlUrl = value;
	},

	get_currentPageUrl: function () {
		return this._currentPageUrl;
	},

	set_currentPageUrl: function (value) {
		if (this._currentPageUrl != value) {
			this._currentPageUrl = value;
		}
	},

	get_uniqueID: function () {
		return this._controlID;
	},

	get_controlID: function () {
		return this._controlID;
	},

	set_controlID: function (value) {
		this._controlID = value;
	},

	get_rootPathName: function () {
		return this._rootPathName;
	},

	set_rootPathName: function myfunction(value) {
		this._rootPathName = value;
	},

	get_relativePath: function () {
		return this._relativePath;
	},

	set_relativePath: function (value) {
		this._relativePath = value;
	},

	get_showImageHandlerUrl: function () {
		return this._showImageHandlerUrl;
	},

	set_showImageHandlerUrl: function (value) {
		this._showImageHandlerUrl = value;
	},

	get_lockID: function () {
		return this._lockID;
	},

	set_lockID: function (value) {
		this._lockID = value;
	},

	get_user: function () {
		return "";
	},

	get_autoDownloadUploadImages: function () {
		return this._autoDownloadUploadImages;
	},

	set_autoDownloadUploadImages: function (value) {
		this._autoDownloadUploadImages = value;
	},

	get_autoUploadImages: function () {
		return this._autoUploadImages;
	},

	set_autoUploadImages: function (value) {
		this._autoUploadImages = value;
	},

	get_readOnly: function () {
		return this._readOnly;
	},
	set_readOnly: function (value) {
		this._readOnly = value;
	},

	collectContent: function () {
		if (this._editor && this._editor.hasContents()) {
			var postedDataElem = $get(this.get_postedDataFormName());

			if (postedDataElem) {
				this._editor.sync();
				this._docProperty.InitialData = encodeURIComponent(this._editor.getContent());
				if (!this._docProperty.DocumentImages) {
					this._docProperty.DocumentImages = [];
				}
				if (!this._docProperty.DocImageProps) {
					this._docProperty.DocImageProps = [];
				}

				for (var i = 0; i < this._uploadedImages.length; i++) {
					var imgProp = {};
					imgProp.ID = this._uploadedImages[i].newMaterialID;
					imgProp.NewName = this._uploadedImages[i].newMaterialID + "." + $HBRootNS.fileIO.getFileExt(this._uploadedImages[i].filePath);
					imgProp.Changed = true;
					this._docProperty.DocumentImages.push(imgProp);
					this._docProperty.DocImageProps.push(imgProp);
				}
				//this._docProperty.DocumentImages = [];
				postedDataElem.value = Sys.Serialization.JavaScriptSerializer.serialize(this._docProperty); //encodeURI(this._editor.getContent());
			}
		}
	},

	_renderEditor: function (options) {
		var editor = new baidu.editor.ui.Editor(options);
		editor.render(this.get_editorContainerClientID());
		editor.Wrapper = this;
		return editor;
	},

	_checkUEditorObject: function () {
		if (typeof (baidu) == "undefined" ||
			typeof (baidu.editor) == "undefined" ||
			typeof (baidu.editor.ui) == "undefined" ||
			typeof (baidu.editor.ui.Editor) == "undefined") {
			throw Error.create("没有正确地初始化UEditor的相关脚本");
		}
	},

	//    generateImageUrl: function (imageInfo) {
	//        var processPageUrl = this.get_currentPageUrl()
	//		+ "?requestType=showImage"
	//		+ "&uploadedImageName=" + imageInfo.newMaterialID + "." + $HBRootNS.fileIO.getFileExt(imageInfo.filePath)
	//		+ "&controlID=" + this.get_controlID()
	//        + "&rootPath=" + this.get_rootPathName();
	//        return processPageUrl;
	//    },

	generateImageUrl: function (imageInfo) {
		var processPageUrl = this.get_showImageHandlerUrl()
        + "?id=" + imageInfo.newMaterialID
        + "&rootPath=" + this.get_rootPathName()
		+ "&imageName=" + imageInfo.newMaterialID + "." + $HBRootNS.fileIO.getFileExt(imageInfo.filePath);
		return processPageUrl;
	},

	_generateUploadedImageInfo: function (arrUploaded) {
		var arrUrl = [];
		if (arrUploaded.length > 0) {
			for (var i = 0; i < arrUploaded.length; i++) {
				var url = this.generateImageUrl(arrUploaded[i])
				this._uploadedImages[i].uploadedUrl = url;
				this._uploadedImages[i].displayBackSetted = true;
			}
		}
	},

	_getNeedShowImageNode: function () {
		var result = [];
		var images = domUtils.getElementsByTagName(this._editor.document, "img");

		for (var j = 0; j < images.length; j++) {
			var imageNode = images[j];
			if (!imageNode.setted) {
				Array.add(result, imageNode);
			}
		}
		return result;
	},
	_showUploadedImage: function myfunction() {
		var needShowImageNode = this._getNeedShowImageNode();
		for (var j = 0; j < needShowImageNode.length; j++) {
			var UEditorNode = this._imageNodes[j];
			var domImgNode = needShowImageNode[j];
			var wordImg = domImgNode.word_img ? domImgNode.word_img.replace("file:///", "") : "";
			for (var i = 0; i < this._newUploadedImages.length; i++) {
				var upImg = this._newUploadedImages[i];
				if (upImg.filePath && upImg.filePath == wordImg) {
					domImgNode.src = upImg.uploadedUrl;
					domImgNode.data_ue_src = upImg.uploadedUrl;
					domImgNode.setted = true;

					Array.forEach(this._imageNodes, function (element, index, array) {
						if (element.attributes.word_img.replace("file:///", "") === wordImg) {
							Array.removeAt(array, index);
						}
					});
					break;
				}
				if (domImgNode.src == upImg.originalSrc) {
					domImgNode.src = upImg.uploadedUrl;
					domImgNode.data_ue_src = domImgNode.src;
					domImgNode.setted = true;
					break;
				}
			}
			this._uploadedImages.push(upImg);
		}
		//this._imageNodes.length = 0;
	},

	clientValidate: function (source, args) {
		args.IsValid = true;

	},

	getImagesToDownload: function () {
		var result = [];
		var images = domUtils.getElementsByTagName(this._editor.document, "img");
		for (var j = 0; j < images.length; j++) {
			var imageNode = images[j];
			var upImgInfo = {};
			if (!imageNode.word_img && typeof (imageNode.setted) == "undefined") {
				upImgInfo.requestUrl = imageNode.src;
				upImgInfo.localPath = this._getLocalPath(imageNode.src);
				Array.add(result, upImgInfo);
			}
		}
		return result;
	},

	check: function (source, args) {
		var localImgCheckResult = false;
		var remoteImgCheckResult = true;
		var checkResult = -1;
		var result = this.getImagesToDownload();
		if (result.length > 0) {
			if (this.get_autoDownloadUploadImages()) {
				if (confirm(String.format("有{0}张未下载的网络图片，你已启用自动下载并上传，单击确定下载并上传，或者点击取消手动下载并上传。", result.length))) {
					try {
						this.downLoadAndUpload(result, false);
					} catch (e) {
						alert("网络路径的图片下载并上传时发生错误: " + e.Message);
					} finally {
						remoteImgCheckResult = true;
						args.IsValid = true;
						checkResult = -1;
					}
				} else {
					remoteImgCheckResult = false;
					args.IsValid = false;
					checkResult = 1;
					return false;
				}
			}
			//            else {
			//                remoteImgCheckResult = false;
			//                args.IsValid = false;
			//                checkResult = 1;               
			//            }
		} else {
			args.IsValid = true;
			checkResult = -1;
		}

		if (this._imageNodes.length > 0) {
			if (this.get_autoUploadImages()) {
				if (confirm(String.format("有{0}张未上传的本地图片，你已启用自动上传，单击确定上传，或者点击取消手动上传。", this._imageNodes.length))) {
					var upResult;
					try {
						upResult = this._innerUpload();
					} catch (e) {
						upResult = false;
					}
					if (upResult == true) {
						checkResult = -1;
						localImgCheckResult = true;
						args.IsValid = false;
					} else {
						checkResult = 1;
						args.IsValid = true;
					}
				}
			} else {
				var message = String.format("您有{0}张本地图片没有转存，请上传后提交。", this._imageNodes.length);
				alert(message);
				args.IsValid = false;
				checkResult = 1;
				return false;
			}
		}
		//        else {
		//            localImgCheckResult = true;
		//            args.IsValid = true;
		//            checkResult = -1;
		//        }

		if (checkResult == -1) {
			this.collectContent();
			//            for (var i = 0; i < $HBRootNS.UEditorWrapper._allEditors.length; i++) {
			//                $HBRootNS.UEditorWrapper._allEditors[i].collectContent();
			//            }
			args.IsValid = true;
		}

		return true;
	},

	_getLocalPath: function (url) {
		var localPath = "";
		//这里目录去掉logOnName路径，是因为officeviewer控件没有创建目录的方法。
		localPath = $HBRootNS.fileIO.getTempDirName() + "\\" + $HBRootNS.UEditorWrapper.NewGuid() + ".";

		localPath += $HBRootNS.fileIO.getFileExt(url);

		return localPath;
	},

	downLoadImage: function (result, showPrompt) {
		var result = result || this.getImagesToDownload();
		var toBeUpload;
		if (result.length > 0) {
			var strDown = String.format("您有{0}张网络图片需要下载，是否下载并上传。", result.length)
			var isContinue;
			isContinue = showPrompt == true ? window.confirm(strDown) : true;
			if (isContinue) {
				var isSuccess = this._innerDownload(result);

				var filesToUpload = new Array();
				for (var i = 0; i < result.length; i++) {
					var innerImageNode = result[i];
					var localPath = innerImageNode.localPath;
					var fileToUpload = {};
					fileToUpload.filePath = localPath.replace("file:///", "");
					fileToUpload.container = this;
					fileToUpload.originalSrc = innerImageNode.requestUrl;
					Array.add(filesToUpload, fileToUpload);
				}
				toBeUpload = filesToUpload;
			}
		}
		return toBeUpload;
	},

	downLoadAndUpload: function (imgNodes, showPrompt) {
		var filesToUpload = this.downLoadImage(imgNodes, showPrompt);
		this.uploadFiles(filesToUpload);
	},

	uploadFiles: function (filesToUpload) {
		if (typeof (filesToUpload) != "undefined" && filesToUpload.length > 0) {
			var information = new Object();
			information.filesToUpload = filesToUpload;
			information.fileMaxSize = this._fileMaxSize;
			information.isNew = true;
			var arg = "dialogHeight : 120px; dialogWidth : 310px; edge : Raised; center : Yes; help : No; resizable : No; status : No; scroll : no";

			var filesUploaded = window.showModalDialog(this.get_dialogUploadFileProcessControlUrl(), information, arg);
			if (filesUploaded) {
				for (var j = 0; j < filesUploaded.length; j++) {
					filesUploaded[j].uploadedUrl = this.generateImageUrl(filesUploaded[j]);
					filesUploaded[j].toBeshow = true;
					for (var i = 0; i < filesToUpload.length; i++) {
						if (filesUploaded[j].filePath == filesToUpload[i].filePath) {
							filesUploaded[j].originalSrc = filesToUpload[i].originalSrc;
							break;
						}
					}
				}
				this._newUploadedImages = filesUploaded;
				this._showUploadedImage();
			}
		}
	},

	add_newUploadedImages: function (filesUploaded) {
		this._newUploadedImages = filesUploaded;
		this._showUploadedImage();
	},

	get_content: function () {
		return this._editor.getContent()
	},

	pseudo: function () {
	}
}

$HBRootNS.UEditorWrapper.DownLoad = function () {

}

$HBRootNS.UEditorWrapper.NewGuid = function () {
	var guid = "";
	for (var i = 1; i <= 32; i++) {
		var n = Math.floor(Math.random() * 16.0).toString(16);
		guid += n;
		if ((i == 8) || (i == 12) || (i == 16) || (i == 20))
			guid += "-";
	}
	return guid;
}

$HBRootNS.UEditorWrapper.GetArgs = function () {
	var args = {};
	var match = null;
	var search = location.search.substring(1);
	var reg = /(?:([^&amp;]+)=([^&amp;]+))/g;
	while ((match = reg.exec(search)) !== null) {
		args[match[1]] = match[2];
	}
	return args;
}

$HBRootNS.UEditorWrapper.registerClass($HBRootNSName + ".UEditorWrapper", $HBRootNS.DialogControlBase);