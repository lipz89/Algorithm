using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HorseCore
{
    /// <summary>
    /// 马踏棋盘
    /// </summary>
    class Program
    {
        static void Main()
        {
            //TestSimple(11, 6, 2, 1);
            //RunSingle(11, 6, 5, 2, true, $"solvers/horse-single-11-6.txt");
            try
            {
                Run(10, 30, true);
                //Task.Run(() => Run(10, 30, true)).Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.Read();
        }

        private static List<Dif> skips = new List<Dif>
        {
            //new Dif(){Width = 11,Height = 6,X = 0,Y = 2},

            //new Dif(){Width = 12,Height = 4,X = 2,Y = 0},
            //new Dif(){Width = 12,Height = 6,X = 0,Y = 0},
            //new Dif(){Width = 12,Height = 6,X = 0,Y = 1},
            //new Dif(){Width = 12,Height = 6,X = 4,Y = 1},

            //new Dif(){Width = 13,Height = 4,X = 2,Y = 0},
            //new Dif(){Width = 13,Height = 4,X = 5,Y = 0},
            //new Dif(){Width = 13,Height = 6,X = 1,Y = 0},
            //new Dif(){Width = 13,Height = 6,X = 5,Y = 1},

            //new Dif(){Width = 14,Height = 4,X = 2,Y = 0},
            //new Dif(){Width = 14,Height = 4,X = 5,Y = 0},
            //new Dif(){Width = 14,Height = 5,X = 3,Y = 0},
            //new Dif(){Width = 14,Height = 6,X = 2,Y = 0},
            //new Dif(){Width = 14,Height = 6,X = 3,Y = 2},
            //new Dif(){Width = 14,Height = 6,X = 4,Y = 0},
            //new Dif(){Width = 14,Height = 6,X = 6,Y = 1},


            //new Dif(){Width = 15,Height = 12,X = 4,Y = 4},
            //new Dif(){Width = 16,Height = 13,X = 3,Y = 2},


            //new Dif(){Width = 19,Height = 12,X = 4,Y = 3},

            //new Dif(){Width = 20,Height = 10,X = 2,Y = 1},
            //new Dif(){Width = 20,Height = 15,X = 7,Y = 0},
            //new Dif(){Width = 20,Height = 17,X = 7,Y = 2},
            //new Dif(){Width = 20,Height = 17,X = 8,Y = 0},
            //new Dif(){Width = 20,Height = 17,X = 8,Y = 3},


            //new Dif(){Width = 21,Height = 14,X = 9,Y = 4},
            //new Dif(){Width = 21,Height = 20,X = 10,Y = 5},

            //new Dif(){Width = 22,Height = 14,X = 8,Y = 2},
        };

        private static void TestSimple(int width, int height, int _x = 0, int _y = 0)
        {
            var solver = new Solver2(width, height, _x, _y);
            var timer = Stopwatch.StartNew();
            var steps = solver.GetSteps(onlyArount: false);
            timer.Stop();
            Console.WriteLine($"棋盘大小：{solver.W}*{solver.H}，起点位置({solver.X},{solver.Y})");
            Console.WriteLine($"访问节点数：{solver.NodeCount}");
            if (steps == null || !steps.Any())
            {
                Console.WriteLine("无解");
            }
            else
            {
                Console.WriteLine($"{steps.Length} 组解法");
                for (var index = 0; index < steps.Length; index++)
                {
                    Console.WriteLine($"第 {index + 1} 组解法：");
                    var step = steps[index];
                    var w = step.GetLength(0);
                    var h = step.GetLength(1);

                    for (int j = 0; j < h; j++)
                    {
                        for (int i = 0; i < w; i++)
                        {
                            Console.Write(step[i, j].ToString().PadLeft(3, ' ') + " ");
                        }

                        Console.WriteLine();
                    }

                    break;
                }

                Console.WriteLine($"{steps.Length} 组解法输出完毕");
            }

            Console.WriteLine("共计耗时：" + timer.Elapsed.ToString("g"));
        }

        static void Run(int min, int max, bool single)
        {
            for (int width = min; width <= max; width++)
            {
                for (int height = width == 3 ? 4 : 3; height <= width; height++)
                {
                    var file = string.Intern($"solvers/horse-{(single ? "single-" : "")}{width}-{height}.txt");
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                    //Console.Clear();
                    // RunAround(width, height, single, file);
                    RunAll(width, height, single, file);
                }
            }
        }

        private static void RunAll(int width, int height, bool single, string file)
        {
            for (int _i = 0; _i < width / 2 + width % 2; _i++)
            {
                for (int _j = 0; _j < (width == height ? _i : height / 2 + height % 2); _j++)
                {
                    var i = _i;
                    var j = _j;
                    var tokenSource = new CancellationTokenSource(100);
                    var run = Task.Run(() => RunSingle(width, height, i, j, single, file), tokenSource.Token);
                    run.ContinueWith(x =>
                    {
                        var dif = new Dif(width, height, i, j);
                        skips.Add(dif);

                        Print(file, dif + "\r\n--------------------------\r\n");
                    }, TaskContinuationOptions.OnlyOnCanceled);
                    //var delay = Task.Delay(100);
                    //var rst = await Task.WhenAny(delay, run);
                    //if (rst == delay)
                    //{
                    //    tokenSource.Cancel();
                    //    var dif = new Dif(width, height, _i, _j);
                    //    skips.Add(dif);

                    //    Print(file, dif.ToString() /*+ "\r\n--------------------------\r\n"*/);
                    //}
                }
            }
        }

        private static void RunSingle(int width, int height, int _i, int _j, bool single, string file)
        {
            var content = new StringBuilder();

            var solver = new Solver2(width, height, _i , _j );
            content.Append($"棋盘({solver.W}*{solver.H}) 起点位置({solver.X},{solver.Y})");
            Stopwatch stopwatch = Stopwatch.StartNew();
            var steps = solver.GetSteps(single ? 1 : int.MaxValue);
            stopwatch.Stop();
            if (steps == null)
            {
                content.Append("\t\t无解");
            }
            else
            {
                var time = stopwatch.Elapsed;
                content.Append("\t\t共计耗时：" + time.ToString("g"));
                //content.AppendLine($"访问节点数：{solver.NodeCount}");
                //if (!steps.Any())
                //{
                //    content.AppendLine("无解。");
                //}
                //else
                //{
                //    content.AppendLine($"共{steps.Length} 组解法");
                //    for (var index = 0; index < steps.Length; index++)
                //    {
                //        content.AppendLine($"第 {index + 1} 组解法：");
                //        var step = steps[index];
                //        var w = step.GetLength(0);
                //        var h = step.GetLength(1);

                //        for (int j = 0; j < h; j++)
                //        {
                //            for (int i = 0; i < w; i++)
                //            {
                //                content.Append(step[i, j].ToString().PadLeft(3, ' ') + " ");
                //            }

                //            content.AppendLine();
                //        }

                //        break;
                //    }
                //}

            }

            //content.AppendLine("-----------------------------------------------");

            Print(file, content.ToString());
        }

        private static void RunAround(int width, int height, bool single, string file)
        {
            var tokenSource = new CancellationTokenSource(100);
            var delay = Task.Run(() => Delay());
            var run = Task.Run(() => RunAroundInner(width, height, single, file), tokenSource.Token);
            run.ContinueWith(x =>
            {
                var dif = new Dif(width, height);
                skips.Add(dif);

                Print(file, dif + "\r\n--------------------------\r\n");
            }, TaskContinuationOptions.OnlyOnCanceled);
            //var rst = await Task.WhenAny(delay, run);
            //if (rst == delay)
            //{
            //    tokenSource.Cancel();
            //    var dif = new Dif(width, height);
            //    skips.Add(dif);

            //    Print(file, dif + "\r\n--------------------------\r\n");
            //}
        }

        private static void RunAroundInner(int width, int height, bool single, string file)
        {
            if (height * width % 2 == 1)
            {
                Print(file, $"{width}-{height}棋盘无环游解。\r\n");
                Print(file, "-----------------------------------------------\r\n");
                return;
            }

            var content = new StringBuilder();
            var solver = new Solver2(width, height);
            Stopwatch stopwatch = Stopwatch.StartNew();
            content.AppendLine($"棋盘({solver.W}*{solver.H})  环游解");
            var steps = solver.GetSteps(single ? 1 : int.MaxValue, true);
            stopwatch.Stop();
            if (steps == null)
            {
                content.AppendLine("无解");
            }
            else
            {
                content.AppendLine($"访问节点数：{solver.NodeCount}");
                if (!steps.Any())
                {
                    content.AppendLine("无环游解。");
                }
                else
                {
                    if (!single)
                    {
                        content.AppendLine($"共{steps.Length} 组环游解法");
                    }

                    for (var index = 0; index < steps.Length; index++)
                    {
                        content.AppendLine($"第 {index + 1} 组环游解法：");

                        var step = steps[index];
                        var w = step.GetLength(0);
                        var h = step.GetLength(1);

                        for (int j = 0; j < h; j++)
                        {
                            for (int i = 0; i < w; i++)
                            {
                                content.Append(step[i, j].ToString().PadLeft(3, ' ') + " ");
                            }

                            content.AppendLine();
                        }

                        break;
                    }
                }

                var time = stopwatch.Elapsed;
                content.AppendLine("共计耗时：" + time.ToString("g"));
            }

            content.AppendLine("-----------------------------------------------");

            Print(file, content.ToString());
        }

        private static void Delay()
        {
            Thread.Sleep(1000);
        }

        private static void Print(string file, string content)
        {
            //lock (file)
            //{
            Console.WriteLine(content);
            //File.AppendAllText(file, content);
            //}
        }
    }
    class Dif
    {
        public Dif(int width, int height, int x, int y)
        {
            Width = width;
            Height = height;
            X = x;
            Y = y;
        }
        public Dif(int width, int height)
        {
            Width = width;
            Height = height;
            X = 0;
            Y = 0;
            Around = true;
        }
        public int Width { get; }
        public int Height { get; }
        public int X { get; }
        public int Y { get; }
        public bool Around { get; }
        public override string ToString()
        {
            if (Around)
            {
                return $"环游解：棋盘({Width}*{Height})在起点({X + 1},{Y + 1})  耗时超过 1 秒钟";
            }
            else
            {
                return $"单独解：棋盘({Width}*{Height})在起点({X + 1},{Y + 1})  耗时超过 1 秒钟";
            }
        }
    }
}
