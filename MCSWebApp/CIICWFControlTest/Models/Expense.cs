using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CIICWFControlTest.Models
{
    [Serializable]
    public class Expense
    {
        public string ID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Department
        {
            get;
            set;
        }

        [Required(ErrorMessage = "金额必填")]
        public float Amount
        {
            get;
            set;
        }

        public DateTime TransitionDate
        {
            get;
            set;
        }
    }
}