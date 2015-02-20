/// <reference path="../Common/MicrosoftAjax.debug.js" />
/// <reference path="../../MCS.Web.Responsive.Library/Resources/ControlBase.js" />

// -------------------------------------------------
// Assembly	：	MCS.Web.WebControls
// FileName	：	AutoCompleteExtender.js
// Remark	：  自动完成的客户端脚本
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		张曦	    20070815		创建
// -------------------------------------------------

$HGRootNS.AutoCompleteExtender = function (element) {
    $HGRootNS.AutoCompleteExtender.initializeBase(this, [element]);
    this._isAutoComplete = true; //是否打开自动完成功能
    this._autoValidateOnChange = true;
    this._minimumPrefixLength = 3; //输入多少个字符后开始自动完成，默认为3
    this._completionInterval = 1000; //自动完成间隔。默认1000毫秒。输入停止后多长时间开始自动完成，单位：毫秒。1000毫秒=1秒
    this._completionBodyCssClass = ""; //自动完成部分的主体区域边框颜色
    this._itemFontColor = "#003399"; //自动完成的选择项目默认字体颜色(只作为默认值)
    this._completionBodyBorderColor = "#003399";
    this._itemHoverFontColor = "#003399"; //鼠标移动到自动完成的选项项目上时的字体颜色(只作为默认值)
    this._itemHoverBackgroundColor = "#FFE6A0"; //鼠标移动到自动完成的选项项目上时的背景色(只作为默认值)
    this._itemHoverCssClass = ""; //鼠标移动到自动完成的项目上的Style
    this._elementDefaultStyle = null; //保存当前输入框的风格，用来从出错后的风格还原
    this._requireValidation = false; //控件是否启用验证,如果启用则在输入内容无法完全匹配到数据源中的某一项
    this._maxCompletionRecordCount = -1; //控件自动完成出来的列表中显示的最大记录数量。默认为-1，表示全部数据	
    this._maxPopupWindowHeight = 260; //控件自动完成弹出的选择窗口的最大高度，默认为260px。
    this._AutoCompletePopup = null; //弹出窗口对象
    this._timer = null; //计时器
    this._focusHandler = null; //得到焦点的Handler
    this._blurHandler = null; //失去焦点的Handler
    this._keyDownHandler = null; //按下键盘的Handler
    this._keyUpHandler = null; //键盘弹起的Handler
    this._pasteHandler = null; //粘贴文本的Handler
    this._changeHandler = null; //文本改变的Handler
    this._tickHandler = null;
    this._autoCallBack = false; //是否自动回调页面
    this._selectIndex = -1; //当前选择的索引
    this._currentPrefix = ""; //当前的输入
    this._cache = null; //缓存输入的内容对应的结果
    this._enableCaching = true; //是否启用缓存
    this._dataList = null; //数据
    this._drawingList = null; //正在绘制的数据,这里的数据已经经过处理，直接使用
    this._dataTextFieldList = []; //提供文本显示的数据源属性集合
    this._compareFieldName = []; //在哪些字段中匹配输入的项目，只要有一个字段符合条件则认为该匹配成功
    this._dataValueField = ""; //制定那个字段为项目的Value值
    this._dataTextFormatString = ""; //显示内容的格式化字符串
    this._divHeight = ""; //弹出控件主区域的高度
    this._showFlag = false; //记录弹出窗口是否正在显示
    this._selectValue = ""; //选择的Value值
    this._text = ""; //选择或者输入的文本值
    this._originalText = ""; //Original Text in input
    this._errorCssClass = ""; //当启用输入验证，并且输入的内容无法完全匹配到数据源中的某一项后的CssClass
    this._itemCssClass = ""; //自动完成的项目CssClass
    this._elementY = 0; //控件的Y值
    this._tmpInnerHTML = "";
    this._eventContext = ""; //事件的上下文
    this._isTextChanged = false; //当前是否需要回调获取数据
    this._tickHandler = Function.createDelegate(this, this._onTimerTick);
    this._focusHandler = Function.createDelegate(this, this._onGotFocus); //得到焦点
    this._blurHandler = Function.createDelegate(this, this._onLostFocus); //失去焦点
    this._keyDownHandler = Function.createDelegate(this, this._onKeyDown); //按下键盘
    this._keyUpHandler = Function.createDelegate(this, this._setTextChanged); //键盘弹起
    this._pasteHandler = Function.createDelegate(this, this._setTextChanged); //粘贴文本
    this._propertyChangeHandler = Function.createDelegate(this, this._propertyChange); //Property Change

    this._beforeShow$delegate = Function.createDelegate(this, this._setSize); //弹出popup前
    this._timer = new $HGRootNS.Timer();
    this._isInvoking = false;

    this._waitingImage = "";
    this._checkImage = "";
    this._checkImageElement = null;
    this._waitingImageElement = null;

    this._imageContainer = null;

    this._showCheckImage = false;

    this._checkImageElementEvents =
    {
        click: Function.createDelegate(this, this._onCheckImageClick)
    };

    //选择项目的处理事件
    this._item$delegates =
    {
        mouseover: Function.createDelegate(this, this._item_onmouseover),
        click: Function.createDelegate(this, this._item_onclick)
    }
}

//---------------------------------------我就素那无耻的分割线---------------------------------------//

