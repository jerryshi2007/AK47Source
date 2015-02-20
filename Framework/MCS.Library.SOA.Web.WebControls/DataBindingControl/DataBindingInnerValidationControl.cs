using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace MCS.Web.WebControls
{
	public class DataBindingInnerValidationControl : CustomValidator
	{
		public DataBindingControl BindingControl
		{
			get;
			set;
		}

		public DataBindingInnerValidationControl()
		{
		}

		public DataBindingInnerValidationControl(DataBindingControl bindingControl)
		{
			this.BindingControl = bindingControl;
		}

		protected override bool EvaluateIsValid()
		{
			if (this.BindingControl != null && this.BindingControl.Data != null && this.BindingControl.AutoCollectDataWhenPostBack)
				this.BindingControl.CollectData(false);

			return true;
		}
	}
}
