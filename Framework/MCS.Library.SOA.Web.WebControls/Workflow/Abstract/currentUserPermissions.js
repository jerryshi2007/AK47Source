var _currentUserIsAdmin = $_currentUserIsAdmin$;
var _currentUserAppAuthInfoString = '$_currentUserAppAuthInfoString$';

var _currentUserAppAuthInfo = Sys.Serialization.JavaScriptSerializer.deserialize(_currentUserAppAuthInfoString);

function _doesCurrentUserHavePermission(appName, progName, authType) {
	var result = false;

	for (var i = 0; i < _currentUserAppAuthInfo.length; i++) {
		var appAuthInfo = _currentUserAppAuthInfo[i];

		if (appAuthInfo.ApplicationName == appName && appAuthInfo.ProgramName == progName && appAuthInfo.AuthType == authType) {
			result = true;
			break;
		}
	}

	return result;
}