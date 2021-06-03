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
using Microsoft.AspNetCore.StaticFiles;
using System.IO;

namespace SPPSApi.Controllers.P01
{
  [Route("api/P00002/[action]")]
  [EnableCors("any")]
  [ApiController]
  public class P00002Controller : BaseController
  {
    P00002_Logic P00002_Logic = new P00002_Logic();
    P00002_DataEntity P00002_DataEntity = new P00002_DataEntity();
    private readonly string FunctionID = "P00002";
    private readonly IWebHostEnvironment _webHostEnvironment;


    #region 根据入库单号荷数量判断是否全部检查完毕

    [HttpPost]
    [EnableCors("any")]
    public string GetInnoApi([FromBody] dynamic data)
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
        string inno = dataForm.Inno == null ? "" : dataForm.Inno;//入库指令书号

        string scanTime = dataForm.ScanTime == null ? "" : dataForm.ScanTime;//客户端时间
        DataTable validateOpr1 = P00001_Logic.ValidateOpr1(inno);
        DataTable validateOpr2 = P00001_Logic.ValidateOpr2(inno);
        if (validateOpr1.Rows.Count == 1 && validateOpr2.Rows.Count == 0)
        {
          string partId = validateOpr1.Rows[0][0].ToString();
          string kanbanOrderNo = validateOpr1.Rows[0][2].ToString();
          string kanbanSerial = validateOpr1.Rows[0][3].ToString();
          string dock = validateOpr1.Rows[0][1].ToString();
          string supplierId = validateOpr1.Rows[0][5].ToString();
          string quantity = validateOpr1.Rows[0][4].ToString();
          DataTable getCheckType = P00002_Logic.GetCheckType(partId, kanbanOrderNo, kanbanSerial, dock, scanTime);
          DataTable getSPIS = P00002_Logic.GetSPIS(partId, scanTime, supplierId);
          if (getCheckType.Rows.Count == 1 && getSPIS.Rows.Count == 1)
          {
            string checkType = getCheckType.Rows[0][0].ToString();
            string tjsx = getCheckType.Rows[0][1].ToString();
            P00002_DataEntity.checkType = checkType;
            P00002_DataEntity.tjsx = tjsx;
            P00002_DataEntity.partId = partId;
            P00002_DataEntity.dock = dock;
            P00002_DataEntity.kanbanOrderNo = kanbanOrderNo;
            P00002_DataEntity.kanbanSerial = kanbanSerial;
            P00002_DataEntity.quantity = quantity;

            apiResult.data = P00002_DataEntity;





          }
          else if (getCheckType.Rows.Count != 1)
          {

            apiResult.code = ComConstant.ERROR_CODE;
            apiResult.data = "品番" + partId + "在检查区分表中没有有效数据";
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
          }
          else if (getSPIS.Rows.Count != 1)
          {
            apiResult.code = ComConstant.ERROR_CODE;
            apiResult.data = "品番" + partId + "在检查法中没有有效数据";
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

          }







        }
        else if (validateOpr1.Rows.Count != 1)
        {
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "入库单号" + inno + "没有进行入荷作业";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

        }
        else if (validateOpr2.Rows.Count > 0)
        {
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "入库单号" + inno + "已经进行检查作业";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

        }




      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "获取入库信息失败";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }


      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
    }







    #endregion





    #region 根据入库单号获得数据

    [HttpPost]
    [EnableCors("any")]
    public string GetInputInfoApi([FromBody] dynamic data)
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
        string inno = dataForm.Inno == null ? "" : dataForm.Inno;//入库指令书号
        DataTable getInnoData = P00002_Logic.GetInnoData(inno);
        if (getInnoData.Rows.Count == 1)
        {
          #region 构造返回数据
          P00002_DataEntity.partId = getInnoData.Rows[0][0].ToString();
          P00002_DataEntity.dock = getInnoData.Rows[0][1].ToString();
          P00002_DataEntity.quantity = getInnoData.Rows[0][2].ToString();
          P00002_DataEntity.kanbanOrderNo = getInnoData.Rows[0][3].ToString();
          P00002_DataEntity.kanbanSerial = getInnoData.Rows[0][4].ToString();
          apiResult.data = P00002_DataEntity;
          #endregion






        }
        else
        {

          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "入库单号" + inno + "在作业实绩情报表中没有或有多条入库数据,请检查!";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

        }







      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "获取品番信息失败";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }


      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
    }





    #endregion



    //SendNGDataApi
    #region 
    [HttpPost]
    [EnableCors("any")]
    public string SendNGDataApi([FromBody] dynamic data)
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
        string quantity = dataForm.Quantity == null ? "" : dataForm.Quantity;//数量
        string dock = dataForm.Dock == null ? "" : dataForm.Dock;//受入
        string kanbanOrderNo = dataForm.KanbanOrderNo == null ? "" : dataForm.KanbanOrderNo;//看板订单号
        string kanbanSerial = dataForm.KanbanSerial == null ? "" : dataForm.KanbanSerial;//看板连番
        string scanTime = dataForm.ScanTime == null ? "" : dataForm.ScanTime;//客户端时间
        string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
        string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
        string value = dataForm.value == null ? "" : dataForm.value;//检查结果
        string ngBlame = dataForm.NGBlame == null ? "" : dataForm.NGBlame;
        string ngReason = dataForm.NGReason == null ? "" : dataForm.NGReason;
        string ngQuantity = dataForm.NGQuantity == null ? "" : dataForm.NGQuantity;



        DataTable validateOpr1 = P00002_Logic.ValidateOpr1(partId, quantity, dock, kanbanOrderNo, kanbanSerial);//修改逻辑1,已经进行检查过的总数
                                                                                                                //   DataTable getInputQuantity = P00002_Logic.GetInputQuantity(partId,kanbanOrderNo,kanbanSerial,dock);//入库数量






        DataTable validateOpr = P00002_Logic.ValidateOpr(partId, quantity, dock, kanbanOrderNo, kanbanSerial);

        if (validateOpr.Rows.Count == 1 && validateOpr1.Rows.Count == 0)







        {
          string packingSpot = validateOpr.Rows[0][0].ToString();//包装场
          string inputNo = validateOpr.Rows[0][1].ToString();//入库单号
          string inOutFlag = validateOpr.Rows[0][2].ToString();//内外区分
          string supplierId = validateOpr.Rows[0][3].ToString();//供应商代码
          string supplierPlant = validateOpr.Rows[0][4].ToString();//供应商工区
          string packingQuantity = validateOpr.Rows[0][5].ToString();//包装单位

          string cpdCompany = validateOpr.Rows[0][6].ToString();//收货方
                                                                // string checkType = validateOpr.Rows[0][7].ToString();//检查区分

          DataTable getCheckType = P00002_Logic.GetCheckType(partId, kanbanOrderNo, kanbanSerial, dock, scanTime);
          string lblStart = validateOpr.Rows[0][8].ToString();//标签开始
          string lblEnd = validateOpr.Rows[0][9].ToString();//标签结束
                                                            //checkType如果是抽检获取检查个数
          if (getCheckType.Rows.Count == 1)
          {
            string checkType = getCheckType.Rows[0][0].ToString();
            if (checkType == "抽检")
            {
              DataTable getCheckQuantity = P00002_Logic.GetCheckQuantity(quantity);
              if (getCheckQuantity.Rows.Count == 1)
              {
                string checkQuantity = getCheckQuantity.Rows[0][0].ToString();
                int sjReultIn = P00002_Logic.InsertOpr(packingSpot, inputNo, kanbanOrderNo, kanbanSerial, partId, inOutFlag, supplierId, supplierPlant, scanTime, serverTime, quantity, packingQuantity, cpdCompany, dock, checkType, lblStart, lblEnd, opearteId, value, ngQuantity, checkQuantity);
                int ngResultIn = P00002_Logic.InsertNG(partId, kanbanOrderNo, kanbanSerial, dock, quantity, ngReason, ngBlame, opearteId, serverTime, ngQuantity);
                if (sjReultIn == 1)
                {
                  #region   构造返回数据
                  P00002_DataEntity.result = "NG成功";
                  apiResult.data = P00002_DataEntity;

                  #endregion
                }





              }
              else
              {

                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "当前品番" + partId + "在检查频度表中不存在有效数据,请检查";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


              }





            }
            else
            {
              #region 原检查方法
              int sjReultIn = P00002_Logic.InsertOpr(packingSpot, inputNo, kanbanOrderNo, kanbanSerial, partId, inOutFlag, supplierId, supplierPlant, scanTime, serverTime, quantity, packingQuantity, cpdCompany, dock, checkType, lblStart, lblEnd, opearteId, value, ngQuantity);
              int ngResultIn = P00002_Logic.InsertNG(partId, kanbanOrderNo, kanbanSerial, dock, quantity, ngReason, ngBlame, opearteId, serverTime, ngQuantity);
              if (sjReultIn == 1)
              {
                #region   构造返回数据
                P00002_DataEntity.result = "NG成功";
                apiResult.data = P00002_DataEntity;

                #endregion
              }

              #endregion



            }





          }
          else
          {
            apiResult.code = ComConstant.ERROR_CODE;
            apiResult.data = "当前品番" + partId + "检查区分表中不存在有效数据,请检查";
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

          }












        }
        else if (validateOpr.Rows.Count != 1)
        {
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "当前品番" + partId + "没有进行入荷作业";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

        }
        else if (validateOpr1.Rows.Count > 0)
        {
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "当前品番" + partId + "已经进行检查作业";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


        }


      }
      catch (Exception ex)
      {
        //ComMessage.GetInstance().ProcessMessage(FunctionID, "", ex, "system");
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "检查失败";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }

      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

    }
    #endregion





    #region  获得用户信息

    [HttpPost]
    [EnableCors("any")]
    public string GetUserInfoApi([FromBody] dynamic data)
    {
      string strToken = Request.Headers["X-Token"];
      if (!isLogin(strToken))
      {
        return error_login();
      }
      LoginInfo loginInfo = getLoginByToken(strToken);
      string opearteId = loginInfo.UserId;
      string banZhi = loginInfo.BanZhi;
      string userName = loginInfo.UserName;
      ApiResult apiResult = new ApiResult();
      try
      {

        P00002_DataEntity.userName = userName;
        P00002_DataEntity.banZhi = banZhi;
        apiResult.data = P00002_DataEntity;



      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "获取用户信息失败!";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }



      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
    }











    #endregion



    #region  获得检查类型
    [HttpPost]
    [EnableCors("any")]
    public string GetCheckApi([FromBody] dynamic data)
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
        string quantity = dataForm.Quantity == null ? "" : dataForm.Quantity;//数量
        string dock = dataForm.Dock == null ? "" : dataForm.Dock;//受入
        string kanbanOrderNo = dataForm.KanbanOrderNo == null ? "" : dataForm.KanbanOrderNo;//看板订单号
        string kanbanSerial = dataForm.KanbanSerial == null ? "" : dataForm.KanbanSerial;//看板连番
        string scanTime = dataForm.ScanTime == null ? "" : dataForm.ScanTime;//客户端时间
                                                                             //DataTable validateData = P00002_Logic.ValidateData(partId, scanTime, dock);
        DataTable validateOpr1 = P00002_Logic.ValidateOpr1(partId, quantity, dock, kanbanOrderNo, kanbanSerial);//修改逻辑2
        DataTable validateOpr = P00002_Logic.ValidateOpr(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
        //DataTable getInputQuantity = P00002_Logic.GetInputQuantity(partId, kanbanOrderNo, kanbanSerial, dock);//入库数量

        DataTable getSupplier = P00002_Logic.GetSupplier(partId, scanTime);
        
        DataTable validateData = P00002_Logic.ValidateData(partId, scanTime);


        DataTable getCheckType = P00002_Logic.GetCheckType(partId, kanbanOrderNo, kanbanSerial, dock, scanTime);


        if (getCheckType.Rows.Count == 1 && getSupplier.Rows.Count == 1 && validateOpr.Rows.Count == 1 && validateOpr1.Rows.Count == 0  && validateData.Rows.Count == 1)
        {

          string supplierId = getSupplier.Rows[0][0].ToString();
          DataTable getSPIS = P00002_Logic.GetSPIS(partId, scanTime, supplierId);
          if (getSPIS.Rows.Count == 1)
          {
            string parts = getSPIS.Rows[0][0].ToString();
            string tjsx = getCheckType.Rows[0][1].ToString();

            string quantity1 = validateOpr.Rows[0][10].ToString();

            string checkType = getCheckType.Rows[0][0].ToString();
            if (checkType=="抽检") {
              DataTable getCheckQuantity = P00002_Logic.GetCheckQuantity(quantity);
              if (getCheckQuantity.Rows.Count != 1) {

                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "当前品番" + partId + "在检查频度表中不存在有效数据,请检查";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

              }



            }


            
            #region 构造返回数据
            P00002_DataEntity.quantity = quantity1;
            P00002_DataEntity.tjsx = tjsx;
            P00002_DataEntity.checkType = checkType;
            P00002_DataEntity.spispath = getSPIS.Rows[0][0].ToString();
            apiResult.data = P00002_DataEntity;
            #endregion
          }
          else
          {


            apiResult.code = ComConstant.ERROR_CODE;
            apiResult.data = "当前品番" + partId + "在检查法中没有有效数据,请检查";
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


          }












        }
       

        else if (getCheckType.Rows.Count != 1)
        {
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "当前品番" + partId + "在检查区分表中没有有效数据,请检查";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


        }
        else if (validateData.Rows.Count != 1)
        {

          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "当前品番" + partId + "在品番基础数据中没有有效数据,请检查";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


        }
        else if (validateOpr1.Rows.Count != 1)
        {
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "当前品番" + partId + "没有进行入荷作业";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

        }
        else if (validateOpr.Rows.Count > 0)
        {
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "当前品番" + partId + "已经进行检查作业";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

        }
















      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "获取检查类型失败";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }
      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

    }






    #endregion

    #region 获取NG信息

    [HttpPost]
    [EnableCors("any")]
    public string GetNGInfoApi([FromBody] dynamic data)
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
        DataTable getNGReason = P00002_Logic.GetNGReason();

        DataTable getNGBlame = P00002_Logic.GetNGBlame();

        if (getNGReason.Rows.Count > 0 && getNGBlame.Rows.Count > 0)
        {
          ArrayList ngReasonList = new ArrayList();
          ArrayList ngBlameList = new ArrayList();
          for (int i = 0; i < getNGReason.Rows.Count; i++)
          {
            ngReasonList.Add(getNGReason.Rows[i][0].ToString());
          }
          for (int j = 0; j < getNGBlame.Rows.Count; j++)
          {
            ngBlameList.Add(getNGBlame.Rows[j][0].ToString());

          }





          P00002_DataEntity.ngBlame = ngBlameList;
          P00002_DataEntity.ngReason = ngReasonList;
          apiResult.data = P00002_DataEntity;





        }
        else
        {
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "NG信息或责任部署没有维护!";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);



        }

      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "获取NG信息列表失败!";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }



      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
    }






    #endregion



    #region 检查
    [HttpPost]
    [EnableCors("any")]
    public string ValidateApi([FromBody] dynamic data)
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
        string quantity = dataForm.Quantity == null ? "" : dataForm.Quantity;//数量
        string dock = dataForm.Dock == null ? "" : dataForm.Dock;//受入
        string kanbanOrderNo = dataForm.KanbanOrderNo == null ? "" : dataForm.KanbanOrderNo;//看板订单号
        string kanbanSerial = dataForm.KanbanSerial == null ? "" : dataForm.KanbanSerial;//看板连番
        string scanTime = dataForm.ScanTime == null ? "" : dataForm.ScanTime;//客户端时间
        string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
        string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
        string value = dataForm.value == null ? "" : dataForm.value;//检查结果
                                                                    //CheckType
        string checkType = dataForm.CheckType == null ? "" : dataForm.CheckType;//检查结果
        DataTable validateOpr1 = P00002_Logic.ValidateOpr1(partId, quantity, dock, kanbanOrderNo, kanbanSerial);//修改逻辑3
        DataTable validateOpr = P00002_Logic.ValidateOpr(partId, quantity, dock, kanbanOrderNo, kanbanSerial);

        // DataTable validateOpr = P00002_Logic.ValidateOpr(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
        //  DataTable getInputQuantity = P00002_Logic.GetInputQuantity(partId, kanbanOrderNo, kanbanSerial, dock);//入库数量

        if (validateOpr.Rows.Count == 1 && validateOpr1.Rows.Count == 0)
        {
          string packingSpot = validateOpr.Rows[0][0].ToString();//包装场
          string inputNo = validateOpr.Rows[0][1].ToString();//入库单号
          string inOutFlag = validateOpr.Rows[0][2].ToString();//内外区分
          string supplierId = validateOpr.Rows[0][3].ToString();//供应商代码
          string supplierPlant = validateOpr.Rows[0][4].ToString();//供应商工区
          string packingQuantity = validateOpr.Rows[0][5].ToString();//包装单位

          string cpdCompany = validateOpr.Rows[0][6].ToString();//收货方
          //string checkType = validateOpr.Rows[0][7].ToString();//检查区分
          string lblStart = validateOpr.Rows[0][8].ToString();//标签开始
          string lblEnd = validateOpr.Rows[0][9].ToString();//标签结束


          if (checkType == "抽检")
          {
            DataTable getCheckQuantity = P00002_Logic.GetCheckQuantity(quantity);
            if (getCheckQuantity.Rows.Count == 1)
            {
              string checkQuantity = getCheckQuantity.Rows[0][0].ToString();

              int sjReultIn = P00002_Logic.InsertOpr1(packingSpot, inputNo, kanbanOrderNo, kanbanSerial, partId, inOutFlag, supplierId, supplierPlant, scanTime, serverTime, quantity, packingQuantity, cpdCompany, dock, checkType, lblStart, lblEnd, opearteId, value, checkQuantity);

              if (sjReultIn == 1)
              {
                #region   构造返回数据
                P00002_DataEntity.result = "检查成功";
                apiResult.data = P00002_DataEntity;

                #endregion
              }




            }
            else
            {

              apiResult.code = ComConstant.ERROR_CODE;
              apiResult.data = "当前品番" + partId + "在检查频度表中不存在有效数据,请检查";
              return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


            }



          }
          else
          {
            int sjReultIn = P00002_Logic.InsertOpr(packingSpot, inputNo, kanbanOrderNo, kanbanSerial, partId, inOutFlag, supplierId, supplierPlant, scanTime, serverTime, quantity, packingQuantity, cpdCompany, dock, checkType, lblStart, lblEnd, opearteId, value);

            if (sjReultIn == 1)
            {
              #region   构造返回数据
              P00002_DataEntity.result = "检查成功";
              apiResult.data = P00002_DataEntity;

              #endregion
            }





          }


















        }
        else if (validateOpr.Rows.Count != 1)
        {
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "当前品番" + partId + "在没有入荷作业";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

        }
        else if (validateOpr1.Rows.Count > 0)
        {

          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "当前品番" + partId + "已经进行检查作业";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

        }

      }
      catch (Exception ex)
      {
        //ComMessage.GetInstance().ProcessMessage(FunctionID, "", ex, "system");
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "检查失败";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }

      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

    }
    #endregion

    [HttpGet]
    [EnableCors("any")]
    public IActionResult getSPISImageList(string path)
    {
      try
      {
        string fileSavePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Image" + Path.DirectorySeparatorChar + "PackingOper" + Path.DirectorySeparatorChar;
        var provider = new FileExtensionContentTypeProvider();
        FileInfo fileInfo = new FileInfo(fileSavePath + path);
        var ext = fileInfo.Extension;
        new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out var contenttype);
        byte[] bt = System.IO.File.ReadAllBytes(fileSavePath + path);
        return File(bt, contenttype ?? "image/Jpeg", fileInfo.Name);
      }
      catch (Exception ex)
      {
        ContentResult result = new ContentResult();
        result.Content = "<script>alert('导出失败,没有找到要导出的图片！')</script>";
        result.ContentType = "text/html;charset=utf-8";
        ComMessage.GetInstance().ProcessMessage("getImage", "M00UE0008", ex, "system");
        return result;
      }
    }


  }

}
