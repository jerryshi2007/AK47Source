<!--
	var C_APP_ADMIN_ID = "11111111-1111-1111-1111-111111111111";
	function getFrameParam()
	{
		if (typeof(secFrm) == "object" && secFrm.value == "2" && parent.window.paramValue2)
				return parent.window.paramValue2.value;
		
		if (parent.window.paramValue)
			return parent.window.paramValue.value;
		
		return "";
	}

	function setSyncData(value)
	{
		if (parent.window.syncData)
			parent.window.syncData.value = value;
	}
	
/*
	function getImgFromClass(strClass)
	{
		var imgName = "";
		switch(strClass)
		{
			case "group":	imgName = "group.gif";
							break;
			case "person":
			case "CN":
			case "USER":	imgName = "user.gif";
							break;
			case "userAccountControl"://�˺Ž���
							imgName = "accountDisabled.gif";
							break;
			case "DC":
			case "dc":
			case "domain":
			case "DOMAIN":	imgName = "domain.gif";
							break;
			case "organizationalUnit":
			case "OU":		imgName = "ou.gif";
							break;
			case "role":
			case "ROLE":
			case "ROLES":
			case "ROLES_TO_FUNCTIONS":
							imgName = "role.gif";
							break;
			case "FUNCTIONS":
							imgName = "function.gif";
							break;
			case "MANAGED_APP":
			case "APPLICATIONS":
			case "RESOURCES":
							imgName = "application.gif";
							break;
			case "ResourceItem":
							imgName = "ResourceItem.gif";
							break;
			case "ResourceFolder":
							imgName = "ResourceFolder.gif";
							break;
			case "ResourceFile":
							imgName = "ResourceFile.gif";
							break;
		}

		return imgName;
	}


	function getStringFromDNPart(strDNPart, bPrefix)
	{
		var arrDNPart = strDNPart.split("=");
		var result = "";

		if (bPrefix)
			result = arrDNPart[0];
		else
		{
			if (arrDNPart[0] == "DC")
				result = C_DC_ROOT_NAME;
			else
				result = arrDNPart[1];
		}

		return result;
	}

	function getNameFromDN(strDN)
	{
		var arrSeg = strDN.split(",");

		return getStringFromDNPart(arrSeg[0]);
	}

	function getOwnerFromDN(strDN, strSplitChar)
	{
		var arrSeg = strDN.split(",");
		var strResult = "";

		if (!strSplitChar)
			strSplitChar = ".";

		for(var i = 1; i< arrSeg.length; i++)
		{
			var arrValue = arrSeg[i].split("=");

			if (arrValue[0] != "DC")
			{
				var strTemp = arrValue[1];

				if (strResult.length > 0)
					strTemp += strSplitChar;

				strResult = strTemp + strResult;
			}
			else
				break;
		}

		return strResult;
	}

	function getParentOUFromDN(strDN)
	{
		var arrSeg = strDN.split(",");
		var strResult = "";

		var bStart = false;

		for (var i = 1; i< arrSeg.length; i++)
		{
			var arrValue = arrSeg[i].split("=");

			if (arrValue[0] == "DC" || arrValue[0] == "OU")
				bStart = true;

			if (bStart)
			{
				if (strResult.length > 0)
					strResult += ",";

				strResult += arrSeg[i];
			}
			else
				break;
		}

		return strResult;
	}

	function getFirstOUFromDN(strDN)
	{
		var arrSeg = strDN.split(",");
		var strResult = "";

		var bStart = false;
		var nStartIndex = 0;

		for (var i = arrSeg.length - 1; i >= 0; i--)
		{
			var arrValue = arrSeg[i].split("=");

			if (arrValue[0] != "DC")
			{
				if (arrValue[0] != "OU")
					nStartIndex = i + 1;
				else
					nStartIndex = i;
				break;
			}
		}

		for (var i = nStartIndex; i < arrSeg.length; i++)
		{
			if (strResult.length > 0)
				strResult += ",";

			strResult += arrSeg[i];
		}

		return strResult;
	}

	function showOUDetailDialog(strOP, strDN)
	{
		var sFeature = "dialogWidth:340px; dialogHeight:400px;center:yes;help:no;resizable:yes;scroll:no;status:no";

		var arg = new Object();

		arg.dn = strDN;
		arg.op = strOP;

		var sPath = "/" + C_APP_ADMIN_ROOT_URI + "/dialogs/editOU.htm";

		return showModalDialog(sPath, arg, sFeature);
	}

	function createMailBoxDialog(strUserDN, strLogOnName)
	{
		var sFeature = "dialogWidth:480px; dialogHeight:360px;center:yes;help:no;resizable:yes;scroll:no;status:no";

		var arg = new Object();

		arg.dn = strUserDN;
		arg.alias = strLogOnName.split("@")[0];

		var sPath = "/" + C_APP_ADMIN_ROOT_URI + "/dialogs/selectMailBox.htm";

		return showModalDialog(sPath, arg, sFeature);
	}

	function changeAlias(strUserDN)
	{
		var xmlDoc = createCommandXML("changeAlias", strUserDN);
		var xmlResult = xmlSend("../ADAccess/ADSupport.aspx", xmlDoc);

		checkErrorResult(xmlResult);
	}
*/
	function showUSERDetailDialog(strOP, strDN)
	{
		var sFeature = "dialogWidth:360px; dialogHeight:460px;center:yes;help:no;resizable:yes;scroll:no;status:no";

		var arg = new Object();

		arg.dn = strDN;
		arg.op = strOP;

		var sPath = "/" + C_APP_ADMIN_ROOT_URI + "/dialogs/editUser.htm";
		var strXml =  showModalDialog(sPath, arg, sFeature);

		if (strXml.length > 0)
		{
			var xmlDoc = createDomDocument(strXml);
			var strAutoCreate = xmlDoc.documentElement.getAttribute("autoCreateMailBox");

			if (strAutoCreate.toLowerCase() == "true")
			{
				var nodeSet = xmlDoc.selectSingleNode(".//SET");
				var strRootDN = xmlDoc.documentElement.getAttribute("dn");

				try
				{
					if (strOP == "insert")
					{
						var userPrincipalNameNode = nodeSet.selectSingleNode("userPrincipalName");

						strLogOnName = "";

						if (userPrincipalNameNode)
							strLogOnName = userPrincipalNameNode.text;

						var strName = nodeSet.selectSingleNode("name").text;
						var strUserDN = "CN=" + strName + "," + strRootDN;

						var homeMDB = createMailBoxDialog(strUserDN, strLogOnName);
						
						if (homeMDB)
							appendNode(xmlDoc, nodeSet, "homeMDB", homeMDB);
					}
					else
					if (strOP == "update")
					{
						var userPrincipalNameNode = nodeSet.selectSingleNode("userPrincipalName");

						if (userPrincipalNameNode)
						{
							if (confirm("�û���¼�����Ѿ��޸ģ��Ƿ�ͬʱ�޸��ʼ��ı�����"))
								changeAlias(strRootDN);
						}
					}
					
					strXml = xmlDoc.xml;
				}
				catch(e)
				{
					showError(e);
				}
			}
		}

		return strXml;
	}

	//params:
	// strOP				��������
	// nID					����id,�򸸶���id
	// strType				���ͣ�������
	// strFatherNodeType	��������ͣ���ʱ��Ҫ��
	// strAppID				Ӧ��id
	// strAppLevel			Ӧ�ò��
	// strCodeName			�����ʶ
	// strClassify			�������
	// strInherited			�Ƿ�Ӹ�Ӧ�ü̳У���insert����ʱ���ã�
	function showAppObjDetailDialog(params)
	{
		var sFeature = "dialogWidth:320px; dialogHeight:300px;center:yes;help:no;resizable:yes;scroll:no;status:no";

		if (params.type == "APPLICATIONS")//����Ӧ����˵��Ҫ�����ʾ�������һЩ
			sFeature = "dialogWidth:480px; dialogHeight:320px;center:yes;help:no;resizable:yes;scroll:no;status:no";


//		if (typeof(event.srcElement.canDelete) != "undefined")
//			arg.disabled = !event.srcElement.canDelete;
//		else
//			arg.disabled = false;

		var sPath = "/" + C_APP_ADMIN_ROOT_URI + "/dialogs/";
		switch (params.type)
		{
			case "APPLICATIONS":
									sPath = sPath + "editAppObjects.htm";
									break;
			case "ROLES":
									sPath = sPath + "editObjAttr.aspx";
									break;
			case "FUNCTIONS":
									sPath = sPath + "editObjAttr.aspx";
									break;
			case "FUNCTION_SETS":
									sPath = sPath + "editObjAttr.aspx";
									break;
			case "SCOPES":
									if(params.classify == "y" && params.inherited == "n")//�½���������Χ
									{
										sPath = sPath + "../dialogs/editScope1.htm";
									}
									if(params.classify == "y" && params.inherited == "y")//�̳л�������Χ
									{
										sPath = sPath + "����";
									}
									if(params.classify == "n" && params.inherited == "n")//�½����ݷ���Χ
									{
										sPath = sPath + "����";
									}
									if(params.classify == "n" && params.inherited == "y")//�̳����ݷ���Χ
									{
										sPath = sPath + "����";
									}
									
									break;
//			case "EXPRESSIONS":
//									sPath = sPath + "editUser.aspx";
//									break;
		}

		var xmlResult = showModalDialog(sPath, params, sFeature);

		if (xmlResult)
			checkErrorResult(xmlResult);
		return xmlResult;
	}

	function showSelectUserDialog(arg)
	{
		var sFeature = "dialogWidth:360px; dialogHeight:400px;center:yes;help:no;resizable:yes;scroll:no;status:no";

		var sPath = "/"+C_ACCREDIT_ADMIN_ROOT_URI+"/exports/selectOGU.aspx";

		return showModalDialog(sPath, arg, sFeature);
	}

	function showSelectOUScopeDialog(arg)
	{
		var sFeature = "dialogWidth:360px; dialogHeight:400px;center:yes;help:no;resizable:yes;scroll:no;status:no";

		var sPath = "/"+C_ACCREDIT_ADMIN_ROOT_URI+"/exports/selectOGU.aspx";

		return showModalDialog(sPath, arg, sFeature);
	}

	function showSelectUsersToRoleDialog(arg)
	{
		var sFeature = "dialogWidth:570px; dialogHeight:480px;center:yes;help:no;resizable:yes;scroll:no;status:no";

		var sPath = "/"+C_ACCREDIT_ADMIN_ROOT_URI+"/exports/selectObjsToRole.aspx";

		return showModalDialog(sPath, arg, sFeature);
	}

	function showGroupUsersDialog(groupDN)
	{
		var sFeature = "dialogWidth:570px; dialogHeight:480px;center:yes;help:no;resizable:yes;scroll:no;status:no";

		var sPath = "/" + C_APP_ADMIN_ROOT_URI + "/dialogs/listGroupUsers.aspx?groupDN=" + groupDN;

		return showModalDialog(sPath, groupDN, sFeature);
	}

	//function showUserFunctionsDialog(objID, appID, objName, objectClass, objRefAppID)
	function showUserFunctionsDialog(obj, mObj)
	{
		var sFeature = "dialogWidth:360px; dialogHeight:420px;center:yes;help:no;resizable:yes;scroll:no;status:no";

		var sPath = "/" + C_APP_ADMIN_ROOT_URI + "/dialogs/userFunctions.htm";

		var arg = new Object();
		
		arg.objID = obj.objID;
		arg.objName = obj.innerText;
		arg.objectClass = obj.objectClass;
		
		arg.appID = mObj.appID;
		arg.refAppID = mObj.refAppID;
		
		arg.appLevel = mObj.appLevel;
		/*arg.objID = objID;
		arg.appID = appID;
		arg.objName = objName;
		arg.objectClass = objectClass;
		arg.refAppID = objRefAppID;*/

		return showModalDialog(sPath, arg, sFeature);
	}

	function showResetPasswordDialog(strDN)
	{
		var sFeature = "dialogWidth:360px; dialogHeight:250px;center:yes;help:no;resizable:yes;scroll:no;status:no";

		var sPath = "/" + C_APP_ADMIN_ROOT_URI + "/dialogs/resetPassword.htm";

		return showModalDialog(sPath, strDN, sFeature);
	}
	


	/*//����������*/
	//
	var C_APP_ADMIN_CODE_NAME = "APP_ADMIN";
	
	//Ӧ�������Ͻ�����ͣ�node.xData.type
	var C_NODE_TYPE_A1 = "rootNode"; 		//�����
	var C_NODE_TYPE_A2 = "appScopeNode";	//Ӧ�÷���Χ����
	var C_NODE_TYPE_A3 = "selfRoleNode";	//����Ȩ��ɫ����
	var C_NODE_TYPE_A4 = "selfFuncNode";	//����Ȩ���ܽ�㼯��
	var C_NODE_TYPE_A5 = "appRoleNode";		//Ӧ�ý�ɫ����
	var C_NODE_TYPE_A6 = "appFuncNode";		//Ӧ�ù��ܽ�㼯��
	var C_NODE_TYPE_A7 = "applicationNode";	//��Ӧ�ü���
	

	//Ӧ����Ҷ������ͣ�node.xData.type
	var C_NODE_TYPE_B21 = "OUScope";		//��������Χ
	var C_NODE_TYPE_B22 = "dataScope";		//���ݷ���Χ
	var C_NODE_TYPE_B31 = "selfRole";		//����Ȩ��ɫ
	var C_NODE_TYPE_B41 = "selfFunc";		//����Ȩ����
	var C_NODE_TYPE_B42 = "selfFuncSet";	//����Ȩ���ܼ���
	var C_NODE_TYPE_B51 = "appRole";		//Ӧ�ý�ɫ
	var C_NODE_TYPE_B61 = "appFunc";		//Ӧ�ù���
	var C_NODE_TYPE_B62 = "appFuncSet";		//Ӧ�ù��ܼ���
	var C_NODE_TYPE_B71 = "appAdmin";		//ͨ����ȨӦ��
	var C_NODE_TYPE_B72 = "application"		//һ��Ӧ��
	//����Ȩ���������
	var C_OBJ_TYPE_A1	= "user";			//��Ա
	var C_OBJ_TYPE_A2	= "organization";	//����	
	var C_OBJ_TYPE_A3	= "group";			//��

	function getImgFromType(strType)
	{
		var imgName = "";
		switch(strType)
		{
			case C_NODE_TYPE_B21://��������Χ
							imgName = "Scope1.gif";
							break;
			
			case C_NODE_TYPE_B22://���ݷ���Χ
							imgName = "Scope2.gif";
							break;

			case C_NODE_TYPE_B31://����Ȩ��ɫ
			case C_NODE_TYPE_B51://Ӧ�ý�ɫ
							imgName = "role.gif";
							break;

			case C_NODE_TYPE_B41://����Ȩ����
			case C_NODE_TYPE_B61://Ӧ�ù���
							imgName = "function.gif";
							break;

			case C_NODE_TYPE_B42://����Ȩ���ܼ���
			case C_NODE_TYPE_B62://Ӧ�ù��ܼ���
							imgName = "functionSet.gif";
							break;
			case C_NODE_TYPE_B71://ͨ����ȨӦ��
							imgName = "computer.gif";
							break;
			case C_NODE_TYPE_B72://һ��Ӧ��
							imgName = "application.gif";
							break;
			//����Ȩ���������
			case C_OBJ_TYPE_A1://��Ա
							imgName = "user.gif";
							break;
			case C_OBJ_TYPE_A2://����	
							imgName = "organization.gif";
							break;
			case C_OBJ_TYPE_A3://��
							imgName = "group.gif";
							break;
			default:
					imgName = getImgFromType(getTypeFromFather(strType));
		}

		return imgName;
	}
	

	function getTypeFromFather( strFatherType )
	{
		var strClassify = "";
		var strFuncType = "";
		var strCodeName = "";
		var strObjClass = "";
		
		var DomNode = null;
		if (arguments.length > 1)
		{
			DomNode = arguments[1];
			if (DomNode != null)
			{
				strClassify	= getSingleNodeText(DomNode, "CLASSIFY", ""); 
				strFuncType	= getSingleNodeText(DomNode, "TYPE", "");
				strCodeName = getSingleNodeText(DomNode, "CODE_NAME", "");
				strObjClass	= getSingleNodeText(DomNode, "OBJECTCLASS", ""); 
			}
		}

		var strType;
		switch (strFatherType)
		{
			case C_NODE_TYPE_A2://Ӧ�÷���Χ����
							if ( strClassify == "y" )
								strType = C_NODE_TYPE_B21;
							else
								strType = C_NODE_TYPE_B22;
							break;
			case C_NODE_TYPE_A3://����Ȩ��ɫ����
							strType = C_NODE_TYPE_B31;
							break;
			case C_NODE_TYPE_A4://����Ȩ������
			case C_NODE_TYPE_B42://����Ȩ���ܼ���
							if ( parseInt(strFuncType) == 0 )
								strType = C_NODE_TYPE_B41;
							else if ( parseInt(strFuncType) == 1 )
								strType = C_NODE_TYPE_B42;
							else
								strType = C_NODE_TYPE_B31;
							break;
			case C_NODE_TYPE_A5://Ӧ�ý�ɫ����
							strType = C_NODE_TYPE_B51;
							break;
			case C_NODE_TYPE_A6://Ӧ�ù�����
			case C_NODE_TYPE_B62://Ӧ�ù��ܼ���
							if ( parseInt(strFuncType) == 0 )
								strType = C_NODE_TYPE_B61;
							else if ( parseInt(strFuncType) == 1 ) 
								strType = C_NODE_TYPE_B62;
							else 
								strType = C_NODE_TYPE_B31;
							break;
			case C_NODE_TYPE_B41://����Ȩ����
			case C_NODE_TYPE_B61://Ӧ�ù���
							strType = C_NODE_TYPE_B31;
							break;
			case C_NODE_TYPE_B31://����Ȩ��ɫ
			case C_NODE_TYPE_B51://Ӧ�ý�ɫ
							switch ( strObjClass )
							{
								case "USERS":
									strType = C_OBJ_TYPE_A1;
									break;
								case "ORGANIZATIONS":
									strType = C_OBJ_TYPE_A2;
									break;
								case "GROUPS":
									strType = C_OBJ_TYPE_A3;
									break;
							}
							break;

			case C_NODE_TYPE_A1://�����
			case C_NODE_TYPE_A7://��Ӧ�ü���
							if ( strCodeName == C_APP_ADMIN_CODE_NAME )
								strType = C_NODE_TYPE_B71; 
							else
								strType = C_NODE_TYPE_B72; 
							break;
			case C_OBJ_TYPE_A1://��Ա
			case C_OBJ_TYPE_A2://����	
			case C_OBJ_TYPE_A3://��
							if ( strClassify == "y" )
								strType = C_NODE_TYPE_B21;
							else
								strType = C_NODE_TYPE_B22;
							break;
			default: 
					strType = strFatherType; 
		}
		return strType;
	}

	function getNameFromType(strType)
	{
		var strName = "";

		switch(strType)
		{
			//Ӧ�������Ͻ�����ͣ�node.xData.type
			case C_NODE_TYPE_A1: 	//�����
									strName = "�����";
									break;
			case C_NODE_TYPE_A2:	//Ӧ�÷���Χ����
									strName = "Ӧ�÷���Χ����";
									break;
			case C_NODE_TYPE_A3:	//����Ȩ��ɫ����
									strName = "����Ȩ��ɫ����";
									break;
			case C_NODE_TYPE_A4:	//����Ȩ������
									strName = "����Ȩ������";
									break;
			case C_NODE_TYPE_A5:	//Ӧ�ý�ɫ����
									strName = "Ӧ�ý�ɫ����";
									break;
			case C_NODE_TYPE_A6:	//Ӧ�ù�����
									strName = "Ӧ�ù�����";
									break;
			case C_NODE_TYPE_A7:	//��Ӧ�ü���
									strName = "��Ӧ�ü���";
									break;
			

			//Ӧ����Ҷ������ͣ�node.xData.type
			case C_NODE_TYPE_B21:	//��������Χ
									strName = "��������Χ";
									break;
			case C_NODE_TYPE_B22:	//���ݷ���Χ
									strName = "���ݷ���Χ";
									break;
			case C_NODE_TYPE_B31:	//����Ȩ��ɫ
									strName = "����Ȩ��ɫ";
									break;
			case C_NODE_TYPE_B41:	//����Ȩ����
									strName = "����Ȩ����";
									break;
			case C_NODE_TYPE_B42:	//����Ȩ���ܼ���
									strName = "����Ȩ���ܼ���";
									break;
			case C_NODE_TYPE_B51:	//Ӧ�ý�ɫ
									strName = "Ӧ�ý�ɫ";
									break;
			case C_NODE_TYPE_B61:	//Ӧ�ù���
									strName = "Ӧ�ù���";
									break;
			case C_NODE_TYPE_B62:	//Ӧ�ù��ܼ���
									strName = "Ӧ�ù��ܼ���";
									break;
			case C_NODE_TYPE_B71:	//ͨ����ȨӦ��
									strName = "ͨ����ȨӦ��";
									break;
			case C_NODE_TYPE_B72:	//һ��Ӧ��
									strName = "һ��Ӧ��";
									break;
			case C_OBJ_TYPE_A1:		//��Ա
									strName = "��Ա";
									break;
			case C_OBJ_TYPE_A2:		//����	
									strName = "����";
									break;
			case C_OBJ_TYPE_A3:		//��
									strName = "��";
									break;
		}

		return strName;
	}

	function getTableFromType (type)
	{
		var strTableName = "";
		switch (type)
		{
			case C_NODE_TYPE_A2:	//Ӧ�÷���Χ����
			case C_NODE_TYPE_B21:	//��������Χ
			case C_NODE_TYPE_B22:	//���ݷ���Χ
									strTableName = "SCOPES";
									break;
			case C_NODE_TYPE_A3:	//����Ȩ��ɫ����
			case C_NODE_TYPE_A5:	//Ӧ�ý�ɫ����
			case C_NODE_TYPE_B31:	//����Ȩ��ɫ
			case C_NODE_TYPE_B51:	//Ӧ�ý�ɫ
									strTableName = "ROLES";
									break;
			case C_NODE_TYPE_B41:	//����Ȩ����
			case C_NODE_TYPE_B61:	//Ӧ�ù���
									strTableName = "FUNCTIONS";
									break;
			case C_NODE_TYPE_B42:	//����Ȩ���ܼ���
			case C_NODE_TYPE_B62:	//Ӧ�ù��ܼ���
									strTableName = "FUNCTION_SETS";
									break;
			case C_NODE_TYPE_A1: 	//�����
			case C_NODE_TYPE_A7:	//��Ӧ�ü���
			case C_NODE_TYPE_B71:	//ͨ����ȨӦ��
			case C_NODE_TYPE_B72:	//һ��Ӧ��
									strTableName = "APPLICATIONS";
									break;
			case C_NODE_TYPE_A4:	//����Ȩ������
									break;
			case C_NODE_TYPE_A6:	//Ӧ�ù�����
									break;
		}
		return strTableName;
	}

	function isPreDefineObj(strType)
	{
		switch (strType)
		{
			//Ӧ�������Ͻ�����ͣ�node.xData.type
			case C_NODE_TYPE_B71:	//ͨ����ȨӦ��
			case C_NODE_TYPE_B31:	//����Ȩ��ɫ
			case C_NODE_TYPE_B41:	//����Ȩ����
			case C_NODE_TYPE_B42:	//����Ȩ���ܼ���
									return true;
									break;
			default: return false;
		}
	}

