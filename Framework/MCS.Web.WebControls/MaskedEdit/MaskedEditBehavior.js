// **************************************************
// MaskedEdit Control
// **************************************************
$HGRootNS.MaskedEditBehavior = function(element) 
{
    $HGRootNS.MaskedEditBehavior.initializeBase(this, [element]);
   
 // **************************************************
    // Properties
    // **************************************************
    //
   
    this._IsValidValue = true;
    //

    // mask
//    this._MaskedEditTextBoxID = "_text";
//    this._MaskedEditButtonID = "_image";
    //


    
    this._Mask = "99:99:99";

    this._Filtered = "";
   
    this._PromptChar = "_";

    // AutoComplete
    this._AutoComplete = true;
    this._AutoCompleteValue =  "";
    // behavior
    this._ClearTextOnInvalid = false;
    this._ClearMaskOnLostfocus = true;

    // CSS
    this._OnFocusCssClass = "MaskedEditFocus";
    this._OnInvalidCssClass = "MaskedEditError";


    this._CultureTimePlaceholder = ":";



    // **************************************************
    // local var special mask
    // **************************************************

  
    this._MaskConv = "";
    // **************************************************
    // Others local Var
    // **************************************************
    // save the Direction selected Text (only for ie)
    this._DirectSelText = "";
    // save the initial value for verify changed
    this._initialvalue = "";
    // save the symbol Negative
    this._LogicSymbol = "";
    // save logic mask with text input
    this._LogicTextMask = "";
    // save logic mask without text
    this._LogicMask = "";
    // save logic mask without text and without escape
    this._LogicMaskConv = "";
    // ID prompt char
    this._LogicPrompt = String.fromCharCode(1);
    // ID escape char
    this._LogicEscape = String.fromCharCode(2);
    // first valid position
    this._LogicFirstPos = -1;
    // Last valid position 
    this._LogicLastPos = -1;
    // Qtd Valid input Position
    this._QtdValidInput = 0;
    // Flag to validate in lost focus not duplicate clearMask execute
    this._InLostfocus = false;

    // Save local Current MessageError
   
    this._CurrentMessageError = "";
    // **************************************************
    // local chars ANSI
    // **************************************************

    this._charNumbers = "0123456789";    

  
    // **************************************************
    // Handler
    // **************************************************
    this._focusHandler = null;
    this._keypressdown = null;
    this._keypressHandler = null;
    this._blurHandler = null;
    //popup event
    this._clickHandler = null;
    this._listClickHandler = null;
    // **************************************************
    // end Declaration
    // **************************************************
   
}    
$HGRootNS.MaskedEditBehavior.prototype = { 
    initialize : function() 
    { 
        var e = this.get_element();

         /*valid mask number*/
        
        //Mask   99:99:99 (2~3)
        var formatArray = this._Mask.split(this._CultureTimePlaceholder);
        if(formatArray.length<2 || formatArray.length>3)
        {   
         this.get_element().value = "Mask format error!";
         this.get_element().disabled = "disabled";
         return;
        }
        
        //
        for(var i=0;i<formatArray.length;i++)
        {
         if(formatArray[i] != "99")
         {
         this.get_element().value = "Mask char number must be '9'";
         this.get_element().disabled = "disabled";
         return;
         }
        }
        
        //AutoCompleteValue
        if(this._AutoCompleteValue != "")
        {
         var autoValueArray = this._AutoCompleteValue.split(":");
         if(autoValueArray.length != formatArray.length)
         {   
          this.get_element().value = "AutoCompleteValue must be equal to mask!";
          this.get_element().disabled = "disabled";
          return;
         }
         for(var i=0;i<autoValueArray.length;i++)
         {
            //two characters
            if(autoValueArray[i].length == 2)
            {
              for(var j=0;j<autoValueArray[i].length;j++)
              {//alert(autoValueArray[i].substring(j,j+1))
             //not numeric
               if(this._charNumbers.indexOf(autoValueArray[i].substring(j,j+1)) == -1)
               {
                this.get_element().value = "AutoCompleteValue must be numeric!";
                this.get_element().disabled = "disabled";
                return;
               }
              }
             }
             else
             {
             this.get_element().value = "AutoCompleteValue format error!";
             this.get_element().disabled = "disabled";
               return;
             }
          }

        }
        
        this._InLostfocus = true;
        // TODO: add your initialization code here         
        $HGRootNS.MaskedEditBehavior.callBaseMethod(this, 'initialize');

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


        //
      
        // Create delegates Attach events
        this._focusHandler = Function.createDelegate(this, this._onFocus);
        $addHandler(e, "focus", this._focusHandler);

        this._keypressdown = Function.createDelegate(this, this._onKeyPressdown1);
        $addHandler(e, "keydown", this._keypressdown); 

        this._keypressHandler = Function.createDelegate(this, this._onKeyPress1);
        $addHandler(e, "keypress", this._keypressHandler); 

        this._blurHandler = Function.createDelegate(this, this._onBlur);
        $addHandler(e, "blur", this._blurHandler);
        
//        this._clickHandler = Function.createDelegate(this, this._onClick);
//        $addHandler($get(this._MaskedEditButtonID), "click", this._clickHandler);
        
        //popup list event
//        this._listClickHandler = Function.createDelegate(this, this._onListClick);
//        $addHandler(this, "click", this._listClickHandler);  
 
         
//           this._listClickHandler = {click : Function.createDelegate(this, this._onListClick)}
      
        if (hasInitialFocus) 
        {
            this._onFocus();
        }
        else if (e.value != "")
        {
            this._InitValue();
            if (this._ClearMaskOnLostfocus)
            {
                e.value = (this._getClearMask(e.value));
            }
        }


    }
//    , dispose : function() {

//        $HGRootNS.MaskedEditBehavior.callBaseMethod(this, 'dispose');
//    }

/*
@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
*/

  , _InitValue : function()
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

//            if (this._InputDirection == $HGRootNS.MaskedEditInputDirections.LeftToRight)
//            {
                this.loadValue(initValue,this._LogicFirstPos);
//            } 
//            else if (this._InputDirection == $HGRootNS.MaskedEditInputDirections.RightToLeft)
//            {
//                this.loadValue(initValue,this._LogicLastPos);
//            }

        }
        return Inipos;
    }
    //
    // convert format Date
    //

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
//        if (this._InputDirection == $HGRootNS.MaskedEditInputDirections.LeftToRight)
//        {

            var oldfocus = this._InLostfocus;   
            var i = 0;
            this._InLostfocus = false;
            if (this._ClearMaskOnLostfocus == false)
            {
                logicPosition  = 0;
            }
            //for mask number count
            for (i = 0 ; i < parseInt(this._Mask.length,10) ; i++) 
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
    //
    // Detach events this.dispose
    //
    , dispose : function() 
    {
        var e = this.get_element();
        // restore maxLength
     
//       var eButton = $get(this._MaskedEditButtonID);
       
        if (this._focusHandler) 
        {
            $removeHandler(e, "focus", this._focusHandler);
            this._focusHandler = null;
        }
        if (this._blurHandler) 
        {
            $removeHandler(e, "blur", this._blurHandler);
            this._blurHandler = null;
        }
        if (this._keypressdown) 
        {
            $removeHandler(e, "keydown", this._keypressdown);
            this._keypressdown = null;
        }
        if (this._keypressHandler) 
        {
            $removeHandler(e, "keypress", this._keypressHandler);
            this._keypressHandler = null;
        }
        //popup click event
//        if (this._clickHandler)
//        {
//            $removeHandler(eButton, "click", this._clickHandler);
//            this._clickHandler = null;
//        }
//        if (this. _listClickHandler)
//        {
//            $removeHandler(this, "click", this._listClickHandler);
//            this._listClickHandler = null;
//        }
        
        $HGRootNS.MaskedEditBehavior.callBaseMethod(this, 'dispose');
    }
    //
    // Check Event Argumet
    //
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
    // attachEvent button click
    //
