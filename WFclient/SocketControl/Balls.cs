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
        private Dictionary<string, Ball> Other_ID;
        public Dictionary<string, Ball> Set_Other_ID
        {
            get => Other_ID;
            set => Other_ID = value;

        }
        private List<litte_ball> little_balls;
        public List<litte_ball> Set_little_balls
        {
            get => little_balls;
            set => little_balls = value;
        }
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
        public void ADD_SELF(string s, ref Ball b)
        {
            b.ID = s;
        }
        public void ADD_Other_ID(string s, ref Ball b)
        {
            bool exist = b.Set_Other_ID.ContainsKey(s);
            if (exist == false)
            {
                b.Set_Other_ID.Add(s, b);
            }
            else
            {
                Console.WriteLine("Add already");
            }
        }
        public void Add_litle_balls(int x, int y,ref Ball b)
        {
            litte_ball tmp = new litte_ball();
            tmp.x = x;
            tmp.y = y;
            if (b.Set_little_balls.Contains(tmp)) return;
            b.Set_little_balls.Add(tmp);
        }
        public void random_little_balls(int number,ref Ball b)
        {
            Random random = new Random();
            for(int i = 0; i < number; i++)
            {
                Add_litle_balls(random.Next(0,1500), random.Next(0,850),ref b);//視窗x.y;
            }
        }
        public void Count_collision(ref Dictionary<string, Ball> other)
        {
            Balls control = new Balls();
            //我先用n^2 寫
            foreach(KeyValuePair<string, Ball>  x in other)
            {
                foreach(KeyValuePair<string,Ball> y in other)
                {
                    if(Math.Pow(Math.Abs(x.Value.x - y.Value.x),2) + Math.Pow(Math.Abs(x.Value.y - y.Value.y),2) < Math.Pow(x.Value.r + y.Value.r, 2))
                    {
                        x.Value.collision = true;
                        y.Value.collision = true;
                        if (x.Value.r > y.Value.r)
                        {
                            y.Value.Dead = true;
                            Ball b = y.Value;
                            control.Add_litle_balls(y.Value.x, y.Value.y, ref b);
                            other[y.Key]= b;
                        }
                        else
                        {
                            x.Value.Dead = true;
                            Ball b = x.Value;
                            control.Add_litle_balls(x.Value.x, x.Value.y, ref b);
                            other[x.Key] = b;
                        }
                    }
                }
            }
        }
        public void Ball_move(ref Ball b)//移動
        {
            switch (b.move)
            {
                case 'd':
                    b.y -= 1;
                    break;
                case 'r':
                    b.x += 1;
                    break;
                case 'l':
                    b.x -= 1;
                    break;
                case 'u':
                    b.y += 1;
                    break;
            }
        }
    }
}