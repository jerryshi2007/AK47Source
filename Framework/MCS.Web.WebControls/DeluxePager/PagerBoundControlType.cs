// -------------------------------------------------
// Assembly	：	MCS.Web.WebControls
// FileName	：	PagerBoundControlStatus.cs
// Remark	：  
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		马泽锋	    20070815		创建
// -------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace MCS.Web.WebControls
{
    /// <summary>
    /// 翻页绑定控件属性对象
    /// </summary>
    /// <remarks>
    /// 翻页绑定控件属性对象
    /// </remarks>
    public class PagerBoundControlStatus
    {
        private DataListControlType  controlMode;
        private bool isPagedControl;
        private bool isDataSourceControl;

        /// <summary>
        /// 控件模型，如：GridView
        /// </summary>
        /// <remarks>
        /// 控件模型，如：GridView
        /// </remarks>
        public DataListControlType  DataListControlType 
        {
            get { return this.controlMode; }
            set { this.controlMode = value; }
        }
        /// <summary>
        /// 绑定的控件是否为翻页控件
        /// </summary>
        /// <remarks>
        /// 绑定的控件是否为翻页控件
        /// </remarks>
        public bool IsPagedControl
        {
            get { return this.isPagedControl; }
            set { this.isPagedControl = value; }
        }
        /// <summary>
        /// 是否支持数据源控件
        /// </summary>
        /// <remarks>
        /// 是否支持数据源控件
        /// </remarks>
        public bool IsDataSourceControl
        {
            get { return this.isDataSourceControl; }
            set { this.isDataSourceControl = value; }
        }
    }

    /// <summary>
    /// 对象接口
    /// </summary>
    /// <remarks>
    /// 对象接口
    /// </remarks>
    public class PagerBoundControlType : IPagerBoundControlType
    {
        /// <summary>
        /// 获取当前绑定的控件的状态对象
        /// </summary>
        /// <param name="controlType">控件类型</param>
        /// <returns>返回控件属性对象</returns>
        /// <remarks>
        /// 获取当前绑定的控件的状态对象
        /// </remarks>
        public PagerBoundControlStatus GetPagerBoundControl(Type controlType)
        {
            PagerBoundControlStatus pbControl = new PagerBoundControlStatus();

			pbControl.DataListControlType  = this.GetBoundControlMode(controlType);
            pbControl.IsPagedControl = this.GetIsPagedControl(pbControl.DataListControlType );
            pbControl.IsDataSourceControl = this.GetIsDataSourceControl(pbControl.DataListControlType);

            return pbControl;            
        }
        /// <summary>
        /// 控件类型
        /// </summary>
        /// <param name="controlType"></param>
        /// <returns></returns>
        /// <remarks>
        /// 控件类型
        /// </remarks>
        private DataListControlType  GetBoundControlMode(Type controlType)
        {
            if (controlType == typeof(DataGrid))
                return DataListControlType.DataGrid;
            else
			if (controlType == typeof(DeluxeGrid))
                return DataListControlType.DeluxeGrid;
            else
			if (controlType == typeof(DetailsView))
                return DataListControlType.DetailsView;
            else
			if (controlType == typeof(FormView))
                return DataListControlType.FormView;
            else
			if (controlType == typeof(GridView))
                return DataListControlType.GridView;
            else
			if (controlType == typeof(Repeater))
                return DataListControlType.Repeater;
            else
			if (controlType == typeof(Table))
                return DataListControlType.Table;
            else
			if (controlType == typeof(DataList))
                return DataListControlType.DataList;
            else
                return this.IsSubclassOfBoundControl(controlType); 
        }

        /// <summary>
        /// 是否派生类的控件
        /// </summary>
        /// <param name="controlType"></param>
        /// <returns></returns>
        /// <remarks>
        /// 是否派生类的控件
        /// </remarks>
        private DataListControlType IsSubclassOfBoundControl(Type controlType)
        {
            if (controlType.IsSubclassOf(typeof(DataGrid)))
                return DataListControlType.DataGrid;
            else
			if (controlType.IsSubclassOf(typeof(DeluxeGrid)))
                return DataListControlType.DeluxeGrid;
            else
			if (controlType.IsSubclassOf(typeof(DetailsView)))
                return DataListControlType.DetailsView;
            else
			if (controlType.IsSubclassOf(typeof(FormView)))
                return DataListControlType.FormView;
            else
			if (controlType.IsSubclassOf(typeof(GridView)))
                return DataListControlType.GridView;
            else
			if (controlType.IsSubclassOf(typeof(Repeater)))
                return DataListControlType.Repeater;
            else
			if (controlType.IsSubclassOf(typeof(Table)))
                return DataListControlType.Table;
            else
			if (controlType.IsSubclassOf(typeof(DataList)))
                return DataListControlType.DataList;
            else
                return DataListControlType.Nothing;
        }

        /// <summary>
        /// 根据控件类型获取当前控件是否具有翻页功能
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        /// <remarks>
        /// 根据控件类型获取当前控件是否具有翻页功能
        /// </remarks>
        private bool GetIsPagedControl(DataListControlType  mode)
        {
            bool result = false;
            switch (mode)
            {
                case DataListControlType .DataGrid:
                case DataListControlType .DeluxeGrid:
                case DataListControlType .DetailsView:
                case DataListControlType .FormView:
                case DataListControlType .GridView:
                    result = true;
                    break;
            }

            return result;
        }

        /// <summary>
        /// 根据控件获取是否支持数据访问控件
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        /// <remarks>
        /// 根据控件获取是否支持数据访问控件
        /// </remarks>
        private bool GetIsDataSourceControl(DataListControlType mode)
        {
            bool result = false;

            switch (mode)
            {
                case DataListControlType.DataGrid:
                case DataListControlType.DeluxeGrid:
                case DataListControlType.DetailsView:
                case DataListControlType.FormView:
                case DataListControlType.GridView:
                case DataListControlType.Repeater:
                case DataListControlType.DataList:
                case DataListControlType.Table:
					result = true;
                    break;
            }

			return result;
        }
    }
}
