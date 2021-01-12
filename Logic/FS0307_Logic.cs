using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Common;
using DataAccess;
using Microsoft.AspNetCore.Http;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.Formula.Functions;
using NPOI.SS.Formula.PTG;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace Logic
{
    public class FS0307_Logic
    {
        FS0307_DataAccess fs0307_dataAccess = new FS0307_DataAccess();

        #region 检索

        public DataTable searchApi(string strYear, string FinishFlag, string SYT, string Receiver, List<string> origin)
        {
            return fs0307_dataAccess.searchApi(strYear, FinishFlag, SYT, Receiver, origin);
        }

        #endregion

        #region 删除
        public void DelApi(List<Dictionary<string, Object>> listInfoData)
        {
            fs0307_dataAccess.DelApi(listInfoData);
        }
        #endregion

        #region 年限抽取

        public void extractPart(string strUserId, List<string> vcOriginCompany)
        {
            fs0307_dataAccess.extractPart(strUserId, vcOriginCompany);
        }

        #endregion

        #region FTMS

        public void FTMS(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0307_dataAccess.FTMSCB(listInfoData, strUserId);
        }

        #endregion

        #region 展开账票

        public string CreateEmailBody(string date)
        {
            StringBuilder sbr = new StringBuilder();
            sbr.AppendLine("展开期限");
            sbr.AppendLine(date);
            return sbr.ToString();
        }

        public bool ZKZP(List<Dictionary<string, Object>> listInfoData, string strUserId, string emailBody, string path)
        {
            try
            {
                bool isSuccess = true;
                //记录列表
                List<MailSend> list = new List<MailSend>();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAuto_id"]);
                    string supplierId = listInfoData[i]["vcSupplier_id"].ToString();
                    int index = -1;
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (list[j].supplierId.Equals(supplierId))
                        {
                            index = j;
                            break;
                        }
                    }

                    if (index == -1)
                    {
                        list.Add(new MailSend(supplierId, iAutoId));
                    }
                    else
                    {
                        list[index].id.Add(iAutoId);
                    }
                }
                //对每个供应商发送邮件，成功则记录
                for (int i = 0; i < list.Count; i++)
                {
                    //TODO 生成附件
                    DataTable dt = fs0307_dataAccess.getFile(list[i].id);
                    string file = generateExcelWithXlt(dt, path, "FS0307_template.xlsx", list[i].supplierId);
                    if (string.IsNullOrWhiteSpace(file))
                    {
                        return false;
                    }
                    //TODO 发送邮件

                    //成功发送邮件,记录结果
                    fs0307_dataAccess.ZKZP(list[i].id, strUserId);
                }

                return isSuccess;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region MailClass

        public class MailSend
        {
            public MailSend(string supplierId, int id)
            {
                this.supplierId = supplierId;
                this.id = new List<int>() { id };
            }
            public string supplierId;
            public List<int> id;

        }

        #endregion

        #region 导入后保存

        public void importSave(DataTable dt, string strUserId)
        {
            fs0307_dataAccess.importSave(dt, strUserId);
        }

        #endregion

        #region 保存

        public void SaveApi(List<Dictionary<string, Object>> list, string strUserId)
        {
            fs0307_dataAccess.SaveApi(list, strUserId);
        }

        #endregion

        #region 织入原单位

        public void InsertUnitApi(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0307_dataAccess.InsertUnitApi(listInfoData, strUserId);
        }

        #endregion

        #region 导出账票

        public static string generateExcelWithXlt(DataTable dt, string rootPath, string xltName, string supplierId)
        {
            try
            {
                XSSFWorkbook xssfworkbook = new XSSFWorkbook();

                string XltPath = rootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Template" + Path.DirectorySeparatorChar + xltName;
                using (FileStream fs = File.OpenRead(XltPath))
                {
                    xssfworkbook = new XSSFWorkbook(fs);
                    fs.Close();
                }

                ISheet sheet = xssfworkbook.GetSheetAt(0);
                int startRowIndex = 6;

                sheet.GetRow(1).GetCell(27).SetCellValue(DateTime.Now.ToString("yyyy/MM/dd"));

                ICellStyle borderStyle = xssfworkbook.CreateCellStyle();
                borderStyle.BorderBottom = BorderStyle.Thin;
                borderStyle.BorderLeft = BorderStyle.Thin;
                borderStyle.BorderRight = BorderStyle.Thin;
                borderStyle.BorderTop = BorderStyle.Thin;
                borderStyle.BottomBorderColor = HSSFColor.Black.Index;
                borderStyle.LeftBorderColor = HSSFColor.Black.Index;
                borderStyle.RightBorderColor = HSSFColor.Black.Index;
                borderStyle.TopBorderColor = HSSFColor.Black.Index;

                ICellStyle borderStyleLeft = xssfworkbook.CreateCellStyle();
                borderStyleLeft.BorderBottom = BorderStyle.Thin;
                borderStyleLeft.BorderLeft = BorderStyle.Thin;
                borderStyleLeft.BorderTop = BorderStyle.Thin;
                borderStyleLeft.BottomBorderColor = HSSFColor.Black.Index;
                borderStyleLeft.LeftBorderColor = HSSFColor.Black.Index;
                borderStyleLeft.TopBorderColor = HSSFColor.Black.Index;

                ICellStyle borderStyleLeftl = xssfworkbook.CreateCellStyle();
                borderStyleLeftl.BorderBottom = BorderStyle.Thin;
                borderStyleLeftl.BorderLeft = BorderStyle.Thick;
                borderStyleLeftl.BorderTop = BorderStyle.Thin;
                borderStyleLeftl.BottomBorderColor = HSSFColor.Black.Index;
                borderStyleLeftl.LeftBorderColor = HSSFColor.Black.Index;
                borderStyleLeftl.TopBorderColor = HSSFColor.Black.Index;

                ICellStyle borderStyleRight = xssfworkbook.CreateCellStyle();
                borderStyleRight.BorderBottom = BorderStyle.Thin;
                borderStyleRight.BorderRight = BorderStyle.Thick;
                borderStyleRight.BorderTop = BorderStyle.Thin;
                borderStyleRight.BottomBorderColor = HSSFColor.Black.Index;
                borderStyleRight.RightBorderColor = HSSFColor.Black.Index;
                borderStyleRight.TopBorderColor = HSSFColor.Black.Index;

                ICellStyle borderStyleMiddle = xssfworkbook.CreateCellStyle();
                borderStyleMiddle.BorderBottom = BorderStyle.Thin;
                borderStyleMiddle.BorderTop = BorderStyle.Thin;
                borderStyleMiddle.BottomBorderColor = HSSFColor.Black.Index;
                borderStyleMiddle.TopBorderColor = HSSFColor.Black.Index;


                if (dt.Rows.Count > 2)
                {
                    sheet.ShiftRows(startRowIndex, sheet.LastRowNum, dt.Rows.Count - 2, true, false);
                    for (int i = startRowIndex; i < startRowIndex + dt.Rows.Count - 3; i++)
                    {
                        var rowInsert = sheet.CreateRow(i);

                        for (int col = 0; col < 5; col++)
                        {
                            var cellInsert = rowInsert.CreateCell(col);
                            cellInsert.CellStyle = borderStyle;
                        }
                    }
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(i + 6);
                    row.Height = 660;
                    for (int j = 0; j < 28; j++)
                    {
                        row.CreateCell(j).CellStyle = borderStyle;
                    }
                    row.GetCell(0).SetCellValue(i + 1);
                    row.GetCell(1).SetCellValue(ObjToString(dt.Rows[i]["vcSupplier_id"]));
                    row.GetCell(2).SetCellValue(ObjToString(dt.Rows[i]["vcPart_id"]));
                    row.GetCell(3).SetCellValue(ObjToString(dt.Rows[i]["vcPartNameEn"]));
                    row.GetCell(4).SetCellValue(ObjToString(dt.Rows[i]["vcCarTypeDev"]));
                    row.GetCell(5).SetCellValue(ObjToString(dt.Rows[i]["dJiuBegin"]));
                    row.GetCell(6).SetCellValue(ObjToString(dt.Rows[i]["vcPM"]));
                    row.GetCell(7).SetCellValue(ObjToString(dt.Rows[i]["vcNum1"]));
                    row.GetCell(8).SetCellValue(ObjToString(dt.Rows[i]["vcNum2"]));
                    row.GetCell(9).SetCellValue(ObjToString(dt.Rows[i]["vcNum3"]));
                    row.GetCell(10).SetCellValue(ObjToString(dt.Rows[i]["vcNumAvg"]));
                    row.GetCell(11).SetCellValue(ObjToString(dt.Rows[i]["vcNXQF"]));
                    row.GetCell(12).SetCellValue(ObjToString(dt.Rows[i]["dTimeFrom"]));
                    row.GetCell(13).SetCellValue(ObjToString(dt.Rows[i]["vcNum11"]));
                    row.GetCell(14).SetCellValue(ObjToString(dt.Rows[i]["vcNum12"]));
                    row.GetCell(15).SetCellValue(ObjToString(dt.Rows[i]["vcNum13"]));
                    row.GetCell(16).SetCellValue(ObjToString(dt.Rows[i]["vcNum14"]));
                    row.GetCell(17).SetCellValue(ObjToString(dt.Rows[i]["vcNum15"]));
                    row.GetCell(18).SetCellValue(ObjToString(dt.Rows[i]["vcNum16"]));
                    row.GetCell(19).SetCellValue(ObjToString(dt.Rows[i]["vcNum17"]));
                    row.GetCell(20).SetCellValue(ObjToString(dt.Rows[i]["vcNum18"]));
                    row.GetCell(21).SetCellValue(ObjToString(dt.Rows[i]["vcNum19"]));
                    row.GetCell(22).SetCellValue(ObjToString(dt.Rows[i]["vcNum20"]));
                    row.GetCell(23).SetCellValue(ObjToString(dt.Rows[i]["vcNum21"]));

                    row.GetCell(24).CellStyle = borderStyleLeftl;
                    row.GetCell(25).CellStyle = borderStyleLeft;
                    row.GetCell(26).CellStyle = borderStyleMiddle;
                    row.GetCell(27).CellStyle = borderStyleRight;
                }
                string strFileName = supplierId + "_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + "_账票明细.xlsx";
                string fileSavePath = rootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar;//文件临时目录，导入完成后 删除
                string path = fileSavePath + strFileName;
                using (FileStream fs = File.OpenWrite(path))
                {
                    xssfworkbook.Write(fs);//向打开的这个xls文件中写入数据  
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

        public static string ObjToString(Object obj)
        {
            try
            {
                return obj.ToString();
            }
            catch (Exception ex)
            {
                return "";
                throw;
            }
        }
    }
}
