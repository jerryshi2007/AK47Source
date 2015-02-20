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
    // TSPWF_USER_ACCOMPLISHED_TASK
    [DataContract(IsReference=true)]
    [MappedToAttribute("TSPWF_USER_ACCOMPLISHED_TASK")]
    [BizObjectCode("TSPWF_USER_ACCOMPLISHED_TASK")]
    public partial class USER_ACCOMPLISHED_TASKBO : BizObjectBase<USER_ACCOMPLISHED_TASKBO>
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
            return "TSPWF_USER_ACCOMPLISHED_TASK";
        }
		#endregion
		
        //[DataMember(Order = 1, IsRequired = true)]
        [DataMember]
        public string TASK_GUID { get; set; } // TASK_GUID (Primary key)

        //[DataMember(Order = 2, IsRequired = false)]
        [DataMember]
        public string APPLICATION_NAME { get; set; } // APPLICATION_NAME

        //[DataMember(Order = 3, IsRequired = false)]
        [DataMember]
        public string PROGRAM_NAME { get; set; } // PROGRAM_NAME

        //[DataMember(Order = 4, IsRequired = false)]
        [DataMember]
        public int? TASK_LEVEL { get; set; } // TASK_LEVEL

        //[DataMember(Order = 5, IsRequired = false)]
        [DataMember]
        public string TASK_TITLE { get; set; } // TASK_TITLE

        //[DataMember(Order = 6, IsRequired = false)]
        [DataMember]
        public string RESOURCE_ID { get; set; } // RESOURCE_ID

        //[DataMember(Order = 7, IsRequired = false)]
        [DataMember]
        public string PROCESS_ID { get; set; } // PROCESS_ID

        //[DataMember(Order = 8, IsRequired = false)]
        [DataMember]
        public string ACTIVITY_ID { get; set; } // ACTIVITY_ID

        //[DataMember(Order = 9, IsRequired = false)]
        [DataMember]
        public string URL { get; set; } // URL

        //[DataMember(Order = 10, IsRequired = false)]
        [DataMember]
        public string DATA { get; set; } // DATA

        //[DataMember(Order = 11, IsRequired = false)]
        [DataMember]
        public int? EMERGENCY { get; set; } // EMERGENCY

        //[DataMember(Order = 12, IsRequired = false)]
        [DataMember]
        public string PURPOSE { get; set; } // PURPOSE

        //[DataMember(Order = 13, IsRequired = false)]
        [DataMember]
        public string STATUS { get; set; } // STATUS

        //[DataMember(Order = 14, IsRequired = false)]
        [DataMember]
        public DateTime? TASK_START_TIME { get; set; } // TASK_START_TIME

        //[DataMember(Order = 15, IsRequired = false)]
        [DataMember]
        public DateTime? EXPIRE_TIME { get; set; } // EXPIRE_TIME

        //[DataMember(Order = 16, IsRequired = false)]
        [DataMember]
        public string SOURCE_ID { get; set; } // SOURCE_ID

        //[DataMember(Order = 17, IsRequired = false)]
        [DataMember]
        public string SOURCE_NAME { get; set; } // SOURCE_NAME

        //[DataMember(Order = 18, IsRequired = false)]
        [DataMember]
        public string SEND_TO_USER { get; set; } // SEND_TO_USER

        //[DataMember(Order = 19, IsRequired = false)]
        [DataMember]
        public string SEND_TO_USER_NAME { get; set; } // SEND_TO_USER_NAME

        //[DataMember(Order = 20, IsRequired = false)]
        [DataMember]
        public DateTime? READ_TIME { get; set; } // READ_TIME

        //[DataMember(Order = 21, IsRequired = false)]
        [DataMember]
        public string CATEGORY_GUID { get; set; } // CATEGORY_GUID

        //[DataMember(Order = 22, IsRequired = false)]
        [DataMember]
        public int? TOP_FLAG { get; set; } // TOP_FLAG

        //[DataMember(Order = 23, IsRequired = false)]
        [DataMember]
        public string DRAFT_DEPARTMENT_NAME { get; set; } // DRAFT_DEPARTMENT_NAME

        //[DataMember(Order = 24, IsRequired = false)]
        [DataMember]
        public DateTime? DELIVER_TIME { get; set; } // DELIVER_TIME

        //[DataMember(Order = 25, IsRequired = false)]
        [DataMember]
        public string DRAFT_USER_ID { get; set; } // DRAFT_USER_ID

        //[DataMember(Order = 26, IsRequired = false)]
        [DataMember]
        public string DRAFT_USER_NAME { get; set; } // DRAFT_USER_NAME

        //[DataMember(Order = 27, IsRequired = false)]
        [DataMember]
        public string TenantCode { get; set; } // TenantCode

        //[DataMember(Order = 28, IsRequired = false)]
        [DataMember]
        public string TaskType { get; set; } // TaskType

        //[DataMember(Order = 29, IsRequired = false)]
        [DataMember]
        public string DepartmentCode { get; set; } // DepartmentCode

        //[DataMember(Order = 30, IsRequired = false)]
        [DataMember]
        public string DepartmentName { get; set; } // DepartmentName


        public USER_ACCOMPLISHED_TASKBO():this(null)
        {
            TASK_START_TIME = System.DateTime.Now;
            TOP_FLAG = 0;
            DELIVER_TIME = System.DateTime.Now;
            InitializePartial();
        }

		public USER_ACCOMPLISHED_TASKBO(BusinessObjectView metadataView):base(metadataView)
		{
		
		}
        partial void InitializePartial();
    }

}
