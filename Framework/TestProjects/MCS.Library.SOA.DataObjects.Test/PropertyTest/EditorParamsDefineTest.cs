using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Test.PropertyTest
{
	[TestClass]
	public class EditorParamsDefineTest
	{
		[TestMethod]
		public void EditorParamsDefineJSONSerializerTest()
		{
			RegisterConverter();

			EditorParamsDefine epd = PrepareEditorParamsDefine();

			string result = JSONSerializerExecute.Serialize(epd);
			Console.WriteLine(result);

			EditorParamsDefine deserializedEpd = JSONSerializerExecute.Deserialize<EditorParamsDefine>(result);

			string reSerialized = JSONSerializerExecute.Serialize(deserializedEpd);

			Assert.AreEqual(result, reSerialized);
		}

		[TestMethod]
		public void DestTest()
		{
			//string joson1 = "{\"cloneControlID\":\"ImageUploaderPropertyEditor_ImageUploader\",\"serverControlProperties\": [{\"propertyName\":\"Width\",\"stringValue\":\"200\"},{\"propertyName\":\"ImageWidth\",\"stringValue\":\"200\"},{\"propertyName\":\"Height\",\"stringValue\":\"300\"},{\"propertyName\":\"ImageHeight\",\"stringValue\":\"350\"}]\"}";
			string josn = "{\"cloneControlID\":\"ImageUploaderPropertyEditor_ImageUploader\",\"serverControlProperties\":\"[{\\\"propertyName\\\":\\\"Width\\\",\\\"stringValue\\\":\\\"200\\\"},{\\\"propertyName\\\":\\\"ImageWidth\\\",\\\"stringValue\\\":\\\"200\\\"},{\\\"propertyName\\\":\\\"Height\\\",\\\"stringValue\\\":\\\"300\\\"},{\\\"propertyName\\\":\\\"ImageHeight\\\",\\\"stringValue\\\":\\\"350\\\"}]\"}";

			//string jsonTest = "{\"cloneControlID\":\"ImageUploaderPropertyEditor_ImageUploader\",\"serverControlProperties\": \"[{\"propertyName\":\"ImageWidth\",\"stringValue\":\"200\"}]\"}";

			EditorParamsDefine deserializedEpd = JSONSerializerExecute.Deserialize<EditorParamsDefine>(josn);

		}

		private static void RegisterConverter()
		{
			JSONSerializerExecute.RegisterConverter(typeof(EditorParamsDefineConverter));
			JSONSerializerExecute.RegisterConverter(typeof(ControlPropertyDefineConverter));
		}

		private static EditorParamsDefine PrepareEditorParamsDefine()
		{
			EditorParamsDefine define = new EditorParamsDefine();
			//define.EnumTypeName= "MCS.Library.SOA.Web.WebControls.Test.PropertyGrid.EnumValueDefine, MCS.Library.SOA.Web.WebControls.Test";
			define.CloneControlID = "ImageUploaderPropertyEditor_ImageUploader";
			define.ServerControlProperties.Add(new ControlPropertyDefine() { DataType = PropertyDataType.String, PropertyName = "Width", StringValue = "200" });
			define.ServerControlProperties.Add(new ControlPropertyDefine() { DataType = PropertyDataType.Decimal, PropertyName = "ImageWidth", StringValue = "200" });

			define.ServerControlProperties.Add(new ControlPropertyDefine() { DataType = PropertyDataType.String, PropertyName = "Height", StringValue = "300" });
			define.ServerControlProperties.Add(new ControlPropertyDefine() { DataType = PropertyDataType.String, PropertyName = "ImageHeight", StringValue = "350" });

			return define;
		}
	}
}
