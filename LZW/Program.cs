using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;

namespace LZW
{
    class Program
    {
        static void Main(string[] args)
        {
            //Test(2150900);


            TestFile("zhuixuOld.txt");

            Console.Read();
        }

        private static void TestFile(string fn)
        {
            var buffer = File.ReadAllBytes(fn);

            Console.WriteLine($"源数据，长度：[{buffer.Length}]");

            var rst = Encode(buffer);

            File.WriteAllBytes($"{fn}.lzw", rst);

            Console.WriteLine($"压缩结果，长度：[{rst.Length}]");

            var decode = Decode(rst);

            Console.WriteLine($"解压结果，长度：[{decode.Length}]");

            File.WriteAllBytes($"lzw_{fn}", decode);

            if(buffer.Length != decode.Length)
            {
                throw new Exception($"数据长度不同");
            }

            for(int i = 0; i < buffer.Length; i++)
            {
                if(buffer[i] != decode[i])
                {
                    throw new Exception($"第{i}个元素不同");
                }
            }

            Console.WriteLine("压缩还原正常");
        }

        private static void Test(int length)
        {
            var buffer = new byte[length];

            var rd = new Random(DateTime.Now.Millisecond);

            rd.NextBytes(buffer);

            Console.WriteLine($"源数据，长度：[{buffer.Length}]");
            Print(buffer);

            var rst = Encode(buffer);

            Console.WriteLine($"压缩结果，长度：[{rst.Length}]");
            Print(rst);

            var decode = Decode(rst);

            Console.WriteLine($"解压结果，长度：[{decode.Length}]");
            Print(decode);

            if(buffer.Length != decode.Length)
            {
                Console.WriteLine($"数据长度不同");
            }

            for(int i = 0; i < Math.Min(decode.Length, buffer.Length); i++)
            {
                if(buffer[i] != decode[i])
                {
                    Console.WriteLine($"第{i}个元素不同");
                    return;
                }
            }

            Console.WriteLine("压缩还原正常");
        }

        private static void Print(byte[] bs)
        {
            //foreach(var v in bs)
            //{
            //    Console.Write(v + " ");
            //}
            //Console.WriteLine();
        }
        private static void PrintDic(IDictionary dic)
        {
            //Console.WriteLine($"字典长度：{dic.Count}");
            //var i = 1;
            //foreach(var v in dic.Keys)
            //{
            //    Console.Write($"{v} = {dic[v]} ;");
            //    if((i++) % 10 == 0)
            //    {
            //        Console.WriteLine();
            //    }
            //}
            //Console.WriteLine();
        }

        private static string HEX2STR(byte v)
        {
            return v.ToString("X").PadLeft(2, '0');
        }

        private static byte STR2HEX(string s)
        {
            return Convert.ToByte(s, 16);
        }

        private static byte[] SHORT2BYTES(ushort v)
        {
            return new byte[] { (byte)(v >> 8), (byte)(v & 0xFF) };
        }
        private static ushort BYTES2SHORT(byte h, byte l)
        {
            return (ushort)(h << 8 | l);
        }

        private static readonly ushort KEY_MAX = 256;

        public static byte[] Encode(byte[] data)
        {
            var result = new List<byte>();
            ushort idleCode = KEY_MAX;
            var dic = new Dictionary<string, ushort>();
            for(byte i = 0; dic.Count < idleCode; i++)
            {
                dic.Add(HEX2STR(i), i);
            }
            string prefix = "";// 词组前缀
            string suffix;// 词组后缀

            for(int k = 0; k < data.Length; k++)
            {
                var c = data[k];
                suffix = prefix + HEX2STR(c);
                if(dic.ContainsKey(suffix))
                {
                    prefix = suffix;
                }
                else
                {
                    idleCode++;
                    dic.Add(suffix, idleCode);
                    result.AddRange(SHORT2BYTES(dic[prefix]));
                    prefix = "" + HEX2STR(c);
                }
                if(k == data.Length - 1)
                {
                    // 最后一次输出
                    if(prefix != "")
                    {
                        result.AddRange(SHORT2BYTES(dic[prefix]));
                    }
                }
                if(dic.Count >= ushort.MaxValue)
                {
                    PrintDic(dic);
                    dic.Clear();
                    for(byte i = 0; dic.Count < KEY_MAX; i++)
                    {
                        dic.Add(HEX2STR(i), i);
                    }
                    idleCode = KEY_MAX;
                    if(prefix != "")
                    {
                        result.AddRange(SHORT2BYTES(dic[prefix]));
                    }
                    suffix = prefix = "";
                }
            }

            PrintDic(dic);

            return result.ToArray();
        }

        public static byte[] Decode(byte[] data)
        {
            var result = new List<byte>();
            ushort idleCode = KEY_MAX;
            var dic = new Dictionary<ushort, string>();
            for(byte i = 0; dic.Count < idleCode; i++)
            {
                dic.Add(i, HEX2STR(i));
            }

            string p = "";
            string c = "";
            for(int i = 0; i < data.Length; i += 2)
            {
                var v = BYTES2SHORT(data[i], data[i + 1]);
                if(dic.ContainsKey(v))
                {
                    c = dic[v];
                }
                else
                {
                    c = c + c.Substring(0, 2);
                }

                if(p != "")
                {
                    idleCode++;
                    dic.Add(idleCode, p + c.Substring(0, 2));
                }

                for(int l = 0; l < c.Length; l += 2)
                {
                    result.Add(STR2HEX(c.Substring(l, 2)));
                }
                p = c;
                if(dic.Count >= ushort.MaxValue)
                {
                    PrintDic(dic);
                    dic.Clear();
                    for(byte j = 0; dic.Count < KEY_MAX; j++)
                    {
                        dic.Add(j, HEX2STR(j));
                    }
                    idleCode = KEY_MAX;
                    p = "";
                    c = "";
                }
            }
            PrintDic(dic);
            return result.ToArray();
        }
    }
}
