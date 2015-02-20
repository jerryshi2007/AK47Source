using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Services
{
    public class ThreadStatusService : MarshalByRefObject, IStatusService
    {
        public ThreadStatusService()
        {
        }

        #region IStatusService ≥…‘±

        public ServiceThreadCollection GetThreadStatus()
        {
            return MCSServiceMain.Instance.Threads;
        }

        #endregion
    }
}
