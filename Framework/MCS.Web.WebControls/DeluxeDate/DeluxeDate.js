
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	DeluxeDate.js
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    吴伟	    20070430		创建
// -------------------------------------------------


$HGRootNS.DeluxeDate = function(element) {
 /// <summary>
    /// A behavior that attaches a calendar date selector to a textbox
    /// </summmary>
    /// <param name="element" type="Sys.UI.DomElement">The element to attach to</param>
    
    ///<summary>
    /// mask part
    /// </summmary>
    this._IsValidValue = true;

    this._MaskedEditButtonID = "";
    //

    //set format锛?9:99:999  __:__:___
    this._Mask = "9999-99-99";

    this._Filtered = "";
    //input format prompt symbol
    this._PromptChar = "_";

    // Message
//    this._MessageValidatorTip = true;
    // AutoComplete
    this._AutoComplete = true;
    this._AutoCompleteValue =  "";
    // behavior
    this._ClearTextOnInvalid = false;
    this._ClearMaskOnLostfocus = true;

    // CSS
    this._OnFocusCssClass = "MaskedEditFocus";
    this._OnInvalidCssClass = "MaskedEditError";

    // globalization 
   // this._CultureName = "zh-CN";
    // globalization Hidden 

    this._CultureTimePlaceholder = "-";

    // **************************************************
    // local var mask valid
    // **************************************************
    //  9 = only numeric

    this._MaskConv = "";
    // **************************************************
    // Others local Var
    // **************************************************
    // save the Direction selected Text (only for ie)
    this._DirectSelText = "";
    // save the initial value for verify changed
    this._initialvalue = "";

    this._LogicSymbol = "";
    // save logic mask with text input
    this._LogicTextMask = "";
    // save logic mask without text
    this._LogicMask = "";
    // save logic mask without text and without escape
    this._LogicMaskConv = "";
    // ID prompt char pane
    this._LogicPrompt = String.fromCharCode(1);
    // ID escape char space
    this._LogicEscape = String.fromCharCode(2);
    // first valid position
    this._LogicFirstPos = -1;
    // Last valid position 
    this._LogicLastPos = -1;
    // Qtd Valid input Position
    this._QtdValidInput = 0;
    // Flag to validate in lost focus not duplicate clearMask execute
    this._InLostfocus = false;
;
    // Save local Current MessageError
    //set erro massage
    this._CurrentMessageError = "";

    this._charNumbers = "0123456789";
    
    /// <param name="element" type="Sys.UI.DomElement">The element to maskededit</param>
    
    $HGRootNS.DeluxeDate.initializeBase(this, [element]);
    this._isOnlyCurrentMonth = true;
    this._selectMonthList = null;
    this._isComplexHeader = true;
    this._format = "d";
    this._cssClass = "ajax__calendar";
    this._enabled = true;
    this._animated = true;

    this._layoutRequested = 0;
    this._layoutSuspended = false;

    this._selectedDate = null;
    this._visibleDate = null;
    this._todaysDate = null;
    this._firstDayOfWeek = $HGRootNS.FirstDayOfWeek.Default;

    this._padding = null;

    this._popupDiv = null;
    this._prevArrow = null;
    this._prevArrowImage = null;
    this._nextArrow = null;
    this._nextArrowImage = null;
    this._title = null;
    //add a select,input,img
    this._titleselect = null;
    this._titleinput = null;
    this._titleimg = null;
    //
    this._today = null;
    this._daysRow = null;
    this._monthsRow = null;
    this._yearsRow = null;
    this._daysBody = null;
    this._monthsBody = null;
    this._yearsBody = null;
    this._button = null;
    
    this._calendarPopup = null;
    this._modeChangeAnimation = null;
    this._modeChangeMoveTopOrLeftAnimation = null;
    this._modeChangeMoveBottomOrRightAnimation = null;
    this._mode = "days";
    this._selectedDateChanging = false;
    this._isOpen = false;
    this._isAnimating = false;
    this._width = 170;
    this._height = 139;
    this._modes = {"days" : null, "months" : null, "years" : null};
    this._modeOrder = {"days" : 0, "months" : 1, "years" : 2 };
    
    // Safari needs a longer delay in order to work properly
    this._blur = new $HGRootNS.DeferredOperation(((Sys.Browser.agent === Sys.Browser.Safari) ? 1000 : 1), this, this._onBlur);
    this._focus = new $HGRootNS.DeferredOperation(((Sys.Browser.agent === Sys.Browser.Safari) ? 1000 : 1), this, this._onFocus);
    

    this._element$delegates = {  //    
          keypress : Function.createDelegate(this,this._onkeypress),          //textbox add maskededit  
//        focus : Function.createDelegate(this, this._element_onfocus),
//        focusout : Function.createDelegate(this, this._element_onblur),    //text blur
//        blur : Function.createDelegate(this, this._element_onblur),
//        change : Function.createDelegate(this, this._element_onchange)    //need to be change
          focus : Function.createDelegate(this,this._onFocus),
          keydown : Function.createDelegate(this,this._onKeyPressdown),
          blur : Function.createDelegate(this, this._onBlur)
    }
    
    //add list event
        this._select$delegates = { 
        change : Function.createDelegate(this, this._select_onselect)
    }
    this._img$delegates = { 
        click : Function.createDelegate(this, this._img_onclick)
    }
    this._cell$delegates = {
        mouseover : Function.createDelegate(this, this._cell_onmouseover),
        mouseout : Function.createDelegate(this, this._cell_onmouseout),
        click : Function.createDelegate(this, this._cell_onclick)
    }
}

