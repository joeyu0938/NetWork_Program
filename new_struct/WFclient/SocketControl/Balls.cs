using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Net.Sockets;
using System.Media;
using System.Diagnostics;
using System.ComponentModel;

namespace Classlibary
{
    [Serializable]
    public class Ball // 玩家ball socket傳送的class
    {
        public Dictionary<string, little_ball> Other_ID { get; set; }
        public List<little_ball> little_balls { get; set; }
        public little_ball self { get; set; }
        public string ID { get; set; }
    }
    [Serializable]
    public class little_ball // 吃的小球class
    {
        public int x { get; set; }
        public int y { get; set; }
        public int col_r { get; set; }
        public int col_g { get; set; }
        public int col_b { get; set; }
        public int r { get; set; }
        public bool collision { get; set; }
        public bool Eat { get; set; }
        public bool Dead { get; set; }
        public char move { get;set; }
    }
    [Serializable]
    public class Balls //對Ball 操作的類別
    {
        public void LoadAsyncSound()
        {
            SoundPlayer Player = new SoundPlayer();
            try
            {
                string fullPath = Path.GetFullPath("eat.wav");
                Player.SoundLocation = fullPath;
                Player.LoadAsync();
                Player.Play();
            }
            catch (Exception ex)
            {
                Player.Dispose();
            }
        }
        //最一開始才要用
        public void random_little_balls(int number, ref List<little_ball> l)
        {
            Random random = new Random();
            for (int i = 0; i < number; i++)
            {
                little_ball tmp = new little_ball();
                tmp.x = random.Next(0, 1500);
                tmp.y = random.Next(0, 850);
                if (!l.Contains(tmp))
                {
                    l.Add(tmp);
                }
                else i--;
            }
        }
        //最一開始才要用
        public void Count_collision(ref Ball set)
        {
            Balls control = new Balls();
            //我先用n^2 寫
            if (set.self.Dead == true) return;
            foreach (KeyValuePair<string, little_ball> y in set.Other_ID)
            {
                if (Math.Pow(Math.Abs(set.self.x - y.Value.x), 2) + Math.Pow(Math.Abs(set.self.y - y.Value.y), 2) < Math.Pow(set.self.r + y.Value.r, 2))
                {
                    set.self.collision = true;
                    y.Value.collision = true;
                    if (set.self.r > y.Value.r)
                    {
                        y.Value.Dead = true;
                    }
                    else
                    {
                        set.self.Dead = true;
                    }
                }
            }
        }
        public void Ball_move(ref Ball set)//移動
        {
            if (set == null) return;
            switch (set.self.move)
            {
                case 'w':
                    set.self.y -= 3;
                    break;
                case 'd':
                    set.self.x += 3;
                    break;
                case 'a':
                    set.self.x -= 3;
                    break;
                case 's':
                    set.self.y += 3;
                    break;
                default:
                    return;
            }
            if (set.self.x < 0) set.self.x = 0;
            if (set.self.x > 1920) set.self.x = 1920;
            if (set.self.y < 0) set.self.y = 0;
            if (set.self.y > 1080) set.self.y = 1080;
            for (int i = set.little_balls.Count - 1; i >= 0; i--)
            {
                if (Math.Pow(Math.Abs(set.self.x - set.little_balls[i].x), 2) + Math.Pow(Math.Abs(set.self.y - set.little_balls[i].y), 2) < Math.Pow(set.self.r + set.little_balls[i].r, 2))
                {
                    LoadAsyncSound();
                    set.self.r += 5;
                    set.little_balls.Remove(set.little_balls[i]);
                }
            }
        }
    }
}