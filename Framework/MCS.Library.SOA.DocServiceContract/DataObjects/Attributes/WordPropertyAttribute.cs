using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DocServiceContract.DataObjects
{

    [AttributeUsage(AttributeTargets.Property)]
    public class WordPropertyAttribute :Attribute
    {

        public string TagID { get; set; }

        public string FormatString { get; set; }

        public bool IsReadOnly { get; set; }

        public WordPropertyAttribute()
        {
            //this.tagID = tagID;
        }
    }
}
