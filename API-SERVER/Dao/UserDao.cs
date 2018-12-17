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
        #region 原java版B2B
        public LoginItem Validate(string userId, string pwd)
        {
            string sql = "SELECT  t1.usercode as userId,t2.role_name as currentAuthority " +
                "FROM t_user_list t1,t_sys_role t2, t_user_role t3 " +
                "WHERE t1.id = t3.user_id AND t2.role_id = t3.role_id AND t1.flag = '1' " +
                "AND t1.usercode ='" + userId + "' AND t1.pwd='" + pwd + "'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                LoginItem loginItem = new LoginItem();
                loginItem.currentAuthority = dt.Rows[0]["currentAuthority"].ToString();
                loginItem.token.userId = dt.Rows[0]["userId"].ToString();
                return loginItem;
            }
            else
            {
                return null;
            }
        }
        public CurrentUserItem CurrentUser(string userId)
        {
            string sql = "SELECT  l.username AS `name`,l.avatar AS avatar,l.usercode AS userid,s.notifyCount AS notifyCount " +
                "FROM t_user_list l, " +
                "(SELECT COUNT(*) AS notifyCount FROM t_user_list t1, t_user_message t2 " +
                                                "WHERE t1.id = t2.userid AND t1.usercode ='" + userId + "' AND t2.`status`='0') s " +
                "WHERE l.usercode ='" + userId + "' AND l.flag='1' ";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                CurrentUserItem currentUserItem = new CurrentUserItem();
                currentUserItem.name = dt.Rows[0]["name"].ToString();
                currentUserItem.avatar = dt.Rows[0]["avatar"].ToString();
                currentUserItem.userid = dt.Rows[0]["userid"].ToString();
                currentUserItem.notifyCount = dt.Rows[0]["notifyCount"].ToString();
                return currentUserItem;
            }
            else
            {
                return new CurrentUserItem();
            }
        }
        public List<Menu> menu()
        {
            List<Menu> menuList = new List<Menu>();
            string sql = "SELECT  t1.id,t1.menuName AS `name`,t1.icon,t1.menuUrl AS path, authority as authoritys " +
                "FROM t_sys_menu t1 " +
                "WHERE t1.menuPid = 0 and t1.flag = '1' " +
                "ORDER BY t1.sort asc ";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Menu menu = new Menu();
                menu.name = dt.Rows[i]["name"].ToString();
                menu.icon = dt.Rows[i]["icon"].ToString();
                menu.path = dt.Rows[i]["path"].ToString();
                string authoritys = dt.Rows[i]["authoritys"].ToString();
                if (authoritys != null && authoritys != "")
                {
                    string[] st = authoritys.Split(',');
                    foreach (string s in st)
                    {
                        menu.authority.Add(s);
                    }
                }
                string sql1 = "SELECT t1.menuName AS `name` ,t1.menuUrl AS path, authority as authoritys " +
                    "FROM t_sys_menu t1 " +
                    "WHERE t1.menuPid = 1 and t1.parent ='" + dt.Rows[i]["id"].ToString() + "' and t1.flag='1' " +
                    "ORDER BY t1.sort asc";
                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "Table").Tables[0];
                for (int j = 0; j < dt1.Rows.Count; j++)
                {
                    MenuChildren menuChildren = new MenuChildren();
                    menuChildren.name = dt1.Rows[j]["name"].ToString();
                    menuChildren.path = dt1.Rows[j]["path"].ToString();
                    string authoritys1 = dt1.Rows[j]["authoritys"].ToString();
                    if (authoritys1 != null && authoritys1 != "")
                    {
                        string[] st = authoritys1.Split(',');
                        foreach (string s in st)
                        {
                            menuChildren.authority.Add(s);
                        }
                    }
                    menu.children.Add(menuChildren);
                }
                menuList.Add(menu);
            }
            return menuList;
        }
        public List<MessageEntity> messagelist(string userId)
        {
            List<MessageEntity> messageList = new List<MessageEntity>();
            string sql = "SELECT  t2.id,t2.sendimg AS avatar,t2.title,t2.sendTime AS datetime,t2.messagetype AS `type` " +
                "FROM t_user_list t1,t_user_message t2 " +
                "WHERE t1.id = t2.userid AND t1.usercode ='" + userId + "' AND t2.`status`='0' " +
                "ORDER BY t2.sendTime DESC";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                MessageEntity messageEntity = new MessageEntity();
                messageEntity.id = dt.Rows[i]["id"].ToString();
                messageEntity.avatar = dt.Rows[i]["avatar"].ToString();
                messageEntity.title = dt.Rows[i]["title"].ToString();
                messageEntity.datetime = dt.Rows[i]["datetime"].ToString();
                messageEntity.type = dt.Rows[i]["type"].ToString();
                messageList.Add(messageEntity);
            }
            return messageList;
        }
        public MessageCountEntity messagecount(string userId)
        {
            MessageCountEntity messageCountEntity = new MessageCountEntity();
            string sql = "SELECT COUNT(*) AS notifyCount " +
                "FROM t_user_list t1,t_user_message t2 " +
                "WHERE t1.id = t2.userid AND t1.usercode ='" + userId + "' AND t2.`status`='0' ";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                messageCountEntity.notifyCount = Convert.ToInt32(dt.Rows[0][0].ToString());
            }
            return messageCountEntity;
        }
        public string messageempty(string userId, string type)
        {
            string sql = "UPDATE  t_user_message m, " +
                "(SELECT t2.id FROM t_user_list t1, t_user_message t2 " +
                             "WHERE t1.id = t2.userid AND t1.usercode ='" + userId + "' AND t2.`status`='0' " +
                                    "AND t2.messagetype='" + type + "' ORDER BY t2.sendTime DESC) t " +
                "SET m.status = '1' WHERE m.id = t.id";
            if (DatabaseOperationWeb.ExecuteDML(sql))
            {
                return "清空成功.";
            }
            else
            {
                return "清空失败";
            }
        }
        public bool insertUser(string mail, string password, string type, string ofAgent, string verifycode, string flag, string avatar)
        {
            string sql = "INSERT INTO t_user_list(usercode, pwd, usertype, ofAgent, verifycode, flag, avatar) " +
                "VALUES('" + mail + "', '" + password + "', '" + type + "', '" + ofAgent + "', '" + verifycode + "', '" + flag + "', '" + avatar + "')";
            return DatabaseOperationWeb.ExecuteDML(sql);
        }
        public bool insertUserRole(string userId, int role_id)
        {
            string sql = "SELECT id FROM t_user_list  WHERE  usercode ='" + userId + "' ";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                string sql1 = "INSERT INTO t_user_role(user_id,role_id)  VALUES('" + dt.Rows[0][0].ToString() + "', '" + role_id + "')";
                return DatabaseOperationWeb.ExecuteDML(sql1);
            }
            else
            {
                return false;
            }
        }
        public bool updateUserPwd(string mail, string password)
        {
            string sql = "update t_user_list set pwd='" + password + "' where usercode ='" + mail + "'";
            return DatabaseOperationWeb.ExecuteDML(sql);
        }

        public UserStatus registerstatus(string userId)
        {
            UserStatus userStatus = new UserStatus();
            string sql = "SELECT t.usertype,t.company,t.contact,t.email,t.tel,t.website, t.img1,t.img2,t.img3,t.three,t.verifycode " +
                "FROM t_user_list t WHERE t.usercode ='" + userId + "' ";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                userStatus.usertype = dt.Rows[0]["usertype"].ToString();
                userStatus.company = dt.Rows[0]["company"].ToString();
                userStatus.contact = dt.Rows[0]["contact"].ToString();
                userStatus.email = dt.Rows[0]["email"].ToString();
                userStatus.website = dt.Rows[0]["website"].ToString();
                userStatus.tel = dt.Rows[0]["tel"].ToString();
                userStatus.img1 = dt.Rows[0]["img1"].ToString();
                userStatus.img2 = dt.Rows[0]["img2"].ToString();
                userStatus.img3 = dt.Rows[0]["img3"].ToString();
                userStatus.three = dt.Rows[0]["three"].ToString();
                userStatus.verifycode = dt.Rows[0]["verifycode"].ToString();
            }
            return userStatus;
        }
        public bool updatetUserRoleRegister(string userId, string roleId)
        {
            string sql = "UPDATE  t_user_role t SET t.role_id = '" + roleId + "' WHERE t.user_id ='" + userId + "'";
            return DatabaseOperationWeb.ExecuteDML(sql);
        }
        public bool updatetUserStatusById(string verifycode, string userId, string failmark)
        {
            string sql = "UPDATE t_user_list t SET t.verifycode =='" + verifycode + "',t.failmark=='" + failmark + "' " +
                "WHERE t.id =='" + userId + "'";
            return DatabaseOperationWeb.ExecuteDML(sql);
        }
        public PageResult getPageUserForCheck(UserParam userParam)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(userParam.current, userParam.pageSize);
            pageResult.list = new List<Object>();
            string st = "";
            if (userParam.usertype != null && userParam.usertype != "")
            {
                st += " and usertype = '" + userParam.usertype + "'";
            }
            if (userParam.usercode != null && userParam.usercode != "")
            {
                st += " and usercode like '%" + userParam.usercode + "%'";
            }
            string sql = "SELECT t.* FROM t_user_list t WHERE t.verifycode = '3' AND t.flag = '1' " + st
                       + " ORDER BY t.createtime desc LIMIT " + (userParam.current - 1) * userParam.pageSize + "," + userParam.pageSize + ";";

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_user_list").Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    UserParam userParam1 = new UserParam();

                    userParam1.id = Convert.ToInt32(dt.Rows[i]["id"]);
                    userParam1.usercode = dt.Rows[i]["usercode"].ToString();
                    userParam1.usertype = dt.Rows[i]["usertype"].ToString();
                    userParam1.username = dt.Rows[i]["username"].ToString();
                    userParam1.avatar = dt.Rows[i]["avatar"].ToString();
                    userParam1.company = dt.Rows[i]["company"].ToString();
                    userParam1.contact = dt.Rows[i]["contact"].ToString();
                    userParam1.website = dt.Rows[i]["website"].ToString();
                    userParam1.tel = dt.Rows[i]["tel"].ToString();
                    userParam1.email = dt.Rows[i]["email"].ToString();
                    userParam1.three = dt.Rows[i]["three"].ToString();
                    userParam1.img1 = dt.Rows[i]["img1"].ToString();
                    userParam1.img2 = dt.Rows[i]["img2"].ToString();
                    userParam1.img3 = dt.Rows[i]["img3"].ToString();
                    userParam1.createtime = dt.Rows[i]["createtime"].ToString();
                    userParam1.verifycode = dt.Rows[i]["verifycode"].ToString();
                    userParam1.flag = dt.Rows[i]["flag"].ToString();
                    userParam1.lasttime = dt.Rows[i]["lasttime"].ToString();
                    pageResult.list.Add(userParam1);
                }
                string sql1 = "SELECT count(*) FROM t_user_list t WHERE t.verifycode = '3' AND t.flag = '1' " + st;
                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_user_list").Tables[0];
                pageResult.pagination.total = Convert.ToInt32(dt1.Rows[0][0]);
            }

            return pageResult;
        }
        public PageResult getUserList(UserParam userParam)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(userParam.current, userParam.pageSize);
            pageResult.list = new List<Object>();
            string st = "";
            if (userParam.company != null && userParam.company != "")
            {
                st += " and company like '%" + userParam.company + "'%";
            }
            if (userParam.verifycode != null && userParam.verifycode != "")
            {
                st += " and verifycode = '" + userParam.verifycode + "'";
            }
            if (userParam.usercode != null && userParam.usercode != "")
            {
                st += " and usercode like '%" + userParam.usercode + "'%";
            }
            string sql = "SELECT t.* FROM t_user_list t WHERE usertype <> 0 " + st
                       + " ORDER BY t.createtime desc LIMIT " + (userParam.current - 1) * userParam.pageSize + "," + userParam.pageSize + ";";

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_user_list").Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    UserParam userParam1 = new UserParam();

                    userParam1.id = Convert.ToInt32(dt.Rows[i]["id"]);
                    userParam1.usercode = dt.Rows[i]["usercode"].ToString();
                    userParam1.usertype = dt.Rows[i]["usertype"].ToString();
                    userParam1.username = dt.Rows[i]["username"].ToString();
                    userParam1.avatar = dt.Rows[i]["avatar"].ToString();
                    userParam1.company = dt.Rows[i]["company"].ToString();
                    userParam1.contact = dt.Rows[i]["contact"].ToString();
                    userParam1.website = dt.Rows[i]["website"].ToString();
                    userParam1.tel = dt.Rows[i]["tel"].ToString();
                    userParam1.email = dt.Rows[i]["email"].ToString();
                    userParam1.three = dt.Rows[i]["three"].ToString();
                    userParam1.img1 = dt.Rows[i]["img1"].ToString();
                    userParam1.img2 = dt.Rows[i]["img2"].ToString();
                    userParam1.img3 = dt.Rows[i]["img3"].ToString();
                    userParam1.createtime = dt.Rows[i]["createtime"].ToString();
                    userParam1.verifycode = dt.Rows[i]["verifycode"].ToString();
                    userParam1.flag = dt.Rows[i]["flag"].ToString();
                    userParam1.lasttime = dt.Rows[i]["lasttime"].ToString();
                    pageResult.list.Add(userParam1);
                }
                string sql1 = "SELECT t.* FROM t_user_list t WHERE usertype <> 0 " + st;
                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_user_list").Tables[0];
                pageResult.pagination.total = Convert.ToInt32(dt1.Rows[0][0]);
            }

            return pageResult;
        }

        public UserParam getUserDetails(string userId)
        {
            UserParam userParam1 = new UserParam();
            string sql = "SELECT t.* FROM t_user_list t WHERE t.id= " + userId;

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_user_list").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                userParam1.id = Convert.ToInt32(dt.Rows[0]["id"]);
                userParam1.usercode = dt.Rows[0]["usercode"].ToString();
                userParam1.usertype = dt.Rows[0]["usertype"].ToString();
                userParam1.username = dt.Rows[0]["username"].ToString();
                userParam1.avatar = dt.Rows[0]["avatar"].ToString();
                userParam1.company = dt.Rows[0]["company"].ToString();
                userParam1.contact = dt.Rows[0]["contact"].ToString();
                userParam1.website = dt.Rows[0]["website"].ToString();
                userParam1.tel = dt.Rows[0]["tel"].ToString();
                userParam1.email = dt.Rows[0]["email"].ToString();
                userParam1.three = dt.Rows[0]["three"].ToString();
                userParam1.img1 = dt.Rows[0]["img1"].ToString();
                userParam1.img2 = dt.Rows[0]["img2"].ToString();
                userParam1.img3 = dt.Rows[0]["img3"].ToString();
                userParam1.createtime = dt.Rows[0]["createtime"].ToString();
                userParam1.verifycode = dt.Rows[0]["verifycode"].ToString();
                userParam1.flag = dt.Rows[0]["flag"].ToString();
                userParam1.lasttime = dt.Rows[0]["lasttime"].ToString();
            }
            return userParam1;
        }
        public MsgResult updateUserStatusByUserId(string userId, string flag)
        {
            MsgResult msg = new MsgResult();
            string sql = "UPDATE t_user_list t SET t.flag ='"+flag+ "' WHERE  t.id ='" + userId + "'";
            if (DatabaseOperationWeb.ExecuteDML(sql))
            {
                if (flag=="0")
                {
                    msg.msg = "冻结成功";
                }
                else
                {
                    msg.msg = "解冻成功";
                }
                msg.type = "1";
            }
            else
            {
                if (flag == "0")
                {
                    msg.msg = "冻结失败";
                }
                else
                {
                    msg.msg = "解冻失败";
                }

            }
            return msg;
        }

        #endregion
        #region 原
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
        public string getUserOrderType(string userId)
        {
            string sql = "SELECT orderType  FROM t_user_list WHERE usercode ='" + userId + "' ";
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
        public string getUserType(string userId)
        {
            string sql = "SELECT usertype  FROM t_user_list WHERE usercode ='" + userId + "' ";
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
            string ut = "and usertype='" + ocp.usertype + "'";
            if (ocp.usertype != 1 && ocp.usertype != 2 && ocp.usertype != 3)
            {
                ut = "and (usertype=1 or usertype=2 or usertype=3) ";
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

            string sql = "select id,usercode,username,company,contact,tel,email from t_user_list where usercode='" + agent + "'" + st + ut +
                "and verifycode=4  and flag=1  limit " + (ocp.current - 1) * ocp.pageSize + "," + ocp.pageSize;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_user_list").Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    OperateCustomerItem oc = new OperateCustomerItem();
                    oc.keyid = Convert.ToString((ocp.current - 1) * ocp.pageSize + i + 1);
                    oc.id = dt.Rows[i]["id"].ToString();
                    oc.username = dt.Rows[i]["username"].ToString();
                    oc.company = dt.Rows[i]["company"].ToString();
                    oc.contact = dt.Rows[i]["contact"].ToString();
                    oc.tel = dt.Rows[i]["tel"].ToString();
                    oc.email = dt.Rows[i]["email"].ToString();

                    pg.list.Add(oc);
                }
            }
            string sql1 = "select count(*) from t_user_list where '" + agent + "'" + st + ut + "and verifycode=4  and flag=1 ";
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_user_list").Tables[0];
            pg.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);

            return pg;
        }
        #endregion
    }
}
