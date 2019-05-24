using API_SERVER.Buss;
using API_SERVER.Common;
using Com.ACBC.Framework.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_SERVER.Dao
{
    public class AccountFundDao
    {
        public AccountFundDao()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }

        /// <summary>
        /// 获取用户余额信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PageResult GetRetailMoney(GetRetailMoneyParam getRetailMoneyParam, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(getRetailMoneyParam.current, getRetailMoneyParam.pageSize);
            pageResult.list = new List<object>();
            string st = "";
            if (getRetailMoneyParam.fundtype!=null && getRetailMoneyParam.fundtype!="")
            {
                string fundtype = (getRetailMoneyParam.fundtype== "充值" ? "1" :"2");
                st += " and b.fundtype='"+ fundtype + "'";
            }
            if (getRetailMoneyParam.fundId != null && getRetailMoneyParam.fundId != "")
            {               
                st += " and b.fundId like '%" + getRetailMoneyParam.fundId + "%'";
            }
            if (getRetailMoneyParam.dateTime != null && getRetailMoneyParam.dateTime.Length == 2)
            {
                st += " and b.paytime between '" + getRetailMoneyParam.dateTime[0] + "' and '" + getRetailMoneyParam.dateTime[1] + "'";
            }
            if (getRetailMoneyParam.orderId != null && getRetailMoneyParam.orderId != "")
            {
                st += " and b.orderId like '%"+ getRetailMoneyParam.orderId + "%'";
            }
           
            string selectRetailMoney = ""
                + "select a.fund,b.fundId,b.fundtype,b.fundprice,b.newfund,b.payid,b.paytime,b.orderId "
                + " from t_user_list a , t_user_fund b "
                + " where a.usercode=b.usercode and b.status='1' and  a.usercode='" + userId + "'" + st
                + " order by paytime desc";
            DataTable dtselectRetailMoney = DatabaseOperationWeb.ExecuteSelectDS(selectRetailMoney, "T").Tables[0];
            GetRetailFundItem fundItem = new GetRetailFundItem();
            fundItem.fund = "0";
            if (dtselectRetailMoney.Rows.Count>0)
            {               
                fundItem.fund= dtselectRetailMoney.Rows[0]["fund"].ToString();
                     
                for (int i= (getRetailMoneyParam.current-1)* getRetailMoneyParam.pageSize; i< dtselectRetailMoney.Rows.Count && i< (getRetailMoneyParam.current* getRetailMoneyParam.pageSize); i++)
                {
                    GetRetailMoneyItem getRetailMoneyItem = new GetRetailMoneyItem();
                    getRetailMoneyItem.keyId = i+1;
                    getRetailMoneyItem.fundId = dtselectRetailMoney.Rows[i]["fundId"].ToString();
                    getRetailMoneyItem.fundtype = (dtselectRetailMoney.Rows[i]["fundtype"].ToString()=="1"?"充值":"订单扣款") ;
                    getRetailMoneyItem.fundprice = dtselectRetailMoney.Rows[i]["fundprice"].ToString();
                    getRetailMoneyItem.newfund = dtselectRetailMoney.Rows[i]["newfund"].ToString();
                    getRetailMoneyItem.payid = dtselectRetailMoney.Rows[i]["payid"].ToString();
                    getRetailMoneyItem.paytime = dtselectRetailMoney.Rows[i]["paytime"].ToString();
                    getRetailMoneyItem.orderId = dtselectRetailMoney.Rows[i]["orderId"].ToString();
                    pageResult.list.Add(getRetailMoneyItem);
                }
                
            }
            pageResult.item = fundItem;
            pageResult.pagination.total = dtselectRetailMoney.Rows.Count;
            return pageResult;
        }

        /// <summary>
        /// 获取用户余额信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool RetailRecharge(string out_trade_no,int  totalPrice,string time, string userId,string url)
        {
            double price =Math.Round(Convert.ToDouble(totalPrice / 100) ,2) ;
            StringBuilder updateBuilder = new StringBuilder();
            updateBuilder.AppendFormat("insert into t_user_fund(fundId,usercode,createTime,fundtype,fundprice,status,inputUser) "
                + " values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", out_trade_no, userId, time,"1", price, "0", userId);
            string update = updateBuilder.ToString();
            if (DatabaseOperationWeb.ExecuteDML(update))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// 生成二维码错误日志
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool RetailRechargeLog(string errLog, string code)
        {
            string time = DateTime.Now.ToString("yyyyMMddhhmmss");
            StringBuilder insertBuilder = new StringBuilder();
            insertBuilder.AppendFormat("insert into t_log_error(code,errLog)"
                +  " values('{0}','{1}')", code, errLog);
            string insert = insertBuilder.ToString();
            if (DatabaseOperationWeb.ExecuteDML(insert))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void errLog(string code, string errLog)
        {
            string sql = "insert into t_log_error(code,errLog) values('"+ code + "','"+ errLog + "')";
            DatabaseOperationWeb.ExecuteDML(sql);
        }

        

    }
}
