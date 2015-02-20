
//var thisID = "";
$HBRootNS.OuUserInputControl = function (element) {
	$HBRootNS.OuUserInputControl.initializeBase(this, [element]);
	//属性定义
	this._thisID = "";
	this._readOnly = false; //控件是否只读
	this._disabled = false; //控件是否禁用
	this._canSelectRoot = true; //是否可以选择根节点
	this._showSideLine = true; //是否显示兼职人员  
	this._multiSelect = true; //是否可以多选  Single:0  单选   Multiple:1  多选
	this._text = ""; //控件显示的文本，不包括验证过的
	this._className = null; //控件整体应用的CSS类名
	this._itemErrorCssClass = null; //输入错误的项目应用的CSS
	this._itemCssClass = null; //正常的已验证项目应用的CSS
	this._selectItemCssClass = null; //被选择的项目应用的CSS
	this._ouUserDataList = null; //OU，User的数据
	this._selectedOuUserData = new Array(); //输入的，并且验证过的OU，User的数据
	this._checkUserImg = null; //执行检察录入项目的图标
	this._ouImg = null; //选择机构的图标
	this._userImg = null; //选择人员的图标
	this._ouUserImg = null; //可选择人员也可选择机构的图标
	this._propertyDialogUrl = null; //显示属性的对话框地址
	this._selectObjectDialogUrl = null; //选择对象的对话框地址
	this._mouseSelectItemId = ""; //鼠标选择的对象ID
	this._tmpText = "";
	this._autocompleteID = ""; //auto控件ID
	this._autocompleteControl = null;
	this._userougraphcontrolID = '';
	this._allowSelectDuplicateObj = false; //是否允许选择重复的人员

	this._checkOguUserImage = null;
	this._checkOguUserImageClientID = ""; //检查对象合法性的图片的客户端ID
	this._hourGlassImage = null;
	this._hourGlassImageClientID = ""; //HourGlass图片的客户端ID

	this._showTreeSelector = true;

	this._transactDialogData$delegate = Function.createDelegate(this, this._transactDialogData);
	this._onMouseSelectItem$delegate = Function.createDelegate(this, this._onMouseSelectItem);

	this._checkSpanEvents =
    {
    	click: Function.createDelegate(this, this._onCheckSpanClick)
    };

	this._spanInputEvents =
    {
    	drop: Function.createDelegate(this, this._canEvt),
    	dragstart: Function.createDelegate(this, this._canEvt),
    	contextmenu: Function.createDelegate(this, this._canEvt),
    	paste: Function.createDelegate(this, this._canEvt),
    	keydown: Function.createDelegate(this, this._onSpanInputKDRw),
    	resize: Function.createDelegate(this, this._resize),
    	keyup: Function.createDelegate(this, this._spanInputKeyUp)
    };
}

