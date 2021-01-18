using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ThoughtWorks.QRCode.Codec;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Data;
using System.Data.SqlClient;

namespace Common
{
    public class PrinterCR
    {
        private MultiExcute excute = new MultiExcute();

        #region 将照片转换为二进制数组
        /// <param name="path">文件路径</param>
        /// <returns>二进制流</returns>
        public byte[] PhotoToArray(string path, string path2)
        {
            try
            {
                FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                byte[] bufferPhoto = new byte[stream.Length];
                stream.Read(bufferPhoto, 0, Convert.ToInt32(stream.Length));
                stream.Flush();
                stream.Close();
                return bufferPhoto;
            }
            catch
            {
                FileStream stream = new FileStream(path2, FileMode.Open, FileAccess.Read);
                byte[] bufferPhoto = new byte[stream.Length];
                stream.Read(bufferPhoto, 0, Convert.ToInt32(stream.Length));
                stream.Flush();
                stream.Close();
                return bufferPhoto;
            }
        }
        #endregion

        #region 生成QRCODE二维码
        /// <param name="msg">二维码序列号</param>
        /// <returns>二维码二进制流</returns>
        public byte[] GenGenerateQRCode(string msg, String ls_savePath)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            qrCodeEncoder.QRCodeVersion = 11;
            qrCodeEncoder.QRCodeScale = 2;
            String data = msg;

            String ls_fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".png";

            qrCodeEncoder.Encode(data).Save(ls_savePath);
            FileStream stream = new FileStream(ls_savePath, FileMode.Open, FileAccess.Read);
            byte[] bufferPhoto = new byte[stream.Length];
            stream.Read(bufferPhoto, 0, Convert.ToInt32(stream.Length));
            stream.Flush();
            stream.Close();
            //删除该照片
            if (File.Exists(ls_savePath))
            {
                File.Delete(ls_savePath);
            }
            return bufferPhoto;
        }
        #endregion

        #region 二维码数据整理
        public string reCode(string vcSupplierCode, string vcSupplierPlant, string vcDock, string vcPartsNo, string iQuantityPerContainer, string vcKBSerial, string vcEDflag, string vcKBorderno)
        {
            string strcode = "";
            strcode += " ";
            strcode += vcSupplierCode != "" ? vcSupplierCode : "    ";
            strcode += vcSupplierPlant != "" ? vcSupplierPlant : " ";
            strcode += "        ";
            strcode += vcDock != "" ? vcDock : "  ";
            strcode += vcPartsNo != "" ? vcPartsNo : "            ";
            strcode += iQuantityPerContainer != "" ? iQuantityPerContainer.PadLeft(5, '0').ToString() : "     ";
            strcode += "                        ";
            strcode += vcKBSerial != "" ? vcKBSerial : "    ";
            strcode += "                                                           ";
            strcode += "NZ";
            strcode += "                                                        ";
            strcode += vcEDflag != "" ? vcEDflag : " ";
            strcode += "                                        ";
            if (vcKBorderno.Length < 12)
            {
                int kblen = vcKBorderno.Length;
                for (int i = 0; i < 12 - kblen; i++)
                {
                    vcKBorderno = vcKBorderno + " ";
                }
            }
            strcode += vcKBorderno;

            strcode += "  ";
            int qq = strcode.Length;
            return strcode;
        }
        #endregion

        /// <summary>
        /// 打印水晶报表
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="printerName"></param>
        /// <param name="reportName"></param>
        /// <param name="left"></param>
        /// <param name="up"></param>
        public void printAllCrReport(DataTable dt, string printerName, string reportName, string left, string up, string SerPath)
        {

            ClassINI ini = new ClassINI(System.AppDomain.CurrentDomain.BaseDirectory + "\\QMClass.ini");
            string ServerName = "";
            string DatabaseName = "";
            string UserID = "";
            string Password = "";
            if (ini.IniReadValue("classini", "ServerName") != "")
            {
                ServerName = ini.IniReadValue("classini", "ServerName");
            }
            if (ini.IniReadValue("classini", "DatabaseName") != "")
            {
                DatabaseName = ini.IniReadValue("classini", "DatabaseName");
            }
            if (ini.IniReadValue("classini", "UserID") != "")
            {
                UserID = ini.IniReadValue("classini", "UserID");
            }
            if (ini.IniReadValue("classini", "Password") != "")
            {
                Password = ini.IniReadValue("classini", "Password");
            }
            ReportDocument rd = new ReportDocument();
            try
            {
                rd.Load(reportName);
                TableLogOnInfo logOnInfo = new TableLogOnInfo();
                logOnInfo.ConnectionInfo.ServerName = ServerName;
                logOnInfo.ConnectionInfo.DatabaseName = DatabaseName;
                logOnInfo.ConnectionInfo.UserID = UserID;
                logOnInfo.ConnectionInfo.Password = Password;
                rd.Database.Tables[0].ApplyLogOnInfo(logOnInfo);
                rd.PrintOptions.PrinterName = printerName;
                //PageMargins margins = rd.PrintOptions.PageMargins;
                //margins.leftMargin = 233;
                //margins.bottomMargin = 232;
                //margins.rightMargin = 232;
                //margins.topMargin = 233;
                //rd.PrintOptions.ApplyPageMargins(margins);
                rd.SetDataSource(dt);
                //FileInfo file = new FileInfo(SerPath);
                //file.Delete();
                //rd.ExportToDisk(ExportFormatType.WordForWindows, SerPath);
                rd.PrintToPrinter(1, true, 0, 0);
                //rd.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                rd.Close();
                rd.Dispose();
                if (dt.Rows.Count < 10)
                {
                    System.Threading.Thread.Sleep(5000);
                }
            }
        }

