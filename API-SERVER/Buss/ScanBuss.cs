using API_SERVER.Common;
using API_SERVER.Dao;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Net;
using System;
using System.Text;
using System.Security.Cryptography;

namespace API_SERVER.Buss
{
    public class ScanBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.ScanApi;
        }
        public bool NeedCheckToken()
        {
            return true;
        }
        
        public object Do_SCAN(object param, string userId)
        {
            SCANParam scanParam = JsonConvert.DeserializeObject<SCANParam>(param.ToString());
            if (scanParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (scanParam.code == null || scanParam.code == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            WXParam wXParam = new WXParam();
            //获取ACCESS_TOKEN
            string _url = "http://console.llwell.net/llback/htmlpage.html?code="+ scanParam.code;
            //获取Ticket
            string _ticket = Requestjsapi_ticket(Request_Url());
            //获取ticket
            string _finalticket = _ticket;
            //获取noncestr
            string _noncestr = CreatenNonce_str();
            //获取timestamp
            long _timestamp = CreatenTimestamp();
            //获取sinature
            string _sinature = GetSignature(_finalticket, _noncestr, _timestamp, _url).ToLower();

            wXParam.appId = "wxfcedc4c4293d0b43";
            wXParam.timestamp = _timestamp.ToString();
            wXParam.noncestr = _noncestr;
            wXParam.signature = _sinature;
            return wXParam;
        }
        public object Do_SCANGOODSURL(object param,string userId)
        {
            SCANParam scanParam = JsonConvert.DeserializeObject<SCANParam>(param.ToString());
            if (scanParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (scanParam.code == null || scanParam.code == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (scanParam.barcode == null || scanParam.barcode == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            ScanDao scanDao = new ScanDao();
            return scanDao.getGoodsUrl(scanParam);
        }
        
        //获取AccessToken
        public static string Request_Url()
        {
            // 设置参数
            string _appid = "wxfcedc4c4293d0b43";
            string _appsecret = "ef45b4809cbffc5a457b903706096d8b";
            string _url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + _appid + "&secret=" + _appsecret;
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
            //由于微信服务器返回的JSON串中包含了很多信息，我们只需要将AccessToken获取就可以了，需要将JSON拆分
            string[] str = content.Split('"');
            content = str[3];
            return content;
        }

        //根据AccessToken来获取jsapi_ticket
        public static string Requestjsapi_ticket(string accesstoken)
        {

            string _accesstoken = accesstoken;
            string url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + _accesstoken + "&type=jsapi";
            string method = "GET";
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
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
            //同样，返回的JSON中只要取出ticket的信息即可
            string[] str = content.Split('"');
            content = str[9];
            return content;
        }

        //接下来就是辅助工具类，生成随机字符串
        #region 字符串随机 CreatenNonce_str()
        private static string[] strs = new string[]
                                    {
                                    "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z",
                                    "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z"
                                    };
        public static string CreatenNonce_str()
        {
            Random r = new Random();
            var sb = new StringBuilder();
            var length = strs.Length;
            for (int i = 0; i < 15; i++)
            {
                sb.Append(strs[r.Next(length - 1)]);
            }
            return sb.ToString();
        }
        #endregion

        //生成时间戳，备用
        #region 时间戳生成 CreatenTimestamp()
        public static long CreatenTimestamp()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }
        #endregion

        //获取签名，这里的三个参数分别为前面生成的ticket，随机字符串以及时间戳
        #region 获取签名 GetSignature()
        public static string GetSignature(string jsapi_ticket, string noncestr, long timestamp, string url)
        {

            var string1Builder = new StringBuilder();
            string1Builder.Append("jsapi_ticket=").Append(jsapi_ticket).Append("&")
                          .Append("noncestr=").Append(noncestr).Append("&")
                          .Append("timestamp=").Append(timestamp).Append("&")
                          .Append("url=").Append(url.IndexOf("#") >= 0 ? url.Substring(0, url.IndexOf("#")) : url);
            return SHA1(string1Builder.ToString());
        }
        #endregion


        //最后就是SHA1的加密算法工具
        #region 加密签名算法 SHA1(content)
        //加密签名算法
        public static string SHA1(string content)
        {
            return SHA1(content, Encoding.UTF8);

        }
        //加密签名
        public static string SHA1(string content, Encoding encode)
        {
            try
            {
                SHA1 sha1 = new SHA1CryptoServiceProvider();
                byte[] bytes_in = encode.GetBytes(content);
                byte[] bytes_out = sha1.ComputeHash(bytes_in);
                sha1.Dispose();
                string result = BitConverter.ToString(bytes_out);
                result = result.Replace("-", "");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("SHA1加密出错：" + ex.Message);
            }
        }  
        #endregion
    }
    public class WXParam
    {
        public string appId;
        public string timestamp;
        public string noncestr;
        public string signature;
    }
    public class SCANParam
    {
        public string code;
        public string barcode;
    }
}
