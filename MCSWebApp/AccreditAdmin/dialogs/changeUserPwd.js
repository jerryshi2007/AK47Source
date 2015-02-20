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
		if (confirm("您确定要修改自己的系统登录密码么？"))
		{
			trueThrow(frmInput.oldPwd.value == "", "对不起，您没有填写旧密码！");
			trueThrow(frmInput.userPassword.value == "", "对不起，您没有填写新密码（系统不允许空密码）！");
			falseThrow(frmInput.userPassword.value == frmInput.pwdRetype.value,"您的新密码与确认密码不符，请确认！");
			
			var xmlDoc = createDomDocument("<ResetPassword/>");
			var root = xmlDoc.documentElement;
			appendNode(root, "GUID", frmInput.UserGuid.value);
			appendNode(root, "OldPwd", frmInput.oldPwd.value);
			appendNode(root, "OldPwdType", frmInput.oldPwdType.value);
			appendNode(root, "NewPwd", frmInput.userPassword.value);
			appendNode(root, "NewPwdType", frmInput.newPwdType.value);
			var xmlResult = xmlSend("../sysAdmin/OGUEditer.aspx", xmlDoc);
			
			checkErrorResult(xmlResult);
			
			alert("密码修改成功！");
			window.close();
		}
	}
	catch (e)
	{
		showError(e);
	}
}

//-->