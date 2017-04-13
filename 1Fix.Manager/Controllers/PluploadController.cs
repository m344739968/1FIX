using _1Fix.BLL;
using _1Fix.Common;
using _1Fix.Entity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _1Fix.Manager.Controllers
{
    public class PluploadController : Controller
    {
        //
        // GET: /Plupload/

        public JsonResult Upload(int? chunk, string name)
        {
            try
            {
                var fileUpload = Request.Files["file"];
                string sn = Request["sn"].ToString();
                var uploadPath = Directory.GetParent(Server.MapPath("~/")).Parent.FullName + Global.UploadImage + "/" + sn;
                chunk = chunk ?? 0;
                //验证文件夹是否存在
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                string fileName = fileUpload.FileName;
                name = DateTime.Now.ToString("yyyyMMddHHmmssfff") + fileName.Substring(fileName.LastIndexOf("."));
                uploadPath = Path.Combine(uploadPath, name);

                using (var fs = new FileStream(uploadPath, chunk == 0 ? FileMode.Create : FileMode.Append))
                {
                    var buffer = new byte[fileUpload.InputStream.Length];
                    fileUpload.InputStream.Read(buffer, 0, buffer.Length);
                    fs.Write(buffer, 0, buffer.Length);
                }

                CheckImages checkimages = new CheckImages();
                checkimages.Sn = sn;
                checkimages.Image = name;
                int result = CheckImagesBLL.AddPhoneImage(checkimages);
                if (result > 0)
                {
                    return Json(new { status = 1, id = checkimages.ID, name = checkimages.Image });
                }
                else
                {
                    return Json(new { status = 0, id = 0, name = "" });
                }
            }
            catch
            {
                return Json(new { status = -1, id = 0, name = "" });
            }
        }

    }
}
