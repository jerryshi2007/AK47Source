using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace PermissionCenter
{
	public class SubTemplateField : TemplateField
	{
		protected override DataControlField CreateField()
		{
			return new SubTemplateField();
		}

		public override void ExtractValuesFromCell(System.Collections.Specialized.IOrderedDictionary dictionary, DataControlFieldCell cell, DataControlRowState rowState, bool includeReadOnly)
		{
			if (rowState == DataControlRowState.Normal)
			{

			}

			base.ExtractValuesFromCell(dictionary, cell, rowState, includeReadOnly);
		}

	}
}