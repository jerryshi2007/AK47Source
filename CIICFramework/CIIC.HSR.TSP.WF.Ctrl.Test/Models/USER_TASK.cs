namespace CIIC.HSR.TSP.WF.Ctrl.Test.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WF.USER_TASK")]
    public partial class USER_TASK
    {
        [Key]
        [StringLength(36)]
        public string TASK_GUID { get; set; }

        [StringLength(64)]
        public string APPLICATION_NAME { get; set; }

        [StringLength(64)]
        public string PROGRAM_NAME { get; set; }

        public int? TASK_LEVEL { get; set; }

        [StringLength(1024)]
        public string TASK_TITLE { get; set; }

        [StringLength(36)]
        public string RESOURCE_ID { get; set; }

        [StringLength(36)]
        public string PROCESS_ID { get; set; }

        [StringLength(36)]
        public string ACTIVITY_ID { get; set; }

        [StringLength(2048)]
        public string URL { get; set; }

        public string DATA { get; set; }

        public int? EMERGENCY { get; set; }

        [StringLength(64)]
        public string PURPOSE { get; set; }

        [StringLength(50)]
        public string STATUS { get; set; }

        public DateTime? TASK_START_TIME { get; set; }

        public DateTime? EXPIRE_TIME { get; set; }

        [StringLength(36)]
        public string SOURCE_ID { get; set; }

        [StringLength(64)]
        public string SOURCE_NAME { get; set; }

        [StringLength(36)]
        public string SEND_TO_USER { get; set; }

        [StringLength(64)]
        public string SEND_TO_USER_NAME { get; set; }

        public DateTime? READ_TIME { get; set; }

        [StringLength(36)]
        public string CATEGORY_GUID { get; set; }

        public int? TOP_FLAG { get; set; }

        [StringLength(512)]
        public string DRAFT_DEPARTMENT_NAME { get; set; }

        public DateTime? DELIVER_TIME { get; set; }

        [StringLength(36)]
        public string DRAFT_USER_ID { get; set; }

        [StringLength(64)]
        public string DRAFT_USER_NAME { get; set; }
    }
}
