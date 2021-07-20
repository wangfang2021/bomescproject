using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using System.Net;
using System.Net.Mail;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Common;

namespace Logic
{
    public class P00004_Logic
    {
        static P00004_DataAccess P00004_DataAccess = new P00004_DataAccess();
        //========================================================================重写========================================================================
        public DataTable GetDockInfo(string opearteId)
        {
            try
            {
                return P00004_DataAccess.getDockInfo(opearteId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetData(string dock, string fork)
        {
            try
            {
                return P00004_DataAccess.GetData(dock, fork);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getDockAndForkInfo(string dock, string fork, string strFlag)
        {
            try
            {
                return P00004_DataAccess.getDockAndForkInfo(dock, fork, strFlag);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setDockAndForkInfo(string strType, string dock, string fork, string strFlag, string strOperatorID)
        {
            try
            {
                P00004_DataAccess.setDockAndForkInfo(strType, dock, fork, strFlag, strOperatorID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable checkPrintName(string iP, string strPointType)
        {
            return P00004_DataAccess.checkPrintName(iP, strPointType);
        }

        public DataSet checkDockAndForkInfo(string dock, string fork)
        {
            try
            {
                return P00004_DataAccess.checkDockAndForkInfo(dock, fork);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getBoxList(string dock, string fork)
        {
            try
            {
                return P00004_DataAccess.getBoxList(dock, fork);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void delBoxList(string caseNo, string dock, string fork)
        {
            try
            {
                P00004_DataAccess.delBoxList(caseNo, dock, fork);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet getDockInfo(string dock, string fork, string strFlag, string strPackingPlant)
        {
            try
            {
                return P00004_DataAccess.getDockInfo(dock, fork, strFlag, strPackingPlant);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setOutPut_Temp(string dock, string fork, string strFlag, string strPackingPlant, string strIP, string strOperater)
        {
            try
            {
                P00004_DataAccess.setOutPut_Temp(dock, fork, strFlag, strPackingPlant, strIP, strOperater);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet getTableInfoFromDB()
        {
            try
            {
                return P00004_DataAccess.getTableInfoFromDB();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getBanZhiTime(string strPackPlant, string strFlag)
        {
            try
            {
                return P00004_DataAccess.getBanZhiTime(strPackPlant, strFlag);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static DataTable setSeqNo(string tmpString, int iAddNum, string formatServerTime, string strHosDate, string strBanZhi, string strType)
        {
            try
            {
                return P00004_DataAccess.setSeqNo(tmpString, iAddNum, formatServerTime, strHosDate, strBanZhi, strType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static DataTable setSeqNo_sell(string tmpString, string formatServerTime, string strBanZhi)
        {
            try
            {
                return P00004_DataAccess.setSeqNo_sell(tmpString, formatServerTime, strBanZhi);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void upSeqNo_sell(int iXSNO_Lin, int iBSNO_Lin)
        {
            try
            {
                P00004_DataAccess.upSeqNo_sell(iXSNO_Lin, iBSNO_Lin);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool setCastListInfo(
            int iXSNO_Lin, int iBSNO_Lin,
            DataTable dtOperateSJ_Temp,
            DataTable dtOperateSJ_InOutput_Temp,
            DataTable dtOrder_Temp,
            DataTable dtORD_INOUT_Temp,
            DataTable dtSell_Temp,
            DataTable dtShipList_Temp,
            DataTable dtSell_Sum_Temp,
            DataTable dtSell_Tool_Temp,
            string strIP, string strSellno, string strDock, string strOperId, string strPrinterName)
        {
            try
            {
                return P00004_DataAccess.setCastListInfo(
                    iXSNO_Lin, iBSNO_Lin,
                    dtOperateSJ_Temp,
                    dtOperateSJ_InOutput_Temp,
                    dtOrder_Temp,
                    dtORD_INOUT_Temp,
                    dtSell_Temp,
                    dtShipList_Temp,
                    dtSell_Sum_Temp,
                    dtSell_Tool_Temp, strIP, strSellno, strDock, strOperId, strPrinterName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getSellInfo(string strSellno)
        {
            try
            {
                return P00004_DataAccess.getSellInfo(strSellno);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool DataTableToCSV_sell(DataTable dt, string Filepath)
        {
            //标头值的容器
            StringBuilder strColu = new StringBuilder();
            //行值的容器
            StringBuilder strValue = new StringBuilder();
            int i = 0;
            StreamWriter sw = null;
            try
            {
                bool bFileExists = File.Exists(Filepath);
                sw = new StreamWriter(new FileStream(Filepath, FileMode.Append), Encoding.GetEncoding("GB2312"));
                //打印标头
                if (!bFileExists)
                {
                    for (i = 0; i <= dt.Columns.Count - 1; i++)
                    {
                        strColu.Append(dt.Columns[i].ColumnName);
                        strColu.Append(",");
                    }
                    strColu.Remove(strColu.Length - 1, 1);//移出掉最后一个,字符
                    sw.WriteLine(strColu);
                }

                //打印行值
                foreach (DataRow drr in dt.Rows)
                {
                    strValue.Remove(0, strValue.Length);//移出
                    for (i = 0; i <= dt.Columns.Count - 1; i++)
                    {
                        strValue.Append("" + drr[i].ToString());
                        strValue.Append(",");
                    }
                    strValue.Remove(strValue.Length - 1, 1);//移出掉最后一个,字符
                    sw.WriteLine(strValue);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
            finally
            {
                if (sw != null)
                    sw.Close();
            }
        }
        public string DataTableToExcel(string[] head, string[] field, DataTable dt, string rootPath, string opearteId, string v2, ref string msg, string sellNo)
        {
            FileStream fs = null;
            int size = 1048576 - 1;

            string strFileName = "INVINTERFACE_" + sellNo + "_APC06_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
            string fileSavePath = rootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "INVINTERFACE" + Path.DirectorySeparatorChar;//文件临时目录，导入完成后 删除

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
                    return path;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                Console.WriteLine(ex.Message);
                return "";
            }
        }
        public DataSet getSumandToolOfSell(string strSellNo, string strYinQuType)
        {
            try
            {
                return P00004_DataAccess.getSumandToolOfSell(strSellNo, strYinQuType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string setEmailBody(string strYingQuName, string truckNo, string strQianFengNo, DataTable dtSell_Sum, DataTable dtSell_Tool)
        {
            try
            {
                return P00004_DataAccess.setEmailBody(strYingQuName, truckNo, strQianFengNo, dtSell_Sum, dtSell_Tool);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string sendEmailInfo_FTMS_OutPut(string strFRId, string strFRName, string strFRAddress, string strTheme, string strEmailBody, string strFilePath)
        {
            try
            {
                FS0602_DataAccess fs0602_DataAccess = new FS0602_DataAccess();
                FS0603_DataAccess fs0603_DataAccess = new FS0603_DataAccess();
                FS0603_Logic fs0603_Logic = new FS0603_Logic();

                DataTable dtEmail = fs0602_DataAccess.getFTMSEmail_OutPut();
                DataTable dtToInfo = fs0603_Logic.createTable("mailaddress");
                for (int j = 0; j < dtEmail.Rows.Count; j++)
                {
                    DataRow drToInfo = dtToInfo.NewRow();
                    drToInfo["address"] = dtEmail.Rows[j]["vcEmail1"].ToString();
                    drToInfo["displayName"] = dtEmail.Rows[j]["vcLXR1"].ToString();
                    dtToInfo.Rows.Add(drToInfo);
                }
                DataTable dtCcInfo = null;
                string result = ComFunction.SendEmailInfo(strFRAddress, strFRName, strEmailBody, dtToInfo, dtCcInfo, strTheme, strFilePath, false);
                if (result != "Success")
                {
                    return "出荷已经完成，但向销售公司发送邮件失败，请采取其他形式联络。";
                }
                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetPrintName(string iP, string strKind)
        {
            return P00004_DataAccess.GetPrintName(iP, strKind);
        }

        public static DataTable getSellInfo(string timeFrom, string timeEnd, string type, string date, string banZhi)
        {
            try
            {
                return P00004_DataAccess.getSellInfo(timeFrom, timeEnd, type, date, banZhi);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static DataTable getToolInfo(string sellNo)
        {
            try
            {
                return P00004_DataAccess.getToolInfo(sellNo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
