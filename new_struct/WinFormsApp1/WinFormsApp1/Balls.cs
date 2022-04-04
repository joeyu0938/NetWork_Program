using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Net.Sockets;
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
        public char move { get; set; }
    }

    public class Balls //對Ball 操作的類別
    {
        //最一開始才要用
        public void random_little_balls(int number, ref List<little_ball> l)
        {
            Random random = new Random();
            for(int i = 0; i < number; i++)
            {
                little_ball tmp = new little_ball();
                tmp.col_r = random.Next(255);
                tmp.col_g = random.Next(255);
                tmp.col_b = random.Next(255);
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
        public void Count_collision(ref Dictionary<string, Ball> other, string ID, ref List<little_ball> little_ball_set)
        {
            Balls control = new Balls();
            //我先用n^2 寫
            if (other[ID].self.Dead == true) return;
            foreach (KeyValuePair<string, Ball> y in other)
            {
                if (other[ID].ID == y.Key) continue;
                if (Math.Pow(Math.Abs(other[ID].self.x - y.Value.self.x), 2) + Math.Pow(Math.Abs(other[ID].self.y - y.Value.self.y), 2) < Math.Pow(other[ID].self.r + y.Value.self.r, 2))
                {
                    other[ID].self.collision = true;
                    y.Value.self.collision = true;
                    if (other[ID].self.r > y.Value.self.r)
                    {
                        y.Value.self.Dead = true;
                        //little_ball c = new little_ball();
                        //c.x = y.Value.self.x;
                        //c.y = y.Value.self.y;
                        //c.r = 1;
                        //little_ball_set.Add(c);
                    }
                    else if (other[ID].self.r < y.Value.self.r)
                    {
                        other[ID].self.Dead = true;
                        //little_ball c = new little_ball();
                        //c.x = other[ID].self.x;
                        //c.y = other[ID].self.y;
                        //c.r = 1;
                        //little_ball_set.Add(c);
                    }
                }
            }
        }
        public void Ball_move(ref Dictionary<string, Ball> set, string id, ref List<little_ball> little_ball_set)//移動
        {
            if (set == null) return;
            switch (set[id].self.move)
            {
                case 'w':
                    set[id].self.y -= 5;
                    break;
                case 'd':
                    set[id].self.x += 5;
                    break;
                case 'a':
                    set[id].self.x -= 5;
                    break;
                case 's':
                    set[id].self.y += 5;
                    break;
                default:
                    return;
            }
            for (int i = little_ball_set.Count - 1; i >= 0; i--)
            {
                if (Math.Pow(Math.Abs(set[id].self.x - little_ball_set[i].x), 2) + Math.Pow(Math.Abs(set[id].self.y - little_ball_set[i].y), 2) < Math.Pow(set[id].self.r + little_ball_set[i].r, 2))
                {
                    set[id].self.r += 5;
                    little_ball_set.Remove(little_ball_set[i]);
                }
            }
        }
    }
}