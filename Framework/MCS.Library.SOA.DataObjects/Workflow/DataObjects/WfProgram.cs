using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	[Serializable]
	[ORTableMapping("WF.PROGRAMS")]
	public class WfProgram
	{
		[ORFieldMapping("APPLICATION_CODE_NAME", PrimaryKey = true)]
		public string ApplicationCodeName
		{
			get;
			set;
		}

		[ORFieldMapping("CODE_NAME", PrimaryKey = true)]
		public string CodeName
		{
			get;
			set;
		}

		[ORFieldMapping("NAME")]
		public string Name
		{
			get;
			set;
		}

		[ORFieldMapping("SORT")]
		public int Sort
		{
			get;
			set;
		}
	}

	/// <summary>
	/// 一个应用下的Program集合
	/// </summary>
	[Serializable]
	public class WfProgramInApplicationCollection : SerializableEditableKeyedDataObjectCollectionBase<string, WfProgram>
	{
		public WfProgramInApplicationCollection()
		{
		}

		protected WfProgramInApplicationCollection(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}

		protected override string GetKeyForItem(WfProgram item)
		{
			return item.CodeName;
		}
	}
}
