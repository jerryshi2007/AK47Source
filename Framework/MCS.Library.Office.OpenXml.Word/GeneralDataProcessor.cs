using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using MCS.Library.SOA.DocServiceContract;

namespace MCS.Library.Office.OpenXml.Word
{
    public abstract class GeneralDataProcessor
    {
        protected WordprocessingDocument document;

        protected DCTDataProperty dataProperty;

        public GeneralDataProcessor(WordprocessingDocument document, DCTDataProperty dataProperty)
        {
            this.document = document;
            this.dataProperty = dataProperty;
        }

        public abstract void Process();

        public static GeneralDataProcessor CreateProcessor(WordprocessingDocument document, DCTDataProperty property)
        {
            if (property is DCTSimpleProperty)
                return new SimplePropertyProcessor(document, (DCTSimpleProperty)property);

            return new ComplexPropertyProcessor(document, (DCTComplexProperty)property);
        }

        protected void FillText(ref Run runElement, string strFormat)
        {
            if (runElement.HasChildren)
            {
                runElement.GetFirstChild<Text>().Text = strFormat;
                //runElement.RemoveAllChildren<Text>();
                //runElement.AppendChild<Text>(new Text(GeneralFormatter.ToString(rows[i], property.FormatString)));
            }
            else
            {
                runElement.AppendChild<Text>(new Text(strFormat));
            }
        }
    }

    public class SimplePropertyProcessor : GeneralDataProcessor
    {
        protected DCTSimpleProperty DataProperty
        {
            get
            {
                return (DCTSimpleProperty)dataProperty;
            }
        }

        public SimplePropertyProcessor(WordprocessingDocument document, DCTSimpleProperty dataProperty)
            : base(document, dataProperty)
        {
        }

        public override void Process()
        {
            DCTSimpleProperty property = DataProperty as DCTSimpleProperty;
            if (null != property)
            {
                var containerElement = document.MainDocumentPart.Document.Body
                .Descendants<SdtElement>().Where(o => o.SdtProperties.Descendants<SdtAlias>().Any(a => a.Val == DataProperty.TagID)).FirstOrDefault();
                if (null == containerElement)
                    return;

                object text = property.Value;

                var pas = containerElement.Descendants<Paragraph>().ToList();

                Paragraph p = pas.FirstOrDefault();
                if (p == null)
                {
                    var runElement = containerElement.Descendants<Run>().First();
                    FillText(ref runElement, GeneralFormatter.ToString(text, property.FormatString));
                    /*var runElement = containerElement.Descendants<Run>().First();
                    runElement.RemoveAllChildren();
                    runElement.AppendChild<Text>(new Text(GeneralFormatter.ToString(strAlltext, property.FormatString))); */
                }
                else
                {
                    string strAlltext = property.Value.ToString();
                    string[] splitArry = new string[] { 
                  "\r\n"
                };
                    string[] rows = strAlltext.Split(splitArry, StringSplitOptions.None);
                    for (int i = 0; i < rows.Length; i++)
                    {
                        if (i == 0)
                        {
                            Run runElement = GetRunElement(p);
                            FillText(runElement, GeneralFormatter.ToString(rows[i], property.FormatString));
                        }
                        else
                        {
                            Paragraph addrow;
                            if (i < pas.Count)
                                addrow = pas[i];
                            else
                                addrow = p.CloneNode(true) as Paragraph;

                            Run addrunelement = GetRunElement(addrow);

                            FillText(addrunelement, GeneralFormatter.ToString(rows[i], property.FormatString));

                            if (i >= pas.Count)
                            {
                                var lastCon = containerElement.Descendants<Paragraph>().Last();
                                lastCon.InsertAfterSelf<Paragraph>(addrow);
                            }
                        }
                    }
                }

                if (property.IsReadOnly)
                {
                    Lock lockControl = new Lock();
                    lockControl.Val = LockingValues.SdtContentLocked;
                    containerElement.SdtProperties.Append(lockControl);
                }
            }
        }

        private Run GetRunElement(Paragraph p)
        {
            var runs = p.Descendants<Run>();
            Run runElement = null;
            if (runs.Count() > 0)
            {
                runElement = p.Descendants<Run>().First();
            }
            else
            {
                runElement = p.AppendChild<Run>(new Run());
            }
            return runElement;
        }

