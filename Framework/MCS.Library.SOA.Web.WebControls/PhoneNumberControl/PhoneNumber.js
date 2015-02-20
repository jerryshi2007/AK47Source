
$HBRootNS.phonenumber = function (element) {
	$HBRootNS.phonenumber.initializeBase(this, [element]);
	this._code = null; 					//编码
	this._resourceID = null; 			//公司或人员ID
	this._telephoneClass = null; 			//类别
	this._stateCode = null; //国别
	this._areaCode = null; //区号
	this._mainCode = null; //电话
	this._extCode = null; //分机号
	this._innerSort = 0; 	//内部排序号
	this._versionStartTime = null; //版本开始时间
	this._versionEndTime = null; //版本结束时间
	this._isDefault = null; //是否默认
	this._description = null; //描述
	this._changed = null;
	this._container = null;  //所在的控件
}

$HBRootNS.phonenumber.prototype =
{
	initialize: function () {
		//业务逻辑，数据初始化
		Array.add($HBRootNS.phonenumber.allTelephones, this);
	}
}


$HBRootNS.phonenumber.allTelephones = new Array(); 	//页面上所有phonenumber对象的集合
$HBRootNS.phonenumber.allTelephoneControls = new Array(); 	//页面上所有phonenumber控件的集合
$HBRootNS.phonenumber.checkTelephones = new Array(); //页面中所有控件的验证状态

//刷新所有phonenumber对象
$HBRootNS.phonenumber.refreshAllStatus = function () {
	for (var i = 0; i < $HBRootNS.phonenumber.allTelephones.length; i++)
		$HBRootNS.phonenumber.allTelephones[i].refreshStatus();
}

