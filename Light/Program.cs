using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Light
{
    class Program
    {
        static void Main(string[] args)
        {
            //Test(4, 4, 1);
            for (int i = 3; i <= 15; i++)
            {
                for (int j = 3; j <= i; j++)
                {
                    Test(i, j, int.MaxValue);
                }
            }
            Console.Read();
        }

        private static void Test(int w, int h, int count)
        {
            Console.WriteLine($"棋盘大小({w}*{h})");
            var map = new Map(w, h);
            Stopwatch stopwatch = Stopwatch.StartNew();
            var steps = map.Solver(count);
            stopwatch.Stop();
            Console.WriteLine("遍历节点数：" + map.NodeCount);
            if (!steps.Any())
            {
                Console.WriteLine("无解");
            }
            else
            {
                Console.WriteLine("解法总数:" + steps.Count);
                for (int index = 0; index < steps.Count; index++)
                {
                    Console.WriteLine("解法" + (index + 1) + ":");
                    var step = steps[index];
                    Console.WriteLine("┌" + new string('—', w) + "┐");

                    for (int j = 0; j < h; j++)
                    {
                        Console.Write("│");
                        for (int i = 0; i < w; i++)
                        {
                            Console.Write(step[i, j] ? " *" : "  ");
                        }

                        Console.WriteLine("│");
                    }

                    Console.WriteLine("└" + new string('—', w) + "┘");
                    ImageCreator.PrintImage(w, h, index, step);
                }
            }

            var time = stopwatch.Elapsed;
            Console.WriteLine("共计耗时：" + time.ToString("g"));
            Console.WriteLine();
        }
    }

    static class ImageCreator
    {
        private const int LENGTH = 20;
        private const int STAR_LEN = 4;
        private const int OFFSET = (LENGTH - STAR_LEN) / 2;

        private static Pen star = new Pen(Color.Black, 2);

        public static void PrintImage(int w, int h, int index, bool[,] step)
        {
            var iw = w * LENGTH + 1;
            var ih = h * LENGTH + 1;
            var image = new Bitmap(iw, ih);
            var gp = Graphics.FromImage(image);
            gp.Clear(Color.LawnGreen);
            for (int i = 0; i <= w; i++)
            {
                gp.DrawLine(Pens.Black, LENGTH * i, 0, LENGTH * i, ih);
            }
            for (int i = 0; i <= h; i++)
            {
                gp.DrawLine(Pens.Black, 0, LENGTH * i, iw, LENGTH * i);
            }

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    if (step[i, j])
                    {
                        var x = i * LENGTH + OFFSET;
                        var y = j * LENGTH + OFFSET;
                        gp.DrawEllipse(star, x, y, STAR_LEN, STAR_LEN);
                    }
                }
            }
            gp.Dispose();
            image.Save($"solvers/{w}-{h}-{index + 1}.png", ImageFormat.Png);
        }
    }

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

    class Map
    {
        public Map(int width, int height)
        {
            Width = width;
            Height = height;
            this.Points = new List<Point>();
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    this.Points.Add(new Point(j, i));
                }
            }
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    var point = this[j, i];
                    point.Links.Add(point);
                    if (j > 0) point.Links.Add(this[j - 1, i]);
                    if (i > 0) point.Links.Add(this[j, i - 1]);
                    if (j < Width - 1) point.Links.Add(this[j + 1, i]);
                    if (i < Height - 1) point.Links.Add(this[j, i + 1]);
                }
            }
        }
        public Point this[int x, int y]
        {
            get { return this.Points.Single(p => p.X == x && p.Y == y); }
        }
        public int Width { get; }
        public int Height { get; }
        public long NodeCount { get; set; }
        public List<Point> Points { get; }

        public void On(int x, int y)
        {
            var point = this[x, y];
            point.Clicked = !point.Clicked;
            foreach (var link in point.Links)
            {
                link.State = !link.State;
            }
        }

        public bool IsSuccess()
        {
            return this.Points.All(x => x.State);
        }

        public bool[,] ToStep()
        {
            var m = new bool[Width, Height];
            foreach (var point in Points)
            {
                m[point.X, point.Y] = point.Clicked;
            }
            return m;
        }

        public List<bool[,]> Solver(int count = 1)
        {
            var result = new List<bool[,]>();
            Step(result, 0, count);
            return result;
        }

        private void Step(List<bool[,]> list, int pos, int count)
        {
            if (this.IsSuccess())
            {
                list.Add(ToStep());
                return;
            }

            if (pos >= Width * Height)
                return;

            var w = pos % Width;
            var h = pos / Width;
            //for (int i = 0; i < w - 1; i++)
            //{
            //    for (int j = 0; j < Height; j++)
            //    {
            //        if (!this[i, j].State)
            //        {
            //            return;
            //        }
            //    }
            //}

            // 剪枝 前面没有再填充机会的格子如果有未点亮的不继续填充下去
            // 只有前面没有再填充机会的格子都已经点亮了才继续填充
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < (i < w - 1 ? h : h - 1); j++)
                {
                    if (!this[i, j].State)
                    {
                        return;
                    }
                }
            }

            for (int dir = 0; dir < 2; dir++)
            {
                this.Step(list, pos + 1, count);
                if (list.Count >= count)
                {
                    return;
                }
                this.NodeCount++;
                this.On(w, h);
            }
        }
    }
}
