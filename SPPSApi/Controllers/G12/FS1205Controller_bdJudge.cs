using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;


namespace SPPSApi.Controllers.G12
{
    [Route("api/FS1205_bdJudge/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1205Controller_bdJudge : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        FS1205_Logic fS1205_Logic = new FS1205_Logic();
        private readonly string FunctionID = "FS1205";

        public FS1205Controller_bdJudge(IWebHostEnvironment webHostEnvironment)
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
                res.Add("PlantSource", dataList_PlantSource);
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
            string vcMonth = dataForm.vcMonth == null ? "" : dataForm.vcMonth;
            string vcWeek = dataForm.vcWeek == null ? "" : dataForm.vcWeek;
            string vcPlant = dataForm.vcPlant == null ? "" : dataForm.vcPlant;
            try
            {
                DataTable dt = fS1205_Logic.TXTSearchWeekLevelPercentage(vcMonth, vcWeek, vcPlant);
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
                string vcMonth = dataForm.vcMonth == null ? "" : dataForm.vcMonth;
                string vcWeek = dataForm.vcWeek == null ? "" : dataForm.vcWeek;
                string vcPlant = dataForm.vcPlant == null ? "" : dataForm.vcPlant;
                JArray listInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                bool hasFind = false;//是否找到需要新增或者修改的数据
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bModFlag == true)
                    {//修改
                        hasFind = true;
                    }
                }
                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少有一个编辑行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                try
                {
                    DataTable dt = fS1205_Logic.TXTSearchWeekLevelPercentage(vcMonth, vcWeek, vcPlant);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        for (int j = 0; j < listInfoData.Count; j++)
                        {
                            if (dt.Rows[i]["iAutoId"].ToString() == listInfoData[j]["iAutoId"].ToString())
                                dt.Rows[i]["vcFlag"] = listInfoData[j]["vcFlag"].ToString();
                        }
                    }
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "无可更新的数据请先检索！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    else
                    {
                        //遍历检查一遍有没有NG数据
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (dt.Rows[i]["vcFlag"].ToString() == "N")
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = "数据中有NG的数据，不能更新！";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                        }
                        //更新操作
                        try
                        {
                            string Month = dt.Rows[0]["vcMonth"].ToString();//获取数据源中的对象月
                            string OrderNo = dt.Rows[0]["vcOrderNo"].ToString();//获取数据源中的订单号
                            string Week = dt.Rows[0]["vcWeek"].ToString();//获取数据源中的对象周
                            string Plant = dt.Rows[0]["vcPlant"].ToString();//获取数据源中的厂区
                            string ColumnName = string.Empty;//对应周的实际订货数量的列名
                            switch (Week)
                            {
                                case "1": ColumnName = "vc1stWeekTotal"; break;
                                case "2": ColumnName = "vc2ndWeekTotal"; break;
                                case "3": ColumnName = "vc3rdWeekTotal"; break;
                                case "4": ColumnName = "vc4thWeekTotal"; break;
                                case "5": ColumnName = "vc5thWeekTotal"; break;
                            }
                            for (int k = 0; k < dt.Rows.Count; k++)
                            {
                                //将本周实际订货数放到对应的周总数中
                                dt.Rows[k][ColumnName] = dt.Rows[k]["vcWeekOrderingCount"];
                                //合计更新
                                dt.Rows[k]["vcTotal"] = (Convert.ToInt32(dt.Rows[k]["vc1stWeekTotal"].ToString()) + Convert.ToInt32(dt.Rows[k]["vc2ndWeekTotal"].ToString()) + Convert.ToInt32(dt.Rows[k]["vc3rdWeekTotal"].ToString()) + Convert.ToInt32(dt.Rows[k]["vc4thWeekTotal"].ToString()) + Convert.ToInt32(dt.Rows[k]["vc5thWeekTotal"].ToString())).ToString();
                            }
                            //设置主键
                            dt.PrimaryKey = new DataColumn[]
                            {
                                dt.Columns["vcMonth"],
                                dt.Columns["vcOrderNo"],
                                dt.Columns["vcWeek"],
                                dt.Columns["vcPlant"],
                                dt.Columns["vcGC"],
                                dt.Columns["vcZB"],
                                dt.Columns["vcPartsno"]
                            };
                            fS1205_Logic.TXTUpdateTableDetermine(dt, Month, OrderNo, Week, Plant);
                            //全部设定为OK后，开始进行平准化操作
                            string msg = fS1205_Logic.TXTInsertToWeekLevelSchedule(dt);
                            if (msg.Length > 0)
                            {
                                //大于0，意思是有异常情况
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = msg;
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                            else
                            {
                                //最终提示信息
                                apiResult.code = ComConstant.SUCCESS_CODE;
                                apiResult.data = null;
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                        }
                        catch (Exception ex)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "更新失败！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                }
                catch (Exception ex)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "更新失败！";
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0902", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
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
            string vcMonth = dataForm.vcMonth;
            string vcWeek = dataForm.vcWeek;
            string vcPlant = dataForm.vcPlant;
            string msg = "";
            try
            {
                DataTable dt = fS1205_Logic.TXTSearchWeekLevelPercentage(vcMonth, vcWeek, vcPlant);
                if (dt != null && dt.Rows.Count > 0)
                {
                    string exlName = "";
                    DataTable tb = fS1205_Logic.BdpdFileExport(dt, ref exlName, ref msg);
                    if (tb != null && tb.Rows.Count > 0)
                    {
                        string[] fields = { "vcMonth", "vcWeek", "vcPartsno", "vcWeekTotal", "vcWeekOrderingCount", "vcWeekLevelPercentage", "vcFlag", "vcQuantityPerContainer", "vcAdjust", "vcMonTotal", "vcRealTotal" };
                        string filepath = ComFunction.generateExcelWithXlt(tb, fields, _webHostEnvironment.ContentRootPath, "FS1205_Exp.xlsx", 1, loginInfo.UserId, FunctionID, true);
                        if (filepath == "")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "导出失败！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        apiResult.code = ComConstant.SUCCESS_CODE;
                        apiResult.data = filepath;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "无数据，导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0904", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 计算
        [HttpPost]
        [EnableCors("any")]
        public string getDataApi([FromBody] dynamic data)
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
            try
            {
                if (CheckWeek(vcMonth, vcWeek, vcPlant) == false)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "对象月#" + vcPlant + "厂" + NumberToText(vcWeek) + "的稼动日历不存在！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                #region 检索txt文件路径
                DataTable rtTb = fS1205_Logic.getTxtFileRoute(vcMonth, vcWeek);
                if (rtTb == null || rtTb.Rows.Count <= 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "未上传文件，不能计算！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                #endregion
                string path = _webHostEnvironment.ContentRootPath + "\\Doc\\orders" + (rtTb.Rows[0]["vcFilePath"] == null ? "" : rtTb.Rows[0]["vcFilePath"].ToString());
                try
                {
                    string[] Line = System.IO.File.ReadAllLines(path);//读取TXT文件里面所有行的字符串
                    string OrderType = Line[0].Substring(22, 1);//订单类型：2为紧急，3为月度，要2的订单文件                
                    if (OrderType == "3")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "订单类型不正确！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    //标记位E是不是和订单类型有关待确认

                    string vcCSVFlag = OrderType == "2" ? "E" : "S";//CSV，已确认，2位紧急，标记为E，3位月度标记为S
                    DataTable dtPartsWeekLevel = fS1205_Logic.TXTCloneWeekLevelPercentage();//获取周计划变动幅度管理表结构                
                    string vcOrderNo = Line[0].Substring(14, 8);//订单号

                    for (int i = 0; i < Line.Length; i++)
                    {
                        #region 按照TXT行数的循环体
                        if (Line[i].Substring(0, 1) == "D")//D开头的为明细部分
                        {
                            string strPartsno = Line[i].Substring(26, 15).Trim();//品番
                            string vcPartsno = fS1205_Logic.TXTPartsNoFomatTo12(strPartsno);//格式化品番
                            string vcGC = string.Empty;//部署
                            string vcZB = string.Empty;//组别
                            string vcCSVItemNo = Line[i].Substring(22, 4).Trim();//CSV
                            string vcCSVOrderDate = Line[i].Substring(6, 8).Trim();//CSV
                            string vcCSVCpdCompany = Line[i].Substring(1, 5).Trim();//CSV
                                                                                    //月度周度校验
                            if (fS1205_Logic.TXTIsPartFrequence(vcPartsno, vcMonth) == false)
                            {
                                //false为月度品番
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = "第" + (i + 1).ToString() + "行，品番：" + vcPartsno + "的品番频度没有维护或为月度品番！";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                            //厂区校验
                            string _Plant = fS1205_Logic.TXTFindPartPlant(vcPartsno, vcMonth);
                            if (_Plant == string.Empty)
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = "第" + (i + 1).ToString() + "行，品番：" + vcPartsno + "没有厂区信息！";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                            else
                            {
                                if (_Plant != vcPlant)
                                {
                                    //apiResult.code = ComConstant.ERROR_CODE;
                                    //apiResult.data = "第" + (i + 1).ToString() + "行，品番：" + vcPartsno + "的厂区为" + _Plant + "，不在所选厂区" + vcPlant + "中！";
                                    //return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                    continue;
                                }               
                            }
                            //部署组别唯一性校验

                            fS1205_Logic.TXTFindGCAndZB(vcPartsno, vcMonth, ref vcGC, ref vcZB);
                            if (vcGC == string.Empty || vcZB == string.Empty)
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = "第" + (i + 1).ToString() + "行，品番：" + vcPartsno + "查询部署和组别的信息不存在或不唯一！";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                            //收容数存在性校验

                            string vcWeekOrderingCount = Convert.ToInt32(Line[i].Substring(41, 7)).ToString();//周订货数量

                            int iQuantityPerContainer = fS1205_Logic.TXTQuantity(vcPartsno, vcMonth);//获取该品番的看板收容数

                            if (iQuantityPerContainer == 0)
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = "第" + (i + 1).ToString() + "行，品番：" + vcPartsno + "的看板收容数不存在，请及时维护！";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                            //周订货数量整除校验

                            int iWeekOrderingCount = Convert.ToInt32(vcWeekOrderingCount);//周订货数量

                            if (iWeekOrderingCount % iQuantityPerContainer != 0)
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = "第" + (i + 1).ToString() + "行，品番：" + vcPartsno + "的周订货数量不是看板收容数的整数倍！";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                            //获取该品番的车型
                            string vcCSVCarFamilyCode = fS1205_Logic.TXTFindCarType(vcPartsno, vcMonth);//CSV
                                                                                                        //开始从对应的月度包装计划获取计划数据

                            string vcCalendar1 = fS1205_Logic.TXTFindProject1(vcGC, vcZB);
                            //（暂时不用）月度包装计划存在性校验，FALSE为该品番在当前对象月不存在月度包装计划

                            //if (lg.TXTCheckMonthPlanExist(vcMonth, vcCalendar1, vcPartsno) == false)
                            //{
                            //    ShowMessage("第" + (i + 1).ToString() + "行，品番：" + vcPartsno + "在当前对象月不存在月度包装计划！", MessageType.Error);
                            //    return;
                            //}
                            //int[] iWeekNum = new int[5];//存放五周的天数（没用上）
                            DataTable dtASP = fS1205_Logic.TXTWeekCalendar(vcPlant, vcMonth, vcWeek, vcCalendar1, vcPartsno);//此表与周计划变动幅度管理表结构相同

                            decimal decWeekLevelPercentage = 0;//波动范围（小数）
                            decimal decWeekTotal = Convert.ToDecimal(dtASP.Rows[0]["vcWeekTotal"]);//该周内示总量
                            decimal decWeekOrderingCount = Convert.ToDecimal(vcWeekOrderingCount);//该周周订货数量

                            //变动幅度为：（周订货数-周内示总量）/周内示总量
                            //周内示数为0时，波动幅度为100%
                            if (decWeekTotal == 0)
                            {
                                decWeekLevelPercentage = 1;
                            }
                            else
                            {
                                decWeekLevelPercentage = (decWeekOrderingCount - decWeekTotal) / decWeekTotal;
                            }
                            //转化成百分数字符串，并保留两位小数

                            string vcWeekLevelPercentage = System.Math.Round(decWeekLevelPercentage * 100, 2).ToString() + "%";
                            //波动判定
                            string vcFlag = string.Empty;
                            //波动范围*100，再取绝对值

                            if (System.Math.Abs(decWeekLevelPercentage * 100) > 20)
                            {
                                vcFlag = "N";//幅度大于20%则判定为NG
                            }
                            else
                            {
                                vcFlag = "Y";//幅度小于等于20%则判定为OK
                            }
                            //开始在临时表中插入数据
                            DataRow dr = dtPartsWeekLevel.NewRow();
                            //先插入从月度包装计划中获取的数据
                            for (int k = 0; k < dtASP.Columns.Count; k++)
                            {
                                if (dtASP.Rows[0][k].ToString() != string.Empty)
                                {
                                    dr[k] = dtASP.Rows[0][k];//dtPartsWeekLevel与dtASP结构相同
                                }
                            }
                            dr["vcFlag"] = vcFlag;
                            dr["vcMonth"] = vcMonth;
                            dr["vcOrderNo"] = vcOrderNo;
                            dr["vcWeek"] = vcWeek;
                            dr["vcPlant"] = vcPlant;
                            dr["vcGC"] = vcGC;
                            dr["vcZB"] = vcZB;
                            dr["vcPartsno"] = vcPartsno;
                            dr["vcWeekOrderingCount"] = vcWeekOrderingCount;
                            dr["vcWeekLevelPercentage"] = vcWeekLevelPercentage;
                            //CSV文件信息
                            dr["vcCSVFlag"] = vcCSVFlag;
                            dr["vcCSVItemNo"] = vcCSVItemNo;
                            dr["vcCSVOrderDate"] = vcCSVOrderDate;
                            dr["vcCSVCpdCompany"] = vcCSVCpdCompany;
                            dr["vcCSVCarFamilyCode"] = vcCSVCarFamilyCode;
                            dtPartsWeekLevel.Rows.Add(dr);
                        }
                        #endregion
                    }
                    //设置主键
                    dtPartsWeekLevel.PrimaryKey = new DataColumn[]
                    {
                    dtPartsWeekLevel.Columns["vcMonth"],
                    dtPartsWeekLevel.Columns["vcOrderNo"],
                    dtPartsWeekLevel.Columns["vcWeek"],
                    dtPartsWeekLevel.Columns["vcPlant"],
                    dtPartsWeekLevel.Columns["vcGC"],
                    dtPartsWeekLevel.Columns["vcZB"],
                    dtPartsWeekLevel.Columns["vcPartsno"]
                    };
                    fS1205_Logic.TXTUpdateTableDetermine(dtPartsWeekLevel, vcMonth, vcOrderNo, vcWeek, vcPlant);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                //DataTable dt = new DataTable();
                //DtConverter dtConverter = new DtConverter();
                //List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
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

        #region 通用方法
        public DataTable ListToDataTable(List<Dictionary<string, Object>> listInfoData)
        {
            DataTable tb = new DataTable();
            if (listInfoData.Count > 0)
            {
                Dictionary<string, object> li = listInfoData[0];
                for (int i = 0; i < li.Count - 1; i++)
                {
                    string colName = li.ToList()[i].Key;
                    tb.Columns.Add(new DataColumn(colName, typeof(string)));
                }
                foreach (Dictionary<string, object> li1 in listInfoData)
                {
                    DataRow r = tb.NewRow();
                    for (int j = 0; j < tb.Columns.Count; j++)
                        r[j] = (li1[tb.Columns[j].ColumnName] == null) ? null : li1[tb.Columns[j].ColumnName].ToString();
                    tb.Rows.Add(r);
                }
            }
            return tb;
        }

        #region 检测所选对象周在当月是否够存在 - 李兴旺
        /// <summary>
        /// 检测所选对象周在当月是否够存在
        /// </summary>
        private bool CheckWeek(string strMonth, string strWeekNum, string strPlant)
        {
            int iWeekNum = Convert.ToInt32(strWeekNum);//所选周数
            int iWeekCount = fS1205_Logic.TXTWeekCount(strMonth, strPlant);//对象月有几周
            if (iWeekNum > iWeekCount)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region 数字与周数文字对应关系 - 李兴旺
        /// <summary>
        /// 数字与周数文字对应关系
        /// </summary>
        /// <param name="iWeekNum">周数数字</param>
        /// <returns>周数文字</returns>
        private string NumberToText(string iWeekNum)
        {
            string strWeekNum = string.Empty;
            switch (iWeekNum)
            {
                case "1": strWeekNum = "第一周"; break;
                case "2": strWeekNum = "第二周"; break;
                case "3": strWeekNum = "第三周"; break;
                case "4": strWeekNum = "第四周"; break;
                case "5": strWeekNum = "第五周"; break;
                default: strWeekNum = ""; break;
            }
            return strWeekNum;
        }
        #endregion
        #endregion
    }
}
