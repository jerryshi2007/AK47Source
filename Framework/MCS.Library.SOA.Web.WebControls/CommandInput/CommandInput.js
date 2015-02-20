
$HBRootNS.CommandInput = function(element)
{
	$HBRootNS.CommandInput.initializeBase(this, [element]);
	
	this._formID = "";
	this._isPostBack = false;
	this._commandInputEventKey = "commandInput";
	
	this._inputEvents = 
	{
		propertychange : Function.createDelegate(this, this._onPropertyChange)
	};
}

$HBRootNS.CommandInput.prototype =
{
	initialize : function()
	{
		$HBRootNS.CommandInput.callBaseMethod(this, "initialize");
		$addHandlers(this.get_element(), this._inputEvents);
	},

	dispose : function()
	{
		$HBRootNS.CommandInput.callBaseMethod(this, "dispose");
	},
	
	get_formID : function()
	{
		return this._formID;
	},
	
	set_formID : function(value)
	{
		this._formID = value;
	},
	
	get_isPostBack : function(value)
	{
		return this._isPostBack;
	},
	
	set_isPostBack : function(value)
	{
		this._isPostBack = value;
	},
	
	add_commandInput : function(value)
	{
		this.get_events().addHandler(this._commandInputEventKey, value);
	},
	
	remove_commandInput : function(value)
	{
		this.get_events().removeHandler(this._commandInputEventKey, value);
	},
	
	_raiseCommandInputEvent : function(commandValue)
	{
		var handler = this.get_events().getHandler(this._commandInputEventKey);
		if (handler)
		{
			var e = new Sys.EventArgs;
			e.commandValue = commandValue;
			e.stopCommand = false;			
			handler(this, e);
			
			return e.stopCommand;
		}
		return false;
	},
	
	_onPropertyChange : function(e)
	{
		if (e.rawEvent.propertyName == "value")
		{
			var value = this.get_element().value;
			
			var stopCommand = this._raiseCommandInputEvent(value);
			
			if (stopCommand)
				return;
				
			switch (value)
			{
				case "refresh":
					if (this._isPostBack)
						$get(this._formID).submit();
					else	                
						window.location.reload();
					break;
					
				case "close":
					window.top.close();
					break;
			}
		}
	}
}

$HBRootNS.CommandInput.registerClass($HBRootNSName + ".CommandInput", $HGRootNS.ControlBase);
