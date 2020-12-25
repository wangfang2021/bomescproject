using System;
using System.Collections.Generic;
using System.Data;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SPPSApi.Controllers.G03
{
    [Route("api/FS0302/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0302Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0302";
        FS0302_Logic fs0302_logic = new FS0302_Logic();

        public FS0302Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        [EnableCors("any")]
        public string PageLoad([FromBody]dynamic data)
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
            string fileName = dataForm.fileName == null ? "" : dataForm.fileName;

            try
            {
                DataTable dtFinishState = fs0302_logic.getFinishState();
                DataTable dtChange = fs0302_logic.getChange();

                List<Object> resList = new List<object>();
                List<string> finish = new List<string>();
                for (int i = 0; i < dtFinishState.Rows.Count; i++)
                {
                    finish.Add(dtFinishState.Rows[i]["vcName"].ToString());
                }
                List<string> change = new List<string>();
                for (int i = 0; i < dtChange.Rows.Count; i++)
                {
                    change.Add(dtChange.Rows[i]["vcName"].ToString());
                }


                DataTable dtSource = fs0302_logic.getData(fileName);
                //  DataTable dtSource = fs0302_logic.getData(fileName);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("dHandleTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dOldProjTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dNewProjTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dtSource, dtConverter);


                resList.Add(finish);
                resList.Add(change);
                resList.Add(dataList);


                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = resList;

                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0201", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }


    }
}