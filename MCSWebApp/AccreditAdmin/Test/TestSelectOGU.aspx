<%@ Page Language="c#" Codebehind="TestSelectOGU.aspx.cs" AutoEventWireup="True"
	Inherits="MCS.Applications.AccreditAdmin.Test.TestSelectOGU" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns:hgui>
<head>
	<title>测试选择组织机构对话框</title>
	<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
	<meta name="CODE_LANGUAGE" content="C#">
	<meta name="vs_defaultClientScript" content="JavaScript">
	<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	<link href="../CSS/Input.css" type="text/css" rel="stylesheet">

	<script type="text/javascript" language="javascript" src="../oguScript/validate.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/xmlHttp.js"></script>

	<script type="text/javascript" language="javascript" src="../Script/ApplicationRoot.js"></script>

	<script type="text/javascript" language="javascript" src="../oguScript/uiScript.js"></script>

	<script type="text/javascript" language="javascript" src="../selfScript/accreditAdmin.js"></script>

	<script type="text/javascript" language="javascript">
			var g_bolShowDefine = false;
			function onSelectOGU()
			{
				try
				{
					var arg = new Object();
					
                    arg.listObjType = 65535;         	//要求树结构中列出的对象类型1=OU, 2=User, 4=Group,8=Sideline（默认列出OU和User）
                    arg.multiSelect = 1;			//1、允许多选；0、不允许多选；(默认为0)
					arg.rootOrg = "中国海关\\01海关总署";	//设置选择最大范围（默认为系统设置的最大范围）
                    /*var strXmlIn = "<NodesSelected>"
                                +       "<object ALL_PATH_NAME=\"中国海关\\01海关总署\\01办公厅\"/>"
                                +       "<object ALL_PATH_NAME=\"中国海关\\01海关总署\"/>"
                                +       "<object ALL_PATH_NAME=\"中国海关\\02广东分署\"/>"
                                + "</NodesSelected>";	
                    arg.strNodesSelected = strXmlIn;*/	// 要求被默认选中的对象
                    arg.canSelectRoot = "true";			// 是否允许选择根部（默认为false）
                    arg.selectSort = 1;					// 是否要求按照选中次序返回结果（默认为0）
                    arg.ShowMyOrg = "1";				// 是否展现当前操作人员所在的机构(cgac\yuanyong [20041101])
					arg.selectObjType = 7;/*
    var strFirstChildren = 
    '<OPERATIONS OBJECTCLASS="OPERATION" ALL_PATH_NAME="领导批办" GLOBAL_SORT="" ORIGINAL_SORT="" DISPLAY_NAME="领导批办" OBJ_NAME="领导批办" PARENT_GUID="" GUID="">' 
+		'<ORGANIZATIONS OBJECTCLASS="ORGANIZATIONS" ALL_PATH_NAME="中国海关\\01海关总署\\00署领导" GLOBAL_SORT="000000000000000000" ORIGINAL_SORT="000000000000000000" DISPLAY_NAME="署领导" OBJ_NAME="00署领导" PARENT_GUID="567e75f7-59b9-477b-9053-9772bc30eae5" GUID="65eb8160-f0fa-4f1c-8984-2649788fe1d0" />' 
+		'<GROUPS OBJECTCLASS="GROUPS" ALL_PATH_NAME="中国海关\\01海关总署\\01办公厅\\办公厅全体人员" GLOBAL_SORT="000000000000000001000017" ORIGINAL_SORT="000000000000000001000034" DISPLAY_NAME="办公厅全体人员" OBJ_NAME="办公厅全体人员" PARENT_GUID="0c6135c6-1038-48ec-a79b-ba63dffac758" GUID="1a3aed53-5e1e-40a2-9183-8163ff86d522"/>' 
+		'<USERS OBJECTCLASS="USERS" ALL_PATH_NAME="中国海关\\01海关总署\\01办公厅\\文电处\\张晓平" GLOBAL_SORT="000000000000000001000009000002" ORIGINAL_SORT="000000000000000001000009000002" DISPLAY_NAME="张晓平" OBJ_NAME="张晓平" PARENT_GUID="d3b05e66-1b7f-4245-92cd-b4a9e59f8463" GUID="1b98c70a-1c4c-46aa-8f02-69e04797619c"/>' 
+		'<ROLES OBJECTCLASS="ROLE" ALL_PATH_NAME="发文管理员" GLOBAL_SORT="" ORIGINAL_SORT="" DISPLAY_NAME="发文管理员" OBJ_NAME="发文管理员" PARENT_GUID="" GUID="">' 
+			'<ORGANIZATIONS OBJECTCLASS="ORGANIZATIONS" ALL_PATH_NAME="中国海关\\01海关总署\\00署领导" GLOBAL_SORT="000000000000000000" ORIGINAL_SORT="000000000000000000" DISPLAY_NAME="署领导" OBJ_NAME="00署领导" PARENT_GUID="567e75f7-59b9-477b-9053-9772bc30eae5" GUID="65eb8160-f0fa-4f1c-8984-2649788fe1d0" />' 
+			'<GROUPS OBJECTCLASS="GROUPS" ALL_PATH_NAME="中国海关\\01海关总署\\01办公厅\\办公厅全体人员" GLOBAL_SORT="000000000000000001000017" ORIGINAL_SORT="000000000000000001000034" DISPLAY_NAME="办公厅全体人员" OBJ_NAME="办公厅全体人员" PARENT_GUID="0c6135c6-1038-48ec-a79b-ba63dffac758" GUID="1a3aed53-5e1e-40a2-9183-8163ff86d522"/>' 
+			'<USERS OBJECTCLASS="USERS" ALL_PATH_NAME="中国海关\\01海关总署\\01办公厅\\文电处\\张晓平" GLOBAL_SORT="000000000000000001000009000002" ORIGINAL_SORT="000000000000000001000009000002" DISPLAY_NAME="张晓平" OBJ_NAME="张晓平" PARENT_GUID="d3b05e66-1b7f-4245-92cd-b4a9e59f8463" GUID="1b98c70a-1c4c-46aa-8f02-69e04797619c"/>' 
+		'</ROLES>' 
+	'</OPERATIONS>';

                    arg.firstChildren = strFirstChildren;
*/
                    var strXml = showSelectUserDialog(arg);
                    
                    if (strXml && strXml.length > 0)//测试结果用
                    {
						prompt("", strXml);
					}
				}
				catch(e)
				{
					showError(e);
				}
			}
			
			function onSelectOGUMulitServer()//跨WebServer调用方式
			{
				try
				{
					var xmlData = createDomDocument("<Params />");
					var root = xmlData.documentElement;
					appendNode(root, "multiSelect", "1");
					appendNode(root, "rootOrg", "中国海关\\01海关总署");
					var strPath = "http://10.99.201.107/Accreditadmin/exports/selectOGU.aspx";//不同WebServer
					window.alert(showSelectUserDialogMulitServer(xmlData, strPath));
				}
				catch (e)
				{
					showError(e);
				}
			}
			
			function changeSourceObj()
			{
				try
				{
				
				}
				catch (e)
				{
					showError(e);
				}
			}
			
			function onShowSelectUsersToRoleDialog()
			{
				try
				{
					var xmlDoc = createDomDocument("<Config />");
					appendNode(xmlDoc.documentElement, "RootOrg", "中国海关\\01海关总署");
					appendNode(xmlDoc.documentElement, "QueryOGU", "true");
					appendNode(xmlDoc.documentElement, "MultiSelect", "false");
					appendNode(xmlDoc.documentElement, "BottomRow", "false");
					appendNode(xmlDoc.documentElement, "Caption", "查找组织机构或用户");
					appendNode(xmlDoc.documentElement, "Logo", "url(../images/32/searchgo.gif)");
					appendNode(xmlDoc.documentElement, "LogoWidth", "45");
					appendNode(xmlDoc.documentElement, "LogoHeight", "42");
					var xmlResult = showSelectUsersToRoleDialog(xmlDoc);
					if (typeof(xmlResult) != "undefined" && xmlResult != null)
					{
						if (typeof(xmlResult) == "object")
							xmlResult = xmlResult.xml;
						if (xmlResult.length > 0)
							alert(xmlResult);
					}
				}
				catch (e)
				{
					showError(e);
				}
			}
			
			function onChowSelectUsersToRoleDialogMulitServer()
			{
				try
				{
					var xmlDoc = createDomDocument("<Config />");
					appendNode(xmlDoc.documentElement, "RootOrg", "中国海关");
					appendNode(xmlDoc.documentElement, "QueryOGU", "true");
					appendNode(xmlDoc.documentElement, "MultiSelect", "false");
					appendNode(xmlDoc.documentElement, "BottomRow", "false");
					appendNode(xmlDoc.documentElement, "Caption", "查找组织机构或用户(cesh)");
					appendNode(xmlDoc.documentElement, "Logo", "url(../images/32/searchgo.gif)");
					appendNode(xmlDoc.documentElement, "LogoWidth", "45");
					appendNode(xmlDoc.documentElement, "LogoHeight", "42");
					
					var strPath = "http://10.99.201.107/Accreditadmin/exports/selectObjsToRole.aspx";//不同WebServer
					var xmlResult = showSelectUsersToRoleDialogMulitServer(xmlDoc, strPath);
					if (typeof(xmlResult) != "undefined" && xmlResult != null)
					{
						if (typeof(xmlResult) == "object")
							xmlResult = xmlResult.xml;
						if (xmlResult.length > 0)
							alert(xmlResult);
					}
				}
				catch (e)
				{
					showError(e);
				}
			}
	</script>

	<style> @media all { hGui\:dnInput { behavior: url(OGUInput.htc); }}
		</style>
