/*******************************************************************
* 	项目名称			:	TPCS								   	
* 	模块名称			:	看板打印					
* 	创建者			    :	GAOLEI								
* 	创建日期			:	2020/09/4							
* 	类名			    :	FS1209_Logic					    
* 	修改者			    :						
* 	修改时间			:						
* 	修改内容			:											
* 					
* 	(C)2020-TJQM INFORMATION TECHNOLOGY CO.,LTD All Rights Reserved.
*******************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS1209_Logic
    {
        FS1209_DataAccess dataAccess = new FS1209_DataAccess();
        //PrinterCR print = new PrinterCR();

        public void BtnPrintAll(DataTable dt, string vctype, string printerName, string userId, ref DataTable dtPorType)
        {
            //DataTable dtPrintCR = new DataTable();//声明
            //DataTable dtPrint = new DataTable();
            //DataTable exdt = new DataTable();
            //PrinterCR print = new PrinterCR();
            //QMPrinter exprint = new QMPrinter();
            //DataTable dtHis = new DataTable();
            //DataTable dtPrintCRLone = print.searchTBCreate();//获得数据库表testprinterCR结构
            //string printIme = System.DateTime.Now.ToString("yyyy-MM-dd");
            //string tmplatePath = HttpContext.Current.Server.MapPath("~/Templates/FS1409.xlt");//看板投放确认单Excel模板
            //dtPrint = dtPrintCRLone.Clone();//克隆表结构
            //exdt = exprint.CreatDataTable();//创建ExcelDataTable，为打印看板确认单提供DataTable
            //dtHis = exprint.CreatDataTableHis();//创建连番DataTable
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    DataTable redt = print.searchPrintKANBALL(dt, vctype, i);
            //    string ls_fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + Guid.NewGuid().ToString().Replace("-", "") + ".png";
            //    String ls_savePath = HttpContext.Current.Server.MapPath("~/QRCodeImages/" + ls_fileName);
            //    string picnull = HttpContext.Current.Server.MapPath("~/images/picnull.JPG");
            //    byte[] vcPhotoPath = print.PhotoToArray(redt.Rows[0]["vcPhotoPath"].ToString(), picnull);//图片二进制流
            //    string reCode = print.reCode(redt.Rows[0]["vcSupplierCode"].ToString(), redt.Rows[0]["vcSupplierPlant"].ToString(), redt.Rows[0]["vcDock"].ToString(), redt.Rows[0]["vcPartsNo"].ToString(), redt.Rows[0]["iQuantityPerContainer"].ToString(), redt.Rows[0]["vcKBSerial"].ToString(), redt.Rows[0]["vcEDflag"].ToString(), redt.Rows[0]["vcKBorderno"].ToString());
            //    byte[] vcQRCodeImge = print.GenGenerateQRCode(reCode, ls_savePath);

            //    string gud;
            //    if (vctype == "3")
            //    {
            //        string QuantityPerContainerFED = resQuantityPerContainer(redt.Rows[0]["vcPartsNo"].ToString(), redt.Rows[0]["vcDock"].ToString(), dt.Rows[i]["vcPlanMonth"].ToString());//检查是收容数
            //        if (QuantityPerContainerFED != redt.Rows[0]["iQuantityPerContainer"].ToString())
            //        {
            //            int vcQuan = (Convert.ToInt32(QuantityPerContainerFED) > Convert.ToInt32(redt.Rows[0]["iQuantityPerContainer"].ToString())) ? (Convert.ToInt32(QuantityPerContainerFED) / Convert.ToInt32(redt.Rows[0]["iQuantityPerContainer"].ToString())) : (Convert.ToInt32(redt.Rows[0]["iQuantityPerContainer"].ToString()) / Convert.ToInt32(QuantityPerContainerFED));
            //            #region 数据库中的最大连番值
            //            string KBSerialend = resKBSerialend(redt.Rows[0]["vcKBorderno"].ToString());
            //            string KBSerialendTable = "";
            //            #endregion
            //            #region 正在操作的table表中最大的连番
            //            DataTable chk = dtPrint.Clone();
            //            DataRow[] rowKB = dtPrint.Select("vcorderno='" + redt.Rows[0]["vcKBorderno"].ToString() + "'");
            //            if (rowKB.Length != 0)
            //            {
            //                foreach (DataRow row in rowKB)
            //                {
            //                    chk.ImportRow(row);
            //                }
            //                DataView dv = chk.DefaultView;
            //                dv.Sort = "vcKBSerial desc";
            //                chk = dv.ToTable();
            //                KBSerialendTable = chk.Rows[0]["vcKBSerial"].ToString();
            //            }
            //            else
            //            {
            //                KBSerialendTable = "0000";
            //            }
            //            #endregion
            //            #region 整理数据 白件的收容数与对应黑件的收容数不相等
            //            for (int q = 1; q <= vcQuan; q++)
            //            {
            //                gud = Guid.NewGuid().ToString("N");
            //                string vcKBSerialend = Convert.ToString(Convert.ToInt32(Convert.ToInt32(KBSerialend) > Convert.ToInt32(KBSerialendTable) ? Convert.ToInt32(KBSerialend) : Convert.ToInt32(KBSerialendTable)) + 10000 + q).Substring(1, 4);

            //                DataRow row = dtPrint.NewRow();
            //                DataRow exrow = exdt.NewRow();
            //                DataRow hisrow = dtHis.NewRow();
            //                row[0] = redt.Rows[0]["vcSupplierCode"].ToString();
            //                row[1] = redt.Rows[0]["vcCpdCompany"].ToString();
            //                row[2] = redt.Rows[0]["vcCarFamilyCode"].ToString();
            //                row[3] = redt.Rows[0]["vcPartsNo"].ToString();
            //                row[4] = redt.Rows[0]["vcPartsNameEN"].ToString();
            //                row[5] = redt.Rows[0]["vcPartsNameCHN"].ToString();
            //                row[6] = redt.Rows[0]["vcLogisticRoute"].ToString();
            //                row[7] = (Convert.ToInt32(QuantityPerContainerFED) > Convert.ToInt32(redt.Rows[0]["iQuantityPerContainer"].ToString())) ? Convert.ToString((Convert.ToInt32(redt.Rows[0]["iQuantityPerContainer"].ToString()))) : Convert.ToString((Convert.ToInt32(QuantityPerContainerFED)));
            //                row[8] = redt.Rows[0]["vcProject01"].ToString();
            //                row[9] = redt.Rows[0]["vcComDate01"].ToString();
            //                row[10] = redt.Rows[0]["vcBanZhi01"].ToString();
            //                row[11] = redt.Rows[0]["vcProject02"].ToString();
            //                row[12] = redt.Rows[0]["vcComDate02"].ToString();
            //                row[13] = redt.Rows[0]["vcBanZhi02"].ToString();
            //                row[14] = redt.Rows[0]["vcProject03"].ToString();
            //                row[15] = redt.Rows[0]["vcComDate03"].ToString();
            //                row[16] = redt.Rows[0]["vcBanZhi03"].ToString();
            //                row[17] = redt.Rows[0]["vcProject04"].ToString();
            //                row[18] = redt.Rows[0]["vcComDate04"].ToString();
            //                row[19] = redt.Rows[0]["vcBanZhi04"].ToString();
            //                row[20] = redt.Rows[0]["vcRemark1"].ToString();
            //                row[21] = redt.Rows[0]["vcRemark2"].ToString();
            //                row[22] = vcKBSerialend;
            //                row[23] = vcPhotoPath;
            //                row[24] = vcQRCodeImge;
            //                row[25] = redt.Rows[0]["vcDock"].ToString();
            //                row[26] = redt.Rows[0]["vcKBorderno"].ToString() + redt.Rows[0]["vcDock"].ToString();
            //                row[27] = gud;//新增主键列
            //                row[28] = redt.Rows[0]["vcKBorderno"].ToString();
            //                row[29] = dt.Rows[i]["vcPorType"].ToString();
            //                row[30] = redt.Rows[0]["vcEDflag"].ToString() == "E" ? "紧急" : "";
            //                row[34] = dt.Rows[i]["vcPlanMonth"].ToString();
            //                row[35] = redt.Rows[0]["vcAB01"].ToString();//20180921添加AB值信息 - 李兴旺
            //                row[36] = redt.Rows[0]["vcAB02"].ToString();//20180921添加AB值信息 - 李兴旺
            //                row[37] = redt.Rows[0]["vcAB03"].ToString();//20180921添加AB值信息 - 李兴旺
            //                row[38] = redt.Rows[0]["vcAB04"].ToString();//20180921添加AB值信息 - 李兴旺
            //                #region 打印Excel看板投放确认单
            //                exrow[0] = row[3].ToString();
            //                exrow[1] = redt.Rows[0]["vcCarFamilyCode"].ToString();
            //                exrow[2] = redt.Rows[0]["vcPartsNameCHN"].ToString();
            //                exrow[3] = redt.Rows[0]["vcProject01"].ToString();
            //                exrow[4] = redt.Rows[0]["vcComDate01"].ToString();
            //                exrow[5] = redt.Rows[0]["vcBanZhi01"].ToString();
            //                exrow[6] = "1";//计数
            //                exrow[7] = dt.Rows[i]["vcPorType"].ToString();
            //                exrow[8] = redt.Rows[0]["vcKBorderno"].ToString();
            //                exrow[9] = redt.Rows[0]["vcKBSerial"].ToString();
            //                #endregion
            //                hisrow[0] = redt.Rows[0]["vcPartsNo"].ToString();
            //                hisrow[1] = redt.Rows[0]["vcDock"].ToString();
            //                hisrow[2] = redt.Rows[0]["vcKBorderno"].ToString();
            //                hisrow[3] = vcKBSerialend;
            //                hisrow[4] = redt.Rows[0]["vcKBSerial"].ToString();
            //                dtHis.Rows.Add(hisrow);
            //                dtPrint.Rows.Add(row);
            //                exdt.Rows.Add(exrow);
            //            }
            //            #endregion
            //        }
            //        else
            //        {
            //            #region 整理数据 白件的收容数与对应的黑件的收容数相等
            //            gud = Guid.NewGuid().ToString("N");
            //            DataRow row = dtPrint.NewRow();
            //            DataRow exrow = exdt.NewRow();
            //            row[0] = redt.Rows[0]["vcSupplierCode"].ToString();
            //            row[1] = redt.Rows[0]["vcCpdCompany"].ToString();
            //            row[2] = redt.Rows[0]["vcCarFamilyCode"].ToString();
            //            row[3] = redt.Rows[0]["vcPartsNo"].ToString();
            //            row[4] = redt.Rows[0]["vcPartsNameEN"].ToString();
            //            row[5] = redt.Rows[0]["vcPartsNameCHN"].ToString();
            //            row[6] = redt.Rows[0]["vcLogisticRoute"].ToString();
            //            row[7] = redt.Rows[0]["iQuantityPerContainer"].ToString();
            //            row[8] = redt.Rows[0]["vcProject01"].ToString();
            //            row[9] = redt.Rows[0]["vcComDate01"].ToString();
            //            row[10] = redt.Rows[0]["vcBanZhi01"].ToString();
            //            row[11] = redt.Rows[0]["vcProject02"].ToString();
            //            row[12] = redt.Rows[0]["vcComDate02"].ToString();
            //            row[13] = redt.Rows[0]["vcBanZhi02"].ToString();
            //            row[14] = redt.Rows[0]["vcProject03"].ToString();
            //            row[15] = redt.Rows[0]["vcComDate03"].ToString();
            //            row[16] = redt.Rows[0]["vcBanZhi03"].ToString();
            //            row[17] = redt.Rows[0]["vcProject04"].ToString();
            //            row[18] = redt.Rows[0]["vcComDate04"].ToString();
            //            row[19] = redt.Rows[0]["vcBanZhi04"].ToString();
            //            row[20] = redt.Rows[0]["vcRemark1"].ToString();
            //            row[21] = redt.Rows[0]["vcRemark2"].ToString();
            //            row[22] = redt.Rows[0]["vcKBSerial"].ToString();
            //            row[23] = vcPhotoPath;
            //            row[24] = vcQRCodeImge;
            //            row[25] = redt.Rows[0]["vcDock"].ToString();
            //            row[26] = redt.Rows[0]["vcKBorderno"].ToString() + redt.Rows[0]["vcDock"].ToString();
            //            row[27] = gud;//新增主键列
            //            row[28] = redt.Rows[0]["vcKBorderno"].ToString();
            //            row[29] = dt.Rows[i]["vcPorType"].ToString();
            //            row[30] = redt.Rows[0]["vcEDflag"].ToString() == "E" ? "紧急" : "";
            //            row[34] = dt.Rows[i]["vcPlanMonth"].ToString();
            //            row[35] = redt.Rows[0]["vcAB01"].ToString();//20180921添加AB值信息 - 李兴旺
            //            row[36] = redt.Rows[0]["vcAB02"].ToString();//20180921添加AB值信息 - 李兴旺
            //            row[37] = redt.Rows[0]["vcAB03"].ToString();//20180921添加AB值信息 - 李兴旺
            //            row[38] = redt.Rows[0]["vcAB04"].ToString();//20180921添加AB值信息 - 李兴旺
            //            #region 打印Excel看板投放确认单
            //            exrow[0] = row[3].ToString();
            //            exrow[1] = redt.Rows[0]["vcCarFamilyCode"].ToString();
            //            exrow[2] = redt.Rows[0]["vcPartsNameCHN"].ToString();
            //            exrow[3] = redt.Rows[0]["vcProject01"].ToString();
            //            exrow[4] = redt.Rows[0]["vcComDate01"].ToString();
            //            exrow[5] = redt.Rows[0]["vcBanZhi01"].ToString();
            //            exrow[6] = "1";//计数
            //            exrow[7] = dt.Rows[i]["vcPorType"].ToString();
            //            exrow[8] = redt.Rows[0]["vcKBorderno"].ToString();
            //            exrow[9] = redt.Rows[0]["vcKBSerial"].ToString();
            //            #endregion
            //            dtPrint.Rows.Add(row);
            //            exdt.Rows.Add(exrow);
            //            #endregion
            //        }
            //    }
            //    else
            //    {
            //        #region 整理数据 打印非秦丰、秦丰ED
            //        gud = Guid.NewGuid().ToString("N");
            //        DataRow row = dtPrint.NewRow();
            //        DataRow exrow = exdt.NewRow();
            //        row[0] = redt.Rows[0]["vcSupplierCode"].ToString();
            //        row[1] = redt.Rows[0]["vcCpdCompany"].ToString();
            //        row[2] = redt.Rows[0]["vcCarFamilyCode"].ToString();
            //        row[3] = redt.Rows[0]["vcPartsNo"].ToString();
            //        row[4] = redt.Rows[0]["vcPartsNameEN"].ToString();
            //        row[5] = redt.Rows[0]["vcPartsNameCHN"].ToString();
            //        row[6] = redt.Rows[0]["vcLogisticRoute"].ToString();
            //        row[7] = redt.Rows[0]["iQuantityPerContainer"].ToString();
            //        row[8] = redt.Rows[0]["vcProject01"].ToString();
            //        row[9] = redt.Rows[0]["vcComDate01"].ToString();
            //        row[10] = redt.Rows[0]["vcBanZhi01"].ToString();
            //        row[11] = redt.Rows[0]["vcProject02"].ToString();
            //        row[12] = redt.Rows[0]["vcComDate02"].ToString();
            //        row[13] = redt.Rows[0]["vcBanZhi02"].ToString();
            //        row[14] = redt.Rows[0]["vcProject03"].ToString();
            //        row[15] = redt.Rows[0]["vcComDate03"].ToString();
            //        row[16] = redt.Rows[0]["vcBanZhi03"].ToString();
            //        row[17] = redt.Rows[0]["vcProject04"].ToString();
            //        row[18] = redt.Rows[0]["vcComDate04"].ToString();
            //        row[19] = redt.Rows[0]["vcBanZhi04"].ToString();
            //        row[20] = redt.Rows[0]["vcRemark1"].ToString();
            //        row[21] = redt.Rows[0]["vcRemark2"].ToString();
            //        row[22] = redt.Rows[0]["vcKBSerial"].ToString();
            //        row[23] = vcPhotoPath;
            //        row[24] = vcQRCodeImge;
            //        row[25] = redt.Rows[0]["vcDock"].ToString();
            //        row[26] = redt.Rows[0]["vcKBorderno"].ToString() + redt.Rows[0]["vcDock"].ToString();
            //        row[27] = gud;//新增主键列
            //        row[28] = redt.Rows[0]["vcKBorderno"].ToString();
            //        row[29] = dt.Rows[i]["vcPorType"].ToString();
            //        row[30] = redt.Rows[0]["vcEDflag"].ToString() == "E" ? "紧急" : "";
            //        row[34] = dt.Rows[i]["vcPlanMonth"].ToString();
            //        row[35] = redt.Rows[0]["vcAB01"].ToString();//20180921添加AB值信息 - 李兴旺
            //        row[36] = redt.Rows[0]["vcAB02"].ToString();//20180921添加AB值信息 - 李兴旺
            //        row[37] = redt.Rows[0]["vcAB03"].ToString();//20180921添加AB值信息 - 李兴旺
            //        row[38] = redt.Rows[0]["vcAB04"].ToString();//20180921添加AB值信息 - 李兴旺
            //        #region 打印Excel看板投放确认单
            //        exrow[0] = row[3].ToString();
            //        exrow[1] = redt.Rows[0]["vcCarFamilyCode"].ToString();
            //        exrow[2] = redt.Rows[0]["vcPartsNameCHN"].ToString();
            //        exrow[3] = redt.Rows[0]["vcProject01"].ToString();
            //        exrow[4] = redt.Rows[0]["vcComDate01"].ToString();
            //        exrow[5] = redt.Rows[0]["vcBanZhi01"].ToString();
            //        exrow[6] = "1";//计数
            //        exrow[7] = dt.Rows[i]["vcPorType"].ToString();
            //        exrow[8] = redt.Rows[0]["vcKBorderno"].ToString();
            //        exrow[9] = redt.Rows[0]["vcKBSerial"].ToString();
            //        #endregion
            //        dtPrint.Rows.Add(row);
            //        exdt.Rows.Add(exrow);
            //        #endregion
            //    }
            //}
            //int qqqq = dtPrint.Rows.Count;
            //dtPrint = print.orderDataTable(dtPrint);//排序
            //exdt = print.orderDataTable(exdt);//排序
            //print.insertTableCR(dtPrint);//插入打印临时子表
            //print.insertTableExcel(exdt);//插入看板确认单Excel
            //print.insertTableKBSerial(dtHis);//插入连番记录表
            //dtPorType = QueryGroup(dtPrint);//用订单号 生产部署 生产日期 生产班值分组,修改不在数据库中取值vcorderno,vcPorType,vcComDate01,vcBanZhi01

            //print.insertTableCRMain(dtPrint, dtPorType);//插入打印临时主表
            //string printDay = KanBIfPrintDay();//获取班值信息
            //string reportPath = HttpContext.Current.Server.MapPath("~/CrReport.rpt");
            //string strLoginId = userId;

            //for (int z = 0; z < dtPorType.Rows.Count; z++)
            //{
            //    //DataTable exdtt = exdt.Clone();
            //    DataTable exdttt = new DataTable();
            //    DataTable exdthj = new DataTable();
            //    //exdttt.Clear();
            //    string vcPorType = dtPorType.Rows[z]["vcPorType"].ToString();
            //    string vcorderno = dtPorType.Rows[z]["vcorderno"].ToString();
            //    string vcComDate01 = dtPorType.Rows[z]["vcComDate01"].ToString();
            //    string vcBanZhi01 = dtPorType.Rows[z]["vcBanZhi01"].ToString();

            //    bool retb = print.printCr(reportPath, vcPorType, vcorderno, vcComDate01, vcBanZhi01, "", "", strLoginId, printerName);//打印水晶报表

            //    #region 打印看板确认单
            //    if (vctype != "3")//打印秦风非ED的数据不需要看板确认单
            //    {
            //        //数据库取出Excel的数据进行打印
            //        DataSet ds = PrintExcel(vcPorType, vcorderno, vcComDate01, vcBanZhi01);
            //        exdttt = ds.Tables[0];
            //        exdthj = ds.Tables[1];
            //        for (int p = 0; p < exdttt.Rows.Count; p++)
            //        {
            //            exdttt.Rows[p]["no"] = p + 1;
            //        }
            //        int dsRowsCount = exdttt.Rows.Count;
            //        int dsRow = dsRowsCount / 43;
            //        int dsrows = dsRowsCount % 43;
            //        //总页数
            //        int pagetotle = 0;
            //        //页数
            //        int pageno = 0;
            //        if (dsRow != 0)
            //        {
            //            pagetotle = ((dsrows + exdthj.Rows.Count + 3) / 43) == 0 ? (dsRow + 1) : (dsRow + 1 + (((exdthj.Rows.Count + 3) / 43) == 0 ? 1 : ((exdthj.Rows.Count + 3) / 43) + 1));
            //        }
            //        else
            //        {
            //            pagetotle = ((dsRowsCount + exdthj.Rows.Count + 3) / 43) == 0 ? 1 : (1 + (((exdthj.Rows.Count + 3) / 43) == 0 ? 1 : ((exdthj.Rows.Count + 3) / 43) + 1));
            //        }
            //        if (dsRow > 0)
            //        {
            //            DataTable inTable = exdttt.Clone();
            //            for (int i = 0; i < exdttt.Rows.Count; i++)
            //            {
            //                DataRow dr = exdttt.Rows[i];
            //                DataRow add = inTable.NewRow();
            //                add.ItemArray = dr.ItemArray;
            //                inTable.Rows.Add(add);
            //                if (inTable.Rows.Count >= 43 || exdttt.Rows.Count - 1 == i)
            //                {
            //                    pageno = pageno + 1;
            //                    string pageB = "0";
            //                    if (inTable.Rows.Count < 43)
            //                    {
            //                        pageB = inTable.Rows.Count + exdthj.Rows.Count + 3 <= 43 ? "1" : "2";
            //                    }
            //                    //打印传值有：订单号、生产部署、计划打印日期 printIme、计划打印班值 printDay 、计划生产日期 、计划生产班值、页码
            //                    exprint.PrintTemplateFromDataTable(inTable, exdthj, tmplatePath, vcorderno, vcPorType, strLoginId, printIme, printDay, vcComDate01, vcBanZhi01 == "白" ? "白值" : "夜值", printerName, Convert.ToString(pagetotle), Convert.ToString(pageno), pageB);
            //                    DataTable checkorderno = new DataTable();
            //                    checkorderno = check(vcorderno, vcPorType);
            //                    int rows;
            //                    rows = checkorderno.Rows.Count;
            //                    //向tKanBanQrTbl表中插入数据
            //                    if (rows == 0)
            //                    {
            //                        //将testprinterExcel表中数据存入到testprinterExcel1中
            //                        InsertInto(vcorderno, vcPorType);
            //                        InsertDate(vcorderno, vcPorType, printIme, printDay == "白值" ? "0" : "1", vcComDate01, vcBanZhi01 == "白" ? "0" : "1");
            //                    }
            //                    inTable = exdt.Clone();
            //                }
            //            }
            //        }
            //        else
            //        {
            //            string pageB = "0";
            //            pageno = pageno + 1;
            //            pageB = exdttt.Rows.Count + exdthj.Rows.Count + 3 <= 43 ? "1" : "2";
            //            //打印传值有：订单号、生产部署、计划打印日期 printIme、计划打印班值 printDay 、计划生产日期 、计划生产班值、页码
            //            exprint.PrintTemplateFromDataTable(exdttt, exdthj, tmplatePath, vcorderno, vcPorType, strLoginId, printIme, printDay, vcComDate01, vcBanZhi01 == "白" ? "白值" : "夜值", printerName, Convert.ToString(pagetotle), Convert.ToString(pageno), pageB);

            //            DataTable checkorderno = new DataTable();
            //            //判断是否存在重复单号
            //            checkorderno = check(vcorderno, vcPorType);
            //            int rows;
            //            rows = checkorderno.Rows.Count;

            //            //向tKanBanQrTbl表中插入数据
            //            if (rows == 0)
            //            {
            //                //将testprinterExcel表中数据存入到testprinterExcel1中
            //                InsertInto(vcorderno, vcPorType);
            //                InsertDate(vcorderno, vcPorType, printIme, printDay == "白值" ? "0" : "1", vcComDate01, vcBanZhi01 == "白" ? "0" : "1");
            //            }
            //        }
            //    }
            //    #endregion
            //    //删除看板打印的临时文件
            //    DeleteprinterCREX(vcPorType, vcorderno, vcComDate01, vcBanZhi01);
            //}
            //print.UpdatePrintKANB(dtPrint, vctype);
            //if (vctype == "3")
            //{
            //    InsertPrint(dtPrint);
            //}
        }

        #region DataTable分组
        public DataTable QueryGroup(DataTable dt)
        {
            int a = dt.Rows.Count;
            DataTable dtPorType = new DataTable("dtPorType");
            DataColumn dc1 = new DataColumn("vcorderno", Type.GetType("System.String"));
            DataColumn dc2 = new DataColumn("vcPorType", Type.GetType("System.String"));
            DataColumn dc3 = new DataColumn("vcComDate01", Type.GetType("System.String"));
            DataColumn dc4 = new DataColumn("vcBanZhi01", Type.GetType("System.String"));
            dtPorType.Columns.Add(dc1);
            dtPorType.Columns.Add(dc2);
            dtPorType.Columns.Add(dc3);
            dtPorType.Columns.Add(dc4);
            //var query = from t in dt.AsEnumerable()
            //            group t by new { t1 = t.Field<string>("vcorderno"), t2 = t.Field<string>("vcPorType"), t3 = t.Field<string>("vcComDate01"), t4 = t.Field<string>("vcBanZhi01") } into m
            //            select new
            //            {
            //                vcorderno = m.Key.t1,
            //                vcPorType = m.Key.t2,
            //                vcComDate01 = m.Key.t3,
            //                vcBanZhi01 = m.Key.t4
            //            };
            //foreach (var item in query.ToList())
            //{
            //    DataRow dr = dtPorType.NewRow();
            //    dr["vcorderno"] = item.vcorderno;
            //    dr["vcPorType"] = item.vcPorType;
            //    dr["vcComDate01"] = item.vcComDate01; ;
            //    dr["vcBanZhi01"] = item.vcBanZhi01;
            //    dtPorType.Rows.Add(dr);
            //}
            return dtPorType;
        }
        #endregion

        #region 检索信息栏绑定生产部署 str2是权限部署
        public DataTable dllPorType(string userId, ref string[] userPorType)
        {
            string RolePorType;
            if (userId.Equals("admin"))
            {
                RolePorType = "admin";
            }
            else
            {
                RolePorType = getRoleTip(userId);
            }
            userPorType = RolePorType.Split('*');
            return dataAccess.dllPorType(userPorType);
        }
        #endregion

        #region 绑定工场 1 2 3 厂
        public DataTable dllPorPlant()
        {
            return dataAccess.dllPorPlant();
        }
        #endregion

        #region 确认当值是否有未打印的数据 适用于看板打印页面和看板再发行页面
        public bool KanBIfPrint(DataTable dtflag)
        {
            return dataAccess.KanBIfPrint(dtflag);
        }
        #endregion

        #region 获取所属打印机的名称
        public string PrintMess(string userid)
        {
            return dataAccess.PrintMess(userid);
        }
        #endregion

        #region 检索看板打印信息 检索的是非秦丰和秦丰ED的看板数据
        public DataTable searchPrint(string vcPrintPartNo, string vcType, string vcKbOrderId, string vcLianFan, string vcPorType, string vcPlant, DataTable dtflag)
        {
            return dataAccess.searchPrint(vcPrintPartNo, vcType, vcKbOrderId, vcLianFan, vcPorType, vcPlant, dtflag);
        }
        #endregion

        #region 检索打印数据 检索的是秦丰非ED的看板信息 检索流程：从补给系统获取已入库白件的品番、受入、订单号、连番，到看板打印表中检索相应的秦丰非ED看板信息
        public DataTable searchPrint(string vcPrintPartNo, string vcKbOrderId, string vcLianFan, string vcPorType, string vcPlant, DataTable dtflag)
        {
            return dataAccess.searchPrint(vcPrintPartNo, vcKbOrderId, vcLianFan, vcPorType, vcPlant, dtflag);
        }
        #endregion

        /// <summary>
        ///检索已入库白件的黑件看板品番
        /// </summary>
        /// <param name="PartNo"></param>
        /// <param name="vcDock"></param>
        /// <returns></returns>
        public string searchMasterED(string PartNo, string vcDock)
        {
            return dataAccess.searchMasterED(PartNo, vcDock);
        }

        public bool InsertPrint(DataTable dt)
        {
            return dataAccess.InsertPrint(dt);
        }

        public DataTable reDataMaster(DataTable dtol)
        {
            return dataAccess.reDataMaster(dtol);
        }

        public string dtKBSerialUP1(string vcPartsNo, string vcDock, string vcKBorderno, string vcKBSerialBefore)
        {
            return dataAccess.dtKBSerialUP1(vcPartsNo, vcDock, vcKBorderno, vcKBSerialBefore);
        }

        public string KanBIfPrintDay()
        {
            return dataAccess.KanBIfPrintDay();
        }

        public DataSet PrintExcel(string vcPorType, string vcorderno, string vcComDate01, string vcBanZhi01)
        {
            return dataAccess.PrintExcel(vcPorType, vcorderno, vcComDate01, vcBanZhi01);
        }

        public string resQuantityPerContainer(string vcpartno, string vcdock, string vcPlanMonth)
        {
            return dataAccess.resQuantityPerContainer(vcpartno, vcdock, vcPlanMonth);
        }

        public string resKBSerialend(string vcKBorderno)
        {
            return dataAccess.resKBSerialend(vcKBorderno);
        }

        public DataTable ceshi()
        {
            return dataAccess.ceshi();
        }

        public void DeleteprinterCREX(string vcPorType, string vcorderno, string vcComDate01, string vcBanZhi01)
        {
            dataAccess.DeleteprinterCREX(vcPorType, vcorderno, vcComDate01, vcBanZhi01);
        }

        public void InsertInto(string vcorderno, string vcPorType)
        {
            dataAccess.InsertInto(vcorderno, vcPorType);
        }

        public void DeleteprinterCREX1(string vcPorType, string vcorderno, string vcComDate01, string vcBanZhi01)
        {
            dataAccess.DeleteprinterCREX1(vcPorType, vcorderno, vcComDate01, vcBanZhi01);
        }

        //特殊打印删除
        public void DeleteprinterCREX2(string vcPorType, string vcorderno, string vcComDate01, string vcBanZhi01)
        {
            dataAccess.DeleteprinterCREX2(vcPorType, vcorderno, vcComDate01, vcBanZhi01);
        }

        public void InsertDate(string vcorderno, string vcPorType, string printIme, string printDay, string vcComDate01, string vcBanZhi01)
        {
            dataAccess.InsertDate(vcorderno, vcPorType, printIme, printDay, vcComDate01, vcBanZhi01);
        }

        public DataTable check(string vcorderno, string vcPorType)
        {
            return dataAccess.check(vcorderno, vcPorType);
        }

        public string getRoleTip(string vcUserId)
        {
            return dataAccess.getRoleTip(vcUserId);
        }
    }
}