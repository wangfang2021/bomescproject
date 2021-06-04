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
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using DataEntity;

using System.Text.RegularExpressions;
using QRCoder;
using System.IO;
using System.ServiceModel;
using System.Drawing;
using System.Drawing.Imaging;
using WebServiceAPI;
using System.Threading.Tasks;

namespace SPPSApi.Controllers.P01
{
  [Route("api/P00001/[action]")]
  [EnableCors("any")]
  [ApiController]
  public class P00001Controller : BaseController
  {

    P00001_DataEntity p00001_DataEntity = new P00001_DataEntity();
    private readonly string FunctionID = "P00001";
    private readonly IWebHostEnvironment _webHostEnvironment;
    ComFunction comFunction = new ComFunction();

    public P00001Controller(IWebHostEnvironment webHostEnvironment)
    {
      _webHostEnvironment = webHostEnvironment;

    }



    #region 登录前验证打印机(PAD,扫描枪)
    //ValidatePrint1Api
    [HttpPost]
    [EnableCors("any")]
    public string ValidatePrint1Api([FromBody] dynamic data)
    {

      string strToken = Request.Headers["X-Token"];
      if (!isLogin(strToken))
      {
        return error_login();
      }
      LoginInfo loginInfo = getLoginByToken(strToken);
      string opearteId = loginInfo.UserId;
      ApiResult apiResult = new ApiResult();


      try
      {
        string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");
        DataTable getPrint = P00001_Logic.GetPrint1(iP);
        if (getPrint.Rows.Count != 2)
        {
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "当前机器没有正确绑定打印机,请联系管理员!";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

        }



      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "验证打印机失败!";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }

      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
    }







    #endregion



    #region 登录前验证打印机(PAD)
    //ValidatePrintApi
    [HttpPost]
    [EnableCors("any")]
    public string ValidatePrintApi([FromBody] dynamic data)
    {

      string strToken = Request.Headers["X-Token"];
      if (!isLogin(strToken))
      {
        return error_login();
      }
      LoginInfo loginInfo = getLoginByToken(strToken);
      string opearteId = loginInfo.UserId;
      ApiResult apiResult = new ApiResult();


      try
      {
        string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");
        DataTable getPrint = P00001_Logic.GetPrint(iP);
        if (getPrint.Rows.Count != 3)
        {
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "当前机器没有正确绑定打印机,请联系管理员!";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

        }



      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "验证打印机失败!";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }

      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
    }







    #endregion







    #region 获取权限

    [HttpPost]
    [EnableCors("any")]
    public string GetRoleInfoApi([FromBody] dynamic data)
    {

      string strToken = Request.Headers["X-Token"];
      if (!isLogin(strToken))
      {
        return error_login();
      }
      LoginInfo loginInfo = getLoginByToken(strToken);
      string opearteId = loginInfo.UserId;
      ApiResult apiResult = new ApiResult();


      try
      {
        DataTable getUserRole = P00001_Logic.GetUserRole(opearteId);
        if (getUserRole.Rows.Count == 1)
        {
          p00001_DataEntity.input = getUserRole.Rows[0][0].ToString();
          p00001_DataEntity.check = getUserRole.Rows[0][1].ToString();
          p00001_DataEntity.pack = getUserRole.Rows[0][2].ToString();
          p00001_DataEntity.output = getUserRole.Rows[0][3].ToString();
          apiResult.data = p00001_DataEntity;



        }
        else
        {
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "用户在权限表中不存在有效数据!";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


        }


      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "获取用户权限失败!";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }

      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
    }






    #endregion


    #region  根据IP获取看板总数

    [HttpPost]
    [EnableCors("any")]
    public string GetKanBanSumApi([FromBody] dynamic data)
    {

      string strToken = Request.Headers["X-Token"];
      if (!isLogin(strToken))
      {
        return error_login();
      }
      LoginInfo loginInfo = getLoginByToken(strToken);
      string opearteId = loginInfo.UserId;
      ApiResult apiResult = new ApiResult();


      try
      {

        dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
        string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");
        DataTable getKanBanSum = P00001_Logic.GetKanBanSum(iP);
        p00001_DataEntity.kanbanSum = int.Parse(getKanBanSum.Rows[0][0].ToString());
        apiResult.data = p00001_DataEntity;




      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "获取看板数量失败!";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }

      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
    }







    #endregion



    #region 生成台车顺番号
    //GenLotId2Api

    //GetTrolletInfoApi
    [HttpPost]
    [EnableCors("any")]
    public string GenLotId2Api([FromBody] dynamic data)
    {

      string strToken = Request.Headers["X-Token"];
      if (!isLogin(strToken))
      {
        return error_login();
      }
      LoginInfo loginInfo = getLoginByToken(strToken);
      string opearteId = loginInfo.UserId;
      ApiResult apiResult = new ApiResult();


      try
      {

        dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
        string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");
        string trolley = dataForm.Trolley == null ? "" : dataForm.Trolley;//台车号
                                                                          //将台车号和台车顺番记录到数据库中
        string tmpString = "TROH2";
        string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
        string formatServerTime = serverTime.Substring(0, 10).Replace("-", "");//格式化号口时间
        DataTable getTroSeq = P00001_Logic.GetSeqNo(tmpString, formatServerTime);
        if (getTroSeq.Rows.Count == 0)
        {
          int seqResultIn = P00001_Logic.InsertSeqNo(tmpString, formatServerTime);
        }
        getTroSeq = P00001_Logic.GetSeqNo(tmpString, formatServerTime);
        string seqNo = getTroSeq.Rows[0][0].ToString().PadLeft(6, '0');//台车连番

        int seqNoNew = int.Parse(seqNo) + 1;
        int seqResultUp = P00001_Logic.UpdateSeqNo(tmpString, formatServerTime, seqNoNew);
        int troResultIn1 = P00001_Logic.InsertTrolley1(seqNo, trolley, iP, opearteId, serverTime);
        p00001_DataEntity.trolleySeqNo = seqNo;
        apiResult.data = p00001_DataEntity;









      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "生成台车顺番号失败!";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }

      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
    }
















    #endregion













    #region 进入画面之后寻找最后一条未入库数据,如果有赋值

    //GetTrolletInfoApi
    [HttpPost]
    [EnableCors("any")]
    public string GetTrolletInfoApi([FromBody] dynamic data)
    {

      string strToken = Request.Headers["X-Token"];
      if (!isLogin(strToken))
      {
        return error_login();
      }
      LoginInfo loginInfo = getLoginByToken(strToken);
      string opearteId = loginInfo.UserId;
      ApiResult apiResult = new ApiResult();


      try
      {

        dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
        string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");
        //先找最近扫描的台车,如果被删除,都不显示,如果台车里面的看板被删除,保留台车号
        DataTable getTrolley = P00001_Logic.GetTrolley(iP);
        if (getTrolley.Rows.Count > 0)
        {
          string staus = getTrolley.Rows[0][1].ToString();
          string trolley = getTrolley.Rows[0][0].ToString();
          string trolleySeqNo = getTrolley.Rows[0][2].ToString();
          if (staus == "0")
          {
            DataTable getKanBan = P00001_Logic.GetKanBan(iP, trolley, trolleySeqNo);
            if (getKanBan.Rows.Count > 0)
            {
              string kanbanStatus = getKanBan.Rows[0][0].ToString();
              DataTable getInfoData = P00001_Logic.GetInfoData(iP);
              if (getInfoData.Rows.Count == 1)
              {
                if (kanbanStatus == "0")
                {




                  p00001_DataEntity.partId = getInfoData.Rows[0][0].ToString();
                  p00001_DataEntity.kanbanOrderNo = getInfoData.Rows[0][1].ToString();
                  p00001_DataEntity.kanbanSerial = getInfoData.Rows[0][2].ToString();
                  p00001_DataEntity.quantity = getInfoData.Rows[0][3].ToString();
                  p00001_DataEntity.dock = getInfoData.Rows[0][4].ToString();
                  p00001_DataEntity.trolley1 = getInfoData.Rows[0][5].ToString();
                  p00001_DataEntity.trolleySeqNo = getInfoData.Rows[0][6].ToString();

                  apiResult.data = p00001_DataEntity;



                }
                else if (kanbanStatus == "4")
                {

                  p00001_DataEntity.trolley1 = getInfoData.Rows[0][5].ToString();
                  p00001_DataEntity.trolleySeqNo = getInfoData.Rows[0][6].ToString();

                  apiResult.data = p00001_DataEntity;




                }

              }


            }







          }



        }












      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "获取看板明细失败!";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }

      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
    }















    #endregion
















    #region 验证账号是否已经登录
    //ValidateLoginApi
    [HttpPost]
    [EnableCors("any")]
    public string ValidateLoginApi([FromBody] dynamic data)
    {

      string strToken = Request.Headers["X-Token"];
      if (!isLogin(strToken))
      {
        return error_login();
      }
      LoginInfo loginInfo = getLoginByToken(strToken);
      string opearteId = loginInfo.UserId;
      ApiResult apiResult = new ApiResult();


      try
      {
        DataTable validateUser = P00001_Logic.ValidateUser(opearteId);
        //ip插入位置
        string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");
        //DataTable getPointDetails = P00001_Logic.GetPointDetails(iP);
        if (validateUser.Rows.Count == 1)
        {

          string pointNo = validateUser.Rows[0][0].ToString();
          //string pointName = getPointDetails.Rows[0][0].ToString();
          DataTable getPointType = P00001_Logic.GetPointType(pointNo);
          if (getPointType.Rows.Count == 1)
          {
            string pointType = getPointType.Rows[0][0].ToString();
            apiResult.code = ComConstant.ERROR_CODE;
            apiResult.data = "账号已经在" + pointType + pointNo + "登录";
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

          }



        }
        else if (validateUser.Rows.Count > 0)
        {

          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "账号信息异常,请联系管理员";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


        }







      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "验证账号失败!";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }

      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
    }








    #endregion






    #region 获取包装单位
    //ValidateQuantity1Api
    [HttpPost]
    [EnableCors("any")]
    public string ValidateQuantity1Api([FromBody] dynamic data)
    {

      string strToken = Request.Headers["X-Token"];
      if (!isLogin(strToken))
      {
        return error_login();
      }
      LoginInfo loginInfo = getLoginByToken(strToken);
      string opearteId = loginInfo.UserId;
      ApiResult apiResult = new ApiResult();


      try
      {
        dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
        string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");
        string partId = dataForm.PartId == null ? "" : dataForm.PartId;//品番
        string dock = dataForm.Dock == null ? "" : dataForm.Dock;//受入
        //ScanTime
        string scanTime = dataForm.ScanTime == null ? "" : dataForm.ScanTime;//客户端时间
        DataTable getQuantity = P00001_Logic.GetQuantity(partId, scanTime, dock);
        if (getQuantity.Rows.Count == 1)
        {
          p00001_DataEntity.packingQuantity = getQuantity.Rows[0][0].ToString();
          apiResult.data = p00001_DataEntity;

        }
        else
        {

          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "品番" + partId + "在包装基础数据中没有有效数据!";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

        }







      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "获取包装单位失败";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }

      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
    }








    #endregion






    #region 退出登录,将状态设置为未登录,更新履历表中的退出时间,更新用户箱号状态
    //UpdateStatus3Api
    [HttpPost]
    [EnableCors("any")]
    public string UpdateStatus3Api([FromBody] dynamic data)
    {

      string strToken = Request.Headers["X-Token"];
      if (!isLogin(strToken))
      {
        return error_login();
      }
      LoginInfo loginInfo = getLoginByToken(strToken);
      string opearteId = loginInfo.UserId;
      ApiResult apiResult = new ApiResult();


      try
      {
        dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
        string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");
        DataTable getPoint = P00001_Logic.GetPoint2(iP);


        if (getPoint.Rows.Count == 1)
        {
          string pointNo = getPoint.Rows[0][0].ToString();
          DataTable getDetail = P00001_Logic.GetDetail(pointNo);


          DataTable getStatus = P00001_Logic.GetPointStatus4(pointNo);
          if (getStatus.Rows.Count == 1 && getDetail.Rows.Count > 0)
          {
            string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
            string uuid = getDetail.Rows[0][0].ToString();
            int poiResultUp = P00001_Logic.UpdateStatus5(pointNo);
            int detResultUp = P00001_Logic.UpdateDetail(uuid, serverTime);
            int caseResultUp = P00001_Logic.UpdateCase(iP);

          }
          else
          {

            apiResult.code = ComConstant.ERROR_CODE;
            apiResult.data = "点位信息异常,请检查!";
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

          }





        }
        else
        {

          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "当前点位表中不存在有效信息,请检查!";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


        }

      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "更改状态失败";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }

      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
    }










