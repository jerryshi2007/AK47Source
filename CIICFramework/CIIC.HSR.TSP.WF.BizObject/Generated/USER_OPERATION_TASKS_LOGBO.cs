// ReSharper disable RedundantUsingDirective
// ReSharper disable DoNotCallOverridableMethodsInConstructor
// ReSharper disable InconsistentNaming
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable PartialMethodWithSinglePart
// ReSharper disable RedundantNameQualifier
using CIIC.HSR.TSP.DataAccess.EF;
using CIIC.HSR.TSP.DataAccess;
using CIIC.HSR.TSP.Models;
using CIIC.HSR.TSP.IoC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Runtime.Serialization;
//using DatabaseGeneratedOption = System.ComponentModel.DataAnnotations.DatabaseGeneratedOption;

namespace CIIC.HSR.TSP.WF.BizObject
{
    // TSPWF_USER_OPERATION_TASKS_LOG
    [DataContract(IsReference=true)]
    [MappedToAttribute("TSPWF_USER_OPERATION_TASKS_LOG")]
    [BizObjectCode("TSPWF_USER_OPERATION_TASKS_LOG")]
    public partial class USER_OPERATION_TASKS_LOGBO : BizObjectBase<USER_OPERATION_TASKS_LOGBO>
    {
		#region Impl abstract
		
		private AdditionalFields _additionalFields;
		
        [DataMember]
        public override AdditionalFields AdditionalFields
        {
            get { return _additionalFields; }
            set { _additionalFields = value; }
        }

        public override string GetBusinessName()
        {
            return "TSPWF_USER_OPERATION_TASKS_LOG";
        }
		#endregion
		
        //[DataMember(Order = 1, IsRequired = true)]
        [DataMember]
        public int LOG_ID { get; set; } // LOG_ID

        //[DataMember(Order = 2, IsRequired = true)]
        [DataMember]
        public string TASK_ID { get; set; } // TASK_ID

        //[DataMember(Order = 3, IsRequired = true)]
        [DataMember]
        public string SEND_TO_USER_ID { get; set; } // SEND_TO_USER_ID

        //[DataMember(Order = 4, IsRequired = false)]
        [DataMember]
        public string SEND_TO_USER_NAME { get; set; } // SEND_TO_USER_NAME

    }

}
