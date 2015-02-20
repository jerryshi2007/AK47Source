using System.DirectoryServices;
using System.Globalization;
using System.Security;

namespace CheckCompileStatus
{
    public class ADHelper
    {
        private string userName;
        private string password;
        private string adServer;
        private string defaultNamingContext;

        [SecurityCritical]
        public ADHelper(string userName, string password, string adServer)
        {
            this.userName = userName;
            this.password = password;
            this.adServer = adServer;

            using (DirectoryEntry rootDSE = new DirectoryEntry(adServer.Format<string>("LDAP://{0}/rootDSE"), userName, password, AuthenticationTypes.Secure))
            {
                defaultNamingContext = rootDSE.Properties["defaultNamingContext"].Value.ToString();
            };
        }

        [SecurityCritical]
        public string GetUserDisplayName(string alias)
        {
            using (DirectoryEntry root = GetRootEntry())
            {
                using (DirectorySearcher searcher = new DirectorySearcher(root))
                {
                    searcher.Filter = alias.Format<string>("(|(&(objectCategory=person)(objectClass=user)(samAccountName={0})))");

                    using (SearchResultCollection searchResult = searcher.FindAll())
                    {

                        using (DirectoryEntry user = searchResult[0].GetDirectoryEntry())
                        {
                            if (user.Properties.Contains("displayName"))
                            {
                                return user.Properties["displayName"].Value.ToString();
                            }
                            else
                            {
                                return alias;
                            }
                        };
                    };
                }
            };
        }

        private DirectoryEntry GetRootEntry()
        {
            return new DirectoryEntry(string.Format(CultureInfo.InvariantCulture, "LDAP://{0}/{1}", adServer, defaultNamingContext), userName, password, AuthenticationTypes.Secure);
        }
    }
}