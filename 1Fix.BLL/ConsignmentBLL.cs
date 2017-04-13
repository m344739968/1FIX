using _1Fix.Common;
using _1Fix.Entity;
using _1Fix.Entity.Model;
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
    /// <summary>
    /// 寄卖中手机业务
    /// </summary>
    public class ConsignmentBLL : BaseBLL
    {
        #region 商城后台
        /// <summary>
        /// 寄卖手机列表
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="phonename"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static PagedList<CuSellPhone> GetSellPhoneList(string nickname, string sn, string phonename, Paging page)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var query = from a in db.SellPhone.AsNoTracking()
                            join b in db.SellBaseInfo.AsNoTracking() on a.SellBaseInfoID equals b.ID
                            join c in db.UserInfo.AsNoTracking() on b.UserInfoID equals c.ID
                            join d in db.CheckFacility.AsNoTracking() on a.Sn equals d.sn
                            select new CuSellPhone
                            {
                                SellPhone = a,
                                SellBaseInfo = b,
                                UserInfo = c,
                                CheckFacility = d
                            };
                if (!string.IsNullOrEmpty(nickname))
                {
                    query = query.Where(n => n.UserInfo.nickname.ToUpper().Contains(nickname.Trim().ToUpper()));
                }
                if (!string.IsNullOrEmpty(sn))
                {
                    query = query.Where(n => n.CheckFacility.sn.ToUpper().Contains(sn.Trim().ToUpper()));
                }
                if (!string.IsNullOrEmpty(phonename))
                {
                    query = query.Where(n => n.SellBaseInfo.PhoneModel.Contains(phonename.Trim()));
                }
                return query.OrderByDescending(n => n.SellPhone.ID)
                    .ToPagedList<CuSellPhone>(page.PageIndex, page.PageSize);
            }
        }
        /// <summary>
        /// 寄卖用户列表
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="phonename"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static PagedList<SellBaseInfo> GetSellUserList(string contactname, string contactphone, string sn, string courierno, Paging page)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var query = db.SellBaseInfo.AsNoTracking().ToList();
                if (!string.IsNullOrEmpty(contactname))
                {
                    query = query.Where(n => n.ContactName.ToUpper().Contains(contactname.Trim().ToUpper())).ToList();
                }
                if (!string.IsNullOrEmpty(contactname))
                {
                    query = query.Where(n => n.ContactPhone.Contains(contactname.Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(sn))
                {
                    query = query.Where(n => n.Sn.Contains(sn.Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(courierno))
                {
                    query = query.Where(n => n.CourierNo.Contains(courierno.Trim())).ToList();
                }
                return query.OrderByDescending(n => n.ID)
                    .ToPagedList<SellBaseInfo>(page.PageIndex, page.PageSize);
            }
        }
        /// <summary>
        /// 获取单个寄卖基础信息
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public static CheckFacility GetCheckFacilityBySn(string sn)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                return db.CheckFacility.AsNoTracking().Where(n => n.sn.ToUpper() == sn.ToUpper()).FirstOrDefault();
            }
        }
        /// <summary>
        /// 获取单个寄卖基础信息
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public static SellBaseInfo GetSellBaseInfoBySn(string sn)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                return db.SellBaseInfo.AsNoTracking().Where(n => n.Sn.ToUpper() == sn.ToUpper()).FirstOrDefault();
            }
        }
        /// <summary>
        /// 获取单个寄卖基础信息
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public static SellBaseInfo GetSellBaseInfoByID(int id)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                return db.SellBaseInfo.AsNoTracking().Where(n => n.ID == id).FirstOrDefault();
            }
        }
        /// <summary>
        /// 审核用户寄卖信息
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public static int SellBaseInfoAudit(int id, string sn, string remark, int status, int auditperson)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var sellbaseinfo = db.SellBaseInfo.SingleOrDefault(n => n.ID == id);
                if (sellbaseinfo != null)
                {
                    using (var tran = new TransactionScope(TransactionScopeOption.Required))
                    {
                        try
                        {
                            sellbaseinfo.Remark = remark;
                            sellbaseinfo.Status = status;
                            sellbaseinfo.AuditPerson = auditperson;
                            db.Entry<SellBaseInfo>(sellbaseinfo).State = EntityState.Modified;

                            if (status == 1)
                            {
                                //审核通过则插入到寄卖手机表中
                                var sellphone = db.SellPhone.SingleOrDefault(n => n.SellBaseInfoID == id);
                                if (sellphone == null)
                                {
                                    sellphone = new SellPhone();
                                    sellphone.SellBaseInfoID = id;
                                    sellphone.Sn = sn;
                                    sellphone.Status = -1;//未上架
                                    sellphone.SellCategory = 1;
                                    sellphone.AddDate = DateTime.Now;
                                    sellphone.AddPerson = auditperson;
                                    db.Entry<SellPhone>(sellphone).State = EntityState.Added;
                                }
                                else
                                {
                                    //sellphone.Status = -1;//未上架
                                    //sellphone.SellCategory = 1;
                                    //sellphone.AddDate = DateTime.Now;
                                    //sellphone.AddPerson = auditperson;
                                    //db.Entry<SellPhone>(sellphone).State = EntityState.Modified;
                                }
                            }
                            int result = db.SaveChanges();
                            tran.Complete();
                            return result;
                        }
                        catch
                        {
                            return 0;
                        }
                    }
                }
                return 0;
            }
        }
        /// <summary>
        /// 上下架操作(定义个人价格，企业价格，大客户价格)
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="PersonPrice"></param>
        /// <param name="EnterprisePrice"></param>
        /// <param name="BigCustomerPrice"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static int SaveSellPhoneUpDown(string sn, int status)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var sellphone = db.SellPhone.FirstOrDefault(n => n.Sn == sn.Trim());
                if (sellphone != null)
                {
                    //sellphone.PersonPrice = PersonPrice;
                    //sellphone.EnterprisePrice = EnterprisePrice;
                    //sellphone.BigCustomerPrice = BigCustomerPrice;
                    sellphone.Status = status;
                    db.Entry<SellPhone>(sellphone).State = EntityState.Modified;
                    int result = db.SaveChanges();
                    return result;
                }
                return 0;
            }
        }
        /// <summary>
        /// 修改价格
        /// </summary>
        /// <param name="id">sellbaseinfoid</param>
        /// <param name="PersonPrice"></param>
        /// <param name="EnterprisePrice"></param>
        /// <param name="BigCustomerPrice"></param>
        /// <returns></returns>
        public static int SaveSellPhonePrice(int sellbaseinfoid, decimal PersonPrice, decimal EnterprisePrice, decimal BigCustomerPrice)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var sellphone = db.SellPhone.FirstOrDefault(n => n.SellBaseInfoID == sellbaseinfoid);
                if (sellphone != null)
                {
                    sellphone.PersonPrice = PersonPrice;
                    sellphone.EnterprisePrice = EnterprisePrice;
                    sellphone.BigCustomerPrice = BigCustomerPrice;
                    db.Entry<SellPhone>(sellphone).State = EntityState.Modified;
                    int result = db.SaveChanges();
                    return result;
                }
                return 0;
            }
        }
        /// <summary>
        /// 获取寄卖手机检测报告信息
        /// </summary>
        /// <param name="PhoneID"></param>
        /// <returns></returns>
        public static CheckFacility GetCheckFacilityList(string PhoneID)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                return db.CheckFacility.AsNoTracking().Where(n => n.ID.ToString() == PhoneID).FirstOrDefault();
            }
        }
        /// <summary>
        /// 获取寄卖手机检测报告信息
        /// </summary>
        /// <param name="PhoneID"></param>
        /// <returns></returns>
        public static CheckFacility GetCheckFacility(string sn)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                return db.CheckFacility.AsNoTracking().Where(n => n.sn.ToUpper() == sn.ToUpper()).FirstOrDefault();
            }
        }

        #endregion

        #region 商城前台
        /// <summary>
        /// 获取寄卖手机列表(商城首页使用)
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="phonename"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static PagedList<CuSellPhone> GetSellPhoneList()
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var query = from a in db.SellPhone.AsNoTracking()
                            join b in db.SellBaseInfo.AsNoTracking() on a.SellBaseInfoID equals b.ID
                            join c in db.UserInfo.AsNoTracking() on b.UserInfoID equals c.ID
                            join d in db.CheckFacility.AsNoTracking() on a.Sn equals d.sn
                            where a.Status == 0 //已上架的
                            select new CuSellPhone
                            {
                                SellPhone = a,
                                SellBaseInfo = b,
                                UserInfo = c,
                                CheckFacility = d
                            };
                return query.OrderByDescending(n => n.SellPhone.ID).Take(8)
                    .ToPagedList<CuSellPhone>(1, 8);
            }
        }
        /// <summary>
        /// 寄卖手机详细信息
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="phonename"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static CuSellPhone GetSellPhoneBySn(string sn)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var query = from a in db.SellPhone.AsNoTracking()
                            join b in db.SellBaseInfo.AsNoTracking() on a.SellBaseInfoID equals b.ID
                            join c in db.UserInfo.AsNoTracking() on b.UserInfoID equals c.ID
                            join d in db.CheckFacility.AsNoTracking() on a.Sn equals d.sn
                            where a.Sn == sn
                            select new CuSellPhone
                            {
                                SellPhone = a,
                                SellBaseInfo = b,
                                UserInfo = c,
                                CheckFacility = d
                            };
                return query.FirstOrDefault();
            }
        }
        /// <summary>
        /// 寄卖手机详细信息(前端修改价格时使用)
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="phonename"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static CuSellPhone GetSellPhoneBySellBaseInfoID(int id)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var query = from a in db.SellPhone.AsNoTracking()
                            join b in db.SellBaseInfo.AsNoTracking() on a.SellBaseInfoID equals b.ID
                            where a.SellBaseInfoID == id
                            select new CuSellPhone
                            {
                                SellPhone = a,
                                SellBaseInfo = b
                            };
                return query.FirstOrDefault();
            }
        }
        /// <summary>
        /// 获取寄售手机实物图片
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public static List<CheckImages> GetCheckImagesList(string sn)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var query = db.CheckImages.AsNoTracking().Where(n => n.Sn.Trim().ToUpper() == sn.Trim().ToUpper());
                List<CheckImages> mo = db.CheckImages.ToList<CheckImages>();

                return query.Take(6).ToList();
            }
        }
        /// <summary>
        /// 修改价格
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="PersonPrice"></param>
        /// <param name="EnterprisePrice"></param>
        /// <param name="BigCustomerPrice"></param>
        /// <returns></returns>
        public static int SaveSellPhonePrice(int id, string sn, string Title, decimal? PersonPrice, decimal? EnterprisePrice, decimal? BigCustomerPrice, string vipgroup)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var sellphone = db.SellPhone.FirstOrDefault(n => n.SellBaseInfoID == id && n.Sn == sn.Trim());
                if (sellphone != null)
                {
                    sellphone.Title = Title;
                    sellphone.PersonPrice = PersonPrice;
                    if (vipgroup == "2")
                    {
                        sellphone.EnterprisePrice = EnterprisePrice;
                    }
                    else if (vipgroup == "3")
                    {
                        sellphone.EnterprisePrice = EnterprisePrice;
                        sellphone.BigCustomerPrice = BigCustomerPrice;
                    }
                    db.Entry<SellPhone>(sellphone).State = EntityState.Modified;
                    int result = db.SaveChanges();
                    return result;
                }
                return 0;
            }
        }

        /// <summary>
        /// 获取寄卖手机列表(商城首页使用)
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="phonename"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static PagedList<CuSellPhone> GetSellPhoneListByKey(string key, string jb, string phonemodelid, string color, int? orderbyprice, int? orderbydate, int? orderbyclicknum, Paging page)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var query = from a in db.SellPhone.AsNoTracking()
                            join b in db.SellBaseInfo.AsNoTracking() on a.SellBaseInfoID equals b.ID
                            join d in db.CheckFacility.AsNoTracking() on a.Sn equals d.sn
                            where a.Status == 0 //已上架的
                            select new CuSellPhone
                            {
                                SellPhone = a,
                                SellBaseInfo = b,
                                CheckFacility = d
                            };
                List<CuSellPhone> nnnn = query.ToList();
                if (!string.IsNullOrEmpty(key))
                {
                    query = query.Where(n => n.CheckFacility.phonename.Contains(key.Trim()) || n.CheckFacility.phonename.Replace(" ", "").Trim().Contains(key.Replace(" ", "").Trim()));
                }
                if (!string.IsNullOrEmpty(jb))
                {
                    query = query.Where(n => n.CheckFacility.jb.Equals(jb));
                }
                if (!string.IsNullOrEmpty(phonemodelid))
                {
                    int modelid = Convert.ToInt32(phonemodelid);
                    query = query.Where(n => n.SellBaseInfo.PhoneModelID == modelid);
                }
                if (!string.IsNullOrEmpty(color))
                {
                    query = query.Where(n => n.CheckFacility.color.Contains(color));
                }
                if (orderbyprice != null)
                {
                    if (orderbyprice == 1)
                    {
                        query = query.OrderBy(n => n.SellPhone.PersonPrice);
                    }
                    else
                    {
                        query = query.OrderByDescending(n => n.SellPhone.PersonPrice);
                    }
                }
                if (orderbydate != null)
                {
                    if (orderbydate == 1)
                    {
                        query = query.OrderBy(n => n.SellPhone.AddDate);
                    }
                    else
                    {
                        query = query.OrderByDescending(n => n.SellPhone.AddDate);
                    }
                }
                if (orderbyclicknum != null)
                {
                    if (orderbyclicknum == 1)
                    {
                        query = query.OrderBy(n => n.SellPhone.ClickNum);
                    }
                    else
                    {
                        query = query.OrderByDescending(n => n.SellPhone.ClickNum);
                    }
                }
                if (orderbyprice == null && orderbydate == null && orderbyclicknum == null)
                {
                    query = query.OrderByDescending(n => n.SellPhone.ID);

                }
                return query.ToPagedList<CuSellPhone>(page.PageIndex, page.PageSize);
            }
        }
        /// <summary>
        /// 获取寄卖手机列表(商城首页使用)
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="phonename"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static PagedList<CuSellPhone> GetSellPhoneListByKey(string key, string jb, string phonemodelid, string color, string price, Paging page)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var query = from a in db.SellPhone.AsNoTracking()
                            join b in db.SellBaseInfo.AsNoTracking() on a.SellBaseInfoID equals b.ID
                            join d in db.CheckFacility.AsNoTracking() on a.Sn equals d.sn
                            where a.Status == 0 //已上架的
                            select new CuSellPhone
                            {
                                SellPhone = a,
                                SellBaseInfo = b,
                                CheckFacility = d
                            };
                List<CuSellPhone> nnnn = query.ToList();
                if (!string.IsNullOrEmpty(key))
                {
                    query = query.Where(n => n.CheckFacility.phonename.Contains(key.Trim()) || n.CheckFacility.phonename.Replace(" ", "").Trim().Contains(key.Replace(" ", "").Trim()));
                }
                if (!string.IsNullOrEmpty(jb))
                {
                    query = query.Where(n => n.CheckFacility.jb.Contains(jb));
                }
                if (!string.IsNullOrEmpty(phonemodelid))
                {
                    int modelid = Convert.ToInt32(phonemodelid);
                    query = query.Where(n => n.SellBaseInfo.PhoneModelID == modelid);
                }
                if (!string.IsNullOrEmpty(color))
                {
                    query = query.Where(n => n.CheckFacility.color.Contains(color));
                }
                if (!string.IsNullOrEmpty(price))
                {
                    string[] tempPrice = price.Split(',');
                    if (tempPrice.Length == 2)
                    {
                        decimal sprice = Convert.ToDecimal(tempPrice[0]);
                        decimal eprice = Convert.ToDecimal(tempPrice[1]);
                        if (sprice == 0)
                        {
                            //小于价格
                            query = query.Where(n => n.SellPhone.PersonPrice >= sprice && n.SellPhone.PersonPrice < eprice);
                        }
                        else if (eprice == 0)
                        {
                            //大于等于价格
                            query = query.Where(n => n.SellPhone.PersonPrice >= sprice);
                        }
                        else if (sprice > 0 && eprice > 0)
                        {
                            //两者之间的价格
                            query = query.Where(n => n.SellPhone.PersonPrice >= sprice && n.SellPhone.PersonPrice < eprice);
                        }
                    }

                }
                return query
                    .OrderByDescending(n => n.SellPhone.ID)
                    .ToPagedList<CuSellPhone>(page.PageIndex, page.PageSize);
            }
        }
        /// <summary>
        /// 上下架操作(定义个人价格，企业价格，大客户价格)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static int SaveSellPhoneUpDown(int sellbaseinfoid, int status)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var sellphone = db.SellPhone.AsNoTracking().FirstOrDefault(n => n.SellBaseInfoID == sellbaseinfoid);
                if (sellphone != null)
                {
                    //下单后就不能修改上下架状态
                    if (sellphone.Status == 1 || sellphone.Status == 2 || sellphone.Status == 3 || sellphone.Status == 4)
                    {
                        return -1;
                    }

                    sellphone.Status = status;
                    db.Entry(sellphone).State = EntityState.Modified;
                    int result = db.SaveChanges();
                    return result;
                }
            }
            return 0;
        }
        /// <summary>
        /// 维护用户点击次数
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public static int UpdateUserClickNum(string sn)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var sellphone = db.SellPhone.AsNoTracking().FirstOrDefault(n => n.Sn == sn);
                if (sellphone != null)
                {
                    int? clickNum = sellphone.ClickNum == null ? 0 : sellphone.ClickNum;
                    sellphone.ClickNum = clickNum + 1;

                    db.Entry(sellphone).State = EntityState.Modified;
                    int result = db.SaveChanges();
                    return result;
                }
                return 0;
            }
        }

        #endregion
    }
}
