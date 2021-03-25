using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.Json;
using System.Threading;
using Common;
using DataAccess;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Logic
{
    public class FS1101_Logic
    {
        FS1101_DataAccess fs1101_DataAccess;

        public FS1101_Logic()
        {
            fs1101_DataAccess = new FS1101_DataAccess();
        }
        public DataTable getSearchInfo(string strPackMaterNo, string strTrolleyNo, string strPartId, string strOrderNo, string strLianFan)
        {
            return fs1101_DataAccess.getSearchInfo(strPackMaterNo, strTrolleyNo, strPartId, strOrderNo, strLianFan);
        }
        public bool getPrintInfo(List<Dictionary<string, Object>> listInfoData, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                DataTable dataTable = fs1101_DataAccess.getPrintTemp("FS1101");
                DataTable dtSub = dataTable.Clone();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string strPackMaterNo = listInfoData[i]["vcPackMaterNo"] == null ? "" : listInfoData[i]["vcPackMaterNo"].ToString();
                    DataTable dtSPInfo = fs1101_DataAccess.getSearchInfo(strPackMaterNo);
                    string uuid = Guid.NewGuid().ToString("N");
                    for (int j = 0; j < dtSPInfo.Rows.Count; j++)
                    {
                        DataRow dataRow = dtSub.NewRow();
                        dataRow["UUID"] = uuid;
                        dataRow["vcTrolleyNo"] = dtSPInfo.Rows[j]["vcTrolleyNo"].ToString();
                        dataRow["vcLotid"] = dtSPInfo.Rows[j]["vcLotid"].ToString();
                        dataRow["vcPackingpartsno"] = dtSPInfo.Rows[j]["vcPackingpartsno"].ToString();
                        dataRow["vcPackinggroup"] = dtSPInfo.Rows[j]["vcPackinggroup"].ToString();
                        dataRow["dQty"] = dtSPInfo.Rows[j]["dQty"].ToString();
                        dataRow["vcPackingpartslocation"] = dtSPInfo.Rows[j]["vcPackingpartslocation"].ToString();
                        dataRow["dPrintDate"] = dtSPInfo.Rows[j]["dPrintDate"].ToString();
                        dataRow["vcCodemage"] = null;
                        dtSub.Rows.Add(dataRow);
                    }
                }
                if (dtSub.Rows.Count == 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "没有有效的断取数据，请确认后再操作。";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage.Rows.Count != 0)
                    return false;
                fs1101_DataAccess.setPrintTemp(dtSub, strOperId, ref dtMessage);
                if (dtMessage.Rows.Count != 0)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet getPrintInfo(string strOperId)
        {
            return fs1101_DataAccess.getPrintInfo(strOperId);
        }
        public string generateExcelWithXlt(DataTable dt, string[] field, string rootPath, string xltName, int sheetindex, int startRow, string strUserId, string strFunctionName)
        {
            try
            {
                string strRiqi = "日期：" + System.DateTime.Now.ToString("yyyy-MM-dd");
                string strTrolleyNo = "台车号：" + dt.Rows[0]["vcTrolleyNo"].ToString();
                string strLotid = "断取指示书号：" + dt.Rows[0]["vcLotid"].ToString();
                XSSFWorkbook hssfworkbook = new XSSFWorkbook();

                string XltPath = rootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Template" + Path.DirectorySeparatorChar + xltName;
                using (FileStream fs = File.OpenRead(XltPath))
                {
                    hssfworkbook = new XSSFWorkbook(fs);
                    fs.Close();
                }

                ISheet sheet = hssfworkbook.GetSheetAt(sheetindex);

                ICellStyle mystyle_querenno = hssfworkbook.CreateCellStyle();
                IFont f = hssfworkbook.CreateFont();
                f.Boldweight = (short)FontBoldWeight.Bold;
                f.FontHeightInPoints = 12;
                mystyle_querenno.SetFont(f);
                mystyle_querenno.Alignment = HorizontalAlignment.Left;
                mystyle_querenno.VerticalAlignment = VerticalAlignment.Center;
                //sheet.GetRow(1).GetCell(0).CellStyle = mystyle_querenno;
                sheet.GetRow(1).GetCell(0).SetCellValue(strRiqi);
                sheet.GetRow(1).GetCell(2).SetCellValue(strTrolleyNo);
                sheet.GetRow(1).GetCell(4).SetCellValue(strLotid);

                ICellStyle mystyle = hssfworkbook.CreateCellStyle();
                mystyle.BorderBottom = BorderStyle.Thin;
                mystyle.BorderLeft = BorderStyle.Thin;
                mystyle.BorderTop = BorderStyle.Thin;
                mystyle.BorderRight = BorderStyle.Thin;
                mystyle.Alignment = HorizontalAlignment.Center;
                mystyle.VerticalAlignment = VerticalAlignment.Center;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(startRow + i);
                    row.HeightInPoints = 25;
                    for (int j = 0; j < field.Length; j++)
                    {
                        Type type = dt.Columns[field[j]].DataType;
                        ICell cell = row.CreateCell(j);
                        cell.CellStyle = mystyle;
                        if (type == Type.GetType("System.Decimal"))
                        {
                            if (dt.Rows[i][field[j]].ToString().Trim() != "")
                                cell.SetCellValue(Convert.ToDouble(dt.Rows[i][field[j]].ToString()));
                        }
                        else if (type == Type.GetType("System.Int32"))
                        {
                            if (dt.Rows[i][field[j]].ToString().Trim() != "")
                                cell.SetCellValue(Convert.ToInt32(dt.Rows[i][field[j]].ToString()));
                        }
                        else if (type == Type.GetType("System.Int16"))
                        {
                            if (dt.Rows[i][field[j]].ToString().Trim() != "")
                                cell.SetCellValue(Convert.ToInt16(dt.Rows[i][field[j]].ToString()));
                        }
                        else if (type == Type.GetType("System.Int64"))
                        {
                            if (dt.Rows[i][field[j]].ToString().Trim() != "")
                                cell.SetCellValue(Convert.ToInt64(dt.Rows[i][field[j]].ToString()));
                        }
                        else
                        {
                            cell.SetCellValue(dt.Rows[i][field[j]].ToString());
                        }
                    }
                }
                ISheet sheet1 = hssfworkbook.GetSheetAt(0);//刷新第一个sheet页的公式
                sheet1.ForceFormulaRecalculation = true;
                string strFileName = strFunctionName + "_打印信息_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + strUserId + "_" + Guid.NewGuid().ToString("N") + ".xlsx";
                string fileSavePath = rootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar;//文件临时目录，导入完成后 删除
                string path = fileSavePath + strFileName;
                using (FileStream fs = File.OpenWrite(path))
                {
                    hssfworkbook.Write(fs);//向打开的这个xls文件中写入数据  
                    fs.Close();
                }
                return path;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }

    }
}
