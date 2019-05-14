using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace EgyptianFractions
{
    /// <summary>
    /// 埃及分数
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            RunEgyptianFractions(40);
            Console.Read();
        }

        /// <summary>
        /// 埃及分数
        /// </summary>
        /// <param name="max"></param>
        private static void RunEgyptianFractions(int max)
        {
            for (int i = 3; i <= max; i++)
            {
                for (int j = 2; j < i; j++)
                {
                    try
                    {
                        if (IsCoPrime(j, i))
                        {
                            Run(j, i);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        static void Run(int a, int b)
        {
            var list = new List<long>();
            long da = a;
            long db = b;
            while (da != 1)
            {
                var q = db / da;
                var r = db % da;
                list.Add(q + 1);
                da = da - r;
                db = db * (q + 1);
                if (db % da == 0)
                {
                    db = db / da;
                    da = 1;
                }
            }
            list.Add(db);

            Console.Write($"{a}/{b} = ");
            foreach (var i in list)
            {
                Console.Write($"1/{i} + ");
            }

            Console.CursorLeft -= 2;
            Console.WriteLine("  ");
        }

        static bool IsCoPrime(int i, int j)
        {
            if (j % i == 0)
                return false;
            for (int k = 2; k <= Math.Sqrt(j) && k < i; k++)
            {
                if (j % k == 0 && i % k == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
