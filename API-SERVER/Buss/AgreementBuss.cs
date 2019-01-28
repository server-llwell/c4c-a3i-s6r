using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_SERVER.Common;
using Newtonsoft.Json;
using API_SERVER.Dao;
using System.IO;

namespace API_SERVER.Buss
{
    public class AgreementBuss:IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.AgreementApi;
        }

        public bool NeedCheckToken()
        {
            return true;
        }

        /// <summary>
        /// 获取合同信息接口-代销
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_ContractInformation(object param, string userId)
        {
           
            if (userId == null)
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }

            AgreementDao agreementDao = new AgreementDao();
            return agreementDao.ContractInformation(userId);
        }

        /// <summary>
        /// 获取合同列表-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_ContractList(object param, string userId)
        {
            ContractListParam clp = JsonConvert.DeserializeObject<ContractListParam>(param.ToString());
            if (clp == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (clp.pageSize == 0)
            {
                clp.pageSize = 10;
            }
            if (clp.current == 0)
            {
                clp.current = 1;
            }
            AgreementDao agreementDao = new AgreementDao();
            return agreementDao.ContractList(clp,userId);
        }


        /// <summary>
        /// 获取合同详情-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_ContractDetails(object param, string userId)
        {
            ContractDetailsParam clp = JsonConvert.DeserializeObject<ContractDetailsParam>(param.ToString());
            if (clp == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if(clp.contractCode==null || clp.contractCode=="")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            AgreementDao agreementDao = new AgreementDao();
            return agreementDao.ContractDetails(clp, userId);
        }

        /// <summary>
        /// 创建合同上传图片-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_ContractUploadImg(object param, string userId)
        {
            MsgResult msg = new MsgResult();
            ContractUploadImgParam clp = JsonConvert.DeserializeObject<ContractUploadImgParam>(param.ToString());
            if (clp == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (clp.fileName == null || clp.fileName == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            else 
            {
                string[] a = clp.fileName.Split(".");
                if (a[1] != "jpg" && a[1] != "png" && a[1] != "jpeg" && a[1] != "tga" && a[1] != "tif")
                {
                    msg.msg = "上传格式不正确或格式不是大写";
                    return msg;
                }
            }
            
            try
            {
                FileManager fm = new FileManager();
                fm.updateFileToOSS(clp.fileName, Global.OssDir, clp.fileName);
                clp.fileName = Global.OssUrl + Global.OssDir + clp.fileName;

                msg.type = "1";
                msg.msg = clp.fileName;
                return msg;
            }
            catch (Exception ex)
            {
                msg.msg = ex.ToString();

                return msg;
            }
        }
        private string path = Path.Combine(System.IO.Path.GetDirectoryName(typeof(Program).Assembly.Location), "upload");

        /// <summary>
        /// 创建合同-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_CreateContract(object param, string userId)
        {
            CreateContractParam ccp = JsonConvert.DeserializeObject<CreateContractParam>(param.ToString());
            if (ccp==null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (ccp.userName == null || ccp.userName =="")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (ccp.customersCode == null || ccp.customersCode == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (ccp.supplierPoint == null || ccp.supplierPoint == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (ccp.purchasePoint == null || ccp.purchasePoint == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (ccp.platformPoint == null || ccp.platformPoint == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (ccp.model == null || ccp.model == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }          
            if (ccp.cycle == null || ccp.cycle == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (ccp.createTime == null || ccp.createTime == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (ccp.date == null || ccp.date.Length != 2)
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            AgreementDao agreementDao = new AgreementDao();
            return agreementDao.CreateContract(ccp, userId);
        }

        /// <summary>
        /// 合同-搜索客户名接口-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_SelectUserName(object param, string userId)
        {
            SelectUserNameParam sunp = JsonConvert.DeserializeObject<SelectUserNameParam>(param.ToString());
            if (sunp == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }           
            AgreementDao agreementDao = new AgreementDao();
            return agreementDao.SelectUserName(sunp, userId);
        }

    }


    public class SelectUserNameParam
    {
        public string userName;//客商名
    }

    public class SelectUserNameItem
    {
        public string userName;//客商名
        public string keyId;//序号
    }

    public class CreateContractParam
    {
        public string customersCode;//客商编码
        public string userName;//客商名
        public string createTime;//签订日期
        public string cycle; //结算周期
        public string model;//合作模式
        public string[] date;//合同期限       
        public string platformPoint;//平台提点
        public string supplierPoint;//供货提点
        public string purchasePoint;//采购提点
        public List<string> list;//图片list
    }

    public class ContractUploadImgParam
    {    
        public string fileName;//文件名
    }

    public class ContractDetailsItem
    {
        public string customersCode;//客商编码
        public string userName;//客商名
        public string createTime;//签订日期
        public string cycle;//结算周期
        public string model;//合作模式
        public string contractDuration;//合同期限
        public string platformPoint;//平台提点
        public string supplierPoint;//供货提点
        public string purchasePoint;//采购提点
        public List<object> list;//图片list
    }

    public class ContractDetailsParam
    {
        public string contractCode;//合同编号
    }

    public class ContractListParam
    {
        public string userName;//客商名
        public string customersCode;//客商编码
        public string contractCode;//合同编号
        public string contractType;//合同类型
        public string cycle;//结算周期
        public string model;//合作模式
        public string status;//合同状态
        public int current;//多少页
        public int pageSize;//页面显示多少个商品
    }

    public class ContractListItem
    {
        public string keyId;//序号
        public string customersCode;//客商编码
        public string contractCode;//合同编号
        public string userName;//客商名
        public string cycle;//结算周期
        public string model;//合作模式
        public string status;//合同状态
        public string contractType;//合同类型
        public string createTime;//签订日期
    }


    public class ContractInformationItem
    {
        public string contractCode;//合同编号
        public string cycle;//结算周期
        public string model;//合作模式
    }

    public class ContractInformationlist
    {
        public string imgUrl;//附件地址
    }
}
