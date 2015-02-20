using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace MCS.Web.WebControls
{
    public class WfParameterValidationControl : CustomValidator
    {
        internal WfCollectParameterControl CollectParameterControl
        {
            get;
            set;
        }

        internal WfParameterValidationControl()
        {
            this.ValidateEmptyText = true;
            this.Enabled = true;
        }

        internal WfParameterValidationControl(WfCollectParameterControl cpcontrol)
            : this()
        {
            this.CollectParameterControl = cpcontrol;
        }

        protected override bool EvaluateIsValid()
        {
            if (this.CollectParameterControl != null && this.CollectParameterControl.Enabled && this.CollectParameterControl.AutoCollectDataWhenPostBack)
                this.CollectParameterControl.CollectData();

            return true;
        }


    }
}
