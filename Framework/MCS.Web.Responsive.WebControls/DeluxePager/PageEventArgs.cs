// -------------------------------------------------
// Assembly	��	MCS.Web.Responsive.WebControls
// FileName	��	PageEventArgs.cs
// Remark	��  
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		�����	    20070815		����
// -------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Web.Responsive.WebControls
{
    /// <summary>
    /// ����ӿ�
    /// </summary>
    /// <remarks>
    /// ����ӿ�
    /// </remarks>
    public class PageEventArgs:IPageEventArgs
    {
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <remarks>
        /// ���캯��
        /// </remarks>
        public PageEventArgs()
        {
            // 
            // TODO: 
            //
        }

        /// <summary>
        /// ��ȡ�󶨷�ҳ�ؼ���Ӧ�ؼ��ķ�ҳ�¼�Args
        /// </summary>
        /// <param name="controlMode"></param>
        /// <param name="eventName"></param>
        /// <param name="commandSource"></param>
        /// <param name="newPageIndex"></param>
        /// <returns></returns>
        /// <remarks>
        /// ��ȡ�󶨷�ҳ�ؼ���Ӧ�ؼ��ķ�ҳ�¼�Args
        /// </remarks>
        public object GetPageEventArgs(DataListControlType  controlMode, string eventName, object commandSource, int newPageIndex)
        { 
            object obj = null;
            switch (controlMode)
            {
                case DataListControlType .GridView: 
                    switch (eventName)
                    {
                        case "EventPageIndexChanged":
                            EventArgs gvEventArgs = new EventArgs();
                            obj = (object)gvEventArgs;
                            break;
                        case "EventPageIndexChanging":
                            GridViewPageEventArgs gridViewEventArgs = new GridViewPageEventArgs(newPageIndex);
                            obj = (object)gridViewEventArgs;
                            break;
                    }

                    break;
                case DataListControlType .DeluxeGrid:
                    switch (eventName)
                    {
                        case "EventPageIndexChanged":
                            EventArgs gvEventArgs = new EventArgs();
                            obj = (object)gvEventArgs;
                            break;
                        case "EventPageIndexChanging":
                            GridViewPageEventArgs gridViewEventArgs = new GridViewPageEventArgs(newPageIndex);
                            obj = (object)gridViewEventArgs;
                            break;
                    }

                    break;

                case DataListControlType .DetailsView:
                    //DetailsViewPageEventArgs detailsViewEventArgs = new DetailsViewPageEventArgs(newPageIndex);
                    //obj = (object)detailsViewEventArgs;
                    switch (eventName)
                    {
                        case "EventPageIndexChanged":
                            EventArgs detailsViewEventArgs = new EventArgs();
                            obj = (object)detailsViewEventArgs;
                            break;
                        case "EventPageIndexChanging":
                            DetailsViewPageEventArgs dvEventArgs = new DetailsViewPageEventArgs(newPageIndex);
                            obj = (object)dvEventArgs;
                            break;
                    }

                    break;

                case DataListControlType .FormView:
                    //DetailsViewPageEventArgs detailsViewEventArgs = new DetailsViewPageEventArgs(newPageIndex);
                    //obj = (object)detailsViewEventArgs;
                    switch (eventName)
                    {
                        case "EventPageIndexChanged":
                            EventArgs formViewEventArgs = new EventArgs();
                            obj = (object)formViewEventArgs;
                            break;
                        case "EventPageIndexChanging":
                            FormViewPageEventArgs fvEventArgs = new FormViewPageEventArgs(newPageIndex);
                            obj = (object)fvEventArgs;
                            break;
                    }

                    break;

                case DataListControlType .DataGrid:
                    DataGridPageChangedEventArgs dataGridEventArgs = new DataGridPageChangedEventArgs(commandSource, newPageIndex);
                    obj = (object)dataGridEventArgs;

                    break;
            }
            return obj; 
        }

        /// <summary>
        /// ���ð󶨶�Ӧ�ؼ��ķ�ҳ����
        /// </summary>
        /// <param name="objControl"></param>
        /// <param name="controlMode"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <remarks>
        /// ���ð󶨶�Ӧ�ؼ��ķ�ҳ����
        /// </remarks>
        public bool SetBoundControlPagerSetting(object objControl, DataListControlType  controlMode, int pageSize)
        {
            bool bl = true;
            if (objControl == null || pageSize <= 0)
                return false;
            switch (controlMode)
            {
                case DataListControlType.DeluxeGrid:
                    DeluxeGrid dg = (DeluxeGrid)objControl;
                    dg.PageSize = pageSize;
                    break;

                case DataListControlType .GridView:
                    GridView gv = (GridView)objControl;
                    gv.PageSize = pageSize; 
                    break;                

                case DataListControlType .DataGrid:
                    DataGrid dgOld = (DataGrid)objControl;
                    dgOld.PageSize = pageSize;                    
                    break;

                default:
                    bl = false;
                    break;
            }
            return bl; 
        }
    }
}
