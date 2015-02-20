using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CIIC.HSR.TSP.WF.Bizlet.Common;

namespace CIIC.HSR.TSP.WF.BizObject
{
    public class UserTaskQueryCondition
    {
        //待办 已办   
       public Bizlet.Common.TaskStatus TaskType { get; set; }

        //流程状态   
       public WfProcessStatus ProcessStatus { get; set; }
        
        //任务名称
        public string TaskTitle { get; set; }

        //部门编号
        public string DepartmentCode { get; set; } 

        //分类
        public string ApplicationName { get; set; }

         //分类
        public string ProgramName { get; set; } 

        //提交时间FROM
        public DateTime? CreatedFrom { get; set; } 

        //提交时间TO
        public DateTime? CreatedTo { get; set; }
        //创建人
        public string CreatorUserId { get; set; } 

    }
}
