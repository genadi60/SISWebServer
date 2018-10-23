using System.Globalization;
using System.Threading;

namespace SIS.WebServer
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    using HTTP.Common;
    using Routing;

    public class Server
    {
        private readonly int _port;

        private readonly TcpListener _listener;

        private readonly ServerRoutingTable _serverRoutingTable;

        private bool _isRunning;

        public Server(int port, ServerRoutingTable serverRoutingTable)
        {
            _port = port;
            _listener = new TcpListener(IPAddress.Parse(GlobalConstants.LocalhostIpAddress), _port);

            _serverRoutingTable = serverRoutingTable;
        }

        public void Run()
        {
            _listener.Start();
            _isRunning = true;

            Console.WriteLine(GlobalConstants.StartMessage, GlobalConstants.LocalhostIpAddress, _port);
            while (_isRunning)
            {
                var client = _listener.AcceptSocket();
                Task.Run(() => ListenLoop(client));
            }
        }

        private async Task ListenLoop(Socket client)
        {
            var connectionHandler = new ConnectionHandler(client, _serverRoutingTable);
            await connectionHandler.ProcessRequestAsync();
        }
    }
}
