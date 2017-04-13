using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace _1Fix.Utility
{
    public class MailHelper
    {
        /// <summary>
        /// 异步发送邮件
        /// </summary>
        /// <param name="msgFrom">发送者邮件</param>
        /// <param name="msgTo">接收者邮件</param>
        /// <param name="msgSubject">邮件主题</param>
        /// <param name="msgBody">邮件主体内容</param>
        /// <param name="UserName">提供身份验证的用户名</param>
        /// <param name="Password">提供身份验证的密码</param>
        /// <param name="Port">SMTP服务器端口</param>
        /// <param name="SmtpHost">SMTP服务器（如：smtp.163.com; smtp.qq.com; smtp.gmail.com）</param>
        public static bool AsyncSendEmail(String msgFrom, String[] msgTo, String msgSubject, String msgBody, String UserName, String Password, int Port, String SmtpHost, String Domain)
        {
            MailMessage mailMsg = new MailMessage();
            mailMsg.From = new MailAddress(msgFrom);
            foreach (var str in msgTo)
            {
                mailMsg.To.Add(str);
            }
            mailMsg.Subject = msgSubject;
            mailMsg.Body = msgBody;
            mailMsg.BodyEncoding = Encoding.UTF8;
            mailMsg.IsBodyHtml = true;
            mailMsg.Priority = MailPriority.High;

            SmtpClient smtp = new SmtpClient();
            smtp.Credentials = new NetworkCredential(UserName, Password, Domain);
            smtp.Port = Port;
            smtp.Host = SmtpHost;

            smtp.EnableSsl = false;
            smtp.SendCompleted += new SendCompletedEventHandler(smtp_SendCompleted);
            try
            {
                smtp.SendAsync(mailMsg, mailMsg);
                return true; //邮件提交成功（发送成功与否不可知）
            }
            catch //(SmtpException ex)
            {
                //邮件发送异常日志

                return false; //邮件提交失败
            }
        }
        /// <summary>
        /// 作者：Leo_ml
        /// 时间：2014.05.13
        /// 描述： 非异步发送邮件
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="msgFrom">发送者邮件</param>
        /// <param name="msgTo">接收者邮件</param>
        /// <param name="msgSubject">邮件主题</param>
        /// <param name="msgBody">邮件主体内容</param>
        /// <param name="UserName">提供身份验证的用户名</param>
        /// <param name="Password">提供身份验证的密码</param>
        /// <param name="Port">SMTP服务器端口</param>
        /// <param name="SmtpHost">SMTP服务器（如：smtp.163.com; smtp.qq.com; smtp.gmail.com）</param>
        public static bool SendEmail(String msgFrom, String[] msgTo, String msgSubject, String msgBody, String UserName, String Password, int Port, String SmtpHost, String Domain)
        {
            MailMessage mailMsg = new MailMessage();
            mailMsg.From = new MailAddress(msgFrom);
            foreach (var str in msgTo)
            {
                mailMsg.To.Add(str);
            }
            mailMsg.Subject = msgSubject;
            mailMsg.Body = msgBody;
            mailMsg.BodyEncoding = Encoding.UTF8;
            mailMsg.IsBodyHtml = true;
            mailMsg.Priority = MailPriority.High;

            SmtpClient smtp = new SmtpClient();
            smtp.Credentials = new NetworkCredential(UserName, Password, Domain);
            smtp.Port = Port;
            smtp.Host = SmtpHost;

            smtp.EnableSsl = false;
            smtp.SendCompleted += new SendCompletedEventHandler(smtp_SendCompleted);
            try
            {
                smtp.Send(mailMsg);
                return true; //邮件提交成功（发送成功与否不可知）
            }
            catch //(SmtpException ex)
            {
                //邮件发送异常日志

                return false; //邮件提交失败
            }
        }
        /// <summary>
        /// 邮件发送完成之后处理（发送日志）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void smtp_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //发送的邮件对像信息（便于日志信息记录）
            //MailMessage mailMsg = (MailMessage)e.UserState;
            //string subject = mailMsg.Subject;

            if (e.Cancelled)
            {
                //邮件被取消发送
            }
            if (e.Error != null)
            {
                //邮件发送异常
            }
            else
            {
                //邮件发送完成
            }
        }
    }
}
