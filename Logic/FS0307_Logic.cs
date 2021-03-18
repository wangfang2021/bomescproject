using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using Common;
using DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.Office.Core;
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
        private MultiExcute excute = new MultiExcute();

        #region 获取抽取状态

        public DataTable getExtractState(string userId)
        {
            DataTable dt = fs0307_dataAccess.getExtractState();
            string OriginCompany = getOriginCompany(userId);
            string[] arrInt = OriginCompany.Split(",");
            string tmp = "";
            for (int i = 0; i < arrInt.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(tmp))
                {
                    tmp += ",";
                }
                tmp += "'" + arrInt[i].ToString() + "'";
            }

            DataRow[] rows = dt.Select("vcValue in (" + tmp + ")");
            DataTable res = rows.CopyToDataTable();

            return res;
        }

        public DataTable getCompany(string OriginCompany)
        {
            return fs0307_dataAccess.getCompany(OriginCompany);
        }

        #endregion

        #region 检索

        public DataTable searchApi(string strYear, string FinishFlag, string userId)
        {
            string OriginCompany = getOriginCompany(userId);
            string[] arrInt = OriginCompany.Split(",");
            string tmp = "";
            for (int i = 0; i < arrInt.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(tmp))
                {
                    tmp += ",";
                }
                tmp += "'" + arrInt[i].ToString() + "'";
            }
            return fs0307_dataAccess.searchApi(strYear, FinishFlag, tmp);
        }

        #endregion

        #region 删除
        public void DelApi(List<Dictionary<string, Object>> listInfoData)
        {
            fs0307_dataAccess.DelApi(listInfoData);
        }
        #endregion

        #region 年限抽取

        public void extractPart(string strUserId, List<string> vcOriginCompany, ref string message)
        {
            fs0307_dataAccess.extractPart(strUserId, vcOriginCompany, ref message);
        }

        #endregion

        #region 创建邮件体

        public List<string> CreateEmailBody(string date, string flag)
        {


            StringBuilder sbr = new StringBuilder();
            sbr.AppendLine("SELECT vcValue3,vcValue4 FROM  TOutCode WHERE vcCodeId = 'C023' AND vcIsColum = '0' AND vcValue2 = '" + flag + "'");
            List<string> res = new List<string>();
            DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            if (dt.Rows.Count > 0)
            {
                string subject = dt.Rows[0]["vcValue3"].ToString();
                string emailBody = dt.Rows[0]["vcValue4"].ToString();
                emailBody = emailBody.Replace("##yyyyMMdd##", date);
                res.Add(subject);
                res.Add(emailBody);
            }

            return res;
        }

        #endregion

        #region FTMS

        public void FTMS(List<Dictionary<string, Object>> listInfoData, string EmailBody, string EmailSubject, string strUserId, ref string refMsg, string Email, string unit)
        {
            try
            {

                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(ObjToString(listInfoData[i]["vcPM"])))
                    {
                        refMsg = "选中数据存在品目区分为空,不能进行FTMS层别展开";
                        return;
                    }
                }

                fs0307_dataAccess.FTMSCB(listInfoData, strUserId);

                //TODO 发送邮件
                string strSubject = EmailSubject;
                DataTable cCDt = null;

                DataTable receiverDt = new DataTable();
                receiverDt.Columns.Add("address");
                receiverDt.Columns.Add("displayName");
                DataTable dt = fs0307_dataAccess.getReceiverEmail();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = receiverDt.NewRow();
                    dr["address"] = dt.Rows[i]["vcValue2"].ToString();
                    dr["displayName"] = dt.Rows[i]["vcValue1"].ToString();
                    receiverDt.Rows.Add(dr);
                }


                string result = ComFunction.SendEmailInfo(Email, unit, EmailBody, receiverDt, cCDt, strSubject, "", false);

                //邮件发送失败
                if (result.Equals("Error"))
                {
                    refMsg = "FTMS层别展开成功，但邮件发送失败。";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        #endregion



        #region 展开账票



        public void ZKZP(List<Dictionary<string, Object>> listInfoData, string strUserId, string emailBody, string EmailSubject, string path, ref string refMsg, string Email, string unit, string Filepath)
        {
            try
            {


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
                List<int> id = new List<int>();
                foreach (MailSend mailSend in list)
                {
                    foreach (int tmp in mailSend.id)
                    {
                        id.Add(tmp);
                    }
                }
                //账票展开
                fs0307_dataAccess.ZKZP(id, strUserId);

                DataTable dtEmail = fs0307_dataAccess.getSupplierEmail();

                refMsg = "账票展开成功。";
                string errorList = "";

                //对每个供应商发送邮件，记录失败供应商前台显示
                for (int i = 0; i < list.Count; i++)
                {
                    //TODO 生成附件
                    DataTable dt = fs0307_dataAccess.getFile(list[i].id);
                    string file = generateExcelWithXlt(dt, path, "FS0307_template.xlsx", list[i].supplierId);
                    string fileName = file;
                    if (!string.IsNullOrWhiteSpace(file))
                    {
                        file = Filepath + file;
                    }
                    //存储文件到固定位置
                    string fileSavePath = path + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "ZPZK" + Path.DirectorySeparatorChar;

                    if (Directory.Exists(fileSavePath) == false)
                    {
                        Directory.CreateDirectory(fileSavePath);
                    }

                    fileSavePath = fileSavePath + fileName;
                    System.IO.File.Copy(file, fileSavePath, true);
                    //
                    DataTable receiverDt = new DataTable();
                    receiverDt.Columns.Add("address");
                    receiverDt.Columns.Add("displayName");

                    DataRow[] dr = dtEmail.Select("vcSupplier_id = '" + list[i].supplierId + "'");

                    for (int j = 0; j < dr.Length; j++)
                    {
                        DataRow dr1 = receiverDt.NewRow();
                        dr1["address"] = dr[j]["vcEmail1"].ToString();
                        dr1["displayName"] = dr[j]["vcLXR1"].ToString();
                        receiverDt.Rows.Add(dr1);
                    }

                    DataTable cCDt = null;

                    string strSubject = EmailSubject;

                    string result = ComFunction.SendEmailInfo(Email, unit, emailBody, receiverDt, cCDt, strSubject, file, true);


                    //失败记录失败供应商
                    if (result.Equals("Error"))
                    {
                        if (!string.IsNullOrWhiteSpace(errorList))
                        {
                            errorList += ";";
                        }

                        errorList += list[i].supplierId;
                    }
                }

                if (!string.IsNullOrWhiteSpace(errorList))
                {
                    refMsg += "但以下供应商:" + errorList + "邮件发送失败。";
                }
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

        public void SaveApi(List<Dictionary<string, Object>> list, string strUserId, ref string Msg)
        {
            fs0307_dataAccess.SaveApi(list, strUserId, ref Msg);
        }

        #endregion

        #region 织入原单位

        public void InsertUnitApi(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string Msg)
        {
            fs0307_dataAccess.InsertUnitApi(listInfoData, strUserId, ref Msg);
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
                    sheet.ShiftRows(startRowIndex, sheet.LastRowNum, dt.Rows.Count - 1, true, false);
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
                    row.GetCell(12).SetCellValue(ObjToString(dt.Rows[i]["dSSDate"]));
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
            }
        }

        public string getOriginCompany(string userId)
        {
            return fs0307_dataAccess.getOriginCompany(userId);
        }

        public List<string> OriginCompanys(string userId)
        {
            List<string> list = new List<string>();
            string ori = fs0307_dataAccess.getOriginCompany(userId);
            string[] arrInt = ori.Split(",");
            for (int i = 0; i < arrInt.Length; i++)
            {
                list.Add(arrInt[i]);
            }

            return list;
        }


    }
}
