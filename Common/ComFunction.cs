using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Common
{
    public class ComFunction
    {
        #region MD5密码加密
        public static string encodePwd(string strPwd)
        {
            if (strPwd == null || strPwd.Trim() == "")
                return "";
            byte[] result = Encoding.Default.GetBytes(strPwd);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            return BitConverter.ToString(output).Replace("-", "");
        }
        #endregion

        #region 数据转换json结果
        public static List<Object> convertToResult(DataTable dt, string[] fields)
        {


            List<Object> res = new List<Object>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Dictionary<string, object> row = new Dictionary<string, object>();
                foreach (string field in fields)
                {
                    row[field] = dt.Rows[i][field];
                }
                row["iAPILineNo"] = i;
                res.Add(row);
            }
            return res;
        }
        public static List<Object> convertAllToResult(DataTable dt)
        {


            List<Object> res = new List<Object>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Dictionary<string, object> row = new Dictionary<string, object>();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string colName = dt.Columns[j].ColumnName;
                    row[colName] = dt.Rows[i][colName];
                }
                row["iAPILineNo"] = i;
                res.Add(row);
            }
            return res;
        }
        /// <summary>
        /// 带格式转换的datatable转json
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="boolFields"></param>
        /// <returns></returns>
        public static List<Object> convertAllToResultByConverter(DataTable dt, DtConverter dtConverter)
        {
            List<Object> res = new List<Object>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Dictionary<string, object> row = new Dictionary<string, object>();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string colName = dt.Columns[j].ColumnName;
                    row[colName] = dtConverter.doConvert(dt.Rows[i][colName], colName);
                }
                row["iAPILineNo"] = i;
                res.Add(row);
            }
            return res;
        }
        public static List<Object> convertAllToResultByConverter_main(DataTable dtMain, DataTable dtChild, DtConverter dtConverter)
        {
            List<Object> res = new List<Object>();
            for (int i = 0; i < dtMain.Rows.Count; i++)
            {
                Dictionary<string, object> row = new Dictionary<string, object>();
                row["iAPILineNo"] = i.ToString();
                for (int j = 0; j < dtMain.Columns.Count; j++)
                {
                    string colName = dtMain.Columns[j].ColumnName;
                    if (colName == "children")
                    {
                        string strChidren = dtMain.Rows[i][colName].ToString();
                        DataRow[] drChild = dtChild.Select("children='" + strChidren + "'");
                        if (drChild.Length > 0)
                        {
                            row[colName] = convertAllToResultByConverter_child(strChidren, dtMain, drChild, i, dtConverter);
                        }
                    }
                    else
                    {
                        row[colName] = dtConverter.doConvert(dtMain.Rows[i][colName], colName);
                    }
                }
                res.Add(row);
            }
            return res;
        }
        public static List<Object> convertAllToResultByConverter_child(string strChidren, DataTable dtMain, DataRow[] drChild, int index, DtConverter dtConverter)
        {
            List<Object> res = new List<Object>();
            DataTable dtChild = dtMain.Clone();
            for (int i = 0; i < drChild.Length; i++)
            {
                dtChild.ImportRow(drChild[i]);
            }
            for (int i = 0; i < dtChild.Rows.Count; i++)
            {
                Dictionary<string, object> row = new Dictionary<string, object>();
                row["iAPILineNo"] = index.ToString() + (i + 1000).ToString().Substring(1, 3);
                for (int j = 0; j < dtChild.Columns.Count; j++)
                {
                    string colName = dtChild.Columns[j].ColumnName;
                    if (colName != "children")
                    {
                        row[colName] = dtConverter.doConvert(dtChild.Rows[i][colName], colName);
                    }
                }
                res.Add(row);
            }
            return res;
        }
        #endregion

        #region 根据Datatable某个字段，把“值”的集合转换json结果
        public static List<Object> convertToResult(DataTable dt, string field)
        {
            List<Object> res = new List<Object>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string strTemp = dt.Rows[i][field] == System.DBNull.Value ? "" : dt.Rows[i][field].ToString();
                res.Add(strTemp);
            }
            return res;
        }
        #endregion

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Text">要加密的文本</param>
        /// <param name="sKey">秘钥</param>
        /// <returns></returns>
        public static string Encrypt(string Text)
        {
            string sKey = ComConstant.SERVER_KEY;
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray;
            inputByteArray = Encoding.Default.GetBytes(Text);
            des.Key = ASCIIEncoding.ASCII.GetBytes(Md5Hash(sKey).Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(Md5Hash(sKey).Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string Decrypt(string Text)
        {
            string sKey = ComConstant.SERVER_KEY;
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            int len;
            len = Text.Length / 2;
            byte[] inputByteArray = new byte[len];
            int x, i;
            for (x = 0; x < len; x++)
            {
                i = Convert.ToInt32(Text.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            des.Key = ASCIIEncoding.ASCII.GetBytes(Md5Hash(sKey).Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(Md5Hash(sKey).Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());


        }
        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string Md5Hash(string input)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        #region Excel操作

        #region VBA调用
        /// <summary>
        /// 调用xslm中宏
        /// </summary>
        /// <param name="app"></param>
        /// <param name="macroName">宏名称</param>
        /// <param name="parameters">宏所需的参数</param>
        /// <param name="rtnValue"></param>
        public static void RunExcelMacro(Microsoft.Office.Interop.Excel.Application app, string macroName, object[] parameters, out object rtnValue)
        {
            // 根据参数组是否为空，准备参数组对象
            object[] paraObjects;
            if (parameters == null)
                paraObjects = new object[] { macroName };
            else
            {
                int paraLength = parameters.Length;
                paraObjects = new object[paraLength + 1];
                paraObjects[0] = macroName;
                for (int i = 0; i < paraLength; i++)
                    paraObjects[i + 1] = parameters[i];
            }
            rtnValue = RunMacro(app, paraObjects);
        }

        /// <summary>
        /// 执行宏
        /// </summary>
        /// <param name="oApp">Excel对象</param>
        /// <param name="oRunArgs">参数（第一个参数为指定宏名称，后面为指定宏的参数值）</param>
        /// <returns>宏返回值</returns>
        private static object RunMacro(object app, object[] oRunArgs)
        {
            object objRtn;     // 声明一个返回对象

            // 反射方式执行宏
            objRtn = app.GetType().InvokeMember("Run", System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.InvokeMethod, null, app, oRunArgs);

            return objRtn;
        }

        #endregion

        #region 导入
        /// <summary>
        /// 导入标准Excel，第一行为列头
        /// </summary>
        /// <param name="FileFullName">文件完整路径</param>
        /// <param name="sheetName">指定sheet页名称，如未匹配则默认选择第一个sheet</param>
        /// <param name="Header">5个数组,第一个为文件中列头，第二个为对应DataTable列头，第三个对应校验数据类型，第四个为最大长度限定（为0不校验），第五个为最小长度限定（为0不校验）</param>
        /// <param name="RetMsg">返回信息</param>
        /// <returns></returns>
        public static DataTable ExcelToDataTable(string FileFullName, string sheetName, string[,] Header, ref string RetMsg)
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

                    //对应索引
                    for (int i = 0; i < Header.GetLength(1); i++)
                    {
                        bool bFound = false;
                        for (int j = 0; j < cellCount; j++)
                        {
                            ICell cell = firstRow.GetCell(j);
                            string cellValue = cell.StringCellValue;
                            if (!string.IsNullOrEmpty(cellValue))
                            {
                                if (Header[0, i] == cellValue)
                                {
                                    bFound = true;
                                    index.Add(j);
                                    break;
                                }
                            }
                        }

                        if (bFound == false)
                        {
                            RetMsg = Header[0, i] + "列不存在";
                            return null;
                        }
                    }

                    //创建Datatable的列
                    for (int i = 0; i < Header.GetLength(1); i++)
                    {
                        data.Columns.Add(Header[1, i].ToString().Trim());
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
                        for (int j = 0; j < Header.GetLength(1); j++)
                        {
                            ICell cell = row.GetCell(index[j]);
                            if (cell != null) //同理，没有数据的单元格都默认是null
                            {

                                if (cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell))
                                {
                                    dataRow[j] = DateTime.FromOADate(cell.NumericCellValue);
                                }
                                else
                                {
                                    dataRow[j] = cell.ToString();
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

                #region 校验格式

                for (int i = 0; i < data.Rows.Count; i++)
                {
                    DataRow dr = data.Rows[i];
                    for (int j = 0; j < Header.GetLength(1); j++)
                    {
                        if (Convert.ToInt32(Header[3, j]) > 0 &&
                            dr[Header[1, j]].ToString().Length > Convert.ToInt32(Header[3, j]))
                        {
                            RetMsg = string.Format("第{0}行{1}大于设定长度", i + 2, Header[0, j]);
                            return null;
                        }

                        if (Convert.ToInt32(Header[4, j]) > 0 &&
                            dr[Header[1, j]].ToString().Length < Convert.ToInt32(Header[4, j]))
                        {
                            RetMsg = string.Format("第{0}行{1}小于设定长度", i + 2, Header[0, j]);
                            return null;
                        }

                        switch (Header[2, j])
                        {
                            case "decimal":
                                if (dr[Header[1, j]] != null && dr[Header[1, j]].ToString() != "" && !CheckDecimal(dr[Header[1, j]].ToString()))
                                {
                                    RetMsg = string.Format("第{0}行{1}不是合法数值", i + 2, Header[0, j]);
                                    return null;
                                }

                                break;
                            case "d":
                                if (dr[Header[1, j]] != null && dr[Header[1, j]].ToString() != "" && !CheckDate(dr[Header[1, j]].ToString()))
                                {
                                    RetMsg = string.Format("第{0}行{1}不是合法日期", i + 2, Header[0, j]);
                                    return null;
                                }

                                break;
                            case "ym":
                                if (dr[Header[1, j]] != null && dr[Header[1, j]].ToString() != "" && !CheckYearMonth(dr[Header[1, j]].ToString()))
                                {
                                    RetMsg = string.Format("第{0}行{1}不是合法日期", i + 2, Header[0, j]);
                                    return null;
                                }

                                break;
                            default:
                                if (Header[2, j].Length > 0 && Regex.Match(dr[Header[1, j]].ToString(), Header[2, j],
                                    RegexOptions.None).Success)
                                {
                                    RetMsg = string.Format("第{0}行{1}有非法字符", i + 2, Header[0, j]);
                                    return null;
                                }

                                break;
                        }
                    }
                }


                #endregion


                return data;
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception: " + ex.Message);
                throw ex;
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

        /// <summary>
        /// 导入标准Excel，第一行为列头
        /// </summary>
        /// <param name="FileFullName">文件完整路径</param>
        /// <param name="sheetName">指定sheet页名称，如未匹配则默认选择第一个sheet</param>
        /// <param name="Header">5个数组,第一个为文件中列头，第二个为对应DataTable列头，第三个对应校验数据类型，第四个为最大长度限定（为0不校验），第五个为最小长度限定（为0不校验）</param>
        /// <param name="RetMsg">返回信息</param>
        /// <returns></returns>
        public static DataTable ExcelToDataTableformRows(string FileFullName, string sheetName, string[,] Header, int heardrow, int datarow, ref string RetMsg)
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

                    //对应索引
                    for (int i = 0; i < Header.GetLength(1); i++)
                    {
                        bool bFound = false;
                        for (int j = 0; j < cellCount; j++)
                        {
                            ICell cell = firstRow.GetCell(j);
                            string cellValue = cell.StringCellValue;
                            if (!string.IsNullOrEmpty(cellValue))
                            {
                                if (Header[0, i] == cellValue)
                                {
                                    bFound = true;
                                    index.Add(j);
                                    break;
                                }
                            }
                        }

                        if (bFound == false)
                        {
                            RetMsg = Header[0, i] + "列不存在";
                            return null;
                        }
                    }

                    //创建Datatable的列
                    for (int i = 0; i < Header.GetLength(1); i++)
                    {
                        data.Columns.Add(Header[1, i].ToString().Trim());
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
                        for (int j = 0; j < Header.GetLength(1); j++)
                        {
                            ICell cell = row.GetCell(index[j]);
                            if (cell != null) //同理，没有数据的单元格都默认是null
                            {
                                switch (cell.CellType)
                                {
                                    case CellType.Blank:
                                        dataRow[j] = string.Empty;
                                        break;
                                    case CellType.Numeric:
                                        //short format = cell.CellStyle.DataFormat;
                                        ////对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理***
                                        //if (format == 14 || format == 31 || format == 57 || format == 58 || format == 20 || format == 22)
                                        //    dataRow[j] = cell.DateCellValue.ToString("yyyy-MM-dd HH:mm:ss");
                                        //else
                                        //    dataRow[j] = cell.NumericCellValue;
                                        //NPOI中数字和日期都是NUMERIC类型的，这里对其进行判断是否是日期类型
                                        if (HSSFDateUtil.IsCellDateFormatted(cell))//日期类型
                                        {
                                            //dataRow[j] = cell.DateCellValue;
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
                                    //case CellType.Formula:
                                    //    HSSFFormulaEvaluator eva = new HSSFFormulaEvaluator(workbook);
                                    //    dataRow[j] = eva.Evaluate(cell).StringValue;
                                    //    break;
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

        public static DataTable ExcelToDataTable(string FileFullName, string sheetName, string[,] Header, int startRow, ref string RetMsg)
        {
            FileStream fs = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            List<int> index = new List<int>();
            DataTable data = new DataTable();
            RetMsg = "";
            ////int startRow = 0;
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

                    //对应索引
                    for (int i = 0; i < Header.GetLength(1); i++)
                    {
                        bool bFound = false;
                        for (int j = 0; j < cellCount; j++)
                        {
                            ICell cell = firstRow.GetCell(j);
                            //string cellValue = cell.StringCellValue;
                            string cellValue = Header[0, j];
                            if (!string.IsNullOrEmpty(cellValue))
                            {
                                if (Header[0, i] == cellValue && i == j)
                                {
                                    bFound = true;
                                    index.Add(j);
                                    break;
                                }
                            }
                        }

                        if (bFound == false)
                        {
                            RetMsg = Header[0, i] + "列不存在";
                            return null;
                        }
                    }

                    //创建Datatable的列
                    for (int i = 0; i < Header.GetLength(1); i++)
                    {
                        data.Columns.Add(Header[1, i].ToString().Trim());
                    }

                    //获取数据首尾行
                    //startRow = sheet.FirstRowNum + 1;
                    int rowCount = sheet.LastRowNum;

                    //读取数据
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　

                        DataRow dataRow = data.NewRow();
                        for (int j = 0; j < Header.GetLength(1); j++)
                        {
                            ICell cell = row.GetCell(index[j]);
                            if (cell != null) //同理，没有数据的单元格都默认是null
                            {

                                if (cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell))
                                {
                                    dataRow[j] = DateTime.FromOADate(cell.NumericCellValue);
                                }
                                else
                                {
                                    dataRow[j] = cell.ToString();
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

                #region 校验格式

                for (int i = 0; i < data.Rows.Count; i++)
                {
                    DataRow dr = data.Rows[i];
                    for (int j = 0; j < Header.GetLength(1); j++)
                    {
                        if (Convert.ToInt32(Header[3, j]) > 0 &&
                            dr[Header[1, j]].ToString().Length > Convert.ToInt32(Header[3, j]))
                        {
                            RetMsg = string.Format("第{0}行{1}大于设定长度", i + 2, Header[0, j]);
                            return null;
                        }

                        if (Convert.ToInt32(Header[4, j]) > 0 &&
                            dr[Header[1, j]].ToString().Length < Convert.ToInt32(Header[4, j]))
                        {
                            RetMsg = string.Format("第{0}行{1}小于设定长度", i + 2, Header[0, j]);
                            return null;
                        }

                        switch (Header[2, j])
                        {
                            case "decimal":
                                if (Convert.ToInt32(Header[4, j]) > 0 && !CheckDecimal(dr[Header[1, j]].ToString()))
                                {
                                    RetMsg = string.Format("第{0}行{1}不是合法数值", i + 2, Header[0, j]);
                                    return null;
                                }

                                break;
                            case "d":
                                if (Convert.ToInt32(Header[4, j]) > 0 && !CheckDate(dr[Header[1, j]].ToString()))
                                {
                                    RetMsg = string.Format("第{0}行{1}不是合法日期", i + 2, Header[0, j]);
                                    return null;
                                }

                                break;
                            case "ym":
                                if (Convert.ToInt32(Header[4, j]) > 0 && !CheckYearMonth(dr[Header[1, j]].ToString()))
                                {
                                    RetMsg = string.Format("第{0}行{1}不是合法日期", i + 2, Header[0, j]);
                                    return null;
                                }

                                break;
                            default:
                                if (Header[2, j].Length > 0 && Regex.Match(dr[Header[1, j]].ToString(), Header[2, j],
                                    RegexOptions.None).Success)
                                {
                                    RetMsg = string.Format("第{0}行{1}有非法字符", i + 2, Header[0, j]);
                                    return null;
                                }

                                break;
                        }
                    }
                }


                #endregion


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

        /// <summary>
        /// 导入Excel（可以导入含有重复标题行）
        /// </summary>
        /// <param name="FileFullName"></param>
        /// <param name="sheetName"></param>
        /// <param name="Header"></param>
        /// <param name="RetMsg"></param>
        /// <returns></returns>
        public static DataTable ImportExcel(string FileFullName, string sheetName, string[,] Header, ref string RetMsg)
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

                    //对应索引
                    for (int i = 0; i < Header.GetLength(1); i++)
                    {
                        bool bFound = false;
                        for (int j = 0; j < cellCount; j++)
                        {
                            ICell cell = firstRow.GetCell(j);
                            string cellValue = cell.StringCellValue;
                            if (!string.IsNullOrEmpty(cellValue))
                            {
                                if (Header[0, i] == cellValue && i == j)
                                {
                                    bFound = true;
                                    index.Add(j);
                                    break;
                                }
                            }
                        }

                        if (bFound == false)
                        {
                            RetMsg = Header[0, i] + "列不存在";
                            return null;
                        }
                    }

                    //创建Datatable的列
                    for (int i = 0; i < Header.GetLength(1); i++)
                    {
                        data.Columns.Add(Header[1, i].ToString().Trim());
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
                        for (int j = 0; j < Header.GetLength(1); j++)
                        {
                            ICell cell = row.GetCell(index[j]);
                            if (cell != null) //同理，没有数据的单元格都默认是null
                            {

                                if (cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell))
                                {
                                    dataRow[j] = DateTime.FromOADate(cell.NumericCellValue);
                                }
                                else
                                {
                                    dataRow[j] = cell.ToString();
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
                #region 校验格式

                for (int i = 0; i < data.Rows.Count; i++)
                {
                    DataRow dr = data.Rows[i];
                    for (int j = 0; j < Header.GetLength(1); j++)
                    {
                        if (Convert.ToInt32(Header[3, j]) > 0 &&
                            dr[Header[1, j]].ToString().Length > Convert.ToInt32(Header[3, j]))
                        {
                            RetMsg = string.Format("第{0}行{1}大于设定长度", i + 2, Header[0, j]);
                            return null;
                        }

                        if (Convert.ToInt32(Header[4, j]) > 0 &&
                            dr[Header[1, j]].ToString().Length < Convert.ToInt32(Header[4, j]))
                        {
                            RetMsg = string.Format("第{0}行{1}小于设定长度", i + 2, Header[0, j]);
                            return null;
                        }

                        switch (Header[2, j])
                        {
                            case "decimal":
                                if (Convert.ToInt32(Header[4, j]) > 0 && !CheckDecimal(dr[Header[1, j]].ToString()))
                                {
                                    RetMsg = string.Format("第{0}行{1}不是合法数值", i + 2, Header[0, j]);
                                    return null;
                                }

                                break;
                            case "d":
                                if (Convert.ToInt32(Header[4, j]) > 0 && !CheckDate(dr[Header[1, j]].ToString()))
                                {
                                    RetMsg = string.Format("第{0}行{1}不是合法日期", i + 2, Header[0, j]);
                                    return null;
                                }

                                break;
                            case "ym":
                                if (Convert.ToInt32(Header[4, j]) > 0 && !CheckYearMonth(dr[Header[1, j]].ToString()))
                                {
                                    RetMsg = string.Format("第{0}行{1}不是合法日期", i + 2, Header[0, j]);
                                    return null;
                                }

                                break;
                            default:
                                if (Header[2, j].Length > 0 && Regex.Match(dr[Header[1, j]].ToString(), Header[2, j],
                                    RegexOptions.None).Success)
                                {
                                    RetMsg = string.Format("第{0}行{1}有非法字符", i + 2, Header[0, j]);
                                    return null;
                                }

                                break;
                        }
                    }
                }


                #endregion


                return data;
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception: " + ex.Message);
                throw ex;
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



        #region 导出
        /// <summary>
        /// DataTable导出为Excel
        /// </summary>
        /// <param name="head">Excel列头</param>
        /// <param name="field">DataTable列头</param>
        /// <param name="dt">DataTable</param>
        /// <param name="mapPath">匹配路径</param>
        /// <param name="responserid"></param>
        /// <param name="strFunctionName"></param>
        /// <param name="RetMsg"></param>
        /// <returns></returns>
        public static string DataTableToExcel(string[] head, string[] field, DataTable dt, string rootPath, string strUserId, string strFunctionName, ref string RetMsg)
        {
            bool result = false;
            RetMsg = "";
            FileStream fs = null;
            int size = 1048576 - 1;

            string strFileName = strFunctionName + "_导出信息_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + strUserId + ".xlsx";
            string fileSavePath = rootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar;//文件临时目录，导入完成后 删除

            string path = fileSavePath + strFileName;

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            try
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    int page = dt.Rows.Count / size;
                    IWorkbook workbook = new XSSFWorkbook();
                    for (int i = 0; i < page + 1; i++)
                    {
                        string sheetname = "Sheet" + (i + 1).ToString();
                        ISheet sheet = workbook.CreateSheet(sheetname);
                        int rowCount = dt.Rows.Count - i * size > size ? size : dt.Rows.Count - i * size;//行数  
                        int columnCount = dt.Columns.Count;//列数  

                        //设置列头  
                        IRow row = sheet.CreateRow(0);
                        ICell cell;
                        for (int h = 0; h < head.Length; h++)
                        {
                            cell = row.CreateCell(h);
                            cell.SetCellValue(head[h]);
                        }
                        List<ICellStyle> styles = new List<ICellStyle>();
                        //设置每列单元格属性
                        for (int h = 0; h < field.Length; h++)
                        {
                            Type type = dt.Columns[field[h]].DataType;
                            ICellStyle dateStyle = workbook.CreateCellStyle();
                            IDataFormat dataFormat = workbook.CreateDataFormat();
                            if (type == Type.GetType("System.DateTime"))
                            {
                                dateStyle.DataFormat = dataFormat.GetFormat("yyyy-m-d hh:mm:ss");
                            }
                            else if (type == Type.GetType("System.Decimal"))
                            {
                                dateStyle.DataFormat = dataFormat.GetFormat("General");
                            }
                            else if (type == Type.GetType("System.Int32"))
                            {
                                dateStyle.DataFormat = dataFormat.GetFormat("General");
                            }
                            else if (type == Type.GetType("System.Int16"))
                            {
                                dateStyle.DataFormat = dataFormat.GetFormat("General");
                            }
                            else if (type == Type.GetType("System.Int64"))
                            {
                                dateStyle.DataFormat = dataFormat.GetFormat("General");
                            }
                            else if (type == Type.GetType("System.String") && field[h].StartsWith("d") && !field[h].StartsWith("dec"))
                            {
                                dateStyle.DataFormat = dataFormat.GetFormat("@");
                            }
                            else if (type == Type.GetType("System.String") && field[h].StartsWith("vc"))
                            {
                                dateStyle.DataFormat = dataFormat.GetFormat("@");
                            }
                            else if (type == Type.GetType("System.String"))
                            {
                                dateStyle.DataFormat = dataFormat.GetFormat("General");
                            }
                            styles.Add(dateStyle);
                        }
                        //设置每行每列的单元格,  
                        for (int j = 0; j < rowCount; j++)
                        {
                            row = sheet.CreateRow(j + 1);
                            for (int l = 0; l < field.Length; l++)
                            {
                                Type type = dt.Columns[field[l]].DataType;
                                cell = row.CreateCell(l);//excel第二行开始写入数据 
                                cell.CellStyle = styles[l];
                                if (type == Type.GetType("System.Decimal"))
                                {
                                    if (dt.Rows[j][field[l]].ToString().Trim() != "")
                                        cell.SetCellValue(Convert.ToDouble(dt.Rows[j + i * size][field[l]].ToString()));
                                }
                                else if (type == Type.GetType("System.Int32"))
                                {
                                    if (dt.Rows[j][field[l]].ToString().Trim() != "")
                                        cell.SetCellValue(Convert.ToInt32(dt.Rows[j + i * size][field[l]].ToString()));
                                }
                                else if (type == Type.GetType("System.Int16"))
                                {
                                    if (dt.Rows[j][field[l]].ToString().Trim() != "")
                                        cell.SetCellValue(Convert.ToInt16(dt.Rows[j + i * size][field[l]].ToString()));
                                }
                                else if (type == Type.GetType("System.Int64"))
                                {
                                    if (dt.Rows[j][field[l]].ToString().Trim() != "")
                                        cell.SetCellValue(Convert.ToInt64(dt.Rows[j + i * size][field[l]].ToString()));
                                }
                                else
                                {
                                    cell.SetCellValue(dt.Rows[j + i * size][field[l]].ToString());
                                }
                                //cell.SetCellValue(dt.Rows[j + i * size][field[l]].ToString());
                            }
                        }
                        using (fs = File.OpenWrite(path))
                        {
                            workbook.Write(fs);//向打开的这个xls文件中写入数据  
                        }
                    }
                    result = true;
                }
                else
                {
                    RetMsg = "传入数据为空。";
                }

                return strFileName;
            }
            catch (Exception ex)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                ComFunction.ConsoleWriteLine(ex.Message);
                RetMsg = "导出文件失败";
                return "";
            }
        }
        #endregion

        #region 导出自定义文件名
        /// <summary>
        /// DataTable导出为Excel
        /// </summary>
        /// <param name="head">Excel列头</param>
        /// <param name="field">DataTable列头</param>
        /// <param name="dt">DataTable</param>
        /// <param name="mapPath">匹配路径</param>
        /// <param name="responserid"></param>
        /// <param name="strFunctionName"></param>
        /// <param name="RetMsg"></param>
        /// <returns>最终文件的全路径</returns>
        public static string DataTableToExcel(string[] head, string[] field, DataTable dt, string rootPath, string fileName, string strUserId, string strFunctionName, ref string RetMsg)
        {
            bool result = false;
            RetMsg = "";
            FileStream fs = null;
            int size = 1048576 - 1;

            string strFileName = strFunctionName + fileName + System.DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + strUserId + ".xlsx";
            string fileSavePath = rootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar;//文件临时目录，导入完成后 删除

            string path = fileSavePath + strFileName;


            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            try
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    int page = dt.Rows.Count / size;
                    IWorkbook workbook = new XSSFWorkbook();
                    for (int i = 0; i < page + 1; i++)
                    {
                        string sheetname = "Sheet" + (i + 1).ToString();
                        ISheet sheet = workbook.CreateSheet(sheetname);
                        int rowCount = dt.Rows.Count - i * size > size ? size : dt.Rows.Count - i * size;//行数  
                        int columnCount = dt.Columns.Count;//列数  

                        //设置列头  
                        IRow row = sheet.CreateRow(0);
                        ICell cell;
                        for (int h = 0; h < head.Length; h++)
                        {
                            cell = row.CreateCell(h);
                            cell.SetCellValue(head[h]);
                        }
                        List<ICellStyle> styles = new List<ICellStyle>();
                        //设置每列单元格属性
                        for (int h = 0; h < field.Length; h++)
                        {
                            Type type = dt.Columns[field[h]].DataType;
                            ICellStyle dateStyle = workbook.CreateCellStyle();
                            IDataFormat dataFormat = workbook.CreateDataFormat();
                            if (type == Type.GetType("System.DateTime"))
                            {
                                dateStyle.DataFormat = dataFormat.GetFormat("yyyy-m-d hh:mm:ss");
                            }
                            else if (type == Type.GetType("System.Decimal"))
                            {
                                dateStyle.DataFormat = dataFormat.GetFormat("General");
                            }
                            else if (type == Type.GetType("System.Int32"))
                            {
                                dateStyle.DataFormat = dataFormat.GetFormat("General");
                            }
                            else if (type == Type.GetType("System.Int16"))
                            {
                                dateStyle.DataFormat = dataFormat.GetFormat("General");
                            }
                            else if (type == Type.GetType("System.Int64"))
                            {
                                dateStyle.DataFormat = dataFormat.GetFormat("General");
                            }
                            else if (type == Type.GetType("System.String") && field[h].StartsWith("d") && !field[h].StartsWith("dec"))
                            {
                                dateStyle.DataFormat = dataFormat.GetFormat("@");
                            }
                            else if (type == Type.GetType("System.String") && field[h].StartsWith("vc"))
                            {
                                dateStyle.DataFormat = dataFormat.GetFormat("@");
                            }
                            else if (type == Type.GetType("System.String"))
                            {
                                dateStyle.DataFormat = dataFormat.GetFormat("General");
                            }
                            styles.Add(dateStyle);
                        }
                        //设置每行每列的单元格,  
                        for (int j = 0; j < rowCount; j++)
                        {
                            row = sheet.CreateRow(j + 1);
                            for (int l = 0; l < field.Length; l++)
                            {
                                cell = row.CreateCell(l);//excel第二行开始写入数据 
                                cell.CellStyle = styles[l];
                                cell.SetCellValue(dt.Rows[j + i * size][field[l]].ToString());
                            }
                        }
                        using (fs = File.OpenWrite(path))
                        {
                            workbook.Write(fs);//向打开的这个xls文件中写入数据  
                        }
                    }
                    result = true;
                }
                else
                {
                    RetMsg = "传入数据为空。";
                }

                return path;
            }
            catch (Exception ex)
            {
                if (fs != null)
                {
                    fs.Close();
                }

                RetMsg = "导出文件失败";
                return "";
            }
        }
        #endregion

        #region 导出带模板
        public static string generateExcelWithXlt(DataTable dt, string[] field, string rootPath, string xltName, int startRow, string strUserId, string strFunctionName)
        {
            return generateExcelWithXlt(dt, field, rootPath, xltName, startRow, strUserId, strFunctionName, false);
        }

        public static string generateExcelWithXlt(DataTable dt, string[] field, string rootPath, string xltName, int startRow, string strUserId, string strFunctionName, bool isAlignCenter)
        {
            try
            {
                XSSFWorkbook hssfworkbook = new XSSFWorkbook();

                string XltPath = rootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Template" + Path.DirectorySeparatorChar + xltName;
                ComFunction.ConsoleWriteLine("generateExcelWithXlt:" + XltPath);
                using (FileStream fs = File.OpenRead(XltPath))
                {
                    hssfworkbook = new XSSFWorkbook(fs);
                    fs.Close();
                }

                ISheet sheet = hssfworkbook.GetSheetAt(0);
                ICellStyle style = hssfworkbook.CreateCellStyle();
                style.Alignment = HorizontalAlignment.Center;


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
                ComFunction.ConsoleWriteLine(ex.Message);
                return "";
            }
        }
        public static string generateExcelWithXlt(DataTable dt, string[] field, string rootPath, string xltName, int sheetindex, int startRow, string strUserId, string strFunctionName)
        {
            try
            {
                XSSFWorkbook hssfworkbook = new XSSFWorkbook();

                string XltPath = rootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Template" + Path.DirectorySeparatorChar + xltName;
                ComFunction.ConsoleWriteLine("generateExcelWithXlt2:" + XltPath);
                using (FileStream fs = File.OpenRead(XltPath))
                {
                    hssfworkbook = new XSSFWorkbook(fs);
                    fs.Close();
                }

                ISheet sheet = hssfworkbook.GetSheetAt(sheetindex);
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
                ComFunction.ConsoleWriteLine(ex.Message);
                return "";
            }
        }
        #endregion

        #region 校验日期
        public static bool CheckDate(string value)
        {
            try
            {
                Convert.ToDateTime(value);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 校验6位年月
        public static bool CheckYMonth(string value)
        {
            if (value.Length != 7)
                return false;
            try
            {
                Convert.ToDateTime(string.Format("{0}-{1}-01", value.Substring(0, 4), value.Substring(5, 2)));
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region 校验6位年月
        public static bool CheckYearMonth(string value)
        {
            if (value.Length != 6)
                return false;
            try
            {
                Convert.ToDateTime(string.Format("{0}-{1}-01", value.Substring(0, 4), value.Substring(4, 2)));
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region 校验decimal类型
        public static bool CheckDecimal(string value)
        {
            try
            {
                Convert.ToDecimal(value);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #endregion

        #region 对象序列化
        public static byte[] objectToBytes(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                return ms.GetBuffer();
            }
        }
        #endregion

        #region 对象反序列化
        public static object bytesToObject(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                IFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(ms);
            }
        }
        #endregion

        #region 文件夹删除
        public static void DeleteFolder(string dir)
        {
            if (System.IO.Directory.Exists(dir))
            {
                string[] fileSystemEntries = System.IO.Directory.GetFileSystemEntries(dir);
                for (int i = 0; i < fileSystemEntries.Length; i++)
                {
                    string text = fileSystemEntries[i];
                    if (System.IO.File.Exists(text))
                    {
                        System.IO.File.Delete(text);
                    }
                    else
                    {
                        DeleteFolder(text);
                    }
                }
                System.IO.Directory.Delete(dir);
            }
        }
        #endregion

        #region 获取数据字典
        public static DataTable getTCode(string strCodeId)
        {
            try
            {
                MultiExcute excute = new MultiExcute();
                System.Data.DataTable dt = new System.Data.DataTable();
                StringBuilder strSql = new StringBuilder();
                strSql.Append("   select vcName,vcValue from TCode where vcCodeId='" + strCodeId + "'     \n");
                if (strCodeId == "C002" || strCodeId == "C016")//变更事项有排序规定，还有类似的在这加or
                    strSql.Append("     order by cast(vcMeaning as int) asc     \n");
                else
                    strSql.Append("     ORDER BY iAutoId    \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 返回insert语句值
        /// <summary>
        /// 返回insert语句值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="isObject">如果insert时间、金额或者其他对象类型数据，为true</param>
        /// <returns></returns>
        public static string getSqlValue(Object obj, bool isObject)
        {
            if (obj == null)
                return "null";
            else if (obj.ToString().Trim() == "" && isObject)
                return "null";
            else
                return "'" + obj.ToString() + "'";
        }
        #endregion

        #region 发送邮件 公共方法
        /// <summary>
        /// 发送邮件的方法
        /// </summary>
        /// <param name="strUserEmail">用户邮箱</param>
        /// /// <param name="strUserName">用户name</param>
        /// <param name="strEmailBody">邮件内容 支持html代码</param>
        /// <param name="receiverDt">发件人Datatable存在两列：address,displayName 必须一样, address->邮件地址, displayName-->显示名称</param>
        /// <param name="cCDt">抄送人Datatable存在两列：address,displayName 必须一样, address->邮件地址, displayName-->显示名称</param>
        /// <param name="strSubject">邮件主题</param>
        /// <param name="strFilePath">附件：需要发送附件就传入附件文件地址 不需要就空</param>
        /// <param name="delFileNameFlag">默认为false, 如果传入了附件之后，需要删除文件就传true,没有附件或者不需要删除附件的就false</param>
        /// <returns></returns>
        public static string SendEmailInfo(string strUserEmail, string strUserName, string strEmailBody, DataTable receiverDt, DataTable cCDt, string strSubject, string strFilePath, bool delFileNameFlag)
        {
            MailMessage MMge = new MailMessage();
            try
            {
                SmtpClient mailClient = new SmtpClient(ComConstant.strSmtp);//服务器地址
                mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                mailClient.UseDefaultCredentials = false;
                mailClient.Timeout = 3600000;
                mailClient.Credentials = new NetworkCredential(ComConstant.strComEmail, ComConstant.strComEmailPwd);  //发送人邮箱登陆用户名和密码

                MMge.From = new MailAddress(strUserEmail, strUserName, Encoding.UTF8);

                //清除MMge
                MMge.To.Clear();          //收件
                MMge.Attachments.Clear(); //附件
                //添加发件人
                for (var i = 0; i < receiverDt.Rows.Count; i++)
                {
                    MMge.To.Add(new MailAddress(receiverDt.Rows[i]["address"].ToString(), receiverDt.Rows[i]["displayName"].ToString(), Encoding.UTF8));
                }
                //添加抄送
                if (cCDt != null)
                {
                    for (var i = 0; i < cCDt.Rows.Count; i++)
                    {
                        MMge.CC.Add(new MailAddress(cCDt.Rows[i]["address"].ToString(), cCDt.Rows[i]["displayName"].ToString(), Encoding.UTF8));
                    }
                }
                if (strFilePath != "")
                    MMge.Attachments.Add(new Attachment(strFilePath));//添加附件

                MMge.Subject = strSubject;//邮件主题

                MMge.IsBodyHtml = true;//这里启用IsBodyHtml是为了支持内容中的Html

                MMge.BodyEncoding = Encoding.Default;//将正文的编码形式设置为UTF8

                MMge.Body = strEmailBody;

                mailClient.Send(MMge);
                return "Success";
            }
            catch (Exception ex)
            {
                MMge.Dispose();
                return "Error";
            }
            finally
            {
                MMge.Dispose();
                if (delFileNameFlag)
                {
                    if (System.IO.File.Exists(strFilePath))
                    {
                        System.IO.File.Delete(strFilePath);
                    }
                }
            }
        }

        /// <summary>
        /// 发送邮件的方法
        /// </summary>
        /// <param name="strUserEmail">用户邮箱</param>
        /// /// <param name="strUserName">用户name</param>
        /// <param name="strEmailBody">邮件内容 支持html代码</param>
        /// <param name="receiverDt">发件人Datatable存在两列：address,displayName 必须一样, address->邮件地址, displayName-->显示名称</param>
        /// <param name="cCDt">抄送人Datatable存在两列：address,displayName 必须一样, address->邮件地址, displayName-->显示名称</param>
        /// <param name="strSubject">邮件主题</param>
        /// <param name="strFilePathArray">附件数组：需要发送附件就传入附件文件地址 不需要就空</param>
        /// <param name="delFileNameFlag">默认为false, 如果传入了附件之后，需要删除文件就传true,没有附件或者不需要删除附件的就false</param>
        /// <returns></returns>
        public static string SendEmailInfo(string strUserEmail, string strUserName, string strEmailBody, DataTable receiverDt, DataTable cCDt, string strSubject, string[] strFilePathArray, bool delFileNameFlag)
        {
            MailMessage MMge = new MailMessage();
            try
            {
                SmtpClient mailClient = new SmtpClient(ComConstant.strSmtp);//服务器地址
                mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                mailClient.UseDefaultCredentials = false;
                mailClient.Timeout = 3600000;
                mailClient.Credentials = new NetworkCredential(ComConstant.strComEmail, ComConstant.strComEmailPwd);  //发送人邮箱登陆用户名和密码

                MMge.From = new MailAddress(strUserEmail, strUserName, Encoding.UTF8);

                //清除MMge
                MMge.To.Clear();          //收件
                MMge.Attachments.Clear(); //附件
                //添加发件人
                for (var i = 0; i < receiverDt.Rows.Count; i++)
                {
                    MMge.To.Add(new MailAddress(receiverDt.Rows[i]["address"].ToString(), receiverDt.Rows[i]["displayName"].ToString(), Encoding.UTF8));
                }
                //添加抄送
                if (cCDt != null)
                {
                    for (var i = 0; i < cCDt.Rows.Count; i++)
                    {
                        MMge.CC.Add(new MailAddress(cCDt.Rows[i]["address"].ToString(), cCDt.Rows[i]["displayName"].ToString(), Encoding.UTF8));
                    }
                }
                foreach (string strFilePath in strFilePathArray)
                {
                    if (strFilePath != "")
                    {
                        MMge.Attachments.Add(new Attachment(strFilePath));//添加附件
                    }
                }

                MMge.Subject = strSubject;//邮件主题

                MMge.IsBodyHtml = true;//这里启用IsBodyHtml是为了支持内容中的Html

                MMge.BodyEncoding = Encoding.Default;//将正文的编码形式设置为UTF8

                MMge.Body = strEmailBody;

                mailClient.Send(MMge);
                return "Success";
            }
            catch (Exception ex)
            {
                MMge.Dispose();
                return "Error";
            }
            finally
            {
                MMge.Dispose();
                if (delFileNameFlag)
                {
                    foreach (string strFilePath in strFilePathArray)
                    {
                        if (System.IO.File.Exists(strFilePath))
                        {
                            System.IO.File.Delete(strFilePath);
                        }
                    }

                }
            }
        }

        #endregion

        /// <summary>
        /// 默认上传文件到116
        /// </summary>
        /// <param name="FtpRemotePath">指定FTP连接成功后的当前目录, 如果不指定即默认为根目录</param>
        /// <param name="filename">源文件完整路径</param>
        public static void FtpUpload(string FtpRemotePath, string filename)
        {
            try
            {
                FTPHelper helper = new FTPHelper("172.23.180.116:21111", FtpRemotePath, "Administrator", "TFTMspps116");
                helper.Upload(filename);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 下载文件到所选目录
        /// </summary>
        /// <param name="FtpRemotePath">指定FTP连接成功后的当前目录, 如果不指定即默认为根目录</param>
        /// <param name="filePath">文件保存路径</param>
        /// <param name="fileName">文件名</param>
        public static string FtpDownload(string FtpRemotePath, string filePath, string fileName)
        {
            try
            {
                FTPHelper helper = new FTPHelper("172.23.180.116:21111", FtpRemotePath, "Administrator", "TFTMspps116");
                return helper.Download(filePath, fileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 默认连接116Ftp
        /// </summary>
        /// <param name="FtpRemotePath">指定FTP连接成功后的当前目录, 如果不指定即默认为根目录</param>
        public static FTPHelper DefaultFtpHelper(string FtpRemotePath)
        {
            try
            {
                FTPHelper helper = new FTPHelper("172.23.180.116:21111", FtpRemotePath, "Administrator", "TFTMspps116");
                return helper;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// DMZ上传文件
        /// </summary>
        /// <param name="filepath">文件名绝对路径</param>
        /// <param name="strFileName">文件名</param>
        /// <param name="strToDir">Doc\\Export\\  或者其他</param>
        /// <returns></returns>
        public static bool HttpUploadFile(string filepath, string strFileName, string strToDir)
        {
            //        调用例子
            //        ComFunction.HttpUploadFile(_webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar
            //+ "Doc" + Path.DirectorySeparatorChar
            //+ "Export" + Path.DirectorySeparatorChar
            //+ "02.切替文件-0412.rar"
            //, "02.切替文件-0412.rar"
            //, "Doc\\Export\\");
            try
            {
                string url = ComConnectionHelper.GetFileUploadHost() + @"/api/Download/uploadDMZApi";
                // 设置参数
                HttpWebRequest request = WebRequest.Create(url + "?name=" + strFileName + "&dir=" + strToDir) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = "POST";
                string boundary = DateTime.Now.Ticks.ToString("X"); // 随机分隔线
                request.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;
                byte[] itemBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
                byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
                int pos = filepath.LastIndexOf("\\");
                string fileName = filepath.Substring(pos + 1);

                //请求头部信息
                StringBuilder sbHeader = new StringBuilder(string.Format("Content-Disposition:form-data;name=\"file\";filename=\"{0}\"\r\nContent-Type:application/octet-stream\r\n\r\n", fileName));
                byte[] postHeaderBytes = Encoding.UTF8.GetBytes(sbHeader.ToString());

                FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read);
                byte[] bArr = new byte[fs.Length];
                fs.Read(bArr, 0, bArr.Length);
                fs.Close();

                Stream postStream = request.GetRequestStream();
                postStream.Write(itemBoundaryBytes, 0, itemBoundaryBytes.Length);
                postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                postStream.Write(bArr, 0, bArr.Length);
                postStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
                postStream.Close();

                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream instream = response.GetResponseStream();
                StreamReader sr = new StreamReader(instream, Encoding.UTF8);
                //返回结果网页（html）代码
                string content = sr.ReadToEnd();
                if (content == "error")
                {
                    throw new Exception("文件上传异常");
                }

                else
                    return true;

            }
            catch (Exception ex)
            {
                ConsoleWriteLine(ex.Message);
                ConsoleWriteLine("filepath:" + filepath);
                ConsoleWriteLine("strFileName:" + strFileName);
                ConsoleWriteLine("strToDir:" + strToDir);
                ComMessage.WriteInDB("dmzUpload", "E", "文件上传异常", ex.Message + " 参数filepath=" + filepath + ",strFileName=" + strFileName + ",strToDir=" + strToDir, ex.StackTrace, "system");
                throw ex;
            }
        }
        public static string HttpGetWindowPath(string pathtype)
        {
            //返回windows路径
            try
            {
                string url = "";
                if (pathtype == "pdf")
                    url = ComConnectionHelper.GetFileUploadHost() + @"/api/Download/getWindowsPath_pdf";
                if (pathtype == "img")
                    url = ComConnectionHelper.GetFileUploadHost() + @"/api/Download/getWindowsPath_img";
                if (pathtype == "crv")
                    url = ComConnectionHelper.GetFileUploadHost() + @"/api/Download/getWindowsPath_crv";
                if (url == "")
                {
                    throw new Exception("路径读取失败");
                }
                // 设置参数
                HttpWebRequest request = WebRequest.Create(url + "?type=" + pathtype) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = "POST";
                string boundary = DateTime.Now.Ticks.ToString("X"); // 随机分隔线
                request.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;

                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream instream = response.GetResponseStream();
                StreamReader sr = new StreamReader(instream, Encoding.UTF8);
                //返回结果网页（html）代码
                string content = sr.ReadToEnd();
                if (content == "error")
                {
                    throw new Exception("路径读取失败");
                }

                else
                    return content;

            }
            catch (Exception ex)
            {
                ConsoleWriteLine(ex.Message);
                ComMessage.WriteInDB("dmzUpload", "E", "路径读取失败", ex.Message, ex.StackTrace, "system");
                throw ex;
            }
        }
        /// <summary>
        /// Http下载文件
        /// </summary>
        /// <param name="strFromDir">从116服务器的目录（相对应用目录）</param>
        /// <param name="strFileName">获取的文件名</param>
        /// <param name="strToPath">保存到linux的目录（相对应用目录）</param>
        /// <returns></returns>
        public static string HttpDownload(string strFromDir, string strFileName, string strToPath)
        {
            //调用例子
            //ComFunction.HttpDownload("Doc\\Export\\",
            //   "02.切替文件-0412.rar",
            //   _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar
            //   + "Doc" + Path.DirectorySeparatorChar
            //   + "PDF" + Path.DirectorySeparatorChar);

            string url = ComConnectionHelper.GetFileUploadHost() + @"/api/Download/downloadDMZApi" + "?name=" + strFileName + "&dir=" + strFromDir;

            string tempFile = strToPath + Path.DirectorySeparatorChar + strFileName;
            if (System.IO.File.Exists(tempFile))
            {
                System.IO.File.Delete(tempFile);    //存在则删除
            }
            try
            {
                FileStream fs = new FileStream(tempFile, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);
                // 设置参数
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream responseStream = response.GetResponseStream();
                //创建本地文件写入流
                //Stream stream = new FileStream(tempFile, FileMode.Create);
                byte[] bArr = new byte[1024];
                int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                while (size > 0)
                {
                    //stream.Write(bArr, 0, size);
                    fs.Write(bArr, 0, size);
                    size = responseStream.Read(bArr, 0, (int)bArr.Length);
                }
                //stream.Close();
                fs.Close();
                responseStream.Close();
                return strToPath + Path.DirectorySeparatorChar + strFileName;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        #region 记录操作日志
        /// <summary>
        /// 记录操作日志
        /// </summary>
        /// <param name="content">需要记录的内容</param>
        public static void ConsoleWriteLine(string content)
        {
            try
            {
                //获取程序目录
                string path_Root = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;
                //Log文件夹路径
                string path_Log = path_Root + "Doc" + Path.DirectorySeparatorChar + "Log" + Path.DirectorySeparatorChar;
                //当前年份
                string strYear = DateTime.Now.ToString("yyyy");
                //当前年的文件夹
                string path_Year = path_Log + strYear + Path.DirectorySeparatorChar;

                //当前天
                string strDay = DateTime.Now.ToString("yyyyMMdd");
                //当前天的文件名
                string file_Day = strDay + ".txt";
                //当前天的文件路径
                string path_Day = path_Year + file_Day;

                //判断当前年的路径是否存在，如果不存在则创建
                if (!Directory.Exists(path_Year))
                {
                    Directory.CreateDirectory(path_Year);
                }

                //判断当前天的文件是否存在，如果不存在则创建
                if (!File.Exists(path_Day))
                {
                    FileStream fs = File.Create(path_Day);
                    fs.Close();
                    fs.Dispose();
                }

                //给传入的内容添加日期前缀
                content = DateTime.Now.ToString("yyyy-MM-dd") + " " + DateTime.Now.ToString("HH:mm:ss") + ":" + content;

                //打开当前天文件，追加内容
                using (StreamWriter sw = new StreamWriter(path_Day, true))
                {
                    sw.WriteLine(content);
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
                Console.WriteLine(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(content);
            }
        }
        #endregion

    }


    #region 导入文件时,作校验的类
    /// <summary>
    /// 导入检查类
    /// </summary>
    public static class FieldCheck
    {
        /// <summary>
        /// 数字
        /// </summary>
        public static readonly string Num = "[^0-9]+";
        /// <summary>
        /// 小数
        /// </summary>
        public static readonly string Float = "[^0-9^.]+";
        /// <summary>
        /// 英文
        /// </summary>
        public static readonly string Char = "[^A-Z^a-z]+";
        /// <summary>
        /// 英数
        /// </summary>
        public static readonly string NumChar = "[^A-Z^a-z^0-9]+";
        /// <summary>
        /// 英数 + "-"
        /// </summary>
        public static readonly string NumCharL = "[^A-Z^a-z^0-9^-]+";
        /// <summary>
        /// 日期
        /// </summary>
        public static readonly string Date = "d";
        /// <summary>
        /// 年月
        /// </summary>
        public static readonly string YearMonth = "ym";
        /// <summary>
        /// 特殊品番
        /// </summary>
        public static readonly string SpecialPartNo = "[^A-Z^a-z^0-9^-]+";
        /// <summary>
        /// 英数 + "/"
        /// </summary>
        public static readonly string NumCharLL = "[^A-Z^a-z^0-9^/]+";
        //Add by liuchunyan at 2012-2-2
        /// <summary>
        /// 数字可以录入带"-"的
        /// </summary>
        public static readonly string FNum = "[^-^0-9]+";

        /// <summary>
        /// 英数 + "/"+"_"+"-"
        /// </summary>
        public static readonly string NumCharLLL = "[^A-Z^a-z^0-9^/^_^-]+";

        public static readonly string Decimal = "decimal";

        /// <summary>
        /// 英数 + "-",且必须有"-"
        /// </summary>
        public static readonly string PartLine = "PartLine";

        public static readonly string Email = @"^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$";

    }
    #endregion


}
