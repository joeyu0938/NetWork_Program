using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using Classlibary;
using SocketControl;

namespace WFclient
{
    public partial class Form1 : Form
    {
        Ball b = new Ball();
        SocketHelper SocketH = new SocketHelper();
        private Graphics g;
        private SolidBrush myBrush = new SolidBrush(System.Drawing.Color.Red);
        private Thread thread_sender;
        private Thread thread_receiver;
        public Form1()
        {
            InitializeComponent();
            g = this.CreateGraphics();
            button1.Location = new Point(this.Size.Width / 2 - button1.Width / 2, this.Size.Height / 2 - button1.Height / 2);
            b.x = 50;
            b.y = 50;
            b.r = 50;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            (thread_sender = new(() =>
            {
                while (true)
                {
                    SocketH.Send();
                    //Invoke(() => {

                    //});
                }
            })
            { IsBackground = true }).Start();

            (thread_receiver = new(() =>
            {
                int count = 0;
                Thread.Sleep(300);
                DateTime LastRev = DateTime.Now;
                while (true)
                {
                    Thread.Sleep(10);
                    string rev = SocketH.Receive();

                    Invoke(() =>
                    {
                        if (rev != "")
                        {
                            count++;
                            label1.Text = string.Format("cnt:{0} ping:{1} ms", count.ToString(),
                                                    (DateTime.Now - LastRev).TotalMilliseconds);
                            LastRev = DateTime.Now;
                        }
                    });
                }
            })
            { IsBackground = true }).Start();
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            button1.Location = new Point(this.Size.Width / 2 - button1.Width / 2, this.Size.Height / 2 - button1.Height / 2);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //Init my socket
            SocketH.Init(b);
            button1.Visible = false;
            g.Clear(Color.Tan);
            g.FillEllipse(myBrush, b.x, b.y, b.r, b.r);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                b.move = 'u';
            }
            else if(e.KeyCode == Keys.A)
            {
                b.move = 'l';
            }
            else if (e.KeyCode == Keys.S)
            {
                b.move = 'd';
            }
            else if(e.KeyCode == Keys.D)
            {
                b.move = 'r';
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            b.move = '\0';
        }
    }
}