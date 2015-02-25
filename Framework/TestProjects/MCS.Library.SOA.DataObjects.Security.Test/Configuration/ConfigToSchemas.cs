using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;

namespace MCS.Library.SOA.DataObjects.Security.Test.Configuration
{
	/// <summary>
	/// 本测试的目的是测试从配置信息中加载Schema相关的属性信息，以及最终生成对象的属性集合。
	/// 凡是带setting结尾的，测试的都是配置信息的直接加载；否则是从配置信息加载成对象的测试。
	/// </summary>
	[TestClass]
	public class ConfigToSchemas
	{
		[TestMethod]
		[Description("加载组的属性集合，生成对应的配置信息类")]
		[TestCategory(Constants.ConfigurationCategory)]
		public void LoadGroupPropertiesDefineSettings()
		{
			SchemaPropertyGroupSettings settings = SchemaPropertyGroupSettings.GetConfig();

			Console.WriteLine("Group count = {0}", settings.Groups.Count);
			Assert.IsTrue(settings.Groups.Count > 0);

			settings.Groups.ForEach<SchemaPropertyGroupConfigurationElement>(g => ConfigurationOutputHelper.Output(g, Console.Out));
		}

		[TestMethod]
		[Description("加载组的属性集合，生成对应的PropertyDefine集合。测试SchemaPropertyDefineCollection的加载逻辑")]
		[TestCategory(Constants.ConfigurationCategory)]
		public void LoadPropertiesDefine()
		{
			SchemaPropertyGroupSettings settings = SchemaPropertyGroupSettings.GetConfig();

			foreach (SchemaPropertyGroupConfigurationElement group in settings.Groups)
			{
				SchemaPropertyDefineCollection propertiesDefine = new SchemaPropertyDefineCollection(group.AllProperties);

				Console.WriteLine("Group name: {0}", group.Name);

				propertiesDefine.ForEach(pd => pd.Output(Console.Out, 1));
			}
		}

		[TestMethod]
		[Description("加载所有对象的属性集合，生成对应的配置信息类")]
		[TestCategory(Constants.ConfigurationCategory)]
		public void LoadObjectSchemasSettings()
		{
			ObjectSchemaSettings settings = ObjectSchemaSettings.GetConfig();

			foreach (ObjectSchemaConfigurationElement schema in settings.Schemas)
			{
				schema.Output(Console.Out);
			}
		}

		[TestMethod]
		[Description("加载所有对象的属性集合，生成SchemaDefine类")]
		[TestCategory(Constants.ConfigurationCategory)]
		public void LoadObjectSchemas()
		{
			ObjectSchemaSettings settings = ObjectSchemaSettings.GetConfig();

			SchemaDefineCollection schemas = new SchemaDefineCollection(settings.Schemas);

			schemas.ForEach(s => s.Output(Console.Out));
		}

		[TestMethod]
		[Description("加载所有Schema定义，生成SchemaInfo类")]
		[TestCategory(Constants.ConfigurationCategory)]
		public void LoadSchemaInfo()
		{
			SchemaInfoCollection sic = new SchemaInfoCollection(ObjectSchemaSettings.GetConfig().Schemas);

			SchemaInfoCollection usersInfo = sic.FilterByCategory("Users");

			Assert.IsTrue(usersInfo.Count > 0);
		}

		[TestMethod]
		[Description("加载所有Schema定义，过滤去其中非关系的对象信息")]
		[TestCategory(Constants.ConfigurationCategory)]
		public void LoadBaseObjectSchemaInfo()
		{
			SchemaInfoCollection sic = new SchemaInfoCollection(ObjectSchemaSettings.GetConfig().Schemas);

			SchemaInfoCollection schemasInfo = sic.FilterByNotRelation();

			schemasInfo.ForEach(s => s.Output(Console.Out));

			Assert.IsTrue(schemasInfo.Count > 0);
		}

		[TestMethod]
		[Description("加载Schema的性能测试")]
		[TestCategory(Constants.ConfigurationCategory)]
		public void GetSchemaPerformanceTest()
		{
			var schemaElem = ObjectSchemaSettings.GetConfig().Schemas[0];

			Stopwatch sw = new Stopwatch();

			sw.Start();
			try
			{
				for (int i = 0; i < 100000; i++)
				{
					SchemaDefine schema = SchemaDefine.GetSchema(schemaElem.Name);
				}
			}
			finally
			{
				sw.Stop();

				Console.WriteLine("经过时间{0:#,##0}毫秒", sw.ElapsedMilliseconds);
			}
		}

		[TestMethod]
		[Description("创建对象的性能测试")]
		[TestCategory(Constants.ConfigurationCategory)]
		public void CreateSchemaObjectPerformanceTest()
		{
			var schemaElem = ObjectSchemaSettings.GetConfig().Schemas[0];

			Stopwatch sw = new Stopwatch();

			sw.Start();
			try
			{
				for (int i = 0; i < 100000; i++)
				{
					SchemaObjectBase obj = SchemaExtensions.CreateObject(schemaElem.Name);
					Assert.IsTrue(obj.Properties != null);
				}
			}
			finally
			{
				sw.Stop();

				Console.WriteLine("经过时间{0:#,##0}毫秒", sw.ElapsedMilliseconds);
			}
		}
	}
}
