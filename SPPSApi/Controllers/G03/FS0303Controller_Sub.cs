using System;
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

namespace SPPSApi.Controllers.G03
{
    [Route("api/FS0303_Sub/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0303Controller_Sub : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0303_Logic fs0303_Logic = new FS0303_Logic();
        private readonly string FunctionID = "FS0303";

        public FS0303Controller_Sub(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region 检索特记
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

            string strPart_id = dataForm.vcPart_id;
            try
            {
                DataTable dt = fs0303_Logic.SearchTeji(strPart_id);
                DataTable dtResult = dt.Clone();
                if (dt.Rows.Count > 0)
                {
                    string strMono = dt.Rows[0]["vcMeno"] == DBNull.Value ? "" : dt.Rows[0]["vcMeno"].ToString();
                    string[] strMonos = strMono.Split(';');
                    for (int i = 0; i < strMonos.Length; i++)
                    {
                        if (strMonos[i].Trim() == "")
                            continue;
                        DataRow row = dtResult.NewRow();
                        row["vcMeno"] = strMonos[i];
                        row["vcModFlag"] = "0";
                        row["vcAddFlag"] = "0";
                        dtResult.Rows.Add(row);
                    }
                }

                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);

                List<Object> dataList = ComFunction.convertAllToResultByConverter(dtResult, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0306", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 保存
        [HttpPost]
        [EnableCors("any")]
        public string saveApi([FromBody] dynamic data)
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
            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                JArray listInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                string strPartId = dataForm.vcPart_id;

                if (strPartId.Trim() == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "品番不能为空！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                fs0303_Logic.SaveTeJi(listInfoData, loginInfo.UserId, strPartId);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0302", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        
    }
}
