using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow.Exporters
{
	internal class WfExportProcessDescriptorContext
	{
		private List<WfPackageRelationMapping> _MappingInfo = null;
		private List<string> _ExistingParts = null;
		private WfExportProcessDescriptorParams _ExportParams = null;
		private XElementFormatter _Formatter = null;

		public WfExportProcessDescriptorContext(WfExportProcessDescriptorParams exportParams)
		{
			exportParams.NullCheck("exportParams");

			this._ExportParams = exportParams;
		}

		public XElementFormatter Formatter
		{
			get
			{
				if (this._Formatter == null)
				{
					this._Formatter = new XElementFormatter();
					this._Formatter.OutputShortType = false;
				}

				return this._Formatter;
			}
		}

		public WfExportProcessDescriptorParams ExportParams
		{
			get
			{
				return this._ExportParams;
			}
		}

		public List<WfPackageRelationMapping> MappingInfo
		{
			get
			{
				if (this._MappingInfo == null)
					this._MappingInfo = new List<WfPackageRelationMapping>();

				return this._MappingInfo;
			}
		}

		public List<string> ExistingParts
		{
			get
			{
				if (this._ExistingParts == null)
					this._ExistingParts = new List<string>();

				return this._ExistingParts;
			}
		}

		public int MatrixCounter
		{
			get;
			set;
		}
	}
}
