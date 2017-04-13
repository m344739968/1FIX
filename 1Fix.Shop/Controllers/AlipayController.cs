using _1Fix.BLL;
using _1Fix.Common;
using _1Fix.Entity;
using _1Fix.Entity.Model;
using _1Fix.Utility;
using Com.Alipay;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace _1Fix.Shop.Controllers
{
    public class AlipayController : Controller
    {
        //
        // GET: /Alipay/

        public ActionResult Index()
        {
            string couponsno = "1";
            //string OrderNo = SellPhoneOrderBLL.GetOrderNo();
            //如果输入优惠券则使用优惠券重新计算价格
            if (!string.IsNullOrEmpty(couponsno))
            {
                string url = Global.ValidCouponsUrl + "?id=" + couponsno;
                //检验优惠券是否可用 
                string result = HttpRequestHelper.HttpPost(url, "");
                if (!string.IsNullOrEmpty(result))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    CuCoupons r = js.Deserialize<CuCoupons>(result);
                    if (r != null && r.Ticket.Status == 1)
                    {
                        var model = SellPhoneOrderBLL.GetSellPhoneOrderByOrderNo("20150914000009");
                        decimal totalfee = Convert.ToDecimal(model.OrderPrice - 100);
                    }
                    else
                    {
                        return Content("<script>window.alert('优惠券验证出错，请重试！');</script>");
                    }
                }
            }
            return View();
        }
        /// <summary>
        /// 检测优惠券码是否有效
        /// </summary>
        /// <param name="couponsno"></param>
        /// <returns></returns>
        public JsonResult ValidCoupons(string couponsno)
        {
            int status = 0;
            string msg = string.Empty;
            if (!string.IsNullOrEmpty(couponsno))
            {
                string url = Global.ValidCouponsUrl + "?id=" + couponsno;
                //检验优惠券是否可用 
                string result = HttpRequestHelper.HttpPost(url, "");
                if (!string.IsNullOrEmpty(result))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    CuCoupons r = js.Deserialize<CuCoupons>(result);
                    if (r != null && r.Ticket.Status == 1)
                    {
                        status = 1;
                        msg = r.Ticket.Coupons.Price.ToString();
                    }
                    else
                    {
                        status = 0;
                    }
                }
            }
            return Json(new { status = status, msg = msg });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderno">订单编号</param>
        /// <param name="totalfee">金额</param>
        /// <param name="couponsno">优惠券编号</param>
        /// <returns></returns>
        public ActionResult Submit(string orderno, decimal totalfee, string couponsno)
        {

            //如果输入优惠券则使用优惠券重新计算价格
            if (!string.IsNullOrEmpty(couponsno))
            {
                string url = Global.ValidCouponsUrl + "?id=" + couponsno;
                //检验优惠券是否可用 
                string result = HttpRequestHelper.HttpPost(url, "");
                if (!string.IsNullOrEmpty(result))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    CuCoupons r = js.Deserialize<CuCoupons>(result);
                    if (r != null && r.Ticket.Status == 1)
                    {
                        var model = SellPhoneOrderBLL.GetSellPhoneOrderByOrderNo(orderno);
                        totalfee = Convert.ToDecimal(model.OrderPrice - r.Ticket.Coupons.Price);
                        model.CouponsNo = couponsno;
                        model.CouponsUserID = r.Ticket.Coupons.UserID;
                        model.CouponsUserName = r.Ticket.Coupons.UserName;
                        model.CouponsPrice = r.Ticket.Coupons.Price;
                        //增加优惠券信息
                        SellPhoneOrderBLL.UpdateSellPhoneOrderCoupons(model);
                    }
                    else
                    {
                        return Content("<script>window.alert('优惠券码无效！');window.close();</script>");
                    }
                }
            }

            ////////////////////////////////////////////请求参数////////////////////////////////////////////

            //支付类型
            string payment_type = "1";
            //必填，不能修改
            //服务器异步通知页面路径
            string notify_url = "http://" + HttpContext.Request.Url.Host + "/Alipay/NotifyUrl";// "http://商户网关地址/create_direct_pay_by_user-CSHARP-UTF-8/notify_url.aspx";
            //需http://格式的完整路径，不能加?id=123这类自定义参数

            //页面跳转同步通知页面路径
            string return_url = "http://" + HttpContext.Request.Url.Host + "/Alipay/ReturnUrl"; //"http://商户网关地址/create_direct_pay_by_user-CSHARP-UTF-8/return_url.aspx";
            //需http://格式的完整路径，不能加?id=123这类自定义参数，不能写成http://localhost/

            //商户订单号
            string out_trade_no = orderno;// WIDout_trade_no.Text.Trim();
            //商户网站订单系统中唯一订单号，必填

            //订单名称
            string subject = " 万修二手交易中心";// WIDsubject.Text.Trim();
            //必填

            //付款金额
            string total_fee = totalfee.ToString();// WIDtotal_fee.Text.Trim();
            //必填

            //订单描述

            string body = "万修订单-订单号：" + orderno + ",订单金额:" + totalfee + "";// WIDbody.Text.Trim();
            //商品展示地址
            string show_url = "";// WIDshow_url.Text.Trim();
            //需以http://开头的完整路径，例如：http://www.商户网址.com/myorder.html

            //防钓鱼时间戳
            string anti_phishing_key = "";
            //若要使用请调用类文件submit中的query_timestamp函数

            //客户端的IP地址
            string exter_invoke_ip = "";
            //非局域网的外网IP地址，如：221.0.0.1

            ////////////////////////////////////////////////////////////////////////////////////////////////

            //把请求参数打包成数组
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
            sParaTemp.Add("partner", Config.Partner);
            sParaTemp.Add("seller_email", Config.Seller_email);
            sParaTemp.Add("_input_charset", Config.Input_charset.ToLower());
            sParaTemp.Add("service", "create_direct_pay_by_user");
            sParaTemp.Add("payment_type", payment_type);
            sParaTemp.Add("notify_url", notify_url);
            sParaTemp.Add("return_url", return_url);
            sParaTemp.Add("out_trade_no", out_trade_no);
            sParaTemp.Add("subject", subject);
            sParaTemp.Add("total_fee", total_fee);
            sParaTemp.Add("body", body);
            sParaTemp.Add("show_url", show_url);
            sParaTemp.Add("anti_phishing_key", anti_phishing_key);
            sParaTemp.Add("exter_invoke_ip", exter_invoke_ip);

            //建立请求
            string sHtmlText = Com.Alipay.Submit.BuildRequest(sParaTemp, "get", "确认");
            //Response.Write(sHtmlText);
            return Content(sHtmlText);
        }
        /// <summary>
        /// 功能：服务器异步通知页面
        /// 版本：3.3
        /// 日期：2012-07-10
        /// 说明：
        /// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
        /// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
        /// 
        /// ///////////////////页面功能说明///////////////////
        /// 创建该页面文件时，请留心该页面文件中无任何HTML代码及空格。
        /// 该页面不能在本机电脑测试，请到服务器上做测试。请确保外部可以访问该页面。
        /// 该页面调试工具请使用写文本函数logResult。
        /// 如果没有收到该页面返回的 success 信息，支付宝会在24小时内按一定的时间策略重发通知
        /// </summary>
        public ActionResult NotifyUrl()
        {
            SortedDictionary<string, string> sPara = GetRequestPost();

            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.Verify(sPara, Request.Form["notify_id"], Request.Form["sign"]);

                if (verifyResult)//验证成功
                {
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //请在这里加上商户的业务逻辑程序代码


                    //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                    //获取支付宝的通知返回参数，可参考技术文档中服务器异步通知参数列表

                    //商户订单号

                    string out_trade_no = Request.Form["out_trade_no"];

                    //支付宝交易号

                    string trade_no = Request.Form["trade_no"];

                    //交易状态
                    string trade_status = Request.Form["trade_status"];

                    if (Request.Form["trade_status"] == "WAIT_BUYER_PAY")//判断支付状态_等待买家付款（文档中有枚举表可以参考）
                    {
                        SellPhoneOrderBLL.UpdateOrderStatus(out_trade_no, 1);
                    }
                    else if (Request.Form["trade_status"] == "WAIT_SELLER_SEND_GOODS")//判断支付状态_买家付款成功,等待卖家发货（文档中有枚举表可以参考）   
                    {
                        //更新自己数据库的订单语句，请自己填写一下
                        string strOrderNO = Request.Form["out_trade_no"];//订单号
                        string strPrice = Request.Form["price"];//金额

                        SellPhoneOrderBLL.UpdateOrderStatus(out_trade_no, 2);
                    }
                    else if (Request.Form["trade_status"] == "WAIT_BUYER_CONFIRM_GOODS")//判断支付状态_卖家已发货等待买家确认（文档中有枚举表可以参考）   
                    {
                        //更新自己数据库的订单语句，请自己填写一下
                        string strOrderNO = Request.Form["out_trade_no"];//订单号
                        string strPrice = Request.Form["price"];//金额

                        SellPhoneOrderBLL.UpdateOrderStatus(out_trade_no, 3);
                    }
                    else if (Request.Form["trade_status"] == "TRADE_FINISHED")//判断支付状态_交易成功结束（文档中有枚举表可以参考）
                    {
                        //判断该笔订单是否在商户网站中已经做过处理
                        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                        //如果有做过处理，不执行商户的业务程序

                        //注意：
                        //退款日期超过可退款期限后（如三个月可退款），支付宝系统发送该交易状态通知
                        SellPhoneOrderBLL.UpdateOrderStatus(out_trade_no, 4);
                    }
                    else if (Request.Form["trade_status"] == "TRADE_SUCCESS")
                    {
                        //判断该笔订单是否在商户网站中已经做过处理
                        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                        //如果有做过处理，不执行商户的业务程序

                        //注意：
                        //付款完成后，支付宝系统发送该交易状态通知
                    }
                    else
                    {

                    }
                    //——请根据您的业务逻辑来编写程序（以上代码仅作参考）——
                    return Content("success");  //请不要修改或删除
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                }
                else//验证失败
                {
                    return Content("fail");
                }
            }
            else
            {
                return Content("无通知参数");
            }
        }
        public ActionResult aaa()
        {
            SellPhoneOrder model = null;
            model = new SellPhoneOrder();
            model.OrderNo = "20150818000006";
            model.BuyerID = "2088802586300930";
            model.BuyerEmail = "";
            model.Totalfee = Convert.ToDecimal(0.01);
            model.Body = "";
            model.TradeNo = "";
            model.NoticeDate = DateTime.Now;
            model.Status = 2;
            SellPhoneOrderBLL.UpdateOrderBuyInfo(model);
            return View();
        }
        /// <summary>
        /// 功能：页面跳转同步通知页面
        /// 版本：3.3
        /// 日期：2012-07-10
        /// 说明：
        /// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
        /// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
        /// 
        /// ///////////////////////页面功能说明///////////////////////
        /// 该页面可在本机电脑测试
        /// 可放入HTML等美化页面的代码、商户业务逻辑程序代码
        /// 该页面可以使用ASP.NET开发工具调试，也可以使用写文本函数LogResult进行调试
        /// </summary>
        public ActionResult ReturnUrl()
        {
            SortedDictionary<string, string> sPara = GetRequestGet();

            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.Verify(sPara, Request.QueryString["notify_id"], Request.QueryString["sign"]);
                SellPhoneOrder model = null;
                if (verifyResult)//验证成功
                {
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //请在这里加上商户的业务逻辑程序代码


                    //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                    //获取支付宝的通知返回参数，可参考技术文档中页面跳转同步通知参数列表

                    //商户订单号
                    string out_trade_no = Request.QueryString["out_trade_no"];

                    //支付宝交易号
                    string trade_no = Request.QueryString["trade_no"];

                    //交易状态
                    string trade_status = Request.QueryString["trade_status"];

                    string buyer_email = Request.QueryString["buyer_email"];
                    string buyer_id = Request.QueryString["buyer_id"];
                    string notify_time = Request.QueryString["notify_time"];
                    string total_fee = Request.QueryString["total_fee"];
                    string body = Request.QueryString["body"];
                    model = new SellPhoneOrder();
                    model.OrderNo = out_trade_no;
                    model.BuyerID = buyer_id;
                    model.BuyerEmail = buyer_email;
                    model.Totalfee = Convert.ToDecimal(total_fee);
                    model.Body = body;
                    model.TradeNo = trade_no;
                    model.NoticeDate = Convert.ToDateTime(notify_time);

                    if (Request.QueryString["trade_status"] == "TRADE_FINISHED" || Request.QueryString["trade_status"] == "TRADE_SUCCESS")
                    {
                        //判断该笔订单是否在商户网站中已经做过处理
                        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                        //如果有做过处理，不执行商户的业务程序

                        //2已付款,等待发货
                        model.Status = 2;
                        SellPhoneOrderBLL.UpdateOrderBuyInfo(model);

                        return RedirectToAction("PaySuccess", "Home", new { status = 1, orderno = out_trade_no, orderprice = model.Totalfee });

                    }
                    else
                    {
                        //1已下单,未付款
                        //model.Status = 1;
                        //SellPhoneOrderBLL.UpdateOrderBuyInfo(model);
                        return RedirectToAction("PaySuccess", "Home", new { status = 0, orderno = out_trade_no, orderprice = model.Totalfee });
                        //return Content("trade_status=" + Request.QueryString["trade_status"]);
                    }

                    //打印页面
                    return Content("验证成功<br />");

                    //——请根据您的业务逻辑来编写程序（以上代码仅作参考）——

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                }
                else//验证失败
                {
                    return RedirectToAction("PaySuccess", "Home", new { status = 0, orderno = "", orderprice = "" });
                    return Content("验证失败");
                }
            }
            else
            {
                return RedirectToAction("PaySuccess", "Home", new { status = 0, orderno = "", orderprice = "" });
                return Content("无返回参数");
            }
        }
        /// <summary>
        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public SortedDictionary<string, string> GetRequestPost()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.Form;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.Form[requestItem[i]]);
            }

            return sArray;
        }
        /// <summary>
        /// 获取支付宝GET过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public SortedDictionary<string, string> GetRequestGet()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.QueryString;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.QueryString[requestItem[i]]);
            }

            return sArray;
        }
    }
}
