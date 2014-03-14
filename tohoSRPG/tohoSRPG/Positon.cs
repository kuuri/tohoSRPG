using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace tohoSRPG
{
    struct Positon
    {
        public int X;
        public int Y;

        public Positon(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Positon operator +(Positon value1, Positon value2)
        {
            Positon result = value1;
            result.X += value2.X;
            result.Y += value2.Y;

            return result;
        }

        public static Positon operator -(Positon value1, Positon value2)
        {
            Positon result = value1;
            result.X -= value2.X;
            result.Y -= value2.Y;

            return result;
        }

        public static bool operator ==(Positon value1, Positon value2)
        {
            return value1.X == value2.X && value1.Y == value2.Y;
        }

        public static bool operator !=(Positon value1, Positon value2)
        {
            return value1.X != value2.X || value1.Y != value2.Y;
        }

        public static float Distance(Positon value1, Positon value2)
        {
            int x = Math.Abs(value1.X - value2.X);
            int y = Math.Abs(value1.Y - value2.Y);

            return Math.Min(x, y) * 1.5f + Math.Abs(x - y);
        }

        public static implicit operator Vector2(Positon value)
        {
            return new Vector2(value.X, value.Y);
        }

        public static Positon Zero
        {
            get { return new Positon(0, 0); }
        }
    }
}
