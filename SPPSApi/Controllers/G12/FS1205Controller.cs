using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace SPPSApi.Controllers.G12
{
    [Route("api/FS1205/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1205Controller : BaseController
    {
        FS1205_Logic fS1205_Logic = new FS1205_Logic();
        private readonly string FunctionID = "FS1205";
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FS1205Controller(IWebHostEnvironment webHostEnvironment)
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
                List<Object> dataList_PlantSource = ComFunction.convertAllToResult(ComFunction.getTCode("C000"));
                List<Object> dataList_TypeSource = ComFunction.convertAllToResult(fS1205_Logic.getPlantype());
                res.Add("PlantSource", dataList_PlantSource);
                res.Add("TypeSource", dataList_TypeSource);
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
            string vcMonth = dataForm.vcMonth;
            string vcWeek = dataForm.vcWeek;
            string vcPlant = dataForm.vcPlant;
            string vcType = dataForm.vcType;
            try
            {
                DataTable dt = fS1205_Logic.TXTSearchWeekLevelSchedule(vcMonth, vcWeek, vcPlant);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 日程别更新
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
                List<Dictionary<string, object>> listInfoData = listInfo.ToObject<List<Dictionary<string, object>>>();
                bool hasFind = false;//是否找到需要新增或者修改的数据

                #region 转换表
                DataTable dt = fS1205_Logic.TXTSearchWeekLevelSchedule("", "", "");
                dt.Columns.Remove("iFlag");
                dt.Columns.Remove("vcModFlag");
                dt.Columns.Remove("vcAddFlag");
                dt.Columns.Remove("iAutoId");
                #endregion

                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == false && bModFlag == true)
                    {//修改
                        hasFind = true;
                        #region 转DataTable
                        DataRow r = dt.NewRow();
                        r["vcMonth"] = listInfoData[i]["vcMonth"];
                        r["vcWeek"] = listInfoData[i]["vcWeek"];
                        r["vcPlant"] = listInfoData[i]["vcPlant"];
                        r["vcGC"] = listInfoData[i]["vcGC"];
                        r["vcZB"] = listInfoData[i]["vcZB"];
                        r["vcPartsno"] = listInfoData[i]["vcPartsno"];
                        r["vcQuantityPerContainer"] = listInfoData[i]["vcQuantityPerContainer"];
                        r["vcD1b"] = listInfoData[i]["vcD1b"];
                        r["vcD1y"] = listInfoData[i]["vcD1y"];
                        r["vcD2b"] = listInfoData[i]["vcD2b"];
                        r["vcD2y"] = listInfoData[i]["vcD2y"];
                        r["vcD3b"] = listInfoData[i]["vcD3b"];
                        r["vcD3y"] = listInfoData[i]["vcD3y"];
                        r["vcD4b"] = listInfoData[i]["vcD4b"];
                        r["vcD4y"] = listInfoData[i]["vcD4y"];
                        r["vcD5b"] = listInfoData[i]["vcD5b"];
                        r["vcD5y"] = listInfoData[i]["vcD5y"];
                        r["vcD6b"] = listInfoData[i]["vcD6b"];
                        r["vcD6y"] = listInfoData[i]["vcD6y"];
                        r["vcD7b"] = listInfoData[i]["vcD7b"];
                        r["vcD7y"] = listInfoData[i]["vcD7y"];
                        r["vcD8b"] = listInfoData[i]["vcD8b"];
                        r["vcD8y"] = listInfoData[i]["vcD8y"];
                        r["vcD9b"] = listInfoData[i]["vcD9b"];
                        r["vcD9y"] = listInfoData[i]["vcD9y"];
                        r["vcD10b"] = listInfoData[i]["vcD10b"];
                        r["vcD10y"] = listInfoData[i]["vcD10y"];
                        r["vcD11b"] = listInfoData[i]["vcD11b"];
                        r["vcD11y"] = listInfoData[i]["vcD11y"];
                        r["vcD12b"] = listInfoData[i]["vcD12b"];
                        r["vcD12y"] = listInfoData[i]["vcD12y"];
                        r["vcD13b"] = listInfoData[i]["vcD13b"];
                        r["vcD13y"] = listInfoData[i]["vcD13y"];
                        r["vcD14b"] = listInfoData[i]["vcD14b"];
                        r["vcD14y"] = listInfoData[i]["vcD14y"];
                        r["vcD15b"] = listInfoData[i]["vcD15b"];
                        r["vcD15y"] = listInfoData[i]["vcD15y"];
                        r["vcD16b"] = listInfoData[i]["vcD16b"];
                        r["vcD16y"] = listInfoData[i]["vcD16y"];
                        r["vcD17b"] = listInfoData[i]["vcD17b"];
                        r["vcD17y"] = listInfoData[i]["vcD17y"];
                        r["vcD18b"] = listInfoData[i]["vcD18b"];
                        r["vcD18y"] = listInfoData[i]["vcD18y"];
                        r["vcD19b"] = listInfoData[i]["vcD19b"];
                        r["vcD19y"] = listInfoData[i]["vcD19y"];
                        r["vcD20b"] = listInfoData[i]["vcD20b"];
                        r["vcD20y"] = listInfoData[i]["vcD20y"];
                        r["vcD21b"] = listInfoData[i]["vcD21b"];
                        r["vcD21y"] = listInfoData[i]["vcD21y"];
                        r["vcD22b"] = listInfoData[i]["vcD22b"];
                        r["vcD22y"] = listInfoData[i]["vcD22y"];
                        r["vcD23b"] = listInfoData[i]["vcD23b"];
                        r["vcD23y"] = listInfoData[i]["vcD23y"];
                        r["vcD24b"] = listInfoData[i]["vcD24b"];
                        r["vcD24y"] = listInfoData[i]["vcD24y"];
                        r["vcD25b"] = listInfoData[i]["vcD25b"];
                        r["vcD25y"] = listInfoData[i]["vcD25y"];
                        r["vcD26b"] = listInfoData[i]["vcD26b"];
                        r["vcD26y"] = listInfoData[i]["vcD26y"];
                        r["vcD27b"] = listInfoData[i]["vcD27b"];
                        r["vcD27y"] = listInfoData[i]["vcD27y"];
                        r["vcD28b"] = listInfoData[i]["vcD28b"];
                        r["vcD28y"] = listInfoData[i]["vcD28y"];
                        r["vcD29b"] = listInfoData[i]["vcD29b"];
                        r["vcD29y"] = listInfoData[i]["vcD29y"];
                        r["vcD30b"] = listInfoData[i]["vcD30b"];
                        r["vcD30y"] = listInfoData[i]["vcD30y"];
                        r["vcD31b"] = listInfoData[i]["vcD31b"];
                        r["vcD31y"] = listInfoData[i]["vcD31y"];
                        r["vcWeekTotal"] = listInfoData[i]["vcWeekTotal"];
                        r["vcLevelD1b"] = listInfoData[i]["vcLevelD1b"];
                        r["vcLevelD1y"] = listInfoData[i]["vcLevelD1y"];
                        r["vcLevelD2b"] = listInfoData[i]["vcLevelD2b"];
                        r["vcLevelD2y"] = listInfoData[i]["vcLevelD2y"];
                        r["vcLevelD3b"] = listInfoData[i]["vcLevelD3b"];
                        r["vcLevelD3y"] = listInfoData[i]["vcLevelD3y"];
                        r["vcLevelD4b"] = listInfoData[i]["vcLevelD4b"];
                        r["vcLevelD4y"] = listInfoData[i]["vcLevelD4y"];
                        r["vcLevelD5b"] = listInfoData[i]["vcLevelD5b"];
                        r["vcLevelD5y"] = listInfoData[i]["vcLevelD5y"];
                        r["vcLevelD6b"] = listInfoData[i]["vcLevelD6b"];
                        r["vcLevelD6y"] = listInfoData[i]["vcLevelD6y"];
                        r["vcLevelD7b"] = listInfoData[i]["vcLevelD7b"];
                        r["vcLevelD7y"] = listInfoData[i]["vcLevelD7y"];
                        r["vcLevelD8b"] = listInfoData[i]["vcLevelD8b"];
                        r["vcLevelD8y"] = listInfoData[i]["vcLevelD8y"];
                        r["vcLevelD9b"] = listInfoData[i]["vcLevelD9b"];
                        r["vcLevelD9y"] = listInfoData[i]["vcLevelD9y"];
                        r["vcLevelD10b"] = listInfoData[i]["vcLevelD10b"];
                        r["vcLevelD10y"] = listInfoData[i]["vcLevelD10y"];
                        r["vcLevelD11b"] = listInfoData[i]["vcLevelD11b"];
                        r["vcLevelD11y"] = listInfoData[i]["vcLevelD11y"];
                        r["vcLevelD12b"] = listInfoData[i]["vcLevelD12b"];
                        r["vcLevelD12y"] = listInfoData[i]["vcLevelD12y"];
                        r["vcLevelD13b"] = listInfoData[i]["vcLevelD13b"];
                        r["vcLevelD13y"] = listInfoData[i]["vcLevelD13y"];
                        r["vcLevelD14b"] = listInfoData[i]["vcLevelD14b"];
                        r["vcLevelD14y"] = listInfoData[i]["vcLevelD14y"];
                        r["vcLevelD15b"] = listInfoData[i]["vcLevelD15b"];
                        r["vcLevelD15y"] = listInfoData[i]["vcLevelD15y"];
                        r["vcLevelD16b"] = listInfoData[i]["vcLevelD16b"];
                        r["vcLevelD16y"] = listInfoData[i]["vcLevelD16y"];
                        r["vcLevelD17b"] = listInfoData[i]["vcLevelD17b"];
                        r["vcLevelD17y"] = listInfoData[i]["vcLevelD17y"];
                        r["vcLevelD18b"] = listInfoData[i]["vcLevelD18b"];
                        r["vcLevelD18y"] = listInfoData[i]["vcLevelD18y"];
                        r["vcLevelD19b"] = listInfoData[i]["vcLevelD19b"];
                        r["vcLevelD19y"] = listInfoData[i]["vcLevelD19y"];
                        r["vcLevelD20b"] = listInfoData[i]["vcLevelD20b"];
                        r["vcLevelD20y"] = listInfoData[i]["vcLevelD20y"];
                        r["vcLevelD21b"] = listInfoData[i]["vcLevelD21b"];
                        r["vcLevelD21y"] = listInfoData[i]["vcLevelD21y"];
                        r["vcLevelD22b"] = listInfoData[i]["vcLevelD22b"];
                        r["vcLevelD22y"] = listInfoData[i]["vcLevelD22y"];
                        r["vcLevelD23b"] = listInfoData[i]["vcLevelD23b"];
                        r["vcLevelD23y"] = listInfoData[i]["vcLevelD23y"];
                        r["vcLevelD24b"] = listInfoData[i]["vcLevelD24b"];
                        r["vcLevelD24y"] = listInfoData[i]["vcLevelD24y"];
                        r["vcLevelD25b"] = listInfoData[i]["vcLevelD25b"];
                        r["vcLevelD25y"] = listInfoData[i]["vcLevelD25y"];
                        r["vcLevelD26b"] = listInfoData[i]["vcLevelD26b"];
                        r["vcLevelD26y"] = listInfoData[i]["vcLevelD26y"];
                        r["vcLevelD27b"] = listInfoData[i]["vcLevelD27b"];
                        r["vcLevelD27y"] = listInfoData[i]["vcLevelD27y"];
                        r["vcLevelD28b"] = listInfoData[i]["vcLevelD28b"];
                        r["vcLevelD28y"] = listInfoData[i]["vcLevelD28y"];
                        r["vcLevelD29b"] = listInfoData[i]["vcLevelD29b"];
                        r["vcLevelD29y"] = listInfoData[i]["vcLevelD29y"];
                        r["vcLevelD30b"] = listInfoData[i]["vcLevelD30b"];
                        r["vcLevelD30y"] = listInfoData[i]["vcLevelD30y"];
                        r["vcLevelD31b"] = listInfoData[i]["vcLevelD31b"];
                        r["vcLevelD31y"] = listInfoData[i]["vcLevelD31y"];
                        r["vcLevelWeekTotal"] = listInfoData[i]["vcLevelWeekTotal"];
                        dt.Rows.Add(r);
                        #endregion
                    }
                }
                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少有一个编辑行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //开始数据验证
                if (hasFind)
                {
                    string[,] strField = new string[,] {{
                                                         "1日白","1日夜","2日白","2日夜","3日白","3日夜", "4日白","4日夜","5日白","5日夜",
                                                         "6日白","6日夜","7日白","7日夜","8日白","8日夜", "9日白","9日夜","10日白","10日夜",
                                                         "11日白","11日夜","12日白","12日夜","13日白","13日夜", "14日白","14日夜","15日白","15日夜",
                                                         "16日白","16日夜","17日白","17日夜","18日白","18日夜", "19日白","19日夜","20日白","20日夜",
                                                         "21日白","21日夜","22日白","22日夜","23日白","23日夜", "24日白","24日夜","25日白","25日夜",
                                                         "26日白","26日夜","27日白","27日夜","28日白","28日夜", "29日白","29日夜","30日白","30日夜","31日白","31日夜"
                                                       },
                                                {
                                                 "vcLevelD1b","vcLevelD1y","vcLevelD2b","vcLevelD2y","vcLevelD3b","vcLevelD3y","vcLevelD4b","vcLevelD4y","vcLevelD5b","vcLevelD5y",
                                                 "vcLevelD6b","vcLevelD6y","vcLevelD7b","vcLevelD7y","vcLevelD8b","vcLevelD8y","vcLevelD9b","vcLevelD9y","vcLevelD10b","vcLevelD10y",
                                                 "vcLevelD11b","vcLevelD11y","vcLevelD12b","vcLevelD12y","vcLevelD13b","vcLevelD13y","vcLevelD14b","vcLevelD14y","vcLevelD15b","vcLevelD15y",
                                                 "vcLevelD16b","vcLevelD16y","vcLevelD17b","vcLevelD17y","vcLevelD18b","vcLevelD18y","vcLevelD19b","vcLevelD19y","vcLevelD20b","vcLevelD20y",
                                                 "vcLevelD21b","vcLevelD21y","vcLevelD22b","vcLevelD22y","vcLevelD23b","vcLevelD23y","vcLevelD24b","vcLevelD24y","vcLevelD25b","vcLevelD25y",
                                                 "vcLevelD26b","vcLevelD26y","vcLevelD27b","vcLevelD27y","vcLevelD28b","vcLevelD28y","vcLevelD29b","vcLevelD29y","vcLevelD30b","vcLevelD30y","vcLevelD31b","vcLevelD31y"
                                                },
                                                {FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                                                 FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                                                 FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                                                 FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                                                 FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num},
                                                {"8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8",
                                                 "8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8","8"},//最大长度设定,不校验最大长度用0
                                                {"0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0",
                                                 "0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0"},//最小长度设定,可以为空用0
                                                {"71","72","73","74","75","76","77","78","79","80","81","82","83","84","85","86","87","88","89","90",
                                                 "91","92","93","94","95","96","97","98","99","100","101","102","103","104","105","106","107","108","109","110",
                                                 "111","112","113","114","115","116","117","118","119","120","121","122","123","124","125","126","127","128","129","130","130","131"
                                                }//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, null, null, true, "FS1205");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = checkRes;
                        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                string Month = listInfoData[0]["vcMonth"].ToString();//获取数据源中的对象月
                string Week = listInfoData[0]["vcWeek"].ToString();//获取数据源中的对象周
                string Plant = listInfoData[0]["vcPlant"].ToString();//获取数据源中的工厂

                //检查数据正确性
                string msg = fS1205_Logic.TXTCheckDataSchedule(dt);
                if (msg.Length > 0)
                {
                    //大于0，意思是数据中有错误
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = msg;
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                else
                {
                    dt.PrimaryKey = new DataColumn[]
                    {
                            dt.Columns["vcMonth"],
                            dt.Columns["vcWeek"],
                            dt.Columns["vcPlant"],
                            dt.Columns["vcGC"],
                            dt.Columns["vcZB"],
                            dt.Columns["vcPartsno"]
                    };
                    fS1205_Logic.TXTUpdateTableSchedule(dt, Month, Week, Plant);
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = "更新成功！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0902", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "更新失败！";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }



        }
        #endregion

        #region 日程别导出
        [HttpPost]
        [EnableCors("any")]
        public string FileExport([FromBody] dynamic data)
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
            string vcMonth = dataForm.vcMonth;
            string vcWeek = dataForm.vcWeek;
            string vcPlant = dataForm.vcPlant;
            string vcType = dataForm.vcType;
            try
            {
                DataTable dt = fS1205_Logic.TXTSearchWeekLevelSchedule(vcMonth, vcWeek, vcPlant);
                string[] fields = { "vcMonth", "vcWeek", "vcPlant", "vcGC", "vcZB", "vcPartsno", "vcQuantityPerContainer", "vcD1b", "vcD1y", "vcD2b", "vcD2y", "vcD3b", "vcD3y", "vcD4b", "vcD4y", "vcD5b", "vcD5y", "vcD6b", "vcD6y", "vcD7b", "vcD7y", "vcD8b", "vcD8y", "vcD9b", "vcD9y", "vcD10b", "vcD10y", "vcD11b", "vcD11y", "vcD12b", "vcD12y", "vcD13b", "vcD13y", "vcD14b", "vcD14y", "vcD15b", "vcD15y", "vcD16b", "vcD16y", "vcD17b", "vcD17y", "vcD18b", "vcD18y", "vcD19b", "vcD19y", "vcD20b", "vcD20y", "vcD21b", "vcD21y", "vcD22b", "vcD22y", "vcD23b", "vcD23y", "vcD24b", "vcD24y", "vcD25b", "vcD25y", "vcD26b", "vcD26y", "vcD27b", "vcD27y", "vcD28b", "vcD28y", "vcD29b", "vcD29y", "vcD30b", "vcD30y", "vcD31b", "vcD31y", "vcWeekTotal", "vcLevelD1b", "vcLevelD1y", "vcLevelD2b", "vcLevelD2y", "vcLevelD3b", "vcLevelD3y", "vcLevelD4b", "vcLevelD4y", "vcLevelD5b", "vcLevelD5y", "vcLevelD6b", "vcLevelD6y", "vcLevelD7b", "vcLevelD7y", "vcLevelD8b", "vcLevelD8y", "vcLevelD9b", "vcLevelD9y", "vcLevelD10b", "vcLevelD10y", "vcLevelD11b", "vcLevelD11y", "vcLevelD12b", "vcLevelD12y", "vcLevelD13b", "vcLevelD13y", "vcLevelD14b", "vcLevelD14y", "vcLevelD15b", "vcLevelD15y", "vcLevelD16b", "vcLevelD16y", "vcLevelD17b", "vcLevelD17y", "vcLevelD18b", "vcLevelD18y", "vcLevelD19b", "vcLevelD19y", "vcLevelD20b", "vcLevelD20y", "vcLevelD21b", "vcLevelD21y", "vcLevelD22b", "vcLevelD22y", "vcLevelD23b", "vcLevelD23y", "vcLevelD24b", "vcLevelD24y", "vcLevelD25b", "vcLevelD25y", "vcLevelD26b", "vcLevelD26y", "vcLevelD27b", "vcLevelD27y", "vcLevelD28b", "vcLevelD28y", "vcLevelD29b", "vcLevelD29y", "vcLevelD30b", "vcLevelD30y", "vcLevelD31b", "vcLevelD31y", "vcLevelWeekTotal"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS1205_Export.xlsx", 1, loginInfo.UserId, FunctionID);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 生成计划
        [HttpPost]
        [EnableCors("any")]
        public string txtScheduleToPlan([FromBody] dynamic data)
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
            string vcMonth = dataForm.vcMonth;
            string vcWeek = dataForm.vcWeek;
            string vcPlant = dataForm.vcPlant;
            string msg = "";
            try
            {
                DataTable dt = fS1205_Logic.TXTSearchWeekLevelSchedule(vcMonth, vcWeek, vcPlant);
                if (dt != null && dt.Rows.Count > 0)
                {
                    //生成计划
                    msg = fS1205_Logic.TXTScheduleToPlan(dt, vcMonth, vcWeek, vcPlant, loginInfo.UserId);
                    if (msg.Length > 0)
                    {
                        //大于0，意思是数据中有错误
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = msg;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    else
                    {
                        apiResult.code = ComConstant.SUCCESS_CODE;
                        apiResult.data = "生成计划成功！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "未检索数据不能生成！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "生成计划失败！";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 生成订单
        [HttpPost]
        [EnableCors("any")]
        public string csvFileExport([FromBody] dynamic data)
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
            string vcMonth = dataForm.vcMonth;
            string vcWeek = dataForm.vcWeek;
            string vcPlant = dataForm.vcPlant;
            string msg;
            try
            {
                DataTable dtSource = new DataTable();
                msg = fS1205_Logic.TXTCSVInfoTableMaker(vcMonth, vcWeek, vcPlant, ref dtSource); //检索CSV用的数据，包装计划为基础;
                if (msg.Length > 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = msg;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                else
                {
                    DataTable dtCol = dtSource.Clone();
                    if (dtSource.Rows.Count > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        //获取列名信息（不用）
                        //string[] strColumnNames = dtSource.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();
                        //sb.AppendLine(string.Join(",", strColumnNames));
                        //获取每行数据信息
                        foreach (DataRow dtRow in dtSource.Rows)
                        {
                            for (int j = 0; j < dtCol.Columns.Count; j++)
                            {
                                if (j != dtCol.Columns.Count - 1)
                                {
                                    sb.Append(dtRow[j].ToString() + ",");
                                }
                                else
                                {
                                    sb.Append(dtRow[j].ToString());
                                    sb.AppendLine();
                                }
                            }
                        }
                        //文件名：EMERGENCY_4_201810_20181025133627
                        string name = "";
                        switch (vcPlant)
                        {
                            case "1": name = "0"; break;
                            case "2": name = "4"; break;
                            case "3": name = "8"; break;
                        }
                        string CSVName = "EMERGENCY_" + name + "_" + vcMonth.Replace("-", "") + "_" + vcWeek + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                        string path = System.IO.Directory.GetCurrentDirectory() + "\\Doc\\Export\\" + CSVName + ".csv";
                        string pathupdate = CSVName + ".csv";
                        //生成CSV文件
                        System.IO.File.WriteAllText(path, sb.ToString());
                        if (System.IO.File.Exists(path))
                        {
                            //string strtmp = this.gvMonPlan.Rows[0].Cells[0].Text;
                            //if (strtmp == "初始化状态没有数据，请进行检索！")
                            //{
                            //}
                            //下载窗口
                            //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "window.open('../tmp/" + pathupdate + "');", true);
                            //ShowMessage("导出CSV文件成功: (" + CSVName + ")！", MessageType.Information);
                            apiResult.code = ComConstant.SUCCESS_CODE;
                            apiResult.data = CSVName + ".csv";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        else
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = msg;
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                    else
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "无可导出的数据！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "生成计划失败！";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 导出计划
        [HttpPost]
        [EnableCors("any")]
        public string planFileExport([FromBody] dynamic data)
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
            string vcMonth = dataForm.vcMonth;
            string vcWeek = dataForm.vcWeek;
            string vcPlant = dataForm.vcPlant;
            string vcType = dataForm.vcType;
            try
            {
                DataTable dtSource = fS1205_Logic.TXTExportPlan(vcMonth, vcWeek, vcPlant, vcType); //检索数据
                if (dtSource.Rows.Count > 0)
                {
                    fS1205_Logic.PartsNoFomatTo10(ref dtSource);
                    //string name = "";
                    //switch (vcType)
                    //{
                    //    case "0": name = "包装周度计划"; break;
                    //    case "2": name = "看板打印周度计划"; break;
                    //    case "1": name = "生产周度计划"; break;
                    //    case "3": name = "涂装周度计划"; break;
                    //    case "4": name = "工程3周度计划"; break;
                    //}
                    string[] fields = { "vcMonth", "vcPlant", "vcPartsno", "vcDock", "vcCarType", "vcCalendar1","vcCalendar2","vcCalendar3","vcCalendar4"
                        ,"vcPartsNameCHN","vcProject1","vcProjectName","vcCurrentPastCode","vcMonTotal"
                        ,"TD1b","TD1y","TD2b","TD2y","TD3b","TD3y","TD4b","TD4y","TD5b","TD5y","TD6b","TD6y"
                        ,"TD7b","TD7y","TD8b","TD8y","TD9b","TD9y","TD10b","TD10y","TD11b","TD11y","TD12b","TD12y"
                        ,"TD13b","TD13y","TD14b","TD14y","TD15b","TD15y","TD16b","TD16y","TD17b","TD17y","TD18b","TD18y"
                        ,"TD19b","TD19y","TD20b","TD20y","TD21b","TD21y","TD22b","TD22y","TD23b","TD23y","TD24b","TD24y"
                        ,"TD25b","TD25y","TD26b","TD26y","TD27b","TD27y","TD28b","TD28y","TD29b","TD29y","TD30b","TD30y"
                        ,"TD31b","TD31y"
                        ,"ED1b","ED1y","ED2b","ED2y","ED3b","ED3y","ED4b","ED4y","ED5b","ED5y","ED6b","ED6y"
                        ,"ED7b","ED7y","ED8b","ED8y","ED9b","ED9y","ED10b","ED10y","ED11b","ED11y","ED12b","ED12y"
                        ,"ED13b","ED13y","ED14b","ED14y","ED15b","ED15y","ED16b","ED16y","ED17b","ED17y","ED18b","ED18y"
                        ,"ED19b","ED19y","ED20b","ED20y","ED21b","ED21y","ED22b","ED22y","ED23b","ED23y","ED24b","ED24y"
                        ,"ED25b","ED25y","ED26b","ED26y","ED27b","ED27y","ED28b","ED28y","ED29b","ED29y","ED30b","ED30y"
                        ,"ED31b","ED31y"
                    };
                    string filepath = ComFunction.generateExcelWithXlt(dtSource, fields, _webHostEnvironment.ContentRootPath, "FS1205_PlanMake.xlsx", 1, loginInfo.UserId, FunctionID);
                    if (filepath == "")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "导出计划失败！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = filepath;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                else
                {
                    //ShowMessage("无可导出的数据！", MessageType.Information);
                    //InitGrid();
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "无可导出的数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
