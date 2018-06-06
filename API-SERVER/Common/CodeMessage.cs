using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_SERVER.Common
{
    /// <summary>
    /// 返回信息对照
    /// </summary>
    public enum CodeMessage
    {
        OK = 0,
        PostNull = -1,


        NotFound = 404,
        InnerError = 500,

        SenparcCode = 1000,

        PaymentError = 3000,
        PaymentTotalPriceZero=3001,
        PaymentMsgError = 3002,

        InvalidToken = 4000,
        InvalidMethod = 4001,
        InvalidParam = 4002,

        GoodsNotFound=6001,

        InitOrderError=7000,
    }
}