//    ,_onClick : function(args)
//    {
//    if(this._popupControl==null)
//      {
//      if(args.target.type=="button")
//        {
//        
////        if( this._DataList!="" )
//        if( this._DataArrayList!=null&&this._DataArrayList.length!=0 )
//          {

////        var bindData = this._DataList.split(";");  
//    
//    //var elt = this.get_element(); 

//    this._popupControl =$create($HGRootNS.PopupControl,{width:this._PopupWidth, height:this._PopupHeight, positionElement : $get(this._MaskedEditButtonID), positioningMode : $HGRootNS.PositioningMode.BottomLeft}, {}, {}, null);  
//    //$HGCommon.set_currentDocument(this._popupControl.get_popupDocument());
//    //this._focus.post(); 
//     

//     for(var i=0;i<this._DataArrayList.length;i++)
//     {
//     $HGCommon.createElementFromTemplate(
//            {
//                nodeName : "div"
//               ,properties : {
//                innerText : this._DataArrayList[i]     
//                              }
//               ,events : this._listClickHandler
//               ,cssClasses : [ "ListCss" ]
//            },
//            
//            this._popupControl.get_popupDocument().body, null, this._popupControl.get_popupDocument()
//        );
//        
//        }

//        
//this._popupControl.show();

////popupControl.show();
//          }
//          else
//          {   
//             alert("未提供可选择的数据!");
//          }

