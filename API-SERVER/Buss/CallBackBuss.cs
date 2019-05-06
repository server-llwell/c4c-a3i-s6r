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
            string appid="";
            string mch_id = "";
            string total_fee = "";
            string out_trade_no = "";
            string result_code = "";
            string transaction_id = "";
            string time_end = "";
            string openid = "";
            CallBackDao callBack = new CallBackDao();
            try
            {
                return_code = resHandler.GetParameter("return_code");
                return_msg = resHandler.GetParameter("return_msg");
                appid = resHandler.GetParameter("appid");
                mch_id = resHandler.GetParameter("mch_id");
                total_fee = resHandler.GetParameter("total_fee");//支付金额
                out_trade_no = resHandler.GetParameter("out_trade_no");//支付单号
                result_code = resHandler.GetParameter("result_code");//
                transaction_id = resHandler.GetParameter("transaction_id");//微信支付订单号
                time_end = resHandler.GetParameter("time_end");//支付完成时间
                openid = resHandler.GetParameter("openid");
                AccountFundDao accountFundDao = new AccountFundDao();
                accountFundDao.errLog("支付参数", return_code+","+ return_msg + "," + appid + "," + mch_id + "," + total_fee + "," + out_trade_no + "," + transaction_id + "," + time_end + "," + openid +"," + result_code+","+ resHandler.IsTenpaySign());


                //验证请求是否从微信发过来（安全）

                if (resHandler.IsTenpaySign() && return_code.ToUpper() == "SUCCESS")               
                {
                    /* 这里可以进行订单处理的逻辑 */
                    // transaction_id:微信支付单号
                    // out_trade_no:商城实际订单号
                    // openId:用户信息
                    // total_fee:实际支付价格
                    callBack.updateUserListForPay(Convert.ToDouble(total_fee), out_trade_no, transaction_id, openid, time_end);
                   
                    if (callBack.checkOrderTotalPrice(out_trade_no, Convert.ToDouble(total_fee), time_end, transaction_id, openid))
                    {
                        return_code = "SUCCESS";
                        return_msg = "OK";
                    }
                    else
                    {
                        callBack.insertPayLog(out_trade_no, transaction_id, Convert.ToDouble(total_fee), openid, time_end, "金额不符");
                        return_code = "FAIL";
                        return_msg = "金额不符";
                    }
                    
                }
                else
                {
                    callBack.insertPayLog(out_trade_no, transaction_id, Convert.ToDouble(total_fee), openid, time_end, "签名失败");
                    return_code = "FAIL";
                    return_msg = "签名失败";
                }
                return string.Format(@"<xml><return_code><![CDATA[{0}]]></return_code><return_msg><![CDATA[{1}]]></return_msg></xml>", return_code, return_msg);

            }
            catch (Exception ex)
            {
                AccountFundDao accountFundDao = new AccountFundDao();
                accountFundDao.errLog("支付回调错误", ex.StackTrace);
                return_code = "FAIL";
                return_msg = "catch错误";
                return string.Format(@"<xml><return_code><![CDATA[{0}]]></return_code><return_msg><![CDATA[{1}]]></return_msg></xml>", return_code, return_msg);
            }
            
        }
    }
}
