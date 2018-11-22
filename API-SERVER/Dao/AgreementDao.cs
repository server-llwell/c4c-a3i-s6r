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

            string sql = "select contractCode,cycle,model from t_contract_list where userCode='"+ userCode+"'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql,"table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                ContractInformationItem cii = new ContractInformationItem();
                
                cii.contractCode = dt.Rows[0]["contractCode"].ToString();
                cii.cycle = dt.Rows[0]["cycle"].ToString();
                cii.model = dt.Rows[0]["model"].ToString();
                pageResult.item = cii;
            }

            string sql1 = "select imgUrl from t_contract_img a,t_contract_list b  where a.contractCode=b.contractCode and b.userCode='"+ userCode+"'";
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1,"table").Tables[0];
            if (dt1.Rows.Count>0)
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
