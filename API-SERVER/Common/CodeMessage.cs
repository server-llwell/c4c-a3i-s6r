﻿using System;
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
        AccountOrPasswordIsIncorrect =3,

        NotFound = 404,
        InnerError = 500,

        SenparcCode = 1000,

        PaymentError = 3000,
        PaymentTotalPriceZero=3001,
        PaymentMsgError = 3002,

        InvalidToken = 4000,
        InvalidMethod = 4001,
        InvalidParam = 4002,
        InterfaceRole = 4003,//接口权限不足
        InterfaceValueError = 4004,//接口的参数不对
        InterfaceDBError = 4005,//接口数据库操作失败
        SecurityKeyNull = 4006,//加密参数为空
        SignError = 4007,//签名错误


        OrderNull = 5001,
        GoodsNotFound = 6001,

        InitOrderError =7000,
    }
}
