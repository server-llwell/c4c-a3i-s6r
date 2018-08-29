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
            string sql = "select a.merchantOrderId,o.tradeTime,o.tradeAmount,a.createTime,a.supplyPrice,a.profitAgent,a.profitDealer,a.platformPrice " +
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

            string sql = "select a.merchantOrderId,o.tradeTime,a.createTime,a.supplyPrice " +
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
            string sql = "select a.merchantOrderId,o.tradeTime,o.tradeAmount,a.createTime,a.purchasePrice " +
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
            string sql = "select a.merchantOrderId,o.tradeTime,o.tradeAmount,a.createTime,a.profitAgent,a.profitDealer,o.distributionCode " +
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
            string sql = "select a.merchantOrderId,o.tradeTime,o.tradeAmount,a.createTime,a.profitDealer " +
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
    }
}
