using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Classlibary;
using System.Text.Json;
namespace Classlibary
{
    [Serializable]
    public class Ball // 玩家ball 的 class
    {
        public System.Net.EndPoint s { get; set; }
        public Dictionary<string, Ball> Other_ID { get; set; }
        public List<litte_ball> little_balls { get; set; }
        public string ID { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int r { get; set; }
        public bool collision { get; set; }
        public bool Eat { get; set; }
        public bool Dead { get; set; }
    }
    public class litte_ball // 給吃的小球class
    {
        public int x, y;
    }

    public class Balls // 傳送的主要類別
    {
        public Balls(string id, Ball ball)
        {
            ball.ID = id;
        }
        public void ADD_Other_ID(string s, Ball b)
        {
            bool exist = b.Other_ID.ContainsKey(s);
            if (exist == false)
            {
                b.Other_ID.Add(s, b);
            }
            else
            {
                Console.WriteLine("Add already");
            }
        }
        public void Add_litle_balls(int x, int y)
        {
            litte_ball tmp = new litte_ball();
            tmp.x = x;
            tmp.y = y;
        }

    }
}
namespace Test_AsynCommunicationB
{
    class UDPCommunication
    {
        bool sendingFlag = true;
        bool receiveingFlag = true;
        IPEndPoint iep = null;
        IPEndPoint iep_Receive = null;
        Socket socketClient = null;
        Socket socketServer = null;
        byte[] byteSendingArray = null;
        byte[] byteReceiveArray = null;


        static void Main(string[] args)
        {
            UDPCommunication obj = new UDPCommunication();
            Console.WriteLine("---非同步通訊，B---");
            obj.OpenSendAndReceiveThread();
        }

        /// <summary>
        /// 分別開啟“接收”與“傳送”執行緒
        /// </summary>
        private void OpenSendAndReceiveThread()
        {
            Thread thSending = new Thread(SendingData);
            Thread thReveive = new Thread(ReceiveData);
            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            thSending.Start();
            thReveive.Start();
        }

        /// <summary>
        /// 傳送執行緒的方法
        /// </summary>
        private void SendingData()
        {
            if (sendingFlag)
            {
                byteSendingArray = new byte[100];

                //定義網路地址
                iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1001);
                sendingFlag = false;
            }
            //傳送資料
            Console.WriteLine("請輸入傳送的資料：");
            EndPoint ep = (EndPoint)iep;
            while (true)
            {

                Ball balls = new Ball();
                balls.ID = "test";
                string jsonstring = JsonSerializer.Serialize(balls);
                //string strMsg = Console.ReadLine();
                //位元組轉換
                byteSendingArray = Encoding.UTF8.GetBytes(jsonstring);
                socketClient.SendTo(byteSendingArray, ep);
            }
            socketClient.Shutdown(SocketShutdown.Both);
            socketClient.Close();
        }

        /// <summary>
        /// 接收執行緒的方法
        /// </summary>
        private void ReceiveData()
        {
            Thread.Sleep(500);
            if (receiveingFlag)
            {
                byteReceiveArray = new byte[100000];
                Console.Write(socketClient.LocalEndPoint.ToString());
                iep_Receive = new IPEndPoint(IPAddress.Parse("127.0.0.1"), ((IPEndPoint)socketClient.LocalEndPoint).Port);
                socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socketServer.Bind(iep_Receive);
                receiveingFlag = false;
            }
            //接受收據
            EndPoint ep = (EndPoint)iep_Receive;
            while (true)
            {
                int intReceiveLenght = socketServer.ReceiveFrom(byteReceiveArray, ref ep);
                string strReceiveStr = Encoding.UTF8.GetString(byteReceiveArray, 0, intReceiveLenght);
                Console.WriteLine(strReceiveStr);
            }

        }

    }//class_end
}

