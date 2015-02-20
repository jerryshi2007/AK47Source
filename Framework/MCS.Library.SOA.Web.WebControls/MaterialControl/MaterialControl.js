// -------------------------------------------------
// FileName	：	MaterialControl.js
// Remark	：	附件控件
// -------------------------------------------------
// VERSION		AUTHOR		DATE			CONTENT
// 1.0			张梁			20070725		创建
// -------------------------------------------------

//material在不同流程环节下的控制模式
$HBRootNS.materialActivityControlMode = function () {
	throw Error.invalidOperation();
}

$HBRootNS.materialActivityControlMode.prototype =
{
	None: 0, 			//正常
	AllowDelete: 1, 	//允许删除不属于此环节的附件
	AllowEdit: 2, 	//允许编辑不属于此环节的附件的属性
	AllowEditContent: 4	//允许编辑不属于此环节的附件的内容
}

$HBRootNS.materialActivityControlMode.registerEnum($HBRootNSName + ".materialActivityControlMode");

//显示方式
$HBRootNS.materialTableShowMode = function () {
	throw Error.invalidOperation();
};
$HBRootNS.materialTableShowMode.prototype =
{
	Inline: 1, 	//单行
	Vertical: 2	//多行
}
$HBRootNS.materialTableShowMode.registerEnum($HBRootNSName + ".materialTableShowMode");

//使用方式
$HBRootNS.materialUseMode = function () {
	throw Error.invalidOperation();
};

$HBRootNS.materialUseMode.prototype =
{
	SingleDraft: 1, 	//起草单个文件
	DraftAndUpload: 2, 	//起草一个文件，上传多个文件
	UploadFile: 3			//上传文件
}

$HBRootNS.materialUseMode.registerEnum($HBRootNSName + ".materialUseMode");

//文件选择方式

$HBRootNS.fileSelectMode = function () {
	throw Error.invalidOperation();
};

$HBRootNS.fileSelectMode.prototype =
{
	TraditionalSingle: 1, 	//传统单文件
	MultiSelectUseActiveX: 2 	//ActiveX多选
}

$HBRootNS.fileSelectMode.registerEnum($HBRootNSName + ".fileSelectMode");

//"添加或修改" 显示为文字还是图片
$HBRootNS.linkShowMode = function () {
	throw Error.invalidOperation();
};

$HBRootNS.linkShowMode.prototype =
{
	Text: 1, 	//显示为文字链接
	Image: 2, 	//显示为图片	
	ImageAndText: 3    //图片在前 文字在后
}

$HBRootNS.linkShowMode.registerEnum($HBRootNSName + ".linkShowMode");

$HBRootNS.MaterialControl = function (element) {
	$HBRootNS.MaterialControl.initializeBase(this, [element]);

	this._caption = "添加或修改"; 				//显示的标题
	this._captionImage = "添加或修改"; 			//显示的标题图片
	this._linkShowMode = $HBRootNS.linkShowMode.Text;
	this._draftText = "起草"; 					//起草时显示的文字
	this._saveOriginalDraft = true; 			//是否保存原始草稿
	this._editText = "编辑"; 					//编辑时显示的文字
	this._displayText = "显示"; 					//正文只读时显示的文字
	this._materialTitle = "正文";                   //新起草文件的默认名字
	this._allowEditContent = true; 				//是否允许修改内容
	this._allowEdit = true; 						//是否允许修改标题等属性
	this._trackRevisions = false; 				//是否保留修改痕迹
	this._showFileTitle = true; 					//是否显示文件标题
	this._modifyCheck = true; 					//是否在离开页面时检查文件修改后未保存
	this._showAllVersions = false; 				//是否显示所有版本
	this._templateUrl = ""; 						//模板文件路径
	this._materials = new Array(); 				//Material对象集合
	this._deltaMaterials = null; 				//保存文件操作结果的集合
	this._user = null; 							//当前用户
	this._department = null; 					//当前用户所在部门
	this._defaultResourceID = null; 				//表单ID
	this._defaultClass = null; 					//类别ID
	this._lockID = null; 						//lockID
	this._rootPathName = null; 					//服务器上保存文件的根路径的配置节点名称
	this._relativePath = null; 					//服务器上保存文件的目录(相对路径)
	this._wfProcessID = null; 					//工作流流程ID
	this._wfActivityID = null; 					//工作流活动ID
	this._wfActivityName = null; 				//工作流活动名称
	this._materialUseMode = $HBRootNS.materialUseMode.UploadFile; //控件使用方式
	this._fileSelectMode = $HBRootNS.fileSelectMode.TraditionalSingle; //文件选择方式
	this._materialTableShowMode = $HBRootNS.materialTableShowMode.Vertical; //显示样式	
	this._controlID = null; 						//控件ID
	this._uniqueID = null; 					//控件唯一ID
	this._dialogUploadFileControlUrl = null; 	//弹出对话框的路径
	this._defaultFileIconPath = null; 			//默认文件图标路径
	this._showUploadDialogBtnObj = null; 		//弹出窗口的按钮
	this._draftBtn = null; 						//起草文件的按钮
	this._materialTable = null; 					//显示material的table对象
	this._editImagePath = null; 					//表示编辑状态的图标路径
	this._setOpenTypeImagePath = null; 			//显示设置文件默认打开方式的页面图标路径
	this._captionImagePath = null;                   //"添加或修改"显示为图片时的图标路径
	this._setOpenTypeImage = null; 				//显示设置文件默认打开方式的页面图标
	this._dialogFileOpenTypeControlUrl = null; 	//显示设置文件默认打开方式的页面路径
	this._uploadImagePath = null; 				//表示未上传状态的图标路径
	this._showVersionImagePath = null; 			//显示版本的图标路径
	this._dialogUploadFileProcessControlUrl = null; //等待文件上传的页面路径
	this._dialogVersionControlUrl = null; 		//显示版本的页面地址
	this._emptyImagePath = null; 				//空白图片的路径,用于显示没有文件时的文件图标
	this._currentPageUrl = null; 				//页面地址
	this._showFileOpenType = false; 				//是否显示设置文件打开方式
	this._showDialogBtnContainer = null; 		//显示showDialogBtn的span
	this._designatedControlToShowDialog = false;    //是否指定了弹出对话框的控件
	this._enabled = true;                          //是否可用
	this._fileExts = "";                            //可以上传的文件的拓展名
	this._fileMaxSize = 0;                          //上传文件的大小限制　单位是字节
	this._autoCheck = true;                         //是否在页面关闭的时候自动校验文件正在打开或者曾经修改
	this._requestContext = null; 				//上传或下载请求中的context
	this._editDocumentInCurrentPage = false;        //是否在浏览器内编辑文档
	this._autoOpenDocument = false;                 //是否自动打开文档
	this._officeViewerWrapperID = "";
	this._officeViewerWrapper = null;
	this._officeViewerWrapperViewer = null;
	this._dialogEditDocumentControlUrl = null;
	this._currentEditingMaterial = null;
	this._officeViewerShowToolBars = true;

	this._downloadTemplateWithViewState = false;
	this._downloadWithViewState = false;

	//附件在流转环节方面的控制
	this._activityControlMode = $HBRootNS.materialVersionType.AllowEditContent;

	this._showUploadDialogBtnEvents =
	{
		click: Function.createDelegate(this, this._showUploadDialogBtnClick)
	};

	this._draftBtnEvents =
	{
		click: Function.createDelegate(this, this._draftBtnClick)
	};

	this._showFileOpenTypeDiagEvents =
	{
		click: Function.createDelegate(this, this._showFileOpenTypeDiagBtnClick)
	}
}

