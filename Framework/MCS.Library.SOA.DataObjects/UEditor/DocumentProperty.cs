using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
    public class DocumentProperty
    {
        public DocumentProperty()
        {
            docImages = new DocumentImageList();
            //docImageProps = new ImagePropertyCollection();
        }

        #region 私有
        private DocumentImageList docImages;
        private string id;
        //private ImagePropertyCollection docImageProps;
        #endregion

        #region 属性
        public string InitialData
        {
            get;
            set;
        }

        public DocumentImageList DocumentImages
        {
            get
            {
                return docImages;
            }
            set
            {
                ExceptionHelper.FalseThrow<ArgumentNullException>(value != null, "docImages");
                docImages = value;
            }
        }

        //public ImagePropertyCollection DocImageProps
        //{
        //    get
        //    {
        //        return this.docImageProps;
        //    }
        //    set
        //    {
        //        ExceptionHelper.FalseThrow<ArgumentNullException>(value != null, "DocImageProps");
        //        docImageProps = value;
        //    }
        //}

        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }

        }
        #endregion
    }
}
