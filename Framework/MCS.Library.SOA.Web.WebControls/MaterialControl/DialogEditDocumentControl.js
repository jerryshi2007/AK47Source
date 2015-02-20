// -------------------------------------------------
// FileName	：	DialogEditDocumentControl.js
// Remark	：	编辑文档
// -------------------------------------------------
// VERSION		AUTHOR		DATE			CONTENT
// 1.0			冯立雷		20111128		创建
// -------------------------------------------------

$HBRootNS.DialogEditDocumentControl = function (element) {
    $HBRootNS.DialogEditDocumentControl.initializeBase(this, [element]);

    this._material = null;
    this._officeViewerWrapperID = "";
}

$HBRootNS.DialogEditDocumentControl.prototype =
{

    initialize: function () {

        $HBRootNS.DialogEditDocumentControl.callBaseMethod(this, "initialize");
        var dialogArg = window.dialogArguments;
      
        this._material = dialogArg.material;

        var delegateOpen = Function.createDelegate(this, this._openDocument);
        if (this._material) {
            window.setTimeout(delegateOpen, 100);
        }

        var delegateCheck = Function.createDelegate(this, this.checkIsDirty);
        $HBRootNS.HBCommon.registSubmitValidator(delegateCheck, 1);

        //this.add_onBeforDocumentSaved(Function.createDelegate(this, this.set_MaterialModified));
        //alert(this._material._materialID);
    },

    dispose: function () {

        this._officeViewerWrapperID = null;

        $HBRootNS.DialogEditDocumentControl.callBaseMethod(this, "dispose");
    },

    _openDocument: function () {
        var viewer = this._get_officeViewerWrapperViewer();
        if (viewer.Open(this._material._realLocalPath)) {
            viewer.SetAppFocus();
            if (this._material._realLocalPath != this._material._localPath) {
                viewer.SaveAs(this._material._localPath);
                this._material._realLocalPath = this._material._localPath;
                viewer.SetAppFocus();
            }
        }
        else {
            this._material.currentDocument = document;
            this._material._downloaded = false;
            this._material._clearLocalData();
            this._material.loadLocalData();
            var executedDownload = false;

            if (this._material._isDraft)
                executedDownload = this._material._downloadTemplate(false);
            else
                executedDownload = this._material._downloadDocument();
            if (executedDownload) {
                if (viewer.Open(this._material._realLocalPath)) {
                    viewer.SetAppFocus();
                    if (this._material._realLocalPath != this._material._localPath) {
                        viewer.SaveAs(this._material._localPath);
                        this._material._realLocalPath = this._material._localPath;
                        viewer.SetAppFocus();
                    }
                }
            }
            this._material.currentDocument = null;
        }
    },

    set_MaterialModified: function () {
        if (this._material) {
            var wrapperViewer = this._get_officeViewerWrapperViewer();
            wrapperViewer.SetAppFocus();
            if (wrapperViewer.IsDirty()) {
                var data = new Object();
                data.modifyTime = new Date();
                data.isModified = true;
                this._material.persistLocalData(data);
            }
        }
    },

    get_officeViewerWrapperID: function () {
        return this.officeViewerWrapperID;
    },
    set_officeViewerWrapperID: function (value) {
        if (this._officeViewerWrapperID != value) {
            this._officeViewerWrapperID = value;
            this.raisePropertyChanged("officeViewerWrapperID");
        }
    },

    _get_officeViewerWrapperViewer: function () {
        return $get(this._officeViewerWrapperID + "_Viewer");
    },

    checkIsDirty: function () {
        window.returnValue = this._material;
        var viewer = this._get_officeViewerWrapperViewer();
        viewer.SetAppFocus();
        var isDirty = viewer.IsDirty();

        if (isDirty == true) {
            event.returnValue = $NT.getText("SOAWebControls", "您编辑了文件但没有保存当前文件，请您保存文件后再关闭页面！");
            return false;
        }

        return true;
    },

    add_onBeforDocumentSaved: function (handler) {
        this.get_events().addHandler("beforDocumentSaved", handler);
    },
    remove_onBeforDocumentSaved: function (handler) {
        this.get_events().removeHandler("beforDocumentSaved", handler);
    },
    raiseBeforDocumentSaved: function () {
        var handlers = this.get_events().getHandler("beforDocumentSaved");
        var e = new Sys.EventArgs();

        if (handlers)
            handlers(this, e);

        return e;
    }
}

$HBRootNS.DialogEditDocumentControl.registerClass($HBRootNSName + ".DialogEditDocumentControl", $HGRootNS.ControlBase);