        private void KillExcelProcess(string p)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 打印水晶报表
        /// </summary>
        /// <param name="reportPath"></param>
        /// <returns></returns>
        public bool printCr(string reportPath, string vcProType, string vcorderno, string vcComDate01, string vcBanZhi01, string vcComDate00, string vcBanZhi00, string vcUser, string strPrinterName)
        {
            try
            {
                DataTable dt = searchPrintCRMain(vcProType, vcorderno, vcComDate01, vcBanZhi01, vcComDate00, vcBanZhi00);//检索打印数据主表
                //QMCommon.OfficeCommon.QMCrPrinter crystalReportCommon = new QMCrPrinter();
                ///获取打印机名
                if (dt.Rows.Count > 0)
                {
                    ReportDocument rd = new ReportDocument();
                    rd.Load(reportPath);
                    rd.PrintOptions.PrinterName = strPrinterName;
                    PageMargins margins = rd.PrintOptions.PageMargins;
                    margins.leftMargin = 0;
                    margins.bottomMargin = 0;
                    margins.rightMargin = 0;
                    margins.topMargin = 0;
                    rd.PrintOptions.ApplyPageMargins(margins);
                    rd.SetDataSource(dt);
                    rd.PrintToPrinter(1, false, 0, 0);
                    rd.Close();


                    //printAllCrReport(dt, strPrinterName, reportPath, "0", "0", "");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 检索打印数据主表
        /// </summary>
        /// <returns></returns>
        private DataTable searchPrintCRMain(string vcProType, string vcorderno, string vcComDate01, string vcBanZhi01, string vcComDate00, string vcBanZhi00)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("SELECT vcNo1,vcNo2,vcNo3 FROM [testprinterCRMAIN]   where vcPorType='" + vcProType + "' ");

            if (vcorderno != "")
            {
                strSQL.AppendLine("  and vcorderno='" + vcorderno + "'");
            }
            if (vcComDate01 != "")
            {
                strSQL.AppendLine("  and vcComDate01='" + vcComDate01 + "'");
            }
            if (vcBanZhi01 != "")
            {
                strSQL.AppendLine("  and vcBanZhi01='" + vcBanZhi01 + "'");
            }
            if (vcComDate00 != "")
            {
                strSQL.AppendLine(" and vcComDate00='" + vcComDate00 + "'");
            }
            if (vcBanZhi00 != "")
            {
                strSQL.AppendLine("  and vcBanZhi00='" + vcBanZhi00 + "'");
            }
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }

        public DataTable searchPorTypeExcep()
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select distinct vcPorType  from testprinterCRExcep");
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }

        #region 看板打印信息160 目前没有引用该方法的实例
        /// <summary>
        /// 看板打印信息160 目前没有引用该方法的实例
        /// </summary>
        /// <param name="vcKBorderno"></param>
        /// <param name="vcKBSerial"></param>
        /// <returns></returns>
        public DataTable searchPrintKANB(string vcKBorderno, string vcKBSerial, string vcPartsNo, string vcDock, string vctype)
        {
            DataTable dt = new DataTable();
            vcPartsNo = typesertch(vctype, vcPartsNo, vcDock);
            if (vctype == "3")
            {
                vcDock = vcPartsNo.Substring(0, 2).ToString();
                vcPartsNo = vcPartsNo.Substring(2, 12).ToString();
            }
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select B.vcSupplierCode,B.vcSupplierPlant,B.vcCpdCompany,");
            strSQL.AppendLine("       B.vcPartsNameEN,B.vcPartsNameCHN,B.vcLogisticRoute,");
            strSQL.AppendLine("       A.vcQuantityPerContainer as iQuantityPerContainer,");
            strSQL.AppendLine("       isnull(A.vcProject01,'') as vcProject01,isnull(A.vcComDate01,'') as vcComDate01,(case when A.vcBanZhi01='1' then '夜' when A.vcBanZhi01='0' then '白' else '' end) as vcBanZhi01,");
            strSQL.AppendLine("       isnull(A.vcProject02,'') as vcProject02,isnull(A.vcComDate02,'') as vcComDate02,(case when A.vcBanZhi02='1' then '夜' when A.vcBanZhi02='0' then '白' else '' end) as vcBanZhi02,");
            strSQL.AppendLine("       isnull(A.vcProject03,'') as vcProject03,isnull(A.vcComDate03,'') as vcComDate03,(case when A.vcBanZhi03='1' then '夜' when A.vcBanZhi03='0' then '白' else '' end) as vcBanZhi03,");
            strSQL.AppendLine("       isnull(A.vcProject04,'') as vcProject04,isnull(A.vcComDate04,'') as vcComDate04,(case when A.vcBanZhi04='1' then '夜' when A.vcBanZhi04='0' then '白' else '' end) as vcBanZhi04,");
            strSQL.AppendLine("       isnull(B.vcRemark1,'') as vcRemark1,isnull(B.vcRemark2,'') as vcRemark2,b.vcPorType from");
            strSQL.AppendLine("(select * from tKanbanPrintTbl) A");
            strSQL.AppendLine("left join ");
            strSQL.AppendLine("(select * from tPartInfoMaster) B");
            strSQL.AppendLine("on A.vcPartsNo=B.vcPartsNo and   A.vcDock=B.vcDock");
            strSQL.AppendLine("    WHERE A.[vcKBorderno]='" + vcKBorderno + "' and A.[vcKBSerial]='" + vcKBSerial + "' and A.vcPartsNo='" + vcPartsNo + "' and  A.vcDock='" + vcDock + "'");

            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }
        #endregion

        #region 获取FS0160看板打印页面的看板打印的打印信息，vctype是标示符|3 是秦丰非ED |<>3 是秦丰ED和非秦丰，在秦丰非ED的打印中需要获取相应的ED的供应商工区
        public DataTable searchPrintKANBALL(DataTable dt, string vctype, int i)
        {
            StringBuilder strSQL = new StringBuilder();
            DataTable dtreturn = new DataTable();
            if (vctype != "3")
            {
                string vcPartsNo = dt.Rows[i]["vcPartsNo"].ToString().Replace("-", "").Trim();
                string vcDock = dt.Rows[i]["vcDock"].ToString().Trim();
                string vcplantMonth = dt.Rows[i]["vcPlanMonth"].ToString().Trim();
                string iNo = dt.Rows[i]["iNo"].ToString().Trim();
                strSQL.AppendLine("SELECT (case when A.vcPrintflagED is not null then A.vcPrintflagED else A.vcPartsNo END) AS vcPartsNo, ");
                strSQL.AppendLine("        B.vcSupplierCode,b.vcSupplierPlant, B.vcCpdCompany,A.vcCarType AS vcCarFamilyCode,A.vcKBorderno,(case when A.vcDockED is not null then A.vcDockED else A.vcDock END) AS vcDock,");
                strSQL.AppendLine("        A.vcEDflag,B.vcPartsNameEN,B.vcPartsNameCHN,B.vcLogisticRoute, ");
                strSQL.AppendLine("        A.vcQuantityPerContainer as iQuantityPerContainer,");
                strSQL.AppendLine("isnull(A.vcProject01,'') as vcProject01,isnull(A.vcComDate01,'') as vcComDate01,(case when A.vcBanZhi01='1' then '夜' when A.vcBanZhi01='0' then '白' else '' end) as vcBanZhi01,isnull(A.vcAB01,'') as vcAB01,");//20180921添加AB值信息 - 李兴旺
                strSQL.AppendLine("isnull(A.vcProject02,'') as vcProject02,isnull(A.vcComDate02,'') as vcComDate02,(case when A.vcBanZhi02='1' then '夜' when A.vcBanZhi02='0' then '白' else '' end) as vcBanZhi02,isnull(A.vcAB02,'') as vcAB02,");//20180921添加AB值信息 - 李兴旺
                strSQL.AppendLine("isnull(A.vcProject03,'') as vcProject03,isnull(A.vcComDate03,'') as vcComDate03,(case when A.vcBanZhi03='1' then '夜' when A.vcBanZhi03='0' then '白' else '' end) as vcBanZhi03,isnull(A.vcAB03,'') as vcAB03,");//20180921添加AB值信息 - 李兴旺
                strSQL.AppendLine("isnull(A.vcProject04,'') as vcProject04,isnull(A.vcComDate04,'') as vcComDate04,(case when A.vcBanZhi04='1' then '夜' when A.vcBanZhi04='0' then '白' else '' end) as vcBanZhi04,isnull(A.vcAB04,'') as vcAB04,");//20180921添加AB值信息 - 李兴旺
                strSQL.AppendLine("isnull(B.vcRemark1,'') as vcRemark1,isnull(B.vcRemark2,'') as vcRemark2,A.vcKBSerial,B.vcPhotoPath,B.vcPorType from ");
                strSQL.AppendLine(" (select * from tKanbanPrintTbl) A");
                strSQL.AppendLine(" left join ");
                strSQL.AppendLine(" (select * from tPartInfoMaster) B");
                strSQL.AppendLine(" on A.vcPartsNo=B.vcPartsNo and   A.vcDock=B.vcDock");
                strSQL.AppendLine(" where A.iNo='" + iNo + "'");
                //strSQL.AppendLine(" where (A.[vcKBorderno]+A.[vcKBSerial]+A.vcPartsNo+A.vcDock)=('"+dt.Rows[i]["vcKBorderno"].ToString().Trim() + dt.Rows[i]["vcKBSerial"].ToString().Trim() + vcPartsNo.Trim() + vcDock.Trim()+"')");
                strSQL.AppendLine(" and (Convert(varchar(6),(CONVERT(datetime,B.dTimeFrom,101)),112)<='" + vcplantMonth.Replace("-", "") + "' and Convert(varchar(6),(CONVERT(datetime,B.dTimeTo,101)),112)>='" + vcplantMonth.Replace("-", "") + "')");
                dtreturn = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            }
            if (vctype == "3")
            {
                string vcPartsNo = dt.Rows[i]["vcPartsNo"].ToString().Replace("-", "").Trim();
                string vcDock = dt.Rows[i]["vcDock"].ToString().Trim();
                string vcplantMonth = dt.Rows[i]["vcPlanMonth"].ToString().Trim();
                string iNo = dt.Rows[i]["iNo"].ToString().Trim();
                string vcCarType = dt.Rows[i]["vcCarFamilyCode"].ToString().Trim();
                //获取白件相应黑件在Master中的车型信息
                //string vcCarType = BreakCarType(vcPartsNo, vcDock, vcplantMonth);
                strSQL.AppendLine("SELECT  A.vcPrintflagED AS vcPartsNo,A.vcDockED AS vcDock, ");
                strSQL.AppendLine("        B.vcSupplierCode,'vcSupplierPlant' as vcSupplierPlant, B.vcCpdCompany,A.vcCarType AS vcCarFamilyCode,A.vcKBorderno, ");
                strSQL.AppendLine("        A.vcEDflag,B.vcPartsNameEN,B.vcPartsNameCHN,B.vcLogisticRoute,   ");
                strSQL.AppendLine("        A.vcQuantityPerContainer as iQuantityPerContainer, ");
                strSQL.AppendLine("isnull(A.vcProject01,'') as vcProject01,isnull(A.vcComDate01,'') as vcComDate01,(case when A.vcBanZhi01='1' then '夜' when A.vcBanZhi01='0' then '白' else '' end) as vcBanZhi01,isnull(A.vcAB01,'') as vcAB01, ");//20180921添加AB值信息 - 李兴旺
                strSQL.AppendLine("isnull(A.vcProject02,'') as vcProject02,isnull(A.vcComDate02,'') as vcComDate02,(case when A.vcBanZhi02='1' then '夜' when A.vcBanZhi02='0' then '白' else '' end) as vcBanZhi02,isnull(A.vcAB02,'') as vcAB02, ");//20180921添加AB值信息 - 李兴旺
                strSQL.AppendLine("isnull(A.vcProject03,'') as vcProject03,isnull(A.vcComDate03,'') as vcComDate03,(case when A.vcBanZhi03='1' then '夜' when A.vcBanZhi03='0' then '白' else '' end) as vcBanZhi03,isnull(A.vcAB03,'') as vcAB03, ");//20180921添加AB值信息 - 李兴旺
                strSQL.AppendLine("isnull(A.vcProject04,'') as vcProject04,isnull(A.vcComDate04,'') as vcComDate04,(case when A.vcBanZhi04='1' then '夜' when A.vcBanZhi04='0' then '白' else '' end) as vcBanZhi04,isnull(A.vcAB04,'') as vcAB04, ");//20180921添加AB值信息 - 李兴旺
                strSQL.AppendLine("isnull(B.vcRemark1,'') as vcRemark1,isnull(B.vcRemark2,'') as vcRemark2,A.vcKBSerial,B.vcPhotoPath,B.vcPorType from ");
                strSQL.AppendLine("  (select * from tKanbanPrintTbl) A ");
                strSQL.AppendLine(" left join ");
                strSQL.AppendLine(" (select * from tPartInfoMaster) B");
                strSQL.AppendLine("  on A.vcPrintflagED=B.vcPartsNo");
                strSQL.AppendLine(" where A.iNo='" + iNo + "'");
                //strSQL.AppendLine("  where (A.[vcKBorderno]+A.[vcKBSerial]+A.vcPrintflagED)=('" + dt.Rows[i]["vcKBorderno"].ToString().Trim() + dt.Rows[i]["vcKBSerial"].ToString().Trim() + vcPartsNo.Trim() + "')");
                strSQL.AppendLine(" and (Convert(varchar(6),(CONVERT(datetime,B.dTimeFrom,101)),112)<='" + vcplantMonth.Replace("-", "") + "' and Convert(varchar(6),(CONVERT(datetime,B.dTimeTo,101)),112)>='" + vcplantMonth.Replace("-", "") + "')  and B.vcCarFamilyCode='" + vcCarType + "'");
                dtreturn = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
                if (dtreturn.Rows.Count != 0)
                {
                    if (dtreturn.Rows[0]["vcSupplierPlant"].ToString() == "vcSupplierPlant")
                    {
                        string SupplPlant = BreakSupplPlant(vcPartsNo, vcDock, vcplantMonth);
                        dtreturn.Rows[0]["vcSupplierPlant"] = SupplPlant;
                    }
                }
            }
            return dtreturn;
        }
        #endregion

        #region 获取FS0160看板打印页面的看板打印的打印信息 获取对应黑件的白件的Master的车型
        private string BreakCarType(string vcPartsNo, string vcDock, string vcPlanMonth)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" select vcPartsNo,vcDock from tKanbanPrintTbl  where vcPrintflagED='" + vcPartsNo + "' and vcDockED='" + vcDock + "' and vcPlanMonth='" + vcPlanMonth + "'");
            DataTable dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            StringBuilder strSQL1 = new StringBuilder();
            strSQL1.AppendLine("select vcCarFamilyCode from tPartInfoMaster where vcPartsNo='" + dt.Rows[0]["vcPartsNo"] + "' and vcDock='" + dt.Rows[0]["vcDock"] + "' and ( CONVERT(varchar(7),dTimeFrom,120)<='" + vcPlanMonth + "' and CONVERT(varchar(7),dTimeTo,120)>='" + vcPlanMonth + "')");
            DataTable dt1 = excute.ExcuteSqlWithSelectToDT(strSQL1.ToString());
            return dt1.Rows[0]["vcCarFamilyCode"].ToString();
        }
        #endregion

        #region 获取FS0160看板打印页面的看板打印的打印信息 获取供应商工区
        private string BreakSupplPlant(string vcPartsNo, string vcDock, string vcPlanMonth)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" select vcPartsNo,vcDock from tKanbanPrintTbl  where vcPrintflagED='" + vcPartsNo + "' and vcDockED='" + vcDock + "' and vcPlanMonth='" + vcPlanMonth + "'");
            DataTable dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            StringBuilder strSQL1 = new StringBuilder();
            strSQL1.AppendLine("select vcSupplierPlant from tPartInfoMaster where vcPartsNo='" + dt.Rows[0]["vcPartsNo"] + "' and vcDock='" + dt.Rows[0]["vcDock"] + "' and ( CONVERT(varchar(7),dTimeFrom,120)<='" + vcPlanMonth + "' and CONVERT(varchar(7),dTimeTo,120)>='" + vcPlanMonth + "')");
            DataTable dt1 = excute.ExcuteSqlWithSelectToDT(strSQL1.ToString());
            return dt1.Rows[0]["vcSupplierPlant"].ToString();
        }
        #endregion

        #region 仅被一个不被引用的方法使用
        public string typesertch(string vctype, string vcPartsNo, string vcDock)
        {
            string vcPartsNoED = vcPartsNo;
            switch (vctype)
            {
                case "1":
                    vcPartsNoED = vcPartsNo;
                    break;
                case "2":
                    vcPartsNoED = vcPartsNo;
                    break;
                case "3":
                    vcPartsNoED = searchMasterED(vcPartsNo, vcDock);
                    break;
                default:
                    vcPartsNoED = vcPartsNo;
                    break;
            }
            return vcPartsNoED;
        }
        #endregion

        public string searchMasterED(string PartNo, string vcDock)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select vcPartsNo,vcDock from tPartInfoMaster where substring(vcPartsNo,0,11)='" + PartNo.Substring(0, 10) + "' and substring(vcPartsNo,11,2)='ED'");
            DataTable dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            if (dt.Rows.Count != 0)
            {
                return (dt.Rows[0]["vcDock"].ToString() + dt.Rows[0]["vcPartsNo"].ToString());
            }
            else
            {
                return vcDock + PartNo;
            }
        }

        /// <summary>
        /// DataTable内部排序
        /// </summary>
        /// <param name="dt1"></param>
        /// <returns></returns>
        public DataTable orderDataTable(DataTable dt1)
        {
            if (dt1.Rows.Count != 0)
            {
                DataRow[] rows = dt1.Select("", "vcPorType desc");
                DataTable t = dt1.Clone();
                t.Clear();
                foreach (DataRow row in rows)
                {
                    t.ImportRow(row);
                }
                dt1 = t;
            }
            return dt1;
        }

        /// <summary>
        /// 插入看板打印临时表
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool insertTableCR(DataTable dt)
        {
            using (SqlConnection connln = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                if (connln.State != ConnectionState.Open)
                    connln.Open();
                SqlTransaction trans = connln.BeginTransaction();
                try
                {
                    SqlCommand cmdln = new SqlCommand("PK_PS0160_InsertTableCR");
                    cmdln.CommandType = CommandType.StoredProcedure;
                    cmdln.Parameters.Add("@vcSupplierCode", SqlDbType.VarChar, 5, "vcSupplierCode");
                    cmdln.Parameters.Add("@vcCpdCompany", SqlDbType.VarChar, 5, "vcCpdCompany");
                    cmdln.Parameters.Add("@vcCarFamilyCode", SqlDbType.VarChar, 5, "vcCarFamilyCode");
                    cmdln.Parameters.Add("@vcPartsNo", SqlDbType.VarChar, 14, "vcPartsNo");
                    cmdln.Parameters.Add("@vcPartsNameEN", SqlDbType.VarChar, 500, "vcPartsNameEN");
                    cmdln.Parameters.Add("@vcPartsNameCHN", SqlDbType.VarChar, 500, "vcPartsNameCHN");
                    cmdln.Parameters.Add("@vcLogisticRoute", SqlDbType.VarChar, 500, "vcLogisticRoute");
                    cmdln.Parameters.Add("@iQuantityPerContainer", SqlDbType.VarChar, 8, "iQuantityPerContainer");
                    cmdln.Parameters.Add("@vcProject01", SqlDbType.VarChar, 20, "vcProject01");
                    cmdln.Parameters.Add("@vcComDate01", SqlDbType.VarChar, 10, "vcComDate01");
                    cmdln.Parameters.Add("@vcBanZhi01", SqlDbType.VarChar, 2, "vcBanZhi01");
                    cmdln.Parameters.Add("@vcProject02", SqlDbType.VarChar, 20, "vcProject02");
                    cmdln.Parameters.Add("@vcComDate02", SqlDbType.VarChar, 10, "vcComDate02");
                    cmdln.Parameters.Add("@vcBanZhi02", SqlDbType.VarChar, 2, "vcBanZhi02");
                    cmdln.Parameters.Add("@vcProject03", SqlDbType.VarChar, 20, "vcProject03");
                    cmdln.Parameters.Add("@vcComDate03", SqlDbType.VarChar, 10, "vcComDate03");
                    cmdln.Parameters.Add("@vcBanZhi03", SqlDbType.VarChar, 2, "vcBanZhi03");
                    cmdln.Parameters.Add("@vcProject04", SqlDbType.VarChar, 20, "vcProject04");
                    cmdln.Parameters.Add("@vcComDate04", SqlDbType.VarChar, 10, "vcComDate04");
                    cmdln.Parameters.Add("@vcBanZhi04", SqlDbType.VarChar, 2, "vcBanZhi04");
                    cmdln.Parameters.Add("@vcRemark1", SqlDbType.VarChar, 500, "vcRemark1");
                    cmdln.Parameters.Add("@vcRemark2", SqlDbType.VarChar, 500, "vcRemark2");
                    cmdln.Parameters.Add("@vcKBSerial", SqlDbType.VarChar, 4, "vcKBSerial");
                    cmdln.Parameters.Add("@vcPhotoPath", SqlDbType.Image, 999999, "vcPhotoPath");
                    cmdln.Parameters.Add("@vcQRCodeImge", SqlDbType.Image, 999999, "vcQRCodeImge");
                    cmdln.Parameters.Add("@vcDock", SqlDbType.VarChar, 4, "vcDock");
                    cmdln.Parameters.Add("@vcKBorderno", SqlDbType.VarChar, 14, "vcKBorderno");
                    cmdln.Parameters.Add("@vcNo", SqlDbType.VarChar, 50, "vcNo");
                    cmdln.Parameters.Add("@vcorderno", SqlDbType.VarChar, 50, "vcorderno");
                    cmdln.Parameters.Add("@vcPorType", SqlDbType.VarChar, 50, "vcPorType");
                    cmdln.Parameters.Add("@vcEDflag", SqlDbType.VarChar, 50, "vcEDflag");
                    cmdln.Parameters.Add("@vcAB01", SqlDbType.VarChar, 10, "vcAB01");//20180921添加AB值信息 - 李兴旺
                    cmdln.Parameters.Add("@vcAB02", SqlDbType.VarChar, 10, "vcAB02");//20180921添加AB值信息 - 李兴旺
                    cmdln.Parameters.Add("@vcAB03", SqlDbType.VarChar, 10, "vcAB03");//20180921添加AB值信息 - 李兴旺
                    cmdln.Parameters.Add("@vcAB04", SqlDbType.VarChar, 10, "vcAB04");//20180921添加AB值信息 - 李兴旺
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.InsertCommand = cmdln;
                    cmdln.CommandTimeout = 0;
                    if (adapter.InsertCommand != null)
                    {
                        adapter.InsertCommand.Connection = connln;
                        adapter.InsertCommand.Transaction = trans;
                        adapter.InsertCommand.CommandTimeout = 0;
                    }
                    if (adapter.DeleteCommand != null)
                    {
                        adapter.DeleteCommand.Connection = connln;
                        adapter.DeleteCommand.Transaction = trans;
                        adapter.DeleteCommand.CommandTimeout = 0;
                    }
                    if (adapter.UpdateCommand != null)
                    {
                        adapter.UpdateCommand.Connection = connln;
                        adapter.UpdateCommand.Transaction = trans;
                        adapter.UpdateCommand.CommandTimeout = 0;
                    }
                    adapter.Update(dt);
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 插入看板确认单Excel
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool insertTableExcel(DataTable dt)
        {
            using (SqlConnection connln = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                if (connln.State != ConnectionState.Open)
                    connln.Open();
                SqlTransaction trans = connln.BeginTransaction();
                try
                {
                    SqlCommand cmdln = new SqlCommand("PK_PS0160_InsertTableExcel");
                    cmdln.Transaction = trans;
                    cmdln.CommandType = CommandType.StoredProcedure;
                    cmdln.Parameters.Add("@vcCarFamilyCode", SqlDbType.VarChar, 5, "vcCarFamlyCode");
                    cmdln.Parameters.Add("@vcpartsNo", SqlDbType.VarChar, 12, "vcpartsNo");
                    cmdln.Parameters.Add("@vcPartsNameCHN", SqlDbType.VarChar, 500, "vcPartsNameCHN");
                    cmdln.Parameters.Add("@iQuantityPerContainer", SqlDbType.VarChar, 8, "iQuantityPerContainer");
                    cmdln.Parameters.Add("@vcProject01", SqlDbType.VarChar, 20, "vcPCB01");
                    cmdln.Parameters.Add("@vcComDate01", SqlDbType.VarChar, 10, "vcPCB02");
                    cmdln.Parameters.Add("@vcBanZhi01", SqlDbType.VarChar, 2, "vcPCB03");
                    cmdln.Parameters.Add("@vcKBSerial", SqlDbType.VarChar, 4, "vcKBSerial");
                    cmdln.Parameters.Add("@vcorderno", SqlDbType.VarChar, 50, "vcKBorderno");
                    cmdln.Parameters.Add("@vcPorType", SqlDbType.VarChar, 50, "vcPorType");
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.InsertCommand = cmdln;
                    cmdln.CommandTimeout = 0;
                    if (adapter.InsertCommand != null)
                    {
                        adapter.InsertCommand.Connection = connln;
                        adapter.InsertCommand.Transaction = trans;
                        adapter.InsertCommand.CommandTimeout = 0;
                    }
                    if (adapter.DeleteCommand != null)
                    {
                        adapter.DeleteCommand.Connection = connln;
                        adapter.DeleteCommand.Transaction = trans;
                        adapter.DeleteCommand.CommandTimeout = 0;
                    }
                    if (adapter.UpdateCommand != null)
                    {
                        adapter.UpdateCommand.Connection = connln;
                        adapter.UpdateCommand.Transaction = trans;
                        adapter.UpdateCommand.CommandTimeout = 0;
                    }
                    adapter.Update(dt);
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 获取要从临时表中获取打印的生产部署160
        /// </summary>
        /// <returns></returns>
        public DataTable searchPorType()
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select vcorderno,vcPorType,vcComDate01,vcBanZhi01 from testprinterCR group by vcorderno,vcPorType,vcComDate01,vcBanZhi01 ");
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }

        /// <summary>
        /// 获取要从临时表中获取打印的生产部署170
        /// </summary>
        /// <returns></returns>
        public DataTable searchPorType00()
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select vcorderno,vcPorType,vcComDate01,vcBanZhi01,vcBanZhi00,vcComDate00 from testprinterCR group by vcorderno,vcPorType,vcComDate01,vcBanZhi01,vcBanZhi00,vcComDate00 ");
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }

        /// <summary>
        /// 插入到主表临时表
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool insertTableCRMain(DataTable dt, DataTable dtPorType)
        {
            using (SqlConnection connln = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                string vcDateTime = System.DateTime.Now.ToString();
                if (connln.State != ConnectionState.Open)
                    connln.Open();
                SqlTransaction trans = connln.BeginTransaction();
                string vcSql = "";
                try
                {
                    for (int z = 0; z < dtPorType.Rows.Count; z++)
                    {
                        DataRow[] row = dt.Select("vcPorType='" + dtPorType.Rows[z]["vcPorType"].ToString() + "' and vcorderno='" + dtPorType.Rows[z]["vcorderno"].ToString() + "' and vcComDate01='" + dtPorType.Rows[z]["vcComDate01"].ToString() + "' and vcBanZhi01='" + dtPorType.Rows[z]["vcBanZhi01"].ToString() + "'");
                        for (int i = 0; i < row.Length; i = i + 3)
                        {
                            if (i < row.Length)
                            {
                                if (i + 1 < row.Length)
                                {
                                    if (i + 2 < row.Length)
                                    {
                                        vcSql = "INSERT INTO [testprinterCRMAIN]([vcNo1],[vcNo2],[vcNo3],vcPorType,vcorderno,vcComDate01,vcBanZhi01) VALUES ('" + row[i]["vcNo"] + "','" + row[i + 1]["vcNo"] + "','" + row[i + 2]["vcNo"] + "','" + dtPorType.Rows[z]["vcPorType"] + "','" + dtPorType.Rows[z]["vcorderno"] + "','" + dtPorType.Rows[z]["vcComDate01"] + "','" + dtPorType.Rows[z]["vcBanZhi01"] + "')";
                                    }
                                    else
                                    {
                                        vcSql = "INSERT INTO [testprinterCRMAIN]([vcNo1],[vcNo2],vcPorType,vcorderno,vcComDate01,vcBanZhi01) VALUES ('" + row[i]["vcNo"] + "','" + row[i + 1]["vcNo"] + "','" + dtPorType.Rows[z]["vcPorType"] + "','" + dtPorType.Rows[z]["vcorderno"] + "','" + dtPorType.Rows[z]["vcComDate01"] + "','" + dtPorType.Rows[z]["vcBanZhi01"] + "')";
                                    }
                                }
                                else
                                {
                                    vcSql = "INSERT INTO [testprinterCRMAIN]([vcNo1],vcPorType,vcorderno,vcComDate01,vcBanZhi01) VALUES ('" + row[i]["vcNo"] + "','" + dtPorType.Rows[z]["vcPorType"] + "','" + dtPorType.Rows[z]["vcorderno"] + "','" + dtPorType.Rows[z]["vcComDate01"] + "','" + dtPorType.Rows[z]["vcBanZhi01"] + "')";
                                }
                            }
                            SqlCommand cmd = new SqlCommand(vcSql, connln);
                            cmd.Transaction = trans;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 插入到特殊打印主表临时表
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool insertTableCRMainEND(DataTable dt, DataTable dtPorType)
        {
            using (SqlConnection connln = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                string vcDateTime = System.DateTime.Now.ToString();
                if (connln.State != ConnectionState.Open)
                    connln.Open();
                SqlTransaction trans = connln.BeginTransaction();
                string vcSql = "";
                SqlCommand cmd = new SqlCommand();
                cmd.Transaction = trans;
                try
                {
                    for (int z = 0; z < dtPorType.Rows.Count; z++)
                    {
                        DataRow[] row = dt.Select("vcPorType='" + dtPorType.Rows[z]["vcPorType"].ToString() + "'");
                        for (int i = 0; i < row.Length; i = i + 3)
                        {
                            if (i < row.Length)
                            {
                                if (i + 1 < row.Length)
                                {
                                    if (i + 2 < row.Length)
                                    {
                                        vcSql = "INSERT INTO [testprinterCRMAIN]([vcNo1],[vcNo2],[vcNo3],vcPorType) VALUES ('" + row[i]["vcNo"] + "','" + row[i + 1]["vcNo"] + "','" + row[i + 2]["vcNo"] + "','" + dtPorType.Rows[z]["vcPorType"] + "')";
                                    }
                                    else
                                    {
                                        vcSql = "INSERT INTO [testprinterCRMAIN]([vcNo1],[vcNo2],vcPorType) VALUES ('" + row[i]["vcNo"] + "','" + row[i + 1]["vcNo"] + "','" + dtPorType.Rows[z]["vcPorType"] + "')";
                                    }
                                }
                                else
                                {
                                    vcSql = "INSERT INTO [testprinterCRMAIN]([vcNo1],vcPorType) VALUES ('" + row[i]["vcNo"] + "','" + dtPorType.Rows[z]["vcPorType"] + "')";
                                }
                            }
                            cmd.CommandText = vcSql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 插入到主表临时表
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool insertTableCRMain00(DataTable dt, DataTable dtPorType)
        {
            using (SqlConnection connln = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                string vcDateTime = System.DateTime.Now.ToString();
                if (connln.State != ConnectionState.Open)
                    connln.Open();
                SqlTransaction trans = connln.BeginTransaction();
                string vcSql = "";
                SqlCommand cmd = new SqlCommand();
                cmd.Transaction = trans;
                try
                {
                    for (int z = 0; z < dtPorType.Rows.Count; z++)
                    {
                        DataRow[] row = dt.Select("vcPorType='" + dtPorType.Rows[z]["vcPorType"].ToString() + "' and vcorderno='" + dtPorType.Rows[z]["vcorderno"].ToString() + "' and vcComDate01='" + dtPorType.Rows[z]["vcComDate01"].ToString() + "' and vcBanZhi01='" + dtPorType.Rows[z]["vcBanZhi01"].ToString() + "'and vcComDate00='" + dtPorType.Rows[z]["vcComDate00"].ToString() + "'and vcBanZhi00='" + dtPorType.Rows[z]["vcBanZhi00"].ToString() + "'");
                        for (int i = 0; i < row.Length; i = i + 3)
                        {
                            if (i < row.Length)
                            {
                                if (i + 1 < row.Length)
                                {
                                    if (i + 2 < row.Length)
                                    {
                                        vcSql = "INSERT INTO [testprinterCRMAIN]([vcNo1],[vcNo2],[vcNo3],vcPorType,vcorderno,vcComDate01,vcBanZhi01,vcComDate00,vcBanZhi00) VALUES ('" + row[i]["vcNo"] + "','" + row[i + 1]["vcNo"] + "','" + row[i + 2]["vcNo"] + "','" + dtPorType.Rows[z]["vcPorType"] + "','" + dtPorType.Rows[z]["vcorderno"] + "','" + dtPorType.Rows[z]["vcComDate01"] + "','" + dtPorType.Rows[z]["vcBanZhi01"] + "','" + dtPorType.Rows[z]["vcComDate00"].ToString() + "','" + dtPorType.Rows[z]["vcBanZhi00"].ToString() + "')";
                                    }
                                    else
                                    {
                                        vcSql = "INSERT INTO [testprinterCRMAIN]([vcNo1],[vcNo2],vcPorType,vcorderno,vcComDate01,vcBanZhi01,vcComDate00,vcBanZhi00) VALUES ('" + row[i]["vcNo"] + "','" + row[i + 1]["vcNo"] + "','" + dtPorType.Rows[z]["vcPorType"] + "','" + dtPorType.Rows[z]["vcorderno"] + "','" + dtPorType.Rows[z]["vcComDate01"] + "','" + dtPorType.Rows[z]["vcBanZhi01"] + "','" + dtPorType.Rows[z]["vcComDate00"].ToString() + "','" + dtPorType.Rows[z]["vcBanZhi00"].ToString() + "')";
                                    }
                                }
                                else
                                {
                                    vcSql = "INSERT INTO [testprinterCRMAIN]([vcNo1],vcPorType,vcorderno,vcComDate01,vcBanZhi01,vcComDate00,vcBanZhi00) VALUES ('" + row[i]["vcNo"] + "','" + dtPorType.Rows[z]["vcPorType"] + "','" + dtPorType.Rows[z]["vcorderno"] + "','" + dtPorType.Rows[z]["vcComDate01"] + "','" + dtPorType.Rows[z]["vcBanZhi01"] + "','" + dtPorType.Rows[z]["vcComDate00"].ToString() + "','" + dtPorType.Rows[z]["vcBanZhi00"].ToString() + "')";
                                }
                            }
                            cmd.CommandText = vcSql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 检索打印数据
        /// </summary>
        /// <returns></returns>
        private DataTable searchPrintCR()
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("SELECT [vcSupplierCode],[vcCpdCompany],[vcCarFamilyCode],[vcPartsNo], ");
            strSQL.AppendLine("       [vcPartsNameEN],[vcPartsNameCHN],[vcLogisticRoute],[iQuantityPerContainer],");
            strSQL.AppendLine("       [vcProject01],[vcComDate01],[vcBanZhi01],[vcProject02],[vcComDate02],");
            strSQL.AppendLine("       [vcBanZhi02],[vcProject03],[vcComDate03],[vcBanZhi03],[vcProject04],");
            strSQL.AppendLine("       [vcComDate04],[vcBanZhi04],[vcRemark1],[vcRemark2],[vcKBSerial],");
            strSQL.AppendLine("       [vcPhotoPath],[vcQRCodeImge],vcKBorderno");
            strSQL.AppendLine("  FROM [testprinterCR]");
            strSQL.AppendLine("");
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }

        /// <summary>
        /// 更新看板打印表
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool UpdatePrintKANB(DataTable dt, string type)
        {
            using (SqlConnection connln = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                string vcDateTime = System.DateTime.Now.ToString();
                if (connln.State != ConnectionState.Open)
                    connln.Open();
                SqlTransaction trans = connln.BeginTransaction();
                string vcSql = "";
                SqlCommand cmd = new SqlCommand();
                cmd.Transaction = trans;
                try
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string strSql = "";
                        string vcKBorderno = dt.Rows[i]["vcorderno"].ToString();
                        string vcKBSerial = dt.Rows[i]["vcKBSerial"].ToString();
                        string vcPartsNo = "";
                        string vcDock = "";
                        string vcPlanMonth = "";
                        if (type == "1")
                        {
                            vcPartsNo = dt.Rows[i]["vcPartsNo"].ToString();
                            vcDock = dt.Rows[i]["vcDock"].ToString();
                            strSql = "UPDATE [tKanbanPrintTbl] SET [vcPrintflag] ='1',[vcPrintTime] = GETDATE() where [vcKBorderno]='" + vcKBorderno + "' and [vcKBSerial]='" + vcKBSerial + "' and [vcPartsNo]='" + vcPartsNo + "' and [vcDock]='" + vcDock + "'";
                        }
                        else if (type == "2")
                        {
                            vcPartsNo = dt.Rows[i]["vcPartsNo"].ToString();
                            vcDock = dt.Rows[i]["vcDock"].ToString();
                            vcDock = dt.Rows[i]["vcDock"].ToString();
                            vcPlanMonth = dt.Rows[i]["vcplanMoth"].ToString();
                            DataTable dts = serachMaster(vcPartsNo, vcDock, vcPlanMonth);
                            strSql = "UPDATE [tKanbanPrintTbl] SET [vcPrintflag] ='1',vcKBType='1',[vcPrintTime] = GETDATE(),vcPrintflagED='" + dts.Rows[0]["vcPartsNo"].ToString() + "',vcDockED='" + dts.Rows[0]["vcDock"].ToString() + "',vcPrintTimeED=getdate() where [vcKBorderno]='" + vcKBorderno + "' and [vcKBSerial]='" + vcKBSerial + "' and [vcPartsNo]='" + vcPartsNo + "' and [vcDock]='" + vcDock + "'";
                        }
                        else if (type == "3")
                        {
                            //插入补给系统区分表
                            //更新内制看板系统iBaijianFlag字段
                            vcPartsNo = dt.Rows[i]["vcPartsNo"].ToString();
                            vcDock = dt.Rows[i]["vcDock"].ToString();
                            string up = dtKBSerialUP(vcPartsNo, vcDock, vcKBorderno, vcKBSerial);
                            if (up != "")
                            {
                                vcKBSerial = up;
                            }
                            strSql = "UPDATE [tKanbanPrintTbl] SET [iBaiJianFlag] ='1' where [vcKBorderno]='" + vcKBorderno + "' and [vcKBSerial]='" + vcKBSerial + "' and [vcPrintflagED]='" + vcPartsNo + "' and [vcDockED]='" + vcDock + "'";
                        }
                        if (strSql != "")
                        {
                            cmd.CommandText = vcSql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }

        public string dtKBSerialUP(string vcPartsNo, string vcDock, string vcKBorderno, string vcKBSerialBefore)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("SELECT [vcKBSerialBefore]");
            strSQL.AppendLine("  FROM [KBSerial_history]");
            strSQL.AppendLine(" WHERE vcPartsNo='" + vcPartsNo + "'");
            strSQL.AppendLine("       AND vcDock='" + vcDock + "'");
            strSQL.AppendLine("       AND vcKBorderno='" + vcKBorderno + "'");
            strSQL.AppendLine("       AND vcKBSerial='" + vcKBSerialBefore + "'");
            if (excute.ExcuteSqlWithSelectToDT(strSQL.ToString()).Rows.Count != 0)
            {
                return excute.ExcuteSqlWithSelectToDT(strSQL.ToString()).Rows[0]["vcKBSerialBefore"].ToString();
            }
            else
            {
                return "";
            }
        }

        public DataTable serachMaster(string vcpart, string vcdock, string vcPlanMonth)
        {
            DataTable dt = new DataTable();
            string strSQL = "";
            strSQL += "select vcPartsNo,vcDock from tPartInfoMaster where vcPartsNo like '" + vcpart.Substring(0, 10).ToString() + "%' and substring(vcPartsNo,11,2)<>'ED' and (Convert(varchar(6),(CONVERT(datetime,dTimeFrom,101)),112)<='" + vcPlanMonth.Replace("-", "") + "' and Convert(varchar(6),(CONVERT(datetime,dTimeTo,101)),112)>='" + vcPlanMonth.Replace("-", "") + "')";
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }

        /// <summary>
        /// 更新看板打印表
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool UpdatePrintKANBCRExcep(DataTable dt)
        {
            using (SqlConnection connln = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                string vcDateTime = System.DateTime.Now.ToString();
                if (connln.State != ConnectionState.Open)
                    connln.Open();
                SqlTransaction trans = connln.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.Transaction = trans;
                try
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string strSql = "";
                        string vcKBorderno = dt.Rows[i]["vcorderno"].ToString();
                        string vcKBSerial = dt.Rows[i]["vcKBSerial"].ToString();
                        string vcPartsNo = dt.Rows[i]["vcPartsNo"].ToString().Replace("-", "");
                        string vcDock = dt.Rows[i]["vcDock"].ToString();
                        strSql = "UPDATE [tKanbanPrintTblExcep] SET [vcPrintflag] ='1',[vcPrintTime] = GETDATE() where [vcKBorderno]='" + vcKBorderno + "' and [vcKBSerial]='" + vcKBSerial + "' and [vcPartsNo]='" + vcPartsNo + "' and [vcDock]='" + vcDock + "'";
                        if (strSql != "")
                        {
                            cmd.CommandText = strSql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 更新看板打印表
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="type">再发行提前打印延迟打印</param>
        /// <returns></returns>
        public bool UpdatePrintKANB170(DataTable dt, string type)
        {
            using (SqlConnection connln = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                string vcDateTime = System.DateTime.Now.ToString();
                if (connln.State != ConnectionState.Open)
                    connln.Open();
                SqlTransaction trans = connln.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.Transaction = trans;
                try
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string strSql = "";
                        string vcKBorderno = dt.Rows[i]["vcorderno"].ToString();
                        string vcKBSerial = dt.Rows[i]["vcKBSerial"].ToString();
                        string vcPartsNo = dt.Rows[i]["vcPartsNo"].ToString();
                        string vcDock = dt.Rows[i]["vcDock"].ToString();
                        if (type != "再发行")
                        {
                            strSql = "UPDATE [tKanbanPrintTbl] SET [vcPrintflag] ='1',[vcPrintTime] = GETDATE() where [vcKBorderno]='" + vcKBorderno + "' and [vcKBSerial]='" + vcKBSerial + "'  and [vcPartsNo]='" + vcPartsNo + "' and [vcDock]='" + vcDock + "'";
                        }
                        if (strSql != "")
                        {
                            cmd.CommandText = strSql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 插入看板打印临时表
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool insertTableCRExcep(DataTable dt)
        {
            using (SqlConnection connln = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                if (connln.State != ConnectionState.Open)
                    connln.Open();
                SqlTransaction trans = connln.BeginTransaction();
                try
                {
                    SqlCommand cmdln = new SqlCommand("PK_PS0160_InsertTableCRExcep");
                    cmdln.CommandType = CommandType.StoredProcedure;
                    cmdln.Parameters.Add("@vcSupplierCode", SqlDbType.VarChar, 5, "vcSupplierCode");
                    cmdln.Parameters.Add("@vcCpdCompany", SqlDbType.VarChar, 5, "vcCpdCompany");
                    cmdln.Parameters.Add("@vcCarFamilyCode", SqlDbType.VarChar, 5, "vcCarFamilyCode");
                    cmdln.Parameters.Add("@vcPartsNo", SqlDbType.VarChar, 14, "vcPartsNo");
                    cmdln.Parameters.Add("@vcPartsNameEN", SqlDbType.VarChar, 500, "vcPartsNameEN");
                    cmdln.Parameters.Add("@vcPartsNameCHN", SqlDbType.VarChar, 500, "vcPartsNameCHN");
                    cmdln.Parameters.Add("@vcLogisticRoute", SqlDbType.VarChar, 500, "vcLogisticRoute");
                    cmdln.Parameters.Add("@vcQuantityPerContainer", SqlDbType.VarChar, 8, "vcQuantityPerContainer");
                    cmdln.Parameters.Add("@vcProject01", SqlDbType.VarChar, 20, "vcProject01");
                    cmdln.Parameters.Add("@vcComDate01", SqlDbType.VarChar, 10, "vcComDate01");
                    cmdln.Parameters.Add("@vcBanZhi01", SqlDbType.VarChar, 2, "vcBanZhi01");
                    cmdln.Parameters.Add("@vcProject02", SqlDbType.VarChar, 20, "vcProject02");
                    cmdln.Parameters.Add("@vcComDate02", SqlDbType.VarChar, 10, "vcComDate02");
                    cmdln.Parameters.Add("@vcBanZhi02", SqlDbType.VarChar, 2, "vcBanZhi02");
                    cmdln.Parameters.Add("@vcProject03", SqlDbType.VarChar, 20, "vcProject03");
                    cmdln.Parameters.Add("@vcComDate03", SqlDbType.VarChar, 10, "vcComDate03");
                    cmdln.Parameters.Add("@vcBanZhi03", SqlDbType.VarChar, 2, "vcBanZhi03");
                    cmdln.Parameters.Add("@vcProject04", SqlDbType.VarChar, 20, "vcProject04");
                    cmdln.Parameters.Add("@vcComDate04", SqlDbType.VarChar, 10, "vcComDate04");
                    cmdln.Parameters.Add("@vcBanZhi04", SqlDbType.VarChar, 2, "vcBanZhi04");
                    cmdln.Parameters.Add("@vcRemark1", SqlDbType.VarChar, 500, "vcRemark1");
                    cmdln.Parameters.Add("@vcRemark2", SqlDbType.VarChar, 500, "vcRemark2");
                    cmdln.Parameters.Add("@vcKBSerial", SqlDbType.VarChar, 4, "vcKBSerial");
                    cmdln.Parameters.Add("@vcPhotoPath", SqlDbType.Image, 999999, "vcPhotoPath");
                    cmdln.Parameters.Add("@vcTip", SqlDbType.VarChar, 500, "vcTip");
                    cmdln.Parameters.Add("@vcDock", SqlDbType.VarChar, 4, "vcDock");
                    cmdln.Parameters.Add("@vcKBorderno", SqlDbType.VarChar, 14, "vcKBorderno");
                    cmdln.Parameters.Add("@vcNo", SqlDbType.VarChar, 50, "vcNo");
                    cmdln.Parameters.Add("@vcorderno", SqlDbType.VarChar, 50, "vcorderno");
                    cmdln.Parameters.Add("@vcPorType", SqlDbType.VarChar, 50, "vcPorType");
                    cmdln.Parameters.Add("@vcEDflag", SqlDbType.VarChar, 50, "vcEDflag");
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.InsertCommand = cmdln;
                    cmdln.CommandTimeout = 0;
                    if (adapter.InsertCommand != null)
                    {
                        adapter.InsertCommand.Connection = connln;
                        adapter.InsertCommand.Transaction = trans;
                        adapter.InsertCommand.CommandTimeout = 0;
                    }
                    if (adapter.DeleteCommand != null)
                    {
                        adapter.DeleteCommand.Connection = connln;
                        adapter.DeleteCommand.Transaction = trans;
                        adapter.DeleteCommand.CommandTimeout = 0;
                    }
                    if (adapter.UpdateCommand != null)
                    {
                        adapter.UpdateCommand.Connection = connln;
                        adapter.UpdateCommand.Transaction = trans;
                        adapter.UpdateCommand.CommandTimeout = 0;
                    }
                    adapter.Update(dt);
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 看板打印信息ALL0170
        /// </summary>
        /// <param name="vcKBorderno"></param>
        /// <param name="vcKBSerial"></param>
        /// <returns></returns>
        public DataTable searchPrintKANBALL(string vcPartsNo, string vcDock, string vcKBorderno, string vcKBSerial)//不必修改
        {
            string partfrom = "";
            StringBuilder strSQL = new StringBuilder();
            partfrom += vcKBorderno + vcKBSerial + vcPartsNo.ToString().Replace("-", "") + vcDock;
            strSQL.AppendLine("SELECT A.vcPartsNo  as vcPartsNo,");
            strSQL.AppendLine("       A.vcDock as vcDock,");
            strSQL.AppendLine("       B.vcSupplierCode,b.vcSupplierPlant, B.vcCpdCompany,A.vcCarType AS vcCarFamilyCode,A.vcKBorderno,a.vcEDflag,");
            strSQL.AppendLine("       B.vcPartsNameEN,B.vcPartsNameCHN,B.vcLogisticRoute, ");
            strSQL.AppendLine("       A.vcQuantityPerContainer as iQuantityPerContainer,");
            strSQL.AppendLine("       isnull(A.vcProject01,'') as vcProject01,isnull(A.vcComDate01,'') as vcComDate01,(case when A.vcBanZhi01='1' then '夜' when A.vcBanZhi01='0' then '白' else '' end) as vcBanZhi01,");
            strSQL.AppendLine("       isnull(A.vcProject02,'') as vcProject02,isnull(A.vcComDate02,'') as vcComDate02,(case when A.vcBanZhi02='1' then '夜' when A.vcBanZhi02='0' then '白' else '' end) as vcBanZhi02,");
            strSQL.AppendLine("       isnull(A.vcProject03,'') as vcProject03,isnull(A.vcComDate03,'') as vcComDate03,(case when A.vcBanZhi03='1' then '夜' when A.vcBanZhi03='0' then '白' else '' end) as vcBanZhi03,");
            strSQL.AppendLine("       isnull(A.vcProject04,'') as vcProject04,isnull(A.vcComDate04,'') as vcComDate04,(case when A.vcBanZhi04='1' then '夜' when A.vcBanZhi04='0' then '白' else '' end) as vcBanZhi04,");
            strSQL.AppendLine("       isnull(B.vcRemark1,'') as vcRemark1,isnull(B.vcRemark2,'') as vcRemark2,A.vcKBSerial,B.vcPhotoPath,B.vcPorType,A.iBaiJianFlag,ISNULL(A.vcPrintflagED,'') AS vcPrintflagED,ISNULL(A.vcDockED,'') AS vcDockED from ");
            strSQL.AppendLine("(select * from tKanbanPrintTbl) A");
            strSQL.AppendLine("left join ");
            strSQL.AppendLine("(select * from tPartInfoMaster) B");
            strSQL.AppendLine("on A.vcPartsNo=B.vcPartsNo and   A.vcDock=B.vcDock");
            strSQL.AppendLine("    WHERE (A.[vcKBorderno]+A.[vcKBSerial]+A.vcPartsNo+A.vcDock) in ('" + partfrom + "')");
            strSQL.AppendLine("    OR    (A.[vcKBorderno]+A.[vcKBSerial]+A.vcPrintflagED+A.vcDockED) in ('" + partfrom + "')");

            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }
        /// <summary>
        /// 看板打印信息ALL0170
        /// </summary>
        /// <param name="vcKBorderno"></param>
        /// <param name="vcKBSerial"></param>
        /// <returns></returns>
        public DataTable searchPrintKANBALL(string vcPartsNo, string vcDock, string vcKBorderno)//不必修改
        {
            string partfrom = "";
            StringBuilder strSQL = new StringBuilder();
            partfrom += vcKBorderno + vcPartsNo.ToString().Replace("-", "") + vcDock;
            strSQL.AppendLine("SELECT A.vcPartsNo  as vcPartsNo,");
            strSQL.AppendLine("       A.vcDock as vcDock,");
            strSQL.AppendLine("       B.vcSupplierCode,b.vcSupplierPlant, B.vcCpdCompany,A.vcCarType AS vcCarFamilyCode,A.vcKBorderno,a.vcEDflag,");
            strSQL.AppendLine("       B.vcPartsNameEN,B.vcPartsNameCHN,B.vcLogisticRoute, ");
            strSQL.AppendLine("       A.vcQuantityPerContainer as iQuantityPerContainer,");
            strSQL.AppendLine("       isnull(A.vcProject01,'') as vcProject01,isnull(A.vcComDate01,'') as vcComDate01,(case when A.vcBanZhi01='1' then '夜' when A.vcBanZhi01='0' then '白' else '' end) as vcBanZhi01,");
            strSQL.AppendLine("       isnull(A.vcProject02,'') as vcProject02,isnull(A.vcComDate02,'') as vcComDate02,(case when A.vcBanZhi02='1' then '夜' when A.vcBanZhi02='0' then '白' else '' end) as vcBanZhi02,");
            strSQL.AppendLine("       isnull(A.vcProject03,'') as vcProject03,isnull(A.vcComDate03,'') as vcComDate03,(case when A.vcBanZhi03='1' then '夜' when A.vcBanZhi03='0' then '白' else '' end) as vcBanZhi03,");
            strSQL.AppendLine("       isnull(A.vcProject04,'') as vcProject04,isnull(A.vcComDate04,'') as vcComDate04,(case when A.vcBanZhi04='1' then '夜' when A.vcBanZhi04='0' then '白' else '' end) as vcBanZhi04,");
            strSQL.AppendLine("       isnull(B.vcRemark1,'') as vcRemark1,isnull(B.vcRemark2,'') as vcRemark2,A.vcKBSerial,B.vcPhotoPath,B.vcPorType,A.iBaiJianFlag,ISNULL(A.vcPrintflagED,'') AS vcPrintflagED,ISNULL(A.vcDockED,'') AS vcDockED from ");
            strSQL.AppendLine("(select * from tKanbanPrintTbl) A");
            strSQL.AppendLine("left join ");
            strSQL.AppendLine("(select * from tPartInfoMaster) B");
            strSQL.AppendLine("on A.vcPartsNo=B.vcPartsNo and   A.vcDock=B.vcDock");
            strSQL.AppendLine("    WHERE (A.[vcKBorderno]+A.vcPartsNo+A.vcDock) in ('" + partfrom + "')");
            strSQL.AppendLine("    OR    (A.[vcKBorderno]+A.vcPrintflagED+A.vcDockED) in ('" + partfrom + "')");

            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }
        #region 获取表结构 为打印数据填充提供DataTable
        public DataTable searchTBCreate()//不必修改
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select top(1)* from testprinterCR");
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }
        #endregion
        /// <summary>
        /// CRExcep表结构
        /// </summary>
        /// <returns></returns>
        public DataTable searchTBCreateCRExcep()//不必修改
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select top(1)* from testprinterCRExcep");
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }
        /// <summary>
        /// 清空打印临时表
        /// </summary>
        /// <returns></returns>
        public bool TurnCate()
        {
            //清空临时表tPartInfoMaster_Temp
            SqlCommand cd = new SqlCommand();
            cd.Connection = new SqlConnection();
            cd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
            if (cd.Connection.State == ConnectionState.Closed)
            {
                cd.Connection.Open();
            }
            //
            cd.CommandText = "truncate table testprinterCR truncate table [testprinterCRMAIN] truncate table [testprinterCRExcep] truncate table testprinterExcel";
            cd.ExecuteNonQuery();
            if (cd.Connection.State == ConnectionState.Open)
            {
                cd.Connection.Close();
            }
            return true;
        }
        /// <summary>
        /// 获取照片等信息
        /// </summary>
        /// <returns></returns>
        public DataTable searchProRuleMst(string vcPartsNo, string vcDock)//不必修改
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("SELECT A.vcSupplierCode,A.vcSupplierPlant,A.vcCarFamilyCode,A.vcPartsNameEN,A.vcPartsNameCHN,A.vcLogisticRoute,A.iQuantityPerContainer,a.vcCpdCompany,");
            strSQL.AppendLine("       B. vcProName1,B.vcProName2,B.vcProName3,B.vcProName4,A.vcRemark1,A.vcRemark2,A.vcPhotoPath FROM (");
            strSQL.AppendLine("(select * from tPartInfoMaster) A");
            strSQL.AppendLine("left join ");
            strSQL.AppendLine("(select vcPorType,vcZB,vcProName1,vcProName2,vcProName3,vcProName4 from ProRuleMst) B");
            strSQL.AppendLine("on A.vcPorType=B.vcPorType AND A.vcZB=B.vcZB)");
            strSQL.AppendLine("WHERE A.vcPartsNo='" + vcPartsNo + "' and A.vcDock='" + vcDock + "'");
            strSQL.AppendLine("");
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }

        /// <summary>
        /// 插入连番表
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool insertTableKBSerial(DataTable dt)
        {
            using (SqlConnection connln = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                if (connln.State != ConnectionState.Open)
                    connln.Open();
                SqlTransaction trans = connln.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.Transaction = trans;
                try
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string strSqlIn = "";
                        string vcPartsNo = dt.Rows[i]["vcPartsNo"].ToString();
                        string vcKBSerial = dt.Rows[i]["vcKBSerial"].ToString();
                        string vcDock = dt.Rows[i]["vcDock"].ToString();
                        string vcKBorderno = dt.Rows[i]["vcKBorderno"].ToString();
                        string vcKBSerialBefore = dt.Rows[i]["vcKBSerialBefore"].ToString();
                        strSqlIn += "INSERT INTO [KBSerial_history]";
                        strSqlIn += "           ([vcPartsNo]";
                        strSqlIn += "           ,[vcDock]";
                        strSqlIn += "           ,[vcKBorderno]";
                        strSqlIn += "           ,[vcKBSerial]";
                        strSqlIn += "           ,[vcKBSerialBefore]";
                        strSqlIn += "           ,[dCreatTime])";
                        strSqlIn += "     VALUES";
                        strSqlIn += "           ('" + vcPartsNo + "'";
                        strSqlIn += "            ,'" + vcDock + "'";
                        strSqlIn += "            ,'" + vcKBorderno + "'";
                        strSqlIn += "            ,'" + vcKBSerial + "'";
                        strSqlIn += "            ,'" + vcKBSerialBefore + "'";
                        strSqlIn += "            ,getdate())";
                        cmd.CommandText = strSqlIn;
                        cmd.ExecuteNonQuery();
                    }
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }
    }
}
