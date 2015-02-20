using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class CalculationChain : IPersistable
	{
		private CalculationCellCollection _CalculationCells;
		public CalculationCellCollection CalculationCells
		{
			get
			{
				if (_CalculationCells == null)
				{
					_CalculationCells = new CalculationCellCollection();
				}

				return _CalculationCells;
			}
		}

		void IPersistable.Save(ExcelSaveContext context)
		{
			context.LinqWriter.WriteCalculation(this);
		}

		void IPersistable.Load(ExcelLoadContext context)
		{
			if (context.Package.PartExists(ExcelCommon.Uri_CalculationChain))
			{
				var root = context.Package.GetXElementFromUri(ExcelCommon.Uri_CalculationChain);
				context.Reader.ReadCalculation(root);
			}
		}
	}
}
