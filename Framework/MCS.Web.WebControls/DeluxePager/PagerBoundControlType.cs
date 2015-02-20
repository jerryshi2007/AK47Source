// -------------------------------------------------
// Assembly	��	MCS.Web.WebControls
// FileName	��	PagerBoundControlStatus.cs
// Remark	��  
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		�����	    20070815		����
// -------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace MCS.Web.WebControls
{
    /// <summary>
    /// ��ҳ�󶨿ؼ����Զ���
    /// </summary>
    /// <remarks>
    /// ��ҳ�󶨿ؼ����Զ���
    /// </remarks>
    public class PagerBoundControlStatus
    {
        private DataListControlType  controlMode;
        private bool isPagedControl;
        private bool isDataSourceControl;

        /// <summary>
        /// �ؼ�ģ�ͣ��磺GridView
        /// </summary>
        /// <remarks>
        /// �ؼ�ģ�ͣ��磺GridView
        /// </remarks>
        public DataListControlType  DataListControlType 
        {
            get { return this.controlMode; }
            set { this.controlMode = value; }
        }
        /// <summary>
        /// �󶨵Ŀؼ��Ƿ�Ϊ��ҳ�ؼ�
        /// </summary>
        /// <remarks>
        /// �󶨵Ŀؼ��Ƿ�Ϊ��ҳ�ؼ�
        /// </remarks>
        public bool IsPagedControl
        {
            get { return this.isPagedControl; }
            set { this.isPagedControl = value; }
        }
        /// <summary>
        /// �Ƿ�֧������Դ�ؼ�
        /// </summary>
        /// <remarks>
        /// �Ƿ�֧������Դ�ؼ�
        /// </remarks>
        public bool IsDataSourceControl
        {
            get { return this.isDataSourceControl; }
            set { this.isDataSourceControl = value; }
        }
    }

    /// <summary>
    /// ����ӿ�
    /// </summary>
    /// <remarks>
    /// ����ӿ�
    /// </remarks>
    public class PagerBoundControlType : IPagerBoundControlType
    {
        /// <summary>
        /// ��ȡ��ǰ�󶨵Ŀؼ���״̬����
        /// </summary>
        /// <param name="controlType">�ؼ�����</param>
        /// <returns>���ؿؼ����Զ���</returns>
        /// <remarks>
        /// ��ȡ��ǰ�󶨵Ŀؼ���״̬����
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
        /// �ؼ�����
        /// </summary>
        /// <param name="controlType"></param>
        /// <returns></returns>
        /// <remarks>
        /// �ؼ�����
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
        /// �Ƿ�������Ŀؼ�
        /// </summary>
        /// <param name="controlType"></param>
        /// <returns></returns>
        /// <remarks>
        /// �Ƿ�������Ŀؼ�
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
        /// ���ݿؼ����ͻ�ȡ��ǰ�ؼ��Ƿ���з�ҳ����
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        /// <remarks>
        /// ���ݿؼ����ͻ�ȡ��ǰ�ؼ��Ƿ���з�ҳ����
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
        /// ���ݿؼ���ȡ�Ƿ�֧�����ݷ��ʿؼ�
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        /// <remarks>
        /// ���ݿؼ���ȡ�Ƿ�֧�����ݷ��ʿؼ�
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
