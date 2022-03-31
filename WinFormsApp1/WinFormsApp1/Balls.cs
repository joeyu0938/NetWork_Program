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
        public System.Net.EndPoint s { get; set; }
        public Dictionary<string, Ball> Other_ID { get; set; }
        public List<litte_ball> little_balls { get; set; }
        public string ID { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int r { get; set; }
        public bool collision { get; set; }
        public bool Eat { get; set; }
        public bool Dead { get; set; }
        public char move { get; set; }
    }
    public class litte_ball // 吃的小球class
    {
        public int x, y;
    }

    public class Balls //對Ball 操作的類別
    {
        //最一開始才要用
        public void random_little_balls(int number, ref List<litte_ball> l)
        {
            Random random = new Random();
            for(int i = 0; i < number; i++)
            {
                litte_ball tmp = new litte_ball();
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
        public void Count_collision(ref Dictionary<string, Ball> other,ref List<litte_ball> little_ball_set)
        {
            Balls control = new Balls();
            //我先用n^2 寫
            foreach(KeyValuePair<string, Ball>  x in other)
            {
                foreach(KeyValuePair<string,Ball> y in other)
                {
                    if (x.Key == y.Key) continue;
                    if(Math.Pow(Math.Abs(x.Value.x - y.Value.x),2) + Math.Pow(Math.Abs(x.Value.y - y.Value.y),2) < Math.Pow(x.Value.r + y.Value.r, 2))
                    {
                        x.Value.collision = true;
                        y.Value.collision = true;
                        if (x.Value.r > y.Value.r)
                        {
                            y.Value.Dead = true;
                            litte_ball c =new litte_ball();
                            c.x = y.Value.x;
                            c.y = y.Value.y;
                            little_ball_set.Add(c);
                        }
                        else
                        {
                            x.Value.Dead = true;
                            litte_ball c = new litte_ball();
                            c.x = x.Value.x;
                            c.y = x.Value.y;
                            little_ball_set.Add(c);
                        }
                    }
                }
            }
        }
        public void Ball_move(ref Dictionary<string, Ball> dic , string id)//移動
        {
            switch (dic[id].move)
            {
                case 'w':
                    dic[id].y -= 1;
                    break;
                case 'd':
                    dic[id].x += 1;
                    break;
                case 'a':
                    dic[id].x -= 1;
                    break;
                case 's':
                    dic[id].y += 1;
                    break;
                default:
                    return;
            }
            litte_ball d = new litte_ball();
            d.x = dic[id].x;
            d.y = dic[id].y;
            if (dic[id].little_balls.Contains(d))
            {
                dic[id].little_balls.Remove(d);
                dic[id].r += 3;//半徑變大
            }
        }
    }
}