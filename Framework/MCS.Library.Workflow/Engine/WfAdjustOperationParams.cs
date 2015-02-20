using System;
using System.Collections.Generic;
using System.Text;

using MCS.Library.OGUPermission;

namespace MCS.Library.Workflow.Engine
{
    public class WfSortBranchParams
    {
        private string _ProcessID;
        private int _Sequence;

        public WfSortBranchParams(string processID, int sequence)
        {
            this._ProcessID = processID;
            this._Sequence = sequence;
        }

        public string ProcessID
        {
            get { return _ProcessID; }
        }

        public int Sequence
        {
            get { return _Sequence; }
        }
    }

    public class WfAdjustBranchesParams
    {
        private List<string> _DeletedBranchIDs = new List<string>();
        private IUser _Operator;
        private WfBranchStartupParamsCollection<WfBranchStartupParams> _AddedBranchesParamsCollection = new WfBranchStartupParamsCollection<WfBranchStartupParams>();
        private List<WfSortBranchParams> _SortBranchParamsList = new List<WfSortBranchParams>();

        public WfBranchStartupParamsCollection<WfBranchStartupParams> AddedBranchesParamsCollection
        {
            get { return _AddedBranchesParamsCollection; }
        }

        public IUser User
        {
            get { return _Operator; }
            set { _Operator = value; }
        }


        public List<string> DeletedBranchIDs
        {
            get { return _DeletedBranchIDs; }
        }

        public List<WfSortBranchParams> SortBranchParamsList
        {
            get { return _SortBranchParamsList; }
        }
    }
}
