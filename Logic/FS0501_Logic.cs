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
        public DataTable Search(string strYearMonth, string strSupplier_id, string strPart_id, string strDyState, string strOperState,string strWorkArea,ref int num)
        {
            return fs0501_DataAccess.Search(strYearMonth, strSupplier_id, strPart_id, strDyState, strOperState, strWorkArea,ref num);
        }
        public DataTable Search_heji(string strYearMonth, string strSupplier_id, string strPart_id, string strDyState, string strOperState, string strWorkArea, ref int num)
        {
            return fs0501_DataAccess.Search_heji(strYearMonth, strSupplier_id, strPart_id, strDyState, strOperState, strWorkArea, ref num);
        }
        #endregion

        #region 是否可操作-按检索条件
        public bool IsDQR(string strYearMonth, string strSupplier_id, string strPart_id, string strDyState, string strOperState,string strWorkArea, ref string strMsg)
        {
            DataTable dt = fs0501_DataAccess.IsDQR(strYearMonth, strSupplier_id, strPart_id, strDyState, strOperState, strWorkArea);
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
        public int ok(string strYearMonth, string strSupplier_id, string strPart_id, string strDyState, string strOperState,string strWorkArea, string strUserId)
        {
            return fs0501_DataAccess.ok(strYearMonth, strSupplier_id, strPart_id, strDyState, strOperState,strWorkArea, strUserId);
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


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(startRow + i);
                    for (int j = 0; j < field.Length; j++)
                    {
                        ICell cell = row.CreateCell(j);
                        cell.SetCellValue(dt.Rows[i][field[j]].ToString());
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
        public void SaveCheck(List<Dictionary<string, Object>> listInfoData, string strUserId, string strYearMonth, string strYearMonth_2, string strYearMonth_3,
            ref Dictionary<string,string> errMessageList, string strUnit)
        {
            DataTable dterrMessage = new DataTable();
            dterrMessage.Columns.Add("vcPart_id");
            dterrMessage.Columns.Add("vcMsg");
            
            fs0501_DataAccess.SaveCheck(listInfoData, strUserId, strYearMonth, strYearMonth_2, strYearMonth_3, ref dterrMessage, strUnit);
            for(int i=0;i<dterrMessage.Rows.Count;i++)
            {
                errMessageList.Add(dterrMessage.Rows[i]["vcPart_id"].ToString(), dterrMessage.Rows[i]["vcMsg"].ToString());
            }
        }
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
