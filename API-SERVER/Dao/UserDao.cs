using API_SERVER.Buss;
using Com.ACBC.Framework.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace API_SERVER.Dao
{
    public class UserDao
    {
        private string path = System.Environment.CurrentDirectory;

        public UserDao()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }
        public bool isAuth(string url, string userCode)
        {
            string sql = "SELECT COUNT(*) " +
                "FROM t_user_list t1,t_api_menu t2, t_sys_role t3,t_api_menu_role t4, t_user_role t5 " +
                "WHERE t1.id = t5.user_id AND t2.menu_id = t4.menu_id AND t3.role_id = t5.role_id " +
                "AND t3.role_id = t4.role_id AND t1.flag = '1' AND t1.usercode ='" + userCode + "' AND t2.menu_url='" + url + "'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_daigou_user").Tables[0];

            if (Convert.ToInt16(dt.Rows[0][0]) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public string getUserType(string userId)
        {
            string sql = "SELECT usertype  FROM t_user_list WHERE usercode ='" + userId + "' ";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_daigou_user").Tables[0];

            if (dt.Rows.Count>0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
            {
                return null;
            }
        }
        public string getUserId(string userCode)
        {
            string sql = "SELECT id  FROM t_user_list WHERE usercode ='" + userCode + "' ";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_daigou_user").Tables[0];

            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
            {
                return null;
            }
        }
        public string getUserCode(string userId)
        {
            string sql = "SELECT usercode  FROM t_user_list WHERE id ='" + userId + "' ";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_daigou_user").Tables[0];

            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
            {
                return null;
            }
        }
        public string getOfAgent(string userCode)
        {
            string sql = "SELECT ofAgent  FROM t_user_list WHERE usercode ='" + userCode + "' ";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_daigou_user").Tables[0];

            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
            {
                return null;
            }
        }
        public string getDefaultAgent()
        {
            string sql = "SELECT usercode  FROM t_user_list WHERE defaultAgent ='1' ";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_daigou_user").Tables[0];

            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
            {
                return null;
            }
        }
        public string getOrderPageQRCode(string userCode)
        {
            string sql = "SELECT orderPageQRCode  FROM t_user_list " +
                "WHERE usercode in (SELECT ofAgent  FROM t_user_list WHERE usercode ='" + userCode + "' ) ";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_daigou_user").Tables[0];

            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获运营客户
        /// </summary>
        /// <returns></returns>
        public string GetOperateCustomer(OperateCustomerParam ocp, string agent)
        {
            return "";
        }
    }
}
