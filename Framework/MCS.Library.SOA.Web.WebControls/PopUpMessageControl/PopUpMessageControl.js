// -------------------------------------------------
// FileName	：	PopUpMessageControl.js
// Remark	：	消息提醒
// -------------------------------------------------
// VERSION		AUTHOR		DATE			CONTENT
// 1.0			张梁		20071127		创建
// -------------------------------------------------

$HBRootNS.PopUpMessageControl = function(element)
{
	$HBRootNS.PopUpMessageControl.initializeBase(this, [element]);

	this._width = 307;
	this._height = 75;
	this._tableMain = null;
	this._divBackground = null;
	this._popUp = null;
	this._enabled = true;				//是否有效
	var showTime = new Object();
	showTime.TotalMilliseconds = 4000;
	this._showTime = showTime;			//显示时间
	this._timer = null;					//定时器
	this._showText = null;				//内容
	this._playSoundPath = null;			//播放的声音文件路径
	this._cssPath = null;				//样式表路径
	this._soundPlayer = null;			//播放器
	this._titleCell = null;				//标题栏
	this._titleDiv = null;
	this._contentCell = null;			//内容栏
	this._titleImagePath = null;
	this._messageIconPath = null;
	this._bgColor = "#E6E5E0";
	this._hideTimer = null;             //逐渐消失使用的定时器.
	this._showTimer = null;             //逐渐显示使用的定时器.
	this._positionElementID = null;
	this._positionX = null;
	this._positionY = null;
	this._ieVersion = 6;
	this._alpha = 0;					//透明度
	this._increment = 5;				//增量
	
	this._mouseEvent =
	    {
	        mouseover : Function.createDelegate(this, this._mouseOver),
	        mouseout : Function.createDelegate(this, this._mouseOut)
	    };
	this._clickContentEvent = 
		{
			click : Function.createDelegate(this, this._clickContent),
			mouseover : Function.createDelegate(this, this._mouseOver),
	        mouseout : Function.createDelegate(this, this._mouseOut)
		};
		
    this._clickIconEvent = 
		{
			click : Function.createDelegate(this, this._clickIcon),
			mouseover : Function.createDelegate(this, this._mouseOver),
	        mouseout : Function.createDelegate(this, this._mouseOut)
		};
}

