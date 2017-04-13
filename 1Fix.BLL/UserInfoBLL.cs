using _1Fix.Common;
using _1Fix.Entity;
using _1Fix.Entity.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webdiyer.WebControls.Mvc;

namespace _1Fix.BLL
{
    public class UserInfoBLL : BaseBLL
    {
        #region 商城前台操作
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="UserInfoID"></param>
        /// <returns></returns>
        public static UserInfo GetUserInfoByUserInfoID(int UserInfoID)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                return db.UserInfo.AsNoTracking().Where(n => n.ID == UserInfoID).FirstOrDefault();
            }
        }
        /// <summary>
        /// 微信注册用户
        /// </summary>
        public static bool SignUp(WeiXinUser user)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                UserInfo userinfo = new UserInfo();
                userinfo.openid = user.openid;
                userinfo.nickname = user.nickname;
                userinfo.sex = Convert.ToInt32(user.sex);
                userinfo.language = "zh_CN";
                userinfo.city = user.city;
                userinfo.province = user.province;
                userinfo.country = user.country;
                userinfo.headimgurl = user.headimgurl;
                userinfo.unionid = user.unionid;
                userinfo.lasttime = DateTime.Now;
                userinfo.VipGroup = 1;//默认个人用户注册
                db.Entry<UserInfo>(userinfo).State = EntityState.Added;
                return db.SaveChanges() > 0;
            }
        }
        /// <summary>
        /// 修改用户最后登录时间
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool Update(WeiXinUser user)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var userinfo = db.UserInfo.AsNoTracking().SingleOrDefault(n => n.openid == user.openid);
                if (userinfo != null)
                {
                    //登录时获取用户级别
                    user.VipGroup = userinfo.VipGroup.ToString();
                    //登录时修改最后登录时间
                    userinfo.lasttime = DateTime.Now;

                    db.Entry<UserInfo>(userinfo).State = EntityState.Modified;
                    return db.SaveChanges() > 0;
                }
                return false;
            }
        }
        /// <summary>
        /// 修改用户最后登录时间
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool BindWeixin(WeiXinUser user)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var userinfo = db.UserInfo.AsNoTracking().SingleOrDefault(n => n.ID == user.id);
                if (userinfo != null)
                {

                    userinfo.openid = user.openid;
                    //userinfo.nickname = user.nickname;
                    userinfo.sex = Convert.ToInt32(user.sex);
                    userinfo.language = "zh_CN";
                    userinfo.city = user.city;
                    userinfo.province = user.province;
                    userinfo.country = user.country;
                    userinfo.headimgurl = user.headimgurl;
                    userinfo.unionid = user.unionid;

                    db.Entry<UserInfo>(userinfo).State = EntityState.Modified;
                    return db.SaveChanges() > 0;
                }
                return false;
            }
        }
        /// <summary>
        /// 检测是否注册
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public static UserInfo IsExists(string openid)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                UserInfo userinfo = db.UserInfo.AsNoTracking().Where(n => n.openid == openid).FirstOrDefault();
                return userinfo;
            }
        }
        /// <summary>
        /// 检测用户名是否存在
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static bool IsExistsUserName(string username)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                UserInfo userinfo = db.UserInfo.AsNoTracking().Where(n => n.username.ToUpper() == username.ToUpper()).FirstOrDefault();
                return userinfo != null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="validcode"></param>
        /// <returns></returns>
        public static int AddUser(string username, string nickname, string password)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                UserInfo userinfo = new UserInfo();
                userinfo.username = username;
                userinfo.nickname = nickname;
                userinfo.password = password;
                userinfo.VipGroup = 1;//默认
                userinfo.lasttime = DateTime.Now;
                db.Entry<UserInfo>(userinfo).State = EntityState.Added;
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static UserInfo IsExistsUser(string username, string password)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                UserInfo userinfo = db.UserInfo.AsNoTracking().Where(n => n.username.ToUpper() == username.ToUpper() && n.password == password).FirstOrDefault();
                return userinfo;
            }
        }
        /// <summary>
        /// 获取我的订单信息
        /// </summary>
        /// <param name="UserInfoID"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static PagedList<CuSellPhoneOrder> GetMySellPhoneOrderList(int UserInfoID, Paging page)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var query = from a in db.SellPhoneOrder.AsNoTracking()
                            join b in db.UserInfo.AsNoTracking() on a.UserInfoID equals b.ID
                            join c in db.SellPhoneOrderDetail.AsNoTracking() on a.OrderNo equals c.OrderNo
                            join d in db.CheckFacility.AsNoTracking() on c.Sn equals d.sn
                            where a.UserInfoID == UserInfoID
                            select new CuSellPhoneOrder
                            {
                                SellPhoneOrder = a,
                                UserInfo = b,
                                SellPhoneOrderDetail = c,
                                CheckFacility = d
                            };
                return query.OrderByDescending(n => n.SellPhoneOrder.ID)
                    .ToPagedList<CuSellPhoneOrder>(page.PageIndex, page.PageSize);
            }
        }
        /// <summary>
        /// 获取我的寄售信息
        /// </summary>
        /// <param name="UserInfoID"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static PagedList<CuSellBaseInfo> GetMySellBaseInfoList(int UserInfoID, Paging page)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var query = from a in db.SellBaseInfo.AsNoTracking()
                            join b in db.UserInfo.AsNoTracking() on a.UserInfoID equals b.ID
                            where a.UserInfoID == UserInfoID
                            select new CuSellBaseInfo
                            {
                                SellBaseInfo = a,
                                UserInfo = b,
                                SellPhoneStatus = db.SellPhone.Where(n => n.SellBaseInfoID == a.ID).FirstOrDefault() == null ? -1 : db.SellPhone.Where(n => n.SellBaseInfoID == a.ID).FirstOrDefault().Status
                            };
                return query.OrderByDescending(n => n.SellBaseInfo.ID)
                    .ToPagedList<CuSellBaseInfo>(page.PageIndex, page.PageSize);
            }
        }
        #endregion

        #region 商城后台操作
        /// <summary>
        /// 管理微信用户列表
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="phonename"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static PagedList<UserInfo> GetUserInfoList(string nickname, Paging page)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var query = db.UserInfo.AsNoTracking().ToList();
                if (!string.IsNullOrEmpty(nickname))
                {
                    query = query.Where(n => n.nickname.ToUpper().Contains(nickname.Trim().ToUpper())).ToList();
                }
                return query.OrderByDescending(n => n.ID)
                    .ToPagedList<UserInfo>(page.PageIndex, page.PageSize);
            }
        }
        /// <summary>
        /// 获取单个用户信息
        /// </summary>
        /// <param name="PhoneID"></param>
        /// <returns></returns>
        public static UserInfo GetUserInfoByID(int ID)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                return db.UserInfo.AsNoTracking().Where(n => n.ID == ID).FirstOrDefault();
            }
        }
        /// <summary>
        /// 更改微信 用户类型 c,b,b+
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="vipgroup"></param>
        /// <returns></returns>
        public static int Update(int ID, int vipgroup)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var userinfo = db.UserInfo.SingleOrDefault(n => n.ID == ID);
                if (userinfo != null)
                {
                    userinfo.VipGroup = vipgroup;
                    db.Entry<UserInfo>(userinfo).State = EntityState.Modified;
                    return db.SaveChanges();
                }
                return 0;
            }
        }
        #endregion

    }
}
