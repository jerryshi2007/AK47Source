
//MCS.Web.Controls.SampleControl = function(element)
$HGRootNS.SampleControl = function (element) {
    $HGRootNS.SampleControl.initializeBase(this, [element]);
    this._text = null;
    this._input = null;
    this._inputStyle = null;
    this._inputStyle2 = null;
    this._inputCssClass = null;
    this._input = null;
    this._samObject = null;
    this._alertImgUrl = null;
    this._exportToWordUrl = null;
    this._samTime = null;
    this._btnEvents =
        {
            click: Function.createDelegate(this, this._onBtnClick)
        };
    this._inputEvents =
        {
            change: Function.createDelegate(this, this._onInputChange)
        };

    this._exportToWordBtnEvents =
        {
            click: Function.createDelegate(this, this._onExportToWordBtnClick)
        };
}

$HGRootNS.SampleControl.prototype =
{
    initialize: function () {
        $HGRootNS.SampleControl.callBaseMethod(this, 'initialize');
        $addHandler(window, "beforeunload", Function.createDelegate(this, this._onBtnClick));
        this._buildControl();
    },

    dispose: function () {
        $HGDomEvent.removeHandlers(this._input, this._inputEvents);
        this._input = null;
        $HGRootNS.SampleControl.callBaseMethod(this, 'dispose');
    },

    _buildControl: function () {
        var elt = this.get_element();
        var btn = $HGDomElement.createElementFromTemplate(
                {
                    nodeName: "img",
                    properties: { src: this._alertImgUrl }
                },
                elt
            );

        this._input = this._input = $HGDomElement.createElementFromTemplate(
                {
                    nodeName: "input",
                    properties: { type: "text", value: this.get_text(), style: this._inputStyle },
                    cssClasses: [this._inputCssClass || "input"],
                    events: this._inputEvents
                },
                elt
            );
        Sys.UI.DomElement.addCssClass(this._input, this._inputCssClass);

        var btn = $HGDomElement.createElementFromTemplate(
                {
                    nodeName: "input",
                    properties: { type: "button", value: "回 调", style: this._inputStyle2 || {} },
                    cssClasses: ["button"],
                    events: this._btnEvents
                },
                elt
            );

        $HGDomElement.createElementFromTemplate(
                {
                    nodeName: "input",
                    properties: { type: "button", value: "导出到Word" },
                    cssClasses: ["button"],
                    events: this._exportToWordBtnEvents
                },
                elt
            );
    },

    _onBtnClick: function () {
        //        alert(String.format.apply(String, ["{0}-{1}","a", "b"]));
        //        alert(String.format("{0}-{1}", ["a", "b"]));
        //        alert(String.format("{0}-{1}", "a", "b"));
        this._invoke("GetSampleObject", [new Date(), "Hujintao", 180], Function.createDelegate(this, this._invokeCallback), Function.createDelegate(this, this._invokeCallbackError));

        //this._invoke("SetSampleObject", [[$Serializer.setType({DT:new Date(), Name:"Hujintao", Height:180}, "SampleObjectTypeKey"), {__type:"MCS.Web.WebControls.SampleObject, DeluxeWorks.Web.WebControls, Version=1.0.1.0, Culture=neutral, PublicKeyToken=04aea43db8c1b49e",DT:new Date(), Name:"Hujintao", Height:180}]], Function.createDelegate(this, this._invokeCallback), false, false);
    },

    _onExportToWordBtnClick: function () {
        window.open(this._exportToWordUrl);
    },

    _onInputChange: function () {
        this.set_text(this._input.value);
    },

    _invokeCallback: function (result) {
        // alert(this.get_text() + "-----" +  result);
        this._raiseCallbackCompleteEvent(result);
    },

    _invokeCallbackError: function (err) {
        switch (err.name) {
            case "Error":
                alert(String.format("调用函数{0}出现错误:{1}", "GetSampleObject", err.message));
                break;

            case "System.Exception":
                alert(String.format("调用函数{0}出现异常:{1}", "GetSampleObject", err.message));
                break;

            default:
                alert(String.format("调用函数{0}出现其他异常:{1}", "GetSampleObject", err.message));
                break;
        }
    },

    _callbackCompleteEventKey: "callbackComplete",

    _raiseCallbackCompleteEvent: function (result) {
        var handler = this.get_events().getHandler(this._callbackCompleteEventKey);
        if (handler) {
            var e = new Sys.EventArgs;
            e.result = result;
            handler(this, e);
        }
    },

    loadClientState: function (value) {
        this._samObject = Sys.Serialization.JavaScriptSerializer.deserialize(value);
    },

    saveClientState: function () {
        return Sys.Serialization.JavaScriptSerializer.serialize(this._samObject);
    },

    set_text: function (value) {
        this._text = value;
        if (this._input)
            this._input.value = value;
    },

    get_text: function () {
        return this._text;
    },

    set_samTime: function (value) {
        this._samTime = $HGDate.convertDate(value);
    },

    get_samTime: function () {
        return this._samTime;
    },

    set_samObject: function (value) {
        this._samObject = $HGDate.convertDate(value);
    },

    get_samObject: function () {
        return this._samObject;
    },

    set_inputStyle: function (value) {
        this._inputStyle = value;
    },

    get_inputStyle: function () {
        return this._inputStyle;
    },

    set_inputStyle2: function (value) {
        this._inputStyle2 = value;
    },

    get_inputStyle2: function () {
        return this._inputStyle2;
    },

    set_inputCssClass: function (value) {
        this._inputCssClass = value;
    },

    get_inputCssClass: function () {
        return this._inputCssClass;
    },

    set_alertImgUrl: function (value) {
        this._alertImgUrl = value;
    },

    get_alertImgUrl: function () {
        return this._alertImgUrl;
    },

    set_exportToWordUrl: function (value) {
        this._exportToWordUrl = value;
    },

    get_exportToWordUrl: function () {
        return this._exportToWordUrl;
    },

    add_callbackComplete: function (value) {
        this.get_events().addHandler(this._callbackCompleteEventKey, value);
    },

    remove_callbackComplete: function (value) {
        this.get_events().removeHandler(this._callbackCompleteEventKey, value);
    }
}

$HGRootNS.SampleControl.registerClass($HGRootNSName + ".SampleControl", $HGRootNS.ControlBase);