    #endregion








    #region 登录到主页,将状态设置为已登录,插入状态到履历表中,将之前未做出货的箱号重新绑定，并显示最新的箱号
    //UpdateStatus2Api
    [HttpPost]
    [EnableCors("any")]
    public string UpdateStatus2Api([FromBody] dynamic data)
    {


      string strToken = Request.Headers["X-Token"];
      if (!isLogin(strToken))
      {
        return error_login();
      }
      LoginInfo loginInfo = getLoginByToken(strToken);
      string opearteId = loginInfo.UserId;
      ApiResult apiResult = new ApiResult();

      try
      {
        dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
        string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");
        string uuid = System.Guid.NewGuid().ToString("N");//生成UUID
        string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间

        DataTable getBanZhi = P00001_Logic.GetBanZhi(serverTime);
        DataTable getPoint = P00001_Logic.GetPoint2(iP);

        if (getPoint.Rows.Count == 1 && getBanZhi.Rows.Count == 1)
        {
          string pointNo = getPoint.Rows[0][0].ToString();
          string date = getBanZhi.Rows[0][0].ToString();
          string banZhi = getBanZhi.Rows[0][1].ToString();
          int detResultIn = P00001_Logic.InsertDetail(date, banZhi, pointNo, uuid, serverTime, opearteId);



          DataTable getStatus = P00001_Logic.GetPointStatus4(pointNo);
          if (getStatus.Rows.Count == 1)
          {

            int poiResultUp = P00001_Logic.UpdateStatus4(pointNo, opearteId);


            #region 将当前绑定，未打印装箱单的箱号重新绑定
            DataTable getCase = P00001_Logic.GetCase(opearteId);
            if (getCase.Rows.Count>0) {
              for (int i=0;i<getCase.Rows.Count;i++) {
                string caseNo = getCase.Rows[i][0].ToString();
                DataTable getCase1 = P00001_Logic.GetCase1(caseNo);
                if (getCase1.Rows.Count==0)//未打印装箱单
                {
                  int caseResultUp = P00001_Logic.UpdateCase(iP,serverTime,opearteId,caseNo);





                }



              }








            }






            #endregion 





          }
          else
          {

            apiResult.code = ComConstant.ERROR_CODE;
            apiResult.data = "点位信息异常,请检查!";
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

          }





        }
        else
        {

          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "当前点位表中不存在有效信息,请检查!";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


        }

      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "更改状态失败";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }

      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
    }


















    #endregion










    #region 退出应用需要更改状态为未用
    //UpdateStatus1Api
    //ReLoginApi
    [HttpPost]
    [EnableCors("any")]
    public string UpdateStatus1Api([FromBody] dynamic data)
    {



      ApiResult apiResult = new ApiResult();


      try
      {
        dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
        string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");
        DataTable getPoint = P00001_Logic.GetPoint2(iP);

        if (getPoint.Rows.Count == 1)
        {
          string pointNo = getPoint.Rows[0][0].ToString();
          DataTable getStatus = P00001_Logic.GetPointStatus4(pointNo);
          if (getStatus.Rows.Count == 1)
          {

            int poiResultUp = P00001_Logic.UpdateStatus3(pointNo);



          }
          else
          {

            apiResult.code = ComConstant.ERROR_CODE;
            apiResult.data = "点位信息异常,请检查!";
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

          }





        }
        else
        {

          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "当前点位表中不存在有效信息,请检查!";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


        }

      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "更改状态失败";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }

      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
    }


















    #endregion







    /*
    #region 重新登录需要更改状态为未登录
    //ReLoginApi
    [HttpPost]
    [EnableCors("any")]
    public string ReLoginApi([FromBody] dynamic data)
    {
      string strToken = Request.Headers["X-Token"];
      if (!isLogin(strToken))
      {
        return error_login();
      }
      LoginInfo loginInfo = getLoginByToken(strToken);
      string opearteId = loginInfo.UserId;
      ApiResult apiResult = new ApiResult();


      try
      {
        dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
        string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");
        DataTable getStatus = P00001_Logic.GetPointStatus2(opearteId,iP);
        if (getStatus.Rows.Count == 1)
        {
          string pointNo = getStatus.Rows[0][0].ToString();
          int PointResulUp = P00001_Logic.UpdateStatus1(pointNo,opearteId);



        }
        else {

          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "点位信息异常!";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

        }


      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "更改状态失败";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }

      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
    }

















    #endregion 

    */












    #region 登录后更新状态为正常

    [HttpPost]
    [EnableCors("any")]
    public string UpdateStatusApi([FromBody] dynamic data)
    {
      string strToken = Request.Headers["X-Token"];
      if (!isLogin(strToken))
      {
        return error_login();
      }
      LoginInfo loginInfo = getLoginByToken(strToken);
      string opearteId = loginInfo.UserId;
      ApiResult apiResult = new ApiResult();


      try
      {
        dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));

