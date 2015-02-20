using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.UI.Control.DefaultActions
{
    [Serializable]
    class DefaultWfClientOpinion
    {
        public string ID
        {
            get;
            set;
        }

        public string ResourceID
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }

        public string IssuePersonID
        {
            get;
            set;
        }

        public string IssuePersonName
        {
            get;
            set;
        }

        public string AppendPersonID
        {
            get;
            set;
        }

        public string AppendPersonName
        {
            get;
            set;
        }

        public string IssueTime
        {
            get;
            set;
        }

        public DateTime AppendTime
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

        public string LevelName
        {
            get;
            set;
        }

        public string LevelDesp
        {
            get;
            set;
        }

        public string OpinionType
        {
            get;
            set;
        }

        public string ExtraData
        {
            get;
            set;
        }
    }
}
