function initControlsFocus(signInNameID, oldPasswordID) {
	var signInName = document.all(signInNameID);
	var oldPassword = document.all(oldPasswordID);

	try {
		document.onkeydown = null;

		if (signInName) {
			if (signInName.value.length > 0) {
				if (oldPassword)
					oldPassword.focus();
				else
					signInName.focus();
			}
			else
				signInName.focus();
		}
	}
	catch (e) {
	}
}

function doDetailErrorMessageClick(detailErrorMessageID) {
	var detailErrorMessage = document.all(detailErrorMessageID);

	try {
		if (detailErrorMessage) {
			if (detailErrorMessage.style.display == "none")
				detailErrorMessage.style.display = "block";
			else
				detailErrorMessage.style.display = "none";
		}
	}
	finally {
		event.returnValue = false;
	}
}

function beforeSubmit(signInNameID, errorMessageID, newPasswordID, confirmPasswordID) {
	var result = true;
	var errorMessage = "";

	document.getElementById(errorMessageID).innerText = "";

	if (document.getElementById(signInNameID).value == "") {
		errorMessage = "用户的登录名不能为空";
	}

	if (document.getElementById(newPasswordID).value != document.getElementById(confirmPasswordID).value) {
		errorMessage = errorMessage + "\n" + "新密码和确认密码必须相同";
	}

	if (errorMessage != "") {
		result = false;
		document.getElementById(errorMessageID).innerText = errorMessage;
	}

	event.returnValue = result;

	return result;
}

function onBackButtonClick() {
	window.navigate(event.srcElement.backUrl);
}