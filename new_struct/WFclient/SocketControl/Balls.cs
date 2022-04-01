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
        public int r { get; set; }
        public bool collision { get; set; }
        public bool Eat { get; set; }
        public bool Dead { get; set; }
        public char move { get;set; }
    }
    [Serializable]
    public class Balls //對Ball 操作的類別
    {
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
        public void Count_collision(ref Dictionary<string, Ball> other, ref List<little_ball> little_ball_set)
        {
            Balls control = new Balls();
            //我先用n^2 寫
            if (other.Count == 0) return;
            foreach (KeyValuePair<string, Ball> x in other)
            {
                foreach (KeyValuePair<string, Ball> y in other)
                {
                    if (x.Key == y.Key) continue;
                    if (Math.Pow(Math.Abs(x.Value.self.x - y.Value.self.x), 2) + Math.Pow(Math.Abs(x.Value.self.y - y.Value.self.y), 2) < Math.Pow(x.Value.self.r + y.Value.self.r, 2))
                    {
                        x.Value.self.collision = true;
                        y.Value.self.collision = true;
                        if (x.Value.self.r > y.Value.self.r)
                        {
                            y.Value.self.Dead = true;
                            little_ball c = new little_ball();
                            c.x = y.Value.self.x;
                            c.y = y.Value.self.y;
                            c.r = 1;
                            little_ball_set.Add(c);
                        }
                        else
                        {
                            x.Value.self.Dead = true;
                            little_ball c = new little_ball();
                            c.x = x.Value.self.x;
                            c.y = x.Value.self.y;
                            c.r = 1;
                            little_ball_set.Add(c);
                        }
                    }
                }
            }
        }
        public void Ball_move(ref Ball set, string id, ref List<little_ball> little_ball_set)//移動
        {
            if (set == null) return;
            switch (set.self.move)
            {
                case 'w':
                    set.self.y -= 1;
                    break;
                case 'd':
                    set.self.x += 1;
                    break;
                case 'a':
                    set.self.x -= 1;
                    break;
                case 's':
                    set.self.y += 1;
                    break;
                default:
                    return;
            }
            little_ball d = new little_ball();
            d.x = set.self.x;
            d.y = set.self.y;
            if (little_ball_set.Contains(d))
            {
                little_ball_set.Remove(d);
                set.self.r += 3;//半徑變大
            }
        }
    }
}