using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using NPOI.XSSF.UserModel;
using System.IO;
using NPOI.SS.UserModel;

namespace Logic
{
    public class FS0309_Logic
    {
        FS0309_DataAccess fs0309_DataAccess;

        public FS0309_Logic()
        {
            fs0309_DataAccess = new FS0309_DataAccess();
        }


        #region 按检索条件检索,返回dt,注意这个dt返回的时候convert了
        public DataTable Search(string strChange, string strPart_id, string strOriginCompany, string strHaoJiu
            , string strProjectType, string strPriceChangeInfo, string strCarTypeDev, string strSupplier_id
            , string strReceiver, string strPriceState
            )
        {
            return convert(fs0309_DataAccess.Search(strChange, strPart_id, strOriginCompany, strHaoJiu
            , strProjectType, strPriceChangeInfo, strCarTypeDev, strSupplier_id
            , strReceiver, strPriceState
            ));
        }
        #endregion

        #region 相同品番行挨着，设定同一个颜色
        public DataTable convert(DataTable dt)
        {
            string strColor_A = "partFS0309A";//这两个变量是行的背景颜色class名字，具体颜色在前台画面定义
            string strColor_B = "partFS0309B";

            dt.Columns.Add("vcBgColor");
            string strTempPartId = "";
            string strTempColor = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string strPart_id = dt.Rows[i]["vcPart_id"] == DBNull.Value ? "" : dt.Rows[i]["vcPart_id"].ToString();
                if (strTempPartId == "")
                {
                    dt.Rows[i]["vcBgColor"] = strColor_A;
                    strTempPartId = strPart_id;
                    strTempColor = strColor_A;
                }
                else
                {
                    if (strTempPartId == strPart_id)
                    {
                        dt.Rows[i]["vcBgColor"] = strTempColor;
                    }
                    else 
                    {
                        strTempPartId = strPart_id;
                        strTempColor = strTempColor==strColor_A? strColor_B: strColor_A;
                        dt.Rows[i]["vcBgColor"] = strTempColor;
                    }
                }
            }
            return dt;
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId,ref string strErrorPartId)
        {
            fs0309_DataAccess.Save(listInfoData, strUserId,ref strErrorPartId);
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            fs0309_DataAccess.importSave(dt, strUserId);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0309_DataAccess.Del(listInfoData, strUserId);
        }
        #endregion

        #region 按检索条件检索,返回dt
        public DataTable Search_GS(string strBegin, string strEnd )
        {
            return fs0309_DataAccess.Search_GS(strBegin, strEnd );
        }
        #endregion

        #region 保存
        public void Save_GS(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorName)
        {
            fs0309_DataAccess.Save_GS(listInfoData, strUserId, ref strErrorName);
        }
        #endregion

        #region 删除
        public void Del_GS(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0309_DataAccess.Del_GS(listInfoData, strUserId);
        }
        #endregion

        #region 检索所有待处理的数据
        public DataTable getAllTask()
        {
            return fs0309_DataAccess.getAllTask();
        }
        #endregion


        #region 测试10万
        public DataTable test10W()
        {
            return fs0309_DataAccess.test10W();
        }
        #endregion

        #region 销售展开（根据检索条件）
        public int sendMail(string strChange, string strPart_id, string strOriginCompany, string strHaoJiu
            , string strProjectType, string strPriceChangeInfo, string strCarTypeDev, string strSupplier_id
            , string strReceiver, string strPriceState,ref string strErr
            )
        {
            return fs0309_DataAccess.sendMail(strChange, strPart_id, strOriginCompany, strHaoJiu
            , strProjectType, strPriceChangeInfo, strCarTypeDev, strSupplier_id
            , strReceiver, strPriceState, ref strErr);
        }
        #endregion

        #region 销售展开（根据所选）
        public void sendMail(List<Dictionary<string,object>> listInfoData, ref string strErr)
        {
            fs0309_DataAccess.sendMail(listInfoData, ref strErr);
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

                ICellStyle style = hssfworkbook.CreateCellStyle();
                style.BorderBottom = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Thin;


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(startRow + i);
                    for (int j = 0; j < field.Length; j++)
                    {
                        ICell cell = row.CreateCell(j);
                        cell.SetCellValue(dt.Rows[i][field[j]].ToString());
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
                return "";
            }
        }
        #endregion


        #region 根据选择公式返回对应金额
        public DataTable getGSChangePrice(string strPartId, string strSupplier, int iAutoId, string strGSName, decimal decPriceOrigin)
        {
            return fs0309_DataAccess.getGSChangePrice(strPartId,strSupplier,iAutoId,strGSName,decPriceOrigin);
        }
        #endregion


        #region 公式计算B、C需要验证该品番是否存在上个状态的数据
        public bool getLastStateGsData(string strPartId, string strSupplier, int iAutoId)
        {
            DataTable dt=fs0309_DataAccess.getLastStateGsData(strPartId, strSupplier, iAutoId);
            if (dt.Rows.Count == 0)
                return false;
            else
                return true;
        }
        #endregion

        #region 公式计算B、C需要验证该品番是否存在上个状态的数据
        public bool isGsExist(string strGs)
        {
            DataTable dt = fs0309_DataAccess.isGsExist(strGs);
            if (dt.Rows.Count == 0)
                return false;
            else
                return true;
        }
        #endregion
    }
}
