$HBRootNS.ImageUploader = function (element) {
    $HBRootNS.ImageUploader.initializeBase(this, [element]);
    this._inputFileID;
    this._imageID;
    this._imgUploader = null;
    this._image = null;
    this._imageWidth = 0;
    this._imageHeight = 0;
    this._imageProperty = {};
    this._currentPageUrl = null;

    this._defaultImg = "";

    this._uploadButtonId = "";
    this._uploadButton = null;
    this._deleteButtonID = "";
    this._deleteButton = null;
    this._innerFrameID = "";
    this._innerFrameContainerID = "";

    this._fileMaxSize = 0;
    this._enabled = true;
    this._controlID = "";
    this._filePathChanged = false;
    this._autoUpload = false;
}

$HBRootNS.ImageUploader.prototype = {
    initialize: function () {
        $HBRootNS.ImageUploader.callBaseMethod(this, 'initialize');

        this._imgUploader = $get(this.get_inputFileID());
        this._image = $get(this.get_imageID());

        this._uploadButton = $get(this.get_uploadButtonId());
        this._deleteButton = $get(this.get_deleteButtonID());

        //        this.uploadButton$delegate = {
        //            click: Function.createDelegate(this, this._uploadButtonElement_onClick)
        //        },
        //        $clearHandlers(this._uploadButton);
        //        $addHandlers(this._uploadButton, this.uploadButton$delegate);
        this._uploadButton.onclick = Function.createDelegate(this, this._uploadButtonElement_onClick);

        //        this.deleteButton$delegate = {
        //            click: Function.createDelegate(this, this._deleteButtonElement_onClick)
        //        },
        //        $clearHandlers(this._deleteButton);
        //        $addHandlers(this._deleteButton, this.deleteButton$delegate);
        this._deleteButton.onclick = Function.createDelegate(this, this._deleteButtonElement_onClick);
        //        this.image$delegate = {
        //            load: Function.createDelegate(this, this._imageElement_onLoad),
        //            error: Function.createDelegate(this, this._imageElement_onError),
        //            click: Function.createDelegate(this, this._imageElement_onClick)
        //        }
        //        $clearHandlers(this._image);
        //        $addHandlers(this._image, this.image$delegate);

        //this._image.onload = Function.createDelegate(this, this._imageElement_onLoad);
        this._image.onerror = Function.createDelegate(this, this._imageElement_onError);
        this._image.onclick = Function.createDelegate(this, this._imageElement_onClick);
        //        this.uploader$delegate = {
        //            change: Function.createDelegate(this, this._uploadElement_onChange)
        //        };

        this._imgUploader.onchange = Function.createDelegate(this, this._uploadElement_onChange);

        //        $clearHandlers(this._imgUploader);
        //        $addHandlers(this._imgUploader, this.uploader$delegate);
    },

    setImage: function (src, width, height) {
        var img = this._image;

        img.src = src;

        if (typeof (width) != "undefined")
            img.width = width;

        if (typeof (height) != "undefined")
            img.height = height;
    },

    saveClientState: function () {
        var state = [this._imageProperty, this.get_inputFileID()];

        return Sys.Serialization.JavaScriptSerializer.serialize(state);
    },

    loadClientState: function (value) {
        if (value) {
            if (value != "") {
                var tempResult = Sys.Serialization.JavaScriptSerializer.deserialize(value);
                if (Object.prototype.toString.apply(tempResult) === '[object Array]')
                    this._imageProperty = tempResult[0];
                else
                    this._imageProperty = tempResult;
            }
        }
    },

    //    _imageElement_onLoad: function () {
    //        var img = event.srcElement;

    //        this.sizeOverflow = false;

    //        if (this._filePathChanged) {
    //            this._filePathChanged = false;

    //            if (parseInt(img.fileSize) > this.get_fileMaxSize()) {
    //                this.sizeOverflow = true;
    //                this._clearImageAndFileInput();
    //                alert(String.format("选择图片大小超过了{0:D}字节,请重新选择", this.get_fileMaxSize()));

    //                return;
    //            }
    //        }

    //        //img.width = this.get_imageWidth();
    //        //img.height = this.get_imageHeight();

    //        this._imageProperty.Width = img.width;
    //        this._imageProperty.Height = img.height;

    //        if (this._isServerUrl(img.nameProp) == false)
    //            this._imageProperty.OriginalName = img.nameProp;

    //        this._imageProperty.Size = img.fileSize;
    //        this._imageProperty.Src = img.src;

    //    },

    _isServerUrl: function (url) {
        return url.indexOf("imagePropID") >= 0 &&
				url.indexOf("filePath") >= 0;
    },

    _uploadElement_onChange: function () {
        this._filePathChanged = true;
        this._image.src = event.srcElement.value;

        this._imageProperty.Width = this._image.width;
        this._imageProperty.Height = this._image.height;


        if (this._isServerUrl(this._image.src) == false) {
            var fileName = this._image.src;
            var nFileNameStart = fileName.lastIndexOf("/");
            fileName = fileName.substring(nFileNameStart + 1);
            this._imageProperty.OriginalName = fileName;
        }

        this._imageProperty.Src = this._image.src;

        if (this._autoUpload) {
            this.doUpload();
        }

        //        window.setTimeout(Function.createDelegate(this, function () {
        //            if (this._autoUpload && this.sizeOverflow == false) {
        //                this.doUpload();
        //            }
        //        }), 300); //不延迟有问题，image还没load就doupload了。。。
    },

    _imageElement_onError: function () {
        this._filePathChanged = false;
        var img = this._image;

        this._clearFileInput(this._imgUploader);
        img.src = this.get_defaultImg();
        img.width = 102;
        img.height = 102;

        return false;
    },

    _imageElement_onClick: function () {
        if (this._image.src.indexOf("file://") == 0)
            alert("图片地址是本地文件系统，不能在新窗口中打开");
        else
            window.open(this._image.src, "_blank");

    },

    _uploadButtonElement_onClick: function () {
        if (this._imgUploader && this._imgUploader.value == "") {
            alert("请选择图片！");
            return;
        }

        this.doUpload();
    },

    doUpload: function () {
        if (this._imgUploader && this._imgUploader.value != "") {
            var form = this._findServerForm(this._imgUploader);

            var clientOPHidden = $get("clientOPHidden");
            var clientImagePropertiesHidden = $get("clientImagePropertiesHidden");

            if (form) {
                var allEmabledElements = this._findAllEnabledInputElements(form);

                var oldTarget = form.target;
                var oldAction = form.action;
                var oldEncType = form.enctype;

                try {
                    form.target = this.get_innerFrameID();
                    form.enctype = "multipart/form-data";

                    clientOPHidden.value = "upload" + "," + this.get_controlID() + "," + this._imgUploader.name;
                    clientImagePropertiesHidden.value = Sys.Serialization.JavaScriptSerializer.serialize(this._imageProperty);
                    //clientUploadCtlIDHidden.value = this.get_controlID();

                    SubmitButtonIntance._setAllButtonsState(true);
                    ProgressBarInstance.show("正在上传...");

                    this._setAllEnabledInputElementsDisabled(allEmabledElements, false);

                    form.submit();

                    this.set_enabled(false);
                }
                finally {
                    form.target = oldTarget;
                    form.action = oldAction;
                    form.enctype = oldEncType;

                    this._setAllEnabledInputElementsDisabled(allEmabledElements, true);
                }
            }
        }
    },

    _setAllEnabledInputElementsDisabled: function (elements, enabled) {
        for (var i = 0; i < elements.length; i++) {
            var elem = elements[i];

            if (elem.type == "button" || (this._imgUploader && this._imgUploader.id == elem.id) ||
				elem.id == "clientImagePropertiesHidden" || elem.id == "clientOPHidden")
                continue;

            elem.disabled = !enabled;
        }
    },

    _findAllEnabledInputElements: function (form) {
        var result = [];

        this._pushEnabledInputElementsToArray(form, "INPUT", result);
        this._pushEnabledInputElementsToArray(form, "SELECT", result);
        this._pushEnabledInputElementsToArray(form, "TEXTAREA", result);

        return result;
    },

    _pushEnabledInputElementsToArray: function (form, tagName, result) {
        var elements = form.getElementsByTagName(tagName);

        for (var i = 0; i < elements.length; i++) {
            var elem = elements[i];

            if (elem.disabled == false)
                result.push(elem);
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
        this._clearImageAndFileInput();
        this.raiseClientImageDeleted();
    },

    _clearImageAndFileInput: function () {
        this._clearFileInput(this._imgUploader);

        this._image.src = this.get_defaultImg();
        this._imageProperty.Changed = true;
        this._imageProperty.OriginalName = "";
        this._imageProperty.NewName = "";
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

    uploadSuccess: function (str, imgUrl) {
        this.set_enabled(true);
        this._resetAllHiddenStates();

        SubmitButton.resetAllStates();
        ProgressBarInstance.hide();
        var imgProp = Sys.Serialization.JavaScriptSerializer.deserialize(str);

        this._imageProperty = imgProp;

        var img = this._image;
        if (imgUrl)
            img.src = imgUrl;
        this._clearFileInput(this._imgUploader);
        this.raiseClientImageUploaded(str, true);
    },

    _resetAllHiddenStates: function () {
        $get("clientImagePropertiesHidden").value = "";
        $get("clientOPHidden").value = "";
    },

    uploadFail: function (message) {
        this._clearImageAndFileInput();

        this.set_enabled(true);
        this._resetAllHiddenStates();

        SubmitButton.resetAllStates();

        $showError("上传失败！ " + message);
        ProgressBarInstance.hide();
        this.raiseClientImageUploaded("", "", false);
    },

    get_inputFileID: function () {
        return this._inputFileID;
    },
    set_inputFileID: function (value) {
        this._inputFileID = value;
    },

    get_imageID: function () {
        return this._imageID;
    },
    set_imageID: function (value) {
        this._imageID = value;
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

    get_uploadButtonId: function () {
        return this._uploadButtonId;
    },
    set_uploadButtonId: function (value) {
        this._uploadButtonId = value;
    },

    get_deleteButtonID: function () {
        return this._deleteButtonID;
    },
    set_deleteButtonID: function (value) {
        this._deleteButtonID = value;
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
    },

    get_controlID: function () {
        return this._controlID;
    },
    set_controlID: function (value) {
        this._controlID = value;
    },
    get_autoUpload: function () {
        return this._autoUpload;
    },
    set_autoUpload: function (value) {
        this._autoUpload = value;
    },

    get_cloneableProperties: function () {
        var baseProperties = $HGRootNS.ImageUploader.callBaseMethod(this, "get_cloneableProperties");
        var currentProperties = ["imageWidth", "imageHeight", "clientStateField", "defaultImg", "currentPageUrl",
					"fileMaxSize", "enabled", "autoUpload"];

        Array.addRange(currentProperties, baseProperties);

        return currentProperties;
    },

    _prepareCloneablePropertyValues: function (newElement) {
        var properties = $HGRootNS.ImageUploader.callBaseMethod(this, "_prepareCloneablePropertyValues", [newElement]);
        this.replaceOldIDs(newElement.id, newElement, properties);
        properties.controlID = newElement.id;
        properties.innerFrameID = this._innerFrameID;

        return properties;
    },

    //递归替换element的OldID
    replaceOldIDs: function (controlID, element, properties) {
        for (var i = 0; i < element.childNodes.length; i++) {
            var curElement = element.childNodes[i];
            switch (curElement.id) {
                case this._inputFileID:
                    curElement.id = curElement.uniqueID;
                    //$clearHandlers(curElement);
                    //$addHandler(curElement, "change", new Function("$find('" + controlID + "')._uploadElement_onChange();"));
                    properties.inputFileID = curElement.id;

                    break;
                case this._imageID:
                    curElement.id = curElement.uniqueID;

                    properties.imageID = curElement.id;
                    break;
                case this._uploadButtonId:
                    curElement.id = curElement.uniqueID;
                    //$clearHandlers(curElement);
                    //$addHandler(curElement, "click", new Function("$find('" + controlID + "')._uploadButtonElement_onClick();"));
                    properties.uploadButtonId = curElement.id;
                    break;
                case this._deleteButtonID:
                    curElement.id = curElement.uniqueID;
                    //$clearHandlers(curElement);
                    //$addHandler(curElement, "click", new Function("$find('" + controlID + "')._deleteButtonElement_onClick();"));
                    properties.deleteButtonID = curElement.id;
                    break;

            }
            this.replaceOldIDs(controlID, curElement, properties);
        }
    },

    cloneElement: function () {
        var result = null;
        var sourceElement = this.get_element();

        if (sourceElement != null) {
            this.onBeforeCloneElement(sourceElement);

            try {
                result = sourceElement.cloneNode(true);
                result.id = result.uniqueID;
                result.control = undefined;
            }
            finally {
                this.onAfterCloneElement(sourceElement, result);
            }
        }
        return result;

    },

    //    showImage: function (imageUrlQuery) {
    //        if (imageUrlQuery == "") {
    //            this._image.src = this.get_defaultImg();
    //        }
    //        else {
    //            var url = this.get_currentPageUrl() + "?" + imageUrlQuery;
    //            this.setImage(url);

    //        }
    //    },

    getImageUrl: function (imageID, filePath) {
        if (imageID == undefined || imageID == "") {
            return this.get_defaultImg();
        }
        else
            return this.get_currentPageUrl() + "?imagePropID=" + imageID + "&filePath=" + filePath;
    },

    showImage: function (imageID, filePath) {
        if (imageID == undefined || imageID == "") {
            this._image.src = this.get_defaultImg();
        }
        else {
            var url = this.get_currentPageUrl() + "?imagePropID=" + imageID + "&filePath=" + filePath;
            this.setImage(url);
            this._imageProperty.Width = this._image.width;
            this._imageProperty.Height = this._image.height;
            this._imageProperty.Src = this._image.src;
            this._imageProperty.ID = imageID;
            this._imageProperty.FilePath = filePath;
        }
    },

    raiseClientImageUploaded: function (imgProJsonStr, isSuccess) {
        var handlers = this.get_events().getHandler("imageUploaded");
        var e = Sys.EventArgs.Empty;
        e.ImgProJsonStr = imgProJsonStr;      
        e.IsSuccess = isSuccess;
        if (handlers) {
            handlers(this, e);
        }
    },
    remove_clientImageUploaded: function (handler) {
        this.get_events().removeHandler("imageUploaded", handler);
    },
    add_clientImageUploaded: function (handler) {
        this.get_events().addHandler("imageUploaded", handler);
    },

    raiseClientImageDeleted: function () {
        var handlers = this.get_events().getHandler("imageDeleted");
        if (handlers) {
            handlers(this, Sys.EventArgs.Empty);
        }
    },
    remove_clientImageDeleted: function (handler) {
        this.get_events().removeHandler("imageDeleted", handler);
    },
    add_clientImageDeleted: function (handler) {
        this.get_events().addHandler("imageDeleted", handler);
    },

    get_currentPageUrl: function () {
        return this._currentPageUrl;
    },
    set_currentPageUrl: function (value) {
        if (this._currentPageUrl != value) {
            this._currentPageUrl = value;
        }
    }
}

$HBRootNS.ImageUploader.registerClass($HBRootNSName + ".ImageUploader", $HBRootNS.ControlBase);