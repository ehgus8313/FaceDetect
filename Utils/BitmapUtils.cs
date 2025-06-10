using DlibDotNet;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APR_TEST.Utils
{
    public static class BitmapUtils
    {
        public static RgbPixel[] BitmapToArray(Bitmap bitmap)
        {
            var result = new RgbPixel[bitmap.Width * bitmap.Height];
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    var color = bitmap.GetPixel(x, y);
                    result[y * bitmap.Width + x] = new RgbPixel
                    {
                        Red = color.R,
                        Green = color.G,
                        Blue = color.B
                    };
                }
            }
            return result;
        }

    }
}
