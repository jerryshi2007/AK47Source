
$HGRootNS.ShowModalDialogSampleControl = function(element)
{
    $HGRootNS.ShowModalDialogSampleControl.initializeBase(this, [element]);
    this._showResultSpan = null;
    this._showDialogBtn = null;
    this._dialogControlUrl = null;
    this._text = null;
    this._showDialogBtnEvents = 
        {
            click : Function.createDelegate(this, this._showDialogBtnClick)
        };
 }

$HGRootNS.ShowModalDialogSampleControl.prototype = 
{
    initialize : function()
    {
        $HGRootNS.ShowModalDialogSampleControl.callBaseMethod(this, 'initialize');
        this._buildControl();
        this._loadData();
    },
    
    dispose : function()
    {
        $HGDomEvent.removeHandlers(this._showDialogBtn, this._showDialogBtnEvents);
        this._showDialogBtn = null;
        
        $HGRootNS.ShowModalDialogSampleControl.callBaseMethod(this, 'dispose');
    },
    
    get_dialogControlUrl : function()
    {
        return this._dialogControlUrl;
    },
    
    set_dialogControlUrl : function(value)
    {
        this._dialogControlUrl = value;
    },
    
    set_text : function(value)
    {
       this._text = value;
       if (this._input)
            this._input.value = value;
    },

    get_text : function()
    {        
        return this._text;
    }, 
    
    _buildControl : function()
    {
        var elt = this.get_element();
        
        this._showResultSpan= $HGDomElement.createElementFromTemplate(
                {
                    nodeName : "span"
                },
                elt
            );      

        this._showDialogBtn = $HGDomElement.createElementFromTemplate(
                {
                    nodeName : "input",
                    properties : { type : "button", value : "弹出" },
                    cssClasses:["button"],
                    events : this._showDialogBtnEvents
                },
                elt
            );      
       
    },

    _showDialogBtnClick : function()
    {
        var arg = {};
        arg.initData = {};
        arg.initData.text = this._text;

        //通过arg传入参数
        window.showModalDialog(this._dialogControlUrl, arg);
        
        var result = arg.returnData;
        
        if (result)
        {
            switch (result.command)
            {
                case "OK":
                    this._text = result.text;
                    this._loadData();
                    break;
            }
        }
    },
    
    _loadData : function()
    {
        this._showResultSpan.innerText = this._text;
    }
}

$HGRootNS.ShowModalDialogSampleControl.registerClass($HGRootNSName + ".ShowModalDialogSampleControl", $HGRootNS.ControlBase);
