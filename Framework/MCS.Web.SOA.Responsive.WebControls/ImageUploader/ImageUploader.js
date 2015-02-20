/// <reference path="../Common/MicrosoftAjax.debug.js" />
/// <reference path="../Script/Resources/HBCommon.js" />


$HBRootNS.ImageUploader = function (element) {
    $HBRootNS.ImageUploader.initializeBase(this, [element]);
    //    this._inputFileID;
    //    this._imageID;
    //    this._imgUploader = null;
    //    this._image = null;
    //    this._imageWidth = 0;
    //    this._imageHeight = 0;
    this._imageProperty = {};
    //    this._currentPageUrl = null;

    //    this._defaultImg = "";

    //    this._uploadButtonId = "";
    //    this._uploadButton = null;
    //    this._deleteButtonID = "";
    //    this._deleteButton = null;
    //    this._innerFrameID = "";
    //    this._innerFrameContainerID = "";

    this._fileMaxSize = 0;
    this._enabled = true;
    //    this._controlID = "";
    //    this._filePathChanged = false;

    // ui:
    this._elemImageArea = null;
    this._elemImage = null;
    this._operationArea = null;
    this._buttonUpload = null;
    this._buttonDelete = null;
    this._buttonView = null;
    this._iframe = null;
    this._form = null;
    this._inputFile = null;
    this._elemProgress = null;

    this._hasImage = null;
    //event handle

    this._handlerFilePicked = null;
}

