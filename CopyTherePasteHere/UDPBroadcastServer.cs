using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CopyTherePasteHere
{
    class UDPBroadcastServer
    {
        private static ASCIIEncoding encoder = new ASCIIEncoding();
        private static readonly byte[] name = Encoding.ASCII.GetBytes(System.Environment.MachineName);


        public UDPBroadcastServer()
        {
            Thread threadUDPServer = new Thread(new ThreadStart(ServerThread));
            threadUDPServer.Start();
        }

        public void Stop()
        {
            udpClient.Close();
        }

        private UdpClient udpClient;
        private IPEndPoint remoteIpEndPoint;
        private byte[] receivedBytes;


        private void ServerThread()
        {
            while (true)
            {
                udpClient = new UdpClient(App.SERVER_PORT);
                remoteIpEndPoint = new IPEndPoint(IPAddress.Any, App.SERVER_PORT);
                try
                {
                    receivedBytes = udpClient.Receive(ref remoteIpEndPoint);
                }
                catch (SocketException)
                { // Do nothing, just prevent empty byte[] to crash program
                }
                if (receivedBytes == null || receivedBytes.Length == 0)
                    return;

                udpClient.Send(name, name.Length, remoteIpEndPoint); // reply back
                udpClient.Close();
            }
        }
    }
}
