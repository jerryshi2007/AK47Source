<!--
	var C_APP_ADMIN_ID = 1;

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
			case "APP_LOG_TYPE":
			case "APP_OPERATION_TYPE":
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

	function getNameFromType(strClass)
	{
		var strName = "";

		switch(strClass)
		{
			case "FUNCTIONS":
							strName = "功能";
							break;
			case "APP_LOG_TYPE":
							strName = "应用程序";
							break;
			case "RESOURCES":
							strName = "资源";
							break;
			case "ROLES":	strName = "角色";
							break;
			case "USERS_TO_ROLES":
							strName = "用户或机构";
							break;
		}

		return strName;
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

//-->