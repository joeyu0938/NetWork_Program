using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using Classlibary;
using SocketControl;
using System.Text.Json;
using System.Collections.Generic;

namespace WFclient
{
    public partial class Form1 : Form
    {
        bool started = false;
        Ball b = new Ball();
        SocketHelper SocketH = new SocketHelper();
        private Graphics g;
        private SolidBrush myBrush = new SolidBrush(System.Drawing.Color.Red);
        private Thread thread_sender;
        private Thread thread_receiver;
        public Form1()
        {
            InitializeComponent();
            //this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;
            g = this.CreateGraphics();
            button1.Location = new Point(this.Size.Width / 2 - button1.Width / 2, this.Size.Height / 2 - button1.Height / 2);
            b.x = 50;
            b.y = 50;
            b.r = 50;
            b.move = 'n';
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            (thread_sender = new(() =>
            {
                while (true)
                {
                    SocketH.Send();
                    Invoke(() => {
                        label2.Text = b.move.ToString();
                    });
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
                    if (rev != "")
                        b = JsonSerializer.Deserialize<Ball>(rev);
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
            if(started) Render();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //Init my socket
            SocketH.Init(b);
            button1.Visible = false;
            started = true;
            Render();
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
            b.move = 'n';
        }
        private void Render()
        {
            g.Clear(Color.Tan);
            g.FillEllipse(myBrush, b.x, b.y, b.r, b.r);
            if (b.Set_little_balls != null && b.Set_little_balls.Count > 0)
            {
                foreach (litte_ball smb_i in b.Set_little_balls)
                {
                    g.FillEllipse(myBrush, smb_i.x, smb_i.y, 10, 10);
                }
            }
        }
    }
}