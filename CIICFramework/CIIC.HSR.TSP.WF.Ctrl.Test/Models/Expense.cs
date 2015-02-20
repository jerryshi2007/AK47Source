using MCS.Library.WF.Contracts.Ogu;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CIIC.HSR.TSP.WF.Ctrl.Test.Models
{
    public class Expense
    {
        
        public Guid ID { get; set; }
        public string Name { get; set; }
        [Required(ErrorMessage= "部门必填！")]
        public string Department { get; set; }
        [Required(ErrorMessage="金额必填")]
        public int Amount { get; set; }
        public DateTime TransitionDate { get; set; }
        public string ProcessId { get; set; }


        public Dictionary<string, List<WfClientUser>> DictionaryWfClientUser
        {
            get;
            set;
        }

        
    }
}