</head>
<body>
	<xml id="ADSearchConfig" src="../xml/ADSearchConfig.xml"></xml>
	<table>
		<tr>
			<td>
				<span>点击右侧的按钮弹出组织机构树</span></td>
			<td>
				<input type="button" value="选择组织机构" onclick="onSelectOGU()"></td>
		</tr>
		<tr>
			<td>
				<span>点击右侧的按钮弹出组织机构树（部署不同WebServer）</span></td>
			<td>
				<input type="button" value="选择组织机构" onclick="onSelectOGUMulitServer()"></td>
		</tr>
		<tr>
			<td>
				<span>输入对象名称</span></td>
			<td>
				<hgui:oguinput id="dnInput" onchange="changeSourceObj();" style="behavior: url(../htc/OGUInput.htc);
					width: 120px">
					<strong></strong>
				</hgui:oguinput>
			</td>
		</tr>
		<tr>
			<td>
				<span>点击右侧的按钮弹出组织机构树（选择和查询）</span></td>
			<td>
				<input type="button" value="查询、选择组织机构" onclick="onShowSelectUsersToRoleDialog()"></td>
		</tr>
		<tr>
			<td>
				<span>点击右侧的按钮弹出组织机构树（选择和查询）（部署不同WebServer）</span></td>
			<td>
				<input type="button" value="查询、选择组织机构" onclick="onChowSelectUsersToRoleDialogMulitServer()"></td>
		</tr>
	</table>
</body>
</html>
