using _1Fix.BLL;
using _1Fix.Entity;
using _1Fix.Utility;
using _1Fix.Utility.Wxpay;
using LitJson;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ThoughtWorks.QRCode.Codec;
using WxPayAPI;

namespace _1Fix.Shop.Controllers
{
    public class WxPayController : Controller
    {
        //
        // GET: /WxPay/
        /// <summary>
        /// 微信支付页面
        /// </summary>
        /// <param name="orderno"></param>
        /// <returns></returns>
        public ActionResult Index(string orderno, decimal totalfee)
        {
            ViewBag.OrderNo = orderno;
            ViewBag.TotalFee = totalfee;
            Log.Info(this.GetType().ToString(), "Native pay mode 2 url is producing...");
            WxPayData data = new WxPayData();
            data.SetValue("body", orderno);//商品描述
            data.SetValue("attach", "");//附加数据
            data.SetValue("out_trade_no", orderno);//随机字符串
            data.SetValue("total_fee", Convert.ToInt32(totalfee * 100));//总金额(默认单位分) 元转分
            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));//交易起始时间
            data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));//交易结束时间
            data.SetValue("goods_tag", orderno);//商品标记
            data.SetValue("trade_type", "NATIVE");//交易类型
            data.SetValue("product_id", orderno);//商品ID或订单号
            WxPayData result = WxPayApi.UnifiedOrder(data);//调用统一下单接口
            if (result.GetValue("return_code").ToString() == "SUCCESS")
            {
                if (result.GetValue("result_code").ToString() == "SUCCESS")
                {
                    string url = result.GetValue("code_url").ToString();//获得统一下单接口返回的二维码链接

                    ViewBag.URL = HttpUtility.UrlEncode(url);

                    Log.Info(this.GetType().ToString(), "Get native pay mode 2 url : " + url);
                }
            }
            else
            {
                var msg = result.GetValue("return_msg").ToString();
                //生成二维码失败
                return Content("<script>window.alert('" + msg + "');location.href='/';</script>");
            }

            return View();
        }
        /// <summary>
        /// 生成微信支付二维码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public void GenerateWxPayQrCode(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                //初始化二维码生成工具
                QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
                qrCodeEncoder.QRCodeVersion = 0;
                qrCodeEncoder.QRCodeScale = 4;

                //将字符串生成二维码图片
                Bitmap image = qrCodeEncoder.Encode(data, Encoding.Default);

                //保存为PNG到内存流  
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, ImageFormat.Png);
                    //return File(ms.GetBuffer(), "image/jpg");
                    Response.ClearContent();
                    Response.ContentType = "image/png";
                    Response.BinaryWrite(ms.ToArray());
                }
            }
            else
            {

            }
        }
        /// <summary>
        /// 微信支付完后通知页面(异步)
        /// </summary>
        /// <returns></returns>
        public ActionResult NotifyUrl()
        {
            var uploadPath = Server.MapPath("~/WxPayLog");
            //验证文件夹是否存在
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            SellPhoneOrder model = null;
            try
            {
                Notify notify = new Notify();
                WxPayData notifyData = notify.GetNotifyData(Request.InputStream);

                string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                string name = DateTime.Now.ToString("yyyyMMddHHmmssfff") + fileName.Substring(fileName.LastIndexOf("."));
                uploadPath = Path.Combine(uploadPath, name);

                using (var fs = new FileStream(uploadPath, FileMode.Create))
                {
                    //获得字节数组
                    byte[] data = System.Text.Encoding.Default.GetBytes(notifyData.ToXml());
                    fs.Write(data, 0, data.Length);
                }
                if (notifyData != null && notifyData.IsSet("return_code"))
                {
                    if (notifyData.GetValue("return_code").ToString() == "SUCCESS")
                    {
                        if (notifyData.GetValue("result_code").ToString() == "SUCCESS")
                        {
                            //1.验证商户订单号是否被处理
                            //2.处理过直接返回成功，否则返回
                            //此处根据out_trade_no 处理业务数据

                            //  <xml>
                            //  <appid><![CDATA[wx8497a7ca9271f52c]]></appid>
                            //  <bank_type><![CDATA[CFT]]></bank_type>
                            //  <cash_fee><![CDATA[1]]></cash_fee>
                            //  <fee_type><![CDATA[CNY]]></fee_type>
                            //  <is_subscribe><![CDATA[Y]]></is_subscribe>
                            //  <mch_id><![CDATA[1226726002]]></mch_id>
                            //  <nonce_str><![CDATA[c15c693398c84e29a23190b9cff795d3]]></nonce_str>
                            //  <openid><![CDATA[oWp3gsgYUF1CCKCSG45EiViF8400]]></openid>
                            //  <out_trade_no><![CDATA[20150831181310]]></out_trade_no>
                            //  <result_code><![CDATA[SUCCESS]]></result_code>
                            //  <return_code><![CDATA[SUCCESS]]></return_code>
                            //  <sign><![CDATA[2CF6FE30ECDE0B6D80B641D8B15FEB5C]]></sign>
                            //  <time_end><![CDATA[20150831181325]]></time_end>
                            //  <total_fee><![CDATA[1]]></total_fee>
                            //  <trade_type><![CDATA[NATIVE]]></trade_type>
                            //  <transaction_id><![CDATA[1009160702201508310755621208]]></transaction_id>
                            //</xml>

                            //处理业务数据结束
                            string trade_no = notifyData.GetValue("out_trade_no").ToString();//商户订单号
                            string appid = notifyData.GetValue("appid").ToString();
                            string bank_type = notifyData.GetValue("bank_type").ToString();
                            //string cash_fee = notifyData.GetValue("cash_fee").ToString();
                            string fee_type = notifyData.GetValue("fee_type").ToString();
                            string is_subscribe = notifyData.GetValue("is_subscribe").ToString();
                            string mch_id = notifyData.GetValue("mch_id").ToString();
                            string openid = notifyData.GetValue("openid").ToString();
                            string time_end = notifyData.GetValue("time_end").ToString();
                            string total_fee = notifyData.GetValue("total_fee").ToString();
                            string trade_type = notifyData.GetValue("trade_type").ToString();
                            string transaction_id = notifyData.GetValue("transaction_id").ToString();

                            model = new SellPhoneOrder();
                            //model.NoticeDate = Convert.ToDateTime(notify_time);
                            var query = SellPhoneOrderBLL.GetSellPhoneOrderByOrderNo(trade_no);
                            if (query != null && query.Status == 1)
                            {
                                //已下单,未付款的订单才更新状态，避免重复更新
                                SellPhoneOrderBLL.UpdateOrderStatus(trade_no, 2);
                                WxPayInfo wxpayinfo = new WxPayInfo();
                                wxpayinfo.OrderNo = trade_no;
                                wxpayinfo.appid = appid;
                                wxpayinfo.mchid = mch_id;
                                wxpayinfo.openid = openid;
                                wxpayinfo.issubscribe = is_subscribe;
                                wxpayinfo.tradetype = trade_type;
                                wxpayinfo.banktype = bank_type;
                                wxpayinfo.feetype = fee_type;
                                wxpayinfo.totalfee = Convert.ToDecimal(total_fee) / 100;
                                wxpayinfo.transactionid = transaction_id;
                                wxpayinfo.timeend = time_end;
                                //新增订单的微信支付信息
                                WxPayInfoBLL.AddWxPayInfo(wxpayinfo);

                                WxPayData res = new WxPayData();
                                res.SetValue("return_code", "SUCCESS");
                                res.SetValue("return_msg", "OK");
                                Log.Info(this.GetType().ToString(), "msg : " + res.ToXml());
                                Response.Write(res.ToXml());
                                Response.End();
                            }
                            else
                            {
                                WxPayData res = new WxPayData();
                                res.SetValue("return_code", "FAIL");
                                res.SetValue("return_msg", "失败");
                                Log.Info(this.GetType().ToString(), "msg : " + res.ToXml());
                                Response.Write(res.ToXml());
                                Response.End();
                            }
                        }
                        else
                        {
                            WxPayData res = new WxPayData();
                            res.SetValue("return_code", "FAIL");
                            res.SetValue("return_msg", "失败");
                            Log.Info(this.GetType().ToString(), "msg : " + res.ToXml());
                            Response.Write(res.ToXml());
                            Response.End();
                        }
                    }
                    if (notifyData.GetValue("return_code").ToString() == "FAIL")
                    {
                        WxPayData res = new WxPayData();
                        res.SetValue("return_code", "FAIL");
                        res.SetValue("return_msg", "失败");
                        Log.Info(this.GetType().ToString(), "msg : " + res.ToXml());
                        Response.Write(res.ToXml());
                        Response.End();
                    }
                }

                ////检查支付结果中transaction_id是否存在
                //if (!notifyData.IsSet("transaction_id"))
                //{
                //    //若transaction_id不存在，则立即返回结果给微信支付后台
                //    WxPayData res = new WxPayData();
                //    res.SetValue("return_code", "FAIL");
                //    res.SetValue("return_msg", "支付结果中微信订单号不存在");
                //    Log.Error(this.GetType().ToString(), "The Pay result is error : " + res.ToXml());
                //    Response.Write(res.ToXml());
                //    Response.End();
                //}

                //string transaction_id = notifyData.GetValue("transaction_id").ToString();

                ////查询订单，判断订单真实性
                //if (!QueryOrder(transaction_id))
                //{
                //    //若订单查询失败，则立即返回结果给微信支付后台
                //    WxPayData res = new WxPayData();
                //    res.SetValue("return_code", "FAIL");
                //    res.SetValue("return_msg", "订单查询失败");
                //    Log.Error(this.GetType().ToString(), "Order query failure : " + res.ToXml());
                //    Response.Write(res.ToXml());
                //    Response.End();
                //}
                ////查询订单成功
                //else
                //{
                //    WxPayData res = new WxPayData();
                //    res.SetValue("return_code", "SUCCESS");
                //    res.SetValue("return_msg", "OK");
                //    Log.Info(this.GetType().ToString(), "order query success : " + res.ToXml());
                //    Response.Write(res.ToXml());
                //    Response.End();
                //}
            }
            catch (Exception e)
            {
                string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                string name = DateTime.Now.ToString("yyyyMMddHHmmssfff") + fileName.Substring(fileName.LastIndexOf("."));
                uploadPath = Path.Combine(uploadPath, name);

                using (var fs = new FileStream(uploadPath, FileMode.Create))
                {
                    //获得字节数组
                    byte[] data = System.Text.Encoding.Default.GetBytes(e.ToString());
                    fs.Write(data, 0, data.Length);
                }
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "失败");
                Log.Info(this.GetType().ToString(), "msg : " + res.ToXml());
                Response.Write(res.ToXml());
                Response.End();
            }
            return View();
        }
        /// <summary>
        /// 微信支付成功后跳转页面
        /// </summary>
        /// <returns></returns>
        public ActionResult PaySuccess()
        {
            DateTime now = DateTime.Now;
            Session["1FixPaySuccessTempWxPay"] = now.ToString();
            return View();
        }
        //查询订单状态
        public JsonResult QueryOrder(string orderno)
        {
            int result = 0;
            string msg = string.Empty;
            var query = SellPhoneOrderBLL.GetSellPhoneOrderByOrderNo(orderno);
            if (query != null && query.Status == 2)
            {
                result = 1;
                msg = "已支付";
            }
            else
            {
                result = 0;
                msg = "未支付";
            }
            return Json(new { status = result, msg = query.Totalfee, });
        }


        /// <summary>
        /// 测试微信支付
        /// </summary>
        /// <returns></returns>
        public static string wxJsApiParam { get; set; } //H5调起JS API参数

        public static string wxEditAddrParam { get; set; }
        public ActionResult Test()
        {
            //Log.Info(this.GetType().ToString(), "Native pay mode 2 url is producing...");
            //WxPayData data = new WxPayData();
            //data.SetValue("body", DateTime.Now.ToString("yyyyMMddHHmmss"));//商品描述
            //data.SetValue("attach", "");//附加数据
            //data.SetValue("out_trade_no", DateTime.Now.ToString("yyyyMMddHHmmss"));//随机字符串
            //data.SetValue("total_fee", 1);//总金额(默认单位分) 元转分
            //data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));//交易起始时间
            //data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));//交易结束时间
            //data.SetValue("goods_tag", DateTime.Now.ToString("yyyyMMddHHmmss"));//商品标记
            //data.SetValue("trade_type", "NATIVE");//交易类型
            //data.SetValue("product_id", DateTime.Now.ToString("yyyyMMddHHmmss"));//商品ID或订单号
            //WxPayData result = WxPayApi.UnifiedOrder(data);//调用统一下单接口
            //if (result.GetValue("result_code").ToString() == "SUCCESS")
            //{
            //    string url = result.GetValue("code_url").ToString();//获得统一下单接口返回的二维码链接

            //    ViewBag.URL = HttpUtility.UrlEncode(url);

            //    Log.Info(this.GetType().ToString(), "Get native pay mode 2 url : " + url);
            //}
            //else
            //{
            //    var msg = result.GetValue("err_code_des").ToString();
            //    //生成二维码失败
            //    return Content("<script>window.alert('" + msg + "');location.href='/';</script>");
            //}

            //string xml = "<xml><appid><![CDATA[wx8497a7ca9271f52c]]></appid><bank_type><![CDATA[CFT]]></bank_type><cash_fee><![CDATA[1]]></cash_fee><fee_type><![CDATA[CNY]]></fee_type><is_subscribe><![CDATA[Y]]></is_subscribe><mch_id><![CDATA[1226726002]]></mch_id><nonce_str><![CDATA[8c192cc78460464db6e6dcb1791cb784]]></nonce_str><openid><![CDATA[oWp3gsgYUF1CCKCSG45EiViF8400]]></openid><out_trade_no><![CDATA[20150901091741]]></out_trade_no><result_code><![CDATA[SUCCESS]]></result_code><return_code><![CDATA[SUCCESS]]></return_code><sign><![CDATA[ADDFE0CA1990A9FC75A596F4A025F719]]></sign><time_end><![CDATA[20150901091823]]></time_end><total_fee><![CDATA[1]]></total_fee><trade_type><![CDATA[NATIVE]]></trade_type><transaction_id><![CDATA[1009160702201509010759137225]]></transaction_id></xml>";
            //HttpRequestHelper.HttpPost("http://localhost:8014/WxPay/NotifyUrl", xml);


            JsApiPay jsApiPay = new JsApiPay(this.HttpContext);
            try
            {
                //调用【网页授权获取用户信息】接口获取用户的openid和access_token
                jsApiPay.GetOpenidAndAccessToken();

                //获取收货地址js函数入口参数
                wxEditAddrParam = jsApiPay.GetEditAddressParameters();
                ViewBag.openid = jsApiPay.openid;
                ViewBag.wxEditAddrParam = wxEditAddrParam;
            }
            catch (Exception ex)
            {
                Response.Write("<span style='color:#FF0000;font-size:20px'>" + "页面加载出错，请重试" + "</span>");
            }
            return View();
        }
        public ActionResult TestJsWxPay()
        {
            //string openid =  Request.QueryString["openid"];
            //string total_fee = Request.QueryString["total_fee"];
            string openid = "oJA3Zwre-ZMZ22hmv4IWGwQLLyO0";
            int total_fee = 1;
            //检测是否给当前页面传递了相关参数
            if (string.IsNullOrEmpty(openid) || string.IsNullOrEmpty(total_fee.ToString()))
            {
                Response.Write("<span style='color:#FF0000;font-size:20px'>" + "页面传参出错,请返回重试" + "</span>");
                Log.Error(this.GetType().ToString(), "This page have not get params, cannot be inited, exit...");
                return View();
            }

            //若传递了相关参数，则调统一下单接口，获得后续相关接口的入口参数
            JsApiPay jsApiPay = new JsApiPay(this.HttpContext);
            jsApiPay.openid = openid;
            jsApiPay.total_fee = total_fee;

            //JSAPI支付预处理
            try
            {
                WxPayData unifiedOrderResult = jsApiPay.GetUnifiedOrderResult();
                wxJsApiParam = jsApiPay.GetJsApiParameters();//获取H5调起JS API参数                    
                Log.Debug(this.GetType().ToString(), "wxJsApiParam : " + wxJsApiParam);
                //在页面上显示订单信息
                Response.Write("<span style='color:#00CD00;font-size:20px'>订单详情：</span><br/>");
                Response.Write("<span style='color:#00CD00;font-size:20px'>" + unifiedOrderResult.ToPrintStr() + "</span>");

            }
            catch (Exception ex)
            {
                Log.Debug(this.GetType().ToString(), "wxJsApiParam : " + wxJsApiParam);
                Response.Write("<span style='color:#FF0000;font-size:20px'>" + "下单失败，请返回重试" + ex.ToString() + "</span>");
            }
            return View();
        }
    }
}
