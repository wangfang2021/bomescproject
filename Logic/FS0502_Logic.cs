using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Common;
using DataAccess;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Logic
{
    public class FS0502_Logic
    {
        FS0502_DataAccess fs0502_DataAccess = new FS0502_DataAccess();

        #region 按检索条件检索,返回dt
        public DataTable Search(string vcSupplier_id, string vcStatus, string vcOrderNo, string vcPart_id)
        {
            DataTable dataTable= fs0502_DataAccess.Search(vcSupplier_id, vcStatus, vcOrderNo, vcPart_id);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                string decBoxQuantity = dataTable.Rows[i]["decBoxQuantity"].ToString();
                if (decBoxQuantity != "")
                {
                    string strBoxColor = IsInteger(decBoxQuantity);
                    dataTable.Rows[i]["boxColor"] = strBoxColor;
                }
            }

            return dataTable;
        }
        public string IsInteger(string s)
        {
            int i;
            double d;
            if (int.TryParse(s, out i))
                return "0";
            else if (double.TryParse(s, out d))
                return (d == Math.Truncate(d) ? "0" : "red");
            else
                return "0";
        }
        #endregion

        #region 分批纳入子画面检索数据,返回dt
        public DataTable SearchSub(string vcOrderNo,string vcPart_id,string vcSupplier_id)
        {
            return fs0502_DataAccess.SearchSub(vcOrderNo, vcPart_id, vcSupplier_id);
        }
        #endregion

        #region 子画面初始化
        public DataTable initSubApi(string iAutoId)
        {
            return fs0502_DataAccess.initSubApi(iAutoId);
        }
        #endregion

        #region 取C056中2个状态
        public DataTable getTCode(string strCodeId)
        {
            return fs0502_DataAccess.getTCode(strCodeId);
        }
        #endregion

        public DataTable getOrderNo(string vcSupplier_id)
        {
            return fs0502_DataAccess.getOrderNo(vcSupplier_id);
        }

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId, string strautoid_main, 
            string vcPart_id, string vcOrderNo, string vcSupplier_id,ref string infopart,string iPackingQty)
        {
            fs0502_DataAccess.Save(listInfoData, strUserId, ref strErrorPartId,strautoid_main,  vcPart_id, vcOrderNo, vcSupplier_id,ref infopart, iPackingQty);
        }
        #endregion

        #region 是否可操作-按列表所选数据
        public bool IsDQR(List<Dictionary<string, Object>> listInfoData, ref string strMsg_status,ref string strMsg_null, string strType)
        {
            DataSet ds = fs0502_DataAccess.IsDQR(listInfoData, strType);
            DataTable dt_status = ds.Tables[0];
            //DataTable dt_null = ds.Tables[1];

            if (dt_status.Rows.Count == 0 )
                return true;
            else
            {
                if(dt_status.Rows.Count>0)
                {
                    for (int i = 0; i < dt_status.Rows.Count; i++)
                    {
                        strMsg_status += dt_status.Rows[i]["vcPart_id"].ToString() + "/";
                    }
                    strMsg_status = strMsg_status.Substring(0, strMsg_status.Length - 1);
                }
                //if(dt_null.Rows.Count>0)
                //{
                //    for(int i=0;i<dt_null.Rows.Count;i++)
                //    {
                //        strMsg_null += dt_null.Rows[i]["vcPart_id"].ToString() + "/";
                //    }
                //    strMsg_null = strMsg_null.Substring(0, strMsg_null.Length - 1);
                //}
                return false;
            }
        }
        #endregion

        #region 是否可操作-按检索条件
        public bool IsDQR(string vcSupplier_id,string vcStatus,string vcOrderNo,string vcPart_id, ref string strMsg_status,ref string strMsg_null)
        {
            DataSet ds = fs0502_DataAccess.IsDQR(vcSupplier_id, vcStatus, vcOrderNo, vcPart_id);
            DataTable dt_status = ds.Tables[0];
            DataTable dt_null = ds.Tables[1];

            if (dt_status.Rows.Count == 0 && dt_null.Rows.Count == 0)
                return true;
            else
            {
                if (dt_status.Rows.Count > 0)
                {
                    for (int i = 0; i < dt_status.Rows.Count; i++)
                    {
                        strMsg_status += dt_status.Rows[i]["vcPart_id"].ToString() + "/";
                    }
                    strMsg_status = strMsg_status.Substring(0, strMsg_status.Length - 1);
                }
                if (dt_null.Rows.Count > 0)
                {
                    for (int i = 0; i < dt_null.Rows.Count; i++)
                    {
                        strMsg_null += dt_null.Rows[i]["vcPart_id"].ToString() + "/";
                    }
                    strMsg_null = strMsg_null.Substring(0, strMsg_null.Length - 1);
                }
                return false;
            }

        }
        #endregion

        #region 提交-按列表所选
        public int ok( List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            return fs0502_DataAccess.ok( listInfoData, strUserId);
        }
        #endregion

        #region 提交-按检索条件
        public int ok(string vcSupplier_id, string vcStatus, string vcOrderNo, string vcPart_id,string strUserId)
        {
            return fs0502_DataAccess.ok(vcSupplier_id, vcStatus, vcOrderNo, vcPart_id, strUserId);
        }
        #endregion

        #region 分批导入子画面删除  不用
        public void DelSub(List<Dictionary<string, Object>> checkedInfoData, string strUserId, ref string strErrorPartId)
        {
            fs0502_DataAccess.DelSub(checkedInfoData, strUserId,ref strErrorPartId);
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
        public string DataTableToExcel(string[] head, string[] field, DataTable dt, string rootPath, string strUserId, string strFunctionName, ref string RetMsg)
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
                                if(h==1)//纳期表发送时间
                                    dateStyle.DataFormat = dataFormat.GetFormat("yyyy-m-d");
                                else if(h==6)//回复截至日期
                                    dateStyle.DataFormat = dataFormat.GetFormat("yyyy-m-d");
                                else if(h==11)//可对应纳期
                                    dateStyle.DataFormat = dataFormat.GetFormat("yyyy-m-d");
                                else if(h==12)//供应商回复时间
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
                Console.WriteLine(ex.Message);
                RetMsg = "导出文件失败";
                return "";
            }
        }
        #endregion
    }
}
