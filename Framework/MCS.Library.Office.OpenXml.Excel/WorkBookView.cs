using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MCS.Library.Core;

namespace MCS.Library.Office.OpenXml.Excel
{
    public class WorkBookView : ElementInfo
    {
        private WorkBook _WorkBook;

        public WorkBookView(WorkBook workbook)
        {
            this._WorkBook = workbook;
			this.BookViewID = workbook.Views.GetNextBookViewID();
        }

        protected internal override string NodeName
        {
            get { return "workbookView"; }
        }

        internal int BookViewID { get; set; }

        // private const string TOP_PATH = "d:bookViews/d:workbookView/@xWindow";
        /// <summary>
        /// 工作簿窗口的左上角的位置
        /// </summary>
        public int Left
        {
            get
            {
                string le = base.GetAttribute("xWindow");
                if (le.IsNotEmpty())
                {
                    return int.Parse(le);
                }
                return 0;
            }
            set
            {
                base.SetAttribute("xWindow", value.ToString());
            }
        }

        // private const string TOP_PATH = "d:bookViews/d:workbookView/@yWindow";
        public int Top
        {
            get
            {
                string le = base.GetAttribute("yWindow");
                if (!string.IsNullOrEmpty(le))
                {
                    return int.Parse(le);
                }
                return 0;
            }
            set
            {
                base.SetAttribute("yWindow", value.ToString());
            }
        }

        //private const string WIDTH_PATH = "d:bookViews/d:workbookView/@windowWidth";
        public int Width
        {
            get
            {
                string le = base.GetAttribute("windowWidth");
                if (le.IsNotEmpty())
                {
                    return int.Parse(le);
                }
                return 0;
            }
            set
            {
                base.SetAttribute("windowWidth", value.ToString());
            }
        }

        //private const string HEIGHT_PATH = "d:bookViews/d:workbookView/@windowHeight";
        public int Height
        {
            get
            {
                string le = base.GetAttribute("windowHeight");
                if (le.IsNotEmpty())
                {
                    return int.Parse(le);
                }
                return 0;
            }
            set
            {
                base.SetAttribute("windowHeight", value.ToString());
            }
        }

        //private const string MINIMIZED_PATH = "d:bookViews/d:workbookView/@minimized";
        public bool Minimized
        {
            get
            {
                string le = base.GetAttribute("minimized");
                if (le.IsNotEmpty())
                {
                    return int.Parse(le) == 1 ? true : false;
                }
                return false;
            }
            internal set
            {
                if (value)
                {
                    base.SetAttribute("minimized", "1");
                }
                else
                {
                    base.Attributes.Remove("minimized");
                }
            }
        }

        // private const string SHOWVERTICALSCROLL_PATH = "d:bookViews/d:workbookView/@showVerticalScroll";
        public bool ShowVerticalScrollBar
        {
            get
            {
                string le = base.GetAttribute("showVerticalScroll");
                if (le.IsNotEmpty())
                {
                    return int.Parse(le) == 1 ? true : false;
                }
                return false;
            }
            internal set
            {
                if (value)
                {
                    base.SetAttribute("showVerticalScroll", "1");
                }
                else
                {
                    base.Attributes.Remove("showVerticalScroll");
                }
            }
        }

        // private const string SHOWHORIZONTALSCR_PATH = "d:bookViews/d:workbookView/@showHorizontalScroll";
        public bool ShowHorizontalScrollBar
        {
            get
            {
                string le = base.GetAttribute("showHorizontalScroll");
                if (le.IsNotEmpty())
                {
                    return int.Parse(le) == 1 ? true : false;
                }
                return false;
            }
            set
            {
                if (value)
                {
                    base.SetAttribute("showHorizontalScroll", "1");
                }
                else
                {
                    base.Attributes.Remove("showHorizontalScroll");
                }
            }
        }

        private const string SHOWSHEETTABS_PATH = "d:bookViews/d:workbookView/@showSheetTabs";
        public bool ShowSheetTabs
        {
            get
            {
                string le = base.GetAttribute("showSheetTabs");
                if (le.IsNotEmpty())
                {
                    return int.Parse(le) == 1 ? true : false;
                }
                return false;
            }
            set
            {
                if (value)
                {
                    base.SetAttribute("showSheetTabs", "1");
                }
                else
                {
                    base.Attributes.Remove("showSheetTabs");
                }
            }
        }

        public void SetWindowSize(int left, int top, int width, int height)
        {
            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
        }
    }
}
