﻿
$HBRootNS.CommonAutoCompleteWithSelectorControl = function (element) {
    $HBRootNS.CommonAutoCompleteWithSelectorControl.initializeBase(this, [element]);
    this._dataSourceType = null;
}

$HBRootNS.CommonAutoCompleteWithSelectorControl.prototype =
{
	initialize: function () {
		$HBRootNS.CommonAutoCompleteWithSelectorControl.callBaseMethod(this, 'initialize');

		this._autoCompleteControl = $find(this._autoCompleteID);

		if (this._autoCompleteControl) {
			this._autoCompleteControl._onMethodComplete = Function.createDelegate(this, this._onAutoCompleteMethodComplete);
		}
	},

	dispose: function () {
		this._dataSourceType = null;
		$HBRootNS.CommonAutoCompleteWithSelectorControl.callBaseMethod(this, "dispose");
	},

	_get_ItemSpanID: function (obj) {
		var result = null;
		var dataKeyName = this.get_dataKeyName();
		if (obj && obj[dataKeyName])
			result = "spn_" + obj[dataKeyName];

		return result;
	},

	_checkInMask: function (obj) {
		var handler = this.get_events().getHandler("onCheckDataMask");
		if (handler) {
			var e = new Sys.EventArgs();
			e.Data = obj;
			handler(this, e);
			if (e.returnValue) {
				return e.returnValue;
			}
		}
		return true;
	},

	_fillItemSpanAttributes: function (obj, aSpan) {
		//		var img = document.createElement("img");
		//		img.src = StatusImageDict["imnhdr.gif"];
		//		img.style["vertical-align"] = "middle";
		//		ChangeImgToImnElement(img, "sip:zhshen@microsoft.com");

		//		window.setTimeout(function () { ProcessImnMarkers([img]); }, 10);
		//		newSpan.insertBefore(img);

		var dataDisplayProp = this.get_dataDisplayPropName();
		var dataDescriptionName = this.get_dataDescriptionPropName();
		with (aSpan) {
			if (obj[dataDescriptionName])
				title = obj[dataDescriptionName];
			else
				title = obj[dataDisplayProp];

			innerText = obj[dataDisplayProp] + ";";
		}
		aSpan["data"] = this._convertObjectToPropertyStr(obj);
	},

	_onAutoCompleteMethodComplete: function (result, context, methodName) {
		this._autoCompleteControl._isInvoking = false;

		if (result != null && result.length == 2) {
			var data = result[0];
			var dataType = result[1];
			if (data != null) {
				if (data.length > 0) {
					for (var i = 0; i < data.length; i++) {
						data[i].__type = dataType;
					}
					this._autoCompleteControl._showCompletionList(context, data, /* cacheResults */true, this.get_popupListWidth());
				}
				else {
					data.__type = dataType;
					this._autoCompleteControl._validateInput(data);
				}
			}
		}

		this._autoCompleteControl._waitingImageElement.style.display = "none";
		this._autoCompleteControl._checkImageElement.style.display = "inline";
	},

	//回掉后台验证成功后调用这个进行处理，如果结果为1个则，直接创建SPAN并显示，多个则弹出对话框让用户进行选择
	_onValidateInvokeComplete: function (result) {
		this._setInvokingStatus(false);

		if (this._autoCompleteControl) {
			this._autoCompleteControl.set_isInvoking(false);
		}

		var obj = null;

		if (result && result.length == 2) {
			var data = result[0];
			var dataType = result[1];
			if (data.length > 1)//多于一个
			{
				for (var i = 0; i < data.length; i++) {
					data[i].__type = dataType;
				}

				var sFeature = "dialogWidth:500px; dialogHeight:300px;center:yes;help:no;resizable:yes;scroll:no;status:no";

				data.nameTable = $NT;
				data.keyName = this.get_dataKeyName();
				data.displayPropName = this.get_dataDisplayPropName();
				data.descriptionPropName = this.get_dataDescriptionPropName(); 

				var resultStr = window.showModalDialog(this.get_selectObjectDialogUrl(), data, sFeature);

				obj = data[resultStr];
			}
			else {
				obj = data[0];
				obj.__type = dataType;
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
		}

		this.setInputAreaText();
	},


	loadClientState: function (value) {
		if (value && value != "") {
			var state = Sys.Serialization.JavaScriptSerializer.deserialize(value);
			if (state != null && state.length) {
				this.set_selectedData(state);
			}
		}
	},

	saveClientState: function () {
		var state = "";
		var data = this.get_selectedData();
		for (var i = 0; i < data.length; i++) {
			data[i]._dataType = undefined;
		}
		if (data) {
			state = Sys.Serialization.JavaScriptSerializer.serialize(data);
		}
		return state;
	},

	dataBind: function () {
		this.setInputAreaText();
	},

	add_checkDataMask: function (handler) {
		this.get_events().addHandler("onCheckDataMask", handler);
	},

	remove_checkDataMask: function (handler) {
		this.get_events().removeHandler("onCheckDataMask", handler);
	}
}

$HBRootNS.CommonAutoCompleteWithSelectorControl.registerClass($HBRootNSName + ".CommonAutoCompleteWithSelectorControl", $HGRootNS.AutoCompleteWithSelectorControlBase);