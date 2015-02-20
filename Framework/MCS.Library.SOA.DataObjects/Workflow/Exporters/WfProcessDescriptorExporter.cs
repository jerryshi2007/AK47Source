using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Xml.Linq;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow.Exporters
{
	/// <summary>
	/// 流程信息的导出器
	/// </summary>
	public static class WfProcessExporter
	{
		private static readonly string WF_PROCESS_SUFFIX = "_proc.xml";
		private static readonly string WF_MATRIX_SUFFIX = "_mtrx.xlsx";
		private static readonly string WF_MATRIXDEF_SUFFIX = "_mtrxdef.xml";
		private static readonly string WF_MAPPING = "mapping.map";

		public static void ExportProcessDescriptors(WfExportProcessDescriptorParams exportParams, Stream outputStream, params string[] processDespKeys)
		{
			exportParams.NullCheck("exportParams");
			processDespKeys.NullCheck("processDespKeys");

			WfExportProcessDescriptorContext context = new WfExportProcessDescriptorContext(exportParams);

			using (Package package = ZipPackage.Open(outputStream, FileMode.Create))
			{
				foreach (string key in processDespKeys)
				{
					WfProcessDescriptor processDesp = (WfProcessDescriptor)WfProcessDescriptorManager.LoadDescriptor(key);

					ExportProcessPart(package, processDesp, context);
					ExportMatrixPart(package, processDesp, context);
				}

				ExportMappingPart(package, context);
			}
		}

		public static byte[] ExportProcessDescriptors(WfExportProcessDescriptorParams exportParams, params string[] processDespKeys)
		{
			using (MemoryStream outputStream = new MemoryStream())
			{
				ExportProcessDescriptors(exportParams, outputStream, processDespKeys);

				outputStream.Flush();

				return outputStream.ToArray();
			}
		}

		private static void ExportProcessPart(Package package, WfProcessDescriptor processDesc, WfExportProcessDescriptorContext context)
		{
			Uri partUri = CreatePartUri(processDesc.Key, WfProcessDescriptorPackagePartType.ProcessPart);

			if (context.ExistingParts.Contains(partUri.ToString()))
				return;

			XElement xeWfProcess = context.Formatter.Serialize(processDesc);
			XDocument xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "true"), xeWfProcess);

			PackagePart part = package.CreatePart(partUri, MediaTypeNames.Text.Xml, CompressionOption.Normal);

			using (Stream stream = part.GetStream())
				xDoc.Save(stream);

			context.ExistingParts.Add(partUri.ToString());
		}

		private static void ExportMatrixPart(Package package, WfProcessDescriptor processDesp, WfExportProcessDescriptorContext context)
		{
			var matrix = WfMatrixAdapter.Instance.LoadByProcessKey(processDesp.Key, false);

			if (matrix == null)
				return;

			matrix.Loaded = true;

			Uri partUri = CreatePartUri(context.MatrixCounter.ToString(), WfProcessDescriptorPackagePartType.MatrixPart);

			if (context.ExistingParts.Contains(partUri.ToString()))
				return;

			context.MappingInfo.Add(new WfPackageRelationMapping()
			{
				MatrixPath = partUri.ToString(),
				MatrixDefID = matrix.Definition.Key,
				ProcessDescriptionID = processDesp.Key
			});

			ExportMatrixDefPart(package, matrix.Definition, context);

			PackagePart part = package.CreatePart(partUri, MediaTypeNames.Application.Octet, CompressionOption.Normal);
			using (MemoryStream bytes = matrix.ExportToExcel2007(context.ExportParams.MatrixRoleAsPerson))
			{
				using (Stream stream = part.GetStream())
				{
					bytes.CopyTo(stream);
				}
			}

			context.ExistingParts.Add(partUri.ToString());
			context.MatrixCounter++;
		}

		private static void ExportMatrixDefPart(Package package, WfMatrixDefinition matrixDef, WfExportProcessDescriptorContext context)
		{
			Uri partUri = CreatePartUri(matrixDef.Key, WfProcessDescriptorPackagePartType.MatrixDefPart);

			if (context.ExistingParts.Contains(partUri.ToString()))
				return;

			XElement matrixDefXml = context.Formatter.Serialize(matrixDef);

			XDocument xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "true"), matrixDefXml);
			PackagePart part = package.CreatePart(partUri, MediaTypeNames.Text.Xml);

			using (var stream = part.GetStream())
			{
				xDoc.Save(stream);
				stream.Flush();
			}

			context.ExistingParts.Add(partUri.ToString());
		}

		private static void ExportMappingPart(Package package, WfExportProcessDescriptorContext context)
		{
			if (context.MappingInfo.Count == 0)
				return;

			Uri partUri = CreatePartUri(string.Empty, WfProcessDescriptorPackagePartType.MappingPart);

			XElement mappingsXml = new XElement("Mappings");

			foreach (var mapItem in context.MappingInfo)
			{
				mappingsXml.Add(new XElement("Item",
					new XAttribute("matrixID", mapItem.MatrixPath),
					new XAttribute("matrixDefID", mapItem.MatrixDefID),
					new XAttribute("processDescID", mapItem.ProcessDescriptionID))
				);
			}

			XDocument xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "true"), mappingsXml);

			PackagePart part = package.CreatePart(partUri, MediaTypeNames.Text.Xml);

			using (Stream stream = part.GetStream())
			{
				xDoc.Save(stream);
			}
		}

		private static Uri CreatePartUri(string fileName, WfProcessDescriptorPackagePartType partType)
		{
			Uri result = null;
			switch (partType)
			{
				case WfProcessDescriptorPackagePartType.ProcessPart:
					result = new Uri("/" + fileName + WfProcessExporter.WF_PROCESS_SUFFIX, UriKind.Relative);
					break;
				case WfProcessDescriptorPackagePartType.MatrixPart:
					result = new Uri("/" + fileName + WfProcessExporter.WF_MATRIX_SUFFIX, UriKind.Relative);
					break;
				case WfProcessDescriptorPackagePartType.MatrixDefPart:
					result = new Uri("/" + fileName + WfProcessExporter.WF_MATRIXDEF_SUFFIX, UriKind.Relative);
					break;
				case WfProcessDescriptorPackagePartType.MappingPart:
					result = new Uri("/" + WfProcessExporter.WF_MAPPING, UriKind.Relative);
					break;
			}

			return result;
		}
	}
}
