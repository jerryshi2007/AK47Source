///
///编辑器控件和
///
UEditorWrapperBridge = (function () {
    var unique;
    function constructor() {
        var activeEditorWrapper;

        return {
            manualUpload: function (arrUrl) {
                if (activeEditorWrapper)
                    return activeEditorWrapper.manualUpload(arrUrl);
            },
            SetActiveEditorWrapper: function (aew) {
                if (aew.constructor === String) {
                    var arrIdSplit = aew.split("_");
                    activeEditorWrapper = $find(arrIdSplit[0]);
                } else {
                    activeEditorWrapper = aew;
                }
            },
            getActiveEditor: function () {
                return activeEditorWrapper;
            }

        }
    }

    return {
        getInstance: function () {
            if (!unique) {
                unique = constructor();
            }
            return unique;
        }

    }
})()