/*	
		switch (type)
		{
			//Ӧ�������Ͻ�����ͣ�node.xData.type
			case C_NODE_TYPE_A1: 	//�����
									break;
			case C_NODE_TYPE_A2:	//Ӧ�÷���Χ����
									break;
			case C_NODE_TYPE_A3:	//����Ȩ��ɫ����
									break;
			case C_NODE_TYPE_A4:	//����Ȩ������
									break;
			case C_NODE_TYPE_A5:	//Ӧ�ý�ɫ����
									break;
			case C_NODE_TYPE_A6:	//Ӧ�ù�����
									break;
			case C_NODE_TYPE_A7:	//��Ӧ�ü���
									break;
			

			//Ӧ����Ҷ������ͣ�node.xData.type
			case C_NODE_TYPE_B21:	//��������Χ
									break;
			case C_NODE_TYPE_B22:	//���ݷ���Χ
									break;
			case C_NODE_TYPE_B31:	//����Ȩ��ɫ
									break;
			case C_NODE_TYPE_B41:	//����Ȩ����
									break;
			case C_NODE_TYPE_B42:	//����Ȩ���ܼ���
									break;
			case C_NODE_TYPE_B51:	//Ӧ�ý�ɫ
									break;
			case C_NODE_TYPE_B61:	//Ӧ�ù���
									break;
			case C_NODE_TYPE_B62:	//Ӧ�ù��ܼ���
									break;
			case C_NODE_TYPE_B71:	//ͨ����ȨӦ��
									break;
			case C_NODE_TYPE_B72:	//һ��Ӧ��
									break;
			//����Ȩ���������
			case C_OBJ_TYPE_A1:		//��Ա
									break;
			case C_OBJ_TYPE_A2:		//����	
									break;
			case C_OBJ_TYPE_A3:		//��
									break;
		}
*/
//-->

