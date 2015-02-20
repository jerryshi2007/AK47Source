
$HGRootNS._ClientMsg = function()
{
    this._images = [
        {msgType : "inform", url : "<%=WebResource("MCS.Web.Library.Resources.ClientMsg.inform.gif")%>"},
        {msgType : "alert", url : "<%=WebResource("MCS.Web.Library.Resources.ClientMsg.alert.gif")%>"},
        {msgType : "stop", url : "<%=WebResource("MCS.Web.Library.Resources.ClientMsg.stop.gif")%>"},
        {msgType : "confirm", url : "<%=WebResource("MCS.Web.Library.Resources.ClientMsg.confirm.gif")%>"}
    ];
    this._dialogUrl = null;
}

$HGRootNS._ClientMsg.prototype = 
{
	inform : function(msg, detailMsg, title)
	{
		if (!title)
			title = "提示";

		this._popMsgWindow("inform", msg, detailMsg, title);
	},
	
	alert : function(msg, detailMsg, title)
	{
		if (!title)
			title = "警告";

		this._popMsgWindow("alert", msg, detailMsg, title);
	},
	
	stop : function(msg, detailMsg, title)
	{
		if (!title)
			title = "错误";

		this._popMsgWindow("stop", msg, detailMsg, title);
	},
	
	confirm : function(msg, detailMsg, title, okBtnText, cancelBtnText, okBtnMethod, cancelBtnMethod)
	{
		if (!title)
			title = "选择";
		
		return this._popMsgWindow("confirm", msg, detailMsg, title, okBtnText, cancelBtnText, okBtnMethod, cancelBtnMethod);
	},
	
	_getImgUrl : function(msgType)
	{
	    for (var i = 0; i < this._images.length; i++)
	    {
	        if (msgType == this._images[i].msgType)
	            return this._images[i].url;
	    }
	    
	    return "";
	},
	
	_popMsgWindow : function(msgType, msg, detailMsg, title, okBtnText, cancelBtnText, okBtnMethod, cancelBtnMethod)
	{
		var arg = new Object();

		arg.msgType = msgType;
		arg.msg = msg;
		arg.imgUrl = this._getImgUrl(msgType);
		arg.detailMsg = detailMsg;
		arg.okBtnText = okBtnText;
		arg.cancelBtnText = cancelBtnText;
		arg.title = title;

		if (typeof(mseeageNotifyMailAddress) != "undefined")
			arg.notifyMailAddress = mseeageNotifyMailAddress;

		if (typeof($NT) != "undefined"){
			arg.nameTable = $NT;
		}
		else {
			arg.nameTable = { category: {} };
		}

		var bResult = false;

		try
		{
			bResult = window.showModalDialog("<%=WebResource("MCS.Web.Library.Resources.ClientMsg.WebMsgBox.htm")%>", arg, "dialogHeight:126px;dialogWidth:209px;help:no;scroll:no;status:no");
		}
		catch(e)
		{
			if (msgType == "confirm")
				bResult = window.confirm(msg);
			else
			{
				bResult = true;
				alert(msg);
			}
		}
		
		if (bResult)
		{
		    if (okBtnMethod) 
		        okBtnMethod.call();
		}
		else
		{
		    if (cancelBtnMethod)
		        cancelBtnMethod.call();
		}
		
		return bResult;
	}
}

$HGClientMsg = $HGRootNS.ClientMsg = new $HGRootNS._ClientMsg();

$showError = function(err) {
	var description = "";
	var message = "";

	if (typeof(err.message) != "undefined")
		message = err.message;
	else
	if (typeof(err.get_message) != "undefined")
		message = err.get_message();
	else
		message = err;

	if (typeof(err.description) != "undefined")
		description = err.description;
	else
	if (typeof(err.get_stackTrace) != "undefined")
		description = err.get_stackTrace();

	$HGClientMsg.stop(message, description, "错误");
}