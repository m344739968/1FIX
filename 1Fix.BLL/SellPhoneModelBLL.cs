using _1Fix.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1Fix.BLL
{
    public class SellPhoneModelBLL : BaseBLL
    {
        /// <summary>
        /// 获取手机型号列表
        /// </summary>
        /// <returns></returns>
        public static List<SellPhoneModel> GetSellPhoneModelList()
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var query = db.SellPhoneModel.AsNoTracking()
                    .OrderByDescending(n => n.index)
                    .ToList();
                return query;
            }
        }
    }
}
