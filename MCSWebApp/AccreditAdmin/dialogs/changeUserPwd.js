<!--
function onDocumentLoad()
{
	try
	{
		window.returnValue = "";		
		initDocumentEvents(frmInput);
	}
	catch (e)
	{
		showError(e);
	}
}

function onSaveClick()
{
	try
	{
		if (confirm("��ȷ��Ҫ�޸��Լ���ϵͳ��¼����ô��"))
		{
			trueThrow(frmInput.oldPwd.value == "", "�Բ�����û����д�����룡");
			trueThrow(frmInput.userPassword.value == "", "�Բ�����û����д�����루ϵͳ����������룩��");
			falseThrow(frmInput.userPassword.value == frmInput.pwdRetype.value,"������������ȷ�����벻������ȷ�ϣ�");
			
			var xmlDoc = createDomDocument("<ResetPassword/>");
			var root = xmlDoc.documentElement;
			appendNode(root, "GUID", frmInput.UserGuid.value);
			appendNode(root, "OldPwd", frmInput.oldPwd.value);
			appendNode(root, "OldPwdType", frmInput.oldPwdType.value);
			appendNode(root, "NewPwd", frmInput.userPassword.value);
			appendNode(root, "NewPwdType", frmInput.newPwdType.value);
			var xmlResult = xmlSend("../sysAdmin/OGUEditer.aspx", xmlDoc);
			
			checkErrorResult(xmlResult);
			
			alert("�����޸ĳɹ���");
			window.close();
		}
	}
	catch (e)
	{
		showError(e);
	}
}

//-->