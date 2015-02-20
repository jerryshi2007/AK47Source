using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;

namespace MCS.Library.SOA.Web.WebControls.Test.PropertyGrid
{
	public enum EnumValueDefine
	{
		[EnumItemDescription("老A")]
		AA = 0,

		[EnumItemDescription("老B")]
		BB,

		[EnumItemDescription("老C")]
		CC,

		DD
	}
}