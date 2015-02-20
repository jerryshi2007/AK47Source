namespace MCS.Library.Office.OpenXml.Excel
{
    using System;
    using MCS.Library.Core;

	//public class PageSetup : ElementInfo
	//{
	//    private WorkSheet _WorkSheet;

	//    public PageSetup(WorkSheet workSheet)
	//    {
	//        this._WorkSheet = workSheet;
	//        this.paperSize = 9;
	//        this.orientation = "portrait";
	//        this.horizontalDpi = 200;
	//        this.verticalDpi = 200;
	//        this.relId = workSheet.RelationshipID;
	//    }

	//    protected internal override string NodeName
	//    {
	//        get { return "pageSetup";  }
	//    }

	//    public int horizontalDpi
	//    {
	//        get
	//        {
	//            string horizontalDpi = base.GetAttribute("horizontalDpi");
	//            if (horizontalDpi.IsNotEmpty())
	//            {
	//                return int.Parse(horizontalDpi);
	//            }
	//            return 0;
	//        }
	//        set
	//        {
	//            base.SetAttribute("horizontalDpi", value.ToString());
	//        }
	//    }

	//    public string orientation
	//    {
	//        get
	//        {
	//            return base.GetAttribute("orientation");
	//        }
	//        set
	//        {
	//            base.SetAttribute("orientation", value);
	//        }
	//    }

	//    public int paperSize
	//    {
	//        get
	//        {
	//            string paperSize = base.GetAttribute("paperSize");
	//            if (!string.IsNullOrEmpty(paperSize)) return int.Parse(paperSize);
	//            return 0;
	//        }
	//        set
	//        {
	//            base.SetAttribute("paperSize", value.ToString());
	//        }
	//    }

	//    public string relId
	//    {
	//        get
	//        {
	//            return base.GetAttribute("r:id");
	//        }
	//        set
	//        {
	//            base.SetAttribute("r:id", value);
	//        }
	//    }

	//    public int verticalDpi
	//    {
	//        get
	//        {
	//            string verticalDpi = base.GetAttribute("verticalDpi");
	//            if (verticalDpi.IsNotEmpty())
	//                return int.Parse(verticalDpi);
	//            return 0;
	//        }
	//        set
	//        {
	//            base.SetAttribute("verticalDpi", value.ToString());
	//        }
	//    }
	//}
}
