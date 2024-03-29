﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using aspnet_debug.Debugger.Contracts;
using aspnet_debug.Shared;

namespace aspnet_debug.Extension.MonoClient
{
    public class DebugSession : IDebugSession
    {
        private readonly TcpCommunication communication;
        private readonly ApplicationType type;

        public DebugSession(DebugClient debugClient, ApplicationType type, Socket socket)
        {
            Client = debugClient;
            this.type = type;
            communication = new TcpCommunication(socket);
        }

        public DebugClient Client { get; private set; }

        public void Disconnect()
        {
            communication.Disconnect();
        }

        public void TransferFiles()
        {
            var info = new DirectoryInfo(Client.OutputDirectory);
            if (!info.Exists)
                throw new DirectoryNotFoundException("Directory not found");

            string targetZip = Path.Combine(info.FullName, "DebugContent.zip");
            if (File.Exists(targetZip))
                File.Delete(targetZip);

            ZipFile.CreateFromDirectory(info.FullName, targetZip);

            communication.Send(Command.DebugContent, new StartDebuggingMessage
            {
                AppType = type,
                DebugContent = File.ReadAllBytes(targetZip),
                FileName = Client.TargetExe
            });

            File.Delete(targetZip);
            Console.WriteLine("Finished transmitting");
        }

        public Task TransferFilesAsync()
        {
            return Task.Factory.StartNew(TransferFiles);
        }

        public async Task WaitForAnswerAsync()
        {
            Task delay = Task.Delay(10000);
            Task msg = await Task.WhenAny(communication.ReceiveAsync(), delay);

            if (msg is Task<MessageBase>)
                return;

            if (msg == delay)
                throw new Exception("Did not receive an answer in time...");
            throw new Exception("Cant start debugging");
        }
    }
}