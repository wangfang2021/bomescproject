using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Logic
{
    public class FS0405_Logic
    {
        FS0405_DataAccess fs0405_DataAccess;

        public FS0405_Logic()
        {
            fs0405_DataAccess = new FS0405_DataAccess();
        }

        #region 检索
        public DataTable Search(string strDXDateMonth, string strInOutFlag, string strState)
        {
            DataTable dt = fs0405_DataAccess.Search(strDXDateMonth, strInOutFlag, strState);
            dt.DefaultView.Sort="vcDXYM,vcInOutFlag";
            dt = dt.DefaultView.ToTable();
            return dt;
        }
        #endregion

        #region 导出
        public DataTable exportSearch(string strDXYM,string strInOutFlag,string strDXYM1,string strDXYM2)
        {
            return fs0405_DataAccess.exportSearch(strDXYM, strInOutFlag, strDXYM1, strDXYM2);
        }
        #endregion

        #region 导出自定义文件名
        /// <summary>
        /// DataTable导出为Excel
        /// </summary>
        /// <param name="head">Excel列头</param>
        /// <param name="field">DataTable列头</param>
        /// <param name="dt">DataTable</param>
        /// <param name="rootPath"></param>
        /// <param name="fileName">生成的文件名</param>
        /// <param name="strFunctionName"></param>
        /// <param name="RetMsg"></param>
        /// <returns>最终文件的全路径</returns>
        public string DataTableToExcel(string[] head, string[] field, DataTable dt, string rootPath, string fileName, ref string RetMsg)
        {
            RetMsg = "";
            FileStream fs = null;
            int size = 1048576 - 1;

            if (!fileName.Contains(".xlsx"))
            {
                fileName = fileName + ".xlsx";
            }
            string strFileName = fileName;
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
                        return fileName;
                    }
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
    }
}
