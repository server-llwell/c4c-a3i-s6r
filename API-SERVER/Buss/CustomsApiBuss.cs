using API_SERVER.Common;
using API_SERVER.Dao;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Http;
using System;

namespace API_SERVER.Buss
{
    public class CustomsApiBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.CustomsApiApi;
        }

        public bool NeedCheckToken()
        {
            return false;
        }
        public object Do_platDataOpen(object param, string userId)
        {
            Console.WriteLine(param);
            PlatReturnData platReturnData = new PlatReturnData();
            Dictionary<string, string> dic = (Dictionary<string, string>)param;
            Console.WriteLine(dic);
            if (dic.Count>0)
            {
                if (dic.ContainsKey("openReq"))
                {
                    PlatDataParam p = JsonConvert.DeserializeObject<PlatDataParam>(dic["openReq"]);

                    CustomsDao customsDao = new CustomsDao();
                    return customsDao.getPlatData(p);
                }
                else
                {
                    platReturnData.code = "99";
                    platReturnData.message = "参数错误，未找到“openReq”！";
                    platReturnData.serviceTime = 100;
                    return platReturnData;
                }
            }
            else
            {
                platReturnData.code = "98";
                platReturnData.message = "参数为空";
                platReturnData.serviceTime = 100;
                return platReturnData;
            }

            

        }
    }
}






