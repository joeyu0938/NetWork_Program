using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace UDP_client
{
    class Program
    {
        static Socket client;
        static void Main(string[] args)
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            client.Bind(new IPEndPoint(IPAddress.Parse("10.141.51.165"), 6000));
            Thread t = new Thread(sendMsg);
            t.Start();
            Thread t2 = new Thread(ReciveMsg);
            t2.Start();
            Console.WriteLine("客戶端已經開啟");
        }
        /// <summary>
        /// 向特定ip的主機的埠傳送資料報
        /// </summary>
        static void sendMsg()
        {
            EndPoint point = new IPEndPoint(IPAddress.Parse("10.141.51.165"), 6001);
            while (true)
            {
                string msg = Console.ReadLine();
                client.SendTo(Encoding.UTF8.GetBytes(msg), point);
            }


        }

        /// <summary>
        /// 接收發送給本機ip對應埠號的資料報
        /// </summary>
        static void ReciveMsg()
        {
            while (true)
            {
                EndPoint point = new IPEndPoint(IPAddress.Any, 0);//用來儲存傳送方的ip和埠號
                byte[] buffer = new byte[1024];
                int length = client.ReceiveFrom(buffer, ref point);//接收資料報
                string message = Encoding.UTF8.GetString(buffer, 0, length);
                Console.WriteLine(point.ToString() + message);
            }
        }

    }
}