using RedisCache;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace Light
{
    class Map2
    {
        public Map2(int width, int height)
        {
            Width = width;
            Height = height;
            this.count = this.Width * this.Height;
            this.clicks = new bool[this.count];
            this.status = new bool[this.count];
            this.links = new List<int>[this.count];
            this.visits = new BigInteger[this.count];

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

            this.ReadRedis();
        }
        private int count;
        private bool[] clicks;
        private bool[] status;
        private List<int>[] links;
        private BigInteger[] visits;
        private Stopwatch stopwatch;
        private long historyTimes;
        private int startPosition;
        public TimeSpan UseTime { get; private set; }

        public int Width { get; }
        public int Height { get; }
        public long NodeCount { get; set; }
        public int SolverCount { get; private set; }

        public void On(int p)
        {
            this.clicks[p] = !this.clicks[p];
            foreach(var link in this.links[p])
            {
                this.status[link] = !this.status[link];
                this.count += this.status[link] ? -1 : 1;
            }
            if(count < 0 || count > this.Width * this.Height)
            {
                Console.WriteLine($"出错了，count值：{count}");
                throw new Exception($"出错了，count值：{count}");
            }
        }

        public bool IsSuccess()
        {
            return this.count == 0;
        }

        public bool[] ToStep()
        {
            return (bool[])this.clicks.Clone();
        }
        public int Solver()
        {
            this.stopwatch = Stopwatch.StartNew();

            while(this.startPosition >= 0)
            {
                var v = this.visits[this.startPosition];
                if(this.startPosition > 0 && v < this.visits[this.startPosition - 1] * 2 || this.startPosition == 0 && v < 2)
                {
                    var even = v % 2 > 0;
                    Step(this.startPosition, even);
                }
                this.startPosition--;
            }

            stopwatch.Stop();
            this.historyTimes = this.historyTimes + stopwatch.ElapsedTicks;
            this.UseTime = TimeSpan.FromTicks(this.historyTimes);
            this.ClearRedis();

            return this.SolverCount;
        }

        private void Step(int pos, bool mid = false)
        {
            if(pos >= Width * Height)
                return;

            this.WriteToRedis(pos);

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
                        for(int v = pos; v < this.visits.Length; v++)
                        {
                            this.visits[v] = this.visits[v - 1] * 2;
                        }
                        this.WriteToRedis(pos);
                        return;
                    }
                }
            }
            var sdir = mid ? 1 : 0;
            for(int dir = sdir; dir < 2; dir++)
            {
                this.NodeCount++;
                this.visits[pos]++;
                this.On(pos);
                if(this.IsSuccess())
                {
                    SolverCount++;
                    var steps = ToStep();
                    Program.Print(this.Width, this.Height, SolverCount, steps);
                    for(int v = pos; v < this.visits.Length; v++)
                    {
                        this.visits[v] = this.visits[v - 1] * 2;
                    }
                    this.WriteToRedis(pos);
                }
                else
                {
                    this.Step(pos + 1);
                }
            }
        }

        private readonly RedisClient redis = null;// new RedisClient("127.0.0.1:6379,password=morelongmoregood");
        private const string redisMapKey = "light_{0}_{1}";
        private void WriteToRedis(int position)
        {
            var data = new Data
            {
                Position = position,
                Clicks = this.clicks,
                Visits = this.visits,
                Count = this.count,
                HistoryTimes = this.stopwatch.ElapsedTicks + historyTimes,
                NodeCount = this.NodeCount,
                SolverCount = this.SolverCount
            };
            redis?.Set(string.Format(redisMapKey, this.Width, this.Height), data);
        }

        private void ClearRedis()
        {
            redis?.Delete(string.Format(redisMapKey, this.Width, this.Height));
        }

        private bool ReadRedis()
        {
            var data = redis?.GetOrDefault<Data>(string.Format(redisMapKey, this.Width, this.Height));
            if(data == null)
            {
                return false;
            }
            Console.WriteLine($"读取缓存 count ：{data.Count}");
            var clicks = data.Clicks;
            for(int i = 0; i < clicks.Length; i++)
            {
                if(clicks[i])
                {
                    this.On(i);
                }
            }
            if(data.Count != this.count)
            {
                Console.WriteLine("缓存数据不完整，清空缓存");
                //this.ClearRedis();
                this.count = this.Width * this.Height;
                this.clicks = new bool[this.count];
                this.status = new bool[this.count];
                return false;
            }
            this.visits = data.Visits;
            this.startPosition = data.Position;
            this.NodeCount = data.NodeCount;
            this.SolverCount = data.SolverCount;
            this.historyTimes = data.HistoryTimes;
            Console.WriteLine("从缓存数据中恢复进程：");
            Console.WriteLine($"已遍历节点：{this.NodeCount}，已查找解法：{this.SolverCount}，已用时间：{TimeSpan.FromTicks(this.historyTimes)}");
            return true;
        }

        class Data
        {
            public int Position { get; set; }
            public long NodeCount { get; set; }
            public int SolverCount { get; set; }
            public int Count { get; set; }
            public bool[] Clicks { get; set; }
            public BigInteger[] Visits { get; set; }
            public long HistoryTimes { get; set; }
        }
    }
}