        string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");
        DataTable getPointStatus = P00001_Logic.GetPointStatus1(opearteId, iP);
        if (getPointStatus.Rows.Count == 1)
        {
          string pointNo = getPointStatus.Rows[0][0].ToString();
          int pointResultUp = P00001_Logic.UpdateStatus(opearteId, iP, pointNo);



        }
        else
        {
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "点位信息异常!";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

        }



      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "更改状态失败";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }

      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
    }

















    #endregion













    #region  进入系统之后更改点位表状态为未登录

    [HttpPost]
    [EnableCors("any")]
    public string ValidateUserApi1([FromBody] dynamic data)
    {

      ApiResult apiResult = new ApiResult();
      try
      {
        dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));

        string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
                                                                                                     //获取当前IP的点位号  获取当前IP的登录信息
        DataTable getPoint = P00001_Logic.GetPoint2(iP);
        if (getPoint.Rows.Count == 1)
        {
          string pointNo = getPoint.Rows[0][0].ToString();
          DataTable getStatus = P00001_Logic.GetPointStatus4(pointNo);
          if (getStatus.Rows.Count == 0)
          {
            int poiResultIn = P00001_Logic.InsertPoint1(pointNo);

          }
          else if (getStatus.Rows.Count == 1)
          {
            int poiResultUp = P00001_Logic.UpdatePoint1(pointNo);

          }
          else
          {

            apiResult.code = ComConstant.ERROR_CODE;
            apiResult.data = "点位信息异常,请检查!";
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

          }



        }
        else
        {
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "当前点位表中不存在有效信息,请检查!";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

        }







      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "更改状态失败";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }

      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
    }




















    #endregion






    //GetTrolleyApi
    #region 获取台车号
    [HttpPost]
    [EnableCors("any")]
    public string GetTrolleyApi([FromBody] dynamic data)
    {
      string strToken = Request.Headers["X-Token"];
      if (!isLogin(strToken))
      {
        return error_login();
      }
      LoginInfo loginInfo = getLoginByToken(strToken);
      string opearteId = loginInfo.UserId;
      ApiResult apiResult = new ApiResult();
      try
      {
        dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));

        string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
        DataTable getTrolley = P00001_Logic.GetTrolley(opearteId, iP);
        if (getTrolley.Rows.Count > 0)
        {
          string trolley = getTrolley.Rows[0][0].ToString();
          DataTable validateQB = P00001_Logic.ValidateQB(trolley);
          if (validateQB.Rows.Count > 0)
          {
            p00001_DataEntity.trolley1 = getTrolley.Rows[0][0].ToString();
            p00001_DataEntity.lotId = getTrolley.Rows[0][1].ToString();
            apiResult.data = p00001_DataEntity;

          }


          // p00001_DataEntity.trolley1 = getTrolley.Rows[0][0].ToString();


        }
        else
        {
          p00001_DataEntity.delRsult = "clear";
          apiResult.data = p00001_DataEntity;

        }
      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "获取台车号失败";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }

      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
    }









    #endregion




    #region  删除看板
    [HttpPost]
    [EnableCors("any")]
    public string DelKanbanApi([FromBody] dynamic data)
    {
      string strToken = Request.Headers["X-Token"];
      if (!isLogin(strToken))
      {
        return error_login();
      }
      LoginInfo loginInfo = getLoginByToken(strToken);
      string opearteId = loginInfo.UserId;
      ApiResult apiResult = new ApiResult();
      try
      {
        dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
        string partId = dataForm.PartId == null ? "" : dataForm.PartId;//品番

        string dock = dataForm.Dock == null ? "" : dataForm.Dock;//受入
        string kanbanOrderNo = dataForm.KanbanOrderNo == null ? "" : dataForm.KanbanOrderNo;//看板订单号
        string kanbanSerial = dataForm.KanbanSerial == null ? "" : dataForm.KanbanSerial;//看板连番
        string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
        int OprResultDel = P00001_Logic.DeleteKanban(partId, kanbanOrderNo, kanbanSerial, dock, iP);
        DataTable getSeqNo = P00001_Logic.GetSeqNo1(iP, kanbanOrderNo, kanbanSerial, dock, partId);
        string trolleySeqNo1 = getSeqNo.Rows[0][0].ToString();
        string trolley1 = getSeqNo.Rows[0][1].ToString();

        DataTable getData = P00001_Logic.GetData(trolley1, trolleySeqNo1, iP);
        DataTable getData1 = P00001_Logic.GetData1(trolley1, trolleySeqNo1, iP);
        if (getData.Rows[0][0].ToString() == "4" && int.Parse(getData1.Rows[0][0].ToString()) > 0)
        {

          //if()
          DataTable getInfoData = P00001_Logic.GetInfoData(iP);
          p00001_DataEntity.trolley1 = getInfoData.Rows[0][5].ToString();
          p00001_DataEntity.trolleySeqNo = getInfoData.Rows[0][6].ToString();
          apiResult.data = p00001_DataEntity;
        }
        else if (getData.Rows[0][0].ToString() == "4" && int.Parse(getData1.Rows[0][0].ToString()) == 0)
        {
          DataTable getSeqNo1 = P00001_Logic.GetSeqNo2(iP, kanbanOrderNo, kanbanSerial, dock, partId);
          string trolleySeqNo = getSeqNo1.Rows[0][0].ToString();
          string trolley = getSeqNo1.Rows[0][1].ToString();
          int trolleyResultUp = P00001_Logic.UpdateTrolley3(trolley, trolleySeqNo, iP);
        }
        else if (getData.Rows[0][0].ToString() == "0")
        {
          DataTable getInfoData = P00001_Logic.GetInfoData(iP);
          p00001_DataEntity.partId = getInfoData.Rows[0][0].ToString();
          p00001_DataEntity.kanbanOrderNo = getInfoData.Rows[0][1].ToString();
          p00001_DataEntity.kanbanSerial = getInfoData.Rows[0][2].ToString();
          p00001_DataEntity.quantity = getInfoData.Rows[0][3].ToString();
          p00001_DataEntity.dock = getInfoData.Rows[0][4].ToString();
          p00001_DataEntity.trolley1 = getInfoData.Rows[0][5].ToString();
          p00001_DataEntity.trolleySeqNo = getInfoData.Rows[0][6].ToString();
          apiResult.data = p00001_DataEntity;
        }
      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "删除看板失败";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }

      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
    }







    #endregion


    #region 获得看板明细
    [HttpPost]
    [EnableCors("any")]
    public string GetTrolleyInfoApi([FromBody] dynamic data)
    {
      string strToken = Request.Headers["X-Token"];
      if (!isLogin(strToken))
      {
        return error_login();
      }
      LoginInfo loginInfo = getLoginByToken(strToken);
      string opearteId = loginInfo.UserId;
      ApiResult apiResult = new ApiResult();
      try
      {
        dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
        string trolley = dataForm.Trolley == null ? "" : dataForm.Trolley;//台车号
        string lotId = dataForm.LotId == null ? "" : dataForm.LotId;//段取指示号
        string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址

        DataTable getTrolleyInfo = P00001_Logic.GetTrolleyInfo(trolley, iP, lotId);
        if (getTrolleyInfo.Rows.Count > 0)
        {
          p00001_DataEntity.kanbanSum1 = getTrolleyInfo.Rows.Count.ToString();
          p00001_DataEntity.kanban = getTrolleyInfo;
          apiResult.data = p00001_DataEntity;


        }
        else
        {
          p00001_DataEntity.kanban = null;
          p00001_DataEntity.kanbanSum1 = "0";
          apiResult.data = p00001_DataEntity;

        }


      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "", ex, "system");
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "删除台车失败";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }

      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
    }



    #endregion

    /*
    #region   数据上传
    [HttpPost]
    [EnableCors("any")]
    public string SendInputDataApi([FromBody] dynamic data)
    {
      string strToken = Request.Headers["X-Token"];
      if (!isLogin(strToken))
      {
        return error_login();
      }
      LoginInfo loginInfo = getLoginByToken(strToken);
      string opearteId = loginInfo.UserId;
      ApiResult apiResult = new ApiResult();
      try
      {
        string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
        DataTable getQBData = P00001_Logic.GetQBData(iP);//从实绩情报表获得数据
        string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
        if (getQBData.Rows.Count > 0)
        {
          for (int i = 0; i < getQBData.Rows.Count; i++)
          {



    select vcTrolleyNo,vcPart_id,vcCpdCompany,vcSR,vcKBOrderNo,vcKBLFNo,vcBZPlant,iPackingQty,vcLabelStart,
    vcLabelEnd,vcSupplierId,vcSupplierPlant,
    vcLotid,vcCheckType,vcIOType from TOperatorQB where vcInputNo='210313200004' and vcZYType='S0' and vcReflectFlag='1'


            string trolleyNo = getQBData.Rows[i][0].ToString(); //台车号
          
            string partId = getQBData.Rows[i][1].ToString();//品番
            string cpdCompany = getQBData.Rows[i][2].ToString();//收货方
            string dock = getQBData.Rows[i][3].ToString();//受入

            string kanbanOrderNo = getQBData.Rows[i][4].ToString();//看板订单号
            string kanbanSerial = getQBData.Rows[i][5].ToString();//看板连番
         
            string packingSpot = getQBData.Rows[i][6].ToString();//包装场
    string  packingQuantity=getQBData.Rows[i][7].ToString();//包装单位
     string lblStart = getQBData.Rows[i][8].ToString();//标签开始
            string lblEnd = getQBData.Rows[i][9].ToString();//标签结束
     string supplierId = getQBData.Rows[i][10].ToString();//供应商代码
            string supplierPlant = getQBData.Rows[i][11].ToString();//供应商工区
    string lotId = getQBData.Rows[i][12].ToString();//段取指示号
      string checkType = getQBData.Rows[i][13].ToString();//检查区分
            string inoutFlag = getQBData.Rows[i][14].ToString();//内外区分



            string lblStart = getQBData.Rows[i][12].ToString();//标签开始
            string lblEnd = getQBData.Rows[i][13].ToString();//标签结束
            int packingQuantity = int.Parse(getQBData.Rows[i][14].ToString());//包装单位
            


            DataTable validateSJ = P00001_Logic.ValidateSJ(partId, kanbanOrderNo, kanbanSerial, inputNo);//验证入出库履历
            DataTable validateOpr = P00001_Logic.ValidateOpr(partId, kanbanOrderNo, kanbanSerial, inputNo);//验证作业实绩
            DataTable getCheckType = P00001_Logic.GetCheckType(partId, scanTime);//获得检查区分
            DataTable getInOutFlag = P00001_Logic.GetInOutFlag(partId, scanTime);//获得内外区分
            DataTable validateOrd = P00001_Logic.ValiateOrd1(partId);//验证订单表
            DataTable getPackItem = P00001_Logic.GetPackItem(scanTime, partId);//获得包材明细



            if (getCheckType.Rows.Count == 1 && getInOutFlag.Rows.Count == 1 && validateSJ.Rows.Count == 0 && validateOpr.Rows.Count == 0 && validateOrd.Rows.Count > 0 && getPackItem.Rows.Count > 0)
            {
              string inOutFlag = getInOutFlag.Rows[0][0].ToString();//内外区分
              string partsNameEn = getInOutFlag.Rows[0][1].ToString();//英文品名
              string carFamilyCode = getInOutFlag.Rows[0][2].ToString();//车种代码
              string supplierName = getInOutFlag.Rows[0][3].ToString();//供应商名称
              string supplierAddress = getInOutFlag.Rows[0][4].ToString();//供应商地址
              string checkType = getCheckType.Rows[0][0].ToString();//检查区分
              int oprReusultIn = P00001_Logic.InsertOpr(packingSpot, inputNo, kanbanOrderNo, kanbanSerial, partId, inOutFlag, supplierId, supplierPlant, scanTime, serverTime, quantity, packingQuantity, cpdCompany, dock, checkType, lblStart, lblEnd, opearteId);//插入作业实际表
              int invResultIn = P00001_Logic.InsertInv(packingSpot, inputNo, partId, cpdCompany, quantity, serverTime, kanbanOrderNo, kanbanSerial, scanTime, opearteId);//插入入出库履历表
              int oprResultUp = P00001_Logic.UpdateOpr(partId, dock, kanbanOrderNo, kanbanSerial);



              #region  插入段取指示表,打印段取指示书
              for (int j = 0; j < getPackItem.Rows.Count; j++)
              {
                string packNo = getPackItem.Rows[j][0].ToString();//包材品番
                string nessNo = getPackItem.Rows[j][1].ToString();//必要数
                DataTable getPackBase = P00001_Logic.GetPackBase(scanTime, packNo, packingSpot);
                if (getPackBase.Rows.Count == 1)
                {
                  string packLocation = getPackBase.Rows[0][0].ToString();//包材位置
                  string distinguish = getPackBase.Rows[0][1].ToString();//包材区分
                  int qty = int.Parse(nessNo) * int.Parse(quantity);
                  DataTable validatePack = P00001_Logic.ValidatePack(lotId, iP, opearteId, packNo);
                  if (validatePack.Rows.Count == 0)
                  {
                    int packResultIn = P00001_Logic.InsertPack(lotId, packNo, distinguish, inputNo, qty, packLocation, scanTime, serverTime, opearteId, iP, trolleyNo);
                  }
                  else if (validatePack.Rows.Count == 1)
                  {
                    int packResultUp = P00001_Logic.UpdatePack(lotId, packNo, iP, opearteId, qty);
                  }
                }
                else
                {
                  P00001_Logic.UpdateOpr1(kanbanOrderNo, kanbanSerial, partId, dock);
                  apiResult.code = ComConstant.ERROR_CODE;
                  apiResult.data = "包材品番" + packNo + "在包材基础数据中没有有效数据,请检查!";
                  return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

              }

              #endregion











              #region  打印标签
              string date = lblStart.Substring(0, 6);//标签首

              int lblfront = int.Parse(lblStart.Substring(6, 5));//开始
              int lblback = int.Parse(lblEnd.Substring(6, 5));//结束
              for (int a = lblfront; a <= lblback; a++)
              {//此时的连番号就是a

                string printCount = date + a.ToString().PadLeft(5, '0');

                int lblResultIn = P00001_Logic.InsertLbl(partsNameEn, partId, cpdCompany, quantity, printCount, supplierName, supplierAddress, carFamilyCode, opearteId, scanTime, iP, qr.GetQrCodeByteArray(printCount));
              }
              #endregion


              #region  更新订单表
              for (int j = 0; j < validateOrd.Rows.Count; j++)
              {
                string targetMonth = validateOrd.Rows[j][0].ToString();
                string orderType = validateOrd.Rows[j][1].ToString();
                string orderNo = validateOrd.Rows[j][2].ToString();
                string seqNo = validateOrd.Rows[j][3].ToString();
                int d1 = int.Parse(validateOrd.Rows[j][4].ToString());
                int d2 = int.Parse(validateOrd.Rows[j][5].ToString());
                int d3 = int.Parse(validateOrd.Rows[j][6].ToString());
                int d4 = int.Parse(validateOrd.Rows[j][7].ToString());
                int d5 = int.Parse(validateOrd.Rows[j][8].ToString());
                int d6 = int.Parse(validateOrd.Rows[j][9].ToString());
                int d7 = int.Parse(validateOrd.Rows[j][10].ToString());
                int d8 = int.Parse(validateOrd.Rows[j][11].ToString());
                int d9 = int.Parse(validateOrd.Rows[j][12].ToString());
                int d10 = int.Parse(validateOrd.Rows[j][13].ToString());
                int d11 = int.Parse(validateOrd.Rows[j][14].ToString());
                int d12 = int.Parse(validateOrd.Rows[j][15].ToString());
                int d13 = int.Parse(validateOrd.Rows[j][16].ToString());
                int d14 = int.Parse(validateOrd.Rows[j][17].ToString());
                int d15 = int.Parse(validateOrd.Rows[j][18].ToString());
                int d16 = int.Parse(validateOrd.Rows[j][19].ToString());
                int d17 = int.Parse(validateOrd.Rows[j][20].ToString());
                int d18 = int.Parse(validateOrd.Rows[j][21].ToString());
                int d19 = int.Parse(validateOrd.Rows[j][22].ToString());
                int d20 = int.Parse(validateOrd.Rows[j][23].ToString());
                int d21 = int.Parse(validateOrd.Rows[j][24].ToString());
                int d22 = int.Parse(validateOrd.Rows[j][25].ToString());
                int d23 = int.Parse(validateOrd.Rows[j][26].ToString());
                int d24 = int.Parse(validateOrd.Rows[j][27].ToString());
                int d25 = int.Parse(validateOrd.Rows[j][28].ToString());
                int d26 = int.Parse(validateOrd.Rows[j][29].ToString());
                int d27 = int.Parse(validateOrd.Rows[j][30].ToString());
                int d28 = int.Parse(validateOrd.Rows[j][31].ToString());
                int d29 = int.Parse(validateOrd.Rows[j][32].ToString());
                int d30 = int.Parse(validateOrd.Rows[j][33].ToString());
                int d31 = int.Parse(validateOrd.Rows[j][34].ToString());
                int[] array = {d1, d2, d3, d4, d5, d6, d7, d8, d9, d10, d11, d12, d13, d14, d15, d16, d17, d18, d19, d20, d21,
              d22,d23,d24,d25,d26,d27,d28,d29,d30,d31 };
                int[] newarray = new int[31];
                for (int l = 0; l < array.Length; l++)
                {
                  if (SumQuantity - array[l] > 0)
                  {
                    newarray[l] = array[l];
                    SumQuantity = SumQuantity - array[l];

                  }
                  else
                  {
                    newarray[l] = SumQuantity;
                    SumQuantity = 0;

                  }
                }
                for (int k = 0; k < newarray.Length; k++)
                {
                  newSum += newarray[k];
                }
                int updateOrd = P00001_Logic.UpdateOrd(targetMonth, orderNo, seqNo, newarray[0], newarray[1], newarray[2], newarray[3], newarray[4], newarray[5], newarray[6], newarray[7], newarray[8], newarray[9], newarray[10],
                  newarray[11], newarray[12], newarray[13], newarray[14], newarray[15], newarray[16], newarray[17], newarray[18], newarray[19], newarray[20], newarray[21], newarray[22], newarray[23],
                  newarray[24], newarray[25], newarray[26], newarray[27], newarray[28], newarray[29], newarray[30], newSum, partId);
                newSum = 0;
              }

              #endregion

            }
            else if (getCheckType.Rows.Count != 1)
            {

              P00001_Logic.UpdateOpr1(kanbanOrderNo, kanbanSerial, partId, dock);

              apiResult.code = ComConstant.ERROR_CODE;
              apiResult.data = "品番" + partId + "在检查区分表中没有有效数据,请检查!";
              return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            else if (getPackItem.Rows.Count == 0)
            {
              P00001_Logic.UpdateOpr1(kanbanOrderNo, kanbanSerial, partId, dock);
              apiResult.code = ComConstant.ERROR_CODE;
              apiResult.data = "品番" + partId + "在包材构成数据中没有有效数据,请检查!";
              return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            else if (getInOutFlag.Rows.Count != 1)
            {
              P00001_Logic.UpdateOpr1(kanbanOrderNo, kanbanSerial, partId, dock);
              apiResult.code = ComConstant.ERROR_CODE;
              apiResult.data = "品番" + partId + "在基础数据中没有有效数据,请检查!";
              return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            else if (validateOpr.Rows.Count > 0 || validateSJ.Rows.Count > 0)
            {
              P00001_Logic.UpdateOpr1(kanbanOrderNo, kanbanSerial, partId, dock);
              apiResult.code = ComConstant.ERROR_CODE;
              apiResult.data = "品番" + partId + "已经入荷,请检查!";
              return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }
            else if (validateOrd.Rows.Count == 0)
            {
              P00001_Logic.UpdateOpr1(kanbanOrderNo, kanbanSerial, partId, dock);
              apiResult.code = ComConstant.ERROR_CODE;
              apiResult.data = "品番" + partId + "无有效订单,请检查!";
              return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


            }









          }//循环结束



          #region  循环打印段取指示书
          DataTable getPack = P00001_Logic.GetPackBase(iP);

          string lotHead = getPack.Rows[0][0].ToString();
          string lotEnd = getPack.Rows[getPack.Rows.Count - 1][0].ToString();
          string lotFront = lotHead.Substring(0, 9);
          int front = int.Parse(lotHead.Substring(9, 5));
          int end = int.Parse(lotEnd.Substring(9, 5));
          for (int j = front; j <= end; j++)
          {
            string lotIdNew = lotFront + j.ToString().PadLeft(5, '0');
            int tpResultIn = P00001_Logic.InsertTP(iP, opearteId, serverTime, lotIdNew);








          }




          #endregion

          int lbResultIn = P00001_Logic.InsertTP1(iP, opearteId, serverTime);





        }
        else
        {

          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "当前没有需要更新的数据";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

        }


      }
      catch (Exception ex)
      {
        //ComMessage.GetInstance().ProcessMessage(FunctionID, "", ex, "system");
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "入库失败";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }

      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
    }







    #endregion

    */
    #region 生成台车顺番号









    #endregion 










    #region 生成段取指示号
    [HttpPost]
    [EnableCors("any")]
    public string GenLotIdApi([FromBody] dynamic data)
    {
      string strToken = Request.Headers["X-Token"];
      if (!isLogin(strToken))
      {
        return error_login();
      }
      LoginInfo loginInfo = getLoginByToken(strToken);
      string opearteId = loginInfo.UserId;

      ApiResult apiResult = new ApiResult();
      try
      {
        dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
        string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
                                                                                                     //Trolley
        string trolley = dataForm.Trolley == null ? "" : dataForm.Trolley;//品番



        #region 在进行之前更新当前用户名的台车号状态
        //int trolleyResultUp1 = P00001_Logic.UpdateTrolley1(iP,opearteId);

        #endregion
        string tmpString = "PAC";
        string packingSpot = "H2";
        string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
        string formatServerTime = serverTime.Substring(0, 10).Replace("-", "");//格式化号口时间
        DataTable pacSeqNodt = P00001_Logic.ValidateSeqNo(packingSpot, formatServerTime, tmpString);//判断连番表里是否有当天的段取指示书连番
        DataTable getTrolley = P00001_Logic.ValidateTrolley(trolley, opearteId, iP);


        if (pacSeqNodt.Rows.Count == 0)
        {
          int inoSeqNoIn = P00001_Logic.InsertSeqNo(packingSpot, formatServerTime, tmpString);//如果当天没有插入信息
          pacSeqNodt = P00001_Logic.ValidateSeqNo(packingSpot, formatServerTime, tmpString);
        }
        string pacSeqNo = pacSeqNodt.Rows[0][2].ToString();//取得包材段取连番号
        string lotId = formatServerTime.Substring(2, 6).Trim() + "-" + packingSpot.Substring(1, 1).Trim() + "-" + pacSeqNo.PadLeft(5, '0').Trim();
        if (getTrolley.Rows.Count == 0)//如果没有进行插入
        {
          int trolleyResultIn = P00001_Logic.InsertTrolley(trolley, opearteId, serverTime, iP, lotId);

        }
        else
        {
          int trolleyResultUp = P00001_Logic.UpdateTrolley(trolley, opearteId, serverTime, iP, lotId);


        }



        int pacSeqNoNew = int.Parse(pacSeqNo) + 1;
        int pacResultUp = P00001_Logic.UpdateSeqNo(packingSpot, formatServerTime, pacSeqNoNew, tmpString);//更新连番表


        p00001_DataEntity.lotId = lotId;
        apiResult.data = p00001_DataEntity;









      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "生成段取指示号失败!";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }

      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
    }



    #endregion

    #region 验证重复台车
    [HttpPost]
    [EnableCors("any")]
    public string GenLotId1Api([FromBody] dynamic data)
    {
      string strToken = Request.Headers["X-Token"];
      if (!isLogin(strToken))
      {
        return error_login();
      }
      LoginInfo loginInfo = getLoginByToken(strToken);
      string opearteId = loginInfo.UserId;

      ApiResult apiResult = new ApiResult();
      try
      {
        dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
        string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
                                                                                                     //Trolley
        string trolley = dataForm.Trolley == null ? "" : dataForm.Trolley;//品番

        DataTable GetTrolleyInfo = P00001_Logic.GetTrolleyInfo1(trolley, iP, opearteId);





      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "验证重复台车失败!";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }

      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
    }



    #endregion









    #region  更新段取指示号
    [HttpPost]
    [EnableCors("any")]
    public string UpdateLotIdApi([FromBody] dynamic data)
    {
      string strToken = Request.Headers["X-Token"];
      if (!isLogin(strToken))
      {
        return error_login();
      }
      LoginInfo loginInfo = getLoginByToken(strToken);
      string opearteId = loginInfo.UserId;

      ApiResult apiResult = new ApiResult();
      try
      {
        dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
        string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
        string trolley = dataForm.Trolley == null ? "" : dataForm.Trolley;//台车号
        DataTable getPackingSpot = P00001_Logic.GetPackingSpot(iP);
        if (getPackingSpot.Rows.Count == 1)
        {

          string packingSpot = getPackingSpot.Rows[0][0].ToString();
          string tmpString = "PAC";

          string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
          string formatServerTime = serverTime.Substring(0, 10).Replace("-", "");//格式化号口时间
          DataTable pacSeqNodt = P00001_Logic.ValidateSeqNo(packingSpot, formatServerTime, tmpString);//判断连番表里是否有当天的段取指示书连番
          if (pacSeqNodt.Rows.Count == 0)
          {
            int inoSeqNoIn = P00001_Logic.InsertSeqNo(packingSpot, formatServerTime, tmpString);//如果当天没有插入信息
            pacSeqNodt = P00001_Logic.ValidateSeqNo(packingSpot, formatServerTime, tmpString);
          }
          string pacSeqNo = pacSeqNodt.Rows[0][2].ToString();//取得包材段取连番号
          string lotId = formatServerTime.Substring(2, 6).Trim() + "-" + packingSpot.Substring(1, 1).Trim() + "-" + pacSeqNo.PadLeft(5, '0').Trim();




          int pacSeqNoNew = int.Parse(pacSeqNo) + 1;
          int pacResultUp = P00001_Logic.UpdateSeqNo(packingSpot, formatServerTime, pacSeqNoNew, tmpString);//更新连番表

          int qbResultUp = P00001_Logic.UpdateQB(lotId, iP, trolley);



        }
        else
        {
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "包装场在实绩情报表不存在或存在多个包装场,请检查!";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

        }





      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "更新段取指示号失败!";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }

      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
    }


    #endregion






    #region  删除台车
    [HttpPost]
    [EnableCors("any")]
    public string DelTrolleyApi([FromBody] dynamic data)
    {
      string strToken = Request.Headers["X-Token"];
      if (!isLogin(strToken))
      {
        return error_login();
      }
      LoginInfo loginInfo = getLoginByToken(strToken);
      string opearteId = loginInfo.UserId;
      ApiResult apiResult = new ApiResult();
      try
      {
        dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
        string trolley = dataForm.Trolley == null ? "" : dataForm.Trolley;//台车号
        string lotId = dataForm.LotId == null ? "" : dataForm.LotId;//台车号

        string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址

        int qbResultDe = P00001_Logic.DeleteTrolley(trolley, iP, lotId);

        int trolleyResultUp = P00001_Logic.UpdateTrolley1(iP, opearteId, trolley, lotId);


      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "删除台车失败";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }

      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
    }


    #endregion








    #region  获得数据
    [HttpPost]
    [EnableCors("any")]
    public string GetDataApi([FromBody] dynamic data)
    {
      string strToken = Request.Headers["X-Token"];
      if (!isLogin(strToken))
      {
        return error_login();
      }
      LoginInfo loginInfo = getLoginByToken(strToken);
      string opearteId = loginInfo.UserId;



      ApiResult apiResult = new ApiResult();
      try
      {
        dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
        string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
                                                                                                     //DataTable getSum = P00001_Logic.GetSum(iP);
        DataTable getSum = P00001_Logic.GetSum(iP);

        if (getSum.Rows.Count > 0)
        {
          int trolleySum = getSum.Rows.Count;
          int kanbanSum = 0;
          for (int i = 0; i < getSum.Rows.Count; i++)
          {
            string kanbanQuantity = getSum.Rows[i][1].ToString();
            kanbanSum += int.Parse(kanbanQuantity);

            p00001_DataEntity.trolleySum = trolleySum;
            p00001_DataEntity.kanbanSum = kanbanSum;
            p00001_DataEntity.trolley = getSum;
            apiResult.data = p00001_DataEntity;

          }
        }
        else
        {
          p00001_DataEntity.trolley = null;
          apiResult.data = p00001_DataEntity;

        }






      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "获取数据失败";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }

      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
    }


    #endregion

    #region  获得数据   
    [HttpPost]
    [EnableCors("any")]
    public string SendInputDataApi([FromBody] dynamic data)
    {
      string strToken = Request.Headers["X-Token"];
      if (!isLogin(strToken))
      {
        return error_login();
      }
      LoginInfo loginInfo = getLoginByToken(strToken);
      string opearteId = loginInfo.UserId;
      ApiResult apiResult = new ApiResult();
      try
      {
        string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
        string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
        string formatServerTime = serverTime.Substring(0, 10).Replace("-", "");//格式化号口时间
        string tmpString = "PACH2";
        string tmpString1 = "LBLH2";
        string tmpString2 = "INOH2";



        #region 在数据上传的时候生成段取指示号,生成入库单号,生成标签ID
        #region 生成段取指示号
        DataTable getLotInfo = P00001_Logic.GetLotInfo(iP);


        if (getLotInfo.Rows.Count > 0)
        {

          DataTable getTroSeq = P00001_Logic.GetSeqNo(tmpString, formatServerTime);
          if (getTroSeq.Rows.Count == 0)
          {
            int seqResultIn = P00001_Logic.InsertSeqNo(tmpString, formatServerTime);
          }
          getTroSeq = P00001_Logic.GetSeqNo(tmpString, formatServerTime);
          string seqNoTrolley = getTroSeq.Rows[0][0].ToString().PadLeft(6, '0');//台车连番

          int seqNoNew = int.Parse(seqNoTrolley) + getLotInfo.Rows.Count;
          int seqResultUp = P00001_Logic.UpdateSeqNo(tmpString, formatServerTime, seqNoNew);




          for (int i = 0; i < getLotInfo.Rows.Count; i++)
          {
            string trolleySeqNo = getLotInfo.Rows[i][0].ToString();
            string lotId = formatServerTime.Substring(2, 6).Trim() + "-" + "2" + "-" + (int.Parse(seqNoTrolley) + i).ToString().PadLeft(5, '0').Trim();
            int troResultUp = P00001_Logic.UpdateTrolley2(trolleySeqNo, lotId, iP);
          }




        }


        #endregion


        #region 更新标签ID
        DataTable getID = P00001_Logic.GetID(iP, serverTime);
        if (getID.Rows.Count > 0)
        {
          DataTable getTroSeq = P00001_Logic.GetSeqNo(tmpString1, formatServerTime);
          if (getTroSeq.Rows.Count == 0)
          {
            int seqResultIn = P00001_Logic.InsertSeqNo(tmpString1, formatServerTime);
          }
          getTroSeq = P00001_Logic.GetSeqNo(tmpString1, formatServerTime);//标签连番
          string lblSeqNo = getTroSeq.Rows[0][0].ToString();//标签开始位置



          for (int i = 0; i < getID.Rows.Count; i++)
          {

            //计算标签首,标签尾
            string partId = getID.Rows[i][0].ToString();
            string kanbanOrderNo = getID.Rows[i][1].ToString();
            string kanbanSerial = getID.Rows[i][2].ToString();
            string dock = getID.Rows[i][3].ToString();
            string quantity = getID.Rows[i][4].ToString();
            string bzUnit = getID.Rows[i][5].ToString();

            if (int.Parse(bzUnit) != 0)
            {

              string lblSart = serverTime.Replace("-", "").Substring(2, 7).Trim() + lblSeqNo.PadLeft(5, '0').Trim();
              string lblEnd = serverTime.Replace("-", "").Substring(2, 7).Trim() + (int.Parse(lblSeqNo) + (int.Parse(quantity) / int.Parse(bzUnit) - 1)).ToString().PadLeft(5, '0').Trim();
              lblSeqNo = (int.Parse(lblSeqNo) + int.Parse(quantity) / int.Parse(bzUnit)).ToString();
              int lblResultUp = P00001_Logic.UpdateLabel2(lblSart, lblEnd, iP, partId, kanbanOrderNo, kanbanSerial, dock);
            }








          }


          //结束循环时候需要更新连番
          int seqResultUp = P00001_Logic.UpdateSeqNo(tmpString1, formatServerTime, int.Parse(lblSeqNo));





        }



        #endregion

        #region 更新入库单号
        DataTable getInv = P00001_Logic.GetInv(iP);
        if (getInv.Rows.Count > 0)
        {
          DataTable getTroSeq = P00001_Logic.GetSeqNo(tmpString2, formatServerTime);
          if (getTroSeq.Rows.Count == 0)
          {
            int seqResultIn = P00001_Logic.InsertSeqNo(tmpString2, formatServerTime);
          }
          getTroSeq = P00001_Logic.GetSeqNo(tmpString2, formatServerTime);
          string invSeqNo = getTroSeq.Rows[0][0].ToString();
          int invSeqNoNew = int.Parse(invSeqNo) + getInv.Rows.Count;
          int seqResultUp = P00001_Logic.UpdateSeqNo(tmpString2, formatServerTime, invSeqNoNew);


          for (int i = 0; i < getInv.Rows.Count; i++)
          {
            string partId = getInv.Rows[i][0].ToString();
            string kanbanOrderNo = getInv.Rows[i][1].ToString();
            string kanbanSerial = getInv.Rows[i][2].ToString();
            string dock = getInv.Rows[i][3].ToString();
            string inno = serverTime.Replace("-", "").Substring(2, 7).Trim() + "2" + (int.Parse(invSeqNo) + i).ToString().PadLeft(5, '0');
            int invResultUp = P00001_Logic.UpdateInv(partId, kanbanOrderNo, kanbanSerial, dock, inno, iP);



          }




        }








        #endregion








        #endregion







        DataTable getQBData = P00001_Logic.GetQBData(iP);//从实绩情报表获得数据

        /*
        #region 打印前更新打印表

        //更新段取表,插入打印时间变成在再发行
        int pacResultUp = P00001_Logic.UpdatePack1(iP, serverTime);

        //更新段取打印表
        int printResultUp = P00001_Logic.UpdatePrint(iP);

        //更新标签打印表
        int printResultUp1 = P00001_Logic.UpdatePrint1(iP);

        //更新标签明细表,添加系统时间变成再发行

        int lblResultUP = P00001_Logic.UpdateLabel(iP,serverTime);
        #endregion

        */


        if (getQBData.Rows.Count > 0)
        {








          for (int i = 0; i < getQBData.Rows.Count; i++)
          {
            string trolleyNo = getQBData.Rows[i][0].ToString(); //台车号
            string inputNo = getQBData.Rows[i][1].ToString();//入库单号
            string partId = getQBData.Rows[i][2].ToString();//品番
            string cpdCompany = getQBData.Rows[i][3].ToString();//收货方
            string dock = getQBData.Rows[i][4].ToString();//受入
            string quantity = getQBData.Rows[i][5].ToString();//数量
            int SumQuantity = int.Parse(quantity);
            int newSum = 0;
            string kanbanOrderNo = getQBData.Rows[i][6].ToString();//看板订单号
            string kanbanSerial = getQBData.Rows[i][7].ToString();//看板连番
            string scanTime = getQBData.Rows[i][8].ToString();//客户端时间
            string packingSpot = getQBData.Rows[i][9].ToString();//包装场
            string supplierId = getQBData.Rows[i][10].ToString();//供应商代码
            string supplierPlant = getQBData.Rows[i][11].ToString();//供应商工区
            string lblStart = getQBData.Rows[i][12].ToString();//标签开始
            string lblEnd = getQBData.Rows[i][13].ToString();//标签结束

            string packingQuantity = getQBData.Rows[i][14].ToString();
            string lotId = getQBData.Rows[i][15].ToString();//段取指示号
            string checkType = getQBData.Rows[i][16].ToString();//检查区分
            string inoutFlag = getQBData.Rows[i][17].ToString();//内外区分
            string kanBan = getQBData.Rows[i][18].ToString();//看板

            //1.验证作业实绩,入出库履历表中是否存在数据

            DataTable validateOpr = P00001_Logic.ValidateOpr(partId, kanbanOrderNo, kanbanSerial, dock);//验证作业实绩

            DataTable validateQF = P00001_Logic.ValidateQF(partId, scanTime);//获得包装区分1.未包装 2.已包装,已包装不打印品番标签 入荷区分 1.可入荷 2.不可入荷 包装单位

            //2.不存在关键信息,不可入荷
            DataTable validateData = P00001_Logic.ValidateData(partId, scanTime, dock);//验证品番基础数据,获得收货方
            DataTable validateData1 = P00001_Logic.ValidateData1(partId, scanTime);//获得收容数 ,供应商代码,供应商工区
                                                                                   // DataTable getCheckType = P00001_Logic.GetCheckType(partId, scanTime);//获得检查区分
            DataTable getInOutFlag = P00001_Logic.GetInOutFlag(partId, scanTime);//获得内外区分
            DataTable validatePrice = P00001_Logic.ValidatePrice(partId, scanTime);//验证单价
            DataTable getPackItem = P00001_Logic.GetPackItem(scanTime, partId);//获得包材明细
            DataTable getTagInfo = P00001_Logic.GetTagInfo(partId, scanTime);

            //3.验证订单,判断是否存在有效订单
            DataTable validateOrd1 = P00001_Logic.ValidateOrd(partId);//验证订单有效性 
            DataTable getCount = P00001_Logic.GetCount(partId);
            DataTable validateOrd = P00001_Logic.ValiateOrd1(partId);//验证订单表
            if (validateData.Rows.Count == 1 && validateOrd1.Rows.Count == 1 && validateData1.Rows.Count == 1
  && getInOutFlag.Rows.Count == 1 && validatePrice.Rows.Count == 1 && validateQF.Rows.Count == 1
 && getPackItem.Rows.Count > 0 && validateOpr.Rows.Count == 0 && getTagInfo.Rows.Count == 1 && validateOrd.Rows.Count > 0)
            {

              int sum = int.Parse(validateOrd1.Rows[0][0].ToString());//当前订单表中的余量
              int sum1 = int.Parse(getCount.Rows[0][0].ToString());
              string bzQf = validateQF.Rows[0][0].ToString();//包装区分
              string rhQf = validateQF.Rows[0][1].ToString();//入荷区分
              string packingQuatity = validateQF.Rows[0][2].ToString();//包装单位
              string packQuantity = validateData1.Rows[0][0].ToString();//收容数
              string partsNameCn = getTagInfo.Rows[0][0].ToString();
              string supplierName = getTagInfo.Rows[0][1].ToString();//供应商名称
              string supplierAddress = getTagInfo.Rows[0][2].ToString();//供应商地址
              string excuteStand = getTagInfo.Rows[0][3].ToString();


              string carFamilyCode = getInOutFlag.Rows[0][2].ToString();//车种代码

              string partsNameEn = getInOutFlag.Rows[0][1].ToString();//英文品名

              #region  验证包材基础数据

              for (int j = 0; j < getPackItem.Rows.Count; j++)
              {
                string packNo = getPackItem.Rows[j][0].ToString();//包材品番
                string nessNo = getPackItem.Rows[j][1].ToString();//必要数
                DataTable getPackBase = P00001_Logic.GetPackBase(scanTime, packNo, packingSpot);
                if (getPackBase.Rows.Count != 1)
                {


                  apiResult.code = ComConstant.ERROR_CODE;
                  apiResult.data = "包材品番" + packNo + "在包材基础数据表里没有有效信息!";
                  return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }


              }
              #endregion

              if (int.Parse(packQuantity) != 0 && rhQf == "1" && int.Parse(packingQuantity) != 0)
              {


                int oprReusultIn = P00001_Logic.InsertOpr(packingSpot, inputNo, kanbanOrderNo, kanbanSerial, partId, inoutFlag, supplierId, supplierPlant, scanTime, serverTime, quantity, int.Parse(packingQuantity), cpdCompany, dock, checkType, lblStart, lblEnd, opearteId);//插入作业实际表
                int invResultIn = P00001_Logic.InsertInv(packingSpot, inputNo, partId, cpdCompany, quantity, serverTime, kanbanOrderNo, kanbanSerial, scanTime, opearteId);//插入入出库履历表
                                                                                                                                                                           //更新实绩情报表



                if (bzQf == "1")
                {


                  #region 存储数据全部存储,在打印时进行区分
                  string date = lblStart.Substring(0, 6);//标签首

                  int front = int.Parse(lblStart.Substring(6, 5));//开始
                  int back = int.Parse(lblEnd.Substring(6, 5));//结束
                  for (int a = front; a <= back; a++)
                  {//此时的连番号就是a

                    string printCount = date + a.ToString().PadLeft(5, '0');
                    string printCount1 = date + a.ToString().PadLeft(5, '0') + "B";
                    string content = "https://wx-m.ftms.com.cn/carowner/part?tabindex=3&tracingcode=" + printCount;
                    string content1 = "https://wx-m.ftms.com.cn/carowner/part?tabindex=3&tracingcode=" + printCount + "B";
                    string partId1 = "";
                    byte[] vs = P00001_Logic.GenerateQRCode(content);
                    byte[] vs2 = P00001_Logic.GenerateQRCode(content1);
                    if (partId.Length == 12)
                    {
                      partId1 = partId.Substring(0, 5) + "-" + partId.Substring(5, 5) + "-" + partId.Substring(10, 2);

                    }
                    int lblResultIn = P00001_Logic.InsertLbl(partsNameEn, partId, cpdCompany, quantity, printCount, supplierName, supplierAddress, carFamilyCode, opearteId, scanTime, iP, partsNameCn, inputNo, vs, printCount1, vs2, excuteStand, packingQuatity, partId1);


                  }

                  #endregion

                }












                #region  根据段取指示号打印段取指示书
                for (int j = 0; j < getPackItem.Rows.Count; j++)
                {

                  string packNo = getPackItem.Rows[j][0].ToString();//包材品番
                  string nessNo = getPackItem.Rows[j][1].ToString();//必要数
                  DataTable getPackBase = P00001_Logic.GetPackBase(scanTime, packNo, packingSpot);
                  if (getPackBase.Rows.Count == 1)
                  {
                    string packLocation = getPackBase.Rows[0][0].ToString();//包材位置
                    string distinguish = getPackBase.Rows[0][1].ToString();//包材区分
                    double qty = double.Parse(nessNo) * double.Parse(quantity) / double.Parse(packingQuantity);//0413
                    DataTable validatePack = P00001_Logic.ValidatePack(lotId, iP, opearteId, packNo);
                    if (validatePack.Rows.Count == 0)
                    {

                      int packResultIn = P00001_Logic.InsertPack(lotId, packNo, distinguish, inputNo, qty, packLocation, scanTime, serverTime, opearteId, iP, trolleyNo, lblStart, lblEnd);
                    }
                    else if (validatePack.Rows.Count == 1)
                    {
                      int packResultUp = P00001_Logic.UpdatePack(lotId, packNo, iP, opearteId, qty);
                    }
                  }
                  else
                  {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "包材品番" + packNo + "在包材基础数据中没有有效数据,请检查!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                  }

                }

                #endregion

                #region 更新订单
                for (int j = 0; j < validateOrd.Rows.Count; j++)
                {
                  string targetMonth = validateOrd.Rows[j][0].ToString();
                  string orderType = validateOrd.Rows[j][1].ToString();
                  string orderNo = validateOrd.Rows[j][2].ToString();
                  string seqNo = validateOrd.Rows[j][3].ToString();
                  int d1 = int.Parse(validateOrd.Rows[j][4].ToString());
                  int d2 = int.Parse(validateOrd.Rows[j][5].ToString());
                  int d3 = int.Parse(validateOrd.Rows[j][6].ToString());
                  int d4 = int.Parse(validateOrd.Rows[j][7].ToString());
                  int d5 = int.Parse(validateOrd.Rows[j][8].ToString());
                  int d6 = int.Parse(validateOrd.Rows[j][9].ToString());
                  int d7 = int.Parse(validateOrd.Rows[j][10].ToString());
                  int d8 = int.Parse(validateOrd.Rows[j][11].ToString());
                  int d9 = int.Parse(validateOrd.Rows[j][12].ToString());
                  int d10 = int.Parse(validateOrd.Rows[j][13].ToString());
                  int d11 = int.Parse(validateOrd.Rows[j][14].ToString());
                  int d12 = int.Parse(validateOrd.Rows[j][15].ToString());
                  int d13 = int.Parse(validateOrd.Rows[j][16].ToString());
                  int d14 = int.Parse(validateOrd.Rows[j][17].ToString());
                  int d15 = int.Parse(validateOrd.Rows[j][18].ToString());
                  int d16 = int.Parse(validateOrd.Rows[j][19].ToString());
                  int d17 = int.Parse(validateOrd.Rows[j][20].ToString());
                  int d18 = int.Parse(validateOrd.Rows[j][21].ToString());
                  int d19 = int.Parse(validateOrd.Rows[j][22].ToString());
                  int d20 = int.Parse(validateOrd.Rows[j][23].ToString());
                  int d21 = int.Parse(validateOrd.Rows[j][24].ToString());
                  int d22 = int.Parse(validateOrd.Rows[j][25].ToString());
                  int d23 = int.Parse(validateOrd.Rows[j][26].ToString());
                  int d24 = int.Parse(validateOrd.Rows[j][27].ToString());
                  int d25 = int.Parse(validateOrd.Rows[j][28].ToString());
                  int d26 = int.Parse(validateOrd.Rows[j][29].ToString());
                  int d27 = int.Parse(validateOrd.Rows[j][30].ToString());
                  int d28 = int.Parse(validateOrd.Rows[j][31].ToString());
                  int d29 = int.Parse(validateOrd.Rows[j][32].ToString());
                  int d30 = int.Parse(validateOrd.Rows[j][33].ToString());
                  int d31 = int.Parse(validateOrd.Rows[j][34].ToString());
                  int[] array = {d1, d2, d3, d4, d5, d6, d7, d8, d9, d10, d11, d12, d13, d14, d15, d16, d17, d18, d19, d20, d21,
              d22,d23,d24,d25,d26,d27,d28,d29,d30,d31 };
                  int[] newarray = new int[31];
                  for (int l = 0; l < array.Length; l++)
                  {
                    if (SumQuantity - array[l] > 0)
                    {
                      newarray[l] = array[l];
                      SumQuantity = SumQuantity - array[l];

                    }
                    else
                    {
                      newarray[l] = SumQuantity;
                      SumQuantity = 0;

                    }
                  }
                  for (int k = 0; k < newarray.Length; k++)
                  {
                    newSum += newarray[k];
                  }
                  int updateOrd = P00001_Logic.UpdateOrd(targetMonth, orderNo, seqNo, newarray[0], newarray[1], newarray[2], newarray[3], newarray[4], newarray[5], newarray[6], newarray[7], newarray[8], newarray[9], newarray[10],
                    newarray[11], newarray[12], newarray[13], newarray[14], newarray[15], newarray[16], newarray[17], newarray[18], newarray[19], newarray[20], newarray[21], newarray[22], newarray[23],
                    newarray[24], newarray[25], newarray[26], newarray[27], newarray[28], newarray[29], newarray[30], newSum, partId);
                  newSum = 0;
                }



                #endregion



                #region 生成构内XML文件





                if (inoutFlag == "1" && kanBan != "")
                {
                  DataTable getPointNo = P00001_Logic.GetPointNo(iP);
                  if (getPointNo.Rows.Count == 1)
                  {
                    string name = getPointNo.Rows[0][1].ToString().PadLeft(5, '0');
                    string formatTime = serverTime.Replace("-", "").Replace(":", "").Replace(" ", "");
                    DataEntity.P00001_DataEntity.ScanData data2 = P00001_Logic.CutScanData(kanBan);
                    P00001_Logic.SaveXml(data2, serverTime, name, formatTime);



                  }


                }




                #endregion







                #region  存取数据到入库指令书
                DataTable getPackInfo = P00001_Logic.GetPackInfo(packingQuantity, partId, scanTime, quantity);
                DataTable temp = P00001_Logic.GetInv();
                DataTable dtInfo = temp.Clone();
                DataRow dr = dtInfo.NewRow();
                dtInfo.Rows.Add(dr);
                for (int o = 0; o < getPackInfo.Rows.Count; o++)
                {
                  string distinguish = getPackInfo.Rows[o][0].ToString();
                  string packLocation = getPackInfo.Rows[o][1].ToString();
                  string packNo = getPackInfo.Rows[o][2].ToString();
                  string quantity1 = getPackInfo.Rows[o][3].ToString();


                  dtInfo.Rows[0][10 + (o * 4) + 0] = distinguish;
                  dtInfo.Rows[0][10 + (o * 4) + 1] = packLocation;
                  dtInfo.Rows[0][10 + (o * 4) + 2] = packNo;
                  dtInfo.Rows[0][10 + (o * 4) + 3] = quantity1;

                }






                #region  包材列
                string itemname1 = dtInfo.Rows[0][10].ToString();
                string packLocation1 = dtInfo.Rows[0][11].ToString();
                string suppName1 = dtInfo.Rows[0][12].ToString();
                string outNum1 = dtInfo.Rows[0][13].ToString();


                string itemname2 = dtInfo.Rows[0][14].ToString();
                string packLocation2 = dtInfo.Rows[0][15].ToString();
                string suppName2 = dtInfo.Rows[0][16].ToString();
                string outNum2 = dtInfo.Rows[0][17].ToString();



                string itemname3 = dtInfo.Rows[0][18].ToString();
                string packLocation3 = dtInfo.Rows[0][19].ToString();
                string suppName3 = dtInfo.Rows[0][20].ToString();
                string outNum3 = dtInfo.Rows[0][21].ToString();



                string itemname4 = dtInfo.Rows[0][22].ToString();
                string packLocation4 = dtInfo.Rows[0][23].ToString();
                string suppName4 = dtInfo.Rows[0][24].ToString();
                string outNum4 = dtInfo.Rows[0][25].ToString();



                string itemname5 = dtInfo.Rows[0][26].ToString();
                string packLocation5 = dtInfo.Rows[0][27].ToString();
                string suppName5 = dtInfo.Rows[0][28].ToString();
                string outNum5 = dtInfo.Rows[0][29].ToString();



                string itemname6 = dtInfo.Rows[0][30].ToString();
                string packLocation6 = dtInfo.Rows[0][31].ToString();
                string suppName6 = dtInfo.Rows[0][32].ToString();
                string outNum6 = dtInfo.Rows[0][33].ToString();



                string itemname7 = dtInfo.Rows[0][34].ToString();
                string packLocation7 = dtInfo.Rows[0][35].ToString();
                string suppName7 = dtInfo.Rows[0][36].ToString();
                string outNum7 = dtInfo.Rows[0][37].ToString();



                string itemname8 = dtInfo.Rows[0][38].ToString();
                string packLocation8 = dtInfo.Rows[0][39].ToString();
                string suppName8 = dtInfo.Rows[0][40].ToString();
                string outNum8 = dtInfo.Rows[0][41].ToString();



                string itemname9 = dtInfo.Rows[0][42].ToString();
                string packLocation9 = dtInfo.Rows[0][43].ToString();
                string suppName9 = dtInfo.Rows[0][44].ToString();
                string outNum9 = dtInfo.Rows[0][45].ToString();


                string itemname10 = dtInfo.Rows[0][46].ToString();
                string packLocation10 = dtInfo.Rows[0][47].ToString();
                string suppName10 = dtInfo.Rows[0][48].ToString();
                string outNum10 = dtInfo.Rows[0][49].ToString();


                string data1 = "包装场" + packingSpot;
                string printDate = serverTime.Substring(0, 10);
                string partsAndNum = partId + quantity.PadLeft(5, '0') + cpdCompany + inputNo;

                byte[] vs1 = P00001_Logic.GenerateQRCode(partsAndNum);



                int intInsertIn = P00001_Logic.InsertInvList(data1, printDate, inputNo, partId, partsNameEn, quantity, packingQuantity, itemname1, packLocation1, suppName1, outNum1,
itemname2, packLocation2, suppName2, outNum2, itemname3, packLocation3, suppName3, outNum3, itemname4, packLocation4, suppName4, outNum4, itemname5, packLocation5, suppName5, outNum5,
itemname6, packLocation6, suppName6, outNum6, itemname7, packLocation7, suppName7, outNum7, itemname8, packLocation8, suppName8, outNum8, itemname9, packLocation9, suppName9, outNum9,
itemname10, packLocation10, suppName10, outNum10, partsAndNum, cpdCompany, opearteId, scanTime, vs1);











                #endregion






                #endregion



              }
              else if (rhQf != "1")
              {
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "当前品番" + partId + "在包装基础数据表中入荷区分为不可入荷,请检查";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


              }




              else if (int.Parse(packQuantity) == 0)
              {
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "当前品番" + partId + "在品番基础数据中的收容数为0,请检查";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

              }
              else if (int.Parse(packingQuantity) == 0)
              {
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "当前品番" + partId + "在包装基础数据中的包装单位为0,请检查";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

              }


            }
            else if (validateOpr.Rows.Count != 0)
            {

              apiResult.code = ComConstant.ERROR_CODE;
              apiResult.data = "品番" + partId + "在作业实绩表中已存在数据";
              return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }


            else if (validateOrd1.Rows.Count == 0 || validateOrd.Rows.Count == 0)
            {

              apiResult.code = ComConstant.ERROR_CODE;
              apiResult.data = "品番" + partId + "在订单基础数据中没有有效订单";
              return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }


            else if (getPackItem.Rows.Count == 0)
            {

              apiResult.code = ComConstant.ERROR_CODE;
              apiResult.data = "品番" + partId + "在包材构成数据表中没有有效数据";
              return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            else if (validatePrice.Rows.Count != 1)
            {

              apiResult.code = ComConstant.ERROR_CODE;
              apiResult.data = "品番" + partId + "在销售价格表中没有有效数据";
              return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            else if (validateQF.Rows.Count != 1)
            {

              apiResult.code = ComConstant.ERROR_CODE;
              apiResult.data = "品番" + partId + "在包装基础数据表中没有有效数据";
              return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            else if (getTagInfo.Rows.Count != 1)
            {

              apiResult.code = ComConstant.ERROR_CODE;
              apiResult.data = "品番" + partId + "在标签信息表中没有有效数据";
              return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            else if (validateData.Rows.Count != 1 || validateData1.Rows.Count != 1 || getInOutFlag.Rows.Count != 1)
            {

              apiResult.code = ComConstant.ERROR_CODE;
              apiResult.data = "品番" + partId + "在品番基础数据中不存在有效数据";
              return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
          }
        }
        else
        {

          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "当前没有需要更新的数据";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

        }

        #region  循环结束之后更新实绩情报表 
        int qbResultUp = P00001_Logic.UpdateQB(iP);


        #endregion


        #region  循环结束之后更新台车表
        int trolleyResultUp = P00001_Logic.UpdateTrolley(opearteId, iP);




        #endregion





        #region  循环打印段取指示书
        DataTable getPack = P00001_Logic.GetPackBase(iP);


        if (getPack.Rows.Count > 0)
        {
          string lotHead = getPack.Rows[0][0].ToString();
          string lotEnd = getPack.Rows[getPack.Rows.Count - 1][0].ToString();
          string lotFront = lotHead.Substring(0, 9);
          int front1 = int.Parse(lotHead.Substring(9, 5));
          int end1 = int.Parse(lotEnd.Substring(9, 5));
          for (int j = front1; j <= end1; j++)
          {
            string lotIdNew = lotFront + j.ToString().PadLeft(5, '0');
            DataTable validateQB = P00001_Logic.ValidateQB5(lotIdNew);
            if (validateQB.Rows.Count > 0)
            {
              int tpResultIn = P00001_Logic.InsertTP(iP, opearteId, serverTime, lotIdNew);

              DataTable getLabel = P00001_Logic.GetLabel(iP, lotIdNew);
              string minLabel = getLabel.Rows[0][0].ToString();
              string maxLabel = getLabel.Rows[0][1].ToString();
              int pacResultUp1 = P00001_Logic.UpdatePack(iP, minLabel, maxLabel, lotIdNew);




            }






          }
        }
        else
        {
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "当前段取指示表中没有对应的数据";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }



        #endregion


        #region 在打印结束之后更新当前IP的首次打印时间，如果打印失败将其变成再发行
        int packResultUp1 = P00001_Logic.UpdatePack2(iP,serverTime);
        #endregion


        #region 在打印品番标签之前对内容进行判断是否打印






        #endregion




        #region  在打印之判断是否有需要打印的内容
        DataTable validatePrint = P00001_Logic.ValidatePrint(iP);
        DataTable getPrintName = P00001_Logic.GetPrintName(iP);

        if (validatePrint.Rows.Count > 0 && getPrintName.Rows.Count == 1)
        {
          string printName = getPrintName.Rows[0][0].ToString();
          int lbResultIn = P00001_Logic.InsertTP1(iP, opearteId, serverTime, printName);


        }
        else if (getPrintName.Rows.Count != 1)
        {
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "打印机表中没有有效的标签打印机,请联系管理员!";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }







        #endregion




        p00001_DataEntity.result = "入库成功";

        apiResult.data = p00001_DataEntity;
        //标签打印插入位置



      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "更新入库数据失败";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }

      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
    }








    #endregion



    #region 插入实绩情报表

    [HttpPost]
    [EnableCors("any")]
    public string ValidateInApi([FromBody] dynamic data)
    {
      string strToken = Request.Headers["X-Token"];
      if (!isLogin(strToken))
      {
        return error_login();
      }
      LoginInfo loginInfo = getLoginByToken(strToken);
      string opearteId = loginInfo.UserId;

      ApiResult apiResult = new ApiResult();


      try
      {
        dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));

        string trolley = dataForm.Trolley == null ? "" : dataForm.Trolley;//台车号
        string partId = dataForm.PartId == null ? "" : dataForm.PartId;//品番
        string quantity1 = dataForm.Quantity == null ? "" : dataForm.Quantity;//数量
        string quantity = quantity1.PadLeft(5, '0');
        string dock = dataForm.Dock == null ? "" : dataForm.Dock;//受入
        string kanbanOrderNo = dataForm.KanbanOrderNo == null ? "" : dataForm.KanbanOrderNo;//看板订单号
        string kanbanSerial = dataForm.KanbanSerial == null ? "" : dataForm.KanbanSerial;//看板连番

        string scanTime = dataForm.ScanTime == null ? "" : dataForm.ScanTime;//客户端时间
        string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
        string kanBan = dataForm.KanBan == null ? "" : dataForm.KanBan;//段取指示号
        string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
        //TrolleySeqNo
        string trolleySeqNo = dataForm.TrolleySeqNo == null ? "" : dataForm.TrolleySeqNo;//台车顺番号

        /* string IP = (Request.HttpContext.Connection.LocalIpAddress.MapToIPv4().ToString());*/ //服务端IP地址
        //KanBan
        string reg = @"^[0-9]+$";
        string reg1 = @"^[a-z0-9A-Z]+$";

        #region  入库发生异常时验证使用
        bool a = trolleySeqNo != "" && scanTime != "" && trolley != "" && partId != "" && quantity != "" && dock != "" && kanbanOrderNo != "" && kanbanSerial != "";
        bool b = trolleySeqNo.Length == 14 && trolley.Length == 6 && Regex.IsMatch(trolley, reg1) && partId.Length == 12;

        bool c = Regex.IsMatch(partId, reg1) && quantity.Length == 5 && Regex.IsMatch(quantity, reg) && dock.Length == 2 && Regex.IsMatch(dock, reg1);
        bool d = (kanbanOrderNo.Length == 10 || kanbanOrderNo.Length == 12) && Regex.IsMatch(kanbanOrderNo, reg1) && kanbanSerial.Length == 4 && Regex.IsMatch(kanbanSerial, reg);
        #endregion





        if (trolleySeqNo != "" && scanTime != "" && trolley != "" && partId != "" && quantity != "" && dock != "" && kanbanOrderNo != "" && kanbanSerial != ""
                    && trolleySeqNo.Length == 6 && trolley.Length == 6 && Regex.IsMatch(trolley, reg1) && partId.Length == 12 &&
                    Regex.IsMatch(partId, reg1) && quantity.Length == 5 && Regex.IsMatch(quantity, reg) && dock.Length == 2 && Regex.IsMatch(dock, reg1) &&
                    (kanbanOrderNo.Length == 10 || kanbanOrderNo.Length == 12) && Regex.IsMatch(kanbanOrderNo, reg1) && kanbanSerial.Length == 4 && Regex.IsMatch(kanbanSerial, reg)
                    )
        {
          #region 验证是否可以入荷
          //1.如果数据重复,入荷区分为不可,不可入荷
          DataTable validateQB = P00001_Logic.ValidateQB(partId, kanbanOrderNo, kanbanSerial, dock);
          DataTable validateQF = P00001_Logic.ValidateQF(partId, scanTime);//获得包装区分1.未包装 2.已包装,已包装不打印品番标签 入荷区分 1.可入荷 2.不可入荷 包装单位

          //2.不存在关键信息,不可入荷
          DataTable validateData = P00001_Logic.ValidateData(partId, scanTime, dock);//验证品番基础数据,获得收货方
          DataTable validateData1 = P00001_Logic.ValidateData1(partId, scanTime);//获得收容数 ,供应商代码,供应商工区
          //DataTable getCheckType = P00001_Logic.GetCheckType(partId, scanTime);//获得检查区分
          DataTable getInOutFlag = P00001_Logic.GetInOutFlag(partId, scanTime);//获得内外区分
          DataTable validatePrice = P00001_Logic.ValidatePrice(partId, scanTime);//验证单价
          DataTable getPackItem = P00001_Logic.GetPackItem(scanTime, partId);//获得包材明细
          DataTable getTagInfo = P00001_Logic.GetTagInfo(partId, scanTime);


          //3.验证订单,判断是否存在有效订单
          DataTable validateOrd = P00001_Logic.ValidateOrd(partId);//验证订单有效性 
          DataTable getCount = P00001_Logic.GetCount(partId);

          if (validateQB.Rows.Count == 0 && validateData.Rows.Count == 1 && validateOrd.Rows.Count == 1 && validateData1.Rows.Count == 1
    && getInOutFlag.Rows.Count == 1 && validatePrice.Rows.Count == 1 && validateQF.Rows.Count == 1
    && getPackItem.Rows.Count > 0 && getTagInfo.Rows.Count == 1)
          {//可以入荷

            int sum = int.Parse(validateOrd.Rows[0][0].ToString());//当前订单表中的余量
            int sum1 = int.Parse(getCount.Rows[0][0].ToString());





            string inoutFlag = getInOutFlag.Rows[0][0].ToString();
            string bzQf = validateQF.Rows[0][0].ToString();//包装区分
            string rhQf = validateQF.Rows[0][1].ToString();//入荷区分
            string packingQuatity = validateQF.Rows[0][2].ToString();//包装单位
            string packQuantity = validateData1.Rows[0][0].ToString();//收容数
            string supplierId = validateData1.Rows[0][1].ToString();
            string supplierPlant = validateData1.Rows[0][2].ToString();
            string cpdCompany = validateData.Rows[0][0].ToString();//收货方
            string packingSpot = "H2";
            string lblSart = "";
            string lblEnd = "";
            string inno = "";
            string tmpString = "INO";//临时字符串
            string tmpString1 = "LBL";
            string formatServerTime = serverTime.Substring(0, 10).Replace("-", "");//格式化号口时间
            DataTable inoSeqNodt = P00001_Logic.ValidateSeqNo(packingSpot, formatServerTime, tmpString);//判断连番表里是否有当天的入库指令书连番
            DataTable lblSeqNodt = P00001_Logic.ValidateSeqNo(packingSpot, formatServerTime, tmpString1);//判断连番表里是否有当天的标签连番

            #region 验证包材基础数据

            for (int j = 0; j < getPackItem.Rows.Count; j++)
            {
              string packNo = getPackItem.Rows[j][0].ToString();//包材品番
              string nessNo = getPackItem.Rows[j][1].ToString();//必要数
              DataTable getPackBase = P00001_Logic.GetPackBase(scanTime, packNo, packingSpot);
              if (getPackBase.Rows.Count != 1)
              {
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "包材品番" + packNo + "在包材基础数据中没有有效数据,请检查!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

              }


            }
            #endregion


            #endregion
            if (sum - sum1 >= int.Parse(quantity) && int.Parse(packQuantity) != 0 && inoSeqNodt.Rows.Count < 2 && lblSeqNodt.Rows.Count < 2 && rhQf == "1" && (int.Parse(quantity) % int.Parse(packingQuatity) == 0))//入荷区分是1代表不可入荷
            {

              /*
              if (inoSeqNodt.Rows.Count == 0)
              {
                int inoSeqNoIn = P00001_Logic.InsertSeqNo(packingSpot, formatServerTime, tmpString);//如果当天没有插入信息
                inoSeqNodt = P00001_Logic.ValidateSeqNo(packingSpot, formatServerTime, tmpString);
              }

              if (bzQf=="1") { 


              if (lblSeqNodt.Rows.Count == 0)
              {

                int lblSeqNoIn = P00001_Logic.InsertSeqNo(packingSpot, formatServerTime, tmpString1);//如果当天没有插入信息
                lblSeqNodt = P00001_Logic.ValidateSeqNo(packingSpot, formatServerTime, tmpString1);
              }
              string lblSeqNo = lblSeqNodt.Rows[0][2].ToString();//取得标签连番号
              
              lblSart = serverTime.Replace("-", "").Substring(2, 7).Trim() + lblSeqNo.PadLeft(5, '0').Trim();
              int lblSeqNoNew = int.Parse(lblSeqNo) + int.Parse(quantity) / int.Parse(packingQuatity);
             lblEnd = serverTime.Replace("-", "").Substring(2, 7).Trim() + (lblSeqNoNew - 1).ToString().PadLeft(5, '0').Trim();
              int lblResultUp = P00001_Logic.UpdateSeqNo(packingSpot, formatServerTime, lblSeqNoNew, tmpString1);

              }



              string inoSeqNo = inoSeqNodt.Rows[0][2].ToString();//取得入库指令书连番号
              string inno = serverTime.Replace("-", "").Substring(2, 7).Trim() + packingSpot.Substring(1, 1).Trim() + (int.Parse(inoSeqNo) + int.Parse(quantity) / int.Parse(packQuantity) - 1).ToString().PadLeft(5, '0').Trim();//获得入库单号
              int inoSeqNoNew = int.Parse(inoSeqNo) + int.Parse(quantity) / int.Parse(packQuantity);//新的入库指令书连番号
              int inoResultUp = P00001_Logic.UpdateSeqNo(packingSpot, formatServerTime, inoSeqNoNew, tmpString);//更新连番表
     */
              int qbResultIn = P00001_Logic.Insert(trolley, partId, quantity, dock, kanbanOrderNo, kanbanSerial, scanTime, iP, serverTime, cpdCompany, inno, opearteId, packingSpot, packingQuatity, lblSart, lblEnd, supplierId, supplierPlant, trolleySeqNo, inoutFlag, kanBan);//插入实绩情报表

            }
            else if (int.Parse(quantity) % int.Parse(packingQuatity) != 0)
            {


              apiResult.code = ComConstant.ERROR_CODE;
              apiResult.data = "当前品番" + partId + "在输入数量不是包装单位的整数倍,请检查";
              return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            else if (sum - sum1 < int.Parse(quantity))
            {
              apiResult.code = ComConstant.ERROR_CODE;
              apiResult.data = "当前品番" + partId + "在订单基础数据中的有效数小于入库数量或当前订单为无效订单,请检查";
              return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            else if (int.Parse(packQuantity) == 0)
            {
              apiResult.code = ComConstant.ERROR_CODE;
              apiResult.data = "当前品番" + partId + "在品番基础数据中的收容数为0,请检查";
              return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }
            else if (inoSeqNodt.Rows.Count > 1)
            {

              apiResult.code = ComConstant.ERROR_CODE;
              apiResult.data = "当前品番" + partId + "在连番表中数据重复,请检查";
              return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }
            else if (rhQf != "1")
            {
              apiResult.code = ComConstant.ERROR_CODE;
              apiResult.data = "当前品番" + partId + "在包装基础数据表中入荷区分为不可入荷,请检查";
              return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


            }



















          }
          else if (validateQB.Rows.Count > 0)
          {
            apiResult.code = ComConstant.ERROR_CODE;
            apiResult.data = "看板订单号" + kanbanOrderNo + "看板连番" + kanbanSerial + "品番" + partId + "已经入库";
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
          }
          else if (validateOrd.Rows.Count == 0)
          {
            apiResult.code = ComConstant.ERROR_CODE;
            apiResult.data = "品番" + partId + "在订单基础数据中没有有效订单";
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

          }

          else if (getPackItem.Rows.Count == 0)
          {
            apiResult.code = ComConstant.ERROR_CODE;
            apiResult.data = "品番" + partId + "在包材构成数据表中没有有效数据";
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

          }
          else if (validatePrice.Rows.Count != 1)
          {
            apiResult.code = ComConstant.ERROR_CODE;
            apiResult.data = "品番" + partId + "在销售价格表中没有有效数据";
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

          }
          else if (validateQF.Rows.Count != 1)
          {
            apiResult.code = ComConstant.ERROR_CODE;
            apiResult.data = "品番" + partId + "在包装基础数据表中没有有效数据";
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

          }
          else if (getTagInfo.Rows.Count != 1)
          {
            apiResult.code = ComConstant.ERROR_CODE;
            apiResult.data = "品番" + partId + "在标签信息表中没有有效数据";
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

          }
          else if (validateData.Rows.Count != 1 || validateData1.Rows.Count != 1 || getInOutFlag.Rows.Count != 1)
          {
            apiResult.code = ComConstant.ERROR_CODE;
            apiResult.data = "品番" + partId + "在品番基础数据中不存在有效数据";
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

          }

          #endregion
























        }
        else
        {


          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "扫描的数据不符合设定或含有特殊字符,请重新扫描!";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


        }

















        /*

        //DataTable dockdt = P00001_Logic.ValidateDock(Dock);//受入不需要验证
        DataTable validateDt = P00001_Logic.Validate(partId, kanbanOrderNo, kanbanSerial, dock);//验证上传数据是否唯一

        DataTable validateData = P00001_Logic.ValidateData(partId, scanTime, dock);//验证当前品番的有效性,获得收货方
        DataTable validateData1 = P00001_Logic.ValidateData1(partId, scanTime);//获得收容数 ,供应商代码,供应商工区

        DataTable validateOrd = P00001_Logic.ValidateOrd(partId);//验证订单有效性 (这个判断有问题)

        DataTable getCount = P00001_Logic.GetCount(partId);//验证当前实绩情报表中当前品番总数


        DataTable getCheckType = P00001_Logic.GetCheckType(partId, scanTime);//获得检查区分

        DataTable getInOutFlag = P00001_Logic.GetInOutFlag(partId, scanTime);//获得内外区分

        DataTable getPackItem = P00001_Logic.GetPackItem(scanTime, partId);//获得包材明细

        DataTable ValidatePrice = P00001_Logic.ValidatePrice(partId, scanTime);//验证单价


        DataTable ValidateQF = P00001_Logic.ValidateQF(partId, scanTime);//验证区分
        */
        /*
        if (validateDt.Rows.Count == 0 && validateData.Rows.Count == 1 && validateOrd.Rows.Count == 1 && validateData1.Rows.Count == 1
          && getCheckType.Rows.Count == 1 && getInOutFlag.Rows.Count == 1 && ValidatePrice.Rows.Count == 1 && ValidateQF.Rows.Count == 1
          && getPackItem.Rows.Count > 0

          )
        {//可以入荷
          int sum = int.Parse(validateOrd.Rows[0][0].ToString());//当前订单表中的余量
          int sum1 = int.Parse(getCount.Rows[0][0].ToString());


          string bzQf = ValidateQF.Rows[0][0].ToString();//包装区分
          string rhQf = ValidateQF.Rows[0][1].ToString();//入荷区分
          string packingQuatity = ValidateQF.Rows[0][2].ToString();//包装单位
          string packQuantity = validateData1.Rows[0][0].ToString();//收容数
          string supplierId = validateData1.Rows[0][1].ToString();
          string supplierPlant = validateData1.Rows[0][2].ToString();
          string cpdCompany = validateData.Rows[0][0].ToString();//收货方
          string packingSpot = validateData.Rows[0][1].ToString();//包装场
          string tmpString = "INO";//临时字符串
          string tmpString1 = "LBL";
          string formatServerTime = serverTime.Substring(0, 10).Replace("-", "");//格式化号口时间
          DataTable inoSeqNodt = P00001_Logic.ValidateSeqNo(packingSpot, formatServerTime, tmpString);//判断连番表里是否有当天的入库指令书连番
          DataTable lblSeqNodt = P00001_Logic.ValidateSeqNo(packingSpot, formatServerTime, tmpString1);//判断连番表里是否有当天的标签连番
          #region 验证包材基础数据

          for (int j = 0; j < getPackItem.Rows.Count; j++)
          {
            string packNo = getPackItem.Rows[j][0].ToString();//包材品番
            string nessNo = getPackItem.Rows[j][1].ToString();//必要数
            DataTable getPackBase = P00001_Logic.GetPackBase(scanTime, packNo, packingSpot);
            if (getPackBase.Rows.Count != 1)
            {
              apiResult.code = ComConstant.ERROR_CODE;
              apiResult.data = "包材品番" + packNo + "在包材基础数据中没有有效数据,请检查!";
              return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }


          }
          #endregion
          if (sum - sum1 >= int.Parse(quantity) && int.Parse(packQuantity) != 0 && inoSeqNodt.Rows.Count < 2 && lblSeqNodt.Rows.Count < 2 && rhQf == "0")//入荷区分是1代表不可入荷
          {
            if (inoSeqNodt.Rows.Count == 0)
            {
              int inoSeqNoIn = P00001_Logic.InsertSeqNo(packingSpot, formatServerTime, tmpString);//如果当天没有插入信息
              inoSeqNodt = P00001_Logic.ValidateSeqNo(packingSpot, formatServerTime, tmpString);
            }
            if (lblSeqNodt.Rows.Count == 0)
            {

              int lblSeqNoIn = P00001_Logic.InsertSeqNo(packingSpot, formatServerTime, tmpString1);//如果当天没有插入信息
              lblSeqNodt = P00001_Logic.ValidateSeqNo(packingSpot, formatServerTime, tmpString1);
            }




            string lblSeqNo = lblSeqNodt.Rows[0][2].ToString();//取得标签连番号
            string inoSeqNo = inoSeqNodt.Rows[0][2].ToString();//取得入库指令书连番号
            string lblSart = serverTime.Replace("-", "").Substring(2, 7).Trim() + lblSeqNo.PadLeft(5, '0').Trim();
            int lblSeqNoNew = int.Parse(lblSeqNo) + int.Parse(quantity);
            string lblEnd = serverTime.Replace("-", "").Substring(2, 7).Trim() + (lblSeqNoNew - 1).ToString().PadLeft(5, '0').Trim();

            string inno = serverTime.Replace("-", "").Substring(2, 7).Trim() + packingSpot.Substring(1, 1).Trim() + (int.Parse(inoSeqNo) + int.Parse(quantity) / int.Parse(packQuantity) - 1).ToString().PadLeft(5, '0').Trim();//获得入库单号
            int inoSeqNoNew = int.Parse(inoSeqNo) + int.Parse(quantity) / int.Parse(packQuantity);//新的入库指令书连番号
            int inoResultUp = P00001_Logic.UpdateSeqNo(packingSpot, formatServerTime, inoSeqNoNew, tmpString);//更新连番表
            int lblResultUp = P00001_Logic.UpdateSeqNo(packingSpot, formatServerTime, lblSeqNoNew, tmpString1);
            int qbResultIn = P00001_Logic.Insert(trolley, partId, quantity, dock, kanbanOrderNo, kanbanSerial, scanTime, iP, serverTime, cpdCompany, inno, opearteId, packingSpot, packingQuatity, lblSart, lblEnd, supplierId, supplierPlant, lotId);//插入实绩情报表

          }
          else if (sum - sum1 < int.Parse(quantity))
          {
            apiResult.code = ComConstant.ERROR_CODE;
            apiResult.data = "当前品番" + partId + "在订单基础数据中的有效数小于入库数量或当前订单为无效订单,请检查";
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
          }
          else if (int.Parse(packQuantity) == 0)
          {
            apiResult.code = ComConstant.ERROR_CODE;
            apiResult.data = "当前品番" + partId + "在品番基础数据中的收容数为0,请检查";
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

          }
          else if (inoSeqNodt.Rows.Count > 1)
          {

            apiResult.code = ComConstant.ERROR_CODE;
            apiResult.data = "当前品番" + partId + "在连番表中数据重复,请检查";
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

          }
          else if (rhQf != "0")
          {
            apiResult.code = ComConstant.ERROR_CODE;
            apiResult.data = "当前品番" + partId + "在包装基础数据表中入荷区分为不可入荷,请检查";
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


          }













        }
        else if (validateDt.Rows.Count > 0)
        {//数据重复
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "看板订单号" + kanbanOrderNo + "看板连番" + kanbanSerial + "品番" + partId + "已经入库,请检查";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        else if (validateData.Rows.Count != 1 || validateData1.Rows.Count != 1 || getInOutFlag.Rows.Count != 1)
        {//品番不存在或者有多条数据
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "品番" + partId + "在品番基础数据中不存在或在当前日期存在多条有效数据,请检查";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        else if (validateOrd.Rows.Count == 0)
        {//没有有效订单
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "品番" + partId + "在订单基础数据中没有有效订单,请检查";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        else if (getCheckType.Rows.Count != 1)
        {
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "品番" + partId + "在检查区分表不存在或在当前日期存在多条有效数据,请检查";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        else if (getCheckType.Rows.Count != 1)
        {
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "品番" + partId + "在检查区分表不存在或在当前日期存在多条有效数据,请检查";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        else if (getPackItem.Rows.Count == 0)
        {
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "品番" + partId + "包材构成表中不存在有效数据,请检查";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }

        else if (ValidatePrice.Rows.Count != 1)
        {
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "品番" + partId + "在销售价格表中不存在有效或在当前日期存在多条有效数据,请检查";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        else if (ValidateQF.Rows.Count != 1)
        {
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "品番" + partId + "在包装基础数据表中不存在有效或在当前日期存在多条有效数据,请检查";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


        }
        */

      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "更新实绩情报表失败!";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

      }
      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
    }














  }

}
