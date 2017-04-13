using _1Fix.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1Fix.BLL
{
    public class SellAddressBLL : BaseBLL
    {
        /// <summary>
        /// 获取默认寄售地址
        /// </summary>
        /// <returns></returns>
        public static SellAddress GetDefaultSellAdress()
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                return db.SellAddress.AsNoTracking().Where(n => n.IsDefault == 1).FirstOrDefault();
            }
        }
    }
}
