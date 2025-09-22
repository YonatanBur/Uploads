using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecuredWeb
{
    public static class BlockListManager
    {
        private static List<string> blockedDomains = new List<string>
        {
            "facebook.com",
            "youtube.com",
            "instagram.com"
        };

        public static bool IsBlocked(string domain)
        {
            foreach (var blocked in blockedDomains)
            {
                if (domain.Contains(blocked))
                    return true;
            }
            return false;
        }

        public static void AddDomain(string domain)
        {
            if (!blockedDomains.Contains(domain))
                blockedDomains.Add(domain);
        }
    }
}
