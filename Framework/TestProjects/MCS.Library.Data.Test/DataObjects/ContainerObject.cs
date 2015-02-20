using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using MCS.Library.Core;

namespace MCS.Library.Data.Test.DataObjects
{
	[ORTableMapping("CONTAINER_TABLE")]
	[Serializable]
	public class ContainerObject
	{
		[ORFieldMapping("ID", PrimaryKey = true)]
		public string ID
		{
			get;
			set;
		}

		[SubClassORFieldMapping("ID", "SUB_ID")]
		[SubClassORFieldMapping("Amount", "SUB_AMOUNT")]
		[SubClassPropertyEncryption("Amount", "")]
		//[SubClassORFieldMapping("Code", "Code")]
		//[SubClassORFieldMapping("SalarySchemaType", "SalarySchemaTypeCode")]
		//[SubClassORFieldMapping("SalaryLevelCode", "SalaryLevelCode")]
		//[SubClassORFieldMapping("SalaryGradeCode", "SalaryGradeCode")]
		//[SubClassORFieldMapping("SalaryValue", "BeforeAdjustSalaryValue")]
		//[SubClassPropertyEncryptionAttribute("SalaryValue", "")]
		public TestObject SubObject
		{
			get;
			set;
		}
	}
}
