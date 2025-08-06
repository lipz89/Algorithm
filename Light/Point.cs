using System.Collections.Generic;

namespace Light
{
    class Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
            Links = new List<Point>();
        }
        public int X { get; }
        public int Y { get; }
        public bool State { get; set; }
        public bool Clicked { get; set; }
        public List<Point> Links { get; }
    }
}
