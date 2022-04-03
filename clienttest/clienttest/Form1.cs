using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using Classlibary;
using System.Text.Json;
using System.Collections.Generic;
using SkiaSharp;
namespace clienttest
{
    public partial class Form1 : Form
    {
        bool started = false;
        Ball b;
        Balls control;
        List<little_ball> random_little_ball_set;
        private Graphics g;
        private SolidBrush myBrush = new SolidBrush(System.Drawing.Color.Red);
        private Thread? thread_render;
        private Thread? thread_sender;
        private int fps;
        SKImageInfo sKImageInfo;
        public Form1()
        {
            InitializeComponent();
            g = this.CreateGraphics();
            //this.WindowState = FormWindowState.Maximized;
            this.KeyPreview = true; 
            this.DoubleBuffered = true; 
            b = new Ball();
            control = new Balls();
            this.TopMost = true;
            button1.Location = new Point(this.Size.Width / 2 - button1.Width / 2, this.Size.Height / 2 - button1.Height / 2);
            b.self = new little_ball();
            b.Other_ID = new Dictionary<string, little_ball>();
            b.little_balls = new List<little_ball>();
            b.self.x = 50;
            b.self.y = 50;
            b.self.r = 50;
            b.self.move = 'n';
            fps = 60;
            random_little_ball_set = new List<little_ball>();
            control.random_little_balls(100, ref random_little_ball_set);
            b.little_balls = random_little_ball_set;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Size = new Size(this.Size.Width, this.Size.Height);
            button1.BringToFront();
            sKImageInfo = new SKImageInfo(this.Size.Width, this.Size.Height);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            (thread_sender = new(() =>
            {
                while (true)
                {
                    Thread.Sleep(1);
                    control.Ball_move(ref b);
                }
            })
            { IsBackground = true }).Start();
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            g = this.CreateGraphics();
            button1.Location = new Point(this.Size.Width / 2 - button1.Width / 2, this.Size.Height / 2 - button1.Height / 2);
            pictureBox1.Size = new Size(this.Size.Width, this.Size.Height);
            sKImageInfo = new SKImageInfo(this.Size.Width, this.Size.Height);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            button1.Visible = false;
            started = true;
            if (started)
            {
                (thread_render = new(() =>
                {
                    while (true)
                    {
                        Render();
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
            b.self.move = 'n';
        }
        private void Render()
        {
            using (SKSurface surface = SKSurface.Create(sKImageInfo))
            {
                SKCanvas canvas = surface.Canvas;
                canvas.Clear(SKColors.Tan);
                using (SKPaint paint = new SKPaint())
                {
                    paint.Color = SKColors.Blue;
                    paint.Style = SKPaintStyle.Fill;
                    canvas.DrawCircle(b.self.x, b.self.y, b.self.r, paint);
                    paint.Color = SKColors.Red;
                    if (b.little_balls != null && b.little_balls.Count > 0)
                    {
                        foreach (little_ball smb_i in b.little_balls)
                        {
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