$HGRootNS.DeluxeDate.prototype = {    


        get_isOnlyCurrentMonth : function()
        {
        return this._isOnlyCurrentMonth;
        },
        set_isOnlyCurrentMonth :function(value)
        {
        this._isOnlyCurrentMonth = value;
        },
        
    //add custom header
        get_isComplexHeader : function() {
        /// <summary>
        /// Whether changing modes is animated
        /// </summary>
        /// <value type="Boolean" />
           
        return this._isComplexHeader;
    },
    set_isComplexHeader : function(value) {
        if (this._isComplexHeader != value) {
            this._isComplexHeader = value;
            this.raisePropertyChanged("isComplexHeader");
        }
    },
    
    get_enabled : function() {
        /// <value type="Boolean">
        /// Whether this behavior is available for the current element
        /// </value>
           
        return this._enabled;
    },
    set_enabled : function(value) {
        if (this._enabled != value) {
            this._enabled = value;
            this.raisePropertyChanged("enabled");
        }
    },
    
    get_animated : function() {
    /// <summary>
    /// Whether changing modes is animated
    /// </summary>
    /// <value type="Boolean" />
           
    return this._animated;
    },
    set_animated : function(value) {
        if (this._animated != value) {
            this._animated = value;
            this.raisePropertyChanged("animated");
        }
    },
    get_format : function() { 
        /// <value type="String">
        /// The format to use for the date value
        /// </value>

        return this._format; 
    },
    set_format : function(value) { 
        if (this._format != value) {
            this._format = value; 
            this.raisePropertyChanged("format");
        }
    },
    
    get_selectedDate : function() {
        /// <value type="Date">
        /// The date value represented by the text box
        /// </value>

        if (this._selectedDate == null) {
            var elt = this.get_element();
            if (elt.value) {
                this._selectedDate = this._parseTextValue(elt.value);
            }
        }
        return this._selectedDate;
    },
    set_selectedDate : function(value) {
        var elt = this.get_element();
        if (this._selectedDate != value) {
            this._selectedDate = value;
            
            this._selectedDateChanging = true;            
            var text = "";
            if (value) {
                text = this._simpleFormat(value);
            } 
            if (text != elt.value) {
                elt.value = text;
                this._fireChanged();
            }
            this._selectedDateChanging = false;
            this.invalidate();
            this.raisePropertyChanged("selectedDate");
        }
    },

    get_visibleDate : function() {
        /// <summary>
        /// The date currently visible in the calendar
        /// </summary>
        /// <value type="Date" />

        return this._visibleDate;
    },
    set_visibleDate : function(value) {
        if (value) value = value.getDateOnly();
        if (this._visibleDate != value) {
            this._switchMonth(value, !this._isOpen);
            this.raisePropertyChanged("visibleDate");
        }
    },

    get_todaysDate : function() {
        /// <value type="Date">
        /// The date to use for "Today"
        /// </value>
        
        if (this._todaysDate != null) {
            return this._todaysDate;
        }
        return new Date().getDateOnly();
    },
    set_todaysDate : function(value) {
        if (value) value = value.getDateOnly();
        if (this._todaysDate != value) {
            this._todaysDate = value;
            this.invalidate();
            this.raisePropertyChanged("todaysDate");
        }
    },
    
    get_firstDayOfWeek : function() {
        /// <value type=$HGRootNSName + ".FirstDayOfWeek">
        /// The day of the week to appear as the first day in the calendar
        /// </value>
        
        return this._firstDayOfWeek;
    },
    set_firstDayOfWeek : function(value) {
        if (this._firstDayOfWeek != value) {
            this._firstDayOfWeek = value;
            this.invalidate();
            this.raisePropertyChanged("firstDayOfWeek");
        }
    },
        
    get_cssClass : function() {
        /// <value type="Sys.UI.DomElement">
        /// The CSS class selector to use to change the calendar's appearance
        /// </value>

        return this._cssClass;
    },
    set_cssClass : function(value) {
        if (this._cssClass != value) {
            if (this._cssClass && this.get_isInitialized()) {
                Sys.UI.DomElement.removeCssClass(this._container, this._cssClass);
            }
            this._cssClass = value;
            if (this._cssClass && this.get_isInitialized()) {
                Sys.UI.DomElement.addCssClass(this._container, this._cssClass);
            }
            this.raisePropertyChanged("cssClass");
        }
    },
    /*@@@@@@@@@@@@@@@@@@@@@@@@
    property
    @@@@@@@@@@@@@@@@@@@@@@@@@@*/
    
    //
    // Helper properties
    //
    get_Mask : function() {
        if (this._MaskConv == "" && this._Mask != "")
        {
            this._convertMask();
        } 
        return this._MaskConv;
    }
    , set_Mask : function(value) 
    {
        this._Mask = value;
        this.raisePropertyChanged('Mask');
    }  
    
    , get_PromptCharacter : function() 
    {
        return this._PromptChar;
    }      
    , set_PromptCharacter : function(value) 
    {
        this._PromptChar = value;
        this.raisePropertyChanged('PromptChar');
    }
    , get_OnFocusCssClass : function() 
    {
        return this._OnFocusCssClass;
    }      
    , set_OnFocusCssClass : function(value) 
    {
        this._OnFocusCssClass = value;
        this.raisePropertyChanged('OnFocusCssClass');
    }
    , get_OnInvalidCssClass : function() 
    {
        return this._OnInvalidCssClass;
    }      
    , set_OnInvalidCssClass : function(value) 
    {
        this._OnInvalidCssClass = value;
        this.raisePropertyChanged('OnInvalidCssClass');
    }
     
    , get_CultureTimePlaceholder : function() 
    {
        return this._CultureTimePlaceholder;
    }      
    , set_CultureTimePlaceholder : function(value) 
    {
        this._CultureTimePlaceholder = value;
        this.raisePropertyChanged('CultureTimePlaceholder');
    }      
      
    , get_ClearMaskOnLostFocus : function() 
    {
        return this._ClearMaskOnLostfocus;
    }      
    , set_ClearMaskOnLostFocus : function(value) 
    {
        this._ClearMaskOnLostfocus = value;
        this.raisePropertyChanged('ClearMaskOnLostfocus');
    }      
    , get_AutoComplete : function() 
    {
        return this._AutoComplete;
    }      
    , set_AutoComplete : function(value) 
    {
        this._AutoComplete = value;
        this.raisePropertyChanged('AutoComplete');
    }   
    , get_AutoCompleteValue : function() 
    {
        return this._AutoCompleteValue;
    }      
    , set_AutoCompleteValue : function(value) 
    {
        this._AutoCompleteValue = value;
        this.raisePropertyChanged('AutoCompleteValue');
    }   

    //component

    ,get_MaskedEditButtonID : function()
    {
        return this._MaskedEditButtonID;
    }
    ,set_MaskedEditButtonID : function(value)
    {
        this._MaskedEditButtonID = value;
    }
  
    ,get_CurrentMessageError : function()
    {
        return this._CurrentMessageError;
    }
    ,set_CurrentMessageError : function(value)
    {
        this._CurrentMessageError = value;
    }
    ,get_IsValidValue : function()
    {
        return this._IsValidValue;
    }
    ,set_IsValidValue :function(value)
    {
        this._IsValidValue = value;
    },
    
    /**************maskededit property end***************/
    /****************************************************/
    
    add_showing : function(handler) {
        /// <summary>
        /// Adds an event handler for the <code>showiwng</code> event.
        /// </summary>
        /// <param name="handler" type="Function">
        /// The handler to add to the event.
        /// </param>
        /// <returns />

        this.get_events().addHandler("showing", handler);
    },
    remove_showing : function(handler) {
        /// <summary>
        /// Removes an event handler for the <code>showing</code> event.
        /// </summary>
        /// <param name="handler" type="Function">
        /// The handler to remove from the event.
        /// </param>
        /// <returns />

        this.get_events().removeHandler("showing", handler);
    },
    raiseShowing : function() {
        /// <summary>
        /// Raise the <code>showing</code> event
        /// </summary>
        /// <returns />

        var handlers = this.get_events().getHandler("showing");
        if (handlers) {
            handlers(this, Sys.EventArgs.Empty);
        }
    },
    
    add_shown : function(handler) {
        /// <summary>
        /// Adds an event handler for the <code>shown</code> event.
        /// </summary>
        /// <param name="handler" type="Function">
        /// The handler to add to the event.
        /// </param>
        /// <returns />

        this.get_events().addHandler("shown", handler);
    },
    remove_shown : function(handler) {
        /// <summary>
        /// Removes an event handler for the <code>shown</code> event.
        /// </summary>
        /// <param name="handler" type="Function">
        /// The handler to remove from the event.
        /// </param>
        /// <returns />

        this.get_events().removeHandler("shown", handler);
    },
    raiseShown : function() {
        /// <summary>
        /// Raise the <code>shown</code> event
        /// </summary>
        /// <returns />

        var handlers = this.get_events().getHandler("shown");
        if (handlers) {
            handlers(this, Sys.EventArgs.Empty);
        }
    },
    
    add_hiding : function(handler) {
        /// <summary>
        /// Adds an event handler for the <code>hiding</code> event.
        /// </summary>
        /// <param name="handler" type="Function">
        /// The handler to add to the event.
        /// </param>
        /// <returns />

        this.get_events().addHandler("hiding", handler);
    },
    remove_hiding : function(handler) {
        /// <summary>
        /// Removes an event handler for the <code>hiding</code> event.
        /// </summary>
        /// <param name="handler" type="Function">
        /// The handler to remove from the event.
        /// </param>
        /// <returns />

        this.get_events().removeHandler("hiding", handler);
    },
    raiseHiding : function() {
        /// <summary>
        /// Raise the <code>hiding</code> event
        /// </summary>
        /// <returns />

        var handlers = this.get_events().getHandler("hiding");
        if (handlers) {
            handlers(this, Sys.EventArgs.Empty);
        }
    },
    
    add_hidden : function(handler) {
        /// <summary>
        /// Adds an event handler for the <code>hidden</code> event.
        /// </summary>
        /// <param name="handler" type="Function">
        /// The handler to add to the event.
        /// </param>
        /// <returns />

        this.get_events().addHandler("hidden", handler);
    },
    remove_hidden : function(handler) {
        /// <summary>
        /// Removes an event handler for the <code>hidden</code> event.
        /// </summary>
        /// <param name="handler" type="Function">
        /// The handler to remove from the event.
        /// </param>
        /// <returns />

        this.get_events().removeHandler("hidden", handler);
    },
    raiseHidden : function() {
        /// <summary>
        /// Raise the <code>hidden</code> event
        /// </summary>
        /// <returns />

        var handlers = this.get_events().getHandler("hidden");
        if (handlers) {
            handlers(this, Sys.EventArgs.Empty);
        }
    },
    
    add_dateSelectionChanged : function(handler) {
        /// <summary>
        /// Adds an event handler for the <code>dateSelectionChanged</code> event.
        /// </summary>
        /// <param name="handler" type="Function">
        /// The handler to add to the event.
        /// </param>
        /// <returns />

        this.get_events().addHandler("dateSelectionChanged", handler);
    },
    remove_dateSelectionChanged : function(handler) {
        /// <summary>
        /// Removes an event handler for the <code>dateSelectionChanged</code> event.
        /// </summary>
        /// <param name="handler" type="Function">
        /// The handler to remove from the event.
        /// </param>
        /// <returns />

        this.get_events().removeHandler("dateSelectionChanged", handler);
    },
    raiseDateSelectionChanged : function() {
        /// <summary>
        /// Raise the <code>dateSelectionChanged</code> event
        /// </summary>
        /// <returns />

        var handlers = this.get_events().getHandler("dateSelectionChanged");
        if (handlers) {
            handlers(this, Sys.EventArgs.Empty);
        }
    },

    initialize : function() {
        /// <summary>
        /// Initializes the components and parameters for this behavior
        /// </summary>
        
        $HGRootNS.DeluxeDate.callBaseMethod(this, "initialize");
        
         if(this._MaskedEditButtonID==""||this._MaskedEditButtonID==null||typeof(this._MaskedEditButtonID)=="undefined"||$get(this._MaskedEditButtonID)==null)
        {
        alert('uninitialize');
        return;
        }
         var e = this.get_element();
                this._InLostfocus = true;

        // if this textbox is focused initially
        var hasInitialFocus = false;
        
        //only for ie , for firefox see keydown
        if (document.activeElement)
        {
            if (e.id == document.activeElement.id)
            {
                hasInitialFocus = true;
            }
        }
        
        if (hasInitialFocus) 
        {
            this._onfocu();
            //set textbox's focus
        }
        else if (e.value != "")
        {
            this._InitValue();
            if (this._ClearMaskOnLostfocus)
            {
                e.value = (this._getClearMask(e.value));
            }
        }
        
        var elt = this.get_element();
        $addHandlers(elt, this._element$delegates);

         $addHandler($get(this._MaskedEditButtonID), "click", Function.createDelegate(this, this._button_onclick));
        this._modeChangeMoveTopOrLeftAnimation = new $HGRootNS.Animation.LengthAnimation(null, null, null, "style", null, 0, 0, "px");
        this._modeChangeMoveBottomOrRightAnimation = new $HGRootNS.Animation.LengthAnimation(null, null, null, "style", null, 0, 0, "px");
        this._modeChangeAnimation = new $HGRootNS.Animation.ParallelAnimation(null, .25, null, [ this._modeChangeMoveTopOrLeftAnimation, this._modeChangeMoveBottomOrRightAnimation ]);

        var value = this.get_selectedDate();
        if (value) {
            this.set_selectedDate(value);
        } 
    },
    
        /**************************************************************/
        /****************maskededit function begin********************/
        /************************************************************/
        
     _InitValue : function()
    {
        var masktxt = this._createMask();
        var Inipos = this._LogicFirstPos;
        var initValue = "";
        var e = this.get_element();
        this._LogicSymbol = "";

        if (e.value != "" && e.value != masktxt)
        {
            initValue = e.value;
        }
        e.value = (masktxt);
        if (initValue != "")
        {
                this.loadValue(initValue,this._LogicFirstPos);
        }
        return Inipos;
    }

    //
    // Set/Remove CssClass
    //
    , AddCssClassMaskedEdit : function(CssClass)
    {
        var e = this.get_element();

        Sys.UI.DomElement.removeCssClass(e,this._OnFocusCssClass);

        Sys.UI.DomElement.removeCssClass(e,this._OnInvalidCssClass);
        if (CssClass != "")
        {
            Sys.UI.DomElement.addCssClass(e,CssClass);
        }
    }
    //
    // Load initial value in mask
    //
    , loadValue : function(initValue,logicPosition)
    {
            var oldfocus = this._InLostfocus;   
            var i = 0;
            this._InLostfocus = false;
            if (this._ClearMaskOnLostfocus == false)
            {
                logicPosition  = 0;
            }
            for (i = 0 ; i < parseInt(initValue.length,10) ; i++) 
            {
                var c = initValue.substring(i,i+1);     
               
               
                if (this._processKey(logicPosition,c)) 
                {
                    this._insertContent(c,logicPosition);
                    if (this._ClearMaskOnLostfocus == false)
                    {
                        logicPosition  = logicPosition+1;
                    }
                    else
                    {
                        logicPosition  = this._getNextPosition(logicPosition+1);
                    }
                }
                else
                {
                    if (this._ClearMaskOnLostfocus == false)
                    {
                        logicPosition  = logicPosition+1;
                    }
                }
            }
            this._InLostfocus = oldfocus;

    }
    
    , _getNextPosition : function(pos)
    {
        while (!this._isValidMaskedEditPosition(pos) && pos < this._LogicLastPos+1)
        {
            pos++;
        }
        if (pos > this._LogicLastPos+1)
        {
            pos = this._LogicLastPos+1;
        }
        return pos;
    }
    
    , _isValidMaskedEditPosition : function(pos) 
    {
        return (this._LogicMask.substring(pos,pos+1) == this._LogicPrompt);
    }
    , _insertContent : function(value,curpos) 
    {
    //if input in placeholder,then pass
    if(this._Mask.indexOf(this._CultureTimePlaceholder)==curpos||this._Mask.lastIndexOf(this._CultureTimePlaceholder)==curpos)
    return;
        var masktext = this.get_element().value;
        masktext = masktext.substring(0,curpos) + value + masktext.substring(curpos+1);
        this._LogicTextMask = this._LogicTextMask.substring(0,curpos) + value + this._LogicTextMask.substring(curpos+1);
        this.get_element().value = (masktext);
        
    }
    
    , setSelectionRange : function(selectionStart, selectionEnd) 
    {
      input = this.get_element();
      if (input.createTextRange) 
      {
        var range = input.createTextRange();
        range.collapse(true);
        range.moveEnd('character', selectionEnd);
        range.moveStart('character', selectionStart);
        range.select();
      }
      else if (input.setSelectionRange) 
      {
        input.setSelectionRange(selectionStart, selectionEnd);
      }
  
    }
    , _SetCancelEvent : function(evt)
    {
        if (Sys.Browser.agent == Sys.Browser.InternetExplorer) 
        {
            evt.returnValue = false;
        }
        else
        {
            if (typeof(evt.returnValue) != "undefined")
            {
                evt.returnValue = false;
            }
            if (evt.preventDefault)
            {
                evt.preventDefault();
            }
        }
    }
    
     , SpecialNavKey : function(keyCode,navkey)
    {
        if (Sys.Browser.agent == Sys.Browser.InternetExplorer)
        {
            return false;
        }
        return (keyCode >= 33 && keyCode <= 45 && navkey);
    }
    ,_OnNavigator  : function(scanCode,evt,navkey)
    {
        if (!navkey)
        {
            return true;
        }
        var curpos;
        if (this._processDeleteKey(scanCode))
        {
            curpos = this._getCurrentPosition();
            this._SetCancelEvent(evt);
            return false;
        }
        if((evt.ctrlKey || evt.altKey || evt.shiftKey || evt.metaKey))
        {
            if (scanCode == 39 && evt.ctrlKey)
            {
                this._DirectSelText = "R";
                curpos = this._getCurrentPosition();
                if (curpos >= this._LogicLastPos+1)
                {
                    this.setSelectionRange(this._LogicLastPos+1,this._LogicLastPos+1);
                    this._SetCancelEvent(evt);
                    return false;
                }
                return  true;
            }
            else if (scanCode == 37 && evt.ctrlKey)
            {
                this._DirectSelText = "L";
                curpos = this._getCurrentPosition();
                if (curpos <= this._LogicFirstPos)
                {
                    this.setSelectionRange(this._LogicFirstPos,this._LogicFirstPos);
                    this._SetCancelEvent(evt);
                    return false;
                }
                return true;
            }
            else if (scanCode == 35 && evt.shiftKey) //END 
            {
                this._DirectSelText = "R";
                curpos = this._getCurrentPosition();
                this.setSelectionRange(curpos,this._LogicLastPos+1);
                this._SetCancelEvent(evt);
                return false;
            }
            else if (scanCode == 36 && evt.shiftKey) //Home 
            {
                this._DirectSelText = "L";
                curpos = this._getCurrentPosition();
                this.setSelectionRange(this._LogicFirstPos,curpos);
                this._SetCancelEvent(evt);
                return false;
            }
            else if (scanCode == 35 || scanCode == 34) //END or pgdown
            {
                this._DirectSelText = "R";
                this.setSelectionRange(this._LogicLastPos+1,this._LogicLastPos+1);
                this._SetCancelEvent(evt);
                return false;
            }
            else if (scanCode == 36 || scanCode == 33) //Home or pgup
            {
                this._DirectSelText = "L";
                this.setSelectionRange(this._LogicFirstPos,this._LogicFirstPos);
                this._SetCancelEvent(evt);
                return false;
            }
            return true;
        }
        if (scanCode == 35 || scanCode == 34) //END or pgdown
        {
            this._DirectSelText = "R";
            this.setSelectionRange(this._LogicLastPos+1,this._LogicLastPos+1);
            this._SetCancelEvent(evt);
            return false;
        }
        else if (scanCode == 36 || scanCode == 33) //Home or pgup
        {
            this._DirectSelText = "L";
            this.setSelectionRange(this._LogicFirstPos,this._LogicFirstPos);
            this._SetCancelEvent(evt);
            return  false;
        }
        else if (scanCode == 37)
        {
            this._DirectSelText = "L";
            curpos = this._getCurrentPosition();
            if (curpos <= this._LogicFirstPos)
            {
                this.setSelectionRange(this._LogicFirstPos,this._LogicFirstPos);
                this._SetCancelEvent(evt);
                return false;
            }
            return true;
        }
        else if (scanCode == 38 || scanCode == 40)
        {
            this._SetCancelEvent(evt);
            return false;
        }
        else if (scanCode == 39)
        {
            this._DirectSelText = "R";
            curpos = this._getCurrentPosition();
            if (curpos >= this._LogicLastPos+1)
            {
                this.setSelectionRange(this._LogicLastPos+1,this._LogicLastPos+1);
                this._SetCancelEvent(evt);
                return false;
            }
            return true;
        }
        return true;
    }
    
    //check date
     ,_CaptureValidatorsControl : function(str)
    {
      if(!str.match(/^\d{4}\-\d\d?\-\d\d?$/))
      {
        return false;
      }   
      var ar = str.replace(/\-0/g,"-").split("-");   
      ar = new Array(parseInt(ar[0]),parseInt(ar[1])-1,parseInt(ar[2]));   
      var d = new Date(ar[0],ar[1],ar[2]);   
      return d.getFullYear() == ar[0] && d.getMonth() == ar[1] && d.getDate() == ar[2];  
    }
    
     ,_getClearMask : function(masktext)
    {
        var i = 0;
        var clearmask = "";
        var qtdok = 0;
        while (i < parseInt(this._LogicTextMask.length,10)) 
        {
            if (qtdok < this._QtdValidInput)
            {
                if (this._isValidMaskedEditPosition(i) && this._LogicTextMask.substring(i, i+1) != this._LogicPrompt)
                {
                    clearmask += this._LogicTextMask.substring(i,i+1);
                    qtdok++;
                }
                else if (this._LogicTextMask.substring(i, i+1) != this._LogicPrompt && this._LogicTextMask.substring(i, i+1) != this._LogicEscape)
                {

                    if (this._LogicTextMask.substring(i,i+1) == this._CultureTimePlaceholder)
                    {
                        clearmask += (clearmask == "")?"":this._CultureTimePlaceholder;
                    }

                }
            }
            i++;
        }
        if (this._LogicSymbol != "" && clearmask != "")
        {
                clearmask += " " + this._LogicSymbol;
        }
        return clearmask;    
    }
     , _convertMask : function() 
    {
        this._MaskConv = "";
        var qtdmask = "";
        var maskchar = "";
        for (i = 0 ; i < parseInt(this._Mask.length,10) ; i++) 
        {

           //force valid standard number is 9
            if (qtdmask.length == 0)
            {
                this._MaskConv += this._Mask.substring(i, i+1);
                qtdmask = "";
                maskchar = this._Mask.substring(i, i+1);
            }
            else if (this._Mask.substring(i, i+1) == "9")
            {
                qtdmask += "9";//alert(this._Mask)
            }
 

        }

        // set spaces for Symbols AM/PM
        
        // set spaces for Symbols Currency
        
        // set spaces for Symbols negative

        this._convertMaskNotEscape();
    }
    //
    // Convert mask with escape to mask not escape
    // length equal to real position 
    //
    , _convertMaskNotEscape : function()
    {
        this._LogicMaskConv = "";
        var atumask = this._MaskConv;
        var flagescape = false;
        for (i = 0 ; i < parseInt(atumask.length,10); i++) 
        {

            if (!flagescape)
            
            {
                this._LogicMaskConv += atumask.substring(i, i+1);    
            }
            else
            {
                this._LogicMaskConv += this._LogicEscape;
                flagescape = false;
            }
        }
     }
    , _CheckArgsEvents : function(args)
    {
        var ret = null;
        if (typeof(args) != "undefined" && args !=null && typeof(args.rawEvent) != "undefined")
        {
           ret = args.rawEvent;
        }
        return ret;
    }
      //
    // create mask empty , logic mask empty
    // convert escape code and Placeholder to culture
    //when clearing show format __:__:__
    , _createMask : function()
    {
        var text;
        if (this._MaskConv == "" && this._Mask != "")
        {
        //set splite format 99:99:99
            this._convertMask();
        } 
        text = this._MaskConv;
        var i = 0;
        var masktext = "";
        var flagescape = false;
        this._LogicTextMask = "";
        this._QtdValidInput = 0;
        while (i < parseInt(text.length,10)) 
        {
                        this._QtdValidInput ++;
                        if (text.substring(i, i+1) == "-")
                        {
                            masktext += this._CultureTimePlaceholder;
                            this._LogicTextMask += this._CultureTimePlaceholder;
                        }  
                    else{
                            masktext += this._PromptChar;
                            this._LogicTextMask += this._LogicPrompt;
                         }
                         


            i++;
        }
        // Set First and last logic position
        this._LogicFirstPos = -1;
        this._LogicLastPos = -1;
        this._LogicMask = this._LogicTextMask;
        for (i = 0 ; i < parseInt(this._LogicMask.length,10) ; i++) 
        {
            if (this._LogicFirstPos == -1 && this._LogicMask.substring(i,i+1) == this._LogicPrompt)
            {
                this._LogicFirstPos = i;
            }
            if (this._LogicMask.substring(i,i+1) == this._LogicPrompt)
            {
                this._LogicLastPos = i;
            }
        }
        return masktext;    
    }
     //
    // delete current Selected
    // return position select or -1 if nothing select
    //
    , _deleteTextSelection : function()
    {
        var masktext = this.get_element().value;
        var input = this.get_element();
        var ret = -1;
        var lenaux = -1;
        var begin = -1;
        if (document.selection) 
        {
            sel = document.selection.createRange();
           //notdo
            if (sel.text != "")
            {
                var aux = sel.text + String.fromCharCode(3);
                sel.text = aux;
                dummy = input.createTextRange();
                dummy.findText(aux);
                dummy.select();
                begin=input.value.indexOf(aux);
                if (this._DirectSelText == "P")
                {
                    this._DirectSelText = "";
                    ret = begin;
                }
                else
                {

                        ret = begin;

                }
                document.selection.clear();
                lenaux = parseInt(aux.length,10)-1;
            }
        }
        //notdo
        else if (input.setSelectionRange) 
        {
            if (input.selectionStart != input.selectionEnd)
            {
                var ini = parseInt(input.selectionStart,10);
                var fim = parseInt(input.selectionEnd,10);
                lenaux = fim - ini;
                begin=input.selectionStart;
                if (this._DirectSelText == "P")
                {
                    this._DirectSelText = "";
                    input.selectionEnd = input.selectionStart;
                    ret = begin;
                }
                else
                {

                        input.selectionEnd = input.selectionStart;
                        ret = begin;

                }
            }
        }
        //notdo
        if (ret !=-1)
        {
            for (i = 0 ; i < lenaux ; i++) 
            {
                if (this._isValidMaskedEditPosition(begin+i))
                {
                    masktext = masktext.substring(0,begin+i) + this._PromptChar + masktext.substring(begin+i+1);
                    this._LogicTextMask = this._LogicTextMask.substring(0,begin+i) + this._LogicPrompt + this._LogicTextMask.substring(begin+i+1);
                }
            }
            this.get_element().value = (masktext);

        }
        return ret;
    }
    , _processKey : function(poscur,key) {
        var posmask = this._LogicMaskConv;
        //  9 = only numeric

        var filter;

            filter = this._charNumbers;

        return ( filter.indexOf(key) != -1);
    }
    //
    // Previous valid Position
    //
    , _getPreviousPosition : function(pos)
    {
        while (!this._isValidMaskedEditPosition(pos) && pos > this._LogicFirstPos)
        {
            pos--;
        }
        if (pos < this._LogicFirstPos)
        {
            pos = this._LogicFirstPos;
        }
        return pos;
    }
     //
    // Current Position
    //
    , _getCurrentPosition : function()
    {
        begin = 0;
        input = this.get_element();
        if (input.setSelectionRange) 
        {
            begin = parseInt(input.selectionStart,10);
        }
        else if (document.selection) 
        {
            sel = document.selection.createRange();
            if (sel.text != "")
            {
                var aux = ""
                if (this._DirectSelText == "R")
                {
                    aux = sel.text + String.fromCharCode(3);
                }
                else if (this._DirectSelText == "L")
                {
                    aux = String.fromCharCode(3) + sel.text ;
                }
                sel.text = aux;
                this._DirectSelText == "";
            }
            else
            {
                sel.text = String.fromCharCode(3);
                this._DirectSelText == "";
            }
            dummy = input.createTextRange();
            dummy.findText(String.fromCharCode(3));
            dummy.select();
            begin=input.value.indexOf(String.fromCharCode(3));
            document.selection.clear();
        }
        if (begin > this._LogicLastPos+1)
        {
            begin = this._LogicLastPos+1;
        }
        if (begin < this._LogicFirstPos)
        {
            begin = this._LogicFirstPos;
        }
        return begin;
    }
    , _AdjustElementTime : function(value,ValueDefault)
    {
        var emp = true;    
        for (i = 0 ; i < parseInt(value.length,10) ; i++) 
        {
            if (value.substring(i,i+1) != this._PromptChar)
            {
                emp = false;
            }
        }
        if (emp)
        {
           return ValueDefault;
        }
        for (i = 0 ; i < parseInt(value.length,10) ; i++) 
        {
            if (value.substring(i,i+1) == this._PromptChar)
            {
                value = value.substring(0,i) + "0" + value.substring(i+1);
            }
        }
        return value;
    }
      , _processDeleteKey : function(scanCode)
    {
        if (scanCode == 46 /*delete*/) 
        {
            var curpos = this._deleteTextSelection();
            if (curpos == -1)
            {
                curpos = this._getCurrentPosition();

                    this._deleteAtPosition(curpos);
                    
            }
            this.setSelectionRange(curpos,curpos);
            return true;
        }
        else if (scanCode == 8 /*back-space*/) 
        {
            var curpos = this._getCurrentPosition();
            if (curpos <= this._LogicFirstPos)
            {
                return true;
            }

                this._deleteAtPosition(curpos);
                this.setSelectionRange(curpos,curpos);

            curpos = this._deleteTextSelection();
            if (curpos == -1)
            {
                curpos = this._getPreviousPosition(this._getCurrentPosition()-1);
                this._backspace(curpos);
            }
            this.setSelectionRange(curpos,curpos);
            return true;
        }
        return false;
    }
    //
    // delete at current position
    //
    , _deleteAtPosition : function(curpos) 
    {
        var masktext = this.get_element().value;
        if (this._isValidMaskedEditPosition(curpos))
        {
            var resttext = masktext.substring(curpos+1);
            var restlogi = this._LogicTextMask.substring(curpos+1);
            masktext = masktext.substring(0,curpos) + this._PromptChar;
            this._LogicTextMask = this._LogicTextMask.substring(0,curpos) + this._LogicPrompt;
            // clear rest of mask
            for (i = 0 ; i < parseInt(resttext.length,10) ; i++) 
            {
                if (this._isValidMaskedEditPosition(curpos+1+i))
                {
                    masktext += this._PromptChar;
                    this._LogicTextMask += this._LogicPrompt;
                }
                else
                {
                    masktext += resttext.substring(i,i+1);
                    this._LogicTextMask += restlogi.substring(i,i+1);
                }
            }
            // insert only valid text
            posaux = this._getNextPosition(curpos);
            for (i = 0 ; i < parseInt(resttext.length,10) ; i++) 
            {
                if (this._isValidMaskedEditPosition(curpos+1+i) && restlogi.substring(i,i+1) != this._LogicPrompt)
                {
                    masktext = masktext.substring(0,posaux) + resttext.substring(i,i+1) + masktext.substring(posaux+1);
                    this._LogicTextMask = this._LogicTextMask.substring(0,posaux) + restlogi.substring(i,i+1) + this._LogicTextMask.substring(posaux+1);
                    posaux = this._getNextPosition(posaux+1);
                }
            }            
        
            this.get_element().value = (masktext);
        }
    }
    //
    // this._backspace at current position
    //
    , _backspace : function(curpos) 
    {
        var masktext = this.get_element().value;
        if (this._isValidMaskedEditPosition(curpos))
        {
            masktext = masktext.substring(0,curpos) + this._PromptChar + masktext.substring(curpos+1);
            this._LogicTextMask = this._LogicTextMask.substring(0,curpos) + this._LogicPrompt + this._LogicTextMask.substring(curpos+1);
            this.get_element().value = (masktext);
        }
    }
    
    , _onKeyPressdown : function(args) 
    {
  
        var evt = this._CheckArgsEvents(args);
        if (evt == null)
        {
            return;
        }
        if (this.get_element().readOnly)
        {
            this._SetCancelEvent(evt);
            return ;
        }
        //enter
        if (evt.keyCode == 13)
        {
            this._onBlur(evt);
//            if (!this._CaptureValidatorsControl())
//            {
//                this._onFocus(evt);   ///m->_onFocus(evt); 
//            }
            return false;
        }

        // FOR FIREFOX (NOT IMPLEMENT document.activeElement)
        if (Sys.Browser.agent != Sys.Browser.InternetExplorer) 
        {
            if (this._InLostfocus)
            {
                this._onFocus(evt);   ///m->_onFocus(evt); 
            }
        }
        if (Sys.Browser.agent == Sys.Browser.InternetExplorer) {
            // IE
            var scanCode = evt.keyCode;
            this._OnNavigator(scanCode,evt,true)
        }
    },
    
    /********************************************************************/
    /********************masked edit function end************************/
    /********************************************************************/
    dispose : function() {
        /// <summary>
        /// Disposes this behavior's resources
        /// </summary>
        
        if (this._calendarPopup) {
            this._calendarPopup.dispose();
            this._calendarPopup = null;
        }
        this._modes = null;
        this._modeOrder = null;
        if (this._modeChangeMoveTopOrLeftAnimation) {
            this._modeChangeMoveTopOrLeftAnimation.dispose();
            this._modeChangeMoveTopOrLeftAnimation = null;
        }
        if (this._modeChangeMoveBottomOrRightAnimation) {
            this._modeChangeMoveBottomOrRightAnimation.dispose();
            this._modeChangeMoveBottomOrRightAnimation = null;
        }
        if (this._modeChangeAnimation) {
            this._modeChangeAnimation.dispose();
            this._modeChangeAnimation = null;
        }

       
        if (this._titleselect){
           $HGDomEvent.removeHandlers(this._titleselect, this._select$delegates);
            this._titleselect = null;
        }
        if (this._titleimg){
           $HGDomEvent.removeHandlers(this._titleimg, this._img$delegates);
            this._titleimg = null;
        }
        //
        if (this._popupDiv) {
            $HGDomEvent.removeHandlers(this._popupDiv, this._popup$delegates);
            this._popupDiv = null;
        }        
        if (this._prevArrow) {
            $HGDomEvent.removeHandlers(this._prevArrow, this._cell$delegates);
            this._prevArrow = null;
        }
        if (this._prevArrowImage) {
            $HGDomEvent.removeHandlers(this._prevArrowImage, this._cell$delegates);
            this._prevArrowImage = null;
        }
        if (this._nextArrow) {
            $HGDomEvent.removeHandlers(this._nextArrow, this._cell$delegates);
            this._nextArrow = null;
        }
        if (this._nextArrowImage) {        
            $HGDomEvent.removeHandlers(this._nextArrowImage, this._cell$delegates);
            this._nextArrowImage = null;
        }
        if (this._title) {
            $HGDomEvent.removeHandlers(this._title, this._cell$delegates);
            this._title = null;
        }
        if (this._today) {
            $HGDomEvent.removeHandlers(this._today, this._cell$delegates);
            this._today = null;
        }
        if (this._daysRow) {
            for (var i = 0; i < this._daysBody.rows.length; i++) {
                var row = this._daysBody.rows[i];
                for (var j = 0; j < row.cells.length; j++) {
                    $HGDomEvent.removeHandlers(row.cells[j].firstChild, this._cell$delegates);
                }
            }
            this._daysRow = null;
        }
        if (this._monthsRow) {
            for (var i = 0; i < this._monthsBody.rows.length; i++) {
                var row = this._monthsBody.rows[i];
                for (var j = 0; j < row.cells.length; j++) {
                    $HGDomEvent.removeHandlers(row.cells[j].firstChild, this._cell$delegates);
                }
            }
            this._monthsRow = null;
        }
        if (this._yearsRow) {
            for (var i = 0; i < this._yearsBody.rows.length; i++) {
                var row = this._yearsBody.rows[i];
                for (var j = 0; j < row.cells.length; j++) {
                    $HGDomEvent.removeHandlers(row.cells[j].firstChild, this._cell$delegates);
                }
            }
            this._yearsRow = null;
        }

        var elt = this.get_element();
        $HGDomEvent.removeHandlers(elt, this._element$delegates);
        $HGRootNS.DeluxeDate.callBaseMethod(this, "dispose");
    },
    
     show : function() {
        /// <summary>
        /// Shows the calendar
        /// </summary>
        
        this._ensureCalendar();
        
        if (!this._isOpen) {
            this.raiseShowing();
            this._isOpen = true;
            this._switchMonth(null,true); 
            this._calendarPopup.show();
        
            this.raiseShown();
        } 
    },
    hide : function() {
        /// <summary>
        /// Hides the calendar
        /// </summary>
        this.raiseHiding();
        if (this._container) {         
            this._calendarPopup.hide();        
            this._switchMode("days", true);            
        }
        this._isOpen = false;        
        this.raiseHidden();
    },
    
    suspendLayout : function() {
        /// <summary>
        /// Suspends layout of the behavior while setting properties
        /// </summary>

        this._layoutSuspended++;
    },
    resumeLayout : function() {
        /// <summary>
        /// Resumes layout of the behavior and performs any pending layout requests
        /// </summary>

        this._layoutSuspended--;
        if (this._layoutSuspended <= 0) {
            this._layoutSuspended = 0;
            if (this._layoutRequested) {
                this._performLayout();
            }
        }
    },
    invalidate : function() {
        /// <summary>
        /// Performs layout of the behavior unless layout is suspended
        /// </summary>
        
        if (this._layoutSuspended > 0) {
            this._layoutRequested = true;
        } else {
            this._performLayout();
        }
    },
    
    _buildCalendar : function() {
        /// <summary>
        /// Builds the calendar's layout
        /// </summary>
        
        var elt = this.get_element();
       
        this._container = $HGDomElement.createElementFromTemplate({
            nodeName : "div",
            cssClasses : [this._cssClass]
        }, this._calendarPopup.get_popupBody(), null, this._calendarPopup.get_popupDocument());


        
         this._popupDiv = $HGDomElement.createElementFromTemplate({ 
            nodeName : "div",
            events : this._popup$delegates, 
            properties : {
               // tabIndex : 0
            },
            cssClasses : ["ajax__calendar_container"], 
            visible : true 
        }, this._container);
        
    },
    _buildHeader : function() {
        /// <summary>
        /// Builds the header for the calendar
        /// </summary>
        
        this._header = $HGDomElement.createElementFromTemplate({ 
            nodeName : "div",
            cssClasses : [ "ajax__calendar_header" ]
        }, this._popupDiv);
          /*0508*/
        if(!this._isComplexHeader)        //不是复合的头，只显示左右的
        {
        var prevArrowWrapper = $HGDomElement.createElementFromTemplate({ nodeName : "div" }, this._header);
        this._prevArrow = $HGDomElement.createElementFromTemplate({ 
            nodeName : "div",
            properties : { mode : "prev" }, 
            events : this._cell$delegates,
            cssClasses : [ "ajax__calendar_prev" ] 
        }, prevArrowWrapper);
        
        var nextArrowWrapper = $HGDomElement.createElementFromTemplate({ nodeName : "div" }, this._header);
        this._nextArrow = $HGDomElement.createElementFromTemplate({ 
            nodeName : "div",
            properties : { mode : "next" },
            events : this._cell$delegates, 
            cssClasses : [ "ajax__calendar_next" ] 
        }, nextArrowWrapper);  
           }
        else   //只显示复合的头，填充右边部分
        {
                    /*0508*/
        var titlePadding = $HGDomElement.createElementFromTemplate({ nodeName : "div" }, this._header);        
        this._padding = $HGDomElement.createElementFromTemplate({ 
            nodeName : "div",
            cssClasses : [ "ajax__calendar_padding" ] 
        }, titlePadding);      
        //add list
        
        this._selectMonthList = $HGDomElement.createElementFromTemplate({ nodeName : "select" , properties : {
//                tabIndex : 0
            },events : this._select$delegates,cssClasses : [ "ajax__calendar_titleselect" ] }, this._header);
       
        for(var scount=0;scount<12;scount++)
        {
           _titleselect = $HGDomElement.createElementFromTemplate({
            nodeName : "option"}, this._selectMonthList);
           _titleselect.value = scount;
           _titleselect.text = scount+1;
        
        }        
              //add year

    var clickYearImg = $HGDomElement.createElementFromTemplate({ nodeName : "div" }, this._header);
       this._titleimg = $HGDomElement.createElementFromTemplate({
            nodeName : "div", properties : { mode : "next" },
            events : this._img$delegates,cssClasses : [ "ajax__calendar_titleimg" ]  }, clickYearImg);
            
            var clickYearInput = $HGDomElement.createElementFromTemplate({ nodeName : "div" , properties : {
//                tabIndex : 0
            },events : this._click$delegates}, this._header);
       this._titleinput = $HGDomElement.createElementFromTemplate({
            nodeName : "input",
            events : this._cell$delegates,cssClasses : [ "ajax__calendar_titleinput" ]  }, clickYearInput);
     }
        var titleWrapper = $HGDomElement.createElementFromTemplate({ nodeName : "div" }, this._header);        
        this._title = $HGDomElement.createElementFromTemplate({ 
            nodeName : "div",
            properties : { mode : "title" },
            events : this._cell$delegates, 
            cssClasses : [ "ajax__calendar_title" ] 
        }, titleWrapper);
    },
    _buildBody : function() {
        /// <summary>
        /// Builds the body region for the calendar
        /// </summary>
        
        this._body = $HGDomElement.createElementFromTemplate({ 
            nodeName : "div",
            cssClasses : [ "ajax__calendar_body" ]
        }, this._popupDiv);
       
        this._buildDays();
        this._buildMonths();
        this._buildYears();
    },
    _buildFooter : function() {
        /// <summary>
        /// Builds the footer for the calendar
        /// </summary>
        
        var todayWrapper = $HGDomElement.createElementFromTemplate({ nodeName : "div" }, this._popupDiv);
        this._today = $HGDomElement.createElementFromTemplate({
            nodeName : "div",
            properties : { mode : "today" },
            events : this._cell$delegates,
            cssClasses : [ "ajax__calendar_footer", "ajax__calendar_today" ]
        }, todayWrapper);
    },
    _buildDays : function() {
        /// <summary>
        /// Builds a "days of the month" view for the calendar
        /// </summary>
        
        var dtf = Sys.CultureInfo.CurrentCulture.dateTimeFormat;

        this._days = $HGDomElement.createElementFromTemplate({ 
            nodeName : "div",
            cssClasses : [ "ajax__calendar_days" ]
        }, this._body);
        this._modes["days"] = this._days;
        
        this._daysTable = $HGDomElement.createElementFromTemplate({ 
            nodeName : "table",
            properties : {
                cellPadding : 0,
                cellSpacing : 0,
                border : 0,
                style : { margin : "auto" }
            } 
        }, this._days);
        
        this._daysTableHeader = $HGDomElement.createElementFromTemplate({ nodeName : "thead" }, this._daysTable);
        this._daysTableHeaderRow = $HGDomElement.createElementFromTemplate({ nodeName : "tr" }, this._daysTableHeader);
        this._daysBody = $HGDomElement.createElementFromTemplate({ nodeName: "tbody" }, this._daysTable);
        
        for (var i = 0; i < 7; i++) {
            var dayCell = $HGDomElement.createElementFromTemplate({ nodeName : "td" }, this._daysTableHeaderRow);
            var dayDiv = $HGDomElement.createElementFromTemplate({
                nodeName : "div",
                cssClasses : [ "ajax__calendar_dayname" ]
            }, dayCell);
        }

        for (var i = 0; i < 6; i++) {
            var daysRow = $HGDomElement.createElementFromTemplate({ nodeName : "tr" }, this._daysBody);
            for(var j = 0; j < 7; j++) {
                var dayCell = $HGDomElement.createElementFromTemplate({ nodeName : "td" }, daysRow);
                var dayDiv = $HGDomElement.createElementFromTemplate({
                    nodeName : "div",
                    properties : {
                        mode : "day",
                        innerHTML : "&nbsp;"
                    },
                    events : this._cell$delegates,
                    cssClasses : [ "ajax__calendar_today" ]
                }, dayCell);
            }
        }
    },
    _buildMonths : function() {
        /// <summary>
        /// Builds a "months of the year" view for the calendar
        /// </summary>
        
        var dtf = Sys.CultureInfo.CurrentCulture.dateTimeFormat;        

        this._months = $HGDomElement.createElementFromTemplate({
            nodeName : "div",
            cssClasses : [ "ajax__calendar_months" ],
            visible : false
        }, this._body);
        this._modes["months"] = this._months;
        
        this._monthsTable = $HGDomElement.createElementFromTemplate({
            nodeName : "table",
            properties : {
                cellPadding : 0,
                cellSpacing : 0,
                border : 0,
                style : { margin : "auto" }
            }
        }, this._months);

        this._monthsBody = $HGDomElement.createElementFromTemplate({ nodeName : "tbody" }, this._monthsTable);
        for (var i = 0; i < 3; i++) {
            var monthsRow = $HGDomElement.createElementFromTemplate({ nodeName : "tr" }, this._monthsBody);
            for (var j = 0; j < 4; j++) {
                var monthCell = $HGDomElement.createElementFromTemplate({ nodeName : "td" }, monthsRow);
                var monthDiv = $HGDomElement.createElementFromTemplate({
                    nodeName : "div",
                    properties : {
                        mode : "month",
                        month : (i * 4) + j,
                        innerHTML : "<br />" + dtf.AbbreviatedMonthNames[(i * 4) + j]
                    },
                    events : this._cell$delegates,
                    cssClasses : [ "ajax__calendar_month" ]
                }, monthCell);
            }
        }
    },
    _buildYears : function() {
        /// <summary>
        /// Builds a "years in this decade" view for the calendar
        /// </summary>
        
        this._years = $HGDomElement.createElementFromTemplate({
            nodeName : "div",
            cssClasses : [ "ajax__calendar_years" ],
            visible : false
        }, this._body);
        this._modes["years"] = this._years;
        
        this._yearsTable = $HGDomElement.createElementFromTemplate({
            nodeName : "table",
            properties : {
                cellPadding : 0,
                cellSpacing : 0,
                border : 0,
                style : { margin : "auto" }
            }
        }, this._years);

        this._yearsBody = $HGDomElement.createElementFromTemplate({ nodeName : "tbody" }, this._yearsTable);        
        for (var i = 0; i < 3; i++) {
            var yearsRow = $HGDomElement.createElementFromTemplate({ nodeName : "tr" }, this._yearsBody);
            for (var j = 0; j < 4; j++) {
                var yearCell = $HGDomElement.createElementFromTemplate({ nodeName : "td" }, yearsRow);
                var yearDiv = $HGDomElement.createElementFromTemplate({ 
                    nodeName : "div", 
                    properties : { 
                        mode : "year",
                        year : ((i * 4) + j) - 1
                    },
                    events : this._cell$delegates,
                    cssClasses : [ "ajax__calendar_year" ]
                }, yearCell);
            }
        }
    },
    
    _performLayout : function() {
        /// <summmary>
        /// Updates the various views of the calendar to match the current selected and visible dates
        /// </summary>
        
        var elt = this.get_element();
        if (!elt) return;
        if (!this.get_isInitialized()) return;
        if (!this._isOpen) return;

        var dtf = Sys.CultureInfo.CurrentCulture.dateTimeFormat;        
        var selectedDate = this.get_selectedDate();
        var visibleDate = this._getEffectiveVisibleDate();
        var todaysDate = this.get_todaysDate(); 
       
        switch (this._mode) {
            case "days":
                
                var firstDayOfWeek = this._getFirstDayOfWeek();
                var daysToBacktrack = visibleDate.getDay() - firstDayOfWeek;
                if (daysToBacktrack <= 0)
                    daysToBacktrack += 7;
                    
                var startDate = new Date(visibleDate.getFullYear(), visibleDate.getMonth(), visibleDate.getDate() - daysToBacktrack);
                var currentDate = startDate;

                for (var i = 0; i < 7; i++) {
                    var dayCell = this._daysTableHeaderRow.cells[i].firstChild;
                    if (dayCell.firstChild) {
                        dayCell.removeChild(dayCell.firstChild);
                    }
                    dayCell.appendChild(this._calendarPopup.get_popupDocument().createTextNode(dtf.ShortestDayNames[(i + firstDayOfWeek) % 7]));
                }
                   var currenMonth = currentDate.getMonth();
                for (var week = 0; week < 6; week ++) {
                    var weekRow = this._daysBody.rows[week];
                    for (var dayOfWeek = 0; dayOfWeek < 7; dayOfWeek++) {
                        var dayCell = weekRow.cells[dayOfWeek].firstChild;
                        if (dayCell.firstChild) {
                            dayCell.removeChild(dayCell.firstChild);
                        }
                        //is show 
                        if(this._isOnlyCurrentMonth)
                        {
                           if( currentDate.getMonth() == (currenMonth +1==12?0:currenMonth +1) )
                           {
                           dayCell.appendChild(this._calendarPopup.get_popupDocument().createTextNode(currentDate.getDate()));
                        
                           dayCell.title = currentDate.localeFormat("D");
                           dayCell.date = currentDate;
  
                           $HGDomElement.removeCssClasses(dayCell.parentNode, [ "ajax__calendar_other", "ajax__calendar_active"]);
                           $HGDomElement.removeCssClasses(dayCell.parentNode, [ "ajax__calendar_currentmonthday"]);
                             if( currentDate.getDate()==new Date().getDate()&&currentDate.getMonth()==new Date().getMonth()&&currentDate.getFullYear()==new Date().getFullYear() )
                             {
                             Sys.UI.DomElement.addCssClass(dayCell.parentNode, "ajax__calendar_currentmonthday");//this._getCssClass(dayCell.date, 'd'));
                             }
                             else
                             {
                             Sys.UI.DomElement.addCssClass(dayCell.parentNode, this._getCssClass(dayCell.date, 'd'));
                             }
                           }
                        }
                       else
                       {
                             dayCell.appendChild(this._calendarPopup.get_popupDocument().createTextNode(currentDate.getDate()));
                        
                             dayCell.title = currentDate.localeFormat("D");
                             dayCell.date = currentDate;
  
                             $HGDomElement.removeCssClasses(dayCell.parentNode, [ "ajax__calendar_other", "ajax__calendar_active"]);
                               $HGDomElement.removeCssClasses(dayCell.parentNode, [ "ajax__calendar_currentmonthday"]);
                             if( currentDate.getDate()==new Date().getDate()&&currentDate.getMonth()==new Date().getMonth()&&currentDate.getFullYear()==new Date().getFullYear() )
                             {
                             Sys.UI.DomElement.addCssClass(dayCell.parentNode, "ajax__calendar_currentmonthday");//this._getCssClass(dayCell.date, 'd'));
                             }
                             else
                             {
                             Sys.UI.DomElement.addCssClass(dayCell.parentNode, this._getCssClass(dayCell.date, 'd'));
                             }
                        }
                        currentDate = new Date(currentDate.getFullYear(), currentDate.getMonth(), currentDate.getDate() + 1);
                    }
                }
                /*0508*/
                   if(!this._isComplexHeader)
                {
                this._prevArrow.date = new Date(visibleDate.getFullYear(), visibleDate.getMonth() - 1, 1);
                this._nextArrow.date = new Date(visibleDate.getFullYear(), visibleDate.getMonth() + 1, 1);
                
                if (this._title.firstChild) {
                    this._title.removeChild(this._title.firstChild);
                }
                
                //Add List
                /*0508*/
                this._title.appendChild(this._calendarPopup.get_popupDocument().createTextNode(visibleDate.localeFormat("yyyy,MMMM")));

      }
 else{
                //add inputdate
              if (this._title.firstChild) {
                    this._title.removeChild(this._title.firstChild);
                }
                
                //Add List
                
                this._title.appendChild(this._calendarPopup.get_popupDocument().createTextNode("   "));
                //add current selected
                this._selectMonthList.options[visibleDate.getMonth()].selected = true;
                this._titleinput.value = visibleDate.getFullYear(); 
                
         }

this._title.date = visibleDate;
 
                break;
            case "months":

                for (var i = 0; i < this._monthsBody.rows.length; i++) {
                    var row = this._monthsBody.rows[i];
                    for (var j = 0; j < row.cells.length; j++) {
                        var cell = row.cells[j].firstChild;
                        cell.date = new Date(visibleDate.getFullYear(), cell.month, 1);
                        $HGDomElement.removeCssClasses(cell.parentNode, [ "ajax__calendar_other", "ajax__calendar_active" ]);
                        Sys.UI.DomElement.addCssClass(cell.parentNode, this._getCssClass(cell.date, 'M'));
                    }
                }
                
                if (this._title.firstChild) {
                    this._title.removeChild(this._title.firstChild);
                }
                this._title.appendChild(this._calendarPopup.get_popupDocument().createTextNode(visibleDate.localeFormat("yyyy")));
                this._title.date = visibleDate;
                this._prevArrow.date = new Date(visibleDate.getFullYear() - 1, 0, 1);
                this._nextArrow.date = new Date(visibleDate.getFullYear() + 1, 0, 1);

                break;
            case "years":

                var minYear = (Math.floor(visibleDate.getFullYear() / 10) * 10);
                for (var i = 0; i < this._yearsBody.rows.length; i++) {
                    var row = this._yearsBody.rows[i];
                    for (var j = 0; j < row.cells.length; j++) {
                        var cell = row.cells[j].firstChild;
                        cell.date = new Date(minYear + cell.year, 0, 1);
                        if (cell.firstChild) {
                            cell.removeChild(cell.lastChild);
                        } else {
                            cell.appendChild(this._calendarPopup.get_popupDocument().createElement("br"));
                        }
                        cell.appendChild(this._calendarPopup.get_popupDocument().createTextNode(minYear + cell.year));
                        $HGDomElement.removeCssClasses(cell.parentNode, [ "ajax__calendar_other", "ajax__calendar_active" ]);
                        Sys.UI.DomElement.addCssClass(cell.parentNode, this._getCssClass(cell.date, 'y'));
                    }
                }

                if (this._title.firstChild) {
                    this._title.removeChild(this._title.firstChild);
                }
                this._title.appendChild(this._calendarPopup.get_popupDocument().createTextNode(minYear.toString() + "-" + (minYear + 9).toString()));
                this._title.date = visibleDate;
                this._prevArrow.date = new Date(minYear - 10, 0, 1);
                this._nextArrow.date = new Date(minYear + 10, 0, 1);

                break;
        }
        if (this._today.firstChild) {
            this._today.removeChild(this._today.firstChild);
        }
        this._today.appendChild(this._calendarPopup.get_popupDocument().createTextNode(todaysDate.localeFormat("yyyy-MMMM-dd")));
    
        //this._today.appendChild(this._calendarPopup.get_popupDocument().createTextNode(String.format("今天: {0}", todaysDate.localeFormat("yyyy年MMMMd日"))));

        this._today.date = todaysDate;        
    },
    
    _ensureCalendar : function() {
    
        if (!this._container) {
            
            var elt = this.get_element();
            //popoup size 200,200
            this._calendarPopup = $create($HGRootNS.PopupControl, {width:200, height:195, positionElement : elt, positioningMode : $HGRootNS.PositioningMode.BottomLeft}, {}, {}, null);         
            var doc = $HGDomElement.get_currentDocument;
            $HGDomElement.set_currentDocument(this._calendarPopup.get_popupDocument());
            try
            {
                //figure
                this._buildCalendar();
                this._buildHeader();
                this._buildBody();
                this._buildFooter();
                $HGDomElement.set_currentDocument(document);
            }
            finally
            {
                $HGDomElement.set_currentDocument(doc);
            }
        }    
    },

    
    _fireChanged : function() {
        /// <summary>
        /// Attempts to fire the change event on the attached textbox
        /// </summary>
        
        var elt = this.get_element();
        if (this._calendarPopup.get_popupDocument().createEventObject) {
            elt.fireEvent("onchange");
        } else if (this._calendarPopup.get_popupDocument().createEvent) {
            var e = this._calendarPopup.get_popupDocument().createEvent("HTMLEvents");
            e.initEvent("change", true, true);
            elt.dispatchEvent(e);
        }
    },
    _switchMonth : function(date, dontAnimate) {
        /// <summary>
        /// Switches the visible month in the days view
        /// </summary>
        /// <param name="date" type="Date">The visible date to switch to</param>
        /// <param name="dontAnimate" type="Boolean">Prevents animation from occuring if the control is animated</param>
        
        // Check _isAnimating to make sure we don't animate horizontally and vertically at the same time
        if (this._isAnimating) {
            return;
        }
        
        var visibleDate = this._getEffectiveVisibleDate();
        if ((date && date.getFullYear() == visibleDate.getFullYear() && date.getMonth() == visibleDate.getMonth())) {
            dontAnimate = true;
        }
        
        if (this._animated && !dontAnimate) {
            this._isAnimating = true;
            
            var newElement = this._modes[this._mode];
            var oldElement = newElement.cloneNode(true);
            this._body.appendChild(oldElement);
            if (visibleDate > date) {
                // animating down
                // the newIndex element is the top
                // the oldIndex element is the bottom (visible)
                
                // move in, fade in
                $HGDomElement.get_currentDocument(newElement, {x:-162,y:0});
                Sys.UI.DomElement.setVisible(newElement, true);
                this._modeChangeMoveTopOrLeftAnimation.set_propertyKey("left");
                this._modeChangeMoveTopOrLeftAnimation.set_target(newElement);
                this._modeChangeMoveTopOrLeftAnimation.set_startValue(-this._width);
                this._modeChangeMoveTopOrLeftAnimation.set_endValue(0);
                
                // move out, fade out
                $HGDomElement.get_currentDocument(oldElement, {x:0,y:0});
                Sys.UI.DomElement.setVisible(oldElement, true);
                this._modeChangeMoveBottomOrRightAnimation.set_propertyKey("left");
                this._modeChangeMoveBottomOrRightAnimation.set_target(oldElement);
                this._modeChangeMoveBottomOrRightAnimation.set_startValue(0);
                this._modeChangeMoveBottomOrRightAnimation.set_endValue(this._width);

            } else {
                // animating up
                // the oldIndex element is the top (visible)
                // the newIndex element is the bottom
                
                // move out, fade out
                $HGDomElement.get_currentDocument(oldElement, {x:0,y:0});
                Sys.UI.DomElement.setVisible(oldElement, true);
                this._modeChangeMoveTopOrLeftAnimation.set_propertyKey("left");
                this._modeChangeMoveTopOrLeftAnimation.set_target(oldElement);
                this._modeChangeMoveTopOrLeftAnimation.set_endValue(-this._width);
                this._modeChangeMoveTopOrLeftAnimation.set_startValue(0);

                // move in, fade in
                $HGDomElement.get_currentDocument(newElement, {x:162,y:0});
                Sys.UI.DomElement.setVisible(newElement, true);
                this._modeChangeMoveBottomOrRightAnimation.set_propertyKey("left");
                this._modeChangeMoveBottomOrRightAnimation.set_target(newElement);
                this._modeChangeMoveBottomOrRightAnimation.set_endValue(0);
                this._modeChangeMoveBottomOrRightAnimation.set_startValue(this._width);
            }
            this._visibleDate = date;
            this.invalidate();
            
            var endHandler = Function.createDelegate(this, function() { 
                this._body.removeChild(oldElement);
                oldElement = null;
                this._isAnimating = false;
                this._modeChangeAnimation.remove_ended(endHandler);
            });
            this._modeChangeAnimation.add_ended(endHandler);
            this._modeChangeAnimation.play();
        } else {
            this._visibleDate = date;
            this.invalidate();
        }
    },
    _switchMode : function(mode, dontAnimate) {
        /// <summary>
        /// Switches the visible view from "days" to "months" to "years"
        /// </summary>
        /// <param name="mode" type="String">The view mode to switch to</param>
        /// <param name="dontAnimate" type="Boolean">Prevents animation from occuring if the control is animated</param>
        
        // Check _isAnimating to make sure we don't animate horizontally and vertically at the same time
        if (this._isAnimating || (this._mode == mode)) {
            return;
        }
       
        var moveDown = this._modeOrder[this._mode] < this._modeOrder[mode];
        var oldElement = this._modes[this._mode];
        var newElement = this._modes[mode];
        this._mode = mode;
              
        if (this._animated && !dontAnimate) { 
            this._isAnimating = true;
            
            this.invalidate();

            if (moveDown) {   
                // animating down
                // the newIndex element is the top
                // the oldIndex element is the bottom (visible)
                
                // move in, fade in
                $HGDomElement.get_currentDocument(newElement, {x:0,y:-this._height});
                Sys.UI.DomElement.setVisible(newElement, true);
                this._modeChangeMoveTopOrLeftAnimation.set_propertyKey("top");
                this._modeChangeMoveTopOrLeftAnimation.set_target(newElement);
                this._modeChangeMoveTopOrLeftAnimation.set_startValue(-this._height);
                this._modeChangeMoveTopOrLeftAnimation.set_endValue(0);
                
                // move out, fade out
                $HGDomElement.get_currentDocument(oldElement, {x:0,y:0});
                Sys.UI.DomElement.setVisible(oldElement, true);

                this._modeChangeMoveBottomOrRightAnimation.set_propertyKey("top");
                this._modeChangeMoveBottomOrRightAnimation.set_target(oldElement);
                this._modeChangeMoveBottomOrRightAnimation.set_startValue(0);
                this._modeChangeMoveBottomOrRightAnimation.set_endValue(this._height);

            } else {  
                // animating up
                // the oldIndex element is the top (visible)
                // the newIndex element is the bottom
                
                // move out, fade out
                $HGDomElement.get_currentDocument(oldElement, {x:0,y:0});
                Sys.UI.DomElement.setVisible(oldElement, true);
                this._modeChangeMoveTopOrLeftAnimation.set_propertyKey("top");
                this._modeChangeMoveTopOrLeftAnimation.set_target(oldElement);
                this._modeChangeMoveTopOrLeftAnimation.set_endValue(-this._height);
                this._modeChangeMoveTopOrLeftAnimation.set_startValue(0);

                // move in, fade in
                $HGDomElement.get_currentDocument(newElement, {x:0,y:139});
                Sys.UI.DomElement.setVisible(newElement, true);
                this._modeChangeMoveBottomOrRightAnimation.set_propertyKey("top");
                this._modeChangeMoveBottomOrRightAnimation.set_target(newElement);
                this._modeChangeMoveBottomOrRightAnimation.set_endValue(0);
                this._modeChangeMoveBottomOrRightAnimation.set_startValue(this._height); 
            }
            var endHandler = Function.createDelegate(this, function() { 
                this._isAnimating = false;
                this._modeChangeAnimation.remove_ended(endHandler);
            });
            this._modeChangeAnimation.add_ended(endHandler);
            this._modeChangeAnimation.play();
        } else {
            this._mode = mode;
            Sys.UI.DomElement.setVisible(oldElement, false);
            this.invalidate();
            Sys.UI.DomElement.setVisible(newElement, true);
            $HGDomElement.get_currentDocument(newElement, {x:0,y:0});
        }
    },
    _isSelected : function(date, part) {
        /// <summary>
        /// Gets whether the supplied date is the currently selected date
        /// </summary>
        /// <param name="date" type="Date">The date to match</param>
        /// <param name="part" type="String">The most significant part of the date to test</param>
        /// <returns type="Boolean" />
        
        var value = this.get_selectedDate();
        if (!value) return false;
        switch (part) {
            case 'd':
                if (date.getDate() != value.getDate()) return false;
                // goto case 'M';
            case 'M':
                if (date.getMonth() != value.getMonth()) return false;
                // goto case 'y';
            case 'y':
                if (date.getFullYear() != value.getFullYear()) return false;
                break;
        }
        return true;
    },
    
    _isOther : function(date, part) {
        /// <summary>
        /// Gets whether the supplied date is in a different view from the current visible month
        /// </summary>
        /// <param name="date" type="Date">The date to match</param>
        /// <param name="part" type="String">The most significant part of the date to test</param>
        /// <returns type="Boolean" />

        var value = this._getEffectiveVisibleDate();
        switch (part) {
            case 'd': 
                return (date.getFullYear() != value.getFullYear() || date.getMonth() != value.getMonth());
            case 'M': 
                return false;
            case 'y': 
                var minYear = (Math.floor(value.getFullYear() / 10) * 10);
                return date.getFullYear() < minYear || (minYear + 10) <= date.getFullYear();
        }
        return false;
    },
    _getCssClass : function(date, part) {
        /// <summary>
        /// Gets the cssClass to apply to a cell based on a supplied date
        /// </summary>
        /// <param name="date" type="Date">The date to match</param>
        /// <param name="part" type="String">The most significant part of the date to test</param>
        /// <returns type="String" />

        if (this._isSelected(date, part)) {
            return "ajax__calendar_active";
        } else if (this._isOther(date, part)) {
            return "ajax__calendar_other";
        } else {
            return "";
        }
    },
    _getEffectiveVisibleDate : function() {
        var value = this.get_visibleDate();
        if (value == null) 
            value = this.get_selectedDate();
        if (value == null)
            value = this.get_todaysDate();
        return new Date(value.getFullYear(), value.getMonth(), 1);
    },
    _getFirstDayOfWeek : function() {
        /// <summary>
        /// Gets the first day of the week
        /// </summary>
        
        if (this.get_firstDayOfWeek() != $HGRootNS.FirstDayOfWeek.Default) {
            return this.get_firstDayOfWeek();
        }
        return Sys.CultureInfo.CurrentCulture.dateTimeFormat.FirstDayOfWeek;
    },
    _parseTextValue : function(text) {
        /// <summary>
        /// Converts a text value from the textbox into a date
        /// </summary>
        /// <param name="text" type="String" mayBeNull="true">The text value to parse</param>
        /// <returns type="Date" />
        
        var value = null;
        if(text) {
            value = Date.parseLocale(text, this.get_format());
        }
        if(isNaN(value)) {
            value = null;
        }
        return value;
    },
    
    /*When img click,set year change*/
    _setYearToDate : function(coordinate){
                //image 18px * 18px
                 if(coordinate>9)
               {
               this._titleinput.value = parseInt(this._titleinput.value) - 1;
                var visibleDate = this._getEffectiveVisibleDate();
                var selectdate = new Date(this._titleinput.value, visibleDate.getMonth(), 1);
                 this._switchMonth(selectdate);
               }
               else
               {
                this._titleinput.value = parseInt(this._titleinput.value) + 1;
                var visibleDate = this._getEffectiveVisibleDate();
                var selectdate = new Date(this._titleinput.value, visibleDate.getMonth(), 1);
                 this._switchMonth(selectdate);
               }
    },
    
    _simpleFormat : function(date)
    {
      
   var c = "-";
   y = date.getFullYear();
   m = "0" + (date.getMonth()+1);
   d = "0" + date.getDate();

   m = m.substr(m.length-2,2);
   d = d.substr(d.length-2,2);

   return y+c+m+c+d;
      },
      
    _onFocus : function(args) {
        /// <summary>
        /// Handles the completion of a deferred focus operation
        /// </summary>

//        this._blur.cancel();
//        this.get_element().focus();

       var evt = this._CheckArgsEvents(args);
     //var e = this.get_element();
        this._InLostfocus = false;
        if (this._OnFocusCssClass != "")
        {
            this.AddCssClassMaskedEdit(this._OnFocusCssClass);
        }
         var Inipos = this._InitValue();
        var ClearText = this._getClearMask(this.get_element().value);
        this._initialvalue = ClearText;
        
        this.setSelectionRange(Inipos,Inipos);
    },
    

    _element_onchange : function(e) {
        /// <summary> 
        /// Handles the change event of the element
        /// </summary>
        /// <param name="e" type="Sys.UI.DomEvent">The arguments for the event</param>
        
        if (!this._selectedDateChanging) {
            var elt = this.get_element();
            this._selectedDate = this._parseTextValue(elt.value);
            this._switchMonth(this._selectedDate, this._selectedDate == null);
        }
    },

    _popup_onfocus : function(e) {
        /// <summary> 
        /// Handles the focus event of the popup
        /// </summary>
        /// <param name="e" type="Sys.UI.DomEvent">The arguments for the event</param>
        
        if ((e.type == 'focus' && Sys.Browser.agent != Sys.Browser.InternetExplorer) ||
            (e.type == 'activate' && Sys.Browser.agent == Sys.Browser.InternetExplorer) ||
            (Sys.Browser.agent === Sys.Browser.Safari) ||
            (Sys.Browser.agent === Sys.Browser.Opera)) {

                this._focus.post();
        }
    },

    _cell_onmouseover : function(e) {
        /// <summary> 
        /// Handles the mouseover event of a cell
        /// </summary>
        /// <param name="e" type="Sys.UI.DomEvent">The arguments for the event</param>
  
        if (Sys.Browser.agent === Sys.Browser.Safari) {
            // Safari doesn't reliably call _cell_onmouseout, so clear other cells here to keep the UI correct
            for (var i = 0; i < this._daysBody.rows.length; i++) {
                var row = this._daysBody.rows[i];
                for (var j = 0; j < row.cells.length; j++) {
                    Sys.UI.DomElement.removeCssClass(row.cells[j].firstChild.parentNode, "ajax__calendar_hover");
                }
            }
            //add other browser
            for(var k=0; k<this._daysTableHeaderRow.cells.length; k++)
            {
             Sys.UI.DomElement.removeCssClass(this._daysTableHeaderRow.cells[k].firstChild.parentNode, "ajax__calendar_weekday"); 
            }
        }

        var target = e.target;

        Sys.UI.DomElement.addCssClass(target.parentNode, "ajax__calendar_hover");
        
        Sys.UI.DomElement.addCssClass(this._daysTableHeaderRow.cells[((target.date.getUTCDay()+(this._getFirstDayOfWeek()+1)) % 7)].firstChild.parentNode, "ajax__calendar_weekday");
         
        e.stopPropagation();
    },
    _cell_onmouseout : function(e) {
        /// <summary> 
        /// Handles the mouseout event of a cell
        /// </summary>
        /// <param name="e" type="Sys.UI.DomEvent">The arguments for the event</param>

        var target = e.target;

        Sys.UI.DomElement.removeCssClass(target.parentNode, "ajax__calendar_hover");

        for(var k=0; k<this._daysTableHeaderRow.cells.length; k++)
        {
         Sys.UI.DomElement.removeCssClass(this._daysTableHeaderRow.cells[k].firstChild.parentNode, "ajax__calendar_weekday"); 
        }
            
        e.stopPropagation();
    },
    _cell_onclick : function(e) {
        /// <summary> 
        /// Handles the click event of a cell
        /// </summary>
        /// <param name="e" type="Sys.UI.DomEvent">The arguments for the event</param>

        if ((Sys.Browser.agent === Sys.Browser.Safari) ||
            (Sys.Browser.agent === Sys.Browser.Opera)) {
            // _popup_onfocus doesn't get called on Safari or Opera, so we call it manually now
            this._popup_onfocus(e);
        }

        if (!this._enabled) 
            return;

        var target = e.target;
        var visibleDate = this._getEffectiveVisibleDate();
        Sys.UI.DomElement.removeCssClass(target.parentNode, "ajax__calendar_hover");
        switch(target.mode) {
            case "prev":
            case "next":
                this._switchMonth(target.date);//alert(target.date);
                break;
            case "title":
                switch (this._mode) {
                    case "days": this._switchMode("months"); break;
                    case "months": this._switchMode("years"); break;
                }
                break;
            case "month":
                if (target.month == visibleDate.getMonth()) {
                    this._switchMode("days");
                } else {
                    this._visibleDate = target.date;
                    this._switchMode("days");
                }
                break;
            case "year":
                if (target.date.getFullYear() == visibleDate.getFullYear()) {
                    this._switchMode("months");
                } else {
                    this._visibleDate = target.date;
                    this._switchMode("months");
                }
                break;
            case "day":
                this.set_selectedDate(target.date);
                this._switchMonth(target.date);
                this.hide();
                this.raiseDateSelectionChanged();
                break;
            case "today":
                this.set_selectedDate(target.date);
                this._switchMonth(target.date);
                this.hide();
                this.raiseDateSelectionChanged();
                break;
        }
   
        e.stopPropagation();
        e.preventDefault();
    },
    
    //add list event
    _select_onselect : function(e) {

     var visibleDate = this._getEffectiveVisibleDate();
                var selectdate = new Date(visibleDate.getFullYear(), e.target.value, 1);
                    this._switchMonth(selectdate);
               
              
    },

    _img_onclick : function(e) {
   
       this._setYearToDate(e.offsetY);
      
//       e.stopPropagation();
//       e.preventDefault();      
    }
    
        ,_button_onclick : function(e) {
        /// <summary> 
        /// Handles the click event of the asociated button
        /// </summary>
        /// <param name="e" type="Sys.UI.DomEvent">The arguments for the event</param>
        
        if (!this._isOpen) {
            e.preventDefault();
            e.stopPropagation();
            if (this._enabled) 
                this.show();
        } else {
            this._isOpen = false; this.show();
        } 
        return false;
       }
     ,_onkeypress : function(args)
    {
     var evt = this._CheckArgsEvents(args);
        if (evt == null)
        {
            return;
        }
        if (this.get_element().readOnly)
        {
            this._SetCancelEvent(evt);
            return;
        }

        var scanCode;
        var navkey = false;
        if (Sys.Browser.agent == Sys.Browser.InternetExplorer) 
        {
            // IE
            scanCode = evt.keyCode;
        }
        //not ie
        else
        {
            if (evt.charCode)
            {
                scanCode = evt.charCode;
            }
            else
            {
                scanCode = evt.keyCode;
            }
            if (evt.keyIdentifier) 
            {
                // Safari
                //3: 'KEY_ENTER', 13
                //63276: 'KEY_PAGE_UP', 33
                //63277: 'KEY_PAGE_DOWN', 34
                //63275: 'KEY_END', 35
                //63273: 'KEY_HOME', 36
                //63234: 'KEY_ARROW_LEFT', 37
                //63232: 'KEY_ARROW_UP', 38
                //63235: 'KEY_ARROW_RIGHT', 39
                //63233: 'KEY_ARROW_DOWN',40
                //63302: 'KEY_INSERT', 45
                //63272: 'KEY_DELETE', 46
                if (evt.ctrlKey || evt.altKey || evt.metaKey) 
                {
                    return;
                }
                if (evt.keyIdentifier.substring(0,2) != "U+") 
                {
                    return;
                }
                if (scanCode == 63272) // delete
                {
                    scanCode = 46; // convert to IE code
                    navkey = true;
                }
                else if (scanCode == 63302) 
                {
                    scanCode = 45; // convert to IE code
                    navkey = true;
                }
                else if (scanCode == 63233) 
                {
                    scanCode = 40; // convert to IE code
                    navkey = true;
                }
                else if (scanCode == 63235) 
                {
                    scanCode = 39; // convert to IE code
                    navkey = true;
                }
                else if (scanCode == 63232) 
                {
                    scanCode = 38; // convert to IE code
                    navkey = true;
                }
                else if (scanCode == 63234) 
                {
                    scanCode = 37; // convert to IE code
                    navkey = true;
                }
                else if (scanCode == 63273) 
                {
                    scanCode = 36; // convert to IE code
                    navkey = true;
                }
                else if (scanCode == 63275) 
                {
                    scanCode = 35; // convert to IE code
                    navkey = true;
                }
                else if (scanCode == 63277) 
                {
                    scanCode = 34; // convert to IE code
                    navkey = true;
                }
                else if (scanCode == 63276) 
                {
                    scanCode = 33; // convert to IE code
                    navkey = true;
                }
                else if (scanCode == 3) 
                {
                    scanCode = 13; // convert to IE code
                    navkey = true;
                }
            }    
            // key delete/backespace and key navigation for not IE browsers
            if (typeof(evt.which) != "undefined" && evt.which !=null)
            {
                if (evt.which == 0)
                {
                    navkey = true;
                }
            }
//            if (navkey && scanCode == 13)
//            {
//                this._onBlur(evt);
////                if (!this._CaptureValidatorsControl())
////                {
////                   this.get_element()._onFocus(evt);      
////                }
//                return false;
//            }
            if (scanCode == 8)
            {
                navkey = true;
            }
            if (!this._OnNavigator(scanCode,evt,navkey))
            {
                return;
            }
            if (this.SpecialNavKey(scanCode,navkey))
            {
                return;
            }
        }
        //
        if (scanCode && scanCode >= 0x20 /* space */) 
        {
            var c = String.fromCharCode(scanCode);
            var curpos = -1;
            //select and delete
            if (Sys.Browser.agent == Sys.Browser.InternetExplorer)
            {
                curpos = this._deleteTextSelection();
            }
            if (curpos == -1)
            {
                curpos = this._getCurrentPosition();
            }
            if (curpos <= this._LogicFirstPos)
            {
                this.setSelectionRange(this._LogicFirstPos,this._LogicFirstPos);
                curpos = this._LogicFirstPos;
            }
            ////////////////modify all logiclastpos+1
            else if (curpos >= this._LogicLastPos)
            {
                this.setSelectionRange(this._LogicLastPos,this._LogicLastPos);
                curpos = this._LogicLastPos;
            }
            var logiccur = curpos;
          
            //else if (this._processKey(logiccur,c)) 
             if (this._processKey(logiccur,c)) 
            {

                    this._insertContent(c,curpos);   
                    curpos = this._getNextPosition(curpos+1);

                this.setSelectionRange(curpos,curpos);
                this._SetCancelEvent(evt);
                return ;
            }
            else
            {
                //valid key navigation for not IE browsers
                //key navigation for IE capture in keydown 
                if (!this.SpecialNavKey(scanCode,navkey))
                {
                    this._SetCancelEvent(evt);
                }
                return;
            }
        }
        
    }
    
    //When textbox onblur reorganize value for input
    ,_onBlur : function(args)
    {
     var evt = this._CheckArgsEvents(args);
        
        this._InLostfocus = true;
        ValueText = this.get_element().value;
        ClearText = this._getClearMask(ValueText);
        // auto format empty text Time
        if (ClearText != "" && this._AutoComplete)
        {
            var CurDate = new Date();
            var Ycur = CurDate.getFullYear().toString();
            if (Ycur.length < 4)
            {
                Ycur = "0" + Hcur;
            }
            //support auto supply the lack: 00:00:00
            if (this._AutoCompleteValue != "" )
            {
                Ycur = this._AutoCompleteValue.substring(0,4);
            }
            var Symb = ""
 
            var Mcur = CurDate.getMonth().toString();
            if (Mcur.length < 2)
            {
                Mcur = "0" + Mcur;
            }
            if (this._AutoCompleteValue != "" )
            {
                Mcur = this._AutoCompleteValue.substring(5,7); 
            }
            var Dcur = CurDate.getDate().toString();
            if (Dcur.length < 2)
            {
                Dcur = "0" + Dcur;
            }
            var maskvalid = this._MaskConv.substring(this._LogicFirstPos,this._LogicFirstPos+this._LogicLastPos+1);    
            var PY = ValueText.substring(this._LogicFirstPos,this._LogicFirstPos+4);
            PY = this._AdjustElementTime(PY,Ycur);
            var PM = ValueText.substring(this._LogicFirstPos+5,this._LogicFirstPos+7);
            PM = this._AdjustElementTime(PM,Mcur);
            var PD;
            //if this is Year:Month:Day format
            if (maskvalid == "9999" + this._CultureTimePlaceholder + "99" + this._CultureTimePlaceholder + "99")
            {
                if (this._AutoCompleteValue != "" )
                {
                    Dcur = this._AutoCompleteValue.substring(8,10);  
                }
                PD = ValueText.substring(this._LogicFirstPos+8,this._LogicLastPos+1);
                PD = this._AdjustElementTime(PD,Dcur);
                ValueText = ValueText.substring(0,this._LogicFirstPos) + PY + this._CultureTimePlaceholder + PM + this._CultureTimePlaceholder + PD + ValueText.substring(this._LogicLastPos+1);
                this._LogicTextMask = this._LogicTextMask.substring(0,this._LogicFirstPos) + PY + this._CultureTimePlaceholder + PM + this._CultureTimePlaceholder + PD + this._LogicTextMask.substring(this._LogicLastPos+1);
            }
            else
            {
                ValueText = ValueText.substring(0,this._LogicFirstPos) + PY + this._CultureTimePlaceholder + PM + ValueText.substring(this._LogicLastPos+1);
                this._LogicTextMask = this._LogicTextMask.substring(0,this._LogicFirstPos) + PY + this._CultureTimePlaceholder + PM + this._LogicTextMask.substring(this._LogicLastPos+1);
            }
     
            this.get_element().value = (ValueText);
            ClearText = this._getClearMask(ValueText);
        }
        // auto format empty text Number
    
        // auto format empty text Date

        // clear mask and set CSS
        if (this._ClearMaskOnLostfocus)
        {
             this.get_element().value = (ClearText);
        }
        ValueText = ClearText;
        this.AddCssClassMaskedEdit("");
        //////////////////////////////////////////////valid

        

       
        // perform validation
        // 
        //is use to valid,Valid default  
     if(this._IsValidValue)
     {
        var IsValid = this._CaptureValidatorsControl(ValueText);//alert(ValueText)
        //
        if (!IsValid)
        {
            alert(this._CurrentMessageError);
            //this.get_element().focus();
            if (this._OnInvalidCssClass != "")
            {
                this.AddCssClassMaskedEdit(this._OnInvalidCssClass);
            }
             
            //resume mask textbox after valided
            if (this._ClearTextOnInvalid)
            {
                this.get_element().value = (this._createMask());
            }
        }
        else
        {
            // trigger TextChanged with postback
           
            if (evt != null &&  typeof(this.get_element().onchange) != "undefined" && this.get_element().onchange != null && !this.get_element().readOnly)
            {
                if (this._initialvalue != this.get_element().value)
                {
                    this.get_element().onchange(evt);
                }
            }
        }
      }//
    }
    
}
$HGRootNS.DeluxeDate.registerClass($HGRootNSName + ".DeluxeDate", $HGRootNS.ControlBase);
