
$HBRootNS.OuUserInputControl = function (element) {
	$HBRootNS.OuUserInputControl.initializeBase(this, [element]);

	//    this._canSelectRoot = true;
	//    this._showSideLine = true;
	this._mergeSelectResult = false;
	this._showDeletedObjects = false;

	this._listMask = $HBRootNS.UserControlObjectMask.All;
	this._selectMask = $HBRootNS.UserControlObjectMask.All;
	this._ouImg = null;

	this._selectObjectDialogUrl = null; //选择对象的对话框地址
	this._mouseSelectItemId = ""; //鼠标选择的对象ID

	this._rootPath = "";
	this._userOUGraphControlID = "";
	this._userOUGraphControl = null;
	this._allowSelectDuplicateObj = false; //是否允许选择重复的人员
	this._enableUserPresence = true;

	this._transactDialogData$delegate = Function.createDelegate(this, this._transactDialogData);
}

$HBRootNS.OuUserInputControl.prototype =
{
	initialize: function () {
		$HBRootNS.OuUserInputControl.callBaseMethod(this, 'initialize');

		this._userOUGraphControl = $find(this._userOUGraphControlID);
		if (this._userOUGraphControl) {
			this._userOUGraphControl.add_dialogConfirmed(this._transactDialogData$delegate);
			this._userOUGraphControl._dialogResult = this.get_selectedData();

			this._userOUGraphControl.set_rootPath(this._rootPath);
			this._userOUGraphControl.set_listMask(this._listMask);
			this._userOUGraphControl.set_selectMask(this._selectMask);
			this._userOUGraphControl.set_enableUserPresence(this._enableUserPresence);
		}
	},

	dispose: function () {
		$HBRootNS.OuUserInputControl.callBaseMethod(this, "dispose");
	},

	//重载。创建名称的span的内容之前。可以插入图标之类的元素
	_beforeCreateItemSpanWithID: function (obj, container) {
		if (this.get_enableUserPresence()) {
			var div = document.createElement("div");

			div.style.verticalAlign = "middle";
			ChangeDivToImnElement(div, obj.clientContext.IMAddress);
			container.appendChild(div);

			window.setTimeout(function () { ProcessImnMarkersByDiv([div]); }, 10);
		}
	},

	/// <summary>
	/// 根据_selectedData中的数据设置显示的文本。
	/// </summary>
	setInputAreaText: function () {
		$HBRootNS.OuUserInputControl.callBaseMethod(this, "setInputAreaText");

		if (this._userOUGraphControl)
			this._userOUGraphControl._dialogResult = this.get_selectedData();
	},

	/// <summary>
	/// 处理从对话框选择过来的信息
	/// </summary>
	_transactDialogData: function (sender) {
		var objs = sender.get_selectedObjects(); //得到在对话框中勾选的组织机构人员

		var thisControl = $get(this._inputAreaClientID);
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

	get_allowSelectDuplicateObj: function () {
		return this._allowSelectDuplicateObj;
	},

	set_allowSelectDuplicateObj: function (value) {
		this._allowSelectDuplicateObj = value;
	},

	get_selectMask: function () {
		return this._selectMask;
	},

	set_selectMask: function (value) {
		this._selectMask = value;

		if (this._userOUGraphControl) {
			this._userOUGraphControl.set_selectMask(value);
		}
	},

	get_listMask: function () {
		return this._listMask;
	},

	set_listMask: function (value) {
		this._listMask = value;

		if (this._userOUGraphControl) {
			this._userOUGraphControl.set_listMask(value);
		}
	},

	set_multiSelect: function (value) {
		$HGRootNS.OuUserInputControl.callBaseMethod(this, "set_multiSelect", [value]);

		if (this._userOUGraphControl) {
			this._userOUGraphControl.set_multiSelect(value);
		}
	},

	get_mergeSelectResult: function () {
		return this._mergeSelectResult;
	},

	set_mergeSelectResult: function (value) {
		this._mergeSelectResult = value;

		if (this._userOUGraphControl) {
			this._userOUGraphControl.set_mergeSelectResult(value);
		}
	},

	get_showDeletedObjects: function () {
		return this._showDeletedObjects;
	},

	set_showDeletedObjects: function (value) {
		this._showDeletedObjects = value;

		if (this._userOUGraphControl) {
			this._userOUGraphControl.set_showDeletedObjects(value);
		}
	},

	get_enableUserPresence: function () {
		return this._enableUserPresence;
	},

	set_enableUserPresence: function (value) {
		this._enableUserPresence = value;

		if (this._userOUGraphControl) {
			this._userOUGraphControl.set_enableUserPresence(value);
		}
	},

	get_selectObjectDialogUrl: function () {
		return this._selectObjectDialogUrl;
	},

	set_selectObjectDialogUrl: function (value) {
		this._selectObjectDialogUrl = value;
	},

	dataBind: function () {
		this.setInputAreaText();
	},

	get_selectedOuUserData: function () {
		return this.get_selectedData();
	},

	set_selectedOuUserData: function (value) {
		this.set_selectedData(value);
	},

	get_userOUGraphControlID: function () {
		return this._userOUGraphControlID;
	},

	set_userOUGraphControlID: function (value) {
		this._userOUGraphControlID = value;
	},

	get_ouImg: function () {
		return this.get_selectorImg();
	},

	set_ouImg: function (value) {
		this.set_selectorImg(value);
	},

	get_cloneableProperties: function () {
		var baseProperties = $HGRootNS.OuUserInputControl.callBaseMethod(this, "get_cloneableProperties");
		var currentProperties = ["showSideLine", "userImg", "ouUserImg", "selectedOuUserData", "rootPath", "selectMask", "listMask", "enableUserPresence"];

		Array.addRange(currentProperties, baseProperties);

		return currentProperties;
	},

	get_rootPath: function () {
		return this._rootPath;
	},

	set_rootPath: function (value) {
		this._rootPath = value;

		if (this._userOUGraphControl) {
			this._userOUGraphControl.set_rootPath(value);
		}
	},

	_prepareCloneablePropertyValues: function (newElement) {
		var properties = $HGRootNS.OuUserInputControl.callBaseMethod(this, "_prepareCloneablePropertyValues", [newElement]);

		var userOUGraphControl = $find(this._userOUGraphControlID).cloneAndAppendToContainer(newElement);

		userOUGraphControl.get_events()._list["dialogConfirmed"] = undefined;

		properties["userOUGraphControlID"] = userOUGraphControl.get_element().id;

		var newOuBtn = $get(properties["ouBtnClientID"]);
		if (newOuBtn != null) {
			$addHandler(newOuBtn, "click", new Function("$find('" + properties["userOUGraphControlID"] + "').showDialog();"));
		}

		return properties;
	},

	_fillItemSpanAttributes: function (obj, aSpan) {
		with (aSpan) {
			if (obj.fullPath)
				title = obj.fullPath;
			else
				title = obj.displayName;

			innerText = obj.displayName + ";";
		}
		aSpan["data"] = this._convertObjectToPropertyStr(obj);
	},

	_get_ItemSpanID: function (obj) {
		var result = null;

		if (obj && obj.id)
			result = "spn_" + obj.id;

		return result;
	},

	_checkInMask: function (obj) {
		var mask = this.get_selectMask();
		return (mask & obj.objectType) > 0;
	},

	checkData: function () {
		if (this._autoCompleteControl) {
			if (this._autoCompleteControl.get_isInvoking()) {
				return;
			}
		}

		var spanInput = $get(this._inputAreaClientID);
		var thisControl = $get(this._inputAreaClientID);
		var sTmpText = thisControl.innerText;
		var spans = spanInput.getElementsByTagName("SPAN");

		if (null != spans && spans.length > 0) {
			for (var i = 0; i < this.get_selectedOuUserData().length; i++) {
				sTmpText = sTmpText.replace(this.get_selectedOuUserData()[i].displayName + ";", "");
			}
		}
		this._tmpText = sTmpText;

		if (sTmpText != "") {
			this._staticInvoke("CheckInputOuUser", [sTmpText], Function.createDelegate(this, this._onValidateInvokeComplete), '', true);
			if (this._autoCompleteControl) {
				this._autoCompleteControl.set_isInvoking(true);
			}
		}

		for (var i = 0; i < spanInput.childNodes.length; i++) {
			if (typeof (spanInput.childNodes[i].id) == 'undefined' || spanInput.childNodes[i].id.length <= 0) {
				return false;
			}
		}

		return true;
	},

	_onPopupItemSelected: function (sender, e) {
		e.cancel = true;

		//重新绘制列表
		var spanInput = $get(this._inputAreaClientID);

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

				var span = this._createItemSpan(this.get_selectedOuUserData()[i]);

				spanInput.insertBefore(span, spanInput.childNodes[i]);

				spanInput.removeChild(spanInput.childNodes[i + 1]);
			}
		}

		this.notifyDataChanged();
	},

	_spanInputKeyUp: function () {
		$HGRootNS.OuUserInputControl.callBaseMethod(this, "_spanInputKeyUp");

		//更改userOUGraphControl控件中选中的数据。
		if (this._userOUGraphControl) {
			this._userOUGraphControl._dialogResult = this.get_selectedOuUserData();
		}
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

		var spanInput = $get(this._inputAreaClientID);

		var fsCS = new Array(1);
		fsCS[0] = this.get_selectedOuUserData();
		fsCS[1] = spanInput.innerText;

		return Sys.Serialization.JavaScriptSerializer.serialize(fsCS);
	},

	//判断用户是否已经在现有列表中出现
	_checkUserInList: function (sid) {
		var blnResult = false;
		var datas = this.get_selectedOuUserData();
		if (datas.length > 0 && datas[0]) {
			for (var i = 0; i < datas.length; i++) {
				if (datas[i].id == sid) {
					blnResult = true;
					break;
				}
			}
		}
		return blnResult;
	},

	//检测是否重复
	_onAutoPopShowing: function (autoControl, e) {
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
		this.setInputAreaText();
	},

	///没用
	RI: function () {
	}
}

$HBRootNS.OuUserInputControl.registerClass($HBRootNSName + ".OuUserInputControl", $HGRootNS.AutoCompleteWithSelectorControlBase);