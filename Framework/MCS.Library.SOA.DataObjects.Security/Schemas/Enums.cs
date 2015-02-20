using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Security
{
	/// <summary>
	/// 组织机构类别
	/// </summary>
	public enum OrganizationClass
	{
		[EnumItemDescription("[未指定]", SortId = 1)]
		None = 0,

		[EnumItemDescription("总部编制", SortId = 2)]
		HQStrengths = 16,

		[EnumItemDescription("大区编制", SortId = 3)]
		LargeRegionStrengths = 32,

		[EnumItemDescription("分部编制", SortId = 4)]
		ExpansionPartStrengths = 64,

		[EnumItemDescription("二级地区编制", SortId = 5)]
		SubLevelStrengths = 128,

		[EnumItemDescription("门店编制", SortId = 6)]
		StoreStrengths = 256,
	}

	/// <summary>
	/// 机构属性
	/// </summary>
	public enum OrganizationType
	{
		[EnumItemDescription("[未指定]", SortId = 1)]
		None = 0,

		[EnumItemDescription("虚拟机构", SortId = 2)]
		Virtual = 1,

		[EnumItemDescription("总部", SortId = 3)]
		Headquarters = 2,

		[EnumItemDescription("大区", SortId = 4)]
		ExpansionPartStrengths = 4,

		[EnumItemDescription("分部", SortId = 5)]
		Branch = 8,

		[EnumItemDescription("一级门店", SortId = 6)]
		TopStore = 16,

		[EnumItemDescription("二级门店", SortId = 6)]
		SecondaryStore = 32,

		[EnumItemDescription("一级职能", SortId = 6)]
		TopMission = 64,

		[EnumItemDescription("二级职能", SortId = 6)]
		SecondaryMission = 64,
	}
}
