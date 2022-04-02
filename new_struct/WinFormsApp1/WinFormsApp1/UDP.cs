using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using Classlibary;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace WinFormsApp1
{

    partial class UDPCommunication : Form1
    {   //宣告類別變數
        Dictionary<string, bool> occupy;
        Dictionary<string, Ball> dicClient;//連線的客戶端集合
        Dictionary<string, little_ball> other_Client;
        List<little_ball> random_little_ball_set;
        bool receiveingFlag = true;
        IPEndPoint iep_Receive = null;
        Socket socketServer = null;
        byte[] byteSendingArray = null;
        byte[] byteReceiveArray = null;
        private Form1 form;
        Thread thReveive;
        ManualResetEvent _pause;
        int little_balls_number = 100;//一百個小點點

        //從 Form1 把 UI 控制權傳到函數裡面
        public void pause(Form1 c)//暫停 (備註:但socket會持續接收:)
        {
            form = c;
            c.listBox1.Items.Add("pause");
            _pause.Reset();
        }
        public void Resume(Form1 c) //恢復
        {
            form = c;
            c.listBox1.Items.Add("Resume");
            _pause.Set();
            OpenSendAndReceiveThread();
        }
        public void Start(Form1 u)
        {
            form = u;
            form.listBox1.Items.Add("---非同步通訊，A---");
            dicClient= new Dictionary<string,Ball>();
            occupy = new Dictionary<string, bool>();
            other_Client = new Dictionary<string, little_ball>();
            OpenSendAndReceiveThread();
        }
        //從 Form1 把 UI 控制權傳到函數裡面


        /// 分別開啟“接收”與“傳送”執行緒
        private void OpenSendAndReceiveThread()
        {
            _pause = new ManualResetEvent(true); //用來插入event 操作
            Balls control = new Balls();
            random_little_ball_set = new List<little_ball>();
            control.random_little_balls(little_balls_number,ref random_little_ball_set);
            thReveive = new Thread(ReceiveData);
            thReveive.Start();
        }
        private delegate void UPDATE_UI(string s); //委派函數，可以在不同執行緒上操作主緒的UI
        //private delegate void UPDATE_BALL(Ball s); 如果改共用參數衝突發生只能走委派

        //傳入:要在server印出的message*
        private void AddMessage(string sMessage)
        {
            if (this.form.listBox1.InvokeRequired) // 若非同執行緒
            {
                UPDATE_UI del = new UPDATE_UI(AddMessage); //利用委派執行
                this.form.listBox1.Invoke(del, sMessage);//從操作主緒的UI
                this.form.Invoke( () =>
                {
                    form.listBox1.TopIndex = form.listBox1.Items.Count - 1;
                });
            }
            else // 同執行緒
            {
                this.form.listBox1.Items.Add(sMessage);
            }
        }
    

        //傳入: Ball的參數 開始傳送data
        private void SendingData(string ID,EndPoint ep,Ball tmp)//Sendingdata 會在新增client 的時候自動再開一個thread
        {
            int cnt = 0;
            byteSendingArray = new byte[10000];
                //定義網路地址
            Socket socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);//定義client 接收樣板
            //傳送資料
            AddMessage("傳送");
            Balls control = new Balls();// 要處理的動作
            dicClient[ID].self = tmp.self;
            other_Client.Add(ep.ToString(), dicClient[ID].self);
            dicClient[ID].Other_ID = other_Client;
            dicClient[ID].ID = ID;
            AddMessage(string.Format("Sending to {0}", ID));
            while (true)
            {
                if(_pause.WaitOne(Timeout.Infinite) ==false )break;
                {
                    try
                    {
                        if (dicClient.ContainsKey(ID) != true) break;//如果被刪除，跳出
                        //設定共有變數
                        lock (dicClient)
                        {
                            //AddMessage(dicClient[ID].move.ToString());
                            //設定共有變數
                            dicClient[ID].Other_ID = other_Client;
                            control.Ball_move(ref dicClient, ID, ref random_little_ball_set);//如果client 端要處理就不用了，如果沒有的話把 上下左右放進來Ball (u,d,l,r)
                            control.Count_collision(ref dicClient, ref random_little_ball_set);//如果client 端要處理就不用了，我函式再改成統合狀態就好
                            dicClient[ID].little_balls = random_little_ball_set;
                            other_Client[ID].x = dicClient[ID].self.x;
                            other_Client[ID].y = dicClient[ID].self.y;
                            other_Client[ID].r = dicClient[ID].self.r;
                            other_Client[ID].move = dicClient[ID].self.move;
                            other_Client[ID].collision = dicClient[ID].self.collision;
                            other_Client[ID].Dead = dicClient[ID].self.Dead;
                            other_Client[ID].Eat = dicClient[ID].self.Eat;
                            dicClient[ID].Other_ID = other_Client;
                            /*lock (_thisLock) 萬一共用變數有問題
                        {
                            //TODO
                        }
                        */
                            //傳送的json string
                            //很重要!!!
                            //AddMessage(set_ball.move.ToString());
                            string jsonstring = JsonSerializer.Serialize(dicClient[ID]);
                            //位元組轉換
                            byteSendingArray = Encoding.UTF8.GetBytes(jsonstring);
                            //AddMessage(string.Format("Sending to {0}", dicClient[ID].s.ToString()));
                            Thread.Sleep(500);
                            socketClient.SendTo(byteSendingArray, ep);
                            //從進來的endpoint(紀錄的Ip & port)出去
                            //傳送的json string
                            //很重要!!!
                        }
                    }
                    catch
                    {

                        cnt++;
                        if (cnt == 1000)
                        {
                            AddMessage(string.Format("Cannot entry :{0}", occupy[ID].ToString())); //server報錯
                            dicClient.Remove(ID);
                            other_Client.Remove(ID);
                            break;
                        }
                    }
                }
            }
            return;
        }

        //接收執行緒的方法
        private void ReceiveData() //ReceiveData 永遠只有一個 不斷接收並更新狀態(可能會有thread 的問題要接了之後才知道)
        {
            
            if (receiveingFlag)
            {
                byteReceiveArray = new byte[10000];
                iep_Receive = new IPEndPoint(IPAddress.Any, 1001);
                socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socketServer.Bind(iep_Receive); // 將endpoint 和 local 綁在一起
                receiveingFlag = false;
            }
            EndPoint ep = (EndPoint)iep_Receive;//接受收據(樣板)
            while (true)
            {
                try
                {
                    if (_pause.WaitOne(Timeout.Infinite) == false) break;

                    //接收傳來的json string
                    //很重要!!!
                    int intReceiveLenght = socketServer.ReceiveFrom(byteReceiveArray, ref ep);
                    string strReceiveStr = Encoding.UTF8.GetString(byteReceiveArray, 0, intReceiveLenght);
                    Ball receive = JsonSerializer.Deserialize<Ball>(strReceiveStr);  //反轉序列化 必須要有一樣且可序列化的class 
                    //接收傳來的json
                    //很重要!!!

                    if (dicClient.ContainsKey(ep.ToString()) != true)//如果用戶不存在就新增
                    {
                        AddMessage(string.Format("Add {0}", ep.ToString()));
                        dicClient[ep.ToString()] = new Ball();
                        dicClient[ep.ToString()].Other_ID = new Dictionary<string, little_ball>();
                        dicClient[ep.ToString()].little_balls = new List<little_ball>();
                        dicClient[ep.ToString()].self = new little_ball();
                        Thread thSending = new Thread(() => SendingData(ep.ToString(), ep, receive));
                        thSending.Start();
                        continue;
                    }
                    /*lock (_thisLock) 萬一共用變數有問題
                    {
                        //TODO
                    }
                    */
                    //也可以改成傳進來的只有需要的參數 就不用receive 並更新一整個 object
                   
                    if (receive.self.Dead == true)
                    {
                        dicClient.Remove(ep.ToString());
                        other_Client.Remove(ep.ToString());
                    }
                    else
                    {
                        lock (dicClient)
                        {
                            dicClient[ep.ToString()].self.x = receive.self.x;
                            dicClient[ep.ToString()].self.y = receive.self.y;
                            dicClient[ep.ToString()].self.r = receive.self.r;
                            dicClient[ep.ToString()].self.move = receive.self.move;
                            AddMessage(String.Format("receive {0}", strReceiveStr));
                        }
                    } //更新客戶們狀態
                }
                catch
                {
                    AddMessage("lost");
                }
            }

        }
    }//class_end
}