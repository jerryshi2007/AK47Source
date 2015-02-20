using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using MCS.Library.SOA.DocServiceContract;
using System.ServiceModel.Activation;
using MCS.Library.Office.OpenXml.Word;

namespace MCS.Library.Services
{
	// 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“DCS_DocumentBuilderService”。
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class DCSDocumentBuilderService : IDCSDocumentBuilderService
	{
		public byte[] DCMBuildDocument(string templateUri, DCTWordDataObject wordData)
		{
			using (DocLibContext context = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
			{
				byte[] templateBinary = context.OpenBinary(templateUri);
				
				return WordEntry.GenerateDocument(templateBinary, wordData);
			}
		}


		public string Hello()
		{
			return "Hello World!";
		}
	}
}
