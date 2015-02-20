using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using MCS.Library.SOA.DocServiceContract;

namespace MCS.Library.Services
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
                return new SamplePropertyProcessor(document, (DCTSimpleProperty)property);
            return new ComplexPropertyProcessor(document, (DCTComplexProperty)property);
        }
    }

    public class SamplePropertyProcessor : GeneralDataProcessor
    {
        protected DCTSimpleProperty DataProperty
        {
            get
            {
                return (DCTSimpleProperty)dataProperty;
            }
        }

        public SamplePropertyProcessor(WordprocessingDocument document, DCTSimpleProperty dataProperty)
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
                var runElement = containerElement.Descendants<Run>().First();
                runElement.RemoveAllChildren();
                runElement.AppendChild<Text>(new Text(GeneralFormatter.ToString(property.Value, property.FormatString)));
                if (property.IsReadOnly)
                {
                    Lock lockControl = new Lock();
                    lockControl.Val = LockingValues.SdtContentLocked;
                    containerElement.SdtProperties.Append(lockControl);
                }
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

                for (int j = bookmarkStart.ColumnFirst; j <= bookmarkStart.ColumnLast; j++)
                {
                    DCTDataProperty dataProperty = wordDataObj.PropertyCollection[properties[j - bookmarkStart.ColumnFirst]];
                    if (dataProperty is DCTSimpleProperty)
                    {
                        DCTSimpleProperty simpleProperty = dataProperty as DCTSimpleProperty;
                        TableCell cell = getCellByIndex(firstCell, j);
                        Paragraph p = cell.Descendants<Paragraph>().FirstOrDefault();
                        p.RemoveAllChildren();
                        p.AppendChild<Run>(new Run(new Text((GeneralFormatter.ToString(simpleProperty.Value, simpleProperty.FormatString)))));
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
    }
}