$HGRootNS.AutoCompleteExtender.prototype =
{
    _onCheckImageClick: function () {
        this._validate();
    },

    _canShowCheckImage: function () {
        var result = this.get_showCheckImage();

        if (result) {
            var elt = this.get_element();

            if (elt) {
                result = (!elt.readOnly && !elt.disabled) || (elt.contentEditable == true);
            }
        }

        return result;
    },

    get_showCheckImage: function () {
        return this._showCheckImage;
    },

    set_showCheckImage: function (value) {
        this._showCheckImage = value;

        if (this._imageContainer != null) {
            if (value)
                this._imageContainer.style.display = "inline";
            else
                this._imageContainer.style.display = "none";
        }
    },

    get_checkImage: function () {
        return this._checkImage;
    },

    set_checkImage: function (value) {
        this._checkImage = value;
    },

    get_waitingImage: function () {
        return this._waitingImage;
    },

    set_waitingImage: function (value) {
        this._waitingImage = value;
    },

    //是否打开自动完成功能
    get_isAutoComplete: function () {
        return this._isAutoComplete;
    },
    set_isAutoComplete: function (value) {
        this._isAutoComplete = value;
    },

    get_autoValidateOnChange: function () {
        return this._autoValidateOnChange;
    },

    set_autoValidateOnChange: function (value) {
        this._autoValidateOnChange = value;
    },

    //输入多少个字符后开始自动完成，默认为3
    get_minimumPrefixLength: function () {
        return this._minimumPrefixLength;
    },
    set_minimumPrefixLength: function (value) {
        this._minimumPrefixLength = value;
    },

    //自动完成间隔。默认1000毫秒。输入停止后多长时间开始自动完成，单位：毫秒。1000毫秒=1秒
    get_completionInterval: function () {
        return this._completionInterval;
    },
    set_completionInterval: function (value) {
        this._completionInterval = value;
    },

    //自动完成部分的主体区域边框颜色
    get_completionBodyBorderColor: function () {
        return this._completionBodyBorderColor;
    },
    set_completionBodyBorderColor: function (value) {
        this._completionBodyBorderColor = value;
    },

    //自动完成的选择项目默认字体颜色
    get_itemFontColor: function () {
        return this._itemFontColor;
    },
    set_itemFontColor: function (value) {
        this._itemFontColor = value;
    },

    //鼠标移动到自动完成的选项项目上时的字体颜色
    get_itemHoverFontColor: function () {
        return this._itemHoverFontColor;
    },
    set_itemHoverFontColor: function (value) {
        this._itemHoverFontColor = value;
    },

    //鼠标移动到自动完成的选项项目上时的背景色
    get_itemHoverBackgroundColor: function () {
        return this._itemHoverBackgroundColor;
    },
    set_itemHoverBackgroundColor: function (value) {
        this._itemHoverBackgroundColor = value;
    },

    //鼠标移动到自动完成的项目上的Style
    get_itemHoverCssClass: function () {
        return this._itemHoverCssClass;
    },
    set_itemHoverCssClass: function (value) {
        this._itemHoverCssClass = value;
    },

    //是否自动回调页面
    get_autoCallBack: function () {
        return this._autoCallBack;
    },
    set_autoCallBack: function (value) {
        this._autoCallBack = value;
    },

    //控件是否启用验证,如果启用则在输入内容无法完全匹配到数据源中的某一项
    get_requireValidation: function () {
        return this._requireValidation;
    },
    set_requireValidation: function (value) {
        this._requireValidation = value;
    },

    //控件自动完成出来的列表中显示的最大记录数量。默认为-1，表示全部数据
    get_maxCompletionRecordCount: function () {
        return this._maxCompletionRecordCount;
    },
    set_maxCompletionRecordCount: function (value) {
        this._maxCompletionRecordCount = value;
    },

    //控件自动完成弹出的选择窗口的最大高度，默认为260px。如果记录的内容小于等于这个值，
    //弹出窗口的高度自适应，如果大于这个值，则显示滚动条
    get_maxPopupWindowHeight: function () {
        return this._maxPopupWindowHeight;
    },
    set_maxPopupWindowHeight: function (value) {
        this._maxPopupWindowHeight = value;
    },

    //当前的输入
    get_currentPrefix: function () {
        return this._currentPrefix;
    },
    set_currentPrefix: function (value) {
        this._currentPrefix = value;
    },

    //是否启用缓存
    get_enableCaching: function () {
        return this._enableCaching;
    },
    set_enableCaching: function (value) {
        this._enableCaching = value;
    },

    //客户端数据源
    get_dataList: function () {
        return this._dataList;
    },
    set_dataList: function (value) {
        this._dataList = value;
    },

    //提供文本显示的数据源属性集合
    get_dataTextFieldList: function () {
        return this._dataTextFieldList;
    },
    set_dataTextFieldList: function (value) {
        this._dataTextFieldList = value;
    },

    //在哪些字段中匹配输入的项目，只要有一个字段符合条件则认为该匹配成功
    get_compareFieldName: function () {
        return this._compareFieldName;
    },
    set_compareFieldName: function (value) {
        this._compareFieldName = value;
    },

    //指定那个字段为项目的Value值
    get_dataValueField: function () {
        return this._dataValueField;
    },
    set_dataValueField: function (value) {
        this._dataValueField = value;
    },

    //显示内容的格式化字符串，如：姓名：{0}
    get_dataTextFormatString: function () {
        return this._dataTextFormatString;
    },
    set_dataTextFormatString: function (value) {
        this._dataTextFormatString = value;
    },

    //正在绘制的数据,这里的数据已经经过处理，直接使用
    get_drawingList: function () {
        return this._drawingList;
    },
    set_drawingList: function (value) {
        this._drawingList = value;
    },

    //记录弹出窗口是否处于可见状态
    get_showFlag: function () {
        return this._showFlag;
    },
    set_showFlag: function (value) {
        this._showFlag = value;
        if (!this._showFlag) {
            this._selectIndex = -1;
        }
    },

    //当启用输入验证，并且输入的内容无法完全匹配到数据源中的某一项后的CssClass
    get_errorCssClass: function () {
        return this._errorCssClass;
    },
    set_errorCssClass: function (value) {
        this._errorCssClass = value;
    },

    //自动完成的项目CssClass
    get_itemCssClass: function () {
        return this._itemCssClass;
    },
    set_itemCssClass: function (value) {
        this._itemCssClass = value;
    },

    //上下文
    get_eventContext: function () {
        return this._eventContext;
    },

    set_eventContext: function (value) {
        this._eventContext = value;
    },

    //是否正在调用
    get_isInvoking: function () {
        return this._isInvoking;
    },

    set_isInvoking: function (value) {
        this._isInvoking = value;
    },

    //回调时的上下文
    get_callBackContext: function () {
        return this._callBackContext;
    },

    set_callBackContext: function (value) {
        this._callBackContext = value;
    },


    //添加auto列表弹出时事件
    add_popShowing: function (handler) {
        this.get_events().addHandler("popShowing", handler);
    },

    //去掉auto列表弹出时事件
    remove_popShowing: function (handler) {
        this.get_events().removeHandler("popShowing", handler);
    },

    //触发auto列表弹出时事件
    raisepopShowing: function (items) {
        var handlers = this.get_events().getHandler("popShowing");

        if (handlers) {
            var e = new Sys.EventArgs();
            e.items = items;
            handlers(this, e);
        }
    },

    add_beforeInvoke: function (handler) {
        this.get_events().addHandler("beforeInvoke", handler);
    },

    remove_beforeInvoke: function (handler) {
        this.get_events().removeHandler("beforeInvoke", handler);
    },

    raiseBeforeInvoke: function () {
        var handlers = this.get_events().getHandler("beforeInvoke");

        if (handlers) {
            handlers(this, this);
        }
    },

    /// <summary>
    /// 控件初始化
    /// </summary>
    initialize: function () {
        $HGRootNS.AutoCompleteExtender.callBaseMethod(this, 'initialize');

        this.initializeTimer(this._timer); //初始化Timer
        this.initializeTextBox(this.get_element()); //初始化目标控件
        //this.initializeImages(element);
    },

    /// <summary>
    /// 控件销毁时执行的操作
    /// </summary>
    dispose: function () {
        var element = this.get_element();
        var blurEventName = (element.nodeName.toLowerCase() == "input" ? "change" : "blur");

        Sys.UI.DomEvent.removeHandler(element, "focus", this._focusHandler);
        Sys.UI.DomEvent.removeHandler(element, "keydown", this._keyDownHandler);
        Sys.UI.DomEvent.removeHandler(element, "keyup", this._keyUpHandler);
        Sys.UI.DomEvent.removeHandler(element, "paste", this._pasteHandler);
        Sys.UI.DomEvent.removeHandler(element, "propertychange", this._propertyChangeHandler);
        Sys.UI.DomEvent.removeHandler(element, blurEventName, this._blurHandler);

        if (this._timer) {
            this._timer.dispose();
            this._timer = null;
        }

        $HGRootNS.AutoCompleteExtender.callBaseMethod(this, "dispose");
    },

    /// <summary>
    /// 添加一个项目选择完毕的事件
    /// </summary>    
    add_itemSelected: function (handler) {
        this.get_events().addHandler("itemSelected", handler);
    },

    /// <summary>
    /// 移除一个项目选择完毕的事件
    /// </summary>
    remove_itemSelected: function (handler) {
        this.get_events().removeHandler("itemSelected", handler);
    },

    /// <summary>
    /// 选择一个项目完毕的事件
    /// </summary>
    _raiseItemSelected: function (id, object) {
        var handlers = this.get_events().getHandler("itemSelected");
        var continueExec = true;

        if (handlers) {
            var e = new Sys.EventArgs();

            e.id = id;
            e.selectedObject = object;
            e.cancel = false;
            e.textField = "a";
            e.valueField = "b";
            //e.eventElement = eventElement;            
            handlers(this, e);
            if (e.cancel)
                continueExec = false;
        }

        return continueExec;
    },

    add_valueValidated: function (handler) {
        this.get_events().addHandler("valueValidated", handler);
    },

    /// <summary>
    /// 移除一个项目选择完毕的事件
    /// </summary>
    remove_valueValidated: function (handler) {
        this.get_events().removeHandler("valueValidated", handler);
    },

    /// <summary>
    /// 选择一个项目完毕的事件
    /// </summary>
    _raiseValueValidated: function (id, dataList) {
        var handlers = this.get_events().getHandler("valueValidated");
        var continueExec = true;

        if (handlers) {
            var e = new Sys.EventArgs();

            e.id = id;

            if (!dataList || !dataList.length)
                dataList = [];

            e.dataList = dataList;
            e.cancel = false;

            handlers(this, e);
            if (e.cancel)
                continueExec = false;
        }

        return continueExec;
    },

    get_cloneableProperties: function () {
        var baseProperties = $HGRootNS.AutoCompleteExtender.callBaseMethod(this, "get_cloneableProperties");
        var currentProperties = ["isAutoComplete", "autoValidateOnChange", "minimumPrefixLength", "completionInterval",
			"itemHoverCssClass", "errorCssClass", "errorCssClass", "itemCssClass", "requireValidation", "eventContext",
			"maxCompletionRecordCount", "maxPopupWindowHeight", "autoCallBack", "enableCaching", "dataList",
			"dataTextFieldList", "compareFieldName", "dataValueField", "dataTextFormatString", "checkImage",
			"showCheckImage", "callBackContext"];

        Array.addRange(currentProperties, baseProperties);

        return currentProperties;
    },

    /// <summary>
    /// 初始化计时器
    /// </summary>
    initializeTimer: function (timer) {
        timer.set_interval(this._completionInterval);
        timer.add_tick(this._tickHandler);
    },

    /// <summary>
    /// 初始化文本框
    /// </summary>
    initializeTextBox: function (element) {
        element.autocomplete = "off";

        this.initializeTextBoxHandlers(element);
    },

    initializeTextBoxHandlers: function (element) {
        $addHandler(element, "focus", this._focusHandler);

        var blurEventName = (element.nodeName.toLowerCase() == "input" ? "change" : "blur");

        $addHandler(element, "keydown", this._keyDownHandler);
        $addHandler(element, "keyup", this._keyUpHandler);
        $addHandler(element, "paste", this._pasteHandler);
        $addHandler(element, "propertychange", this._propertyChangeHandler);
        $addHandler(element, blurEventName, this._blurHandler);
    },

    initializeImages: function (element) {
        this._imageContainer = document.createElement("span");

        element.insertAdjacentElement("afterEnd", this._imageContainer);

        if (this._canShowCheckImage())
            this._imageContainer.style.display = "inline";
        else
            this._imageContainer.style.display = "none";

        this._checkImageElement = document.createElement("img");
        this._checkImageElement.src = this._checkImage;
        this._checkImageElement.style.cursor = "pointer";
        this.initializeImagesHandlers();

        this._imageContainer.appendChild(this._checkImageElement);

        this._waitingImageElement = document.createElement("img");
        this._waitingImageElement.src = this._waitingImage;
        this._waitingImageElement.style.display = "none";

        this._imageContainer.appendChild(this._waitingImageElement);
    },

    initializeImagesHandlers: function () {
        $addHandlers(this._checkImageElement, this._checkImageElementEvents);
    },

    onBeforeCloneElement: function (sourceElement) {
        $clearHandlers(sourceElement);
        $clearHandlers(this._checkImageElement);
    },

    onAfterCloneElement: function (sourceElement, newElement) {
        this.initializeTextBoxHandlers(sourceElement);
        this.initializeImagesHandlers();
    },

    /// <summary>
    /// 当popup窗口打开时设置popup窗口的高度
    /// </summary>
    _setSize: function (popupwin, e) {
        if (e.height > this.get_maxPopupWindowHeight()) {
            this._divHeight = this.get_maxPopupWindowHeight();
            e.height = this.get_maxPopupWindowHeight();
            popupwin.get_popupDocument().body.document.getElementById('listPanl').style.height = this._divHeight;
        }
        else {
            this._divHeight = "";
        }

        this._elementY = 0;
        this._getElementTop(this.get_element());
        var eleY = window.screenTop + this._elementY + this.get_element().clientHeight + e.height - document.body.scrollTop;
        //alert(aa);
        if (eleY > window.screen.availHeight) {
            e.y = 0 - e.height;
        }
    },

    _getElementTop: function (value) {
        if (value != null) {
            this._elementY = this._elementY + value.offsetTop;
            this._getElementTop(value.offsetParent);
        }
    },

    /// <summary>
    /// 绘制选项列表
    /// </summary>
    _createList: function (listPanl) {
        var fsShowTextString = ""; //保存这个Item显示的具体内容,如果指定了格式，则匹配,否则直接输出
        var fsItemValue = ""; //保存Item的Value
        var arrayList, i, j;

        if (this.get_drawingList() && this.get_drawingList().length > 0) {
            if (typeof (this.get_drawingList()[0]) == "object") {//传过来的是一个对象
                for (i = 0; i < this.get_drawingList().length; i++) {//循环所有数据
                    var drawingObject = this.get_drawingList()[i];

                    if (this.get_dataTextFieldList() && this.get_dataTextFieldList().length > 0) {//指定了显示的数据字段
                        arrayList = new Array(this.get_dataTextFieldList().length); //定义数组保存制定字段的具体内容

                        fsShowTextString = ""; //初始化变量
                        fsItemValue = ""; //初始化变量

                        for (j = 0; j < this.get_dataTextFieldList().length; j++) {//循环指定的字段,并向上面定义的数组中赋值
                            arrayList[j] = drawingObject[this.get_dataTextFieldList()[j]];
                            fsShowTextString += drawingObject[this.get_dataTextFieldList()[j]];
                        }

                        if (this.get_dataValueField() && this.get_dataValueField() != "") {//如果指定了Value字段
                            fsItemValue = drawingObject[this.get_dataValueField()];
                        }

                        if (this.get_dataTextFormatString() && this.get_dataTextFormatString() != "") {//如果指定了格式，则匹配
                            Array.insert(arrayList, 0, this.get_dataTextFormatString());
                            fsShowTextString = String.format.apply(String, arrayList)//格式化字符串
                        }
                    }
                    else {//没有制定则显示全部数据
                        arrayList = new Array(drawingObject.length); //定义数组保存制定字段的具体内容

                        fsShowTextString = ""; //初始化变量
                        fsItemValue = ""; //初始化变量
                        for (var fsField in drawingObject) {//遍历dataList中的每一个字段
                            arrayList[i] = drawingObject[fsField];
                            fsShowTextString += drawingObject[fsField];
                        }

                        if (this.get_dataValueField() && this.get_dataValueField() != "") {//如果指定了Value字段
                            fsItemValue = drawingObject[this.get_dataValueField()];
                        }

                        if (this.get_dataTextFormatString() && this.get_dataTextFormatString() != "") {//如果指定了格式，则匹配
                            fsShowTextString = String.format(this.get_dataTextFormatString(), arrayList); //格式化字符串
                        }
                    }

                    var cssClass = "";

                    if (this._itemCssClass && this._itemCssClass != "") {
                        cssClass = this._itemCssClass;
                    }

                    this._createItem(fsShowTextString, fsItemValue, this._item$delegates, i, listPanl.childNodes[0], cssClass);
                }
            }
            else {//传过来的是一个数组
                //现在只处理字符串的一维数组
                for (i = 0; i < this.get_drawingList().length; i++) {
                    this._createItem(this.get_drawingList()[i], "", this._item$delegates, i, listPanl.childNodes[0], cssClass);
                }
            }
        }
    },


    /// <summary>
    /// 绘制选项列表的一个Item
    /// </summary>
    /// <param name="itemText">Item的显示文本</param>
    /// <param name="itemValue">Item的Value值</param>
    /// <param name="currentIndex">Item的位置索引值,只是Item在显示的内容中的索引值</param>
    /// <param name="parentElement">所属的列表对象（父对象）</param>
    _createItem: function (itemText, itemValue, itemEvents, currentIndex, parentElement, itemCssClass) {
        var itemLi = $HGDomElement.createElementFromTemplate(
        {
            nodeName: "li",
            events: itemEvents,
            cssClasses: [itemCssClass],
            properties:
                {
                    id: "li_Item_" + currentIndex,
                    indexValue: currentIndex, //这个属性非常酷的保存了每个Item的Index值
                    border: 1,
                    value: itemValue,
                    innerText: itemText,
                    title: itemText
                }
        }, parentElement);

        return itemLi;
    },

    /// <summary>
    /// 显示自动完成的Popup窗口
    /// </summary>
    _showCompletionList: function (prefixText, completionItems, cacheResults, width) {

        //如果使用缓存，则将结果保存到缓存
        if (cacheResults && this.get_enableCaching()) {
            if (!this._cache) {
                this._cache = {};
            }
            this._cache[prefixText] = completionItems;
        }
        this.raisepopShowing(completionItems);
        this.set_drawingList(completionItems); //设置绘制的数据

        var iWidth = this.get_element().clientWidth + 2;

        if (this._AutoCompletePopup == null) {
            this._AutoCompletePopup = document.createElement("div");
            this._AutoCompletePopup.className = "droplist";
            this._AutoCompletePopup.style.width = iWidth + "px";
            this._AutoCompletePopup.style.display = "none";

            var ul = document.createElement("ul");
            this._AutoCompletePopup.appendChild(ul);

            this.get_element().parentNode.appendChild(this._AutoCompletePopup);
        }

        this._AutoCompletePopup.firstChild.innerHTML = "";

        this._createList(this._AutoCompletePopup); //创建Popup中的具体内容

        this._showDropList(); //Show出来
        this._timer.set_enabled(false);
        this.set_showFlag(true); //窗口已经弹出
        if (this.get_element().nodeType === 1 && this.get_element().nodeName.toUpperCase() == "DIV") {
            //////this.get_element().innerHTML = this._tmpInnerHTML;
        }
        if (completionItems.length > 0)
            this._highlightItem(0);
    },

    _showDropList: function () {
        this._AutoCompletePopup.style.display = "block";
    },

    _hideDropList: function () {
        this._AutoCompletePopup.style.display = "none";
    },

    _innerTextFilter: function (sText) {
        return sText.substring(sText.lastIndexOf(';') + 1).trim();
    },

    _replaceLastInputText: function (sText) {
        //var sTmpText = this.get_element().innerText
        for (var n = this.get_element().firstChild; n; n = n.nextSibling) {
            if (n.nodeType === 3) {
                n.setAttribute("data-data", sText);
            }
        }

        //sTmpText = sTmpText.substring(0,sTmpText.lastIndexOf(';') + 1) + sText;
        //this.get_element().innerText = sTmpText;
    },

    /// <summary>
    /// 输入停止到时的处理
    /// </summary>
    _onTimerTick: function (sender, eventArgs) {
        if (this._isInvoking) return;

        //this._text = this.get_element().value;
        if (this.get_element().nodeType === 1) {
            this._text = this._innerTextFilter($HGDomElement.innerText(this.get_element()));
            this._tmpInnerHTML = this.get_element().innerHTML;
        }
        else {
            this._text = this.get_element().value;
        }

        if (this._text == "　")//不是空格，绝对不是
        {
            return;
        }

        if (this._isAutoComplete == false) {
            return;
        }

        //		if (this._AutoCompletePopup) {
        //			this._AutoCompletePopup.get_popupBody().innerHTML = "";
        //		}	//SZ Comment, 2010/7/5

        if (this._text.trim().length < this._minimumPrefixLength) {//输入的内容长度大于等于设定的长度则开始处理
            this._currentPrefix = null;
            this._hideCompletionList();
            return;
        }

        if (this._autoCallBack) {//是否存在回调的方法,存在则回调取数据
            if (this._originalText != this._text) {
                this._currentPrefix = this._text;

                try {
                    if (this._cache && this._cache[this._text]) {
                        this._showCompletionList(this._text, this._cache[this._text], /* cacheResults */false);

                        return;
                    }

                    this._invokeRemoteMethod(this._onMethodComplete);
                }
                finally {
                    this._isTextChanged = false;
                    this._originalText = this._text;
                }
            }
        }
        else if (this._dataList) {//不存在回调的方法则判断是否有客户端数据源，如果有则从客户端取数据
            this._currentPrefix = this._text;
            this._transactDataList();
        }
    },

    /// <summary>
    /// 处理客户端数据源中的数据
    ///     客户端数据源中包含全部的数据，需要根据 this._currentPrefix 中保存的
    ///     当前输入的前缀去匹配，从中筛选出符合条件的数据保存到 this._drawingList 
    ///     中，然后的处理跟invoke后台方法的一样了！NB吧！！
    /// </summary>
    _transactDataList: function () {
        var result, i, k, compareFlag, fiRecordCount = 0; //记录当前记录数量
        if (!this._dataList) {
            return;
        } //没有数据则返回
        if (typeof (this._dataList[0]) == "object") {//传过来的是一个对象
            if (this._dataTextFieldList.length < 1) {
                return;
            } //没有制定匹配字段则返回
            result = [];
            this._darwingList = null; //初始化待绘制数据列表

            for (i = 0; i < this._dataList.length; i++) {//循环全部的数据
                compareFlag = false; //是否匹配,匹配则添加到待绘制列表
                for (k = 0; k < this._compareFieldName.length; k++) {//循环各个字段
                    if (this._dataList[i][this._compareFieldName[k]].toString().indexOf(this._currentPrefix) == 0) {
                        compareFlag = true;
                        break;
                    }
                }
                if (compareFlag) {//如果该项目是匹配的项目则添加
                    fiRecordCount++;
                    if (this._maxCompletionRecordCount < 0 || fiRecordCount <= this._maxCompletionRecordCount)//在限定的数量范围内
                        Array.insert(result, result.length, this._dataList[i]);
                    else
                        break; //达到了最大数量，跳出循环
                }
            }

            if (result && result.length > 0) {
                this._showCompletionList(this._currentPrefix, result, /* cacheResults */true);
            }
        }
        else {//传来的就是一个数组
            result = [];
            this._darwingList = null; //初始化待绘制数据列表

            for (i = 0; i < this._dataList.length; i++) {//循环全部的数据
                compareFlag = false; //是否匹配,匹配则添加到待绘制列表
                if (this._dataList[i].indexOf(this._currentPrefix) == 0) {
                    compareFlag = true;
                }
                if (compareFlag) {//如果该项目是匹配的项目则添加
                    fiRecordCount++;
                    if (this._maxCompletionRecordCount < 0 || fiRecordCount <= this._maxCompletionRecordCount)//在限定的数量范围内
                        Array.insert(result, result.length, this._dataList[i]);
                    else
                        break;
                }
            }

            if (result && result.length > 0) {
                this._showCompletionList(this._currentPrefix, result, /* cacheResults */true);
            }
        }
    },

    /// <summary>
    /// 鼠标移动到选择项目上时的操作
    /// </summary>
    _item_onmouseover: function (e) {
        this._highlightItem(e.target.indexValue);
        this._selectIndex = e.target.indexValue;
        e.stopPropagation();
    },

    //当选择项目被点击的时候的操作
    _item_onclick: function (e) {
        this._selectItem(e.target);
    },

    _selectItem: function (itemElement) {
        var elt = this.get_element(); //得到输入框的对象 

        if (this._raiseItemSelected(itemElement.value, this._drawingList[itemElement.indexValue])) {

            //elt.value = itemElement.innerText;

            if (elt.nodeType === 1 && elt.nodeName.toUpperCase() == "DIV") {
                this._replaceLastInputText(itemElement.textContent || itemElement.innerText);
            }
            else {
                elt.value = itemElement.textContent || itemElement.innerText;
            }
        }

        if (elt.nodeType === 1 && elt.nodeName.toUpperCase() == "DIV") {
            this._text = this._innerTextFilter(elt.textContent || elt.innerText); //设置当前文本
        }
        else {
            this._text = elt.value; //设置当前文本
        }

        this._selectValue = itemElement.value; //设置当前的Value值
        this._selectIndex = itemElement.indexValue; //这个是索引值，只是在当前数据源中的

        this._originalText = this._text;
        this.goOnEnter = false;
        this._hideCompletionList();
    },

    /// <summary>
    /// 得到焦点时的处理
    /// </summary>
    _onGotFocus: function (ev) {
        this._timer.set_enabled(true);
    },

    /// <summary>
    /// 如果有滚动条，则设置位置以保证用远可以看到当前高亮的项目
    /// 他很无耻，我很无奈……
    /// </summary>
    _setScroll: function () {
        var item = this._AutoCompletePopup.get_popupDocument().body.document.all("div_Item_" + this._selectIndex); //得到当前选择的项目
        var listPanl = this._AutoCompletePopup.get_popupDocument().body.document.all('listPanl'); //得到当前的框架Div
        if (item && (item.offsetTop + item.offsetHeight) > (listPanl.scrollTop + this._maxPopupWindowHeight)) {//如果选择项目的下边界超出了当前显示的下边界,这个是向下选的时候
            listPanl.scrollTop += (item.offsetTop + item.offsetHeight) - (listPanl.scrollTop + this._maxPopupWindowHeight);
        }
        else if (item && item.offsetTop < listPanl.scrollTop) {//当选项的上边界超出当前显示的区域的上边界，这个是向上选择的处理
            listPanl.scrollTop -= listPanl.scrollTop - item.offsetTop;
        }
    },

    /// <summary>
    /// 设置高亮项目
    /// </summary>
    _highlightItem: function (newSelectIndex) {
        if (this.get_showFlag()) {//如果窗口有显示
            var items = this._AutoCompletePopup.firstChild.getElementsByTagName("li");
            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                Sys.UI.DomElement.removeCssClass(item, "droplist-selected");
            }

            Sys.UI.DomElement.addCssClass(items[newSelectIndex], "droplist-selected");

            //设置当前选择的索引值
            this._selectIndex = newSelectIndex;

            //this._setScroll(); //设置滚动条
        }
    },

    //回车，选择当前项目值
    _treatEnterAction: function (ev) {
        this.goOnEnter = true;
        ev.preventDefault();
        var needReturn = false; //是否需要出发默认的回车，如果在弹出选择的时候按回车为选择项目，没有弹出则默认的回车，该提交提交，该咋地咋地

        if (this.get_element().nodeType === 1 && this.get_element().nodeName.toUpperCase() == "DIV") {
            this._text = this._innerTextFilter(this.get_element().textContent || this.get_element().innerText);
        }
        else {
            this._text = this.get_element().value; //保存值
        }

        needReturn = !this._showFlag;
        if (this._selectIndex !== -1) {
            nowItem = this._AutoCompletePopup.firstChild.childNodes[this._selectIndex];

            this._selectItem(nowItem);
        }

        return needReturn;
    },

    _treatUpAction: function (ev) {
        //向上箭头,按下后下移一个项目，如果最后一个则回到第一个
        if (this.get_drawingList() != null && this.get_drawingList().length > 0) {
            var nowSelectIndex;

            if (this._selectIndex > 0) {
                nowSelectIndex = this._selectIndex - 1;
            }
            else {
                nowSelectIndex = this.get_drawingList().length - 1;
            }

            this._highlightItem(nowSelectIndex);
        }
    },

    _treatDownAction: function (ev) {
        //向下箭头,按下后上移一个项目，如果第一个则回到最后一个
        if (this.get_drawingList() != null && this.get_drawingList().length > 0) {
            var nowSelectIndex;

            if (this._selectIndex < (this.get_drawingList().length - 1)) {
                nowSelectIndex = this._selectIndex + 1;
            }
            else {
                nowSelectIndex = 0;
            }

            this._highlightItem(nowSelectIndex);
        }
    },

    /// <summary>
    /// 按下键盘的处理
    /// </summary>
    _onKeyDown: function (ev) {
        var key = ev.keyCode ? ev.keyCode : ev.rawEvent.keyCode;

        switch (key) {
            case Sys.UI.Key.esc:
                this._hideCompletionList();
                ev.preventDefault();
                break;
            case Sys.UI.Key.up:
                this._treatUpAction(ev);
                ev.preventDefault();
                break;
            case Sys.UI.Key.down:
                this._treatDownAction(ev);
                ev.preventDefault();
                break;
            case Sys.UI.Key.enter:
                ev.preventDefault();
                return this._treatEnterAction(ev);
            case Sys.UI.Key.left:
            case Sys.UI.Key.right:
                return;
            case Sys.UI.Key.backspace:
                return this._treatBackSpaceAction(ev);
            case Sys.UI.Key.del:
                return this._treatDelKeyAction(ev);
            default:
                this._hideCompletionList();
                Sys.UI.DomElement.removeCssClass(this.get_element(), this._errorCssClass);
                this._selectValue = ""; //在输入框中手工输入信息，清空当前的Value值
                this._timer.set_enabled(false);
                this._timer.set_enabled(true);
        }
    },

    _findParentNodeIsInputArea: function (node, inputArea) {
        while (node && n.parentNode != inputArea)
            node = node.parentNode;

        return node;
    },

    // 按下回退
    _treatBackSpaceAction: function (ev) {
        this._hideCompletionList();

        var inputArea = this.get_element();

        if (window.getSelection) {
            var sel = window.getSelection();
            if (ev.target == inputArea) {
                switch (sel.anchorNode.nodeType) {
                    case 1: //Element
                        if (sel.anchorNode == inputArea) {
                            //移除输入的文本
                            if (sel.anchorOffset > 0) {
                                sel.anchorNode.removeChild(sel.anchorNode.childNodes[sel.anchorOffset - 1]);
                            }
                            ev.preventDefault();
                        } else if (!!sel.anchorNode.getAttribute('data-id')) {
                            sel.anchorNode.parentNode.removeChild(sel.anchorNode);
                            ev.preventDefault();
                        }
                        break;
                    case 3: //Text
                        if (sel.anchorNode.parentNode == inputArea) {
                            //删除纯文本，自动
                        } else {
                            var n = this._findParentNodeIsInputArea(sel.anchorNode);

                            if (n)
                                n.parentNode.removeChild(n);    //删除下级节点的文本节点
                        }
                        break;
                    default:
                        ev.preventDefault();
                        break;
                }
            } else {
                var n = this._findParentNodeIsInputArea(sel.anchorNode);

                if (n)
                    n.parentNode.removeChild(n);
            }
        }
        else {
            for (n = inputArea.firstChild; n;) {
                if (n.nodeType === 1 && n.nodeName.toUpperCase() == 'DIV' && n.firstChild == null) {
                    var nn = n;
                    n = n.nextSibling;
                    nn.parentNode.removeChild(nn);
                    nn = null;
                } else {
                    n = n.nextSibling;
                }
            }
        }
    },

    _treatDelKeyAction: function (ev) {
        var n, nn;
        this._hideCompletionList();

        var inputArea = this.get_element();
        var nextElem;

        if (window.getSelection) {
            var sel = window.getSelection();

            if (ev.target == inputArea) {
                if (sel.anchorNode.nodeType === 1) {
                    if (sel.anchorNode == inputArea) {
                        //移除输入的文本

                        if (sel.anchorOffset > 0) {
                            var tNode = sel.anchorNode.childNodes[sel.anchorOffset];
                            if (tNode)
                                sel.anchorNode.removeChild(tNode);
                        } else if (sel.anchorNode.firstChild) {
                            sel.anchorNode.removeChild(sel.anchorNode.firstChild);
                        }
                        ev.preventDefault();
                    } else if (!!sel.anchorNode.getAttribute('data-id')) {
                        sel.anchorNode.parentNode.removeChild(sel.anchorNode);
                        ev.preventDefault();
                    }
                }
                else if (sel.anchorNode.nodeType == 3) {
                    if (sel.anchorNode.parentNode.nodeName.toUpperCase() == "A") {
                        sel.anchorNode.parentNode.parentNode.removeChild(sel.anchorNode.parentNode);
                        ev.preventDefault();
                    } else if (sel.anchorNode.parentNode == inputArea) {
                        //删除纯文本，自动
                    }
                } else {
                    ev.preventDefault();
                }
            } else {
                n = sel.anchorNode;
                while (n && n.parentNode != inputArea) {
                    n = n.parentNode;
                }

                if (n && n.parentNode == inputArea) {
                    n.parentNode.removeChild(n);
                }

                n = null;
            }
        }
        else {
            for (n = inputArea.firstChild; n;) {
                if (n.nodeType === 1 && n.nodeName.toUpperCase() == 'DIV' && n.firstChild == null) {
                    nn = n;
                    n = n.nextSibling;
                    nn.parentNode.removeChild(nn);
                    nn = null;
                } else {
                    n = n.nextSibling;
                }
            }
        }
    },

    /// <summary>
    /// 失去焦点时的处理
    /// </summary>
    _onLostFocus: function () {
        this._timer.set_enabled(false);

        window.setTimeout(Function.createDelegate(this, this._hideCompletionList), 200);
        //this._hideCompletionList();

        if (this.get_element().nodeName.toUpperCase() == "DIV") {
            this._text = this._innerTextFilter($HGDomElement.innerText(this.get_element()));
            this._currentPrefix = $HGDomElement.innerText(this.get_element());
        }
        else {
            this._text = this.get_element().value;
            this._currentPrefix = this.get_element().value;
        }

        //if (this._requireValidation && this._originalText != this._text) {//如果需要验证则执行验证方法
        if (this._requireValidation) {//如果需要验证则执行验证方法
            this._validate();
        }

        this._originalText = this._text;
    },

    applyTextChange: function () {
        this._text = "";
        var element = this.get_element();

        if (element.nodeName.toUpperCase() === "INPUT")
            this._text = element.value;
        else
            this._text = $HGDomElement.innerText(element);

        this._originalText = this._text;
    },

    /// <summary>
    /// 隐藏自动完成框
    /// </summary>
    _hideCompletionList: function () {
        if (this._AutoCompletePopup && this.get_showFlag()) {
            this._hideDropList();
        }
        this.set_showFlag(false);
    },

    /// <summary>
    /// Invoke调用成功后的处理
    /// </summary>
    _onMethodComplete: function (result, context, methodName) {
        this._isInvoking = false;
        //if (result != null && result.length > 0)
        if (result != null) {
            if (result.length > 0)
                this._showCompletionList(context, result, /* cacheResults */true);
            else
                this._validateInput(result);
        }

        //		this._waitingImageElement.style.display = "none";
        //		this._checkImageElement.style.display = "inline";
    },

    _onCallBackPageMethodError: function () {
        this._isInvoking = false;

        //		this._waitingImageElement.style.display = "none";
        //		this._checkImageElement.style.display = "inline";
    },

    /// <summary>
    /// 加载ClientState
    ///     ClientState中保存的是一个长度为2的一维数组
    ///         第一个为输入框中的文本
    ///         第二个为选中项目的Value，如果手工输入不是选择则为 空
    ///         第三个为DataList数据源
    /// </summary>
    /// <param name="clientState">序列化后的clientState</param>
    loadClientState: function (value) {
        if (value) {
            var elt = this.get_element(); //得到输入框的对象
            var fsArray = Sys.Serialization.JavaScriptSerializer.deserialize(value);

            if (fsArray && fsArray.length > 0) {
                this._text = fsArray[0];
                if (fsArray.length > 1 && fsArray[1]) {
                    this._selectValue = fsArray[1];
                }
                else {
                    this._selectValue = "";
                }

                if (fsArray.length > 2 && fsArray[2]) {
                    this._dataList = fsArray[2];
                }
                else {
                    this._dataList = null;
                }

                if (fsArray.length > 3 && fsArray[3]) {
                    this._eventContext = fsArray[3];
                }
                else {
                    this._eventContext = null;
                }
            }
            else {
                this._text = "";
                this._selectValue = "";
                this._dataList = null;
                this._eventContext = null;
            }

            //elt.value = this._text;
            //this._currentPrefix = elt.value;

            if (elt.nodeType === 1 && elt.nodeName.toUpperCase() == "DIV") {
                //                this._replaceLastInputText(this._text);
                //              this._currentPrefix = this._innerTextFilter(elt.innerText);
            }
            else {
                //elt.value = this._text;
                this._originalText = elt.value;
                this._text = this._originalText;
                this._currentPrefix = elt.value;
            }
        }
    },

    /// <summary>
    /// 保存ClientState
    ///     ClientState中保存的是一个长度为3的一维数组
    ///         第一个为输入框中的文本
    ///         第二个为选中项目的Value，如果手工输入不是选择则为 String.Empty
    ///         第三个为DataList数据源
    /// </summary>
    /// <returns>序列化后的CLientState字符串</returns>
    saveClientState: function () {
        //var fsCS = {this.get_selectIndex(), this.get_text()};
        var fsCS = new Array(3);
        fsCS[0] = this._text;
        fsCS[1] = this._selectValue;
        fsCS[2] = this._dataList;
        fsCS[3] = this._eventContext;
        return Sys.Serialization.JavaScriptSerializer.serialize(fsCS);
    },

    /// <summary>
    /// 验证输入的合法性
    /// 当输入信息而不是选择自动完成项目的时候执行此验证
    /// 在数据源中判断输入的内容是否在制定的验证字段中有完全匹配的数据，如果没有则为验证失败
    /// 如果有多个匹配项则只匹配到第一个。后面的自动忽略。并格式化文本框中的文本为制定的Text
    /// </summary>
    _validate: function () {
        if (this._isInvoking)
            return;

        var text = "";

        if (this.get_element().nodeName.toUpperCase() == "DIV") {
            text = this._innerTextFilter($HGDomElement.innerText(this.get_element()));
        }
        else {
            text = this.get_element().value;
        }

        if (this._autoCallBack) {//是否存在回调的方法,存在则回调取数据
            if (text != "" && this.get_autoValidateOnChange()) {
                this._invokeRemoteMethod(this._onValidateInvokeComplete);
            }
            else {
                this._validateInput([]);
            }
        }
        else if (this._dataList) {//不存在回调的方法则判断是否有客户端数据源，如果有则从客户端取数据
            this._validateInClientDataSource();
        }

        this._originalText = this._text;
    },

    set_context: function (context) {
        this._eventContext = context;
    },

    _invokeRemoteMethod: function (completeMethod) {
        this.raiseBeforeInvoke();

        this._staticInvoke("CallBackPageMethod", [this._currentPrefix, this._maxCompletionRecordCount],
					    Function.createDelegate(this, completeMethod),
					    Function.createDelegate(this, this._onCallBackPageMethodError));
        //this._invoke("CallBackPageMethod", [this._currentPrefix, this._maxCompletionRecordCount], Function.createDelegate(this, completeMethod),
        //					Function.createDelegate(this, this._onCallBackPageMethodError));

        this._isInvoking = true;
        //		this._waitingImageElement.style.display = "inline";
        //		this._checkImageElement.style.display = "none";
    },

    /// <summary>
    /// 验证数据时invoke服务端方法成功后的操作
    /// </summary>
    _onValidateInvokeComplete: function (result) {
        this._isInvoking = false;
        //		this._waitingImageElement.style.display = "none";
        //		this._checkImageElement.style.display = "inline";
        this._validateInput(result);
    },

    /// <summary>
    /// 从客户端数据源验证数据
    /// </summary>
    _validateInClientDataSource: function () {
        this._validateInput(this._dataList);
    },

    /// <summary>
    /// 验证输入的内容
    /// </summary>
    _validateInput: function (result) {
        var i, k;
        if (this._raiseValueValidated(this.get_element(), result)) {
            var typeError = true; //记录是否验证失败
            if (result && result.length > 0) {//如果得到了结果则开始处理
                if (typeof (result[0]) == "object") {//传过来的是一个对象
                    for (i = 0; i < result.length; i++) {//循环所有数据
                        for (k = 0; k < this._compareFieldName.length; k++)//循环所有的匹配字段
                        {
                            var fsShowTextString = ""; //显示的文本
                            if (this.get_dataTextFieldList() && this.get_dataTextFieldList().length > 0) {//指定了显示的字段，则显示指定内容
                                var arrayList = new Array(this.get_dataTextFieldList().length); //定义数组，保存字段的具体内容
                                for (var j = 0; j < this.get_dataTextFieldList().length; j++) {//循环指定的字段
                                    arrayList[j] = result[i][this.get_dataTextFieldList()[j]]; //数据数组，如果有格式用来进行格式化
                                    fsShowTextString += result[i][this.get_dataTextFieldList()[j]]; //先全拼上，如果有格式再覆盖，没有格式就可以直接输出了
                                }
                                if (this.get_dataTextFormatString() && this.get_dataTextFormatString() != "") {//如果指定了格式，则匹配
                                    Array.insert(arrayList, 0, this.get_dataTextFormatString());
                                    fsShowTextString = String.format.apply(String, arrayList)//格式化字符串
                                }
                            }
                            else {//没有指定显示的字段则为显示全部数据
                                for (var fsField in result[i]) {//遍历dataList中的每一个字段
                                    arrayList[i] = result[i][fsField];
                                    fsShowTextString += result[i][fsField];
                                }
                            }

                            if (result[i][this._compareFieldName[k]] == this._text || fsShowTextString == this._text) {//如果匹配到数据
                                typeError = false; //设定为没有错误

                                //this.get_element().value = fsShowTextString;//设置显示内容
                                if (this.get_element().nodeName.toUpperCase() === "DIV") {
                                    this._replaceLastInputText(fsShowTextString); //设置显示内容
                                }
                                else {
                                    this.get_element().value = fsShowTextString; //设置显示内容
                                }
                                this._text = fsShowTextString; //设置text
                                this._selectIndex = -1;
                                if (this.get_dataValueField() && this.get_dataValueField() != "") {//如果指定了Value字段
                                    this._selectValue = result[i][this.get_dataValueField()]; //设置value值
                                }
                                break; //跳出循环
                            }
                        }
                        //如果检测到了数据就是说没有错误，直接跳出
                        if (!typeError) {
                            break;
                        }
                    }
                }
                else {//传过来的是一个数组
                    //现在只处理字符串的一维数组
                    for (i = 0; i < result.length; i++) {
                        if (result[i] == this._text) {//找到了匹配项目
                            //this.get_element().value = result[i];
                            if (this.get_element().nodeName.toUpperCase() == "DIV") {
                                this._replaceLastInputText(result[i]);
                            }
                            else {
                                this.get_element().value = result[i];
                            }
                            this._text = result[i];
                            this._selectValue = "";
                            this._selectIndex = -1;
                            typeError = false;
                            break;
                        }
                    }
                }
            }

            if (typeError) {
                this._setErrorStyle(); //设置出错风格
                var elt = this.get_element();
                if (elt.onTypeError) {//如果文本框由onTypeError的JS代码，则执行
                    eval(elt.onTypeError);
                }
            }
        }
    },

    /// <summary>
    /// 设置出错风格
    /// </summary>
    _setErrorStyle: function () {
        Sys.UI.DomElement.addCssClass(this.get_element(), this._errorCssClass);
    },

    _propertyChange: function (e) {
        if (e.rawEvent.propertyName == "readOnly" || e.rawEvent.propertyName == "disabled")
            this.set_showCheckImage(!e.handlingElement[e.rawEvent.propertyName]);
    },

    //设置文本是否变化标示
    _setTextChanged: function (e) {
        this._currentPrefix = $HGDomElement.innerText(this.get_element());
        if (!this._isTextChanged) {
            if (this._text != this._currentPrefix) {
                this._isTextChanged = true;
            }
        }
    },

    RI: function () {
    }
}

