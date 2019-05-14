using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicProgramming
{
    /// <summary>
    /// 问题描述
    /// 某幢大楼有100层。你手里有两颗一模一样的玻璃珠。当你拿着玻璃珠在某一层往下扔的时候，一定会有两个结果，玻璃珠碎了或者没碎。这幢大楼有个临界楼层。低于它的楼层，往下扔玻璃珠，玻璃珠不会碎，等于或高于它的楼层，扔下玻璃珠，玻璃珠一定会碎。玻璃珠碎了就不能再扔。现在让你设计一种方式，找到这个临界楼层，使得在该方式下，尝试的次数最少。也就是设计一种最有效的方式。
    /// 例如：有这样一种方式，第一次选择在60层扔，若碎了，说明临界点在60层及以下楼层，这时只有一颗珠子，剩下的只能是从第一层，一层一层往上实验，最坏的情况，要实验59次，加上之前的第一次，一共60次。若没碎，则只要从61层往上试即可，最多只要试40次，加上之前一共需41次。两种情况取最多的那种。故这种方式最坏的情况要试60次。仔细分析一下。如果不碎，我还有两颗珠子，第二颗珠子会从N+1层开始试吗？很显然不会，此时大楼还剩100-N层，问题就转化为100-N的问题了。
    /// </summary>
    class Floor
    {
        public static void Run(int n)
        {
            var arr = new int[n + 1];
            arr[1] = 1;
            int floorThr(int N)
            {
                for (int i = 2; i <= N; i++)
                {
                    arr[i] = i;
                    for (int j = 1; j < i; j++)
                    {
                        int tmp = Math.Max(j, 1 + arr[i - j]);    //j的遍历相当于把每层都试一遍
                        if (tmp < arr[i])
                            arr[i] = tmp;
                    }
                }
                return arr[N];
            }
            var time = floorThr(n);
            Console.WriteLine($"最多尝试 {time} 次");
        }
        public static void Run2(int n)
        {
            var sum = 0;
            var step = 0;
            for (int i = 1; i < n; i++)
            {
                sum += i;
                if (sum >= n)
                {
                    step = i;
                    break;
                }
            }
            var arr = new int[n];

            for (int i = 0, j = 1, k = 0; i + k - 2 < n; i += step, j++, step--, k = 1)
            {
                if (i + step - 1 < n)
                {
                    arr[i + step - 1] = step + j - 1;
                }

                for (k = 1; k < step && i + k - 1 < n; k++)
                {
                    arr[i + k - 1] = j + k;
                }
            }

            Console.WriteLine(arr.Max());
        }
    }


    class Bag01
    {

    }
}
