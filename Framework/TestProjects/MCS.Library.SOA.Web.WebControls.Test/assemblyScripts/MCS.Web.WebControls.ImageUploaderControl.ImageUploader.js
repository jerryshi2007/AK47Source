$HBRootNS.ImageUploader = function (element) {
	$HBRootNS.ImageUploader.initializeBase(this, [element]);
	this._inputFileId;
	this._imageId;
	this._imgUploader = null;
	this._image = null;
	this._imageWidth = 0;
	this._imageHeight = 0;
	this._imageProperty = {};

	this._defaultImg = "";
	this._serverUploadID = "";
	this._serverUpload = null;

	this._uploadButtonId = "";
	this._uploadButton = null;
	this._deleteButtonId = "";
	this._deleteButton = null;
	this._innerFrameID = "";
	this._innerFrameContainerID = "";

	this._fileMaxSize = 0;
	this._enabled = true;
}

$HBRootNS.ImageUploader.prototype = {
	initialize: function () {
		$HBRootNS.ImageUploader.callBaseMethod(this, 'initialize');

		this._imgUploader = $get(this.get_inputFileId());
		this._image = $get(this.get_imageId());
		this._serverUpload = $get(this.get_serverUploadID());
		this._uploadButton = $get(this.get_uploadButtonId());
		this._deleteButton = $get(this.get_deleteButtonId());
		this.uploadButton$delegate = {
			click: Function.createDelegate(this, this._uploadButtonElement_onClick)
		},
        $addHandlers(this._uploadButton, this.uploadButton$delegate);
		this.deleteButton$delegate = {
			click: Function.createDelegate(this, this._deleteButtonElement_onClick)
		},
        $addHandlers(this._deleteButton, this.deleteButton$delegate);

		this.image$delegate = {
			load: Function.createDelegate(this, this._imageElement_onLoad)
            , error: Function.createDelegate(this, this._imageElement_onError)
		}
		$addHandlers(this._image, this.image$delegate);
		this.uploader$delegate = {
			change: Function.createDelegate(this, this._uploadElement_onChange)
		}
		$addHandlers(this._imgUploader, this.uploader$delegate);

	},

	setImage: function (src, width, height) {
		var img = $get(this.get_imageId());

		img.src = src;

		img.width = width;
		img.height = height;
	},

	saveClientState: function () {
		return Sys.Serialization.JavaScriptSerializer.serialize(this._imageProperty);
	},

	loadClientState: function (value) {
		if (value) {
			if (value != "") {
				this._imageProperty = Sys.Serialization.JavaScriptSerializer.deserialize(value);
				if (this._imageProperty && this._imageProperty.Src != null)
					this.setImage(this._imageProperty);

			}
		}
	},

	_imageElement_onLoad: function () {
		var hiddenField = $get("clientImagePropertiesHidden");

		var img = event.srcElement;

		img.width = this.get_imageWidth();
		img.height = this.get_imageHeight();
		if (!this._imageProperty) {
			var imgPropJsonStr = hiddenField.value;
			this._imageProperty = Sys.Serialization.JavaScriptSerializer.deserialize(imgPropJsonStr);
		}
		this._imageProperty.Width = img.width;
		this._imageProperty.Height = img.height;
		this._imageProperty.OriginalName = img.nameProp;
		this._imageProperty.Size = img.fullSize;
		this._imageProperty.Src = img.src;

		hiddenField.value = Sys.Serialization.JavaScriptSerializer.serialize(this._imageProperty)
	},

	_uploadElement_onChange: function () {
		var img = $get(this.get_imageId());
		var hiddenField = $get("clientImagePropertiesHidden");

		img.src = event.srcElement.value;
		if (parseInt(img.fileSize) > this.get_fileMaxSize()) {
			this.setImage(this.get_defaultImg(), 102, 102);
			this._clearFileInput(event.srcElement);
			alert("选择图片大小超过限制,请重新选择");
			return;
		}
		this._imageProperty.Width = img.width;
		this._imageProperty.Height = img.height;
		this._imageProperty.OriginalName = img.nameProp;
		this._imageProperty.Size = img.fileSize;
		this._imageProperty.Src = img.src;

		hiddenField.value = Sys.Serialization.JavaScriptSerializer.serialize(this._imageProperty)

	},

	_imageElement_onError: function () {
		var img = $get(this.get_imageId());
		//        img.width = 400;
		//        img.height = 300;
		this._clearFileInput(this._imgUploader);
		img.src = this.get_defaultImg();
		img.width = 102;
		img.height = 102;

		//alert("请选择合法的图片文件。");
		return false;
	},

	_uploadButtonElement_onClick: function () {
		if (this._imgUploader && this._imgUploader.value == "") {
			alert("请选择图片！");
			return;
		}

		var form = this._findServerForm(this._imgUploader);

		var clientOPHidden = $get("clientOPHidden");

		if (form) {
			var oldTarget = form.target;
			var oldAction = form.action;
			var oldEncType = form.enctype;

			try {
				this.set_enabled(false);
				
				form.target = this.get_innerFrameID();
				clientOPHidden.value = "upload";

				//SubmitButtonIntance._setAllButtonsState(true);
				//ProgressBarInstance.show("正在上传");
				//form.submit();
				this._serverUpload.click();
			}
			finally {
				form.target = oldTarget;
				form.action = oldAction;
				form.enctype = oldEncType;
			}
		}
	},

	get_enabled: function () {
		return this._enabled;
	},

	set_enabled: function (enabled) {
		this._set_elementEnabled(this._imgUploader, enabled);
		this._set_elementEnabled(this._uploadButton, enabled);
		this._set_elementEnabled(this._deleteButton, enabled);
	},

	_set_elementEnabled: function (element, enabled) {
		if (element) {
			element.disabled = !enabled;
		}
	},

	_deleteButtonElement_onClick: function () {
		this._clearFileInput(this._imgUploader);
		this._image.src = this.get_defaultImg();
	},

	_clearFileInput: function (file) {
		var form = document.createElement('form');
		document.body.appendChild(form);
		//记住file在旧表单中的的位置
		var pos = file.nextSibling;
		form.appendChild(file);
		form.reset();
		pos.parentNode.insertBefore(file, pos);
		document.body.removeChild(form);
	},

	_findServerForm: function (elem) {
		var form = null;

		while (elem) {
			if (elem.tagName == "FORM") {
				form = elem;
				break;
			}

			elem = elem.parentElement;
		}

		return form;
	},

	uploadSuccess: function myfunction(str) {
		var imgProp = Sys.Serialization.JavaScriptSerializer.deserialize(str);
		this._imageProperty = imgProp;

		this.set_enabled(true);
		this._resetAllHiddenStates();

		SubmitButton.resetAllStates();
	},

	_resetAllHiddenStates: function () {
		$get("clientImagePropertiesHidden").value = "";
		$get("clientOPHidden").value = "";
	},

	uploadFail: function (message) {
		this._clearFileInput(this._imgUploader);
		this.setImage(this.get_defaultImg(), 102, 102);

		this.set_enabled(true);
		this._resetAllHiddenStates();

		SubmitButton.resetAllStates();

		$showError("上传失败！ " + message);
	},

	get_inputFileId: function () {
		return this._inputFileId;
	},
	set_inputFileId: function (value) {
		this._inputFileId = value;
	},

	get_imageId: function () {
		return this._imageId;
	},
	set_imageId: function (value) {
		this._imageId = value;
	},

	get_imageWidth: function () {
		return this._imageWidth;
	},
	set_imageWidth: function (value) {
		this._imageWidth = value;
	},

	get_imageHeight: function () {
		return this._imageHeight;
	},
	set_imageHeight: function (value) {
		this._imageHeight = value;
	},

	get_defaultImg: function () {
		return this._defaultImg;
	},
	set_defaultImg: function (value) {
		this._defaultImg = value;
	},

	get_serverUploadID: function () {
		return this._serverUploadID;
	},
	set_serverUploadID: function (value) {
		this._serverUploadID = value;
	},

	get_uploadButtonId: function () {
		return this._uploadButtonId;
	},
	set_uploadButtonId: function (value) {
		this._uploadButtonId = value;
	},

	get_deleteButtonId: function () {
		return this._deleteButtonId;
	},
	set_deleteButtonId: function (value) {
		this._deleteButtonId = value;
	},

	get_innerFrameID: function () {
		return this._innerFrameID;
	},
	set_innerFrameID: function (value) {
		this._innerFrameID = value;
	},

	get_innerFrameContainerID: function () {
		return this._innerFrameContainerID;
	},
	set_innerFrameContainerID: function (value) {
		this._innerFrameContainerID = value;
	},

	get_fileMaxSize: function () {
		return this._fileMaxSize;
	},
	set_fileMaxSize: function (value) {
		this._fileMaxSize = value;
	}
}

