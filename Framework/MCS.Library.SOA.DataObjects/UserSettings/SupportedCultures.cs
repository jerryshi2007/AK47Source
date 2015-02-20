using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	public enum SupportedCultures
	{
		[EnumItemDescription("简体中文", ShortName = "zh-CN")]
		ZH_CN = 2052,

		[EnumItemDescription("English (United States)", ShortName="en-US")]
		EN_US = 1033
	}
}
