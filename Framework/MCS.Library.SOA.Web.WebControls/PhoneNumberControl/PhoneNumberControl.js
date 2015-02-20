
//电话显示方式
$HBRootNS.phoneNumberUseMode = function () {
	throw Error.invalidOperation();
};
//电话显示类型
$HBRootNS.phoneNumberCategory = function () {
	throw Error.invalidOperation();
};

$HBRootNS.phoneNumberUseMode.prototype =
{
	ShowPhoneNumber: 1,                //电话号码
	ShowExtNumber: 2,                      //分机
	ShowAreaNumber: 4,                     //区号
	ShowStateNumber: 8,                    //国别
	ShowFullNumber: 15                     //国别 - 区号 - 电话 - 分机
}


$HBRootNS.phoneNumberCategory.prototype =
{
	Cellphone: 1,      //手机
	Phone: 2          //固定电话
}

$HBRootNS.phoneNumberUseMode.registerEnum($HBRootNSName + ".phoneNumberUseMode");
$HBRootNS.phoneNumberCategory.registerEnum($HBRootNSName + ".phoneNumberCategory");

$HBRootNS.SpecificGlobalMessage = ""; //默认提示信息

//声明构造函数
$HBRootNS.PhoneNumberControl = function (element) {
	$HBRootNS.PhoneNumberControl.initializeBase(this, [element]);

	this._phonenumber = null;
	this._previoustelephone = null;
	this._phoneNumberUseMode = $HBRootNS.phoneNumberUseMode.ShowPhoneNumber; //电话显示样式 （默认仅电话）
	this._phoneNumberCategory = $HBRootNS.phoneNumberCategory.Cellphone; //电话显示类型 （默认仅手机）
	this._stateDropdownListID = null; //国别号
	this._areaInputBoxID = null; //区号控件
	this._mainInputBoxID = null; //电话控件
	this._extInputBoxID = null; //分机控件
	this._IsValidValue = true; //验证
	this._errorMessage = ""; //用户自定义提示信息
	this._telephones = new Array(); 				//Material对象集合
	this._onInvalidCssClass = "telephone_masked_editerror";
	this._customRegularExpression = ""; //用户自定义正则表示式
	this._autoVaildate = null; //是否验证（默认为验证）
	this._allowEmpty = null; //是否为空（默认为空）
	this._readOnly = null;
	this._controlID = null; //控件编号
	this._onchangeHandler = null;
	this._currentAreaCode = null;
	this._currentStateCode = null;
	this._defaultStateCode = null;
	this._defaultAreaCode = null;
	this._specificMessage = null; //提示信息
	this._customAreaRegularExpression = null; //区号正则
	this._customMainRegularExpression = null; //电话正则
	this._customExtRegularExpression = null;//分机正则
}

