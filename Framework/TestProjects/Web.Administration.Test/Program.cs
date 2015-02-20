using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Web.Administration.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            RunspaceConfiguration runSpaceConfiguration = RunspaceConfiguration.Create();

            using (Runspace runSpace = RunspaceFactory.CreateRunspace(runSpaceConfiguration))
            {
                runSpace.Open();

                RunScript(runSpace, "[System.Reflection.Assembly]::LoadFrom(\"C:\\windows\\system32\\inetsrv\\Microsoft.Web.Administration.dll\" )");
                RunScript(runSpace, "(New-Object Microsoft.Web.Administration.ServerManager).Sites | Format-List");
            }

            
            ServerManager manager = new ServerManager();

            foreach (Site site in manager.Sites)
            {
                Console.WriteLine("Site: {0}", site.Name);
            }

            //Site defaultSite = manager.Sites.FirstOrDefault();

            //Application app = defaultSite.Applications.FirstOrDefault(a => a.Path == "/CATest");

            //if (app != null)
            //    defaultSite.Applications.Remove(app);

            //app = defaultSite.Applications.Add("/CATest", @"C:\inetpub\CATest");


            //manager.CommitChanges();

            foreach (ApplicationPool pool in manager.ApplicationPools)
            {
                Console.WriteLine("Application Pool: {0}, Pipeline Mode: {1}", pool.Name, pool.ManagedPipelineMode);
            }
        }

        private static void RunScript(Runspace runSpace, string script)
        {
            RunspaceInvoke scriptInvoker = new RunspaceInvoke(runSpace);
            Pipeline pipeline = runSpace.CreatePipeline();
            pipeline.Commands.AddScript(script);

            Collection<PSObject> psObjects = pipeline.Invoke();

            if (pipeline.Error.Count > 0)
                throw new Exception("脚本执行失败");

            foreach (PSObject pso in psObjects)
                Console.WriteLine(pso.ToString());
        }
    }
}
