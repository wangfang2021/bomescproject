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


        public static DataTable GetPackData(string partId, string scanTime)
        {
            return P00003_DataAccess.GetPackData(partId, scanTime);
        }

        public int UpdateCase1(string opearteId, string iP)
        {
            return P00003_DataAccess.UpdateCase1(opearteId, iP);
        }

        public DataTable GetUserRole(string user)
        {
            return P00003_DataAccess.GetUserRole(user);
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

        public DataTable GetStatus2(string iP, string opearteId)
        {
            return P00003_DataAccess.GetStatus2(iP, opearteId);
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

        public int UpdateFre(string time, string serverTime, string formatDate, string opearteId)
        {
            return P00003_DataAccess.UpdateFre(time, serverTime, formatDate, opearteId);
        }

        public static DataTable GetPM(string dock, string partId)
        {
            return P00003_DataAccess.GetPM(dock, partId);
        }

        public static DataTable GetData(string partId, string dock, string kanbanOrderNo, string kanbanSerial)
        {
            return P00003_DataAccess.GetData(partId, dock, kanbanOrderNo, kanbanSerial);
        }

        public DataTable GetCase(string opearteId)
        {
            return P00003_DataAccess.GetCase(opearteId);
        }

        public DataTable GetBanZhi(string serverTime)
        {
            return P00003_DataAccess.GetBanZhi(serverTime);
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

        //========================================================================重写========================================================================
        public DataTable getOperCaseNo(string iP, string strPointState, string strOperatorID)
        {
            try
            {
                return P00003_DataAccess.getOperCaseNo(iP, strPointState, strOperatorID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetCaseNoInfo(string strCaseNo)
        {
            try
            {
                return P00003_DataAccess.GetCaseNoInfo(strCaseNo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void SetCaseNoInfo(string strBoxNo, string strCaseNo, string strHostIp, string strSheBeiNo, string strPointState, string strOperatorID)
        {
            try
            {
                P00003_DataAccess.SetCaseNoInfo(strBoxNo, strCaseNo, strHostIp, strSheBeiNo, strPointState, strOperatorID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetKanBanInfo(string partId, string kanbanOrderNo, string kanbanSerial, string dock, string scanTime)
        {
            try
            {
                return P00003_DataAccess.GetKanBanInfo(partId, kanbanOrderNo, kanbanSerial, dock, scanTime);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetPackList(string strInno)
        {
            try
            {
                return P00003_DataAccess.GetPackList(strInno);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setPrintLable(string strIP, string strInvNo, string strPrinterName, string strOperId)
        {
            try
            {
                P00003_DataAccess.setPrintLable(strIP, strInvNo, strPrinterName, strOperId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getPackInfo(string partId, string kanbanOrderNo, string kanbanSerial, string dock, string packQuantity)
        {
            try
            {
                return P00003_DataAccess.getPackInfo(partId, kanbanOrderNo, kanbanSerial, dock, packQuantity);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet getTableFromDB(string serverTime)
        {
            try
            {
                return P00003_DataAccess.getTableFromDB(serverTime);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool setPackAndZxInfo(string strIP, string strPointName, string strType, string partId, string kanbanOrderNo, string kanbanSerial, string dock, string packQuantity, string caseno, string boxno, string scanTime, DataTable dtPackList, string strOperId)
        {
            try
            {
                return P00003_DataAccess.setPackAndZxInfo(strIP, strPointName, strType, partId, kanbanOrderNo, kanbanSerial, dock, packQuantity, caseno, boxno, scanTime, dtPackList, strOperId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getBoxMasterInfo(string caseno, string serverTime)
        {
            try
            {
                return P00003_DataAccess.getBoxMasterInfo(caseno, serverTime);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool setCastListInfo(DataTable dtOperateSJ_Temp, DataTable dtCaseList_Temp, string strIP, string caseno, string boxno, string scanTime, string strOperId)
        {
            try
            {
                return P00003_DataAccess.setCastListInfo( dtOperateSJ_Temp,  dtCaseList_Temp,  strIP,  caseno,  boxno,  scanTime,  strOperId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
