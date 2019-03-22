using System;
using System.Collections.Generic;
using System.Linq;

namespace HorseCore
{
    class Solver2
    {
        private readonly Map map;
        private readonly Point start;

        public Solver2(int width, int height, int _x = 1, int _y = 1)
        {
            if (width < 3 || height < 3)
            {
                throw new Exception("棋盘太小了！");
            }

            if (_x < 1 || _y < 1 || _x > width || _y > height)
            {
                throw new Exception("指定起点不在棋盘内！");
            }

            this.x = _x - 1;
            this.y = _y - 1;
            this.W = width;
            this.H = height;
            this.map = new Map(W, H);
            this.start = map[x, y];
        }

        private List<int[,]> steps;
        public long NodeCount { get; private set; }
        private int x, y;
        public int X => x + 1;
        public int Y => y + 1;
        public int W { get; }
        public int H { get; }
        public int[][,] GetSteps(int count = 1, bool onlyArount = false)
        {
            if (W * H % 2 == 1 && (x + y) % 2 == 1) // 奇数格棋盘交错无解 
                return null;
            if (W == 4 && (x == 1 || x == 2)) // 四排格棋盘二三排无解 
                return null;
            if (H == 4 && (y == 1 || y == 2))
                return null;
            if (W == 4 && H == 4) // 4*4格全部无解
                return null;
            if (onlyArount && W * H % 2 == 1)//奇数格棋盘无环游解
                return null;

            steps = new List<int[,]>();
            Step(start, count, 1, onlyArount);
            return steps.ToArray();
        }

        private void Step(Point point, int count, int step, bool onlyArount)
        {
            if (step < W * H - 1)
            {
                // 孤立格剪枝，剪枝会很大程度上提升搜索效率
                if (point.Links.Any(x => x.Links.Count == 1))
                {
                    return;
                }
                // 环游格剪枝
                if (onlyArount && step > 2 && start.CanLink(point) && start.Links.Count(x => x.State == 0) == 1)
                {
                    return;
                }
            }

            this.NodeCount++;
            map.In(point, step);

            if (step == W * H)
            {
                var gotAnswer = true;
                if (onlyArount)
                {
                    gotAnswer = start.CanLink(point);
                }

                if (gotAnswer)
                {
                    // 求到解，将解法填充到输出区域
                    var answer = new int[W, H];
                    for (int i = 0; i < W; i++)
                    {
                        for (int j = 0; j < H; j++)
                        {
                            answer[i, j] = this.map[i, j].State;
                        }
                    }

                    steps.Add(answer);
                    if (steps.Count >= count)
                    {
                        return;
                    }
                }
            }
            else
            {
                // 贪心策略，取连接格少的先走，因为多的先走掉的话，之后可通的路就少了
                // 该贪心策略只对求少量解时大部分情况下有很高的效率提升，对于少量情况甚至有可能降低效率
                // 对于求所有解没有任何帮助
                if (point.Links.Count > 1)
                    point.Links.Sort(PointComparer.Default);

                foreach (var link in point.Links)
                {
                    this.Step(link, count, step + 1, onlyArount);
                    if (steps.Count >= count)
                    {
                        return;
                    }
                    // 单通非终点格截枝 
                    if (link.Links.Count == 1)
                    {
                        // 非最后黑白同色格 
                        if (step % 2 == W * H % 2)
                        {
                            break;
                        }
                        // 非最后环游格
                        if (onlyArount && !start.CanLink(link))
                        {
                            break;
                        }
                    }
                }
            }

            map.Out(point);
        }

        public override string ToString()
        {
            var str = string.Empty;
            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    str += map[i, j].ToString().PadLeft(2, ' ') + " ";
                }

                str += Environment.NewLine;
            }

            return str;
        }
    }

    class Point
    {
        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public int X { get; }
        public int Y { get; }
        public int State { get; set; }
        public List<Point> Links { get; set; }

        public bool CanLink(Point p)
        {
            var dx = Math.Abs(p.X - this.X);
            var dy = Math.Abs(p.Y - this.Y);
            return dx + dy == 3 && Math.Abs(dx - dy) == 1;
        }
    }

    class PointComparer : IComparer<Point>
    {
        public int Compare(Point x, Point y)
        {
            return x.Links.Count - y.Links.Count;
        }

        public static IComparer<Point> Default { get; } = new PointComparer();
    }
    class Map
    {
        private static readonly int[][] moves =
        {
            new[] {1, 2}, new[] {2, 1},
            new[] {-1, 2}, new[] {-2, 1},
            new[] {1, -2}, new[] {2, -1},
            new[] {-1, -2}, new[] {-2, -1},
        };
        private readonly List<Point> list;
        public Point this[int x, int y]
        {
            get { return list.Single(p => p.X == x && p.Y == y); }
        }
        public Map(int w, int h)
        {
            list = new List<Point>();
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    list.Add(new Point(i, j) { Links = new List<Point>() });
                }
            }

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    var point = this[i, j];
                    foreach (var move in moves)
                    {
                        var x = i + move[0];
                        var y = j + move[1];
                        if (x < 0 || y < 0 || x >= w || y >= h)
                        {
                            continue;
                        }
                        point.Links.Add(this[x, y]);
                    }
                }
            }
        }

        public void In(Point p, int step)
        {
            p.Links.ForEach(x => x.Links.Remove(p));
            p.State = step;
        }
        public void Out(Point p)
        {
            p.Links.ForEach(x => x.Links.Add(p));
            p.State = 0;
        }
    }
}