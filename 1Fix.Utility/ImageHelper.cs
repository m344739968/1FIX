using System.Drawing;
using System.Drawing.Imaging;

namespace _1Fix.Utility
{
    public class ImageHelper
    {
        /// <summary>
        /// 作者：Vincen
        /// 时间：2013.12.13
        /// 描述：生成缩略图方式（枚举）
        /// </summary>
        public enum ModeType
        {
            HeightWidth = 1,    //指定高宽缩放（可能变形）
            Width,              //指定宽，高按比例
            Height,             //指定高，宽按比例
            Cut                 //指定高宽裁减（不变形）
        }
        public static string GetImageColor(string color)
        {
            string result = string.Empty;
            switch (color)
            {
                case "黑色":
                    result = "black";
                    break;
                case "银色":
                case "白色":
                    result = "white";
                    break;
                case "金色":
                    result = "gold";
                    break;
                case "粉色":
                    result = "pink";
                    break;
                default:
                    result = "black";
                    break;
            }
            return result;
        }
        /// <summary>
        /// 作者：Vincen
        /// 时间：2013.12.16
        /// 描述：剪切图片
        /// </summary>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap KiCut(Bitmap b, int x, int y, int width, int height)
        {
            if (b == null)
            {
                return null;
            }
            var w = b.Width;
            var h = b.Height;
            if (x >= w || y >= h)
            {
                return null;
            }

            if (x + width > w)
            {
                width = w - x;
            }

            if (y + height > h)
            {
                height = h - y;
            }

            try
            {
                var bmpOut = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                Graphics g = Graphics.FromImage(bmpOut);
                g.DrawImage(b, new Rectangle(0, 0, width, height), new Rectangle(x, y, width, height), GraphicsUnit.Pixel);
                g.Dispose();
                return bmpOut;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 作者：Vincen
        /// 时间：2013.12.13
        /// 描述：生成缩略图
        /// </summary>
        /// <param name="imgPath"></param>
        /// <param name="savePath"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static void MakeThumbnail(string imgPath, string savePath, int w, int h, ModeType mode)
        {
            MakeThumbnail(imgPath, savePath, 0, 0, w, h, mode);
        }

        /// <summary>
        /// 作者：Vincen
        /// 时间：2013.12.13
        /// 描述：生成缩略图
        /// </summary>
        /// <param name="imgPath">源图路径（物理路径）</param>
        /// <param name="savePath"></param>
        /// <param name="x">开始坐标（x轴）</param>
        /// <param name="y">开始坐标（y轴）</param>
        /// <param name="w">缩略图宽度</param>
        /// <param name="h">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>   
        public static void MakeThumbnail(string imgPath, string savePath, int x, int y, int w, int h, ModeType mode)
        {
            var img = Image.FromFile(imgPath);
            MakeThumbnail(img, savePath, x, y, w, h, mode);
        }

        /// <summary>
        /// 作者：Vincen
        /// 时间：2013.12.13
        /// 描述：生成缩略图
        /// </summary>
        /// <param name="img"></param>
        /// <param name="savePath"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static void MakeThumbnail(Image img, string savePath, int x, int y, int w, int h, ModeType mode)
        {
            var towidth = w;
            var toheight = h;

            var ow = img.Width;
            var oh = img.Height;

            switch (mode)
            {
                case ModeType.HeightWidth:
                    break;
                case ModeType.Width:
                    toheight = img.Height * w / img.Width;
                    break;
                case ModeType.Height:
                    towidth = img.Width * h / img.Height;
                    break;
                case ModeType.Cut:
                    if ((float)img.Width / (float)img.Height > (float)towidth / (float)toheight)
                    {
                        oh = img.Height;
                        ow = img.Height * towidth / toheight;
                        y = 0;
                        x = (img.Width - ow) / 2;
                    }
                    else
                    {
                        ow = img.Width;
                        oh = img.Width * h / towidth;
                        x = 0;
                        y = (img.Height - oh) / 2;
                    }
                    break;
            }

            //新建一个bmp图片
            using (Image bitmap = new Bitmap(towidth, toheight))
            {
                //新建一个画板
                var g = Graphics.FromImage(bitmap);

                //设置高质量插值法
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

                //设置高质量,低速度呈现平滑程度
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                //清空画布并以透明背景色填充
                g.Clear(Color.Transparent);

                //在指定位置并且按指定大小绘制原图片的指定部分
                g.DrawImage(img, new Rectangle(0, 0, towidth, toheight),
                    new Rectangle(x, y, ow, oh),
                    GraphicsUnit.Pixel);

                img.Dispose();
                g.Dispose();
                bitmap.Save(savePath);
                //return bitmap;
            }
        }
    }
}
