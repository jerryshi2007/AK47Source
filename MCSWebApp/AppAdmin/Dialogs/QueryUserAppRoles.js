	function preSubmit()
	{
		return true;
	}

	function getUser()
	{
		try
		{
			document.all.hdUserGuid.value = "";
			document.all.hdAllPathName.value = ""; 
			var xd = createDomDocument( document.all.hdConfig.value );
			xmlResult = showSelectUsersToRoleDialog(xd);
			/*********************************************************
				//返回信息的格式
				<NodesSelected ACCESS_LEVEL="">
					<object 
						OBJECTCLASS="ORGANIZATIONS" 
						POSTURAL="" 
						RANK_NAME=""
						STATUS="1" 
						ALL_PATH_NAME="中国海关\01海关总署\" 
						GLOBAL_SORT="000000000000" 
						ORIGINAL_SORT="000000000000" 
						DISPLAY_NAME="海关总署" 
						OBJ_NAME="01海关总署" 
						LOGON_NAME="" 
						PARENT_GUID="e588c4c6-4097-4979-94c2-9e2429989932" 
						GUID="567e75f7-59b9-477b-9053-9772bc30eae5"
					/>
				</NodesSelected>
			************************************************************/
			if (xmlResult)
			{
				if (typeof(xmlResult) == "string")
					xmlResult = createDomDocument(xmlResult);
				var root = xmlResult.documentElement;
				for(var i = 0; i < root.childNodes.length; i++)
				{
					
					strObjId		= getNodeAttribute(root.childNodes[i], "GUID");

						
						
					strType			= getNodeAttribute(root.childNodes[i], "OBJECTCLASS");
					strAllPath		= getNodeAttribute(root.childNodes[i], "ALL_PATH_NAME");
					strParentId		= getNodeAttribute(root.childNodes[i], "PARENT_GUID");
					strName			= getNodeAttribute(root.childNodes[i], "DISPLAY_NAME");
					switch(strType)
					{
						case "USERS":
							strClassify = "0";
							break;
						case "ORGANIZATIONS":
							strClassify = "1";
							break;
						case "GROUPS":
							strClassify = "2";
							break;
					}
					
					if ( strClassify != "0" )
						continue;
						
					document.all.hdUserGuid.value = strObjId;
					document.all.hdAllPathName.value = strAllPath; 
					document.all.txtUser.value = strName; 
					break;
				}

				if (document.all.hdUserGuid.value != "")
				{
					document.all.btnQuery.disabled = false;
				}
				else
				{
					document.all.btnQuery.disabled = true;
				}
			}

		}
		catch(e)
		{
			showError(e);
		}
	}