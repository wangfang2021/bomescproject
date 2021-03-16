using System;
using System.Collections.Generic;
using System.Data;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SPPSApi.Controllers.G03
{
    [Route("api/FS0300/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0300Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0300";
        FS0300_Logic fs0300_logic = new FS0300_Logic();

        public FS0300Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        [EnableCors("any")]
        public string searchApi([FromBody]dynamic data)
        {
            //验证是否登录
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);

            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
            string PartId = dataForm.PartId == null ? "" : dataForm.PartId;
            string Supplier_id = dataForm.SupplierId == null ? "" : dataForm.SupplierId;

            try
            {
                DataTable dt = fs0300_logic.searchApi(PartId, Supplier_id);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("dTimeFrom", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dTimeTo", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dJiuBegin", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dJiuEnd", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dSSDate", ConvertFieldType.DateType, "yyyy/MM/dd");

                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0001", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }


        #region 导出
        [HttpPost]
        [EnableCors("any")]
        public string exportApi([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));

            string PartId = dataForm.PartId == null ? "" : dataForm.PartId;
            string Supplier_id = dataForm.SupplierId == null ? "" : dataForm.SupplierId;
            try
            {
                string resMsg = "";
                DataTable dt = fs0300_logic.searchApi(PartId, Supplier_id);
                string[] fields = { "vcOriginCompany", "vcPart_id", "vcPartNameEn", "vcPartNameCn", "vcCarTypeDesign", "vcCarTypeDev", "dTimeFrom1", "dTimeTo1", "vcPartReplace", "vcInOutflag", "vcOE", "vcHKPart_id", "vcHaoJiu", "dJiuBegin1", "dJiuEnd1", "vcJiuYear", "vcNXQF", "dSSDate1", "vcSupplier_id", "vcSupplierPlant", "vcSupplier_name", "vcSCPlace", "vcCHPlace", "vcSYTCode", "vcSCSName", "vcSCSAdress", "vcZXBZNo", "vcCarTypeName", "vcFXDiff", "vcFXNo", "vcSufferIn", "iPackingQty", "vcBoxType", "decPriceOrigin", "decPriceTNPWithTax", "vcPackNo", "vcBZPlant", "vcBZUnit" };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0300.xlsx", 1, loginInfo.UserId, FunctionID);
                if (filepath == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "导出生成文件失败";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = filepath;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0002", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

    }
}