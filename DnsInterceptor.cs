using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SecuredWeb
{
    public static class DnsInterceptor
    {
        public static void Start()
        {
            Console.WriteLine("Starting DNS Interception...");
            dnssniffer.Start();     
        }
    }
}

