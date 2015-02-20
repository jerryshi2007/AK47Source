using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Packaging;
using System.Xml.Linq;
using MCS.Library.Core;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class VmlDrawingPictureCollection : ExcelCollectionBase<string, VmlDrawingPicture>, IPersistable
	{
		//private List<VmlDrawingPicture> pictureList = new List<VmlDrawingPicture>();
		//private Dictionary<string, VmlDrawingPicture> pictureDic = new Dictionary<string, VmlDrawingPicture>();

		internal WorkSheet _WorkSheet;

		public VmlDrawingPictureCollection(WorkSheet worksheet)
		{
			this._WorkSheet = worksheet;
		}

		internal VmlDrawingPictureCollection(WorkSheet worksheet, string relationshipID)
			: this(worksheet)
		{
			this._RelationshipID = relationshipID;
		}

		private string _RelationshipID;
		internal string RelationshipID
		{
			get { return this._RelationshipID; }
			set { this._RelationshipID = value; }
		}


		internal Uri PictureUri
		{
			get;
			set; 
		}


		void IPersistable.Save(ExcelSaveContext context)
		{
			context.LinqWriter.WriteReadHeaderFooterVmlDrawingPicture(this);
			//HeaderFooterVmlDrawingPicture
		}

		void IPersistable.Load(ExcelLoadContext context)
		{
			PackagePart currentSheetPart = context.Package.GetPart(this._WorkSheet.SheetUri);
			if (currentSheetPart.RelationshipExists(this._RelationshipID))
			{
				PackageRelationship rel = currentSheetPart.GetRelationship(this._RelationshipID);
				PictureUri = PackUriHelper.ResolvePartUri(rel.SourceUri, rel.TargetUri);

				XElement vmlDrawingPictureXml = context.Package.GetXElementFromUri(PictureUri);
				if (vmlDrawingPictureXml != null)
				{
					context.Reader.ReadHeaderFooterVmlDrawingPicture(this, vmlDrawingPictureXml);
				}
			}
		}

		public new VmlDrawingPicture this[string Name]
		{
			get
			{
				if (base.ContainsKey(Name))
				{
					return base[Name];
				}
				else
				{
					return null;
				}
			}
		}

		protected override string GetKeyForItem(VmlDrawingPicture item)
		{
			return item.Id;
		}
	}
}