//        }///
//      }//
//      else
//      {
//      //alert("has a popup");
//      this._popupControl.show();
//      }
//     
//    }
    //
    // attachEvent list click
    //
//    ,_onListClick : function(args)
//    {
//    var clickValue = args.target.innerHTML;
//    if(clickValue==null)
//    {return;}
//    $get(this._MaskedEditTextBoxID).value = clickValue;
//    this._hide();
//    }
    //
    // attachEvent focus
    //
    ,_onFocus : function(args)
    {
//        if(args.target.type=="button")
//    {return;}

    var evt = this._CheckArgsEvents(args);
     var e = this.get_element();
        this._InLostfocus = false;
        if (this._OnFocusCssClass != "")
        {
            this.AddCssClassMaskedEdit(this._OnFocusCssClass);
        }
         var Inipos = this._InitValue();
        var ClearText = this._getClearMask(e.value);
        this._initialvalue = ClearText;
        
        this.setSelectionRange(Inipos,Inipos);
        
    //alert($get(this._MaskedEditTextBoxID).value)
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
    //
    // attachEvent Blur
    //
    , _onBlur : function(args) 
    {
//        if(args.target.type=="button")
//        {return;}
        var evt = this._CheckArgsEvents(args);
        
        this._InLostfocus = true;
        ValueText = this.get_element().value;
        ClearText = this._getClearMask(ValueText);
        // auto format empty text Time
        if (ClearText != "" && this._AutoComplete)
        {
            var CurDate = new Date();
            var Hcur = CurDate.getHours().toString();
            if (Hcur.length < 2)
            {
                Hcur = "0" + Hcur;
            }
            
            if (this._AutoCompleteValue != "" )
            {
                Hcur = this._AutoCompleteValue.substring(0,2);
            }
            var Symb = ""
 
            var Mcur = CurDate.getMinutes().toString();
            if (Mcur.length < 2)
            {
                Mcur = "0" + Mcur;
            }
            if (this._AutoCompleteValue != "" )
            {
                Mcur = this._AutoCompleteValue.substring(3,5);
            }
            var Scur = CurDate.getSeconds().toString();
            if (Scur.length < 2)
            {
                Scur = "0" + Scur;
            }
            var maskvalid = this._MaskConv.substring(this._LogicFirstPos,this._LogicFirstPos+this._LogicLastPos+1);    
            var PH = ValueText.substring(this._LogicFirstPos,this._LogicFirstPos+2);
            PH = this._AdjustElementTime(PH,Hcur);
            var PM = ValueText.substring(this._LogicFirstPos+3,this._LogicFirstPos+5);
            PM = this._AdjustElementTime(PM,Mcur);
            var PS;
           
            if (maskvalid == "99" + this._CultureTimePlaceholder + "99" + this._CultureTimePlaceholder + "99")
            {
                if (this._AutoCompleteValue != "" )
                {
                    Scur = this._AutoCompleteValue.substring(6,8);
                }
                PS = ValueText.substring(this._LogicFirstPos+6,this._LogicLastPos+1);
                PS = this._AdjustElementTime(PS,Scur);
                ValueText = ValueText.substring(0,this._LogicFirstPos) + PH + this._CultureTimePlaceholder + PM + this._CultureTimePlaceholder + PS + ValueText.substring(this._LogicLastPos+1);
                this._LogicTextMask = this._LogicTextMask.substring(0,this._LogicFirstPos) + PH + this._CultureTimePlaceholder + PM + this._CultureTimePlaceholder + PS + this._LogicTextMask.substring(this._LogicLastPos+1);
            }
            else
            {
                ValueText = ValueText.substring(0,this._LogicFirstPos) + PH + this._CultureTimePlaceholder + PM + ValueText.substring(this._LogicLastPos+1);
                this._LogicTextMask = this._LogicTextMask.substring(0,this._LogicFirstPos) + PH + this._CultureTimePlaceholder + PM + this._LogicTextMask.substring(this._LogicLastPos+1);
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
       //////// this.ShowTooltipMessage(false);
        // 
       
     if(this._IsValidValue)
     {
        var IsValid = this._CaptureValidatorsControl(PH,PM,PS);
       
        if (!IsValid)
        {
            alert(this._CurrentMessageError);
            //this.get_element().focus();
            if (this._OnInvalidCssClass != "")
            {
                this.AddCssClassMaskedEdit(this._OnInvalidCssClass);
            }
            
         
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
                if (this._initialvalue !=  $get(this._MaskedEditTextBoxID).value)
                {
                    this.get_element().onchange(evt);
                }
            }
        }
      }// 
    }
    //
    // Capture and execute validator to control
    //
    ,_CaptureValidatorsControl : function(PH,PM,PS)
    {
      var ret = true;
     if(parseInt(PH,10)>=24||parseInt(PM,10)>=60)
     {
     ret = false;
     }
     
     if(PS!=null&&typeof(PS)!="undefined")
     {
      if(parseInt(PS,10)>=60)
      {
      ret = false;
      }
     }
      return ret;
      //alert(this.get_element().value.substring(0,2))//this._CultureTimePlaceholder))

    }
    
    //
    // Set Cancel Event for cross browser
    //
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
   , _onKeyPress1 : function(args) 
        {

// if(args.target.type=="button")
//        {return;}
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
//                if (!this._CaptureValidatorsControl())
//                {
//                    this._onFocu(evt);      ///m->_onFocus(evt); 
//                }
//                return;
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


    //
    // keypress Navigate key (up/down/left/right/pgup/pgdown/home/end)
    // not IE process for event keypress
    //
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
//            if (curpos <= this._LogicFirstPos && this._InputDirection != $HGRootNS.MaskedEditInputDirections.RightToLeft)
//            {
//                this.setSelectionRange(this._LogicFirstPos,this._LogicFirstPos);
//            }
//            if (this._MessageValidatorTip) 
//            {
//                if (this._getClearMask(this.get_element().value) == "")
//                {
//       //////////             this.ShowTooltipMessage(true);
//                }
//            }
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
    , _onKeyPressdown1 : function(args) 
    {
  
//   if(args.target.type=="button")
//        {return;}
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
//            this._onBlur(evt);
//            if (!this._CaptureValidatorsControl())
//            {
//                this._onFocu(evt);   ///m->_onFocus(evt); 
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
    }
    //
    // Set Cursor at position in TextBox
    //
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
    

    


    //
    // Process del and this._backspace key
    //
    , _processDeleteKey : function(scanCode)
    {
        if (scanCode == 46 /*delete*/) 
        {
            var curpos = this._deleteTextSelection();
            if (curpos == -1)
            {
                curpos = this._getCurrentPosition();
//                if (this._InputDirection == $HGRootNS.MaskedEditInputDirections.RightToLeft &&  curpos <= this._LogicFirstPos)
//                {
//                    var logicur = this._getLastEmptyPosition();
//                    logicur = this._getNextPosition(logicur+1);
//                    this._backspace(logicur);
//                }
//                else
//                {
                    this._deleteAtPosition(curpos);
//                }
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
//            if (this._InputDirection == $HGRootNS.MaskedEditInputDirections.LeftToRight)
//            {
                this._deleteAtPosition(curpos);
                this.setSelectionRange(curpos,curpos);
//            }
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
//                    if (this._InputDirection == $HGRootNS.MaskedEditInputDirections.LeftToRight)
//                    {
                        ret = begin;
//                    } 
//                    else if (this._InputDirection == $HGRootNS.MaskedEditInputDirections.RightToLeft)
//                    {
//                        ret = begin+parseInt(aux.length,10)-1;
//                    }
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
//                    if (this._InputDirection == $HGRootNS.MaskedEditInputDirections.LeftToRight)
//                    {
                        input.selectionEnd = input.selectionStart;
                        ret = begin;
//                    }
//                    else if (this._InputDirection == $HGRootNS.MaskedEditInputDirections.RightToLeft)
//                    {
//                        input.selectionStart = input.selectionEnd;
//                        ret = begin+lenaux;
//                    }
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
//            if (this._InputDirection == $HGRootNS.MaskedEditInputDirections.RightToLeft)
//            {
//                //fix at first position
//                if (ret > this._LogicLastPos || ret < this._LogicFirstPos)
//                {
//                    ret = this._LogicFirstPos;
//                }
//            }
        }
        return ret;
    }
    //
    // Insert Content at position in curpos
    //
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


    //
    // get first position not empty in text 
    //

    //
    // position is valid edit ?
    //
    , _isValidMaskedEditPosition : function(pos) 
    {
        return (this._LogicMask.substring(pos,pos+1) == this._LogicPrompt);
    }
    //
    // Next valid Position
    //
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
    // Validate key at position in mask and/or filter
    //
    , _processKey : function(poscur,key) {
        var posmask = this._LogicMaskConv;
        //  9 = only numeric

        var filter;
//        if  (posmask.substring(poscur,poscur+1) == "9")
//        {
            filter = this._charNumbers;
//        }
        if (filter == "")
        {
            return true;
        }
        // return true if we should accept the character.
        return (!filter || filter.length == 0 || filter.indexOf(key) != -1);
    }    
    //
    // create mask empty , logic mask empty
    // convert escape code and Placeholder to culture
    
    , _createMask : function()
    {
        var text;
        if (this._MaskConv == "" && this._Mask != "")
        {
     
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
                        if (text.substring(i, i+1) == ":")
                        {
                            masktext += this._CultureTimePlaceholder;
                            this._LogicTextMask += this._CultureTimePlaceholder;
                        }  
                    else{
                            masktext += this._PromptChar;
                            this._LogicTextMask += this._LogicPrompt;
                         }
                         

          //
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
    // return text without mask but with placeholders 
   
    , _getClearMask : function(masktext)
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
    //
    // Convert notation {Number} in PAD's Number
   
    , _convertMask : function() 
    {
        this._MaskConv = "";
        var qtdmask = "";
        var maskchar = "";
        for (i = 0 ; i < parseInt(this._Mask.length,10) ; i++) 
        {
//          if (this._Mask.substring(i, i+1)=="9")
//          {
           
            if (qtdmask.length == 0)
            {
                this._MaskConv += this._Mask.substring(i, i+1);
                qtdmask = "";
                maskchar = this._Mask.substring(i, i+1);
            }
            else if (this._Mask.substring(i, i+1) == "9")
            {
                qtdmask += "9";
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
//            if (atumask.substring(i, i+1) == this._charEscape)
//            {
//                flagescape = true;
//            }
            //else if (!flagescape)
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
    
    /*@@@@@@@@@@@@@@@@@@@@@@@@
    property
    @@@@@@@@@@@@@@@@@@@@@@@@@@*/
    
    //
    // Helper properties
    //
    , get_Mask : function() {
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
//    , get_MessageValidatorTip : function() 
//    {
//        return this._MessageValidatorTip;
//    }      
//    , set_MessageValidatorTip : function(value) 
//    {
//        this._MessageValidatorTip = value;
//        this.raisePropertyChanged('MessageValidatorTip');
//    }      
  
//    , get_AcceptNegative : function() 
//    {
//        return this._AcceptNegative;
//    }      
//    , set_AcceptNegative : function(value) 
//    {
//        this._AcceptNegative= value;
//        this.raisePropertyChanged('AcceptNegative');
//    }   

 
   
  
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
   
//    ,  get_DataList : function()
//    {
//        return this._DataList;
//    } 
//    //new add
//    ,  set_DataList :function(value)
//    {
//        this._DataList = value;
//        this.raisePropertyChanged('DataList');
//    }

//    ,get_cssClass : function() {
//        return this._cssClass || "ListCss"; 
//    }
//    ,set_cssClass : function(value) { 
//        if (this._cssClass != value) {
//            this._cssClass = value; 
//            this.raisePropertyChanged("cssClass");
//        }
//    }

    //component
//    ,get_MaskedEditTextBoxID : function()
//    {
//        return this._MaskedEditTextBoxID;
//    }
//    ,set_MaskedEditTextBoxID : function(value)
//    {
//        this._MaskedEditTextBoxID = value;  
//        //this.raisePropertyChanged('MaskedEditTextBoxID');
//    }
//    ,get_MaskedEditButtonID : function()
//    {
//        return this._MaskedEditButtonID;
//    }
//    ,set_MaskedEditButtonID : function(value)
//    {
//        this._MaskedEditButtonID = value;
//    }
  
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
    }

    //popup
//    ,get_PopupWidth : function()
//    {
//       return this._PopupWidth;
//    }
//    ,set_PopupWidth : function(value)
//    {
//       this._PopupWidth = value;
//    }
//    ,get_PopupHeight : function()
//    {
//       return this._PopupHeight;
//    }
//    ,set_PopupHeight : function(value)
//    {
//       this._PopupHeight = value;
//    }
//    ,get_DataArrayList : function()
//    {
//       return this._DataArrayList;
//    }
//    ,set_DataArrayList : function(value)
//    {
//       this._DataArrayList = value;
//    }
}
$HGRootNS.MaskedEditBehavior.registerClass('$HGRootNS.MaskedEditBehavior', $HGRootNS.ControlBase);//.DynamicPopulateBehaviorBase);



