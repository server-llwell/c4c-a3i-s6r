using API_SERVER.Buss;
using API_SERVER.Common;
using Com.ACBC.Framework.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static API_SERVER.Buss.AgreementBuss;

namespace API_SERVER.Dao
{
    public class AgreementDao
    {
        private string path = System.Environment.CurrentDirectory;

        public AgreementDao()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }

        public PageResult ContractInformation(string userCode)
        {
            PageResult pageResult = new PageResult();
            pageResult.list = new List<object>();

            string sql = "select contractCode,cycle,platformType from t_contract_list a,t_base_platform b where a.model=b.platformId and userCode='" + userCode+"'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql,"table").Tables[0];
            ContractInformationItem cii = new ContractInformationItem();
            if (dt.Rows.Count > 0)
            {
                

                cii.contractCode = dt.Rows[0]["contractCode"].ToString();
                switch (dt.Rows[0]["cycle"].ToString())
                {
                    case "1":
                        cii.cycle = "实时";
                        break;
                    case "2":
                        cii.cycle = "日结";
                        break;
                    case "3":
                        cii.cycle = "周结";
                        break;
                    case "4":
                        cii.cycle = "半月结";
                        break;
                    case "5":
                        cii.cycle = "月结";
                        break;
                    case "6":
                        cii.cycle = "季结";
                        break;
                    case "7":
                        cii.cycle = "半年结";
                        break;
                    case "8":
                        cii.cycle = "年结";
                        break;

                    default: /* 可选的 */
                        cii.cycle = "其他";
                        break;
                }

                cii.model = dt.Rows[0]["platformType"].ToString();
              

                pageResult.item = cii;
            }
            else
            {
                pageResult.item = cii;
            }

            string sql1 = "select imgUrl from t_contract_img a,t_contract_list b  where a.contractCode=b.contractCode and b.userCode='"+ userCode+"'";
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1,"table").Tables[0];
            if (dt1.Rows.Count > 0)
            {
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    ContractInformationlist cil = new ContractInformationlist();
                    cil.imgUrl = dt1.Rows[i]["imgUrl"].ToString();
                    pageResult.list.Add(cil);
                }

            }
           

            return pageResult;
        }

        /// <summary>
        /// 获取合同详情-供应商
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Object ContractDetails( string userCode)
        {
            string sql = ""
                + "select * from (select a.contractCode,a.customersCode,b.username,a.createTime,a.cycle,c.platformType,a.beginTime,a.endTime,a.platformPoint,a.supplierPoint,a.purchasePoint,a.freightBelong,a.taxBelong,a.merchantName,a.depositBank,a.depositBankSubbranch,a.bankCard"
                + " from  t_contract_list a,t_user_list b, t_base_platform c"
                + " where a.userCode=b.usercode and a.model=c.platformId  and b.usercode='"+ userCode + "') x LEFT JOIN (select contractCode,imgUrl from t_contract_img ) y on x.contractCode=y.contractCode";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

            ContractDetailsItem cdi = new ContractDetailsItem();
            cdi.list = new List<object>();

            if (dt.Rows.Count > 0)
            {
                cdi.customersCode = dt.Rows[0]["customersCode"].ToString();
                cdi.userName = dt.Rows[0]["username"].ToString();
                cdi.contractDuration = Convert.ToDateTime(dt.Rows[0]["beginTime"]).ToString("yyyy-MM-dd") + " 至 " + Convert.ToDateTime(dt.Rows[0]["endTime"]).ToString("yyyy-MM-dd");
                cdi.createTime = dt.Rows[0]["createTime"].ToString();
                cdi.freightBelong = dt.Rows[0]["freightBelong"].ToString();
                cdi.taxBelong = dt.Rows[0]["taxBelong"].ToString();
                cdi.merchantName = dt.Rows[0]["merchantName"].ToString();
                cdi.depositBank = dt.Rows[0]["depositBank"].ToString();
                cdi.depositBankSubbranch = dt.Rows[0]["depositBankSubbranch"].ToString();
                cdi.bankCard = dt.Rows[0]["bankCard"].ToString();
                switch (dt.Rows[0]["cycle"].ToString())
                {
                    case "1":
                        cdi.cycle = "实时";
                        break;
                    case "2":
                        cdi.cycle = "日结";
                        break;
                    case "3":
                        cdi.cycle = "周结";
                        break;
                    case "4":
                        cdi.cycle = "半月结";
                        break;
                    case "5":
                        cdi.cycle = "月结";
                        break;
                    case "6":
                        cdi.cycle = "季结";
                        break;
                    case "7":
                        cdi.cycle = "半年结";
                        break;
                    case "8":
                        cdi.cycle = "年结";
                        break;

                    default: /* 可选的 */
                        cdi.cycle = "其他";
                        break;
                }

                cdi.model = dt.Rows[0]["platformType"].ToString();
                cdi.platformPoint = dt.Rows[0]["platformPoint"].ToString();
                cdi.purchasePoint = dt.Rows[0]["purchasePoint"].ToString();
                cdi.supplierPoint = dt.Rows[0]["supplierPoint"].ToString();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cdi.list.Add(dt.Rows[i]["imgUrl"].ToString());
                }
            }
            return cdi;
        }


        /// <summary>
        /// 获取合同列表-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PageResult ContractList(ContractListParam clp, string userCode)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(clp.current,clp.pageSize);
            pageResult.list = new List<object>();

            string customersCode = "";
            if(clp.customersCode != null && clp.customersCode != "")
                customersCode= " (a.customersCode like '%" + clp.customersCode + "%') and ";
            string userName = "";
            if (clp.userName != null && clp.userName != "")
                userName = " ( b.username like '%"+ clp.userName + "%') and ";
            string model = "";
            if (clp.model != null && clp.model != "")
                model = " a.model='"+ clp.model + "' and ";
            string contractType = "";
            if (clp.contractType != null && clp.contractType != "")
                contractType = " a.contractType='"+ clp.contractType + "' and ";
            string contractCode = "";
            if (clp.contractCode != null && clp.contractCode != "")
                contractCode = " a.contractCode like '%"+ clp.contractCode + "%' and ";
            string cycle = "";
            if (clp.cycle != null && clp.cycle != "")
                cycle = " a.cycle='"+ clp.cycle + "' and ";
            string status = "";
            if (clp.status != null && clp.status != "")
                status = " a.`status`='"+ clp.status + "' and ";
            string sql = ""
                + "select a.customersCode,a.contractCode,a.userCode,b.username,a.contractType,a.cycle,a.model,a.`status`,a.createTime,a.endTime"
                + " from t_contract_list a,t_user_list b "
                + " where "+ customersCode + userName + model + contractType + contractCode + cycle + status + " a.userCode=b.usercode  "
                + "order by  a.createTime desc limit " + (clp.current - 1) * clp.pageSize + "," + clp.pageSize;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql,"T").Tables[0];
            ArrayList list = new ArrayList();
            if (dt.Rows.Count>0)
            {
                for (int i=0;i< dt.Rows.Count;i++)
                {
                    ContractListItem contractListItem = new ContractListItem();
                    contractListItem.keyId = Convert.ToString((clp.current - 1) * clp.pageSize + i + 1);
                    contractListItem.contractType = dt.Rows[i]["contractType"].ToString();
                    contractListItem.createTime= Convert.ToDateTime(dt.Rows[i]["createTime"]).ToString("yyyy.MM.dd");
                    contractListItem.contractCode = dt.Rows[i]["contractCode"].ToString();
                    contractListItem.cycle = dt.Rows[i]["cycle"].ToString();
                    contractListItem.model = dt.Rows[i]["model"].ToString();
                    contractListItem.customersCode = dt.Rows[i]["customersCode"].ToString();
                    contractListItem.userName = dt.Rows[i]["username"].ToString();
                    contractListItem.status = dt.Rows[i]["status"].ToString();
                    DateTime dateTime = DateTime.Now;
                    
                    int day= (Convert.ToDateTime(dt.Rows[i]["endTime"]) - dateTime).Days;
                    if (day > 0 && 30 > day)
                    {
                        if (contractListItem.status != "2")
                        {
                            string update = ""
                                + " update t_contract_list"
                                + " set `status`='2'"
                                + " where contractCode='" + contractListItem.contractCode + "'";
                            list.Add(update);
                            contractListItem.status = "2";
                        }
                    }
                    else if (day < 0)
                    {
                        if (contractListItem.status != "3")
                        {
                            string update = ""
                                + " update t_contract_list"
                                + " set `status`='3'"
                                + " where contractCode='" + contractListItem.contractCode + "'";
                            list.Add(update);
                            contractListItem.status = "3";
                        }
                    }
                    else
                    {
                        if (contractListItem.status != "1")
                        {
                            string update = ""
                                + " update t_contract_list"
                                + " set `status`='1'"
                                + " where contractCode='" + contractListItem.contractCode + "'";
                            list.Add(update);
                            contractListItem.status = "1";
                        }
                    }
                    DatabaseOperationWeb.ExecuteDML(list);                       
                    
                    pageResult.list.Add(contractListItem);
                }
            }
            string sql1 = ""
                + "select count(*)"
                + " from t_contract_list a,t_user_list b "
                + " where " + customersCode + userName + model + contractType + contractCode + cycle + status + " a.userCode=b.usercode  ";
               
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "T").Tables[0];
            pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);

            return pageResult;

        }

        /// <summary>
        /// 获取合同详情-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Object ContractDetails(ContractDetailsParam clp, string userCode)
        {
            string sql = ""
                + "select * from (select a.contractCode,a.customersCode,b.username,a.createTime,a.cycle,c.platformType,a.beginTime,a.endTime,a.platformPoint,a.supplierPoint,a.purchasePoint,a.freightBelong,a.taxBelong,a.merchantName,a.depositBank,a.depositBankSubbranch,a.bankCard"
                + " from  t_contract_list a,t_user_list b, t_base_platform c"
                + " where a.userCode=b.usercode and a.model=c.platformId) x LEFT JOIN (select contractCode,imgUrl from t_contract_img ) y on x.contractCode=y.contractCode"
                + " where x.contractCode='"+ clp.contractCode + "'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql,"T").Tables[0];

            ContractDetailsItem cdi = new ContractDetailsItem();
            cdi.list = new List<object>();

            if (dt.Rows.Count>0)
            {                
                cdi.customersCode = dt.Rows[0]["customersCode"].ToString();
                cdi.userName = dt.Rows[0]["username"].ToString();
                cdi.contractDuration =Convert.ToDateTime(dt.Rows[0]["beginTime"]).ToString("yyyy-MM-dd")+" 至 "+ Convert.ToDateTime(dt.Rows[0]["endTime"]).ToString("yyyy-MM-dd");
                cdi.createTime = dt.Rows[0]["createTime"].ToString();
                cdi.freightBelong= dt.Rows[0]["freightBelong"].ToString();
                cdi.taxBelong= dt.Rows[0]["taxBelong"].ToString();
                cdi.merchantName= dt.Rows[0]["merchantName"].ToString();
                cdi.depositBank= dt.Rows[0]["depositBank"].ToString();
                cdi.depositBankSubbranch= dt.Rows[0]["depositBankSubbranch"].ToString();
                cdi.bankCard= dt.Rows[0]["bankCard"].ToString();
                switch (dt.Rows[0]["cycle"].ToString())
                {
                    case "1":
                        cdi.cycle = "实时";
                        break;
                    case "2":
                        cdi.cycle = "日结";
                        break;
                    case "3":
                        cdi.cycle = "周结";
                        break;
                    case "4":
                        cdi.cycle = "半月结";
                        break;
                    case "5":
                        cdi.cycle = "月结";
                        break;
                    case "6":
                        cdi.cycle = "季结";
                        break;
                    case "7":
                        cdi.cycle = "半年结";
                        break;
                    case "8":
                        cdi.cycle = "年结";
                        break;

                    default: /* 可选的 */
                        cdi.cycle = "其他";
                        break;
                }
                
                cdi.model = dt.Rows[0]["platformType"].ToString();
                cdi.platformPoint = dt.Rows[0]["platformPoint"].ToString();
                cdi.purchasePoint = dt.Rows[0]["purchasePoint"].ToString();
                cdi.supplierPoint = dt.Rows[0]["supplierPoint"].ToString();

                for (int i=0;i< dt.Rows.Count;i++)
                {
                    cdi.list.Add(dt.Rows[i]["imgUrl"].ToString());
                }
            }
            return cdi;
        }


        /// <summary>
        /// 创建合同-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public MsgResult CreateContract(CreateContractParam ccp, string userCode)
        {
            MsgResult msg = new MsgResult();
            ArrayList list = new ArrayList();

            string sql = ""
                + "select usercode,usertype "
                + " from t_user_list"
                + " where username='"+ ccp.userName + "'" ;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql,"T").Tables[0];
            if (dt.Rows.Count < 1)
            {
                msg.msg = "客商名错误，无此用户";
                return msg;
            }               
            DateTime dateTime = DateTime.Now;
            string status = "3";
            if (30 < (Convert.ToDateTime(ccp.date[1]) - dateTime).Days)
                status = "1";
            else if ((Convert.ToDateTime(ccp.date[1]) - dateTime).Days > 0 && 30 > (Convert.ToDateTime(ccp.date[1]) - dateTime).Days)
                status = "2";
            string contractCode = dt.Rows[0]["usercode"].ToString() + dateTime.ToString("yyyyMMddhhmmssfff"); 
            string insertContract = ""
                + "insert into t_contract_list(userCode,contractCode,createTime,cycle,model,beginTime,endTime,platformPoint,supplierPoint,purchasePoint,customersCode,contractType,`status`,inputOperator,freightBelong,taxBelong,merchantName,depositBank,depositBankSubbranch,bankCard)"
                + " values('"+ dt.Rows[0]["usercode"].ToString() + "','" + contractCode + "','" + ccp.createTime + "','" + ccp.cycle + "','" + ccp.model + "','" + ccp.date[0] + "','" + ccp.date[1] + "','" + ccp.platformPoint + "','" + ccp.supplierPoint + "','" + ccp.purchasePoint + "','" 
                + ccp.customersCode + "','"+dt.Rows[0]["usertype"].ToString() + "','"+status+"','"+userCode+"','"+ ccp.freightBelong + "','"+ ccp.taxBelong + "','"+ ccp.merchantName + "','"+ ccp.depositBank + "','"+ ccp.depositBankSubbranch + "','"+ ccp.bankCard + "')";
            list.Add(insertContract);

            for (int i=0;i< ccp.list.Count;i++)
            {
                string insertImg = ""
                    + "insert into t_contract_img(contractCode,imgUrl)"
                    + " values('"+ contractCode + "','"+ ccp.list[i] + "')";
                list.Add(insertImg);
            }

            if (!DatabaseOperationWeb.ExecuteDML(list))
                return msg;

            msg.type = "1";
            return msg;

        }


        /// <summary>
        /// 合同-搜索客户名接口-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Object SelectUserName(SelectUserNameParam clp, string userCode)
        {
            string userName = "";
            if (clp.userName!=null && clp.userName !="")
            {
                userName = " and username like '%" + clp.userName + "%'";
            }
            string sql = ""
                + "select username"
                + " from t_user_list" 
                + " where verifycode='4'  " + userName;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql,"T").Tables[0];
            List<object> list = new List<object>();
            if (dt.Rows.Count>0)
            {
                for (int i=0;i< dt.Rows.Count;i++)
                {
                    SelectUserNameItem suni = new SelectUserNameItem();
                    suni.keyId = i.ToString();
                    suni.userName = dt.Rows[i]["username"].ToString();
                    list.Add(suni);
                }
                
            }
            
            return list;
        }
    }
}
