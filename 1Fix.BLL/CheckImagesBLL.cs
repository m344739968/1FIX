using _1Fix.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace _1Fix.BLL
{
    public class CheckImagesBLL
    {
        /// <summary>
        /// 手机图片编辑
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int AddPhoneImages(List<CheckImages> list)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                int result = 0;
                using (var tran = new TransactionScope(TransactionScopeOption.Required))
                {
                    try
                    {
                        if (list != null && list.Count > 0)
                        {
                            var query = db.CheckImages.AsNoTracking().Where(n => n.Sn == list[0].Sn);
                            foreach (CheckImages iamges in query)
                            {
                                db.Entry<CheckImages>(iamges).State = EntityState.Deleted;
                            }
                            foreach (CheckImages item in list)
                            {
                                db.Entry<CheckImages>(item).State = EntityState.Added;
                            }
                            result = db.SaveChanges();
                            tran.Complete();
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    catch
                    {
                        return 0;
                    }
                    return result;
                }
            }
        }

        /// <summary>
        /// 手机图片编辑
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int AddPhoneImage(CheckImages checkimage)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                int result = 0;
                using (var tran = new TransactionScope(TransactionScopeOption.Required))
                {
                    try
                    {
                        db.Entry<CheckImages>(checkimage).State = EntityState.Added;
                        result = db.SaveChanges();
                        tran.Complete();
                    }
                    catch
                    {
                        return 0;
                    }
                    return result;
                }
            }
        }
        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public static List<CheckImages> GetCheckImagesListBySn(string sn)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                return db.CheckImages.AsNoTracking().Where(n => n.Sn == sn).ToList();
            }
        }

        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public static int DelCheckImages(int id)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                int result = 0;
                var query= db.CheckImages.AsNoTracking().Where(n => n.ID == id).FirstOrDefault();
                db.Entry<CheckImages>(query).State = EntityState.Deleted;
                result = db.SaveChanges();
                return result;
            }
        }

    }
}
