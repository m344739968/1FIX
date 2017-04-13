using _1Fix.Common;
using _1Fix.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webdiyer.WebControls.Mvc;

namespace _1Fix.BLL
{
    public class MicroMessageNewPhoneBLL
    {
        /// <summary>
        /// 获取微信价格表列表
        /// </summary>
        /// <param name="phonename"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static PagedList<MicroMessage_NewPhone_Money> GetMicroMessageNewPhoneList(string phonename, Paging page)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var query = db.MicroMessage_NewPhone_Money.AsNoTracking().ToList();
                if (!string.IsNullOrEmpty(phonename))
                {
                    query = query.Where(n => n.PhoneName.ToUpper().Contains(phonename.Trim().ToUpper())).ToList();
                }
                return query.OrderBy(n => n.OrderID)
                    .ToPagedList<MicroMessage_NewPhone_Money>(page.PageIndex, page.PageSize);
            }
        }
        /// <summary>
        /// 获取单个微信价格表信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static MicroMessage_NewPhone_Money GetMicroMessageNewPhoneByID(int? id)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var query = db.MicroMessage_NewPhone_Money.AsNoTracking().Where(n => n.ID == id).FirstOrDefault();
                return query;
            }
        }
        /// <summary>
        /// 编辑价格表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Update(MicroMessage_NewPhone_Money model)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                int result = 0;
                if (model.ID > 0)
                {
                    db.Entry<MicroMessage_NewPhone_Money>(model).State = EntityState.Modified;
                }
                else
                {
                    db.Entry<MicroMessage_NewPhone_Money>(model).State = EntityState.Added;
                }
                result = db.SaveChanges();
                return result;
            }
        }

        /// <summary>
        /// 删除价格表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Delete(int id)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                int result = 0;
                if (id > 0)
                {
                    var query = db.MicroMessage_NewPhone_Money.AsNoTracking().Where(n => n.ID == id).FirstOrDefault();
                    if (query != null)
                    {
                        db.Entry<MicroMessage_NewPhone_Money>(query).State = EntityState.Deleted;
                    }
                }
                result = db.SaveChanges();
                return result;
            }
        }
    }
}
