using API_SERVER.Buss;
using API_SERVER.Common;
using Com.ACBC.Framework.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API_SERVER.Dao
{
    public class BalanceDao
    {
        public BalanceDao()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }
        #region 旧结算表

        /// <summary>
        /// 获取运营的结算列表
        /// </summary>
        /// <param name="searchBalanceParam"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PageResult getBalanceListByOperator(SearchBalanceParam searchBalanceParam, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(searchBalanceParam.current, searchBalanceParam.pageSize);
            pageResult.list = new List<Object>();
            string st = "";
            if (searchBalanceParam.OrderDate != null && searchBalanceParam.OrderDate.Length == 2)
            {
                st += " and o.tradeTime BETWEEN str_to_date('" + searchBalanceParam.OrderDate[0] + "', '%Y-%m-%d') " +
                            "AND DATE_ADD(str_to_date('" + searchBalanceParam.OrderDate[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            if (searchBalanceParam.BalanceDate != null && searchBalanceParam.BalanceDate.Length == 2)
            {
                st += " and a.createTime BETWEEN str_to_date('" + searchBalanceParam.BalanceDate[0] + "', '%Y-%m-%d') " +
                            "AND DATE_ADD(str_to_date('" + searchBalanceParam.BalanceDate[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            if (searchBalanceParam.merchantOrderId != null && searchBalanceParam.merchantOrderId.Trim() != "")
            {
                st += " and o.merchantOrderId like '%" + searchBalanceParam.merchantOrderId.Trim() + "%' ";
            }
            if (searchBalanceParam.purchaseCode != null && searchBalanceParam.purchaseCode.Trim() != "")
            {
                st += " and o.purchaserCode = '" + searchBalanceParam.purchaseCode.Trim() + "' ";
            }
            if (searchBalanceParam.platformId != null && searchBalanceParam.platformId.Trim() != "")
            {
                st += " and o.platformId = '" + searchBalanceParam.platformId.Trim() + "' ";
            }
            if (searchBalanceParam.supplierId != null && searchBalanceParam.supplierId.Trim() != "")
            {
                UserDao userDao = new UserDao();
                string code = userDao.getUserCode(searchBalanceParam.supplierId);
                if (code != null && code != "")
                {
                    st += " and o.customerCode = '" + code + "' ";
                }
            }
            string sql = "select a.id, a.merchantOrderId,o.tradeTime,o.tradeAmount,a.createTime,a.supplyPrice,a.profitAgent,a.profitDealer,a.platformPrice " +
                         "from t_order_list o ,t_account_analysis a " +
                         "where o.merchantOrderId = a.merchantOrderId " + st +
                         " ORDER BY o.id desc LIMIT " + (searchBalanceParam.current - 1) * searchBalanceParam.pageSize + "," + searchBalanceParam.pageSize;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                BalanceTotalItem balanceTotalItem = new BalanceTotalItem();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BalanceItem balanceItem = new BalanceItem();
                    balanceItem.keyId = Convert.ToString((searchBalanceParam.current - 1) * searchBalanceParam.pageSize + i + 1);
                    balanceItem.id = dt.Rows[i]["id"].ToString();
                    balanceItem.merchantOrderId = dt.Rows[i]["agentCode"].ToString();
                    balanceItem.tradeTime = dt.Rows[i]["tradeTime"].ToString();
                    balanceItem.tradeAmount = Convert.ToDouble(dt.Rows[i]["tradeAmount"]);
                    balanceItem.waybillTime = dt.Rows[i]["createTime"].ToString();
                    balanceItem.supplier = Convert.ToDouble(dt.Rows[i]["supplyPrice"]);
                    balanceItem.purchase = Convert.ToDouble(dt.Rows[i]["profitAgent"]) + Convert.ToDouble(dt.Rows[i]["profitDealer"]);
                    balanceItem.platform = Convert.ToDouble(dt.Rows[i]["platformPrice"]);
                    //balanceItem.distribution = dt.Rows[i]["distribution"].ToString();
                    pageResult.list.Add(balanceItem);
                }
                string sql1 = "SELECT o.tradeAmount,a.createTime,a.supplyPrice,a.profitAgent,a.profitDealer,a.platformPrice " +
                              "where o.merchantOrderId = a.merchantOrderId " + st;
                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "Table").Tables[0];
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    balanceTotalItem.totalSales += Convert.ToDouble(dt1.Rows[i]["tradeAmount"]);
                    balanceTotalItem.totalSupplier += Convert.ToDouble(dt1.Rows[i]["supplyPrice"]);
                    balanceTotalItem.totalPurchase += Convert.ToDouble(dt1.Rows[i]["profitAgent"]) + Convert.ToDouble(dt.Rows[i]["profitDealer"]);
                    balanceTotalItem.totalPlatform += Convert.ToDouble(dt1.Rows[i]["platformPrice"]);
                }
                balanceTotalItem.total = dt1.Rows.Count;
                pageResult.item = balanceTotalItem;
            }
            return pageResult;
        }


        /// <summary>
        /// 获取供应商的结算列表
        /// </summary>
        /// <param name="searchBalanceParam"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PageResult getBalanceListBySupplier(SearchBalanceParam searchBalanceParam, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(searchBalanceParam.current, searchBalanceParam.pageSize);
            pageResult.list = new List<Object>();
            string st = " and o.customerCode = '" + userId + "' ";
            if (searchBalanceParam.OrderDate != null && searchBalanceParam.OrderDate.Length == 2)
            {
                st += " and o.tradeTime BETWEEN str_to_date('" + searchBalanceParam.OrderDate[0] + "', '%Y-%m-%d') " +
                            "AND DATE_ADD(str_to_date('" + searchBalanceParam.OrderDate[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            if (searchBalanceParam.BalanceDate != null && searchBalanceParam.BalanceDate.Length == 2)
            {
                st += " and a.createTime BETWEEN str_to_date('" + searchBalanceParam.BalanceDate[0] + "', '%Y-%m-%d') " +
                            "AND DATE_ADD(str_to_date('" + searchBalanceParam.BalanceDate[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            if (searchBalanceParam.merchantOrderId != null && searchBalanceParam.merchantOrderId.Trim() != "")
            {
                st += " and o.merchantOrderId like '%" + searchBalanceParam.merchantOrderId.Trim() + "%' ";
            }
            if (searchBalanceParam.purchaseCode != null && searchBalanceParam.purchaseCode.Trim() != "")
            {
                st += " and o.purchaserCode = '" + searchBalanceParam.purchaseCode.Trim() + "' ";
            }
            if (searchBalanceParam.platformId != null && searchBalanceParam.platformId.Trim() != "")
            {
                st += " and o.platformId = '" + searchBalanceParam.platformId.Trim() + "' ";
            }

            string sql = "select a.id, a.merchantOrderId,o.tradeTime,a.createTime,a.supplyPrice " +
                         "from t_order_list o ,t_account_analysis a " +
                         "where o.merchantOrderId = a.merchantOrderId " + st +
                         " ORDER BY o.id desc LIMIT " + (searchBalanceParam.current - 1) * searchBalanceParam.pageSize + "," + searchBalanceParam.pageSize;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                BalanceTotalItem balanceTotalItem = new BalanceTotalItem();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BalanceItem balanceItem = new BalanceItem();
                    balanceItem.keyId = Convert.ToString((searchBalanceParam.current - 1) * searchBalanceParam.pageSize + i + 1);
                    balanceItem.id = dt.Rows[i]["id"].ToString();
                    balanceItem.merchantOrderId = dt.Rows[i]["agentCode"].ToString();
                    balanceItem.tradeTime = dt.Rows[i]["tradeTime"].ToString();
                    balanceItem.tradeAmount = Convert.ToDouble(dt.Rows[i]["supplyPrice"]);
                    balanceItem.waybillTime = dt.Rows[i]["createTime"].ToString();
                    balanceItem.supplier = Convert.ToDouble(dt.Rows[i]["supplyPrice"]);
                    pageResult.list.Add(balanceItem);
                }
                string sql1 = "SELECT o.tradeAmount,a.createTime,a.supplyPrice,a.profitAgent,a.profitDealer,a.platformPrice " +
                              "where o.merchantOrderId = a.merchantOrderId " + st;
                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "Table").Tables[0];
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    balanceTotalItem.totalSales += Convert.ToDouble(dt1.Rows[i]["supplyPrice"]);
                    balanceTotalItem.totalSupplier += Convert.ToDouble(dt1.Rows[i]["supplyPrice"]);
                }
                balanceTotalItem.total = dt1.Rows.Count;
                pageResult.item = balanceTotalItem;
            }
            return pageResult;
        }


        /// <summary>
        /// 获取采购商的结算列表
        /// </summary>
        /// <param name="searchBalanceParam"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PageResult getBalanceListByPurchase(SearchBalanceParam searchBalanceParam, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(searchBalanceParam.current, searchBalanceParam.pageSize);
            pageResult.list = new List<Object>();
            string st = " and o.purchaserCode = '" + userId + "' ";
            if (searchBalanceParam.OrderDate != null && searchBalanceParam.OrderDate.Length == 2)
            {
                st += " and o.tradeTime BETWEEN str_to_date('" + searchBalanceParam.OrderDate[0] + "', '%Y-%m-%d') " +
                            "AND DATE_ADD(str_to_date('" + searchBalanceParam.OrderDate[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            if (searchBalanceParam.BalanceDate != null && searchBalanceParam.BalanceDate.Length == 2)
            {
                st += " and a.createTime BETWEEN str_to_date('" + searchBalanceParam.BalanceDate[0] + "', '%Y-%m-%d') " +
                            "AND DATE_ADD(str_to_date('" + searchBalanceParam.BalanceDate[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            if (searchBalanceParam.merchantOrderId != null && searchBalanceParam.merchantOrderId.Trim() != "")
            {
                st += " and o.merchantOrderId like '%" + searchBalanceParam.merchantOrderId.Trim() + "%' ";
            }
            string sql = "select a.id,  a.merchantOrderId,o.tradeTime,o.tradeAmount,a.createTime,a.purchasePrice " +
                         "from t_order_list o ,t_account_analysis a " +
                         "where o.merchantOrderId = a.merchantOrderId " + st +
                         " ORDER BY o.id desc LIMIT " + (searchBalanceParam.current - 1) * searchBalanceParam.pageSize + "," + searchBalanceParam.pageSize;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                BalanceTotalItem balanceTotalItem = new BalanceTotalItem();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BalanceItem balanceItem = new BalanceItem();
                    balanceItem.keyId = Convert.ToString((searchBalanceParam.current - 1) * searchBalanceParam.pageSize + i + 1);
                    balanceItem.id = dt.Rows[i]["id"].ToString();
                    balanceItem.merchantOrderId = dt.Rows[i]["agentCode"].ToString();
                    balanceItem.tradeTime = dt.Rows[i]["tradeTime"].ToString();
                    balanceItem.tradeAmount = Convert.ToDouble(dt.Rows[i]["tradeAmount"]);
                    balanceItem.waybillTime = dt.Rows[i]["createTime"].ToString();
                    balanceItem.purchase = Convert.ToDouble(dt.Rows[i]["purchasePrice"]);
                    //balanceItem.distribution = dt.Rows[i]["distribution"].ToString();
                    pageResult.list.Add(balanceItem);
                }
                string sql1 = "SELECT select a.merchantOrderId,o.tradeTime,o.tradeAmount,a.createTime,a.purchasePrice " +
                              "where o.merchantOrderId = a.merchantOrderId " + st;
                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "Table").Tables[0];
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    balanceTotalItem.totalSales += Convert.ToDouble(dt1.Rows[i]["tradeAmount"]);
                    balanceTotalItem.totalPurchase += Convert.ToDouble(dt1.Rows[i]["purchasePrice"]);
                }
                balanceTotalItem.total = dt1.Rows.Count;
                pageResult.item = balanceTotalItem;
            }
            return pageResult;
        }


        /// <summary>
        /// 获取代理的结算列表
        /// </summary>
        /// <param name="searchBalanceParam"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PageResult getBalanceListByAgent(SearchBalanceParam searchBalanceParam, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(searchBalanceParam.current, searchBalanceParam.pageSize);
            pageResult.list = new List<Object>();
            string st = " and o.purchaserCode = '" + userId + "' ";
            if (searchBalanceParam.OrderDate != null && searchBalanceParam.OrderDate.Length == 2)
            {
                st += " and o.tradeTime BETWEEN str_to_date('" + searchBalanceParam.OrderDate[0] + "', '%Y-%m-%d') " +
                            "AND DATE_ADD(str_to_date('" + searchBalanceParam.OrderDate[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            if (searchBalanceParam.BalanceDate != null && searchBalanceParam.BalanceDate.Length == 2)
            {
                st += " and a.createTime BETWEEN str_to_date('" + searchBalanceParam.BalanceDate[0] + "', '%Y-%m-%d') " +
                            "AND DATE_ADD(str_to_date('" + searchBalanceParam.BalanceDate[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            if (searchBalanceParam.merchantOrderId != null && searchBalanceParam.merchantOrderId.Trim() != "")
            {
                st += " and o.merchantOrderId like '%" + searchBalanceParam.merchantOrderId.Trim() + "%' ";
            }
            string sql = "select a.id,  a.merchantOrderId,o.tradeTime,o.tradeAmount,a.createTime,a.profitAgent,a.profitDealer,o.distributionCode " +
                         "from t_order_list o ,t_account_analysis a " +
                         "where o.merchantOrderId = a.merchantOrderId " + st +
                         " ORDER BY o.id desc LIMIT " + (searchBalanceParam.current - 1) * searchBalanceParam.pageSize + "," + searchBalanceParam.pageSize;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                BalanceTotalItem balanceTotalItem = new BalanceTotalItem();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BalanceItem balanceItem = new BalanceItem();
                    balanceItem.keyId = Convert.ToString((searchBalanceParam.current - 1) * searchBalanceParam.pageSize + i + 1);
                    balanceItem.id = dt.Rows[i]["id"].ToString();
                    balanceItem.merchantOrderId = dt.Rows[i]["agentCode"].ToString();
                    balanceItem.tradeTime = dt.Rows[i]["tradeTime"].ToString();
                    balanceItem.tradeAmount = Convert.ToDouble(dt.Rows[i]["tradeAmount"]);
                    balanceItem.waybillTime = dt.Rows[i]["createTime"].ToString();
                    balanceItem.purchase = Convert.ToDouble(dt.Rows[i]["profitAgent"]) + Convert.ToDouble(dt.Rows[i]["profitDealer"]);
                    balanceItem.distribution = dt.Rows[i]["distributionCode"].ToString();
                    pageResult.list.Add(balanceItem);
                }
                string sql1 = "SELECT o.tradeAmount,a.createTime,a.profitAgent,a.profitDealer " +
                              "where o.merchantOrderId = a.merchantOrderId " + st;
                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "Table").Tables[0];
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    balanceTotalItem.totalSales += Convert.ToDouble(dt1.Rows[i]["tradeAmount"]);
                    balanceTotalItem.totalPurchase += Convert.ToDouble(dt1.Rows[i]["profitAgent"]) + Convert.ToDouble(dt.Rows[i]["profitDealer"]);
                }
                balanceTotalItem.total = dt1.Rows.Count;
                pageResult.item = balanceTotalItem;
            }
            return pageResult;
        }

        /// <summary>
        /// 获取分销的结算列表
        /// </summary>
        /// <param name="searchBalanceParam"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PageResult getBalanceListByDistribution(SearchBalanceParam searchBalanceParam, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(searchBalanceParam.current, searchBalanceParam.pageSize);
            pageResult.list = new List<Object>();
            string st = " and o.distributionCode = '" + userId + "' ";
            if (searchBalanceParam.OrderDate != null && searchBalanceParam.OrderDate.Length == 2)
            {
                st += " and o.tradeTime BETWEEN str_to_date('" + searchBalanceParam.OrderDate[0] + "', '%Y-%m-%d') " +
                            "AND DATE_ADD(str_to_date('" + searchBalanceParam.OrderDate[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            if (searchBalanceParam.BalanceDate != null && searchBalanceParam.BalanceDate.Length == 2)
            {
                st += " and a.createTime BETWEEN str_to_date('" + searchBalanceParam.BalanceDate[0] + "', '%Y-%m-%d') " +
                            "AND DATE_ADD(str_to_date('" + searchBalanceParam.BalanceDate[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            if (searchBalanceParam.merchantOrderId != null && searchBalanceParam.merchantOrderId.Trim() != "")
            {
                st += " and o.merchantOrderId like '%" + searchBalanceParam.merchantOrderId.Trim() + "%' ";
            }
            string sql = "select a.id,  a.merchantOrderId,o.tradeTime,o.tradeAmount,a.createTime,a.profitDealer " +
                         "from t_order_list o ,t_account_analysis a " +
                         "where o.merchantOrderId = a.merchantOrderId " + st +
                         " ORDER BY o.id desc LIMIT " + (searchBalanceParam.current - 1) * searchBalanceParam.pageSize + "," + searchBalanceParam.pageSize;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                BalanceTotalItem balanceTotalItem = new BalanceTotalItem();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BalanceItem balanceItem = new BalanceItem();
                    balanceItem.keyId = Convert.ToString((searchBalanceParam.current - 1) * searchBalanceParam.pageSize + i + 1);
                    balanceItem.id = dt.Rows[i]["id"].ToString();
                    balanceItem.merchantOrderId = dt.Rows[i]["agentCode"].ToString();
                    balanceItem.tradeTime = dt.Rows[i]["tradeTime"].ToString();
                    balanceItem.tradeAmount = Convert.ToDouble(dt.Rows[i]["tradeAmount"]);
                    balanceItem.waybillTime = dt.Rows[i]["createTime"].ToString();
                    balanceItem.purchase = Convert.ToDouble(dt.Rows[i]["profitDealer"]);
                    //balanceItem.distribution = dt.Rows[i]["distribution"].ToString();
                    pageResult.list.Add(balanceItem);
                }
                string sql1 = "SELECT o.tradeAmount,a.createTime,a.profitDealer " +
                              "where o.merchantOrderId = a.merchantOrderId " + st;
                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "Table").Tables[0];
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    balanceTotalItem.totalSales += Convert.ToDouble(dt1.Rows[i]["tradeAmount"]);
                    balanceTotalItem.totalPurchase += Convert.ToDouble(dt1.Rows[i]["profitDealer"]);
                }
                balanceTotalItem.total = dt1.Rows.Count;
                pageResult.item = balanceTotalItem;
            }
            return pageResult;
        }

        #endregion

        #region 新结算表
        public List<PurchaseItem> getPartner(string userCode)
        {
            List<PurchaseItem> list = new List<PurchaseItem>();
            string sql = "select * from t_user_list where ofAgent='" + userCode + "'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                PurchaseItem purchaseItem = new PurchaseItem();
                purchaseItem.purchaseCode = dt.Rows[i]["usercode"].ToString();
                purchaseItem.purchaseName = dt.Rows[i]["username"].ToString();
                list.Add(purchaseItem);
            }
            return list;
        }

        /// <summary>
        /// 获取结算收益-预估收益--供应代理
        /// </summary>
        /// <param name="searchBalanceParam"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PageResult getEstimateBySupplierAgent(SearchBalanceParam searchBalanceParam, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(searchBalanceParam.current, searchBalanceParam.pageSize);
            pageResult.list = new List<Object>();
            string st = " and o.supplierAgentCode = '" + userId + "' ";
            if (searchBalanceParam.OrderDate != null && searchBalanceParam.OrderDate.Length == 2)
            {
                st += " and o.tradeTime BETWEEN str_to_date('" + searchBalanceParam.OrderDate[0] + "', '%Y-%m-%d') " +
                            "AND DATE_ADD(str_to_date('" + searchBalanceParam.OrderDate[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            if (searchBalanceParam.BalanceDate != null && searchBalanceParam.BalanceDate.Length == 2)
            {
                st += " and a.createTime BETWEEN str_to_date('" + searchBalanceParam.BalanceDate[0] + "', '%Y-%m-%d') " +
                            "AND DATE_ADD(str_to_date('" + searchBalanceParam.BalanceDate[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            if (searchBalanceParam.merchantOrderId != null && searchBalanceParam.merchantOrderId.Trim() != "")
            {
                st += " and o.merchantOrderId like '%" + searchBalanceParam.merchantOrderId.Trim() + "%' ";
            }
            if (searchBalanceParam.purchaseCode != null && searchBalanceParam.purchaseCode.Trim() != "")
            {
                st += " and o.purchaserCode = '" + searchBalanceParam.purchaseCode.Trim() + "' ";
            }
            if (searchBalanceParam.accountCode != null && searchBalanceParam.accountCode.Trim() != "")
            {
                st += " and a.supplierAgentAccountCode = '" + searchBalanceParam.accountCode.Trim() + "' ";
            }
            else
            {
                st += " and (a.supplierAgentAccountCode is null or a.supplierAgentAccountCode = '') ";
            }
            string sql = "select a.id,a.merchantOrderId,o.tradeTime,o.waybilltime,a.supplyPrice,a.createTime,a.supplierAgentPrice,o.customerCode " +
                         "from t_order_list o ,t_account_analysis a " +
                         "where o.merchantOrderId = a.merchantOrderId  and a.supplierAgentPrice is not null " + st +
                         " ORDER BY o.id desc LIMIT " + (searchBalanceParam.current - 1) * searchBalanceParam.pageSize + "," + searchBalanceParam.pageSize;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                BalanceTotalItemNew balanceTotalItem = new BalanceTotalItemNew();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BalanceItemNew balanceItem = new BalanceItemNew();
                    balanceItem.keyId = Convert.ToString((searchBalanceParam.current - 1) * searchBalanceParam.pageSize + i + 1);
                    balanceItem.id = dt.Rows[i]["id"].ToString();
                    balanceItem.merchantOrderId = dt.Rows[i]["merchantOrderId"].ToString();
                    balanceItem.tradeTime = dt.Rows[i]["tradeTime"].ToString();
                    balanceItem.tradeAmount = Convert.ToDouble(dt.Rows[i]["supplyPrice"]);
                    balanceItem.waybillTime = dt.Rows[i]["waybilltime"].ToString();
                    balanceItem.profit = Convert.ToDouble(dt.Rows[i]["supplierAgentPrice"]);
                    balanceItem.distribution = dt.Rows[i]["customerCode"].ToString();
                    balanceItem.payType = "1";
                    pageResult.list.Add(balanceItem);
                }
                string sql1 = "SELECT a.supplyPrice,a.createTime,a.supplierAgentPrice " +
                              "from t_order_list o ,t_account_analysis a " +
                              "where o.merchantOrderId = a.merchantOrderId  and a.supplierAgentPrice is not null " + st;
                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "Table").Tables[0];
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    balanceTotalItem.totalSales += Convert.ToDouble(dt1.Rows[i]["supplyPrice"]);
                    balanceTotalItem.totalProfit += Convert.ToDouble(dt1.Rows[i]["supplierAgentPrice"]);
                }
                balanceTotalItem.total = dt1.Rows.Count;
                pageResult.pagination.total = dt1.Rows.Count;
                pageResult.item = balanceTotalItem;
            }
            return pageResult;
        }
        /// <summary>
        /// 获取结算收益-预估收益--采购代理
        /// </summary>
        /// <param name="searchBalanceParam"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PageResult getEstimateByPurchaseAgent(SearchBalanceParam searchBalanceParam, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(searchBalanceParam.current, searchBalanceParam.pageSize);
            pageResult.list = new List<Object>();
            string st = " and o.purchaseAgentCode = '" + userId + "' ";
            if (searchBalanceParam.OrderDate != null && searchBalanceParam.OrderDate.Length == 2)
            {
                st += " and o.tradeTime BETWEEN str_to_date('" + searchBalanceParam.OrderDate[0] + "', '%Y-%m-%d') " +
                            "AND DATE_ADD(str_to_date('" + searchBalanceParam.OrderDate[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            if (searchBalanceParam.BalanceDate != null && searchBalanceParam.BalanceDate.Length == 2)
            {
                st += " and a.createTime BETWEEN str_to_date('" + searchBalanceParam.BalanceDate[0] + "', '%Y-%m-%d') " +
                            "AND DATE_ADD(str_to_date('" + searchBalanceParam.BalanceDate[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            if (searchBalanceParam.merchantOrderId != null && searchBalanceParam.merchantOrderId.Trim() != "")
            {
                st += " and o.merchantOrderId like '%" + searchBalanceParam.merchantOrderId.Trim() + "%' ";
            }
            if (searchBalanceParam.purchaseCode != null && searchBalanceParam.purchaseCode.Trim() != "")
            {
                st += " and o.purchaserCode = '" + searchBalanceParam.purchaseCode.Trim() + "' ";
            }
            if (searchBalanceParam.accountCode != null && searchBalanceParam.accountCode.Trim() != "")
            {
                st += " and a.purchaseAgentAccountCode = '" + searchBalanceParam.accountCode.Trim() + "' ";
            }
            else
            {
                st += " and (a.purchaseAgentAccountCode is null or a.purchaseAgentAccountCode = '') ";
            }
            string sql = "select a.id,a.merchantOrderId,o.tradeTime,o.waybilltime,a.purchasePrice,a.createTime,a.purchaseAgentPrice,o.purchaserCode " +
                         "from t_order_list o ,t_account_analysis a " +
                         "where o.merchantOrderId = a.merchantOrderId and a.purchaseAgentPrice is not null " + st +
                         " ORDER BY o.id desc LIMIT " + (searchBalanceParam.current - 1) * searchBalanceParam.pageSize + "," + searchBalanceParam.pageSize;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                BalanceTotalItemNew balanceTotalItem = new BalanceTotalItemNew();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BalanceItemNew balanceItem = new BalanceItemNew();
                    balanceItem.keyId = Convert.ToString((searchBalanceParam.current - 1) * searchBalanceParam.pageSize + i + 1);
                    balanceItem.id = dt.Rows[i]["id"].ToString();
                    balanceItem.merchantOrderId = dt.Rows[i]["merchantOrderId"].ToString();
                    balanceItem.tradeTime = dt.Rows[i]["tradeTime"].ToString();
                    balanceItem.tradeAmount = Convert.ToDouble(dt.Rows[i]["purchasePrice"]);
                    balanceItem.waybillTime = dt.Rows[i]["waybilltime"].ToString();
                    balanceItem.profit = Convert.ToDouble(dt.Rows[i]["purchaseAgentPrice"]);
                    balanceItem.distribution = dt.Rows[i]["purchaserCode"].ToString();
                    balanceItem.payType = "1";
                    pageResult.list.Add(balanceItem);
                }
                string sql1 = "SELECT a.purchasePrice,a.createTime,a.purchaseAgentPrice " +
                              "from t_order_list o ,t_account_analysis a " +
                              "where o.merchantOrderId = a.merchantOrderId and a.purchaseAgentPrice is not null " + st;
                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "Table").Tables[0];
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    balanceTotalItem.totalSales += Convert.ToDouble(dt1.Rows[i]["purchasePrice"]);
                    balanceTotalItem.totalProfit += Convert.ToDouble(dt1.Rows[i]["purchaseAgentPrice"]);
                }
                balanceTotalItem.total = dt1.Rows.Count;
                pageResult.pagination.total = dt1.Rows.Count;
                pageResult.item = balanceTotalItem;
            }
            return pageResult;
        }
        /// <summary>
        /// 获取结算收益-预估收益--采购
        /// </summary>
        /// <param name="searchBalanceParam"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PageResult getEstimateByPurchase(SearchBalanceParam searchBalanceParam, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(searchBalanceParam.current, searchBalanceParam.pageSize);
            pageResult.list = new List<Object>();
            string st = " and o.purchaserCode = '" + userId + "' ";
            if (searchBalanceParam.OrderDate != null && searchBalanceParam.OrderDate.Length == 2)
            {
                st += " and o.tradeTime BETWEEN str_to_date('" + searchBalanceParam.OrderDate[0] + "', '%Y-%m-%d') " +
                            "AND DATE_ADD(str_to_date('" + searchBalanceParam.OrderDate[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            if (searchBalanceParam.BalanceDate != null && searchBalanceParam.BalanceDate.Length == 2)
            {
                st += " and a.createTime BETWEEN str_to_date('" + searchBalanceParam.BalanceDate[0] + "', '%Y-%m-%d') " +
                            "AND DATE_ADD(str_to_date('" + searchBalanceParam.BalanceDate[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            if (searchBalanceParam.merchantOrderId != null && searchBalanceParam.merchantOrderId.Trim() != "")
            {
                st += " and o.merchantOrderId like '%" + searchBalanceParam.merchantOrderId.Trim() + "%' ";
            }
            if (searchBalanceParam.purchaseCode != null && searchBalanceParam.purchaseCode.Trim() != "")
            {
                st += " and o.purchaserCode = '" + searchBalanceParam.purchaseCode.Trim() + "' ";
            }
            if (searchBalanceParam.accountCode != null && searchBalanceParam.accountCode.Trim() != "")
            {
                st += " and a.purchaseAccountCode = '" + searchBalanceParam.accountCode.Trim() + "' ";
            }
            else
            {
                st += " and (a.purchaseAccountCode is null or a.purchaseAccountCode = '') ";
            }
            string sql = "select a.id,a.merchantOrderId,o.tradeTime,o.waybilltime,o.tradeAmount,a.createTime,a.purchasePrice,o.purchaserCode " +
                         "from t_order_list o ,t_account_analysis a " +
                         "where o.merchantOrderId = a.merchantOrderId and a.purchasePrice is not null " + st +
                         " ORDER BY o.id desc LIMIT " + (searchBalanceParam.current - 1) * searchBalanceParam.pageSize + "," + searchBalanceParam.pageSize;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                BalanceTotalItemNew balanceTotalItem = new BalanceTotalItemNew();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BalanceItemNew balanceItem = new BalanceItemNew();
                    balanceItem.keyId = Convert.ToString((searchBalanceParam.current - 1) * searchBalanceParam.pageSize + i + 1);
                    balanceItem.id = dt.Rows[i]["id"].ToString();
                    balanceItem.merchantOrderId = dt.Rows[i]["merchantOrderId"].ToString();
                    balanceItem.tradeTime = dt.Rows[i]["tradeTime"].ToString();
                    balanceItem.tradeAmount = Convert.ToDouble(dt.Rows[i]["tradeAmount"]);
                    balanceItem.waybillTime = dt.Rows[i]["waybilltime"].ToString();
                    balanceItem.profit = Convert.ToDouble(dt.Rows[i]["purchasePrice"]);
                    //balanceItem.distribution = dt.Rows[i]["purchaserCode"].ToString();
                    balanceItem.payType = "1";
                    pageResult.list.Add(balanceItem);
                }
                string sql1 = "SELECT o.tradeAmount,a.createTime,a.purchasePrice " +
                              "from t_order_list o ,t_account_analysis a " +
                              "where o.merchantOrderId = a.merchantOrderId and a.purchasePrice is not null " + st;
                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "Table").Tables[0];
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    balanceTotalItem.totalSales += Convert.ToDouble(dt1.Rows[i]["tradeAmount"]);
                    balanceTotalItem.totalProfit += Convert.ToDouble(dt1.Rows[i]["purchasePrice"]);
                }
                balanceTotalItem.total = dt1.Rows.Count;
                pageResult.pagination.total = dt1.Rows.Count;
                pageResult.item = balanceTotalItem;
            }
            return pageResult;
        }
        /// <summary>
        /// 获取结算收益-预估收益--代理
        /// </summary>
        /// <param name="searchBalanceParam"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PageResult getEstimateByAgent(SearchBalanceParam searchBalanceParam, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(searchBalanceParam.current, searchBalanceParam.pageSize);
            pageResult.list = new List<Object>();
            string st = " and o.purchaserCode = '" + userId + "' ";
            if (searchBalanceParam.OrderDate != null && searchBalanceParam.OrderDate.Length == 2)
            {
                st += " and o.tradeTime BETWEEN str_to_date('" + searchBalanceParam.OrderDate[0] + "', '%Y-%m-%d') " +
                            "AND DATE_ADD(str_to_date('" + searchBalanceParam.OrderDate[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            if (searchBalanceParam.BalanceDate != null && searchBalanceParam.BalanceDate.Length == 2)
            {
                st += " and a.createTime BETWEEN str_to_date('" + searchBalanceParam.BalanceDate[0] + "', '%Y-%m-%d') " +
                            "AND DATE_ADD(str_to_date('" + searchBalanceParam.BalanceDate[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            if (searchBalanceParam.merchantOrderId != null && searchBalanceParam.merchantOrderId.Trim() != "")
            {
                st += " and o.merchantOrderId like '%" + searchBalanceParam.merchantOrderId.Trim() + "%' ";
            }
            if (searchBalanceParam.purchaseCode != null && searchBalanceParam.purchaseCode.Trim() != "")
            {
                st += " and o.purchaserCode = '" + searchBalanceParam.purchaseCode.Trim() + "' ";
            }
            if (searchBalanceParam.accountCode != null && searchBalanceParam.accountCode.Trim() != "")
            {
                st += " and a.agentAccountCode = '" + searchBalanceParam.accountCode.Trim() + "' ";
            }
            else
            {
                st += " and (a.agentAccountCode is null or a.agentAccountCode = '') ";
            }
            string sql = "select a.id,a.merchantOrderId,o.tradeTime,o.waybilltime,o.tradeAmount,a.createTime,a.profitAgent,o.purchaserCode " +
                         "from t_order_list o ,t_account_analysis a " +
                         "where o.merchantOrderId = a.merchantOrderId and a.profitAgent is not null " + st +
                         " ORDER BY o.id desc LIMIT " + (searchBalanceParam.current - 1) * searchBalanceParam.pageSize + "," + searchBalanceParam.pageSize;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                BalanceTotalItemNew balanceTotalItem = new BalanceTotalItemNew();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BalanceItemNew balanceItem = new BalanceItemNew();
                    balanceItem.keyId = Convert.ToString((searchBalanceParam.current - 1) * searchBalanceParam.pageSize + i + 1);
                    balanceItem.id = dt.Rows[i]["id"].ToString();
                    balanceItem.merchantOrderId = dt.Rows[i]["merchantOrderId"].ToString();
                    balanceItem.tradeTime = dt.Rows[i]["tradeTime"].ToString();
                    balanceItem.tradeAmount = Convert.ToDouble(dt.Rows[i]["tradeAmount"]);
                    balanceItem.waybillTime = dt.Rows[i]["waybilltime"].ToString();
                    balanceItem.profit = Convert.ToDouble(dt.Rows[i]["profitAgent"]);
                    //balanceItem.distribution = dt.Rows[i]["purchaserCode"].ToString();
                    balanceItem.payType = "1";
                    pageResult.list.Add(balanceItem);
                }
                string sql1 = "SELECT o.tradeAmount,a.createTime,a.profitAgent " +
                              "from t_order_list o ,t_account_analysis a " +
                              "where o.merchantOrderId = a.merchantOrderId and a.profitAgent is not null " + st;
                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "Table").Tables[0];
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    balanceTotalItem.totalSales += Convert.ToDouble(dt1.Rows[i]["tradeAmount"]);
                    balanceTotalItem.totalProfit += Convert.ToDouble(dt1.Rows[i]["profitAgent"]);
                }
                balanceTotalItem.total = dt1.Rows.Count;
                pageResult.pagination.total = dt1.Rows.Count;
                pageResult.item = balanceTotalItem;
            }
            return pageResult;
        }

        /// <summary>
        /// 获取结算收益-已结算
        /// </summary>
        /// <param name="searchBalanceParam"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PageResult getSettle(SearchBalanceParam searchBalanceParam, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(searchBalanceParam.current, searchBalanceParam.pageSize);
            pageResult.list = new List<Object>();
            string st = "";
            if (searchBalanceParam.BalanceDate != null && searchBalanceParam.BalanceDate.Length == 2)
            {
                st += " and createTime BETWEEN str_to_date('" + searchBalanceParam.BalanceDate[0] + "', '%Y-%m-%d') " +
                            "AND DATE_ADD(str_to_date('" + searchBalanceParam.BalanceDate[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            if (searchBalanceParam.accountStatus != null && searchBalanceParam.accountStatus.Trim() != "")
            {
                st += " and status = '" + searchBalanceParam.accountStatus.Trim() + "' ";
            }
            string sql = "select * from t_account_list where usercode = '" + userId + "' " + st +
                         " ORDER BY id desc LIMIT " + (searchBalanceParam.current - 1) * searchBalanceParam.pageSize + "," + searchBalanceParam.pageSize;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    AccountItem accountItem = new AccountItem();
                    accountItem.keyId = Convert.ToString((searchBalanceParam.current - 1) * searchBalanceParam.pageSize + i + 1);
                    accountItem.id = dt.Rows[i]["id"].ToString();
                    accountItem.accountCode = dt.Rows[i]["accountCode"].ToString();
                    accountItem.createTime = dt.Rows[i]["createTime"].ToString();
                    accountItem.usercode = dt.Rows[i]["usercode"].ToString();
                    accountItem.price = dt.Rows[i]["price"].ToString();
                    accountItem.status = dt.Rows[i]["status"].ToString();
                    pageResult.list.Add(accountItem);
                }
                string sql1 = "select sum(price) as pricesum,count(*) count1 from t_account_list  where usercode = '" + userId + "' " + st;
                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "Table").Tables[0];
                pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0]["count1"]);
                ProfitTotalItem profitTotalItem = new ProfitTotalItem();
                profitTotalItem.totalProfit = dt1.Rows[0]["pricesum"].ToString();
                pageResult.item = profitTotalItem;
            }
            return pageResult;
        }

        public ProfitTotalItem getTotalProfit(string userId)
        {
            ProfitTotalItem profitTotalItem = new ProfitTotalItem();
            try
            {
                UserDao userDao = new UserDao();
                string userType = userDao.getUserType(userId);
                if (userType == "9")
                {
                    string sql = "select sum(a.supplierAgentPrice) supplierAgentPrice " +
                             "from t_order_list o ,t_account_analysis a " +
                             "where o.merchantOrderId = a.merchantOrderId and a.supplierAgentPrice is not null " +
                             "and o.supplierAgentCode = '" + userId + "' and (a.supplierAgentAccountCode is null or a.supplierAgentAccountCode = '') ";
                    DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        profitTotalItem.totalEstimate = dt.Rows[0][0].ToString();
                    }
                }
                else if (userType == "10")
                {
                    string sql = "select sum(a.purchaseAgentPrice) purchaseAgentPrice " +
                             "from t_order_list o ,t_account_analysis a " +
                             "where o.merchantOrderId = a.merchantOrderId and a.purchaseAgentPrice is not null " +
                             "and o.purchaseAgentCode = '" + userId + "' and (a.purchaseAgentAccountCode is null or a.purchaseAgentAccountCode = '') ";
                    DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        profitTotalItem.totalEstimate = dt.Rows[0][0].ToString();
                    }
                }
                else if (userType == "2")
                {
                    string sql = "select sum(a.purchasePrice) purchasePrice " +
                             "from t_order_list o ,t_account_analysis a " +
                             "where o.merchantOrderId = a.merchantOrderId and a.purchasePrice is not null " +
                             "and o.purchaserCode = '" + userId + "' and (a.purchaseAccountCode is null or a.purchaseAccountCode = '') ";
                    DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        profitTotalItem.totalEstimate = dt.Rows[0][0].ToString();
                    }
                }
                else if (userType == "3")
                {
                    string sql = "select sum(a.profitAgent) profitAgent " +
                             "from t_order_list o ,t_account_analysis a " +
                             "where o.merchantOrderId = a.merchantOrderId and a.profitAgent is not null " +
                             "and o.purchaserCode = '" + userId + "' and (a.agentAccountCode is null or a.agentAccountCode = '') ";
                    DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        profitTotalItem.totalEstimate = dt.Rows[0][0].ToString();
                    }
                }

                string sql1 = "select status,sum(price) sumprice from t_account_list where usercode = 'gongying' group by status";
                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "Table").Tables[0];
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    if (dt1.Rows[i]["status"].ToString() == "0")
                    {
                        profitTotalItem.totalUnpaid = dt1.Rows[i]["sumprice"].ToString();
                    }
                    else if (dt1.Rows[i]["status"].ToString() == "1")
                    {
                        profitTotalItem.totalProfit = dt1.Rows[i]["sumprice"].ToString();
                    }
                }
                if (profitTotalItem.totalEstimate == "")
                {
                    profitTotalItem.totalEstimate = "0";
                }
                if (profitTotalItem.totalUnpaid == "")
                {
                    profitTotalItem.totalUnpaid = "0";
                }
                if (profitTotalItem.totalProfit == "")
                {
                    profitTotalItem.totalProfit = "0";
                }
            }
            catch (Exception)
            {

            }

            return profitTotalItem;
        }


        #endregion

        /// <summary>
        /// 获取代销、供应商-货款结算
        /// </summary>
        /// <param name="paymentParam"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PageResult GetPayment(PaymentParam paymentParam, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(paymentParam.current, paymentParam.pageSize);
            pageResult.list = new List<object>();

            string ac = "";
            if (paymentParam.accountCode != null && paymentParam.accountCode != "")
            {
                ac = " and a.accountCode like '%" + paymentParam.accountCode + "%'";
            }

            string st = "";
            if (paymentParam.status == "0")
            {
                st = " and `status`='0'";
            }
            else if (paymentParam.status == "1")
            {
                st = " and `status`='1'";
            }

            string t = "";
            if (paymentParam.date != null && paymentParam.date.Length == 2)
            {
                t = " and createTime between  str_to_date('" + paymentParam.date[0] + "', '%Y-%m-%d') " +
                               "AND DATE_ADD(str_to_date('" + paymentParam.date[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }

            string sql = "SELECT b.accountType,b.price,a.accountCode from t_account_list a,t_account_info b  where a.accountCode=b.accountCode and usercode='" +
                userId + "' " + ac + st + t;

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

            string sql1 = "SELECT dateFrom,dateTo,`status`,b.accountType,b.price,a.accountCode from t_account_list a,t_account_info b  where a.accountCode=b.accountCode and usercode='" +
               userId + "' " + ac + st + t + " group by a.accountCode " + " order by dateTo desc   LIMIT " + (paymentParam.current - 1) * paymentParam.pageSize + "," + paymentParam.pageSize;
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "T").Tables[0];



            if (dt.Rows.Count > 0)
            {

                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    PaymentItem paymentItem = new PaymentItem();
                    paymentItem.keyId = Convert.ToString((paymentParam.current - 1) * paymentParam.pageSize + i + 1);
                    paymentItem.accountCode = dt1.Rows[i]["accountCode"].ToString();
                    paymentItem.date = Convert.ToDateTime(dt1.Rows[i]["dateFrom"]).ToString("yyyy.MM.dd") + "~" + Convert.ToDateTime(dt1.Rows[i]["dateTo"]).ToString("yyyy.MM.dd");
                    if (dt1.Rows[i]["status"].ToString()!="0")
                    {
                        paymentItem.status = "1";
                        paymentItem.flag = dt1.Rows[i]["status"].ToString();
                    }
                    else
                        paymentItem.status = "0";
                    DataRow[] dr = dt.Select("accountCode='" + paymentItem.accountCode + "'");
                    for (int j = 0; j < dr.Length; j++)
                    {
                                          
                            switch (dr[j]["accountType"].ToString())
                            {
                                case "1":
                                    paymentItem.purchasemoney += Convert.ToDouble(dr[j]["price"].ToString());
                                    break;
                                case "2":
                                    paymentItem.refundmoney += Convert.ToDouble(dr[j]["price"].ToString());
                                    break;
                                case "3":
                                    paymentItem.othermoney += Convert.ToDouble(dr[j]["price"].ToString());
                                    break;
                                case "4":
                                    paymentItem.paymoney += Convert.ToDouble(dr[j]["price"].ToString());
                                    break;
                            }
                        
                    }
                    paymentItem.refundmoney = Math.Round(paymentItem.refundmoney, 2);
                    paymentItem.purchasemoney = Math.Round(paymentItem.purchasemoney, 2);
                    paymentItem.othermoney = Math.Round(paymentItem.othermoney, 2);
                    paymentItem.paymoney = Math.Round(paymentItem.paymoney, 2);
                    pageResult.list.Add(paymentItem);
                }
                string sql3 = "SELECT count(*) from t_account_list a,t_account_info b  where a.accountCode=b.accountCode and usercode='" +
                userId + "' " + ac + st + t + " group by a.accountCode ";

                DataTable dt3 = DatabaseOperationWeb.ExecuteSelectDS(sql3, "t_goods_list").Tables[0];
                pageResult.pagination.total = Convert.ToInt16(dt3.Rows.Count);
            }
            return pageResult;
        }


        /// <summary>
        /// 获取代销-货款结算-确认付款
        /// </summary>
        /// <param name="paymentParam"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public MsgResult SurePayMent(PaymentDetailedParam pdp, string userId)
        {
            MsgResult msg = new MsgResult();

            string sql = ""
                + "select count(*)"
                + " from t_account_list"
                + " where accountCode='" + pdp.accountCode + "' and usercode='" + userId + "'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql,"T").Tables[0];
            if (Convert.ToInt16(dt.Rows[0][0]) > 0)
            {
                string update = ""
                + " update t_account_list"
                + " set `status`='0'"
                + " where accountCode='" + pdp.accountCode + "' and usercode='" + userId + "'";
                if (DatabaseOperationWeb.ExecuteDML(update))
                    msg.type = "1";
            }
            else
                msg.msg = "无此结算单，或账号权限不足";
            return msg;

        }




        /// <summary>
        /// 获取代销-货款结算明细
        /// </summary>
        /// <param name="paymentParam"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PageResult GetPaymentDetailed(PaymentDetailedParam paymentDetailedParam, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(paymentDetailedParam.current, paymentDetailedParam.pageSize);
            pageResult.list = new List<object>();

            string sql = "select (select platformType from t_base_platform e where e.platformId=a.platformId ) platformType,c.accountType,c.orderId,d.barCode,b.slt,b.goodsName,skuUnitPrice,b.purchasePrice,brand,quantity,tradeTime "
                + " from t_order_list a,t_order_goods b,t_account_info c,t_goods_list d "
                + " WHERE a.merchantOrderId = b.merchantOrderId and c.orderId = a.merchantOrderId  and d.barcode = b.barCode  and c.accountCode = '" + paymentDetailedParam.accountCode +
                "' order by c.accountType desc,tradeTime desc  limit " + (paymentDetailedParam.current - 1) * paymentDetailedParam.pageSize + "," + paymentDetailedParam.pageSize;

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    PaymentDetailedItem paymentDetailedItem = new PaymentDetailedItem();
                    if (dt.Rows[i]["accountType"].ToString() == "2")
                    {
                        paymentDetailedItem.purchasePrice = -Math.Round(Convert.ToDouble(dt.Rows[i]["purchasePrice"].ToString()), 2);
                        paymentDetailedItem.skuUnitPrice = -Math.Round(Convert.ToDouble(dt.Rows[i]["skuUnitPrice"].ToString()), 2);                       
                    }
                    else
                    {
                        paymentDetailedItem.purchasePrice = Math.Round(Convert.ToDouble(dt.Rows[i]["purchasePrice"].ToString()), 2);
                        paymentDetailedItem.skuUnitPrice = Math.Round(Convert.ToDouble(dt.Rows[i]["skuUnitPrice"].ToString()), 2);                       
                    }
                    paymentDetailedItem.accountType = dt.Rows[i]["accountType"].ToString();
                    paymentDetailedItem.keyId = Convert.ToString((paymentDetailedParam.current - 1) * paymentDetailedParam.pageSize + i + 1);
                    paymentDetailedItem.orderId= dt.Rows[i]["orderId"].ToString();
                    paymentDetailedItem.orderType = dt.Rows[i]["platformType"].ToString();
                    paymentDetailedItem.barCode = dt.Rows[i]["barCode"].ToString();
                    paymentDetailedItem.brand = dt.Rows[i]["brand"].ToString();
                    paymentDetailedItem.goodsName = dt.Rows[i]["goodsName"].ToString();                 
                    paymentDetailedItem.quantity = Convert.ToInt16(dt.Rows[i]["quantity"].ToString());                  
                    paymentDetailedItem.slt = dt.Rows[i]["slt"].ToString();
                    paymentDetailedItem.tradeTime = dt.Rows[i]["tradeTime"].ToString();
                    paymentDetailedItem.money = Math.Round(paymentDetailedItem.quantity * paymentDetailedItem.skuUnitPrice, 2);
                    pageResult.list.Add(paymentDetailedItem);
                }
            }
            string sql1 = "select count(*) " +
                "from t_order_list a,t_order_goods b,t_account_info c,t_goods_list d " +
                "WHERE a.merchantOrderId = b.merchantOrderId and c.orderId = a.merchantOrderId  and d.barcode = b.barCode  and c.accountCode = '" + paymentDetailedParam.accountCode +
                "'";
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t").Tables[0];
            pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);
            return pageResult;
        }


        /// <summary>
        /// 获取供应-货款结算明细
        /// </summary>
        /// <param name="paymentParam"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PageResult GetPaymentDetailedGY(PaymentDetailedParam paymentDetailedParam, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(paymentDetailedParam.current, paymentDetailedParam.pageSize);
            pageResult.list = new List<object>();

            string sql = "select (select platformType from t_base_platform e where e.platformId=a.platformId ) platformType,c.accountType,c.orderId,d.barCode,b.slt,b.goodsName,b.supplyPrice,brand,quantity,tradeTime "
                + " from t_order_list a,t_order_goods b,t_account_info c,t_goods_list d "
                + " WHERE a.merchantOrderId = b.merchantOrderId and c.orderId = a.merchantOrderId  and d.barcode = b.barCode  and c.accountCode = '" + paymentDetailedParam.accountCode +
                "' order by c.accountType desc,tradeTime desc   limit " + (paymentDetailedParam.current - 1) * paymentDetailedParam.pageSize + "," + paymentDetailedParam.pageSize;

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    PaymentDetailedItem paymentDetailedItem = new PaymentDetailedItem();
                    if (dt.Rows[i]["accountType"].ToString() == "2")
                    {
                        paymentDetailedItem.purchasePrice = -Math.Round(Convert.ToDouble(dt.Rows[i]["supplyPrice"].ToString()), 2);                                               
                    }
                    else
                    {
                        paymentDetailedItem.purchasePrice = Math.Round(Convert.ToDouble(dt.Rows[i]["supplyPrice"].ToString()), 2);                                            
                    }
                    paymentDetailedItem.accountType = dt.Rows[i]["accountType"].ToString();
                    paymentDetailedItem.keyId = Convert.ToString((paymentDetailedParam.current - 1) * paymentDetailedParam.pageSize + i + 1);
                    paymentDetailedItem.orderId = dt.Rows[i]["orderId"].ToString();
                    paymentDetailedItem.orderType = dt.Rows[i]["platformType"].ToString();
                    paymentDetailedItem.barCode = dt.Rows[i]["barCode"].ToString();
                    paymentDetailedItem.brand = dt.Rows[i]["brand"].ToString();
                    paymentDetailedItem.goodsName = dt.Rows[i]["goodsName"].ToString();                   
                    paymentDetailedItem.quantity = Convert.ToInt16(dt.Rows[i]["quantity"].ToString());                    
                    paymentDetailedItem.slt = dt.Rows[i]["slt"].ToString();
                    paymentDetailedItem.money = Math.Round(paymentDetailedItem.quantity * paymentDetailedItem.purchasePrice, 2);
                    paymentDetailedItem.tradeTime = dt.Rows[i]["tradeTime"].ToString();                    
                    pageResult.list.Add(paymentDetailedItem);
                }
            }
            string sql1 = "select count(*) " +
                "from t_order_list a,t_order_goods b,t_account_info c,t_goods_list d " +
                "WHERE a.merchantOrderId = b.merchantOrderId and c.orderId = a.merchantOrderId  and d.barcode = b.barCode  and c.accountCode = '" + paymentDetailedParam.accountCode +
                "'";
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t").Tables[0];
            pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);
            return pageResult;
        }


        /// <summary>
        /// 获取代销-货款结算其他明细
        /// </summary>
        /// <param name="paymentParam"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PageResult GetPaymentOtherDetailed(PaymentDetailedParam paymentDetailedParam, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(paymentDetailedParam.current, paymentDetailedParam.pageSize);
            pageResult.list = new List<object>();

            string sql = "select `year`,`month`,a.price,detail,adjustName from t_account_adjust a,t_base_adjust b,t_account_info c,t_account_list d " +
                          "where a.adjustType = b.adjustType  and c.orderId = a.adjustCode and d.accountCode = c.accountCode   and d.accountCode = '" + paymentDetailedParam.accountCode + "' ORDER BY `year` DESC,`month` DESC  limit " +
                          (paymentDetailedParam.current - 1) * paymentDetailedParam.pageSize + "," + paymentDetailedParam.pageSize;

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    PaymentOtherDetailedItem paymentOtherDetailedItem = new PaymentOtherDetailedItem();
                    paymentOtherDetailedItem.keyId = Convert.ToString((paymentDetailedParam.current - 1) * paymentDetailedParam.pageSize + i + 1);
                    paymentOtherDetailedItem.month = dt.Rows[i]["month"].ToString();
                    paymentOtherDetailedItem.year = dt.Rows[i]["year"].ToString();
                    paymentOtherDetailedItem.price = Math.Round(Convert.ToDouble(dt.Rows[i]["price"].ToString()),2);
                    paymentOtherDetailedItem.adjustName = dt.Rows[i]["adjustName"].ToString();
                    paymentOtherDetailedItem.detail = dt.Rows[i]["detail"].ToString();

                    pageResult.list.Add(paymentOtherDetailedItem);
                }

            }
            string sql1 = "select count(*) from t_account_adjust a,t_base_adjust b,t_account_info c,t_account_list d " +
                          "where a.adjustType = b.adjustType  and c.id = a.adjustCode and d.accountCode = c.accountCode   and d.accountCode = '" + paymentDetailedParam.accountCode + "'";
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t").Tables[0];
            pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);
            return pageResult;
        }


        /// <summary>
        /// 获取代销-货款结算(打印)接口
        /// </summary>
        /// <param name="paymentParam"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PaymentPrinting GetPaymentPrinting(PaymentDetailedParam paymentDetailedParam, string userId)
        {
            PaymentPrinting paymentPrinting = new PaymentPrinting();
            PaymentPrintingItem paymentPrintingItem = new PaymentPrintingItem();

            string sql = "select a.accountCode,dateFrom,dateTo,a.price,contractCode,b.accountType,b.price,d.username   from t_user_list d,t_account_info b,t_account_list a left join  t_contract_list c  on a.usercode = c.userCode " +
                         "where  a.accountCode = b.accountCode  and  a.usercode=d.usercode   and a.accountCode = '"+ paymentDetailedParam .accountCode+ "' order by b.accountType asc";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
         
            if (dt.Rows.Count>0)
            {
                paymentPrintingItem.accountCode = dt.Rows[0]["accountCode"].ToString();
                paymentPrintingItem.date = Convert.ToDateTime(dt.Rows[0]["dateFrom"]).ToString("yyyy.MM.dd") + "~" + Convert.ToDateTime(dt.Rows[0]["dateTo"]).ToString("yyyy.MM.dd");
                paymentPrintingItem.contractCode = dt.Rows[0]["contractCode"].ToString();
                paymentPrintingItem.today = DateTime.Now.ToString("yyyy-MM-dd");

                var number= Math.Round(Convert.ToDouble( dt.Rows[0]["price"].ToString()) ,2);
                var s = number.ToString("#L#E#D#C#K#E#D#C#J#E#D#C#I#E#D#C#H#E#D#C#G#E#D#C#F#E#D#C#.0B0A");
                var d = Regex.Replace(s, @"((?<=-|^)[^1-9]*)|((?'z'0)[0A-E]*((?=[1-9])|(?'-z'(?=[F-L\.]|$))))|((?'b'[F-L])(?'z'0)[0A-L]*((?=[1-9])|(?'-z'(?=[\.]|$))))", "${b}${z}");
                paymentPrintingItem.money = Regex.Replace(d, ".", m => "负元空零壹贰叁肆伍陆柒捌玖空空空空空空空分角拾佰仟万亿兆京垓秭穰"[m.Value[0] - '-'].ToString());
                paymentPrintingItem.name= dt.Rows[0]["username"].ToString();

                paymentPrinting.item = paymentPrintingItem;

                
                for (int j=0;j<5;j++)
                {
                    PaymentPrintingList paymentPrintingList = new PaymentPrintingList();
                    paymentPrinting.list.Add(paymentPrintingList);
                }
                paymentPrinting.list[1].type = "采退金额";
                paymentPrinting.list[0].type = "采购金额";
                paymentPrinting.list[3].explain = "其他费用";
                paymentPrinting.list[4].explain = "实际应付款";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                                         
                    switch (dt.Rows[i]["accountType"].ToString())
                    {
                        case "1":
                            paymentPrinting.list[0].keyId = "1";
                            
                            paymentPrinting.list[0].price += Math.Round(Convert.ToDouble( dt.Rows[i]["price1"].ToString()),2);
                            break;
                        case "2":
                            paymentPrinting.list[1].keyId = "2";
                            
                            paymentPrinting.list[1].price += Math.Round(Convert.ToDouble( dt.Rows[i]["price1"].ToString()),2);
                            break;
                        case "3":
                           
                            paymentPrinting.list[3].keyId = "4";
                            
                            paymentPrinting.list[3].price += Math.Round(Convert.ToDouble(dt.Rows[i]["price1"].ToString()),2);
                            
                            break;
                        
                    }
                   
                }
                paymentPrinting.list[2].keyId = "3";
                paymentPrinting.list[2].explain = "金额合计";              
                paymentPrinting.list[2].price = Math.Round(paymentPrinting.list[0].price- paymentPrinting.list[1].price,2);
                paymentPrinting.list[2].price = Math.Round(paymentPrinting.list[2].price, 2);
                paymentPrinting.list[4].keyId = "5";
                paymentPrinting.list[4].price = paymentPrinting.list[2].price - paymentPrinting.list[3].price;
                paymentPrinting.list[4].price = Math.Round(paymentPrinting.list[4].price, 2);               
                paymentPrinting.list[3].price = Math.Round(paymentPrinting.list[3].price, 2);
                paymentPrinting.list[0].price = Math.Round(paymentPrinting.list[0].price, 2);
                paymentPrinting.list[1].price = Math.Round(paymentPrinting.list[1].price, 2);
            }
            else
            {
                paymentPrinting.item = paymentPrintingItem;               
            }
            return paymentPrinting;
        }
        public MsgResult settleAccounts(SettleAccountsParam settleAccountsParam,string userId)
        {
            MsgResult msgResult = new MsgResult();
            msgResult.msg = "操作失败";
            msgResult.type = "0";
            DateTime dateFrom = DateTime.Now.AddYears(-10);
            string sql1 = "select max(dateTo) from t_account_list where usercode = '" + settleAccountsParam.userCode + "'";
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "T").Tables[0];
            if (dt1.Rows[0][0]!=null&& dt1.Rows[0][0].ToString()!="")
            {
                dateFrom = Convert.ToDateTime(dt1.Rows[0][0]).AddDays(1);
            }
            DateTime dateTo = DateTime.Now;
            try
            {
                dateTo = Convert.ToDateTime(settleAccountsParam.dateTo);
            }
            catch (Exception)
            {
                msgResult.msg = "结账日期错误";
                return msgResult;
            }
            UserDao userDao = new UserDao();
            string userType = userDao.getUserType(settleAccountsParam.userCode);
            string st = "";
            if (userType=="1")//供货商
            {
                st = " and o.customerCode = '" + settleAccountsParam.userCode + "' and a.supplyAccountCode is null ";
            }
            else if (userType == "2")//采购商
            {
                st = " and o.purchaserCode = '" + settleAccountsParam.userCode + "' and a.purchaseAccountCode is null  ";
            }
            else if (userType == "3")//代理
            {
                st = " and o.purchaserCode = '" + settleAccountsParam.userCode + "' and a.agentAccountCode is null  ";
            }
            else if (userType == "4")//分销
            {
                st = " and o.distributionCode = '" + settleAccountsParam.userCode + "' and a.dealerAccountCode is null  ";
            }
            else
            {
                msgResult.msg = "用户类型错误，请咨询技术人员";
                return msgResult;
            }
            string sql = "select a.* from t_account_analysis a ,t_order_list o " +
                "where a.merchantOrderId = o.merchantOrderId and o.tradeTime BETWEEN str_to_date('" + dateFrom.ToString("yyyy-MM-dd") + "', '%Y-%m-%d') " +
                            "AND DATE_ADD(str_to_date('" + dateTo.ToString("yyyy-MM-dd") + "', '%Y-%m-%d'),INTERVAL 1 DAY) "+st;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt.Rows.Count>0)
            {
                string accountCode = DateTime.Now.ToString("yyyyMMddHHmmssff")+ settleAccountsParam.userCode.Substring(0,2).ToUpper();
                double totalPrice = 0;
                ArrayList al = new ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //处理添加到t_account_info
                    double price = 0;
                    string fieldName = "";
                    if (userType == "1")//供货商
                    {
                        price = Convert.ToDouble(dt.Rows[i]["supplyPrice"]);
                        fieldName = "supplyAccountCode";
                    }
                    else if (userType == "2")//采购商
                    {
                        price = Convert.ToDouble(dt.Rows[i]["purchasePrice"]);
                        fieldName = "purchaseAccountCode";
                    }
                    else if (userType == "3")//代理
                    {
                        price = Convert.ToDouble(dt.Rows[i]["profitAgent"]);
                        fieldName = "agentAccountCode";
                    }
                    else if (userType == "4")//分销
                    {
                        price = Convert.ToDouble(dt.Rows[i]["profitDealer"]);
                        fieldName = "dealerAccountCode";
                    }
                    string accountType = "1";//结算类型：1采，2退，3其他，4付

                    if (dt.Rows[i]["status"].ToString()=="1")//如果是退单，accountType变1，其他和额外支付以后补上
                    {
                        accountType = "2";
                        totalPrice -= price;
                    }
                    else
                    {
                        totalPrice += price;
                    }

                    string insql1 = "insert into t_account_info(accountCode,accountType,orderId,price) " +
                        "values('" + accountCode + "','" + accountType + "','" + dt.Rows[i]["merchantOrderId"].ToString() + "'," + price + ")";
                    al.Add(insql1);
                    //处理添加accountCode到t_account_analysis
                    string upsql = "update t_account_analysis set "+fieldName+ " = '" + accountCode + "' " +
                                   "where id = '" + dt.Rows[i]["id"].ToString() + "' ";
                    al.Add(upsql);
                    
                }
                string insql2 = "insert into t_account_list(accountCode,dateFrom," +
                                "dateTo,createTime," +
                                "usercode,price,status,inputOperator) " +
                                "values('" + accountCode + "','" + dateFrom.ToString("yyyy-MM-dd") + " 00:00:00" + "'," +
                                "'" + dateTo.ToString("yyyy-MM-dd") + " 23:59:59" + "',now()," +
                                "'" + settleAccountsParam.userCode + "',"+totalPrice.ToString()+",'0','"+userId+"')";
                al.Add(insql2);
                if (DatabaseOperationWeb.ExecuteDML(al))
                {
                    msgResult.msg = "结账完成";
                    msgResult.type = "1";
                    return msgResult;
                }
            }
            else
            {
                msgResult.msg = "没有可结账的记录！";
                return msgResult;
            }

            return msgResult;
        }



        /// <summary>
        /// 获取采购结算-运营
        /// </summary>
        /// <param name="paymentParam"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PageResult PurchasePayment(PaymentParam paymentParam, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(paymentParam.current, paymentParam.pageSize);
            pageResult.list = new List<object>();

            string ac = "";
            if (paymentParam.accountCode != null && paymentParam.accountCode != "")
            {
                ac = " and a.accountCode like '%" + paymentParam.accountCode + "%'";
            }

            string st = "";
            if (paymentParam.status == "0")
            {
                st = " and `status`='0'";
            }
            else if (paymentParam.status == "1")
            {
                st = " and `status`='1'";
            }

            string t = "";
            if (paymentParam.date != null && paymentParam.date.Length == 2)
            {
                t = " and createTime between  str_to_date('" + paymentParam.date[0] + "', '%Y-%m-%d') " +
                               "AND DATE_ADD(str_to_date('" + paymentParam.date[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }

            string sql = "SELECT a.dateFrom,a.dateTo,`status`,b.accountType,b.price,a.accountCode from t_account_list a,t_account_info b  where a.accountCode=b.accountCode " 
                 + ac + st + t ;

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

            string sql1 = "SELECT a.usercode,a.dateFrom,a.dateTo,`status`,b.accountType,b.price,a.accountCode from t_account_list a,t_account_info b  where a.accountCode=b.accountCode " 
                 + ac + st + t + " group by a.accountCode " + " order by dateTo desc   LIMIT " + (paymentParam.current - 1) * paymentParam.pageSize + "," + paymentParam.pageSize;
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "T").Tables[0];



            if (dt.Rows.Count > 0)
            {

                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    PaymentItem paymentItem = new PaymentItem();
                    paymentItem.keyId = Convert.ToString((paymentParam.current - 1) * paymentParam.pageSize + i + 1);
                    paymentItem.userCode = dt1.Rows[i]["usercode"].ToString();
                    paymentItem.accountCode = dt1.Rows[i]["accountCode"].ToString();
                    paymentItem.date = dt1.Rows[i]["dateFrom"].ToString() + "~" + dt1.Rows[i]["dateTo"].ToString();
                    if (dt1.Rows[i]["status"].ToString() != "0")
                    {
                        paymentItem.status = "1";
                        paymentItem.flag = dt1.Rows[i]["status"].ToString();
                    }
                    else
                        paymentItem.status = "0";

                    DataRow[] dr = dt.Select("accountCode='"+ paymentItem.accountCode + "'");
                    for (int j = 0; j < dr.Length; j++)
                    {
                                              
                            switch (dr[j]["accountType"].ToString())
                            {
                                case "1":
                                    paymentItem.purchasemoney += Convert.ToDouble(dr[j]["price"].ToString());
                                    break;
                                case "2":
                                    paymentItem.refundmoney += Convert.ToDouble(dr[j]["price"].ToString());
                                    break;
                                case "3":
                                    paymentItem.othermoney += Convert.ToDouble(dr[j]["price"].ToString());
                                    break;
                                case "4":
                                    paymentItem.paymoney += Convert.ToDouble(dr[j]["price"].ToString());
                                    break;
                            }
                        
                    }
                    paymentItem.refundmoney = Math.Round(paymentItem.refundmoney, 2);
                    paymentItem.purchasemoney = Math.Round(paymentItem.purchasemoney, 2);
                    paymentItem.othermoney = Math.Round(paymentItem.othermoney, 2);
                    paymentItem.paymoney = Math.Round(paymentItem.paymoney, 2);
                    pageResult.list.Add(paymentItem);
                }
                string sql3 = "SELECT count(*) from t_account_list a,t_account_info b  where a.accountCode=b.accountCode " 
                        + ac + st + t + " group by a.accountCode ";

                DataTable dt3 = DatabaseOperationWeb.ExecuteSelectDS(sql3, "t_goods_list").Tables[0];
                pageResult.pagination.total = Convert.ToInt16(dt3.Rows.Count);
            }
            return pageResult;
        }

        /// <summary>
        /// 货款结算-完成对账接口-运营
        /// </summary>
        /// <param name="paymentParam"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public MsgResult FinishReconciliation(PaymentDetailedParam paymentParam, string userId)
        {
            MsgResult msg = new MsgResult();
            string select = "select accountCode "
                 + " from t_account_list " 
                 + " where accountCode='" + paymentParam.accountCode + "'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(select, "T").Tables[0];
            if (dt.Rows.Count > 0)
            {
                string sql = ""
                                + " update t_account_list  "
                                + " set `status`='2'"
                                + " where accountCode='" + paymentParam.accountCode + "'";
                if (DatabaseOperationWeb.ExecuteDML(sql))
                    msg.type = "1";
            }
            else
            {
                msg.msg = "结算单号错误，请联系客服";
            }
            
            return msg;
        }


        /// <summary>
        /// 获取运营-手动调账
        /// </summary>
        /// <param name="paymentParam"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PageResult ManualChangeAccount(ManualChangeAccountParam paymentDetailedParam, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(paymentDetailedParam.current, paymentDetailedParam.pageSize);
            pageResult.list = new List<object>();

            string t = "";
            
            if (paymentDetailedParam.date != null && paymentDetailedParam.date.Length == 2)
            {
                DateTime dt2 = DateTime.Parse(paymentDetailedParam.date[0]);
                DateTime dt3 = DateTime.Parse(paymentDetailedParam.date[1]);
                string m = dt2.Month.ToString();

                t = " and a.createtime  between  str_to_date('"+ paymentDetailedParam.date[0] + "', '%Y-%m-%d')" 
                 + "AND DATE_ADD(str_to_date('" + paymentDetailedParam.date[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }

            string sql = "select a.`year`,a.`month`,a.price,a.detail,b.adjustName,c.username,(select customersCode  from t_contract_list d  where d.userCode=a.userCode) customersCode"
                      + " from t_account_adjust a,t_base_adjust b,t_user_list c"
                      + " where a.adjustType = b.adjustType and  a.userCode=c.usercode" + t + "  ORDER BY `year` DESC,`month` DESC  limit "
                      + (paymentDetailedParam.current - 1) * paymentDetailedParam.pageSize + "," + paymentDetailedParam.pageSize;

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    PaymentOtherDetailedItem paymentOtherDetailedItem = new PaymentOtherDetailedItem();
                    paymentOtherDetailedItem.keyId = Convert.ToString((paymentDetailedParam.current - 1) * paymentDetailedParam.pageSize + i + 1);
                    paymentOtherDetailedItem.month = dt.Rows[i]["month"].ToString();
                    paymentOtherDetailedItem.year = dt.Rows[i]["year"].ToString();
                    paymentOtherDetailedItem.price = Math.Round(Convert.ToDouble(dt.Rows[i]["price"].ToString()), 2);
                    paymentOtherDetailedItem.adjustName = dt.Rows[i]["adjustName"].ToString();
                    paymentOtherDetailedItem.detail = dt.Rows[i]["detail"].ToString();
                    paymentOtherDetailedItem.userName= dt.Rows[i]["username"].ToString();
                    paymentOtherDetailedItem.customersCode= dt.Rows[i]["customersCode"].ToString();
                    pageResult.list.Add(paymentOtherDetailedItem);
                }

            }
            string sql1 = "select count(*) from t_account_adjust a,t_base_adjust b " +
                          "where a.adjustType = b.adjustType  " + t;
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t").Tables[0];
            pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);
            return pageResult;
        }


        /// <summary>
        /// 财务管理-创建手动调账-调整事项下拉接口-运营
        /// </summary>
        /// <param name="paymentParam"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object AdjustmentMatters(string userId)
        {
            List<object> list = new List<object>();
            string sql = ""
                + "select adjustType,adjustName "
                + " from t_base_adjust";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql,"T").Tables[0];
            if (dt.Rows.Count>0)
            {
                for (int i=0;i< dt.Rows.Count;i++)
                {
                    AdjustmentMattersItem amp = new AdjustmentMattersItem();
                    amp.adjustCode = dt.Rows[i]["adjustType"].ToString();
                    amp.adjustName= dt.Rows[i]["adjustName"].ToString();
                    list.Add(amp);
                }
            }
            return list;
        }



        /// <summary>
        /// 财务管理-创建手动调账-获取客商信息接口-运营
        /// </summary>
        /// <param name="paymentParam"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object CustomersInformation(CustomersInformationParam cip,string userId)
        {
            List<object> list = new List<object>();
            string customersCode = "";
            if (cip.customersCode!=null && cip.customersCode!="")
            {
                customersCode = " a.customersCode like '%"+ cip.customersCode + "'% and ";
            }
            string username = "";
            if (cip.userName!=null && cip.userName!="")
            {
                username = "  b.username like '%"+ cip.userName + "%' and ";
            }

            string sql = ""
                + " select a.customersCode,b.usercode"
                + " from t_contract_list a,t_user_list b"
                + " where " + username + customersCode + " a.userCode=b.usercode and (b.usertype='1' or b.usertype='2')";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql,"T").Tables[0];
            if (dt.Rows.Count>0)
            {
                for (int i=0;i<dt.Rows.Count;i++)
                {
                    CustomersInformationItem cii = new CustomersInformationItem();
                    cii.customersCode = dt.Rows[i]["customersCode"].ToString();
                    cii.userCode= dt.Rows[i]["userCode"].ToString();
                    list.Add(cii);
                }
            }

            return list;
        }


        /// <summary>
        /// 财务管理-创建手动调账-获取客商信息接口-运营
        /// </summary>
        /// <param name="paymentParam"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public MsgResult CreateAccount(CreateAccountParam cip, string userId)
        {
            MsgResult msg = new MsgResult();
            string year = cip.date.Split("-")[0];
            string month = cip.date.Split("-")[1];
            string createTime= DateTime.Now.ToString("yyyyMMddhhmmssff");
            string adjustCode = "ADJUST" + createTime;
            string insert = ""
                + "insert into t_account_adjust(adjustCode,userCode,year,month,price,adjustType,detail,createtime)"
                + "  values('"+ adjustCode + "','"+ cip.userCode + "','"+ year + "','" + month + "','" + cip.price + "','" + cip.adjustType + "','" + cip.detail + "','" + createTime + "')";
            if (DatabaseOperationWeb.ExecuteDML(insert))
                msg.type = "1";
            return msg;

        }
    }
}
