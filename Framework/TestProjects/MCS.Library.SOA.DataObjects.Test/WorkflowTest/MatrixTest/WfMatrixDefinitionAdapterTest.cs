using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Test.WorkflowTest.MatrixTest
{
    /// <summary>
    /// 已经迁移到MCS.Library.SOA.DataObjects.Tenant.Test
    /// </summary>
	[TestClass]
	public class WfMatrixDefinitionAdapterTest
	{
		private int _matrixColumnNum = 10;

		[TestMethod]
		[ExpectedException(typeof(SystemSupportException))]
		public void WfMatrixDefinitionAdapterUpateTest()
		{
			var matrixDef = BuildNewMatrixDef(_matrixColumnNum);

			WfMatrixDefinitionAdapter.Instance.Update(matrixDef);

			var matrixDefInDB = WfMatrixDefinitionAdapter.Instance.Load(matrixDef.Key);

			Assert.AreEqual(matrixDef.Description, matrixDefInDB.Description);
			Assert.AreEqual(matrixDef.Dimensions.Count, matrixDefInDB.Dimensions.Count);

			WfMatrixDefinitionAdapter.Instance.Delete(matrixDefInDB);

			var deletedMatrix = WfMatrixDefinitionAdapter.Instance.Load(matrixDef.Key);

			Assert.Fail("异常未抛出");
		}

		public static bool CheckWfMatrixDef(string matrixDefKey)
		{
			try
			{
				WfMatrixDefinitionAdapter.Instance.Load(matrixDefKey);
				return true;
			}
			catch (SystemSupportException)
			{
				return false;
			}
		}

		public static WfMatrixDefinition BuildNewMatrixDef(int columnNum)
		{
			var matrix = new WfMatrixDefinition()
			{
				Key = Guid.NewGuid().ToString(),
				Name = "testName",
				Description = "testDesc",
				Enabled = true
			};

			for (int i = 0; i < columnNum; i++)
			{
				var suffixStr = i.ToString();
				matrix.Dimensions.Add(new WfMatrixDimensionDefinition()
				{
					MatrixKey = matrix.Key,
					DimensionKey = "Key" + suffixStr,
					Name = "Name" + suffixStr,
					Description = "Description" + suffixStr,
					SortOrder = i,
					DataType = PropertyDataType.String
				});
			}

			return matrix;
		}
	}
}