$HGRootNS.AutoCompleteExtender.registerClass($HGRootNSName + ".AutoCompleteExtender", $HGRootNS.BehaviorBase);

$HGRootNS.AutoCompleteControl = function (element) {
    $HGRootNS.AutoCompleteControl.initializeBase(this, [element]);

    this._targetControlClientID = null;
    this._autoCompleteExtenderClientID = null;

    this._value = "";
    this._text = this.get_targetControlText();
}

$HGRootNS.AutoCompleteControl.prototype = {

    initialize: function () {
        $HGRootNS.AutoCompleteControl.callBaseMethod(this, "initialize");

        this.get_extender().add_itemSelected(Function.createDelegate(this, this._onItemSelected));
        this.get_extender().add_valueValidated(Function.createDelegate(this, this._onValidated));
    },

    dispose: function () {
        $HGRootNS.AutoCompleteControl.callBaseMethod(this, "dispose");
    },

    get_targetControlClientID: function () {
        return this._targetControlClientID;
    },

    set_targetControlClientID: function (value) {
        this._targetControlClientID = value;
    },

    get_autoCompleteExtenderClientID: function () {
        return this._autoCompleteExtenderClientID;
    },

    set_autoCompleteExtenderClientID: function (value) {
        this._autoCompleteExtenderClientID = value;
    },

    get_extender: function () {
        var result = null;

        if (this._autoCompleteExtenderClientID != null && this._autoCompleteExtenderClientID != "")
            result = $find(this._autoCompleteExtenderClientID);

        return result;
    },

    get_targetControl: function () {
        var result = null;

        if (this._targetControlClientID != null && this._targetControlClientID != "")
            result = $get(this._targetControlClientID);

        return result;
    },

    _onItemSelected: function (extender, e) {
        var selectedObject = e.selectedObject;

        this.set_selectedObject(selectedObject);

        this.raiseValueChanged(selectedObject);

        e.cancel = true;
    },

    _onValidated: function (extender, e) {
        var selectedObject = { Value: "", Text: "" };
        var raiseEvent = false;

        if (e.dataList.length > 0) {
            selectedObject = e.dataList[0];
            raiseEvent = true;
        }
        else {
            selectedObject.Value = this._value;
            selectedObject.Text = this._text;
        }

        this.set_selectedObject(selectedObject);

        if (raiseEvent)
            this.raiseValueChanged(selectedObject);

        e.cancel = true;
    },

    get_targetControlText: function () {
        var result = "";
        var targetControl = this.get_targetControl();

        if (targetControl) {
            if (targetControl.nodeName.toUpperCase() == "INPUT")
                result = targetControl.value;
            else
                result = $HGDomElement.innerText(targetControl);
        }

        return result;
    },

    set_targetControlText: function (value) {
        var targetControl = this.get_targetControl();

        if (targetControl) {
            if (targetControl.nodeName.toUpperCase() == "INPUT")
                targetControl.value = value;
            else
                if ("textContent" in targetControl)
                    targetControl.textContent = value;
                else
                    targetControl.innerText = value;
        }
    },

    get_value: function () {
        return this._value;
    },

    set_value: function (value) {
        this._value = value;
    },

    get_text: function () {
        return this._text;
    },

    set_text: function (value) {
        this._text = value;
    },

    set_selectedObject: function (selectedObject) {
        this.set_targetControlText(selectedObject.Text);
        this.get_extender()._selectValue = selectedObject.Value;

        this._value = selectedObject.Value;
        this._text = selectedObject.Text;
    },

    saveClientState: function () {
        var state = [this.get_value(), this.get_text()];

        return Sys.Serialization.JavaScriptSerializer.serialize(state);
    },

    add_valueChanged: function (handler) {
        this.get_events().addHandler("valueChanged", handler);
    },

    remove_valueChanged: function (handler) {
        this.get_events().removeHandler("valueChanged", handler);
    },

    raiseValueChanged: function (object) {
        var handlers = this.get_events().getHandler("valueChanged");

        if (handlers) {
            var e = new Sys.EventArgs();

            e.selectedObject = object;

            handlers(this, e);
        }
    },

    RI: function () {
    }
}