        private void FillText(Run runElement, string strFormat)
        {
            if (runElement.HasChildren)
            {
                runElement.GetFirstChild<Text>().Text = strFormat;
            }
            else
            {
                runElement.AppendChild<Text>(new Text(strFormat));
            }
        }
    }

    public class ComplexPropertyProcessor : GeneralDataProcessor
    {
        protected DCTComplexProperty DataProperty
        {
            get
            {
                return (DCTComplexProperty)dataProperty;
            }
        }

        public ComplexPropertyProcessor(WordprocessingDocument document, DCTComplexProperty dataProperty)
            : base(document, dataProperty)
        {
        }

        public override void Process()
        {
            var titleRow = document.MainDocumentPart.Document.Body
                .Descendants<TableRow>().Where(o => o.Descendants<BookmarkStart>().Any(mark => mark.Name == DataProperty.TagID)).FirstOrDefault();

            var sdtElements = titleRow.Descendants<SdtElement>().Where(o => o.SdtProperties.Descendants<SdtAlias>().Any(a => a.Val != null)).ToList();

            var bookmarkStart = titleRow.Descendants<BookmarkStart>().Where(o => o.Name == DataProperty.TagID).FirstOrDefault();

            List<string> properties = new List<string>();
            for (int i = 0; i < sdtElements.Count; i++)
            {
                properties.Add(sdtElements[i].Descendants<SdtAlias>().FirstOrDefault().Val.Value);
            }

            TableRow curRow = titleRow;

            foreach (DCTWordDataObject wordDataObj in DataProperty.DataObjects)
            {
                TableRow newRow = curRow.InsertAfterSelf<TableRow>((TableRow)curRow.NextSibling<TableRow>().CloneNode(true));

                TableCell firstCell = newRow.GetFirstChild<TableCell>();
                int columnFirst = Int32Value.ToInt32(bookmarkStart.ColumnFirst);
                //== default(Int32Value) ? bookmarkStart.ColumnFirst.Value : 0;
                int columnLast = Int32Value.ToInt32(bookmarkStart.ColumnLast);

                for (int j = columnFirst; j <= columnLast; j++)
                {
                    DCTDataProperty dataProperty = wordDataObj.PropertyCollection[properties[j - columnFirst]];
                    if (dataProperty is DCTSimpleProperty)
                    {
                        DCTSimpleProperty simpleProperty = dataProperty as DCTSimpleProperty;
                        TableCell cell = getCellByIndex(firstCell, j);
                        Paragraph p = cell.GetFirstChild<Paragraph>();

                        string strAlltext = simpleProperty.Value.ToString();
                        string[] splitArry = new string[] { "\r\n" };
                        string[] rows = strAlltext.Split(splitArry, StringSplitOptions.None);

                        for (int i = 0; i < rows.Length; i++)
                        {
                            if (i == 0)
                            {
                                Run runElement = GetRunElement(p);
                                FillText(ref runElement, GeneralFormatter.ToString(rows[i], simpleProperty.FormatString));
                            }
                            else
                            {
                                Paragraph addrow = p.CloneNode(true) as Paragraph;
                                Run addrunelement = GetRunElement(addrow);
                                //addrow.GetFirstChild<Run>();
                                FillText(ref addrunelement, GeneralFormatter.ToString(rows[i], simpleProperty.FormatString));
                                p.InsertAfterSelf<Paragraph>(addrow);
                            }
                        }
                    }

                }

                curRow = newRow;
            }
        }

        private TableCell getCellByIndex(TableCell firstCell, int index)
        {
            TableCell result = firstCell;
            for (int i = 0; i < index; i++)
            {
                result = result.NextSibling<TableCell>();
            }
            return result;
        }

        /*
        private void FillText(ref Run runElement, string strFormat)
        {
            if (runElement.HasChildren)
            {
                runElement.GetFirstChild<Text>().Text = strFormat;
                //runElement.RemoveAllChildren<Text>();
                //runElement.AppendChild<Text>(new Text(GeneralFormatter.ToString(rows[i], property.FormatString)));
            }
            else
            {
                runElement.AppendChild<Text>(new Text(strFormat));
            }
        } */

        private Run GetRunElement(Paragraph p)
        {
            var runs = p.Descendants<Run>();
            Run runElement = null;
            if (runs.Count() > 0)
            {
                runElement = p.Descendants<Run>().First();
            }
            else
            {
                runElement = p.AppendChild<Run>(new Run());
            }
            return runElement;
        }
    }
}
