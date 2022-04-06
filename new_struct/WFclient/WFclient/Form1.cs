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
using System.Diagnostics;
using System.Media;

namespace WFclient
{
    public partial class Form1 : Form
    {
        private float formsize_x;
        private float formsize_y;
        bool started = false;
        Ball b;
        Balls control;
        SocketHelper SocketH = new SocketHelper();
        private Graphics g;
        private SolidBrush myBrush = new SolidBrush(System.Drawing.Color.Red);
        private Thread thread_sender;
        private Thread thread_receiver;
        private Thread thread_render;
        SKImageInfo sKImageInfo;
        public Form1()
        {
            InitializeComponent();
            b = new Ball();
            control = new Balls();
            b.self = new little_ball();
            b.Other_ID = new Dictionary<string, little_ball>();
            b.little_balls = new List<little_ball>();
            b.self.move = 'n';
            b.self.x = 0;
            b.self.y = 0;
            b.self.r = 0;
            b.self.Dead = false;
            button1.Location = new Point(this.Size.Width / 2 - button1.Width / 2, this.Size.Height / 2 - button1.Height / 2);
            button2.Location = new Point(this.Size.Width / 2 - button1.Width / 2, this.Size.Height / 2 - button1.Height / 2);
            button2.Visible = false;
            button2.Enabled = false;
            button3.Visible = false;
            button3.Enabled = false;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Size = new Size(this.Size.Width, this.Size.Height);
            pictureBox1.SendToBack();
            sKImageInfo = new SKImageInfo(this.Size.Width, this.Size.Height);
            label1.Parent = pictureBox1;
            label2.Parent = pictureBox1;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            formsize_x = this.Width;
            formsize_y = this.Height;
            setTag(this);
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            float new_x = (this.Width) / formsize_x;
            float new_y = (this.Height) / formsize_y;
            setControls(new_x, new_y, this);
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
                (thread_sender = new(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(1);
                        //control.Count_collision(ref b);
                        lock (b)
                        {
                            control.Ball_move(ref b);
                            SocketH.Send(ref b);
                            if (b.self.Dead == true) break;
                            Invoke(() =>
                            {
                                label2.Text = b.self.move.ToString();
                            });
                        }
                    }
                })
                { IsBackground = true }).Start();

                (thread_receiver = new(() =>
                {
                    int count = 0;
                    double ping = 0;
                    DateTime dateTime = DateTime.Now;
                    DateTime LastRev = DateTime.Now;
                    while (true)
                    {
                        string rev = SocketH.Receive();
                        if (rev != "")
                            b = JsonSerializer.Deserialize<Ball>(rev);
                        if (rev == "")
                        {
                            SocketH.Init(); 
                        }
                        Invoke(() =>
                        {
                            if (rev != "")
                            {
                                count++;
                                if ((DateTime.Now - dateTime).TotalMilliseconds > 500)
                                {
                                    dateTime = DateTime.Now;
                                    ping = (dateTime - LastRev).TotalMilliseconds;
                                }
                                label1.Text = string.Format("cnt:{0} ping:{1} ms", count.ToString(), ping);
                                LastRev = DateTime.Now;
                            }
                        });
                        if (b.self.Dead == true) break;
                    }
                })
                { IsBackground = true }).Start();

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
            SocketH.Send(ref b);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            started = false;
            System.Environment.Exit(0);
        }
        private void button3_Click(object sender, EventArgs e)
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
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ((char)Keys.Escape))
            {
                if (button3.Visible == true)
                {
                    button3.Visible = false;
                    button3.Enabled = false;
                }
                else
                {
                    button3.Visible = true;
                    button3.Enabled = true;
                }
            }
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
                label1.BackColor = Color.Transparent;
                label2.BackColor = Color.Transparent;
                using (SKSurface surface = SKSurface.Create(sKImageInfo))
                {
                    SKCanvas canvas = surface.Canvas;
                    canvas.Clear(SKColors.Tan);
                    using (SKPaint paint = new SKPaint())
                    {
                        SKPaint textPaint = new SKPaint();
                        textPaint.Color = SKColors.Black;
                        textPaint.TextSize = b.self.r;
                        paint.Color = SKColors.Blue;
                        paint.Style = SKPaintStyle.Fill;
                        float text_x = b.self.x - (textPaint.MeasureText(b.self.r.ToString())/2);
                        float text_y = b.self.y + (textPaint.TextSize / 2);
                        canvas.DrawCircle(b.self.x, b.self.y, b.self.r, paint);
                        canvas.DrawText(b.self.r.ToString(), text_x, text_y, textPaint);
                        if (b.Other_ID.Count() != 0)
                        {
                            foreach (string other in b.Other_ID.Keys)
                            {
                                if (other == b.ID) continue;
                                if (b.Other_ID[other].Dead == true) continue;
                                paint.Color = SKColors.Green;
                                textPaint.TextSize = b.Other_ID[other].r;
                                canvas.DrawCircle(b.Other_ID[other].x, b.Other_ID[other].y, b.Other_ID[other].r, paint);
                                text_x = b.Other_ID[other].x - (textPaint.MeasureText(b.Other_ID[other].r.ToString()) / 2);
                                text_y = b.Other_ID[other].y + (textPaint.TextSize / 2);
                                canvas.DrawText(b.Other_ID[other].r.ToString(), text_x, text_y, textPaint);
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
        private void setTag(Control cons)
        {
            foreach (Control con in cons.Controls)
            {
                con.Tag = con.Width + ":" + con.Height + ":" + con.Left + ":" + con.Top + ":" + con.Font.Size;
                if (con.Controls.Count > 0)
                    setTag(con);
            }
        }
        private void setControls(float newx, float newy, Control cons)
        {
            //遍歷窗體中的控制項，重新設置控制項的值
            foreach (Control con in cons.Controls)
            {

                string[] mytag = con.Tag.ToString().Split(new char[] { ':' });//獲取控制項的Tag屬性值，並分割後存儲字元串數組
                float a = System.Convert.ToSingle(mytag[0]) * newx;//根據窗體縮放比例確定控制項的值，寬度
                con.Width = (int)a;//寬度
                a = System.Convert.ToSingle(mytag[1]) * newy;//高度
                con.Height = (int)(a);
                a = System.Convert.ToSingle(mytag[2]) * newx;//左邊距離
                con.Left = (int)(a);
                a = System.Convert.ToSingle(mytag[3]) * newy;//上邊緣距離
                con.Top = (int)(a);
                Single currentSize = System.Convert.ToSingle(mytag[4]) * newy;//字體大小
                con.Font = new Font(con.Font.Name, currentSize, con.Font.Style, con.Font.Unit);
                if (con.Controls.Count > 0)
                {
                    setControls(newx, newy, con);
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            b.self.Dead = true;
            SocketH.Send(ref b);
            started = false;
            System.Environment.Exit(0);
        }
    }
}