$HBRootNS.ImageUploader.registerClass($HBRootNSName + ".ImageUploader", $HBRootNS.ControlBase);uttonId: function () {
        return this._uploadButtonId;
    },
    set_uploadButtonId: function (value) {
        this._uploadButtonId = value;
    },
    get_deleteButtonId: function () {
        return this._deleteButtonId;
    },
    set_deleteButtonId: function (value) {
        this._deleteButtonId = value;
    },
    get_innerFrameId: function () {
        return this.innerFrameId;
    },
    set_innerFrameId: function (value) {
        this.innerFrameId = value;
    },
    get_loadingId: function () {
        return this._loadingId;
    },
    set_loadingId: function (value) {
        this._loadingId = value;
    },
    get_hiddenFlagId: function () {
        return this._hiddenFlagId;
    },
    set_hiddenFlagId: function (value) {
        this._hiddenFlagId = value;
    },
    get_preventSubmitButtonId: function () {
        return this._preventSubmitButtonId;
    },
    set_preventSubmitButtonId: function (value) {
        this._preventSubmitButtonId = value;
    },
    get_fileMaxSize: function () {
        return this._fileMaxSize;
    },
    set_fileMaxSize: function (value) {
        this._fileMaxSize = value;
    }
}



$HBRootNS.ImageUploader.registerClass($HBRootNSName + ".ImageUploader", $HBRootNS.ControlBase);