$HGRootNS.AutoCompleteControl.registerClass($HGRootNSName + ".AutoCompleteControl", $HGRootNS.ControlBase);

//AutoCompleteWithSelectorControlBase
$HGRootNS.AutoCompleteWithSelectorControlBase = function (element) {
    $HGRootNS.AutoCompleteWithSelectorControlBase.initializeBase(this, [element]);

    this._readOnly = false;
    this._disabled = false;

    this._tmpText = "";
    this._text = ""; //控件显示的文本，不包括验证过的
    this._className = null; //控件整体应用的CSS类名
    this._itemErrorCssClass = null; //输入错误的项目应用的CSS
    this._itemCssClass = null; //正常的已验证项目应用的CSS
    this._selectItemCssClass = null; //被选择的项目应用的CSS

    this._checkImg = null; //执行检查录入项目的图标
    this._selectorImg = null; //选择机构的图标
    this._userImg = null; //选择人员的图标
    this._ouUserImg = null; //可选择人员也可选择机构的图标
    this._hourglassImg = null;
    this._checkText = "检查...";
    this._checkingText = "正在检查";
    this._checkInputCallBackMethod = "";
    this._multiSelect = true;
    this._dataType = "";
    this._allowSelectDuplicateObj = false; //是否允许选择重复的人员
    this._dataDisplayPropName = "";
    this._dataDescriptionPropName = "";
    this._selectObjectDialogUrl = "";
    this._dataKeyName = "";

    this._autoCompleteID = ""; //auto控件ID
    this._autoCompleteControl = null;

    this._inputArea = null;
    this._chkUser = null;
    this._hourglass = null;
    this._ouBtn = null;

    this._inputAreaClientID = "";
    this._checkOguUserImageClientID = "";
    this._hourglassImageClientID = "";
    this._ouBtnClientID = "";

    this._callBackContext = null;

    this._showSelector = true;
    this._showCheckIcon = true;
    this._popupListWidth = "";

    this._selectedData = new Array(); //输入的，并且验证过的数据

    this._spanInputEvents = {
        drop: Function.createDelegate(this, this._canEvt),
        dragstart: Function.createDelegate(this, this._canEvt),
        contextmenu: Function.createDelegate(this, this._canEvt),
        paste: Function.createDelegate(this, this._paste),
        keydown: Function.createDelegate(this, this._onSpanInputKDRw),
        resize: Function.createDelegate(this, this._resize),
        keyup: Function.createDelegate(this, this._spanInputKeyUp)
    };

    this._chkUserEvents = {
        click: Function.createDelegate(this, this._checkInput)
    };
}