$HBRootNS.MaterialControl.prototype =
{
	initialize: function () {
		$HBRootNS.MaterialControl.callBaseMethod(this, "initialize");

		try {
			if (this._templateUrl != "" && this._materialUseMode == $HBRootNS.materialUseMode.SingleDraft
			&& this._materials.length == 0 && this._allowEditContent == true)
				this._createDraftMaterial();
		}
		catch (e) {
			alert("初始化附件控件出错——" + e.message);
		}

		this._buildControl();

		$HBRootNS.material.initDownloadFileFrame();

		if (this._autoOpenDocument == true) {
			if (this._materialUseMode == $HBRootNS.materialUseMode.SingleDraft && this._materials.length == 1) {
				this._materials[0].click();
			}
		}

		//this.add_onBeforDocumentSaved(Function.createDelegate(this, this.set_MaterialModified));
	},

	dispose: function () {
		this._materialTable = null;
		this._materials = null;
		this._deltaMaterials = null;
		this._user = null;
		this._department = null;
		this._setOpenTypeImage = null;

		if (this._showUploadDialogBtnObj) {
			$HGDomEvent.removeHandlers(this._showUploadDialogBtnObj, this._showUploadDialogBtnEvents);
			this._showUploadDialogBtnObj = null;
		}

		if (this._setOpenTypeImage) {
			$HGDomEvent.removeHandlers(this._setOpenTypeImage, this._showFileOpenTypeDiagEvents);
			this._setOpenTypeImage = null;
		}

		if (this._draftBtn) {
			$HGDomEvent.removeHandlers(this._draftBtn, this._draftBtnEvents);
			this._draftBtn = null;
		}

		this._showDialogBtnContainer = null;

		$HBRootNS.MaterialControl.callBaseMethod(this, "dispose");
	},

	_showUploadDialogBtn: function (container) {
		if (this._materialUseMode != $HBRootNS.materialUseMode.SingleDraft && this._allowEdit && this._designatedControlToShowDialog == false) {
			this._showUploadDialogBtnObj = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "a",
				properties:
			    {
			    	href: "#",
			    	style:
				    {
				    	cursor: "pointer"
				    }
			    },
				cssClasses: ["materialControlUploadBtn"],
				events: this._showUploadDialogBtnEvents
			},
			container
			);

			if (this._linkShowMode == $HBRootNS.linkShowMode.Text) {
				this._showUploadDialogBtnObj.innerText = this._caption;
			}
			else {
				$HGDomElement.createElementFromTemplate(
			    {
			    	nodeName: "img",
			    	properties:
			        {
			        	src: this._captionImagePath,
			        	title: this._caption,
			        	border: "0",
			        	align: "absmiddle"
			        }
			    },
			    this._showUploadDialogBtnObj
			    );

				if (this._linkShowMode == $HBRootNS.linkShowMode.ImageAndText) {
					$HGDomElement.createTextNode(" " + this._caption, this._showUploadDialogBtnObj, $HGDomElement.get_currentDocument());
				}
			}
		}
	},

	_showDraftBtn: function (container) {
		if (this._templateUrl != "" && this._materialUseMode == $HBRootNS.materialUseMode.DraftAndUpload && this._allowEdit) {
			$HGDomElement.createTextNode(" ", this._showDialogBtnContainer, $HGDomElement.get_currentDocument());

			this._draftBtn = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "a",
				properties:
				{
					innerText: this._draftText,
					href: "#",
					style:
					{
						cursor: "pointer"
					}
				},
				events: this._draftBtnEvents
			},
			container
			);
		}
	},

	_draftBtnClick: function () {
		var material = this._createDraftMaterial();

		material.render();
		material.click();

		this._showMaterials();
	},

	_showSetOpenTypeImage: function (container) {
		if (this._materialUseMode != $HBRootNS.materialUseMode.SingleDraft && this._showFileOpenType == true) {
			this._setOpenTypeImage = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "img",
				properties:
				{
					src: this._setOpenTypeImagePath,
					title: "点击此处设置文件的打开方式",
					align: "absmiddle",
					style:
					{
						cursor: "pointer"
					}
				},
				events: this._showFileOpenTypeDiagEvents
			},
			container
			);
		}
	},

	_showFileOpenTypeDiagBtnClick: function () {
		var arg = "dialogHeight : 420px; dialogWidth : 480px; edge : Raised; center : Yes; help : No; resizable : No; status : No; scroll : no";

		var openInlineFileExt = window.showModalDialog(this._dialogFileOpenTypeControlUrl, this._user.id, arg);

		if (openInlineFileExt)
			$HBRootNS.material.openInlineFileExt = openInlineFileExt;
	},

	_buildControl: function () {
		var element = this.get_element();

		if (this._allowEdit) {
			this._showDialogBtnContainer = $HGDomElement.createElementFromTemplate({ nodeName: "span" }, element);

			if (this._designatedControlToShowDialog == false) {

				this._showSetOpenTypeImage(this._showDialogBtnContainer);
				this._showUploadDialogBtn(this._showDialogBtnContainer);
			}

			this._showDraftBtn(this._showDialogBtnContainer);
		}
		else if (this._materialTableShowMode == $HBRootNS.materialTableShowMode.Vertical) {
			this._showDialogBtnContainer = $HGDomElement.createElementFromTemplate({ nodeName: "span" }, element);
		}

		this._buildMaterialTable(element);

		this._showMaterials();
	},

	_createDraftMaterial: function () {
		var properties =
			{
				materialID: $HBRootNS.material.newMaterialID,
				isDraft: true,
				container: this,
				modifyTime: new Date()
			};

		var material = $HBRootNS.material.createMaterial(properties);

		Array.add(this._materials, material);

		return material;
	},

	_showMaterials: function () {
		var tableContent = this._materialTable.firstChild;

		while (tableContent.childNodes.length > 0)
			this._materialTable.deleteRow(0);

		var tableRow, tableCell;

		if (this._materialTableShowMode == $HBRootNS.materialTableShowMode.Vertical) {
			for (var i = 0; i < this._materials.length; i++) {
				tableRow = this._buildMaterialTableRow();
				tableCell = this._buildMaterialTableCell(tableRow);
				tableCell.appendChild(this._materials[i].get_element());
				this._materials[i].render();
			}
		}
		else {
			tableRow = this._buildMaterialTableRow();
			tableCell = this._buildMaterialTableCell(tableRow);

			if (this._allowEdit == false && this._enabled == true)
				this._showSetOpenTypeImage(tableCell);

			for (var i = 0; i < this._materials.length; i++) {
				tableCell.appendChild(this._materials[i].get_element());
				this._materials[i].render();
			}
		}
	},

	//重新显示Materials
	_reRenderMaterials: function () {
		for (var i = 0; i < this._materials.length; i++) {
			this._materials[i].render();
		}
	},

	showDialog: function () {
		if (this._materialUseMode == $HBRootNS.materialUseMode.SingleDraft)
			return false;

		if (event)
			event.returnValue = false;

		this._showUploadDialogBtnClick();
	},

	_showUploadDialogBtnClick: function () {

		var information = new Object();
		information.container = this;

		var newMaterials = new Array();
		var materials = new Array();

		//未起草的文件不传给弹出窗口
		for (var i = 0; i < this._materials.length; i++) {
			if (this._materials[i].get_materialID() != $HBRootNS.material.newMaterialID && this._materials[i].get_isDraft() == false)
				Array.add(materials, this._materials[i].generateMaterial());
			else
				Array.add(newMaterials, this._materials[i]);
		}

		information.materials = Sys.Serialization.JavaScriptSerializer.serialize(materials);
		information.deltaMaterials = Sys.Serialization.JavaScriptSerializer.serialize(this._deltaMaterials);
		information.userID = this._user.id;
		information.openInlineFileExts = $HBRootNS.material.openInlineFileExt;
		information.fileExts = this._fileExts;
		information.fileMaxSize = this._fileMaxSize;
		information.fileCountLimited = this._fileCountLimited;

		var returnInformation = null;

		if (this._fileSelectMode == $HBRootNS.fileSelectMode.TraditionalSingle) {
			var arg = "dialogHeight : 450px; dialogWidth : 600px; edge : Raised; center : Yes; help : No; resizable : No; status : No; scroll : no";
			returnInformation = window.showModalDialog(this._dialogUploadFileTraditionalControlUrl, information, arg);
		}
		else if (this._fileSelectMode == $HBRootNS.fileSelectMode.MultiSelectUseActiveX) {
			var arg = "dialogHeight : 600px; dialogWidth : 600px; edge : Raised; center : Yes; help : No; resizable : No; status : No; scroll : no";
			returnInformation = window.showModalDialog(this._dialogUploadFileControlUrl, information, arg);
		}

		if (returnInformation) {
			returnInformation = Sys.Serialization.JavaScriptSerializer.deserialize(returnInformation);

			//this._dataBind(returnInformation.materials);
			this._windowdataBind(returnInformation.materials); //SZ Modified
			this._deltaMaterials = returnInformation.deltaMaterials;

			for (var i = 0; i < newMaterials.length; i++)
				Array.add(this._materials, newMaterials[i]);

			this._removeDeletedMaterialFromAllMaterials(returnInformation.deletedIDs);

			this._showMaterials();

			this.raiseMaterialsChanged();
		}
	},

	showUploadDialogByClass: function (materialClass) {
		var information = new Object();
		information.container = this;

		var newMaterials = new Array();
		var materials = new Array();
		var otherMaterials = new Array();

		//未起草的文件不传给弹出窗口
		for (var i = 0; i < this._materials.length; i++) {
			if (this._materials[i].get_materialID() != $HBRootNS.material.newMaterialID && this._materials[i].get_isDraft() == false) {
				if (this._materials[i].get_materialClass() == materialClass) {
					Array.add(materials, this._materials[i].generateMaterial());
				} else {
					Array.add(otherMaterials, this._materials[i].generateMaterial());
				}
			}
			else
				Array.add(newMaterials, this._materials[i]);
		}

		information.materials = Sys.Serialization.JavaScriptSerializer.serialize(materials);
		information.deltaMaterials = Sys.Serialization.JavaScriptSerializer.serialize(this._deltaMaterials);
		information.userID = this._user.id;
		information.openInlineFileExts = $HBRootNS.material.openInlineFileExt;
		information.fileExts = this._fileExts;
		information.fileMaxSize = this._fileMaxSize;
		information.fileCountLimited = this._fileCountLimited;

		var returnInformation = null;

		if (this._fileSelectMode == $HBRootNS.fileSelectMode.TraditionalSingle) {
			var arg = "dialogHeight : 450px; dialogWidth : 600px; edge : Raised; center : Yes; help : No; resizable : No; status : No; scroll : no";
			returnInformation = window.showModalDialog(this._dialogUploadFileTraditionalControlUrl, information, arg);
		}
		else if (this._fileSelectMode == $HBRootNS.fileSelectMode.MultiSelectUseActiveX) {
			var arg = "dialogHeight : 600px; dialogWidth : 600px; edge : Raised; center : Yes; help : No; resizable : No; status : No; scroll : no";
			returnInformation = window.showModalDialog(this._dialogUploadFileControlUrl, information, arg);
		}

		if (returnInformation) {
			returnInformation = Sys.Serialization.JavaScriptSerializer.deserialize(returnInformation);

			for (var j = 0; j < returnInformation.materials.length; j++) {
				returnInformation.materials[j].materialClass = materialClass;
			}
			for (var j = 0; j < returnInformation.deltaMaterials.insertedMaterials.length; j++) {
				returnInformation.deltaMaterials.insertedMaterials[j].materialClass = materialClass;
			}
			for (var j = 0; j < returnInformation.deltaMaterials.updatedMaterials.length; j++) {
				returnInformation.deltaMaterials.updatedMaterials[j].materialClass = materialClass;
			}
			for (var j = 0; j < returnInformation.deltaMaterials.deletedMaterials.length; j++) {
				returnInformation.deltaMaterials.deletedMaterials[j].materialClass = materialClass;
			}

			if (returnInformation.materials.length > 0) {
				otherMaterials = otherMaterials.concat(returnInformation.materials);
			}

			this._windowdataBind(otherMaterials);
			this._deltaMaterials = returnInformation.deltaMaterials;

			for (var i = 0; i < newMaterials.length; i++)
				Array.add(this._materials, newMaterials[i]);

			this._removeDeletedMaterialFromAllMaterials(returnInformation.deletedIDs);

			this._showMaterials();
		}
	},

	//SZ Add
	_windowdataBind: function (materials) {
		var insert = 1;
		var properties;
		for (var i = 0; i < materials.length; i++) //新对象
		{
			insert = 1;
			for (var j = 0; j < this._materials.length; j++)//旧对象
			{
				if (this._materials[j]._materialID == materials[i].id) {
					this._materials[j]._title = materials[i].title; //更新
					this._materials[j]._sortID = materials[i].sortID;
					this._materials[j]._pageQuantity = materials[i].pageQuantity;
					insert = 0;
				}
			} //插入
			if (insert == 1) {
				properties =
				{
					materialID: materials[i].id,
					department: materials[i].department,
					resourceID: materials[i].resourceID,
					sortID: materials[i].sortID,
					materialClass: materials[i].materialClass,
					title: materials[i].title,
					pageQuantity: materials[i].pageQuantity,
					relativeFilePath: materials[i].relativeFilePath,
					originalName: materials[i].originalName,
					creator: materials[i].creator,
					lastUploadTag: materials[i].lastUploadTag,
					createDateTime: materials[i].createDateTime,
					modifyTime: materials[i].modifyTime,
					wfProcessID: materials[i].wfProcessID,
					wfActivityID: materials[i].wfActivityID,
					wfActivityName: materials[i].wfActivityName,
					parentID: materials[i].parentID,
					sourceMaterial: materials[i].sourceMaterial,
					versionType: materials[i].versionType,
					extraData: materials[i].extraData,
					fileIconPath: materials[i].fileIconPath,
					showFileUrl: materials[i].showFileUrl,
					isDraft: materials[i].hasOwnProperty("_isDraft") ? materials[i].get_isDraft() : false,
					fileIconPath: materials[i].fileIconPath,
					container: this
				};
				Array.add(this._materials, $HBRootNS.material.createMaterial(properties));
			}
		}
		var deleteflag = 1; // 删除
		for (var k = this._materials.length - 1; k >= 0; k--)//旧对象
		{
			deleteflag = 1;
			for (var m = materials.length - 1; m >= 0; m--)//新对象
			{
				if (this._materials[k]._materialID == materials[m].id) {
					deleteflag = 0;
				}
			}
			if (deleteflag == 1)//移除
			{
				Array.remove($HBRootNS.material.allMaterials, this._materials[k]);
				Array.remove(this._materials, this._materials[k]);
			}
		}
	},

	_removeDeletedMaterialFromAllMaterials: function (deletedIDs) {
		for (var i = 0; i < deletedIDs.length; i++) {
			this._removeFromAllMaterialsById(deletedIDs[i]);
		}
	},

	_removeFromAllMaterialsById: function (id) {
		for (var i = $HBRootNS.material.allMaterials.length - 1; i >= 0; i--) {
			if ($HBRootNS.material.allMaterials[i].get_materialID() == id)
				Array.removeAt($HBRootNS.material.allMaterials, i);
		}
	},

	_generateMultiMaterial: function () {
		var multiMaterial = new Object();
		multiMaterial.materials = new Array();
		multiMaterial.deltaMaterials = this._deltaMaterials;

		var material;
		var indexInInserted, indexInUpdated;

		for (var i = 0; i < this._materials.length; i++) {
			if (this._materials[i].get_materialID() == $HBRootNS.material.newMaterialID
				|| this._materials[i].get_materialID() == "null"
				|| this._materials[i].get_materialID() == null)
				continue;

			material = this._materials[i].getResultMaterial(); //key

			if (material)
				Array.add(multiMaterial.materials, material);

			if (this._materials[i].get_uploaded() && material) {
				indexInInserted = this._findInInsertedMaterials(this._materials[i].get_materialID());
				indexInUpdated = this._findInUpdatedMaterials(this._materials[i].get_materialID());

				if (this._materials[i].get_isDraft() == true && this._materials[i].get_materialID() != $HBRootNS.material.newMaterialID && indexInInserted == -1) {
					Array.add(multiMaterial.deltaMaterials.insertedMaterials, material);
					if (this._allowEdit && this._allowEditContent == true && this._materials[i]._isOfficeDocument) {
						this._materials[i].setSaveTime();
					}
				}
				else if (indexInInserted != -1 && indexInUpdated == -1) {
					multiMaterial.deltaMaterials.insertedMaterials[indexInInserted] = material;
					if (this._allowEdit && this._allowEditContent == true && this._materials[i]._isOfficeDocument) {
						this._materials[i].setSaveTime();
					}
				}
				else if (this._materials[i].get_contentModified() == true && indexInInserted == -1) {
					Array.add(multiMaterial.deltaMaterials.updatedMaterials, material);
					if (this._allowEdit && this._allowEditContent == true && this._materials[i]._isOfficeDocument) {
						this._materials[i].setSaveTime();
					}
				}
			}
		}

		return multiMaterial;
	},

	get_materialsResult: function () {
		return this._generateMultiMaterial().materials;
	},

	_findInInsertedMaterials: function (id) {
		return this._findInList(id, this._deltaMaterials.insertedMaterials);
	},

	_findInUpdatedMaterials: function (id) {
		return this._findInList(id, this._deltaMaterials.updatedMaterials);
	},

	_findInList: function (id, list) {
		for (var i = 0; i < list.length; i++) {
			if (list[i].id == id)
				return i;
		}
		return -1;
	},

	_removeMaterial: function (materials, material) {
		if (materials != null) {
			for (var i = 0; i < materials.length; i++) {
				if (materials[i].id == material.id)
					Array.removeAt(materials, i);
			}
		}
	},

	_dataBind: function (materials) {
		Array.clear(this._materials);

		var properties;

		for (var i = 0; i < materials.length; i++) {
			properties =
				{
					materialID: materials[i].id,
					department: materials[i].department,
					resourceID: materials[i].resourceID,
					sortID: materials[i].sortID,
					materialClass: materials[i].materialClass,
					title: materials[i].title,
					pageQuantity: materials[i].pageQuantity,
					relativeFilePath: materials[i].relativeFilePath,
					originalName: materials[i].originalName,
					creator: materials[i].creator,
					lastUploadTag: materials[i].lastUploadTag,
					createDateTime: materials[i].createDateTime,
					modifyTime: materials[i].modifyTime,
					wfProcessID: materials[i].wfProcessID,
					wfActivityID: materials[i].wfActivityID,
					wfActivityName: materials[i].wfActivityName,
					parentID: materials[i].parentID,
					sourceMaterial: materials[i].sourceMaterial,
					versionType: materials[i].versionType,
					extraData: materials[i].extraData,
					fileIconPath: materials[i].fileIconPath,
					showFileUrl: materials[i].showFileUrl,
					isDraft: materials[i].hasOwnProperty("_isDraft") ? materials[i].get_isDraft() : false,
					fileIconPath: materials[i].fileIconPath,
					container: this
				};
			Array.add(this._materials, $HBRootNS.material.createMaterial(properties));
		}
	},

	saveClientState: function () {
		var multiMaterial = this._generateMultiMaterial();
		return Sys.Serialization.JavaScriptSerializer.serialize(multiMaterial);
	},

	loadClientState: function (value) {
		if (value) {
			var multiMaterial = Sys.Serialization.JavaScriptSerializer.deserialize(value);

			try {
				this._dataBind(multiMaterial.materials);
			}
			catch (e) {
				alert("初始化附件控件出错——" + e.message);
			}

			this._deltaMaterials = multiMaterial.deltaMaterials;
		}
	},

	_buildMaterialTable: function (element) {
		if (this._materialTable == null) {
			this._materialTable = $HGDomElement.createElementFromTemplate(
			{
				nodeName: "table",
				width: "100%",
				properties:
				{
					style:
					{
						wordBreak: "break-all"
					}
				}
			},
			element);

			$HGDomElement.createElementFromTemplate({ nodeName: "tbody" }, this._materialTable);
		}
	},

	_buildMaterialTableRow: function () {
		var materialTableRow = $HGDomElement.createElementFromTemplate({ nodeName: "tr" }, this._materialTable.firstChild);
		return materialTableRow;
	},

	_buildMaterialTableCell: function (materialTableRow) {
		var materialTableCell = $HGDomElement.createElementFromTemplate(
		{
			nodeName: "td",
			properties:
			{
				style:
				{
					wordWrap: "break-word"
				}
			}
		},
		materialTableRow);

		return materialTableCell;
	},

	get_activityControlMode: function () {
		return this._activityControlMode;
	},
	set_activityControlMode: function (value) {
		if (this._activityControlMode != value) {
			this._activityControlMode = value;
			this.raisePropertyChanged("activityControlMode");
		}
	},

	get_caption: function () {
		return this._caption;
	},
	set_caption: function (value) {
		if (this._caption != value && value.trim() != "") {
			this._caption = value;
			this.raisePropertyChanged("caption");

			if (this._showUploadDialogBtnObj != null) {
				if (this._linkShowMode == $HBRootNS.linkShowMode.Text) {
					this._showUploadDialogBtnObj.innerText = value;
				}
				else if (this._linkShowMode == $HBRootNS.linkShowMode.Image) {
					this._showUploadDialogBtnObj.firstChild.title = value;
				}
				else {
					this._showUploadDialogBtnObj.firstChild.title = value;
					this._showUploadDialogBtnObj.lastChild.nodeValue = " " + value;
					this._showUploadDialogBtnObj.lastChild.data = " " + value;
				}
			}
		}
	},

	get_draftText: function () {
		return this._draftText;
	},
	set_draftText: function (value) {
		if (this._draftText != value && value.trim() != "") {
			this._draftText = value;
			this.raisePropertyChanged("draftText");

			this._reRenderMaterials();
		}
	},

	get_saveOriginalDraft: function () {
		return this._saveOriginalDraft;
	},
	set_saveOriginalDraft: function (value) {
		this._saveOriginalDraft = value;
	},

	get_editText: function () {
		return this._editText;
	},
	set_editText: function (value) {
		if (this._editText != value && value.trim() != "") {
			this._editText = value;
			this.raisePropertyChanged("editText");

			this._reRenderMaterials();
		}
	},

	get_displayText: function () {
		return this._displayText;
	},

	set_displayText: function (value) {
		if (this._displayText != value && value.trim() != "") {
			this._displayText = value;
			this.raisePropertyChanged("displayText");

			this._reRenderMaterials();
		}
	},

	get_materialTableShowMode: function () {
		return this._materialTableShowMode;
	},
	set_materialTableShowMode: function (value) {
		if (this._materialTableShowMode != value) {
			this._materialTableShowMode = value;
			this.raisePropertyChanged("materialTableShowMode");

			if (this._materialTable)
				this._showMaterials();
		}
	},

	get_linkShowMode: function () {
		return this._linkShowMode;
	},
	set_linkShowMode: function (value) {
		if (this._linkShowMode != value) {
			this._linkShowMode = value;
			this.raisePropertyChanged("linkShowMode");

			if (this._showUploadDialogBtnObj)
				throw Error.create("不允许在运行状态下修改linkShowMode属性");
		}
	},

	get_materialUseMode: function () {
		return this._materialUseMode;
	},
	set_materialUseMode: function (value) {
		if (this._materialUseMode != value) {
			this._materialUseMode = value;
			this.raisePropertyChanged("materialUseMode");

			if (this._materialTable)
				throw Error.create("不允许在运行状态下修改materialUseMode属性");
		}
	},

	get_fileSelectMode: function () {
		return this._fileSelectMode;
	},
	set_fileSelectMode: function (value) {
		if (this._fileSelectMode != value) {
			this._fileSelectMode = value;
			this.raisePropertyChanged("fileSelectMode");
		}
	},


	get_allowEditContent: function () {
		return this._allowEditContent;
	},
	set_allowEditContent: function (value) {
		if (this._allowEditContent != value) {
			this._allowEditContent = value;
			this.raisePropertyChanged("allowEditContent");

			if (this._materialTable)
				this._showMaterials();
		}
	},

	get_allowEdit: function () {
		return this._allowEdit;
	},
	set_allowEdit: function (value) {
		if (this._allowEdit != value) {
			this._allowEdit = value;
			this.raisePropertyChanged("allowEdit");

			if (this._showDialogBtnContainer) {
				if (value == true) {
					if (this._showDialogBtnContainer.style.display == "none")
						this._showDialogBtnContainer.style.display = "block";
					else if (this._showUploadDialogBtnObj == null)
						this._showUploadDialogBtn(this._showDialogBtnContainer);
				}
				else
					this._showDialogBtnContainer.style.display = "none";
			}
		}
	},

	get_trackRevisions: function () {
		return this._trackRevisions;
	},
	set_trackRevisions: function (value) {
		if (this._trackRevisions != value) {
			this._trackRevisions = value;
			this.raisePropertyChanged("trackRevisions");
		}
	},

	get_modifyCheck: function () {
		return this._modifyCheck;
	},
	set_modifyCheck: function (value) {
		if (this._modifyCheck != value) {
			this._modifyCheck = value;
			this.raisePropertyChanged("modifyCheck");
		}
	},

	get_showAllVersions: function () {
		return this._showAllVersions;
	},
	set_showAllVersions: function (value) {
		if (this._showAllVersions != value) {
			this._showAllVersions = value;
			this.raisePropertyChanged("showAllVersions");

			this._reRenderMaterials();
		}
	},

	get_materials: function (isInTempFolder) {
		var materials = new Array();

		for (var i = 0; i < this._materials.length; i++) {
			if (this._materials[i].get_materialID() == $HBRootNS.material.newMaterialID)
				continue;

			var material = this._materials[i].getResultMaterial();

			if (material) {

				if (!isInTempFolder && material.showFileUrl.indexOf("Temp\\") == 0)
					material.showFileUrl = material.showFileUrl.substring(4);

				Array.add(materials, material);
			}
		}

		return materials;
	},
	set_materials: function (value) {
		if (value != null && value != "undefined" && this._materials != value) {
			if (value.length == 0 && this._templateUrl != "" && this._allowEditContent == true
				&& this._materialUseMode == $HBRootNS.materialUseMode.SingleDraft)
				Array.add(value, this._createDraftMaterial());

			this._dataBind(value);
			this._showMaterials();
			this.raisePropertyChanged("materials");
		}
	},

	get_deltaMaterials: function () {
		if (this._rootPathName)
			this._deltaMaterials.rootPathName = this._rootPathName;

		return this._deltaMaterials;
	},

	get_user: function () {
		return this._user;
	},
	set_user: function (value) {
		if (this._user != value) {
			this._user = value;
		}
		this.raisePropertyChanged("user");
	},

	get_department: function () {
		return this._department;
	},
	set_department: function (value) {
		if (this._department != value) {
			this._department = value;
			this.raisePropertyChanged("department");
		}
	},

	get_templateUrl: function () {
		return this._templateUrl;
	},
	set_templateUrl: function (value) {
		if (this._templateUrl != value) {
			this._templateUrl = value;
			this.raisePropertyChanged("templateUrl");
		}
	},

	get_editImagePath: function () {
		return this._editImagePath;
	},
	set_editImagePath: function (value) {
		if (this._editImagePath != value) {
			this._editImagePath = value;
			this.raisePropertyChanged("editImagePath");

			this._reRenderMaterials();
		}
	},

	get_uploadImagePath: function () {
		return this._uploadImagePath;
	},
	set_uploadImagePath: function (value) {
		if (this._uploadImagePath != value) {
			this._uploadImagePath = value;
			this.raisePropertyChanged("uploadImagePath");

			this._reRenderMaterials();
		}
	},

	get_showVersionImagePath: function () {
		return this._showVersionImagePath;
	},
	set_showVersionImagePath: function (value) {
		if (this._showVersionImagePath != value) {
			this._showVersionImagePath = value;
			this.raisePropertyChanged("showVersionImagePath");

			this._reRenderMaterials();
		}
	},

	get_emptyImagePath: function () {
		return this._emptyImagePath;
	},
	set_emptyImagePath: function (value) {
		if (this._emptyImagePath != value) {
			this._emptyImagePath = value;
			this.raisePropertyChanged("emptyImagePath");

			this._reRenderMaterials();
		}
	},

	get_setOpenTypeImagePath: function () {
		return this._setOpenTypeImagePath;
	},
	set_setOpenTypeImagePath: function (value) {
		if (this._setOpenTypeImagePath != value) {
			this._setOpenTypeImagePath = value;
			this.raisePropertyChanged("setOpenTypeImagePath");

			if (this._setOpenTypeImage)
				this._setOpenTypeImage.src = this._setOpenTypeImagePath;
		}
	},

	get_captionImagePath: function () {
		return this._captionImagePath;
	},
	set_captionImagePath: function (value) {
		if (this._captionImagePath != value) {
			this._captionImagePath = value;
			this.raisePropertyChanged("captionImagePath");

			if (this._captionImagePath)
				this._captionImagePath.src = this._captionImagePath;
		}
	},

	get_currentPageUrl: function () {
		return this._currentPageUrl;
	},
	set_currentPageUrl: function (value) {
		if (this._currentPageUrl != value) {
			this._currentPageUrl = value;
			this.raisePropertyChanged("currentPageUrl");
		}
	},

	get_defaultResourceID: function () {
		return this._defaultResourceID;
	},
	set_defaultResourceID: function (value) {
		if (this._defaultResourceID != value) {
			this._defaultResourceID = value;
			this.raisePropertyChanged("defaultResourceID");
		}
	},

	get_defaultClass: function () {
		return this._defaultClass;
	},
	set_defaultClass: function (value) {
		if (this._defaultClass != value) {
			this._defaultClass = value;
			this.raisePropertyChanged("defaultClass");
		}
	},

	get_lockID: function () {
		return this._lockID;
	},
	set_lockID: function (value) {
		if (this._lockID != value) {
			this._lockID = value;
			this.raisePropertyChanged("lockID");
		}
	},

	get_rootPathName: function () {
		return this._rootPathName;
	},
	set_rootPathName: function (value) {
		if (this._rootPathName != value) {
			this._rootPathName = value;
			this.raisePropertyChanged("rootPathName");
		}
	},

	get_relativePath: function () {
		return this._relativePath;
	},
	set_relativePath: function (value) {
		if (value.endsWith("\\") == false)
			value += "\\";

		if (this._relativePath != value) {
			this._relativePath = value;
			this.raisePropertyChanged("relativePath");
		}
	},

	get_dialogUploadFileControlUrl: function () {
		return this._dialogUploadFileControlUrl;
	},

	set_dialogUploadFileControlUrl: function (value) {
		if (this._dialogUploadFileControlUrl != value) {
			this._dialogUploadFileControlUrl = value;
			this.raisePropertyChanged("dialogUploadFileControlUrl");
		}
	},

	get_dialogUploadFileTraditionalControlUrl: function () {
		return this._dialogUploadFileTraditionalControlUrl;
	},

	set_dialogUploadFileTraditionalControlUrl: function (value) {
		if (this._dialogUploadFileTraditionalControlUrl != value) {
			this._dialogUploadFileTraditionalControlUrl = value;
		}
	},

	get_dialogUploadFileProcessControlUrl: function () {
		return this._dialogUploadFileProcessControlUrl;
	},

	set_dialogUploadFileProcessControlUrl: function (value) {
		if (this._dialogUploadFileProcessControlUrl != value) {
			this._dialogUploadFileProcessControlUrl = value;
			this.raisePropertyChanged("dialogUploadFileProcessControlUrl");
		}
	},

	get_dialogFileOpenTypeControlUrl: function () {
		return this._dialogFileOpenTypeControlUrl;
	},

	set_dialogFileOpenTypeControlUrl: function (value) {
		if (this._dialogFileOpenTypeControlUrl != value) {
			this._dialogFileOpenTypeControlUrl = value;
			this.raisePropertyChanged("dialogFileOpenTypeControlUrl");
		}
	},

	get_dialogEditDocumentControlUrl: function () {
		return this._dialogEditDocumentControlUrl;
	},

	set_dialogEditDocumentControlUrl: function (value) {
		if (this._dialogEditDocumentControlUrl != value) {
			this._dialogEditDocumentControlUrl = value;
			this.raisePropertyChanged("dialogEditDocumentControlUrl");
		}
	},

	get_showFileOpenType: function () {
		return this._showFileOpenType;
	},

	set_showFileOpenType: function (value) {
		if (this._showFileOpenType != value) {
			this._showFileOpenType = value;
			this.raisePropertyChanged("showFileOpenType");
		}
	},

	get_wfProcessID: function () {
		return this._wfProcessID;
	},

	set_wfProcessID: function (value) {
		if (this._wfProcessID != value) {
			this._wfProcessID = value;
			this.raisePropertyChanged("wfProcessID");
		}
	},

	get_wfActivityID: function () {
		return this._wfActivityID;
	},

	set_wfActivityID: function (value) {
		if (this._wfActivityID != value) {
			this._wfActivityID = value;
			this.raisePropertyChanged("wfActivityID");
		}
	},

	get_officeViewerWrapperID: function () {
		return this._officeViewerWrapperID;
	},

	set_officeViewerWrapperID: function (value) {
		if (this._officeViewerWrapperID != value) {
			this._officeViewerWrapperID = value;
			this.raisePropertyChanged("officeViewerWrapperID");
		}
	},

	get_downloadTemplateWithViewState: function () {
		return this._downloadTemplateWithViewState;
	},

	set_downloadTemplateWithViewState: function (value) {
		this._downloadTemplateWithViewState = value;
	},

	get_downloadWithViewState: function () {
		return this._downloadWithViewState;
	},

	set_downloadWithViewState: function (value) {
		this._downloadWithViewState = value;
	},

	get_wfActivityName: function () {
		return this._wfActivityName;
	},
	set_wfActivityName: function (value) {
		if (this._wfActivityName != value) {
			this._wfActivityName = value;
			this.raisePropertyChanged("wfActivityName");
		}
	},

	get_dialogVersionControlUrl: function () {
		return this._dialogVersionControlUrl;
	},

	set_dialogVersionControlUrl: function (value) {
		if (this._dialogVersionControlUrl != value) {
			this._dialogVersionControlUrl = value;
			this.raisePropertyChanged("dialogVersionControlUrl");
		}
	},

	get_showFileTitle: function () {
		return this._showFileTitle;
	},

	set_showFileTitle: function (value) {
		if (this._showFileTitle != value) {
			this._showFileTitle = value;
			this.raisePropertyChanged("showFileTitle");

			this._reRenderMaterials();
		}
	},

	get_defaultFileIconPath: function () {
		return this._defaultFileIconPath;
	},

	set_defaultFileIconPath: function (value) {
		if (this._defaultFileIconPath != value) {
			this._defaultFileIconPath = value;
			this.raisePropertyChanged("defaultFileIconPath");

			this._reRenderMaterials();
		}
	},

	get_controlID: function () {
		return this._controlID;
	},

	set_controlID: function (value) {
		if (this._controlID != value) {
			this._controlID = value;
			this.raisePropertyChanged("controlID");
		}
	},

	get_uniqueID: function () {
		return this._uniqueID;
	},

	set_uniqueID: function (value) {
		if (this._uniqueID != value) {
			this._uniqueID = value;
		}
	},

	get_designatedControlToShowDialog: function () {
		return this._designatedControlToShowDialog;
	},

	set_designatedControlToShowDialog: function (value) {
		if (this._designatedControlToShowDialog != value) {
			this._designatedControlToShowDialog = value;
			this.raisePropertyChanged("designatedControlToShowDialog");
		}
	},

	get_enabled: function () {
		return this._enabled;
	},

	set_enabled: function (value) {
		if (this._enabled != value) {
			this._enabled = value;
			this.raisePropertyChanged("enabled");

			if (this._enabled == false) {
				this.set_allowEditContent(false);
				this.set_allowEdit(false);
			}
		}
	},

	get_openInlineFileExt: function () {
		return $HBRootNS.material.openInlineFileExt;
	},

	set_openInlineFileExt: function (value) {
		if ($HBRootNS.material.openInlineFileExt != value) {
			$HBRootNS.material.openInlineFileExt = value;
			this.raisePropertyChanged("openInlineFileExt");
		}
	},

	get_fileExts: function () {
		return this._fileExts;
	},

	set_fileExts: function (value) {
		if (this._fileExts != value) {
			this._fileExts = value;
			this.raisePropertyChanged("fileExts");
		}
	},


	get_fileMaxSize: function () {
		return this._fileMaxSize;
	},

	set_fileMaxSize: function (value) {
		if (this._fileMaxSize != value) {
			this._fileMaxSize = value;
			this.raisePropertyChanged("fileMaxSize");
		}
	},

	get_fileCountLimited: function () {
		return this._fileCountLimited;
	},

	set_fileCountLimited: function (value) {
		if (this._fileCountLimited != value) {
			this._fileCountLimited = value;
			this.raisePropertyChanged("fileCountLimited");
		}
	},

	get_autoCheck: function () {
		return this._autoCheck;
	},

	set_autoCheck: function (value) {
		if (this._autoCheck != value) {
			this._autoCheck = value;
			this.raisePropertyChanged("autoCheck");
		}
	},

	get_requestContext: function () {
		return this._requestContext;
	},

	set_requestContext: function (value) {
		this._requestContext = value;
	},

	get_materialTitle: function () {
		return this._materialTitle;
	},

	set_materialTitle: function (value) {
		if (this._materialTitle != value) {
			this._materialTitle = value;
			this.raisePropertyChanged("materialTitle");
		}
	},

	get_editDocumentInCurrentPage: function () {
		return this._editDocumentInCurrentPage;
	},
	set_editDocumentInCurrentPage: function (value) {
		if (this._editDocumentInCurrentPage != value) {
			this._editDocumentInCurrentPage = value;
			this.raisePropertyChanged("editDocumentInCurrentPage");
		}
	},

	get_officeViewerShowToolBars: function () {
		return this._officeViewerShowToolBars;
	},
	set_officeViewerShowToolBars: function (value) {
		if (this._officeViewerShowToolBars != value) {
			this._officeViewerShowToolBars = value;
			this.raisePropertyChanged("officeViewerShowToolBars");
		}
	},

	get_autoOpenDocument: function () {
		return this._autoOpenDocument;
	},
	set_autoOpenDocument: function (value) {
		if (this._autoOpenDocument != value) {
			this._autoOpenDocument = value;
			this.raisePropertyChanged("autoOpenDocument");
		}
	},

	_get_officeViewerWrapper: function () {
		return $get(this._officeViewerWrapperID);
	},

	_get_officeViewerWrapperViewer: function () {
		return $get(this._officeViewerWrapperID + "_Viewer");
	},

	add_onDocumentOpen: function (handler) {
		this.get_events().addHandler("onDocumentOpen", handler);
	},
	remove_onDocumentOpen: function (handler) {
		this.get_events().removeHandler("onDocumentOpen", handler);
	},
	raiseDocumentOpen: function (doc) {
		var handlers = this.get_events().getHandler("onDocumentOpen");

		var continueExec = true;

		if (handlers) {
			var e = new Sys.EventArgs();

			e.doc = doc;

			handlers(this, e);

			if (e.cancel)
				continueExec = false;
		}

		return continueExec;
	},

	add_materialsChanged: function (handler) {
		this.get_events().addHandler("materialsChanged", handler);
	},

	remove_materialsChanged: function (handler) {
		this.get_events().removeHandler("materialsChanged", handler);
	},

	raiseMaterialsChanged: function () {
		var handlers = this.get_events().getHandler("materialsChanged");

		if (handlers) {
			var e = new Sys.EventArgs();
			e.materials = this.get_materialsResult();
			handlers(this, e);
		}
	},

	add_onDocumentDownload: function (handler) {
		this.get_events().addHandler("onDocumentDownload", handler);
	},
	remove_onDocumentDownload: function (handler) {
		this.get_events().removeHandler("onDocumentDownload", handler);
	},
	raiseDocumentDownload: function (doc, executedDownload, material) {
		var handlers = this.get_events().getHandler("onDocumentDownload");

		var continueExec = true;

		if (handlers) {
			var e = new Sys.EventArgs();

			e.doc = doc;
			e.executedDownload = executedDownload;

			handlers(this, e);

			if (e.cancel)
				continueExec = false;

			if (e.title != null)
				material.set_title(e.title);
		}

		return continueExec;
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
	},

	//    set_MaterialModified: function () {
	//        if (this._currentEditingMaterial) {
	//            var wrapperViewer = this._get_officeViewerWrapperViewer();
	//            wrapperViewer.SetAppFocus();
	//            if (wrapperViewer.IsDirty()) {
	//                var data = new Object();
	//                data.modifyTime = new Date();
	//                data.isModified = true;
	//                this._currentEditingMaterial.persistLocalData(data);
	//                this._currentEditingMaterial.loadLocalData();
	//            }
	//        }
	//    },

	_showEditDocumentDialog: function (material) {
		if (material) {
			var width = window.screen.width - 120;
			var height = window.screen.height - 80;
			var arg = "status:false;dialogWidth:" + width + "px;dialogHeight:" + height + "px;edge:Raised; enter: Yes; help: No; resizable: Yes; status: No";

			var information = new Object();
			information.container = this;
			information.material = material;
			information.showTooBars = this._officeViewerShowToolBars;

			var returnInfo = window.showModalDialog(this._dialogEditDocumentControlUrl, information, arg);

			material.loadLocalData();
		}
	},

	openDocument: function (material, isFirstTime) {
		if (material) {
			if (this._editDocumentInCurrentPage == true && this._materialUseMode == $HBRootNS.materialUseMode.SingleDraft) {
				var wrapper = this._get_officeViewerWrapper();
				var wrapperViewer = this._get_officeViewerWrapperViewer();

				//wrapperViewer.toolBars = this._officeViewerShowToolBars;
				window.setTimeout(function () {
					if (wrapperViewer.Open(material._realLocalPath) == true) {
						wrapperViewer.SetAppFocus();
						if (material._localPath != material._realLocalPath) {
							wrapperViewer.SaveAs(material._localPath);
							material._realLocalPath = material._localPath;
						}
					}
					else if (isFirstTime) {
						material._clearLocalData();
						material._initLocalData();
						material._afterClick(false, false);
					}
				}, 100);
			}
			else {
				this._showEditDocumentDialog(material);
			}
			this._currentEditingMaterial = material;
		}
	},

	//    get_Downloader: function () {
	//        return this._get_officeViewerWrapperViewer();
	//    },

	//    _downloadToTempDir: function (remoteUrl) {
	//        var downloader = this.get_Downloader();
	//        return downloader.HttpDownLoadFileToTempDir(remoteUrl);
	//    },

	//    download: function (remoteUrl) {
	//        return this._downloadToTempDir(remoteUrl);
	//    },

	_setNewMaterialInfo: function (newFile, index) {
		var xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
		var fileExt = $HBRootNS.fileIO.getFileExt(newFile.filePath);
		var newFileName = "newfile." + (fileExt != "" ? fileExt : "doc");

		var processPageUrl = String.format("{0}?requestType=getnewmaterialinfo&fileName={1}&requestContext={2}&t={3}",
				this.get_currentPageUrl(),
				escape(newFileName),
				escape(this.get_requestContext()),
                Date.parse(new Date()) + index);

		try {
			xmlhttp.open("GET", processPageUrl, false);
			xmlhttp.send();

			if (xmlhttp.status == 200) {
				var str = xmlhttp.responseText;
				var materialInfo = Sys.Serialization.JavaScriptSerializer.deserialize(str);

				newFile.lastUploadTag = materialInfo.uploadTag;
				newFile.newMaterialID = materialInfo.materialID;
				newFile.fileIconPath = materialInfo.fileIconPath == "" ?
                this.get_defaultFileIconPath() : materialInfo.fileIconPath; ;
			}
			else {
				alert("获取新增附件ID出错！");
			}

		}
		catch (e) {
			alert("获取新增附件ID出错——" + e);
		}
	},

	//	uploadNewFiles: function (filesToUpload, fileMaxSize) {
	//		var uploadedFiles = new Array();

	//		if (filesToUpload == null || filesToUpload.length == 0) {
	//			return;
	//		}
	//		else {
	//			for (var i = 0; i < filesToUpload.length; i++) {
	//				var currentFile = filesToUpload[i];
	//				var localPath = currentFile.filePath;

	//				this._setNewMaterialInfo(currentFile, i);

	//				if (fileMaxSize == null)
	//					fileMaxSize = 0;

	//				var controlID = this.get_uniqueID();

	//				var processPageUrl = this.get_currentPageUrl()
	//		                    + "?requestType=upload"
	//                            + "&upmethod=new"
	//		                    + "&lockID=" + this.get_lockID()
	//		                    + "&userID=" + this.get_user().id
	//		                    + "&rootPathName=" + this.get_rootPathName()
	//		                    + "&fileMaxSize=" + fileMaxSize
	//		                    + "&controlID=" + controlID;

	//				if (processPageUrl == null || processPageUrl == "")
	//					throw Error.create("请求页面地址为空");

	//				if (localPath == null || localPath == "")
	//					throw Error.create("文件路径为空");

	//				processPageUrl += "&fileName=" + escape(currentFile.newMaterialID + "." + $HBRootNS.fileIO.getFileExt(localPath));
	//				//                if ($HBRootNS.officeDocument.checkIsOfficeDocument(localPath)) {
	//				//                    processPageUrl += "&fileName=" + escape($HBRootNS.fileIO.getFileNameWithExt(localPath));
	//				//                }
	//				//                else {
	//				//                    processPageUrl += "&fileName=" + escape($HBRootNS.material.newMaterialID + "." + $HBRootNS.fileIO.getFileExt(localPath));
	//				//                }

	//				processPageUrl = "http://" + window.location.host + processPageUrl;

	//				try {
	//					this._doUpload(processPageUrl, localPath);

	//					Array.add(uploadedFiles, currentFile);
	//				}
	//				catch (e) {
	//					alert("上传文件" + currentFile.filePath + "时发生错误：\n" + e.message);
	//				}
	//			}

	//			return uploadedFiles;
	//		}
	//	},

	//    uploadFiles: function (filesToUpload, fileMaxSize) {
	//        var uploadedFiles = new Array();

	//        if (filesToUpload == null || filesToUpload.length == 0) {
	//            return;
	//        }
	//        else {
	//            for (var i = 0; i < filesToUpload.length; i++) {
	//                var currentFile = filesToUpload[i];
	//                var localPath = currentFile.filePath;

	//                if (fileMaxSize == null)
	//                    fileMaxSize = 0;

	//                var controlID = this.get_uniqueID();

	//                var processPageUrl = this.get_currentPageUrl()
	//		                    + "?requestType=upload"
	//                            + "&upmethod=new"
	//		                    + "&lockID=" + this.get_lockID()
	//		                    + "&userID=" + this.get_user().id
	//		                    + "&rootPathName=" + this.get_rootPathName()
	//		                    + "&fileMaxSize=" + fileMaxSize
	//		                    + "&controlID=" + controlID;

	//                if (processPageUrl == null || processPageUrl == "")
	//                    throw Error.create("请求页面地址为空");

	//                if (localPath == null || localPath == "")
	//                    throw Error.create("文件路径为空");

	//                if ($HBRootNS.officeDocument.checkIsOfficeDocument(localPath)) {
	//                    processPageUrl += "&fileName=" + escape($HBRootNS.fileIO.getFileNameWithExt(localPath));
	//                }
	//                else {
	//                    processPageUrl += "&fileName=" + escape($HBRootNS.material.newMaterialID + "." + $HBRootNS.fileIO.getFileExt(localPath));
	//                }

	//                processPageUrl = "http://" + window.location.host + processPageUrl;

	//                try {
	//                    this._doUpload(processPageUrl, localPath);

	//                    //currentFile.newMaterialID = "";
	//                    Array.add(uploadedFiles, currentFile);
	//                }
	//                catch (e) {
	//                    alert("上传文件" + currentFile.filePath + "时发生错误：\n" + e.message);
	//                    //window.returnValue = filesToUpload;
	//                }
	//            }

	//            return uploadedFiles;
	//            //window.returnValue = filesToUpload;
	//        }
	//    },

	//    _doUpload: function (processPageUrl, localPath) {
	//        var wrapperViewer = this._get_officeViewerWrapperViewer();

	//        wrapperViewer.HttpInit();
	//        if (this._editDocumentInCurrentPage == true && wrapperViewer.IsOpened()) {
	//            wrapperViewer.SetAppFocus();
	//            wrapperViewer.HttpAddPostOpenedFile(localPath);
	//        }
	//        else {
	//            wrapperViewer.HttpAddPostFile(localPath);
	//        }
	//        wrapperViewer.HttpPost(processPageUrl);
	//    }

	get_cloneableProperties: function () {
		var baseProperties = $HBRootNS.MaterialControl.callBaseMethod(this, "get_cloneableProperties");
		var currentProperties = ["allowEdit", "allowEditContent", "autoCheck", "autoOpenDocument", "activityControlMode", "caption", "captionImagePath"
		, "draftText", "editDocumentInCurrentPage", "saveOriginalDraft", "materialTitle", "editText", "displayText", "requestContext", "enabled"
			, "trackRevisions", "showFileTitle", "modifyCheck", "showAllVersions", "templateUrl", "rootPathName", "relativePath", "materialUseMode"
		, "fileSelectMode", "materialTableShowMode", "linkShowMode", "showFileOpenType", "fileExts", "fileMaxSize", "fileCountLimited", "defaultResourceID"
			, "defaultClass", "wfProcessID", "wfActivityID", "wfActivityName", "downloadTemplateWithViewState", "downloadWithViewState", "lockID", "user", "department"
			, "dialogUploadFileControlUrl", "dialogUploadFileTraditionalControlUrl", "dialogFileOpenTypeControlUrl", "dialogVersionControlUrl", "dialogUploadFileProcessControlUrl", "dialogEditDocumentControlUrl"
			, "uniqueID", "controlID", "officeViewerWrapperID", "editImagePath", "uploadImagePath", "emptyImagePath", "defaultFileIconPath", "showVersionImagePath"
		, "setOpenTypeImagePath", "captionImagePath", "currentPageUrl", "designatedControlToShowDialog", "openInlineFileExt"];

		Array.addRange(currentProperties, baseProperties);

		return currentProperties;
	},

	_prepareCloneablePropertyValues: function (newElement) {
		var properties = $HBRootNS.MaterialControl.callBaseMethod(this, "_prepareCloneablePropertyValues", [newElement]);

		var officeViewerControl = $find(this._officeViewerWrapperID);

		if (officeViewerControl != null) {
			var newofficeViewerControl = officeViewerControl.cloneAndAppendToContainer(newElement.parentNode);
			properties["officeViewerWrapperID"] = newofficeViewerControl.get_element().id;
		}

		return properties;
	},

	onAfterCloneComponent: function (newElement, newComponent) {
		newComponent._deltaMaterials = { "insertedMaterials": [], "updatedMaterials": [], "deletedMaterials": [], "rootPathName": "" };
		$HBRootNS.MaterialControl.allClonedMeterialControls[newElement.id] = newComponent;
	}

}

