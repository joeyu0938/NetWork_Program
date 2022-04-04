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
using SkiaSharp;
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
        private Thread thread_checker;
        private int fps;
        SKImageInfo sKImageInfo;
        public Form1()
        {
            InitializeComponent();
            //this.WindowState = FormWindowState.Maximized;
            b = new Ball();
            this.TopMost = true;
            button1.Location = new Point(this.Size.Width / 2 - button1.Width / 2, this.Size.Height / 2 - button1.Height / 2);
            button2.Location = new Point(this.Size.Width / 2 - button1.Width / 2, this.Size.Height / 2 - button1.Height / 2);
            b.self = new little_ball();
            b.Other_ID = new Dictionary<string, little_ball>();
            b.little_balls = new List<little_ball>();
            b.self.move = 'n';
            b.self.x = 0;
            b.self.y = 0;
            b.self.r = 0;
            b.self.Dead = false;
            fps = 60;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Size = new Size(this.Size.Width, this.Size.Height);
            pictureBox1.SendToBack();
            sKImageInfo = new SKImageInfo(this.Size.Width, this.Size.Height);
            button2.Visible = false;
            button2.Enabled = false;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            (thread_sender = new(() =>
            {
                while (true)
                {
                    Thread.Sleep(30);
                    SocketH.Send(ref b);
                    if (b.self.Dead == true) break;
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
                DateTime LastRev = DateTime.Now;
                while (true)
                {
                    if (b.self.Dead == true) break;
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
            button2.Location = new Point(this.Size.Width / 2 - button1.Width / 2, this.Size.Height / 2 - button1.Height / 2);
            pictureBox1.Size = new Size(this.Size.Width, this.Size.Height);
            sKImageInfo = new SKImageInfo(this.Size.Width, this.Size.Height);
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
                        Render();
                        if (b.self.Dead == true)
                        {
                            Invoke(() =>
                            {
                                button2.Visible = true;
                                button2.Enabled = true;
                            });
                        }
                    }
                })
                { IsBackground = true }).Start();
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            started = false;
            System.Environment.Exit(0);
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
            b.self.move = 'n';
        }
        private void Render()
        {
            if (b.self.Dead == true)
            {
                Invoke(() =>
                {
                    label1.Visible = false;
                    label2.Visible = false;
                });
                using (SKSurface surface = SKSurface.Create(sKImageInfo))
                {
                    SKCanvas canvas = surface.Canvas;
                    canvas.Clear(SKColors.Black);
                    using (SKPaint textPaint = new SKPaint())
                    {
                        textPaint.Color = SKColors.White;
                        textPaint.IsAntialias = true;
                        textPaint.TextSize = 48;
                        canvas.DrawText("You loose!", 50, 50, textPaint);
                    }
                    using (SKImage image = surface.Snapshot())
                    using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
                    using (MemoryStream mstream = new MemoryStream(data.ToArray()))
                    {
                        Bitmap bm = new Bitmap(mstream, false);
                        pictureBox1.Image = bm;
                    }
                }
            }
            else
            {
                label1.BackColor = Color.Tan;
                label2.BackColor = Color.Tan;
                using (SKSurface surface = SKSurface.Create(sKImageInfo))
                {
                    SKCanvas canvas = surface.Canvas;
                    canvas.Clear(SKColors.Tan);
                    using (SKPaint paint = new SKPaint())
                    {
                        paint.Color = SKColors.Blue;
                        paint.Style = SKPaintStyle.Fill;
                        canvas.DrawCircle(b.self.x, b.self.y, b.self.r, paint);
                        if (b.Other_ID.Count() != null)
                        {
                            Random random = new Random();
                            foreach (string other in b.Other_ID.Keys)
                            {
                                if (other == b.ID) continue;
                                paint.Color = SKColors.Green;
                                canvas.DrawCircle(b.Other_ID[other].x, b.Other_ID[other].y, b.Other_ID[other].r, paint);
                            }
                        }
                        if (b.little_balls != null && b.little_balls.Count > 0)
                        {
                            foreach (little_ball smb_i in b.little_balls)
                            {
                                paint.Color = new SKColor(Convert.ToByte(smb_i.col_b), Convert.ToByte(smb_i.col_g), Convert.ToByte(smb_i.col_r));
                                canvas.DrawCircle(smb_i.x, smb_i.y, 10, paint);
                            }
                        }
                    }
                    using (SKImage image = surface.Snapshot())
                    using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
                    using (MemoryStream mstream = new MemoryStream(data.ToArray()))
                    {
                        Bitmap bm = new Bitmap(mstream, false);
                        pictureBox1.Image = bm;
                    }
                }
            }
        }

    }
}