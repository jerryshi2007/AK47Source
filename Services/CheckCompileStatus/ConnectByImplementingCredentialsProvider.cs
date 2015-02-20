using System;
using System.Net;
using Microsoft.TeamFoundation.Client;

namespace CheckCompileStatus
{
    public class ConnectByImplementingCredentialsProvider : ICredentialsProvider
    {
        private string userName;
        private string password;
        private string domain;

        public ConnectByImplementingCredentialsProvider(string userName, string password, string domain)
        {
            this.userName = userName;
            this.password = password;
            this.domain = domain;
        }

        public ICredentials GetCredentials(Uri uri, ICredentials failedCredentials)
        {
            ConsoleColorHelper.Set(MessageType.High);
            Console.WriteLine();
            Console.Write(uri);
            ConsoleColorHelper.Reset();
            Console.WriteLine(Resource.Default.GetCredentialsInfo);
            return new NetworkCredential(userName, password, domain);
        }

        public void NotifyCredentialsAuthenticated(Uri uri)
        {
            ConsoleColorHelper.Set(MessageType.High);
            Console.WriteLine();
            Console.Write(uri);
            ConsoleColorHelper.Reset();
            Console.WriteLine(Resource.Default.NotifyCredentialsAuthenticatedInfo);
        }
    }
}
