using CIIC.HSR.TSP.IoC;
using CIIC.HSR.TSP.WF.UI.Control.Interfaces;
using CIICWFControlTest.Bridges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CIICWFControlTest.App_Start
{
    public class IoCConfig
    {
        public static void Start()
        {
            Containers.Global.AddConfigures(WebSiteIoCConfigure.Create
             (
                 sl =>
                 {
                     sl.Register<IWFUserContext, DeluxeWfUserContext>();
                 }
            ));

            //Containers.Global.Singleton.Register<IWFUserContext, DeluxeWfUserContext>();
        }
    }

    public class WebSiteIoCConfigure : IIoCConfigure
    {
        public static WebSiteIoCConfigure Create(Action<IIoCContainer> configureAction)
        {
            return new WebSiteIoCConfigure { _configureAction = configureAction };
        }

        Action<IIoCContainer> _configureAction;

        public void Configure(IIoCContainer container)
        {
            if (_configureAction != null)
            {
                _configureAction(container);
            }
        }
    }
}