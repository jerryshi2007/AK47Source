using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Office.OpenXml.Excel
{
    public class WorkbookFileVersion : ElementInfo
    {
        public string AppName
        {
            get
            {
                return base.GetAttribute("appName");
            }
            set
            {
                base.SetAttribute("appName", value);
            }
        }

        public int LastEdited
        {
            get
            {
                string le = base.GetAttribute("lastEdited");
                if (le.IsNotEmpty())
                {
                    return int.Parse(le);
                }
                return 0;
            }
            set
            {
                base.SetAttribute("lastEdited", value.ToString());
            }
        }

        public int LowestEdited
        {
            get
            {
                string le = base.GetAttribute("lowestEdited");
                if (le.IsNotEmpty())
                {
                    return int.Parse(le);
                }
                return 0;
            }
            set
            {
                base.SetAttribute("lowestEdited", value.ToString());
            }
        }

        public int RupBuild
        {
            get
            {
                string rb = base.GetAttribute("rupBuild");
                if (rb.IsNotEmpty())
                {
                    return int.Parse(rb);
                }
                return 0;
            }
            set
            {
                base.SetAttribute("rupBuild", value.ToString());
            }
        }

        protected internal override string NodeName
        {
            get { return "fileVersion"; }
        }
    }
}