$HGRootNS.AutoCompleteWithSelectorControlBase.prototype = {
    initialize: function () {
        $HBRootNS.AutoCompleteWithSelectorControlBase.callBaseMethod(this, 'initialize');

        this._initElements();

        this._autoCompleteControl = $find(this._autoCompleteID);

        if (this._autoCompleteControl) {
            this._autoCompleteControl._container = this;
            this._autoCompleteControl.add_popShowing(Function.createDelegate(this, this._onAutoPopShowing));
            this._autoCompleteControl.add_itemSelected(Function.createDelegate(this, this._onPopupItemSelected));
        }

        this.setInputAreaText();
    },

    dispose: function () {
        this._selectedData = null;
        this._callBackContext = null;
        $HBRootNS.AutoCompleteWithSelectorControlBase.callBaseMethod(this, 'dispose');
    },

    _setButtonStatus: function () {
        if (this.get_enabled() == true && this.get_readOnly() == false) {
            if (this._inputArea != null) {
                this._inputArea.contentEditable = true;
                this._setItemsReadOnly(false);
            }

            if (this._chkUser != null)
                if (this.get_showCheckIcon()) {
                    this._chkUser.parentNode.style.display = "";
                }
                else {
                    this._chkUser.parentNode.style.display = "none";
                }
            if (this._ouBtn != null) {
                if (this.get_showSelector()) {
                    this._ouBtn.parentNode.style.display = "";
                } else {
                    this._ouBtn.parentNode.style.display = "none";
                }
            }
        }
        else {
            if (this._inputArea != null) {
                this._inputArea.contentEditable = false;
                this._inputArea.style.borderWidth = "0px";
                this._setItemsReadOnly(true);
            }

            if (this._chkUser != null)
                this._chkUser.parentNode.style.display = "none";

            if (this._ouBtn != null)
                this._ouBtn.parentNode.style.display = "none";
        }
    },

    _initElements: function () {
        this._inputArea = $get(this._inputAreaClientID);
        this._chkUser = $get(this._checkOguUserImageClientID);
        this._hourglass = $get(this._hourglassImageClientID);
        this._ouBtn = $get(this._ouBtnClientID);

        $addHandler(this._ouBtn, "click", Function.createDelegate(this, this._ouBtnClick));

        this._addHtmlElementHandlers();
        this._setButtonStatus();
    },

    _ouBtnClick: function () {
        this.raiseSelectData();
    },

    _clientBuildElements: function (container) {
        var row = this._buildTableAndRow(container);
        var cellInput = this._buildTableCell(row, "");

        var inputArea = this._buildInputArea(cellInput);
        if (container.id)
            inputArea.id = container.id + "_inputArea";

        var cellCheckUser = this._buildTableCell(row, "17px");

        var chkUser = this._buildCheckImage(cellCheckUser);
        chkUser.id = container.id + "_chkUser";

        var hourglassImage = this._buildHourglassImage(cellCheckUser);
        hourglassImage.id = container.id + "_hourglass";

        var cellBtn = this._buildTableCell(row, "17px");
        var ouBtn = this._buildOUButtonImage(cellBtn);
        ouBtn.id = container.id + "_lnkbtn";

        var properties = {
            inputAreaClientID: inputArea.id,
            checkOguUserImageClientID: chkUser.id,
            hourglassImageClientID: hourglassImage.id,
            ouBtnClientID: ouBtn.id
        };

        return properties;
    },

    _buildInputArea: function (cell) {
        var container = document.createElement("div");
        container.className = "aucocomplete-input-container form-control";
        cell.appendChild(container);

        var span = document.createElement("div");
        span.className = "autocomplete-input " + this.get_className();
        container.appendChild(span);

        if (this._readOnly === true) {
            span.style.borderWidth = "0px";
            span.className += " readonly";
        }

        return span;
    },

    _buildCheckImage: function (cell) {
        var img = document.createElement("img");
        img.src = this.get_checkImg();
        img.title = this.get_checkText();
        cell.appendChild(img);
        img.style.cursor = "pointer";

        return img;
    },

    _buildHourglassImage: function (cell) {
        var img = document.createElement("img");
        img.src = this.get_hourglassImg();
        img.title = this.get_checkingText();
        cell.appendChild(img);
        img.style.cursor = "pointer";
        img.style.display = "none";

        return img;
    },

    _buildOUButtonImage: function (cell) {
        var img = document.createElement("img");
        img.src = this.get_selectorImg();
        cell.appendChild(img);
        img.style.cursor = "pointer";

        return img;
    },

    _buildTableCell: function (row, widthDesp) {
        var cell = document.createElement("div");
        cell.className = "autocomplete-cell";
        cell.style.width = widthDesp;
        row.appendChild(cell);
        return cell;
    },

    _buildTableAndRow: function (container) {
        var table = document.createElement("div");
        table.className = "autocomplete-layout";
        container.appendChild(table);
        return table;
    },

    _addHtmlElementHandlers: function () {
        if (this._inputArea != null)
            $addHandlers(this._inputArea, this._spanInputEvents);

        if (this._chkUser != null)
            $addHandlers(this._chkUser, this._chkUserEvents);
    },

    focusInputArea: function () {
        spanInput = this._inputArea;
        spanInput.focus();
    },

    get_callBackContext: function () {
        return this._callBackContext;
    },

    set_callBackContext: function (value) {
        this._callBackContext = value;
        if (this._autoCompleteControl) {
            this._autoCompleteControl.set_context(value);
        }
    },

    set_context: function (context) {
        this.set_callBackContext(context);
    },
    get_context: function () {
        return this.get_callBackContext();
    },

    get_selectedData: function () {
        return this._selectedData;
    },

    set_selectedData: function (value) {
        this._selectedData = value;
    },

    clearData: function () {
        this._selectedData = [];
        this.setInputAreaText();
    },

    get_selectedSingleData: function () {
        var result = null;

        var data = this.get_selectedData();

        if (data.length > 0)
            result = data[0];

        return result;
    },

    set_selectedSingleData: function (value) {
        var data = [];

        if (value != null)
            data = [value];

        this.set_selectedData(data);
    },

    get_readOnly: function () {
        return this._readOnly;
    },

    set_readOnly: function (value) {
        this._readOnly = value;

        this._setButtonStatus();
    },

    _set_disabled: function (value) {
        this._disabled = value;
        this._element.disabled = value;
        this._setButtonStatus();
    },

    get_enabled: function () {
        return !this._disabled;
    },

    set_enabled: function (value) {
        this._set_disabled(value == false);
    },

    get_inputAreaClientID: function () {
        return this._inputAreaClientID;
    },

    set_inputAreaClientID: function (value) {
        this._inputAreaClientID = value;
    },

    get_checkOguUserImageClientID: function () {
        return this._checkOguUserImageClientID;
    },

    set_checkOguUserImageClientID: function (value) {
        this._checkOguUserImageClientID = value;
    },

    get_hourglassImageClientID: function () {
        return this._hourglassImageClientID;
    },

    set_hourglassImageClientID: function (value) {
        this._hourglassImageClientID = value;
    },

    get_ouBtnClientID: function () {
        return this._ouBtnClientID;
    },

    set_ouBtnClientID: function (value) {
        this._ouBtnClientID = value;
    },

    get_text: function () {
        return this._text;
    },

    set_text: function (value) {
        this._text = value;
    },

    get_className: function () {
        return this._className;
    },

    set_className: function (value) {
        this._className = value;
    },

    get_itemErrorCssClass: function () {
        return this._itemErrorCssClass;
    },

    set_itemErrorCssClass: function (value) {
        this._itemErrorCssClass = value;
    },

    get_itemCssClass: function () {
        return this._itemCssClass;
    },

    set_itemCssClass: function (value) {
        this._itemCssClass = value;
    },

    get_selectItemCssClass: function () {
        return this._selectItemCssClass;
    },

    set_selectItemCssClass: function (value) {
        this._selectItemCssClass = value;
    },

    get_checkImg: function () {
        return this._checkImg;
    },

    set_checkImg: function (value) {
        this._checkImg = value;
    },

    get_selectorImg: function () {
        return this._selectorImg;
    },

    set_selectorImg: function (value) {
        this._selectorImg = value;
    },

    get_userImg: function () {
        return this._userImg;
    },

    set_userImg: function (value) {
        this._userImg = value;
    },

    get_ouUserImg: function () {
        return this._ouUserImg;
    },

    set_ouUserImg: function (value) {
        this._ouUserImg = value;
    },

    get_showSelector: function () {
        return this._showSelector;
    },

    set_showSelector: function (value) {
        this._showSelector = value;
        this._setButtonStatus();
    },

    get_showCheckIcon: function () {
        return this._showCheckIcon;
    },

    set_showCheckIcon: function (value) {
        this._showCheckIcon = value;
    },

    get_popupListWidth: function () {
        return this._popupListWidth;
    },

    set_popupListWidth: function (value) {
        this._popupListWidth = value;
    },

    //auto控件ID
    get_autoCompleteID: function () {
        return this._autoCompleteID;
    },

    set_autoCompleteID: function (value) {
        this._autoCompleteID = value;
    },

    get_hourglassImg: function () {
        return this._hourglassImg;
    },

    set_hourglassImg: function (value) {
        this._hourglassImg = value;
    },

    get_checkText: function () {
        return this._checkText;
    },

    set_checkText: function (value) {
        this._checkText = value;
    },

    get_checkingText: function () {
        return this._checkingText;
    },

    set_checkingText: function (value) {
        this._checkingText = value;
    },

    get_checkInputCallBackMethod: function () {
        return this._checkInputCallBackMethod;
    },

    set_checkInputCallBackMethod: function (value) {
        this._checkInputCallBackMethod = value;
    },

    get_dataType: function () {
        return this._dataType;
    },

    set_dataType: function (value) {
        this._dataType = value;
    },

    get_multiSelect: function () {
        return this._multiSelect;
    },

    set_multiSelect: function (value) {
        this._multiSelect = value;
    },

    get_allowSelectDuplicateObj: function () {
        return this._allowSelectDuplicateObj;
    },
    set_allowSelectDuplicateObj: function (value) {
        this._allowSelectDuplicateObj = value;
    },

    get_dataKeyName: function () {
        return this._dataKeyName;
    },
    set_dataKeyName: function (value) {
        this._dataKeyName = value;
    },

    get_dataDisplayPropName: function () {
        return this._dataDisplayPropName;
    },
    set_dataDisplayPropName: function (value) {
        this._dataDisplayPropName = value;
    },

    get_dataDescriptionPropName: function () {
        return this._dataDescriptionPropName;
    },
    set_dataDescriptionPropName: function (value) {
        this._dataDescriptionPropName = value;
    },

    get_selectObjectDialogUrl: function () {
        return this._selectObjectDialogUrl;
    },

    set_selectObjectDialogUrl: function (value) {
        this._selectObjectDialogUrl = value;
    },

    _onAutoPopShowing: function (sender, e) {
    },

    _onPopupItemSelected: function (sender, e) {
        var n, nn, cc, i;
        e.cancel = true;

        //重新绘制列表
        var spanInput = $get(this.get_element().id + '_inputArea');
        if (!this._multiSelect) {
            //清除非文本节点？
            this.set_selectedData(new Array());
            for (n = spanInput.firstChild; n;) {
                if (n.nodeType === 1) {
                    nn = n;
                    n = n.nextSibling;
                    spanInput.removeChild(nn);
                } else {
                    n = n.nextSibling;
                }
            }
        }

        //将e.selectedObject加入_selectedOuUserData
        if (!this._checkDataInList(e.selectedObject))
            Array.add(this.get_selectedData(), e.selectedObject);

        var isIE = !!window.ActiveXObject || "ActiveXObject" in window;
        var isFirefox = navigator.userAgent.toLowerCase().indexOf("firefox") > -1 ? true : false;

        //this.setInputAreaText();

        //todo:aaaaaa

        i = 0;
        for (n = spanInput.firstChild; n;) {
            if (n.nodeType === 3) { //文本节点
                spanInput.focus();
                if (this.get_selectedData().length < i || typeof (this.get_selectedData()[i]) == 'undefined')
                    break;

                nn = this._createItemSpan(this.get_selectedData()[i++]);
                spanInput.insertBefore(nn, n);
                nn = n;
                n = n.nextSibling;
                spanInput.removeChild(nn);
            }
        }
        //        cc = spanInput.childNodes;
        //        for (var i = 0; i < cc.length; i++) {

        //            if (cc[i].nodeType == 3)//文字节点
        //            {
        //                spanInput.focus();

        //                if (this.get_selectedData().length < i || typeof (this.get_selectedData()[i]) == 'undefined')
        //                    break;

        //                var span = null;
        //                if (!isFirefox) {
        //                    span = this._createItemSpan(this.get_selectedData()[i]);
        //                } else {
        //                    var obj = this.get_selectedData()[i];
        //                    var spanID = this._get_ItemSpanID(obj);

        //                    if (typeof (spanID) != "undefined") {
        //                        span = this._createInnerItemSpan(obj, spanID);
        //                        spanInput.insertBefore(span, null);
        //                    }
        //                }
        //                spanInput.insertBefore(span, cc[i]);
        //                spanInput.removeChild(cc[i + 1]);

        //                if (!isIE && !isFirefox) {
        //                    if (i === this.get_selectedData().length - 1) {
        //                        var emptyNode = document.createTextNode(" ");
        //                        spanInput.insertBefore(emptyNode, null);
        //                    }
        //                }
        //                else if (isFirefox) {
        //                    if (i === this.get_selectedData().length - 1) {
        //                        var emptyNode = document.createElement("span");
        //                        emptyNode.style.display = "inline-table";
        //                        spanInput.insertBefore(emptyNode, null);
        //                    }
        //                }
        //            }
        //        }

        this.notifyDataChanged();
    },

    get_cloneableProperties: function () {
        var baseProperties = $HGRootNS.AutoCompleteWithSelectorControlBase.callBaseMethod(this, "get_cloneableProperties");
        var currentProperties = ["text", "callBackContext", "readOnly", "enabled", "className", "itemErrorCssClass",
				"itemCssClass", "selectItemCssClass", "showSelector", "showCheckIcon", "popupListWidth",
				"checkImg", "selectorImg", "hourglassImg", "checkText", "checkingText", "allowSelectDuplicateObj", "multiSelect", "selectedData",
            "dataKeyName", "dataDisplayPropName", "dataDescriptionPropName", "selectObjectDialogUrl", "checkInputCallBackMethod"];

        Array.addRange(currentProperties, baseProperties);

        return currentProperties;
    },

    _prepareCloneablePropertyValues: function (newElement) {
        var properties = $HGRootNS.AutoCompleteWithSelectorControlBase.callBaseMethod(this, "_prepareCloneablePropertyValues", [newElement]);

        var extendedProperties = this._clientBuildElements(newElement);

        for (var name in extendedProperties) {
            properties[name] = extendedProperties[name];
        }

        var autoComplete = this._autoCompleteControl.cloneAndAppendToContainer(newElement, $get(properties["inputAreaClientID"]));

        autoComplete.get_events()._list["itemSelected"] = undefined;
        autoComplete.get_events()._list["popShowing"] = undefined;

        properties["autoCompleteID"] = autoComplete.get_element().id + "$AutoCompleteExtender";

        return properties;
    },

    _canEvt: function (e) {
        var evt = window.event || e;
        evt.returnValue = false;
        evt.cancelBubble = true;
    },

    _paste: function (e) {
        var isFirefox = navigator.userAgent.toLowerCase().indexOf("firefox") > -1 ? true : false;
        if (isFirefox) {
            e.preventDefault();
        }
        window.setTimeout(Function.createDelegate(this, this._afterPaste), 150);
    },

    _afterPaste: function () {
        var changed = false, subSpan, obj;
        this.set_selectedData(new Array());
        var spanInput = this._inputArea;
        for (var i = 0; i < spanInput.childNodes.length; i++) {
            var span = spanInput.childNodes[i];
            if (span.nodeName.toUpperCase() == "DIV") {
                if ((subSpan = span.firstChild) && subSpan.nodeType == 1) {
                    if (subSpan.nodeName.toUpperCase() == "DIV") {
                        if (subSpan.getAttribute("data-data") == undefined || subSpan.getAttribute("data-data") == "") {
                            //							spanInput.removeChild(span);
                            //							i--;
                            continue;
                        }

                        obj = this._convertPropertyStrToObject(subSpan.getAttribute("data-data"));

                        if (this.get_dataType() != "" && obj._dataType != this.get_dataType()) {
                            //							spanInput.removeChild(span);
                            //							i--;
                            continue;
                        }

                        if (this._checkInMask(obj)) {
                            if (this._checkDataInList(obj)) {
                                spanInput.removeChild(span);
                                i--;
                            }
                            else {//各种校验通过
                                subSpan.onclick = function () {
                                    var sel = window.getSelection();
                                    sel.selectAllChildren(subSpan);
                                };
                                Array.add(this.get_selectedData(), obj);
                                changed = true;
                            }
                        }
                    } else if (span.getAttribute("data-data")) {//sub is #text
                        obj = this._convertPropertyStrToObject(span.getAttribute("data-data"));

                        if (this.get_dataType() != "" && obj._dataType != this.get_dataType()) {
                            continue;
                        }

                        if (this._checkInMask(obj)) {
                            if (this._checkDataInList(obj)) {
                                spanInput.removeChild(span);
                                i--;
                            }
                            else {//各种校验通过
                                spanInput.removeChild(span);
                                var aSpan = this._createItemSpan(obj);
                                spanInput.insertBefore(aSpan, spanInput.childNodes[i + 1]);
                                aSpan.onclick = function () {
                                    var sel = window.getSelection();
                                    sel.selectAllChildren(span);
                                };
                                Array.add(this.get_selectedData(), obj);
                                changed = true;
                            }
                        }
                    }
                }
            }
        }

        if (changed) {
            this.notifyDataChanged();
        }
    },

    _checkInMask: function (obj) {
        throw Error.create("必须重写_checkInMask方法。");
    },

    _resize: function () {
        //var inputArea = $find(this.get_element().id).findElement("inputArea"); //得到输入框控件
        var inputArea = $get(this._inputAreaClientID);

        if (inputArea.offsetHeight >= 50)//三行高度
        {
            inputArea.style.height = 50;
            inputArea.style.overflow = "auto";
        }
    },

    _onSpanInputKDRw: function (ev) {
        var iKC = ev.keyCode ? ev.keyCode : ev.rawEvent.keyCode;

        if (this._disabled == true)//只读
        {
            try {
                ev.keyCode = 0;
            }
            catch (e) {
            }
            finally {
                ev.returnValue = false;
            }
        }
        else {

            switch (iKC) {
                case 13: //回车
                    this._canEvt(ev);

                    if (this._autoCompleteControl.get_showFlag() === false && this._autoCompleteControl.goOnEnter === true)
                        this._checkInput();

                    break;
                case 186: //分号
                case 59: // Firefox
                    this._checkInput();
                    ev.keyCode = 35;
                    break;
                default:
                    break;
            }
        }
    },

    _checkInput: function () {
        var spanInput = this._inputArea, n, hasSpan = false;

        var displayPropName = this.get_dataDisplayPropName();

        if (spanInput != null) {
            var sTmpText = $HGDomElement.innerText(spanInput);

            for (n = this._inputArea.firstChild; n; n = n.nextSibling) {
                if (n.nodeType == 1 && n.nodeName.toUpperCase() == "DIV") {
                    hasSpan = true;
                    n = null;
                    break;
                }
            }

            if (hasSpan) {
                for (var i = 0; i < this.get_selectedData().length; i++) {
                    sTmpText = sTmpText.replace(this.get_selectedData()[i][displayPropName] + ";", "");
                }
            }

            if (sTmpText.length) {
                this._validateInput(sTmpText);
                this._tmpText = sTmpText;
            }
        }

        return false;
    },

    //回掉后台方法对输入的信息进行验证
    _validateInput: function (sText) {
        var methodName = this.get_checkInputCallBackMethod();
        if (methodName == "") {
            return;
        }

        if (this._autoCompleteControl) {
            if (this._autoCompleteControl.get_isInvoking()) {
                return;
            }
        }

        this._staticInvoke(methodName,
			[sText, this.get_context()],
			Function.createDelegate(this, this._onValidateInvokeComplete),
			Function.createDelegate(this, this._onValidateInvokeError));

        this._setInvokingStatus(true);

        if (this._autoCompleteControl) {
            this._autoCompleteControl.set_isInvoking(true);
        }
    },

    _setInvokingStatus: function (value) {
        if (value) {
            this._chkUser.style.display = "none";
            this._hourglass.style.display = "inline";
        }
        else {
            this._chkUser.style.display = "inline";
            this._hourglass.style.display = "none";
        }
    },

    _onValidateInvokeError: function (e) {
        this._setInvokingStatus(false);
    },

    //回掉后台验证成功后调用这个进行处理，如果结果为1个则，直接创建SPAN并显示，多个则弹出对话框让用户进行选择
    _onValidateInvokeComplete: function (result) {
        var objPicker = null;
        this._setInvokingStatus(false);

        if (this._autoCompleteControl) {
            this._autoCompleteControl.set_isInvoking(false);
        }

        var obj = null;

        if (result) {
            if (result.length > 1)//多于一个
            {
                //				var sFeature = "dialogWidth:500px; dialogHeight:300px;center:yes;help:no;resizable:yes;scroll:no;status:no";

                //				result.nameTable = $NT;
                //				result.keyName = this.get_dataKeyName();
                //				result.displayPropName = this.get_dataDisplayPropName();
                //				result.descriptionPropName = this.get_dataDescriptionPropName();

                //				var resultStr = window.showModalDialog(this.get_selectObjectDialogUrl(), result, sFeature);

                obj = null; //result[resultStr];



                var options = {
                    title: "请选择数据",
                    width: "500px",
                    height: "300px",
                    onOk: Function.createDelegate(this, function (args) {

                        var id = $('a.active', objPicker).attr('data-id');
                        if (id) {
                            for (var i = result.length - 1; i >= 0; i--) {
                                if (this._get_ItemSpanID(result[i]) == id) {
                                    obj = result[i];
                                    break;
                                }
                            }
                        }

                        if (obj != null) {
                            this._tmpText = "";

                            if (this._multiSelect == false) {
                                this.set_selectedData(new Array());
                            }

                            if (this._allowSelectDuplicateObj || !this._checkDataInList(obj))
                                Array.add(this.get_selectedData(), obj);

                            this.notifyDataChanged();
                        } else {
                            args.canceled = true;
                        }

                        this.setInputAreaText();

                        this._inputArea.focus();

                    }),
                    onCancel: Function.createDelegate(this, function () {
                        this._inputArea.focus();
                    }),
                    okBtn: {
                        visible: true,
                        text: "确定"
                    },
                    cancelBtn: {
                        visible: true,
                        text: "取消"
                    }
                };

                var box = $HGModalBox.show(options, "");

                objPicker = this._buildObjectPicker(box, result);

            }
            else {
                obj = result[0];

                this._tmpText = "";

                if (this._multiSelect == false) {
                    this.set_selectedData(new Array());
                }

                if (this._allowSelectDuplicateObj || !this._checkDataInList(obj))
                    Array.add(this.get_selectedData(), obj);

                this.notifyDataChanged();
                this.setInputAreaText();
            }
        }
    },

    _buildObjectPicker: function (box, options) {
        var container = box.widgetbody;
        var div = document.createElement("div");
        div.className = "container";
        container.appendChild(div);
        box.okBtn.disabled = true;

        var list = document.createElement("div");
        list.className = "list-group";
        div.appendChild(list);
        list = $(list);

        var that = this;

        list.on('click', 'a', function () {
            $('a.active', list).toggleClass("active");
            $(this).toggleClass("active");
            box.okBtn.disabled = false;
        }).on('dblclick', 'a', function () {
            $('a.active', list).toggleClass("active");
            $(this).toggleClass("active");
            box.okBtn.disabled = false;
            $(box.okBtn).trigger('click');
        }).on('keypress', function (e) {
            debugger;
        }).focus();

        $(options).each(function () {
            var li = document.createElement('a');
            li.href = "javascript:void(0);";
            li.className = "list-group-item";
            list.append(li);
            li.setAttribute('data-id', that._get_ItemSpanID(this));
            li.appendChild(document.createTextNode(that._get_ItemDescriptionText(this)));
            li = document.createElement("input");
        });

        return list;
    },

    add_selectedDataChanged: function (handler) {
        this.get_events().addHandler("selectedDataChanged", handler);
    },

    remove_selectedDataChanged: function (handler) {
        this.get_events().removeHandler("selectedDataChanged", handler);
    },

    raiseSelectedDataChanged: function (selectedOuUserData) {
        var handlers = this.get_events().getHandler("selectedDataChanged");

        if (handlers)
            handlers(selectedOuUserData);
    },

    notifyDataChanged: function () {
        this.raiseSelectedDataChanged(this.get_selectedData());
    },

    add_selectData: function (handler) {
        this.get_events().addHandler("selectData", handler);
    },

    remove_selectData: function (handler) {
        this.get_events().removeHandler("selectData", handler);
    },

    raiseSelectData: function (selectedData) {
        var handler = this.get_events().getHandler("selectData");

        if (handler) {
            var e = new Sys.EventArgs();
            e.Data = selectedData;
            handler(this, e);

            if (e.resultValue && e.resultValue != "") {
                var data = Sys.Serialization.JavaScriptSerializer.deserialize(e.resultValue);
                this._transactSelectData(data);
            }
        }
    },

    _transactSelectData: function (objs) {
        var i;
        if (objs instanceof Array) {

            var thisControl = $get(this._inputAreaClientID); //$find(this.get_element().id).findElement("inputArea"); //得到输入框控件

            //像输入框中拼文本信息，如果现有信息最后没有分号则加入一个分号
            var text = $HGDomElement.innerText(thisControl);
            if (text.trim() != "") {
                if (text.substring(text.length - 1) != ";") {
                    $HGDomElement.innerText(thisControl, text + ";");
                }
            }

            //组织机构人员控件已确认选择的内容，如果为null则先初始化
            if (this.get_selectedData() == null) {
                this.set_selectedData(new Array());
            }

            if (!this._multiSelect) {
                this.set_selectedData(new Array());
            }

            //循环选择的内容，添加现在文本框中不包含的内容
            for (i = 0; i < objs.length; i++) {
                if (!this._checkDataInList(objs[i])) {
                    Array.add(this.get_selectedData(), objs[i]);
                }
            }

            var dataKeyName = this.get_dataKeyName();

            //循环文本框中的数据，删除在树中去掉的内容
            for (i = 0; i < this.get_selectedData().length; i++) {
                var flag = true;
                for (var j = 0; j < objs.length; j++) {
                    if (this.get_selectedData()[i][dataKeyName] == objs[j][dataKeyName]) {
                        flag = false;
                        break;
                    }
                }

                if (flag) {
                    Array.remove(this.get_selectedData(), this.get_selectedData()[i]);
                    i--;
                }
            }

            this._tmpText = '';
            this.setInputAreaText();
            this.notifyDataChanged();
        }
    },

    _checkDataInList: function (obj) {
        var keyName = this.get_dataKeyName();
        if (obj[keyName]) {
            var sid = obj[keyName];
            var blnResult = false;
            var datas = this.get_selectedData();
            if (datas.length > 0 && datas[0]) {
                for (var i = 0; i < datas.length; i++) {
                    if (datas[i][keyName] == sid) {
                        blnResult = true;
                        break;
                    }
                }
            }
            return blnResult;
        }
        return true;
    },

    _onCheckInputOuUserError: function () {
        if (this._autoCompleteControl) {
            this._autoCompleteControl.set_isInvoking(false);
        }
    },

    _createItemSpan: function (obj) {
        var span = null;
        var spanID = this._get_ItemSpanID(obj);

        if (obj && typeof (spanID) != "undefined")
            span = this._createItemSpanWithID(obj, spanID);

        return span;
    },

    _get_ItemSpanID: function (obj) {
        throw Error.create("必须重写_get_ItemSpanID方法提供spanID");
    },

    _get_ItemDescriptionText: function (obj) {
        var rst = obj.description;
        if (!rst) {
            throw Error.create("必须重写_get_ItemDescriptionText方法提供DescriptionText");
        }
        return rst;
    },

    _createItemSpanWithID: function (obj, spanID) {
        var spanInput = this._inputArea;

        var newSpan = document.createElement("div");

        this._beforeCreateItemSpanWithID(obj, newSpan);

        newSpan.contentEditable = false;
        newSpan.tabIndex = -1;
        newSpan.setAttribute('data-id', spanID);
        newSpan.className = "rwNRR ac-parsed-item";
        newSpan.setAttribute('data-role', 'parseditem');

        var aSpan = this._createInnerItemSpan(obj, spanID);

        newSpan.insertBefore(aSpan, null);
        spanInput.insertBefore(newSpan, null);

        return newSpan;
    },

    _createInnerItemSpan: function (dataObj, spanID) {
        var aSpan = document.createElement("span");

        aSpan.className = "rwRR ac-parsed-item-inner";
        aSpan.tabIndex = -1;
        aSpan.setAttribute('data-id', spanID);
        aSpan.contentEditable = false;

        //onclick = "var obj = event.srcElement;var oRng = document.body.createTextRange();oRng.moveToElementText(obj);oRng.select();";
        //oncontextmenu = "var obj = event.srcElement;var oRng = document.body.createTextRange();oRng.moveToElementText(obj);oRng.select();";
        aSpan.ondblclick = this._canEvt;
        aSpan.oncontrolselect = this._canEvt;


        var inputArea = this._inputArea;
        aSpan.onclick = function (e) {
            //			var evt = window.event || e;
            //			var obj = evt.target || evt.srcElement;
            //			var oRng = document.body.createTextRange();
            //			oRng.moveToElementText(obj);
            //			oRng.select();
            window.setTimeout(function () {
                var sel = window.getSelection();
                sel.selectAllChildren(aSpan);
            }, 10);

        };

        $addHandler(aSpan, "keydown", Function.createDelegate(this, this._onItemKeyDown));

        aSpan.oncontextmenu = function () {
            var sel = window.getSelection();
            sel.selectAllChildren(aSpan);
            return false;
        };

        this._fillItemSpanAttributes(dataObj, aSpan);

        return aSpan;

    },

    _onItemKeyDown: function (ev) {
        if (this.get_readOnly() === false && this.get_enabled() === true) {
            var k = ev.keyCode ? ev.keyCode : ev.rawEvent.keyCode;
            if (k === Sys.UI.Key.backspace || k === Sys.UI.Key.del) {
                var isIE = !!window.ActiveXObject || "ActiveXObject" in window;
                var isFirefox = navigator.userAgent.toLowerCase().indexOf("firefox") > -1 ? true : false;

                var element = ev.target;
                if (!isIE && !isFirefox) {
                    element.parentNode.parentNode.removeChild(element.parentNode);
                    this._inputArea.focus();
                    ev.returnValue = false;
                    ev.cancelBubble = true;
                    ev.preventDefault();
                }
                else if (isFirefox) {
                    element.parentNode.removeChild(element);
                    if (this._inputArea.firstChild && this._inputArea.firstChild.nodeName.toUpperCase() == "DIV" && this._inputArea.firstChild.firstChild == null) {
                        this._inputArea.removeChild(this._inputArea.firstChild);
                    }
                    this._inputArea.focus();
                    ev.returnValue = false;
                    ev.cancelBubble = true;
                    ev.preventDefault();
                }
            }
        }
    },

    //创建名称的span的内容之前。可以插入图标之类的元素
    _beforeCreateItemSpanWithID: function (obj, container) {
    },

    _convertPropertyStrToObject: function (propertyStr) {
        var jsonStr = propertyStr.replace(/\$\\/g, "\"");
        var obj = Sys.Serialization.JavaScriptSerializer.deserialize(jsonStr);
        return obj;
    },

    _convertObjectToPropertyStr: function (object) {
        object._dataType = this.get_dataType();
        var jsonStr = Sys.Serialization.JavaScriptSerializer.serialize(object);
        var regex = new RegExp("\"", "gm");
        return jsonStr.replace(regex, "$\\");
    },

    //填充每一个输入项中的Span相关的属性
    _fillItemSpanAttributes: function (obj, aSpan) {
        throw Error.create("必须重载_fillItemSpanAttributes方法");
    },

    /// <summary>
    /// 根据_selectedData中的数据设置显示的文本。
    /// </summary>
    // 旧版
    //    setInputAreaText: function () {
    //        spanInput = this._inputArea;

    //        var isIE = !!window.ActiveXObject || "ActiveXObject" in window;
    //        var isFirefox = navigator.userAgent.toLowerCase().indexOf("firefox") > -1 ? true : false;

    //        if (spanInput) {
    //            spanInput.innerHTML = "";

    //            if (null != this.get_selectedData()) {   //如果选中并验证的信息中有数据
    //                for (var i = 0; i < this.get_selectedData().length; i++) {
    //                    if (this._isDataObject(this.get_selectedData()[i])) {
    //                        if (!isFirefox) {
    //                            var span = this._createItemSpan(this.get_selectedData()[i]);
    //                            if (span) {
    //                                spanInput.insertBefore(span, null);

    //                                try { document.selection.createRange().text = ""; }
    //                                catch (err) { }
    //                            }
    //                        } else {
    //                            var obj = this.get_selectedData()[i];
    //                            var spanID = this._get_ItemSpanID(obj);

    //                            if (typeof (spanID) != "undefined") {
    //                                span = this._createInnerItemSpan(obj, spanID);
    //                                spanInput.insertBefore(span, null);
    //                            }
    //                        }

    //                        if (!isIE && !isFirefox) {
    //                            if (i === this.get_selectedData().length - 1) {
    //                                var emptyNode = document.createTextNode(" ");
    //                                spanInput.insertBefore(emptyNode, null);
    //                            }
    //                        }
    //                        else if (isFirefox) {
    //                            if (i === this.get_selectedData().length - 1) {
    //                                var emptyNode = document.createElement("span");
    //                                emptyNode.style.display = "inline-table";
    //                                spanInput.insertBefore(emptyNode, null);
    //                            }
    //                        }

    //                    }
    //                }
    //            }

    //            if (this._tmpText.length > 0) {
    //                var a = document.createElement("a");
    //                spanInput.appendChild(a);
    //                a.appendChild(document.createTextNode(this._tmpText));
    //            }
    //        }
    //    },
    // 新版：
    setInputAreaText: function () {
        var fc, i, n;
        var selectedData = this.get_selectedData();
        if (this._inputArea) {
            fc = this._inputArea.firstChild;
            ; // this._inputArea.innerHTML = " ";

            if (null != selectedData) {   //如果选中并验证的信息中有数据
                for (i = 0; i < selectedData.length; i++) {
                    if (this._isDataObject(selectedData[i])) {

                        var span = this._createItemSpan(this.get_selectedData()[i]);
                        if (span) {
                            this._inputArea.insertBefore(span, fc);

                            if (document.selection) {
                                try { document.selection.createRange().text = ""; }
                                catch (err) { }
                            }
                        }
                        //                        } else {
                        //                            var obj = this.get_selectedData()[i];
                        //                            var spanID = this._get_ItemSpanID(obj);

                        //                            if (typeof (spanID) != "undefined") {
                        //                                span = this._createInnerItemSpan(obj, spanID);
                        //                                spanInput.insertBefore(span, null);
                        //                            }
                        //                        }

                        //                        if (!isIE && !isFirefox) {
                        //                            if (i === this.get_selectedData().length - 1) {
                        //                                var emptyNode = document.createTextNode(" ");
                        //                                spanInput.insertBefore(emptyNode, null);
                        //                            }
                        //                        }
                        //                        else if (isFirefox) {
                        //                            if (i === this.get_selectedData().length - 1) {
                        //                                var emptyNode = document.createElement("span");
                        //                                emptyNode.style.display = "inline-table";
                        //                                spanInput.insertBefore(emptyNode, null);
                        //                            }
                        //                        }

                    }
                }
            }

            if (this._tmpText.length > 0) {
                var a = document.createElement("a");
                this._inputArea.insertBefore(a, fc);
                a.appendChild(document.createTextNode(this._tmpText));
            }

            while (fc) {
                n = fc;
                fc = fc.nextSibling;
                this._inputArea.removeChild(n);
            }

            n = null;
        }
    },

    _setItemsReadOnly: function (value) {
        for (var n = this._inputArea.firstChild; n; n = n.nextSibling) {
            if (n.nodeType === 1 && n.nodeName.toUpperCase() === "DIV") {
                var subSpan = n.firstChild;
                if (subSpan && subSpan.nodeType === 1)
                    subSpan.contentEditable = !value;
            }
        }
    },

    _isDataObject: function (obj) {
        var keyName = this.get_dataKeyName();
        if (obj && obj[keyName]) {
            return true;
        }
        return false;
    },

    _spanInputKeyUp: function () {
        var n, nn, i, k, spanInput = this._inputArea;
        //var spanInput = document.all(this.get_element().id + '_inputArea');
        //        var isFirefox = navigator.userAgent.toLowerCase().indexOf("firefox") > -1 ? true : false;
        //        var spanInput = $get(this._inputAreaClientID);


        var sArrIDList = new Array();

        for (n = spanInput.firstChild; n; n = n.nextSibling) {
            if (n.nodeType === 1 && n.nodeName.toUpperCase() === "DIV") {
                if (!!n.getAttribute('data-id')) { // 有内容的span
                    sArrIDList.push(n.getAttribute('data-id'));
                }
            }
        }

        var sTmpList = this.get_selectedData();
        var dataChanged = false;

        this.set_selectedData(new Array());

        var keyName = this.get_dataKeyName();

        if (sArrIDList.length > 0) {
            for (i = 0; i < sTmpList.length; i++) {
                for (k = 0; k < sArrIDList.length; k++) {
                    if (sTmpList[i][keyName] == sArrIDList[k]) {
                        Array.add(this.get_selectedData(), sTmpList[i]);
                        break;
                    }
                }
            }
        }

        dataChanged = this.get_selectedData().length != sTmpList.length;

        //循环selectedData,如果selectedData中的数据已经不再span中，则移出
        if (this.get_selectedData() != null) {
            for (i = 0; i < this.get_selectedData().length; i++) {
                var itemInSpan = false; //是否存在于Span中
                for (n = spanInput.firstChild; n; n = n.nextSibling) {
                    if (n.nodeType === 1 && n.nodeName.toUpperCase() === "DIV") {
                        var key = n.getAttribute('data-id')

                        if (this.get_selectedData()[i][keyName] == key) {
                            itemInSpan = true;
                            n = null;
                            break;
                        }
                    }
                }

                if (!itemInSpan)//如果不在SPAN中，则移出
                {
                    Array.remove(this.get_selectedData(), this.get_selectedData()[i]);
                    dataChanged = true;
                    i--;
                }
            }
        }

        if (dataChanged)
            this.notifyDataChanged();
    },

    pseudo: function () {
    }
}

$HGRootNS.AutoCompleteWithSelectorControlBase.registerClass($HGRootNSName + ".AutoCompleteWithSelectorControlBase", $HGRootNS.ControlBase);