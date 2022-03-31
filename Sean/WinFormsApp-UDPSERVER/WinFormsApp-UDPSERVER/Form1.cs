using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using System.ComponentModel;
using System.Windows.Forms;
namespace WinFormsApp_UDPSERVER
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
        }
        private static Socket server;
        
        private void server_on_Click(object sender, EventArgs e)
        {
            try
            {
                server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                server.Bind(new IPEndPoint(IPAddress.Parse("10.141.51.165"), 6001));//繫結埠號和IP
                //Thread t = new Thread(ReciveMsg);//開啟接收訊息執行緒
                //t.Start();
                label1.Text = "服務端已經開啟";
            }
            catch (Exception ex)
            {
                if (ex != null)
                    label1.Text = "open server wrong:" + ex.Message.ToString();
            }
        }
        //void ReciveMsg()
        //{
        //    while (true)
        //    {
        //        EndPoint point = new IPEndPoint(IPAddress.Any, 0);//用來儲存傳送方的ip和埠號
        //        byte[] buffer = new byte[1024];
        //        int length = server.ReceiveFrom(buffer, ref point);//接收資料報
        //        string message = Encoding.UTF8.GetString(buffer, 0, length);
        //        label2.Text = message;
        //    }
        //}

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            while (true)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                EndPoint point = new IPEndPoint(IPAddress.Any, 0);//用來儲存傳送方的ip和埠號
                byte[] buffer = new byte[1024];
                int length = server.ReceiveFrom(buffer, ref point);//接收資料報
                string message = Encoding.UTF8.GetString(buffer, 0, length);
                Random myObject = new Random();
                int ranNum = myObject.Next(0, 100);
                worker.ReportProgress(Convert.ToInt32(message));
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            label2.Text = e.ProgressPercentage.ToString();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                label2.Text = "Canceled!";
            }
            else if (e.Error != null)
            {
                label2.Text = "Error: " + e.Error.Message;
            }
            else
            {
                label2.Text = "Done!";
            }
        }

        private void startAsyncButton_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy != true)
            {
                // Start the asynchronous operation.
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void cancelAsyneButtom_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.WorkerSupportsCancellation == true)
            {
                // Cancel the asynchronous operation.
                backgroundWorker1.CancelAsync();
            }
        }
    }
}