$HBRootNS.MaterialControl.registerClass($HBRootNSName + ".MaterialControl", $HGRootNS.ControlBase);

$HBRootNS.MaterialControl.timer = $create($HGRootNS.Timer, { interval: 2000, enabled: true }, null, null, null);
$HBRootNS.MaterialControl.timer.add_tick($HBRootNS.material.refreshAllStatus);

$HBRootNS.MaterialControl.allClonedMeterialControls = {};
$HBRootNS.MaterialControl.MaterialControlCommonHiddenField = "MaterialControlCommonHiddenField";

$HBRootNS.MaterialControl.getCommonDeltaMaterials = function () {
	var result = {};

	for (var mtctrlID in $HBRootNS.MaterialControl.allClonedMeterialControls) {
		var clonedControl = $HBRootNS.MaterialControl.allClonedMeterialControls[mtctrlID];
		result[mtctrlID] = clonedControl._generateMultiMaterial().deltaMaterials;
	}

	return result;
},

$HBRootNS.MaterialControl.materialClientValidate = function (source, arguments) {
	arguments.IsValid = true;

	if ($HBRootNS.HBCommon.executeValidators) {
		var rtnValue = $HBRootNS.material.checkAllMaterialsOpend();
		if (rtnValue == 1) {
			window.alert("您有正在打开的文档文件，当前的页面不能提交，请您保存文件后再提交页面！");
			arguments.IsValid = false;
		}
		else if (rtnValue == 2) {
			window.alert("您编辑了文件但没有保存当前文件，请您保存文件！");
			arguments.IsValid = false;
		}

		$HBRootNS.material.checkedOpened = arguments.IsValid;

		for (var i = 0; i < $HBRootNS.material.allMaterials.length; i++) {
			var material = $HBRootNS.material.allMaterials[i];
			if (material.get_uploaded() == false && material.get_contentModified() == true) {
				$HBRootNS.material.proecssedUpload = false;
				break;
			}
		}

		if ($HBRootNS.material.proecssedUpload == false && $HBRootNS.material.checkedOpened == true) {
			arguments.IsValid = $HBRootNS.material.allMaterialsUpload();
		}
	}

	var commonDeltaMaterials = $HBRootNS.MaterialControl.getCommonDeltaMaterials();
	$get($HBRootNS.MaterialControl.MaterialControlCommonHiddenField).value = Sys.Serialization.JavaScriptSerializer.serialize(commonDeltaMaterials);
}

$HBRootNS.HBCommon.registSubmitValidator($HBRootNS.material.checkAllMaterialsUpLoaded, 1);