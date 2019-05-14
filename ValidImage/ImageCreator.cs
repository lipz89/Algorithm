using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace ValidImage
{
    class ImageCreator
    {
        private static readonly Dictionary<Color, string> colors = new Dictionary<Color, string>
        {
            { Color.Aqua,"青色"},
            { Color.Blue,"蓝色"},
            { Color.Red ,"红色"},
            { Color.Yellow ,"黄色"},
            { Color.Green ,"绿色"},
            { Color.Purple ,"紫色"},
            { Color.Black ,"黑色"}
        };
        private static readonly List<string> FontFamilies = new List<string>
        {
            "arial","axure","calibri","fixedsys","chiller","cooper","verdana","stencil","modern","sitka","latin","impact","broadway","britannic","colonna mt","lnk free"
        };

        private const string LETTERS = "abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ";
        private const string DIGITALS = "0123456789";

        public static ImageInfo Create(Options options)
        {
            var width = 200;
            var height = 200;
            var bitmap = new Bitmap(width, height);
            var families = FontFamily.Families.Where(x => FontFamilies.Any(f => x.Name.StartsWith(f, StringComparison.OrdinalIgnoreCase))).ToList();

            var gp = Graphics.FromImage(bitmap);
            gp.Clear(Color.White);
            var rd = new Random(DateTime.Now.Millisecond);

            var rcolors = colors.Keys.ToList();

            var colorIndex = rd.Next(colors.Count);
            var color = rcolors[colorIndex];
            var strings = string.Empty;
            switch (options.WordType)
            {
                case WordType.Digitals:
                    strings = DIGITALS;
                    break;
                case WordType.Letters:
                    strings = LETTERS;
                    break;
                default:
                    strings = LETTERS + DIGITALS + DIGITALS + DIGITALS;
                    break;
            }

            var charWidth = 20;
            var charHeight = 20;

            var code = string.Empty;
            for (int i = 0; i < options.WordLength; i++)
            {
                var index = rd.Next(strings.Length);
                var ch = strings[index];
                code += ch;
            }

            for (int i = 0; i < options.InterferenceCount; i++)
            {
                var index = rd.Next(strings.Length);
                var ch = strings[index].ToString();
                var left = rd.Next(width - charWidth);
                var top = rd.Next(height - charHeight - 15);
                var icolorIndex = rd.Next(colors.Count - 1);
                if (icolorIndex >= colorIndex)
                {
                    icolorIndex += 1;
                }
                var icolor = rcolors[icolorIndex];
                var fontfml = families[rd.Next(families.Count)];
                var fontsize = rd.Next(4) + 18;
                var font = new Font(fontfml, fontsize);
                var angle = rd.Next(100) - 50;

                Matrix matrix = gp.Transform;
                matrix.RotateAt(angle, new PointF(left, top));
                gp.Transform = matrix;

                gp.DrawString(ch, font, new SolidBrush(icolor), left, top);

                matrix = gp.Transform;
                matrix.RotateAt(-angle, new PointF(left, top));
                gp.Transform = matrix;
            }

            var seeds = new int[options.WordLength];
            var points = new PointF[options.WordLength];
            seeds[0] = 10;
            var step = (width - charWidth * 2) / options.WordLength;
            var label = string.Empty;
            for (int i = 0; i < options.WordLength; i++)
            {
                switch (options.Direction)
                {
                    case Direction.Down:
                        label = "从上到下";
                        points[i] = new PointF(rd.Next(width - charWidth), seeds[0] + step * i);
                        break;
                    case Direction.Up:
                        label = "从下到上";
                        points[i] = new PointF(rd.Next(width - charWidth), height - (seeds[0] + step * i) - charHeight - 15);
                        break;
                    case Direction.Right:
                        label = "从左向右";
                        points[i] = new PointF(seeds[0] + step * i, rd.Next(height - charHeight - 15));
                        break;
                    default:
                        label = "从右向左";
                        points[i] = new PointF(width - (seeds[0] + step * i) - charWidth, rd.Next(height - charHeight - 15));
                        break;
                }
            }

            for (int i = 0; i < options.WordLength; i++)
            {
                var ch = code[i].ToString();
                var fontfml = families[rd.Next(families.Count)];
                var fontsize = rd.Next(4) + 16;
                var font = new Font(fontfml, fontsize, FontStyle.Bold);
                var angle = rd.Next(100) - 50;
                Matrix matrix = gp.Transform;
                matrix.RotateAt(angle, points[i]);
                gp.Transform = matrix;

                gp.DrawString(ch, font, new SolidBrush(color), points[i]);

                matrix = gp.Transform;
                matrix.RotateAt(-angle, points[i]);
                gp.Transform = matrix;
            }

            gp.DrawString(label + "的" + colors[color] + "字符", new Font(FontFamily.GenericMonospace, 14, FontStyle.Bold), new SolidBrush(color), 3, height - 18);

            label += "依次填写" + colors[color] + "的字符为验证码";

            return new ImageInfo
            {
                Image = bitmap,
                Code = code,
                Description = label
            };
        }
    }

    class ImageInfo
    {
        public Image Image { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }

    class Options
    {
        private int wordLength = 6;
        private int interferenceCount = 15;
        public ValidType ValidType { get; set; }
        public WordType WordType { get; set; }
        public Direction Direction { get; set; }

        public int WordLength
        {
            get => wordLength;
            set => wordLength = value < 4 ? 4 : value > 8 ? 8 : value;
        }

        public int InterferenceCount
        {
            get => interferenceCount;
            set => interferenceCount = value < 10 ? 10 : value > 20 ? 20 : value;
        }
    }

    enum ValidType
    {
        PickColorWords,
    }

    enum WordType
    {
        DigitalsAndLetters, Digitals, Letters
    }

    enum Direction
    {
        Left, Right, Up, Down
    }
}
