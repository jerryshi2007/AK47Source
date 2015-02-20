using System;
using System.Collections.Generic;

namespace MCS.Library.Data
{
    /// <summary>
    /// ���ݷ��ʹ��̵�ί�з������塣
    /// </summary>
    /// <param name="sender">�¼�Դ</param>
    /// <param name="e">�¼�����</param>
    public delegate void DbEventHandler(object sender, DbEventArgs e);


    /// <summary>
    /// ���ݷ��ʹ��̵��¼�����
    /// </summary>
    public class DbEventArgs : EventArgs
    {
        private object executor;

        /// <summary>
        /// ���ݷ��ʷ����Ĳ�ͬ��������Command����DataAdapter�����ɾ����Database������󶨡�
        /// </summary>
        public virtual object Executor
        {
            get 
            {
                return this.executor; 
            }
            set
            {
                this.executor = value; 
            }
        }
    }
}
