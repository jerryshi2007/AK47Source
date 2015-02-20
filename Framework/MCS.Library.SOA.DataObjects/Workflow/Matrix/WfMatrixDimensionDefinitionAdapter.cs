using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfMatrixDimensionDefinitionAdapter :
		UpdatableAndLoadableAdapterBase<WfMatrixDimensionDefinition, WfMatrixDimensionDefinitionCollection>
	{
		public static readonly WfMatrixDimensionDefinitionAdapter Instance = new WfMatrixDimensionDefinitionAdapter();

		private WfMatrixDimensionDefinitionAdapter()
		{
		}

		protected override string GetConnectionName()
		{
			return WorkflowSettings.GetConfig().ConnectionName;
		}

		public WfMatrixDimensionDefinition Load(string matrixDefKey, string dimensionDefKey)
		{
			dimensionDefKey.NullCheck("dimensionDefKey");
			matrixDefKey.NullCheck("matrixDefKey");

			WfMatrixDimensionDefinitionCollection dimensionDefList = Load(builder =>
			{
				builder.AppendItem("DIMENSION_KEY", dimensionDefKey);
				builder.AppendItem("MATRIX_DEF_KEY", matrixDefKey);
			});

			(dimensionDefList.Count > 0).FalseThrow("不能找到DIMENSION_KEY为{1}的记录", dimensionDefKey);

			return dimensionDefList[0];
		}
	}
}
