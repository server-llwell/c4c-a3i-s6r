using API_SERVER.Common;
using Newtonsoft.Json;
using API_SERVER.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace API_SERVER.Buss
{
    public class WarehouseBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.WarehouseApi;
        }

        public bool NeedCheckToken()
        {
            return true;
        }

        /// <summary>
        /// 获取收货订单-代销接口
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_CollectGoods(object param, string userId)
        {
            CollectGoodsIn cgi = JsonConvert.DeserializeObject<CollectGoodsIn>(param.ToString());
            if (cgi == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            if (cgi.pageSize == 0)
            {
                cgi.pageSize = 10;
            }
            if (cgi.current == 0)
            {
                cgi.current = 1;
            }

            WarehouseDao warehouseDao = new WarehouseDao();
            return warehouseDao.CollectGoods(cgi, userId);
        }
        /// <summary>
        /// 获取收货订单详情-代销接口
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_CollectGoodsList(object param, string userId)
        {
            CollectGoodsListIn cgi = JsonConvert.DeserializeObject<CollectGoodsListIn>(param.ToString());

            if (cgi == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (cgi.sendid == null || cgi.sendid == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }

            if (cgi.pageSize == 0)
            {
                cgi.pageSize = 10;
            }
            if (cgi.current == 0)
            {
                cgi.current = 1;
            }

            WarehouseDao warehouseDao = new WarehouseDao();
            return warehouseDao.CollectGoodsList(cgi, userId);
        }

        /// <summary>
        /// 获取收货、退货订单确认、填写运单号-代销接口
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_ConfirmGoods(object param, string userId)
        {
            ConfirmGoodsIn cgi = JsonConvert.DeserializeObject<ConfirmGoodsIn>(param.ToString());
            if (cgi == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (cgi.sendid == null || cgi.sendid == "")
            {

                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }

            WarehouseDao warehouseDao = new WarehouseDao();
            return warehouseDao.ConfirmGoods(cgi, userId);
        }

        /// <summary>
        /// 获取收货、退货订单确认、填写运单号-代销接口
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_ExportSendGoods(object param, string userId)
        {
            ExportSendGoodsParam cgi = JsonConvert.DeserializeObject<ExportSendGoodsParam>(param.ToString());
            if (cgi == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (cgi.sendid == null || cgi.sendid == "")
            {

                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (cgi.exportType == null || cgi.exportType == "")
            {

                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }

            WarehouseDao warehouseDao = new WarehouseDao();
            return warehouseDao.exportSendGoods(cgi, userId);
        }

        /// <summary>
        /// 我要发货-运营-导入
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_OperationDeliveryImport(object param, string userId)
        {
            OperationDeliveryImportParam onLoadGoodsListParam = JsonConvert.DeserializeObject<OperationDeliveryImportParam>(param.ToString());
            if (onLoadGoodsListParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (onLoadGoodsListParam.fileTemp == null || onLoadGoodsListParam.fileTemp == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (onLoadGoodsListParam.usercode == null || onLoadGoodsListParam.usercode == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            WarehouseDao warehouseDao = new WarehouseDao();
            return warehouseDao.OperationDeliveryImport(onLoadGoodsListParam, userId);
        }

        /// <summary>
        /// 我要发货-运营-采购商下拉
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_DeliveryPurchasersList(object param, string userId)
        {
            
            WarehouseDao warehouseDao = new WarehouseDao();
            return warehouseDao.DeliveryPurchasersList(userId);
        }

        /// <summary>
        /// 发货列表-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_DeliverOrderList(object param, string userId)
        {
            DeliverOrderListParam dolp = JsonConvert.DeserializeObject<DeliverOrderListParam>(param.ToString());
            if (dolp == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (dolp.pageSize == 0)
            {
                dolp.pageSize = 10;
            }
            if (dolp.current == 0)
            {
                dolp.current = 1;
            }

            WarehouseDao warehouseDao = new WarehouseDao();
            return warehouseDao.DeliverOrderList(dolp, userId);

        }

        /// <summary>
        /// 发货列表撤回-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_DeliverOrderListWithdraw(object param, string userId)
        {
            DeliverOrderListWithdrawParam deliverOrderListWithdrawParam = JsonConvert.DeserializeObject<DeliverOrderListWithdrawParam>(param.ToString());
            if (deliverOrderListWithdrawParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (deliverOrderListWithdrawParam.id == null || deliverOrderListWithdrawParam.id == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            WarehouseDao warehouseDao = new WarehouseDao();
            return warehouseDao.DeliverOrderListWithdraw(deliverOrderListWithdrawParam, userId);
        }

        /// <summary>
        /// 发货列表删除-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_DeliverOrderListDelete(object param, string userId)
        {
            DeliverOrderListWithdrawParam deliverOrderListWithdrawParam = JsonConvert.DeserializeObject<DeliverOrderListWithdrawParam>(param.ToString());
            if (deliverOrderListWithdrawParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (deliverOrderListWithdrawParam.id == null || deliverOrderListWithdrawParam.id == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            WarehouseDao warehouseDao = new WarehouseDao();
            return warehouseDao.DeliverOrderListDelete(deliverOrderListWithdrawParam, userId);
        }

        /// <summary>
        /// 发货商品列表-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_DeliverGoodsList(object param, string userId)
        {
            DeliverOrderListWithdrawParam deliverOrderListWithdrawParam = JsonConvert.DeserializeObject<DeliverOrderListWithdrawParam>(param.ToString());
            if (deliverOrderListWithdrawParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (deliverOrderListWithdrawParam.id == null || deliverOrderListWithdrawParam.id == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (deliverOrderListWithdrawParam.pageSize == 0)
            {
                deliverOrderListWithdrawParam.pageSize = 10;
            }
            if (deliverOrderListWithdrawParam.current == 0)
            {
                deliverOrderListWithdrawParam.current = 1;
            }
            WarehouseDao warehouseDao = new WarehouseDao();
            return warehouseDao.DeliverGoodsList(deliverOrderListWithdrawParam, userId);
        }

        /// <summary>
        /// 查看发货单-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_DeliverOrderDetails(object param, string userId)
        {
            DeliverOrderListWithdrawParam dolwp = JsonConvert.DeserializeObject<DeliverOrderListWithdrawParam>(param.ToString());
            if (dolwp == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (dolwp.id == null || dolwp.id == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (dolwp.pageSize == 0)
            {
                dolwp.pageSize = 10;
            }
            if (dolwp.current == 0)
            {
                dolwp.current = 1;
            }
            WarehouseDao warehouseDao = new WarehouseDao();
            return warehouseDao.DeliverOrderDetails(dolwp, userId);

        }


        /// <summary>
        /// 发货数量or安全数量修改-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_DeliverGoodsNum(object param, string userId)
        {
            DeliverGoodsNumParam dgnp = JsonConvert.DeserializeObject<DeliverGoodsNumParam>(param.ToString());
            if (dgnp == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (dgnp.id == null || dgnp.id == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (dgnp.barcode == null || dgnp.barcode == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (dgnp.goodsNum == null || dgnp.goodsNum == "")
            {
                if (dgnp.safeNum == null || dgnp.safeNum == "")
                {
                    throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
                }
            }
            WarehouseDao warehouseDao = new WarehouseDao();
            return warehouseDao.DeliverGoodsNum(dgnp, userId);
        }

        /// <summary>
        /// 发货商品删除-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_DeliverGoodsDelete(object param, string userId)
        {
            DeliverGoodsDeleteParam dgdp = JsonConvert.DeserializeObject<DeliverGoodsDeleteParam>(param.ToString());
            if (dgdp == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (dgdp.id == null || dgdp.id == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (dgdp.barcode == null || dgdp.barcode == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }

            WarehouseDao warehouseDao = new WarehouseDao();
            return warehouseDao.DeliverGoodsDelete(dgdp, userId);
        }


        /// <summary>
        /// 选择发货商品列表-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_ChooseDeliverGoods(object param, string userId)
        {
            ChooseDeliverGoodsParam odip = JsonConvert.DeserializeObject<ChooseDeliverGoodsParam>(param.ToString());
            if (odip == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (odip.usercode == null || odip.usercode == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (odip.isDelete == null || odip.isDelete == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (odip.pageSize == 0)
            {
                odip.pageSize = 10;
            }
            if (odip.current == 0)
            {
                odip.current = 1;
            }
            WarehouseDao warehouseDao = new WarehouseDao();
            return warehouseDao.ChooseDeliverGoods(odip, userId);

        }

        /// <summary>
        /// 选中发货商品-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_ChooseGoods(object param, string userId)
        {
            DeliverGoodsDeleteParam dgnp = JsonConvert.DeserializeObject<DeliverGoodsDeleteParam>(param.ToString());
            if (dgnp == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (dgnp.barcode == null || dgnp.barcode == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (dgnp.id == null || dgnp.id == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (dgnp.usercode == null || dgnp.usercode == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (dgnp.ischoose != true && dgnp.ischoose != false)
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            WarehouseDao warehouseDao = new WarehouseDao();
            return warehouseDao.ChooseGoods(dgnp, userId);

        }

        /// <summary>
        /// 发货单提交接口-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_DeliverOrderSubmission(object param, string userId)
        {
            DeliverOrderConserveParam dodi = JsonConvert.DeserializeObject<DeliverOrderConserveParam>(param.ToString());
            if (dodi == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (dodi.express == null || dodi.express == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (dodi.id == null || dodi.id == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (dodi.contact == null || dodi.contact == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (dodi.getTel == null || dodi.getTel == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (dodi.sendName == null || dodi.sendName == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (dodi.sendTel == null || dodi.sendTel == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (dodi.waybillNo == null || dodi.waybillNo == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (dodi.usercode == null || dodi.usercode == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }

            WarehouseDao warehouseDao = new WarehouseDao();
            return warehouseDao.DeliverOrderSubmission(dodi, userId);
        }

        /// <summary>
        /// 发货单保存接口-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_DeliverOrderConserve(object param, string userId)
        {
            DeliverOrderConserveParam dodi = JsonConvert.DeserializeObject<DeliverOrderConserveParam>(param.ToString());
            if (dodi == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            WarehouseDao warehouseDao = new WarehouseDao();
            return warehouseDao.DeliverOrderConserve(dodi, userId);
        }

        /// <summary>
        /// 库存管理-平台库存-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_PlatformInventory(object param, string userId)
        {
            PlatformInventoryParam dodi = JsonConvert.DeserializeObject<PlatformInventoryParam>(param.ToString());
            if (dodi == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (dodi.pageSize == 0)
            {
                dodi.pageSize = 10;
            }
            if (dodi.current == 0)
            {
                dodi.current = 1;
            }
            WarehouseDao warehouseDao = new WarehouseDao();
            return warehouseDao.PlatformInventory(dodi, userId);
        }


        /// <summary>
        /// 我要发货-导入入库商品-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_OnloadWarehousingGoods(object param, string userId)
        {
            OperationDeliveryImportParam onLoadGoodsListParam = JsonConvert.DeserializeObject<OperationDeliveryImportParam>(param.ToString());
            if (onLoadGoodsListParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (onLoadGoodsListParam.fileTemp == null || onLoadGoodsListParam.fileTemp == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
           
            WarehouseDao warehouseDao = new WarehouseDao();
            return warehouseDao.OnloadWarehousingGoods(onLoadGoodsListParam, userId);
        }


        /// <summary>
        /// 库存管理-门店库存查询分页-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_StoreInventory(object param, string userId)
        {
            StoreInventoryParam dodi = JsonConvert.DeserializeObject<StoreInventoryParam>(param.ToString());
            if (dodi == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (dodi.pageSize == 0)
            {
                dodi.pageSize = 10;
            }
            if (dodi.current == 0)
            {
                dodi.current = 1;
            }
            WarehouseDao warehouseDao = new WarehouseDao();
            return warehouseDao.StoreInventory(dodi, userId);
        }

        public class StoreInventoryItem
        {
            public string keyId;//序号
            public string supplierName;//供应商
            public string purchasesnName;//采购商名
            public string warehouse;//仓库
            public string goodsName;//商品名
            public string barcode;//商品条码
            public string model;//规格
            public string country;//原产地
            public string brand;//生产商
            public string pNum;//库存
            public string pprice;//零售价
            public string rprice;//零售价
            public string inprice;//采购价
            public string time;//同步时间
            public string safeNum;//安全数量

        }

        public class StoreInventoryParam
        {
            public string purchasesnName;//采购商名
            public string select;//搜索条件  
            public int current;//多少页
            public int pageSize;//页面显示多少个商品
        }

        public class PlatformInventoryParam
        {
            public string supplierName;//供应商
            public string warehouse;//仓库
            public string select;//搜索条件          
            public int current;//多少页
            public int pageSize;//页面显示多少个商品

        }


        public class Msg : MsgResult
        {
            public string ifuploda;//1导入，0非导入
        }

        public class DeliverOrderConserveParam
        {
            public string sendName;//发货人
            public string sendTel;//发货人电话
            public string express;//快递
            public string waybillNo;//快递单号
            public string contact;//联系人
            public string usercode;//采购商编码
            public string getTel;//联系人电话
            public string id;//单号         
        }

        public class ChooseDeliverItem
        {
            public string id;//单号
            public string num;//数量
            public string usercode;//采购商编码
            public List<object> list;//仓库列表
        }

        public class ChooseDeliverGoodsItem
        {
            public string keyId;//序号
            public string supplierName;//供应商
            public string warehouse;//仓库
            public string goodsName;//商品名
            public string barcode;//商品条码
            public string model;//规格
            public string country;//原产地
            public string brand;//生产商
            public string pNum;//库存
            public string rprice;//零售价
            public string inprice;//采购价
            public string time;//同步时间
            public bool ischoose;//是否选中 
        }


        public class ChooseDeliverGoodsParam
        {
            public string id;//单号
            public string usercode;//采购商编码
            public string supplierName;//供应商
            public string warehouse;//仓库
            public string select;//搜索条件
            public string isDelete;//是否删除1:删除，0：不删除
            public int current;//多少页
            public int pageSize;//页面显示多少个商品
        }

        public class DeliverGoodsDeleteParam
        {
            public string id;//单号
            public string barcode;//商品编号
            public string usercode;//采购商编码
            public bool ischoose;//是否选中

        }

        public class DeliverGoodsNumParam
        {
            public string id;//单号
            public string barcode;//商品编号
            public string goodsNum;//发货商品数
            public string safeNum;//安全数量           
        }

        public class DeliverGoodsNumItem
        {
            public string barcode;//商品编号
            public string pNum;//当前库存
            public string type="0";//0失败，1成功
        }

        public class DeliverOrderDetailsItem
        {
            public string sendName;//发货人
            public string sendTel;//发货人电话
            public string express;//快递
            public string waybillNo;//快递单号
            public string getName;//采购商名
            public string contact;//联系人
            public string getTel;//联系人电话
            public string usercode;//采购商code
            public string id;//单号
            public string ifupload;//1导入，0非导入
        }

      

        public class DeliveryPurchasersListItem
        {
            public string getName;//采购商名
            public string usercode;//采购商编码
            public string contact;//联系人
            public string getTel;//联系人电话
        }

        public class DeliverGoodsListItem
        {
            public string keyId;//序号
            public string id;//发货单编号
            public string goodsName;//商品名
            public string barcode;//条码
            public string model;//规格
            public string country;//原产地
            public string brand;//生产商
            public string rprice;//零售价
            public string pprice;//供货价
            public string pNum;//库存
            public string mNum;//最大库存
            public string goodsNum;//发货数
            public string safeNum;//安全库存数
            
        }

        public class DeliverOrderListWithdrawParam
        {
            public string id;//发货单号
            public int current;//多少页
            public int pageSize;//页面显示多少个商品
        }

        public class DeliverOrderListParam
        {
            public string purchasersName;//采购商名
            public string status;//状态
            public string id;//单据编号
            public string[] date;//时间区间
            public int current;//多少页
            public int pageSize;//页面显示多少个商品
        }

        public class DeliverOrderListItem
        {
            public string keyId;//序号
            public string id;//发货单编号
            public string purchasersCode;//采购商编码
            public string purchasersName;//采购商名
            public string goodsTotal;//发货总数量
            public string sendTime;//发货日期
            public string sendName;//发货人
            public string sendTel;//发货人电话
            public string status;//状态

        }

        public class OperationDeliveryImportParam
        {
            public string usercode;//采购商
            public string id;//单号
            public string fileTemp;//文件
        }

        public class CollectGoodsIn
        {
            public string[] date;//日期区间
            public string sendType;//订单类型
            public string status;//状态
            public string sendid;//单据号
            public int current;//多少页
            public int pageSize;//页面显示多少个商品

        }

        public class CollectGoodsItem
        {
            public string keyId;//序号
            public string sendid;//单据号
            public string sendType;//订单类型
            public string goodsTotal;//发货数量
            public string sendTime;//发货日期
            public string sendName;//发货人
            public string sendTel;//联系人电话
            public string status;//状态
        }

        public class CollectGoodsListIn
        {
            public string sendid;//订单号
            public int current;//多少页
            public int pageSize;//页面显示多少个商品

        }
        public class CollectGoodsListSum
        {
            public double money=0;//合计
            public string waybillNo;//运单号
        }

        public class CollectGoodsListItem
        {
            public string keyId;//序号
            public string id;//t_warehouse_send_goods的id
            public string barcode;//商品条码
            public string slt;//商品图片地址
            public string goodsName;//商品名称
            public string brand;//品牌
            public string supplyPrice;//供货价
            public string goodsNum;//商品数量
            public string goodsTotal;//总金额
            
        }

        public class ConfirmGoodsIn
        {
            public string sendid;//订单号
            public string waybillNo;//退单运单号
        }

        public class ExportSendGoodsParam
        {
            public string sendid;//单据编号
            public string exportType;//导出类型 1系统默认，2美团
        }

    }

}

