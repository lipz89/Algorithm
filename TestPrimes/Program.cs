using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TestPrimes
{
    class Program
    {
        static void Main(string[] args)
        {
            var max = 1000;
            Console.WriteLine($"取 {max} 之内的素数：");
            RunPrime(GetPrimes, max);
            RunPrime(GetPrimes2, max);
            RunPrime(TestLinq, max);
            RunPrime(TestLinq2, max);

            Console.Read();
        }

        private static void RunPrime(GetPrimesDelegate getPrimesDelegate, int max)
        {
            Console.WriteLine("素数算法：" + getPrimesDelegate.Method.Name);
            Stopwatch stopwatch = Stopwatch.StartNew();
            var list = getPrimesDelegate(max, out int count);
            stopwatch.Stop();
            var time = stopwatch.Elapsed;
            Console.WriteLine("共计耗时：" + time.ToString("g"));
            Console.WriteLine("求余次数：" + count);
            Console.WriteLine("素数个数：" + list.Count);
            //Console.WriteLine(string.Join(",", list));
            Console.WriteLine(" ");
        }

        delegate List<int> GetPrimesDelegate(int max, out int count);


        static List<int> GetPrimes(int max, out int count)
        {
            count = 0;
            var list = new List<int> { 2 };
            for (int i = 3; i <= max; i++)
            {
                var isp = true;
                var si = Math.Sqrt(i);
                for (int j = 0; j < list.Count; j++)
                {
                    var item = list[j];
                    if (item > si)
                        break;
                    count++;
                    if (i % item == 0)
                    {
                        isp = false;
                        break;
                    }
                }

                if (isp)
                {
                    list.Add(i);
                }
            }
            return list;
        }
        static List<int> GetPrimes2(int max, out int count)
        {
            count = 0;
            var list = new List<int> { 2 };
            for (int i = 3; i <= max; i++)
            {
                var isp = true;
                var si = Math.Sqrt(i);
                for (int j = 2; j <= si; j++)
                {
                    count++;
                    if (i % j == 0)
                    {
                        isp = false;
                        break;
                    }
                }

                if (isp)
                {
                    list.Add(i);
                }
            }
            return list;
        }
        private static List<int> TestLinq(int max, out int count)
        {
            var enumer = Enumerable.Range(2, max - 2);
            var list = new List<int>();

            var pcount = 0;
            int temp = 2;
            while ((temp = enumer.FirstOrDefault()) > 0)
            {
                int n = temp;
                list.Add(n);
                enumer = enumer.Skip(1).Where(x =>
                {
                    pcount++;
                    return x % n != 0;
                });
            }

            count = pcount;
            return list;
        }
        private static List<int> TestLinq2(int max, out int count)
        {
            var enumer = Enumerable.Range(2, max - 2);
            var list = new List<int>();

            var pcount = 0;
            while (enumer.Any())
            {
                int n = enumer.First();
                list.Add(n);
                enumer = enumer.Skip(1).Where(x =>
                {
                    pcount++;
                    return x % n != 0;
                }).ToList();
            }

            count = pcount;
            return list;
        }
    }
}
