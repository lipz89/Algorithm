using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidImage
{
    class Program
    {
        static void Main(string[] args)
        {
            Random rd = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < 1000; i++)
            {
                Create(rd);
            }

            Console.Read();
        }

        private static void Create(Random rd)
        {
            var options = new Options();
            options.WordLength = rd.Next(4) + 4;
            options.InterferenceCount = rd.Next(10) + 10;
            options.Direction = (Direction)rd.Next(4);
            options.WordType = (WordType)rd.Next(3);

            var image = ImageCreator.Create(options);
            image.Image.Save($"tmp_{image.Code}.jpg");
        }
    }
}
