using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ArmaCommandConsole
{
    public class Program
    {
        private static StreamWriter _streamWriter;

        public static void Main(string[] args)
        {
            StartTcpListener();
            ContinuouslyReadFromConsoleAndSendCommand();
        }

        private static void StartTcpListener()
        {
            var backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };

            backgroundWorker.DoWork += delegate
            {
                var server = new TcpListener(IPAddress.Any, 7845);
                server.Start();
                Console.WriteLine("TCP listener started on port 7845.");
                try
                {
                    var client = server.AcceptTcpClient();
                    Console.WriteLine("Connected to ARMA.");
                    _streamWriter = new StreamWriter(client.GetStream());
                    Console.WriteLine("Ready to receive commands...");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            };

            backgroundWorker.RunWorkerAsync();
        }

        private static void Send(string command)
        {
            if (_streamWriter != null)
            {
                _streamWriter.WriteLine(command);
                _streamWriter.Flush();
            }
            else
            {
                Console.WriteLine("Not connected to ARMA.");
            }
        }

        private static void ContinuouslyReadFromConsoleAndSendCommand()
        {
            while (true)
            {
                var command = ReadCommandFromConsole();
                if (command.ToLowerInvariant().Equals("exit")) {
                    Environment.Exit(0);
                }
                Send(command);
            }
        }

        private static string ReadCommandFromConsole()
        {
            return Console.ReadLine();
        }
    }
}