using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace SPPSApi.Controllers.G12
{
    [Route("api/FS1207_Sub3/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1207Controller_Sub3 : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS1207";
        FS1207_Logic logic = new FS1207_Logic();

        public FS1207Controller_Sub3(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region 页面初始化
        [HttpPost]
        [EnableCors("any")]
        public string pageloadApi()
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
                Dictionary<string, Object> res = new Dictionary<string, Object>();
                List<object> dataList_Plant = ComFunction.convertAllToResult(ComFunction.getTCode("C000"));
                res.Add("dataList_Plant", dataList_Plant);
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
        #endregion

        #region 导入
        [HttpPost]
        [EnableCors("any")]
        public string computeApi([FromBody] dynamic data)
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
            string vcMon = dataForm.vcMon == null ? "" : dataForm.vcMon;
            string vcPlant = dataForm.vcPlant == null ? "" : dataForm.vcPlant;
            try
            {
                //判断此对象月信息是否已发注
                string msg = logic.checkFZ(vcMon);
                if (msg.Length > 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = msg;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                #region 存入到临时DataTable中
                DataTable tb = logic.getNQCReceiveInfo(vcMon, vcPlant);
                DataTable dttmp = logic.dtResultClone();
                if (tb != null && tb.Rows.Count > 0)
                {
                    for (int i = 0; i < tb.Rows.Count; i++)
                    {
                        #region 
                        DataRow dr = dttmp.NewRow();
                        dr["vcMonth"] = tb.Rows[i]["Start_date_for_daily_qty"].ToString().Substring(0, 4) + '-' + tb.Rows[i]["Start_date_for_daily_qty"].ToString().Substring(4, 2);
                        dr["vcPartsNo"] = tb.Rows[i]["Part_No"].ToString() + tb.Rows[i]["Part_Suffix"].ToString();
                        dr["vcSource"] = tb.Rows[i]["Source_Code"].ToString();
                        dr["vcDock"] = tb.Rows[i]["Parts_Master_Matching_Key"].ToString();
                        dr["D1"] = (tb.Rows[i]["Daily_Qty_01"] != null && tb.Rows[i]["Daily_Qty_01"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_01"]) : 0;
                        dr["D2"] = (tb.Rows[i]["Daily_Qty_02"] != null && tb.Rows[i]["Daily_Qty_02"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_02"]) : 0;
                        dr["D3"] = (tb.Rows[i]["Daily_Qty_03"] != null && tb.Rows[i]["Daily_Qty_03"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_03"]) : 0;
                        dr["D4"] = (tb.Rows[i]["Daily_Qty_04"] != null && tb.Rows[i]["Daily_Qty_04"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_04"]) : 0;
                        dr["D5"] = (tb.Rows[i]["Daily_Qty_05"] != null && tb.Rows[i]["Daily_Qty_05"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_05"]) : 0;
                        dr["D6"] = (tb.Rows[i]["Daily_Qty_06"] != null && tb.Rows[i]["Daily_Qty_06"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_06"]) : 0;
                        dr["D7"] = (tb.Rows[i]["Daily_Qty_07"] != null && tb.Rows[i]["Daily_Qty_07"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_07"]) : 0;
                        dr["D8"] = (tb.Rows[i]["Daily_Qty_08"] != null && tb.Rows[i]["Daily_Qty_08"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_08"]) : 0;
                        dr["D9"] = (tb.Rows[i]["Daily_Qty_09"] != null && tb.Rows[i]["Daily_Qty_09"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_09"]) : 0;
                        dr["D10"] = (tb.Rows[i]["Daily_Qty_10"] != null && tb.Rows[i]["Daily_Qty_10"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_10"]) : 0;
                        dr["D11"] = (tb.Rows[i]["Daily_Qty_11"] != null && tb.Rows[i]["Daily_Qty_11"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_11"]) : 0;
                        dr["D12"] = (tb.Rows[i]["Daily_Qty_12"] != null && tb.Rows[i]["Daily_Qty_12"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_12"]) : 0;
                        dr["D13"] = (tb.Rows[i]["Daily_Qty_13"] != null && tb.Rows[i]["Daily_Qty_13"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_13"]) : 0;
                        dr["D14"] = (tb.Rows[i]["Daily_Qty_14"] != null && tb.Rows[i]["Daily_Qty_14"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_14"]) : 0;
                        dr["D15"] = (tb.Rows[i]["Daily_Qty_15"] != null && tb.Rows[i]["Daily_Qty_15"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_15"]) : 0;
                        dr["D16"] = (tb.Rows[i]["Daily_Qty_16"] != null && tb.Rows[i]["Daily_Qty_16"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_16"]) : 0;
                        dr["D17"] = (tb.Rows[i]["Daily_Qty_17"] != null && tb.Rows[i]["Daily_Qty_17"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_17"]) : 0;
                        dr["D18"] = (tb.Rows[i]["Daily_Qty_18"] != null && tb.Rows[i]["Daily_Qty_18"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_18"]) : 0;
                        dr["D19"] = (tb.Rows[i]["Daily_Qty_19"] != null && tb.Rows[i]["Daily_Qty_19"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_19"]) : 0;
                        dr["D20"] = (tb.Rows[i]["Daily_Qty_20"] != null && tb.Rows[i]["Daily_Qty_20"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_20"]) : 0;
                        dr["D21"] = (tb.Rows[i]["Daily_Qty_21"] != null && tb.Rows[i]["Daily_Qty_21"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_21"]) : 0;
                        dr["D22"] = (tb.Rows[i]["Daily_Qty_22"] != null && tb.Rows[i]["Daily_Qty_22"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_22"]) : 0;
                        dr["D23"] = (tb.Rows[i]["Daily_Qty_23"] != null && tb.Rows[i]["Daily_Qty_23"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_23"]) : 0;
                        dr["D24"] = (tb.Rows[i]["Daily_Qty_24"] != null && tb.Rows[i]["Daily_Qty_24"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_24"]) : 0;
                        dr["D25"] = (tb.Rows[i]["Daily_Qty_25"] != null && tb.Rows[i]["Daily_Qty_25"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_25"]) : 0;
                        dr["D26"] = (tb.Rows[i]["Daily_Qty_26"] != null && tb.Rows[i]["Daily_Qty_26"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_26"]) : 0;
                        dr["D27"] = (tb.Rows[i]["Daily_Qty_27"] != null && tb.Rows[i]["Daily_Qty_27"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_27"]) : 0;
                        dr["D28"] = (tb.Rows[i]["Daily_Qty_28"] != null && tb.Rows[i]["Daily_Qty_28"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_28"]) : 0;
                        dr["D29"] = (tb.Rows[i]["Daily_Qty_29"] != null && tb.Rows[i]["Daily_Qty_29"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_29"]) : 0;
                        dr["D30"] = (tb.Rows[i]["Daily_Qty_30"] != null && tb.Rows[i]["Daily_Qty_30"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_30"]) : 0;
                        dr["D31"] = (tb.Rows[i]["Daily_Qty_31"] != null && tb.Rows[i]["Daily_Qty_31"].ToString().Length > 0) ? Convert.ToInt32(tb.Rows[i]["Daily_Qty_31"]) : 0;
                        dr["vcPlant"] = vcPlant;
                        dr["Creater"] = loginInfo.UserId;
                        dr["dCreateDate"] = DateTime.Now.ToString();

                        //导入标志“0”，发注时更新为“1”
                        // dr["iFZFlg"] = "0";
                        #endregion
                        dttmp.Rows.Add(dr);
                        //计算总数
                        int a = 0;
                        for (int j = 5; j <= 35; j++)
                        {
                            a += Convert.ToInt32(dttmp.Rows[i][j].ToString());
                        }
                        dttmp.Rows[i]["Total"] = a;
                    }
                }
                #endregion

                #region 从临时dttmp中取对象月数据
                DataTable dtSource = logic.getSource();
                DataTable dt = new DataTable();
                dt = dttmp.Clone();
                for (int i = 0; i < dtSource.Rows.Count; i++)
                {
                    DataRow[] rows = dttmp.Select("vcMonth='" + vcMon + "' and vcSource='" + dtSource.Rows[i]["vcSource"] + "' and vcDock='" + dtSource.Rows[i]["vcDock"] + "'");
                    if (rows.Length > 0)
                    {
                        foreach (DataRow row in rows)
                        {
                            dt.ImportRow(row);
                        }
                    }
                }
                #endregion

                if (dt.Rows.Count > 0)
                {
                    msg = logic.UpdateTable(dt, loginInfo.UserId, vcMon, vcPlant);
                    if (msg.Length > 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "导入失败！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    else
                    {
                        apiResult.code = ComConstant.SUCCESS_CODE;
                        apiResult.data = "导入成功！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "对象月数据不存在！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            }
            catch (Exception ex)
            {
                //ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0905", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败" + ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
