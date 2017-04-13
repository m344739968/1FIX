using _1Fix.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1Fix.BLL
{
    public class WxPayInfoBLL
    {
        /// <summary>
        /// 添加用户收货地址
        /// </summary>
        /// <param name="useraddress"></param>
        /// <returns></returns>
        public static int AddWxPayInfo(WxPayInfo wxpayinfo)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                int result = 0;
                if (wxpayinfo != null)
                {
                    db.Entry<WxPayInfo>(wxpayinfo).State = EntityState.Added;
                    result = db.SaveChanges();
                }
                return result;
            }
        }
    }
}
