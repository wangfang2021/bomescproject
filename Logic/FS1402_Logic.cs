using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using System.IO;
using Common;

namespace Logic
{
    public class FS1402_Logic
    {
        FS1402_DataAccess fs1402_DataAccess;
        FS0603_Logic fS0603_Logic = new FS0603_Logic();

        public FS1402_Logic()
        {
            fs1402_DataAccess = new FS1402_DataAccess();
        }
        public DataTable getSearchInfo(string strCheckType, string strPartId, string strSupplierId)
        {
            return fs1402_DataAccess.getSearchInfo(strCheckType, strPartId, strSupplierId);
        }
        public DataTable checkSaveInfo(DataTable dtImport, ref DataTable dtMessage)
        {
            try
            {
                for (int i = 0; i < dtImport.Rows.Count; i++)
                {
                    string strType = dtImport.Rows[i]["vcType"].ToString();
                    if (strType == "add")
                    {
                        string strPartId = dtImport.Rows[i]["vcPartId"].ToString();
                        string strSupplierId = dtImport.Rows[i]["vcSupplierId"].ToString();
                        string strFromTime = dtImport.Rows[i]["dFromTime"].ToString();
                        string strToTime = dtImport.Rows[i]["dToTime"].ToString();
                        string strCheckP = dtImport.Rows[i]["vcCheckP"].ToString();
                        if (strPartId == "" || strSupplierId == "" || strFromTime == "" || strToTime == "")
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = "缺少必填项请完善数据。";
                            dtMessage.Rows.Add(dataRow);
                        }
                        else
                        {
                            if (strCheckP == "")
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = strPartId + "品番检查频度为空请完善数据。";
                                dtMessage.Rows.Add(dataRow);
                            }
                            else
                            {
                                DataTable dtCheckTime = getSearchInfo("", strPartId, strSupplierId);
                                if (dtCheckTime.Rows.Count > 0)
                                {
                                    string strLinid_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["LinId"].ToString();
                                    string strFromTime_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["dFromTime"].ToString();
                                    string strToTime_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["dToTime"].ToString();
                                    if (Convert.ToDateTime(strFromTime_before) >= Convert.ToDateTime(strFromTime))
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcMessage"] = "品番" + strPartId + "【检查频度】维护有误(当前有效的开始使用时间小于维护的开始使用时间)";
                                        dtMessage.Rows.Add(dataRow);
                                    }
                                    strToTime_before = Convert.ToDateTime(strFromTime).AddDays(-1).ToString("yyyy-MM-dd");
                                    DataRow drImport = dtImport.NewRow();
                                    drImport["LinId"] = strLinid_before;
                                    drImport["dToTime"] = strToTime_before;
                                    drImport["vcType"] = "mod";
                                    dtImport.Rows.Add(drImport);
                                }
                            }
                        }
                    }
                }
                return dtImport;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setSaveInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                fs1402_DataAccess.setSaveInfo(dtImport, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable ImportFile(DirectoryInfo theFolder, string fileSavePath, string sheetName, string[,] headers, bool bSourceFile, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                DataTable dataTable = new DataTable();
                //读取导入文件信息
                string strMessage = "";
                foreach (FileInfo info in theFolder.GetFiles())
                {
                    DataTable dt = ComFunction.ExcelToDataTable(info.FullName, sheetName, headers, ref strMessage);
                    if (strMessage != "")
                    {
                        ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = "导入终止，文件" + info.Name + ":" + strMessage;
                        dtMessage.Rows.Add(dataRow);
                    }
                    if (dataTable.Columns.Count == 0)
                        dataTable = dt.Clone();
                    if (dt.Rows.Count == 0)
                    {
                        ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = "导入终止，文件" + info.Name + "没有要导入的数据";
                        dtMessage.Rows.Add(dataRow);
                    }
                    foreach (DataRow row in dt.Rows)
                    {
                        dataTable.ImportRow(row);
                    }
                }
                //读取数据后删除文件夹
                if (bSourceFile)
                {
                    ComFunction.DeleteFolder(fileSavePath);
                }
                //查重
                var result = from r in dataTable.AsEnumerable()
                             group r by new { r2 = r.Field<string>("vcPartId"), r3 = r.Field<string>("vcSupplierId") } into g
                             where g.Count() > 1
                             select g;
                if (result.Count() > 0)
                {
                    StringBuilder sbr = new StringBuilder();
                    sbr.Append("导入数据重复:<br/>");
                    foreach (var item in result)
                    {
                        sbr.Append("品番:" + item.Key.r2 + " 供应商编码:" + item.Key.r3 + "<br/>");
                    }
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = sbr.ToString();
                    dtMessage.Rows.Add(dataRow);
                }
                dataTable.Columns.Add("LinId");
                dataTable.Columns.Add("vcType");
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    dataTable.Rows[i]["vcType"] = "add";
                    if (dataTable.Rows[i]["vcPartId"].ToString().Replace("-", "").Length != 12)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = "Excel第" + (i + 1) + "行品番格式错误";
                        dtMessage.Rows.Add(dataRow);
                    }
                    try
                    {
                        Convert.ToDateTime(dataTable.Rows[i]["dFromTime"].ToString());
                        Convert.ToDateTime(dataTable.Rows[i]["dToTime"].ToString());
                        if (Convert.ToDateTime(dataTable.Rows[i]["dFromTime"].ToString()) > Convert.ToDateTime(dataTable.Rows[i]["dToTime"].ToString()))
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = "Excel第" + (i + 1) + "行起始时间大于结束时间";
                            dtMessage.Rows.Add(dataRow);
                        }
                    }
                    catch
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = "Excel文件中第" + (i + 1) + "行日期格式有误";
                        dtMessage.Rows.Add(dataRow);
                    }
                    if (!(dataTable.Rows[i]["vcSupplierId"].ToString().Length == 4))
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = "Excel第" + (i + 1) + "行供应商格式错误";
                        dtMessage.Rows.Add(dataRow);
                    }
                    if (!(dataTable.Rows[i]["vcCheckP"].ToString() == "全检" || dataTable.Rows[i]["vcCheckP"].ToString() == "抽检" || dataTable.Rows[i]["vcCheckP"].ToString() == "免检"))
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = "Excel第" + (i + 1) + "行检查区类型错误,选填:全检|抽检|免检";
                        dtMessage.Rows.Add(dataRow);
                    }
                }

                return dataTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getSearchSubInfo()
        {
            return fs1402_DataAccess.getSearchSubInfo();
        }
        public DataTable checksubSaveInfo(List<Dictionary<string, Object>> listInfoData, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtInfo = fS0603_Logic.createTable("SpotQty1402");
                DataTable dtCheck = fS0603_Logic.createTable("SpotQty1402");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string strPackingQty = listInfoData[i]["iPackingQty"] == null ? "" : listInfoData[i]["iPackingQty"].ToString();
                    string strSpotQty = listInfoData[i]["iSpotQty"] == null ? "" : listInfoData[i]["iSpotQty"].ToString();
                    if (strPackingQty != "0")
                    {
                        DataRow drInfo = dtInfo.NewRow();
                        drInfo["iPackingQty"] = strPackingQty;
                        drInfo["iSpotQty"] = strSpotQty;
                        dtInfo.Rows.Add(drInfo);
                    }
                }
                if (dtInfo.Rows.Count == 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "请维护抽检数对应关系";
                    dtMessage.Rows.Add(dataRow);
                }
                var check = from t in dtInfo.AsEnumerable()
                            group t by new
                            {
                                t1 = t.Field<string>("iPackingQty")
                            } into m
                            select new
                            {
                                PackingQty = m.Key.t1,
                                rowCount = m.Count()
                            };
                if (check.ToList().Count > 0)
                {
                    check.ToList().ForEach(q =>
                    {
                        DataRow drCheck = dtCheck.NewRow();
                        drCheck["iPackingQty"] = q.PackingQty;
                        drCheck["iSpotQty"] = q.rowCount;
                        dtCheck.Rows.Add(drCheck);
                    });
                }
                for (int i = 0; i < dtCheck.Rows.Count; i++)
                {
                    if(dtCheck.Rows[i]["iSpotQty"].ToString()!="1")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = dtCheck.Rows[i]["iPackingQty"].ToString()+ "收容数对应多个抽检数，请检查修改";
                        dtMessage.Rows.Add(dataRow);
                    }
                }
                return dtInfo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setSavesubInfo(DataTable dtInfo, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                fs1402_DataAccess.setSavesubInfo(dtInfo, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
