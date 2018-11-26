using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace API_SERVER.Dao
{
    public  class SMSEMAILHandle
    {
        /// <summary>
        /// 发件邮件服务器的Smtp设置
        /// </summary>
        public string SendSetSmtp = System.Environment.GetEnvironmentVariable("SendSetSmtp");
        
        /// <summary>
        /// 发件人的邮件
        /// </summary>
        public string SendEmail = System.Environment.GetEnvironmentVariable("SendEmail");

        /// <summary>
        /// 发件人的邮件密码
        /// </summary>
        public string SendPwd = System.Environment.GetEnvironmentVariable("SendPwd");
        /// <summary>
        /// 发件邮件服务器的Smtp设置
        /// </summary>
        public string SMSKEY = System.Environment.GetEnvironmentVariable("SMSKEY");

        public bool MailSend(string ConsigneeAddress, string ConsigneeName, string ConsigneeHand,string ConsigneeTheme, string SendContent)
        {
            try
            {
                //确定smtp服务器端的地址，实列化一个客户端smtp 
                System.Net.Mail.SmtpClient sendSmtpClient = new System.Net.Mail.SmtpClient(SendSetSmtp);//发件人的邮件服务器地址
                //构造一个发件的人的地址
                System.Net.Mail.MailAddress sendMailAddress = new MailAddress(SendEmail, ConsigneeHand, Encoding.UTF8);//发件人的邮件地址和收件人的标题、编码

                //构造一个收件的人的地址
                System.Net.Mail.MailAddress consigneeMailAddress = new MailAddress(ConsigneeAddress, ConsigneeName, Encoding.UTF8);//收件人的邮件地址和收件人的名称 和编码

                //构造一个Email对象
                System.Net.Mail.MailMessage mailMessage = new MailMessage(sendMailAddress, consigneeMailAddress);//发件地址和收件地址
                mailMessage.Subject = ConsigneeTheme;//邮件的主题
                mailMessage.BodyEncoding = Encoding.UTF8;//编码
                mailMessage.SubjectEncoding = Encoding.UTF8;//编码
                mailMessage.Body = SendContent;//发件内容
                mailMessage.IsBodyHtml = false;//获取或者设置指定邮件正文是否为html

                //设置邮件信息 (指定如何处理待发的电子邮件)
                sendSmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//指定如何发邮件 是以网络来发
                sendSmtpClient.EnableSsl = false;//服务器支持安全接连，安全则为true

                sendSmtpClient.UseDefaultCredentials = false;//是否随着请求一起发

                //用户登录信息
                NetworkCredential myCredential = new NetworkCredential(SendEmail, SendPwd);
                sendSmtpClient.Credentials = myCredential;//登录
                sendSmtpClient.Send(mailMessage);//发邮件
                return true;//发送成功
            }
            catch (Exception)
            {
                return false;//发送失败
            }
        }

        public bool smsSend(string phoneNum,string code)
        {
            try
            {
                string _url = "http://v.juhe.cn/sms/send?mobile=" + phoneNum + "&tpl_id=68600&tpl_value=%23code%23%3d"
                    + code + "&key="+ SMSKEY;
                string method = "GET";
                HttpWebRequest request = WebRequest.Create(_url) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = method;
                request.ContentType = "text/html";
                request.Headers.Add("charset", "utf-8");

                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream responseStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(responseStream, Encoding.UTF8);
                //返回结果网页（html）代码
                string content = sr.ReadToEnd();
                return true;//发送成功
            }
            catch
            {
                return false;//发送失败
            }
            
        }

        /// <summary>
        /// 审核通过时的提示邮件
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool sendRegisterSuccess(string phoneNum)
        {
            try
            {
                string _url = "http://v.juhe.cn/sms/send?mobile=" + phoneNum + "&tpl_id=68761&key=" + SMSKEY;
                string method = "GET";
                HttpWebRequest request = WebRequest.Create(_url) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = method;
                request.ContentType = "text/html";
                request.Headers.Add("charset", "utf-8");

                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream responseStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(responseStream, Encoding.UTF8);
                //返回结果网页（html）代码
                string content = sr.ReadToEnd();
                return true;//发送成功
            }
            catch
            {
                return false;//发送失败
            }

        }
        /// <summary>
        /// 审核失败时的提示邮件
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool sendRegisterFail(string phoneNum)
        {
            try
            {
                string _url = "http://v.juhe.cn/sms/send?mobile=" + phoneNum + "&tpl_id=68762&key=" + SMSKEY;
                string method = "GET";
                HttpWebRequest request = WebRequest.Create(_url) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = method;
                request.ContentType = "text/html";
                request.Headers.Add("charset", "utf-8");

                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream responseStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(responseStream, Encoding.UTF8);
                //返回结果网页（html）代码
                string content = sr.ReadToEnd();
                return true;//发送成功
            }
            catch
            {
                return false;//发送失败
            }

        }
    }
}
