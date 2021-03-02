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
    public class FS1404_Logic
    {
        FS1404_DataAccess fs1404_DataAccess;

        public FS1404_Logic()
        {
            fs1404_DataAccess = new FS1404_DataAccess();
        }
        public DataTable getSearchInfo(string strPartId, string strSupplierId, string strSupplierPlant)
        {
            return fs1404_DataAccess.getSearchInfo(strPartId, strSupplierId, strSupplierPlant);
        }
        public DataTable checkSaveInfo(DataTable dtImport, ref DataTable dtMessage)
        {
            try
            {
                for (int i = 0; i < dtImport.Rows.Count; i++)
                {
                    string strPartId = dtImport.Rows[i]["vcPartId"].ToString();
                    string strSupplierId = dtImport.Rows[i]["vcSupplierId"].ToString();
                    string strSupplierPlant = dtImport.Rows[i]["vcSupplierPlant"].ToString();
                    string strFromTime = dtImport.Rows[i]["dFromTime"].ToString();
                    DataTable dtCheckTime = getSearchInfo(strPartId, strSupplierId, strSupplierPlant);
                    if (dtCheckTime.Rows.Count > 0)
                    {
                        string strLinid_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["LinId"].ToString();
                        string strFromTime_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["dFromTime"].ToString();
                        string strToTime_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["dToTime"].ToString();
                        if (Convert.ToDateTime(strFromTime_before) >= Convert.ToDateTime(strFromTime))
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = "品番" + strPartId + "【SPIS区分】维护有误(当前有效的开始使用时间小于维护的开始使用时间)";
                            dtMessage.Rows.Add(dataRow);
                        }
                        strToTime_before = Convert.ToDateTime(strFromTime).AddDays(-1).ToString("yyyy-MM-dd");
                        DataRow drImport = dtImport.NewRow();
                        drImport["LinId"] = strPartId;
                        drImport["dToTime"] = strToTime_before;
                        drImport["vcType"] = "mod";
                        dtImport.Rows.Add(drImport);
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
                fs1404_DataAccess.setSaveInfo(dtImport, strOperId, ref dtMessage);
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
                    if (info.FullName.IndexOf(".xlsx") > 0 || info.FullName.IndexOf(".xlsm") > 0 || info.FullName.IndexOf(".xls") > 0)
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
                                 group r by new { r2 = r.Field<string>("vcPartId"), r3 = r.Field<string>("vcSupplierCode"), r4 = r.Field<string>("vcSupplierPlant") } into g
                                 where g.Count() > 1
                                 select g;
                    if (result.Count() > 0)
                    {
                        StringBuilder sbr = new StringBuilder();
                        sbr.Append("导入数据重复:<br/>");
                        foreach (var item in result)
                        {
                            sbr.Append("品番:" + item.Key.r2 + " 供应商编码:" + item.Key.r3 + " 供应商工区:" + item.Key.r4 + "<br/>");
                        }
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = sbr.ToString();
                        dtMessage.Rows.Add(dataRow);
                    }
                    dataTable.Columns.Add("vcType");
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        dataTable.Rows[i]["vcPartId"] = "add";
                        if (dataTable.Rows[i]["vcPartId"].ToString().Replace("-", "").Length != 12)
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = "Excel第" + (i + 1) + "行品番格式错误";
                            dtMessage.Rows.Add(dataRow);
                        }
                        try
                        {
                            Convert.ToDateTime(dataTable.Rows[i]["vcTimeFrom"].ToString());
                            Convert.ToDateTime(dataTable.Rows[i]["vcTimeTo"].ToString());
                            if (Convert.ToDateTime(dataTable.Rows[i]["vcTimeFrom"].ToString()) > Convert.ToDateTime(dataTable.Rows[i]["vcTimeTo"].ToString()))
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
                        if (!(dataTable.Rows[i]["vcSupplierCode"].ToString().Length == 4 && dataTable.Rows[i]["vcSupplierPlant"].ToString().Length == 1))
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = "Excel第" + (i + 1) + "行车种代码、供应商(工区)格式错误";
                            dtMessage.Rows.Add(dataRow);
                        }
                        if (!(dataTable.Rows[i]["vcPicUrl"].ToString() == ""))
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = "Excel第" + (i + 1) + "行SPIS区分为空";
                            dtMessage.Rows.Add(dataRow);
                        }
                    }
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
