﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SPPSApi.Controllers.G06
{
    [Route("api/FS0624/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0624Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0624_Logic fs0624_Logic = new FS0624_Logic();
        private readonly string FunctionID = "FS0624";

        public FS0624Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region 检索
        [HttpPost]
        [EnableCors("any")]
        public string searchApi([FromBody] dynamic data)
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

            string strChange = dataForm.Change;
            string strPart_id = dataForm.Part_id;
            string strOriginCompany = dataForm.OriginCompany;
            string strHaoJiu = dataForm.HaoJiu;
            string strProjectType = dataForm.ProjectType;
            if (loginInfo.Special == "财务用户")
                strProjectType = "内制";

            string strPriceChangeInfo = dataForm.PriceChangeInfo;
            string strCarTypeDev = dataForm.CarTypeDev;
            string strSupplier_id = dataForm.Supplier_id;
            string strReceiver = dataForm.Receiver;
            string strPriceState = dataForm.PriceState;

            try
            {
                DataTable dt = fs0624_Logic.Search(strChange, strPart_id, strOriginCompany, strHaoJiu
                    , strProjectType, strPriceChangeInfo, strCarTypeDev, strSupplier_id
                    , strReceiver, strPriceState
                    );
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2401", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

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

            string strChange = dataForm.Change;
            string strPart_id = dataForm.Part_id;
            string strOriginCompany = dataForm.OriginCompany;
            string strHaoJiu = dataForm.HaoJiu;
            string strProjectType = dataForm.ProjectType;
            string strPriceChangeInfo = dataForm.PriceChangeInfo;
            string strCarTypeDev = dataForm.CarTypeDev;
            string strSupplier_id = dataForm.Supplier_id;
            string strReceiver = dataForm.Receiver;
            string strPriceState = dataForm.PriceState;
            try
            {
                DataTable dt = fs0624_Logic.Search(strChange, strPart_id, strOriginCompany, strHaoJiu
                   , strProjectType, strPriceChangeInfo, strCarTypeDev, strSupplier_id
                   , strReceiver, strPriceState
                   );
                string[] fields = { "vcChange_Name", "vcPart_id", "dUseBegin", "dUseEnd", "vcProjectType_Name", "vcSupplier_id"
                ,"vcSupplier_Name","dProjectBegin","dProjectEnd","vcHaoJiu_Name","dJiuBegin","dJiuEnd","dJiuBeginSustain","vcPriceChangeInfo"
                ,"vcPriceState_Name","dPriceStateDate","vcPriceGS","decPriceOrigin","decPriceAfter","decPriceTNPWithTax","dPricebegin","dPriceEnd"
                ,"vcCarTypeDev","vcCarTypeDesign","vcPart_Name","vcOE_Name","vcPart_id_HK","vcStateFX","vcFXNO","vcSumLater","vcReceiver_Name"
                ,"vcOriginCompany_Name"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0624_Export.xlsx", 2, loginInfo.UserId, FunctionID);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2402", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
