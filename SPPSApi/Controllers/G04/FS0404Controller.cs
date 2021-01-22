﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
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

namespace SPPSApi.Controllers.G04
{
    [Route("api/FS0404/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0404Controller : BaseController
    {
        FS0404_Logic fs0404_Logic = new FS0404_Logic();
        private readonly string FunctionID = "FS0404";

        private readonly IWebHostEnvironment _webHostEnvironment;


        public FS0404Controller(IWebHostEnvironment webHostEnvironment)
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
                Dictionary<string, object> res = new Dictionary<string, object>();

                List<Object> dataList_C045 = ComFunction.convertAllToResult(ComFunction.getTCode("C045"));//荷姿状态
                List<Object> dataList_C046 = ComFunction.convertAllToResult(ComFunction.getTCode("C046"));//荷姿状态
                List<Object> dataList_C003 = ComFunction.convertAllToResult(ComFunction.getTCode("C003"));//荷姿状态
                res.Add("C045", dataList_C045);
                res.Add("C046", dataList_C046);
                res.Add("C003", dataList_C003);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0401", ex, loginInfo.UserId);
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
            string vcOrderState = dataForm.vcOrderState == null ? "" : dataForm.vcOrderState;
            string vcInOutFlag = dataForm.vcInOutFlag == null ? "" : dataForm.vcInOutFlag;
            string vcOrderNo = dataForm.vcOrderNo == null ? "" : dataForm.vcOrderNo;
            string dTargetDate = dataForm.dTargetDate == null ? "" : dataForm.dTargetDate;
            string vcOrderType = dataForm.vcOrderType == null ? "" : dataForm.vcOrderType;
            try
            {
                DataTable dt = fs0404_Logic.Search(vcOrderState, vcInOutFlag,vcOrderNo, dTargetDate, vcOrderType,loginInfo.UserId);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dUploadDate", ConvertFieldType.DateType, "yyyy-MM-dd HH:mm:ss");
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0402", ex, loginInfo.UserId);
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
            string vcOrderState = dataForm.vcOrderState == null ? "" : dataForm.vcOrderState;
            string vcInOutFlag = dataForm.vcInOutFlag == null ? "" : dataForm.vcInOutFlag;
            string vcOrderNo = dataForm.vcOrderNo == null ? "" : dataForm.vcOrderNo;
            string dTargetDate = dataForm.dTargetDate == null ? "" : dataForm.dTargetDate;
            string vcOrderType = dataForm.vcOrderType == null ? "" : dataForm.vcOrderType;
            try
            {
                DataTable dt = fs0404_Logic.Search(vcOrderState, vcInOutFlag, vcOrderNo, dTargetDate, vcOrderType, loginInfo.UserId);
                string[] head = new string[] { };
                string[] field = new string[] { };
                //[vcOrderNo], [dTargetDate], [vcOrderType] ,vcOrderState,[dUploadDate],[vcMemo]
                head = new string[] { "订单编号", "对象年月日", "订单类型","内外", "状态", "备注", "上传时间" };

                field = new string[] { "vcOrderNo", "dTargetDate", "vcOrderType", "vcInOutFlag", "vcOrderState", "vcMemo", "dUploadDate" };
                string msg = string.Empty;
                //string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0309_Export.xlsx", 2, loginInfo.UserId, FunctionID);
                string filepath = ComFunction.DataTableToExcel(head, field, dt, ".", loginInfo.UserId, FunctionID, ref msg);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0403", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 上传确定
        [HttpPost]
        [EnableCors("any")]
        public string updateOKApi([FromBody] dynamic data)
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
            string vcOrderType = dataForm.vcOrderType == null ? "" : dataForm.vcOrderType;
            string vcInOutFlag = dataForm.vcInOutFlag == null ? "" : dataForm.vcInOutFlag;
            string dTargetDate = dataForm.dTargetDate == null ? "" : dataForm.dTargetDate;
            string lastOrderNo = dataForm.lastOrdderNo == null ? "" : dataForm.lastOrdderNo;
            string newOrderNo = dataForm.newOrderNo == null ? "" : dataForm.newOrderNo;
            string vcMemo = dataForm.vcMemo == null ? "" : dataForm.vcMemo;
            string dTargetWeek = dataForm.dTargetWeek == null ? "" : dataForm.dTargetWeek;
            List<Dictionary<string, Object>> fileList = dataForm.file.ToObject<List<Dictionary<string, Object>>>();
            List<Dictionary<string, Object>> fileDelList = dataForm.fileDel.ToObject<List<Dictionary<string, Object>>>();
            try
            {
                if (vcOrderType == "0")
                {
                    dTargetDate = dTargetDate.Replace("-", "");
                } else if (vcOrderType == "1")
                {
                    dTargetDate = dTargetDate.Replace("-", "").Substring(0,4);
                }
                else if (vcOrderType == "2") {
                    dTargetDate = dTargetDate.Replace("-", "").Substring(0, 6);
                } else
                { 
                
                }
                //if (vcInOutFlag.Length==0)
                //{
                //    apiResult.code = ComConstant.ERROR_CODE;
                //    apiResult.data = "内外选项不能为空,请确认！";
                //    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //}
                    #region 判断
                    #endregion
                if (lastOrderNo.Length >0)
                {
                    if (newOrderNo.Length==0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "请确认是否在修正订单，修正订单必须输入原订单号和修正订单号,请确认！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                //订单类型	0	日度
                //订单类型	1	周度
                //订单类型	2	月度
                //订单类型	3	紧急
                #region
                String realPath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "orders";
                if (vcOrderType=="0")
                {
                    if (dTargetDate.Length==0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "订单类型为日度时，对象年月(日)不能为空！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (fileList.Count>1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "订单类型为日度时，每次仅能上传一个文件,请确认！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    for (int i = 0; i < fileList.Count; i++)
                    {
                        if (fileList[i]["fileName"].ToString().Substring(0, 2).ToUpper() != "RD")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "文件名不匹配日度订单命名规则，请以RD开始命名 '";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        string strfilePath = fileList[i]["filePath"].ToString();
                        string strMsg = string.Empty;
                        Order order = GetPartFromFile(realPath+strfilePath, ref strMsg);
                        if (strMsg.Length>0)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = strMsg;
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }else
                        {
                            //月度订单	S　	S　  紧急订单 H   E     特殊订单    R E周度订单 W   E 日度订单    D E大客户订单 F   F三包订单    C E
                            
                            string code = order.Head.ToString().Substring(0, 1);
                            if (code!= "D")
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = "日度订单的Code代码必须是D开始，请查看订单内容！'";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                        }
                    }
                }
                if (vcOrderType == "1")
                {
                    if (dTargetDate.Length == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "订单类型为周度时，对象周的对象年月不能为空！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (dTargetWeek.Length == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "订单类型为周度时，对象周不能为空！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (fileList.Count > 1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "订单类型为周度时，每次仅能上传一个文件,请确认！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    for (int i = 0; i < fileList.Count; i++)
                    {
                        if (fileList[i]["fileName"].ToString().Substring(0, 2).ToUpper() != "ZD")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "文件名不匹配周度订单命名规则，请以ZD开始命名 '";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        string strfilePath = fileList[i]["filePath"].ToString();
                        string strMsg = string.Empty;
                        Order order = GetPartFromFile(realPath + strfilePath, ref strMsg);
                        if (strMsg.Length > 0)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = strMsg;
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        else
                        {
                            //月度订单	S　	S　  紧急订单 H   E     特殊订单    R E周度订单 W   E 日度订单    D E大客户订单 F   F三包订单    C E

                            string code = order.Head.ToString().Substring(0, 1);
                            if (code != "W")
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = "日度订单的Code代码必须是W开始，请查看订单内容！'";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                        }
                    }
                }
                if (vcOrderType == "2")
                {
                    if (dTargetDate.Length == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "订单类型为月度时，对象年月(日)不能为空！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (lastOrderNo.Length > 0 && newOrderNo.Length > 0)
                    {
                        if (fileList.Count != 1)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "订单类型为月度时，修正订单每次仅上传一个文件,请确认！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                    else {
                        if (fileList.Count != 2)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "订单类型为月度时，每次仅必须上传两个文件,请确认！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                    
                    for (int i = 0; i < fileList.Count; i++)
                    {
                        if (fileList[i]["fileName"].ToString().Substring(0, 2).ToUpper() != "YD")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "文件名不匹配月度订单命名规则，请以YD开始命名 '";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        string fileNameNew = fileList[i]["fileName"].ToString().Trim().Substring(0, fileList[i]["fileName"].ToString().Trim().LastIndexOf("."));
                        string fileOrderNoNew = fileNameNew.Substring(fileNameNew.LastIndexOf("-") + 1);
                        ////订单类型	0	日度
                        //订单类型	1	周度
                        //订单类型	2	月度
                        //订单类型	3	紧急
                         string vcInOutFlagNew = fileOrderNoNew.Substring(fileOrderNoNew.Length - 1, 1);
                        if (vcInOutFlagNew!="0"&& vcInOutFlagNew != "1")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "月度订单文件名最后一位必须是0或1,用于区分内制0和外注1！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }

                        string strfilePath = fileList[i]["filePath"].ToString();
                        string strMsg = string.Empty;
                        Order order = GetPartFromFile(realPath + strfilePath, ref strMsg);
                        if (strMsg.Length > 0)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = strMsg;
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        else
                        {
                            //月度订单	S　	S　  紧急订单 H   E     特殊订单    R E周度订单 W   E 日度订单    D E大客户订单 F   F三包订单    C E

                            string code = order.Head.ToString().Substring(0, 1);
                            if (code != "S")
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = "日度订单的Code代码必须是S开始，请查看订单内容！'";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                        }
                    }
                }
                if (vcOrderType == "3")
                {
                    if (fileList.Count > 1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "订单类型为紧急时，每次仅能上传一个文件,请确认！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    for (int i = 0; i < fileList.Count; i++)
                    {
                        if (fileList[i]["fileName"].ToString().Substring(0, 2).ToUpper() != "JJ")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "文件名不匹配紧急订单命名规则，请以JJ开始命名 '";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        string strfilePath = fileList[i]["filePath"].ToString();
                        string strMsg = string.Empty;
                        Order order = GetPartFromFile(realPath + strfilePath, ref strMsg);
                        if (strMsg.Length > 0)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = strMsg;
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        else
                        {
                            //月度订单	S　	S　  紧急订单 H   E     特殊订单    R E周度订单 W   E 日度订单    D E大客户订单 F   F三包订单    C E

                            string code = order.Head.ToString().Substring(0, 1);
                            if (code != "H")
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = "日度订单的Code代码必须是H开始，请查看订单内容！'";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                        }
                    }
                }
                #endregion
                //判断是修正 还是新增
                if (lastOrderNo.Length > 0 && newOrderNo.Length > 0)
                {
                    //判定原订单号是否存在
                    DataTable dt = fs0404_Logic.isCheckByOrderNo(lastOrderNo);
                    if (dt.Rows.Count==0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "请确认是否在修正订单，修正订单的原订单号不存在,请确认！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    // 判断状态 是0  或者2 撤销  1已做成
                    if (dt.Rows[0]["vcOrderState"].ToString()!="0"&& dt.Rows[0]["vcOrderState"].ToString() != "2")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "请确认是否在修正订单，修正订单的状态必须是已上传未处理或者撤销,请确认！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
           
                    fs0404_Logic.updateBylastOrderNo(vcOrderType, vcInOutFlag, dTargetDate, dTargetWeek, lastOrderNo, newOrderNo, vcMemo, fileList,loginInfo.UserId);
                    if (System.IO.File.Exists(realPath + dt.Rows[0]["vcFilePath"]))
                    {
                        System.IO.File.Delete(realPath + dt.Rows[0]["vcFilePath"]);
                    }
                }
                else
                {
                    fs0404_Logic.addOrderNo(vcOrderType, vcInOutFlag, dTargetDate, dTargetWeek, lastOrderNo, newOrderNo, vcMemo, fileList, loginInfo.UserId);
                }

                //删除误操作的上传订单
                if (fileDelList.Count > 0)
                {
                    for (int i = 0; i < fileDelList.Count; i++) {
                        string filePath = fileDelList[i]["filePath"].ToString();
                        if(filePath.Length>0)
                        {
                            if (System.IO.File.Exists(realPath + filePath))
                            {
                                System.IO.File.Delete(realPath + filePath);
                            }
                        }
                    }
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0404", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "上传订单失败！";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 读取txt

        public Order GetPartFromFile(string path, ref string msg)
        {
            string[] strs = System.IO.File.ReadAllLines(@path);
            Order order = new Order();

            Head head = new Head();
            List<Detail> details = new List<Detail>();
            Tail tail = new Tail();

            //获取Detail
            for (int i = 0; i < strs.Length; i++)
            {
                string temp = strs[i];
                //判断空行
                if (string.IsNullOrWhiteSpace(temp))
                {
                    continue;
                }
                //获取Head
                if (temp[0] == 'D')
                {
                    Detail detail = new Detail();
                    detail.DataId = temp.Substring(0, 1);
                    detail.CPD = temp.Substring(1, 5);
                    detail.Date = temp.Substring(6, 8);
                    detail.Type = temp.Substring(14, 8);
                    detail.ItemNo = temp.Substring(22, 4);
                    detail.PartsNo = temp.Substring(26, 12);
                    detail.QTY = temp.Substring(41, 7);
                    detail.Price = temp.Substring(48, 9);
                    details.Add(detail);
                }
                else if (temp[0] == 'H')
                {
                    head.DataId = temp.Substring(0, 1);
                    head.CPD = temp.Substring(1, 5);
                    head.Date = temp.Substring(6, 8);
                    head.No = temp.Substring(14, 8);
                    head.Type = temp.Substring(22, 1);
                    head.SendDate = temp.Substring(28, 8);
                }
                else if (temp[0] == 'T')
                {
                    tail.DataId = temp.Substring(0, 1);
                    tail.CPD = temp.Substring(1, 5);
                    tail.Date = temp.Substring(6, 8);
                    tail.No = temp.Substring(14, 8);
                }
            }

            order.Head = head;
            order.Details = details;
            order.Tail = tail;

            if (order.Head == null)
            {
                msg = "订单Head部分有误";
                return null;
            }
            else if (order.Details.Count == 0)
            {
                msg = "订单Detail为空";
                return null;
            }
            else if (order.Tail == null)
            {
                msg = "订单Tail部分有误";
                return null;
            }

            return order;
        }

        public class Order
        {
            public Order()
            {
                this.Details = new List<Detail>();
            }

            public Head Head;
            public List<Detail> Details;
            public Tail Tail;
        }

        public class Head
        {
            public string DataId;
            public string CPD;
            public string Date;
            public string No;
            public string Type;
            public string Code;
            public string SendDate;
        }

        public class Detail
        {
            public string DataId;
            public string CPD;
            public string Date;
            public string Type;
            public string ItemNo;
            public string PartsNo;
            public string QTY;
            public string Price;

        }

        public class Tail
        {
            public string DataId;
            public string CPD;
            public string Date;
            public string No;
        }


        #endregion
    }
}

