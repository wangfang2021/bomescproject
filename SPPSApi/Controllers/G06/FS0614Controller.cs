using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Logic;

namespace SPPSApi.Controllers.G06
{
    [Route("api/FS0614/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0614Controller : BaseController
    {
        FS0614_Logic fs0614_logic = new FS0614_Logic();
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0614";

        public FS0614Controller(IWebHostEnvironment webHostEnvironment)
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

                List<Object> dataList_C044 = ComFunction.convertAllToResult(ComFunction.getTCode("C044")); //收货方
                List<Object> dataList_C045 = ComFunction.convertAllToResult(ComFunction.getTCode("C045")); //原单位


                res.Add("C044", dataList_C044);
                res.Add("C045", dataList_C045);
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
            string orderState = dataForm.orderState == null ? "" : dataForm.orderState;
            string targetYM = dataForm.targetYM == null ? "" : dataForm.targetYM;
            targetYM = targetYM.Replace("-", "");
            string orderNo = dataForm.orderNo == null ? "" : dataForm.orderNo;
            string orderType = dataForm.orderType == null ? "" : dataForm.orderType;
            string dUpload = dataForm.dUpload == null ? "" : dataForm.dUpload;
            try
            {
                DataTable dt = fs0614_logic.searchApi(orderState, targetYM, orderNo, orderType, dUpload);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("dUploadDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dCreateDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;

                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M02UE0201", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }

        #endregion

        #region 生成订单
        [HttpPost]
        [EnableCors("any")]
        public string createApi([FromBody]dynamic data)
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
                bool hasFind = false;//是否找到需要新增或者修改的数据
                if (listInfoData.Count > 0)
                {
                    hasFind = true;
                }
                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选中一行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                string refMsg = "";
                //开始数据验证
                if (hasFind)
                {
                    bool flag = fs0614_logic.checkType(listInfoData, ref refMsg);
                    if (!flag)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = refMsg;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string realPath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" +
                                      Path.DirectorySeparatorChar + "orders";

                    fs0614_logic.CreateOrder(listInfoData, realPath, loginInfo.UserId, ref refMsg);

                }


                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0705", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }

        #endregion

        #region 撤销订单
        [HttpPost]
        [EnableCors("any")]
        public string cancelApi([FromBody]dynamic data)
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
                bool hasFind = false;//是否找到需要新增或者修改的数据
                if (listInfoData.Count > 0)
                {
                    hasFind = true;
                }
                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选中一行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                string refMsg = "";
                //开始数据验证
                if (hasFind)
                {
                    fs0614_logic.cancelFile(listInfoData, loginInfo.UserId);
                }

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0705", ex, loginInfo.UserId);
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
            string fileName = dataForm.fileName;

            try
            {
                string realPath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" +
                                  Path.DirectorySeparatorChar + "orders";
                string filepath = fs0614_logic.getPath(fileName);
                if (!string.IsNullOrWhiteSpace(filepath))
                {
                    string fileSavePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" +
                                          Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar +
                                          "Order"; //文件临时目录，导入完成后 删除
                    string tmp = fileSavePath + Path.DirectorySeparatorChar;
                    if (Directory.Exists(tmp))
                    {
                        DeleteFolder(tmp);
                    }

                    string filepathTmp = realPath + filepath;
                    string filePathFinally = fileSavePath + filepath;

                    string strPath = Path.GetDirectoryName(filePathFinally);
                    if (!Directory.Exists(strPath))
                    {
                        Directory.CreateDirectory(strPath);
                    }

                    if (System.IO.File.Exists(filepathTmp))
                    {
                        System.IO.File.Copy(filepathTmp, filePathFinally, true);
                        filepath = "Order" + Path.DirectorySeparatorChar + filepath;
                    }
                    else
                    {
                        filepath = "";
                    }
                }
                else
                {
                    filepath = "";
                }

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = filepath;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE1403", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "下载文件失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }

        #endregion

        #region 文件夹方法


        #region 获取文件路径

        public string getPath(string fileName)
        {
            return "";
        }

        #endregion
        #region 清空文件夹

        public void DeleteSrcFolder(string file)
        {
            //去除文件夹和子文件的只读属性
            //去除文件夹的只读属性
            System.IO.DirectoryInfo fileInfo = new DirectoryInfo(file);
            fileInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;
            //去除文件的只读属性
            System.IO.File.SetAttributes(file, System.IO.FileAttributes.Normal);
            //判断文件夹是否还存在
            if (Directory.Exists(file))
            {
                foreach (string f in Directory.GetFileSystemEntries(file))
                {
                    if (System.IO.File.Exists(f))
                    {
                        //如果有子文件删除文件
                        System.IO.File.Delete(f);
                    }
                    else
                    {
                        //循环递归删除子文件夹 
                        DeleteFolder(f);
                    }
                }
                //删除空文件夹
                //Directory.Delete(file);
            }
        }

        public void DeleteFolder(string file)
        {
            //去除文件夹和子文件的只读属性
            //去除文件夹的只读属性
            System.IO.DirectoryInfo fileInfo = new DirectoryInfo(file);
            fileInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;
            //去除文件的只读属性
            System.IO.File.SetAttributes(file, System.IO.FileAttributes.Normal);
            //判断文件夹是否还存在
            if (Directory.Exists(file))
            {
                foreach (string f in Directory.GetFileSystemEntries(file))
                {
                    if (System.IO.File.Exists(f))
                    {
                        //如果有子文件删除文件
                        System.IO.File.Delete(f);
                    }
                    else
                    {
                        //循环递归删除子文件夹 
                        DeleteFolder(f);
                    }
                }
                //删除空文件夹
                Directory.Delete(file);
            }
        }
        #endregion

        #endregion

    }
}