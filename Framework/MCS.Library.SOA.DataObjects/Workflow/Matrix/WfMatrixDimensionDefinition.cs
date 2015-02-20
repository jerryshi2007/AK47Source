using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	[Serializable]
	[ORTableMapping("WF.MATRIX_DIMENSION_DEFINITION")]
	public class WfMatrixDimensionDefinition
	{
		[ORFieldMapping("MATRIX_DEF_KEY", PrimaryKey = true)]
		public string MatrixKey
		{
			get;
			set;
		}

		[ORFieldMapping("DIMENSION_KEY", PrimaryKey = true)]
		public string DimensionKey
		{
			get;
			set;
		}

		[ORFieldMapping("SORT_ORDER")]
		public int SortOrder
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

		[ORFieldMapping("DESCRIPTION")]
		public string Description
		{
			get;
			set;
		}

		private PropertyDataType _DateType = PropertyDataType.String;

		[ORFieldMapping("DATA_TYPE")]
		public PropertyDataType DataType
		{
			get
			{
				return this._DateType;
			}
			set
			{
				this._DateType = value;
			}
		}
	}

	/// <summary>
	/// Matrix中的维度集合（仅在一个Matrix中，Key是DemensionKey
	/// </summary>
	[Serializable]
	public class WfMatrixDimensionDefinitionCollection : EditableKeyedDataObjectCollectionBase<string, WfMatrixDimensionDefinition>
	{
		protected override string GetKeyForItem(WfMatrixDimensionDefinition item)
		{
			return item.DimensionKey;
		}
	}

}
