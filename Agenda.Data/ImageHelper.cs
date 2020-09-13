using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace Agenda.Data
{
    public static class ImageHelper
    {
        public static void MakeThumbnail(Stream input, MemoryStream output)
        {
            input.Seek(0, SeekOrigin.Begin);
            Bitmap image = new Bitmap(input);

            Bitmap newimage = new Bitmap(image, 100, 100);

            newimage.Save(output, ImageFormat.Png);

            output.Seek(0, SeekOrigin.Begin);

        }
    }
}
