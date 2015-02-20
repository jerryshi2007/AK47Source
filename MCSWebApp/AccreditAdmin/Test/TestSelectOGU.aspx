<%@ Page Language="c#" Codebehind="TestSelectOGU.aspx.cs" AutoEventWireup="True"
	Inherits="MCS.Applications.AccreditAdmin.Test.TestSelectOGU" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns:hgui>
<head>
	<title>����ѡ����֯�����Ի���</title>
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
					
                    arg.listObjType = 65535;         	//Ҫ�����ṹ���г��Ķ�������1=OU, 2=User, 4=Group,8=Sideline��Ĭ���г�OU��User��
                    arg.multiSelect = 1;			//1�������ѡ��0���������ѡ��(Ĭ��Ϊ0)
					arg.rootOrg = "�й�����\\01��������";	//����ѡ�����Χ��Ĭ��Ϊϵͳ���õ����Χ��
                    /*var strXmlIn = "<NodesSelected>"
                                +       "<object ALL_PATH_NAME=\"�й�����\\01��������\\01�칫��\"/>"
                                +       "<object ALL_PATH_NAME=\"�й�����\\01��������\"/>"
                                +       "<object ALL_PATH_NAME=\"�й�����\\02�㶫����\"/>"
                                + "</NodesSelected>";	
                    arg.strNodesSelected = strXmlIn;*/	// Ҫ��Ĭ��ѡ�еĶ���
                    arg.canSelectRoot = "true";			// �Ƿ�����ѡ�������Ĭ��Ϊfalse��
                    arg.selectSort = 1;					// �Ƿ�Ҫ����ѡ�д��򷵻ؽ����Ĭ��Ϊ0��
                    arg.ShowMyOrg = "1";				// �Ƿ�չ�ֵ�ǰ������Ա���ڵĻ���(cgac\yuanyong [20041101])
					arg.selectObjType = 7;/*
    var strFirstChildren = 
    '<OPERATIONS OBJECTCLASS="OPERATION" ALL_PATH_NAME="�쵼����" GLOBAL_SORT="" ORIGINAL_SORT="" DISPLAY_NAME="�쵼����" OBJ_NAME="�쵼����" PARENT_GUID="" GUID="">' 
+		'<ORGANIZATIONS OBJECTCLASS="ORGANIZATIONS" ALL_PATH_NAME="�й�����\\01��������\\00���쵼" GLOBAL_SORT="000000000000000000" ORIGINAL_SORT="000000000000000000" DISPLAY_NAME="���쵼" OBJ_NAME="00���쵼" PARENT_GUID="567e75f7-59b9-477b-9053-9772bc30eae5" GUID="65eb8160-f0fa-4f1c-8984-2649788fe1d0" />' 
+		'<GROUPS OBJECTCLASS="GROUPS" ALL_PATH_NAME="�й�����\\01��������\\01�칫��\\�칫��ȫ����Ա" GLOBAL_SORT="000000000000000001000017" ORIGINAL_SORT="000000000000000001000034" DISPLAY_NAME="�칫��ȫ����Ա" OBJ_NAME="�칫��ȫ����Ա" PARENT_GUID="0c6135c6-1038-48ec-a79b-ba63dffac758" GUID="1a3aed53-5e1e-40a2-9183-8163ff86d522"/>' 
+		'<USERS OBJECTCLASS="USERS" ALL_PATH_NAME="�й�����\\01��������\\01�칫��\\�ĵ紦\\����ƽ" GLOBAL_SORT="000000000000000001000009000002" ORIGINAL_SORT="000000000000000001000009000002" DISPLAY_NAME="����ƽ" OBJ_NAME="����ƽ" PARENT_GUID="d3b05e66-1b7f-4245-92cd-b4a9e59f8463" GUID="1b98c70a-1c4c-46aa-8f02-69e04797619c"/>' 
+		'<ROLES OBJECTCLASS="ROLE" ALL_PATH_NAME="���Ĺ���Ա" GLOBAL_SORT="" ORIGINAL_SORT="" DISPLAY_NAME="���Ĺ���Ա" OBJ_NAME="���Ĺ���Ա" PARENT_GUID="" GUID="">' 
+			'<ORGANIZATIONS OBJECTCLASS="ORGANIZATIONS" ALL_PATH_NAME="�й�����\\01��������\\00���쵼" GLOBAL_SORT="000000000000000000" ORIGINAL_SORT="000000000000000000" DISPLAY_NAME="���쵼" OBJ_NAME="00���쵼" PARENT_GUID="567e75f7-59b9-477b-9053-9772bc30eae5" GUID="65eb8160-f0fa-4f1c-8984-2649788fe1d0" />' 
+			'<GROUPS OBJECTCLASS="GROUPS" ALL_PATH_NAME="�й�����\\01��������\\01�칫��\\�칫��ȫ����Ա" GLOBAL_SORT="000000000000000001000017" ORIGINAL_SORT="000000000000000001000034" DISPLAY_NAME="�칫��ȫ����Ա" OBJ_NAME="�칫��ȫ����Ա" PARENT_GUID="0c6135c6-1038-48ec-a79b-ba63dffac758" GUID="1a3aed53-5e1e-40a2-9183-8163ff86d522"/>' 
+			'<USERS OBJECTCLASS="USERS" ALL_PATH_NAME="�й�����\\01��������\\01�칫��\\�ĵ紦\\����ƽ" GLOBAL_SORT="000000000000000001000009000002" ORIGINAL_SORT="000000000000000001000009000002" DISPLAY_NAME="����ƽ" OBJ_NAME="����ƽ" PARENT_GUID="d3b05e66-1b7f-4245-92cd-b4a9e59f8463" GUID="1b98c70a-1c4c-46aa-8f02-69e04797619c"/>' 
+		'</ROLES>' 
+	'</OPERATIONS>';

                    arg.firstChildren = strFirstChildren;
*/
                    var strXml = showSelectUserDialog(arg);
                    
                    if (strXml && strXml.length > 0)//���Խ����
                    {
						prompt("", strXml);
					}
				}
				catch(e)
				{
					showError(e);
				}
			}
			
			function onSelectOGUMulitServer()//��WebServer���÷�ʽ
			{
				try
				{
					var xmlData = createDomDocument("<Params />");
					var root = xmlData.documentElement;
					appendNode(root, "multiSelect", "1");
					appendNode(root, "rootOrg", "�й�����\\01��������");
					var strPath = "http://10.99.201.107/Accreditadmin/exports/selectOGU.aspx";//��ͬWebServer
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
					appendNode(xmlDoc.documentElement, "RootOrg", "�й�����\\01��������");
					appendNode(xmlDoc.documentElement, "QueryOGU", "true");
					appendNode(xmlDoc.documentElement, "MultiSelect", "false");
					appendNode(xmlDoc.documentElement, "BottomRow", "false");
					appendNode(xmlDoc.documentElement, "Caption", "������֯�������û�");
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
					appendNode(xmlDoc.documentElement, "RootOrg", "�й�����");
					appendNode(xmlDoc.documentElement, "QueryOGU", "true");
					appendNode(xmlDoc.documentElement, "MultiSelect", "false");
					appendNode(xmlDoc.documentElement, "BottomRow", "false");
					appendNode(xmlDoc.documentElement, "Caption", "������֯�������û�(cesh)");
					appendNode(xmlDoc.documentElement, "Logo", "url(../images/32/searchgo.gif)");
					appendNode(xmlDoc.documentElement, "LogoWidth", "45");
					appendNode(xmlDoc.documentElement, "LogoHeight", "42");
					
					var strPath = "http://10.99.201.107/Accreditadmin/exports/selectObjsToRole.aspx";//��ͬWebServer
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
				<span>����Ҳ�İ�ť������֯������</span></td>
			<td>
				<input type="button" value="ѡ����֯����" onclick="onSelectOGU()"></td>
		</tr>
		<tr>
			<td>
				<span>����Ҳ�İ�ť������֯������������ͬWebServer��</span></td>
			<td>
				<input type="button" value="ѡ����֯����" onclick="onSelectOGUMulitServer()"></td>
		</tr>
		<tr>
			<td>
				<span>�����������</span></td>
			<td>
				<hgui:oguinput id="dnInput" onchange="changeSourceObj();" style="behavior: url(../htc/OGUInput.htc);
					width: 120px">
					<strong></strong>
				</hgui:oguinput>
			</td>
		</tr>
		<tr>
			<td>
				<span>����Ҳ�İ�ť������֯��������ѡ��Ͳ�ѯ��</span></td>
			<td>
				<input type="button" value="��ѯ��ѡ����֯����" onclick="onShowSelectUsersToRoleDialog()"></td>
		</tr>
		<tr>
			<td>
				<span>����Ҳ�İ�ť������֯��������ѡ��Ͳ�ѯ��������ͬWebServer��</span></td>
			<td>
				<input type="button" value="��ѯ��ѡ����֯����" onclick="onChowSelectUsersToRoleDialogMulitServer()"></td>
		</tr>
	</table>
</body>
</html>
