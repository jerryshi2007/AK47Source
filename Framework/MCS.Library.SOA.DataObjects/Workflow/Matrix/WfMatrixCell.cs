using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	[Serializable]
	public class WfMatrixCell
	{
		public WfMatrixCell()
		{
		}

		public WfMatrixCell(WfMatrixDimensionDefinition dd)
		{
			dd.NullCheck("dd");

			Definition = dd;
		}

		public WfMatrixDimensionDefinition Definition
		{
			get;
			internal set;
		}

		public string StringValue
		{
			get;
			set;
		}
	}

	[Serializable]
	public class WfMatrixCellCollection : EditableDataObjectCollectionBase<WfMatrixCell>
	{
		public WfMatrixCell FindByDefinitionKey(string key)
		{
			key.NullCheck("key");

			WfMatrixCell result = null;

			foreach (WfMatrixCell cell in this)
			{
				if (cell.Definition == null)
					continue;

				if (string.Compare(cell.Definition.DimensionKey, key, true) == 0)
				{
					result = cell;
					break;
				}
			}

			return result;
		}

		public T GetValue<T>(string key, T defaultValue)
		{
			WfMatrixCell cell = FindByDefinitionKey(key);

			T result = defaultValue;

			if (cell != null)
				result = (T)DataConverter.ChangeType<object>(cell.StringValue, typeof(T));

			return result;
		}
	}
}
