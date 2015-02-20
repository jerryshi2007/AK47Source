using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using System.IO;
using System.IO.Packaging;
using MCS.Library.SOA.DataObjects.Workflow;
using System.Xml.Linq;
using System.Net.Mime;
using MCS.Library.Office.SpreadSheet;

namespace WorkflowDesigner
{
	/// <summary>
	/// 流程模板Zip包内容类型
	/// </summary>
	public enum PackagePartType
	{
		ProcessPart,
		MatrixPart,
		MatrixDefPart,
		MappingPart
	}

	/// <summary>
	/// 流程模板Zip包关系映射
	/// </summary>
	public struct PackageRelationMapping
	{
		public string MatrixPath { get; set; }
		public string MatrixDefID { get; set; }
		public string ProcessDescriptionID { get; set; }
	}

	public static class ZipPackageCommon
	{
		public static readonly string WF_PROCESS_SUFFIX = "_proc.xml";
		public static readonly string WF_MATRIX_SUFFIX = "_mtrx.xlsx";
		public static readonly string WF_MATRIXDEF_SUFFIX = "_mtrxdef.xml";
		public static readonly string WF_MAPPING = "mapping.map";
	}

	/// <summary>
	/// 指定流程模板KEY，将这些模板导出为ZIP文件
	/// </summary>
	[Obsolete("被WfProcessExporter.ExportProcessDescriptors替换了")]
	public class ZipPackageExporter
	{
		private XElementFormatter XmlFormatter { get; set; }
		private bool IsRoleAsPerson { get; set; }
		private List<string> ProcessKeys { get; set; }
		private List<PackageRelationMapping> MappingInfo { get; set; }
		private List<string> ExistingParts { get; set; }
		private int MatrixCounter { get; set; }

		public ZipPackageExporter(string[] processKeys, bool isRoleAsPerson)
		{
			this.XmlFormatter = new XElementFormatter();
			XmlFormatter.OutputShortType = false;
			this.ProcessKeys = new List<string>(processKeys);
			this.IsRoleAsPerson = isRoleAsPerson;
			this.MappingInfo = new List<PackageRelationMapping>();
			this.ExistingParts = new List<string>();
		}

		public byte[] Export()
		{
			using (MemoryStream outputStream = new MemoryStream())
			{
				using (Package package = ZipPackage.Open(outputStream, FileMode.Create))
				{
					foreach (var key in this.ProcessKeys)
					{
						var processDesc = (WfProcessDescriptor)WfProcessDescriptorManager.LoadDescriptor(key);
						ExportProcessPart(package, processDesc);
						ExportMatrixPart(package, processDesc);
					}

					ExportMappingPart(package);
					package.Flush();
					outputStream.Flush();
					this.Reset();

					return outputStream.ToArray();
				}
			}
		}

		private void ExportMappingPart(Package package)
		{
			if (this.MappingInfo.Count == 0)
			{
				return;
			}

			Uri partUri = CreatePartUri("", PackagePartType.MappingPart);
			XElement mappingsXml = new XElement("Mappings");

			foreach (var mapItem in this.MappingInfo)
			{
				mappingsXml.Add(new XElement("Item",
					new XAttribute("matrixID", mapItem.MatrixPath),
					new XAttribute("matrixDefID", mapItem.MatrixDefID),
					new XAttribute("processDescID", mapItem.ProcessDescriptionID))
				);
			}

			var xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "true"), mappingsXml);

			PackagePart part = package.CreatePart(partUri, MediaTypeNames.Text.Xml);
			using (var stream = part.GetStream())
			{
				xDoc.Save(stream);
				stream.Flush();
			}
		}

		private void ExportProcessPart(Package package, WfProcessDescriptor processDesc)
		{
			Uri partUri = CreatePartUri(processDesc.Key, PackagePartType.ProcessPart);

			if (ExistingParts.Contains(partUri.ToString()))
			{
				return;
			}

			XElement xeWfProcess = this.XmlFormatter.Serialize(processDesc);
			var xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "true"), xeWfProcess);

			PackagePart part = package.CreatePart(partUri, MediaTypeNames.Text.Xml);
			using (var stream = part.GetStream())
			{
				xDoc.Save(stream);
				stream.Flush();
			}

			ExistingParts.Add(partUri.ToString());
		}

		private void ExportMatrixPart(Package package, WfProcessDescriptor processDesc)
		{
			var matrix = WfMatrixAdapter.Instance.LoadByProcessKey(processDesc.Key, false);
			if (matrix == null)
			{
				return;
			}
			matrix.Loaded = true;

			Uri partUri = CreatePartUri(this.MatrixCounter.ToString(), PackagePartType.MatrixPart);
			if (ExistingParts.Contains(partUri.ToString()))
			{
				return;
			}

			this.MappingInfo.Add(new PackageRelationMapping()
			{
				MatrixPath = partUri.ToString(),
				MatrixDefID = matrix.Definition.Key,
				ProcessDescriptionID = processDesc.Key
			});

			ExportMatrixDefPart(package, matrix.Definition);

		
			PackagePart part = package.CreatePart(partUri, MediaTypeNames.Application.Octet);
			using (MemoryStream bytes = matrix.ExportToExcel2007(this.IsRoleAsPerson))
			{
				using (var stream = part.GetStream())
				{
					bytes.CopyTo(stream);
					//stream.Write(bytes, 0, bytes.Length);
					stream.Flush();
				}
			}

			ExistingParts.Add(partUri.ToString());
			this.MatrixCounter++;
		}

		private void ExportMatrixDefPart(Package package, WfMatrixDefinition matrixDef)
		{
			Uri partUri = CreatePartUri(matrixDef.Key, PackagePartType.MatrixDefPart);
			if (ExistingParts.Contains(partUri.ToString()))
			{
				return;
			}

			XElement matrixDefXml = XmlFormatter.Serialize(matrixDef);
			var xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "true"), matrixDefXml);
			PackagePart part = package.CreatePart(partUri, MediaTypeNames.Text.Xml);

			using (var stream = part.GetStream())
			{
				xDoc.Save(stream);
				stream.Flush();
			}

			ExistingParts.Add(partUri.ToString());
		}

		private static Uri CreatePartUri(string fileName, PackagePartType partType)
		{
			Uri result = null;
			switch (partType)
			{
				case PackagePartType.ProcessPart:
					result = new Uri("/" + fileName + ZipPackageCommon.WF_PROCESS_SUFFIX, UriKind.Relative);
					break;
				case PackagePartType.MatrixPart:
					result = new Uri("/" + fileName + ZipPackageCommon.WF_MATRIX_SUFFIX, UriKind.Relative);
					break;
				case PackagePartType.MatrixDefPart:
					result = new Uri("/" + fileName + ZipPackageCommon.WF_MATRIXDEF_SUFFIX, UriKind.Relative);
					break;
				case PackagePartType.MappingPart:
					result = new Uri("/" + ZipPackageCommon.WF_MAPPING, UriKind.Relative);
					break;
			}

			return result;
		}

		private void Reset()
		{
			this.MappingInfo.Clear();
			this.ExistingParts.Clear();
			this.MatrixCounter = 0;
		}
	}
}