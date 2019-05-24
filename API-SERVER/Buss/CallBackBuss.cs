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
            string sign = "";
            CallBackDao callBack = new CallBackDao();
            WxpayStatus wxpayStatus = new WxpayStatus();
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
                string strtime_end = resHandler.GetParameter("time_end") ;//支付完成时间
                char[] strTime = strtime_end.ToCharArray();
                for (int i=0;i< strtime_end.Length;i++)
                {
                    if (i == 0)
                    {
                        time_end = strTime[i].ToString();
                    }
                    time_end += strTime[i].ToString();
                    if (i == 3)
                    {
                        time_end += "-";
                    }
                    else if(i == 5)
                    {
                        time_end += "-";
                    }
                    else if (i == 7)
                    {
                        time_end += " ";
                    }
                    else if (i == 9)
                    {
                        time_end += ":";
                    }
                    else if (i == 11)
                    {
                        time_end += ":";
                    }
                }
                
                openid = resHandler.GetParameter("openid");
                sign= resHandler.GetParameter("sign");

                //验证请求是否从微信发过来（安全）
                resHandler.SetKey(Global.APIKEY);
                if (resHandler.IsTenpaySign() && return_code.ToUpper() == "SUCCESS")               
                {
                    /* 这里可以进行订单处理的逻辑 */
                    // transaction_id:微信支付单号
                    // out_trade_no:商城实际订单号
                    // openId:用户信息
                    // total_fee:实际支付价格
                                       
                    if (callBack.checkOrderTotalPrice(out_trade_no, Convert.ToDouble(total_fee), time_end, transaction_id, openid))
                    {
                        callBack.updateUserListForPay(Convert.ToDouble(total_fee), out_trade_no, transaction_id, openid, time_end);
                        return_code = "SUCCESS";
                        return_msg = "OK";                       
                        wxpayStatus.fundId = out_trade_no;
                        wxpayStatus.type = 1;
                        wxpayStatus.msg = "支付成功";
                        Utils.SetCache(wxpayStatus.fundId,wxpayStatus,0,0,15);
                    }
                    else
                    {
                        callBack.insertPayLog(out_trade_no, transaction_id, Convert.ToDouble(total_fee), openid, time_end, "金额不符");
                        return_code = "FAIL";
                        return_msg = "金额不符";
                        wxpayStatus.fundId = out_trade_no;
                        wxpayStatus.msg = "支付金额不符";
                        Utils.SetCache(wxpayStatus.fundId, wxpayStatus, 0, 0, 15);
                    }
                    
                }
                else
                {
                    callBack.insertPayLog(out_trade_no, transaction_id, Convert.ToDouble(total_fee), openid, time_end, "签名失败");
                    return_code = "FAIL";
                    return_msg = "签名失败";
                    wxpayStatus.fundId = out_trade_no;
                    wxpayStatus.msg = "支付签名失败";
                    Utils.SetCache(wxpayStatus.fundId, wxpayStatus, 0, 0, 15);
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
    public class WxpayStatus
    {
        public string fundId;//支付单号
        public string msg;//返回信息
        public int type=0;//0支付失败，1支付成功
    }
}
