using API_SERVER.Common;
using Com.ACBC.Framework.Database;
using System;
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

    }
}
