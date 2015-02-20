/// <reference path="../Common/MicrosoftAjax.debug.js" />
/// <reference path="../Script/Resources/HBCommon.js" />

$HBRootNS.ImageUploaderDialog = function (element) {
    $HBRootNS.ImageUploaderDialog.initializeBase(this, [element]);
    this._loaded = false;
    this._imageUploaderClientID = null;
    this._applicationLoad$delegate = Function.createDelegate(this, this._applicationLoad);
}

$HBRootNS.ImageUploaderDialog.prototype = {

    initialize: function () {
        $HBRootNS.ImageUploaderDialog.callBaseMethod(this, 'initialize');

        if (window.dialogArguments) {
            var imageUploader = $find(this.get_imageUploaderClientID());
            imageUploader.showImage(window.dialogArguments.imageID, window.dialogArguments.filePath);
        }
        Sys.Application.add_load(this._applicationLoad$delegate);
    },

    _applicationLoad: function () {
        if (this._loaded == false) {
            this._loaded = true;
        }
    },

    dispose: function () {
//        this._confirmButtonClientID = null;
        this._imageUploaderClientID = null;
        $HBRootNS.ImageUploaderDialog.callBaseMethod(this, 'dispose');
    },

    /*--------------------get-set-------------------------{*/

    get_imageUploaderClientID: function () {
        return this._imageUploaderClientID;
    },
    set_imageUploaderClientID: function (value) {
        this._imageUploaderClientID = value;
    },


//    /*---------------------------------------------------}*/

//    /*--------------------回调-------------------------{*/
//    /*--------------------回调-------------------------}*/
//    onConfirmButtonClick: function () {
//        var imageUploader = $find(this.get_imageUploaderClientID());
//        //var returnValue = [imageUploader._imageProperty.ID, imageUploader._imageProperty.FilePath];
//        window.returnValue = Sys.Serialization.JavaScriptSerializer.serialize(imageUploader._imageProperty);
//        window.close();

//    },

    _onConfirm: function (arg) {
        var imageUploader = $find(this.get_imageUploaderClientID());
        var ppt = imageUploader.get_innerProperty();
        if (!ppt || !ppt.ID || !ppt.NewName || !ppt.Changed) {
            arg.canceled = true;
        } else {
            arg.ImageID = ppt.ID;
            arg.Path = ppt.FilePath;
            arg.ImagePropertyString = Sys.Serialization.JavaScriptSerializer.serialize(ppt);
        }
        ppt = null;
        imageUploader = null;
    },

    showDialog: function (imageID, filePath, callbackOK) {
        var params = new Object();
        params.imageID = imageID;
        params.filePath = filePath;
        this._showDialog(this._dialogUrl
                                        + "&imageID=" + imageID + "&filePath=" + filePath, params, function (s, e) {
                                            if (callbackOK) { callbackOK(e.ImagePropertyString) };
                                        });
    },

    dataBind: function (args) {
        var imageUploader = $find(this.get_imageUploaderClientID());
        imageUploader.showImage(args.imageID, args.filePath);
    }
}

$HBRootNS.ImageUploaderDialog.registerClass($HBRootNSName + ".ImageUploaderDialog", $HGRootNS.DialogControlBase);
