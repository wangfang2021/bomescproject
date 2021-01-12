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

        public FS1402_Logic()
        {
            fs1402_DataAccess = new FS1402_DataAccess();
        }
        public DataTable getSearchInfo(string strCheckQf, string strPartId, string strSuplierId, string strSuplierPlant)
        {
            return fs1402_DataAccess.getSearchInfo(strCheckQf, strPartId, strSuplierId, strSuplierPlant);
        }
        public bool ImportFile(DirectoryInfo theFolder, string fileSavePath, string sheetName, string[,] headers, bool bSourceFile, string strOperId, ref string strMessage)
        {
            try
            {
                DataTable dataTable = new DataTable();
                //读取导入文件信息
                foreach (FileInfo info in theFolder.GetFiles())
                {
                    DataTable dt = ComFunction.ExcelToDataTable(info.FullName, sheetName, headers, ref strMessage);
                    if (strMessage != "")
                    {
                        ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                        strMessage = "导入终止，文件" + info.Name + ":" + strMessage;
                        return false;
                    }
                    if (dataTable.Columns.Count == 0)
                        dataTable = dt.Clone();
                    if (dt.Rows.Count == 0)
                    {
                        ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                        strMessage = "导入终止，文件" + info.Name + "没有要导入的数据";
                        return false;
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
                    strMessage = sbr.ToString();
                    return false;
                }
                //校验格式
                #region
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    if (dataTable.Rows[i]["vcPartId"].ToString().Replace("-", "").Length != 12)
                    {
                        strMessage = strMessage + "Excel第" + (i + 1) + "行品番格式错误";
                    }
                    try
                    {
                        Convert.ToDateTime(dataTable.Rows[i]["vcTimeFrom"].ToString());
                        Convert.ToDateTime(dataTable.Rows[i]["vcTimeTo"].ToString());
                        if (Convert.ToDateTime(dataTable.Rows[i]["vcTimeFrom"].ToString()) > Convert.ToDateTime(dataTable.Rows[i]["vcTimeTo"].ToString()))
                        {
                            strMessage = strMessage + "Excel第" + (i + 1) + "行起始时间大于结束时间";
                        }
                    }
                    catch
                    {
                        strMessage = strMessage + "Excel文件中第" + (i + 1) + "行日期格式有误";
                    }
                    if (!(dataTable.Rows[i]["vcSupplierCode"].ToString().Length == 4 && dataTable.Rows[i]["vcCarfamilyCode"].ToString().Length == 4 && dataTable.Rows[i]["vcSupplierPlant"].ToString().Length == 1))
                    {
                        strMessage = strMessage + "Excel第" + (i + 1) + "行车种代码、供应商(工区)格式错误";
                    }
                    if (!(dataTable.Rows[i]["vcCheckP"].ToString() == "全检" || dataTable.Rows[i]["vcCheckP"].ToString() == "抽检" || dataTable.Rows[i]["vcCheckP"].ToString() == "免检"))
                    {
                        strMessage = strMessage + "Excel第" + (i + 1) + "行检查区类型错误,选填:全检|抽检|免检";
                    }
                }
                if (strMessage != "")
                    return false;
                #endregion
                //数据库校验
                //本次的起始日期不能小于上次的起始时间
                #region 
                dataTable.Columns.Add("vcType");
                strMessage = checkDbInfo(dataTable, strMessage);
                if (strMessage != "")
                    return false;
                #endregion
                //插入数据库
                fs1402_DataAccess.setDataInfo(dataTable, strOperId);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string checkDbInfo(DataTable dataTable, string strMessage)
        {
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                DataTable dtCheck = fs1402_DataAccess.getCheckInfo(dataTable.Rows[i]["vcPartId"].ToString(), dataTable.Rows[i]["vcSupplierCode"].ToString(), dataTable.Rows[i]["vcSupplierPlant"].ToString());
                if (dtCheck.Rows.Count != 0 && dtCheck.Rows[0]["vcTimeFrom"].ToString() != "")
                {
                    if (Convert.ToDateTime(dtCheck.Rows[0]["vcTimeFrom"].ToString()) >= Convert.ToDateTime(dataTable.Rows[i]["vcTimeFrom"].ToString()))
                    {
                        strMessage = strMessage + "第" + (i + 1) + "本次的起始日期不能小于上次的起始时间";
                    }
                    else
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["vcPartId"] = dataTable.Rows[i]["vcPartId"].ToString();
                        dataRow["vcSupplierCode"] = dataTable.Rows[i]["vcSupplierCode"].ToString();
                        dataRow["vcSupplierPlant"] = dataTable.Rows[i]["vcSupplierPlant"].ToString();
                        dataRow["vcTimeFrom"] = dtCheck.Rows[0]["vcTimeFrom"].ToString();
                        dataRow["vcTimeTo"] = Convert.ToDateTime(dataTable.Rows[i]["vcTimeFrom"].ToString()).AddDays(-1).ToString("yyyy-MM-dd");
                        dataRow["vcType"] = "update";
                        dataTable.Rows.Add(dataRow);
                    }
                }
            }
            return strMessage;
        }
        public DataTable getSubInfo(string strLinid)
        {
            return fs1402_DataAccess.getSubInfo(strLinid);
        }

        public bool setDataInfo(string strType, string strInfo, string strPartNo, string strFromTime, string strToTime, string strSupplierCode, string strSupplierPlant, string strCarFamilyCode, string strCheckQf, string strTeJi, string strChangeReason, string strOperId, ref string strMessage)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("vcPartId");
            dataTable.Columns.Add("vcTimeFrom");
            dataTable.Columns.Add("vcTimeTo");
            dataTable.Columns.Add("vcSupplierCode");
            dataTable.Columns.Add("vcSupplierPlant");
            dataTable.Columns.Add("vcCarfamilyCode");
            dataTable.Columns.Add("vcCheckP");
            dataTable.Columns.Add("vcChangeRea");
            dataTable.Columns.Add("vcTJSX");
            DataRow dataRow = dataTable.NewRow();
            dataRow["vcPartId"] = strPartNo;
            dataRow["vcTimeFrom"] = strFromTime;
            dataRow["vcTimeTo"] = strToTime;
            dataRow["vcSupplierCode"] = strSupplierCode;
            dataRow["vcSupplierPlant"] = strSupplierPlant;
            dataRow["vcCarfamilyCode"] = strCarFamilyCode;
            dataRow["vcCheckP"] = strCheckQf;
            dataRow["vcChangeRea"] = strChangeReason;
            dataRow["vcTJSX"] = strTeJi;
            dataRow["vcType"] = strType;
            dataTable.Rows.Add(dataRow);
            strMessage = checkDbInfo(dataTable, strMessage);
            if (strMessage != "")
                return false;
            //插入数据库
            fs1402_DataAccess.setDataInfo(dataTable, strOperId);
            return true;

        }
    }
}