$HBRootNS.ImageUploader.prototype = {
    initialize: function () {
        var imgUrl;
        $HBRootNS.ImageUploader.callBaseMethod(this, 'initialize');

        //        this._imgUploader = $get(this.get_inputFileID());
        //        this._image = $get(this.get_imageID());

        //        this._uploadButton = $get(this.get_uploadButtonId());
        //        this._deleteButton = $get(this.get_deleteButtonID());

        //        this._uploadButton.onclick = Function.createDelegate(this, this._uploadButtonElement_onClick);
        //        this._deleteButton.onclick = Function.createDelegate(this, this._deleteButtonElement_onClick);
        //        this._image.onerror = Function.createDelegate(this, this._imageElement_onError);
        //        this._image.onclick = Function.createDelegate(this, this._imageElement_onClick);

        //        this._imgUploader.onchange = Function.createDelegate(this, this._uploadElement_onChange);

        this.buildContent(this.get_element());
        if (this._imageProperty && this._imageProperty.ID) {
            imgUrl = this.getImageUrl(this._imageProperty.ID, this._imageProperty.FilePath);
            this._elemImage.src = imgUrl;
        }
        this.applyStyles();

    },

    buildContent: function (container) {
        var spanIconDeleted, spanIconUpload, spanIconOpen, divOpWrapper;
        Sys.UI.DomElement.addCssClass(container, "imageuploader");
        this._elemImageArea = document.createElement("div");
        this._elemImageArea.className = "image-wrapper";
        container.appendChild(this._elemImageArea);

        this._elemImage = document.createElement("img");
        this._elemImageArea.appendChild(this._elemImage);

        divOpWrapper = document.createElement("div");
        divOpWrapper.className = "operations-wrapper";
        container.appendChild(divOpWrapper);

        this._operationArea = document.createElement("div");
        this._operationArea.className = "operations input-group-btn";
        divOpWrapper.appendChild(this._operationArea);

        this._buttonUpload = document.createElement("div");
        this._buttonUpload.className = "cmdbtn btn btn-default btn-upload";
        this._buttonUpload.title = "上传";
        this._operationArea.appendChild(this._buttonUpload);
        spanIconUpload = document.createElement("span");
        spanIconUpload.className = "glyphicon glyphicon-open";
        this._buttonUpload.appendChild(spanIconUpload);

        this._buttonDelete = document.createElement("div");
        this._buttonDelete.className = "cmdbtn btn btn-default btn-delete";
        this._buttonDelete.title = "删除";
        this._operationArea.appendChild(this._buttonDelete);
        spanIconDeleted = document.createElement("span");
        spanIconDeleted.className = "glyphicon glyphicon-trash";
        this._buttonDelete.appendChild(spanIconDeleted);

        this._buttonView = document.createElement("a")
        this._buttonView.className = "cmdbtn btn btn-default btn-view";
        this._buttonView.target = "_blank";
        this._buttonView.title = "在新窗口中打开";
        this._operationArea.appendChild(this._buttonView);
        spanIconOpen = document.createElement("span");
        spanIconOpen.className = "glyphicon glyphicon-new-window";
        this._buttonView.appendChild(spanIconOpen);

        this._inputFile = document.createElement("input");
        this._inputFile.type = "file";
        this._inputFile.accept = "image/jpeg,image/png,image/gif";
        this._inputFile.className = "file-uploader";
        this._buttonUpload.appendChild(this._inputFile);

        this._elemProgress = document.createElement("div");
        this._elemProgress.className = "progress";
        container.appendChild(this._elemProgress);
        this._elemProgress.appendChild(document.createTextNode("处理中..."));

        this._handlerFilePicked = Function.createDelegate(this, this._onPickFile);
        this._handlerImageLoaded = Function.createDelegate(this, this._onImageLoaded);
        this._handlerImageError = Function.createDelegate(this, this._onImageError);
        this._handlerDeleteImage = Function.createDelegate(this, this._onDeleteImage);
        this._fileUploadedCallbackHandler = Function.createDelegate(this, this._fileUploadedCallback);

        $addHandler(this._inputFile, "change", this._handlerFilePicked);
        $addHandler(this._buttonDelete, "click", this._handlerDeleteImage);
        $addHandler(this._elemImage, "load", this._handlerImageLoaded);
        $addHandler(this._elemImage, "error", this._handlerImageError);
    },

    //    setImage: function (src, width, height) {
    //        var img = this._image;

    //        img.src = src;

    //        if (typeof (width) != "undefined")
    //            img.width = width;

    //        if (typeof (height) != "undefined")
    //            img.height = height;
    //    },

    saveClientState: function () {
        var state = this._imageProperty;

        return Sys.Serialization.JavaScriptSerializer.serialize(state);
    },

    loadClientState: function (value) {
        if (value) {
            if (value != "") {
                var tempResult = Sys.Serialization.JavaScriptSerializer.deserialize(value);
                this._imageProperty = tempResult;
            }
        }
    },

    applyStyles: function () {
        if (this._imageProperty) {
            if (typeof (this._imageProperty.NewName) === 'string' && this._imageProperty.NewName.length) {
                Sys.UI.DomElement.removeCssClass(this.get_element(), "noimage");
            } else {
                Sys.UI.DomElement.addCssClass(this.get_element(), "noimage");
            }

            if (this.get_enabled()) {
                Sys.UI.DomElement.removeCssClass(this.get_element(), "readonly");
            } else {
                Sys.UI.DomElement.addCssClass(this.get_element(), "readonly");
            }
        }
    },

    dispose: function () {
        if (this._form) {
            this._form.parentNode.removeChild(this._form);
            this._form = null;
        }

        if (this._iframe) {
            $removeHandler(this._iframe, "load", this._fileUploadCallbackHandler);
            this._iframe.parentNode.removeChild(this._iframe);
            this._iframe = null;
        }

        $removeHandler(this._inputFile, "change", this._handlerFilePicked);
        $removeHandler(this._buttonDelete, "click", this._handlerDeleteImage);
        $removeHandler(this._elemImage, "load", this._handlerImageLoaded);
        $removeHandler(this._elemImage, "error", this._handlerImageError);

        this._handlerFilePicked = null;
        this._handlerImageLoaded = null;
        this._handlerImageError = null;
        this._handlerDeleteImage = null;
        this._fileUploadedCallbackHandler = null;

        this._elemImageArea = null;
        this._elemProgress = null;
        this._elemImage = null;
        this._operationArea = null;
        this._buttonUpload = null;
        this._buttonDelete = null;
        this._buttonView = null;
        this._iframe = null;
        this._form = null;
        this._inputFile = null;
        this.get_element().innerHTML = '';

        $HBRootNS.ImageUploader.callBaseMethod(this, 'dispose');

    },

    //    _isServerUrl: function (url) {
    //        return url.indexOf("imagePropID") >= 0 &&
    //				url.indexOf("filePath") >= 0;
    //    },

    _onPickFile: function (e) {
        var that = this, fileMime;
        if (this._inputFile.files.length == 0) {
            return;
        }

        if (!this.get_enabled()) {
            return;
        }

        fileMime = this._inputFile.files[0].type;
        if (fileMime !== "image/png" && fileMime !== "image/jpeg" && fileMime !== "image/gif") {
            $HBRootNS.BalloonMessage(this._operationArea, "warning", "无效的格式", "选择的文件不是图片", 100, 40, "down");
            return;
        }

        if (this.get_fileMaxSize() > 0 && this._inputFile.files[0].size > this.get_fileMaxSize()) {
            $HBRootNS.BalloonMessage(this._operationArea, "warning", "文件太大", "选择的文件太大", 100, 40, "down");
            return;
        }

        if (!this._form) {
            var form = document.createElement("form");
            var iframe = document.createElement("iframe");
            var input = document.createElement("input");
            input.type = "hidden";
            input.name = "imageInfo";
            iframe.id = "iframe_" + new Date().getTime();
            iframe.name = iframe.id;
            document.body.appendChild(iframe);
            iframe.style.display = "none";
            form.method = "POST";
            form.style.display = "none";
            document.body.appendChild(form);
            form.enctype = "multipart/form-data";
            form.target = iframe.id;
            form.appendChild(input);

            this._form = form;
            this._iframe = iframe;
            this._inputInfo = input;

            $addHandler(this._iframe, "load", Function.createDelegate(this, this._fileUploadedCallback));
        }

        this._imageProperty.OriginalName = this._inputFile.files[0].name;
        this._form.appendChild(this._inputFile);
        this._inputFile.name = "imagedata";
        this._inputInfo.value = Sys.Serialization.JavaScriptSerializer.serialize(this._imageProperty);
        this._form.action = "some.imgupload?action=upload";
        try {
            Sys.UI.DomElement.addCssClass(this.get_element(), "uploading");
            this._form.submit();
        } catch (ex) {
            Sys.UI.DomElement.removeCssClass(this.get_element(), "uploading");
        }

        this.applyStyles();
    },

    _fileUploadedCallback: function (win) {
        Sys.UI.DomElement.removeCssClass(this.get_element(), "uploading");
        var elemImgInfo = this._iframe.contentDocument.getElementById('imgInfo');
        var elemImgPath = this._iframe.contentDocument.getElementById('imgPath');

        if (elemImgInfo) {

            var imgProp = Sys.Serialization.JavaScriptSerializer.deserialize(elemImgInfo.value);

            this._imageProperty = imgProp;

            if (elemImgPath)
                this._elemImage.src = elemImgPath.value;
            //this._clearFileInput(this._imgUploader);
            this.raiseClientImageUploaded(elemImgInfo.value, true);
        }

        this._form.reset();
        this._buttonUpload.appendChild(this._inputFile);
        this._inputFile.name = "";
        this._buttonView.href = elemImgPath.value;
        this.applyStyles();
    },

    _onDeleteImage: function () {
        if (this.get_enabled()) {
            this._elemImage.src = "";
            this._imageProperty.Changed = true;
            this._imageProperty.OriginalName = "";
            this._imageProperty.NewName = "";

            this.raiseClientImageDeleted();
            if (this._inputFile) {
                this._buttonView.href = "";

                this.applyStyles();
            }
        }
    },

    _onImageLoaded: function () {
        console.info('图片已加载');
        this.applyStyles();
    },

    _onImageError: function () {
        console.error('图片加载错误');
    },



    //    _uploadElement_onChange: function () {
    //        try {
    //            var path, name;
    //            this._filePathChanged = true;
    //            if (this._imgUploader.files.length) {

    //                name = this._imgUploader.files[0].name;
    //                this._image.src = decodeURIComponent(this._imgUploader.files[0].mozFullPath || this._imgUploader.value);

    //                this._imageProperty.Width = this._image.width;
    //                this._imageProperty.Height = this._image.height;

    //                if (this._isServerUrl(this._image.src) == false) {
    //                    this._imageProperty.OriginalName = name;
    //                }

    //                this._imageProperty.Src = this._image.src;
    //            }
    //        } catch (e) {

    //        }


    //        if (this._autoUpload) {
    //            this.doUpload();
    //        }

    //        return true;

    //    },

    //    _imageElement_onError: function () {
    //        this._filePathChanged = false;
    //        var img = this._image;

    //        //this._clearFileInput(this._imgUploader);
    //        img.src = this.get_defaultImg();
    //        img.width = 102;
    //        img.height = 102;

    //        return false;
    //    },

    //    _imageElement_onClick: function () {
    //        if (this._image.src.indexOf("file://") == 0)
    //            alert("图片地址是本地文件系统，不能在新窗口中打开");
    //        else
    //            window.open(this._image.src, "_blank");

    //    },

    //    _uploadButtonElement_onClick: function () {
    //        if (this._imgUploader && this._imgUploader.value == "") {
    //            alert("请选择图片！");
    //            return;
    //        }

    //        this.doUpload();
    //    },

    //    doUpload: function () {
    //        if (this._imgUploader && (this._imgUploader.value != "" || this._imgUploader.files.length)) {
    //            var form = this._findServerForm(this._imgUploader);

    //            var clientOPHidden = $get("clientOPHidden");
    //            var clientImagePropertiesHidden = $get("clientImagePropertiesHidden");

    //            if (form) {
    //                var allEmabledElements = this._findAllEnabledInputElements(form);

    //                var oldTarget = form.target;
    //                var oldAction = form.action;
    //                var oldEncType = form.enctype;

    //                try {
    //                    form.target = this.get_innerFrameID();
    //                    form.enctype = "multipart/form-data";

    //                    clientOPHidden.value = "upload" + "," + this.get_controlID() + "," + this._imgUploader.name;
    //                    clientImagePropertiesHidden.value = Sys.Serialization.JavaScriptSerializer.serialize(this._imageProperty);
    //                    //clientUploadCtlIDHidden.value = this.get_controlID();

    //                    SubmitButtonIntance._setAllButtonsState(true);
    //                    ProgressBarInstance.show("正在上传...");

    //                    this._setAllEnabledInputElementsDisabled(allEmabledElements, false);

    //                    form.submit();

    //                    this.set_enabled(false);
    //                } catch (ex) {
    //                    if (console)
    //                        consol.error('上传文件出错：' + ex.message);
    //                }
    //                finally {
    //                    form.target = oldTarget;
    //                    form.action = oldAction;
    //                    form.enctype = oldEncType;

    //                    this._setAllEnabledInputElementsDisabled(allEmabledElements, true);
    //                }
    //            }
    //        }
    //    },

    //    _setAllEnabledInputElementsDisabled: function (elements, enabled) {
    //        for (var i = 0; i < elements.length; i++) {
    //            var elem = elements[i];

    //            if (elem.type == "button" || (this._imgUploader && this._imgUploader.id == elem.id) ||
    //				elem.id == "clientImagePropertiesHidden" || elem.id == "clientOPHidden")
    //                continue;

    //            elem.disabled = !enabled;
    //        }
    //    },

    //    _findAllEnabledInputElements: function (form) {
    //        var result = [];

    //        this._pushEnabledInputElementsToArray(form, "INPUT", result);
    //        this._pushEnabledInputElementsToArray(form, "SELECT", result);
    //        this._pushEnabledInputElementsToArray(form, "TEXTAREA", result);

    //        return result;
    //    },

    //    _pushEnabledInputElementsToArray: function (form, tagName, result) {
    //        var elements = form.getElementsByTagName(tagName);

    //        for (var i = 0; i < elements.length; i++) {
    //            var elem = elements[i];

    //            if (elem.disabled == false)
    //                result.push(elem);
    //        }
    //    },

    get_innerProperty: function () {
        return this._imageProperty;
    },

    get_enabled: function () {
        return this._enabled;
    },

    set_enabled: function (enabled) {
        this._enabled = enabled;
        this._set_elementEnabled(this.inputFile, enabled);
        //this._set_elementEnabled(this._uploadButton, enabled);
        //this._set_elementEnabled(this._deleteButton, enabled);

        if (this._inputFile)
            this.applyStyles();
    },

    _set_elementEnabled: function (element, enabled) {
        if (element) {
            element.disabled = !enabled;
        }
    },

    _deleteButtonElement_onClick: function () {
        //this._clearImageAndFileInput();
        this.raiseClientImageDeleted();
    },

    //    _clearImageAndFileInput: function () {
    //        this._clearFileInput(this._imgUploader);

    //        this._image.src = this.get_defaultImg();
    //        this._imageProperty.Changed = true;
    //        this._imageProperty.OriginalName = "";
    //        this._imageProperty.NewName = "";
    //    },

    //    _clearFileInput: function (file) {
    //        var form = document.createElement('form');
    //        document.body.appendChild(form);
    //        //记住file在旧表单中的的位置
    //        var pos = file.nextSibling;
    //        form.appendChild(file);
    //        form.reset();
    //        pos.parentNode.insertBefore(file, pos);
    //        document.body.removeChild(form);
    //    },

    //    _findServerForm: function (elem) {
    //        var form = null;

    //        while (elem) {
    //            if (elem.tagName == "FORM") {
    //                form = elem;
    //                break;
    //            }

    //            elem = elem.parentElement;
    //        }

    //        return form;
    //    },

    //    uploadSuccess: function (str, imgUrl) {
    //        this.set_enabled(true);
    //        this._resetAllHiddenStates();

    //        SubmitButton.resetAllStates();
    //        ProgressBarInstance.hide();
    //        var imgProp = Sys.Serialization.JavaScriptSerializer.deserialize(str);

    //        this._imageProperty = imgProp;

    //        var img = this._image;
    //        if (imgUrl)
    //            img.src = imgUrl;
    //        //this._clearFileInput(this._imgUploader);
    //        this.raiseClientImageUploaded(str, true);
    //    },

    //    _resetAllHiddenStates: function () {
    //        $get("clientImagePropertiesHidden").value = "";
    //        $get("clientOPHidden").value = "";
    //    },

    //    uploadFail: function (message) {
    //        this._clearImageAndFileInput();

    //        this.set_enabled(true);
    //        this._resetAllHiddenStates();

    //        SubmitButton.resetAllStates();

    //        $showError("上传失败！ " + message);
    //        ProgressBarInstance.hide();
    //        this.raiseClientImageUploaded("", "", false);
    //    },

    //    get_inputFileID: function () {
    //        return this._inputFileID;
    //    },
    //    set_inputFileID: function (value) {
    //        this._inputFileID = value;
    //    },

    //    get_imageID: function () {
    //        return this._imageID;
    //    },
    //    set_imageID: function (value) {
    //        this._imageID = value;
    //    },

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

    //    get_uploadButtonId: function () {
    //        return this._uploadButtonId;
    //    },
    //    set_uploadButtonId: function (value) {
    //        this._uploadButtonId = value;
    //    },

    //    get_deleteButtonID: function () {
    //        return this._deleteButtonID;
    //    },
    //    set_deleteButtonID: function (value) {
    //        this._deleteButtonID = value;
    //    },

    //    get_innerFrameID: function () {
    //        return this._innerFrameID;
    //    },
    //    set_innerFrameID: function (value) {
    //        this._innerFrameID = value;
    //    },

    //    get_innerFrameContainerID: function () {
    //        return this._innerFrameContainerID;
    //    },
    //    set_innerFrameContainerID: function (value) {
    //        this._innerFrameContainerID = value;
    //    },

    get_fileMaxSize: function () {
        return this._fileMaxSize;
    },
    set_fileMaxSize: function (value) {
        this._fileMaxSize = value;
    },

    //    get_controlID: function () {
    //        return this._controlID;
    //    },
    //    set_controlID: function (value) {
    //        this._controlID = value;
    //    },
    //    get_autoUpload: function () {
    //        return this._autoUpload;
    //    },
    //    set_autoUpload: function (value) {
    //        this._autoUpload = value;
    //    },

    get_cloneableProperties: function () {
        var baseProperties = $HGRootNS.ImageUploader.callBaseMethod(this, "get_cloneableProperties");
        var currentProperties = ["imageWidth", "imageHeight", "clientStateField", "defaultImg", "currentPageUrl",
					"fileMaxSize", "enabled", "autoUpload"];

        Array.addRange(currentProperties, baseProperties);

        return currentProperties;
    },

    _prepareCloneablePropertyValues: function (newElement) {
        var properties = $HGRootNS.ImageUploader.callBaseMethod(this, "_prepareCloneablePropertyValues", [newElement]);
        //this.replaceOldIDs(newElement.id, newElement, properties);
        //        properties.controlID = newElement.id;
        //properties.innerFrameID = this._innerFrameID;

        return properties;
    },

    //    //递归替换element的OldID
    //    replaceOldIDs: function (controlID, element, properties) {
    //        debugger;
    //        throw Error.create("不应该执行到这里");
    //        for (var i = 0; i < element.childNodes.length; i++) {
    //            var curElement = element.childNodes[i];
    //            switch (curElement.id) {
    //                case this._inputFileID:
    //                    curElement.id = curElement.uniqueID;
    //                    properties.inputFileID = curElement.id;

    //                    break;
    //                case this._imageID:
    //                    curElement.id = curElement.uniqueID;

    //                    properties.imageID = curElement.id;
    //                    break;
    //                case this._uploadButtonId:
    //                    curElement.id = curElement.uniqueID;
    //                    properties.uploadButtonId = curElement.id;
    //                    break;
    //                case this._deleteButtonID:
    //                    curElement.id = curElement.uniqueID;
    //                    properties.deleteButtonID = curElement.id;
    //                    break;

    //            }
    //            this.replaceOldIDs(controlID, curElement, properties);
    //        }
    //    },

    cloneElement: function () {
        var result = null;
        var sourceElement = this.get_element();
        var seed = 1, newId;
        if (sourceElement != null) {
            this.onBeforeCloneElement(sourceElement);

            try {
                //                result =  sourceElement.cloneNode(true);

                result = document.createElement("div");
                result.className = "imageuploader";

                //                newId = result.uniqueID || ((sourceElement.id || "") + "autogen_" + seed);
                //                while (document.getElementById(newId)) {
                //                    seed++;
                //                    newId = ((sourceElement.id || "") + "_autogen_" + seed);
                //                }
                //                result.id = newId;
                //result.control = undefined;
            }
            finally {
                this.onAfterCloneElement(sourceElement, result);
            }
        }
        return result;

    },

    getImageUrl: function (imageID, filePath) {
        if (!imageID) {
            return this.get_defaultImg();
        }
        else
            return "some.imgupload?action=getimage&imagePropID=" + imageID + "&filePath=" + filePath;
    },

    showImage: function (imageID, filePath) {
        if (imageID == undefined || imageID == "") {
            this._elemImage.src = this.get_defaultImg();
        }
        else {
            var url = "some.imgupload?action=getimage&imagePropID=" + imageID + "&filePath=" + filePath;
            this._elemImage.src = url;
            // this.setImage(url);
            this._imageProperty.Width = this._elemImage.width;
            this._imageProperty.Height = this._elemImage.height;
            this._imageProperty.Src = this._elemImage.src;
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
    }

    //    get_currentPageUrl: function () {
    //        return this._currentPageUrl;
    //    },
    //    set_currentPageUrl: function (value) {
    //        if (this._currentPageUrl != value) {
    //            this._currentPageUrl = value;
    //        }
    //    }
}

$HBRootNS.ImageUploader.registerClass($HBRootNSName + ".ImageUploader", $HBRootNS.ControlBase);