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
    // TSPWF_USER_OPERATION_LOG
    [DataContract(IsReference=true)]
    [MappedToAttribute("TSPWF_USER_OPERATION_LOG")]
    [BizObjectCode("TSPWF_USER_OPERATION_LOG")]
    public partial class USER_OPERATION_LOGBO : BizObjectBase<USER_OPERATION_LOGBO>
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
            return "TSPWF_USER_OPERATION_LOG";
        }
		#endregion
		
        //[DataMember(Order = 1, IsRequired = true)]
        [DataMember]
        public int ID { get; set; } // ID

        //[DataMember(Order = 2, IsRequired = false)]
        [DataMember]
        public string RESOURCE_ID { get; set; } // RESOURCE_ID

        //[DataMember(Order = 3, IsRequired = false)]
        [DataMember]
        public string SUBJECT { get; set; } // SUBJECT

        //[DataMember(Order = 4, IsRequired = false)]
        [DataMember]
        public string APPLICATION_NAME { get; set; } // APPLICATION_NAME

        //[DataMember(Order = 5, IsRequired = false)]
        [DataMember]
        public string PROGRAM_NAME { get; set; } // PROGRAM_NAME

        //[DataMember(Order = 6, IsRequired = false)]
        [DataMember]
        public string PROCESS_ID { get; set; } // PROCESS_ID

        //[DataMember(Order = 7, IsRequired = false)]
        [DataMember]
        public string ACTIVITY_ID { get; set; } // ACTIVITY_ID

        //[DataMember(Order = 8, IsRequired = false)]
        [DataMember]
        public string ACTIVITY_NAME { get; set; } // ACTIVITY_NAME

        //[DataMember(Order = 9, IsRequired = false)]
        [DataMember]
        public string OPERATOR_ID { get; set; } // OPERATOR_ID

        //[DataMember(Order = 10, IsRequired = false)]
        [DataMember]
        public string OPERATOR_NAME { get; set; } // OPERATOR_NAME

        //[DataMember(Order = 11, IsRequired = false)]
        [DataMember]
        public string TOP_DEPT_ID { get; set; } // TOP_DEPT_ID

        //[DataMember(Order = 12, IsRequired = false)]
        [DataMember]
        public string TOP_DEPT_NAME { get; set; } // TOP_DEPT_NAME

        //[DataMember(Order = 13, IsRequired = false)]
        [DataMember]
        public string REAL_USER_ID { get; set; } // REAL_USER_ID

        //[DataMember(Order = 14, IsRequired = false)]
        [DataMember]
        public string REAL_USER_NAME { get; set; } // REAL_USER_NAME

        //[DataMember(Order = 15, IsRequired = true)]
        [DataMember]
        public DateTime OPERATE_DATETIME { get; set; } // OPERATE_DATETIME

        //[DataMember(Order = 16, IsRequired = false)]
        [DataMember]
        public string OPERATE_NAME { get; set; } // OPERATE_NAME

        //[DataMember(Order = 17, IsRequired = false)]
        [DataMember]
        public string OPERATE_TYPE { get; set; } // OPERATE_TYPE

        //[DataMember(Order = 18, IsRequired = false)]
        [DataMember]
        public string OPERATE_DESCRIPTION { get; set; } // OPERATE_DESCRIPTION

        //[DataMember(Order = 19, IsRequired = false)]
        [DataMember]
        public string HTTP_CONTEXT { get; set; } // HTTP_CONTEXT

        //[DataMember(Order = 20, IsRequired = false)]
        [DataMember]
        public string CORRELATION_ID { get; set; } // CORRELATION_ID

    }

}
