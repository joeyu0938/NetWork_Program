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
        Ball b;
        SocketHelper SocketH = new SocketHelper();
        private Graphics g;
        private SolidBrush myBrush = new SolidBrush(System.Drawing.Color.Red);
        private Thread thread_sender;
        private Thread thread_receiver;
        private Thread thread_render;
        private int fps;
        public Form1()
        {
            InitializeComponent();
            //this.WindowState = FormWindowState.Maximized;
            b = new Ball();
            this.TopMost = true;
            g = this.CreateGraphics();
            button1.Location = new Point(this.Size.Width / 2 - button1.Width / 2, this.Size.Height / 2 - button1.Height / 2);
            b.self = new little_ball();
            b.Other_ID = new Dictionary<string, little_ball>();
            b.little_balls = new List<little_ball>();
            b.self.x = 50;
            b.self.y = 50;
            b.self.r = 50;
            b.self.move = 'n';
            fps = 60;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            (thread_sender = new(() =>
            {
                while (true)
                {
                    SocketH.Send(ref b);
                    Invoke(() =>
                    {
                        label2.Text = b.self.move.ToString();
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
                    Thread.Sleep(1);
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
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //Init my socket
            SocketH.Init();
            button1.Visible = false;
            started = true;
            if (started)
            {
                (thread_render = new(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(1000 / fps);
                        Invoke(() =>
                        {
                            Render();
                        });
                    }
                })
                { IsBackground = true }).Start();
            }
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                b.self.move = 'w';
            }
            else if (e.KeyCode == Keys.A)
            {
                b.self.move = 'a';
            }
            else if (e.KeyCode == Keys.S)
            {
                b.self.move = 's';
            }
            else if (e.KeyCode == Keys.D)
            {
                b.self.move = 'd';
            }
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
        }
        private void Render()
        {
            g.Clear(Color.Tan);
            g.FillEllipse(myBrush, b.self.x, b.self.y, b.self.r, b.self.r);
            if (b.little_balls != null && b.little_balls.Count > 0)
            {
                foreach (little_ball smb_i in b.little_balls)
                {
                    g.FillEllipse(myBrush, smb_i.x, smb_i.y, 10, 10);
                }
            }
        }
    }
}