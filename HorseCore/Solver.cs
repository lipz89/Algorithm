using System;
using System.Collections.Generic;

namespace HorseCore
{
    class Solver
    {
        private readonly int[,] map;

        private readonly int[][] moves =
        {
            new[] {1, 2}, new[] {2, 1},
            new[] {-1, 2}, new[] {-2, 1},
            new[] {1, -2}, new[] {2, -1},
            new[] {-1, -2}, new[] {-2, -1}
        };

        public Solver(int width, int height, int _x = 1, int _y = 1)
        {
            if (width < 3 || height < 3)
            {
                throw new Exception("棋盘太小了！");
            }

            if (_x < 1 || _y < 1 || _x > width || _y > height)
            {
                throw new Exception("指定起点不在棋盘内！");
            }

            this.W = width;
            this.H = height;
            this.x = _x - 1;
            this.y = _y - 1;
            this.map = new int[W, H];
        }

        private List<int[,]> steps;

        public int NodeCount { get; private set; }

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

            steps = new List<int[,]>();
            map[x, y] = 1;
            Step(x, y, count, 2, onlyArount);
            return steps.ToArray();
        }

        private void Step(int x, int y, int count, int step, bool onlyArount)
        {
            for (int index = 0; index < moves.Length; index++)
            {
                var move = moves[index];

                var _x = x + move[0];
                var _y = y + move[1];
                if (_x < 0 || _y < 0 || _x >= W || _y >= H)
                {
                    continue;
                }

                if (this.map[_x, _y] != 0)
                {
                    continue;
                }

                this.NodeCount++;

                this.map[_x, _y] = step;
                if (step == W * H)
                {
                    var gotAnswer = true;
                    if (onlyArount)
                    {
                        gotAnswer = false;
                        var dx = Math.Abs(_x - x);
                        var dy = Math.Abs(_y - y);
                        if (dx + dy == 3 && Math.Abs(dx - dy) == 1)
                        {
                            gotAnswer = true;
                        }
                    }

                    if (gotAnswer)
                    {
                        var answer = new int[W, H];
                        for (int i = 0; i < W; i++)
                        {
                            for (int j = 0; j < H; j++)
                            {
                                answer[i, j] = this.map[i, j];
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
                    this.Step(_x, _y, count, step + 1, onlyArount);
                    if (steps.Count >= count)
                    {
                        return;
                    }
                }

                this.map[_x, _y] = 0;
            }
        }

        public override string ToString()
        {
            var str = string.Empty;
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    str += map[i, j].ToString().PadLeft(2, ' ') + " ";
                }

                str += Environment.NewLine;
            }

            return str;
        }
    }
}