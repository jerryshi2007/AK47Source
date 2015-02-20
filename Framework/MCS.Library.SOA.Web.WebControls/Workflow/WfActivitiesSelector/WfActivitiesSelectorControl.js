
$HBRootNS.WfActivitiesSelectorControl = function(element)
{
	$HBRootNS.WfActivitiesSelectorControl.initializeBase(this, [element]);
}

$HBRootNS.WfActivitiesSelectorControl.prototype =
{
	initialize : function()
    {
		this._initialize();
        $HBRootNS.WfActivitiesSelectorControl.callBaseMethod(this, 'initialize');
    },
    
    dispose : function()
    {
        $HBRootNS.WfActivitiesSelectorControl.callBaseMethod(this, 'dispose');
    },

	_initialize : function()
	{
	},

	//Render End

	_pseudo : function()
	{
	}
}

$HBRootNS.WfActivitiesSelectorControl.registerClass($HBRootNSName + ".WfActivitiesSelectorControl", $HBRootNS.DialogControlBase);