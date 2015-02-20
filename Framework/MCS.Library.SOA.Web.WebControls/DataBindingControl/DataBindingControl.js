// -------------------------------------------------
// FileName	:	DataBindingControl.js
// Remark	:	DataBindingControl
// -------------------------------------------------
// VERSION		AUTHOR		     DATE			CONTENT
// 1.0			xuwenzhuo		 20080408		ceate
// -------------------------------------------------

//************************ClientBindingDataType*******************************{

$HBRootNS.ClientBindingDataType = function (element) {
	throw Error.invalidOperation();
}

$HBRootNS.ClientBindingDataType.prototype = {
	None: 0,
	String: 1,
	Number: 2,
	Float: 3,
	DateTime: 4,
	Object: 5
}

$HBRootNS.ClientBindingDataType.registerEnum($HBRootNSName + ".ClientBindingDataType");

//************************ClientBindingDataType*******************************}

//************************BindingDirection*******************************{

$HBRootNS.BindingDirection = function (element) {
	throw Error.invalidOperation();
}

$HBRootNS.BindingDirection.prototype = {
	None: 0,
	ControlToData: 1,
	DataToControl: 2,
	Both: 3
}

$HBRootNS.BindingDirection.registerEnum($HBRootNSName + ".BindingDirection");

//************************BindingDirection*******************************}

//************************DataBindingControl*******************************{

$HBRootNS.DataBindingControl = function (element) {
	$HBRootNS.DataBindingControl.initializeBase(this, [element]);
	this.validList = null; 		     //ClientVdtData List.
	this.validControls = null;       // Control and ClientVdtData List.
	this.errorMessages = null;    	 //errormessage List.
	this.errorImg = null;
	this.isValidate = true; 		 //validate result.
	this._clientItemBindings = [];
	this._loaded = false;

	this._applicationLoad$delegate = Function.createDelegate(this, this._applicationLoad);
}

