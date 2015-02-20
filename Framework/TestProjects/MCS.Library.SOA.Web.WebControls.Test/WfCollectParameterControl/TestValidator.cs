using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.WfCollectParameterControl
{
	public class TestValidator : CustomValidator
	{
		protected override bool OnServerValidate(string value)
		{
			return false;
		}
	}
}