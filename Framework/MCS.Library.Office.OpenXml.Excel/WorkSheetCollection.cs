using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MCS.Library.Core;
using System.Reflection;

namespace MCS.Library.Office.OpenXml.Excel
{
    public class WorkSheetCollection : ExcelCollectionBase<string, WorkSheet>
    {
        internal static XmlNamespaceManager NameSpaceManager;

        static WorkSheetCollection()
        {
            NameSpaceManager = CreateDefaultSheetsXmlNamespace();
        }

        internal static XmlNamespaceManager CreateDefaultSheetsXmlNamespace()
        {
            NameTable nt = new NameTable();
            XmlNamespaceManager sheetsManager = new XmlNamespaceManager(nt);
            sheetsManager.AddNamespace(string.Empty, ExcelCommon.Schema_WorkBook_Main.NamespaceName);
            sheetsManager.AddNamespace("d", ExcelCommon.Schema_WorkBook_Main.NamespaceName);
            sheetsManager.AddNamespace("r", ExcelCommon.Schema_Relationships);

            return sheetsManager;
        }

        public new WorkSheet this[string name]
        {
            get
            {
                WorkSheet result = null;

                if (base.InnerDict.ContainsKey(name))
                    result = base.InnerDict[name];

                return result;
            }
        }

        /// <summary>
        /// 根据sheetCode拿工作表
        /// </summary>
        /// <param name="sheetCode"></param>
        /// <returns></returns>
        public WorkSheet GetSheetByCode(string sheetCode)
        {
            WorkSheet resutl = null;
            foreach (WorkSheet item in this)
            {
                if (string.Compare(item.SheetCode, sheetCode) == 0)
                {
                    resutl = item;
                    break;
                }
            }

            return resutl;
        }

        /// <summary>
        /// 拿工作薄
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="sheetCode"></param>
        /// <param name="outsheet"></param>
        /// <returns></returns>
        public bool TryGetWrokSheet(string sheetName, string sheetCode, out WorkSheet outsheet)
        {
            bool result = false;

            if (this.ContainsKey(sheetName))
            {
                result = true;
                outsheet = this[sheetName];
            }
            else
            {
                outsheet = this.GetSheetByCode(sheetCode);
                if (outsheet != null)
                    result = true;
            }

            return result;
        }

        internal bool IsCreateEmpty()
        {
            bool result = false;

            if (this.ContainsKey(WorkBook.DefaultSheetName) && this.Count == 1)
                result = true;

            return result;
        }

        protected override string GetKeyForItem(WorkSheet item)
        {
            return item.Name;
        }
    }
}
