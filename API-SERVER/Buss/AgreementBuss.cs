using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_SERVER.Common;
using Newtonsoft.Json;
using API_SERVER.Dao;

namespace API_SERVER.Buss
{
    public class AgreementBuss:IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.AgreementApi;
        }

        public bool NeedCheckToken()
        {
            return true;
        }

        /// <summary>
        /// 获取合同信息接口-代销
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_ContractInformation(object param, string userId)
        {
           
            if (userId == null)
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }

            AgreementDao agreementDao = new AgreementDao();
            return agreementDao.ContractInformation(userId);
        }

        public class ContractInformationItem
        {
            public string contractCode;//合同编号
            public string cycle;//结算周期
            public string model;//合作模式
        }

        public class ContractInformationlist
        {
            public string imgUrl;//附件地址
        }
    }
}
