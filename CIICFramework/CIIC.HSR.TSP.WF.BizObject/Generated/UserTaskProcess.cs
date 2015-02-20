#region 作者版本
// -------------------------------------------------
// Assembly	：	HB.DataObjects
// FileName	：	UserTask.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    李苗	    20070723		创建
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace CIIC.HSR.TSP.WF.BizObject
{
	/// <summary>
	/// 待办箱实体类
	/// </summary>
    public partial class USER_TASKBO_PROCESS 
	{
        public string TASK_GUID { get; set; } 

        public string APPLICATION_NAME { get; set; } 

        public string PROGRAM_NAME { get; set; } 

        public int? TASK_LEVEL { get; set; } 

        public string TASK_TITLE { get; set; } 
   
        public string RESOURCE_ID { get; set; } 

        public string PROCESS_ID { get; set; } 
     
        public string ACTIVITY_ID { get; set; } 

        public string URL { get; set; } 

        public string DATA { get; set; }

        public int? EMERGENCY { get; set; }
    
        public string PURPOSE { get; set; } 
        public string STATUS { get; set; } 
    
        public DateTime? TASK_START_TIME { get; set; } 
      
        public DateTime? EXPIRE_TIME { get; set; } 
    
        public string SOURCE_ID { get; set; } 
    
        public string SOURCE_NAME { get; set; } 
      
        public string SEND_TO_USER { get; set; } 

        public string SEND_TO_USER_NAME { get; set; } 

        public DateTime? READ_TIME { get; set; } 
    
        public string CATEGORY_GUID { get; set; } 

        public int? TOP_FLAG { get; set; } 

        public string DRAFT_DEPARTMENT_NAME { get; set; } 
  
        public DateTime? DELIVER_TIME { get; set; } 

        public string DRAFT_USER_ID { get; set; } 

        public string DRAFT_USER_NAME { get; set; } 

        public string TenantCode { get; set; } 
   
        public string TaskType { get; set; } 

        public string DepartmentCode { get; set; } 

        public string DepartmentName { get; set; } 

        public string PROCESS_KEY { get; set; }

        public string PROCESS_NAME { get; set; }

        public string PROCESS_STATUS { get; set; }

        public DateTime Created { get; set; } 
        
	}

    public partial class USER_TASKBO_TOPUNPROCESS
    {

        public string TASK_TITLE { get; set; }
        public string URL { get; set; }
        public DateTime Created { get; set; }

        public int? TotalCnt { get; set; }

    }

     public partial class ProcessBO
     {
         [DataMember]
         public string EmailCollector { get; set; }
     }
}
