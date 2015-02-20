//---------------------
ChinaCustoms = {};
ChinaCustoms.Framework = {};
ChinaCustoms.Framework.DeluxeWorks = {};
ChinaCustoms.Framework.DeluxeWorks.Web = {};
ChinaCustoms.Framework.DeluxeWorks.Web._PopupMsg = function()
{
    this._images = [
        {msgType : "inform", url : "<%=WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.Library.Resources.PopupMsg.inform.gif")%>"},
        {msgType : "alert", url : "<%=WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.Library.Resources.PopupMsg.alert.gif")%>"},
        {msgType : "stop", url : "<%=WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.Library.Resources.PopupMsg.stop.gif")%>"},
        {msgType : "confirm", url : "<%=WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.Library.Resources.PopupMsg.confirm.gif")%>"}
    ];
    this._dialogUrl = null;
};

ChinaCustoms.Framework.DeluxeWorks.Web._PopupMsg.prototype = 
{
	popInform : function(msg, detailMsg, title)
	{
		if (!title)
			title = "提示";

		this._popMsgWindow("inform", msg, detailMsg, title);
	},
	
	popAlert : function(msg, detailMsg, title)
	{
		if (!title)
			title = "警告";

		this._popMsgWindow("alert", msg, detailMsg, title);
	},
	
	popStop : function(msg, detailMsg, title)
	{
		if (!title)
			title = "错误";

		this._popMsgWindow("stop", msg, detailMsg, title);
	},
	
	popConfirm : function(msg, detailMsg, title, okBtnText, cancelBtnText)
	{
		if (!title)
			title = "选择";
		
		return this._popMsgWindow("confirm", msg, detailMsg, title, okBtnText, cancelBtnText);
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
	
	_popMsgWindow : function(msgType, msg, detailMsg, title, okBtnText, cancelBtnText)
	{
		var arg = new Object();

		arg.msgType = msgType;
		arg.msg = msg;
		arg.imgUrl = this._getImgUrl(msgType);
		arg.detailMsg = detailMsg;
		arg.okBtnText = okBtnText;
		arg.cancelBtnText = cancelBtnText;
		
		var bResult = window.showModalDialog("<%=WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.Library.Resources.PopupMsg.WebMsgBox.htm")%>&title=" + escape(title), arg, "dialogHeight:126px;dialogWidth:209px;help:no;scroll:no;status:no");
		
		return bResult;
	}
};

$HGPopupMsg = ChinaCustoms.Framework.DeluxeWorks.Web.PopupMsg = new ChinaCustoms.Framework.DeluxeWorks.Web._PopupMsg();
