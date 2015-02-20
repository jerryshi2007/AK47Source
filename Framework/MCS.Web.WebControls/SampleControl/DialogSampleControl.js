
$HGRootNS.DialogSampleControl = function(element)
{
    $HGRootNS.DialogSampleControl.initializeBase(this, [element]);
    this._okBtn = null;
    this._cancelBtn = null;
    this._dataControlID = null;
    this._dataControl = null;
    this._dialogArg = null;
    this._okBtnEvents = 
        {
            click : Function.createDelegate(this, this._onOkBtnClick)
        };
    this._cancelBtnEvents = 
        {
            click : Function.createDelegate(this, this._onCancelBtnClick)
        };
}

$HGRootNS.DialogSampleControl.prototype = 
{
    initialize : function()
    {
        $HGRootNS.DialogSampleControl.callBaseMethod(this, 'initialize');
        this._dialogArg = window.dialogArguments;
        this._dataControl = Sys.Application.findComponent(this._dataControlID);
        
        this._buildControl();
        this._loadDialogArgument();
    },
    
    dispose : function()
    {
        $HGDomEvent.removeHandlers(this._okBtn, this._okBtnEvents);
        $HGDomEvent.removeHandlers(this._cancelBtn, this._cancelBtnEvents);
        this._okBtn = null;
        this._cancelBtn = null;
        this._dataControl = null;
        
        $HGRootNS.DialogSampleControl.callBaseMethod(this, 'dispose');
    },    
    
    _buildControl : function()
    {
        var elt = this.get_element();

        this._okBtn = $HGDomElement.createElementFromTemplate(
                {
                    nodeName : "input",
                    properties : { type : "button", value : "确 定" },
                    cssClasses:["button"],
                    events : this._okBtnEvents
                },
                elt
            );
        
        this._cancelBtn = $HGDomElement.createElementFromTemplate(
                {
                    nodeName : "input",
                    properties : { type : "button", value : "取 消" },
                    cssClasses:["button"],
                    events : this._cancelBtnEvents
                },
                elt
            );
    },
    
    _loadDialogArgument : function()
    {
        if (this._dialogArg && this._dialogArg.initData)
        {
            this._dataControl.set_text(this._dialogArg.initData.text);
        }
    },

    _onOkBtnClick : function()
    {
        var text = this._dataControl.get_text();
        this._dialogArg.returnData = {command:"OK", text:text};
        window.close();
    },

    _onCancelBtnClick : function()
    {
        window.close();
    },


    set_dataControlID : function(value)
    {
       this._dataControlID = value;
    },

    get_dataControlID : function()
    {        
        return this._dataControlID;
    }, 
 
    set_dataPropertyName : function(value)
    {
       this._dataPropertyName = value; 
    },
    
    get_dataPropertyName : function()
    {
        return this._dataPropertyName;
    }  
}

$HGRootNS.DialogSampleControl.registerClass($HGRootNSName + ".DialogSampleControl", $HGRootNS.ControlBase);
