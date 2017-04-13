using _1Fix.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1Fix.BLL
{
    public class UserAddressBLL : BaseBLL
    {
        #region 商城前端操作
        /// <summary>
        /// 获取用户所有收货地址
        /// </summary>
        /// <returns></returns>
        public static List<UserAddress> GetUserAddressList(int UserInfoID)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                return db.UserAddress.AsNoTracking()
                    .OrderByDescending(n => n.ID)
                    .Where(n => n.UserInfoID == UserInfoID).ToList();
            }
        }
        /// <summary>
        /// 添加用户收货地址
        /// </summary>
        /// <param name="useraddress"></param>
        /// <returns></returns>
        public static int AddUserAddress(UserAddress useraddress)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                int result = 0;
                if (useraddress != null)
                {
                    db.Entry<UserAddress>(useraddress).State = EntityState.Added;
                    result = db.SaveChanges();
                }
                return result;
            }
        }
        /// <summary>
        /// 更新用户地址
        /// </summary>
        /// <param name="useraddress"></param>
        /// <returns></returns>
        public static int UpdateUserAddress(UserAddress useraddress)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                int result = 0;
                if (useraddress != null)
                {
                    db.Entry<UserAddress>(useraddress).State = EntityState.Modified;
                    result = db.SaveChanges();
                }
                return result;
            }
        }
        /// <summary>
        /// 删除地址
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int DeleteUserAddress(int id)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                int result = 0;
                var query = db.UserAddress.AsNoTracking().Where(n => n.ID == id).FirstOrDefault();
                if (query != null)
                {
                    db.Entry<UserAddress>(query).State = EntityState.Deleted;
                    result = db.SaveChanges();
                }
                return result;
            }
        }
        /// <summary>
        /// 获取用户收货地址
        /// </summary>
        /// <returns></returns>
        public static UserAddress GetUserAddress(int id)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                return db.UserAddress.AsNoTracking().Where(n => n.ID == id).FirstOrDefault();
            }
        }
        #endregion


    }
}
