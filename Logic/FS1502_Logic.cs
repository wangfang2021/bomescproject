using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;

namespace Logic
{
    public class FS1502_Logic
    {
        FS1502_DataAccess fs1502_DataAccess;

        public FS1502_Logic()
        {
            fs1502_DataAccess = new FS1502_DataAccess();
        }

        #region 检索
        public DataTable Search(string dBZDate)
        {
            return fs1502_DataAccess.Search(dBZDate);
        }
        #endregion

        #region 检索_报表
        public DataTable Search_report(string dBZDate)
        {
            return fs1502_DataAccess.Search_report(dBZDate);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs1502_DataAccess.Save(listInfoData, strUserId);
        }
        #endregion

        #region 读取txt文件
        public DataTable ReadTxtFile(string FileName, ref string strMsg)
        {
            //从指定的目录以打开或者创建的形式读取csv文件
            FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            //为上面创建的文件流创建读取数据流
            StreamReader read = new StreamReader(fs, System.Text.Encoding.Default);
            try
            {
                DataTable dt = new DataTable();
                Dictionary<string, int> dict = new Dictionary<string, int>();//<字段名(string),字段名对应的列号(int)>
                int irowNo = 0;
                read.BaseStream.Seek(0, SeekOrigin.Begin);
                while (read.Peek() > -1)
                {
                    irowNo = irowNo + 1;
                    string line = read.ReadLine();
                    if (irowNo == 1)
                    {//标题行
                        string[] sArray = Regex.Split(line, ",", RegexOptions.IgnoreCase);
                        for (int i = 0; i < sArray.Length; i++)
                        {
                            string strColumnName = sArray[i].ToString();
                            if (strColumnName == "SUPL" || strColumnName == "PLANT" || strColumnName == "DOCK" || strColumnName == "UNLD DTE" ||
                                strColumnName == "FRQ" || strColumnName == "PART #" || strColumnName == "FINL ORD(PCS)")
                            {
                                if (dt.Columns.Contains(strColumnName) == false)
                                {
                                    dt.Columns.Add(strColumnName);
                                    dict.Add(strColumnName, i);
                                }
                            }
                        }
                    }
                    if (irowNo >= 2)
                    {//数据行
                        DataRow dr = dt.NewRow();
                        string[] sArray = Regex.Split(line, ",", RegexOptions.IgnoreCase);
                        if (sArray.Length == 1 && sArray[0] == "")
                        {
                            //有时txt文件最后一行是空行，会导致处理失败
                        }
                        else
                        {
                            for (int i = 0; i < sArray.Length; i++)
                            {
                                foreach (KeyValuePair<string, int> kvp in dict)
                                {
                                    string columnname = kvp.Key;//字段名
                                    int columnindex = kvp.Value;//字段名对应的列号
                                    if (i == columnindex)
                                    {
                                        dr[columnname] = sArray[i].ToString();
                                    }
                                }
                            }
                            if (Convert.ToInt32(dr["FINL ORD(PCS)"].ToString()) > 0)
                            { //只有数量>0才加入到dt中
                                dt.Rows.Add(dr);
                            }
                        }
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return null;
            }
            finally
            {
                read.Close();
                fs.Close();
            }
        }
        #endregion

        #region 导入后保存
        public void importSave_Sub(DataTable dt, string vcFZPlant, string dBZDate, string strUserId)
        {
            fs1502_DataAccess.importSave_Sub(dt, vcFZPlant, dBZDate, strUserId);
        }
        #endregion

        #region 计算
        public void Cal(string dBZDate, string strUserId)
        {
            fs1502_DataAccess.Cal(dBZDate, strUserId);
        }
        #endregion

        #region 导出带模板
        public string generateExcelWithXlt(DataTable dt, string[] field, string rootPath, string xltName, int startRow, string strUserId, string strFunctionName)
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

                //设置合并后居中样式
                ICellStyle cellstyle = hssfworkbook.CreateCellStyle();
                cellstyle.VerticalAlignment = VerticalAlignment.Center;
                cellstyle.Alignment = HorizontalAlignment.Center;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(startRow + i);
                    for (int j = 0; j < field.Length; j++)
                    {
                        string value = dt.Rows[i][field[j]].ToString();
                        ICell cell = row.CreateCell(j);
                        if (j == 0)
                            cell.SetCellValue(value);
                        else
                            cell.SetCellValue(Convert.ToInt32(value==""?"0":value));//设置数字格式
                    }
                    string strBigPM = dt.Rows[i]["vcBigPM"].ToString();
                    if (strBigPM.Contains("合计"))
                    {
                        //合并操作
                        sheet.AddMergedRegion(new CellRangeAddress(startRow + i, startRow + i, 0, 6));//起始行，结束行，起始列，结束列
                        // 设置合并后style
                        var cell = sheet.GetRow(startRow + i).GetCell(0);
                        cell.CellStyle = cellstyle;
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
                return "";
            }
        }
        #endregion

        #region 导出带模板_报表
        public string generateExcelWithXlt_report(DataTable dt, string[] field, string rootPath, string xltName, int startRow, string strUserId, string strFunctionName)
        {
            try
            {
                HSSFWorkbook hssfworkbook = new HSSFWorkbook();
                string XltPath = rootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Template" + Path.DirectorySeparatorChar + xltName;
                using (FileStream fs = File.OpenRead(XltPath))
                {
                    hssfworkbook = new HSSFWorkbook(fs);
                    fs.Close();
                }
                ISheet sheet = hssfworkbook.GetSheet("数据源");
                ICellStyle dateStyle = hssfworkbook.CreateCellStyle();
                IDataFormat dataFormat = hssfworkbook.CreateDataFormat();
                dateStyle.DataFormat = dataFormat.GetFormat("d日");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(startRow + i);
                    for (int j = 0; j < field.Length; j++)
                    {
                        ICell cell = row.CreateCell(j);
                        if (j == 3)
                            cell.SetCellValue(Convert.ToInt32(dt.Rows[i][field[j]].ToString()));//设置数字格式
                        else if(j==1)
                        {//设置日期格式
                            cell.CellStyle = dateStyle;
                            cell.SetCellValue(Convert.ToDateTime(dt.Rows[i][field[j]].ToString()));
                        }
                        else
                            cell.SetCellValue(dt.Rows[i][field[j]].ToString());
                    }
                }
                //源行数赋值
                ICell cell2 = sheet.GetRow(1).CreateCell(4);
                cell2.SetCellValue(dt.Rows.Count + 1);

                string strFileName = strFunctionName + "_导出信息_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + strUserId + ".xls";
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
                return "";
            }
        }
        #endregion
    }
}
