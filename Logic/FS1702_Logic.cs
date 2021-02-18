using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;

namespace Logic
{
    public class FS1702_Logic
    {
        FS1702_DataAccess fs1702_DataAccess;

        public FS1702_Logic()
        {
            fs1702_DataAccess = new FS1702_DataAccess();
        }

        #region 绑定工程
        public DataTable getAllProject()
        {
            return fs1702_DataAccess.getAllProject();
        }
        #endregion

        #region 检索
        public DataTable Search(string vcProject, string dChuHeDateFrom, string dChuHeDateTo)
        {
            return fs1702_DataAccess.Search(vcProject, dChuHeDateFrom, dChuHeDateTo);
        }
        public DataTable Search_jinji(string vcPart_id)
        {
            return fs1702_DataAccess.Search_jinji(vcPart_id);
        }
        #endregion

        #region 按用户文件格式读取数据
        public DataTable ExcelToDataTableformRows(string FileFullName, string sheetName, ref string RetMsg)
        {
            FileStream fs = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            List<int> index = new List<int>();
            DataTable data = new DataTable();
            RetMsg = "";
            int startRow = 0;

            try
            {
                fs = new FileStream(FileFullName, FileMode.Open, FileAccess.Read);

                if (FileFullName.IndexOf(".xlsx") > 0 || FileFullName.IndexOf(".xlsm") > 0) // 2007版本
                    workbook = new XSSFWorkbook(fs);
                else if (FileFullName.IndexOf(".xls") > 0) // 2003版本
                    workbook = new HSSFWorkbook(fs);

                if (sheetName != null)
                {
                    sheet = workbook.GetSheet(sheetName);
                    if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }
                else
                {
                    sheet = workbook.GetSheetAt(0);
                }

                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数

                    //创建Datatable的列
                    for (int i = 0; i < cellCount; i++)
                    {
                        if (i == 0)
                            data.Columns.Add("vcProject");
                        else if (i == 1)
                            data.Columns.Add("vcPart_id");
                        else
                        {
                            ICell temp = firstRow.GetCell(i);
                            if (temp != null) //同理，没有数据的单元格都默认是null
                            {
                                switch (temp.CellType)
                                {
                                    case CellType.Blank:
                                        data.Columns.Add(string.Empty);
                                        break;
                                    case CellType.Numeric:
                                        //NPOI中数字和日期都是NUMERIC类型的，这里对其进行判断是否是日期类型
                                        if (HSSFDateUtil.IsCellDateFormatted(temp))//日期类型
                                        {
                                            var value = temp.NumericCellValue;
                                            data.Columns.Add(DateTime.FromOADate(value).ToString("yyyy-MM-dd"));
                                        }
                                        else//其他数字类型
                                        {
                                            data.Columns.Add(temp.NumericCellValue.ToString());
                                        }
                                        break;
                                    case CellType.String:
                                        data.Columns.Add(temp.StringCellValue);
                                        break;
                                    default:
                                        data.Columns.Add(temp.ToString());
                                        break;
                                }
                            }
                        }
                    }

                    //获取数据首尾行
                    startRow = sheet.FirstRowNum + 1;
                    int rowCount = sheet.LastRowNum;

                    //读取数据
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　

                        DataRow dataRow = data.NewRow();
                        for (int j = 0; j < cellCount; j++)
                        {
                            ICell cell = row.GetCell(j);
                            if (cell != null) //同理，没有数据的单元格都默认是null
                            {
                                switch (cell.CellType)
                                {
                                    case CellType.Blank:
                                        dataRow[j] = string.Empty;
                                        break;
                                    case CellType.Numeric:
                                        //NPOI中数字和日期都是NUMERIC类型的，这里对其进行判断是否是日期类型
                                        if (HSSFDateUtil.IsCellDateFormatted(cell))//日期类型
                                        {
                                            var value = cell.NumericCellValue;
                                            dataRow[j] = DateTime.FromOADate(value).ToString("yyyy-MM-dd HH:mm:ss");
                                        }
                                        else//其他数字类型
                                        {
                                            dataRow[j] = cell.NumericCellValue;
                                        }
                                        break;
                                    case CellType.String:
                                        dataRow[j] = cell.StringCellValue;
                                        break;
                                    default:
                                        dataRow[j] = cell.ToString();
                                        break;
                                }
                            }
                        }

                        data.Rows.Add(dataRow);
                    }
                }

                for (int i = 0; i < data.Columns.Count; i++)
                {
                    data.Columns[i].DataType = typeof(string);
                }
                return data;
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception: " + ex.Message);
                return null;
            }
            finally
            {
                if (workbook != null)
                {
                    workbook.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }

        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId,ref string strErrorName)
        {
            fs1702_DataAccess.importSave(dt, strUserId,ref strErrorName);
        }
        #endregion

        #region 确认单打印
        public void qrdPrint(List<Dictionary<string, Object>> checkedInfoData,string strUserId)
        {
            //更新确认单打印时间
            fs1702_DataAccess.qrdPrint(checkedInfoData, strUserId);
        }
        #endregion

        #region 出荷看板打印
        public void kbPrint(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            //更新出荷看板打印时间
            fs1702_DataAccess.kbPrint(checkedInfoData, strUserId);
        }
        #endregion

        #region 出荷完了
        public void chuheOK(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            //更新出荷完了时间+更新在库
            fs1702_DataAccess.chuheOK(checkedInfoData, strUserId);
        }
        #endregion

        public DataTable GetqrdInfo(string vcProject, string dChuHeDate)
        {
            return fs1702_DataAccess.GetqrdInfo(vcProject, dChuHeDate);
        }
        public DataTable getKBData(string vcProject, string dChuHeDate)
        {
            return fs1702_DataAccess.getKBData(vcProject, dChuHeDate);
        }

        public string generateExcelWithXlt(string vcQueRenNo, DataTable dt, string[] field, string rootPath, string xltName, int sheetindex, int startRow, string strUserId, string strFunctionName)
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

                ISheet sheet = hssfworkbook.GetSheetAt(sheetindex);

                ICellStyle mystyle_querenno = hssfworkbook.CreateCellStyle();
                IFont f = hssfworkbook.CreateFont();
                f.Boldweight = (short)FontBoldWeight.Bold;
                f.FontHeightInPoints = 12;
                mystyle_querenno.SetFont(f);
                mystyle_querenno.Alignment = HorizontalAlignment.Left;
                mystyle_querenno.VerticalAlignment = VerticalAlignment.Center;
                sheet.GetRow(1).GetCell(0).CellStyle = mystyle_querenno;
                sheet.GetRow(1).GetCell(0).SetCellValue("确认单号："+vcQueRenNo);

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

        #region 保存
        public void Save_jinji(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs1702_DataAccess.Save_jinji(listInfoData, strUserId);
        }
        #endregion
    }

}
