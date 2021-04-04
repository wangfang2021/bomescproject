using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using DataAccess;
using System.Collections;
using System;
using System.Linq;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Logic
{
    public class FS1206_Logic
    {
        FS1206_DataAccess dataAccess = new FS1206_DataAccess();
        public DataTable Search(string strPartsNo, string mon)
        {
            return dataAccess.Search(strPartsNo, mon);
        }

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            dataAccess.Del(listInfoData, strUserId);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            dataAccess.Save(listInfoData, strUserId, ref strErrorPartId);
        }
        #endregion

        #region 导入后保存
        public string UpdateTable(DataTable dt, string userid)
        {
            return dataAccess.UpdateTable(dt, userid);
        }
        #endregion

        #region 校验数据格式
        public string checkExcelData(DataTable dt)
        {
            string msg = "";
            DataTable dt_chk = new DataTable();
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                string vcPartsNo = dt.Rows[i][0].ToString().Trim().ToUpper();
                string vcPartsNoFZ = dt.Rows[i][1].ToString().Trim().ToUpper();
                string vcSourse = dt.Rows[i][2].ToString().Trim().ToUpper();
                string iSRNum = dt.Rows[i][5].ToString().Trim();
                if (vcPartsNo.Length <= 0)
                {
                    msg = "第" + (i + 2) + "行，品番不能为空。";
                    return msg;
                }
                if (vcPartsNo.Length != 10 && vcPartsNo.Length != 12)
                {
                    msg = "第" + (i + 2) + "行，品番格式不正确。";
                    return msg;
                }
                if (vcPartsNoFZ.Length <= 0)
                {
                    msg = "第" + (i + 2) + "行，发注品番不能为空。";
                    return msg;
                }
                if (vcPartsNoFZ.Length != 10 && vcPartsNoFZ.Length != 12)
                {
                    msg = "第" + (i + 2) + "行，发注品番格式不正确。";
                    return msg;
                }
                if (vcSourse.Length <= 0)
                {
                    msg = "第" + (i + 2) + "行，SOURCE不能为空。";
                    return msg;
                }
                if (iSRNum.Length <= 0)
                {
                    msg = "第" + (i + 2) + "行，收容数不能为空。";
                    return msg;
                }
            }
            return msg;
        }
        #endregion

        public string ExcelPos(int i)//取得列位置
        {
            string re = "error";
            List<string> A = new List<string>();
            A.Add("A");
            A.Add("B");
            A.Add("C");
            A.Add("D");
            A.Add("E");
            A.Add("F");
            A.Add("G");
            A.Add("H");
            A.Add("I");
            A.Add("J");
            A.Add("K");
            A.Add("L");
            A.Add("M");
            A.Add("N");
            A.Add("O");
            A.Add("P");
            A.Add("Q");
            A.Add("R");
            A.Add("S");
            A.Add("T");
            A.Add("U");
            A.Add("V");
            A.Add("W");
            A.Add("X");
            A.Add("Y");
            A.Add("Z");
            if (i < 26) re = A[i];
            if (i >= 26) re = A[(i / 26) - 1] + A[i % 26];
            return re;
        }

        public string InUpdeOldData(DataTable dt, string useid)
        {
            return dataAccess.InUpdeOldData(dt, useid);
        }

        public string generateExcelWithXlt(DataTable dt, string[] field, string rootPath, string xltName, int startRow, string strUserId, string strFunctionName, bool isAlignCenter)
        {
            try
            {
                XSSFWorkbook hssfworkbook = new XSSFWorkbook();

                string XltPath = rootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Template" + Path.DirectorySeparatorChar + xltName;
                using (FileStream fs = File.OpenRead(XltPath))
                {
                    hssfworkbook = new XSSFWorkbook(fs);
                    fs.Close();
                }

                ISheet sheet = hssfworkbook.GetSheetAt(0);
                ICellStyle style = hssfworkbook.CreateCellStyle();
                style.Alignment = HorizontalAlignment.Center;
                style.BorderDiagonalLineStyle = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(startRow + i);
                    for (int j = 0; j < field.Length; j++)
                    {
                        Type type = dt.Columns[field[j]].DataType;
                        ICell cell = row.CreateCell(j);
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
                        if (isAlignCenter)
                            cell.CellStyle = style;
                    }
                }
                string strFileName = strFunctionName + "_导出信息_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + strUserId + ".xlsx";
                string fileSavePath = rootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar;//文件临时目录，导入完成后 删除
                string path = fileSavePath + strFileName;
                using (FileStream fs = File.OpenWrite(path))
                {
                    hssfworkbook.Write(fs);//向打开的这个xls文件中写入数据  
                    fs.Close();
                }
                return strFileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }
    }
}