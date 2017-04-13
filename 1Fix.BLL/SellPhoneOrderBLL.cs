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
    /// 寄卖手机订单业务
    /// </summary>
    public class SellPhoneOrderBLL : BaseBLL
    {
        #region 商城后台
        /// <summary>
        /// 寄卖手机订单列表
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="phonename"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static PagedList<CuSellPhoneOrder> GetSellPhoneOrderList(string nickname, string orderno, Paging page)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var query = from a in db.SellPhoneOrder.AsNoTracking()
                            join b in db.UserInfo.AsNoTracking() on a.UserInfoID equals b.ID
                            join c in db.UserAddress.AsNoTracking() on a.UserAddressID equals c.ID
                            select new CuSellPhoneOrder
                            {
                                SellPhoneOrder = a,
                                UserInfo = b,
                                UserAddress = c
                            };
                if (!string.IsNullOrEmpty(nickname))
                {
                    query = query.Where(n => n.UserInfo.nickname.ToUpper().Contains(nickname.Trim().ToUpper()));
                }
                if (!string.IsNullOrEmpty(orderno))
                {
                    query = query.Where(n => n.SellPhoneOrder.OrderNo.Contains(orderno.Trim()));
                }
                return query.OrderByDescending(n => n.SellPhoneOrder.ID)
                    .ToPagedList<CuSellPhoneOrder>(page.PageIndex, page.PageSize);
            }
        }
        /// <summary>
        /// 获取订单详细列表
        /// </summary>
        /// <param name="orderno"></param>
        /// <returns></returns>
        public static List<CuSellPhoneOrderDetail> GetSellPhoneOrderDetailByOrderNo(string orderno)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var query = from a in db.SellPhoneOrderDetail.AsNoTracking()
                            join b in db.CheckFacility.AsNoTracking() on a.Sn equals b.sn
                            where a.OrderNo == orderno
                            select new CuSellPhoneOrderDetail
                            {
                                SellPhoneOrderDetail = a,
                                CheckFacility = b
                            };
                return query.ToList();
            }
        }

        /// <summary>
        /// 更新订单状态
        /// </summary>
        /// <param name="orderno"></param>
        /// <returns></returns>
        public static int UpdateOrderStatus(string orderno, int status)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                int result = 0;
                using (var tran = new TransactionScope(TransactionScopeOption.Required))
                {
                    try
                    {
                        var sellphoneorder = db.SellPhoneOrder.AsNoTracking().Where(n => n.OrderNo == orderno).FirstOrDefault();
                        if (sellphoneorder != null)
                        {
                            //更新订单状态
                            sellphoneorder.Status = status;
                            db.Entry<SellPhoneOrder>(sellphoneorder).State = EntityState.Modified;

                        }
                        var query = db.SellPhoneOrderDetail.Where(n => n.OrderNo == orderno);
                        foreach (var item in query)
                        {
                            //更新寄卖手机状态
                            var sellphone = db.SellPhone.AsNoTracking().Where(n => n.Sn == item.Sn).FirstOrDefault();
                            if (sellphone != null)
                            {
                                sellphone.Status = status;
                                db.Entry<SellPhone>(sellphone).State = EntityState.Modified;
                            }
                        }
                        result = db.SaveChanges();
                        tran.Complete();
                    }
                    catch
                    {
                        return result;
                    }
                }
                return result;
            }
        }
        /// <summary>
        /// 修改订单买家信息
        /// </summary>
        /// <param name="orderno"></param>
        /// <returns></returns>
        public static int UpdateOrderBuyInfo(SellPhoneOrder model)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                int result = 0;
                using (var tran = new TransactionScope(TransactionScopeOption.Required))
                {
                    try
                    {
                        var sellphoneorder = db.SellPhoneOrder.AsNoTracking().Where(n => n.OrderNo == model.OrderNo).FirstOrDefault();
                        if (sellphoneorder != null)
                        {
                            sellphoneorder.BuyerEmail = model.BuyerEmail;
                            sellphoneorder.BuyerID = model.BuyerID;
                            sellphoneorder.TradeNo = model.TradeNo;
                            sellphoneorder.Totalfee = model.Totalfee;
                            sellphoneorder.Body = model.Body;
                            sellphoneorder.NoticeDate = model.NoticeDate;
                            sellphoneorder.OrderNo = model.OrderNo;
                            sellphoneorder.Status = model.Status; //修改状态
                            db.Entry<SellPhoneOrder>(sellphoneorder).State = EntityState.Modified;

                            //修改寄卖手机的状态
                            var query = db.SellPhoneOrderDetail.AsNoTracking().Where(n => n.OrderNo == model.OrderNo);
                            foreach (var item in query)
                            {
                                //更新寄卖手机状态
                                var sellphone = db.SellPhone.AsNoTracking().Where(n => n.Sn == item.Sn).FirstOrDefault();
                                if (sellphone != null)
                                {
                                    sellphone.Status = model.Status; //修改状态
                                    db.Entry<SellPhone>(sellphone).State = EntityState.Modified;
                                }
                            }

                            result = db.SaveChanges();
                            tran.Complete();
                        }
                    }
                    catch
                    {
                        return result;
                    }
                }
                return result;
            }
        }
        /// <summary>
        /// 已付款，发货操作
        /// </summary>
        /// <param name="orderno">订单号</param>
        /// <param name="sendperson">发货人</param>
        /// <param name="SendCourierNo">发送快递单号</param>
        /// <returns></returns>
        public static int OrderSend(string orderno, string sendperson, string sendcourierno)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                int result = 0;
                using (var tran = new TransactionScope(TransactionScopeOption.Required))
                {
                    try
                    {
                        var sellphoneorder = db.SellPhoneOrder.AsNoTracking().Where(n => n.OrderNo == orderno).FirstOrDefault();
                        if (sellphoneorder != null)
                        {
                            sellphoneorder.Status = 3;//已付款，已发货
                            sellphoneorder.SendPerson = sendperson;
                            sellphoneorder.SendCourierNo = sendcourierno;
                            sellphoneorder.SendDate = DateTime.Now;
                            db.Entry<SellPhoneOrder>(sellphoneorder).State = EntityState.Modified;

                            //修改寄卖手机的状态
                            var query = db.SellPhoneOrderDetail.AsNoTracking().Where(n => n.OrderNo == sellphoneorder.OrderNo);
                            foreach (var item in query)
                            {
                                //更新寄卖手机状态
                                var sellphone = db.SellPhone.AsNoTracking().Where(n => n.Sn == item.Sn).FirstOrDefault();
                                if (sellphone != null)
                                {
                                    sellphone.Status = 3;
                                    db.Entry<SellPhone>(sellphone).State = EntityState.Modified;
                                }
                            }

                            result = db.SaveChanges();
                            tran.Complete();
                        }
                    }
                    catch
                    {
                        return result;
                    }
                }
                return result;
            }
        }

        #endregion

        #region 商城前台
        /// <summary>
        /// 生成订单号
        /// </summary>
        /// <returns></returns>
        public static string GetOrderNo()
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                string result = db.proc_GenerateOrderNo().FirstOrDefault();
                return result;
            }
        }
        /// <summary>
        /// 生成用户订单信息
        /// </summary>
        /// <param name="sellphoneorder"></param>
        /// <returns></returns>
        public static int AddSellPhoneOrder(SellPhoneOrder sellphoneorder, SellPhoneOrderDetail sellphoneorderdetail)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                int result = 0;
                if (sellphoneorder != null && sellphoneorderdetail != null)
                {
                    using (var tran = new TransactionScope(TransactionScopeOption.Required))
                    {
                        try
                        {
                            //订单主表
                            sellphoneorder.OrderNo = GetOrderNo();
                            db.Entry<SellPhoneOrder>(sellphoneorder).State = EntityState.Added;
                            //订单详细表
                            sellphoneorderdetail.OrderNo = sellphoneorder.OrderNo;
                            db.Entry<SellPhoneOrderDetail>(sellphoneorderdetail).State = EntityState.Added;

                            //修改寄售手机状态
                            var query = db.SellPhone.AsNoTracking().Where(n => n.Sn == sellphoneorderdetail.Sn).FirstOrDefault();
                            if (query != null)
                            {
                                query.Status = 1; //已下单，未付款
                                db.Entry<SellPhone>(query).State = EntityState.Modified;
                            }

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
        /// 获取订单主表，子表详细信息
        /// </summary>
        /// <param name="orderno"></param>
        /// <returns></returns>
        public static CuSellPhoneOrder GetSellPhoneOrder(string orderno)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var query = from a in db.SellPhoneOrder.AsNoTracking()
                            join b in db.SellPhoneOrderDetail.AsNoTracking() on a.OrderNo equals b.OrderNo
                            join c in db.CheckFacility.AsNoTracking() on b.Sn equals c.sn
                            where a.OrderNo == orderno
                            select new CuSellPhoneOrder
                            {
                                SellPhoneOrder = a,
                                SellPhoneOrderDetail = b,
                                CheckFacility = c
                            };
                return query.FirstOrDefault();
            }
        }
        /// <summary>
        /// 获取订单主表信息
        /// </summary>
        /// <param name="orderno"></param>
        /// <returns></returns>
        public static SellPhoneOrder GetSellPhoneOrderByOrderNo(string orderno)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var query = db.SellPhoneOrder.AsNoTracking().Where(n => n.OrderNo == orderno).FirstOrDefault();
                return query;
            }
        }
        /// <summary>
        /// 用户获取自己历史订单信息
        /// </summary>
        /// <param name="UserInfoID"></param>
        /// <returns></returns>
        public static PagedList<CuSellPhoneOrder> GetCuSellPhoneOrderListByUserInfoID(int UserInfoID, Paging page)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var query = from a in db.SellPhoneOrder.AsNoTracking()
                            join b in db.UserInfo.AsNoTracking() on a.UserInfoID equals b.ID
                            join c in db.UserAddress.AsNoTracking() on a.UserAddressID equals c.ID
                            join d in db.SellPhoneOrderDetail.AsNoTracking() on a.OrderNo equals d.OrderNo
                            where a.UserInfoID == UserInfoID
                            select new CuSellPhoneOrder
                            {
                                SellPhoneOrder = a,
                                UserInfo = b,
                                UserAddress = c,
                                SellPhoneOrderDetail = d
                            };
                return query.OrderByDescending(n => n.SellPhoneOrder.ID)
                    .ToPagedList<CuSellPhoneOrder>(page.PageIndex, page.PageSize);
            }
        }
        /// <summary>
        /// 修改优惠券信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int UpdateSellPhoneOrderCoupons(SellPhoneOrder model)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                if (model != null)
                {
                    db.Entry<SellPhoneOrder>(model).State = EntityState.Modified;
                    return db.SaveChanges();
                }
                return 0;
            }
        }

        #endregion
    }
}
