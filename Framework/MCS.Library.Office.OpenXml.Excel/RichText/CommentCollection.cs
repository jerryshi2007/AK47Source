using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Packaging;
using System.Xml.Linq;

namespace MCS.Library.Office.OpenXml.Excel
{
	internal sealed class CommentCollection : IPersistable
	{
		internal WorkSheet _WorkSheet = null;

		public CommentCollection(WorkSheet worksheet)
		{
			this._WorkSheet = worksheet;
		}

		internal CommentCollection(WorkSheet worksheet, string relationshipID)
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

		private Uri _VmlDrawingsUri = null;
		internal Uri vmlDrawingsUri
		{
			get
			{
				if (this._VmlDrawingsUri == null)
				{
					this._VmlDrawingsUri = new Uri(string.Format(@"/xl/drawings/vmlDrawing{0}.vml", this._WorkSheet.PositionID + 1), UriKind.Relative);
				}
				return this._VmlDrawingsUri;
			}
			set
			{
				this._VmlDrawingsUri = value;
			}
		}

		private Uri _CommentUri = null;
		internal Uri CommentUri
		{
			get
			{
				if (this._CommentUri == null)
				{

				}
				return this._CommentUri;
			}
			set
			{
				this._CommentUri = value;
			}

		}

		void IPersistable.Save(ExcelSaveContext context)
		{
			context.LinqWriter.WriteComments(this);
			context.LinqWriter.WriteCommentsVmlDrawing(this);
		}

		void IPersistable.Load(ExcelLoadContext context)
		{
			PackagePart currentSheetPart = context.Package.GetPart(this._WorkSheet.SheetUri);
			if (currentSheetPart.RelationshipExists(this.RelationshipID))
			{
				PackageRelationship vmlDrawingRelation = currentSheetPart.GetRelationship(this.RelationshipID);
				this._VmlDrawingsUri = PackUriHelper.ResolvePartUri(vmlDrawingRelation.SourceUri, vmlDrawingRelation.TargetUri);
				XElement vmlDrawingElement = context.Package.GetXElementFromUri(this._VmlDrawingsUri);
				context.Reader.ReadSheetCommentVmlDrawing(this, vmlDrawingElement);

				PackageRelationshipCollection commentParts = currentSheetPart.GetRelationshipsByType(ExcelCommon.Schema_Comment);
				foreach (PackageRelationship commentPartRelationship in commentParts)
				{
					this._CommentUri = PackUriHelper.ResolvePartUri(commentPartRelationship.SourceUri, commentPartRelationship.TargetUri);
					PackagePart commentPart = context.Package.GetPart(this._CommentUri);
					XDocument commentXml = XDocument.Load(commentPart.GetStream());

					context.Reader.ReadSheetComments(this, commentXml.Root);
				}
			}
		}
	}
}
