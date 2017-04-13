using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace _1Fix.Common
{
    public class ValidCode
    {
        /// <summary>
        /// 作者：Malei
        /// 时间：2013.11.2
        /// 描述：返回指定长度的验证码，默认为4位
        /// </summary>
        /// <returns></returns>
        public static string GenerateCheckCode(int codeLength = 4)
        {
            var checkCode = String.Empty;
            var numbers = new char[] { '1', '3', '4', '5', '6', '7', '8', '9' };
            var characters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

            var random = new Random();

            for (int i = 0; i < codeLength; i++)
            {
                var number = random.Next(1, 10);
                char code;
                if (number % 2 == 0)
                    code = numbers[random.Next(0, 8)];
                else
                    code = characters[random.Next(0, 24)];

                checkCode += code;
            }
            return checkCode;
        }
        /// <summary>
        /// 作者：Malei
        /// 时间：2013.11.2
        /// 描述：生成短信验证码，默认为4位
        /// </summary>
        /// <returns></returns>
        public static string GenerateMsgCode(int codeLength = 4)
        {
            var checkCode = String.Empty;
            var numbers = new char[] { '1', '3', '4', '5', '6', '7', '8', '9' };

            var random = new Random();

            for (int i = 0; i < codeLength; i++)
            {
                var number = random.Next(1, 10);
                char code;
                if (number % 2 == 0)
                    code = numbers[random.Next(0, 8)];
                else
                    code = numbers[random.Next(0, 8)];

                checkCode += code;
            }
            return checkCode;
        }
        /// <summary>
        /// 作者：Malei
        /// 时间：2013.11.2
        /// 描述：获取注册验证码
        /// </summary>
        public static void GetSignupCheckCode()
        {
            string checkCode = GenerateCheckCode();
            if (checkCode == null || checkCode.Trim() == String.Empty)
            {
                return;
            }
            HttpContext.Current.Response.Cookies.Add(new HttpCookie(Global.ValidSignupCodeKey, checkCode));
            var image = new Bitmap((int)Math.Ceiling((checkCode.Length * 12.5)), 22);
            Graphics g = Graphics.FromImage(image);
            try
            {
                //清空图片背景色
                g.Clear(Color.White);
                var font = new Font("Verdana", 11, (FontStyle.Regular | FontStyle.Italic));
                var brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.DarkRed, 1.2f, true);
                g.DrawString(checkCode, font, brush, 2, 2);

                var ms = new System.IO.MemoryStream();
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                HttpContext.Current.Response.ClearContent();
                HttpContext.Current.Response.ContentType = "image/Gif";
                HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }
        /// <summary>
        /// 作者：Malei
        /// 时间：2013.11.2
        /// 描述：获取登录验证码
        /// </summary>
        public static void GetSigninCheckCode()
        {
            string checkCode = GenerateCheckCode();
            if (checkCode == null || checkCode.Trim() == String.Empty)
            {
                return;
            }
            HttpContext.Current.Response.Cookies.Add(new HttpCookie(Global.ValidSigninCodeKey, checkCode));
            var image = new Bitmap((int)Math.Ceiling((checkCode.Length * 12.5)), 22);
            Graphics g = Graphics.FromImage(image);
            try
            {
                //清空图片背景色
                g.Clear(Color.White);
                var font = new Font("Verdana", 11, (FontStyle.Regular | FontStyle.Italic));
                var brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.DarkRed, 1.2f, true);
                g.DrawString(checkCode, font, brush, 2, 2);

                var ms = new System.IO.MemoryStream();
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                HttpContext.Current.Response.ClearContent();
                HttpContext.Current.Response.ContentType = "image/Gif";
                HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }
    }
}
