using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SPPSApi.Controllers.G13
{
    [Route("api/FS1302/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1302Controller : BaseController
    {
        FS0603_Logic fS0603_Logic = new FS0603_Logic();
        FS1301_Logic fS1301_Logic = new FS1301_Logic();
        private readonly string FunctionID = "FS1302";

        private readonly IWebHostEnvironment _webHostEnvironment;
        public FS1302Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        /// <summary>
        /// 页面初始化
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string pageloadApi1()
        {
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
                Dictionary<string, object> res = new Dictionary<string, object>();

                DataTable dt1 = new DataTable();
                dt1.Columns.Add("id", typeof(string));
                dt1.Columns.Add("date", typeof(string));
                dt1.Columns.Add("name", typeof(string));
                dt1.Columns.Add("address", typeof(string));
                dt1.Columns.Add("children", typeof(string));

                DataRow dr1 = dt1.NewRow();
                dr1["id"] = "1";
                dr1["date"] = "2021-01-01";
                dr1["name"] = "王方";
                dr1["address"] = "瑞达";
                dr1["children"] = "wf";
                dt1.Rows.Add(dr1);

                DataRow dr12 = dt1.NewRow();
                dr12["id"] = "2";
                dr12["date"] = "2021-02-01";
                dr12["name"] = "王方2";
                dr12["address"] = "瑞达2";
                dr12["children"] = "";
                dt1.Rows.Add(dr12);

                DataTable dt2 = new DataTable();
                dt2.Columns.Add("id", typeof(string));
                dt2.Columns.Add("date", typeof(string));
                dt2.Columns.Add("name", typeof(string));
                dt2.Columns.Add("address", typeof(string));
                dt2.Columns.Add("children", typeof(string));

                DataRow dr2 = dt2.NewRow();
                dr2["id"] = "1";
                dr2["date"] = "2021-12-01";
                dr2["name"] = "sss王方sss";
                dr2["address"] = "sss瑞达sss";
                dr2["children"] = "wf";
                dt2.Rows.Add(dr2);
                DataRow dr22 = dt2.NewRow();
                dr22["id"] = "2";
                dr22["date"] = "2021-12-01";
                dr22["name"] = "vvv王方vvv";
                dr22["address"] = "vvv瑞达vv";
                dr22["children"] = "wf";
                dt2.Rows.Add(dr22);
                DtConverter dtConverter = new DtConverter();
                List<Object> dataList = ComFunction.convertAllToResultByConverter1(dt1, dt2, dtConverter);
                res.Add("tempList", dataList);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M00UE0006", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "初始化失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }

    }
}
