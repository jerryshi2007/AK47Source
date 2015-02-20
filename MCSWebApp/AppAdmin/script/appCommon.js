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
			case "userAccountControl"://账号禁用
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
							if (confirm("用户登录名称已经修改，是否同时修改邮件的别名？"))
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
	// strOP				操作类型
	// nID					对象id,或父对名id
	// strType				类型（表名）
	// strFatherNodeType	父结点类型（有时需要）
	// strAppID				应用id
	// strAppLevel			应用层次
	// strCodeName			对象标识
	// strClassify			对象分类
	// strInherited			是否从父应用继承，（insert操作时有用）
	function showAppObjDetailDialog(params)
	{
		var sFeature = "dialogWidth:320px; dialogHeight:300px;center:yes;help:no;resizable:yes;scroll:no;status:no";

		if (params.type == "APPLICATIONS")//对于应用来说，要求的显示界面更大一些
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
									if(params.classify == "y" && params.inherited == "n")//新建机构服务范围
									{
										sPath = sPath + "../dialogs/editScope1.htm";
									}
									if(params.classify == "y" && params.inherited == "y")//继承机构服务范围
									{
										sPath = sPath + "待定";
									}
									if(params.classify == "n" && params.inherited == "n")//新建数据服务范围
									{
										sPath = sPath + "待定";
									}
									if(params.classify == "n" && params.inherited == "y")//继承数据服务范围
									{
										sPath = sPath + "待定";
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
	


	/*//新增的内容*/
	//
	var C_APP_ADMIN_CODE_NAME = "APP_ADMIN";
	
	//应用树集合结点类型：node.xData.type
	var C_NODE_TYPE_A1 = "rootNode"; 		//根结点
	var C_NODE_TYPE_A2 = "appScopeNode";	//应用服务范围集合
	var C_NODE_TYPE_A3 = "selfRoleNode";	//自授权角色集合
	var C_NODE_TYPE_A4 = "selfFuncNode";	//自授权功能结点集合
	var C_NODE_TYPE_A5 = "appRoleNode";		//应用角色集合
	var C_NODE_TYPE_A6 = "appFuncNode";		//应用功能结点集合
	var C_NODE_TYPE_A7 = "applicationNode";	//子应用集合
	

	//应用树叶结点类型：node.xData.type
	var C_NODE_TYPE_B21 = "OUScope";		//机构服务范围
	var C_NODE_TYPE_B22 = "dataScope";		//数据服务范围
	var C_NODE_TYPE_B31 = "selfRole";		//自授权角色
	var C_NODE_TYPE_B41 = "selfFunc";		//自授权功能
	var C_NODE_TYPE_B42 = "selfFuncSet";	//自授权功能集合
	var C_NODE_TYPE_B51 = "appRole";		//应用角色
	var C_NODE_TYPE_B61 = "appFunc";		//应用功能
	var C_NODE_TYPE_B62 = "appFuncSet";		//应用功能集合
	var C_NODE_TYPE_B71 = "appAdmin";		//通用授权应用
	var C_NODE_TYPE_B72 = "application"		//一般应用
	//被授权对象的类型
	var C_OBJ_TYPE_A1	= "user";			//人员
	var C_OBJ_TYPE_A2	= "organization";	//机构	
	var C_OBJ_TYPE_A3	= "group";			//组

	function getImgFromType(strType)
	{
		var imgName = "";
		switch(strType)
		{
			case C_NODE_TYPE_B21://机构服务范围
							imgName = "Scope1.gif";
							break;
			
			case C_NODE_TYPE_B22://数据服务范围
							imgName = "Scope2.gif";
							break;

			case C_NODE_TYPE_B31://自授权角色
			case C_NODE_TYPE_B51://应用角色
							imgName = "role.gif";
							break;

			case C_NODE_TYPE_B41://自授权功能
			case C_NODE_TYPE_B61://应用功能
							imgName = "function.gif";
							break;

			case C_NODE_TYPE_B42://自授权功能集合
			case C_NODE_TYPE_B62://应用功能集合
							imgName = "functionSet.gif";
							break;
			case C_NODE_TYPE_B71://通用授权应用
							imgName = "computer.gif";
							break;
			case C_NODE_TYPE_B72://一般应用
							imgName = "application.gif";
							break;
			//被授权对象的类型
			case C_OBJ_TYPE_A1://人员
							imgName = "user.gif";
							break;
			case C_OBJ_TYPE_A2://机构	
							imgName = "organization.gif";
							break;
			case C_OBJ_TYPE_A3://组
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
			case C_NODE_TYPE_A2://应用服务范围集合
							if ( strClassify == "y" )
								strType = C_NODE_TYPE_B21;
							else
								strType = C_NODE_TYPE_B22;
							break;
			case C_NODE_TYPE_A3://自授权角色集合
							strType = C_NODE_TYPE_B31;
							break;
			case C_NODE_TYPE_A4://自授权功能树
			case C_NODE_TYPE_B42://自授权功能集合
							if ( parseInt(strFuncType) == 0 )
								strType = C_NODE_TYPE_B41;
							else if ( parseInt(strFuncType) == 1 )
								strType = C_NODE_TYPE_B42;
							else
								strType = C_NODE_TYPE_B31;
							break;
			case C_NODE_TYPE_A5://应用角色集合
							strType = C_NODE_TYPE_B51;
							break;
			case C_NODE_TYPE_A6://应用功能树
			case C_NODE_TYPE_B62://应用功能集合
							if ( parseInt(strFuncType) == 0 )
								strType = C_NODE_TYPE_B61;
							else if ( parseInt(strFuncType) == 1 ) 
								strType = C_NODE_TYPE_B62;
							else 
								strType = C_NODE_TYPE_B31;
							break;
			case C_NODE_TYPE_B41://自授权功能
			case C_NODE_TYPE_B61://应用功能
							strType = C_NODE_TYPE_B31;
							break;
			case C_NODE_TYPE_B31://自授权角色
			case C_NODE_TYPE_B51://应用角色
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

			case C_NODE_TYPE_A1://根结点
			case C_NODE_TYPE_A7://子应用集合
							if ( strCodeName == C_APP_ADMIN_CODE_NAME )
								strType = C_NODE_TYPE_B71; 
							else
								strType = C_NODE_TYPE_B72; 
							break;
			case C_OBJ_TYPE_A1://人员
			case C_OBJ_TYPE_A2://机构	
			case C_OBJ_TYPE_A3://组
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
			//应用树集合结点类型：node.xData.type
			case C_NODE_TYPE_A1: 	//根结点
									strName = "根结点";
									break;
			case C_NODE_TYPE_A2:	//应用服务范围集合
									strName = "应用服务范围集合";
									break;
			case C_NODE_TYPE_A3:	//自授权角色集合
									strName = "自授权角色集合";
									break;
			case C_NODE_TYPE_A4:	//自授权功能树
									strName = "自授权功能树";
									break;
			case C_NODE_TYPE_A5:	//应用角色集合
									strName = "应用角色集合";
									break;
			case C_NODE_TYPE_A6:	//应用功能树
									strName = "应用功能树";
									break;
			case C_NODE_TYPE_A7:	//子应用集合
									strName = "子应用集合";
									break;
			

			//应用树叶结点类型：node.xData.type
			case C_NODE_TYPE_B21:	//机构服务范围
									strName = "机构服务范围";
									break;
			case C_NODE_TYPE_B22:	//数据服务范围
									strName = "数据服务范围";
									break;
			case C_NODE_TYPE_B31:	//自授权角色
									strName = "自授权角色";
									break;
			case C_NODE_TYPE_B41:	//自授权功能
									strName = "自授权功能";
									break;
			case C_NODE_TYPE_B42:	//自授权功能集合
									strName = "自授权功能集合";
									break;
			case C_NODE_TYPE_B51:	//应用角色
									strName = "应用角色";
									break;
			case C_NODE_TYPE_B61:	//应用功能
									strName = "应用功能";
									break;
			case C_NODE_TYPE_B62:	//应用功能集合
									strName = "应用功能集合";
									break;
			case C_NODE_TYPE_B71:	//通用授权应用
									strName = "通用授权应用";
									break;
			case C_NODE_TYPE_B72:	//一般应用
									strName = "一般应用";
									break;
			case C_OBJ_TYPE_A1:		//人员
									strName = "人员";
									break;
			case C_OBJ_TYPE_A2:		//机构	
									strName = "机构";
									break;
			case C_OBJ_TYPE_A3:		//组
									strName = "组";
									break;
		}

		return strName;
	}

	function getTableFromType (type)
	{
		var strTableName = "";
		switch (type)
		{
			case C_NODE_TYPE_A2:	//应用服务范围集合
			case C_NODE_TYPE_B21:	//机构服务范围
			case C_NODE_TYPE_B22:	//数据服务范围
									strTableName = "SCOPES";
									break;
			case C_NODE_TYPE_A3:	//自授权角色集合
			case C_NODE_TYPE_A5:	//应用角色集合
			case C_NODE_TYPE_B31:	//自授权角色
			case C_NODE_TYPE_B51:	//应用角色
									strTableName = "ROLES";
									break;
			case C_NODE_TYPE_B41:	//自授权功能
			case C_NODE_TYPE_B61:	//应用功能
									strTableName = "FUNCTIONS";
									break;
			case C_NODE_TYPE_B42:	//自授权功能集合
			case C_NODE_TYPE_B62:	//应用功能集合
									strTableName = "FUNCTION_SETS";
									break;
			case C_NODE_TYPE_A1: 	//根结点
			case C_NODE_TYPE_A7:	//子应用集合
			case C_NODE_TYPE_B71:	//通用授权应用
			case C_NODE_TYPE_B72:	//一般应用
									strTableName = "APPLICATIONS";
									break;
			case C_NODE_TYPE_A4:	//自授权功能树
									break;
			case C_NODE_TYPE_A6:	//应用功能树
									break;
		}
		return strTableName;
	}

	function isPreDefineObj(strType)
	{
		switch (strType)
		{
			//应用树集合结点类型：node.xData.type
			case C_NODE_TYPE_B71:	//通用授权应用
			case C_NODE_TYPE_B31:	//自授权角色
			case C_NODE_TYPE_B41:	//自授权功能
			case C_NODE_TYPE_B42:	//自授权功能集合
									return true;
									break;
			default: return false;
		}
	}

/*	
		switch (type)
		{
			//应用树集合结点类型：node.xData.type
			case C_NODE_TYPE_A1: 	//根结点
									break;
			case C_NODE_TYPE_A2:	//应用服务范围集合
									break;
			case C_NODE_TYPE_A3:	//自授权角色集合
									break;
			case C_NODE_TYPE_A4:	//自授权功能树
									break;
			case C_NODE_TYPE_A5:	//应用角色集合
									break;
			case C_NODE_TYPE_A6:	//应用功能树
									break;
			case C_NODE_TYPE_A7:	//子应用集合
									break;
			

			//应用树叶结点类型：node.xData.type
			case C_NODE_TYPE_B21:	//机构服务范围
									break;
			case C_NODE_TYPE_B22:	//数据服务范围
									break;
			case C_NODE_TYPE_B31:	//自授权角色
									break;
			case C_NODE_TYPE_B41:	//自授权功能
									break;
			case C_NODE_TYPE_B42:	//自授权功能集合
									break;
			case C_NODE_TYPE_B51:	//应用角色
									break;
			case C_NODE_TYPE_B61:	//应用功能
									break;
			case C_NODE_TYPE_B62:	//应用功能集合
									break;
			case C_NODE_TYPE_B71:	//通用授权应用
									break;
			case C_NODE_TYPE_B72:	//一般应用
									break;
			//被授权对象的类型
			case C_OBJ_TYPE_A1:		//人员
									break;
			case C_OBJ_TYPE_A2:		//机构	
									break;
			case C_OBJ_TYPE_A3:		//组
									break;
		}
*/
//-->

