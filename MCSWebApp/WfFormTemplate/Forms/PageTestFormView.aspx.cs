﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.MVC;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Principal;
using System.Reflection;
using System.Threading;
using MCS.Web.Library.Script;
using MCS.Web.WebControls;
using WfFormTemplate.DataObjects;

namespace WfFormTemplate.Forms
{
    public partial class PageTestFormView : ViewBase<DynamicFormCommandState>
    {
        protected override void CommandStateInitialized()
        {
            //bindingControl.Data = this.ViewData.Data;
            if (!IsPostBack)
            {
               
            }
        }

        protected void moveToControl_AfterCreateExecutor(WfExecutorBase executor)
        {
            executor.PrepareMoveToTasks += new PrepareTasksEventHandler(executor_PrepareMoveToTasks);
            executor.BeforeExecute += new ExecutorEventHandler(executor_BeforeExecute);
            executor.SaveApplicationData += new ExecutorEventHandler(executor_SaveApplicationData);
            executor.PrepareApplicationData += new ExecutorEventHandler(executor_PrepareApplicationData);
            executor.AfterSaveApplicationData += new ExecutorEventHandler(executor_AfterSaveApplicationData);
        }
        protected void moveToControl_ProcessChanged(IWfProcess process)
        {


        }

        void executor_PrepareApplicationData(WfExecutorDataContext dataContext)
        {
            //bindingControl.CollectData(true);
        }

        void executor_SaveApplicationData(WfExecutorDataContext dataContext)
        {

        }

        void executor_BeforeExecute(WfExecutorDataContext dataContext)
        {


        }

        //protected override void OnPreRender(EventArgs e)
        //{

        //    base.OnPreRender(e);
        //    this.Form.Controls.Add(new ClientGrid() { ID = "1111" });
        //}

        void executor_PrepareMoveToTasks(WfExecutorDataContext dataContext, UserTaskCollection tasks)
        {

        }

        void executor_AfterSaveApplicationData(WfExecutorDataContext dataContext)
        {

        }


    }
}