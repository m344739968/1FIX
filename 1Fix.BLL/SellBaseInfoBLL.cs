using _1Fix.Common;
using _1Fix.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Webdiyer.WebControls.Mvc;

namespace _1Fix.BLL
{
    public class SellBaseInfoBLL : BaseBLL
    {
        #region 商城前台
        /// <summary>
        /// 生成用户寄售信息
        /// </summary>
        /// <param name="sellphoneorder"></param>
        /// <returns></returns>
        public static int AddSellPhoneOrder(SellBaseInfo sellbaseinfo)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                int result = 0;
                if (sellbaseinfo != null)
                {
                    using (var tran = new TransactionScope(TransactionScopeOption.Required))
                    {
                        try
                        {
                            sellbaseinfo.PhoneModel = db.SellPhoneModel.Where(n => n.ID == sellbaseinfo.PhoneModelID).FirstOrDefault().PhoneName;
                            db.Entry<SellBaseInfo>(sellbaseinfo).State = EntityState.Added;

                            result = db.SaveChanges();
                            tran.Complete();
                        }
                        catch
                        {
                            return 0;
                        }
                    }
                }
                return result;
            }
        }
        /// <summary>
        /// 用户获取自己历史寄售信息
        /// </summary>
        /// <param name="UserInfoID"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static PagedList<SellBaseInfo> GetSellBaseInfoList(int UserInfoID, Paging page)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var query = db.SellBaseInfo.AsNoTracking().Where(n => n.UserInfoID == UserInfoID)
                    .ToPagedList<SellBaseInfo>(page.PageIndex, page.PageSize);
                return query;
            }
        }
        #endregion
    }
}
