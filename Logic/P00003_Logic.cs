using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using System.Net;
using QRCoder;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace Logic
{
  public class P00003_Logic
  {
    static P00003_DataAccess P00003_DataAccess = new P00003_DataAccess();






    public DataTable ValidateOpr(string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial)
    {
      return P00003_DataAccess.ValidateOpr(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
    }

    public DataTable ValidateInv(string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial)
    {
      return P00003_DataAccess.ValidateInv(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
    }

    public static DataTable GetPackData(string partId, string scanTime)
    {
      return P00003_DataAccess.GetPackData(partId, scanTime);
    }

    public static DataTable ValidateCaseNo(string caseNo)
    {
      return P00003_DataAccess.ValidateCaseNo(caseNo);

    }

    public int UpdateCase1(string opearteId, string iP)
    {
      return P00003_DataAccess.UpdateCase1(opearteId, iP);
    }



    public DataTable GetCaseList(string iP, string caseNo)
    {
      return P00003_DataAccess.GetCaseList(iP, caseNo);
    }

    public DataTable GetCaseList(string opearteId)
    {
      return P00003_DataAccess.GetCaseList(opearteId);
    }

    public DataTable ValidateSJ(string partId, string dock, string kanbanOrderNo, string kanbanSerial)
    {
      return P00003_DataAccess.ValidateSJ(partId, dock, kanbanOrderNo, kanbanSerial);
    }

    public DataTable GetUserRole(string user)
    {
      return P00003_DataAccess.GetUserRole(user);
    }

    public DataTable ValidateSJ1(string partId, string dock, string kanbanOrderNo, string kanbanSerial)
    {
      return P00003_DataAccess.ValidateSJ1(partId, dock, kanbanOrderNo, kanbanSerial);
    }

    public DataTable GetPoint(string iP)
    {
      return P00003_DataAccess.GetPoint(iP);
    }

    public int UpdateEffi1(string pointNo, decimal effi)
    {
      return P00003_DataAccess.UpdateEffi1(pointNo, effi);
    }

    public static decimal[] getOperEfficacyInfo(string strPackPlant, string strOperater, string strPointNo)
    {
      try
      {
        decimal[] vs = new decimal[3];
        //获取当前班值信息
        DataTable dtBanZhiInfo = P00003_DataAccess.getBanZhiTime(strPackPlant, "2");
        if (dtBanZhiInfo == null || dtBanZhiInfo.Rows.Count == 0)
        {
          vs[0] = -1;//班值信息为空--报错显示
          return vs;
        }
        string strHosDate = dtBanZhiInfo.Rows[0]["dHosDate"].ToString();
        string strBanZhi = dtBanZhiInfo.Rows[0]["vcBanZhi"].ToString();
        string strFromTime_nw = dtBanZhiInfo.Rows[0]["tFromTime_nw"].ToString();

        //获取休息阶段、点位登录履历、操作人当日完成基准时间
        DataSet dsOperPointInfo = P00003_DataAccess.getOperPointInfo(strPackPlant, strBanZhi, strHosDate, strOperater, strFromTime_nw);
        if (dsOperPointInfo == null)
        {
          vs[0] = -2;//点位信息获取失败--报错显示
          return vs;
        }
        if (dsOperPointInfo.Tables[0].Rows.Count == 0)
        {
          vs[0] = -3;//当值休息时间获取失败--报错显示
          return vs;
        }
        if (dsOperPointInfo.Tables[1].Rows.Count == 0)
        {
          vs[0] = -4;//当值点位履历获取失败--报错显示
          return vs;
        }
        if (dsOperPointInfo.Tables[2].Rows.Count == 0)
        {
          vs[0] = -5;//操作人当日完成基准时间获取失败--报错显示
          return vs;
        }

        //点位完成基准时间（ss）
        decimal decOperStandard = Convert.ToDecimal(dsOperPointInfo.Tables[2].Rows[0]["decOperStandard"].ToString());
        //点位在线有效时间（ss）
        decimal decOnLine = getOnLineDetails(dsOperPointInfo.Tables[1], dsOperPointInfo.Tables[0]);
        decimal decOperEfficacy = 0;
        if (decOnLine > 0)
          decOperEfficacy = decOperStandard / decOnLine;
        vs[0] = 0;
        vs[1] = decOnLine;
        vs[2] = decOperEfficacy;
        return vs;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    private static decimal getOnLineDetails(DataTable dtPointDetails, DataTable dtRest)
    {
      try
      {
        DataTable dtPointDetails_Temp = dtPointDetails.Clone();
        dtPointDetails_Temp.Columns.Add("iOnLine");
        for (int i = 0; i < dtPointDetails.Rows.Count; i++)
        {
          DataTable dtRest_Temp = dtRest.Clone();
          string strHosDate = dtPointDetails.Rows[i]["dHosDate"].ToString();
          string strPackPlant = dtPointDetails.Rows[i]["vcPackPlant"].ToString();
          string strBanZhi = dtPointDetails.Rows[i]["vcBanZhi"].ToString();
          string strPointNo = dtPointDetails.Rows[i]["vcPointNo"].ToString();
          string strUUID = dtPointDetails.Rows[i]["UUID"].ToString();
          string strEntryTime = dtPointDetails.Rows[i]["dEntryTime"].ToString();
          string strDestroyTime = dtPointDetails.Rows[i]["dDestroyTime"].ToString();
          if (Convert.ToDateTime(strEntryTime) >= Convert.ToDateTime(strDestroyTime))
          {
            DataRow drPointDetails_Temp = dtPointDetails_Temp.NewRow();
            drPointDetails_Temp["dHosDate"] = strHosDate;
            drPointDetails_Temp["vcPackPlant"] = strPackPlant;
            drPointDetails_Temp["vcBanZhi"] = strBanZhi;
            drPointDetails_Temp["vcPointNo"] = strPointNo;
            drPointDetails_Temp["UUID"] = strUUID;
            drPointDetails_Temp["dEntryTime"] = "1900-01-01";
            drPointDetails_Temp["dDestroyTime"] = "1900-01-01";
            dtPointDetails_Temp.Rows.Add(drPointDetails_Temp);
          }
          else
          {
            //判断开始结束是否是在休息范围内
            DataRow[] drRest_00 = dtRest.Select("vcPackPlant='" + strPackPlant + "' and vcBanZhi='" + strBanZhi + "' and tBeforTime<='" + strEntryTime + "' and tLastTime>='" + strDestroyTime + "'");
            if (drRest_00.Length != 0)
            {
              DataRow drPointDetails_Temp = dtPointDetails_Temp.NewRow();
              drPointDetails_Temp["dHosDate"] = strHosDate;
              drPointDetails_Temp["vcPackPlant"] = strPackPlant;
              drPointDetails_Temp["vcBanZhi"] = strBanZhi;
              drPointDetails_Temp["vcPointNo"] = strPointNo;
              drPointDetails_Temp["UUID"] = strUUID;
              drPointDetails_Temp["dEntryTime"] = "1900-01-01";
              drPointDetails_Temp["dDestroyTime"] = "1900-01-01";
              dtPointDetails_Temp.Rows.Add(drPointDetails_Temp);
            }
            else
            {
              //判断开始在休息时间之间
              DataRow[] drRest_20 = dtRest.Select("vcPackPlant='" + strPackPlant + "' and vcBanZhi='" + strBanZhi + "' and tBeforTime<='" + strEntryTime + "' and tLastTime>='" + strEntryTime + "'");
              if (drRest_20.Length != 0)
              {
                strEntryTime = drRest_20[0]["tLastTime"].ToString();
              }
              //判断开始在休息时间之前
              DataRow[] drRest_10 = dtRest.Select("vcPackPlant='" + strPackPlant + "' and vcBanZhi='" + strBanZhi + "' and tBeforTime>='" + strEntryTime + "'");
              if (drRest_10.Length != 0)
              {
                for (int j = 0; j < drRest_10.Length; j++)
                {
                  DataRow drRest_Temp = dtRest_Temp.NewRow();
                  drRest_Temp["TANK"] = drRest_10[j]["TANK"];
                  drRest_Temp["vcPackPlant"] = drRest_10[j]["vcPackPlant"];
                  drRest_Temp["vcBanZhi"] = drRest_10[j]["vcBanZhi"];
                  drRest_Temp["tBeforTime"] = drRest_10[j]["tBeforTime"];
                  drRest_Temp["tLastTime"] = drRest_10[j]["tLastTime"];
                  drRest_Temp["iMinute"] = drRest_10[j]["iMinute"];
                  dtRest_Temp.Rows.Add(drRest_Temp);
                }
                dtRest_Temp.DefaultView.Sort = "TANK ASC";
                dtRest_Temp = dtRest_Temp.DefaultView.ToTable();
                int iTANK = Convert.ToInt32(dtRest_Temp.Rows[0]["TANK"].ToString());
                //判断结束在休息时间中
                DataRow[] drRest_11 = dtRest_Temp.Select("vcPackPlant='" + strPackPlant + "' and vcBanZhi='" + strBanZhi + "' and tBeforTime<='" + strDestroyTime + "' and tLastTime>='" + strDestroyTime + "'");
                if (drRest_11.Length != 0)
                {
                  int iTANK_11 = Convert.ToInt32(drRest_11[drRest_11.Length - 1]["TANK"].ToString());
                  for (int j = 0; j < iTANK_11 - iTANK + 1; j++)
                  {
                    string strEntryTime_11 = "";
                    string strDestroyTime_11 = "";
                    if (j == 0)
                      strEntryTime_11 = strEntryTime;
                    else
                      strEntryTime_11 = dtRest_Temp.Rows[j - 1]["tLastTime"].ToString();

                    if (j == 0)
                      strDestroyTime_11 = dtRest_Temp.Rows[j]["tBeforTime"].ToString();
                    //else
                    //if (iTANK_11 - iTANK + 1 - 1 == j)
                    //    strDestroyTime_11 = dtRest_Temp.Rows[j]["tBeforTime"].ToString();
                    else
                      strDestroyTime_11 = dtRest_Temp.Rows[j]["tBeforTime"].ToString();


                    DataRow drPointDetails_Temp = dtPointDetails_Temp.NewRow();
                    drPointDetails_Temp["dHosDate"] = strHosDate;
                    drPointDetails_Temp["vcPackPlant"] = strPackPlant;
                    drPointDetails_Temp["vcBanZhi"] = strBanZhi;
                    drPointDetails_Temp["vcPointNo"] = strPointNo;
                    drPointDetails_Temp["UUID"] = strUUID;
                    drPointDetails_Temp["dEntryTime"] = strEntryTime_11;
                    drPointDetails_Temp["dDestroyTime"] = strDestroyTime_11;
                    dtPointDetails_Temp.Rows.Add(drPointDetails_Temp);
                  }
                }
                else
                {
                  //判断结束在休息时间间
                  DataRow[] drRest_12 = dtRest_Temp.Select("vcPackPlant='" + strPackPlant + "' and vcBanZhi='" + strBanZhi + "' and tLastTime<'" + strDestroyTime + "'");
                  if (drRest_12.Length != 0)
                  {
                    for (int j = 0; j < drRest_12.Length; j++)
                    {
                      string strEntryTime_11 = "1900-01-01";
                      string strDestroyTime_11 = "1900-01-01";
                      if (j == 0)
                      {
                        strEntryTime_11 = strEntryTime;
                        strDestroyTime_11 = drRest_12[j]["tBeforTime"].ToString();
                        DataRow drPointDetails_Temp_1 = dtPointDetails_Temp.NewRow();
                        drPointDetails_Temp_1["dHosDate"] = strHosDate;
                        drPointDetails_Temp_1["vcPackPlant"] = strPackPlant;
                        drPointDetails_Temp_1["vcBanZhi"] = strBanZhi;
                        drPointDetails_Temp_1["vcPointNo"] = strPointNo;
                        drPointDetails_Temp_1["UUID"] = strUUID;
                        drPointDetails_Temp_1["dEntryTime"] = strEntryTime_11;
                        drPointDetails_Temp_1["dDestroyTime"] = strDestroyTime_11;
                        dtPointDetails_Temp.Rows.Add(drPointDetails_Temp_1);
                      }
                      if (j == drRest_12.Length - 1)
                      {
                        strEntryTime_11 = drRest_12[j]["tLastTime"].ToString();
                        strDestroyTime_11 = strDestroyTime;
                        DataRow drPointDetails_Temp_1 = dtPointDetails_Temp.NewRow();
                        drPointDetails_Temp_1["dHosDate"] = strHosDate;
                        drPointDetails_Temp_1["vcPackPlant"] = strPackPlant;
                        drPointDetails_Temp_1["vcBanZhi"] = strBanZhi;
                        drPointDetails_Temp_1["vcPointNo"] = strPointNo;
                        drPointDetails_Temp_1["UUID"] = strUUID;
                        drPointDetails_Temp_1["dEntryTime"] = strEntryTime_11;
                        drPointDetails_Temp_1["dDestroyTime"] = strDestroyTime_11;
                        dtPointDetails_Temp.Rows.Add(drPointDetails_Temp_1);
                      }
                      else
                      {
                        strEntryTime_11 = drRest_12[j]["tLastTime"].ToString();
                        strDestroyTime_11 = drRest_12[j + 1]["tBeforTime"].ToString();
                        DataRow drPointDetails_Temp_1 = dtPointDetails_Temp.NewRow();
                        drPointDetails_Temp_1["dHosDate"] = strHosDate;
                        drPointDetails_Temp_1["vcPackPlant"] = strPackPlant;
                        drPointDetails_Temp_1["vcBanZhi"] = strBanZhi;
                        drPointDetails_Temp_1["vcPointNo"] = strPointNo;
                        drPointDetails_Temp_1["UUID"] = strUUID;
                        drPointDetails_Temp_1["dEntryTime"] = strEntryTime_11;
                        drPointDetails_Temp_1["dDestroyTime"] = strDestroyTime_11;
                        dtPointDetails_Temp.Rows.Add(drPointDetails_Temp_1);
                      }
                    }
                  }
                  else
                  {
                    DataRow drPointDetails_Temp = dtPointDetails_Temp.NewRow();
                    drPointDetails_Temp["dHosDate"] = strHosDate;
                    drPointDetails_Temp["vcPackPlant"] = strPackPlant;
                    drPointDetails_Temp["vcBanZhi"] = strBanZhi;
                    drPointDetails_Temp["vcPointNo"] = strPointNo;
                    drPointDetails_Temp["UUID"] = strUUID;
                    drPointDetails_Temp["dEntryTime"] = strEntryTime;
                    drPointDetails_Temp["dDestroyTime"] = strDestroyTime;
                    dtPointDetails_Temp.Rows.Add(drPointDetails_Temp);
                  }
                }
              }

            }
          }
        }
        decimal decOnLine = 0;
        for (int i = 0; i < dtPointDetails_Temp.Rows.Count; i++)
        {
          DateTime dEntryTime = Convert.ToDateTime(dtPointDetails_Temp.Rows[i]["dEntryTime"].ToString());
          DateTime dDestroyTime = Convert.ToDateTime(dtPointDetails_Temp.Rows[i]["dDestroyTime"].ToString());
          TimeSpan timeSpan = dDestroyTime.Subtract(dEntryTime);
          double secInterval = timeSpan.TotalSeconds;
          decOnLine = decOnLine + Convert.ToDecimal(secInterval);
        }
        return decOnLine;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public DataTable GetStanTime()
    {
      return P00003_DataAccess.GetStanTime();
    }

    public DataTable GetCaseNo(string iP)
    {
      return P00003_DataAccess.GetCaseNo(iP);
    }

    public DataTable GetStatus2(string iP, string opearteId)
    {
      return P00003_DataAccess.GetStatus2(iP, opearteId);
    }

    public int UpdateCase2(string caseNo, string serverTime)
    {
      return P00003_DataAccess.UpdateCase2(caseNo, serverTime);
    }

    public int UpdateStatus4(string pointNo, string opearteId)
    {
      return P00003_DataAccess.UpdateStatus4(pointNo, opearteId);
    }

    public int UpdateStatus5(string pointNo, string opearteId)
    {
      return P00003_DataAccess.UpdateStatus5(pointNo, opearteId);
    }

    public DataTable GetTime(string formatDate, string opearteId)
    {
      return P00003_DataAccess.GetTime(formatDate, opearteId);
    }

    public int UpdateEffi(string formatDate, string opearteId, string stopTime)
    {
      return P00003_DataAccess.UpdateEffi(formatDate, opearteId, stopTime);
    }

    public int InsertFre(string time, string formatDate, string effiEncy, string opearteId, string serverTime, string iP, string date, string banZhi)
    {
      return P00003_DataAccess.InsertFre(time, formatDate, effiEncy, opearteId, serverTime, iP, date, banZhi);
    }

    public DataTable GetCheckType(string partId, string scanTime)
    {
      return P00003_DataAccess.GetCheckType(partId, scanTime);
    }

    public int UpdateFre(string time, string serverTime, string formatDate, string opearteId)
    {
      return P00003_DataAccess.UpdateFre(time, serverTime, formatDate, opearteId);
    }

    public int InsertTP(string iP, string opearteId, string serverTime, string caseNo)
    {
      return P00003_DataAccess.InsertTP(iP, opearteId, serverTime, caseNo);
    }

    public DataTable ValidateData1(string partId, string scanTime)
    {
      return P00003_DataAccess.ValidateData1(partId, scanTime);
    }

    public DataTable GetPackInfo(string partId, string scanTime, string packingQuatity, string quantity)
    {
      return P00003_DataAccess.GetPackInfo(partId, scanTime, packingQuatity, quantity);
    }

    public int UpdateCase(string count, string sum, string caseNo)
    {
      return P00003_DataAccess.UpdateCase(count, sum, caseNo);
    }

    public static int InsertOpr(string bzPlant, string inputNo, string kanbanOrderNo, string kanbanSerial, string partId, string inoutFlag, string supplier_id, string supplierGQ, string scanTime, string serverTime, string quantity, string bZUnit, string sHF, string dock, string checkType, string labelStart, string labelEnd, string checkStatus, string opearteId, string timeStart, string timeEnd, string iP)
    {
      return P00003_DataAccess.InsertOpr(bzPlant, inputNo, kanbanOrderNo, kanbanSerial, partId, inoutFlag, supplier_id, supplierGQ, scanTime, serverTime, quantity, int.Parse(bZUnit), sHF, dock, checkType, labelStart, labelEnd, checkStatus, opearteId, timeStart, timeEnd, iP);
    }

    public int UpdateFre1(int totalTime, string opearteId, string formatDate)
    {
      return P00003_DataAccess.UpdateFre1(totalTime, opearteId, formatDate);
    }

    public DataTable ValidateOpr1(string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial)
    {
      return P00003_DataAccess.ValidateOpr1(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
    }

    public static int UpdateInv(string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial, string scanTime, string serverTime, string opearteId)
    {
      return P00003_DataAccess.UpdateInv(partId, quantity, dock, kanbanOrderNo, kanbanSerial, scanTime, serverTime, opearteId);
    }







    public int UpdateCase(string opearteId, string caseNo)
    {
      return P00003_DataAccess.UpdateCase(opearteId, caseNo);
    }

    public DataTable ValidateCaseNo3(string caseNo)
    {
      return P00003_DataAccess.ValidateCaseNo3(caseNo);
    }

    public DataTable ValidateCaseNo2(string caseNo)
    {
      return P00003_DataAccess.ValidateCaseNo2(caseNo);
    }

    public DataTable ValidateCaseNo1(string caseNo)
    {
      return P00003_DataAccess.ValidateCaseNo1(caseNo);
    }



    public int UpdateCaseInfo(string caseNo, string opearteId, string iP, string serverTime)
    {
      return P00003_DataAccess.UpdateCaseInfo(caseNo, opearteId, iP, serverTime);
    }

    public int InsertCaseInfo(string caseNo, string opearteId, string iP, string serverTime)
    {
      return P00003_DataAccess.InsertCaseInfo(caseNo, opearteId, iP, serverTime);
    }











    public static DataTable GetPM(string dock, string partId)
    {
      return P00003_DataAccess.GetPM(dock, partId);
    }



    public static DataTable GetData(string partId, string dock, string kanbanOrderNo, string kanbanSerial)
    {
      return P00003_DataAccess.GetData(partId, dock, kanbanOrderNo, kanbanSerial);
    }

    public static DataTable ValidateOpr2(string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial)
    {
      return P00003_DataAccess.ValidateOpr2(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
    }

    public DataTable ValidateOpr3(string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial)
    {
      return P00003_DataAccess.ValidateOpr3(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
    }

    public int InsertSj(string supplierId, string supplierPlant, string packingQuantity, string checkType, string lblStart, string lblEnd, string inOutFlag, string checkStatus, string packingSpot, string inputNo, string checkNum, string partId, string kanbanOrderNo, string kanbanSerial, string dock, string opearteId, string scanTime, string serverTime, string iP, string sHF, string quantity1, string caseNo)
    {
      return P00003_DataAccess.InsertSj(supplierId, supplierPlant, packingQuantity, checkType, lblStart, lblEnd, inOutFlag, checkStatus, packingSpot, inputNo, checkNum, partId, kanbanOrderNo, kanbanSerial, dock, opearteId, scanTime, serverTime, iP, sHF, quantity1, caseNo);
    }

    public DataTable ValidateSJ2(string partId, string dock, string kanbanOrderNo, string kanbanSerial)
    {
      return P00003_DataAccess.ValidateSJ2(partId, dock, kanbanOrderNo, kanbanSerial);
    }

    public DataTable GetStatus1(string iP, string opearteId)
    {
      return P00003_DataAccess.GetStatus1(iP, opearteId);
    }

    public int UpdateCase3(string caseNo)
    {
      return P00003_DataAccess.UpdateCase3(caseNo);
    }

    public int UpdateStatus3(string pointNo, string opearteId)
    {
      return P00003_DataAccess.UpdateStatus3(pointNo, opearteId);
    }

    public DataTable GetCase1(string caseNo)
    {
      return P00003_DataAccess.GetCase1(caseNo);
    }

    public DataTable ValidateInv1(string partId, string kanbanOrderNo, string kanbanSerial)
    {
      return P00003_DataAccess.ValidateInv1(partId, kanbanOrderNo, kanbanSerial);
    }

    public int UpdateInv1(string partId, string kanbanOrderNo, string kanbanSerial, string quantity)
    {
      return P00003_DataAccess.UpdateInv1(partId, kanbanOrderNo, kanbanSerial, quantity);
    }

    public DataTable GetPartsName(string scanTime, string partId)
    {
      return P00003_DataAccess.GetPartsName(scanTime, partId);
    }



    public int InsertSj1(string supplier_id, string supplierGQ, string bZUnit, string checkType, string labelStart, string labelEnd, string inoutFlag, string checkStatus, string bzPlant, string inputNo, string quantity, string partId, string kanbanOrderNo, string kanbanSerial, string dock, string opearteId, string scanTime, string serverTime, string iP, string sHF, string caseNo)
    {
      return P00003_DataAccess.InsertSj1(supplier_id, supplierGQ, bZUnit, checkType, labelStart, labelEnd, inoutFlag, checkStatus, bzPlant, inputNo, quantity, partId, kanbanOrderNo, kanbanSerial, dock, opearteId, scanTime, serverTime, iP, sHF, caseNo);
    }

    public int InsertCase(string sHF, string cpdName, string cpdAddress, string caseNo, string inputNo, string partId, string quantity, string partsName, string opearteId, string serverTime, string iP, byte[] vs, string labelStart, string labelEnd)
    {
      return P00003_DataAccess.InsertCase(sHF, cpdName, cpdAddress, caseNo, inputNo, partId, quantity, partsName, opearteId, serverTime, iP, vs, labelStart, labelEnd);
    }

    public DataTable ValidateOpr4(string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial)
    {
      return P00003_DataAccess.ValidateOpr4(quantity, dock, kanbanOrderNo, kanbanSerial, partId);
    }

    public int InsertTP1(string iP, string opearteId, string serverTime, string inputNo, string printName)
    {
      return P00003_DataAccess.InsertTP1(iP, opearteId, serverTime, inputNo, printName);
    }

    public DataTable GetInputQuantity(string kanbanOrderNo, string kanbanSerial, string partId, string dock)
    {
      return P00003_DataAccess.GetInputQuantity(kanbanOrderNo, kanbanSerial, partId, dock);
    }

    public DataTable ValidateCaseNo4(string caseNo)
    {
      return P00003_DataAccess.ValidateCaseNo4(caseNo);
    }

    public DataTable ValidateOpr5(string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial)
    {
      return P00003_DataAccess.ValidateOpr5(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
    }

    public DataTable ValidateTime(string partId, string dock)
    {
      return P00003_DataAccess.ValidateTime(partId, dock);
    }

    public DataTable GetCase(string opearteId)
    {
      return P00003_DataAccess.GetCase(opearteId);
    }

    public int UpdateEffi(string formatDate, double effiencyNew, string opearteId, string serverTime)
    {
      return P00003_DataAccess.UpdateEffi(formatDate, effiencyNew, opearteId, serverTime);
    }

    public DataTable ValidateEffi(string opearteId, string formatDate)
    {
      throw new NotImplementedException();
    }

    public int UpdateTime(string formatDate, int totalTime, string opearteId)
    {
      return P00003_DataAccess.UpdateTime(formatDate, totalTime, opearteId);
    }

    public DataTable GetCaseList1(string caseNo)
    {
      return P00003_DataAccess.GetCaseList1(caseNo);
    }

    public DataTable ValidateInv(string inputNo)
    {
      return P00003_DataAccess.ValidateInv(inputNo);
    }

    public int UpdateInv(string inputNo, string quantity)
    {
      return P00003_DataAccess.UpdateInv(inputNo, quantity);
    }

    public DataTable ValidateData(string partId, string scanTime)
    {
      return P00003_DataAccess.ValidateData(partId, scanTime);
    }

    public DataTable GetStatus(string iP)
    {
      return P00003_DataAccess.GetStatus(iP);
    }

    public int UpdateStatus2(string iP, string opearteId, string pointNo)
    {
      return P00003_DataAccess.UpdateStatus2(iP, opearteId, pointNo);
    }

    public DataTable GetBanZhi(string serverTime)
    {
      return P00003_DataAccess.GetBanZhi(serverTime);
    }

    public DataTable GetLabel(string inputNo)
    {
      return P00003_DataAccess.GetLabel(inputNo);
    }

    public DataTable GetQuantity(string kanbanOrderNo, string kanbanSerial, string partId, string dock)
    {
      return P00003_DataAccess.GetQuantity(kanbanOrderNo, kanbanSerial, partId, dock);
    }

    public int InsertBox(string caseNo, string inputNo, string partId, string kanbanOrderNo, string kanbanSerial, string quantity, string opearteId, string scanTime, string labelStart, string labelEnd, string rhQuantity, string serverTime, string dock)
    {
      return P00003_DataAccess.InsertBox(caseNo, inputNo, partId, kanbanOrderNo, kanbanSerial, quantity, opearteId, scanTime, labelStart, labelEnd, rhQuantity, serverTime, dock);
    }

    public DataTable GetCaseInfo(string caseNo)
    {
      return P00003_DataAccess.GetCaseInfo(caseNo);
    }

    public DataTable GetCaseInfo1(string caseNo)
    {
      return P00003_DataAccess.GetCaseInfo1(caseNo);
    }

    public byte[] GenerateQRCode(string content)
    {
      var generator = new QRCodeGenerator();
      var codeData = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.M, true);

      QRCoder.QRCode qrcode = new QRCoder.QRCode(codeData);
      var bitmapImg = qrcode.GetGraphic(10, Color.Black, Color.White, false);
      using MemoryStream stream = new MemoryStream();
      bitmapImg.Save(stream, ImageFormat.Jpeg);
      return stream.GetBuffer();
    }

    public int UpdateBox(string caseNo, string serverTime)
    {
      return P00003_DataAccess.UpdateBox(caseNo, serverTime);
    }

    public DataTable GetPoingNo(string iP)
    {
      return P00003_DataAccess.GetPointNo(iP);
    }

    public DataTable GetPackData1(string partId, string serverTime)
    {
      return P00003_DataAccess.GetPackData1(partId, serverTime);
    }

    public DataTable GetPackBase(string packNo, string serverTime)
    {
      return P00003_DataAccess.GetPackBase(packNo, serverTime);
    }

    public int InsertPackWork(string packNo, string gpsNo, string packsupplier, string bZUnit, string biYao, string opearteId, string serverTime, string quantity)
    {
      return P00003_DataAccess.InsertPackWork(packNo, gpsNo, packsupplier, bZUnit, biYao, opearteId, serverTime, quantity);
    }

    public DataTable GetPrintName(string iP)
    {
      return P00003_DataAccess.GetPrintName(iP);
    }

    public int UpdateInv2(string packNo, string gpsNo, string packsupplier, string bZUnit, string biYao, string opearteId, string serverTime, string quantity)
    {
      return P00003_DataAccess.UpdateInv2(packNo, gpsNo, packsupplier, bZUnit, biYao, opearteId, serverTime, quantity);
    }

    public int UpdateCase5(string iP, string caseNo)
    {
      return P00003_DataAccess.UpdateCase5(iP, caseNo);
    }

    public DataTable GetZaiKu(string packNo, string gpsNo, string packsupplier)
    {
      return P00003_DataAccess.GetZaiKu(packNo, gpsNo, packsupplier);
    }

    public int InsertZaiKu(string packNo, string gpsNo, string packsupplier, string opearteId, string serverTime)
    {
      return P00003_DataAccess.InsertZaiKu(packNo, gpsNo, packsupplier, opearteId, serverTime);
    }

    public DataTable ValidateCase(string partId, string kanbanOrderNo, string kanbanSerial, string dock, string caseNo)
    {
      return P00003_DataAccess.ValidateCaseNo(partId,kanbanOrderNo,kanbanSerial,dock,caseNo);
    }
  }
}
