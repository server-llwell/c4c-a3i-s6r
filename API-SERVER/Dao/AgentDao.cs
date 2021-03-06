﻿using API_SERVER.Buss;
using API_SERVER.Common;
using Com.ACBC.Framework.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace API_SERVER.Dao
{
    public class AgentDao
    {
        public AgentDao()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }

        public PageResult getDistributionList(AgentParam agentParam, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(agentParam.current, agentParam.pageSize);
            pageResult.list = new List<Object>();
            string st = "";
            if (agentParam.search!=null &&agentParam.search!="")
            {
                st = " and ( nickName like '%"+ agentParam.search + "%' or username like '%"+ agentParam.search + "%')" ;
            }
            string sql = "select * from v_eshop_user where ofAgent='" + userId+"' "+st +
                         " ORDER BY id desc LIMIT " + (agentParam.current - 1) * agentParam.pageSize + "," + agentParam.pageSize ;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                string sql1 = "SELECT count(*) FROM v_eshop_user where ofAgent='" + userId + "' " + st;

                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "Table").Tables[0];
                pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DistributionParam distributionParam = new DistributionParam();
                    distributionParam.keyId = Convert.ToString((agentParam.current - 1) * agentParam.pageSize + i + 1);
                    distributionParam.id = dt.Rows[i]["id"].ToString();
                    distributionParam.agentCode = dt.Rows[i]["userCode"].ToString();
                    distributionParam.userName = dt.Rows[i]["userName"].ToString();
                    distributionParam.img = dt.Rows[i]["img"].ToString();
                    distributionParam.wxName = dt.Rows[i]["nickname"].ToString();
                    pageResult.list.Add(distributionParam);
                }
            }
            return pageResult;
        }

        public PageResult getDistributionOrderList(AgentParam agentParam, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(agentParam.current, agentParam.pageSize);
            pageResult.item = agentParam.agentCode;
            pageResult.list = new List<Object>();
            string sql = "select o.parentOrderId,o.tradeTime,o.tradeAmount,s.statusName,sum(g.profitDealer) price from t_base_status s,t_order_list o,t_order_goods g " +
                "where s.statusId=o.status and o.parentOrderId=g.merchantOrderId and  o.distributionCode='"+ agentParam.agentCode + "' and o.purchaserCode='"+ userId + "' " +
                " GROUP BY o.parentOrderId" +
                " ORDER BY o.tradeTime desc  ";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int i = (agentParam.current - 1) * agentParam.pageSize; i < dt.Rows.Count && i < agentParam.current * agentParam.pageSize; i++)
                {
                    getDistributionOrderListItem gDOI = new getDistributionOrderListItem();
                    gDOI.keyId = i + 1;
                    gDOI.orderId = dt.Rows[i]["parentOrderId"].ToString();
                    gDOI.status= dt.Rows[i]["statusName"].ToString();
                    gDOI.tradeAmount = dt.Rows[i]["tradeAmount"].ToString();
                    gDOI.tradeTime = dt.Rows[i]["tradeTime"].ToString();
                    gDOI.agentPrice= dt.Rows[i]["price"].ToString();
                    pageResult.list.Add(gDOI);
                }               
            }
            pageResult.pagination.total = dt.Rows.Count;
            return pageResult;
        }

        public MsgResult addDistribution(DistributionParam distributionParam,string userId)
        {
            MsgResult msg = new MsgResult();
            string sql = "insert into t_agent_distribution(agentCode,userName,company,mobile,wxName,createTime) " +
                         "values('" + userId + "','" + distributionParam.userName + "','" + distributionParam.company 
                         + "','" + distributionParam.mobile + "','" + distributionParam.wxName + "',now())";
            if (DatabaseOperationWeb.ExecuteDML(sql))
            {
                msg.msg = "添加成功";
                msg.type = "1";
            }
            else
            {
                msg.msg = "数据库添加失败";
            }
            return msg;
        }


        public MsgResult updateDistribution(DistributionParam distributionParam, string userId)
        {
            MsgResult msg = new MsgResult();
            string sql = "select * from t_agent_distribution where agentCode= '" + userId + "' and id =" + distributionParam.id;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["flag"].ToString()=="1"&& dt.Rows[0]["wxName"].ToString() != distributionParam.wxName)
                {
                    msg.msg = "已审核的分销商不能修改微信昵称！";
                }
                else
                {
                    string upSql = "update t_agent_distribution set userName='" + distributionParam.userName + "'," +
                                   "company='" + distributionParam.company + "'," +
                                   "mobile='" + distributionParam.mobile + "'," +
                                   "wxName='" + distributionParam.wxName + "' " +
                                   "where id  =" + distributionParam.id;
                    if (DatabaseOperationWeb.ExecuteDML(upSql))
                    {
                        msg.msg = "修改成功";
                        msg.type = "1";
                    }
                    else
                    {
                        msg.msg = "数据库修改失败";
                    }
                }
            }
            else
            {
                msg.msg = "未找到对应数据";
            }
            
            return msg;
        }

        public AgentQRCode getAgentQRCode(string userId)
        {
            AgentQRCode agentQRCode = new AgentQRCode();
            string sql = "select qrcoder from t_user_list where usercode = '" + userId + "'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                agentQRCode.agentQRCodeUrl = dt.Rows[0]["qrcoder"].ToString();
            }
            else
            {
                agentQRCode.agentQRCodeUrl = "";
            }
            return agentQRCode;
        }
    }
}
