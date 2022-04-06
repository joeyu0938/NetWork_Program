using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Classlibary;
using System.Text.Json;
namespace SocketControl
{
    public class SocketHelper
    {
        public Ball BallRef;

        bool Initialized = false;
        Socket socketClient;
        Socket socketServer;
        IPEndPoint iep;
        IPEndPoint iep_Receive;
        //private EndPoint ep_sever;
        private byte[] byteSendingArray = new byte[100000];
        private byte[] byteReceiveArray = new byte[100000];
        public SocketHelper()
        {
            iep = new IPEndPoint(IPAddress.Parse("192.168.0.200"), 1001); 
            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); 
        }
        public void Init()
        {
            BallRef = new Ball();
            BallRef.self = new little_ball();
            BallRef.Other_ID = new Dictionary<string, little_ball>();
            BallRef.little_balls = new List<little_ball>();
            EndPoint ep = (EndPoint)iep;
            string jsonstring = JsonSerializer.Serialize(BallRef);
            byteSendingArray = Encoding.UTF8.GetBytes(jsonstring);
            socketClient.SendTo(byteSendingArray, ep);
            iep_Receive = (IPEndPoint)socketClient.LocalEndPoint;
            Initialized = true;
        }
        public void Send(ref Ball ball)
        {
            if (!Initialized)
                return;
            BallRef.self = ball.self;
            BallRef.little_balls = ball.little_balls;
            BallRef.Other_ID = ball.Other_ID;
            BallRef.ID = ball.ID;
            EndPoint ep = (EndPoint)iep;
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonstring = JsonSerializer.Serialize(BallRef, options);
            byteSendingArray = Encoding.UTF8.GetBytes(jsonstring);
            socketClient.SendTo(byteSendingArray, ep);
        }
        public string Receive()
        {
            if (!Initialized)
                return "";
            //接受收據
            EndPoint ep = (EndPoint)iep_Receive;
            socketClient.ReceiveTimeout = 1000;
            try
            {
                int intReceiveLenght = socketClient.ReceiveFrom(byteReceiveArray, ref ep);
                return Encoding.UTF8.GetString(byteReceiveArray, 0, intReceiveLenght);
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}