using RedisCache;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace Light
{
    class Map
    {
        public Map(int width, int height)
        {
            Width = width;
            Height = height;
            this.count = this.Width * this.Height;
            this.clicks = new bool[this.count];
            this.status = new bool[this.count];
            this.links = new List<int>[this.count];

            for(int i = 0; i < this.count; i++)
            {
                var w = i % Width;
                var h = i / Width;
                this.links[i] = new List<int>();
                this.links[i].Add(i);
                if(w > 0)
                    this.links[i].Add(i - 1);
                if(h > 0)
                    this.links[i].Add(i - width);
                if(w < Width - 1)
                    this.links[i].Add(i + 1);
                if(h < Height - 1)
                    this.links[i].Add(i + Width);
            }
        }
        private int count;
        private bool[] clicks;
        private bool[] status;
        private List<int>[] links;
        private Stopwatch stopwatch;
        private long historyTimes;
        private int startPosition;
        public TimeSpan UseTime { get; private set; }

        public int Width { get; }
        public int Height { get; }
        public long NodeCount { get; set; }
        public int SolverCount { get; private set; }

        public int Solver()
        {
            this.stopwatch = Stopwatch.StartNew();

            Step(0);

            stopwatch.Stop();
            this.historyTimes = this.historyTimes + stopwatch.ElapsedTicks;
            this.UseTime = TimeSpan.FromTicks(this.historyTimes);

            return this.SolverCount;
        }

        private void Step(int pos)
        {
            if(pos >= Width)
                return;

            var w = pos % Width;
            var h = pos / Width;

            // 剪枝 前面没有再填充机会的格子如果有未点亮的不继续填充下去
            // 只有前面没有再填充机会的格子都已经点亮了才继续填充
            for(int i = 0; i < Width; i++)
            {
                for(int j = 0; j < (i < w - 1 ? h : h - 1); j++)
                {
                    if(!this.status[i + j * Width])
                    {
                        return;
                    }
                }
            }
            bool[] _cc = new bool[this.count];
            var minStep = -1;
            // 第一行的所有点击情况数，每种状态用数字i的二进制形式表示，每一位1或0表示点击
            var hasMirror = false;
            var list = new HashSet<int>(); // 主要用于判断某元素是否存在，Hash速度比List更快
            var len = Math.Pow(2, this.Width);
            for(int i = 0; i < len; i++)
            {
                if(list.Contains(i))
                {
                    continue;
                }
                hasMirror = false;
                var rev = GetRevereValue(i, this.Width);
                if(rev != i)
                {
                    list.Add(rev);
                    hasMirror = true;
                }

                this.NodeCount++;
                var cc = new bool[this.count];
                var ss = new bool[this.count];
                int k = 0;
                // 通过第一行的点击初始化状态
                // 以宽度3为例，8种点击状态，000,001,010,011,100,101,110,111
                // 这里可以继续剪枝，非对称状态只需要镜像状态可忽略，比如001和100,011和110,8种状态只需要验证6种
                // 当规模继续扩大，剪枝比例接近于一半，能在有限范围内降低复杂度，时间复杂度降低一半
                for(k = 0; k < this.Width; k++)
                {
                    if((i & (1 << k)) == (1 << k))
                    {
                        cc[k] = !cc[k];
                        foreach(var link in this.links[k])
                        {
                            ss[link] = !ss[link];
                        }
                    }
                }

                //Console.WriteLine(string.Join(" ", cc.Take(this.Width).Select(x => x ? "*" : "_")));
                // 下面每一行的点击，都以让上一行全部变亮为规则，逐行点击
                for(int r = 1, ro = 0; r < this.Height; r++, ro++)
                {
                    for(int c = 0; c < this.Width; c++)
                    {
                        if(!ss[c + ro * this.Width])
                        {
                            k = c + r * this.Width;
                            cc[k] = !cc[k];
                            foreach(var link in this.links[k])
                            {
                                ss[link] = !ss[link];
                            }
                        }
                    }
                }
                // 按照上述规则，执行到最后一行，前面的每一行都会成功切换状态，验证最后一行是否全部切换即可
                if(ss.All(x => x))
                {
                    //计算所有解的数量，如果当前方案存在镜像，多加1
                    SolverCount += (hasMirror ? 2 : 1);
                    var st = cc.Count(x => x);
                    if(minStep == -1 || minStep > st)
                    {
                        minStep = st;
                        _cc = cc.ToArray();
                    }
                }
            }
            if(minStep > 0)
            {
                Program.Print2(this.Width, this.Height, SolverCount, _cc);
            }
        }
        private int GetRevereValue(int n, int len)
        {
            int res = 0;
            for(int i = 0; i < len; ++i)
            {
                res = (res << 1) + (n & 1);
                n >>= 1;
            }
            return res;
        }

        private int GetRevereValue(int number)
        {
            // 数字转二进制字符串
            string binaryString = Convert.ToString(number, 2);
            Console.WriteLine("原始二进制字符串: " + binaryString);

            // 翻转二进制字符串
            char[] charArray = binaryString.ToCharArray();
            Array.Reverse(charArray);
            string reversedBinaryString = new string(charArray);
            Console.WriteLine("翻转后的二进制字符串: " + reversedBinaryString);

            // 二进制字符串转数字
            int reversedNumber = Convert.ToInt32(reversedBinaryString, 2);
            return reversedNumber;
        }
    }
}
