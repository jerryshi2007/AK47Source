	//向WebServer发送请求，得到查询结果
	function queryList(xmlDoc)
	{	
		//alert(xmlDoc.xml);
			
		//进行查询
		var xmlResult = xmlSend("./XmlRequestService/XmlReadRequest.aspx", xmlDoc);
		checkErrorResult(xmlResult);
		//alert(xmlResult.xml);
		return xmlResult;
		
	}

	//查询应用树
	//<queryList type="APPLICATION" parent_id=""/>
	//<queryList type="APPLICATION" parent_id="xxx"/>
	function queryApplication(parentID)
	{
		var xmlDoc;	
		xmlDoc = createDomDocument("<queryList/>");

		appendAttr(xmlDoc, xmlDoc.documentElement, "type", "APPLICATION");
		appendAttr(xmlDoc, xmlDoc.documentElement, "parent_id", parentID);
		
		return queryList(xmlDoc);
	}
	
	//查询服务范围
	//<queryList type="APP_SCOPE" app_id="xxx" />
	function queryScope(appID)
	{
		var xmlDoc;	
		xmlDoc = createDomDocument("<queryList/>");

		appendAttr(xmlDoc, xmlDoc.documentElement, "type", "APP_SCOPE");
		appendAttr(xmlDoc, xmlDoc.documentElement, "app_id", appID);
		
		return queryList(xmlDoc);
	}

	//自授权角色
	//<queryList type="ROLE" app_id="xxx" classify="y"/>
	function querySelfRole(appID)
	{
		var xmlDoc;	
		xmlDoc = createDomDocument("<queryList/>");

		appendAttr(xmlDoc, xmlDoc.documentElement, "type", "ROLE");
		appendAttr(xmlDoc, xmlDoc.documentElement, "app_id", appID);
		appendAttr(xmlDoc, xmlDoc.documentElement, "classify", "y");
		
		return queryList(xmlDoc);
	}

	//自授权功能
	//<queryList type="FUNCTION" app_id="xxx" classify="y" parent_id=""/>
	//<queryList type="FUNCTION" app_id="xxx" classify="y" parent_id="xxx"/>
	function querySelfFunc( appID, parentID )
	{
		var xmlDoc;	
		xmlDoc = createDomDocument("<queryList/>");

		appendAttr(xmlDoc, xmlDoc.documentElement, "type", "FUNCTION");
		appendAttr(xmlDoc, xmlDoc.documentElement, "app_id", appID);
		appendAttr(xmlDoc, xmlDoc.documentElement, "classify", "y");
		if (parentID)
			appendAttr(xmlDoc, xmlDoc.documentElement, "parent_id", parentID);
		else
			appendAttr(xmlDoc, xmlDoc.documentElement, "parent_id", "");
		
		return queryList(xmlDoc);
	}

	//应用授权角色
	//<queryList type="ROLE" app_id="xxx" classify="n"/>
	function queryAppRole( appID )
	{
		var xmlDoc;	
		xmlDoc = createDomDocument("<queryList/>");

		appendAttr(xmlDoc, xmlDoc.documentElement, "type", "ROLE");
		appendAttr(xmlDoc, xmlDoc.documentElement, "app_id", appID);
		appendAttr(xmlDoc, xmlDoc.documentElement, "classify", "n");
		
		return queryList(xmlDoc);
	}
	
	//应用授权功能
	//<queryList type="FUNCTION" app_id="xxx" classify="n" parent_id=""/>
	//<queryList type="FUNCTION" app_id="xxx" classify="n" parent_id="xxx"/>
	function queryAppFunc( appID, parentID )
	{
		var xmlDoc;	
		xmlDoc = createDomDocument("<queryList/>");

		appendAttr(xmlDoc, xmlDoc.documentElement, "type", "FUNCTION");
		appendAttr(xmlDoc, xmlDoc.documentElement, "app_id", appID);
		appendAttr(xmlDoc, xmlDoc.documentElement, "classify", "n");
		if (parentID)
			appendAttr(xmlDoc, xmlDoc.documentElement, "parent_id", parentID);
		else
			appendAttr(xmlDoc, xmlDoc.documentElement, "parent_id", "");
		
		return queryList(xmlDoc);
	}
	
	//查询功能对应的角色
	//<queryList type="FUNCTION_TO_ROLE" app_id="xxx" func_id="xxx"/>
	function queryFunction2Role( appID, funcID )
	{
		var xmlDoc;	
		xmlDoc = createDomDocument("<queryList/>");

		appendAttr(xmlDoc, xmlDoc.documentElement, "type", "FUNCTION_TO_ROLE");
		appendAttr(xmlDoc, xmlDoc.documentElement, "app_id", appID);
		appendAttr(xmlDoc, xmlDoc.documentElement, "func_id", funcID);
		
		return queryList(xmlDoc);
	}
	
	//<queryList type="ROLE_TO_FUNCTION" app_id="xxx" role_id="xxx"/>
	function queryRole2Function( appID, roleID)
	{
		var xmlDoc;	
		xmlDoc = createDomDocument("<queryList/>");

		appendAttr(xmlDoc, xmlDoc.documentElement, "type", "ROLE_TO_FUNCTION");
		appendAttr(xmlDoc, xmlDoc.documentElement, "app_id", appID);
		appendAttr(xmlDoc, xmlDoc.documentElement, "role_id", roleID);
		
		return queryList(xmlDoc);
	}
	
	//<queryList type="FUNCTION_SET_TO_ROLE" app_id="xxx" func_set_id='xxx'/>
	function queryFuncSet2Role( appID, FuncSetID )
	{
		var xmlDoc;	
		xmlDoc = createDomDocument("<queryList/>");

		appendAttr(xmlDoc, xmlDoc.documentElement, "type", "FUNCTION_SET_TO_ROLE");
		appendAttr(xmlDoc, xmlDoc.documentElement, "app_id", appID);
		appendAttr(xmlDoc, xmlDoc.documentElement, "func_set_id", FuncSetID);
		return queryList(xmlDoc);
	}
	
	//<queryList type="FUNCTION_SET_TO_FUNCTION" app_id="xxx" func_set_id="xxx"/>
	function queryFuncSet2Function( appID, FuncSetID )
	{
		var xmlDoc;	
		xmlDoc = createDomDocument("<queryList/>");

		appendAttr(xmlDoc, xmlDoc.documentElement, "type", "FUNCTION_SET_TO_FUNCTION");
		appendAttr(xmlDoc, xmlDoc.documentElement, "app_id", appID);
		appendAttr(xmlDoc, xmlDoc.documentElement, "func_set_id", FuncSetID);
		
		return queryList(xmlDoc);
	}
	
	//<queryList type="ROLE_TO_EXPRESSION" app_id="xxx" role_id="xxx"/>
	function queryRole2Exp( appID, roleID )
	{
		var xmlDoc;	
		xmlDoc = createDomDocument("<queryList/>");

		appendAttr(xmlDoc, xmlDoc.documentElement, "type", "ROLE_TO_EXPRESSION");
		appendAttr(xmlDoc, xmlDoc.documentElement, "app_id", appID);
		appendAttr(xmlDoc, xmlDoc.documentElement, "role_id", roleID);
		
		return queryList(xmlDoc);
	} 
	
	//<queryList type="EXPRESSION_SCOPE" app_id="xxx" exp_id="xxx"/>
	function queryExpScope( appID, expID )
	{
		var xmlDoc;	
		xmlDoc = createDomDocument("<queryList/>");

		appendAttr(xmlDoc, xmlDoc.documentElement, "type", "EXPRESSION_SCOPE");
		appendAttr(xmlDoc, xmlDoc.documentElement, "app_id", appID);
		appendAttr(xmlDoc, xmlDoc.documentElement, "exp_id", expID);
		
		return queryList(xmlDoc);
	}
	
	//<queryObj type="xxx" ID="xxx" app_id="xxx"/>
	//<queryObj type="xxx" ID="xxx" />
	function queryObj( strTableName, ID, appID )
	{
		var xmlDoc = createDomDocument("<queryObj/>");

		var strTable = strTableName;
			
		appendAttr(xmlDoc, xmlDoc.documentElement, "type", strTable);

		if (ID)
			appendAttr(xmlDoc, xmlDoc.documentElement, "id", ID);
		
		if (appID && appID.length > 0)
			appendAttr(xmlDoc, xmlDoc.documentElement, "app_id", appID);

		//var xmlResult = xmlSend("../AppAdminOP/AppDBReader.aspx", xmlDoc);
		var xmlResult = xmlSend("../XmlRequestService/XmlReadRequest.aspx", xmlDoc);
		checkErrorResult(xmlResult);
		//alert(xmlResult.xml);
		return xmlResult;

	}
	
	//<queryUserFuncScopes app_code_name="xxx" func_code_names="xxx" delegation_mask="xxx" scope_mask=""/>
	function queryUserFuncScopes(appCodeName, funcCodeNames, delegationMask, scopeMask)
	{
		var xmlDoc = createDomDocument("<queryUserFuncScopes/>");
		appendAttr(xmlDoc, xmlDoc.documentElement, "app_code_name", appCodeName);
		appendAttr(xmlDoc, xmlDoc.documentElement, "func_code_names", funcCodeNames);
		if (delegationMask)
			appendAttr(xmlDoc, xmlDoc.documentElement, "delegation_mask", delegationMask);
		if (scopeMask)
			appendAttr(xmlDoc, xmlDoc.documentElement, "scope_mask", scopeMask);
		
		var xmlResult = xmlSend("../XmlRequestService/XmlReadRequest.aspx", xmlDoc);
		checkErrorResult(xmlResult);
		//alert(xmlResult.xml);
		return xmlResult;
	}
	
	
	