$HBRootNS.PopUpMessageControl.prototype =
{
	initialize : function()
	{
		$HBRootNS.PopUpMessageControl.callBaseMethod(this, "initialize");
	    
	    this._ieVersion = parseFloat(navigator.appVersion.split("MSIE")[1]);
	    
		if(this._enabled)
		{
			this._timer = $create($HGRootNS.Timer, {interval : this._showTime.TotalMilliseconds, enabled : true}, null, null, null);
			this._timer.add_tick(Function.createDelegate(this, this._hide));
			this._timer.set_enabled(false);
			   
//			if (this._ieVersion == "6")
//			{
	            this._hideTimer = $create($HGRootNS.Timer, {interval : 100,enabled: true}, null, null, null);
		        this._hideTimer.add_tick(Function.createDelegate(this, this._hidePopUp));
			    this._hideTimer.set_enabled(false);
        			
			    this._showTimer = $create($HGRootNS.Timer, {interval : 100,enabled: true}, null, null, null);
			    this._showTimer.add_tick(Function.createDelegate(this, this._showPopUp));
			    this._showTimer.set_enabled(false);
//			}
			
			this._buildControl();
		}
	},

	dispose : function()
	{
		$HBRootNS.PopUpMessageControl.callBaseMethod(this, "dispose");
		
		this._tableMain = null;
		this._popUp = null;
		this._timer = null;
		this._divBackground = null;
		this._soundPlayer = null;
		this._hideTimer = null;
		this._showTimer = null;
	},

    _mouseOver : function()
    {
		this._hideTimer.set_enabled(false);
		this._showTimer.set_enabled(false);
		this._divBackground.style.filter = "progid:DXImageTransform.Microsoft.Alpha(opacity=100)";
		this._tableMain.style.filter = "progid:DXImageTransform.Microsoft.Alpha(opacity=100)";
		this._alpha = 100;
        this._timer.set_enabled(false);
    },
    
    _mouseOut : function()
    {
        this._timer.set_enabled(true);
    },

	show : function()
	{
		//初始化所有的东东
		this._timer.set_enabled(false);
		this._hideTimer.set_enabled(false);
		this._showTimer.set_enabled(false);
    	this._alpha = 0
	    this._divBackground.className = "oa_ajax_control_popUpMessage_background";
    	this._tableMain.className = "oa_ajax_control_popUpMessage_table";
        
//        if (this._ieVersion == "6")
//	    {	
//    	    this._divBackground.style.visibility = "hidden";
//		    this._divBackground.filters[0].Apply();
//		    this._divBackground.style.visibility = "visible";
//		    this._divBackground.filters[0].play();
//    		
//            this._tableMain.style.visibility = "hidden";
//		    this._tableMain.filters[0].Apply();
//		    this._tableMain.style.visibility = "visible";
//		    this._tableMain.filters[0].play();
		    
		this._showTimer.set_enabled(true);
//        }

        this._popUp.show();
        
		try
		{
		    if (this._playSoundPath != null && this._playSoundPath != "")
			    this._soundPlayer.play();
		}
		catch(e)
		{}

		
	},

	_hide : function()
	{
//	    if (this._ieVersion == "6")
//	    {
//	        this._divBackground.style.visibility = "visible";
//		    this._divBackground.filters[0].Apply();
//		    this._divBackground.style.visibility = "hidden";
//		    this._divBackground.filters[0].play();
//    		
//            this._tableMain.style.visibility = "visible";
//		    this._tableMain.filters[0].Apply();
//		    this._tableMain.style.visibility = "hidden";
//		    this._tableMain.filters[0].play();
    				
		this._hideTimer.set_enabled(true);
		    
//		}
//		else
//		{
//		    this._popUp.hide();
//		}
		
		this._timer.set_enabled(false);
	},
    
    _showPopUp : function()
    { 
//        if (this._ieVersion == "6")
//        {
//            this._divBackground.filters[0].stop();
//            this._tableMain.filters[0].stop();
//        }
		this._alpha = this._alpha + this._increment;
		if(this._alpha >= 100)
			this._alpha = 100;
		
		this._divBackground.style.filter = "progid:DXImageTransform.Microsoft.Alpha(opacity=" + this._alpha + ")";
		this._tableMain.style.filter = "progid:DXImageTransform.Microsoft.Alpha(opacity=" + this._alpha + ")";
		
		if(this._alpha >= 100)
		{
			this._timer.set_enabled(true);
			this._showTimer.set_enabled(false);
        }
    },
    
    _hidePopUp : function()
    {
    	this._alpha = this._alpha - this._increment;
		
		if(this._alpha <= 0)
			this._alpha = 0;
		this._divBackground.style.filter = "progid:DXImageTransform.Microsoft.Alpha(opacity=" + this._alpha + ")";
		this._tableMain.style.filter = "progid:DXImageTransform.Microsoft.Alpha(opacity=" + this._alpha + ")";
		
		if(this._alpha <= 0)
		{
			this._popUp.hide();
			this._hideTimer.set_enabled(false);
        }
         //this._popUp.hide();
         
//         if (this._ieVersion == "6")
//         {
//             this._divBackground.filters[0].stop();
//             this._tableMain.filters[0].stop();
//         }
         
    	 //this._hideTimer.set_enabled(false);
    },
    
    _clickContent : function()
    {
        this.raiseClick();
    },
    
    _clickIcon : function()
    {
        this.raiseClick();
    },
    
	_buildControl : function()
	{
	    var element = null;
	    
	    if (this._positionElementID != null && this._positionElementID != "")
	        element = $get(this._positionElementID);

		this._popUp = $create($HGRootNS.PopupControl,
			{
				width : this._width,
				height : this._height,
				positionElement : element,
				applyFilter : false,
				x : this._positionX,
				y : this._positionY,
				usePublicPopupWindow : false
			}, null, null, null);

		this._popUp.get_popupDocument().createStyleSheet(this._cssPath, 1);
		this._createPlaySound();
		this._showMainTable();
		this._showContent();
		this._showBackground();
	},

	_showBackground : function()
	{
		this._divBackground = this._popUp.createElementFromTemplate(
			{
				nodeName : "div"				
			},
			this._popUp.get_popupBody());
	},

	_showMainTable : function()
	{
		this._tableMain = this._popUp.createElementFromTemplate(
			{
				nodeName : "table",
				properties:
				{
					cellPadding : "0",
					cellSpacing : "0"
				}
			},
			this._popUp.get_popupBody());

		this._popUp.createElementFromTemplate({nodeName : "tbody"}, this._tableMain);
	},

	_createPlaySound : function()
	{
		this._soundPlayer = $HGDomElement.createElementFromTemplate(
			{
				nodeName : "embed",
				properties:
				{
					src : this._playSoundPath,
					type : "audio/wav",
					autostart : "false",
					hidden : "true"
				}				
			},
			document.body);
	},

	_showContent : function()
	{
		var row = this._popUp.createElementFromTemplate({nodeName : "tr"},this._tableMain.firstChild);

		this._titleCell = this._popUp.createElementFromTemplate(
			{
				nodeName : "td",				
				properties:
				{
				    colSpan : "2",
					innerHTML : String.format("<img src='{0}' /><div/>", this._titleImagePath),
					vAlign : "top"
				},
				cssClasses : ["oa_ajax_control_popUpMessage_headCell"],
				events : this._mouseEvent	
			},
			row);
		
		this._titleDiv = this._popUp.createElementFromTemplate(
			{
				nodeName : "div",				
				properties:
				{
					innerText: ""
				}
			},
			this._titleCell);

		this._titleDiv.style.marginLeft = "4px";

		row = this._popUp.createElementFromTemplate(
			{
				nodeName : "tr"
			},
			this._tableMain.firstChild);

         this._popUp.createElementFromTemplate(
			{
				nodeName : "td",
				properties:
				{
					innerHTML : String.format("<img src='{0}' style='cursor:pointer'/>", this._messageIconPath)				    
				},
				cssClasses : ["oa_ajax_control_popUpMessage_iconCell"],
				events : this._clickIconEvent
			},
			row);

		this._contentCell = this._popUp.createElementFromTemplate(
			{
				nodeName : "td",
				properties:
				{
					innerText : (this._showText == null ? "" : this._showText),
					width : (this._width - 50) + "px"
				},
				cssClasses : ["oa_ajax_control_popUpMessage_bodyCell"],
				events : this._clickContentEvent
			},
			row);
	},

	get_showTime : function()
	{
		return this._showTime;
	},
	set_showTime : function(value)
	{
		if (this._showTime != value)
		{
			this._showTime = value;
			this.raisePropertyChanged("showTime");
		}
	},

	get_showText : function()
	{
		return this._showText;
	},
	set_showText : function(value)
	{
		if (this._showText != value)
		{
			this._showText = value;
			this.raisePropertyChanged("showText");
			
			if(this._contentCell)
				this._contentCell.innerHTML = value;
		}
	},

	get_showTitle : function()
	{
		return this._showTitle;
	},
	set_showTitle : function(value)
	{
		if (this._showTitle != value)
		{
			this._showTitle = value;
			this.raisePropertyChanged("showTitle");
			
			if (this._titleDiv)
			     this._titleDiv.innerText = this._showTitle;
		}
	},
 
	get_playSoundPath : function()
	{
		return this._playSoundPath;
	},
	set_playSoundPath : function(value)
	{
		if (this._playSoundPath != value)
		{
			this._playSoundPath = value;
			this.raisePropertyChanged("playSoundPath");
		}
	},

	get_cssPath : function()
	{
		return this._cssPath;
	},
	set_cssPath : function(value)
	{
		if (this._cssPath != value)
		{
			this._cssPath = value;
			this.raisePropertyChanged("cssPath");
		}
	},

	get_enabled : function()
	{
		return this._enabled;
	},
	set_enabled : function(value)
	{
		if (this._enabled != value)
		{
			this._enabled = value;
			this.raisePropertyChanged("enabled");
		}
	},
	
	get_titleImagePath : function()
	{
		return this._titleImagePath;
	},
	set_titleImagePath : function(value)
	{
		if (this._titleImagePath != value)
		{
			this._titleImagePath = value;
			this.raisePropertyChanged("titleImagePath");
		}
	},
	
	get_messageIconPath : function()
	{
		return this._messageIconPath;
	},
	set_messageIconPath : function(value)
	{
		if (this._messageIconPath != value)
		{
			this._messageIconPath = value;
			this.raisePropertyChanged("messageIconPath");
		}
	},
	
	get_positionElementID : function()
	{
		return this._positionElementID;
	},
	set_positionElementID : function(value)
	{
		if (this._positionElementID != value)
		{
			this._positionElementID = value;
			this.raisePropertyChanged("positionElementID");
			
			if (this._popUp != null)
			{
			     var element = null;
    	    
	             if (value != null && value != "")
	                element = $get(value);

		         this._popUp.set_positionElement(element);
		     }
		}
	},
	
	set_positionElement : function(value)
	{
        if (this._popUp != null)
            this._popUp.set_positionElement(value);
	},
	
	get_positionX : function()
	{
		return this._positionX;
	},
	set_positionX : function(value)
	{
		if (this._positionX != value)
		{
			this._positionX = value;
			this.raisePropertyChanged("positionX");
			
		    if (this._popUp != null)
		        this._popUp.set_x(value);
		}
	},
	
	get_positionY : function()
	{
		return this._positionY;
	},
	set_positionY : function(value)
	{
		if (this._positionY != value)
		{
			this._positionY = value;
			this.raisePropertyChanged("positionY");
			
			if (this._popUp != null)
		        this._popUp.set_y(value);
		}
	},
	
	get_popUp : function()
	{
		return this._popUp;
	},

	get_contentCell : function()
	{
		return this._contentCell;
	},
	
	get_width : function()
	{
	    return this._width;
	},
	
	get_height : function()
	{
	    return this._height;
	},
	
	add_onClick : function(handler)
	{
		this.get_events().addHandler("onClick", handler);
	},
	remove_onClick : function(handler)
	{
		this.get_events().removeHandler("onClick", handler);
	},
	raiseClick : function()
	{
		var handlers = this.get_events().getHandler("onClick");
		
        var continueExec = true;

        if (handlers)
        {
            var e = new Sys.EventArgs();
			
            handlers(this, e);

            if (e.cancel)
                continueExec = false;
        }

        return continueExec; 
	}
}

$HBRootNS.PopUpMessageControl.registerClass($HBRootNSName + ".PopUpMessageControl", $HGRootNS.ControlBase);