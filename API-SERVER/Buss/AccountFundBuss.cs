using API_SERVER.Common;
using API_SERVER.Dao;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace API_SERVER.Buss
{
    public class AccountFundBuss : IBuss
    {
        private string path = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "upload");
        public const string SIGN_TYPE_MD5 = "MD5";
        public const string SIGN_TYPE_HMAC_SHA256 = "";
        private static string USER_AGENT = string.Format("WXPaySDK/{3} ({0}) .net/{1} {2}", Environment.OSVersion, Environment.Version, Global.MCHID, typeof(AccountFundBuss).Assembly.GetName().Version);
        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //直接确认，否则打不开    
            return true;
        }

        public ApiType GetApiType()
        {
            return ApiType.AccountFundApi;
        }

        public bool NeedCheckToken()
        {
            return true;
        }

        /// <summary>
        /// 获取用户余额信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_GetRetailMoney(object param, string userId)
        {
            GetRetailMoneyParam getRetailMoneyParam = JsonConvert.DeserializeObject<GetRetailMoneyParam>(param.ToString());
            if (getRetailMoneyParam==null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (getRetailMoneyParam.pageSize == 0)
            {
                getRetailMoneyParam.pageSize = 10;
            }
            if (getRetailMoneyParam.current == 0)
            {
                getRetailMoneyParam.current = 1;
            }
            AccountFundDao accountFundDao = new AccountFundDao();

            return accountFundDao.GetRetailMoney(getRetailMoneyParam,userId);
        }

        

        /// <summary>
        /// 用户充值
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_RetailRecharge(object param, string userId)
        {
            RetailRechargeParam retailRechargeParam = JsonConvert.DeserializeObject<RetailRechargeParam>(param.ToString());
            RetailRechargeItem item = new RetailRechargeItem();
            AccountFundDao accountFundDao = new AccountFundDao();
           
            if (retailRechargeParam.totalPrice < 100)
            {
                item.msg = "充值金额不能小于100";
                return item;
            }
            
            string time = "";
            var out_trade_no = "";
            int totalPrice = 0;
            string url = "";
            try
            {                                                          
                time = DateTime.Now.ToString("yyyyMMddhhmmssff");
                out_trade_no = userId + time;//充值单号

                totalPrice = Convert.ToInt32(retailRechargeParam.totalPrice * 100);//总金额                
                if (totalPrice == 0)
                {
                    throw new ApiException(CodeMessage.PaymentTotalPriceZero, "PaymentTotalPriceZero");
                }
                AccountFundBuss date = new AccountFundBuss();
                date.SetValue("body", "流连优选-会员充值");//商品描述            
                date.SetValue("out_trade_no", out_trade_no);//充值单号
                date.SetValue("total_fee", totalPrice);//总金额
                date.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));//交易起始时间
                date.SetValue("time_expire", DateTime.Now.AddHours(1).ToString("yyyyMMddHHmmss"));//交易结束时间           
                date.SetValue("trade_type", "NATIVE");//交易类型
                                                      //SetValue("product_id", productId);//商品ID
                                                      //SetValue("attach", "test");//附加数据
                                                      //SetValue("goods_tag", "jjj");//订单优惠标记

                AccountFundBuss result = AccountFundBuss.UnifiedOrder(date);//调用统一下单接口

                object o = null;
                result.m_values.TryGetValue("code_url", out o);
                url = o.ToString();//获得统一下单接口返回的二维码链接

                //if (!Directory.Exists(path))
                //{
                //    Directory.CreateDirectory(path);
                //}
                //QRCodeGenerator generator = new QRCodeGenerator();
                //QRCodeData codeData = generator.CreateQrCode(url, QRCodeGenerator.ECCLevel.M, true);
                //QRCoder.QRCode qrcode = new QRCoder.QRCode(codeData);

                //Bitmap qrImage = qrcode.GetGraphic(5, Color.Black, Color.White, true);
                //qrImage.Save(Path.Combine(path, out_trade_no + ".jpg"), System.Drawing.Imaging.ImageFormat.Jpeg);
                //CreateCodeEwm(url, Path.Combine(path, out_trade_no + ".jpg"));
                //FileManager fileManager = new FileManager();
                //fileManager.updateFileToOSS(out_trade_no + ".jpg", Global.OssDirOrder, userId + ".jpg");


                //string url2 = Global.OssUrl + Global.OssDirOrder + userId + ".jpg";

                if (accountFundDao.RetailRecharge(out_trade_no, totalPrice, time, userId, url))
                {
                    item.type = 1;
                    item.msg = out_trade_no;
                    item.url = url;
                }
                else
                {
                    item.msg = "数据库错误，请联系客服";
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.StackTrace);
                accountFundDao.errLog(userId, e.StackTrace);
                throw e;
                //item.msg = e.ToString();
                //accountFundDao.RetailRechargeLog(item.msg, "生成二维码失败，请联系客服");
                ////item.msg = "生成二维码失败，请联系客服";
                //return item;
            }
            //try
            //{

            //}
            //catch(Exception e)
            //{
            //    item.msg = e.ToString();
            //    accountFundDao.RetailRechargeLog(item.msg, "二维码上传失败，请联系客服");
            //    //item.msg = "二维码上传失败，请联系客服";
            //    return item;
            //}

            return item;
        }

       



        //采用排序的Dictionary的好处是方便对数据包进行签名，不用再签名之前再做一次排序
        private SortedDictionary<string, object> m_values = new SortedDictionary<string, object>();

        /**
        * 设置某个字段的值
        * @param key 字段名
         * @param value 字段值
        */
        public void SetValue(string key, object value)
        {
            m_values[key] = value;
        }

       
        /**
       * 
       * 统一下单
       * @param WxPaydata inputObj 提交给统一下单API的参数
       * @param int timeOut 超时时间
       * @throws WxPayException
       * @return 成功时返回，其他抛异常
       */
        public static AccountFundBuss UnifiedOrder(AccountFundBuss inputObj, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
            //检测必填参数
            if (!inputObj.IsSet("out_trade_no"))
            {
                throw new ApiException(CodeMessage.InvalidParam, "缺少统一支付接口必填参数out_trade_no！");
            }
            else if (!inputObj.IsSet("body"))
            {
                throw new ApiException(CodeMessage.InvalidParam, "缺少统一支付接口必填参数body！");
            }
            else if (!inputObj.IsSet("total_fee"))
            {
                throw new ApiException(CodeMessage.InvalidParam, "缺少统一支付接口必填参数total_fee！");
            }
            else if (!inputObj.IsSet("trade_type"))
            {
                throw new ApiException(CodeMessage.InvalidParam, "缺少统一支付接口必填参数trade_type！");
            }

            //关联参数
            //if (inputObj.GetValue("trade_type").ToString() == "JSAPI" && !inputObj.IsSet("openid"))
            //{
            //    throw new ApiException(CodeMessage.InvalidParam, "统一支付接口中，缺少必填参数openid！trade_type为JSAPI时，openid为必填参数！");
            //}
            //if (inputObj.GetValue("trade_type").ToString() == "NATIVE" )
            //{
            //    throw new ApiException(CodeMessage.InvalidParam, "统一支付接口中，缺少必填参数product_id！trade_type为JSAPI时，product_id为必填参数！");
            //}

            //异步通知url未设置，则使用配置文件中的url
            if (!inputObj.IsSet("notify_url"))
            {
                inputObj.SetValue("notify_url", Global.NotifyUrl);//异步通知url
            }
            var ran = new Random();//随机数
            var nonce_str = string.Format("{0}{1}{2}", Global.MCHID, DateTime.Now.ToString("yyyyMMddHHmmss"), ran.Next(999));

            inputObj.SetValue("appid", Global.WXAPI);//公众账号ID
            inputObj.SetValue("mch_id", Global.MCHID);//商户号
            inputObj.SetValue("spbill_create_ip", "127.0.0.1");//终端ip	  	    
            inputObj.SetValue("nonce_str", nonce_str);//随机字符串
            //inputObj.SetValue("sign_type", WxPayData.SIGN_TYPE_HMAC_SHA256);//签名类型

            //签名
            string sign = inputObj.MakeSign(SIGN_TYPE_MD5);
            inputObj.SetValue("sign", sign);

            string xml = inputObj.ToXml();

            var start = DateTime.Now;

            string response = Post(xml, url, false, timeOut);

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);

            AccountFundBuss result = new AccountFundBuss();
            result.FromXml(response);

            return result;
        }

        


        /**
       * @将xml转为Data对象并返回对象内部的数据
       * @param string 待转换的xml串
       * @return 经转换得到的Dictionary
       *
       */
        public SortedDictionary<string, object> FromXml(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                throw new ApiException(CodeMessage.InvalidParam, "将空的xml串转换为Data不合法!");
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNode xmlNode = xmlDoc.FirstChild;//获取到根节点<xml>
            XmlNodeList nodes = xmlNode.ChildNodes;
            foreach (XmlNode xn in nodes)
            {
                XmlElement xe = (XmlElement)xn;
                m_values[xe.Name] = xe.InnerText;//获取xml的键值对到Data内部的数据中
            }

            try
            {
                //2015-06-29 错误是没有签名
                if (m_values["return_code"].ToString() != "SUCCESS")
                {
                    return m_values;
                }
                CheckSign(SIGN_TYPE_MD5);//验证签名,不通过会抛异常
            }
            catch (Exception ex)
            {
                throw new ApiException(CodeMessage.InvalidParam, ex.Message);
            }

            return m_values;
        }

        /**
        * 
        * 检测签名是否正确
        * 正确返回true，错误抛异常
        */
        public bool CheckSign(string signType)
        {
            //如果没有设置签名，则跳过检测
            if (!IsSet("sign"))
            {
                throw new ApiException(CodeMessage.InvalidParam, "WxPayData签名存在但不合法!");
            }
            //如果设置了签名但是签名为空，则抛异常
            else if (GetValue("sign") == null || GetValue("sign").ToString() == "")
            {
                throw new ApiException(CodeMessage.InvalidParam, "WxPayData签名存在但不合法!");
            }

            //获取接收到的签名
            string return_sign = GetValue("sign").ToString();

            //在本地计算新的签名
            string cal_sign = MakeSign(signType);

            if (cal_sign == return_sign)
            {
                return true;
            }

            throw new ApiException(CodeMessage.InvalidParam, "WxPayData签名验证错误!");
        }


        public static string Post(string xml, string url, bool isUseCert, int timeout)
        {
            System.GC.Collect();//垃圾回收，回收没有正常关闭的http连接

            string result = "";//返回结果

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream reqStream = null;

            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;
                //设置https验证方式
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                            new RemoteCertificateValidationCallback(CheckValidationResult);
                }

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = USER_AGENT;
                request.Method = "POST";
                request.Timeout = timeout * 1000;

                //设置代理服务器
                //WebProxy proxy = new WebProxy();                          //定义一个网关对象
                //proxy.Address = new Uri(WxPayConfig.PROXY_URL);              //网关服务器端口:端口
                //request.Proxy = proxy;

                //设置POST的数据类型和长度
                request.ContentType = "text/xml";
                byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
                request.ContentLength = data.Length;

                //是否使用证书
                //if (isUseCert)
                //{
                //    string path = HttpContext.Current.Request.PhysicalApplicationPath;
                //    X509Certificate2 cert = new X509Certificate2(path + WxPayConfig.GetConfig().GetSSlCertPath(), WxPayConfig.GetConfig().GetSSlCertPassword());
                //    request.ClientCertificates.Add(cert);
                //    Log.Debug("WxPayApi", "PostXml used cert");
                //}

                //往服务器写入数据
                reqStream = request.GetRequestStream();
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();

                //获取服务端返回
                response = (HttpWebResponse)request.GetResponse();

                //获取服务端返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = sr.ReadToEnd().Trim();
                sr.Close();
            }
            catch (System.Threading.ThreadAbortException e)
            {
                System.Threading.Thread.ResetAbort();
            }
            catch (WebException e)
            {
                throw new ApiException(CodeMessage.InvalidParam, e.ToString());
            }
            catch (Exception e)
            {
                throw new ApiException(CodeMessage.InvalidParam, e.ToString());
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }

        /**
         * 判断某个字段是否已设置
         * @param key 字段名
         * @return 若字段key已被设置，则返回true，否则返回false
         */
        public bool IsSet(string key)
        {
            object o = null;
            m_values.TryGetValue(key, out o);
            if (null != o)
                return true;
            else
                return false;
        }

        /**
        * 根据字段名获取某个字段的值
        * @param key 字段名
         * @return key对应的字段值
        */
        public object GetValue(string key)
        {
            object o = null;
            m_values.TryGetValue(key, out o);
            return o;
        }

        /**
        * @生成签名，详见签名生成算法
        * @return 签名, sign字段不参加签名
        */
        public string MakeSign(string signType)
        {
            //转url格式
            string str = ToUrl();
            //在string后加入API KEY
            str += "&key=" + Global.APIKEY;
            if (signType == SIGN_TYPE_MD5)
            {
                var md5 = MD5.Create();
                var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
                var sb = new StringBuilder();
                foreach (byte b in bs)
                {
                    sb.Append(b.ToString("x2"));
                }
                //所有字符转为大写
                return sb.ToString().ToUpper();
            }            
            else
            {
                throw new ApiException(CodeMessage.PaymentTotalPriceZero, "sign_type 不合法");
            }
        }

        

        /**
       * @Dictionary格式转化成url参数格式
       * @ return url格式串, 该串不包含sign字段值
       */
        public string ToUrl()
        {
            string buff = "";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                if (pair.Value == null)
                {                   
                    throw new ApiException(CodeMessage.PaymentTotalPriceZero, "Data内部含有值为null的字段!");
                }

                if (pair.Key != "sign" && pair.Value.ToString() != "")
                {
                    buff += pair.Key + "=" + pair.Value + "&";
                }
            }
            buff = buff.Trim('&');
            return buff;
        }

        /**
      * @将Dictionary转成xml
      * @return 经转换得到的xml串
      * @throws WxPayException
      **/
        public string ToXml()
        {
            //数据为空时不能转化为xml格式
            if (0 == m_values.Count)
            {
                throw new ApiException(CodeMessage.PaymentTotalPriceZero, "Data数据为空!");
            }

            string xml = "<xml>";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                //字段值不能为null，会影响后续流程
                if (pair.Value == null)
                {
                    throw new ApiException(CodeMessage.PaymentTotalPriceZero, "Data内部含有值为null的字段!");
                }

                if (pair.Value.GetType() == typeof(int))
                {
                    xml += "<" + pair.Key + ">" + pair.Value + "</" + pair.Key + ">";
                }
                else if (pair.Value.GetType() == typeof(string))
                {
                    xml += "<" + pair.Key + ">" + "<![CDATA[" + pair.Value + "]]></" + pair.Key + ">";
                }
                else//除了string和int类型不能含有其他数据类型
                {
                    throw new ApiException(CodeMessage.PaymentTotalPriceZero, "Data字段数据类型错误!");
                }
            }
            xml += "</xml>";
            return xml;
        }


    }

    

    public class RetailRechargeParam
    {
        public double totalPrice;//充值金额
    }

    public class GetRetailMoneyParam
    {
        public string fundId;//支付单号
        public string fundtype;//类别：1：充值，2：订单扣款
        public string[] dateTime;//时间段
        public string orderId;//订单号
        public int pageSize;//个数
        public int current;//页数
    }

    public class GetRetailMoneyItem
    {
        public int  keyId;//序号
        public string fundId;//支付订单号
        public string fundtype;//类别：1：充值，2：订单扣款
        public string fundprice;//订单金额
        public string newfund;//历史余额
        public string payid;//微信支付订单号
        public string paytime;//支付时间
        public string orderId;//商品订单号   
    }

    public class GetRetailFundItem
    {
        public string fund;//余额
    }

    public class RetailRechargeItem
    {
        public int type=0;//0失败,1成功
        public string msg;//信息
        public string url;//二维码地址
    }

}
