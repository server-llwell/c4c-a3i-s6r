using API_SERVER.Buss;
using API_SERVER.Common;
using Com.ACBC.Framework.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
                    balanceTotalItem.totalPurchase +=  Convert.ToDouble(dt1.Rows[i]["profitDealer"]);
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
            string sql = "select * from t_user_list where ofAgent='"+userCode+"'";
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
            string sql = "select a.id,a.merchantOrderId,o.tradeTime,o.waybilltime,o.tradeAmount,a.createTime,a.supplierAgentPrice,o.purchaserCode " +
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
                    balanceItem.tradeAmount = Convert.ToDouble(dt.Rows[i]["tradeAmount"]);
                    balanceItem.waybillTime = dt.Rows[i]["waybilltime"].ToString();
                    balanceItem.profit = Convert.ToDouble(dt.Rows[i]["supplierAgentPrice"]);
                    balanceItem.distribution = dt.Rows[i]["purchaserCode"].ToString();
                    balanceItem.payType = "1";
                    pageResult.list.Add(balanceItem);
                }
                string sql1 = "SELECT o.tradeAmount,a.createTime,a.supplierAgentPrice " +
                              "from t_order_list o ,t_account_analysis a " +
                              "where o.merchantOrderId = a.merchantOrderId  and a.supplierAgentPrice is not null " + st;
                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "Table").Tables[0];
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    balanceTotalItem.totalSales += Convert.ToDouble(dt1.Rows[i]["tradeAmount"]);
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
            string sql = "select a.id,a.merchantOrderId,o.tradeTime,o.waybilltime,o.tradeAmount,a.createTime,a.purchaseAgentPrice,o.purchaserCode " +
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
                    balanceItem.tradeAmount = Convert.ToDouble(dt.Rows[i]["tradeAmount"]);
                    balanceItem.waybillTime = dt.Rows[i]["waybilltime"].ToString();
                    balanceItem.profit = Convert.ToDouble(dt.Rows[i]["purchaseAgentPrice"]);
                    balanceItem.distribution = dt.Rows[i]["purchaserCode"].ToString();
                    balanceItem.payType = "1";
                    pageResult.list.Add(balanceItem);
                }
                string sql1 = "SELECT o.tradeAmount,a.createTime,a.purchaseAgentPrice " +
                              "from t_order_list o ,t_account_analysis a " +
                              "where o.merchantOrderId = a.merchantOrderId and a.purchaseAgentPrice is not null " + st;
                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "Table").Tables[0];
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    balanceTotalItem.totalSales += Convert.ToDouble(dt1.Rows[i]["tradeAmount"]);
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
                    balanceTotalItem.totalProfit += Convert.ToDouble(dt1.Rows[i]["purchaseAgentPrice"]);
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
                string sql1 = "select * from t_account_list  where usercode = '" + userId + "' " + st;
                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "Table").Tables[0];
                pageResult.pagination.total = dt1.Rows.Count;
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
                    if (dt1.Rows[i]["status"].ToString()=="0")
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
    }
}
