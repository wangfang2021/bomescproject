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

        #region 绑定供应商
        public DataTable getAllSupplier()
        {
            return fs1702_DataAccess.getAllSupplier();
        }
        #endregion

        #region 检索
        public DataTable Search(string vcGQ, string vcSupplier_id, string vcSHF, string vcPart_id, string vcCarType, string vcTimeFrom, string vcTimeTo)
        {
            return fs1702_DataAccess.Search(vcGQ, vcSupplier_id, vcSHF, vcPart_id, vcCarType, vcTimeFrom, vcTimeTo);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs1702_DataAccess.Save(listInfoData, strUserId);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            fs1702_DataAccess.Del(checkedInfoData, strUserId);
        }
        #endregion

        #region 品番+开始时间+收货方 不能重复
        public bool RepeatCheck(string strPart_id,string strTimeFrom,string strSHF)
        {
            int num = fs1702_DataAccess.RepeatCheck(strPart_id,strTimeFrom,strSHF);
            if(num>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion 

        public bool DateRegionCheck(string strPart_id, string strSHF, string strTimeFrom, string strTimeTo, string strMode, string strAutoId)
        {
            int num = fs1702_DataAccess.DateRegionCheck(strPart_id, strSHF, strTimeFrom, strTimeTo, strMode, strAutoId);
            if (num > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string generateExcelWithXlt_Module(string rootPath, string xltName,string strUserId, string strFunctionName)
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
                ISheet sheet = hssfworkbook.GetSheetAt(1);

                DataTable dt_gq = fs1702_DataAccess.GetName("gq");//工区
                for(int i=0;i<dt_gq.Rows.Count;i++)
                {
                    string gq = dt_gq.Rows[i]["vcName"].ToString();
                    IRow row = sheet.GetRow(i+1);
                    if (row == null)
                        row = sheet.CreateRow(i + 1);
                    ICell cell = row.GetCell(0);//工区
                    if (cell == null)
                        cell = row.CreateCell(0);
                    cell.SetCellValue(gq);
                }
                DataTable dt_shf = fs1702_DataAccess.GetName("shf");//收货方
                for (int i = 0; i < dt_shf.Rows.Count; i++)
                {
                    string shf = dt_shf.Rows[i]["vcName"].ToString();
                    IRow row = sheet.GetRow(i + 1);
                    if (row == null)
                        row = sheet.CreateRow(i + 1);
                    ICell cell = row.GetCell(1);//收货方
                    if (cell == null)
                        cell = row.CreateCell(1);
                    cell.SetCellValue(shf);
                }
                DataTable dt_supplier = fs1702_DataAccess.GetName("supplier");//供应商
                for (int i = 0; i < dt_supplier.Rows.Count; i++)
                {
                    string supplier = dt_supplier.Rows[i]["vcName"].ToString();
                    IRow row = sheet.GetRow(i + 1);
                    if (row == null)
                        row = sheet.CreateRow(i + 1);
                    ICell cell = row.GetCell(2);//供应商
                    if (cell == null)
                        cell = row.CreateCell(2);
                    cell.SetCellValue(supplier);
                }
                DataTable dt_bzplant = fs1702_DataAccess.GetName("bzplant");//包装场
                for (int i = 0; i < dt_bzplant.Rows.Count; i++)
                {
                    string bzplant = dt_bzplant.Rows[i]["vcName"].ToString();
                    IRow row = sheet.GetRow(i + 1);
                    if (row == null)
                        row = sheet.CreateRow(i + 1);
                    ICell cell = row.GetCell(3);//包装场
                    if (cell == null)
                        cell = row.CreateCell(3);
                    cell.SetCellValue(bzplant);
                }

                ISheet sheet1 = hssfworkbook.GetSheetAt(1);//刷新第一个sheet页的公式
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

        public DataTable getName(string kind)
        {
            return fs1702_DataAccess.GetName(kind);
        }

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId,ref string strErrorName)
        {
            fs1702_DataAccess.importSave(dt, strUserId,ref strErrorName);
        }
        #endregion
    }

}
