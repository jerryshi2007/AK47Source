using MCS.Library.Core;
using MCS.Library.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl.Ogu
{
    //[Serializable]
    //public class WSUpdatableRequestSoapMessage : SimpleRequestSoapMessage
    //{
    //    private bool _WithLock;

    //    public WSUpdatableRequestSoapMessage(System.IO.Stream stream)
    //        : base(stream)
    //    {
    //    }

    //    public bool WithLock
    //    {
    //        get
    //        {
    //            return this._WithLock;
    //        }
    //        set
    //        {
    //            this._WithLock = value;
    //        }
    //    }

    //    protected override void ReadHeaderNode(XmlNamespaceManager namespaceManager, XmlNode headerNode)
    //    {
    //        base.ReadHeaderNode(namespaceManager, headerNode);

    //        this._WithLock = XmlHelper.GetSingleNodeValue(headerNode, "t:SchemaServiceUpdatableClientSoapHeader/t:WithLock", namespaceManager, true);
    //    }
    //}
}
