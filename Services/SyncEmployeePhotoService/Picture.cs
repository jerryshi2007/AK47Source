using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SyncEmployeePhotoService
{
    public class Picture
    {
        public string LOGIN_NAME
        {
            get;
            set;
        }
        public string FILE_NAME
        {
            get;
            set;
        }

        public byte[] CONTENT_DATA
        {
            get;
            set;
        }
    }
}