$HBRootNS.PhoneNumberControl.prototype =
{
    initialize: function () {

        $HBRootNS.PhoneNumberControl.callBaseMethod(this, "initialize");
        if (this.get_element().style.display != "none") {
            Array.add($HBRootNS.phonenumber.allTelephoneControls, this);

            if (!this._readOnly) {
                this.addEventControl();
            }
        }

        this._dataBind(this._phonenumber);
    },

    addEventControl: function () {
        this._onchangeHandler = Function.createDelegate(this, this._onChange);

        if (document.getElementById(this._stateDropdownListID) != null) {
            var statedropdownlist = document.getElementById(this._stateDropdownListID);
            $clearHandlers(statedropdownlist);
            $addHandler(statedropdownlist, "change", this._onchangeHandler);
        }
        if (document.getElementById(this._areaInputBoxID) != null) {
            var areainputbox = document.getElementById(this._areaInputBoxID);
            $clearHandlers(areainputbox);
            $addHandler(areainputbox, "change", this._onchangeHandler);
        }
        if (document.getElementById(this._mainInputBoxID) != null) {
            var maininputbox = document.getElementById(this._mainInputBoxID);
            $clearHandlers(maininputbox);
            $addHandler(maininputbox, "change", this._onchangeHandler);
        }
        if (document.getElementById(this._extInputBoxID) != null) {
            var extinputbox = document.getElementById(this._extInputBoxID);
            $clearHandlers(extinputbox);
            $addHandler(extinputbox, "change", this._onchangeHandler);
        }

    },

    get_data: function () {
        return this._phonenumber;
    },

    set_data: function (value) {
        this._phonenumber = value;
    },

    saveClientState: function () {

        var phonenumber = this._generateTelephone();
        phonenumber.changed = this._dataChanage(this._previousTelephone);
        phonenumber.container = null;
        var result = Sys.Serialization.JavaScriptSerializer.serialize(phonenumber);

        return result;

    },
    loadClientState: function (value) {
        if (value) {
            var phonenumber = Sys.Serialization.JavaScriptSerializer.deserialize(value);
            this._dataBind(phonenumber);
            this._phonenumber = phonenumber;
            this._previousTelephone = {};
            for (var prop in phonenumber) {
                this._previousTelephone[prop] = phonenumber[prop];
            }
        }
    },

    _dataChanage: function (previousTelephone) {
        var changed = false;
        if (this._previousTelephone.stateCode != "") {
            if (this._previousTelephone.stateCode != this._phonenumber.stateCode) {
                changed = true; //未改变
            }
        }
        if (this._previousTelephone.areaCode != "") {
            if (this._previousTelephone.areaCode != this._phonenumber.areaCode) {
                changed = true;
            }
        }
        if (this._previousTelephone.mainCode != "") {
            if (this._previousTelephone.mainCode != this._phonenumber.mainCode) {
                changed = true;
            }
        }
        if (this._previousTelephone.extCode != "") {
            if (this._previousTelephone.extCode != this._phonenumber.extCode) {
                changed = true;
            }
        }

        return changed;
    },

    _dataBind: function (phonenumber) {

        if (this._phonenumber == null) {
            phonenumber = new $HBRootNS.phonenumber();
        }
        if (!this._readOnly) {//数据加载 显示数据库国别与区号->默认国别与区号->用户设置国别与区号

            if (document.getElementById(this._stateDropdownListID) != null) {
                if (phonenumber.stateCode != null && phonenumber.stateCode != "") {
                    document.getElementById(this._stateDropdownListID).value = phonenumber.stateCode;
                }
                else if (this._currentStateCode != null && this._currentStateCode != "")
                    document.getElementById(this._stateDropdownListID).value = this._currentStateCode;
            }
            if (document.getElementById(this._areaInputBoxID) != null) {
                if (phonenumber.areaCode != null && phonenumber.areaCode != "")
                    document.getElementById(this._areaInputBoxID).value = phonenumber.areaCode;
                else if (this._currentAreaCode != null && this._currentAreaCode != "")
                    document.getElementById(this._areaInputBoxID).value = this._currentAreaCode;
            }
            if (phonenumber.mainCode != null && document.getElementById(this._mainInputBoxID) != null) {
                document.getElementById(this._mainInputBoxID).value = phonenumber.mainCode;
            }
            if (phonenumber.extCode != null && document.getElementById(this._extInputBoxID) != null) {
                document.getElementById(this._extInputBoxID).value = phonenumber.extCode;
            }


            $HBRootNS.PhoneNumberControl._phoneNumberCategory = this._phoneNumberCategory;
            $HBRootNS.PhoneNumberControl._phoneNumberUseMode = this._phoneNumberUseMode;
            $HBRootNS.PhoneNumberControl._ReadOnly = this._readOnly;
        }

        phonenumber.container = this;
    },

    _generateTelephone: function () {

        if (this._phonenumber == null) {
            this._phonenumber = new $HBRootNS.phonenumber();
        }

        if (document.getElementById(this._stateDropdownListID) != null)
            this._phonenumber.stateCode = document.getElementById(this._stateDropdownListID).value;
        if (document.getElementById(this._areaInputBoxID) != null)
            this._phonenumber.areaCode = document.getElementById(this._areaInputBoxID).value;
        if (document.getElementById(this._mainInputBoxID) != null)
            this._phonenumber.mainCode = document.getElementById(this._mainInputBoxID).value;
        if (document.getElementById(this._extInputBoxID) != null)
            this._phonenumber.extCode = document.getElementById(this._extInputBoxID).value;

        return this._phonenumber;

    },
    get_readOnly: function () {

        return this._readOnly;

    },
    set_readOnly: function (value) {

        this._readOnly = value;
    },

    get_onchangeHandler: function () {
        return this._onchangeHandler;
    },

    set_onchangeHandler: function (value) {
        this._onchangeHandler = value;
    },

    get_container: function () {
        return this._container;
    },

    set_container: function (value) {
        if (this._container != value) {
            this._container = value;
        }
    },
    get_phoneNumberUseMode: function () {
        return this._phoneNumberUseMode;
    },
    set_phoneNumberUseMode: function (value) {
        if (this._phoneNumberUseMode != value) {
            this._phoneNumberUseMode = value;
        }
    },

    get_phoneNumberCategory: function () {
        return this._phoneNumberCategory;
    },
    set_phoneNumberCategory: function (value) {
        if (this._phoneNumberCategory != value) {
            this._phoneNumberCategory = value;
        }
    },

    get_stateDropdownListID: function () {
        return this._stateDropdownListID;
    },
    set_stateDropdownListID: function (value) {
        if (this._stateDropdownListID != value) {
            this._stateDropdownListID = value;;
        }
    },

    get_areaInputBoxID: function () {
        return this._areaInputBoxID;
    },
    set_areaInputBoxID: function (value) {
        if (this._areaInputBoxID != value) {
            this._areaInputBoxID = value;
        }
    },

    get_mainInputBoxID: function () {
        return this._mainInputBoxID;
    },
    set_mainInputBoxID: function (value) {
        if (this._mainInputBoxID != value) {
            this._mainInputBoxID = value;
        }
    },

    get_extInputBoxID: function () {
        return this._extInputBoxID;
    },
    set_extInputBoxID: function (value) {
        if (this._extInputBoxID != value) {
            this._extInputBoxID = value;
        }
    },

    get_phonenumber: function () {
        return this._phonenumber;
    },
    set_phonenumber: function (value) {
        if (this._phonenumber != value) {
            this._phonenumber = value;
        }
    },
    get_readOnly: function () {
        return this._readOnly;
    },
    set_readOnly: function (value) {
        if (this._readOnly != value) {
            this._readOnly = value;
        }
    },
    get_autoVaildate: function () {
        return this._autoVaildate;
    },

    set_autoVaildate: function (value) {
        this._autoVaildate = value;
    },
    get_errorMessage: function () {
        return this._errorMessage;
    },

    set_errorMessage: function (value) {
        this._errorMessage = value;
    },

    get_customAreaRegularExpression: function () {
        return this._customAreaRegularExpression;
    },
    set_customAreaRegularExpression: function (value) {
        this._customAreaRegularExpression = value;
    },

    get_customMainRegularExpression: function () {
        return this._customMainRegularExpression;
    },
    set_customMainRegularExpression: function (value) {
        this._customMainRegularExpression = value;
    },

    get_customExtRegularExpression: function () {
        return this._customExtRegularExpression;
    },
    set_customExtRegularExpression: function (value) {
        this._customExtRegularExpression = value;
    },

    get_onInvalidCssClass: function () {
        return this._onInvalidCssClass;
    },

    set_onInvalidCssClass: function (value) {
        this._onInvalidCssClass = value;
    },

    get_controlID: function () {
        return this._controlID;
    },

    set_controlID: function (value) {
        this._controlID = value;
    },
    get_allowEmpty: function () {
        return this._allowEmpty;
    },

    set_allowEmpty: function (value) {
        this._allowEmpty = value;
    },
    get_currentStateCode: function () {
        return this._currentStateCode;
    },
    set_currentStateCode: function (value) {
        if (this._currentStateCode != value) {
            this._currentStateCode = value;
        }
    },
    get_currentAreaCode: function () {
        return this._currentAreaCode;
    },
    set_currentAreaCode: function (value) {
        if (this._currentAreaCode != value) {
            this._currentAreaCode = value;
        }
    },
    get_defaultStateCode: function () {
        return this._defaultStateCode;
    },
    set_defaultStateCode: function (value) {
        if (this._defaultStateCode != value) {
            this._defaultStateCode = value;
        }
    },
    get_defaultAreaCode: function () {
        return this._defaultAreaCode;
    },
    set_defaultAreaCode: function (value) {
        if (this._defaultAreaCode != value) {
            this._defaultAreaCode = value;
        }
    },
    get_specificMessage: function () {
        return this._specificMessage;
    },
    set_specificMessage: function (value) {
        if (this._specificMessage != value) {
            this._specificMessage = value;
        }
    },
    get_cloneableProperties: function () {

        var baseProperties = $HGRootNS.PhoneNumberControl.callBaseMethod(this, "get_cloneableProperties");

        var currentProperties = ["phonenumber", "autoVaildate", "allowEmpty", "container", "generateTelephone", "stateCode", "readOnly",
		 "phoneNumberUseMode", "phoneNumberCategory", "onchangeHandler", "onChange", "onTelephoneCustomValidate", "onTelephoneNumberValidate",
		 "onAfterCloneElement", "raiseClientValueChanged", "removeClientValueChanged", "addClientValueChanged", "onInvalidCssClass", "currentAreaCode",
		 "currentStateCode", "previousTelephone", "specificMessage", "errorMessage", "onAreaNumberValidate", "onMainNumberValidate", "onExtNumberValidate",
		 "customExtRegularExpression", "customMainRegularExpression", "customAreaRegularExpression"];

        Array.addRange(currentProperties, baseProperties);

        return currentProperties;
    },
    _prepareCloneablePropertyValues: function (newElement) {
        var properties = $HGRootNS.PhoneNumberControl.callBaseMethod(this, "_prepareCloneablePropertyValues", [newElement]);
        this.replaceOldIDs(newElement.id, newElement, properties);

        properties.controlID = newElement.id;

        return properties;
    },

    //递归替换element的OldID
    replaceOldIDs: function (controlID, element, properties) {
        for (var i = 0; i < element.childNodes.length; i++) {
            var curElement = element.childNodes[i];
            switch (curElement.id) {
                case this._stateDropdownListID:
                    curElement.id = curElement.uniqueID;
                    properties.stateDropdownListID = curElement.id;
                    break;
                case this._areaInputBoxID:
                    curElement.id = curElement.uniqueID + "_areaInputBox";
                    properties.areaInputBoxID = curElement.id;
                    break;
                case this._mainInputBoxID:
                    curElement.id = curElement.uniqueID + "_mainInputBox";
                    properties.mainInputBoxID = curElement.id;
                    break;
                case this._extInputBoxID:
                    curElement.id = curElement.uniqueID + "_extInputBox";
                    properties.extInputBoxID = curElement.id;
                    break;

            }
        }
    },

    //	onAfterCloneElement: function (sourceElement, newElement) {
    //		for (var i = 0; i < sourceElement.childNodes.length; i++) {
    //			var newNode = sourceElement.childNodes[i].cloneNode(false);
    //			newElement.appendChild(newNode);
    //		}
    //	},

    cloneElement: function () {

        var result = null;
        var sourceElement = this.get_element();

        if (sourceElement != null) {
            this.onBeforeCloneElement(sourceElement);

            try {
                result = sourceElement.cloneNode(true);
                result.style.display = "";
                result.id = result.uniqueID;
                result.control = undefined;
            }
            finally {
                this.onAfterCloneElement(sourceElement, result);
            }
        }
        return result;

    },
    cloneAndAppendToContainer: function (newElement) {
        var baseProperties = $HGRootNS.PhoneNumberControl.callBaseMethod(this, "cloneAndAppendToContainer", [newElement]);
    },

    disposeEventControl: function () {
        if (this._onchangeHandler) {
            var statedropdownlist = document.getElementById(this._stateDropdownListID);
            if (statedropdownlist != null)
                $clearHandlers(statedropdownlist);
                //$removeHandler(statedropdownlist, "change", this._onchangeHandler);

            var areainputbox = document.getElementById(this._areaInputBoxID);
            if (areainputbox != null)
                $clearHandlers(areainputbox);
                //$removeHandler(areainputbox, "change", this._onchangeHandler);

            var maininputbox = document.getElementById(this._mainInputBoxID);
            if (maininputbox != null)
                $clearHandlers(maininputbox);
                //$removeHandler(maininputbox, "change", this._onchangeHandler);

            var extinputbox = document.getElementById(this._extInputBoxID);
            if (extinputbox != null)
                $clearHandlers(extinputbox);
                //$removeHandler(extinputbox, "change", this._onchangeHandler);

            this._onchangeHandler = null;
        }
    },
    dispose: function () {

        $HBRootNS.phonenumber.allTelephoneControls.length = 0;

        this.disposeEventControl();

        $HBRootNS.PhoneNumberControl.callBaseMethod(this, 'dispose');

    },

    _onChange: function (args) {
        var IsValid = true;
        if (!this._readOnly) {
            if (this._customRegularExpression != "") {
                IsValid = this.onTelephoneCustomValidate();
            }
            else {

                if (event.srcElement.id.indexOf("areaInputBox") > 0)
                    IsValid = this.onAreaNumberValidate();
                else if (event.srcElement.id.indexOf("mainInputBox") > 0)
                    IsValid = this.onMainNumberValidate();
                else if (event.srcElement.id.indexOf("extInputBox") > 0)
                    IsValid = this.onExtNumberValidate();
            }
        }

        if (IsValid == false && this._specificMessage.length > 0 && event.srcElement.tagName != "SELECT") {

            Sys.UI.DomElement.addCssClass(event.srcElement, this._onInvalidCssClass);
            alert(this._specificMessage);
        }
        else
            Sys.UI.DomElement.removeCssClass(event.srcElement, this._onInvalidCssClass);

        if (IsValid) {
            this._generateTelephone();
            this.raiseClientValueChanged();
        }

        return IsValid;
    },

    onTelephoneNumberValidate: function () {

        var flag = true;

        if (this._readOnly == false && this._autoVaildate == true && this.get_element().style.display != "none") {
            if (this._phonenumber == null) {
                this._phonenumber = new $HBRootNS.phonenumber();
            }

            var area = this.onAreaNumberValidate();
            $HBRootNS.SpecificGlobalMessage += this._specificMessage;
            var main = this.onMainNumberValidate();
            $HBRootNS.SpecificGlobalMessage += this._specificMessage;
            var ext = this.onExtNumberValidate();
            $HBRootNS.SpecificGlobalMessage += this._specificMessage;

            if (!area || !main || !ext) {
                flag = false;
            }
        }

        return flag;
    },

    onAreaNumberValidate: function () { //对区号验证

        var flag = true; //是否通过验证
        this._specificMessage = "";

        if (document.getElementById(this._areaInputBoxID) != null) {

            var areaCode = document.getElementById(this._areaInputBoxID).value;

            if (areaCode == "" && this._allowEmpty == false) {
                this._specificMessage = "区号不能为空" + "\n";
                flag = false;
            }
            else if (this._customAreaRegularExpression != "") {//使用正则

                var reg = new RegExp(this._customAreaRegularExpression);
                if (!reg.test(areaCode) && areaCode != "") {
                    this._specificMessage = "错误位置:区号[" + areaCode + "]格式不正确" + "\n";
                    flag = false;
                }
            }
            //			else if (!/^(\d{3,4})$/.test(areaCode) && areaCode != "") {
            //				this._specificMessage = "错误位置:区号[" + areaCode + "]必须为3-4位的数字" + "\n";
            //				flag = false;
            //			}

            if (flag)
                Sys.UI.DomElement.addCssClass(document.getElementById(this._areaInputBoxID), "");
            else
                Sys.UI.DomElement.addCssClass(document.getElementById(this._areaInputBoxID), this._onInvalidCssClass);
        }

        return flag;

    },

    onMainNumberValidate: function () {
        var flag = true;
        this._specificMessage = "";

        if (document.getElementById(this._mainInputBoxID) != null) {

            var mainCode = document.getElementById(this._mainInputBoxID).value;

            var phonecategory = "电话";
            var regular = /^(\d{7,8})$/;
            var message = "7-8位";
            if (this._phoneNumberCategory == $HBRootNS.phoneNumberCategory.Cellphone) {
                phonecategory = "手机";
                regular = /^(\d{11,12})$/;
                message = "11-12位";
            }

            if (mainCode == "" && this._allowEmpty == false) {
                this._specificMessage = phonecategory + "号码不能为空" + "\n";
                flag = false;
            }
            else if (this._customMainRegularExpression != "") {//正则
                var reg = new RegExp(this._customMainRegularExpression);
                if (!reg.test(mainCode) && mainCode != "") {
                    this._specificMessage = "错误位置:" + phonecategory + "号码[" + mainCode + "]格式不正确" + "\n";
                    flag = false;
                }
            }
            //			else if (!regular.test(mainCode) && mainCode != "") {
            //				this._specificMessage = "错误位置:" + phonecategory + "号码[" + mainCode + "]必须为" + message + "的数字" + "\n";
            //				flag = false;
            //			}

            if (flag)
                Sys.UI.DomElement.addCssClass(document.getElementById(this._mainInputBoxID), "");
            else
                Sys.UI.DomElement.addCssClass(document.getElementById(this._mainInputBoxID), this._onInvalidCssClass);
        }

        return flag;
    },

    onExtNumberValidate: function () {

        var flag = true;
        this._specificMessage = "";

        if (document.getElementById(this._extInputBoxID) != null) {

            var extCode = document.getElementById(this._extInputBoxID).value;

            if (extCode == "" && this._allowEmpty == false) {
                this._specificMessage = "分机号码不能为空" + "\n";
                flag = false;
            }
            else if (this._customExtRegularExpression != "") {//使用正则

                var reg = new RegExp(this._customExtRegularExpression);

                if (!reg.test(extCode) && extCode != "") {
                    this._specificMessage = "错误位置:分机号码[" + extCode + "]格式不正确" + "\n";
                    flag = false;
                }
            }
            //			else if (!/^(\d{1,4})$/.test(extCode) && extCode != "") {
            //				this._specificMessage = "错误位置:分机号码[" + extCode + "]必须为1-4位的数字" + "\n";
            //				flag = false;
            //			}

            if (flag)
                Sys.UI.DomElement.addCssClass(document.getElementById(this._extInputBoxID), "");
            else
                Sys.UI.DomElement.addCssClass(document.getElementById(this._extInputBoxID), this._onInvalidCssClass);
        }

        return flag;
    },

    raiseClientValueChanged: function () {
        var handlers = this.get_events().getHandler("onClientValueChanged");
        var e = {};
        e.Data = this.get_data();

        if (handlers) {
            handlers(this, e);
        }
    },

    remove_ClientValueChanged: function (handler) {
        this.get_events().removeHandler("onClientValueChanged", handler);
    },

    add_ClientValueChanged: function (handler) {
        this.get_events().addHandler("onClientValueChanged", handler);
    }
}


