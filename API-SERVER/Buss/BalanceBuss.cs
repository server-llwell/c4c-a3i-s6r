using API_SERVER.Common;
using API_SERVER.Dao;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace API_SERVER.Buss
{
    public class BalanceBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.BalanceApi;
        }

        public bool NeedCheckToken()
        {
            return true;
        }
        public object Do_GetDistributionList(object param, string userId)
        {
            AgentParam agentParam = JsonConvert.DeserializeObject<AgentParam>(param.ToString());
            if (agentParam.pageSize == 0)
            {
                agentParam.pageSize = 10;
            }
            if (agentParam.current == 0)
            {
                agentParam.current = 1;
            }
            AgentDao agentDao = new AgentDao();
            return agentDao.getDistributionList(agentParam,userId);
        }

        public object Do_UpdateDistribution(object param, string userId)
        {
            MsgResult msg = new MsgResult();
            DistributionParam distributionParam = JsonConvert.DeserializeObject<DistributionParam>(param.ToString());
            if (distributionParam.userName == null || distributionParam.userName == "")
            {
                msg.msg = "缺少分销商名称";
            }
            if (distributionParam.mobile == null || distributionParam.mobile == "")
            {
                msg.msg += "缺少联系电话";
            }
            if (distributionParam.wxName == null || distributionParam.wxName == "")
            {
                msg.msg += "缺少微信昵称";
            }
            if (msg.msg!="")
            {
                return msg;
            }
            AgentDao agentDao = new AgentDao();
            if (distributionParam.id == null || distributionParam.id == "")
            {
                return agentDao.addDistribution(distributionParam, userId);
            }
            else
            {
                return agentDao.updateDistribution(distributionParam, userId);
            }

        }
        public object Do_GetAgentQRCode(object param, string userId)
        {
            userId = "547472506@qq.com";
            AgentDao agentDao = new AgentDao();
            return agentDao.getAgentQRCode(userId);
        }
    }
    public class SearchBalanceParam
    {
        public string[] OrderDate;//订单日期区间
        public string[] BalanceDate;//结算日期区间-发货时间
        public string merchantOrderId;//订单id
        public string purchaseCode;//销售商code
        public string platformId;//平台渠道
        public string supplierId;//供应商id
        public int current;//多少页
        public int pageSize;//页面显示多少个商品
    }

    public class BalanceTotalItem
    {
        public double total = 0;//总单数
        public double totalSales = 0;//总销量
        public double totalSupplier = 0;//供货总结算额-供货商的结算金额
        public double totalPurchase = 0;//佣金总结算额-采购代理分销的结算金额
        public double totalPlatform = 0;// 平台总提点额
    }
    public class BalanceItem
    {
        public string keyId;//序号
        public string id;//id
        public string merchantOrderId;//订单号
        public string tradeTime;//订单时间
        public double tradeAmount;//订单销售额 
        public string waybillTime;//结算时间（发货时间）
        public double supplier;//供货结算额 -供货的结算额
        public double purchase;//佣金结算额-采购代理分销的结算额
        public double platform;//平台提点额
        public string distribution;
    }
}






