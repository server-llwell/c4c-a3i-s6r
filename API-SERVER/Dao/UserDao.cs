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
        public PageResult GetOperateCustomer(OperateCustomerParam ocp, string agent)
        {
            PageResult pg = new PageResult();
            pg.pagination = new Page(ocp.current, ocp.pageSize);
            pg.list = new List<object>();
            string  ut="and usertype='"+ ocp.usertype+"'";
            if (ocp.usertype != 1 && ocp.usertype != 2 && ocp.usertype!=3)
            {
                ut = "and usertype=1 or usertype=2 or usertype=3";
            }
            string st = "";
            if (ocp.search != null && ocp.search != "")
            {
                st += " and (username like '%" + ocp.search + "%' " +
                    " or company  like'%" + ocp.search + "%' " +
                    " or tel  like'%" + ocp.search + "%' " +
                    " or email  like'%" + ocp.search + "%' " +
                     " or contact like '%" + ocp.search + "%') ";
            }

            string sql = "select id,usercode,username,company,contact,tel,email from t_user_list where usercode='"+agent+"'"+ st+ ut+
                "and verifycode=4  and flag=1  limit " + (ocp.current-1)*ocp.pageSize +","+ocp.pageSize;
            DataTable dt= DatabaseOperationWeb.ExecuteSelectDS(sql, "t_user_list").Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int i=0;i<dt.Rows.Count;i++)
                {
                    OperateCustomerItem oc = new OperateCustomerItem();
                    oc.keyid = Convert.ToString((ocp.current-1)* ocp.pageSize + i + 1);
                    oc.id = dt.Rows[i]["id"].ToString();
                    oc.username = dt.Rows[i]["username"].ToString();
                    oc.company = dt.Rows[i]["company"].ToString();
                    oc.contact = dt.Rows[i]["contact"].ToString();
                    oc.tel = dt.Rows[i]["tel"].ToString();
                    oc.email = dt.Rows[i]["email"].ToString();

                    pg.list.Add(oc);
                }
            }
            string sql1 = "select count(*) from t_user_list where '" + agent+"'" + st + ut + "and verifycode=4  and flag=1 ";
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_user_list").Tables[0];
            pg.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);

            return pg;
        }
    }
}
