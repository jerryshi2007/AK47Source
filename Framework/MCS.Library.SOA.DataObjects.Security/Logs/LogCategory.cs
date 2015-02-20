using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Security
{
	[Serializable]
	public class LogCategory
	{
		[ORFieldMapping("Category")]
		public string Category { get; set; }

		[ORFieldMapping("OperationType")]
		public string OperationType
		{
			get
			{
				return this.EnumValue.ToString();
			}

			set
			{
				EnumValue = (MCS.Library.SOA.DataObjects.Security.Actions.SCOperationType)Enum.Parse(typeof(MCS.Library.SOA.DataObjects.Security.Actions.SCOperationType), value);
			}
		}

		public MCS.Library.SOA.DataObjects.Security.Actions.SCOperationType EnumValue { get; set; }

		public string Description
		{
			get
			{
				return EnumItemDescriptionAttribute.GetAttribute(this.EnumValue).Description;
			}
		}
	}

	public class LogCategoryCollection : EditableDataObjectCollectionBase<LogCategory>
	{

	}
}
