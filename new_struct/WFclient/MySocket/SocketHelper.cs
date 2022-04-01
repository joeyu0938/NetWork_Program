using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

using System.Text.Json;
namespace MySocket
{
    public class SocketHelper
    {
        private byte[] byteSendingArray;
        private Socket sendsocket = null;
        private Socket receivesocket = null;
        private IPEndPoint iep_client = null;
        private IPEndPoint iep_sever = null;
        private EndPoint ep_sever = null;
        public void Init()
        {
            byteSendingArray = new byte[10000];
            sendsocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Send();
            receivesocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            receivesocket.Bind(iep_client);
        }
        public void Bind()
        {

        }
        private void Send()
        {
            iep_sever = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1001);
            string jsonstring = JsonSerializer.Serialize(b);
            byteSendingArray = Encoding.UTF8.GetBytes(jsonstring);
            sendsocket.SendTo(byteSendingArray, iep_sever);
            iep_client = new IPEndPoint(IPAddress.Parse("127.0.0.1"), ((IPEndPoint)sendsocket.LocalEndPoint).Port);
        }
    }
}
