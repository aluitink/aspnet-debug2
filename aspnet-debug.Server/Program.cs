﻿using System;
using System.IO;
using aspnet_debug.Shared.Logging;
using aspnet_debug.Shared.Server;

namespace aspnet_debug.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Configure(new FileInfo("log4net.xml"), new DirectoryInfo("logs"));
            
            Log.Logger.Info("Server starting...");
            using (var server = new MonoDebugServer())
            {
                server.StartAnnouncing();
                server.Start();

                server.WaitForExit();
            }
        }
    }
}
