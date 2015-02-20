using System;
using System.Collections.ObjectModel;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using System.Diagnostics;
using System.Text;


namespace CheckCompileStatus
{
    /// <summary>
    /// TFSHelper
    /// </summary>
    public static class TFSHelper
    {
        /// <summary>
        /// GetLatestVersion
        /// </summary>
        /// <param name="userName">UserName</param>
        /// <param name="password">Password</param>
        /// <param name="domain">DomainName</param>
        /// <param name="uri">TFS Server Uri</param>
        /// <param name="workspaceName">WorkSpace Name</param>
        /// <param name="workspaceServerPath">Server Path</param>
        /// <param name="all">GetAll OR GetNew</param>
        public static void GetLatestVersion(string userName, string password, string domain, Uri uri, string workspaceName, string workspaceServerPath, bool all)
        {
            int current = 0;
            int count = 0;

            var projectCollection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(uri, new ConnectByImplementingCredentialsProvider(userName, password, domain));
            projectCollection.EnsureAuthenticated();

            var versionControl = projectCollection.GetService<VersionControlServer>();
            versionControl.Getting += delegate(object sender, GettingEventArgs e)
            {
                current++;
                ConsoleColorHelper.Set(MessageType.High);
                Console.Write(Resource.Default.GetLatestVersionProcessInfo, current, count);
                ConsoleColorHelper.Reset();
                Console.WriteLine(Resource.Default.GetLatestVersionProcessInfo2, e.TargetLocalItem);
            };
            var workspace = versionControl.GetWorkspace(workspaceName, versionControl.AuthorizedUser);

            ConsoleColorHelper.Set(MessageType.High);
            Console.WriteLine();
            Console.WriteLine(Resource.Default.StartthedownloadInfo);
            ConsoleColorHelper.Reset();

            StringBuilder sbEventLog = new StringBuilder();
            sbEventLog.AppendFormat("ChcekComplieStatus tool, getting files. \r\nStart at: {0}\r\n", DateTime.Now.ToLongTimeString());
            EventLogEntryType logType = EventLogEntryType.Information;
            GetStatus getStatus;
            if (all)
            {
                 getStatus = workspace.Get(new GetRequest(workspaceServerPath, RecursionType.Full, LatestVersionSpec.Instance), GetOptions.Overwrite | GetOptions.GetAll,
                    delegate(Workspace space, ILocalUpdateOperation[] operations, object userdata)
                    {
                        count = operations.Length;
                        ConsoleColorHelper.Set(MessageType.High);
                        Console.WriteLine();
                        Console.Write(count);
                        ConsoleColorHelper.Reset();
                        Console.WriteLine(Resource.Default.ItemsToDownloadInfo);
                    },
                null);
            }
            else
            {
                getStatus = workspace.Get(new GetRequest(workspaceServerPath, RecursionType.Full, LatestVersionSpec.Instance), GetOptions.Overwrite | GetOptions.GetAll,
                    delegate(Workspace space, ILocalUpdateOperation[] operations, object userdata)
                    {
                        count = operations.Length;
 
                        //Event Log, Add by Aqee Li, 130101
                        int countIgore=0;
                        foreach (ILocalUpdateOperation opt in operations)
                            if ((opt.VersionLocal == opt.VersionServer) && opt.IsConflict != true)
                            {
                                opt.Ignore = true;
                                countIgore++;
                            }
                        sbEventLog.AppendLine( string.Format("Total files: {0}, ignored: {1}",count, countIgore));

                        ConsoleColorHelper.Set(MessageType.High);
                        Console.WriteLine();
                        Console.Write(count);
                        ConsoleColorHelper.Reset();
                        Console.WriteLine(Resource.Default.ItemsToDownloadInfo);
                        Console.WriteLine();
                        Console.Write(countIgore);
                        Console.WriteLine(Resource.Default.ItemsIgnoredInfo);
                    },
                    null);
            }

            //Event Log, Add by Aqee Li, 130101
            if (getStatus.NumOperations != 0)
                sbEventLog.AppendFormat(", Operations: {0} \r\n", getStatus.NumOperations);
            if (getStatus.NumUpdated != 0)
                sbEventLog.AppendFormat(", Updated: {0} \r\n", getStatus.NumUpdated);
            if (getStatus.NumWarnings != 0)
            {
                sbEventLog.AppendFormat(", Warnings: {0} \r\n", getStatus.NumWarnings);
                logType = EventLogEntryType.Warning;
            }
            if (getStatus.NumConflicts != 0)
            {
                sbEventLog.AppendFormat(", Operations: {0} \r\n", getStatus.NumConflicts);
                logType = EventLogEntryType.Warning;
            }
            if (getStatus.NumFailures != 0) 
            {
                sbEventLog.AppendFormat(", Failures: {0}. \r\n Failures:\r\n", getStatus.NumFailures);
                logType = EventLogEntryType.Error;
                foreach (Failure failure in getStatus.GetFailures())
                    sbEventLog.AppendFormat("\t{0}\r\n",failure.GetFormattedMessage());
            }

            sbEventLog.AppendFormat("Finished get files at: {0}\r\n", DateTime.Now.ToLongTimeString());
            EventLogHelper.WriteEventLog(sbEventLog.ToString(), logType);

            ConsoleColorHelper.Set(MessageType.High);
            Console.Write(getStatus.NumOperations);
            ConsoleColorHelper.Reset();
            Console.WriteLine(Resource.Default.ItemsGetSucceedInfo);
        }

        /// <summary>
        /// GetAlias
        /// </summary>
        /// <param name="userName">User Name</param>
        /// <param name="password">Password</param>
        /// <param name="domain">Domain Name</param>
        /// <param name="uri">TFS Server Uri</param>
        /// <param name="errors">Compile errors</param>
        /// <param name="warnings">Compile warnings</param>
        public static void GetAlias(string userName, string password, string domain, Uri uri, Collection<CompileResult> errors, Collection<CompileResult> warnings)
        {
            var projectCollection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(uri, new ConnectByImplementingCredentialsProvider(userName, password, domain));
            projectCollection.EnsureAuthenticated();

            var versionControl = projectCollection.GetService<VersionControlServer>();
            if (errors != null)
            {
                foreach (var error in errors)
                {
                    var item = versionControl.GetItem(error.ServerPath);
                    var changeset = versionControl.GetChangeset(item.ChangesetId);
                    error.Owner = changeset.Owner;
                    error.CreationDate = changeset.CreationDate;
                }
            }

            if (warnings != null)
            {
                foreach (var warning in warnings)
                {
                    var item = versionControl.GetItem(warning.ServerPath);
                    var changeset = versionControl.GetChangeset(item.ChangesetId);
                    warning.Owner = changeset.Owner;
                    warning.CreationDate = changeset.CreationDate;
                }
            }
        }
    }
}
