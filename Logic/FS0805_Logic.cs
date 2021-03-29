using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using NPOI.XSSF.UserModel;
using System.IO;
using NPOI.SS.UserModel;

namespace Logic
{
    public class FS0805_Logic
    {
        FS0805_DataAccess fs0805_DataAccess;
        FS0617_Logic fS0617_Logic = new FS0617_Logic();

        public FS0805_Logic()
        {
            fs0805_DataAccess = new FS0805_DataAccess();
        }
        public DataTable getSearchInfo(string strSellNo)
        {
            return fs0805_DataAccess.getSearchInfo(strSellNo, "Info");
        }
        public bool getPrintInfo(string strSellNo, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                DataTable dataTable = fs0805_DataAccess.getPrintTemp("FS0805");
                DataTable dtSub = dataTable.Clone();
                DataTable dtSPInfo = fs0805_DataAccess.getSearchInfo(strSellNo, "List");
                string uuid = Guid.NewGuid().ToString("N");
                for (int j = 0; j < dtSPInfo.Rows.Count; j++)
                {
                    DataRow dataRow = dtSub.NewRow();
                    dataRow["UUID"] = uuid;
                    dataRow["vcRows"] = (j + 1).ToString();
                    dataRow["vcCpdcompany"] = dtSPInfo.Rows[j]["vcCpdcompany"].ToString();
                    dataRow["vcCompany"] = dtSPInfo.Rows[j]["vcCompany"].ToString();
                    dataRow["vcPackingspot"] = dtSPInfo.Rows[j]["vcPackingspot"].ToString();
                    dataRow["vcControlno"] = dtSPInfo.Rows[j]["vcControlno"].ToString();
                    dataRow["vcPartsno"] = dtSPInfo.Rows[j]["vcPartsno"].ToString();
                    dataRow["vcOrderno"] = dtSPInfo.Rows[j]["vcOrderno"].ToString();
                    dataRow["vcSeqno"] = dtSPInfo.Rows[j]["vcSeqno"].ToString();
                    dataRow["vcInvoiceno"] = dtSPInfo.Rows[j]["vcInvoiceno"].ToString();
                    dataRow["vcPartsnamechn"] = dtSPInfo.Rows[j]["vcPartsnamechn"].ToString();
                    dataRow["vcPartsnameen"] = dtSPInfo.Rows[j]["vcPartsnameen"].ToString();
                    dataRow["vcShippingqty"] = dtSPInfo.Rows[j]["vcShippingqty"].ToString();
                    dataRow["vcCaseno"] = dtSPInfo.Rows[j]["vcCaseno"].ToString().Length == 8 ?
                        dtSPInfo.Rows[j]["vcCaseno"].ToString().Substring(0, 4) + "-" + dtSPInfo.Rows[j]["vcCaseno"].ToString().Substring(4, 4) :
                        dtSPInfo.Rows[j]["vcCaseno"].ToString();
                    dataRow["vcCostwithtaxes"] = dtSPInfo.Rows[j]["vcCostwithtaxes"].ToString();
                    dataRow["vcPrice"] = dtSPInfo.Rows[j]["vcPrice"].ToString();
                    dataRow["dPrintDate"] = dtSPInfo.Rows[j]["dPrintDate"].ToString();
                    dataRow["vcCodemage"] = null;
                    dtSub.Rows.Add(dataRow);
                }
                if (dtSub.Rows.Count == 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "没有有效的发货明细数据，请确认后再操作。";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage.Rows.Count != 0)
                    return false;
                fs0805_DataAccess.setPrintTemp(dtSub, strOperId, ref dtMessage);
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

        public DataTable getPrintInfo(string strOperId)
        {
            return fs0805_DataAccess.getPrintInfo(strOperId);
        }
        public string generateExcelWithXlt(DataTable dt, string[] field, string rootPath, string xltName, int sheetindex, int startRow, string strUserId, string strFunctionName)
        {
            try
            {
                string strCpdcompany = "出货商名：" + dt.Rows[0]["vcCpdcompany"].ToString();
                string strCompany = "出货商代码：" + dt.Rows[0]["vcCompany"].ToString();
                string strPackingspot = "包装场：" + dt.Rows[0]["vcPackingspot"].ToString();
                string strRiqi = "印刷日：" + System.DateTime.Now.ToString("yyyy-MM-dd");
                string strControlno = "出货明细番号：" + dt.Rows[0]["vcControlno"].ToString();
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
                sheet.GetRow(1).GetCell(0).SetCellValue(strCpdcompany);
                sheet.GetRow(2).GetCell(0).SetCellValue(strCompany);
                sheet.GetRow(2).GetCell(4).SetCellValue(strPackingspot);
                sheet.GetRow(1).GetCell(8).SetCellValue(strRiqi);
                sheet.GetRow(2).GetCell(8).SetCellValue(strControlno);

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
