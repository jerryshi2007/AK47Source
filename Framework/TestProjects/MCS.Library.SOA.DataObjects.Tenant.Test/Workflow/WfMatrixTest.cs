using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MCS.Library.SOA.DataObjects.Tenant.Test.Workflow
{
    [TestClass]
    public class WfMatrixTest
    {
        private int _matrixRowNum = 15;
		private int _matrixColumnNum = 10;
		private WfMatrix _wfMatrixInstance = null;
		private bool _isWfMatrixDefExist = false;

        public WfMatrixTest()
		{
		}

		[TestInitialize]
		public void Setup()
		{
			WfMatrixAdapter.Instance.ClearAll();
			WfMatrixDefinitionAdapter.Instance.ClearAll();

			_wfMatrixInstance = BuildWfMatrix();
			_isWfMatrixDefExist = CheckWfMatrixDef(_wfMatrixInstance.Definition.Key);
		}

		[TestCleanup]
		public void TearDown()
		{
			//WfMatrixAdapter.Instance.Delete(_wfMatrixInstance.MatrixID);

			//if (_isWfMatrixDefExist == false)
			//{
			//    WfMatrixDefinitionAdapter.Instance.Delete(_wfMatrixInstance.Definition);
			//}
		}

		[TestMethod]
		public void Test()
		{
			WfMatrixQueryParamCollection queryParams = new WfMatrixQueryParamCollection("957e9597-b7f7-4ffa-ad77-2dbf10ac3497");
			queryParams.Add(new WfMatrixQueryParam()
			{
				QueryName = "支付方式",
				QueryValue = "网银"
			});

			queryParams.Add(new WfMatrixQueryParam()
			{
				QueryName = "成本中心",
				QueryValue = "成1"
			});
			queryParams.Add(new WfMatrixQueryParam()
			{
				QueryName = "部门",
				QueryValue = "商务部"
			});

			var matrix = WfMatrixAdapter.Instance.Query(queryParams);

		}

		[TestMethod]
		public void UpdateTest()
		{
			WfMatrixAdapter.Instance.Update(_wfMatrixInstance);
		}

		[TestMethod]
		public void LoadTest()
		{
			UpdateTest();

			var matrixCollection = WfMatrixAdapter.Instance.Load(p => p.AppendItem("MATRIX_ID", _wfMatrixInstance.MatrixID));
			Assert.IsTrue(matrixCollection.Count == 1);

			Assert.AreEqual(_wfMatrixInstance.CreatorID, matrixCollection[0].CreatorID);
		}

		[TestMethod]
		public void DeleteTest()
		{
			WfMatrixAdapter.Instance.Delete(_wfMatrixInstance);
		}

		[TestMethod]
		public void ReplaceTest()
		{
			_wfMatrixInstance.Rows.ReplcaeOperators(WfMatrixOperatorType.Person, "fanhy", "liming");

			WfMatrixAdapter.Instance.Update(_wfMatrixInstance);

			WfMatrix loadMatrix = WfMatrixAdapter.Instance.Load(_wfMatrixInstance.MatrixID);

			foreach (WfMatrixRow row in loadMatrix.Rows)
				Assert.AreEqual(-1, row.Operator.IndexOf("fanhy"));
		}

		[TestMethod]
		public void QueryTest()
		{
			UpdateTest();

			WfMatrixQueryParamCollection dict = new WfMatrixQueryParamCollection(_wfMatrixInstance.MatrixID);

			var wfMatrixQuery = WfMatrixAdapter.Instance.Query(dict);

			Assert.AreEqual(this._matrixRowNum, wfMatrixQuery.Rows.Count);

			dict.Add(new WfMatrixQueryParam()
			{
				QueryName = _wfMatrixInstance.Rows[0].Cells[0].Definition.DimensionKey,
				QueryValue = _wfMatrixInstance.Rows[0].Cells[0].StringValue
			});

			dict.Add(new WfMatrixQueryParam()
			{
				QueryName = _wfMatrixInstance.Rows[0].Cells[1].Definition.DimensionKey,
				QueryValue = _wfMatrixInstance.Rows[0].Cells[1].StringValue
			});

			wfMatrixQuery = WfMatrixAdapter.Instance.Query(dict);

			WfMatrixRowUsersCollection rowUserCollection = wfMatrixQuery.Rows.GenerateRowsUsers();
			foreach (var rowUsers in rowUserCollection)
			{
				Console.WriteLine(rowUsers.Users[0].DisplayName);
				Assert.AreEqual(rowUsers.Users.Count, 1);
			}
			Assert.AreEqual(wfMatrixQuery.Rows.Count, _matrixRowNum);
		}


		public WfMatrix BuildWfMatrix()
		{
			var matrixDef = BuildNewMatrixDef(_matrixColumnNum);

			WfMatrix matrixInstance = new WfMatrix(matrixDef)
			{
				CreatorID = "testCreatorID1",
				CreatorName = "testCreatorName1",
				MatrixID = UuidHelper.NewUuidString(),
				ProcessKey = "testProcessKey",
				ActivityKey = "testActivityKey"
			};

			for (int i = 0; i < _matrixRowNum; i++)
			{
				var row = new WfMatrixRow()
				{
					RowNumber = i,
					OperatorType = WfMatrixOperatorType.Person,
					Operator = "wanhw"
				};

				if (i % 2 == 0)
				{
					row.Operator = "fanhy";
				}

				for (int j = 0; j < matrixDef.Dimensions.Count; j++)
				{
					row.Cells.Add(new WfMatrixCell(matrixDef.Dimensions[j])
					{
						StringValue = "String" + j.ToString()
					});
				}
				matrixInstance.Rows.Add(row);
			}

			return matrixInstance;
		}

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
