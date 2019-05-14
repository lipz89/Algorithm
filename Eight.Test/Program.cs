using System;
using System.Diagnostics;
using System.Linq;
using Eight.Core;

namespace Eight.Test
{
    class Program
    {
        static void Main()
        {
            Start();
            //GetRev("187206345", "278354106");
        }

        private static void GetRev(params string[] n)
        {
            foreach (var s in n)
            {
                Console.WriteLine(s);
                var count = 0;
                for (int i = 0; i < s.Length - 1; i++)
                {
                    for (int j = i + 1; j < s.Length; j++)
                    {
                        if (s[i] > s[j])
                        {
                            count++;
                        }
                    }
                }
                Console.WriteLine("逆序数：" + count);
            }

            Console.Read();
        }

        private static void Start()
        {
            Console.Title = "八字码游戏";
            Logic mapLogic = null;
            string lastFinish, lastStart;
            while (true)
            {
            NewGame:
                Console.Clear();
                Console.WriteLine("方向键操作，F3自定义游戏，F4重置当前游戏，ESC退出");
                var logic = mapLogic ?? new Logic();
                Console.WriteLine("目标状态：");
                Print(logic.State.ToCharArray());
                Console.WriteLine("当前状态：");
                Print(logic.Value);
                lastFinish = logic.State;
                lastStart = new string(logic.Value);

                var step = 0;
                while (true)
                {
                    var cmd = GetValue();
                    if (cmd.Type == CmdType.Normal)
                    {
                        if (logic.Move(cmd.Direction))
                        {
                            step++;
                            Console.CursorTop -= 3;
                            Console.CursorLeft = 0;
                            Print(logic.Value);

                            if (logic.IsFinished())
                            {
                                Console.WriteLine($"完成游戏，使用步骤数：{step}");
                                break;
                            }
                        }
                    }
                    else if (cmd.Type == CmdType.NewMap)
                    {
                        mapLogic = new Logic(cmd.Finish, cmd.Start);
                        goto NewGame;
                    }
                    else if (cmd.Type == CmdType.NewGame)
                    {
                        mapLogic = new Logic(lastFinish, lastStart);
                        goto NewGame;
                    }
                    else if (cmd.Type == CmdType.ReInit)
                    {
                        mapLogic = logic;
                        goto NewGame;
                    }
                    else if (cmd.Type == CmdType.Exit)
                    {
                        goto Exit;
                    }
                    else if (cmd.Type == CmdType.Auto)
                    {
                        if (Auto(logic)) ;
                        break;
                    }
                }

                Console.Write("F3自定义游戏，F4重置当前游戏，其他任意键新游戏，ESC退出:");
                var cmd2 = GetValue();
                if (cmd2.Type == CmdType.Exit)
                {
                    goto Exit;
                }
                if (cmd2.Type == CmdType.NewMap)
                {
                    mapLogic = new Logic(cmd2.Finish, cmd2.Start);
                    goto NewGame;
                }
                if (cmd2.Type == CmdType.ReInit)
                {
                    mapLogic = new Logic(lastFinish, lastStart);
                    goto NewGame;
                }
                mapLogic = null;
            }
        Exit:;
        }

        private static bool Auto(Logic logic)
        {
            Console.CursorLeft = 0;
            Console.WriteLine("自动求解中，请稍候。。。");
            var auto = new SolverOne(logic);
            var st = Stopwatch.StartNew();
            var hasSolver = auto.Run();
            var time = st.ElapsedMilliseconds;

            Console.WriteLine("自动求解完成。");
            var queue = auto.GetQueueHistory();
            var stateTotal = auto.GetStateTotal();
            Console.WriteLine($"队列轨迹：{string.Join(",", queue)}");
            Console.WriteLine($"队列峰值：{queue.Max()}；搜索深度：{queue.Count}；搜索总状态数：{stateTotal}； 耗时：{time} 毫秒");

            if (!hasSolver)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("该盘无解！");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                var steps = auto.GetSteps();
                Console.WriteLine($"步骤数：{steps.Count}");
                Console.WriteLine(string.Join(",", steps));
            }
            Console.ForegroundColor = ConsoleColor.White;
            return hasSolver;
        }

        private static Command GetValue()
        {
            while (true)
            {
                var dir = Console.ReadKey().Key;
                Console.CursorLeft = 0;
                switch (dir)
                {
                    case ConsoleKey.UpArrow:
                        return new Command { Type = CmdType.Normal, Direction = Direction.Up };
                    case ConsoleKey.LeftArrow:
                        return new Command { Type = CmdType.Normal, Direction = Direction.Left };
                    case ConsoleKey.RightArrow:
                        return new Command { Type = CmdType.Normal, Direction = Direction.Right };
                    case ConsoleKey.DownArrow:
                        return new Command { Type = CmdType.Normal, Direction = Direction.Down };
                    case ConsoleKey.Escape:
                        return new Command { Type = CmdType.Exit };
                    case ConsoleKey.F1:
                        return new Command { Type = CmdType.Auto };
                    case ConsoleKey.F2:
                        return new Command { Type = CmdType.NewGame };
                    case ConsoleKey.F4:
                        return new Command { Type = CmdType.ReInit };
                    case ConsoleKey.F3:
                        Console.Clear();
                        if (GetMapPair(out string finish, out string start))
                        {
                            return new Command { Type = CmdType.NewMap, Start = start, Finish = finish };
                        }
                        else
                        {
                            return new Command { Type = CmdType.NewGame };
                        }
                    default: continue;
                }
            }
        }

        private static bool GetMapPair(out string finish, out string start)
        {
            if (GetMap("目标状态", out finish) && GetMap("初始状态", out start))
            {
                return true;
            }

            finish = null;
            start = null;
            return false;
        }

        private static bool GetMap(string info, out string input)
        {
            while (true)
            {
                Console.Write("请输入" + info + ":");
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    return false;
                }

                if (!IsCheck(input))
                {
                    Console.Write("格式不正确，");
                }
                else
                {
                    break;
                }
            }
            return true;
        }

        private static bool IsCheck(string map)
        {
            if (!map.Contains('0'))
                map = map.Replace(' ', '0');
            if (map.Length != 9)
                return false;
            map = new string(map.ToCharArray().OrderBy(x => x).ToArray());
            return map == "012345678";
        }

        private static void Print(char[] state)
        {
            for (int i = 0; i < state.Length; i++)
            {
                if (state[i] == '0')
                    Console.Write("   ");
                else
                    Console.Write(state[i] + "  ");
                if (i % 3 == 2)
                {
                    Console.WriteLine();
                }
            }
        }


        class Command
        {
            public CmdType Type { get; set; }
            public Direction Direction { get; set; }
            public string Start { get; set; }
            public string Finish { get; set; }
        }

        enum CmdType
        {
            Normal,
            NewMap,
            NewGame,
            ReInit,
            Auto,
            Exit,
        }
    }
}
