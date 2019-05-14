using System;
using System.Collections.Generic;

namespace IntegerDivision
{
    /// <summary>
    /// 整数划分
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            //Test(2);
            Test(20);
            Console.Read();
        }

        static void Test(int i)
        {
            var count = Solver.Count(i);
            var rst = Solver.Run(i);
            Console.WriteLine($"共 {rst.Count} = {count} 种分法：");
            var index = 1;

            foreach (var lst in rst)
            {
                Console.Write($"第 {index++} 种：\t");
                var str = string.Join(",", lst);
                Console.WriteLine(str);
            }

            Console.WriteLine("输出完毕");

            //var rst2 = Solver.Run2(i);
        }
    }

    class Solver
    {
        public static List<List<int>> Run(int target)
        {
            return Run(target, 1);
        }

        private static List<List<int>> Run(int target, int min)
        {
            var rst = new List<List<int>>();
            for (int i = min; i <= target / 2; i++)
            {
                var rsts = GetResult(target - i, i);
                rst.AddRange(rsts);
            }
            rst.Add(new List<int> { target });
            return rst;//.OrderByDescending(x => x.First()).ToList();
        }

        private static List<List<int>> GetResult(int target, int min)
        {
            var rst = new List<List<int>>();
            if (target >= min)
            {
                var rst2 = Run(target, min);
                rst2.ForEach(x => x.Add(min));
                rst.AddRange(rst2);
            }

            return rst;
        }

        public static int Count(int target)
        {
            return Count(target, target);
        }
        private static int Count(int n, int m)
        {
            if (n == 1 || m == 1)
                return 1;
            if (n <= m)
                return 1 + Count(n, n - 1);

            return Count(n, m - 1) + Count(n - m, m);
        }

        public static List<List<int>> Run2(int target)
        {
            return Run2(target, target);
        }
        private static List<List<int>> Run2(int n, int m)
        {
            if (n == 1)
            {
                return new List<List<int>> { new List<int> { 1 } };
            }
            if (m == 1)
            {
                var lst = new List<int>();
                for (int i = 0; i < n; i++)
                {
                    lst.Add(1);
                }
                return new List<List<int>> { lst };
            }
            if (n <= m)
            {
                var rst = Run2(n, n - 1);
                rst.Add(new List<int> { n });
                return rst;
            }
            else
            {
                var rst2 = Run2(n - m, m);
                rst2.ForEach(x => x.Add(m));
                var rst = Run2(n, m - 1);
                rst.AddRange(rst2);
                return rst;
            }
        }
    }
}
