using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Core
{
    /// <summary>
    /// The type of logon operation to perform.
    /// </summary>
    public enum LogonType : uint
    {
        /// <summary>
        /// This logon type is intended for users who will be interactively using the computer, such as a user being logged on by a terminal server, remote shell, or similar process. This logon type has the additional expense of caching logon information for disconnected operations; therefore, it is inappropriate for some client/server applications, such as a mail server.
        /// </summary>
        Interactive = 2,

        /// <summary>
        /// This logon type is intended for high performance servers to authenticate plaintext passwords. The LogonUser function does not cache credentials for this logon type.
        /// </summary>
        Network = 3,

        /// <summary>
        /// This logon type is intended for batch servers, where processes may be executing on behalf of a user without their direct intervention. This type is also for higher performance servers that process many plaintext authentication attempts at a time, such as mail or Web servers. The LogonUser function does not cache credentials for this logon type.
        /// </summary>
        Batch = 4,

        /// <summary>
        /// Indicates a service-type logon. The account provided must have the service privilege enabled.
        /// </summary>
        Service = 5,

        /// <summary>
        /// This logon type is for GINA DLLs that log on users who will be interactively using the computer. This logon type can generate a unique audit record that shows when the workstation was unlocked.
        /// </summary>
        Unlock = 7,

        /// <summary>
        /// This logon type preserves the name and password in the authentication package, which allows the server to make connections to other network servers while impersonating the client. A server can accept plaintext credentials from a client, call LogonUser, verify that the user can access the system across the network, and still communicate with other servers.
        /// </summary>
        NetworkClearText = 8,

        /// <summary>
        /// This logon type allows the caller to clone its current token and specify new credentials for outbound connections. The new logon session has the same local identifier but uses different credentials for other network connections.<br/>
        /// This logon type is supported only by the LOGON32_PROVIDER_WINNT50 logon provider.
        /// </summary>
        NewCredentials = 9
    }

    /// <summary>
    /// LogonProvider
    /// </summary>
    public enum LogonProvider : uint
    {
        /// <summary>
        /// Use the standard logon provider for the system.<br/>
        /// The default security provider is negotiate, unless you pass NULL for the domain name and the user name is not in UPN format. In this case, the default provider is NTLM.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Use the Windows NT 3.5 logon provider.
        /// </summary>
        WinNT35 = 1,

        /// <summary>
        /// Use the NTLM logon provider.
        /// </summary>
        WinNT40 = 2,

        /// <summary>
        /// Use the negotiate logon provider.
        /// </summary>
        WinNT50 = 3,
    }

    /// <summary>
    /// Windows API
    /// </summary>
    internal static class NativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetComputerNameEx(EnvironmentHelper.COMPUTER_NAME_FORMAT nameType, StringBuilder nameBuffer, ref uint bufferSize);

        [DllImport("secur32.dll", CharSet = CharSet.Unicode)]
        public static extern byte GetComputerObjectName(EnvironmentHelper.EXTENDED_NAME_FORMAT nameFormat, StringBuilder nameBuffer, ref ulong bufferSize);

        [DllImport("rpcrt4.dll", SetLastError = true)]
        public static extern int UuidCreateSequential(out Guid guid);

        [DllImport("Advapi32.dll", CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword,
            LogonType dwLogonType, LogonProvider dwLogonProvider, out IntPtr phToken);

        [DllImport("Kernel32.dll")]
        public static extern int GetLastError();
    }
}
