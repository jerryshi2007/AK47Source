
$HBRootNS.phonenumber = function (element) {
	$HBRootNS.phonenumber.initializeBase(this, [element]);
	this._code = null; 					//����
	this._resourceID = null; 			//��˾����ԱID
	this._telephoneClass = null; 			//���
	this._stateCode = null; //����
	this._areaCode = null; //����
	this._mainCode = null; //�绰
	this._extCode = null; //�ֻ���
	this._innerSort = 0; 	//�ڲ������
	this._versionStartTime = null; //�汾��ʼʱ��
	this._versionEndTime = null; //�汾����ʱ��
	this._isDefault = null; //�Ƿ�Ĭ��
	this._description = null; //����
	this._changed = null;
	this._container = null;  //���ڵĿؼ�
}

$HBRootNS.phonenumber.prototype =
{
	initialize: function () {
		//ҵ���߼������ݳ�ʼ��
		Array.add($HBRootNS.phonenumber.allTelephones, this);
	}
}


$HBRootNS.phonenumber.allTelephones = new Array(); 	//ҳ��������phonenumber����ļ���
$HBRootNS.phonenumber.allTelephoneControls = new Array(); 	//ҳ��������phonenumber�ؼ��ļ���
$HBRootNS.phonenumber.checkTelephones = new Array(); //ҳ�������пؼ�����֤״̬

//ˢ������phonenumber����
$HBRootNS.phonenumber.refreshAllStatus = function () {
	for (var i = 0; i < $HBRootNS.phonenumber.allTelephones.length; i++)
		$HBRootNS.phonenumber.allTelephones[i].refreshStatus();
}

