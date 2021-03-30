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
    public class FS0501_Logic
    {
        FS0501_DataAccess fs0501_DataAccess = new FS0501_DataAccess();

        #region 按检索条件检索,返回dt
        public DataTable Search(string strYearMonth, string strSupplier_id, string strPart_id, string strDyState, string strOperState,ref int num)
        {
            return fs0501_DataAccess.Search(strYearMonth, strSupplier_id, strPart_id, strDyState, strOperState,ref num);
        }
        public DataTable Search_heji(string strYearMonth, string strSupplier_id, string strPart_id, string strDyState, string strOperState, ref int num)
        {
            return fs0501_DataAccess.Search_heji(strYearMonth, strSupplier_id, strPart_id, strDyState, strOperState, ref num);
        }
        #endregion

        #region 是否可操作-按检索条件
        public bool IsDQR(string strYearMonth, string strSupplier_id, string strPart_id, string strDyState, string strOperState, ref string strMsg)
        {
            DataTable dt = fs0501_DataAccess.IsDQR(strYearMonth, strSupplier_id, strPart_id, strDyState, strOperState);
            if (dt.Rows.Count == 0)
                return true;
            else
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strMsg += dt.Rows[i]["vcPart_id"].ToString() + "/";
                }
                strMsg = strMsg.Substring(0, strMsg.Length - 1);
                return false;
            }

        }
        #endregion

        #region 是否可操作-按列表所选数据
        public bool IsDQR(string strYearMonth, string strSupplier_id, List<Dictionary<string, Object>> listInfoData, ref string strMsg, string strType)
        {
            DataTable dt = fs0501_DataAccess.IsDQR(strYearMonth, strSupplier_id, listInfoData, strType);
            if (dt.Rows.Count == 0)
                return true;
            else
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strMsg += dt.Rows[i]["vcPart_id"].ToString() + "/";
                }
                strMsg = strMsg.Substring(0, strMsg.Length - 1);
                return false;
            }
        }
        #endregion

        #region 提交-按检索条件
        public int ok(string strYearMonth, string strSupplier_id, string strPart_id, string strDyState, string strOperState, string strUserId)
        {
            return fs0501_DataAccess.ok(strYearMonth, strSupplier_id, strPart_id, strDyState, strOperState, strUserId);
        }
        #endregion

        #region 提交-按列表所选
        public int ok(string strYearMonth, List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            return fs0501_DataAccess.ok(strYearMonth, listInfoData, strUserId);
        }
        #endregion

        #region 导出带模板
        public string generateExcelWithXlt(DataTable dt, string[] field, string rootPath, string xltName, int startRow, string strUserId, string strFunctionName, string strYearMonth, string strYearMonth_2, string strYearMonth_3)
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
                        cell.CellStyle = style;
                    }
                }

                //以下业务特别处理

                int iMonth = Convert.ToInt32(strYearMonth.Substring(4, 2));//对象月
                int iMonth_2 = Convert.ToInt32(strYearMonth_2.Substring(4, 2));//内示月
                int iMonth_3 = Convert.ToInt32(strYearMonth_3.Substring(4, 2));//内内示月

                sheet.GetRow(1).GetCell(5).SetCellValue(iMonth + "月");
                sheet.GetRow(1).GetCell(6).SetCellValue(iMonth_2 + "月");
                sheet.GetRow(1).GetCell(7).SetCellValue(iMonth_3 + "月");
                sheet.GetRow(1).GetCell(5).SetCellValue(iMonth + "月");
                sheet.GetRow(1).GetCell(6).SetCellValue(iMonth_2 + "月");
                sheet.GetRow(1).GetCell(7).SetCellValue(iMonth_3 + "月");

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

        #region 保存后校验
        #region 保存后校验
        public void SaveCheck(List<Dictionary<string, Object>> listInfoData, string strUserId, string strYearMonth, string strYearMonth_2, string strYearMonth_3,
            ref Dictionary<string, string> errMessageList, string strUnit)
        {
            DataTable dterrMessage = new DataTable();
            dterrMessage.Columns.Add("vcPart_id");
            dterrMessage.Columns.Add("vcMsg");
            DataTable temp =fs0501_DataAccess.GetReceiver();
            string strReceiver = temp.Rows.Count == 1 ? temp.Rows[0][0].ToString() : "APC06";
            FS0603_Logic fs0603_Logic = new FS0603_Logic();
            FS0603_DataAccess fs0603_DataAccess = new FS0603_DataAccess();
            DataTable dtMultiple = fs0603_Logic.createTable("SOQ602");
            for (int i = 0; i < listInfoData.Count; i++)
            {
                bool bmodflag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                bool baddflag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                if (baddflag == false && bmodflag == true)
                {//修改
                    string strDyState = "";
                    string strHyState = "";
                    string strPart_id = fs0603_DataAccess.setNullValue(listInfoData[i]["vcPart_id"], "", "");
                    string strCbSOQN = fs0603_DataAccess.setNullValue(listInfoData[i]["iCbSOQN"], "", "0");
                    string strCbSOQN1 = fs0603_DataAccess.setNullValue(listInfoData[i]["iCbSOQN1"], "", "0");
                    string strCbSOQN2 = fs0603_DataAccess.setNullValue(listInfoData[i]["iCbSOQN2"], "", "0");
                    string strTzhSOQN = fs0603_DataAccess.setNullValue(listInfoData[i]["iTzhSOQN"], "", "0");
                    string strTzhSOQN1 = fs0603_DataAccess.setNullValue(listInfoData[i]["iTzhSOQN1"], "", "0");
                    string strTzhSOQN2 = fs0603_DataAccess.setNullValue(listInfoData[i]["iTzhSOQN2"], "", "0");
                    string strSupplierId = fs0603_DataAccess.setNullValue(listInfoData[i]["vcSupplier_id"], "", "");
                    string strExpectTime = "";
                    string strInputType = "supplier";
                    DataRow drMultiple = dtMultiple.NewRow();
                    drMultiple["vcYearMonth"] = strYearMonth;
                    drMultiple["vcDyState"] = strDyState;
                    drMultiple["vcHyState"] = strHyState;
                    drMultiple["vcPart_id"] = strPart_id;
                    drMultiple["iCbSOQN"] = strCbSOQN;
                    drMultiple["iCbSOQN1"] = strCbSOQN1;
                    drMultiple["iCbSOQN2"] = strCbSOQN2;
                    drMultiple["iTzhSOQN"] = strTzhSOQN;
                    drMultiple["iTzhSOQN1"] = strTzhSOQN1;
                    drMultiple["iTzhSOQN2"] = strTzhSOQN2;
                    drMultiple["vcSupplierId"] = strSupplierId;
                    drMultiple["dExpectTime"] = strExpectTime;
                    drMultiple["vcInputType"] = strInputType;
                    dtMultiple.Rows.Add(drMultiple);
                }
            }
            //fs0501_DataAccess.SaveCheck(listInfoData, strUserId, strYearMonth, strYearMonth_2, strYearMonth_3, ref dterrMessage, strUnit);
            fs0501_DataAccess.SaveCheck(dtMultiple, strUserId, strYearMonth, strYearMonth_2, strYearMonth_3, ref dterrMessage, strUnit, strReceiver);
            for (int i = 0; i < dterrMessage.Rows.Count; i++)
            {
                errMessageList.Add(dterrMessage.Rows[i]["vcPart_id"].ToString(), dterrMessage.Rows[i]["vcMsg"].ToString());
            }
        }
        #endregion
        #endregion

        #region 插入导入履历
        public void importHistory(string strYearMonth, Dictionary<string,string> errMessageDict, string strUserId)
        {
            fs0501_DataAccess.importHistory(strYearMonth, errMessageDict, strUserId);
        }
        #endregion

        #region 导入后保存
        public void importSave(string strYearMonth, string strUserId, string strUnit)
        {
            fs0501_DataAccess.importSave(strYearMonth, strUserId, strUnit);
        }
        #endregion

        #region 检索保存错误信息
        public DataTable SearchHistory(string strYearMonth, string strUserId)
        {
            return fs0501_DataAccess.SearchHistory(strYearMonth, strUserId);
        }
        #endregion

        #region 获取数据字典
        public DataTable getTCode(string strCodeId)
        {
            return fs0501_DataAccess.getTCode(strCodeId);
        }
        #endregion

        #region 获取供应商工区
        public DataTable getWorkArea(string strSupplier_id)
        {
            return fs0501_DataAccess.getWorkArea(strSupplier_id);
        }
        #endregion
    }
}
