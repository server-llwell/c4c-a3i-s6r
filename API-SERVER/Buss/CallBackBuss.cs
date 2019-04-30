using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_SERVER.Common;
using API_SERVER.Dao;
using Senparc.Weixin.MP.TenPayLibV3;

namespace API_SERVER.Buss
{
    public class CallBackBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.CallBackApi;
        }

        public bool NeedCheckToken()
        {
            return true;
        }

        public string PaymentCallBack(ResponseHandler resHandler)
        {
            string return_code = "";
            string return_msg = "";
            try
            {
                return_code = resHandler.GetParameter("return_code");
                return_msg = resHandler.GetParameter("return_msg");
                string appid = resHandler.GetParameter("appid");
                string mch_id = resHandler.GetParameter("mch_id");
                string total_fee = resHandler.GetParameter("total_fee");//支付金额
                string out_trade_no = resHandler.GetParameter("out_trade_no");//支付单号
                string result_code = resHandler.GetParameter("result_code");//
                string transaction_id = resHandler.GetParameter("transaction_id");//微信支付订单号
                string time_end = resHandler.GetParameter("time_end");//支付完成时间
                string openid = resHandler.GetParameter("openid");

                Console.WriteLine();
                Console.WriteLine("return_code:" + return_code);
                Console.WriteLine("out_trade_no:" + out_trade_no);
                Console.WriteLine("transaction_id:" + transaction_id);
                Console.WriteLine("total_fee:" + total_fee);
                Console.WriteLine("time_end:" + time_end);
                Console.WriteLine("result_code:" + result_code);
                Console.WriteLine("------------------------------------------");

                CallBackDao callBack = new CallBackDao();
                
                //验证请求是否从微信发过来（安全）
                if (resHandler.IsTenpaySign() && return_code.ToUpper() == "SUCCESS")               
                {
                    /* 这里可以进行订单处理的逻辑 */
                    // transaction_id:微信支付单号
                    // out_trade_no:商城实际订单号
                    // openId:用户信息
                    // total_fee:实际支付价格

                    if (callBack.checkOrderTotalPrice(out_trade_no, Convert.ToDouble(total_fee), time_end, transaction_id, openid))
                    {
                        return_code = "SUCCESS";
                        return_msg = "OK";
                    }
                    else
                    {
                        return_code = "FAIL";
                        return_msg = "金额不符";
                    }
                    
                }
                else
                {
                    return_code = "FAIL";
                    return_msg = "签名失败";
                }
                return string.Format(@"<xml><return_code><![CDATA[{0}]]></return_code><return_msg><![CDATA[{1}]]></return_msg></xml>", return_code, return_msg);

            }
            catch (Exception ex)
            {
                return_code = "FAIL";
                return_msg = "签名失败";
                return string.Format(@"<xml><return_code><![CDATA[{0}]]></return_code><return_msg><![CDATA[{1}]]></return_msg></xml>", return_code, return_msg);
            }
            
        }
    }
}