//客户端数据验证
$HBRootNS.PhoneNumberControl.telephoneClientValidate = function (source, arguments) {

	var msg = "";
	$HBRootNS.SpecificGlobalMessage = "";
	$HBRootNS.phonenumber.checkTelephones.length = 0;

	for (var i = 0; i < $HBRootNS.phonenumber.allTelephoneControls.length; i++) {

		var PhoneNumberControl = $HBRootNS.phonenumber.allTelephoneControls[i];

		IsValid = PhoneNumberControl.onTelephoneNumberValidate();

		Array.add($HBRootNS.phonenumber.checkTelephones, IsValid);

		if (!IsValid) {
			//如果没设置ErrorMessage,提示默认的指定信息
			if (PhoneNumberControl._errorMessage == "") {
				msg = $HBRootNS.SpecificGlobalMessage;
			}
			else {
				msg += PhoneNumberControl._errorMessage + "\n";
			}
		}
	}

	if (msg.length > 0) {
		alert(msg);
	}

	for (var i = 0; i < $HBRootNS.phonenumber.checkTelephones.length; i++) {
		validState = $HBRootNS.phonenumber.checkTelephones[i];
		if (validState == false) {
			arguments.IsValid = false;
			break;
		}
		else {
			arguments.IsValid = true;
		}
	}

}

//注册继承关系
$HBRootNS.PhoneNumberControl.registerClass($HBRootNSName + ".PhoneNumberControl", $HBRootNS.ControlBase);