$HBRootNS.DataBindingControl.prototype = {
	initialize: function () {
		Sys.Application.add_load(this._applicationLoad$delegate);

		$HBRootNS.DataBindingControl.callBaseMethod(this, 'initialize');

		this.validControls = new Array();
		this.errorMessages = new Array();
	},

	dispose: function () {
		if (this._applicationLoad$delegate) {
			Sys.Application.remove_load(this._applicationLoad$delegate);
			this._applicationLoad$delegate = null;
		}

		this.validList = null;
		this.validControlList = null;
		this.errorMessages = null;
	},

	_applicationLoad: function () {
		if (this._loaded == false) {
			this.createValidators();
			this.addEventToControl();

			this._loaded = true;
		}
	},

	createValidators: function () {
		if (!this.validList)
			return;

		for (var i = 0; i < this.validList.length; i++) {
			var validator = this.validList[i];

			var control = this._findControl(validator.ControlID, validator.ClientIsHtmlElement);

			if (control) {
				this.validControls.push(new Array(control, validator));
			}
		}
	},

	get_clientItemBindings: function () {
		return this._clientItemBindings;
	},

	get_errorImg: function () {
		return this.errorImg;
	},

	set_errorImg: function (value) {
		if (this.errorImg != value) {
			this.errorImg = value;
			this.raisePropertyChanged("errorImg");
		}
	},

	collectData: function (autoValidate) {

		if (typeof (autoValidate) == "undefined")
			autoValidate = true;

		if (autoValidate) {
			if (!this.checkAllData()) {
				var e = Error.create();

				e.number = "100022";
				e.message = this.errorMessages.join("\n");
				e.description = e.message;

				throw e;
			}
		}

		var result = {};

		for (var i = 0; i < this._clientItemBindings.length; i++) {
			var item = this._clientItemBindings[i];

			var control = this._findControl(item.ControlID, item.ClientIsHtmlElement);

			if (control) {
				var e = this._raiseClientCollectData(item, control, result);

				if (e.cancel == false) {
					var value = this._get_controlPropertyValueByItem(control, item);

					if (typeof (value) != "undefined") {
						value = this._changeStringToTargetType(value, item.ClientDataType);
						result[item.ClientDataPropertyName] = value;
					}
				}
			}
		}

		return result;
	},

	add_clientCollectData: function (handler) {
		this.get_events().addHandler('clientCollectData', handler);
	},

	remove_clientCollectData: function (handler) {
		this.get_events().removeHandler('clientCollectData', handler);
	},

	_raiseClientCollectData: function (item, control, data) {
		var handler = this.get_events().getHandler("clientCollectData");
		var e = new Sys.EventArgs();

		e.bindingItem = item;
		e.control = control;
		e.data = data;
		e.cancel = false;

		if (handler) {
			handler(this, e);
		}

		return e;
	},

	dataBind: function (data) {
		for (var i = 0; i < this._clientItemBindings.length; i++) {
			var item = this._clientItemBindings[i];

			var control = this._findControl(item.ControlID, item.ClientIsHtmlElement);

			if (control) {
				var e = this._raiseClientDataBinding(item, control, data);

				if (e.cancel == false) {
					if (typeof (data[item.ClientDataPropertyName]) != "undefined") {
						var propertyValue = data[item.ClientDataPropertyName];

						if (item.ClientIsHtmlElement) {
							propertyValue = this._formatDataByType(propertyValue, item);
							control[this._translateHtmlControlValueProperty(control, item)] = propertyValue;
						}
						else {
							if (control[item.ClientSetPropName])
								control[item.ClientSetPropName](propertyValue);
						}

						if (typeof (control.dataBind) != "undefined")
							control.dataBind();
					}
				}
			}
		}
	},

	add_clientDataBinding: function (handler) {
		this.get_events().addHandler('clientDataBinding', handler);
	},

	remove_clientDataBinding: function (handler) {
		this.get_events().removeHandler('clientDataBinding', handler);
	},

	_raiseClientDataBinding: function (item, control, data) {
		var handler = this.get_events().getHandler("clientDataBinding");
		var e = new Sys.EventArgs();

		e.bindingItem = item;
		e.control = control;
		e.data = data;
		e.cancel = false;

		if (handler) {
			handler(this, e);
		}

		return e;
	},

	_translateHtmlControlValueProperty: function (control, item) {
		var propName = item.ClientPropName;

		var value = control[item.ClientSetPropName];
		if (value == null || typeof (value) == "undefined")
			propName = "innerText";

		return propName;
	},

	_formatDataByType: function (data, item) {
		var result = data;

		if (item.Format) {
			switch (item.ClientDataType) {
				case $HBRootNS.ClientBindingDataType.Number:
				case $HBRootNS.ClientBindingDataType.Float:
					result = $HGRootNS.Formatter.pictureFormat(data, item.Format);
					break;
				case $HBRootNS.ClientBindingDataType.DateTime:
					if (Date.isMinDate(data))
						result = "";
					else
						result = String.format(item.Format, data);
					break;
			}
		}

		return result;
	},

	_changeStringToTargetType: function (str, targetType) {
		var result = str;

		switch (targetType) {
			case $HBRootNS.ClientBindingDataType.Number:
			case $HBRootNS.ClientBindingDataType.Float:
				{
					str = str.replace(/,/g, '');
					result = (targetType == $HBRootNS.ClientBindingDataType.Number) ? parseInt(str) : parseFloat(str);

					if (isNaN(result))
						result = 0;

					break;
				}
			case $HBRootNS.ClientBindingDataType.DateTime:
				result = this.parseStrToDate(str);
				break;
		}

		return result;
	},

	_findControl: function (controlID, isHtmlElement) {
		var control = null;

		if (isHtmlElement)
			control = $get(controlID);
		else
			control = $find(controlID);

		return control;
	},

	loadClientState: function (value) {
		if (value && value.length > 0) {
			var state = Sys.Serialization.JavaScriptSerializer.deserialize(value);

			this.validList = state[0];
			this._clientItemBindings = state[1];
		}
	},

	findvalidControl: function (control) {
		if (control) {
			for (var n = 0; n < this.validControls.length; n++) {
				if (control == this.validControls[n][0]) {
					return this.validControls[n];
				}
			}
		}
		return null;
	},

	//////////////////////////////////////////////////////////
	bindControl: function (control, dataType, formatString) {
		var binder = new $HBRootNS.TextBoxValidationBinder();
		binder.set_control(control);
		binder.set_dataType(dataType);
		binder.set_formatString(formatString);
		//binder.add_dataChange(Function.createDelegate(this, this._onDataChange)); //binder里头抛出的事件  
		binder.bind();
	},

	addEventToControl: function () {
		if (!this.validControls)
			return;

		for (var i = 0; i < this.validControls.length; i++) {
			var validator = this.validControls[i][1];
			var validateControl = this.validControls[i][0];

			if (validator.ClientIsHtmlElement) {
				if (validator.CvtList && validator.CvtList.length > 0) {
					if (validator.IsValidateOnBlur) {
						$addHandler(validateControl, "change", Function.createDelegate(this, this.checkData));
					}
				}

				if (validator.FormatString && validator.FormatString.length > 0 && validator.AutoFormatOnBlur) {
					$addHandler(validateControl, "change", Function.createDelegate(this, this.formatData));
				}

				if (validator.IsOnlyNum) {    //add check user input events
					$addHandler(validateControl, "keypress", Function.createDelegate(this, this.onlyNumKeyPress));

					if (validator.AutoFormatOnBlur)
						$addHandler(validateControl, "change", Function.createDelegate(this, this.onlyNum));

					validateControl.style.imeMode = 'disabled';
				}
				else if (validator.IsFloat) {
					$addHandler(validateControl, "keypress", Function.createDelegate(this, this.onlyFloatKeyPress));

					if (validator.AutoFormatOnBlur)
						$addHandler(validateControl, "change", Function.createDelegate(this, this.onlyFloat));

					validateControl.style.imeMode = 'disabled';
				}
			}
		}
	},

	//add ExEvents,params: target control ,callback function ,isValidateonblur,errorMsg
	addExEvent: function (control, func, isValidateonblur, errorMsg, groupID) {

		var eItem = new Object();

		eItem.ControlID = control.id;
		eItem.IsValidateOnBlur = isValidateonblur;
		eItem.Func = func;
		eItem.errorMsg = errorMsg;
		eItem.ValidationGroup = groupID;

		this.validControls.push(new Array(control, eItem));

		if (eItem.IsValidateOnBlur) {
			$addHandler(control, "change", Function.createDelegate(this, func));
		}

	},

	checkData: function (e) {
		this.errorMessages = new Array();
		var isValidate = false;
		var validControl = this.findvalidControl(e.target);

		if (validControl == null)
			return;

		var control = e.target;
		var validator = validControl[1];

		isValidate = this.validateData(this._get_controlPropertyValue(control, validator), validator);

		if (!isValidate) {
			//如果当前校验的是数字，清掉不是数字的数据。
			if (validator.IsOnlyNum || validator.IsFloat) {
				if (e.target.value) {
					e.target.value = e.target.value.toString().replace(/,|\D|./g, '');
				}
			}

			this.showError(e.target, false);
		}

		return isValidate;
	},

	//if groupID is undefined or < 0, exec all validators
	checkAllData: function (groupID) {
		var isValidate = true;
		var doCompareGroupID = (groupID >= 0);

		this.errorMessages = new Array();

		for (var k = 0; k < this.validControls.length; k++) {

			var doValidate = true;

			if (doCompareGroupID)
				doValidate = (this.validControls[k][1].ValidationGroup & groupID) != 0;

			if (doValidate) {
				var tempisValidate = true;

				if (this.validControls[k][1].Func) {
					if (!this.validControls[k][1].Func()) {
						tempisValidate = false;
						this.errorMessages.push(this.validControls[k][1].errorMsg);
					}
				}
				else if (!this.validateData(
						this._get_controlPropertyValue(this.validControls[k][0], this.validControls[k][1]),
							this.validControls[k][1])) {
					tempisValidate = false;
				}

				if (!tempisValidate) {
					isValidate = false;
				}
			}
		}

		this.isValidate = isValidate;

		return isValidate;
	},

	_get_controlPropertyValue: function (control, validator) {
		var result;

		if (validator.ClientIsHtmlElement)
			result = control[validator.ValidateProp];
		else
			result = control[validator.ValidateProp]();

		return result;
	},

	_get_controlPropertyValueByItem: function (control, bindingItem) {
		var result;
		var propName = this._translateHtmlControlValueProperty(control, bindingItem);

		if (bindingItem.ClientIsHtmlElement)
			result = control[propName];
		else
			result = control[propName]();

		return result;
	},

	validateData: function (cvalue, validator) {
		var isValidate = false;

		if (typeof (cvalue) == 'undefined')
			return true;   //can not validate data,renturn true;

		if (cvalue != null && typeof (cvalue) == "string")
			cvalue = cvalue.toString().replace(/^[\s　]+|[\s　]+$/g, '');

		if (!validator.CvtList && validator.CvtList.length <= 0)
			return true;

		if (validator.IsAnd) {
			isValidate = this.andValidateData(cvalue, validator.CvtList);
		}
		else {
			isValidate = this.orValidateData(cvalue, validator.CvtList);
		}

		return isValidate;
	},

	orValidateData: function (cvalue, vadlist) {
		var tempErrorTemplates = [];
		for (var i = 0; i < vadlist.length; i++) {
			//			if (vadlist[i].VType == 0) {
			//				if (this.rangeCheck(cvalue, vadlist[i]))
			//					return true;
			//			}
			//			else if (vadlist[i].VType == 1) {
			//				if (this.expressionCheck(cvalue, vadlist[i]))
			//					return true;
			//			}
			//			else if (vadlist[i].VType == 2) {
			//				if (this.emptyCheck(cvalue, vadlist[i]))
			//					return true;
			//			}
			if (typeof (vadlist[i].ClientValidateMethodName) != "undefined" && vadlist[i].ClientValidateMethodName.length > 0) {
				var method = $HGRootNS.ValidatorManager[vadlist[i].ClientValidateMethodName];
				if (typeof (method) != "function") {
					this.errorMessages.push("未找到方法：" + vadlist[i].ClientValidateMethodName);
					return false;
				}
				else {
					var methodInstance = new method();
					if (methodInstance.validate(cvalue, vadlist[i].AdditionalData))
						return true;
					else {
						tempErrorTemplates.push(vadlist[i].MessageTemplate);
					}
				}
			}
		}
		var strTemp = "";
		for (var i = 0; i < tempErrorTemplates.length; i++) {
			strTemp += tempErrorTemplates[i] + "|";
		}
		strTemp = strTemp.substring(0, strTemp.length - 1);
		this.errorMessages.push(strTemp)
		return false;
	},

	andValidateData: function (cvalue, vadlist) {
		for (var i = 0; i < vadlist.length; i++) {
			//                        if (vadlist[i].VType == 0) {
			//                            if (!this.rangeCheck(cvalue, vadlist[i]))
			//                                return false;
			//                        }
			//                        else if (vadlist[i].VType == 1) {
			//                            if (!this.expressionCheck(cvalue, vadlist[i]))
			//                                return false;
			//                        }
			//                        else if (vadlist[i].VType == 2) {
			//                            if (!this.emptyCheck(cvalue, vadlist[i]))
			//                                return false;
			//                        }
			if (typeof (vadlist[i].ClientValidateMethodName) != "undefined" && vadlist[i].ClientValidateMethodName.length > 0) {
				var method = $HGRootNS.ValidatorManager[vadlist[i].ClientValidateMethodName];
				if (typeof (method) != "function") {
					this.errorMessages.push("未找到方法：" + vadlist[i].ClientValidateMethodName);
					return false;
				}
				else {
					var methodInstance = new method();
					if (!methodInstance.validate(cvalue, vadlist[i].AdditionalData)) {
						this.errorMessages.push(vadlist[i].MessageTemplate);
						return false;
					}
				}
			}
		}
		return true;
	},

	rangeCheck: function (cvalue, vadItem) {
		var isValidate = false;
		var lowerBound;
		var upperBound;
		var sourcevalue;

		if (vadItem.DType == 0 || vadItem.DType == 3) {
			cvalue = cvalue.replace(/,/g, ''); //convert to int
		}

		if (vadItem.DType == 0) {
			if (!cvalue.toString().match('^[0-9]*$')) {
				this.errorMessages.push(vadItem.MessageTemplate);
				return false;
			}

			if (cvalue.length <= 0)
				cvalue = 0;

			sourcevalue = parseInt(cvalue);
			lowerBound = parseInt(vadItem.LowerBound);
			upperBound = parseInt(vadItem.UpperBound);
		}
		else if (vadItem.DType == 2) {
			sourcevalue = this.parseStrToDate(cvalue);
			lowerBound = this.parseStrToDate(vadItem.LowerBound);
			upperBound = this.parseStrToDate(vadItem.UpperBound);
		}
		else if (vadItem.DType == 3) {
			if (cvalue.length <= 0) cvalue = 0;
			if (cvalue == '.') { // only one char '.', invalid, add by Chen Weiwei 2009.08.02
				isValidate = false;
				this.errorMessages.push(vadItem.MessageTemplate);
				return isValidate;
			}
			sourcevalue = parseFloat(cvalue);
			lowerBound = parseFloat(vadItem.LowerBound);
			upperBound = parseFloat(vadItem.UpperBound);
		}
		else if (vadItem.DType == 4) {
			sourcevalue = cvalue.length;
			lowerBound = vadItem.LowerBound;
			upperBound = vadItem.UpperBound;
		}
		else if (vadItem.DType == 5) {
			sourcevalue = cvalue.replace(/[^\x00-\xff]/g, '**').length;
			lowerBound = vadItem.LowerBound;
			upperBound = vadItem.UpperBound;
		}
		else {
			lowerBound = vadItem.LowerBound;
			upperBound = vadItem.UpperBound;
		}
		if (sourcevalue < lowerBound || sourcevalue > upperBound) {
			isValidate = false;
			this.errorMessages.push(vadItem.MessageTemplate);
		}
		else {
			isValidate = true;
		}

		return isValidate;
	},

	expressionCheck: function (cvalue, vadItem) {
		if (vadItem.DType == 0 || vadItem.DType == 3) {
			cvalue = cvalue.replace(/,/g, '');   //convert to int
		}

		var isValidate = false;
		var result = cvalue.toString().match(vadItem.Expression);

		if (result != null) {
			isValidate = true;
		}
		else {
			isValidate = false;
			this.errorMessages.push(vadItem.MessageTemplate);
		}

		return isValidate;
	},

	emptyCheck: function (cvalue, vadItem) {
		var isValidate = false;

		if (cvalue) {
			if (vadItem.DType == 0 || vadItem.DType == 2 || vadItem.DType == 3) {
				//Number
				isValidate = (isNaN(cvalue) == false);

				if (isValidate && vadItem.DType == 2)
					isValidate = Date.isMinDate(cvalue) == false;
			}
			else if (vadItem.DType == 1) {
				//String
				isValidate = cvalue.length > 0;
			}
			else if (vadItem.DType == 7) {
				isValidate = cvalue != vadItem.Tag;
			}
			else
				isValidate = true;
		}

		if (isValidate == false)
			this.errorMessages.push(vadItem.MessageTemplate);

		return isValidate;
	},

	parseStrToDate: function (str) {
		if (typeof (str) == 'Date')
			return str;

		if (str == "")
			return Date.minDate;

		var tdata = str.split(' ');
		var dar = new Array(0, 0, 0);
		var tar = new Array(0, 0, 0);

		if (tdata.length > 0) {
			dar = tdata[0].replace(/\-0/g, "-").split("-");
			if (dar.length == 3) {
				dar = new Array(parseInt(dar[0]), parseInt(dar[1]) - 1, parseInt(dar[2]));
			}
			else {
				dar = new Array(1, 1, 1);
			}
		}
		if (tdata.length > 1) {
			tar = tdata[1].replace(/\-0/g, ":").split(":");
			if (tar.length == 3) {
				tar = new Array(parseInt(tar[0]), parseInt(tar[1]) - 1, parseInt(tar[2]));
			}
			else {
				tar = new Array(0, 0, 0);
			}
		}
		return new Date(dar[0], dar[1], dar[2], tar[0], tar[1], tar[2]);
	},

	showError: function (target, isValt) {
		$HGClientMsg.stop(this.errorMessages.join('<br/>'), '', $NT.getText("SOAWebControls", "输入非法"));
		this.errorMessages = new Array();
	},

	onlyNum: function (e) {
		//if (event.shiftKey || !(event.keyCode == 8 || event.keyCode == 9 || (event.keyCode >= 48 &&
		//    event.keyCode <= 57) || (event.keyCode >= 96 && event.keyCode <= 105))) {
		//    event.returnValue = false;
		//}

		e.target.value = e.target.value.toString().replace(/[^,(0-9)|-]/g, '');

		this.numericCheck(e.target.value);
	},

	onlyNumKeyPress: function (e) {
		var value = e.target.value;
		if (value.indexOf('-') > -1) {
			if ((e.rawEvent.keyCode < 48 || e.rawEvent.keyCode > 57)) {
				e.rawEvent.keyCode = 0;
				e.rawEvent.returnValue = false;
			}
		}
		else {
			if ((e.rawEvent.keyCode < 48 || e.rawEvent.keyCode > 57) && e.rawEvent.keyCode != 45) {
				e.rawEvent.keyCode = 0;
				e.rawEvent.returnValue = false;
			}
		}
	},

	onlyFloatKeyPress: function (e) {
		var value = e.target.value;
		if (value.indexOf('-') > -1) {
			if ((e.rawEvent.keyCode < 48 || e.rawEvent.keyCode > 57) && e.rawEvent.keyCode != 46) {
				e.rawEvent.keyCode = 0;
				e.rawEvent.returnValue = false;
			}
		}
		else {
			if ((e.rawEvent.keyCode < 48 || e.rawEvent.keyCode > 57) && e.rawEvent.keyCode != 46 && e.rawEvent.keyCode != 45) {
				e.rawEvent.keyCode = 0;
				e.rawEvent.returnValue = false;
			}
		}
	},

	onlyFloat: function (e) {

		//if (event.shiftKey || !(event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 110 || event.keyCode == 190 || (event.keyCode >= 48 &&
		//    event.keyCode <= 57) || (event.keyCode >= 96 && event.keyCode <= 105))) {
		//    event.returnValue = false;
		//}

		e.target.value = e.target.value.toString().replace(/[^\.,(0-9)|-]/g, '');

		this.numericCheck(e.target.value);
	},

	numericCheck: function (nr) {
		nr = nr.replace(/,/g, '');  //convert to int
		var nArgs = arguments.length;
		var nCount = 0;
		var nPointIndex = -1;
		var nSignIndex = -1;
		for (var i = 0; i < nr.length; i++) {
			var ch = nr.substr(i, 1);
			if (ch < "0" || ch > "9") {
				if (ch == ".") {
					if (nPointIndex != -1) {
						$HGClientMsg.stop($NT.getText("SOAWebControls", "数字类型只能有一个小数点"), '', $NT.getText("SOAWebControls", "输入非法"));
						return false;
					}
					else
						nPointIndex = i;
				}
				else if (ch == "-" || ch == "+") {
					if (nSignIndex != -1) {
						$HGClientMsg.stop(String.format($NT.getText("SOAWebControls", "数字类型只能有一个\"{0}\""), ch), '', $NT.getText("SOAWebControls", "输入非法"));
						return false;
					}
					else
						nSignIndex = i;
				}
				else if (ch != ",")	//过滤掉数字
				{
					$HGClientMsg.stop($NT.getText("SOAWebControls", "必须输入合法的数字"), '', $NT.getText("SOAWebControls", "输入非法"));
					return false;
				}
			}
		}

		if (nPointIndex == -1)
			nPointIndex = nr.length;

		if (nArgs > 1)	//参数个数大于1
		{
			var nNumber = nr * 1;
			var intDigit = arguments[1];
			var fracDigit;
			var minValue;
			var maxvalue;

			if (nArgs > 2) {
				fracDigit = arguments[2];
				if (nArgs > 3) {
					minValue = arguments[3];
					if (nArgs > 4)
						maxValue = arguments[4];
				}
			}
		}

		if (typeof (intDigit) != "undefined" && (nr.substring(0, nPointIndex) * 1).toString().length > intDigit) {
			$HGClientMsg.stop(String.format($NT.getText("SOAWebControls", "整数部分的位数不能超过{0}位"), intDigit), '', '输入非法');
			return false;
		}

		var strFrac = nr.substring(nPointIndex + 1, nr.length);

		if (strFrac.length > 0) {
			strFrac = "0." + strFrac;
			if (typeof (fracDigit) != "undefined" && (strFrac * 1).toString().length - 2 > fracDigit) {
				$HGClientMsg.stop(String.format($NT.getText("SOAWebControls", "小数部分的位数不能超过{0}位"), fracDigit), '', '输入非法');
				return false;
			}
		}

		if (typeof (minValue) != "undefined" && typeof (maxValue) != "undefined") {
			if ((nr * 1) < minValue || (nr * 1) > maxValue) {
				$HGClientMsg.stop(String.format($NT.getText("SOAWebControls", "数字必须在{0}和{1}之间"), minValue, maxValue), '', '输入非法');
				return false;
			}
		}
		else if (typeof (minValue) != "undefined") {
			if ((nr * 1) < minValue) {
				$HGClientMsg.stop(String.format($NT.getText("SOAWebControls", "数字必须大于等于{0}"), minValue), '', '输入非法');
				return false;
			}
		}
		else if (typeof (maxValue) != "undefined") {
			if ((nr * 1) > maxValue) {
				$HGClientMsg.stop(String.format($NT.getText("SOAWebControls", "数字必须小于等于{0}"), maxValue), '', '输入非法');
				return false;
			}
		}
		return true;
	},

	formatData: function (e) {
		var validControl = this.findvalidControl(e.target);

		if (validControl == null)
			return;

		e.target.value = $HGRootNS.Formatter.pictureFormat(e.target.value, validControl[1].FormatString);
	}
}

$HBRootNS.DataBindingControl.checkBindingControlDataByGroup = function (groupID) {
	var isValidate = true;
	var allErrorMessages = [];

	for (var controlID in Sys.Application._components) {
		var cValidate = true;

		if (Sys.Application._components[controlID].checkAllData) {
			cValidate = Sys.Application._components[controlID].checkAllData(groupID);

			if (!cValidate) {
				for (var i = 0; i < Sys.Application._components[controlID].errorMessages.length; i++)
					allErrorMessages.push(Sys.Application._components[controlID].errorMessages[i]);

				isValidate = false;
			}
		}
	}

	return { isValid: isValidate, errorMessages: allErrorMessages };
}

$HBRootNS.DataBindingControl.checkBindingControlData = function (val, args) {
	var result = $HBRootNS.DataBindingControl.checkBindingControlDataByGroup();

	if (args) {
		if (!result.isValid) {
			$HGClientMsg.stop(result.errorMessages.join('<br/>'), '', $NT.getText("SOAWebControls", "输入非法"));
		}
		args.IsValid = result.isValid;
	}

	return result;
}

$HBRootNS.DataBindingControl.registerClass($HBRootNSName + ".DataBindingControl", $HGRootNS.ControlBase);

//************************DataBindingControl*******************************}