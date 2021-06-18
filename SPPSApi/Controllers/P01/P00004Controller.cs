using System;
using System.Data;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DataEntity;


namespace SPPSApi.Controllers.P01
{
  [Route("api/P00004/[action]")]
  [EnableCors("any")]
  [ApiController]
  public class P00004Controller : BaseController
  {
    P00004_Logic P00004_Logic = new P00004_Logic();
    P00004_DataEntity P00004_DataEntity = new P00004_DataEntity();
    private readonly string FunctionID = "P00004";


    #region 验证当前用户和设备是否有绑定的受入和叉车
    //ValidateUserApi
    public string ValidateUserApi([FromBody] dynamic data)
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
        DataTable getDockInfo = P00004_Logic.GetDockInfo(opearteId);
        if (getDockInfo.Rows.Count == 1)
        {
          P00004_DataEntity.fork = getDockInfo.Rows[0][0].ToString();
          P00004_DataEntity.dockSell = getDockInfo.Rows[0][1].ToString();
          P00004_DataEntity.getDockResult = "yes";
          apiResult.data = P00004_DataEntity;

        }
        else if (getDockInfo.Rows.Count == 0)
        {
          P00004_DataEntity.getDockResult = "no";
          apiResult.data = P00004_DataEntity;
        }
        else
        {
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "当前用户绑定多个DOCK,请检查!";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

        }
      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "获取信息失败!";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }


      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

    }









    #endregion





    #region 解除绑定
    //DelBindApi
    public string DelBindApi([FromBody] dynamic data)
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
        string fork = dataForm.Fork == null ? "" : dataForm.Fork;
        string dock = dataForm.Dock == null ? "" : dataForm.Dock;
        string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
        DataTable getData = P00004_Logic.GetData(dock, fork);
        //解绑之前需要判断当前叉车是否为dock的最后一辆


        /*
        if (getData.Rows.Count == 1) {
          int dockResultUp = P00004_Logic.UpdateDock2(dock, fork, opearteId, serverTime);
        } else {
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "当前受入表中没有绑定数据!";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }

        */

        DataTable validateDock1 = P00004_Logic.ValidateDock1(dock);
        DataTable validateShip = P00004_Logic.ValidateShip(fork, dock);
        if (validateDock1.Rows.Count == 1 && validateShip.Rows.Count > 0)
        {
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "当前叉车为" + dock + "最后一台叉车,不能解绑!";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }

        else
        {
          int dockResultUp1 = P00004_Logic.UpdateDock1(dock, fork, serverTime, opearteId);


        }








      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "解除绑定失败!";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }


      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

    }














    #endregion

    #region 发送邮件
    //SendMailApi
    public string SendMailApi([FromBody] dynamic data)
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
        P00004_Logic.SendMail();







      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "邮件发送失败!";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }


      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

    }








    #endregion




    #region  发货0315
    public string SendSellDataApi([FromBody] dynamic data)
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

        string truckNo = dataForm.TruckNo == null ? "" : dataForm.TruckNo;
        string bian = dataForm.Bian == null ? "" : dataForm.Bian;
        string bPQuantity = dataForm.BPQuantity == null ? "" : dataForm.BPQuantity;
        string cBQuantity = dataForm.CBQuantity == null ? "" : dataForm.CBQuantity;
        string hUQuantity = dataForm.HUQuantity == null ? "" : dataForm.HUQuantity;
        string hUQuantity1 = dataForm.HUQuantity1 == null ? "" : dataForm.HUQuantity1;
        string pCQuantity = dataForm.PCQuantity == null ? "" : dataForm.PCQuantity;
        string dockSell = dataForm.Dock == null ? "" : dataForm.Dock;
        string qianFen = dataForm.QianFen == null ? "" : dataForm.QianFen;
        string scanTime = dataForm.ScanTime == null ? "" : dataForm.ScanTime;//客户端时间
        string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
        string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
        //1.验证用户填写的箱数是否等于临时表中的箱数,不等于报错
        int caseSum = int.Parse(bPQuantity) + int.Parse(cBQuantity) + int.Parse(hUQuantity) + int.Parse(hUQuantity1) + int.Parse(pCQuantity);
        DataTable getCaseSum = P00004_Logic.GetCaseSum(dockSell);
        int caseSum1 = getCaseSum.Rows.Count;
        string seqNo = "";
        string formatDate = serverTime.Substring(0, 10);
        string formatDate1 = serverTime.Substring(0, 10).Replace("-", "");
        string sellNo = "";
        string bianCi = "";





        if (caseSum == caseSum1)
        {
          for (int i = 0; i < getCaseSum.Rows.Count; i++)
          {



            string caseNo = getCaseSum.Rows[i][0].ToString().Split("*")[1];
            //2.拿到箱号之后取出入库指令号和数量,验证信息无误后放到临时表中,验证临时表中是否有相关信息
            DataTable getSellInfo = P00004_Logic.GetSellInfo(caseNo);
            DataTable getSellInfo1 = P00004_Logic.GetSellInfo1(caseNo);
            DataTable getSellInfo2 = P00004_Logic.GetSellInfo2(caseNo);
            if (getSellInfo.Rows.Count > 0 && getSellInfo1.Rows.Count == 0)
            {
              for (int j = 0; j < getSellInfo.Rows.Count; j++)
              {
                string inno = getSellInfo.Rows[j][0].ToString();//入库单号
                string quantity = getSellInfo.Rows[j][1].ToString();//数量
                DataTable getQBData = P00004_Logic.GetQBData(inno);//从实绩情报表获得数据

                if (getQBData.Rows.Count == 1)
                {

                  string trolley = getQBData.Rows[0][0].ToString(); //台车号

                  string partId = getQBData.Rows[0][1].ToString();//品番
                  string cpdCompany = getQBData.Rows[0][2].ToString();//收货方
                  string dock = getQBData.Rows[0][3].ToString();//受入

                  string kanbanOrderNo = getQBData.Rows[0][4].ToString();//看板订单号
                  string kanbanSerial = getQBData.Rows[0][5].ToString();//看板连番

                  string packingSpot = getQBData.Rows[0][6].ToString();//包装场
                  string packingQuatity = getQBData.Rows[0][7].ToString();//包装单位
                  string lblStart = getQBData.Rows[0][8].ToString();//标签开始
                  string lblEnd = getQBData.Rows[0][9].ToString();//标签结束
                  string supplierId = getQBData.Rows[0][10].ToString();//供应商代码
                  string supplierPlant = getQBData.Rows[0][11].ToString();//供应商工区
                  string lotId = getQBData.Rows[0][12].ToString();//段取指示号
                  string checkType = getQBData.Rows[0][13].ToString();//检查区分
                  string inoutFlag = getQBData.Rows[0][14].ToString();//内外区分
                  DataTable validateData = P00004_Logic.ValidateData(partId, scanTime, dock);//验证品番基础数据,获得收货方
                  DataTable validateInv = P00004_Logic.ValidateInv(partId, kanbanOrderNo, kanbanSerial);
                  DataTable validatePrice = P00004_Logic.ValidatePrice(partId, scanTime,cpdCompany,supplierId);//验证单价
                  #region 验证订单
                  DataTable validateOrd1 = P00004_Logic.ValidateOrd1(partId);//验证订单有效性 

                  DataTable getCount = P00004_Logic.GetCount(partId);


                  #endregion




                  if (validateData.Rows.Count == 1 && validatePrice.Rows.Count == 1 && validateInv.Rows.Count == 1 && (int.Parse(validateInv.Rows[0][0].ToString()) >= int.Parse(quantity)) && (int.Parse(validateOrd1.Rows[0][0].ToString()) - int.Parse(getCount.Rows[0][0].ToString()) >= int.Parse(quantity)))
                  {

                    int qbResultIn = P00004_Logic.Insert(trolley, partId, quantity, dock, kanbanOrderNo, kanbanSerial, scanTime, iP, serverTime, cpdCompany, inno, opearteId, packingSpot, packingQuatity, lblStart, lblEnd, supplierId, supplierPlant, lotId, inoutFlag, checkType, caseNo, dockSell);//插入实绩情报表

                  }
                  else if (validateData.Rows.Count != 1)
                  {
                    P00004_Logic.DelData(dockSell);
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "箱号" + caseNo + "中品番" + partId + "在品番基础数据中没有有效数据";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


                  }
                  else if (validatePrice.Rows.Count != 1)
                  {
                    P00004_Logic.DelData(dockSell);
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "箱号" + caseNo + "中品番" + partId + "在销售单价表中没有有效数据";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                  }
                  else if (validateInv.Rows.Count != 1)
                  {
                    P00004_Logic.DelData(dockSell);
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "箱号" + caseNo + "中品番" + partId + "在入出库履历表中没有有效数据";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                  }
                  else if (int.Parse(validateInv.Rows[0][0].ToString()) < int.Parse(quantity))
                  {

                    P00004_Logic.DelData(dockSell);
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "箱号" + caseNo + "中品番" + partId + "在入出库履历中的待出荷数小于当前数量";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


                  }
                  else if ((int.Parse(validateInv.Rows[0][0].ToString()) < int.Parse(quantity)) && (int.Parse(validateOrd1.Rows[0][0].ToString()) - int.Parse(getCount.Rows[0][0].ToString()) >= int.Parse(quantity)))
                  {
                    P00004_Logic.DelData(dockSell);
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "箱号" + caseNo + "中品番" + partId + "在订单表中的出库数小于当前数量";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


                  }
                }
                else
                {
                  P00004_Logic.DelData(dockSell);
                  apiResult.code = ComConstant.ERROR_CODE;
                  apiResult.data = "箱号" + caseNo + "中入库指令书" + inno + "在实绩情报表中有多条数据!";
                  return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
              }
            }
            else if (getSellInfo1.Rows.Count > 0 || getSellInfo2.Rows.Count > 0)
            {
              P00004_Logic.DelData(dockSell);
              apiResult.code = ComConstant.ERROR_CODE;
              apiResult.data = "箱号" + caseNo + "已经出货,请检查!";
              return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            else if (getSellInfo.Rows.Count == 0)
            {
              P00004_Logic.DelData(dockSell);
              apiResult.code = ComConstant.ERROR_CODE;
              apiResult.data = "没有需要更新的数据!";
              return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
          }
        }
        else
        {

          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "填写的箱数与实绩不匹配,当前" + dockSell + "共有" + caseSum1 + "箱";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

        }














        string bianCiSeqNo = "";




        //全部插入到实绩情报表之后执行更新操作
        DataTable getQBData1 = P00004_Logic.GetQBData1(dockSell);
        DataTable getBanZhi = P00004_Logic.GetBanZhi(serverTime);
        string date = "";
        string banzhi = "";
        if (getBanZhi.Rows.Count == 1)
        {
          date = getBanZhi.Rows[0][0].ToString();
          banzhi = getBanZhi.Rows[0][1].ToString();
        }
        else
        {

          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "获取班值失败!";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);



        }

        if (bian == "1")
        {
          bianCi = "cx";

        }
        else if (bian == "2")
        {

          bianCi = "wz";
        }





        if (bianCi == "cx")
        {
          string tmpString = "SHPH2";
          DataTable getSeqNo = P00004_Logic.GetSeqNo(tmpString, formatDate);
          if (getSeqNo.Rows.Count == 0)
          {
            int seqResultIn = P00004_Logic.InsertSeqNo(tmpString, formatDate);
          }
          getSeqNo = P00004_Logic.GetSeqNo(tmpString, formatDate);
          seqNo = getSeqNo.Rows[0][0].ToString();
          int seqNoNew = int.Parse(seqNo) + 1;
          int seqResultUp = P00004_Logic.UpdateSeqNo(seqNoNew, formatDate, tmpString);
          sellNo = "XS" + formatDate1 + "0" + seqNo.PadLeft(3, '0');
          int toolResult = P00004_Logic.InsertTool(sellNo, opearteId, scanTime, hUQuantity, hUQuantity1, bPQuantity, pCQuantity, cBQuantity, bianCi);
        }
        else if (bianCi == "wz")
        {
          string tmpString = "SHPH2";
          DataTable getSeqNo = P00004_Logic.GetSeqNo(tmpString, formatDate);
          if (getSeqNo.Rows.Count == 0)
          {
            int seqResultIn = P00004_Logic.InsertSeqNo(tmpString, formatDate);
          }
          getSeqNo = P00004_Logic.GetSeqNo(tmpString, formatDate);
          seqNo = getSeqNo.Rows[0][0].ToString();
          int seqNoNew = int.Parse(seqNo) + 1;
          int seqResultUp = P00004_Logic.UpdateSeqNo(seqNoNew, formatDate, tmpString);
          sellNo = "XS" + formatDate1 + "0" + seqNo.PadLeft(3, '0');
          int toolResult = P00004_Logic.InsertTool(sellNo, opearteId, scanTime, hUQuantity, hUQuantity1, bPQuantity, pCQuantity, cBQuantity, bianCi);

        }
        if (bianCi == "cx")
        {
          string tmpString = "SHPCX";

          DataTable getSeqNo = P00004_Logic.GetSeqNo(tmpString, formatDate);
          if (getSeqNo.Rows.Count == 0)
          {
            int seqResultIn = P00004_Logic.InsertSeqNo(tmpString, formatDate);
          }
          getSeqNo = P00004_Logic.GetSeqNo(tmpString, formatDate);
          bianCiSeqNo = getSeqNo.Rows[0][0].ToString();
          int seqNoNew = int.Parse(bianCiSeqNo) + 1;
          int seqResultUp = P00004_Logic.UpdateSeqNo(seqNoNew, formatDate, tmpString);
          int sumResultIn = P00004_Logic.InsertSum(bianCiSeqNo, sellNo, truckNo, caseSum, bianCi, opearteId, serverTime, date, banzhi, qianFen);






        }
        else if (bianCi == "wz")
        {
          string tmpString = "SHPWZ";
          DataTable getSeqNo = P00004_Logic.GetSeqNo(tmpString, formatDate);
          if (getSeqNo.Rows.Count == 0)
          {
            int seqResultIn = P00004_Logic.InsertSeqNo(tmpString, formatDate);
          }
          getSeqNo = P00004_Logic.GetSeqNo(tmpString, formatDate);
          bianCiSeqNo = getSeqNo.Rows[0][0].ToString();
          int seqNoNew = int.Parse(bianCiSeqNo) + 1;
          int seqResultUp = P00004_Logic.UpdateSeqNo(seqNoNew, formatDate, tmpString);


          int sumResultIn = P00004_Logic.InsertSum(bianCiSeqNo, sellNo, truckNo, caseSum, bianCi, opearteId, serverTime, date, banzhi, qianFen);



        }





        if (getQBData1.Rows.Count > 0)
        {
          for (int j = 0; j < getQBData1.Rows.Count; j++)
          {
            string trolley = getQBData1.Rows[j][0].ToString(); //台车号

            string partId = getQBData1.Rows[j][1].ToString();//品番
            string cpdCompany = getQBData1.Rows[j][2].ToString();//收货方
            string dock = getQBData1.Rows[j][3].ToString();//受入

            string kanbanOrderNo = getQBData1.Rows[j][4].ToString();//看板订单号
            string kanbanSerial = getQBData1.Rows[j][5].ToString();//看板连番

            string packingSpot = getQBData1.Rows[j][6].ToString();//包装场
            string packingQuatity = getQBData1.Rows[j][7].ToString();//包装单位
            string lblStart = getQBData1.Rows[j][8].ToString();//标签开始
            string lblEnd = getQBData1.Rows[j][9].ToString();//标签结束
            string supplierId = getQBData1.Rows[j][10].ToString();//供应商代码
            string supplierPlant = getQBData1.Rows[j][11].ToString();//供应商工区
            string lotId = getQBData1.Rows[j][12].ToString();//段取指示号
            string checkType = getQBData1.Rows[j][13].ToString();//检查区分
            string inoutFlag = getQBData1.Rows[j][14].ToString();//内外区分
            string caseNo = getQBData1.Rows[j][15].ToString();//箱号
            string inputNo = getQBData1.Rows[j][16].ToString();//入库单号
            DataTable getOprData = P00004_Logic.GetOprData(caseNo, inputNo);//验证作业实绩
            DataTable getPrice = P00004_Logic.ValidatePrice(partId, scanTime,cpdCompany,supplierId);
            DataTable validateData = P00004_Logic.ValidateData(partId, scanTime, dock);//验证品番基础数据,获得收货方
            DataTable validateInv = P00004_Logic.ValidateInv(partId, kanbanOrderNo, kanbanSerial);
            DataTable validateOrd1 = P00004_Logic.ValidateOrd1(partId);//验证订单有效性 
            DataTable validateOrd = P00004_Logic.ValiateOrd2(partId);//验证订单表
            DataTable getCount = P00004_Logic.GetCount(partId);
            //DataTable getPartsName = P00004_Logic.GetPartsName1(partId,scanTime);

            if (getOprData.Rows.Count > 0 && validateData.Rows.Count == 1 && validateInv.Rows.Count == 1)
            {
              string quantity = getOprData.Rows[0][0].ToString();//装箱数量
              string checkStatus = getOprData.Rows[0][1].ToString();//检查结果
              string partsNameEn = validateData.Rows[0][0].ToString();
              string partsNameCn = validateData.Rows[0][1].ToString();
              string price = getPrice.Rows[0][0].ToString();
              string invoiceNo = sellNo.Substring(4, 10);
              string dCH = validateInv.Rows[0][0].ToString();
              DataTable getPoint = P00004_Logic.GetPoint(iP);
              if (getPoint.Rows.Count != 1)
              {
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "当前点位信息异常，请检查！";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

              }
              string pointType = getPoint.Rows[0][0].ToString() + getPoint.Rows[0][1].ToString();




              if (int.Parse(dCH) >= int.Parse(quantity) && (int.Parse(validateInv.Rows[0][0].ToString()) >= int.Parse(quantity)) && (int.Parse(validateOrd1.Rows[0][0].ToString()) - int.Parse(getCount.Rows[0][0].ToString()) >= int.Parse(quantity)))
              {

                int sjReultIn = P00004_Logic.InsertSj(packingSpot, inputNo, kanbanOrderNo, kanbanSerial, partId, inoutFlag, supplierId, supplierPlant, scanTime, serverTime, quantity, packingQuatity, cpdCompany, dock, checkType, lblStart, lblEnd, opearteId, checkStatus, caseNo, sellNo,iP,pointType);

                int sellResultIn = P00004_Logic.InsertSell(bianCiSeqNo, sellNo, truckNo, cpdCompany, partId, kanbanOrderNo, kanbanSerial, invoiceNo, caseNo, partsNameEn, quantity, bianCi, opearteId, scanTime, supplierId, lblStart, lblEnd, price);


                int invResultUp = P00004_Logic.UpdateInv1(partId, kanbanOrderNo, kanbanSerial, quantity);


                int shpResulIn = P00004_Logic.InsertShip1(cpdCompany, packingSpot, sellNo, partId, invoiceNo, bianCiSeqNo, quantity, caseNo, partsNameEn, opearteId, iP, partsNameCn, price, serverTime, supplierId);








                //int mailResult = P00004_Logic.SendMail();
                int SumQuantity = int.Parse(quantity);
                int newSum = 0;
                #region  更新订单表
                for (int o = 0; o < validateOrd.Rows.Count; o++)
                {
                  string targetMonth = validateOrd.Rows[o][0].ToString();
                  string orderType = validateOrd.Rows[o][1].ToString();
                  string orderNo = validateOrd.Rows[o][2].ToString();
                  string seqNo1 = validateOrd.Rows[o][3].ToString();
                  int d1 = int.Parse(validateOrd.Rows[o][4].ToString());
                  int d2 = int.Parse(validateOrd.Rows[o][5].ToString());
                  int d3 = int.Parse(validateOrd.Rows[o][6].ToString());
                  int d4 = int.Parse(validateOrd.Rows[o][7].ToString());
                  int d5 = int.Parse(validateOrd.Rows[o][8].ToString());
                  int d6 = int.Parse(validateOrd.Rows[o][9].ToString());
                  int d7 = int.Parse(validateOrd.Rows[o][10].ToString());
                  int d8 = int.Parse(validateOrd.Rows[o][11].ToString());
                  int d9 = int.Parse(validateOrd.Rows[o][12].ToString());
                  int d10 = int.Parse(validateOrd.Rows[o][13].ToString());
                  int d11 = int.Parse(validateOrd.Rows[o][14].ToString());
                  int d12 = int.Parse(validateOrd.Rows[o][15].ToString());
                  int d13 = int.Parse(validateOrd.Rows[o][16].ToString());
                  int d14 = int.Parse(validateOrd.Rows[o][17].ToString());
                  int d15 = int.Parse(validateOrd.Rows[o][18].ToString());
                  int d16 = int.Parse(validateOrd.Rows[o][19].ToString());
                  int d17 = int.Parse(validateOrd.Rows[o][20].ToString());
                  int d18 = int.Parse(validateOrd.Rows[o][21].ToString());
                  int d19 = int.Parse(validateOrd.Rows[o][22].ToString());
                  int d20 = int.Parse(validateOrd.Rows[o][23].ToString());
                  int d21 = int.Parse(validateOrd.Rows[o][24].ToString());
                  int d22 = int.Parse(validateOrd.Rows[o][25].ToString());
                  int d23 = int.Parse(validateOrd.Rows[o][26].ToString());
                  int d24 = int.Parse(validateOrd.Rows[o][27].ToString());
                  int d25 = int.Parse(validateOrd.Rows[o][28].ToString());
                  int d26 = int.Parse(validateOrd.Rows[o][29].ToString());
                  int d27 = int.Parse(validateOrd.Rows[o][30].ToString());
                  int d28 = int.Parse(validateOrd.Rows[o][31].ToString());
                  int d29 = int.Parse(validateOrd.Rows[o][32].ToString());
                  int d30 = int.Parse(validateOrd.Rows[o][33].ToString());
                  int d31 = int.Parse(validateOrd.Rows[o][34].ToString());
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
                  int updateOrd = P00004_Logic.UpdateOrd(targetMonth, orderNo, seqNo1, newarray[0], newarray[1], newarray[2], newarray[3], newarray[4], newarray[5], newarray[6], newarray[7], newarray[8], newarray[9], newarray[10],
                    newarray[11], newarray[12], newarray[13], newarray[14], newarray[15], newarray[16], newarray[17], newarray[18], newarray[19], newarray[20], newarray[21], newarray[22], newarray[23],
                    newarray[24], newarray[25], newarray[26], newarray[27], newarray[28], newarray[29], newarray[30], newSum, partId);
                  newSum = 0;
                }






                #endregion









              }
              else if ((int.Parse(validateInv.Rows[0][0].ToString()) < int.Parse(quantity)) && (int.Parse(validateOrd1.Rows[0][0].ToString()) - int.Parse(getCount.Rows[0][0].ToString()) >= int.Parse(quantity)))
              {


                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "箱号" + caseNo + "中品番" + partId + "在订单表中的出库数小于当前数量";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


              }
              else
              {

                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "箱号" + caseNo + "中品番" + partId + "入出库履历中的待出荷数量大于当前数量";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);



              }










            }
            else if (getOprData.Rows.Count == 0)
            {
              apiResult.code = ComConstant.ERROR_CODE;
              apiResult.data = "箱号" + caseNo + "中品番" + partId + "在作业作业实绩表中没有有效的装箱数据!";
              return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }

            else if (validateData.Rows.Count != 1)
            {
              apiResult.code = ComConstant.ERROR_CODE;
              apiResult.data = "箱号" + caseNo + "中品番" + partId + "在品番基础数据表中没有有效的装箱数据!";
              return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }
            else if (validateInv.Rows.Count != 1)
            {

              apiResult.code = ComConstant.ERROR_CODE;
              apiResult.data = "箱号" + caseNo + "中品番" + partId + "在入出库履历表中没有有效数据";
              return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }











          }









        }
        else
        {

          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "没有需要更新的数据";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


        }



        #region 做成CSV文件
        DataTable getData = P00004_Logic.GetData1(sellNo);
        string path = "";
        if (getData.Rows.Count > 0)
        {




          string[] head = new string[] { };
          string[] field = new string[] { };
          string msg = string.Empty;

          head = new string[] { "inv_no", "inv_date", "part_no", "part_name", "case_no", "ord_no", "item_no", "dlr_no", "qty", "price" };
          field = new string[] { "inv_no", "inv_date", "part_no", "part_name", "case_no", "ord_no", "item_no", "dlr_no", "qty", "price" };

          path = P00004_Logic.DataTableToExcel(head, field, getData, ".", opearteId, "P00001", ref msg, sellNo);




        }

        //head, field, dt, ".", loginInfo.UserId, FunctionID,ref msg
        //string[] head, string[] field, DataTable dt, string rootPath, string fileName, string strUserId, string strFunctionName, ref string RetMsg







        #endregion







        #region 发送邮件到销售公司
        DataTable getDataInfo = P00004_Logic.GetDataInfo(sellNo, bianCi);


        //vcBianCi,vcTruckNo,iToolQuantity,vcYinQuType,vcBanZhi,vcDate

        DataTable getSellInfo3 = P00004_Logic.GetSellInfo3(sellNo, bianCi);

        if (getDataInfo.Rows.Count == 1 && getSellInfo3.Rows.Count > 0)
        {

          string bianci = getDataInfo.Rows[0][0].ToString();
          string truckNo1 = getDataInfo.Rows[0][1].ToString();
          string toolSum = getDataInfo.Rows[0][2].ToString();
          string yinQuType = getDataInfo.Rows[0][3].ToString();
          string banZhi = getDataInfo.Rows[0][4].ToString();
          string date1 = getDataInfo.Rows[0][5].ToString();
          string info = "";
          string yinQuType1 = "";
          if (yinQuType == "cx")
          {
            yinQuType1 = "成型";
          }
          else
          {


            yinQuType1 = "外注加钣金";

          }

          for (int i = 0; i < getSellInfo3.Rows.Count; i++)
          {

            info = info + getSellInfo3.Rows[i][0].ToString() + ":" + getSellInfo3.Rows[i][1].ToString() + "<br>";

          }


        

          string path1 = @"C:\Users\Administrator\Desktop\laowu 0531修改\FILE\" + path;
          string mail = "fqm_wufan@tftm.com.cn";
          string userName = "laowu";


          string emailBody = "FTMS各位相关同事:<br>大家好!<br>附件为销售数据,请查收!<br>发货日期:" + date1 + "<br>发货班值:" + banzhi + "<br>便次区分:" + yinQuType1 + "第" + bianci + "便<br>引取车牌照号:" + truckNo1 + "<br>合计数量" + toolSum + "个<br>器具明细:" + info + "<br>收货时请及时确认数量<br>以上";
          string subject = "发货";
          DataTable getCode = P00004_Logic.GetCode();
          if (getCode.Rows.Count > 0)
          {
            ComFunction.SendEmailInfo(mail, userName, emailBody, getCode, getCode, subject, path1, true);
          }




        }
        else
        {

          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "发货信息不存在,无法发送邮件";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);



        }






        #endregion


        int shpResultUp = P00004_Logic.UpdateShip(dockSell, opearteId, serverTime);
        int dockResultUp = P00004_Logic.UpdateDock(dockSell, opearteId, serverTime);
        int qbResultUp = P00004_Logic.UpdateQB(dockSell);
        int printResultIn = P00004_Logic.InsertPrint(opearteId, iP, serverTime, sellNo);

        if (printResultIn == 1)
        {
          P00004_DataEntity.shipResult = "发货成功";
          apiResult.data = P00004_DataEntity;



        }




      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "发货失败";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }


      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

    }





    #endregion





    #region 绑定受入与叉车信息

    public string BindDockApi([FromBody] dynamic data)
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
        string dock = dataForm.Dock == null ? "" : dataForm.Dock;
        string fork = dataForm.Fork == null ? "" : dataForm.Fork;
        string scanTime = dataForm.ScanTime == null ? "" : dataForm.ScanTime;
        DataTable validateDock = P00004_Logic.ValidateDock(dock, fork);//验证叉车信息,如果当前表中没有可以直接绑定,

        if (validateDock.Rows.Count == 0)
        {
          int dockResultIn = P00004_Logic.InsertDock(dock, fork, scanTime, opearteId);
        }
        else if (validateDock.Rows.Count == 1)
        {
          string oldDock = validateDock.Rows[0][0].ToString();//当前叉车号之前绑定的受入
          if (oldDock == dock)//如果之前绑定的受入就是当前受入,更新用户名和时间

          {
            int dockResultUp = P00004_Logic.UpdateDock(dock, fork, scanTime, opearteId);


          }
          else //如果有需要判断之前绑定的Dock叉车数,不能为零

          {

            apiResult.code = ComConstant.ERROR_CODE;
            apiResult.data = "当前叉车号" + fork + "已经绑定" + oldDock + "";
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            /*
            DataTable validateDock1 = P00004_Logic.ValidateDock1(oldDock);
            if (validateDock1.Rows.Count == 1)
            {
              apiResult.code = ComConstant.ERROR_CODE;
              apiResult.data = "当前叉车为" + oldDock + "最后一台叉车,不能解绑!";
              return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }

            else
            {
              int dockResultUp1 = P00004_Logic.UpdateDock1(dock, fork, scanTime, opearteId);


            }
            */
          }









        }
        else
        {

          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "当前叉车绑定了多个DOCK,请检查!";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);



        }




      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "system");
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "绑定受入信息失败";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }


      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

    }





    #endregion






    #region  获取出货器具信息

    public string GetToolInfoApi([FromBody] dynamic data)
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
        string sellNo = dataForm.SellNo == null ? "" : dataForm.SellNo;

        DataTable getToolInfo = P00004_Logic.GetToolInfo(sellNo);
        if (getToolInfo.Rows.Count == 5)
        {
          P00004_DataEntity.HUQuantity = getToolInfo.Rows[0][1].ToString();
          P00004_DataEntity.BPQuantity = getToolInfo.Rows[1][1].ToString();
          P00004_DataEntity.CBQuantity = getToolInfo.Rows[2][1].ToString();
          P00004_DataEntity.HUQuantity1 = getToolInfo.Rows[3][1].ToString();
          P00004_DataEntity.PCQuantity = getToolInfo.Rows[4][1].ToString();

          apiResult.data = P00004_DataEntity;




        }
        else
        {
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "没有找到对应销售单号的器具信息!";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }


      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "获取器具信息失败!";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }


      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

    }







    #endregion








    #region 获取出货便次明细

    public string GetSellDataApi([FromBody] dynamic data)
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
        string timeFrom = dataForm.Timefrom == null ? "" : dataForm.Timefrom;
        string timeEnd = dataForm.Timeend == null ? "" : dataForm.Timeend;
        string type = dataForm.Type == null ? "" : dataForm.Type;
        string time = dataForm.Time == null ? "" : dataForm.Time;
        //根据客户端时间判断班值
        DataTable getBanZhi = P00004_Logic.GetBanZhi(time);
        if (getBanZhi.Rows.Count == 1)
        {
          string date = getBanZhi.Rows[0][0].ToString();
          string banZhi = getBanZhi.Rows[0][1].ToString();


          DataTable GetSellData = P00004_Logic.GetSellData(timeFrom, timeEnd, type, date, banZhi);
          if (GetSellData.Rows.Count > 0)
          {
            P00004_DataEntity.sellList = GetSellData;
            apiResult.data = P00004_DataEntity;



          }
          else
          {
            apiResult.code = ComConstant.ERROR_CODE;
            apiResult.data = "当前班值没有出货数据,请检查!";
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
          }


        }
        else
        {
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "获取班值失败!";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }







      }
      catch (Exception ex)
      {
        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "获取出货便次明细失败";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }


      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

    }



    #endregion



    /*
      #region  发货
      public string ShipApi([FromBody] dynamic data)
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

        #region   校验当前临时表中是否为同一个工区
        DataTable getPlantdt = P00004_Logic.GetPlant();
        if (getPlantdt.Rows.Count == 1) {
          string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
          string plantCode = getPlantdt.Rows[0][0].ToString();//工场代码
          string packingSpot= getPlantdt.Rows[0][1].ToString();//包装场代码
          string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
          string formatServerTime = serverTime.Substring(0, 10).Replace("-", "");//格式化号口时间
          //生成销售单号
          string tmpString = "SHP";

          DataTable shpSeqNodt = P00004_Logic.ValidateSeqNo(packingSpot, formatServerTime, tmpString);
          if (shpSeqNodt.Rows.Count==0) {//如果当前没有插入信息
            int shpSeqNoIn = P00004_Logic.InsertSeqNo(packingSpot, formatServerTime, tmpString);//如果当天没有插入信息
            shpSeqNodt= P00004_Logic.ValidateSeqNo(packingSpot, formatServerTime, tmpString);

          }
       
          string shpSeqNo = shpSeqNodt.Rows[0][2].ToString();
          string shpSqnNoNew = (int.Parse(shpSeqNo) + 1).ToString();
          int shpResultUp = P00004_Logic.UpdateSeqNo(packingSpot, formatServerTime, shpSqnNoNew, tmpString);
       string formatTime= serverTime.Substring(2, 6).Replace("-", "");
          string sellNo = "XS20" +formatTime+plantCode + packingSpot.Substring(1, 1)+ shpSeqNo.PadLeft(4, '0');









          DataTable getShipData = P00004_Logic.GetShipData();
          for (int i = 0; i < getShipData.Rows.Count; i++)
          {
            string caseNo = getShipData.Rows[i][0].ToString();

            DataTable sjResult = P00004_Logic.ValidateOpr(caseNo);
            DataTable sjResult1 = P00004_Logic.ValidateOpr1(caseNo);

            //DataTable qbResult = P00004_Logic.ValidateQB(caseNo);
           // DataTable qbResult1 = P00004_Logic.ValidateQB1(caseNo);
            if (sjResult.Rows.Count > 0 && sjResult1.Rows.Count == 0)
            {

              for (int j = 0; j < sjResult.Rows.Count; j++)//在循环结束之后需要删除该箱号信息
              {
             



                string inoNo = sjResult.Rows[j][0].ToString();//入库单号
                string partId = sjResult.Rows[j][1].ToString();//品番
                string cpdCompany = sjResult.Rows[j][2].ToString();//收货方
                string dock = sjResult.Rows[j][3].ToString();//受入
                string quantity = sjResult.Rows[j][4].ToString();//数量
           
                string kanbanOrderNo = sjResult.Rows[j][5].ToString();//看板订单号
                string kanbanSerial = sjResult.Rows[j][6].ToString();//看板连番

                string inoutFlag = sjResult.Rows[j][7].ToString();//看板连番
                string supplier_id = sjResult.Rows[j][8].ToString();//看板连番
                string supplierGQ = sjResult.Rows[j][9].ToString();//看板连番
                string bZUnit = sjResult.Rows[j][10].ToString();//看板连番
                string checkType = sjResult.Rows[j][11].ToString();//看板连番
                string labelStart = sjResult.Rows[j][12].ToString();//看板连番
                string labelEnd = sjResult.Rows[j][13].ToString();//看板连番
                string checkStatus = sjResult.Rows[j][14].ToString();//看板连番






                int SumQuantity = int.Parse(quantity);
                int newSum = 0;
               // DataTable validateOpr = P00004_Logic.ValidateOpr(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
                //从入出库履历取装箱数据
                DataTable validateInv = P00004_Logic.ValidateInv(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
                //校验品番的有效性
               // DataTable validateData = P00004_Logic.ValidateData(partId, serverTime, dock);

                DataTable validateOrd = P00004_Logic.ValidateOrd(partId);//验证订单有效性
                DataTable getPartsName = P00004_Logic.GetPartsName(serverTime, partId);
                int sumQty = 0;//补品总的可出库数量

                for (int n = 0; n < validateOrd.Rows.Count; n++)
                {

                  sumQty += int.Parse(validateOrd.Rows[n][4].ToString());
                }
                if (sjResult.Rows.Count > 0 && validateInv.Rows.Count == 1 && sumQty >= SumQuantity && getPartsName.Rows.Count == 1)
                {//可以出库

                  string bzPlant = validateInv.Rows[0][0].ToString();//包装场
                  string dBZ = validateInv.Rows[0][3].ToString();//待包装
                  string dZX = validateInv.Rows[0][4].ToString();//待装箱
                  string dCH = validateInv.Rows[0][5].ToString();//待出荷
                  string partsName = getPartsName.Rows[0][0].ToString();

                  //  DataTable rInvdt = P00004_Logic.ValidateRinv(bzPlant, partId, cpdCompany);


                  //int rinvResultUp = P00004_Logic.UpdateRinv(bzPlant, partId, cpdCompany, int.Parse(quantity));//更新在库履历
                  // int qbResultIn = P00004_Logic.Insert(partId, quantity, dock, kanbanOrderNo, kanbanSerial, serverTime,  cpdCompany, inoNo, caseNo);//插入实绩情报表
                  int invResult = P00004_Logic.UpdateInv(partId, quantity, dock, kanbanOrderNo, kanbanSerial, serverTime);                                                                                                                    //更新入出库履历表
                  int oprReusultIn = P00004_Logic.InsertOpr(bzPlant, inoNo, kanbanOrderNo, kanbanSerial, partId, inoutFlag, supplier_id, supplierGQ, serverTime, quantity, bZUnit, cpdCompany, dock, checkType, labelStart, labelEnd, caseNo, checkStatus, sellNo);//插入作业实际表

                  #region  更新订单表
                  for (int k = 0; k < validateOrd.Rows.Count; k++)
                  {
                    string targetMonth = validateOrd.Rows[k][0].ToString();
                    string orderType = validateOrd.Rows[k][1].ToString();
                    string orderNo = validateOrd.Rows[k][2].ToString();
                    string seqNo = validateOrd.Rows[k][3].ToString();
                    int d1 = int.Parse(validateOrd.Rows[k][4].ToString());
                    int d2 = int.Parse(validateOrd.Rows[k][5].ToString());
                    int d3 = int.Parse(validateOrd.Rows[k][6].ToString());
                    int d4 = int.Parse(validateOrd.Rows[k][7].ToString());
                    int d5 = int.Parse(validateOrd.Rows[k][8].ToString());
                    int d6 = int.Parse(validateOrd.Rows[k][9].ToString());
                    int d7 = int.Parse(validateOrd.Rows[k][10].ToString());
                    int d8 = int.Parse(validateOrd.Rows[k][11].ToString());
                    int d9 = int.Parse(validateOrd.Rows[k][12].ToString());
                    int d10 = int.Parse(validateOrd.Rows[k][13].ToString());
                    int d11 = int.Parse(validateOrd.Rows[k][14].ToString());
                    int d12 = int.Parse(validateOrd.Rows[k][15].ToString());
                    int d13 = int.Parse(validateOrd.Rows[k][16].ToString());
                    int d14 = int.Parse(validateOrd.Rows[k][17].ToString());
                    int d15 = int.Parse(validateOrd.Rows[k][18].ToString());
                    int d16 = int.Parse(validateOrd.Rows[k][19].ToString());
                    int d17 = int.Parse(validateOrd.Rows[k][20].ToString());
                    int d18 = int.Parse(validateOrd.Rows[k][21].ToString());
                    int d19 = int.Parse(validateOrd.Rows[k][22].ToString());
                    int d20 = int.Parse(validateOrd.Rows[k][23].ToString());
                    int d21 = int.Parse(validateOrd.Rows[k][24].ToString());
                    int d22 = int.Parse(validateOrd.Rows[k][25].ToString());
                    int d23 = int.Parse(validateOrd.Rows[k][26].ToString());
                    int d24 = int.Parse(validateOrd.Rows[k][27].ToString());
                    int d25 = int.Parse(validateOrd.Rows[k][28].ToString());
                    int d26 = int.Parse(validateOrd.Rows[k][29].ToString());
                    int d27 = int.Parse(validateOrd.Rows[k][30].ToString());
                    int d28 = int.Parse(validateOrd.Rows[k][31].ToString());
                    int d29 = int.Parse(validateOrd.Rows[k][32].ToString());
                    int d30 = int.Parse(validateOrd.Rows[k][33].ToString());
                    int d31 = int.Parse(validateOrd.Rows[k][34].ToString());
                    string cpdName = "一汽丰田";
                    int shpResultIn = P00004_Logic.InsertShip(cpdName, cpdCompany, packingSpot, sellNo, partId, orderNo, seqNo, quantity, caseNo,partsName,opearteId,iP);
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
                    for (int a = 0; a < newarray.Length; a++)
                    {
                      newSum += newarray[a];
                    }
                    int updateOrd = P00004_Logic.UpdateOrd(targetMonth, orderNo, seqNo, newarray[0], newarray[1], newarray[2], newarray[3], newarray[4], newarray[5], newarray[6], newarray[7], newarray[8], newarray[9], newarray[10],
                      newarray[11], newarray[12], newarray[13], newarray[14], newarray[15], newarray[16], newarray[17], newarray[18], newarray[19], newarray[20], newarray[21], newarray[22], newarray[23],
                      newarray[24], newarray[25], newarray[26], newarray[27], newarray[28], newarray[29], newarray[30], newSum, partId);
                    newSum = 0;
                  }

                  #endregion



                }
                else if (sjResult.Rows.Count == 0) {
                  apiResult.code = ComConstant.ERROR_CODE;
                  apiResult.data = "当前没有要更新的数据";
                  return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                } else if (validateInv.Rows.Count!=1) {
                  apiResult.code = ComConstant.ERROR_CODE;
                  apiResult.data = "品番"+partId+"在入出库履历中没有对应或存在多条数据,请检查!";
                  return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                else if (sumQty < SumQuantity)
                {
                  apiResult.code = ComConstant.ERROR_CODE;
                  apiResult.data = "品番" + partId + "在订单表中的计划数小于出荷数,请检查!";
                  return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                else if (getPartsName.Rows.Count != 1)
                {
                  apiResult.code = ComConstant.ERROR_CODE;
                  apiResult.data = "品番" + partId + "在品番基础数据中没有有效或存在多条数据,请检查!";
                  return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }






              }

              int printResultIn = P00004_Logic.InsertPrint(opearteId,iP,serverTime,sellNo);

            
              int trunResult = P00004_Logic.Truncate();
              if (trunResult == -1)
              {
                #region 构造返回数据
                P00004_DataEntity.result = "出库成功";
                apiResult.data = P00004_DataEntity;
                #endregion
              }




            }
            else {
              apiResult.code = ComConstant.ERROR_CODE;
              apiResult.data = "当前没有要更新的数据或者存在已经发货的数据";
              return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }

          }
        } else {
          apiResult.code = ComConstant.ERROR_CODE;
          apiResult.data = "当前没有要出货的数据或当前出货的品番不是一个工区";
          return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


        }

        #endregion


      }
      catch (Exception ex)
      {
          ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "出库失败";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }
      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

    }
















    #endregion

    */
    #region 删除箱号

    //DelCaseApi
    public string DelCaseApi([FromBody] dynamic data)
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
        string caseNo = dataForm.CaseNo == null ? "" : dataForm.CaseNo;
        int caseResultDel = P00004_Logic.DelCase(caseNo);




      }

      catch (Exception ex)
      {
        //ComMessage.GetInstance().ProcessMessage(FunctionID, "", ex, "system");
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "获取出货数据失败";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }

      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

    }














    #endregion









    #region  获得出货数据
    public string GetShipDataApi([FromBody] dynamic data)
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
        string dock = dataForm.Dock == null ? "" : dataForm.Dock;
        DataTable getShipData = P00004_Logic.GetShipData(dock);
        #region  构造返回数据
        P00004_DataEntity.sum = getShipData.Rows.Count.ToString();
        P00004_DataEntity.caseNo = getShipData;
        apiResult.data = P00004_DataEntity;

        #endregion




      }

      catch (Exception ex)
      {
        //ComMessage.GetInstance().ProcessMessage(FunctionID, "", ex, "system");
        apiResult.code = ComConstant.ERROR_CODE;
        apiResult.data = "获取出货数据失败";
        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
      }

      return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

    }


    #endregion












  }

}
