using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.UI.Control.DefaultActions
{
    class WFOpinionQueryCondition
    {
        public DateTime? IssueTime
        {
            get;
            set;
        }

        public string ResourceID
        {
            get;
            set;
        }

        public string ProcessID
        {
            get;
            set;
        }

        public string ActivityID
        {
            get;
            set;
        }
        public bool InMoveToModel
        {
            get;
            set;
        }

        
    }
}
