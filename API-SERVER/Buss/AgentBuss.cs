﻿using API_SERVER.Common;
using API_SERVER.Dao;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace API_SERVER.Buss
{
    public class AgentBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.AgentApi;
        }

        public object Do_GetDistributionList(object param, string userId)
        {
            userId = "547472506@qq.com";
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
            userId = "547472506@qq.com";
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
    }
    public class AgentParam
    {
        public string search;
        public int current;//多少页
        public int pageSize;//页面显示多少个商品
    }
    public class DistributionParam
    {
        public string keyId;//序号
        public string id;//id
        public string agentCode;//代理用户code
        public string userName;//分销商名称
        public string company;//分销商公司 
        public string mobile;//电话
        public string wxName;//微信昵称
        public string createTime;//创建时间
        public string flag;//状态：0新增，1处理完成
    }
}





