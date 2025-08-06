using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Light
{
    /// <summary>
    /// 九宝莲灯
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            for(int i = 3; i <= 20; i++)
            {
                for(int j = 3; j <= i; j++)
                {
                    Test(j, i);
                }
            }
            //Test(16,16);
            //var ts = ReadHistory();
            //for(int i = 3; i <= 20; i++)
            //{
            //    for(int j = 3; j <= i; j++)
            //    {
            //        if(ts.Any(x => x.Item1 == j && x.Item2 == i))
            //        {
            //            continue;
            //        }
            //        Test(i, j);
            //    }
            //}
            //Console.Read();
        }

        private static List<Tuple<int, int>> ReadHistory()
        {
            Directory.CreateDirectory("../../solvers");
            if(File.Exists("../../hitorys.txt"))
            {
                var txt = File.ReadAllLines("../../hitorys.txt").Where(x => x.Length > 0);
                return txt.Select(x => x.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Take(2).Select(i => int.Parse(i)).ToArray()).Select(x => new Tuple<int, int>(x[0], x[1])).ToList();
            }
            return new List<Tuple<int, int>>();
        }

        private static string solverFile = "../../newsolvers.md";
        private static void Test(int w, int h)
        {
            Console.WriteLine($"棋盘大小({w}*{h})");
            File.AppendAllText(solverFile, $"### ({w}*{h})");
            var map = new Map(w, h);
            var count = map.Solver();

            Console.WriteLine($"遍历节点数：{map.NodeCount}");
            File.AppendAllText(solverFile, $"\r\n\r\n 遍历节点数：{map.NodeCount}， ");
            if(count == 0)
            {
                Console.WriteLine("无解");
                File.AppendAllText(solverFile, $"无解，");
                //File.AppendAllText("../../hitorys.txt", $"\r\n{h} {w} 0");
            }
            else
            {
                Console.WriteLine($"解法总数:{count}");
                File.AppendAllText(solverFile, $"解法总数:{count}， ");
                //File.AppendAllText("../../hitorys.txt", $"\r\n{h} {w} {count}");
            }

            Console.WriteLine($"共计耗时：{map.UseTime.ToString("g")}");
            File.AppendAllText(solverFile, $"共计耗时：{map.UseTime.ToString("g")}\r\n");
            Console.WriteLine();
        }

        internal static void Print(int w, int h, int index, bool[] step)
        {
            Console.WriteLine($"解法{index}   \t( COUNT =  {step.Count(x => x)}  ):");
            File.AppendAllText(solverFile, $"\r\n#### {index}.   ( {step.Count(x => x)} ):");
            var info = "";
            for(int i = 0; i < w; i++)
            {
                if(step[i])
                {
                    info += (i + 1) + ", ";
                }
            }
            Console.WriteLine($"    提示：\t{info.Trim(',')}");
            File.AppendAllText(solverFile, $"\r\n\t> {info.Trim(',',' ')}");

            //Console.WriteLine("┌" + new string('—', w) + "┐");
            //for(int j = 0; j < h; j++)
            //{
            //    Console.Write("│");
            //    for(int i = 0; i < w; i++)
            //    {
            //        Console.Write(step[i + j * w] ? " *" : "  ");
            //    }

            //    Console.WriteLine("│");
            //}
            //Console.WriteLine("└" + new string('—', w) + "┘");
            //ImageCreator.PrintImage(w, h, index, step);
        }
        internal static void Print2(int w, int h, int index, bool[] step)
        {
            Console.WriteLine($"总计解法：{index}   \t最小步数 {step.Count(x => x)}:");
            File.AppendAllText(solverFile, $"\r\n总计解法：{index}，   \t最小步数： {step.Count(x => x)}:");
            var info = "";
            for(int i = 0; i < w; i++)
            {
                if(step[i])
                {
                    info += (i + 1) + ", ";
                }
            }
            Console.WriteLine($"    提示：\t{info.Trim(',')}");
            File.AppendAllText(solverFile, $"\r\n\r\n\t {info.Trim(',', ' ')}");

            //Console.WriteLine("┌" + new string('—', w) + "┐");
            //for(int j = 0; j < h; j++)
            //{
            //    Console.Write("│");
            //    for(int i = 0; i < w; i++)
            //    {
            //        Console.Write(step[i + j * w] ? " *" : "  ");
            //    }

            //    Console.WriteLine("│");
            //}
            //Console.WriteLine("└" + new string('—', w) + "┘");
            //ImageCreator.PrintImage(w, h, index, step);
        }
    }
}
