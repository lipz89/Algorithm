using System.Drawing;
using System.Drawing.Imaging;

namespace Light
{
    static class ImageCreator
    {
        private const int LENGTH = 20;
        private const int STAR_LEN = 4;
        private const int OFFSET = (LENGTH - STAR_LEN) / 2;

        private static Pen star = new(Color.Black, 2);

        public static void PrintImage(int w, int h, int index, bool[] step)
        {
            var iw = w * LENGTH + 1;
            var ih = h * LENGTH + 1;
            var image = new Bitmap(iw, ih);
            var gp = Graphics.FromImage(image);
            gp.Clear(Color.LawnGreen);
            for(int i = 0; i <= w; i++)
            {
                gp.DrawLine(Pens.Black, LENGTH * i, 0, LENGTH * i, ih);
            }
            for(int i = 0; i <= h; i++)
            {
                gp.DrawLine(Pens.Black, 0, LENGTH * i, iw, LENGTH * i);
            }

            for(int i = 0; i < w; i++)
            {
                for(int j = 0; j < h; j++)
                {
                    if(step[i + j * w])
                    {
                        var x = i * LENGTH + OFFSET;
                        var y = j * LENGTH + OFFSET;
                        gp.DrawEllipse(star, x, y, STAR_LEN, STAR_LEN);
                    }
                }
            }
            gp.Dispose();
            image.Save($"../../solvers/{h}-{w}-{index}.png", ImageFormat.Png);
        }
    }
}
