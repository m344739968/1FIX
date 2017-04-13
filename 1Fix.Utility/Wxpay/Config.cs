﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Configuration;

namespace WxPayAPI
{
    /**
     * 配置账号信息
     * appid微信公众号: wx8497a7ca9271f52c
`    * mchid商户ID: 1226726002
     * key商户支付秘钥Key: wanxiu12345678912345678912345678
     * appsecret: 82cde402531267cfba0d302b3265ee68 
    */
    public class WxPayConfig
    {
        //=======【基本信息设置】=====================================
        /* 微信公众号信息配置
        * APPID：绑定支付的APPID（必须配置）
        * MCHID：商户号（必须配置）
        * KEY：商户支付密钥，参考开户邮件设置（必须配置）
        * APPSECRET：公众帐号secert（仅JSAPI支付的时候需要配置）
        */
        public static string APPID = WebConfigurationManager.AppSettings["WxPayAppID"]; //"wx2428e34e0e7dc6ef";
        public static string MCHID = WebConfigurationManager.AppSettings["WxPayMchID"]; //"1233410002";
        public static string KEY = WebConfigurationManager.AppSettings["WxPayKey"]; //"e10adc3849ba56abbe56e056f20f883e";
        public static string APPSECRET = WebConfigurationManager.AppSettings["WxPayAppSecret"]; //"51c56b886b5be869567dd389b3e5d1d6";

        //=======【证书路径设置】===================================== 
        /* 证书路径,注意应该填写绝对路径（仅退款、撤销订单时需要）
        */
        public static string SSLCERT_PATH = "cert/apiclient_cert.p12";
        public static string SSLCERT_PASSWORD = "1233410002";



        //=======【支付结果通知url】===================================== 
        /* 支付结果通知回调url，用于商户接收支付结果
        */
        public static string NOTIFY_URL = "http://" + HttpContext.Current.Request.Url.Host + "/WxPay/NotifyUrl";// "http://paysdk.weixin.qq.com/example/ResultNotifyPage.aspx";

        //=======【商户系统后台机器IP】===================================== 
        /* 此参数可手动配置也可在程序中自动获取
        */
        public static string IP = HttpContext.Current.Request.UserHostAddress;// "8.8.8.8";


        //=======【代理服务器设置】===================================
        /* 默认IP和端口号分别为0.0.0.0和0，此时不开启代理（如有需要才设置）
        */
        public const string PROXY_URL = "http://10.152.18.220:8080";

        //=======【上报信息配置】===================================
        /* 测速上报等级，0.关闭上报; 1.仅错误时上报; 2.全量上报
        */
        public const int REPORT_LEVENL = 1;

        //=======【日志级别】===================================
        /* 日志等级，0.不输出日志；1.只输出错误信息; 2.输出错误和正常信息; 3.输出错误信息、正常信息和调试信息
        */
        public const int LOG_LEVENL = 3;
    }
}