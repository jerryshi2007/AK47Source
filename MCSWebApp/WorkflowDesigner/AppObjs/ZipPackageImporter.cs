using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Xml.Linq;
using System.Text;
using System.IO.Packaging;

using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Office.SpreadSheet;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Principal;
using System.Transactions;
using MCS.Library.Data;
using MCS.Library.Core;

namespace WorkflowDesigner
{
	/// <summary>
	/// 导入流中ZIP文件中的流程模板
	/// </summary>
    [Obsolete]
	public class ZipPackageImporter
	{
		public static readonly int StepCount = 6;
		private XElementFormatter XmlFormatter { get; set; }
		private PackagePartCollection PackageParts { get; set; }
		private List<PackagePart> MatrixDefParts { get; set; }
		private List<PackagePart> MatrixParts { get; set; }
		private List<PackagePart> ProcessDescParts { get; set; }
		private Dictionary<string, PackageRelationMapping> MappingInfo { get; set; }
		private Dictionary<string, WfMatrixDefinition> MatrixDefinitions { get; set; }
		private Stream ImportStream { get; set; }

		public event Action<string> NotifyEveryStep;

		public ZipPackageImporter(Stream importStream)
		{
			this.ImportStream = importStream;
			this.XmlFormatter = new XElementFormatter();
			XmlFormatter.OutputShortType = false;

			MatrixDefParts = new List<PackagePart>();
			MatrixParts = new List<PackagePart>();
			ProcessDescParts = new List<PackagePart>();
			MappingInfo = new Dictionary<string, PackageRelationMapping>();
			MatrixDefinitions = new Dictionary<string, WfMatrixDefinition>();
		}

		public void Import()
		{
			using (var package = ZipPackage.Open(this.ImportStream))
			{
				OnNotifyEveryStep("开始导入文件...\n");
				this.PackageParts = package.GetParts();

				CatalogZipParts();
				ParseMappings();
				ParseMatrixDefinitions();

				OnNotifyEveryStep(string.Format("	共发现{0}个矩阵定义，{1}个矩阵数据，{2}个流程模板文件...\n"
					, this.MatrixDefParts.Count, this.MatrixParts.Count, this.ProcessDescParts.Count));

				using (TransactionScope tran = TransactionScopeFactory.Create())
				{
					SaveMatrix();
					OnNotifyEveryStep("	导入矩阵完成...\n");
					SaveProcessDescription();
					OnNotifyEveryStep("	导入流程模板完成...\n");
					tran.Complete();
				}
				OnNotifyEveryStep("文件导入完成!");
				this.Reset();
			}
		}

		private void OnNotifyEveryStep(string info)
		{
			if (this.NotifyEveryStep != null)
			{
				NotifyEveryStep(info);
			}
		}

		private void ParseMatrixDefinitions()
		{
			foreach (var part in MatrixDefParts)
			{
				var xDoc = XDocument.Load(part.GetStream());
				var matrixDef = (WfMatrixDefinition)XmlFormatter.Deserialize(xDoc.Root);

				MatrixDefinitions.Add(matrixDef.Key, matrixDef);
			}
		}

		private void SaveProcessDescription()
		{
			foreach (var part in ProcessDescParts)
			{
				var xmlDoc = XDocument.Load(part.GetStream());
				IWfProcessDescriptor wfProcessDesc = (IWfProcessDescriptor)XmlFormatter.Deserialize(xmlDoc.Root);
				WfProcessDescHelper.SaveWfProcess(wfProcessDesc);
			}
		}

		private void SaveMatrix()
		{
			foreach (var part in MatrixParts)
			{
				var partMapInfo = this.MappingInfo[part.Uri.ToString()];
				var matrixDef = this.MatrixDefinitions[partMapInfo.MatrixDefID];

				WfMatrix.ImportNewMatrixFromExcel2007(part.GetStream(), null, partMapInfo.ProcessDescriptionID, matrixDef);
			}
		}

		private void ParseMappings()
		{
			var mapPart = this.PackageParts.FirstOrDefault(p => p.Uri.ToString().EndsWith(ZipPackageCommon.WF_MAPPING));
			if (mapPart == null)
			{
				return;
			}

			using (var stream = mapPart.GetStream())
			{
				XDocument doc = XDocument.Load(stream);
				foreach (XElement node in doc.Root.Nodes())
				{
					var matrixID = node.Attribute("matrixID").Value;
					this.MappingInfo.Add(matrixID, new PackageRelationMapping()
					{
						MatrixDefID = node.Attribute("matrixDefID").Value,
						MatrixPath = matrixID,
						ProcessDescriptionID = node.Attribute("processDescID").Value
					});
				}
			}
		}

		private void CatalogZipParts()
		{
			foreach (var part in this.PackageParts)
			{
				if (part.Uri.ToString().EndsWith(ZipPackageCommon.WF_PROCESS_SUFFIX))
				{
					ProcessDescParts.Add(part);
					continue;
				}
				if (part.Uri.ToString().EndsWith(ZipPackageCommon.WF_MATRIXDEF_SUFFIX))
				{
					MatrixDefParts.Add(part);
					continue;
				}
				if (part.Uri.ToString().EndsWith(ZipPackageCommon.WF_MATRIX_SUFFIX))
				{
					MatrixParts.Add(part);
					continue;
				}
			}
		}

		private void Reset()
		{
			MatrixDefParts.Clear();
			MatrixParts.Clear();
			ProcessDescParts.Clear();
			MappingInfo.Clear();
			MatrixDefinitions.Clear();
		}
	}
}