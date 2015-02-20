using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using System.Xml.Serialization;
using MCS.Library.SOA.DocServiceContract;
using DocumentFormat.OpenXml.Wordprocessing;
using WordProcessing;

namespace MCS.Library.Services
{
    public class WordEntry
    {


        public static byte[] GenerateDocument(byte[] templateBuffer, DCTWordDataObject dataobject)
        {
            byte[] buffer = templateBuffer;//File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, templatePath));
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(buffer, 0, buffer.Length);
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(ms, true))
                {
                    var permList = wordDoc.MainDocumentPart.Document.Body.Descendants<PermStart>().ToList();
                    foreach (DCTDataProperty property in dataobject.PropertyCollection)
                    {
                        GeneralDataProcessor gdp = GeneralDataProcessor.CreateProcessor(wordDoc, property);
                        gdp.Process();
                    }
                    DeleteComments(wordDoc);
                    wordDoc.MainDocumentPart.WordprocessingCommentsPart.Comments.Save();
                    wordDoc.MainDocumentPart.Document.Save();
                }
                byte[] resultBuffer = new byte[(int)ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(resultBuffer, 0, (int)ms.Length);
                return resultBuffer;
            }
        }

        public static DCTWordDataObject CollectData(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(data, 0, data.Length);
                using (WordprocessingDocument document = WordprocessingDocument.Open(ms, true))
                {
                    DCTWordDataObject wdo = new DCTWordDataObject();
                    wdo.PropertyCollection = new DCTDataPropertyCollection();

                    List<int> ignoreControls = new List<int>();

                    //读取所有标签标记的区域(bookmarks)
                    processDataAreas(document, wdo, ignoreControls);

                    //遍历所有控件
                    processDataControl(document, wdo, ignoreControls);

                    return wdo;
                }
            }
        }

        private static void processDataControl(WordprocessingDocument document, DCTWordDataObject wdo, List<int> ignoreControls)
        {
            var AllSdtElements = document.MainDocumentPart.Document.Body.Descendants<SdtElement>().ToList();

            foreach (SdtElement se in AllSdtElements)
            {
                var curAlias = se.SdtProperties.Descendants<SdtAlias>().ToList();
                if (curAlias.Count == 0)
                    continue;
                var curId = se.SdtProperties.Descendants<SdtId>().ToList();
                if (curId.Count == 0)
                    continue;
                //如果控件在黑名单中，忽略
                if (curAlias[0].Val == null || ignoreControls.Contains(curId[0].Val.Value))
                    continue;
                //生成简单类型
                DCTSimpleProperty sp = new DCTSimpleProperty() { TagID = curAlias[0].Val.Value, Value = se.InnerText };
                wdo.PropertyCollection.Add(sp);
            }
        }

        public static void DeleteComments(WordprocessingDocument document, string author = "")
        {
            // Get an existing Wordprocessing document.

            // Set commentPart to the document WordprocessingCommentsPart, 
            // if it exists.
            WordprocessingCommentsPart commentPart =
              document.MainDocumentPart.WordprocessingCommentsPart;

            // If no WordprocessingCommentsPart exists, there can be no comments. 
            // Stop execution and return from the method.
            if (commentPart == null)
            {
                return;
            }

            List<Comment> commentsToDelete =
              commentPart.Comments.Elements<Comment>().ToList();

            // Create a list of comments by the specified author.
            if (!String.IsNullOrEmpty(author))
            {
                commentsToDelete = commentsToDelete.
                  Where(c => c.Author == author).ToList();
            }
            IEnumerable<string> commentIds =
              commentsToDelete.Select(r => r.Id.Value);

            // Delete each comment in commentToDelete from the 
            // Comments collection.
            foreach (Comment c in commentsToDelete)
            {
                c.Remove();
            }

            // Save comment part change.
            //commentPart.Comments.Save();

            Document doc = document.MainDocumentPart.Document;

            // Delete CommentRangeStart within main document.
            List<CommentRangeStart> commentRangeStartToDelete =
              doc.Descendants<CommentRangeStart>().
              Where(c => commentIds.Contains(c.Id.Value)).ToList();
            foreach (CommentRangeStart c in commentRangeStartToDelete)
            {
                c.Remove();
            }

            // Delete CommentRangeEnd within the main document.
            List<CommentRangeEnd> commentRangeEndToDelete =
              doc.Descendants<CommentRangeEnd>().
              Where(c => commentIds.Contains(c.Id.Value)).ToList();
            foreach (CommentRangeEnd c in commentRangeEndToDelete)
            {
                c.Remove();
            }

            // Delete CommentReference within main document.
            List<CommentReference> commentRangeReferenceToDelete =
              doc.Descendants<CommentReference>().
              Where(c => commentIds.Contains(c.Id.Value)).ToList();
            foreach (CommentReference c in commentRangeReferenceToDelete)
            {
                c.Remove();
            }

            // Save changes back to the MainDocumentPart part.


        }

        private static void processDataAreas(WordprocessingDocument document, DCTWordDataObject wdo, List<int> ignoreControls)
        {
            var titleRows = document.MainDocumentPart.Document.Body
        .Descendants<TableRow>().Where(o => o.Descendants<BookmarkStart>().Any());

            foreach (TableRow titleRow in titleRows)
            {
                var bookmarkStart = titleRow.Descendants<BookmarkStart>().FirstOrDefault();

                if (bookmarkStart == null)
                    continue;

                if (bookmarkStart.ColumnFirst == null)
                    continue;


                //针对每一个区域，先读取表头的控件,同时将控件加入黑名单
                DCTDataColumnCollection collection = new DCTDataColumnCollection(titleRow);

                ignoreControls.AddRange(collection.CoveredControlIds);


                //遍历区域中的每一行，生成复杂属性

                var curRow = titleRow.NextSibling<TableRow>();

                DCTComplexProperty cp = new DCTComplexProperty();

                cp.TagID = bookmarkStart.Name;

                while (null != curRow && (!curRow.Descendants<BookmarkEnd>().Any(o => o.Id == bookmarkStart.Id)))
                {
                    DCTWordDataObject childWdo = new DCTWordDataObject();

                    DCTDataRow newDataRow = DCTDataRow.FromTableRow(curRow, collection);

                    if (!newDataRow.IsEmpty)
                        childWdo.PropertyCollection.AddRange<DCTSimpleProperty>(newDataRow.ToSimpleProperties());

                    curRow = curRow.NextSibling<TableRow>();

                    cp.DataObjects.Add(childWdo);
                }

                wdo.PropertyCollection.Add(cp);
            }
        }


    }
}
