namespace Buddy.Clash.DefaultSelectors
{
    using System;
    using System.Collections.Generic;

    public class Cast
    {
        public string SpellName = "";
        public VectorAI Position;
        public Handcard hc = new Handcard();

        public Cast(string spellName, VectorAI position, Handcard handCard)
        {
            this.SpellName = spellName;
            this.Position = position;
            this.hc = handCard;
        }

        public override string ToString()
        {
            return this.SpellName + " " + this.Position.ToString();
        }
    }

    public class VectorAI
    {
        private int x = 0;
        private int y = 0;

        public VectorAI(int X, int Y)
        {
            x = X;
            y = Y;
        }
        
        public VectorAI(Engine.NativeObjects.Native.Vector2 pos)
        {
            x = pos.X;
            y = pos.Y;
        }

        public Engine.NativeObjects.Native.Vector2 ToVector2()
        {
            return new Engine.NativeObjects.Native.Vector2(x, y);
        }

        public VectorAI(int X, int Y, int random)
        {
            Random rnd = new Random();
            x = X + (random / 2 - rnd.Next(random));
            y = Y + (random / 2 - rnd.Next(random));
        }

        public VectorAI(string s) //{3500/25500}
        {
            s = s.Substring(1, s.Length - 2);
            string[] ss = s.Split('/');
            x = Convert.ToInt32(ss[0]);
            y = Convert.ToInt32(ss[1]);
        }

        public int X
        {
            get { return x; }
            set { x = value; }
        }
        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        public int getDistance(VectorAI v2)
        {
            return (int)Math.Sqrt((v2.x - x) * (v2.x - x) + (v2.y - y) * (v2.y - y));
        }

        public override string ToString()
        {
            return "{" + this.x + "/" + this.y + "}";
        }

        public void AddYInDirection(Playfield p, int y = 1000)
        {
            VectorAI moveVector = new VectorAI(0, y);
            //Logger.Debug("PlayerPosition: {0}", fieldPosition);

            if (p.home)
                this.Y -= (y * 4);
            else
                this.Y += y;
        }

        public void SubtractYInDirection(Playfield p, int y = 1000)
        {
            VectorAI moveVector = new VectorAI(0, y);
            //Logger.Debug("PlayerPosition: {0}", fieldPosition);

            if (p.home)
                this.Y += (y * 4);
            else
                this.Y -= y;
        }

    }
}