$HBRootNS.OuUserInputControl.prototype =
{
    initialize: function () {
        $HBRootNS.OuUserInputControl.callBaseMethod(this, 'initialize');
        this._thisID = this.get_element().id;
        $find(this.get_element().id + "_" + this.get_element().id).add_dialogConfirmed(this._transactDialogData$delegate);
        this._buildControl();
        this._autocompleteControl = $find(this._autocompleteID);

        if (this._autocompleteControl) {
            this._autocompleteControl.add_popShowing(Function.createDelegate(this, this._onautopopShowing));
            this._autocompleteControl.add_itemSelected(Function.createDelegate(this, this._onPopupItemSelected));
        }

        this.setInputAreaText();

        this._checkOguUserImage = $get(this._checkOguUserImageClientID);
        this._hourGlassImage = $get(this._hourGlassImageClientID);
    },

    dispose: function () {
        $HBRootNS.OuUserInputControl.callBaseMethod(this, 'dispose');
    },

    /// <summary>
    /// Focus input area.
    /// </summary>
    focusInputArea: function () {
        spanInput = document.all(this.get_element().id + '_inputArea');
        spanInput.focus();
    },

    _onPopupItemSelected: function (autoControl, e) {
    },

    /// <summary>
    /// 根据_selectedOuUserData中的数据设置显示的文本。
    /// </summary>
    setInputAreaText: function () {
        spanInput = document.all(this.get_element().id + '_inputArea');

        spanInput.innerHTML = "";
        if (null != this.get_selectedOuUserData())//如果选中并验证的信息中有数据
        {
            for (var i = 0; i < this.get_selectedOuUserData().length; i++) {
                if (this.get_selectedOuUserData()[i] && this.get_selectedOuUserData()[i].id) {
                    var span = this._createUserSpan(this.get_selectedOuUserData()[i].displayName + ";",
							 "spn_" + this.get_selectedOuUserData()[i].id,
							 this.get_selectedOuUserData()[i].fullPath);

                    spanInput.insertBefore(span);

                    try {
                        document.selection.createRange().text = "";
                    }
                    catch (err)
					{ }
                }
            }
        }

        if (this._tmpText.length > 0) {
            var a = document.createElement("a");
            a.innerHTML = this._tmpText;
            spanInput.appendChild(a);
        }

        $find(this.get_element().id + "_" + this.get_element().id)._dialogResult = this.get_selectedOuUserData();
    },

    _resize: function () {
        var thisControl = $find(this._thisID).findElement("inputArea"); //得到输入框控件

        if (thisControl.offsetHeight >= 50)//三行高度
        {
            thisControl.style.height = 50;
            thisControl.style.overflow = "auto";
        }
    },

    /// <summary>
    /// 处理从对话框选择过来的信息
    /// </summary>
    _transactDialogData: function (sender) {
        var objs = sender.get_selectedObjects(); //得到在对话框中勾选的组织机构人员
        var thisControl = $find(this._thisID).findElement("inputArea"); //得到输入框控件

        //像输入框中拼文本信息，如果现有信息最后没有分号则加入一个分号
        if (thisControl.innerText.trim() != "") {
            if (thisControl.innerText.substring(thisControl.innerText.length - 1) != ";") {
                thisControl.innerText += ";";
            }
        }

        //组织机构人员控件已确认选择的内容，如果为null则先初始化
        if (this.get_selectedOuUserData() == null) {
            this.set_selectedOuUserData(new Array());
        }

        if (!this._multiSelect) {
            this.set_selectedOuUserData(new Array());
        }

        //循环选择的内容，添加现在文本框中不包含的内容
        for (var i = 0; i < objs.length; i++) {
            if (!this._checkUserInList(objs[i].id)) {
                Array.add(this.get_selectedOuUserData(), objs[i]);
            }
        }

        //循环文本框中的数据，删除在树中去掉的内容
        for (var i = 0; i < this.get_selectedOuUserData().length; i++) {
            var flag = true;
            for (var j = 0; j < objs.length; j++) {
                if (this.get_selectedOuUserData()[i].id == objs[j].id) {
                    flag = false;
                    break;
                }
            }

            if (flag) {
                Array.remove(this.get_selectedOuUserData(), this.get_selectedOuUserData()[i]);
                i--;
            }
        }

        this._tmpText = '';
        this.setInputAreaText();

        this.notifyDataChanged();
    },

    _buildControl: function () {
        var elt = this.get_element();
        var spanInput = document.all(this.get_element().id + '_inputArea');
        $addHandlers(spanInput, this._spanInputEvents);
    },

    get_allowSelectDuplicateObj: function () {
        return this._allowSelectDuplicateObj;
    },

    set_allowSelectDuplicateObj: function (value) {
        this._allowSelectDuplicateObj = value;
    },

    /// <summary>
    /// 控件是否只读
    ///   获取this._readOnly的值
    /// </summary>
    get_readOnly: function () {
        return this._readOnly;
    },
    /// <summary>
    /// 控件是否只读
    ///   设置this._readOnly的值
    /// </summary>
    /// <param name="value">要设置的值</param>
    set_readOnly: function (value) {
        this._readOnly = value;

        if (value == true) {
            document.all(this.get_element().id + "_chkUser").style.display = "none";
            //document.all(this.get_element().id + "_lnkbtn").style.display = "none";
            document.all(this.get_element().id + '_inputArea').contentEditable = "false";
        }
        else {
            document.all(this.get_element().id + "_chkUser").style.display = "";
            //document.all(this.get_element().id + "_lnkbtn").style.display = "";
            document.all(this.get_element().id + '_inputArea').contentEditable = "true";
        }

        this.set_ouSelectBtnStyle();
    },

    /// <summary>
    /// 控件是否禁用
    ///   获取this._disabled的值
    /// </summary>
    get_disabled: function () {
        return this._disabled;
    },

    /// <summary>
    /// 控件是否禁用
    ///   设置this._disabled的值
    /// </summary>
    /// <param name="value">要设置的值</param>
    set_disabled: function (value) {
        if (value == true) {
            if (document.all(this.get_element().id + "_chkUser")) {
                document.all(this.get_element().id + "_chkUser").style.display = "none";
            }
            //document.all(this.get_element().id + "_lnkbtn").style.display = "none";
            document.all(this.get_element().id + '_inputArea').contentEditable = "false";
        }
        else {
            if (document.all(this.get_element().id + "_chkUser")) {
                document.all(this.get_element().id + "_chkUser").style.display = "";
            }
            //document.all(this.get_element().id + "_lnkbtn").style.display = "";
            document.all(this.get_element().id + '_inputArea').contentEditable = "true";
        }

        this._disabled = value;

        this.set_ouSelectBtnStyle();
    },

    get_showTreeSelector: function () {
        return this._showTreeSelector;
    },

    set_showTreeSelector: function (value) {
        this._showTreeSelector = value;
        this.set_ouSelectBtnStyle();
    },

    /// <summary>
    /// 控件是否可用
    /// </summary>
    get_enabled: function () {
        return !this._disabled;
    },

    /// <summary>
    /// 控件是否可用
    ///   设置this._enabled的值
    /// </summary>
    /// <param name="value">控件是否可用</param>
    set_enabled: function (value) {
        this.set_disabled(value == false);
    },

    get_checkOguUserImageClientID: function () {
        return this._checkOguUserImageClientID;
    },

    set_checkOguUserImageClientID: function (value) {
        this._checkOguUserImageClientID = value;
    },

    get_hourGlassImageClientID: function () {
        return this._hourGlassImageClientID;
    },

    set_hourGlassImageClientID: function (value) {
        this._hourGlassImageClientID = value;
    },

    /// <summary>
    /// 是否可以选择根节点
    ///   获取this._canSelectRoot的值
    /// </summary>
    get_canSelectRoot: function () {
        return this._canSelectRoot;
    },
    /// <summary>
    /// 是否可以选择根节点
    ///   设置this._canSelectRoot的值
    /// </summary>
    /// <param name="value">要设置的值</param>
    set_canSelectRoot: function (value) {
        this._canSelectRoot = value;
    },

    /// <summary>
    /// 是否显示兼职人员
    ///   获取this._showSideLine的值
    /// </summary>
    get_showSideLine: function () {
        return this._showSideLine;
    },
    /// <summary>
    /// 是否显示兼职人员
    ///   设置this._showSideLine的值
    /// </summary>
    /// <param name="value">要设置的值</param>
    set_showSideLine: function (value) {
        this._showSideLine = value;
    },

    /// <summary>
    /// 是否可以多选  Single:0  单选   Multiple:1  多选
    ///   获取this._multiSelect的值
    /// </summary>
    get_multiSelect: function () {
        return this._multiSelect;
    },
    /// <summary>
    /// 是否可以多选  Single:0  单选   Multiple:1  多选
    ///   设置this._multiSelect的值
    /// </summary>
    /// <param name="value">要设置的值</param>
    set_multiSelect: function (value) {
        this._multiSelect = value;
    },

    /// <summary>
    /// 控件显示的文本
    ///   获取this._text的值
    /// </summary>
    get_text: function () {
        return this._text;
    },
    /// <summary>
    /// 控件显示的文本
    ///   设置this._text的值
    /// </summary>
    /// <param name="value">要设置的值</param>
    set_text: function (value) {
        this._text = value;
    },

    /// <summary>
    /// 控件整体应用的CSS类名
    ///   获取this._className的值
    /// </summary>
    get_className: function () {
        return this._className;
    },
    /// <summary>
    /// 控件整体应用的CSS类名
    ///   设置this._className的值
    /// </summary>
    /// <param name="value">要设置的值</param>
    set_className: function (value) {
        this._className = value;
    },

    /// <summary>
    /// 输入错误的项目应用的CSS
    ///   获取this._itemErrorCssClass的值
    /// </summary>
    get_itemErrorCssClass: function () {
        return this._itemErrorCssClass;
    },
    /// <summary>
    /// 输入错误的项目应用的CSS
    ///   设置this._itemErrorCssClass的值
    /// </summary>
    /// <param name="value">要设置的值</param>
    set_itemErrorCssClass: function (value) {
        this._itemErrorCssClass = value;
    },

    /// <summary>
    /// 正常的已验证项目应用的CSS
    ///   获取this._itemCssClass的值
    /// </summary>
    get_itemCssClass: function () {
        return this._itemCssClass;
    },
    /// <summary>
    /// 正常的已验证项目应用的CSS
    ///   设置this._itemCssClass的值
    /// </summary>
    /// <param name="value">要设置的值</param>
    set_itemCssClass: function (value) {
        this._itemCssClass = value;
    },

    /// <summary>
    /// 被选择的项目应用的CSS
    ///   获取this._selectItemCssClass的值
    /// </summary>
    get_selectItemCssClass: function () {
        return this._selectItemCssClass;
    },
    /// <summary>
    /// 被选择的项目应用的CSS
    ///   设置this._selectItemCssClass的值
    /// </summary>
    /// <param name="value">要设置的值</param>
    set_selectItemCssClass: function (value) {
        this._selectItemCssClass = value;
    },

    /// <summary>
    /// OU，User的数据
    ///   获取this._ouUserDataList的值
    /// </summary>
    get_ouUserDataList: function () {
        return this._ouUserDataList;
    },
    /// <summary>
    /// OU，User的数据
    ///   设置this._ouUserDataList的值
    /// </summary>
    /// <param name="value">要设置的值</param>
    set_ouUserDataList: function (value) {
        this._ouUserDataList = value;
    },

    /// <summary>
    /// 执行检察录入项目的图标
    ///   获取this._checkUserImg的值
    /// </summary>
    get_checkUserImg: function () {
        return this._checkUserImg;
    },
    /// <summary>
    /// 执行检察录入项目的图标
    ///   设置this._checkUserImg的值
    /// </summary>
    /// <param name="value">要设置的值</param>
    set_checkUserImg: function (value) {
        this._checkUserImg = value;
    },

    /// <summary>
    /// 选择机构的图标
    ///   获取this._ouImg的值
    /// </summary>
    get_ouImg: function () {
        return this._ouImg;
    },
    /// <summary>
    /// 选择机构的图标
    ///   设置this._ouImg的值
    /// </summary>
    /// <param name="value">要设置的值</param>
    set_ouImg: function (value) {
        this._ouImg = value;
    },

    /// <summary>
    /// 选择人员的图标
    ///   获取this._userImg的值
    /// </summary>
    get_userImg: function () {
        return this._userImg;
    },
    /// <summary>
    /// 选择人员的图标
    ///   设置this._userImg的值
    /// </summary>
    /// <param name="value">要设置的值</param>
    set_userImg: function (value) {
        this._userImg = value;
    },

    /// <summary>
    /// 可选择人员也可选择机构的图标
    ///   获取this._ouUserImg的值
    /// </summary>
    get_ouUserImg: function () {
        return this._ouUserImg;
    },
    /// <summary>
    /// 可选择人员也可选择机构的图标
    ///   设置this._ouUserImg的值
    /// </summary>
    /// <param name="value">要设置的值</param>
    set_ouUserImg: function (value) {
        this._ouUserImg = value;
    },

    get_propertyDialogUrl: function () {
        return this._propertyDialogUrl;
    },

    set_propertyDialogUrl: function (value) {
        this._propertyDialogUrl = value;
    },

    get_selectObjectDialogUrl: function () {
        return this._selectObjectDialogUrl;
    },

    set_selectObjectDialogUrl: function (value) {
        this._selectObjectDialogUrl = value;
    },

    get_selectedSingleData: function () {
        var result = null;

        var data = this.get_selectedOuUserData();

        if (data.length > 0)
            result = data[0];

        return result;
    },

    get_selectedOuUserData: function () {
        return this._selectedOuUserData;
    },

    set_selectedOuUserData: function (value) {
        this._selectedOuUserData = value;
    },

    //auto控件ID
    get_autocompleteID: function () {
        return this._autocompleteID;
    },

    set_autocompleteID: function (value) {
        this._autocompleteID = value;
    },
    //userougraphcontrolID控件ID
    get_userougraphcontrolID: function () {
        return this._userougraphcontrolID;
    },

    set_userougraphcontrolID: function (value) {
        this._userougraphcontrolID = value;
    },

    //回掉后台方法对输入的信息进行验证
    _validateInput: function (sText) {
        if (this._autocompleteControl) {
            if (this._autocompleteControl.get_isInvoking()) {
                return;
            }
        }
        /*
        this._invoke("CheckInputOuUser",
        [sText],
        Function.createDelegate(this, this._onValidateInvokeComplete),
        Function.createDelegate(this, this._onValidateInvokeError));
        */
        this._staticInvoke("CheckInputOuUser",
			[sText],
			Function.createDelegate(this, this._onValidateInvokeComplete),
			Function.createDelegate(this, this._onValidateInvokeError));
        this._setInvokingStatus(true);

        if (this._autocompleteControl) {
            this._autocompleteControl.set_isInvoking(true);
        }
    },

    _setInvokingStatus: function (value) {
        if (value) {
            this._checkOguUserImage.style.display = "none";
            this._hourGlassImage.style.display = "inline";
        }
        else {
            this._checkOguUserImage.style.display = "inline";
            this._hourGlassImage.style.display = "none";
        }
    },

    _onValidateInvokeError: function (e) {
        this._setInvokingStatus(false);
    },

    //回掉后台验证成功后调用这个进行处理，如果结果为1个则，直接创建SPAN并显示，多个则弹出对话框让用户进行选择
    _onValidateInvokeComplete: function (result) {
        this._setInvokingStatus(false);

        if (this._autocompleteControl) {
            this._autocompleteControl.set_isInvoking(false);
        }
        var obj = null;
        if (result.length > 1)//多于一个
        {
            var sAllText = "";
            for (var i = 0; i < result.length; i++) {
                sAllText += "," + result[i].fullPath;
            }
            if (sAllText.length > 0) {
                sAllText = sAllText.substring(1);
            }

            var sFeature = "dialogWidth:500px; dialogHeight:300px;center:yes;help:no;resizable:yes;scroll:no;status:no";

            result.nameTable = $NT;
            var resultStr = window.showModalDialog(this.get_selectObjectDialogUrl(), result, sFeature);

            obj = result[resultStr];
        }
        else {
            obj = result[0];
        }

        if (obj != null) {
            this._tmpText = "";

            if (!this._multiSelect) {
                this.set_selectedOuUserData(new Array());
            }

            if (this._allowSelectDuplicateObj || !this._checkUserInList(obj.id))
                Array.add(this.get_selectedOuUserData(), obj);

            this.notifyDataChanged();
        }

        this.setInputAreaText();
    },

    _onCheckInputOuUserError: function () {
        if (this._autocompleteControl) {
            this._autocompleteControl.set_isInvoking(false);
        }
    },

    _showPropertyDialog: function () {
        for (var i = 0; i < this.get_selectedOuUserData().length; i++) {
            if (this.get_selectedOuUserData()[i].id == this._mouseSelectItemId.substring(4)) {
                var sQueryStr = "";
                sQueryStr += "&Name=" + this.get_selectedOuUserData()[i].name;
                sQueryStr += "&Path=" + this.get_selectedOuUserData()[i].fullPath;
                sQueryStr += "&GUID=" + this.get_selectedOuUserData()[i].id;

                var sFeature = "dialogWidth:160px; dialogHeight:40px;center:yes;help:no;resizable:yes;scroll:no;status:no";
                var arg = { nameTable: $NT };
                var resultStr = window.showModalDialog(this.get_propertyDialogUrl() + sQueryStr, arg, sFeature);

                break;
            }
        }
    },

    _onCheckSpanClick: function () {
        var spanInput = document.all(this.get_element().id + '_inputArea');
        var divText = spanInput.innerText;

        if (divText != "" && divText.substr(divText.length, 1) != ";") {
            divText = divText + ";";
        }
        var userList = divText.split(";");

        spanInput.innerText = "";

        for (var i = 0; i < userList.length; i++) {
            var user = userList[i].trim();

            if (user != "") {
                var span = this._createUserSpan(user, "spn" + i);

                spanInput.insertBefore(span);

                if (i < userList.length - 1) {
                    var textDiv = document.createTextNode(";");
                    spanInput.insertBefore(textDiv);
                }
            }
        }
    },

    _createUserSpan: function (userName, spanID, hint) {
        var spanInput = document.all(this.get_element().id + '_inputArea');

        var newSpan = document.createElement("span");
        newSpan.contentEditable = false;
        newSpan.style.cursor = "hand";
        newSpan.tabIndex = -1;
        newSpan.id = spanID;
        newSpan.className = "rwNRR";

        var aSpan = document.createElement("span");
        //aSpan.onmousedown = onSelectSpan;
        //aSpan.attachEvent("onclick", onSelectSpan);

        with (aSpan) {
            style.cursor = "hand";
            className = "rwRR";
            tabIndex = -1;
            id = "sub_" + spanID;

            if (hint)
                title = hint;
            else
                title = userName;

            contentEditable = true;

            onclick = "var obj = event.srcElement;var oRng = document.body.createTextRange();oRng.moveToElementText(obj);oRng.select();";
            oncontextmenu = "var obj = event.srcElement;var oRng = document.body.createTextRange();oRng.moveToElementText(obj);oRng.select();";
            ondblclick = this._canEvt;
            oncontrolselect = this._canEvt;
            innerText = userName;
        }
        newSpan.insertBefore(aSpan);
        spanInput.insertBefore(newSpan);

        return newSpan;
    },

    _onMouseSelectItem: function () {
        this._mouseSelectItemId = event.srcElement.id;
    },

    _menuDelEvent: function () {
        if (this._mouseSelectItemId != null && this._mouseSelectItemId != "") {
            var sTmpList = this.get_selectedOuUserData();

            this.set_selectedOuUserData(new Array());

            if (sTmpList != null && sTmpList.length > 0) {
                for (var i = 0; i < sTmpList.length; i++) {
                    if (sTmpList[i].id != this._mouseSelectItemId.substring(4)) {
                        Array.add(this.get_selectedOuUserData(), sTmpList[i]);
                    }
                }
            }
        }

        this.setInputAreaText();
    },

    _onInnerSpanSelect: function () {
        $find(this._thisID + "_ctrlDM").showPopupMenu(event.x, event.y);
        event.returnValue = false;
    },

    _onSpanInputKDRw: function () {
        var e = event;
        var iKC = e.keyCode;

        if (this._disabled == true)//只读
        {
            try {
                e.keyCode = 0;
            }
            catch (e)
             { }
            finally {
                e.returnValue = false;
            }
        }

        switch (iKC) {
            case 13:
                this._canEvt();               //回车

                if (this._autocompleteControl.get_showFlag() == false)
                    this._checkOuUser();

                break;
            case 186: //;
                this._checkOuUser();
                //e.keyCode = 0;
                //e.returnValue = false; 
                e.keyCode = 35;
                break;
            default:
                break;
        }
    },

    checkData: function () {
        if (this._autocompleteControl) {
            if (this._autocompleteControl.get_isInvoking()) {
                return;
            }
        }
        var spanInput = document.all(this.get_element().id + '_inputArea');
        var thisControl = $find(this.get_element().id).findElement("inputArea"); //得到输入框控件

        var sTmpText = thisControl.innerText;
        var spans = spanInput.getElementsByTagName("SPAN");

        if (null != spans && spans.length > 0) {
            for (var i = 0; i < this.get_selectedOuUserData().length; i++) {
                sTmpText = sTmpText.replace(this.get_selectedOuUserData()[i].displayName + ";", "");
            }
        }
        this._tmpText = sTmpText;

        if (sTmpText != "") {
            //this._invoke("CheckInputOuUser", [sTmpText], Function.createDelegate(this, this._onValidateInvokeComplete), '', true);
            this._staticInvoke("CheckInputOuUser", [sTmpText], Function.createDelegate(this, this._onValidateInvokeComplete), '', true);
            if (this._autocompleteControl) {
                this._autocompleteControl.set_isInvoking(true);
            }
        }
        for (var i = 0; i < spanInput.childNodes.length; i++) {
            if (typeof (spanInput.childNodes[i].id) == 'undefined' || spanInput.childNodes[i].id.length <= 0) {
                return false;
            }
        }

        return true;
    },

    _checkOuUser: function () {
        var spanInput = document.all(this.get_element().id + '_inputArea');
        var thisControl = $find(this._thisID).findElement("inputArea"); //得到输入框控件
        var sTmpText = thisControl.innerText;

        var spans = spanInput.getElementsByTagName("SPAN");

        if (null != spans && spans.length > 0) {
            for (var i = 0; i < this.get_selectedOuUserData().length; i++) {
                sTmpText = sTmpText.replace(this.get_selectedOuUserData()[i].displayName + ";", "");
            }
        }
        if (sTmpText != "") {
            this._validateInput(sTmpText);
            this._tmpText = sTmpText;
        }

        return false;
    },

    _canEvt: function () {
        event.returnValue = false;
        event.cancelBubble = true;
    },

    _autoCompleteItemClick: function (sender, e) {
        e.cancel = true;
        //重新绘制列表
        var spanInput = document.all(this.get_element().id + '_inputArea');

        if (!this._multiSelect) {
            this.set_selectedOuUserData(new Array());
            for (var i = 0; i < spanInput.childNodes.length; i++) {
                if (spanInput.childNodes[i].nodeName != "#text") {
                    spanInput.removeChild(spanInput.childNodes[i]);
                }
            }
        }

        //将e.selectedObject加入_selectedOuUserData
        if (!this._checkUserInList(e.selectedObject.id))
            Array.add(this.get_selectedOuUserData(), e.selectedObject);

        //this.setInputAreaText();
        for (var i = 0; i < spanInput.childNodes.length; i++) {
            if (spanInput.childNodes[i].nodeName == "#text")//文字节点
            {
                spanInput.focus();
                if (e.selectedObject.objectType == 1) {
                    img = this._imgOu;
                    textField = this._ouShowText;
                }
                else if (e.selectedObject.objectType == 2) {
                    img = this._imgUser;
                    textField = this._userShowText;
                }
                else {
                    img = this._imgRole;
                    textField = this._roleShowText;
                }

                if (this.get_selectedOuUserData().length < i || typeof (this.get_selectedOuUserData()[i]) == 'undefined')
                    break;

                var span = this._createUserSpan(this.get_selectedOuUserData()[i].displayName + ";",
					"spn_" + this.get_selectedOuUserData()[i].id,
					this.get_selectedOuUserData()[i].fullPath);

                spanInput.insertBefore(span, spanInput.childNodes[i]);

                spanInput.removeChild(spanInput.childNodes[i + 1]);
            }
        }

        this.notifyDataChanged();
        e.cancel = true;
    },

    _spanInputKeyUp: function () {
        var spanInput = document.all(this.get_element().id + '_inputArea');
        var spans = spanInput.getElementsByTagName("SPAN");
        var sArrIDList = new Array();

        if (null != spans && spans.length > 0) {
            for (var i = 0; i < spans.length; i++) {
                if (spans[i].innerText.length > 0)//清除没有内容的span
                {
                    Array.add(sArrIDList, spans[i].id.replace("spn_", ""));
                }
            }
        }

        var sTmpList = this.get_selectedOuUserData();
        var dataChanged = false;

        this.set_selectedOuUserData(new Array());

        if (sArrIDList != null && sArrIDList.length > 0) {
            for (var i = 0; i < sTmpList.length; i++) {
                for (var k = 0; k < sArrIDList.length; k++) {
                    if (sTmpList[i].id == sArrIDList[k]) {
                        Array.add(this.get_selectedOuUserData(), sTmpList[i]);
                        break;
                    }
                }
            }
        }

        dataChanged = this.get_selectedOuUserData().length != sTmpList.length;

        //循环selectedData,如果selectedData中的数据已经不再span中，则移出
        if (this.get_selectedOuUserData() != null) {
            for (var i = 0; i < this.get_selectedOuUserData().length; i++) {
                var itemInSpan = false; //是否存在于Span中
                for (var k = 0; k < spanInput.childNodes.length; k++)//span中的Item
                {
                    if (spanInput.childNodes[k].nodeName == "SPAN") {
                        if (this.get_selectedOuUserData()[i].id == spanInput.childNodes[k].id.substring(4)) {
                            if (document.all('sub_' + spanInput.childNodes[k].id).innerHTML == "") {
                                spanInput.removeChild(spanInput.childNodes[k]);
                            }
                            else {
                                itemInSpan = true;
                            }
                            break;
                        }
                    }
                }

                if (!itemInSpan)//如果不在SPAN中，则移出
                {
                    Array.remove(this.get_selectedOuUserData(), this.get_selectedOuUserData()[i]);
                    dataChanged = true;
                    i--;
                }
            }
        }

        //更改userOUGraphControl控件中选中的数据。
        var userOUGraphControl = $find(this._userougraphcontrolID);
        if (userOUGraphControl) {
            userOUGraphControl._dialogResult = this._selectedOuUserData;
        }

        if (dataChanged)
            this.notifyDataChanged();
    },

    _checkIdInSelectedList: function (objectID) {
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
            var fsCS = Sys.Serialization.JavaScriptSerializer.deserialize(value);
            if (fsCS != null && fsCS.length) {
                this.set_selectedOuUserData(fsCS[0]);
                var spanInput = document.all(this.get_element().id + '_inputArea');
                spanInput.innerText = fsCS[1];
            }
        }
    },

    /// <summary>
    /// 保存ClientState
    /// </summary>
    /// <returns>序列化后的CLientState字符串</returns>
    saveClientState: function () {
        //var fsCS = {};
        for (var i = 0; i < this.get_selectedOuUserData().length; i++) {
            $Serializer.setType(this.get_selectedOuUserData()[i], "oguObject");
        }

        var spanInput = document.all(this.get_element().id + '_inputArea');

        var fsCS = new Array(1);
        fsCS[0] = this.get_selectedOuUserData();
        fsCS[1] = spanInput.innerText;
        return Sys.Serialization.JavaScriptSerializer.serialize(fsCS);

    },

    _shellSort: function (arr) { //插入排序->希儿排序
        var increment = arr.length - 1;
        do {
            increment = (increment / 3 | 0) + 1;
            arr = this._shellPass(arr, increment);
        }
        while (increment > 1)

        return arr;
    },

    _shellPass: function (arr, d) { //希儿排序分段执行函数
        var temp, j;
        for (var i = d; i < arr.length - 1; i++) {
            if (arr[i].globalSortID.localeCompare(arr[i - d].globalSortID) == -1) {
                temp = arr[i]; j = i - d;
                do {
                    arr[j + d] = arr[j];
                    j = j - d;
                }

                while (j > -1 && temp.globalSortID.localeCompare(arr[j].globalSortID) == -1)
					;

                arr[j + d] = temp;
            } //endif
        }
        return arr;
    },

    //判断用户是否已经在现有列表中出现
    _checkUserInList: function (sid) {
        var blnResult = false;
        for (var i = 0; i < this.get_selectedOuUserData().length; i++) {
            if (this.get_selectedOuUserData()[i].id == sid) {
                blnResult = true;
                break;
            }
        }

        return blnResult;
    },

    _onautopopShowing: function (autoControl, e)//检测是否重复
    {
        var items = e.items;
        for (var i = 0; i < items.length; i++) {
            this.checkSameName(items[i], items);
        }
    },

    checkSameName: function (item, items) {
        for (var i = 0; i < items.length; i++) {
            if (item != items[i] && typeof (item.displayName) != 'undefined' && items[i].displayName && item.name == items[i].displayName) {
                var strs = new Array();
                var paths = item.fullPath.replace(/\\|\//g, '-').split('-');

                if (paths.length > 1) {
                    for (var j = 1; j < paths.length - 1; j++) {
                        strs.push(paths[j]);
                    }
                }
                item.name = item.name + "[" + strs.join('-') + "]";
                break;
            }
        }
    },

    clearSelectedOuUserData: function () {
        this.set_selectedOuUserData(new Array());
        this.setInputAreaText()
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
        this.raiseSelectedDataChanged(this.get_selectedOuUserData());
    },

    set_ouSelectBtnStyle: function () {
        var btn = $get(this.get_element().id + "_lnkbtn");

        if (this._disabled || this._readOnly || this._showTreeSelector == false) {
            btn.style.display = "none";
        }
        else {
            btn.style.display = "inline";
        }
    },

    ///没用
    RI: function () {
    }
}

$HBRootNS.OuUserInputControl.registerClass($HBRootNSName + ".OuUserInputControl", $HGRootNS.ControlBase);