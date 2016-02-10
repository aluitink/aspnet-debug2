using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspnet_debug.Shared.Server;

namespace aspnet_debug.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var server = new MonoDebugServer())
            {
                server.StartAnnouncing();
                server.Start();

                server.WaitForExit();
            }
        }
    }
}
