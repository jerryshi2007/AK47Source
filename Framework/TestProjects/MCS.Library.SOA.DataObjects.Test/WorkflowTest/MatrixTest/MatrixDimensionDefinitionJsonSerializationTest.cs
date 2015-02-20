using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Test.WorkflowTest.MatrixTest
{
	[TestClass]
	public class MatrixDimensionDefinitionJsonSerializationTest
	{
		private int _matrixColumnNum = 10;

		[TestMethod]
		public void WfMatrixDemensionDefinitionSerilizationTest()
		{
			WfConverterHelper.RegisterConverters();
			JSONSerializerExecute.RegisterConverter(typeof(WfMatrixDefinitionConverter));
			JSONSerializerExecute.RegisterConverter(typeof(WfMatrixDefinitionConverter));
			var matrixDef = WfMatrixDefinitionAdapterTest.BuildNewMatrixDef(_matrixColumnNum);
			string strSerializeMatrixDef = JSONSerializerExecute.Serialize(matrixDef);

			var matrixDef2 = JSONSerializerExecute.Deserialize<WfMatrixDefinition>(strSerializeMatrixDef);

			string strReserializeMatrixDef = JSONSerializerExecute.Serialize(matrixDef2);
			Console.WriteLine(strSerializeMatrixDef);
			Console.WriteLine(strReserializeMatrixDef);
			Assert.AreEqual(strSerializeMatrixDef, strReserializeMatrixDef);
			
		}
	}
}
