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
        List<litte_ball> random_little_ball_set;
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
            dicClient= new Dictionary<string, Ball>();
            occupy = new Dictionary<string, bool>();
            OpenSendAndReceiveThread();
        }
        //從 Form1 把 UI 控制權傳到函數裡面


        /// 分別開啟“接收”與“傳送”執行緒
        private void OpenSendAndReceiveThread()
        {
            _pause = new ManualResetEvent(true); //用來插入event 操作
            Balls control = new Balls();
            random_little_ball_set = new List<litte_ball>();
            
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
        private void SendingData(string ID,ref EndPoint ep,Ball tmp)//Sendingdata 會在新增client 的時候自動再開一個thread
        {
            int cnt = 0;
            byteSendingArray = new byte[10000];
                //定義網路地址
            Socket socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);//定義client 接收樣板
            //傳送資料
            AddMessage("傳送");
            Balls control = new Balls();// 要處理的動作
            Ball set_ball = new Ball();
            set_ball.ID = ID;
            set_ball.s = ep;
            set_ball.Set_little_balls = random_little_ball_set;//初次進入撒現在剩下的小點點
            set_ball.x = tmp.x;
            set_ball.y = tmp.y;
            set_ball.r = tmp.r;
            set_ball.Set_Other_ID = dicClient;
            AddMessage(string.Format("Sending to {0}", set_ball.s.ToString()));
            while (true)
            {
                if(_pause.WaitOne(Timeout.Infinite) ==false )break;
                {
                    try
                    {
                        if (set_ball == null) break;//如果被刪除，跳出
                        //設定共有變數
                        lock (dicClient)
                        {

                            set_ball.Set_Other_ID = dicClient;
                            set_ball.x = dicClient[ID].x;
                            set_ball.y = dicClient[ID].y;
                            set_ball.r = dicClient[ID].r;
                            set_ball.move = dicClient[ID].move;
                            //設定共有變數
                            control.Ball_move(ref set_ball, ID, ref random_little_ball_set);//如果client 端要處理就不用了，如果沒有的話把 上下左右放進來Ball (u,d,l,r)
                            control.Count_collision(ref dicClient, ref random_little_ball_set);//如果client 端要處理就不用了，我函式再改成統合狀態就好
                            set_ball.Set_little_balls = random_little_ball_set;
                            set_ball.Set_Other_ID = dicClient;
                            dicClient.Remove(ID);
                            dicClient[ID] = new Ball();
                            dicClient[ID] = set_ball;
                        }

                        /*lock (_thisLock) 萬一共用變數有問題
                        {
                            //TODO
                        }
                        */
                        //傳送的json string
                        //很重要!!!
                        string jsonstring = JsonSerializer.Serialize(set_ball);
                        //位元組轉換
                        byteSendingArray = Encoding.UTF8.GetBytes(jsonstring);
                        //AddMessage(string.Format("Sending to {0}", dicClient[ID].s.ToString()));
                        socketClient.SendTo(byteSendingArray, set_ball.s);
                        //從進來的endpoint(紀錄的Ip & port)出去
                        //傳送的json string
                        //很重要!!!
                    }
                    catch
                    {
                        
                        cnt++;
                        if(cnt == 1000)
                        {
                            AddMessage(string.Format("Cannot entry :{0}", occupy[ID].ToString())); //server報錯
                            dicClient.Remove(ID);
                            set_ball =null;
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
                    receive.s = ep;
                    //接收傳來的json
                    //很重要!!!
                    
                    if (occupy.ContainsKey(ep.ToString())!= true)//如果用戶不存在就新增
                    {
                        if (receive.Dead == true) continue; //如果用戶死亡
                        AddMessage(string.Format("Add {0}", receive.s));
                        Thread thSending = new Thread(() => SendingData(receive.s.ToString(), ref ep, receive));
                        occupy[ep.ToString()] = true;
                        thSending.Start();
                        continue;
                    }
                    /*lock (_thisLock) 萬一共用變數有問題
                    {
                        //TODO
                    }
                    */
                    //也可以改成傳進來的只有需要的參數 就不用receive 並更新一整個 object
                    
                    if (receive.Dead == true) dicClient.Remove(receive.s.ToString());
                    else
                    {
                        
                        if (dicClient.ContainsKey(ep.ToString()) != true) dicClient[ep.ToString()] = new Ball();
                        lock (dicClient)
                        {
                            dicClient[ep.ToString()] = new Ball();
                            dicClient[ep.ToString()].x = receive.x;
                            dicClient[ep.ToString()].y = receive.y;
                            dicClient[ep.ToString()].r = receive.r;
                            dicClient[ep.ToString()].move = receive.move;
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