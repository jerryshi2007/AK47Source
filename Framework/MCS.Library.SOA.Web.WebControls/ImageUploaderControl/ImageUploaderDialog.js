
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
        this._confirmButtonClientID = null;
        this._imageUploaderClientID = null;
        $HBRootNS.ImageUploaderDialog.callBaseMethod(this, 'dispose');
    },

    /*--------------------get-set-------------------------{*/
    get_confirmButtonClientID: function () {
        return this._confirmButtonClientID;
    },
    set_confirmButtonClientID: function (value) {
        this._confirmButtonClientID = value;
    },
    get_imageUploaderClientID: function () {
        return this._imageUploaderClientID;
    },
    set_imageUploaderClientID: function (value) {
        this._imageUploaderClientID = value;
    },

    get_dialogHeaderTextSpanID: function () {
        return "ExecutorEditControlDialog_dialogHeaderText";
    },

    /*---------------------------------------------------}*/

    /*--------------------回调-------------------------{*/
    /*--------------------回调-------------------------}*/
    onConfirmButtonClick: function () {
        var imageUploader = $find(this.get_imageUploaderClientID());
        //var returnValue = [imageUploader._imageProperty.ID, imageUploader._imageProperty.FilePath];
        window.returnValue = Sys.Serialization.JavaScriptSerializer.serialize(imageUploader._imageProperty);
        window.close();

    },

    showDialog: function (imageID, filePath) {
        var params = new Object();
        params.imageID = imageID;
        params.filePath = filePath;
        var resultStr = this._showDialog(params, this._dialogUrl
                                        + "&imageID=" + imageID + "&filePath=" + filePath);
        return resultStr;
    }
}

$HBRootNS.ImageUploaderDialog.registerClass($HBRootNSName + ".ImageUploaderDialog", $HGRootNS.DialogControlBase);
