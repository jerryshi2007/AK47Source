using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.Data.Test.DataObjects;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;
using System.Data;

namespace MCS.Library.Data.Test
{
	[TestClass]
	public class PropertyEncryptionTest
	{
		[TestMethod]
		public void EncryptAmountProperty()
		{
			TestObject data = CreateTestObject();

			string sql = ORMapping.GetInsertSql(data, TSqlBuilder.Instance);

			Console.WriteLine(sql);

			Assert.AreEqual(-1, sql.IndexOf(data.ToString()));
		}

		[TestMethod]
		public void EncryptSubAmountProperty()
		{
			ContainerObject data = CreateContainerObject();

			string sql = ORMapping.GetInsertSql(data, TSqlBuilder.Instance);

			Console.WriteLine(sql);

			Assert.AreEqual(-1, sql.IndexOf(data.ToString()));
		}

		[TestMethod]
		public void DecryptAmountProperty()
		{
			DataTable table = PrepareTestTable();

			TestObject data = new TestObject();

			ORMapping.DataRowToObject(table.Rows[0], data);

			Console.WriteLine(data.Amount);

			Assert.AreEqual(240000, data.Amount);
		}

		[TestMethod]
		public void DecryptSubAmountProperty()
		{
			DataTable table = PrepareContainerTable();

			ContainerObject data = new ContainerObject();

			ORMapping.DataRowToObject(table.Rows[0], data);

			Console.WriteLine(data.SubObject.Amount);

			Assert.AreEqual(240000, data.SubObject.Amount);
		}

		private static TestObject CreateTestObject()
		{
			TestObject data = new TestObject();

			data.ID = UuidHelper.NewUuidString();
			data.Name = "Hello";
			//data.Amount = 2500;
			data.Amount = 240000;

			return data;
		}

		private static ContainerObject CreateContainerObject()
		{
			ContainerObject result = new ContainerObject();

			result.ID = UuidHelper.NewUuidString();
			result.SubObject = CreateTestObject();

			return result;
		}

		private DataTable PrepareContainerTable()
		{
			DataTable table = new DataTable();

			table.Columns.Add("ID");
			table.Columns.Add("SUB_ID");
			table.Columns.Add("SUB_AMOUNT");

			DataRow row = table.NewRow();

			row["ID"] = UuidHelper.NewUuidString();
			row["SUB_ID"] = UuidHelper.NewUuidString();
			row["SUB_AMOUNT"] = "849b3d75e892b66e";	//240000
			table.Rows.Add(row);

			return table;
		}

		private DataTable PrepareTestTable()
		{
			DataTable table = new DataTable();

			table.Columns.Add("ID");
			table.Columns.Add("NAME");
			table.Columns.Add("AMOUNT");

			DataRow row = table.NewRow();

			row["ID"] = UuidHelper.NewUuidString();
			row["NAME"] = UuidHelper.NewUuidString();
			//row["AMOUNT"] = "c3d8d1af3f41d3ef";	//2500
			row["AMOUNT"] = "849b3d75e892b66e";	//240000
			table.Rows.Add(row);

			return table;
		}
	}
}
